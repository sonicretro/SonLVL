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
                tileList1.Images = new List<Bitmap>(LevelData.CompChunkBmps);
                tileList1.ImageSize = LevelData.chunksz;
                chunkSelect.Maximum = LevelData.Chunks.Count;
                tileList1.SelectedIndex = (int)chunkSelect.Value;
            }
        }
    }
}
