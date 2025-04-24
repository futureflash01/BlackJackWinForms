using BlackjackGame.Properties;
using DarkModeForms;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace BlackjackGame.Forms
{
    public partial class AboutForm : Form
    {

        private DarkModeCS dm;

        public AboutForm(string versionLabel, bool isDarkMode)
        {
            InitializeComponent();
            this.Text = $"BlackJack Game ({versionLabel})";

            dm = new DarkModeCS(this)
            {
                ColorMode = DarkModeCS.DisplayMode.DarkMode
            };

            if (isDarkMode)
            {
                logoPB.Image = Resources.AppIcon_Dark;
            }

            else
            {
                logoPB.Image = Resources.AppIcon;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
