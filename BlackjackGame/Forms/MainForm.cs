using BlackjackGame.Helpers;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlackjackGame.Forms
{
    public partial class MainForm : Form
    {
        private Deck deck;

        private Hand playerHand;
        private Hand dealerHand;

        private bool dealerHiddenRevealed = false;

        private PictureBox hiddenCardPictureBox;

        private SoundPlayer dealCardSound = new SoundPlayer(Properties.Resources.CardFlip);
        private SoundPlayer victorySound = new SoundPlayer(Properties.Resources.Victory);
        private SoundPlayer defeatSound = new SoundPlayer(Properties.Resources.Defeat);
        private SoundPlayer tieSound = new SoundPlayer(Properties.Resources.Tie);

        #region Make Window Title Bar Dark

        // Code below was copied from https://stackoverflow.com/questions/57124243/winforms-dark-title-bar-on-windows-10 . Thank you Jonas Kohl <3
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        private static bool UseImmersiveDarkMode(IntPtr handle, bool enabled)
        {
            if (IsWindows10OrGreater(17763))
            {
                var attribute = DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1;
                if (IsWindows10OrGreater(18985))
                {
                    attribute = DWMWA_USE_IMMERSIVE_DARK_MODE;
                }

                int useImmersiveDarkMode = enabled ? 1 : 0;
                return DwmSetWindowAttribute(handle, (int)attribute, ref useImmersiveDarkMode, sizeof(int)) == 0;
            }

            return false;
        }

        private static bool IsWindows10OrGreater(int build = -1)
        {
            return Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= build;
        }

        #endregion Make Window Title Bar Dark

        public MainForm()
        {
            InitializeComponent();
            UseImmersiveDarkMode(this.Handle, true); // Enable Dark Mode for title bar ONLY
            StartNewGame();
        }

        private async void StartNewGame()
        {
            // Reset hidden card reveal state
            dealerHiddenRevealed = false;

            // Reset game logic
            deck = new Deck();
            playerHand = new Hand();
            dealerHand = new Hand();

            // Clear player and dealer areas
            pnlPlayerArea.Controls.Clear();
            pnlDealerArea.Controls.Clear();

            // Remove hidden dealer card PictureBox if it exists
            if (hiddenCardPictureBox != null)
            {
                this.Controls.Remove(hiddenCardPictureBox);
                hiddenCardPictureBox.Dispose();
                hiddenCardPictureBox = null;
            }

            // Reset UI elements
            lblStatus.Text = "Welcome to BlackJack. Good Luck!";
            lblPlayerValue.Text = "Player: 0";
            lblDealerValue.Text = "Dealer: ?";

            btnHit.Visible = true;
            btnStand.Visible = true;

            btnStand.Enabled = true;
            btnHit.Enabled = true;

            btnPlayAgain.Visible = false;

            // Deal player card 1
            await DealCard(playerHand, pnlPlayerArea);

            // Deal dealer card 1 (face up)
            await DealCard(dealerHand, pnlDealerArea);

            // Deal player card 2
            await DealCard(playerHand, pnlPlayerArea);

            // Deal dealer card 2 (face down)
            await DealCard(dealerHand, pnlDealerArea, isFaceDown: true);

            // Update value labels
            UpdateHandValueLabels();
        }

        private async Task DealCard(Hand hand, Panel targetPanel, bool isFaceDown = false)
        {
            Card card = deck.DrawCard();
            hand.AddCard(card);

            PictureBox pb = new PictureBox
            {
                Size = new Size(125, 150),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = isFaceDown ? Properties.Resources.Back : GetCardImage(card),
                BackColor = Color.Transparent,
                Left = (hand.Cards.Count - 1) * 120,
                Top = 10,
                Cursor = Cursors.Hand
            };

            pb.MouseEnter += (s, e) => HoverCardEffect(pb, true);
            pb.MouseLeave += (s, e) => HoverCardEffect(pb, false);

            targetPanel.Controls.Add(pb);

            // Save reference to the hidden dealer card if applicable
            if (targetPanel == pnlDealerArea && isFaceDown && hiddenCardPictureBox == null)
            {
                hiddenCardPictureBox = pb;
            }

            try { dealCardSound?.Play(); } catch { }

            await Task.Delay(150); // Smooth dealing feel
            UpdateHandValueLabels();
        }

        private void HoverCardEffect(PictureBox pb, bool isHovered)
        {
            if (isHovered)
            {
                pb.Size = new Size(130, 155); // Make the card appear slightly bigger
                pb.Location = new Point(pb.Location.X - 5, pb.Location.Y - 5); // Slight lift
            }
            else
            {
                pb.Size = new Size(125, 150); // Return to original size
                pb.Location = new Point(pb.Location.X + 5, pb.Location.Y + 5); // Return to original position
            }
        }

        private Image GetCardImage(Card card)
        {
            try
            {
                Debug.WriteLine($"Attempting to load image for card: {card.ID}");
                var img = (Image)Properties.Resources.ResourceManager.GetObject(card.ID);

                if (img == null)
                {
                    Debug.WriteLine($"Image for card {card.ID} not found! Using fallback.");
                    img = Properties.Resources.Back; // Fallback image if not found
                }

                return img;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading image for card {card.ID}: {ex.Message}");
                return Properties.Resources.Back; // Fallback
            }
        }

        private void btnHit_Click(object sender, EventArgs e)
        {
            DealCard(playerHand, pnlPlayerArea); // Ideally, this function should be awaited. However, you can't 'await' a void function
            if (playerHand.GetValue() > 21)
            {
                try { defeatSound?.Play(); } catch { } // I made all 'try-catch' functions a one-liner for better formatting

                EndGame("Player busts! Dealer wins.");
            }
        }

        private void btnStand_Click(object sender, EventArgs e)
        {
            btnHit.Enabled = false;
            btnStand.Enabled = false;

            RevealDealerHiddenCard();

            while (dealerHand.GetValue() < 17)
            {
                DealCard(dealerHand, pnlDealerArea); // As mentioned before, you can't 'await' a void function
                Application.DoEvents(); // Ensure UI is caught up
                System.Threading.Thread.Sleep(500);
            }

            int playerValue = playerHand.GetValue();
            int dealerValue = dealerHand.GetValue();

            string result = dealerValue > 21 ? "Dealer busts! You win!" : dealerValue > playerValue ? "Dealer wins!" : dealerValue < playerValue ? "You win!" : "Push! It's a tie!";

            EndGame(result);
        }

        private void RevealDealerHiddenCard()
        {
            if (!dealerHiddenRevealed && hiddenCardPictureBox != null)
            {
                dealerHiddenRevealed = true;

                if (dealerHand.Cards.Count > 1)
                {
                    var secondCard = dealerHand.Cards[1];
                    Debug.WriteLine("Revealing hidden dealer card: " + secondCard.ID);

                    Application.DoEvents(); // Ensure UI is caught up
                    hiddenCardPictureBox.Image = GetCardImage(secondCard);
                    hiddenCardPictureBox.Invalidate();
                    hiddenCardPictureBox.Refresh();
                }

                UpdateHandValueLabels();
            }
        }

        private void EndGame(string message)
        {
            lblStatus.Text = message;
            btnPlayAgain.Visible = true;
            btnHit.Visible = false;
            btnStand.Visible = false;

            RevealDealerHiddenCard();

            if (message.Contains("You win!"))
            {
                try { victorySound?.Play(); } catch { }
            }

            else if (message.Contains("Push!"))
            {
                try { tieSound?.Play(); } catch { }
            }

            else
            {
                try { defeatSound?.Play(); } catch { }
            }
        }

        private void btnPlayAgain_Click(object sender, EventArgs e)
        {
            StartNewGame();
        }

        private void UpdateHandValueLabels()
        {
            int currentPlayerValue = playerHand.GetValue();
            int currentDealerValue = dealerHand.GetValue();

            // Parse the previous numeric values from the labels (default to 0 if parsing fails)
            int.TryParse(new string(lblPlayerValue.Text.Where(char.IsDigit).ToArray()), out int previousPlayerValue);
            int.TryParse(new string(lblDealerValue.Text.Where(char.IsDigit).ToArray()), out int previousDealerValue);

            // Determine what the dealer's visible value should be
            int visibleDealerValue = dealerHiddenRevealed ? currentDealerValue : (dealerHand.Cards.Count > 0 ? dealerHand.Cards[0].Value : 0);

            // Animate both values from previous to current
            AnimateChipStack(lblPlayerValue, previousPlayerValue, currentPlayerValue, "Player");
            AnimateChipStack(lblDealerValue, previousDealerValue, visibleDealerValue, "Dealer");
        }

        private void AnimateChipStack(Label lbl, int fromValue, int toValue, string prefix)
        {
            Timer chipTimer = new Timer { Interval = 15 }; // Adjust the animation speed
            int displayedValue = fromValue;

            chipTimer.Tick += (s, e) =>
            {
                if (displayedValue < toValue)
                {
                    displayedValue++;
                    lbl.Text = $"{prefix}: {displayedValue}";
                }
                else if (displayedValue > toValue)
                {
                    displayedValue--;
                    lbl.Text = $"{prefix}: {displayedValue}";
                }
                else
                {
                    chipTimer.Stop();
                    lbl.Text = $"{prefix}: {toValue}";
                }
            };

            chipTimer.Start();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            blackjackMenuStrip.Renderer = new NoHighlightRenderer(); // Custom renderer preventing default Windows behavior form changing ToolStripMenuItem hover color. This is used for the 'About' button

            foreach (Control c in this.Controls) // Got tired of changing each button's properties to change the cursor to a hand when hovered over
            {
                if (c is Button)
                {
                    c.Cursor = Cursors.Hand;
                }
            }
        }

        private void madeByLabel_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = "https://youtube.com/@FutureFlash", UseShellExecute = true }); // Extra stuff for DotNet Core
        }

        private void aboutToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void aboutToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form AboutForm = new AboutForm(versionLabel.Text);

            if (AboutForm.IsDisposed)
            {
                AboutForm = new AboutForm(versionLabel.Text);
                SystemSounds.Beep.Play();
                AboutForm.ShowDialog();
            }
            else
            {
                SystemSounds.Beep.Play();
                AboutForm.ShowDialog();
            }
        }
    }
}