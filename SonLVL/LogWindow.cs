using System;
using System.Windows.Forms;

namespace SonicRetro.SonLVL.GUI
{
	public partial class LogWindow : Form
	{
		public LogWindow()
		{
			InitializeComponent();
		}

		private void LogWindow_Load(object sender, EventArgs e)
		{
			UpdateLines();
		}

		private void LogWindow_FormClosed(object sender, FormClosedEventArgs e)
		{
			MainForm.Instance.LogWindow = null;
			MainForm.Instance.logToolStripMenuItem.Enabled = true;
		}

		internal void UpdateLines()
		{
			richTextBox1.Lines = MainForm.Instance.LogFile.ToArray();
			richTextBox1.SelectionLength = 0;
			richTextBox1.SelectionStart = richTextBox1.TextLength;
			richTextBox1.ScrollToCaret();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			MainForm.Instance.LogFile.Clear();
			UpdateLines();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			SaveFileDialog a = new SaveFileDialog() { DefaultExt = "log", Filter = "Log Files|*.log;*.txt|All Files|*.*" };
			if (a.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				System.IO.File.WriteAllLines(a.FileName, MainForm.Instance.LogFile.ToArray());
		}
	}
}
