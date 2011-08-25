using System;
using System.Windows.Forms;

namespace SonLVLINIEditor
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            GameData.MainForm = this;
        }

        private GameData data = new GameData();

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
                    comboBox1.Items.Clear();
                    foreach (LevelData item in data.levels)
                        comboBox1.Items.Add(item.Name);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog fd = new SaveFileDialog())
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
                    data.Save(fd.FileName);
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1)
            {
                propertyGrid2.SelectedObject = null;
                button4.Enabled = false;
            }
            else
            {
                propertyGrid2.SelectedObject = data.levels[comboBox1.SelectedIndex];
                button4.Enabled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            data.levels.Add(new LevelData());
            comboBox1.Items.Add(data.levels[data.levels.Count - 1].Name);
            comboBox1.SelectedIndex = data.levels.Count - 1;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            data.levels.RemoveAt(comboBox1.SelectedIndex);
            comboBox1.Items.RemoveAt(comboBox1.SelectedIndex);
        }

        internal void ChangeLevelName()
        {
            comboBox1.Items[comboBox1.SelectedIndex] = data.levels[comboBox1.SelectedIndex].Name;
        }
    }
}
