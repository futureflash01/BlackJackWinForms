using System.Drawing;
using System.Windows.Forms;

namespace BlackjackGame.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnHit;
        private System.Windows.Forms.Button btnStand;
        private System.Windows.Forms.Label lblPlayerValue;
        private System.Windows.Forms.Label lblDealerValue;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnPlayAgain;
        private System.Windows.Forms.Panel pnlPlayerArea;
        private System.Windows.Forms.Panel pnlDealerArea;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            btnHit = new Button();
            btnStand = new Button();
            btnPlayAgain = new Button();
            lblPlayerValue = new Label();
            lblDealerValue = new Label();
            lblStatus = new Label();
            pnlPlayerArea = new Panel();
            pnlDealerArea = new Panel();
            versionLabel = new Label();
            madeByLabel = new Label();
            blackjackMenuStrip = new MenuStrip();
            gameToolStripMenuItem = new ToolStripMenuItem();
            blackjackToolStripMenuItem = new ToolStripMenuItem();
            rideTheBusToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            themeToolStripMenuItem = new ToolStripMenuItem();
            changingThemeWillRestartGameToolStripMenuItem = new ToolStripMenuItem();
            defaultToolStripMenuItem = new ToolStripMenuItem();
            darkToolStripMenuItem = new ToolStripMenuItem();
            hiddenLabel1 = new Label();
            hiddenLabel2 = new Label();
            hiddenLabel3 = new Label();
            blackjackMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // btnHit
            // 
            btnHit.BackColor = Color.FromArgb(0, 120, 215);
            btnHit.FlatAppearance.BorderSize = 0;
            btnHit.FlatStyle = FlatStyle.Flat;
            btnHit.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnHit.ForeColor = Color.White;
            btnHit.Location = new Point(368, 267);
            btnHit.Name = "btnHit";
            btnHit.Size = new Size(120, 50);
            btnHit.TabIndex = 3;
            btnHit.Text = "Hit";
            btnHit.UseVisualStyleBackColor = false;
            btnHit.Click += btnHit_Click;
            // 
            // btnStand
            // 
            btnStand.BackColor = Color.FromArgb(192, 64, 0);
            btnStand.FlatAppearance.BorderSize = 0;
            btnStand.FlatStyle = FlatStyle.Flat;
            btnStand.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnStand.ForeColor = Color.White;
            btnStand.Location = new Point(508, 267);
            btnStand.Name = "btnStand";
            btnStand.Size = new Size(120, 50);
            btnStand.TabIndex = 4;
            btnStand.Text = "Stand";
            btnStand.UseVisualStyleBackColor = false;
            btnStand.Click += btnStand_Click;
            // 
            // btnPlayAgain
            // 
            btnPlayAgain.BackColor = Color.SeaGreen;
            btnPlayAgain.FlatAppearance.BorderSize = 0;
            btnPlayAgain.FlatStyle = FlatStyle.Flat;
            btnPlayAgain.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnPlayAgain.ForeColor = Color.White;
            btnPlayAgain.Location = new Point(420, 267);
            btnPlayAgain.Name = "btnPlayAgain";
            btnPlayAgain.Size = new Size(160, 50);
            btnPlayAgain.TabIndex = 5;
            btnPlayAgain.Text = "Play Again";
            btnPlayAgain.UseVisualStyleBackColor = false;
            btnPlayAgain.Visible = false;
            btnPlayAgain.Click += btnPlayAgain_Click;
            // 
            // lblPlayerValue
            // 
            lblPlayerValue.AutoSize = true;
            lblPlayerValue.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblPlayerValue.ForeColor = Color.LightGreen;
            lblPlayerValue.Location = new Point(23, 302);
            lblPlayerValue.Name = "lblPlayerValue";
            lblPlayerValue.Size = new Size(89, 25);
            lblPlayerValue.TabIndex = 0;
            lblPlayerValue.Text = "Player: 0";
            // 
            // lblDealerValue
            // 
            lblDealerValue.AutoSize = true;
            lblDealerValue.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblDealerValue.ForeColor = Color.LightSkyBlue;
            lblDealerValue.Location = new Point(24, 32);
            lblDealerValue.Name = "lblDealerValue";
            lblDealerValue.Size = new Size(88, 25);
            lblDealerValue.TabIndex = 1;
            lblDealerValue.Text = "Dealer: ?";
            // 
            // lblStatus
            // 
            lblStatus.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblStatus.ForeColor = Color.Gold;
            lblStatus.Location = new Point(12, 230);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(976, 30);
            lblStatus.TabIndex = 2;
            lblStatus.Text = "Welcome to BlackJack!";
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlPlayerArea
            // 
            pnlPlayerArea.BackColor = Color.Transparent;
            pnlPlayerArea.Location = new Point(23, 330);
            pnlPlayerArea.Name = "pnlPlayerArea";
            pnlPlayerArea.Size = new Size(965, 170);
            pnlPlayerArea.TabIndex = 6;
            // 
            // pnlDealerArea
            // 
            pnlDealerArea.BackColor = Color.Transparent;
            pnlDealerArea.Location = new Point(23, 60);
            pnlDealerArea.Name = "pnlDealerArea";
            pnlDealerArea.Size = new Size(965, 170);
            pnlDealerArea.TabIndex = 7;
            // 
            // versionLabel
            // 
            versionLabel.AutoSize = true;
            versionLabel.ForeColor = SystemColors.ControlDark;
            versionLabel.Location = new Point(966, 527);
            versionLabel.Name = "versionLabel";
            versionLabel.Size = new Size(28, 15);
            versionLabel.TabIndex = 8;
            versionLabel.Text = "v1.3";
            // 
            // madeByLabel
            // 
            madeByLabel.AutoSize = true;
            madeByLabel.Cursor = Cursors.Hand;
            madeByLabel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Point, 0);
            madeByLabel.ForeColor = Color.IndianRed;
            madeByLabel.Location = new Point(846, 526);
            madeByLabel.Name = "madeByLabel";
            madeByLabel.Size = new Size(117, 15);
            madeByLabel.TabIndex = 9;
            madeByLabel.Text = "Made by FutureFlash";
            madeByLabel.Click += madeByLabel_Click;
            // 
            // blackjackMenuStrip
            // 
            blackjackMenuStrip.BackColor = Color.FromArgb(35, 35, 35);
            blackjackMenuStrip.Items.AddRange(new ToolStripItem[] { aboutToolStripMenuItem, gameToolStripMenuItem, themeToolStripMenuItem });
            blackjackMenuStrip.Location = new Point(0, 0);
            blackjackMenuStrip.Name = "blackjackMenuStrip";
            blackjackMenuStrip.Size = new Size(1000, 25);
            blackjackMenuStrip.TabIndex = 10;
            blackjackMenuStrip.Text = "menuStrip1";
            // 
            // gameToolStripMenuItem
            // 
            gameToolStripMenuItem.BackColor = Color.FromArgb(45, 45, 45);
            gameToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { blackjackToolStripMenuItem, rideTheBusToolStripMenuItem });
            gameToolStripMenuItem.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            gameToolStripMenuItem.ForeColor = Color.WhiteSmoke;
            gameToolStripMenuItem.Name = "gameToolStripMenuItem";
            gameToolStripMenuItem.Size = new Size(94, 21);
            gameToolStripMenuItem.Text = "Game Mode";
            // 
            // blackjackToolStripMenuItem
            // 
            blackjackToolStripMenuItem.BackColor = Color.FromArgb(80, 80, 80);
            blackjackToolStripMenuItem.ForeColor = Color.WhiteSmoke;
            blackjackToolStripMenuItem.Name = "blackjackToolStripMenuItem";
            blackjackToolStripMenuItem.Size = new Size(180, 22);
            blackjackToolStripMenuItem.Text = "Blackjack";
            blackjackToolStripMenuItem.Click += BlackjackToolStripMenuItem_Click;
            // 
            // rideTheBusToolStripMenuItem
            // 
            rideTheBusToolStripMenuItem.BackColor = Color.FromArgb(80, 80, 80);
            rideTheBusToolStripMenuItem.ForeColor = Color.WhiteSmoke;
            rideTheBusToolStripMenuItem.Name = "rideTheBusToolStripMenuItem";
            rideTheBusToolStripMenuItem.Size = new Size(180, 22);
            rideTheBusToolStripMenuItem.Text = "Ride the Bus";
            rideTheBusToolStripMenuItem.Click += RideTheBusToolStripMenuItem_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.BackColor = Color.FromArgb(55, 55, 55);
            aboutToolStripMenuItem.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            aboutToolStripMenuItem.ForeColor = Color.WhiteSmoke;
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(58, 21);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            aboutToolStripMenuItem.MouseEnter += aboutToolStripMenuItem_MouseEnter;
            aboutToolStripMenuItem.MouseLeave += aboutToolStripMenuItem_MouseLeave;
            // 
            // themeToolStripMenuItem
            // 
            themeToolStripMenuItem.BackColor = Color.FromArgb(80, 80, 80);
            themeToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { changingThemeWillRestartGameToolStripMenuItem, defaultToolStripMenuItem, darkToolStripMenuItem });
            themeToolStripMenuItem.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            themeToolStripMenuItem.ForeColor = Color.WhiteSmoke;
            themeToolStripMenuItem.Name = "themeToolStripMenuItem";
            themeToolStripMenuItem.Size = new Size(62, 21);
            themeToolStripMenuItem.Text = "Theme";
            // 
            // changingThemeWillRestartGameToolStripMenuItem
            // 
            changingThemeWillRestartGameToolStripMenuItem.BackColor = Color.FromArgb(80, 80, 80);
            changingThemeWillRestartGameToolStripMenuItem.Enabled = false;
            changingThemeWillRestartGameToolStripMenuItem.ForeColor = Color.WhiteSmoke;
            changingThemeWillRestartGameToolStripMenuItem.Name = "changingThemeWillRestartGameToolStripMenuItem";
            changingThemeWillRestartGameToolStripMenuItem.Size = new Size(290, 22);
            changingThemeWillRestartGameToolStripMenuItem.Text = "Changing theme will restart game!";
            // 
            // defaultToolStripMenuItem
            // 
            defaultToolStripMenuItem.BackColor = Color.FromArgb(80, 80, 80);
            defaultToolStripMenuItem.ForeColor = Color.WhiteSmoke;
            defaultToolStripMenuItem.Name = "defaultToolStripMenuItem";
            defaultToolStripMenuItem.Size = new Size(290, 22);
            defaultToolStripMenuItem.Text = "Default";
            defaultToolStripMenuItem.Click += defaultToolStripMenuItem_Click;
            // 
            // darkToolStripMenuItem
            // 
            darkToolStripMenuItem.BackColor = Color.FromArgb(80, 80, 80);
            darkToolStripMenuItem.ForeColor = Color.WhiteSmoke;
            darkToolStripMenuItem.Name = "darkToolStripMenuItem";
            darkToolStripMenuItem.Size = new Size(290, 22);
            darkToolStripMenuItem.Text = "Dark";
            darkToolStripMenuItem.Click += darkToolStripMenuItem_Click;
            // 
            // hiddenLabel1
            // 
            hiddenLabel1.Location = new Point(0, 25);
            hiddenLabel1.Name = "hiddenLabel1";
            hiddenLabel1.Size = new Size(41, 32);
            hiddenLabel1.TabIndex = 11;
            hiddenLabel1.Text = " ";
            hiddenLabel1.Click += hiddenLabel1_Click;
            hiddenLabel1.DoubleClick += hiddenLabel1_DoubleClick;
            // 
            // hiddenLabel2
            // 
            hiddenLabel2.Location = new Point(368, 25);
            hiddenLabel2.Name = "hiddenLabel2";
            hiddenLabel2.Size = new Size(260, 32);
            hiddenLabel2.TabIndex = 12;
            hiddenLabel2.Text = " ";
            hiddenLabel2.Click += hiddenLabel2_Click;
            // 
            // hiddenLabel3
            // 
            hiddenLabel3.Location = new Point(901, 25);
            hiddenLabel3.Name = "hiddenLabel3";
            hiddenLabel3.Size = new Size(99, 32);
            hiddenLabel3.TabIndex = 13;
            hiddenLabel3.Text = " ";
            hiddenLabel3.Click += hiddenLabel3_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 30);
            ClientSize = new Size(1000, 551);
            Controls.Add(lblDealerValue);
            Controls.Add(hiddenLabel3);
            Controls.Add(hiddenLabel2);
            Controls.Add(hiddenLabel1);
            Controls.Add(madeByLabel);
            Controls.Add(versionLabel);
            Controls.Add(lblPlayerValue);
            Controls.Add(lblStatus);
            Controls.Add(btnHit);
            Controls.Add(btnStand);
            Controls.Add(btnPlayAgain);
            Controls.Add(pnlPlayerArea);
            Controls.Add(pnlDealerArea);
            Controls.Add(blackjackMenuStrip);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = blackjackMenuStrip;
            MaximizeBox = false;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "BlackJack Game";
            Load += MainForm_Load;
            blackjackMenuStrip.ResumeLayout(false);
            blackjackMenuStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.Label madeByLabel;
        private System.Windows.Forms.MenuStrip blackjackMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Label hiddenLabel1;
        private System.Windows.Forms.Label hiddenLabel2;
        private System.Windows.Forms.Label hiddenLabel3;
        private System.Windows.Forms.ToolStripMenuItem themeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem defaultToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem darkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changingThemeWillRestartGameToolStripMenuItem;
        private ToolStripMenuItem gameToolStripMenuItem;
        private ToolStripMenuItem blackjackToolStripMenuItem;
        private ToolStripMenuItem rideTheBusToolStripMenuItem;
    }
}