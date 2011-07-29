using System;
using System.Windows.Forms;

namespace SonicRetro.SonLVL
{
    public partial class AddGroupDialog : Form
    {
        public AddGroupDialog()
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
