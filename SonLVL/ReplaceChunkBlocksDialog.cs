using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SonicRetro.SonLVL.API;

namespace SonicRetro.SonLVL
{
	public partial class ReplaceChunkBlocksDialog : Form
	{
		public ReplaceChunkBlocksDialog()
		{
			InitializeComponent();
		}

		private void ReplaceChunkBlocksDialog_VisibleChanged(object sender, EventArgs e)
		{
			if (Visible)
			{
				findBlock.UpdateStuff();
				replaceBlock.UpdateStuff();
			}
		}
	}
}
