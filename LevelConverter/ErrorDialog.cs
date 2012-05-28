using System;
using System.Windows.Forms;

namespace SonicRetro.SonLVL.LevelConverter
{
    public partial class ErrorDialog : Form
    {
        public ErrorDialog(string message, bool allowContinue)
        {
            InitializeComponent();
            label1.Text = message;
            okButton.Enabled = allowContinue;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (BugReportDialog err = new BugReportDialog())
                err.ShowDialog();
        }
    }
}
