using System;
using System.Windows.Forms;

namespace SonLVLINIEditor
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private GameData data;

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fd = new OpenFileDialog())
            {
                fd.DefaultExt = "ini";
                fd.Filter = "INI Files|*.ini|All Files|*.*";
                if (System.IO.File.Exists(textBox1.Text))
                {
                    fd.InitialDirectory = System.IO.Path.GetDirectoryName(textBox1.Text);
                    fd.FileName = System.IO.Path.GetFileName(textBox1.Text);
                }
                if (fd.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = fd.FileName;
                    data = new GameData(fd.FileName);
                    propertyGrid1.SelectedObject = data;
                }
            }

        }
    }
}
