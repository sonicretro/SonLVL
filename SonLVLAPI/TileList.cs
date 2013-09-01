using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SonicRetro.SonLVL.API
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
        [DefaultValue(8)]
        public int ImageSize
        {
            get { return imageSize; }
            set
            {
                imageSize = value;
                ChangeSize();
            }
        }

        private Direction direction = Direction.Vertical;
        [DefaultValue(Direction.Vertical)]
        public Direction Direction
        {
            get { return direction; }
            set
            {
                direction = value;
                SuspendLayout();
                vScrollBar1.Visible = !(hScrollBar1.Visible = value == API.Direction.Horizontal);
                ResumeLayout();
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
            switch (Direction)
            {
                case Direction.Horizontal:
                    int tilesPerCol = Math.Max((Height - hScrollBar1.Height) / (imageSize + 4), 1);
                    hScrollBar1.Maximum = Math.Max(((int)Math.Ceiling(Images.Count / (double)tilesPerCol) * (imageSize + 4)) - Width, 0);
                    break;
                case Direction.Vertical:
                    int tilesPerRow = Math.Max((Width - vScrollBar1.Width) / (imageSize + 4), 1);
                    vScrollBar1.Maximum = Math.Max(((int)Math.Ceiling(Images.Count / (double)tilesPerRow) * (imageSize + 4)) - Height, 0);
                    break;
            }
            Invalidate();
        }

        private void TileList_Resize(object sender, EventArgs e) { ChangeSize(); }

        private void TileList_Paint(object sender, PaintEventArgs e)
        {
            if (Images.Count == 0) return;
            int actualImageSize = imageSize + 4;
            switch (Direction)
            {
                case Direction.Horizontal:
                    int tilesPerCol = Math.Max((Height - hScrollBar1.Height) / actualImageSize, 1);
                    int numCols = (int)Math.Ceiling(Images.Count / (double)tilesPerCol);
                    int stc = hScrollBar1.Value / actualImageSize;
                    int edc = Math.Min((int)Math.Ceiling((hScrollBar1.Value + Width) / (double)actualImageSize), numCols);
                    Graphics g = e.Graphics;
                    g.SetOptions();
                    g.Clear(BackColor);
                    if (Images.Count == 0) return;
                    int i = stc * tilesPerCol;
                    for (int c = stc; c < edc; c++)
                        for (int r = 0; r < tilesPerCol; r++)
                        {
                            if (i == selectedIndex)
                                g.DrawRectangle(new Pen(Color.Yellow, 2), (actualImageSize * c) - hScrollBar1.Value, actualImageSize * r, actualImageSize - 1, actualImageSize - 1);
                            g.DrawImage(Images[i], (actualImageSize * c) + 2 - hScrollBar1.Value, (actualImageSize * r) + 2, imageSize, imageSize);
                            i++;
                            if (i == Images.Count) return;
                        }
                    break;
                case Direction.Vertical:
                    int tilesPerRow = Math.Max((Width - vScrollBar1.Width) / actualImageSize, 1);
                    int numRows = (int)Math.Ceiling(Images.Count / (double)tilesPerRow);
                    int str = vScrollBar1.Value / actualImageSize;
                    int edr = Math.Min((int)Math.Ceiling((vScrollBar1.Value + Height) / (double)actualImageSize), numRows);
                    g = e.Graphics;
                    g.SetOptions();
                    g.Clear(BackColor);
                    if (Images.Count == 0) return;
                    i = str * tilesPerRow;
                    for (int r = str; r < edr; r++)
                        for (int c = 0; c < tilesPerRow; c++)
                        {
                            if (i == selectedIndex)
                                g.DrawRectangle(new Pen(Color.Yellow, 2), actualImageSize * c, (actualImageSize * r) - vScrollBar1.Value, actualImageSize - 1, actualImageSize - 1);
                            g.DrawImage(Images[i], (actualImageSize * c) + 2, (actualImageSize * r) + 2 - vScrollBar1.Value, imageSize, imageSize);
                            i++;
                            if (i == Images.Count) return;
                        }
                    break;
            }
        }

        private void TileList_MouseDown(object sender, MouseEventArgs e)
        {
            if (Images.Count == 0) return;
            int actualImageSize = imageSize + 4;
            switch (Direction)
            {
                case Direction.Horizontal:
                    int tilesPerCol = Math.Max((Height - hScrollBar1.Height) / actualImageSize, 1);
                    int numCols = (int)Math.Ceiling(Images.Count / (double)tilesPerCol);
                    int selY = e.Y / actualImageSize;
                    if (selY >= tilesPerCol) return;
                    int selX = (e.X + hScrollBar1.Value) / actualImageSize;
                    if (selX * tilesPerCol + selY < Images.Count)
                        SelectedIndex = selX * tilesPerCol + selY;
                    break;
                case Direction.Vertical:
                    int tilesPerRow = Math.Max((Width - vScrollBar1.Width) / actualImageSize, 1);
                    int numRows = (int)Math.Ceiling(Images.Count / (double)tilesPerRow);
                    selX = e.X / actualImageSize;
                    if (selX >= tilesPerRow) return;
                    selY = (e.Y + vScrollBar1.Value) / actualImageSize;
                    if (selY * tilesPerRow + selX < Images.Count)
                        SelectedIndex = selY * tilesPerRow + selX;
                    break;
            }
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
            switch (Direction)
            {
                case Direction.Horizontal:
                    int tilesPerCol = Math.Max((Height - hScrollBar1.Height) / actualImageSize, 1);
                    int colsPerPage = Width / actualImageSize;
                    switch (e.KeyCode)
                    {
                        case Keys.Down:
                            SelectedIndex = Math.Min(SelectedIndex + 1, Images.Count - 1);
                            break;
                        case Keys.Left:
                            SelectedIndex = Math.Max(SelectedIndex - tilesPerCol, 0);
                            break;
                        case Keys.Right:
                            SelectedIndex = Math.Min(SelectedIndex + tilesPerCol, Images.Count - 1);
                            break;
                        case Keys.Up:
                            SelectedIndex = Math.Max(SelectedIndex - 1, 0);
                            break;
                        case Keys.Home:
                            SelectedIndex = 0;
                            break;
                        case Keys.End:
                            SelectedIndex = Images.Count - 1;
                            break;
                        case Keys.PageUp:
                            SelectedIndex = Math.Max(SelectedIndex - (colsPerPage * tilesPerCol), 0);
                            break;
                        case Keys.PageDown:
                            SelectedIndex = Math.Min(SelectedIndex + (colsPerPage * tilesPerCol), Images.Count - 1);
                            break;
                    }
                    break;
                case Direction.Vertical:
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
                    break;
            }
        }

        private void ScrollToSelected()
        {
            if (selectedIndex == -1) return;
            ChangeSize();
            int actualImageSize = imageSize + 4;
            switch (Direction)
            {
                case Direction.Horizontal:
                    int tilesPerCol = Math.Max((Height - hScrollBar1.Height) / actualImageSize, 1);
                    int x = ((SelectedIndex / tilesPerCol) * actualImageSize) - hScrollBar1.Value;
                    if (x < 0)
                        hScrollBar1.Value += x;
                    if (x + actualImageSize > Width)
                        hScrollBar1.Value += (x + actualImageSize) - Width;
                    break;
                case Direction.Vertical:
                    int tilesPerRow = Math.Max((Width - vScrollBar1.Width) / actualImageSize, 1);
                    int y = ((SelectedIndex / tilesPerRow) * actualImageSize) - vScrollBar1.Value;
                    if (y < 0)
                        vScrollBar1.Value += y;
                    if (y + actualImageSize > Height)
                        vScrollBar1.Value += (y + actualImageSize) - Height;
                    break;
            }
        }
    }
}