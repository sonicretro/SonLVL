using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SonicRetro.SonLVL.API;

namespace SonicRetro.SonLVL
{
	public partial class ReplaceBlockTilesDialog : Form
	{
		public ReplaceBlockTilesDialog()
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
