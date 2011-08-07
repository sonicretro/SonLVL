using System.Windows.Forms;

namespace SonicRetro.SonLVL
{
    public partial class ToolWindow : Form
    {
        public ToolWindow()
        {
            InitializeComponent();
        }

        private void ToolWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing)
                return;
            Hide();
            LevelData.MainForm.toolWindowToolStripMenuItem.Checked = false;
            e.Cancel = true;
        }
    }
}