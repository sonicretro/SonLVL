using System;
using System.Windows.Forms;

namespace SonicRetro.SonLVL
{
    public partial class AddLineDialog : Form
    {
        public AddLineDialog()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
