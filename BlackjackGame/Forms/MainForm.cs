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
        private int easterEggCount = 0;
        private bool isDarkTheme = false;

        private Hand playerHand;
        private Hand dealerHand;

        private bool dealerHiddenRevealed = false;

        private PictureBox hiddenCardPictureBox;

        private SoundPlayer dealCardSound = new SoundPlayer(Properties.Resources.CardFlip);
        private SoundPlayer victorySound = new SoundPlayer(Properties.Resources.Victory);
        private SoundPlayer defeatSound = new SoundPlayer(Properties.Resources.Defeat);
        private SoundPlayer tieSound = new SoundPlayer(Properties.Resources.Tie);

        #region Dark Title Bar

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

            // Enable Dark Mode for title bar ONLY. Read "Dark Title Bar" code region from above to learn more
            UseImmersiveDarkMode(this.Handle, true);

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

            // Change UI colors accordingly
            SetupColors();

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

        private void SetupColors()
        {
            // This sets up the UI colors to match the current theme
            if (isDarkTheme)
            {
                btnHit.BackColor = Color.FromArgb(20, 20, 20);
                btnStand.BackColor = Color.FromArgb(20, 20, 20);
                btnPlayAgain.BackColor = Color.FromArgb(20, 20, 20);

                btnHit.FlatAppearance.BorderColor = Color.WhiteSmoke;
                btnStand.FlatAppearance.BorderColor = Color.WhiteSmoke;
                btnPlayAgain.FlatAppearance.BorderColor = Color.WhiteSmoke;

                btnHit.FlatAppearance.BorderSize = 1;
                btnStand.FlatAppearance.BorderSize = 1;
                btnPlayAgain.FlatAppearance.BorderSize = 1;

                lblStatus.ForeColor = Color.FromArgb(150, 150, 150);

                lblPlayerValue.ForeColor = Color.FromArgb(225, 225, 225);
                lblDealerValue.ForeColor = Color.FromArgb(225, 225, 225);
            }

            else
            {
                btnHit.BackColor = Color.FromArgb(0, 120, 215);
                btnStand.BackColor = Color.FromArgb(192, 64, 0);
                btnPlayAgain.BackColor = Color.SeaGreen;

                btnHit.FlatAppearance.BorderSize = 0;
                btnStand.FlatAppearance.BorderSize = 0;
                btnPlayAgain.FlatAppearance.BorderSize = 0;

                lblStatus.ForeColor = Color.Gold;

                lblPlayerValue.ForeColor = Color.LightGreen;
                lblDealerValue.ForeColor = Color.LightSkyBlue;
            }
        }

        private async Task DealCard(Hand hand, Panel targetPanel, bool isFaceDown = false)
        {
            if (isDarkTheme)
            {
                Card card = deck.DrawCard();
                hand.AddCard(card);

                PictureBox pb = new PictureBox
                {
                    Size = new Size(107, 150),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Image = isFaceDown ? Properties.Resources.Back_Dark : GetCardImage(card),
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
            }

            else
            {
                Card card = deck.DrawCard();
                hand.AddCard(card);

                PictureBox pb = new PictureBox
                {
                    Size = new Size(107, 150),
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
            }

            // I made all 'try-catch' functions a one-liner for better formatting
            try { dealCardSound?.Play(); } catch { }

            // Smooth dealing feel
            await Task.Delay(150);

            UpdateHandValueLabels();
        }

        private void HoverCardEffect(PictureBox pb, bool isHovered)
        {
            if (isHovered)
            {
                // Make the card appear slightly bigger
                pb.Size = new Size(117, 160);
                
                // Slight lift
                pb.Location = new Point(pb.Location.X - 3, pb.Location.Y - 3);
            }

            else
            {
                // Return to original size
                pb.Size = new Size(107, 150);
                
                // Return to original position
                pb.Location = new Point(pb.Location.X + 3, pb.Location.Y + 3);
            }
        }

        private Image GetCardImage(Card card)
        {
            try
            {
                if (isDarkTheme)
                {

                    var img = (Image)Properties.Resources.ResourceManager.GetObject(card.ID + "_Dark");

                    if (img == null)
                    {
                        // Fallback image if not found
                        img = Properties.Resources.Back_Dark;
                    }

                    return img;
                }

                else
                {
                    var img = (Image)Properties.Resources.ResourceManager.GetObject(card.ID);

                    if (img == null)
                    {
                        // Fallback image if not found
                        img = Properties.Resources.Back;
                    }

                    return img;
                }
            }

            catch (Exception ex)
            {
                // Fallback
                return Properties.Resources.Back;
            }
        }

        private void btnHit_Click(object sender, EventArgs e)
        {
            // Ideally, this function should be awaited. However, you can't 'await' a void function
            DealCard(playerHand, pnlPlayerArea);

            if (playerHand.GetValue() > 21)
            {
                // Another one-liner for formatting
                try { defeatSound?.Play(); } catch { }

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
                // As mentioned before, you can't 'await' a void function
                DealCard(dealerHand, pnlDealerArea);

                // Ensure UI is caught up
                Application.DoEvents();
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

                    // Ensure UI is caught up
                    Application.DoEvents();
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
                // Even more one-liners. You gotta love 'em
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
            int displayedValue = fromValue;

            // Adjust the animation speed
            Timer chipTimer = new Timer { Interval = 15 };

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
            // Custom renderer preventing default Windows behavior form changing ToolStripMenuItem hover color. This is used for the 'About' button
            blackjackMenuStrip.Renderer = new NoHighlightRenderer();
            defaultToolStripMenuItem.Checked = true;

            // Got tired of changing each button's cursor to a hand
            foreach (Control c in this.Controls)
            {
                if (c is Button)
                {
                    c.Cursor = Cursors.Hand;
                }
            }
        }

        private void madeByLabel_Click(object sender, EventArgs e)
        {
            // Regular 'Process.Start("URL")' doesn't work in newer DotNet Core versions. Extra code is needed
            Process.Start(new ProcessStartInfo { FileName = "https://youtube.com/@FutureFlash", UseShellExecute = true });
        }

        // I know this is a extra, but I focus on tiny details to make the UI responsive and look as nice as I can
        private void aboutToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void aboutToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void themeToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void themeToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form AboutForm = new AboutForm(versionLabel.Text, isDarkTheme);

            if (AboutForm.IsDisposed)
            {
                AboutForm = new AboutForm(versionLabel.Text, isDarkTheme);
                SystemSounds.Beep.Play();
                AboutForm.ShowDialog();
            }

            else
            {
                SystemSounds.Beep.Play();
                AboutForm.ShowDialog();
            }
        }

        private void hiddenLabel1_Click(object sender, EventArgs e)
        {
            // // hmmmm.... [part 1]
            easterEggCount++;
        }

        private void hiddenLabel2_Click(object sender, EventArgs e)
        {
            // hmmmm.... [part 2]
            easterEggCount++;
        }

        private void hiddenLabel3_Click(object sender, EventArgs e)
        {
            // hmmmm.... [part 3]
            easterEggCount++;

            if (easterEggCount == 3)
            {
                Form EasterEggForm = new EasterEggForm();

                if (EasterEggForm.IsDisposed)
                {
                    EasterEggForm = new EasterEggForm();
                    SystemSounds.Beep.Play();
                    EasterEggForm.Show();
                }

                else
                {
                    SystemSounds.Beep.Play();
                    EasterEggForm.Show();
                }
            }
        }

        private void hiddenLabel1_DoubleClick(object sender, EventArgs e)
        {
            easterEggCount = 0;
            StartNewGame();
        }

        // Once again, I got this from StackOverflow. Link: https://stackoverflow.com/questions/13603654/check-only-one-toolstripmenuitem . Huge thanks to Julio Borges <3
        // This method ensures that only one theme is selected at a time. Of course, Windows Forms doesn't support this by default, which is why I had to use StackOverflow for help
        private void UncheckOtherToolStripMenuItems(ToolStripMenuItem selectedMenuItem)
        {
            selectedMenuItem.Checked = true;

            foreach (var toolStripMenuItem in from object item in selectedMenuItem.Owner.Items let toolStripMenuItem = item as ToolStripMenuItem where toolStripMenuItem != null where !item.Equals(selectedMenuItem) select toolStripMenuItem)
            {
                toolStripMenuItem.Checked = false;
            }
        }

        private void defaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isDarkTheme = false;

            UncheckOtherToolStripMenuItems((ToolStripMenuItem)sender);
            StartNewGame();
        }

        private void darkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isDarkTheme = true;

            UncheckOtherToolStripMenuItems((ToolStripMenuItem)sender);
            StartNewGame();
        }
    }
}