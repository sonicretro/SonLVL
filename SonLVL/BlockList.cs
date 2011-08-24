using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SonicRetro.SonLVL
{
    public partial class BlockList : UserControl
    {
        private int selectedIndex = -1;
        [Browsable(false)]
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                if (SelectedIndexChanged != null)
                    SelectedIndexChanged(this, EventArgs.Empty);
            }
        }
        public event EventHandler SelectedIndexChanged;

        private int imageSize = 8;
        public int ImageSize
        {
            get { return imageSize; }
            set
            {
                imageSize = value;
                ChangeSize();
            }
        }

        public List<Bitmap[]> Images = new List<Bitmap[]>();

        public BlockList()
        {
            InitializeComponent();
        }

        public void ChangeSize()
        {
            int tilesPerRow = Width / (imageSize + 4);
            vScrollBar1.Maximum = Math.Max(((int)Math.Ceiling(Images.Count / (double)tilesPerRow) * (imageSize + 4)) - Height, 0);
        }

        private void TileList_Resize(object sender, EventArgs e) { ChangeSize(); }

        private void TileList_Paint(object sender, PaintEventArgs e)
        {
            int actualImageSize = imageSize + 4;
            int tilesPerRow = Width / actualImageSize;
            int numRows = (int)Math.Ceiling(Images.Count / (double)tilesPerRow);
            int str = vScrollBar1.Value / actualImageSize;
            int edr = Math.Min((int)Math.Ceiling((vScrollBar1.Value + Height) / (double)actualImageSize), numRows);
            Graphics g = e.Graphics;
            g.SetOptions();
            g.Clear(BackColor);
            if (Images.Count == 0) return;
            int i = str * tilesPerRow;
            for (int r = str; r < edr; r++)
                for (int c = 0; c < tilesPerRow; c++)
                {
                    if (i == selectedIndex)
                        g.DrawRectangle(new Pen(Color.Yellow, 2), actualImageSize * c, (actualImageSize * r) - vScrollBar1.Value, actualImageSize, actualImageSize);
                    if (LevelData.MainForm.lowToolStripMenuItem.Checked)
                        g.DrawImage(Images[i][0], (actualImageSize * c) + 2, (actualImageSize * r) + 2 - vScrollBar1.Value, imageSize, imageSize);
                    if (LevelData.MainForm.highToolStripMenuItem.Checked)
                        g.DrawImage(Images[i][1], (actualImageSize * c) + 2, (actualImageSize * r) + 2 - vScrollBar1.Value, imageSize, imageSize);
                    i++;
                    if (i == Images.Count) return;
                }
        }

        private void TileList_MouseDown(object sender, MouseEventArgs e)
        {
            int actualImageSize = imageSize + 4;
            int tilesPerRow = Width / actualImageSize;
            int numRows = (int)Math.Ceiling(Images.Count / (double)tilesPerRow);
            int selX = e.X / actualImageSize;
            if (selX >= tilesPerRow) return;
            int selY = (e.Y + vScrollBar1.Value) / actualImageSize;
            if (selY * tilesPerRow + selX < Images.Count)
                SelectedIndex = selY * tilesPerRow + selX;
            Invalidate();
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            Invalidate();
        }
    }
}