using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SonicRetro.SonLVL
{
    public partial class TileList : UserControl
    {
        private int selectedIndex = -1;
        [Browsable(false)]
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                ScrollToSelected();
                Invalidate();
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

        public List<Bitmap> Images = new List<Bitmap>();

        public TileList()
        {
            InitializeComponent();
        }

        public void ChangeSize()
        {
            int tilesPerRow = Math.Max((Width - vScrollBar1.Width) / (imageSize + 4), 1);
            vScrollBar1.Maximum = Math.Max(((int)Math.Ceiling(Images.Count / (double)tilesPerRow) * (imageSize + 4)) - Height, 0);
        }

        private void TileList_Resize(object sender, EventArgs e) { ChangeSize(); }

        private void TileList_Paint(object sender, PaintEventArgs e)
        {
            if (Images.Count == 0) return;
            int actualImageSize = imageSize + 4;
            int tilesPerRow = Math.Max((Width - vScrollBar1.Width) / actualImageSize, 1);
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
                        g.DrawRectangle(new Pen(Color.Yellow, 2), actualImageSize * c, (actualImageSize * r) - vScrollBar1.Value, actualImageSize - 1, actualImageSize - 1);
                    g.DrawImage(Images[i], (actualImageSize * c) + 2, (actualImageSize * r) + 2 - vScrollBar1.Value, imageSize, imageSize);
                    i++;
                    if (i == Images.Count) return;
                }
        }

        private void TileList_MouseDown(object sender, MouseEventArgs e)
        {
            if (Images.Count == 0) return;
            int actualImageSize = imageSize + 4;
            int tilesPerRow = Math.Max((Width - vScrollBar1.Width) / actualImageSize, 1);
            int numRows = (int)Math.Ceiling(Images.Count / (double)tilesPerRow);
            int selX = e.X / actualImageSize;
            if (selX >= tilesPerRow) return;
            int selY = (e.Y + vScrollBar1.Value) / actualImageSize;
            if (selY * tilesPerRow + selX < Images.Count)
                SelectedIndex = selY * tilesPerRow + selX;
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            Invalidate();
        }

        private void TileList_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                    e.IsInputKey = true;
                    break;
            }
        }

        private void TileList_KeyDown(object sender, KeyEventArgs e)
        {
            if (Images.Count == 0) return;
            int actualImageSize = imageSize + 4;
            int tilesPerRow = Math.Max((Width - vScrollBar1.Width) / actualImageSize, 1);
            int rowsPerPage = Height / actualImageSize;
            switch (e.KeyCode)
            {
                case Keys.Down:
                    SelectedIndex = Math.Min(SelectedIndex + tilesPerRow, Images.Count - 1);
                    break;
                case Keys.Left:
                    SelectedIndex = Math.Max(SelectedIndex - 1, 0);
                    break;
                case Keys.Right:
                    SelectedIndex = Math.Min(SelectedIndex + 1, Images.Count - 1);
                    break;
                case Keys.Up:
                    SelectedIndex = Math.Max(SelectedIndex - tilesPerRow, 0);
                    break;
                case Keys.Home:
                    SelectedIndex = 0;
                    break;
                case Keys.End:
                    SelectedIndex = Images.Count - 1;
                    break;
                case Keys.PageUp:
                    SelectedIndex = Math.Max(SelectedIndex - (rowsPerPage * tilesPerRow), 0);
                    break;
                case Keys.PageDown:
                    SelectedIndex = Math.Min(SelectedIndex + (rowsPerPage * tilesPerRow), Images.Count - 1);
                    break;
            }
        }

        private void ScrollToSelected()
        {
            if (selectedIndex == -1) return;
            ChangeSize();
            int actualImageSize = imageSize + 4;
            int tilesPerRow = Math.Max((Width - vScrollBar1.Width) / actualImageSize, 1);
            int y = ((SelectedIndex / tilesPerRow) * actualImageSize) - vScrollBar1.Value;
            if (y < 0)
                vScrollBar1.Value += y;
            if (y + actualImageSize > Height)
                vScrollBar1.Value += (y + actualImageSize) - Height;
        }
    }
}