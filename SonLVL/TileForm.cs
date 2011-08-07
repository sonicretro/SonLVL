using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SonicRetro.SonLVL
{
    public partial class TileForm : Form
    {
        public TileForm()
        {
            InitializeComponent();
            curpal = new Color[16];
            for (int i = 0; i < 16; i++)
                curpal[i] = LevelData.PaletteToColor(0, i, false);
        }

        public int selectedChunk, selectedBlock, selectedTile;
        public Point selectedChunkBlock, selectedBlockTile, selectedColor;

        private void ChunkPicture_MouseClick(object sender, MouseEventArgs e)
        {
            selectedChunkBlock = new Point(e.X / 16, e.Y / 16);
            ChunkBlockPropertyGrid.SelectedObject = LevelData.Chunks[selectedChunk].blocks[e.X / 16, e.Y / 16];
            ChunkPicture.Invalidate();
        }

        private void ChunkBlockPropertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            LevelData.RedrawChunk(selectedChunk);
            LevelData.MainForm.DrawLevel();
            ChunkPicture.Invalidate();
        }

        private void ChunkSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ChunkSelector.SelectedIndex > -1)
            {
                selectedChunk = ChunkSelector.SelectedIndex;
                selectedChunkBlock = new Point();
                ChunkBlockPropertyGrid.SelectedObject = LevelData.Chunks[selectedChunk].blocks[0, 0];
                ChunkPicture.Invalidate();
                ChunkID.Text = selectedChunk.ToString();
            }
        }

        private void ChunkPicture_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(LevelData.PaletteToColor(2, 0, false));
            e.Graphics.DrawImage(LevelData.ChunkBmps[selectedChunk][0], 0, 0, LevelData.chunksz, LevelData.chunksz);
            e.Graphics.DrawImage(LevelData.ChunkBmps[selectedChunk][1], 0, 0, LevelData.chunksz, LevelData.chunksz);
            if (LevelData.MainForm.path1ToolStripMenuItem.Checked)
                e.Graphics.DrawImage(LevelData.ChunkColBmps[selectedChunk][0], 0, 0, LevelData.chunksz, LevelData.chunksz);
            if (LevelData.MainForm.path2ToolStripMenuItem.Checked)
                e.Graphics.DrawImage(LevelData.ChunkColBmps[selectedChunk][1], 0, 0, LevelData.chunksz, LevelData.chunksz);
            e.Graphics.DrawRectangle(Pens.White, selectedChunkBlock.X * 16 - 1, selectedChunkBlock.Y * 16 - 1, 18, 18);
        }

        private void BlockPicture_MouseClick(object sender, MouseEventArgs e)
        {
            selectedBlockTile = new Point(e.X / 32, e.Y / 32);
            BlockTilePropertyGrid.SelectedObject = LevelData.Blocks[selectedBlock].tiles[e.X / 32, e.Y / 32];
            BlockPicture.Invalidate();
        }

        private void BlockTilePropertyGrid_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            LevelData.RedrawBlock(selectedBlock, true);
            LevelData.MainForm.DrawLevel();
            BlockPicture.Invalidate();
        }

        private void BlockSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (BlockSelector.SelectedIndex > -1)
            {
                selectedBlock = BlockSelector.SelectedIndex;
                selectedBlockTile = new Point();
                BlockTilePropertyGrid.SelectedObject = LevelData.Blocks[selectedBlock].tiles[0, 0];
                BlockCollision1.Value = LevelData.ColInds1[selectedBlock];
                BlockCollision2.Value = LevelData.ColInds2[selectedBlock];
                BlockID.Text = selectedBlock.ToString();
                BlockPicture.Invalidate();
            }
        }

        private void BlockPicture_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.Clear(LevelData.PaletteToColor(2, 0, false));
            e.Graphics.DrawImage(LevelData.BlockBmpBits[selectedBlock][0].Scale(4).ToBitmap(LevelData.BmpPal), 0, 0, 64, 64);
            e.Graphics.DrawImage(LevelData.BlockBmpBits[selectedBlock][1].Scale(4).ToBitmap(LevelData.BmpPal), 0, 0, 64, 64);
            if (LevelData.MainForm.path1ToolStripMenuItem.Checked)
                e.Graphics.DrawImage(LevelData.ColBmps[LevelData.ColInds1[selectedBlock]], 0, 0, 64, 64);
            if (LevelData.MainForm.path2ToolStripMenuItem.Checked)
                e.Graphics.DrawImage(LevelData.ColBmps[LevelData.ColInds2[selectedBlock]], 0, 0, 64, 64);
            e.Graphics.DrawRectangle(Pens.White, selectedBlockTile.X * 32, selectedBlockTile.Y * 32, 31, 31);
        }

        private void PalettePanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);
            for (int y = 0; y <= 3; y++)
                for (int x = 0; x <= 15; x++)
                {
                    e.Graphics.FillRectangle(new SolidBrush(LevelData.PaletteToColor(y, x, false)), x * 32, y * 32, 32, 32);
                    e.Graphics.DrawRectangle(Pens.White, x * 32, y * 32, 31, 31);
                }
            e.Graphics.DrawRectangle(new Pen(Color.Yellow, 2), selectedColor.X * 32, selectedColor.Y * 32, 32, 32);
        }

        int[] cols;
        private void PalettePanel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int line = e.Y / 32;
            int index = e.X / 32;
            ColorDialog a = new ColorDialog
            {
                AllowFullOpen = true,
                AnyColor = true,
                Color = LevelData.PaletteToColor(line, index, false)
            };
            if (cols != null)
                a.CustomColors = cols;
            if (a.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LevelData.ColorToPalette(line, index, a.Color);
                PalettePanel.Invalidate();
                LevelData.PaletteChanged();
                ChunkSelector.Invalidate();
                ChunkPicture.Invalidate();
                BlockSelector.Invalidate();
                BlockPicture.Invalidate();
                TileSelector.Invalidate();
                TilePicture.Invalidate();
            }
            cols = a.CustomColors;
        }

        private void BlockCollision1_ValueChanged(object sender, EventArgs e)
        {
            LevelData.ColInds1[selectedBlock] = (byte)BlockCollision1.Value;
        }

        private void BlockCollision2_ValueChanged(object sender, EventArgs e)
        {
            LevelData.ColInds2[selectedBlock] = (byte)BlockCollision2.Value;
        }

        private Color[] curpal;
        private void PalettePanel_MouseDown(object sender, MouseEventArgs e)
        {
            bool newpal = e.Y / 32 != selectedColor.Y;
            selectedColor = new Point(e.X / 32, e.Y / 32);
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                contextMenuStrip2.Show(PalettePanel, e.Location);
            }
            PalettePanel.Invalidate();
            if (newpal)
            {
                curpal = new Color[16];
                for (int i = 0; i < 16; i++)
                    curpal[i] = LevelData.PaletteToColor(selectedColor.Y, i, false);
            }
            TilePicture.Invalidate();
        }

        private void importToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog a = new OpenFileDialog())
            {
                a.DefaultExt = "bin";
                a.Filter = "MD Palettes|*.bin|Image Files|*.bmp;*.png;*.jpg;*.gif";
                a.RestoreDirectory = true;
                if (a.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    int l = selectedColor.Y;
                    int x = selectedColor.X;
                    switch (System.IO.Path.GetExtension(a.FileName))
                    {
                        case ".bin":
                            byte[] file = System.IO.File.ReadAllBytes(a.FileName);
                            for (int i = 0; i < file.Length; i += 2)
                            {
                                LevelData.Palette[LevelData.CurPal][l, x] = BitConverter.ToUInt16(file, i);
                                x++;
                                if (x == 16)
                                {
                                    x = 0;
                                    l++;
                                    if (l == 4)
                                        break;
                                }
                            }
                            break;
                        case ".bmp":
                        case ".png":
                        case ".jpg":
                        case ".gif":
                            Bitmap bmp = new Bitmap(a.FileName);
                            if ((bmp.PixelFormat & System.Drawing.Imaging.PixelFormat.Indexed) == System.Drawing.Imaging.PixelFormat.Indexed)
                            {
                                Color[] pal = bmp.Palette.Entries;
                                for (int i = 0; i < pal.Length; i++)
                                {
                                    LevelData.ColorToPalette(l, x, pal[i]);
                                    x++;
                                    if (x == 16)
                                    {
                                        x = 0;
                                        l++;
                                        if (l == 4)
                                            break;
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            LevelData.PaletteChanged();
            ChunkSelector.Invalidate();
            ChunkPicture.Invalidate();
            BlockSelector.Invalidate();
            BlockPicture.Invalidate();
            TileSelector.Invalidate();
            TilePicture.Invalidate();
        }

        private BitmapBits tile;
        private void TileSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TileSelector.SelectedIndex > -1)
            {
                selectedTile = TileSelector.SelectedIndex;
                tile = BitmapBits.FromTile(LevelData.Tiles[selectedTile], 0);
                TileID.Text = selectedTile.ToString();
                TilePicture.Invalidate();
            }
        }

        private void TilePicture_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SetOptions();
            e.Graphics.DrawImage(tile.Scale(16).ToBitmap(curpal), 0, 0, 128, 128);
        }

        private void TilePicture_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                tile.Bits[((e.Y / 16) * 8) + (e.X / 16)] = (byte)selectedColor.X;
                TilePicture.Invalidate();
            }
        }

        private void TilePicture_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left && new Rectangle(Point.Empty, TilePicture.Size).Contains(e.Location))
            {
                tile.Bits[((e.Y / 16) * 8) + (e.X / 16)] = (byte)selectedColor.X;
                TilePicture.Invalidate();
            }
        }

        private void TilePicture_MouseUp(object sender, MouseEventArgs e)
        {
            LevelData.Tiles[selectedTile] = tile.ToTile();
            LevelData.Tiles[selectedTile].CopyTo(LevelData.TileArray, selectedTile * 32);
            for (int i = 0; i < LevelData.Blocks.Count; i++)
            {
                bool dr = false;
                for (int y = 0; y < 2; y++)
                    for (int x = 0; x < 2; x++)
                        if (LevelData.Blocks[i].tiles[x, y].Tile == selectedTile)
                            dr = true;
                if (dr)
                    LevelData.RedrawBlock(i, true);
            }
            TileSelector.Images[selectedTile] = LevelData.TileToBmp4bpp(LevelData.Tiles[selectedTile], 0, 2);
        }

        private void ChunkSelector_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                pasteBeforeToolStripMenuItem.Enabled = Clipboard.GetDataObject().GetDataPresent("SonLVLChunk") & LevelData.Chunks.Count < 256;
                pasteAfterToolStripMenuItem.Enabled = pasteBeforeToolStripMenuItem.Enabled;
                importToolStripMenuItem.Enabled = LevelData.Chunks.Count < 256;
                insertAfterToolStripMenuItem.Enabled = importToolStripMenuItem.Enabled;
                insertBeforeToolStripMenuItem.Enabled = importToolStripMenuItem.Enabled;
                contextMenuStrip1.Show(ChunkSelector, e.Location);
            }
        }

        private void BlockSelector_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                pasteBeforeToolStripMenuItem.Enabled = Clipboard.GetDataObject().GetDataPresent("SonLVLBlock");
                pasteAfterToolStripMenuItem.Enabled = pasteBeforeToolStripMenuItem.Enabled;
                importToolStripMenuItem.Enabled = true;
                insertAfterToolStripMenuItem.Enabled = importToolStripMenuItem.Enabled;
                insertBeforeToolStripMenuItem.Enabled = importToolStripMenuItem.Enabled;
                contextMenuStrip1.Show(BlockSelector, e.Location);
            }
        }

        private void TileSelector_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                pasteBeforeToolStripMenuItem.Enabled = Clipboard.GetDataObject().GetDataPresent("SonLVLTile");
                pasteAfterToolStripMenuItem.Enabled = pasteBeforeToolStripMenuItem.Enabled;
                importToolStripMenuItem.Enabled = true;
                insertAfterToolStripMenuItem.Enabled = importToolStripMenuItem.Enabled;
                insertBeforeToolStripMenuItem.Enabled = importToolStripMenuItem.Enabled;
                contextMenuStrip1.Show(TileSelector, e.Location);
            }
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0: // Chunks
                    Clipboard.SetData("SonLVLChunk", LevelData.Chunks[selectedChunk].GetBytes());
                    LevelData.Chunks.RemoveAt(selectedChunk);
                    LevelData.ChunkBmpBits.RemoveAt(selectedChunk);
                    LevelData.ChunkBmps.RemoveAt(selectedChunk);
                    LevelData.ChunkColBmpBits.RemoveAt(selectedChunk);
                    LevelData.ChunkColBmps.RemoveAt(selectedChunk);
                    LevelData.MainForm.SelectedTile = (byte)Math.Min(LevelData.MainForm.SelectedTile, LevelData.Chunks.Count - 1);
                    for (int y = 0; y < LevelData.FGLayout.GetLength(1); y++)
                        for (int x = 0; x < LevelData.FGLayout.GetLength(0); x++)
                            if (LevelData.FGLayout[x, y] > selectedChunk)
                                LevelData.FGLayout[x, y]--;
                    for (int y = 0; y < LevelData.BGLayout.GetLength(1); y++)
                        for (int x = 0; x < LevelData.BGLayout.GetLength(0); x++)
                            if (LevelData.BGLayout[x, y] > selectedChunk)
                                LevelData.BGLayout[x, y]--;
                    ChunkSelector.SelectedIndex = Math.Min(ChunkSelector.SelectedIndex, LevelData.Chunks.Count - 1);
                    break;
                case 1: // Blocks
                    Clipboard.SetData("SonLVLBlock", LevelData.Blocks[selectedBlock].GetBytes());
                    LevelData.Blocks.RemoveAt(selectedBlock);
                    LevelData.BlockBmps.RemoveAt(selectedBlock);
                    LevelData.BlockBmpBits.RemoveAt(selectedBlock);
                    LevelData.ColInds1.RemoveAt(selectedBlock);
                    if (LevelData.EngineVersion == EngineVersion.S2 || LevelData.EngineVersion == EngineVersion.S3K || LevelData.EngineVersion == EngineVersion.SKC)
                        LevelData.ColInds2.RemoveAt(selectedBlock);
                    for (int i = 0; i < LevelData.Chunks.Count; i++)
                    {
                        bool dr = false;
                        for (int y = 0; y < LevelData.chunksz / 16; y++)
                            for (int x = 0; x < LevelData.chunksz / 16; x++)
                                if (LevelData.Chunks[i].blocks[x, y].Block == selectedBlock)
                                    dr = true;
                                else if (LevelData.Chunks[i].blocks[x, y].Block > selectedBlock)
                                    LevelData.Chunks[i].blocks[x, y].Block--;
                        if (dr)
                            LevelData.RedrawChunk(i);
                    }
                    BlockSelector.SelectedIndex = Math.Min(BlockSelector.SelectedIndex, LevelData.Blocks.Count - 1);
                    break;
                case 2: // Tiles
                    Clipboard.SetData("SonLVLTile", LevelData.Tiles[selectedTile]);
                    LevelData.Tiles.RemoveAt(selectedTile);
                    LevelData.UpdateTileArray();
                    TileSelector.Images.RemoveAt(selectedTile);
                    for (int i = 0; i < LevelData.Blocks.Count; i++)
                    {
                        bool dr = false;
                        for (int y = 0; y < 2; y++)
                            for (int x = 0; x < 2; x++)
                                if (LevelData.Blocks[i].tiles[x, y].Tile == selectedTile)
                                    dr = true;
                                else if (LevelData.Blocks[i].tiles[x, y].Tile > selectedTile)
                                    LevelData.Blocks[i].tiles[x, y].Tile--;
                        if (dr)
                            LevelData.RedrawBlock(i, true);
                    }
                    TileSelector.SelectedIndex = Math.Min(TileSelector.SelectedIndex, TileSelector.Images.Count - 1);
                    break;
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0: // Chunks
                    Clipboard.SetData("SonLVLChunk", LevelData.Chunks[selectedChunk].GetBytes());
                    break;
                case 1: // Blocks
                    Clipboard.SetData("SonLVLBlock", LevelData.Blocks[selectedBlock].GetBytes());
                    break;
                case 2: // Tiles
                    Clipboard.SetData("SonLVLTile", LevelData.Tiles[selectedTile]);
                    break;
            }
        }

        private void pasteBeforeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0: // Chunks
                    LevelData.Chunks.InsertBefore(selectedChunk, new Chunk((byte[])Clipboard.GetData("SonLVLChunk"), 0));
                    LevelData.ChunkBmpBits.Insert(selectedChunk, new BitmapBits[2]);
                    LevelData.ChunkBmps.Insert(selectedChunk, new Bitmap[2]);
                    LevelData.ChunkColBmpBits.Insert(selectedChunk, new BitmapBits[2]);
                    LevelData.ChunkColBmps.Insert(selectedChunk, new Bitmap[2]);
                    for (int y = 0; y < LevelData.FGLayout.GetLength(1); y++)
                        for (int x = 0; x < LevelData.FGLayout.GetLength(0); x++)
                            if (LevelData.FGLayout[x, y] >= selectedChunk)
                                LevelData.FGLayout[x, y]++;
                    for (int y = 0; y < LevelData.BGLayout.GetLength(1); y++)
                        for (int x = 0; x < LevelData.BGLayout.GetLength(0); x++)
                            if (LevelData.BGLayout[x, y] >= selectedChunk)
                                LevelData.BGLayout[x, y]++;
                    LevelData.RedrawChunk(selectedChunk);
                    ChunkSelector.SelectedIndex = selectedChunk;
                    break;
                case 1: // Blocks
                    LevelData.Blocks.InsertBefore(selectedBlock, new Block((byte[])Clipboard.GetData("SonLVLBlock"), 0));
                    LevelData.BlockBmps.Insert(selectedBlock, new Bitmap[2]);
                    LevelData.BlockBmpBits.Insert(selectedBlock, new BitmapBits[2]);
                    LevelData.ColInds1.Insert(selectedBlock, 0);
                    if (LevelData.EngineVersion == EngineVersion.S2 || LevelData.EngineVersion == EngineVersion.S3K || LevelData.EngineVersion == EngineVersion.SKC)
                        LevelData.ColInds2.Insert(selectedBlock, 0);
                    for (int i = 0; i < LevelData.Chunks.Count; i++)
                        for (int y = 0; y < LevelData.chunksz / 16; y++)
                            for (int x = 0; x < LevelData.chunksz / 16; x++)
                                if (LevelData.Chunks[i].blocks[x, y].Block >= selectedBlock)
                                    LevelData.Chunks[i].blocks[x, y].Block++;
                    LevelData.RedrawBlock(selectedBlock, false);
                    BlockSelector.SelectedIndex = selectedBlock;
                    break;
                case 2: // Tiles
                    byte[] t = (byte[])Clipboard.GetData("SonLVLTile");
                    LevelData.Tiles.InsertBefore(selectedTile, t);
                    LevelData.UpdateTileArray();
                    TileSelector.Images.Insert(selectedTile, LevelData.TileToBmp4bpp(LevelData.Tiles[selectedTile], 0, 2));
                    for (int i = 0; i < LevelData.Blocks.Count; i++)
                        for (int y = 0; y < 2; y++)
                            for (int x = 0; x < 2; x++)
                                if (LevelData.Blocks[i].tiles[x, y].Tile >= selectedTile)
                                    LevelData.Blocks[i].tiles[x, y].Tile++;
                    TileSelector.SelectedIndex = selectedTile;
                    break;
            }
        }

        private void pasteAfterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0: // Chunks
                    LevelData.Chunks.InsertAfter(selectedChunk, new Chunk((byte[])Clipboard.GetData("SonLVLChunk"), 0));
                    selectedChunk++;
                    LevelData.ChunkBmpBits.Insert(selectedChunk, new BitmapBits[2]);
                    LevelData.ChunkBmps.Insert(selectedChunk, new Bitmap[2]);
                    LevelData.ChunkColBmpBits.Insert(selectedChunk, new BitmapBits[2]);
                    LevelData.ChunkColBmps.Insert(selectedChunk, new Bitmap[2]);
                    for (int y = 0; y < LevelData.FGLayout.GetLength(1); y++)
                        for (int x = 0; x < LevelData.FGLayout.GetLength(0); x++)
                            if (LevelData.FGLayout[x, y] >= selectedChunk)
                                LevelData.FGLayout[x, y]++;
                    for (int y = 0; y < LevelData.BGLayout.GetLength(1); y++)
                        for (int x = 0; x < LevelData.BGLayout.GetLength(0); x++)
                            if (LevelData.BGLayout[x, y] >= selectedChunk)
                                LevelData.BGLayout[x, y]++;
                    LevelData.RedrawChunk(selectedChunk);
                    ChunkSelector.SelectedIndex = selectedChunk;
                    break;
                case 1: // Blocks
                    LevelData.Blocks.InsertAfter(selectedBlock, new Block((byte[])Clipboard.GetData("SonLVLBlock"), 0));
                    selectedBlock++;
                    LevelData.BlockBmps.Insert(selectedBlock, new Bitmap[2]);
                    LevelData.BlockBmpBits.Insert(selectedBlock, new BitmapBits[2]);
                    LevelData.ColInds1.Insert(selectedBlock, 0);
                    if (LevelData.EngineVersion == EngineVersion.S2 || LevelData.EngineVersion == EngineVersion.S3K || LevelData.EngineVersion == EngineVersion.SKC)
                        LevelData.ColInds2.Insert(selectedBlock, 0);
                    for (int i = 0; i < LevelData.Chunks.Count; i++)
                        for (int y = 0; y < LevelData.chunksz / 16; y++)
                            for (int x = 0; x < LevelData.chunksz / 16; x++)
                                if (LevelData.Chunks[i].blocks[x, y].Block >= selectedBlock)
                                    LevelData.Chunks[i].blocks[x, y].Block++;
                    LevelData.RedrawBlock(selectedBlock, false);
                    BlockSelector.SelectedIndex = selectedBlock;
                    break;
                case 2: // Tiles
                    byte[] t = (byte[])Clipboard.GetData("SonLVLTile");
                    LevelData.Tiles.InsertAfter(selectedTile, t);
                    selectedTile++;
                    LevelData.UpdateTileArray();
                    TileSelector.Images.Insert(selectedTile, LevelData.TileToBmp4bpp(LevelData.Tiles[selectedTile], 0, 2));
                    for (int i = 0; i < LevelData.Blocks.Count; i++)
                        for (int y = 0; y < 2; y++)
                            for (int x = 0; x < 2; x++)
                                if (LevelData.Blocks[i].tiles[x, y].Tile >= selectedTile)
                                    LevelData.Blocks[i].tiles[x, y].Tile++;
                    TileSelector.SelectedIndex = selectedTile;
                    break;
            }
        }

        private void insertBeforeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0: // Chunks

                    LevelData.Chunks.InsertBefore(selectedChunk, new Chunk());
                    LevelData.ChunkBmpBits.Insert(selectedChunk, new BitmapBits[2]);
                    LevelData.ChunkBmps.Insert(selectedChunk, new Bitmap[2]);
                    LevelData.ChunkColBmpBits.Insert(selectedChunk, new BitmapBits[2]);
                    LevelData.ChunkColBmps.Insert(selectedChunk, new Bitmap[2]);
                    for (int y = 0; y < LevelData.FGLayout.GetLength(1); y++)
                        for (int x = 0; x < LevelData.FGLayout.GetLength(0); x++)
                            if (LevelData.FGLayout[x, y] >= selectedChunk)
                                LevelData.FGLayout[x, y]++;
                    for (int y = 0; y < LevelData.BGLayout.GetLength(1); y++)
                        for (int x = 0; x < LevelData.BGLayout.GetLength(0); x++)
                            if (LevelData.BGLayout[x, y] >= selectedChunk)
                                LevelData.BGLayout[x, y]++;
                    LevelData.RedrawChunk(selectedChunk);
                    ChunkSelector.SelectedIndex = selectedChunk;
                    break;
                case 1: // Blocks
                    LevelData.Blocks.InsertBefore(selectedBlock, new Block());
                    LevelData.BlockBmps.Insert(selectedBlock, new Bitmap[2]);
                    LevelData.BlockBmpBits.Insert(selectedBlock, new BitmapBits[2]);
                    LevelData.ColInds1.Insert(selectedBlock, 0);
                    if (LevelData.EngineVersion == EngineVersion.S2 || LevelData.EngineVersion == EngineVersion.S3K || LevelData.EngineVersion == EngineVersion.SKC)
                        LevelData.ColInds2.Insert(selectedBlock, 0);
                    for (int i = 0; i < LevelData.Chunks.Count; i++)
                        for (int y = 0; y < LevelData.chunksz / 16; y++)
                            for (int x = 0; x < LevelData.chunksz / 16; x++)
                                if (LevelData.Chunks[i].blocks[x, y].Block >= selectedBlock)
                                    LevelData.Chunks[i].blocks[x, y].Block++;
                    LevelData.RedrawBlock(selectedBlock, false);
                    BlockSelector.SelectedIndex = selectedBlock;
                    break;
                case 2: // Tiles
                    LevelData.Tiles.InsertBefore(selectedTile, new byte[32]);
                    LevelData.UpdateTileArray();
                    TileSelector.Images.Insert(selectedTile, LevelData.TileToBmp4bpp(LevelData.Tiles[selectedTile], 0, 2));
                    for (int i = 0; i < LevelData.Blocks.Count; i++)
                        for (int y = 0; y < 2; y++)
                            for (int x = 0; x < 2; x++)
                                if (LevelData.Blocks[i].tiles[x, y].Tile >= selectedTile)
                                    LevelData.Blocks[i].tiles[x, y].Tile++;
                    TileSelector.SelectedIndex = selectedTile;
                    break;
            }
        }

        private void insertAfterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0: // Chunks

                    LevelData.Chunks.InsertAfter(selectedChunk, new Chunk());
                    selectedChunk++;
                    LevelData.ChunkBmpBits.Insert(selectedChunk, new BitmapBits[2]);
                    LevelData.ChunkBmps.Insert(selectedChunk, new Bitmap[2]);
                    LevelData.ChunkColBmpBits.Insert(selectedChunk, new BitmapBits[2]);
                    LevelData.ChunkColBmps.Insert(selectedChunk, new Bitmap[2]);
                    for (int y = 0; y < LevelData.FGLayout.GetLength(1); y++)
                        for (int x = 0; x < LevelData.FGLayout.GetLength(0); x++)
                            if (LevelData.FGLayout[x, y] >= selectedChunk)
                                LevelData.FGLayout[x, y]++;
                    for (int y = 0; y < LevelData.BGLayout.GetLength(1); y++)
                        for (int x = 0; x < LevelData.BGLayout.GetLength(0); x++)
                            if (LevelData.BGLayout[x, y] >= selectedChunk)
                                LevelData.BGLayout[x, y]++;
                    LevelData.RedrawChunk(selectedChunk);
                    ChunkSelector.SelectedIndex = selectedChunk;
                    break;
                case 1: // Blocks
                    LevelData.Blocks.InsertAfter(selectedBlock, new Block());
                    selectedBlock++;
                    LevelData.BlockBmps.Insert(selectedBlock, new Bitmap[2]);
                    LevelData.BlockBmpBits.Insert(selectedBlock, new BitmapBits[2]);
                    LevelData.ColInds1.Insert(selectedBlock, 0);
                    if (LevelData.EngineVersion == EngineVersion.S2 || LevelData.EngineVersion == EngineVersion.S3K || LevelData.EngineVersion == EngineVersion.SKC)
                        LevelData.ColInds2.Insert(selectedBlock, 0);
                    for (int i = 0; i < LevelData.Chunks.Count; i++)
                        for (int y = 0; y < LevelData.chunksz / 16; y++)
                            for (int x = 0; x < LevelData.chunksz / 16; x++)
                                if (LevelData.Chunks[i].blocks[x, y].Block >= selectedBlock)
                                    LevelData.Chunks[i].blocks[x, y].Block++;
                    LevelData.RedrawBlock(selectedBlock, false);
                    BlockSelector.SelectedIndex = selectedBlock;
                    break;
                case 2: // Tiles
                    LevelData.Tiles.InsertAfter(selectedTile, new byte[32]);
                    selectedTile++;
                    LevelData.UpdateTileArray();
                    TileSelector.Images.Insert(selectedTile, LevelData.TileToBmp4bpp(LevelData.Tiles[selectedTile], 0, 2));
                    for (int i = 0; i < LevelData.Blocks.Count; i++)
                        for (int y = 0; y < 2; y++)
                            for (int x = 0; x < 2; x++)
                                if (LevelData.Blocks[i].tiles[x, y].Tile >= selectedTile)
                                    LevelData.Blocks[i].tiles[x, y].Tile++;
                    TileSelector.SelectedIndex = selectedTile;
                    break;
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0: // Chunks
                    LevelData.Chunks.RemoveAt(selectedChunk);
                    LevelData.ChunkBmpBits.RemoveAt(selectedChunk);
                    LevelData.ChunkBmps.RemoveAt(selectedChunk);
                    LevelData.ChunkColBmpBits.RemoveAt(selectedChunk);
                    LevelData.ChunkColBmps.RemoveAt(selectedChunk);
                    LevelData.MainForm.SelectedTile = (byte)Math.Min(LevelData.MainForm.SelectedTile, LevelData.Chunks.Count - 1);
                    for (int y = 0; y < LevelData.FGLayout.GetLength(1); y++)
                        for (int x = 0; x < LevelData.FGLayout.GetLength(0); x++)
                            if (LevelData.FGLayout[x, y] > selectedChunk)
                                LevelData.FGLayout[x, y]--;
                    for (int y = 0; y < LevelData.BGLayout.GetLength(1); y++)
                        for (int x = 0; x < LevelData.BGLayout.GetLength(0); x++)
                            if (LevelData.BGLayout[x, y] > selectedChunk)
                                LevelData.BGLayout[x, y]--;
                    ChunkSelector.SelectedIndex = Math.Min(ChunkSelector.SelectedIndex, LevelData.Chunks.Count - 1);
                    break;
                case 1: // Blocks
                    LevelData.Blocks.RemoveAt(selectedBlock);
                    LevelData.BlockBmps.RemoveAt(selectedBlock);
                    LevelData.BlockBmpBits.RemoveAt(selectedBlock);
                    LevelData.ColInds1.RemoveAt(selectedBlock);
                    if (LevelData.EngineVersion == EngineVersion.S2 || LevelData.EngineVersion == EngineVersion.S3K || LevelData.EngineVersion == EngineVersion.SKC)
                        LevelData.ColInds2.RemoveAt(selectedBlock);
                    for (int i = 0; i < LevelData.Chunks.Count; i++)
                    {
                        bool dr = false;
                        for (int y = 0; y < LevelData.chunksz / 16; y++)
                            for (int x = 0; x < LevelData.chunksz / 16; x++)
                                if (LevelData.Chunks[i].blocks[x, y].Block == selectedBlock)
                                    dr = true;
                                else if (LevelData.Chunks[i].blocks[x, y].Block > selectedBlock)
                                    LevelData.Chunks[i].blocks[x, y].Block--;
                        if (dr)
                            LevelData.RedrawChunk(i);
                    }
                    BlockSelector.SelectedIndex = Math.Min(BlockSelector.SelectedIndex, LevelData.Blocks.Count - 1);
                    break;
                case 2: // Tiles
                    LevelData.Tiles.RemoveAt(selectedTile);
                    LevelData.UpdateTileArray();
                    TileSelector.Images.RemoveAt(selectedTile);
                    for (int i = 0; i < LevelData.Blocks.Count; i++)
                    {
                        bool dr = false;
                        for (int y = 0; y < 2; y++)
                            for (int x = 0; x < 2; x++)
                                if (LevelData.Blocks[i].tiles[x, y].Tile == selectedTile)
                                    dr = true;
                                else if (LevelData.Blocks[i].tiles[x, y].Tile > selectedTile)
                                    LevelData.Blocks[i].tiles[x, y].Tile--;
                        if (dr)
                            LevelData.RedrawBlock(i, true);
                    }
                    TileSelector.SelectedIndex = Math.Min(TileSelector.SelectedIndex, TileSelector.Images.Count - 1);
                    break;
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog opendlg = new OpenFileDialog())
            {
                opendlg.DefaultExt = "png";
                opendlg.Filter = "Image Files|*.bmp;*.png;*.jpg;*.gif";
                opendlg.RestoreDirectory = true;
                if (opendlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    Bitmap bmp = new Bitmap(opendlg.FileName);
                    int w = bmp.Width;
                    int h = bmp.Height;
                    int pal = 0;
                    List<BitmapBits> tiles = new List<BitmapBits>();
                    byte[] tile;
                    int curtilecnt = LevelData.Tiles.Count / 32;
                    int curblkcnt = LevelData.Blocks.Count;
                    switch (tabControl1.SelectedIndex)
                    {
                        case 0: // Chunks
                            for (int cy = 0; cy < h / LevelData.chunksz; cy++)
                                for (int cx = 0; cx < w / LevelData.chunksz; cx++)
                                {
                                    Chunk cnk = new Chunk();
                                    for (int by = 0; by < LevelData.chunksz / 16; by++)
                                        for (int bx = 0; bx < LevelData.chunksz / 16; bx++)
                                        {
                                            Block blk = new Block();
                                            for (int y = 0; y < 2; y++)
                                                for (int x = 0; x < 2; x++)
                                                {
                                                    tile = LevelData.BmpToTile(bmp.Clone(new Rectangle((cx * 16) + (bx * 16) + (x * 8), (cy * 16) + (by * 16) + (y * 8), 8, 8), bmp.PixelFormat), out pal);
                                                    blk.tiles[x, y].Palette = (byte)pal;
                                                    BitmapBits bits = BitmapBits.FromTile(tile, 0);
                                                    bool match = false;
                                                    for (int i = 0; i < tiles.Count; i++)
                                                    {
                                                        if (tiles[i].Equals(bits))
                                                        {
                                                            match = true;
                                                            blk.tiles[x, y].Tile = (ushort)(i + curtilecnt);
                                                            break;
                                                        }
                                                        BitmapBits flip = new BitmapBits(bits);
                                                        flip.Flip(true, false);
                                                        if (tiles[i].Equals(flip))
                                                        {
                                                            match = true;
                                                            blk.tiles[x, y].Tile = (ushort)(i + curtilecnt);
                                                            blk.tiles[x, y].XFlip = true;
                                                            break;
                                                        }
                                                        flip = new BitmapBits(bits);
                                                        flip.Flip(false, true);
                                                        if (tiles[i].Equals(flip))
                                                        {
                                                            match = true;
                                                            blk.tiles[x, y].Tile = (ushort)(i + curtilecnt);
                                                            blk.tiles[x, y].YFlip = true;
                                                            break;
                                                        }
                                                        flip = new BitmapBits(bits);
                                                        flip.Flip(true, true);
                                                        if (tiles[i].Equals(flip))
                                                        {
                                                            match = true;
                                                            blk.tiles[x, y].Tile = (ushort)(i + curtilecnt);
                                                            blk.tiles[x, y].XFlip = true;
                                                            blk.tiles[x, y].YFlip = true;
                                                            break;
                                                        }
                                                    }
                                                    if (match) continue;
                                                    tiles.Add(bits);
                                                    LevelData.Tiles.Add(tile);
                                                    selectedTile = LevelData.Tiles.Count - 1;
                                                    blk.tiles[x, y].Tile = (ushort)selectedTile;
                                                    LevelData.UpdateTileArray();
                                                    TileSelector.Images.Add(LevelData.TileToBmp4bpp(LevelData.Tiles[selectedTile], 0, 2));
                                                }
                                            LevelData.Blocks.Add(blk);
                                            LevelData.ColInds1.Add(0);
                                            if (LevelData.EngineVersion == EngineVersion.S2 || LevelData.EngineVersion == EngineVersion.S3K || LevelData.EngineVersion == EngineVersion.SKC)
                                                LevelData.ColInds2.Add(0);
                                            selectedBlock = LevelData.Blocks.Count - 1;
                                            LevelData.BlockBmps.Add(new Bitmap[2]);
                                            LevelData.BlockBmpBits.Add(new BitmapBits[2]);
                                            LevelData.RedrawBlock(selectedBlock, false);
                                            cnk.blocks[bx, by].Block = (ushort)selectedBlock;
                                        }
                                    LevelData.Chunks.Add(cnk);
                                    selectedChunk = LevelData.Chunks.Count - 1;
                                    LevelData.ChunkBmpBits.Add(new BitmapBits[2]);
                                    LevelData.ChunkBmps.Add(new Bitmap[2]);
                                    LevelData.ChunkColBmpBits.Add(new BitmapBits[2]);
                                    LevelData.ChunkColBmps.Add(new Bitmap[2]);
                                    LevelData.RedrawChunk(selectedChunk);
                                }
                            TileSelector.SelectedIndex = selectedTile;
                            BlockSelector.SelectedIndex = selectedBlock;
                            ChunkSelector.SelectedIndex = selectedChunk;
                            break;
                        case 1: // Blocks
                            for (int by = 0; by < h / 16; by++)
                                for (int bx = 0; bx < w / 16; bx++)
                                {
                                    Block blk = new Block();
                                    for (int y = 0; y < 2; y++)
                                        for (int x = 0; x < 2; x++)
                                        {
                                            tile = LevelData.BmpToTile(bmp.Clone(new Rectangle((bx * 16) + (x * 8), (by * 16) + (y * 8), 8, 8), bmp.PixelFormat), out pal);
                                            blk.tiles[x, y].Palette = (byte)pal;
                                            BitmapBits bits = BitmapBits.FromTile(tile, 0);
                                            bool match = false;
                                            for (int i = 0; i < tiles.Count; i++)
                                            {
                                                if (tiles[i].Equals(bits))
                                                {
                                                    match = true;
                                                    blk.tiles[x,y].Tile = (ushort)(i + curtilecnt);
                                                    break;
                                                }
                                                BitmapBits flip = new BitmapBits(bits);
                                                flip.Flip(true, false);
                                                if (tiles[i].Equals(flip))
                                                {
                                                    match = true;
                                                    blk.tiles[x, y].Tile = (ushort)(i + curtilecnt);
                                                    blk.tiles[x, y].XFlip = true;
                                                    break;
                                                }
                                                flip = new BitmapBits(bits);
                                                flip.Flip(false, true);
                                                if (tiles[i].Equals(flip))
                                                {
                                                    match = true;
                                                    blk.tiles[x, y].Tile = (ushort)(i + curtilecnt);
                                                    blk.tiles[x, y].YFlip = true;
                                                    break;
                                                }
                                                flip = new BitmapBits(bits);
                                                flip.Flip(true, true);
                                                if (tiles[i].Equals(flip))
                                                {
                                                    match = true;
                                                    blk.tiles[x, y].Tile = (ushort)(i + curtilecnt);
                                                    blk.tiles[x, y].XFlip = true;
                                                    blk.tiles[x, y].YFlip = true;
                                                    break;
                                                }
                                            }
                                            if (match) continue;
                                            tiles.Add(bits);
                                            LevelData.Tiles.Add(tile);
                                            selectedTile = LevelData.Tiles.Count - 1;
                                            blk.tiles[x, y].Tile = (ushort)selectedTile;
                                            LevelData.UpdateTileArray();
                                            TileSelector.Images.Add(LevelData.TileToBmp4bpp(LevelData.Tiles[selectedTile], 0, 2));
                                        }
                                    LevelData.Blocks.Add(blk);
                                    LevelData.ColInds1.Add(0);
                                    if (LevelData.EngineVersion == EngineVersion.S2 || LevelData.EngineVersion == EngineVersion.S3K || LevelData.EngineVersion == EngineVersion.SKC)
                                        LevelData.ColInds2.Add(0);
                                    selectedBlock = LevelData.Blocks.Count - 1;
                                    LevelData.BlockBmps.Add(new Bitmap[2]);
                                    LevelData.BlockBmpBits.Add(new BitmapBits[2]);
                                    LevelData.RedrawBlock(selectedBlock, false);
                                }
                            TileSelector.SelectedIndex = selectedTile;
                            BlockSelector.SelectedIndex = selectedBlock;
                            break;
                        case 2: // Tiles
                            for (int y = 0; y < h / 8; y++)
                                for (int x = 0; x < w / 8; x++)
                                {
                                    tile = LevelData.BmpToTile(bmp.Clone(new Rectangle(x * 8, y * 8, 8, 8), bmp.PixelFormat), out pal);
                                    BitmapBits bits = BitmapBits.FromTile(tile, 0);
                                    bool match = false;
                                    for (int i = 0; i < tiles.Count; i++)
                                    {
                                        if (tiles[i].Equals(bits))
                                        {
                                            match = true;
                                            break;
                                        }
                                        BitmapBits flip = new BitmapBits(bits);
                                        flip.Flip(true, false);
                                        if (tiles[i].Equals(flip))
                                        {
                                            match = true;
                                            break;
                                        }
                                        flip = new BitmapBits(bits);
                                        flip.Flip(false, true);
                                        if (tiles[i].Equals(flip))
                                        {
                                            match = true;
                                            break;
                                        }
                                        flip = new BitmapBits(bits);
                                        flip.Flip(true, true);
                                        if (tiles[i].Equals(flip))
                                        {
                                            match = true;
                                            break;
                                        }
                                    }
                                    if (match) continue;
                                    tiles.Add(bits);
                                    LevelData.Tiles.Add(tile);
                                    selectedTile = LevelData.Tiles.Count - 1;
                                    LevelData.UpdateTileArray();
                                    TileSelector.Images.Add(LevelData.TileToBmp4bpp(LevelData.Tiles[selectedTile], 0, 2));
                                }
                            TileSelector.SelectedIndex = selectedTile;
                            break;
                    }
                }
            }
        }

        private int selectedCol;
        private void CollisionSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CollisionSelector.SelectedIndex > -1)
            {
                selectedCol = CollisionSelector.SelectedIndex;
                ColAngle.Value = LevelData.Angles[selectedCol];
                ColID.Text = selectedCol.ToString();
                ChunkPicture.Invalidate();
            }
        }

        private void ColPicture_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SetOptions();
            e.Graphics.DrawImage(LevelData.ColBmpBits[selectedCol].Scale(4).ToBitmap(Color.Black, Color.White), 0, 0, 64, 64);
        }

        private void ColPicture_MouseDown(object sender, MouseEventArgs e)
        {
            int x = e.X / 4;
            int y = e.Y / 4;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    LevelData.ColArr1[selectedCol][x] = (sbyte)(16 - y);
                    break;
                case MouseButtons.Right:
                    if (y == 16)
                        LevelData.ColArr1[selectedCol][x] = 0;
                    else
                        LevelData.ColArr1[selectedCol][x] = (sbyte)(-y - 1);
                    break;
            }
            LevelData.RedrawCol(selectedCol, true);
            ColPicture.Invalidate();
            CollisionSelector.Images[selectedCol] = LevelData.ColBmps[selectedCol];
        }

        private void ColAngle_ValueChanged(object sender, EventArgs e)
        {
            LevelData.Angles[selectedCol] = (byte)ColAngle.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CollisionSelector sel = new CollisionSelector();
            sel.ShowDialog(this);
            BlockCollision1.Value = sel.Selection;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CollisionSelector sel = new CollisionSelector();
            sel.ShowDialog(this);
            BlockCollision2.Value = sel.Selection;
        }

        private void TileForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing)
                return;
            Hide();
            LevelData.MainForm.tileEditorToolStripMenuItem.Checked = false;
            e.Cancel = true;
        }
    }
}