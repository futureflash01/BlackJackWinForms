namespace BlackjackGame.Forms
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            descriptionLabel = new System.Windows.Forms.Label();
            okButton = new System.Windows.Forms.Button();
            madebyLabel = new System.Windows.Forms.Label();
            logoPB = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)logoPB).BeginInit();
            SuspendLayout();
            // 
            // descriptionLabel
            // 
            descriptionLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            descriptionLabel.Location = new System.Drawing.Point(299, 12);
            descriptionLabel.Name = "descriptionLabel";
            descriptionLabel.Size = new System.Drawing.Size(601, 244);
            descriptionLabel.TabIndex = 0;
            descriptionLabel.Text = resources.GetString("descriptionLabel.Text");
            // 
            // okButton
            // 
            okButton.Cursor = System.Windows.Forms.Cursors.Hand;
            okButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            okButton.Location = new System.Drawing.Point(815, 256);
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(67, 27);
            okButton.TabIndex = 1;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += okButton_Click;
            // 
            // madebyLabel
            // 
            madebyLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 7F, System.Drawing.FontStyle.Bold);
            madebyLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            madebyLabel.Location = new System.Drawing.Point(301, 203);
            madebyLabel.Name = "madebyLabel";
            madebyLabel.Size = new System.Drawing.Size(601, 83);
            madebyLabel.TabIndex = 2;
            madebyLabel.Text = "Made by FutureFlash on 4/18/2025 \r\n\r\nStarted at 11:57 AM and finished at 5:09 PM (MST).\r\nTotal Time: 5 hours and 12 minutes. This\r\nincludes form design, programming, implementing\r\ngame logic, etc.";
            // 
            // logoPB
            // 
            logoPB.Image = Properties.Resources.AppIcon;
            logoPB.Location = new System.Drawing.Point(9, 8);
            logoPB.Name = "logoPB";
            logoPB.Size = new System.Drawing.Size(280, 280);
            logoPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            logoPB.TabIndex = 3;
            logoPB.TabStop = false;
            // 
            // AboutForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(903, 299);
            Controls.Add(logoPB);
            Controls.Add(okButton);
            Controls.Add(madebyLabel);
            Controls.Add(descriptionLabel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutForm";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "About";
            Load += AboutForm_Load;
            ((System.ComponentModel.ISupportInitialize)logoPB).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label madebyLabel;
        private System.Windows.Forms.PictureBox logoPB;
    }
}