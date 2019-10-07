using System;
using System.Windows.Forms;

namespace SonicRetro.SonLVL
{
	public partial class ReplaceTilesDialog : Form
	{
		public ReplaceTilesDialog()
		{
			InitializeComponent();
		}

		private void ReplaceBlockTilesDialog_VisibleChanged(object sender, EventArgs e)
		{
			if (Visible)
			{
				findTile.UpdateStuff();
				replaceTile.UpdateStuff();
			}
		}
	}
}
