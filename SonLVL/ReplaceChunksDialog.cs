using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SonicRetro.SonLVL.API;

namespace SonicRetro.SonLVL
{
	public partial class ReplaceChunksDialog : Form
	{
		public ReplaceChunksDialog()
		{
			InitializeComponent();
		}

		private void tileList1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (tileList1.SelectedIndex != -1)
				findChunk.Value = tileList1.SelectedIndex;
		}

		private void ReplaceChunksDialog_VisibleChanged(object sender, EventArgs e)
		{
			if (Visible)
			{
				tileList2.Images = tileList1.Images = LevelData.CompChunkBmps;
				tileList2.ImageWidth = tileList1.ImageWidth = LevelData.Level.ChunkWidth;
				tileList2.ImageHeight = tileList1.ImageHeight = LevelData.Level.ChunkHeight;
				replaceChunk.Maximum = findChunk.Maximum = LevelData.Chunks.Count;
				tileList1.SelectedIndex = (int)findChunk.Value;
				tileList2.SelectedIndex = (int)replaceChunk.Value;
			}
		}

		private void tileList2_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (tileList2.SelectedIndex != -1)
				replaceChunk.Value = tileList2.SelectedIndex;
		}
	}
}
