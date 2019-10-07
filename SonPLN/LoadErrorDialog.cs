using System;
using System.Windows.Forms;

namespace SonicRetro.SonLVL.SonPLN
{
	public partial class LoadErrorDialog : Form
	{
		public LoadErrorDialog(string error)
		{
			InitializeComponent();
			label1.Text += error;
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void reportButton_Click(object sender, EventArgs e)
		{
			using (API.BugReportDialog dlg = new API.BugReportDialog("SonPLN", string.Join(Environment.NewLine, MainForm.Instance.LogFile.ToArray())))
				dlg.ShowDialog(this);
			Close();
		}
	}
}
