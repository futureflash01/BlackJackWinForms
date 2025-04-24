namespace BlackjackGame.Forms
{
    partial class EasterEggForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EasterEggForm));
            tabControl1 = new System.Windows.Forms.TabControl();
            imageTab = new System.Windows.Forms.TabPage();
            criticalErrorLabel = new System.Windows.Forms.Label();
            soundTab = new System.Windows.Forms.TabPage();
            tabControl1.SuspendLayout();
            imageTab.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(imageTab);
            tabControl1.Controls.Add(soundTab);
            tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            tabControl1.Location = new System.Drawing.Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new System.Drawing.Size(1034, 661);
            tabControl1.TabIndex = 0;
            // 
            // imageTab
            // 
            imageTab.AutoScroll = true;
            imageTab.Controls.Add(criticalErrorLabel);
            imageTab.Location = new System.Drawing.Point(4, 24);
            imageTab.Name = "imageTab";
            imageTab.Padding = new System.Windows.Forms.Padding(3);
            imageTab.Size = new System.Drawing.Size(1026, 633);
            imageTab.TabIndex = 0;
            imageTab.Text = "Images";
            imageTab.UseVisualStyleBackColor = true;
            imageTab.MouseEnter += imageTab_MouseEnter;
            // 
            // criticalErrorLabel
            // 
            criticalErrorLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            criticalErrorLabel.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            criticalErrorLabel.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            criticalErrorLabel.Location = new System.Drawing.Point(3, 3);
            criticalErrorLabel.Name = "criticalErrorLabel";
            criticalErrorLabel.Size = new System.Drawing.Size(1020, 627);
            criticalErrorLabel.TabIndex = 0;
            criticalErrorLabel.Text = resources.GetString("criticalErrorLabel.Text");
            criticalErrorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // soundTab
            // 
            soundTab.Location = new System.Drawing.Point(4, 24);
            soundTab.Name = "soundTab";
            soundTab.Padding = new System.Windows.Forms.Padding(3);
            soundTab.Size = new System.Drawing.Size(1026, 633);
            soundTab.TabIndex = 1;
            soundTab.Text = "Sounds";
            soundTab.UseVisualStyleBackColor = true;
            // 
            // EasterEggForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1034, 661);
            Controls.Add(tabControl1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "EasterEggForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "TEST MODE - Resource Viewer";
            Load += EasterEggForm_Load;
            tabControl1.ResumeLayout(false);
            imageTab.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage imageTab;
        private System.Windows.Forms.TabPage soundTab;
        private System.Windows.Forms.Label criticalErrorLabel;
    }
}