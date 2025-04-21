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

        public AboutForm(string versionLabel)
        {
            InitializeComponent();
            this.Text = $"BlackJack Game ({versionLabel})";

            dm = new DarkModeCS(this)
            {
                ColorMode = DarkModeCS.DisplayMode.DarkMode
            };
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {

        }
    }
}
