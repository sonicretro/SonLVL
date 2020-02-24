using System;
using System.Windows.Forms;
using SonicRetro.SonLVL.API;

namespace SonicRetro.SonLVL.GUI
{
	public partial class ResizeLevelDialog : Form
	{
		private bool fg;

		public ResizeLevelDialog(bool FG)
		{
			fg = FG;
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

		private void levelHeight_ValueChanged(object sender, EventArgs e)
		{
			int maxBytes = LevelData.LayoutFormat.MaxBytes;
			if (maxBytes != -1)
			{
				if (fg)
					levelWidth.Maximum = Math.Floor((maxBytes - (LevelData.Layout.BGLayout.Length)) / levelHeight.Value);
				else
					levelWidth.Maximum = Math.Floor((maxBytes - (LevelData.Layout.FGLayout.Length)) / levelHeight.Value);
			}
		}
	}
}
