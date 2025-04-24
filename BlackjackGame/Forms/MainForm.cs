using BlackjackGame.Helpers;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using BlackjackGame;

namespace BlackjackGame.Forms
{
    public partial class MainForm : Form
    {
        private GameMode currentMode = GameMode.Blackjack;

        private bool isDarkTheme = false;
        private bool dealerHiddenRevealed = false;

        private int easterEggCount = 0;

        private Deck deck;
        private Card firstCard, secondCard, thirdCard;

        private Hand dealerHand;
        private Hand playerHand;

        private int rideBusScore = 0;
        private int rideBusStage = 0;

        private PictureBox hiddenCardPictureBox;

        private SoundPlayer dealCardSound = new SoundPlayer(Properties.Resources.CardFlip);
        private SoundPlayer defeatSound = new SoundPlayer(Properties.Resources.Defeat);
        private SoundPlayer tieSound = new SoundPlayer(Properties.Resources.Tie);
        private SoundPlayer victorySound = new SoundPlayer(Properties.Resources.Victory);

        public MainForm()
        {
            InitializeComponent();

            // Enable Dark Mode for title bar ONLY. Read "Dark Title Bar" code region from above to learn more
            UseImmersiveDarkMode(this.Handle, true);

            StartNewGame();
        }

        private enum GameMode { Blackjack, RideTheBus }
        private bool InRideBusMode => currentMode == GameMode.RideTheBus;

        #region Dark Title Bar

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 = 19;

        // Code below was copied from https://stackoverflow.com/questions/57124243/winforms-dark-title-bar-on-windows-10 . Thank you Jonas Kohl <3
        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
        private static bool IsWindows10OrGreater(int build = -1)
        {
            return Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= build;
        }

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
        #endregion Dark Title Bar

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

