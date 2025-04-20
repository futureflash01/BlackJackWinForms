using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DarkModeForms;

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
    }
}
