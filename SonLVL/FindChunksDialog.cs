using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SonicRetro.SonLVL.API;

namespace SonicRetro.SonLVL
{
	public partial class FindChunksDialog : Form
	{
		public FindChunksDialog()
		{
			InitializeComponent();
		}

		private void tileList1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (tileList1.SelectedIndex != -1)
				chunkSelect.Value = tileList1.SelectedIndex;
		}

		private void FindChunksDialog_VisibleChanged(object sender, EventArgs e)
		{
			if (Visible)
			{
				tileList1.Images = LevelData.CompChunkBmps;
				tileList1.ImageWidth = LevelData.Level.ChunkWidth;
				tileList1.ImageHeight = LevelData.Level.ChunkHeight;
				chunkSelect.Maximum = LevelData.Chunks.Count;
				tileList1.SelectedIndex = (int)chunkSelect.Value;
			}
		}
	}
}