        private void btnPlayAgain_Click(object sender, EventArgs e)
        {
            if (currentMode == GameMode.Blackjack)
            {
                StartNewGame();
            }

            else
            {
                StartRideTheBusGame();
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

        private void darkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isDarkTheme = true;

            UncheckOtherToolStripMenuItems((ToolStripMenuItem)sender);

            if (currentMode == GameMode.Blackjack)
            {
                StartNewGame();
            }

            else
            {
                StartRideTheBusGame();
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

        private void defaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isDarkTheme = false;
            UncheckOtherToolStripMenuItems((ToolStripMenuItem)sender);

            if (currentMode == GameMode.Blackjack)
            {
                StartNewGame();
            }

            else
            {
                StartRideTheBusGame();
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

        private void hiddenLabel1_Click(object sender, EventArgs e)
        {
            // // hmmmm.... [part 1]
            easterEggCount++;
        }

        private void hiddenLabel1_DoubleClick(object sender, EventArgs e)
        {
            easterEggCount = 0;
            StartNewGame();
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

        private void madeByLabel_Click(object sender, EventArgs e)
        {
            // Regular 'Process.Start("URL")' doesn't work in newer DotNet Core versions. Extra code is needed
            Process.Start(new ProcessStartInfo { FileName = "https://youtube.com/@FutureFlash", UseShellExecute = true });
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Custom renderer preventing default Windows behavior form changing ToolStripMenuItem hover color. This is used for the 'About' button
            blackjackMenuStrip.Renderer = new NoHighlightRenderer();

            defaultToolStripMenuItem.Checked = true;
            blackjackToolStripMenuItem.Checked = true;

            // Got tired of changing each button's cursor to a hand
            foreach (Control c in this.Controls)
            {
                if (c is Button)
                {
                    c.Cursor = Cursors.Hand;
                }
            }
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

        private async void StartNewGame()
        {
            // Reset hidden card reveal state
            dealerHiddenRevealed = false;

            // Reset game logic
            deck = new Deck();

            playerHand = new Hand();
            dealerHand = new Hand();

            // Dispose all PictureBoxes manually
            ClearPanels(pnlPlayerArea);
            ClearPanels(pnlDealerArea);

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

        private void ShowPrompt(string option1, string option2, Action<string> callback)
        {
            pnlPlayerArea.Controls.Clear();

            Button btn1 = new Button
            {
                Text = option1,
                Size = new Size(100, 40),
                Location = new Point(10, 10),
                BackColor = Color.Maroon,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            Button btn2 = new Button
            {
                Text = option2,
                Size = new Size(100, 40),
                Location = new Point(120, 10),
                BackColor = Color.FromArgb(15, 15, 15),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btn1.Click += (s, e) => callback(option1);
            btn2.Click += (s, e) => callback(option2);

            pnlPlayerArea.Controls.Add(btn1);
            pnlPlayerArea.Controls.Add(btn2);

            if (isDarkTheme)
            {
                btn1.BackColor = Color.FromArgb(30, 30, 30);
                btn2.BackColor = Color.FromArgb(30, 30, 30);
            }

            else
            {
                btn1.BackColor = Color.Maroon;
                btn2.BackColor = Color.FromArgb(15, 15, 15);
            }
        }

        private void DisableButtons()
        {
            foreach (Button btn in pnlPlayerArea.Controls)
            {
                btn.Enabled = false;
            }
        }

        private void ClearPanels(Panel pnl)
        {
            foreach (Control ctrl in pnl.Controls)
            {
                ctrl.Dispose();
            }

            pnl.Controls.Clear();
        }

        private async void StartRideTheBusGame()
        {
            // Hide all Blackjack controls
            btnStand.Visible = false;
            btnHit.Visible = false;
            btnPlayAgain.Visible = false;

            // Initialize a fresh deck
            deck = new Deck();

            // Reset Ride the Bus state
            rideBusScore = 0;
            rideBusStage = 1;

            firstCard = null;
            secondCard = null;
            thirdCard = null;

            // Clear UI panels
            ClearPanels(pnlDealerArea);
            ClearPanels(pnlPlayerArea);

            // Set up Stage 1
            lblStatus.Text = "Ride the Bus - Stage 1: Red or Black?";
            lblPlayerValue.Text = "Score: 0";
            lblDealerValue.Text = "";

            ShowPrompt("Red", "Black", OnRedOrBlackChosen);
        }

        private void OnRedOrBlackChosen(string choice)
        {
            // Draw and display the first card
            firstCard = deck.DrawCard();
            DisplayCard(firstCard, pnlDealerArea);

            // Determine if the guess was correct
            bool isRed = firstCard.Suit == Suit.Hearts || firstCard.Suit == Suit.Diamonds;
            bool correct = (choice == "Red" && isRed) || (choice == "Black" && !isRed);

            // Handle outcome and transition to Stage 2
            HandleRideStageResult(correct, 100, "Stage 2: Higher or Lower?", () => ShowPrompt("Higher", "Lower", OnHigherLowerChosen));
        }

        private void OnHigherLowerChosen(string choice)
        {
            // Draw and display the second card
            secondCard = deck.DrawCard();
            DisplayCard(secondCard, pnlDealerArea);

            // Determine result based on value comparison
            bool isEqual = secondCard.Value == firstCard.Value;
            bool correct = (choice == "Higher" && secondCard.Value > firstCard.Value) || (choice == "Lower" && secondCard.Value < firstCard.Value) || (choice == "Higher" && isEqual) || (choice == "Lower" && isEqual);

            // Continue to Stage 3
            HandleRideStageResult(correct, 200, "Stage 3: Inside or Outside?", () => ShowPrompt("Inside", "Outside", OnInsideOutsideChosen));
        }

        private void OnInsideOutsideChosen(string choice)
        {
            // Draw and show the third card
            thirdCard = deck.DrawCard();
            DisplayCard(thirdCard, pnlDealerArea);

            // Calculate card range from previous two
            int min = Math.Min(firstCard.Value, secondCard.Value);
            int max = Math.Max(firstCard.Value, secondCard.Value);

            bool isInside = thirdCard.Value > min && thirdCard.Value < max;
            bool isOutside = thirdCard.Value < min || thirdCard.Value > max;
            bool isEqual = thirdCard.Value == min || thirdCard.Value == max;

            // Accept same value card as being inside
            bool correct = (choice == "Inside" && isInside) || (choice == "Outside" && isOutside) || (choice == "Inside" && isEqual);

            // Advance to final stage
            HandleRideStageResult(correct, 300, "Stage 4: Guess the Suit", () => ShowPromptSuitOptions());
        }

        private void ShowPromptSuitOptions()
        {
            // Clear any remaining controls
            pnlPlayerArea.Controls.Clear();

            // Define suit options
            string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
            int x = 10;

            // Create a button for each suit
            foreach (var suit in suits)
            {
                Button btn = new Button
                {
                    Text = suit,
                    Size = new Size(100, 40),
                    Location = new Point(x, 10),
                    BackColor = Color.DarkSlateGray,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Tag = suit,
                    Cursor = Cursors.Hand
                };

                btn.Click += (s, e) => OnSuitChosen(suit);
                pnlPlayerArea.Controls.Add(btn);
                x += 110;

                // Adjust styling for theme
                if (isDarkTheme)
                {
                    btn.BackColor = Color.FromArgb(30, 30, 30);
                }

                else
                {
                    btn.BackColor = Color.DarkSlateGray;
                }
            }
        }

        private void OnSuitChosen(string guess)
        {
            // Draw and display final card
            Card finalCard = deck.DrawCard();
            DisplayCard(finalCard, pnlDealerArea);

            // Evaluate guess
            bool correct = finalCard.Suit.ToString() == guess;
            rideBusScore += correct ? 600 : 0;

            // Provide feedback
            if (correct)
            {
                lblStatus.Text = "Incredible! You guessed the suit!";
                try { victorySound?.Play(); } catch { }
                DisableButtons();
            }

            else
            {
                lblStatus.Text = "Wrong suit, but nice try!";
                try { defeatSound?.Play(); } catch { }
                DisableButtons();
            }

            // Show score and replay option
            lblPlayerValue.Text = $"Score: {rideBusScore}";
            btnPlayAgain.Visible = true;
        }

        private void HandleRideStageResult(bool correct, int points, string nextText, Action nextStage)
        {
            if (correct)
            {
                // Add points and continue
                rideBusScore += points;

                lblPlayerValue.Text = $"Score: {rideBusScore}";
                lblStatus.Text = nextText;

                DisableButtons();

                Task.Delay(800).ContinueWith(_ => Invoke(nextStage));
                try { dealCardSound?.Play(); } catch { }
            }

            else
            {
                // End game on failure
                lblStatus.Text = "Incorrect! You lose your progress.";
                try { defeatSound?.Play(); } catch { }
                lblPlayerValue.Text = $"Score: {rideBusScore}";

                DisableButtons();
                btnPlayAgain.Visible = true;
            }
        }

        private void DisplayCard(Card card, Panel target)
        {
            // Create and show a PictureBox for a card
            PictureBox pb = new PictureBox
            {
                Size = new Size(107, 150),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = GetCardImage(card),
                BackColor = Color.Transparent,
                Left = target.Controls.Count * 120,
                Top = 10,
                Cursor = Cursors.Hand,
            };

            pb.MouseEnter += (s, e) => HoverCardEffect(pb, true);
            pb.MouseLeave += (s, e) => HoverCardEffect(pb, false);

            target.Controls.Add(pb);
        }

        private void BlackjackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UncheckOtherToolStripMenuItems((ToolStripMenuItem)sender);

            ClearPanels(pnlPlayerArea);
            ClearPanels(pnlDealerArea);

            currentMode = GameMode.Blackjack;
            StartNewGame();
        }

        private void RideTheBusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UncheckOtherToolStripMenuItems((ToolStripMenuItem)sender);

            currentMode = GameMode.RideTheBus;
            StartRideTheBusGame();
        }

        // I know these are extra, but I just want the button to feel nice
        private void aboutToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void aboutToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }
    }
}