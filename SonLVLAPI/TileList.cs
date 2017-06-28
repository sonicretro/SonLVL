using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SonicRetro.SonLVL.API
{
	[DefaultEvent("SelectedIndexChanged")]
	public partial class TileList : UserControl
	{
		private int selectedIndex = -1;
		[Browsable(false)]
		public int SelectedIndex
		{
			get { return selectedIndex; }
			set
			{
				bool changed = value != selectedIndex;
				selectedIndex = value;
				ScrollToSelected();
				Invalidate();
				if (changed)
					SelectedIndexChanged(this, EventArgs.Empty);
			}
		}
		public event EventHandler SelectedIndexChanged = delegate { };

		public event EventHandler ItemDrag = delegate { };

		private int imageWidth = 8;
		[DefaultValue(8)]
		public int ImageWidth
		{
			get { return imageWidth; }
			set
			{
				imageWidth = value;
				ChangeSize();
			}
		}

		private int imageHeight = 8;
		[DefaultValue(8)]
		public int ImageHeight
		{
			get { return imageHeight; }
			set
			{
				imageHeight = value;
				ChangeSize();
			}
		}

		[DefaultValue(8)]
		public int ImageSize
		{
			get { return imageWidth == imageHeight ? imageWidth : -1; }
			set
			{
				if (value == -1) return;
				imageWidth = imageHeight = value;
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
					int tilesPerCol = Math.Max((Height - hScrollBar1.Height) / (imageHeight + 4), 1);
					hScrollBar1.Maximum = Math.Max(((int)Math.Ceiling(Images.Count / (double)tilesPerCol) * (imageWidth + 4)) - Width, 0);
					hScrollBar1.SmallChange = hScrollBar1.LargeChange = imageHeight + 4;
					break;
				case Direction.Vertical:
					int tilesPerRow = Math.Max((Width - vScrollBar1.Width) / (imageWidth + 4), 1);
					vScrollBar1.Maximum = Math.Max(((int)Math.Ceiling(Images.Count / (double)tilesPerRow) * (imageHeight + 4)) - Height, 0);
					vScrollBar1.SmallChange = vScrollBar1.LargeChange = imageWidth + 4;
					break;
			}
			Invalidate();
		}

		private void TileList_Resize(object sender, EventArgs e) { ChangeSize(); ScrollToSelected(); }

		private void TileList_Paint(object sender, PaintEventArgs e)
		{
			if (Images.Count == 0) return;
			int actualImageWidth = imageWidth + 4;
			int actualImageHeight = imageHeight + 4;
			switch (Direction)
			{
				case Direction.Horizontal:
					int tilesPerCol = Math.Max((Height - hScrollBar1.Height) / actualImageHeight, 1);
					int numCols = (int)Math.Ceiling(Images.Count / (double)tilesPerCol);
					int stc = hScrollBar1.Value / actualImageWidth;
					int edc = Math.Min((int)Math.Ceiling((hScrollBar1.Value + Width) / (double)actualImageWidth), numCols);
					Graphics g = e.Graphics;
					g.SetOptions();
					g.Clear(BackColor);
					int i = stc * tilesPerCol;
					for (int c = stc; c < edc; c++)
						for (int r = 0; r < tilesPerCol; r++)
						{
							if (i == selectedIndex)
								g.DrawRectangle(new Pen(Color.Yellow, 2), (actualImageWidth * c) + 1 - hScrollBar1.Value, actualImageHeight * r + 1, actualImageWidth - 2, actualImageHeight - 2);
							g.DrawImage(Images[i], (actualImageWidth * c) + 2 - hScrollBar1.Value, (actualImageHeight * r) + 2, imageWidth, imageHeight);
							i++;
							if (i == Images.Count) return;
						}
					break;
				case Direction.Vertical:
					int tilesPerRow = Math.Max((Width - vScrollBar1.Width) / actualImageWidth, 1);
					int numRows = (int)Math.Ceiling(Images.Count / (double)tilesPerRow);
					int str = vScrollBar1.Value / actualImageHeight;
					int edr = Math.Min((int)Math.Ceiling((vScrollBar1.Value + Height) / (double)actualImageHeight), numRows);
					g = e.Graphics;
					g.SetOptions();
					g.Clear(BackColor);
					i = str * tilesPerRow;
					for (int r = str; r < edr; r++)
						for (int c = 0; c < tilesPerRow; c++)
						{
							if (i == selectedIndex)
								g.DrawRectangle(new Pen(Color.Yellow, 2), actualImageWidth * c + 1, (actualImageHeight * r) - vScrollBar1.Value + 1, actualImageWidth - 2, actualImageHeight - 2);
							g.DrawImage(Images[i], (actualImageWidth * c) + 2, (actualImageHeight * r) + 2 - vScrollBar1.Value, imageWidth, imageHeight);
							i++;
							if (i == Images.Count) return;
						}
					break;
			}
		}

		public int GetItemAtPoint(Point point)
		{
			if (Images.Count == 0) return -1;
			int actualImageWidth = imageWidth + 4;
			int actualImageHeight = imageHeight + 4;
			switch (Direction)
			{
				case Direction.Horizontal:
					int tilesPerCol = Math.Max((Height - hScrollBar1.Height) / actualImageHeight, 1);
					int numCols = (int)Math.Ceiling(Images.Count / (double)tilesPerCol);
					int selY = Math.Min(point.Y / actualImageHeight, tilesPerCol);
					int selX = (point.X + hScrollBar1.Value) / actualImageWidth;
					if (selX * tilesPerCol + selY < Images.Count)
						return selX * tilesPerCol + selY;
					break;
				case Direction.Vertical:
					int tilesPerRow = Math.Max((Width - vScrollBar1.Width) / actualImageWidth, 1);
					int numRows = (int)Math.Ceiling(Images.Count / (double)tilesPerRow);
					selX = Math.Min(point.X / actualImageWidth, tilesPerRow);
					selY = (point.Y + vScrollBar1.Value) / actualImageHeight;
					if (selY * tilesPerRow + selX < Images.Count)
						return selY * tilesPerRow + selX;
					break;
			}
			return Images.Count - 1;
		}

		public Rectangle GetItemBounds(int index)
		{
			int actualImageWidth = imageWidth + 4;
			int actualImageHeight = imageHeight + 4;
			switch (Direction)
			{
				case Direction.Horizontal:
					int tilesPerCol = Math.Max((Height - hScrollBar1.Height) / actualImageHeight, 1);
					int r = index % tilesPerCol;
					int c = index / tilesPerCol;
					return new Rectangle((c * actualImageWidth) - hScrollBar1.Value, r * actualImageHeight, actualImageWidth, actualImageHeight);
				case Direction.Vertical:
					int tilesPerRow = Math.Max((Width - vScrollBar1.Width) / actualImageWidth, 1);
					r = index / tilesPerRow;
					c = index % tilesPerRow;
					return new Rectangle(c * actualImageWidth, (r * actualImageHeight) - vScrollBar1.Value, actualImageWidth, actualImageHeight);
			}
			return Rectangle.Empty;
		}

		Rectangle? dragRect;
		private void TileList_MouseDown(object sender, MouseEventArgs e)
		{
			dragRect = null;
			int index = GetItemAtPoint(e.Location);
			if (index == -1) return;
			SelectedIndex = index;
			dragRect = new Rectangle(new Point(e.X - (SystemInformation.DragSize.Width / 2),
				e.Y - (SystemInformation.DragSize.Height / 2)), SystemInformation.DragSize);
		}

		private void TileList_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left && dragRect.HasValue)
				if (!dragRect.Value.Contains(e.Location))
				{
					ItemDrag(this, new EventArgs());
					dragRect = null;
				}
		}

		private void TileList_MouseUp(object sender, MouseEventArgs e)
		{
			dragRect = null;
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
			int actualImageWidth = imageWidth + 4;
			int actualImageHeight = imageHeight + 4;
			switch (Direction)
			{
				case Direction.Horizontal:
					int tilesPerCol = Math.Max((Height - hScrollBar1.Height) / actualImageHeight, 1);
					int colsPerPage = Width / actualImageWidth;
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
					int tilesPerRow = Math.Max((Width - vScrollBar1.Width) / actualImageWidth, 1);
					int rowsPerPage = Height / actualImageHeight;
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

		[Browsable(false)]
		public int ScrollValue
		{
			get
			{
				return direction == API.Direction.Horizontal ? hScrollBar1.Value : vScrollBar1.Value;
			}
			set
			{
				if (direction == API.Direction.Horizontal)
					hScrollBar1.Value = Math.Min(hScrollBar1.Maximum, Math.Max(0, value));
				else
					vScrollBar1.Value = Math.Min(vScrollBar1.Maximum, Math.Max(0, value));
			}
		}

		private void ScrollToSelected()
		{
			if (selectedIndex == -1) return;
			ChangeSize();
			int actualImageWidth = imageWidth + 4;
			int actualImageHeight = imageHeight + 4;
			switch (Direction)
			{
				case Direction.Horizontal:
					int tilesPerCol = Math.Max((Height - hScrollBar1.Height) / actualImageHeight, 1);
					int x = ((SelectedIndex / tilesPerCol) * actualImageWidth) - hScrollBar1.Value;
					if (x < 0)
						hScrollBar1.Value += x;
					if (x + actualImageWidth > Width)
						hScrollBar1.Value += (x + actualImageWidth) - Width;
					break;
				case Direction.Vertical:
					int tilesPerRow = Math.Max((Width - vScrollBar1.Width) / actualImageWidth, 1);
					int y = ((SelectedIndex / tilesPerRow) * actualImageHeight) - vScrollBar1.Value;
					if (y < 0)
						vScrollBar1.Value += y;
					if (y + actualImageHeight > Height)
						vScrollBar1.Value += (y + actualImageHeight) - Height;
					break;
			}
		}

		void TileList_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			ScrollValue -= e.Delta / SystemInformation.MouseWheelScrollDelta * SystemInformation.MouseWheelScrollLines * 16;
			Invalidate();
		}
	}
}
