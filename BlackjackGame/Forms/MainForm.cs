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

        // -------------------- Form Load --------------------
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

        // -------------------- MenuStrip Handlers --------------------
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

        private void aboutToolStripMenuItem_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }

        private void aboutToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        // -------------------- Button Handlers --------------------
        private void btnHit_Click(object sender, EventArgs e)
        {
            DealCard(playerHand, pnlPlayerArea);

            if (playerHand.GetValue() > 21)
            {
                try { defeatSound?.Play(); } catch { }

                EndGame("Player busts! Dealer wins.");
            }
        }

        private void btnStand_Click(object sender, EventArgs e)
        {
            RevealDealerHiddenCard();

            while (dealerHand.GetValue() < 17)
            {
                DealCard(dealerHand, pnlDealerArea);

                btnHit.Enabled = false;
                btnStand.Enabled = false;

                Application.DoEvents();
                System.Threading.Thread.Sleep(500);
            }

            int playerValue = playerHand.GetValue();
            int dealerValue = dealerHand.GetValue();

            btnHit.Enabled = true;
            btnStand.Enabled = true;

            string result = dealerValue > 21 ? "Dealer busts! You win!" : dealerValue > playerValue ? "Dealer wins!" : dealerValue < playerValue ? "You win!" : "Push! It's a tie!";

            EndGame(result);
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

        // -------------------- Game Setup and Logic --------------------
        private async void StartNewGame()
        {
            dealerHiddenRevealed = false;

            deck = new Deck();

            playerHand = new Hand();
            dealerHand = new Hand();

            ClearPanels(pnlPlayerArea);
            ClearPanels(pnlDealerArea);

            if (hiddenCardPictureBox != null)
            {
                this.Controls.Remove(hiddenCardPictureBox);
                hiddenCardPictureBox.Dispose();
                hiddenCardPictureBox = null;
            }

            lblStatus.Text = "Welcome to BlackJack. Good Luck!";
            lblPlayerValue.Text = "Player: 0";
            lblDealerValue.Text = "Dealer: ?";

            btnHit.Visible = true;
            btnStand.Visible = true;

            btnStand.Enabled = true;
            btnHit.Enabled = true;

            btnPlayAgain.Visible = false;

            SetupColors();

            await DealCard(playerHand, pnlPlayerArea);
            await DealCard(dealerHand, pnlDealerArea);
            await DealCard(playerHand, pnlPlayerArea);
            await DealCard(dealerHand, pnlDealerArea, isFaceDown: true);

            UpdateHandValueLabels();
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

                if (targetPanel == pnlDealerArea && isFaceDown && hiddenCardPictureBox == null)
                {
                    hiddenCardPictureBox = pb;
                }
            }

            try { dealCardSound?.Play(); } catch { }

            await Task.Delay(150);

            UpdateHandValueLabels();
        }

        private void RevealDealerHiddenCard()
        {
            if (!dealerHiddenRevealed && hiddenCardPictureBox != null)
            {
                dealerHiddenRevealed = true;

                if (dealerHand.Cards.Count > 1)
                {
                    try
                    {
                        var secondCard = dealerHand.Cards[1];

                        Application.DoEvents();
                        hiddenCardPictureBox.Image = GetCardImage(secondCard);
                        hiddenCardPictureBox.Invalidate();
                        hiddenCardPictureBox.Refresh();
                    }

                    catch
                    {
                        MessageBox.Show($"You're clicking too fast! The playing card images did not have time to load, please wait a full second before hitting or standing.", "Slow Down!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
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

        private void UpdateHandValueLabels()
        {
            int currentPlayerValue = playerHand.GetValue();
            int currentDealerValue = dealerHand.GetValue();

            int.TryParse(new string(lblPlayerValue.Text.Where(char.IsDigit).ToArray()), out int previousPlayerValue);
            int.TryParse(new string(lblDealerValue.Text.Where(char.IsDigit).ToArray()), out int previousDealerValue);

            int visibleDealerValue = dealerHiddenRevealed ? currentDealerValue : (dealerHand.Cards.Count > 0 ? dealerHand.Cards[0].Value : 0);

            AnimateChipStack(lblPlayerValue, previousPlayerValue, currentPlayerValue, "Player");
            AnimateChipStack(lblDealerValue, previousDealerValue, visibleDealerValue, "Dealer");
        }

        private void AnimateChipStack(Label lbl, int fromValue, int toValue, string prefix)
        {
            int displayedValue = fromValue;

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

        private void SetupColors()
        {
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

        private Image GetCardImage(Card card)
        {
            try
            {
                if (isDarkTheme)
                {
                    var img = (Image)Properties.Resources.ResourceManager.GetObject(card.ID + "_Dark");

                    if (img == null)
                    {
                        img = Properties.Resources.Back_Dark;
                    }

                    return img;
                }

                else
                {
                    var img = (Image)Properties.Resources.ResourceManager.GetObject(card.ID);

                    if (img == null)
                    {
                        img = Properties.Resources.Back;
                    }

                    return img;
                }
            }

            catch
            {
                return Properties.Resources.Back;
            }
        }

        private void HoverCardEffect(PictureBox pb, bool isHovered)
        {
            if (isHovered)
            {
                pb.Size = new Size(117, 160);
                pb.Location = new Point(pb.Location.X - 3, pb.Location.Y - 3);
            }

            else
            {
                pb.Size = new Size(107, 150);
                pb.Location = new Point(pb.Location.X + 3, pb.Location.Y + 3);
            }
        }

        // -------------------- Ride the Bus Game --------------------
        private async void StartRideTheBusGame()
        {
            btnStand.Visible = false;
            btnHit.Visible = false;
            btnPlayAgain.Visible = false;

            deck = new Deck();

            rideBusScore = 0;
            rideBusStage = 1;

            firstCard = null;
            secondCard = null;
            thirdCard = null;

            ClearPanels(pnlDealerArea);
            ClearPanels(pnlPlayerArea);

            lblStatus.Text = "Ride the Bus - Stage 1: Red or Black?";
            lblPlayerValue.Text = "";
            lblDealerValue.Text = "";
            rtbScoreLabel.Text = "Score: 0";

            ShowPrompt("Red", "Black", OnRedOrBlackChosen);
        }

        private void ShowPrompt(string option1, string option2, Action<string> callback)
        {
            pnlPlayerArea.Controls.Clear();

            Button btn1 = new Button
            {
                Text = option1,
                Size = new Size(100, 40),
                Location = new Point(365, 0),
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
                Location = new Point(485, 0),
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

            AddScoreLabel();
        }

        private void ShowPromptSuitOptions()
        {
            foreach (var b in pnlPlayerArea.Controls.OfType<Button>().ToList())
            {
                b.Dispose();
            }

            AddScoreLabel();

            string[] suits = { "Spades", "Diamonds", "Clubs", "Hearts" };

            int btnW = 100;
            int btnH = 40;
            int spacing = 10;
            int totalW = suits.Length * btnW + (suits.Length - 1) * spacing;

            int startX = (pnlPlayerArea.Width - totalW) / 2;
            int y = 0;

            int x = startX;

            foreach (var suit in suits)
            {
                Button btn = new Button
                {
                    Text = suit,
                    Size = new Size(btnW, btnH),
                    Location = new Point(x, y),
                    BackColor = Color.DarkSlateGray,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Tag = suit,
                    Cursor = Cursors.Hand
                };

                btn.Click += (s, e) => OnSuitChosen(suit);
                pnlPlayerArea.Controls.Add(btn);

                x += btnW + spacing;
            }
        }

        private void OnRedOrBlackChosen(string choice)
        {
            firstCard = deck.DrawCard();
            DisplayCard(firstCard, pnlDealerArea);

            bool isRed = firstCard.Suit == Suit.Hearts || firstCard.Suit == Suit.Diamonds;
            bool correct = (choice == "Red" && isRed) || (choice == "Black" && !isRed);

            HandleRideStageResult(correct, 100, "Stage 2: Higher or Lower?", () => ShowPrompt("Higher", "Lower", OnHigherLowerChosen));
        }

        private void OnHigherLowerChosen(string choice)
        {
            secondCard = deck.DrawCard();
            DisplayCard(secondCard, pnlDealerArea);

            bool isEqual = secondCard.Value == firstCard.Value;
            bool correct = (choice == "Higher" && secondCard.Value > firstCard.Value) || (choice == "Lower" && secondCard.Value < firstCard.Value) || (choice == "Higher" && isEqual) || (choice == "Lower" && isEqual);

            HandleRideStageResult(correct, 200, "Stage 3: Inside or Outside?", () => ShowPrompt("Inside", "Outside", OnInsideOutsideChosen));
        }

        private void OnInsideOutsideChosen(string choice)
        {
            thirdCard = deck.DrawCard();
            DisplayCard(thirdCard, pnlDealerArea);

            int min = Math.Min(firstCard.Value, secondCard.Value);
            int max = Math.Max(firstCard.Value, secondCard.Value);

            bool isInside = thirdCard.Value > min && thirdCard.Value < max;
            bool isOutside = thirdCard.Value < min || thirdCard.Value > max;
            bool isEqual = thirdCard.Value == min || thirdCard.Value == max;

            bool correct = (choice == "Inside" && isInside) || (choice == "Outside" && isOutside) || (choice == "Inside" && isEqual);

            HandleRideStageResult(correct, 300, "Stage 4: Guess the Suit     ", () => ShowPromptSuitOptions());
        }

        private void OnSuitChosen(string guess)
        {
            Card finalCard = deck.DrawCard();
            DisplayCard(finalCard, pnlDealerArea);

            bool correct = finalCard.Suit.ToString() == guess;
            rideBusScore += correct ? 600 : 0;

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

            rtbScoreLabel.Text = $"Score: {rideBusScore}";
            btnPlayAgain.Visible = true;
        }

        private void HandleRideStageResult(bool correct, int points, string nextText, Action nextStage)
        {
            if (correct)
            {
                rideBusScore += points;
                rtbScoreLabel.Text = $"Score: {rideBusScore}";
                lblStatus.Text = nextText;

                DisableButtons();

                Task.Delay(800).ContinueWith(_ => Invoke(nextStage));
                try { dealCardSound?.Play(); } catch { }
            }

            else
            {
                lblStatus.Text = "Incorrect! You lose your progress.";
                try { defeatSound?.Play(); } catch { }
                rtbScoreLabel.Text = $"Score: {rideBusScore}";

                DisableButtons();
                btnPlayAgain.Visible = true;
            }
        }

        private void DisplayCard(Card card, Panel target)
        {
            PictureBox pb = new PictureBox
            {
                Size = new Size(107, 150),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = GetCardImage(card),
                BackColor = Color.Transparent,
                Left = 240 + target.Controls.Count * 120,
                Top = 10,
                Cursor = Cursors.Hand,
            };

            pb.MouseEnter += (s, e) => HoverCardEffect(pb, true);
            pb.MouseLeave += (s, e) => HoverCardEffect(pb, false);

            target.Controls.Add(pb);
        }

        private void DisableButtons()
        {
            foreach (Control c in pnlPlayerArea.Controls)
            {
                if (c is Button)
                {
                    c.Enabled = false;
                }
            }
        }

        private void AddScoreLabel()
        {
            if (rtbScoreLabel == null || rtbScoreLabel.IsDisposed)
            {
                rtbScoreLabel = new Label
                {
                    AutoSize = true,
                    Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                    ForeColor = Color.LightGreen,
                    Location = new Point(425, 55),
                    Text = "Score: 0"
                };
            }

            if (!pnlPlayerArea.Controls.Contains(rtbScoreLabel))
            {
                pnlPlayerArea.Controls.Add(rtbScoreLabel);
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

        // -------------------- Helpers --------------------
        private void UncheckOtherToolStripMenuItems(ToolStripMenuItem selectedMenuItem)
        {
            selectedMenuItem.Checked = true;

            foreach (var toolStripMenuItem in from object item in selectedMenuItem.Owner.Items let toolStripMenuItem = item as ToolStripMenuItem where toolStripMenuItem != null where !item.Equals(selectedMenuItem) select toolStripMenuItem)
            {
                toolStripMenuItem.Checked = false;
            }
        }

        private void madeByLabel_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo { FileName = "https://youtube.com/@FutureFlash", UseShellExecute = true });
        }

        // -------------------- Easter Egg/Test Mode --------------------
        private void hiddenLabel1_Click(object sender, EventArgs e)
        {
            easterEggCount++;
        }

        private void hiddenLabel1_DoubleClick(object sender, EventArgs e)
        {
            easterEggCount = 0;
            StartNewGame();
        }

        private void hiddenLabel2_Click(object sender, EventArgs e)
        {
            easterEggCount++;
        }

        private void hiddenLabel3_Click(object sender, EventArgs e)
        {
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
    }
}
