using SonicRetro.SonLVL.API;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace S1SSEdit
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		const int gridsize = 24;
		List<byte> objids = new List<byte>();
		List<LayoutSection> layoutSections = new List<LayoutSection>();
		List<Bitmap> layoutSectionImages = new List<Bitmap>();
		internal byte[,] layout = new byte[0x40, 0x40];
		string filename = null;
		byte fgobj = 1;
		byte bgobj = 0;
		Graphics layoutgfx;
		ImageAttributes imageTransparency = new ImageAttributes();
		Tool tool = Tool.Pencil;
		ShapeMode rectmode = ShapeMode.Edge;
		ShapeMode diammode = ShapeMode.Edge;
		ShapeMode ovalmode = ShapeMode.Edge;
		Stack<Action> undoList = new Stack<Action>();
		Stack<Action> redoList = new Stack<Action>();
		int lastSaveUndoCount = 0;
		bool drawing = false;
		Point firstloc, prevloc;
		Rectangle selection;
		List<ObjLoc> drawlist;
		byte?[,] drawrect;
		byte[,] fillrect;

		private void MainForm_Load(object sender, EventArgs e)
		{
			imageTransparency.SetColorMatrix(new ColorMatrix() { Matrix33 = 0.75f }, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
			LayoutDrawer.Init();
			int x = 0;
			int y = 0;
			BitmapBits bmp = new BitmapBits(objectPalette.Size);
			foreach (var item in LayoutDrawer.ObjectBmps)
			{
				objids.Add(item.Key);
				bmp.DrawBitmap(item.Value, x * 24, y * 24);
				if (++x == 16)
				{
					x = 0;
					++y;
				}
			}
			objectPalette.BackgroundImage = bmp.ToBitmap(LayoutDrawer.Palette);
			foreObjPicture.Image = LayoutDrawer.ObjectBmps[1].ToBitmap(LayoutDrawer.Palette).To32bpp();
			backObjPicture.Image = new Bitmap(24, 24);
			if (File.Exists("LayoutSections.sls"))
				layoutSections = DeserializeCompressed<List<LayoutSection>>("LayoutSections.sls");
			layoutSectionListBox.Items.Clear();
			layoutSectionListBox.BeginUpdate();
			foreach (LayoutSection sec in layoutSections)
			{
				layoutSectionListBox.Items.Add(sec.Name);
				layoutSectionImages.Add(MakeLayoutSectionImage(sec));
			}
			layoutSectionListBox.EndUpdate();
			layoutgfx = layoutPanel.CreateGraphics();
			layoutgfx.SetOptions();
		}

		private static T DeserializeCompressed<T>(string fn)
		{
			using (FileStream fs = File.OpenRead(fn))
			using (DeflateStream ds = new DeflateStream(fs, CompressionMode.Decompress))
				return (T)new BinaryFormatter().Deserialize(ds);
		}

		private Bitmap MakeLayoutSectionImage(LayoutSection section)
		{
			return LayoutDrawer.DrawLayout(section.Layout).ToBitmap(LayoutDrawer.Palette).To32bpp();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (undoList.Count != lastSaveUndoCount)
				switch (MessageBox.Show(this, "Do you want to save before exiting?", "S1SSEdit", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning))
				{
					case DialogResult.Cancel:
						e.Cancel = true;
						break;
					case DialogResult.Yes:
						saveToolStripMenuItem_Click(this, EventArgs.Empty);
						break;
				}
		}

		private void UpdateText()
		{
			StringBuilder sb = new StringBuilder("S1SSEdit - ");
			if (filename == null)
				sb.Append("New Stage");
			else
				sb.Append(Path.GetFileName(filename));
			if (undoList.Count != lastSaveUndoCount)
				sb.Append(" *");
			Text = sb.ToString();
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(this, $"Unload the current stage and start a new one?", "S1SSEdit", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.OK)
			{
				layout = new byte[0x40, 0x40];
				filename = null;
				LoadFile();
			}
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (undoList.Count != lastSaveUndoCount)
				switch (MessageBox.Show(this, "Do you want to save the current file?", "S1SSEdit", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning))
				{
					case DialogResult.Cancel:
						return;
					case DialogResult.Yes:
						saveToolStripMenuItem_Click(this, EventArgs.Empty);
						break;
				}
			using (OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "bin", Filter = "Binary Files|*.bin|All Files|*.*" })
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					filename = dlg.FileName;
					layout = new byte[0x40, 0x40];
					byte[] tmp = Compression.Decompress(filename, CompressionType.Enigma);
					int i = 0;
					for (int y = 0; y < 0x40; y++)
						for (int x = 0; x < 0x40; x++)
							layout[x, y] = tmp[i++];
					LoadFile();
				}
		}

		private void LoadFile()
		{
			lastSaveUndoCount = 0;
			undoList.Clear();
			redoList.Clear();
			undoToolStripMenuItem.DropDownItems.Clear();
			undoToolStripMenuItem.Enabled = false;
			redoToolStripMenuItem.DropDownItems.Clear();
			redoToolStripMenuItem.Enabled = false;
			if (saveUndoHistoryToolStripMenuItem.Checked && filename != null && File.Exists(Path.ChangeExtension(filename, ".undo")))
			{
				undoList = DeserializeCompressed<Stack<Action>>(Path.ChangeExtension(filename, ".undo"));
				foreach (Action a in undoList)
					undoToolStripMenuItem.DropDownItems.Add(a.Name);
				lastSaveUndoCount = undoList.Count;
				if (undoList.Count > 0)
					undoToolStripMenuItem.Enabled = true;
			}
			UpdateText();
			DrawLayout();
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (filename == null)
				saveAsToolStripMenuItem_Click(sender, e);
			else
				SaveLayout();
		}

		private void SaveLayout()
		{
			byte[] tmp = new byte[0x40 * 0x40];
			int i = 0;
			for (int y = 0; y < 0x40; y++)
				for (int x = 0; x < 0x40; x++)
					tmp[i++] = layout[x, y];
			Compression.Compress(tmp, filename, CompressionType.Enigma);
			if (saveUndoHistoryToolStripMenuItem.Checked)
			{
				string fn = Path.ChangeExtension(filename, ".undo");
				if (undoList.Count > 0)
					SerializeCompressed(fn, undoList);
				else if (File.Exists(fn))
					File.Delete(fn);
			}
			lastSaveUndoCount = undoList.Count;
			UpdateText();
		}

		private static void SerializeCompressed(string fn, object obj)
		{
			using (FileStream fs = File.Create(fn))
			using (DeflateStream ds = new DeflateStream(fs, CompressionMode.Compress))
				new BinaryFormatter().Serialize(ds, obj);
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "bin", Filter = "Binary Files|*.bin|All Files|*.*", FileName = filename ?? "New Stage.bin" })
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					filename = dlg.FileName;
					SaveLayout();
				}
		}

		private void exportImageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "png", Filter = "PNG Files|*.png" })
				if (dlg.ShowDialog(this) == DialogResult.OK)
					using (Bitmap bmp = LayoutDrawer.DrawLayout(layout).ToBitmap(LayoutDrawer.Palette))
						bmp.Save(dlg.FileName);
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void undoToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			DoUndo(undoToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem) + 1);
		}

		private void redoToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			DoRedo(redoToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem) + 1);
		}

		private void DoUndo(int count = 1)
		{
			if (drawing) return;
			for (int i = 0; i < count; i++)
			{
				undoToolStripMenuItem.DropDownItems.RemoveAt(0);
				Action act = undoList.Pop();
				act.Do(layout);
				redoList.Push(act);
				redoToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem(act.Name));
				redoToolStripMenuItem.Enabled = true;
			}
			if (undoList.Count == 0)
				undoToolStripMenuItem.Enabled = false;
			UpdateText();
			DrawLayout();
		}

		private void DoRedo(int count = 1)
		{
			if (drawing) return;
			for (int i = 0; i < count; i++)
			{
				redoToolStripMenuItem.DropDownItems.RemoveAt(0);
				Action act = redoList.Pop();
				act.Do(layout);
				undoList.Push(act);
				undoToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem(act.Name));
				undoToolStripMenuItem.Enabled = true;
			}
			if (redoList.Count == 0)
				redoToolStripMenuItem.Enabled = false;
			UpdateText();
			DrawLayout();
		}

		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Y:
					if (e.Control && !drawing && redoList.Count > 0)
					{
						DoRedo();
						e.SuppressKeyPress = true;
					}

					break;
				case Keys.Z:
					if (e.Control && !drawing && undoList.Count > 0)
					{
						DoUndo();
						e.SuppressKeyPress = true;
					}

					break;
			}
		}

#region Object Palette
		// Also handles backObjPicture.Click
		private void foreObjPicture_Click(object sender, EventArgs e)
		{
			byte a = fgobj;
			fgobj = bgobj;
			bgobj = a;
			Image b = foreObjPicture.Image;
			foreObjPicture.Image = backObjPicture.Image;
			backObjPicture.Image = b;
		}

		private void objectPalette_MouseDown(object sender, MouseEventArgs e)
		{
			byte type = objids[(e.Y / 24) * 16 + e.X / 24];
			Bitmap pic = LayoutDrawer.ObjectBmps[type].ToBitmap(LayoutDrawer.Palette).To32bpp();
			switch (e.Button)
			{
				case MouseButtons.Left:
					fgobj = type;
					foreObjPicture.Image = pic;
					break;
				case MouseButtons.Right:
					bgobj = type;
					backObjPicture.Image = pic;
					break;
			}
		}
#endregion

#region Tool Palette
		private void selectButton_CheckedChanged(object sender, EventArgs e)
		{
			if (selectButton.Checked)
			{
				tool = Tool.Select;
				selection = Rectangle.Empty;
				toolOptionsPanel.Invalidate();
			}
		}

		private void pencilButton_CheckedChanged(object sender, EventArgs e)
		{
			if (pencilButton.Checked)
			{
				tool = Tool.Pencil;
				toolOptionsPanel.Invalidate();
			}
		}

		private void fillButton_CheckedChanged(object sender, EventArgs e)
		{
			if (fillButton.Checked)
			{
				tool = Tool.Fill;
				toolOptionsPanel.Invalidate();
			}
		}

		private void lineButton_CheckedChanged(object sender, EventArgs e)
		{
			if (lineButton.Checked)
			{
				tool = Tool.Line;
				toolOptionsPanel.Invalidate();
			}
		}

		private void rectangleButton_CheckedChanged(object sender, EventArgs e)
		{
			if (rectangleButton.Checked)
			{
				tool = Tool.Rectangle;
				toolOptionsPanel.Invalidate();
			}
		}

		private void diamondButton_CheckedChanged(object sender, EventArgs e)
		{
			if (diamondButton.Checked)
			{
				tool = Tool.Diamond;
				toolOptionsPanel.Invalidate();
			}
		}

		private void ovalButton_CheckedChanged(object sender, EventArgs e)
		{
			if (ovalButton.Checked)
			{
				tool = Tool.Oval;
				toolOptionsPanel.Invalidate();
			}
		}

		private void toolOptionsPanel_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.Clear(toolOptionsPanel.BackColor);
			ShapeMode mode;
			switch (tool)
			{
				case Tool.Rectangle:
					mode = rectmode;
					break;
				case Tool.Diamond:
					mode = diammode;
					break;
				case Tool.Oval:
					mode = ovalmode;
					break;
				default:
					return;
			}
			e.Graphics.FillRectangle(SystemBrushes.Highlight, 1, 2 + 20 * (int)mode, 36, 18);
			e.Graphics.DrawRectangle(mode == ShapeMode.Edge ? SystemPens.HighlightText : SystemPens.ControlText, 5, 6, 27, 9);
			e.Graphics.DrawRectangle(mode == ShapeMode.FillEdge ? SystemPens.HighlightText : SystemPens.ControlText, 5, 26, 27, 9);
			e.Graphics.FillRectangle(Brushes.DarkGray, 6, 27, 26, 8);
			e.Graphics.FillRectangle(Brushes.DarkGray, 5, 46, 28, 10);
		}

		private void toolOptionsPanel_MouseClick(object sender, MouseEventArgs e)
		{
			switch (tool)
			{
				case Tool.Rectangle:
					if (e.Y <= 20)
						rectmode = ShapeMode.Edge;
					else if (e.Y <= 40)
						rectmode = ShapeMode.FillEdge;
					else
						rectmode = ShapeMode.Fill;
					break;
				case Tool.Diamond:
					if (e.Y <= 20)
						diammode = ShapeMode.Edge;
					else if (e.Y <= 40)
						diammode = ShapeMode.FillEdge;
					else
						diammode = ShapeMode.Fill;
					break;
				case Tool.Oval:
					if (e.Y <= 20)
						ovalmode = ShapeMode.Edge;
					else if (e.Y <= 40)
						ovalmode = ShapeMode.FillEdge;
					else
						ovalmode = ShapeMode.Fill;
					break;
				default:
					return;
			}
			toolOptionsPanel.Invalidate();
		}
#endregion

		private void layoutPanel_Paint(object sender, PaintEventArgs e)
		{
			DrawLayout();
		}

		private void DrawLayout()
		{
			Point gridloc = layoutPanel.PointToClient(Cursor.Position);
			gridloc = new Point(gridloc.X / gridsize, gridloc.Y / gridsize);
			byte[,] tmplayout = (byte[,])layout.Clone();
			if (drawing)
				switch (tool)
				{
					case Tool.Pencil:
					case Tool.Line:
						foreach (ObjLoc loc in drawlist)
							tmplayout[loc.X, loc.Y] = loc.Object;
						break;
					case Tool.Rectangle:
						gridloc = new Point(Math.Min(gridloc.X, firstloc.X / gridsize), Math.Min(gridloc.Y, firstloc.Y / gridsize));
						switch (rectmode)
						{
							case ShapeMode.Edge:
								foreach (ObjLoc loc in drawlist)
									tmplayout[loc.X, loc.Y] = loc.Object;
								break;
							case ShapeMode.FillEdge:
							case ShapeMode.Fill:
								for (int y = 0; y < fillrect.GetLength(1); y++)
									for (int x = 0; x < fillrect.GetLength(0); x++)
									{
										int px = x + gridloc.X;
										int py = y + gridloc.Y;
										if (px >= 0 && px < 0x40 && py >= 0 && py < 0x40)
											tmplayout[x + gridloc.X, y + gridloc.Y] = fillrect[x, y];
									}
								break;
						}
						break;
					case Tool.Diamond:
						gridloc = new Point(Math.Min(gridloc.X, firstloc.X / gridsize), Math.Min(gridloc.Y, firstloc.Y / gridsize));
						switch (diammode)
						{
							case ShapeMode.Edge:
								foreach (ObjLoc loc in drawlist)
									tmplayout[loc.X, loc.Y] = loc.Object;
								break;
							case ShapeMode.FillEdge:
							case ShapeMode.Fill:
								for (int y = 0; y < drawrect.GetLength(1); y++)
									for (int x = 0; x < drawrect.GetLength(0); x++)
									{
										int px = x + gridloc.X;
										int py = y + gridloc.Y;
										if (drawrect[x, y].HasValue && px >= 0 && px < 0x40 && py >= 0 && py < 0x40)
											tmplayout[x + gridloc.X, y + gridloc.Y] = drawrect[x, y].Value;
									}
								break;
						}
						break;
					case Tool.Oval:
						gridloc = new Point(Math.Min(gridloc.X, firstloc.X / gridsize), Math.Min(gridloc.Y, firstloc.Y / gridsize));
						for (int y = 0; y < drawrect.GetLength(1); y++)
							for (int x = 0; x < drawrect.GetLength(0); x++)
							{
								int px = x + gridloc.X;
								int py = y + gridloc.Y;
								if (drawrect[x, y].HasValue && px >= 0 && px < 0x40 && py >= 0 && py < 0x40)
									tmplayout[x + gridloc.X, y + gridloc.Y] = drawrect[x, y].Value;
							}
						break;
				}
			BitmapBits layoutbmp = LayoutDrawer.DrawLayout(tmplayout);
			LayoutDrawer.Palette.Entries[0] = SystemColors.Control;
			using (Bitmap bmp = layoutbmp.ToBitmap(LayoutDrawer.Palette).To32bpp())
			{
				Graphics gfx = Graphics.FromImage(bmp);
				if (tool == Tool.Select)
				{
					if (!selection.IsEmpty)
					{
						Rectangle selbnds = new Rectangle(selection.X * gridsize, selection.Y * gridsize, selection.Width * gridsize, selection.Height * gridsize);
						using (SolidBrush brush = new SolidBrush(Color.FromArgb(128, SystemColors.Highlight)))
							gfx.FillRectangle(brush, selbnds);
						selbnds.Width--; selbnds.Height--;
						using (Pen pen = new Pen(Color.FromArgb(128, Color.Black)) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot })
							gfx.DrawRectangle(pen, selbnds);
					}
				}
				else if (!drawing)
					gfx.DrawImage(foreObjPicture.Image, new Rectangle(gridloc.X * gridsize, gridloc.Y * gridsize, 24, 24), 0, 0, 24, 24, GraphicsUnit.Pixel, imageTransparency);
				layoutgfx.DrawImage(bmp, 0, 0, layoutPanel.Width, layoutPanel.Height);
			}
			LayoutDrawer.Palette.Entries[0] = Color.Transparent;
		}

		private void DoAction(Action action)
		{
			if (redoList.Count > 0)
			{
				redoList.Clear();
				redoToolStripMenuItem.DropDownItems.Clear();
				redoToolStripMenuItem.Enabled = false;
				if (lastSaveUndoCount > undoList.Count)
					lastSaveUndoCount = -1;
			}
			undoList.Push(action);
			undoToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem(action.Name));
			undoToolStripMenuItem.Enabled = true;
			action.Do(layout);
			UpdateText();
		}

		private void layoutPanel_MouseDown(object sender, MouseEventArgs e)
		{
			Point loc = e.Location;
			Point gridloc = new Point(loc.X / gridsize, loc.Y / gridsize);
			if (tool == Tool.Select)
			{
				if (e.Button == MouseButtons.Left)
				{
					drawing = true;
					selection = new Rectangle(gridloc.X, gridloc.Y, 1, 1);
					firstloc = prevloc = loc;
					DrawLayout();
				}
				return;
			}
			drawing = true;
			byte obj = 0;
			switch (e.Button)
			{
				case MouseButtons.Left:
					obj = fgobj;
					break;
				case MouseButtons.Right:
					obj = bgobj;
					break;
				default:
					return;
			}
			switch (tool)
			{
				case Tool.Pencil:
				case Tool.Line:
					drawlist = new List<ObjLoc>() { new ObjLoc(obj, gridloc) };
					break;
				case Tool.Fill:
					{
						drawing = false;
						byte oldind = layout[gridloc.X, gridloc.Y];
						if (oldind == obj) return;
						Queue<Point> pts = new Queue<Point>(0x40 * 0x40 / 2);
						pts.Enqueue(gridloc);
						byte?[,] fillgrid = new byte?[0x40, 0x40];
						fillgrid[gridloc.X, gridloc.Y] = obj;
						while (pts.Count > 0)
						{
							Point pt = pts.Dequeue();
							int tmp = pt.X - 1;
							if (tmp != -1 && layout[tmp, pt.Y] == oldind && !fillgrid[tmp, pt.Y].HasValue)
							{
								fillgrid[tmp, pt.Y] = obj;
								pts.Enqueue(new Point(tmp, pt.Y));
							}
							tmp = pt.X + 1;
							if (tmp != 0x40 && layout[tmp, pt.Y] == oldind && !fillgrid[tmp, pt.Y].HasValue)
							{
								fillgrid[tmp, pt.Y] = obj;
								pts.Enqueue(new Point(tmp, pt.Y));
							}
							tmp = pt.Y - 1;
							if (tmp != -1 && layout[pt.X, tmp] == oldind && !fillgrid[pt.X, tmp].HasValue)
							{
								fillgrid[pt.X, tmp] = obj;
								pts.Enqueue(new Point(pt.X, tmp));
							}
							tmp = pt.Y + 1;
							if (tmp != 0x40 && layout[pt.X, tmp] == oldind && !fillgrid[pt.X, tmp].HasValue)
							{
								fillgrid[pt.X, tmp] = obj;
								pts.Enqueue(new Point(pt.X, tmp));
							}
						}
						DoAction(new FillAction(fillgrid));
					}
					break;
				case Tool.Rectangle:
					switch (rectmode)
					{
						case ShapeMode.Edge:
							drawlist = new List<ObjLoc>() { new ObjLoc(obj, gridloc) };
							break;
						case ShapeMode.FillEdge:
						case ShapeMode.Fill:
							fillrect = new byte[1, 1] { { obj } };
							break;
					}
					break;
				case Tool.Diamond:
					switch (diammode)
					{
						case ShapeMode.Edge:
							drawlist = new List<ObjLoc>() { new ObjLoc(obj, gridloc) };
							break;
						case ShapeMode.FillEdge:
						case ShapeMode.Fill:
							drawrect = new byte?[1, 1] { { obj } };
							break;
					}
					break;
				case Tool.Oval:
					drawrect = new byte?[1, 1] { { obj } };
					break;
			}
			firstloc = prevloc = loc;
			DrawLayout();
		}

		private void DrawLine(byte obj, int x1, int y1, int x2, int y2)
		{
			if (y1 == y2)
			{
				if (y1 < 0 || y1 >= 0x40) return;
				if (x1 > x2)
				{
					int tmp = x1;
					x1 = x2;
					x2 = tmp;
				}
				for (int x = Math.Max(0, x1); x <= Math.Min(0x3F, x2); x++)
				{
					ObjLoc s = new ObjLoc(obj, x, y1);
					if (!drawlist.Contains(s))
						drawlist.Add(s);
				}
			}
			else if (x1 == x2)
			{
				if (x1 < 0 || x1 >= 0x40) return;
				if (y1 > y2)
				{
					int tmp = y1;
					y1 = y2;
					y2 = tmp;
				}
				for (int y = Math.Max(0, y1); y <= Math.Min(0x3F, y2); y++)
				{
					ObjLoc s = new ObjLoc(obj, x1, y);
					if (!drawlist.Contains(s))
						drawlist.Add(s);
				}
			}
			else
			{
				bool steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);
				if (steep)
				{
					int tmp = x1;
					x1 = y1;
					y1 = tmp;
					tmp = x2;
					x2 = y2;
					y2 = tmp;
				}
				if (x1 > x2)
				{
					int tmp = x1;
					x1 = x2;
					x2 = tmp;
					tmp = y1;
					y1 = y2;
					y2 = tmp;
				}
				int deltax = x2 - x1;
				int deltay = Math.Abs(y2 - y1);
				double error = 0;
				double deltaerr = deltay / (double)deltax;
				int ystep;
				int y = y1;
				if (y1 < y2)
					ystep = 1;
				else
					ystep = -1;
				for (int x = x1; x <= x2; x++)
				{
					if (x >= 0 && x < 0x40 && y >= 0 && y < 0x40)
					{
						ObjLoc s;
						if (steep)
							s = new ObjLoc(obj, y, x);
						else
							s = new ObjLoc(obj, x, y);
						if (!drawlist.Contains(s))
							drawlist.Add(s);
					}
					error += deltaerr;
					if (error >= 0.5)
					{
						y += ystep;
						error -= 1.0;
					}
				}
			}
		}

		private void DrawLine(byte?[,] rect, byte obj, int x1, int y1, int x2, int y2)
		{
			if (y1 == y2)
			{
				if (x1 > x2)
				{
					int tmp = x1;
					x1 = x2;
					x2 = tmp;
				}
				for (int x = x1; x <= x2; x++)
					rect[x, y1] = obj;
			}
			else if (x1 == x2)
			{
				if (y1 > y2)
				{
					int tmp = y1;
					y1 = y2;
					y2 = tmp;
				}
				for (int y = y1; y <= y2; y++)
					rect[x1, y] = obj;
			}
			else
			{
				bool steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);
				if (steep)
				{
					int tmp = x1;
					x1 = y1;
					y1 = tmp;
					tmp = x2;
					x2 = y2;
					y2 = tmp;
				}
				if (x1 > x2)
				{
					int tmp = x1;
					x1 = x2;
					x2 = tmp;
					tmp = y1;
					y1 = y2;
					y2 = tmp;
				}
				int deltax = x2 - x1;
				int deltay = Math.Abs(y2 - y1);
				double error = 0;
				double deltaerr = deltay / (double)deltax;
				int ystep;
				int y = y1;
				if (y1 < y2)
					ystep = 1;
				else
					ystep = -1;
				for (int x = x1; x <= x2; x++)
				{
					if (steep)
						rect[y, x] = obj;
					else
						rect[x, y] = obj;
					error += deltaerr;
					if (error >= 0.5)
					{
						y += ystep;
						error -= 1.0;
					}
				}
			}
		}

		private void layoutPanel_MouseMove(object sender, MouseEventArgs e)
		{
			Point loc = e.Location;
			if (!drawing)
			{
				DrawLayout();
				prevloc = loc;
				return;
			}
			Point gridloc = new Point(loc.X / gridsize, loc.Y / gridsize);
			if (gridloc == new Point(prevloc.X / gridsize, prevloc.Y / gridsize))
				return;
			byte obj = 0;
			byte bgsphere = 0;
			switch (e.Button)
			{
				case MouseButtons.Left:
					obj = fgobj;
					bgsphere = this.bgobj;
					break;
				case MouseButtons.Right:
					obj = this.bgobj;
					bgsphere = fgobj;
					break;
			}
			switch (tool)
			{
				case Tool.Select:
					selection = new Rectangle(
						Math.Min(gridloc.X, firstloc.X / gridsize),
						Math.Min(gridloc.Y, firstloc.Y / gridsize),
						Math.Abs(gridloc.X - firstloc.X / gridsize) + 1,
						Math.Abs(gridloc.Y - firstloc.Y / gridsize) + 1);
					break;
				case Tool.Pencil:
					{
						int x1 = prevloc.X;
						int y1 = prevloc.Y;
						int x2 = loc.X;
						int y2 = loc.Y;
						if (y1 == y2)
						{
							if (y1 >= 0x40 * gridsize || y1 < 0)
								return;
							if (x1 > x2)
							{
								int tmp = x1;
								x1 = x2;
								x2 = tmp;
							}
							if (x1 >= 0x40 * gridsize || x2 < 0)
								return;
							x1 = Math.Max(x1, 0);
							x2 = Math.Min(x2, 0x40 * gridsize - 1);
							for (int x = x1; x <= x2; x++)
							{
								ObjLoc s = new ObjLoc(obj, x / gridsize, y1 / gridsize);
								if (!drawlist.Contains(s))
									drawlist.Add(s);
							}
						}
						else if (x1 == x2)
						{
							if (x1 >= 0x40 * gridsize || x1 < 0)
								return;
							if (y1 > y2)
							{
								int tmp = y1;
								y1 = y2;
								y2 = tmp;
							}
							if (y1 >= 0x40 * gridsize || y2 < 0)
								return;
							y1 = Math.Max(y1, 0);
							y2 = Math.Min(y2, 0x40 * gridsize - 1);
							for (int y = y1; y <= y2; y++)
							{
								ObjLoc s = new ObjLoc(obj, x1 / gridsize, y / gridsize);
								if (!drawlist.Contains(s))
									drawlist.Add(s);
							}
						}
						else
						{
							bool steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);
							if (steep)
							{
								int tmp = x1;
								x1 = y1;
								y1 = tmp;
								tmp = x2;
								x2 = y2;
								y2 = tmp;
							}
							if (x1 > x2)
							{
								int tmp = x1;
								x1 = x2;
								x2 = tmp;
								tmp = y1;
								y1 = y2;
								y2 = tmp;
							}
							int deltax = x2 - x1;
							int deltay = Math.Abs(y2 - y1);
							double error = 0;
							double deltaerr = (double)deltay / (double)deltax;
							int ystep;
							int y = y1;
							if (y1 < y2)
								ystep = 1;
							else
								ystep = -1;
							for (int x = x1; x <= x2; x++)
							{
								if (x >= 0 && x < 0x40 * gridsize && y >= 0 && y < 0x40 * gridsize)
								{
									ObjLoc s;
									if (steep)
										s = new ObjLoc(obj, y / gridsize, x / gridsize);
									else
										s = new ObjLoc(obj, x / gridsize, y / gridsize);
									if (!drawlist.Contains(s))
										drawlist.Add(s);
								}
								error += deltaerr;
								if (error >= 0.5)
								{
									y += ystep;
									error -= 1.0;
								}
							}
						}
					}
					break;
				case Tool.Fill:
					return;
				case Tool.Line:
					drawlist = new List<ObjLoc>();
					DrawLine(obj, firstloc.X / gridsize, firstloc.Y / gridsize, gridloc.X, gridloc.Y);
					break;
				case Tool.Rectangle:
					{
						int width = Math.Abs(gridloc.X - firstloc.X / gridsize) + 1;
						int height = Math.Abs(gridloc.Y - firstloc.Y / gridsize) + 1;
						if (ModifierKeys == Keys.Control)
							height = width = Math.Max(width, height);
						switch (rectmode)
						{
							case ShapeMode.Edge:
								{
									drawlist = new List<ObjLoc>();
									int x = Math.Min(gridloc.X, firstloc.X / gridsize);
									int y = Math.Min(gridloc.Y, firstloc.Y / gridsize);
									DrawLine(obj, x, y, x + width - 1, y);
									DrawLine(obj, x, y, x, y + height - 1);
									DrawLine(obj, x, y + height - 1, x + width - 1, y + height - 1);
									DrawLine(obj, x + width - 1, y, x + width - 1, y + height - 1);
								}
								break;
							case ShapeMode.FillEdge:
								fillrect = new byte[width, height];
								if (obj != 0)
								{
									for (int x = 0; x < width; x++)
									{
										fillrect[x, 0] = obj;
										fillrect[x, height - 1] = obj;
									}
									for (int y = 1; y < height - 1; y++)
									{
										fillrect[0, y] = obj;
										fillrect[width - 1, y] = obj;
									}
								}
								if (bgsphere != 0)
									for (int y = 1; y < height - 1; y++)
										for (int x = 1; x < width - 1; x++)
											fillrect[x, y] = bgsphere;
								break;
							case ShapeMode.Fill:
								fillrect = new byte[width, height];
								if (obj != 0)
									for (int y = 0; y < height; y++)
										for (int x = 0; x < width; x++)
											fillrect[x, y] = obj;
								break;
						}
					}
					break;
				case Tool.Diamond:
					{
						int width = Math.Abs(gridloc.X - firstloc.X / gridsize) + 1;
						int height = Math.Abs(gridloc.Y - firstloc.Y / gridsize) + 1;
						if (ModifierKeys == Keys.Control)
							height = width = Math.Max(width, height);
						switch (diammode)
						{
							case ShapeMode.Edge:
								{
									drawlist = new List<ObjLoc>();
									int x = Math.Min(gridloc.X, firstloc.X / gridsize);
									int y = Math.Min(gridloc.Y, firstloc.Y / gridsize);
									DrawLine(obj, x + width / 2, y, x + width - 1, y + height / 2);
									DrawLine(obj, x + width - 1, y + height / 2, x + width / 2, y + height - 1);
									DrawLine(obj, x + width / 2, y + height - 1, x, y + height / 2);
									DrawLine(obj, x, y + height / 2, x + width / 2, y);
								}
								break;
							case ShapeMode.FillEdge:
								drawrect = new byte?[width, height];
								DrawLine(drawrect, obj, width / 2, 0, width - 1, height / 2);
								DrawLine(drawrect, obj, width - 1, height / 2, width / 2, height - 1);
								DrawLine(drawrect, obj, width / 2, height - 1, 0, height / 2);
								DrawLine(drawrect, obj, 0, height / 2, width / 2, 0);
								for (int y = 0; y < height; y++)
								{
									int minX = int.MaxValue;
									int maxX = int.MinValue;
									for (int x = 0; x < width; x++)
										if (drawrect[x, y].HasValue)
										{
											minX = Math.Min(minX, x);
											maxX = Math.Max(maxX, x);
										}
									for (int x = minX + 1; x < maxX; x++)
										if (!drawrect[x, y].HasValue)
											drawrect[x, y] = bgsphere;
								}
								break;
							case ShapeMode.Fill:
								drawrect = new byte?[width, height];
								DrawLine(drawrect, obj, width / 2, 0, width - 1, height / 2);
								DrawLine(drawrect, obj, width - 1, height / 2, width / 2, height - 1);
								DrawLine(drawrect, obj, width / 2, height - 1, 0, height / 2);
								DrawLine(drawrect, obj, 0, height / 2, width / 2, 0);
								for (int y = 0; y < height; y++)
								{
									int minX = int.MaxValue;
									int maxX = int.MinValue;
									for (int x = 0; x < width; x++)
										if (drawrect[x, y].HasValue)
										{
											minX = Math.Min(minX, x);
											maxX = Math.Max(maxX, x);
										}
									for (int x = minX + 1; x < maxX; x++)
										drawrect[x, y] = obj;
								}
								break;
						}
					}
					break;
				case Tool.Oval:
					{
						int width = Math.Abs(gridloc.X - firstloc.X / gridsize) + 1;
						int height = Math.Abs(gridloc.Y - firstloc.Y / gridsize) + 1;
						if (ModifierKeys == Keys.Control)
							height = width = Math.Max(width, height);
						drawrect = new byte?[width, height];
						for (double a = 0; a < 2 * Math.PI; a += 0.04)
						{
							int x = (int)(Math.Cos(a) * (width / 2.01) + (width / 2.0));
							int y = (int)(Math.Sin(a) * (height / 2.01) + (height / 2.0));
							drawrect[x, y] = obj;
						}
						switch (ovalmode)
						{
							case ShapeMode.FillEdge:
								for (int y = 0; y < height; y++)
								{
									int minX = int.MaxValue;
									int maxX = int.MinValue;
									for (int x = 0; x < width; x++)
										if (drawrect[x, y].HasValue)
										{
											minX = Math.Min(minX, x);
											maxX = Math.Max(maxX, x);
										}
									for (int x = minX + 1; x < maxX; x++)
										if (!drawrect[x, y].HasValue)
											drawrect[x, y] = bgsphere;
								}
								break;
							case ShapeMode.Fill:
								for (int y = 0; y < height; y++)
								{
									int minX = int.MaxValue;
									int maxX = int.MinValue;
									for (int x = 0; x < width; x++)
										if (drawrect[x, y].HasValue)
										{
											minX = Math.Min(minX, x);
											maxX = Math.Max(maxX, x);
										}
									for (int x = minX + 1; x < maxX; x++)
										drawrect[x, y] = obj;
								}
								break;
						}
					}
					break;
			}
			prevloc = loc;
			DrawLayout();
		}

		private void layoutPanel_MouseUp(object sender, MouseEventArgs e)
		{
			if (tool == Tool.Select && e.Button == MouseButtons.Right)
			{
				pasteOnceToolStripMenuItem.Enabled = pasteRepeatingToolStripMenuItem.Enabled = Clipboard.ContainsData(typeof(byte[,]).AssemblyQualifiedName);
				pasteSectionOnceToolStripMenuItem.Enabled = pasteSectionRepeatingToolStripMenuItem.Enabled = layoutSectionListBox.SelectedIndex != -1;
				rotateLeftToolStripMenuItem.Enabled = rotateRightToolStripMenuItem.Enabled = selection.Width == selection.Height;
				layoutContextMenuStrip.Show(layoutPanel, e.Location);
			}

			if (!drawing) return;
			drawing = false;
			Point gridloc = new Point(e.X / gridsize, e.Y / gridsize);
			switch (tool)
			{
				case Tool.Pencil:
					DoAction(new PencilAction(drawlist));
					break;
				case Tool.Fill:
					return;
				case Tool.Line:
					DoAction(new LineAction(drawlist));
					break;
				case Tool.Rectangle:
					switch (rectmode)
					{
						case ShapeMode.Edge:
							DoAction(new RectangleEdgeAction(drawlist));
							break;
						case ShapeMode.FillEdge:
						case ShapeMode.Fill:
							DoAction(new RectangleFillAction(fillrect, Math.Min(gridloc.X, firstloc.X / gridsize), Math.Min(gridloc.Y, firstloc.Y / gridsize)));
							break;
					}
					break;
				case Tool.Diamond:
					switch (diammode)
					{
						case ShapeMode.Edge:
							DoAction(new DiamondEdgeAction(drawlist));
							break;
						case ShapeMode.FillEdge:
						case ShapeMode.Fill:
							DoAction(new DiamondFillAction(drawrect, Math.Min(gridloc.X, firstloc.X / gridsize), Math.Min(gridloc.Y, firstloc.Y / gridsize)));
							break;
					}
					break;
				case Tool.Oval:
					DoAction(new OvalAction(drawrect, Math.Min(gridloc.X, firstloc.X / gridsize), Math.Min(gridloc.Y, firstloc.Y / gridsize)));
					break;
			}
			DrawLayout();
		}

		private void layoutPanel_KeyDown(object sender, KeyEventArgs e)
		{

		}

		private void cutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Rectangle area = selection;
			if (area.IsEmpty)
				area = new Rectangle(0, 0, 0x40, 0x40);
			byte[,] sect = new byte[area.Width, area.Height];
			for (int y = 0; y < area.Height; y++)
				for (int x = 0; x < area.Width; x++)
					sect[x, y] = layout[area.X + x, area.Y + y];
			Clipboard.SetData(sect.GetType().AssemblyQualifiedName, sect);
			DoAction(new CutAction(new byte[area.Width, area.Height], area.Location));
			DrawLayout();
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Rectangle area = selection;
			if (area.IsEmpty)
				area = new Rectangle(0, 0, 0x40, 0x40);
			byte[,] sect = new byte[area.Width, area.Height];
			for (int y = 0; y < area.Height; y++)
				for (int x = 0; x < area.Width; x++)
					sect[x, y] = layout[area.X + x, area.Y + y];
			Clipboard.SetData(sect.GetType().AssemblyQualifiedName, sect);
		}

		private void pasteOnceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			byte[,] data = (byte[,])Clipboard.GetData(typeof(byte[,]).AssemblyQualifiedName);
			DoAction(new PasteOnceAction(data, selection.Location));
			selection.Size = new Size(data.GetLength(0), data.GetLength(1));
			DrawLayout();
		}

		private void pasteRepeatingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Rectangle area = selection;
			if (area.IsEmpty)
				area = new Rectangle(0, 0, 0x40, 0x40);
			byte[,] sect = new byte[area.Width, area.Height];
			byte[,] copy = (byte[,])Clipboard.GetData(typeof(byte[,]).AssemblyQualifiedName);
			int copywidth = copy.GetLength(0);
			int copyheight = copy.GetLength(1);
			for (int y = 0; y < area.Height; y++)
				for (int x = 0; x < area.Width; x++)
					sect[x, y] = copy[x % copywidth, y % copyheight];
			DoAction(new PasteRepeatingAction(sect, selection.Location));
			DrawLayout();
		}

		private void importToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void flipHorizontallyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Rectangle area = selection;
			if (area.IsEmpty)
				area = new Rectangle(0, 0, 0x40, 0x40);
			DoAction(new FlipHorizontallyAction(area));
			DrawLayout();
		}

		private void flipVerticallyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Rectangle area = selection;
			if (area.IsEmpty)
				area = new Rectangle(0, 0, 0x40, 0x40);
			DoAction(new FlipVerticallyAction(area));
			DrawLayout();
		}

		private void rotateLeftToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Rectangle area = selection;
			if (area.IsEmpty)
				area = new Rectangle(0, 0, 0x40, 0x40);
			DoAction(new RotateLeftAction(area));
			DrawLayout();
		}

		private void rotateRightToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Rectangle area = selection;
			if (area.IsEmpty)
				area = new Rectangle(0, 0, 0x40, 0x40);
			DoAction(new RotateRightAction(area));
			DrawLayout();
		}

		private void saveSectionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (LayoutSectionNameDialog dlg = new LayoutSectionNameDialog())
			{
				dlg.Value = "Section " + (layoutSections.Count + 1);
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					Rectangle area = selection;
					if (area.IsEmpty)
						area = new Rectangle(0, 0, 0x40, 0x40);
					byte[,] sect = new byte[area.Width, area.Height];
					for (int y = 0; y < area.Height; y++)
						for (int x = 0; x < area.Width; x++)
							sect[x, y] = layout[area.X + x, area.Y + y];
					LayoutSection sec = new LayoutSection(dlg.Value, sect);
					layoutSections.Add(sec);
					layoutSectionImages.Add(MakeLayoutSectionImage(sec));
					layoutSectionListBox.Items.Add(sec.Name);
					layoutSectionListBox.SelectedIndex = layoutSections.Count - 1;
					SerializeCompressed("LayoutSections.sls", layoutSections);
				}
			}
		}

		private void pasteSectionOnceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DoAction(new PasteOnceAction(layoutSections[layoutSectionListBox.SelectedIndex].Layout, selection.Location));
			selection.Size = new Size(layoutSections[layoutSectionListBox.SelectedIndex].Layout.GetLength(0), layoutSections[layoutSectionListBox.SelectedIndex].Layout.GetLength(1));
			DrawLayout();
		}

		private void pasteSectionRepeatingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Rectangle area = selection;
			if (area.IsEmpty)
				area = new Rectangle(0, 0, 0x40, 0x40);
			byte[,] sect = new byte[area.Width, area.Height];
			byte[,] copy = layoutSections[layoutSectionListBox.SelectedIndex].Layout;
			int copywidth = copy.GetLength(0);
			int copyheight = copy.GetLength(1);
			for (int y = 0; y < area.Height; y++)
				for (int x = 0; x < area.Width; x++)
					sect[x, y] = copy[x % copywidth, y % copyheight];
			DoAction(new PasteRepeatingAction(sect, selection.Location));
			DrawLayout();
		}

		private void layoutSectionListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (layoutSectionListBox.SelectedIndex == -1)
				layoutSectionPreview.Image = null;
			else
				layoutSectionPreview.Image = layoutSectionImages[layoutSectionListBox.SelectedIndex];
		}

		private void layoutSectionListBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (layoutSectionListBox.SelectedIndex != -1 && e.KeyCode == Keys.Delete
				&& MessageBox.Show(this, "Are you sure you want to delete layout section \"" + layoutSections[layoutSectionListBox.SelectedIndex].Name + "\"?", "S1SSEdit", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
			{
				layoutSections.RemoveAt(layoutSectionListBox.SelectedIndex);
				layoutSectionImages.RemoveAt(layoutSectionListBox.SelectedIndex);
				layoutSectionListBox.Items.RemoveAt(layoutSectionListBox.SelectedIndex);
				SerializeCompressed("LayoutSections.sls", layoutSections);
			}
		}
	}

	enum Tool { Select, Pencil, Fill, Line, Rectangle, Diamond, Oval }

	enum ShapeMode { Edge, FillEdge, Fill }

	[Serializable]
	public class LayoutSection
	{
		public string Name { get; set; }
		public byte[,] Layout { get; set; }

		public LayoutSection() { }

		public LayoutSection(string name, byte[,] layout)
		{
			Name = name;
			Layout = layout;
		}
	}

	[Serializable]
	class ObjLoc : IEquatable<ObjLoc>
	{
		public byte Object { get; set; }
		public int X { get; set; }
		public int Y { get; set; }

		public ObjLoc(byte obj, int x, int y)
		{
			Object = obj;
			X = x;
			Y = y;
		}

		public ObjLoc(byte obj, Point position)
			: this(obj, position.X, position.Y)
		{ }

		public bool Equals(ObjLoc other)
		{
			return Object == other.Object && X == other.X && Y == other.Y;
		}

		public override bool Equals(object obj)
		{
			return obj is ObjLoc && Equals((ObjLoc)obj);
		}

		public override int GetHashCode()
		{
			return Object.GetHashCode() ^ X.GetHashCode() ^ Y.GetHashCode();
		}

		public override string ToString()
		{
			return $"{Object}, {X}, {Y}";
		}
	}

	[Serializable]
	abstract class Action
	{
		public abstract string Name { get; }
		public abstract void Do(byte[,] layout);
	}

	[Serializable]
	abstract class ObjListAction : Action
	{
		public ObjListAction(List<ObjLoc> objs)
		{
			this.objs = objs;
		}

		List<ObjLoc> objs;

		public override void Do(byte[,] layout)
		{
			foreach (ObjLoc s in objs)
			{
				byte sp = layout[s.X, s.Y];
				layout[s.X, s.Y] = s.Object;
				s.Object = sp;
			}
		}
	}

	[Serializable]
	abstract class AreaFillAction : Action
	{
		public AreaFillAction(byte[,] objs, Point position)
		{
			int xoff = 0;
			int yoff = 0;
			int width = objs.GetLength(0);
			int height = objs.GetLength(1);
			bool copy = false;
			if (position.X < 0)
			{
				xoff = -position.X;
				width += position.X;
				position.X = 0;
				copy = true;
			}
			if (position.Y < 0)
			{
				yoff = -position.Y;
				height += position.Y;
				position.Y = 0;
				copy = true;
			}
			if (position.X + width > 0x40)
			{
				width = 0x40 - position.X;
				copy = true;
			}
			if (position.Y + height > 0x40)
			{
				height = 0x40 - position.Y;
				copy = true;
			}
			if (!copy)
				this.objs = objs;
			else
			{
				this.objs = new byte[width, height];
				for (int y = 0; y < height; y++)
					for (int x = 0; x < width; x++)
						this.objs[x, y] = objs[x + xoff, y + yoff];
			}
			this.position = position;
		}

		public AreaFillAction(byte[,] objs, int x, int y)
			: this(objs, new Point(x, y))
		{ }

		byte[,] objs;
		Point position;

		public override void Do(byte[,] layout)
		{
			for (int y = 0; y < objs.GetLength(1); y++)
				for (int x = 0; x < objs.GetLength(0); x++)
				{
					byte tmp = layout[x + position.X, y + position.Y];
					layout[x + position.X, y + position.Y] = objs[x, y];
					objs[x, y] = tmp;
				}
		}
	}

	[Serializable]
	abstract class AreaAction : Action
	{
		public AreaAction(byte?[,] objs)
		{
			int minX = 0x40;
			int minY = 0x40;
			int maxX = -1;
			int maxY = -1;
			for (int y = 0; y < 0x40; y++)
				for (int x = 0; x < 0x40; x++)
					if (objs[x, y].HasValue)
					{
						minX = Math.Min(minX, x);
						minY = Math.Min(minY, y);
						maxX = Math.Max(maxX, x);
						maxY = Math.Max(maxY, y);
					}
			if (minX == 0 && minY == 0 && maxX == 0x3F && maxY == 0x3F)
				this.objs = objs;
			else
			{
				this.objs = new byte?[maxX - minX + 1, maxY - minY + 1];
				for (int y = minY; y <= maxY; y++)
					for (int x = minX; x <= maxX; x++)
						this.objs[x - minX, y - minY] = objs[x, y];
			}
			position = new Point(minX, minY);
		}

		public AreaAction(byte?[,] objs, Point position)
		{
			int xoff = 0;
			int yoff = 0;
			int width = objs.GetLength(0);
			int height = objs.GetLength(1);
			bool copy = false;
			if (position.X < 0)
			{
				xoff = -position.X;
				width += position.X;
				position.X = 0;
				copy = true;
			}
			if (position.Y < 0)
			{
				yoff = -position.Y;
				height += position.Y;
				position.Y = 0;
				copy = true;
			}
			if (position.X + width > 0x40)
			{
				width = 0x40 - position.X;
				copy = true;
			}
			if (position.Y + height > 0x40)
			{
				height = 0x40 - position.Y;
				copy = true;
			}
			if (!copy)
				this.objs = objs;
			else
			{
				this.objs = new byte?[width, height];
				for (int y = 0; y < height; y++)
					for (int x = 0; x < width; x++)
						this.objs[x, y] = objs[x + xoff, y + yoff];
			}
			this.position = position;
		}

		public AreaAction(byte?[,] objs, int x, int y)
			: this(objs, new Point(x, y))
		{ }

		byte?[,] objs;
		Point position;

		public override void Do(byte[,] layout)
		{
			for (int y = 0; y < objs.GetLength(1); y++)
				for (int x = 0; x < objs.GetLength(0); x++)
					if (objs[x, y].HasValue)
					{
						byte tmp = layout[x + position.X, y + position.Y];
						layout[x + position.X, y + position.Y] = objs[x, y].Value;
						objs[x, y] = tmp;
					}
		}
	}

	[Serializable]
	abstract class RotateAction : Action
	{
		public RotateAction(Rectangle area, bool right)
		{
			this.area = area;
			this.right = right;
		}

		Rectangle area;
		bool right;

		public override void Do(byte[,] layout)
		{
			byte[,] copy = (byte[,])layout.Clone();
			if (right)
			{
				for (int y = 0; y < area.Height; y++)
					for (int x = 0; x < area.Width; x++)
						layout[area.X + area.Height - y - 1, area.Y + x] = copy[area.X + x, area.Y + y];
			}
			else
			{
				for (int y = 0; y < area.Height; y++)
					for (int x = 0; x < area.Width; x++)
						layout[area.X + y, area.Y + area.Width - x - 1] = copy[area.X + x, area.Y + y];
			}
			right = !right;
		}
	}

	[Serializable]
	class PencilAction : ObjListAction
	{
		public PencilAction(List<ObjLoc> objs) : base(objs) { }

		public override string Name => "Draw";
	}

	[Serializable]
	class FillAction : AreaAction
	{
		public FillAction(byte?[,] objs) : base(objs) { }

		public override string Name => "Fill";
	}

	[Serializable]
	class LineAction : ObjListAction
	{
		public LineAction(List<ObjLoc> objs) : base(objs) { }

		public override string Name => "Line";
	}

	[Serializable]
	class RectangleEdgeAction : ObjListAction
	{
		public RectangleEdgeAction(List<ObjLoc> objs) : base(objs) { }

		public override string Name => "Rectangle";
	}

	[Serializable]
	class RectangleFillAction : AreaFillAction
	{
		public RectangleFillAction(byte[,] objs, Point position) : base(objs, position) { }

		public RectangleFillAction(byte[,] objs, int x, int y) : base(objs, x, y) { }

		public override string Name => "Rectangle";
	}

	[Serializable]
	class DiamondEdgeAction : ObjListAction
	{
		public DiamondEdgeAction(List<ObjLoc> objs) : base(objs) { }

		public override string Name => "Diamond";
	}

	[Serializable]
	class DiamondFillAction : AreaAction
	{
		public DiamondFillAction(byte?[,] objs, Point position) : base(objs, position) { }

		public DiamondFillAction(byte?[,] objs, int x, int y) : base(objs, x, y) { }

		public override string Name => "Diamond";
	}

	[Serializable]
	class OvalAction : AreaAction
	{
		public OvalAction(byte?[,] objs, Point position) : base(objs, position) { }

		public OvalAction(byte?[,] objs, int x, int y) : base(objs, x, y) { }

		public override string Name => "Oval";
	}

	[Serializable]
	class CutAction : AreaFillAction
	{
		public CutAction(byte[,] objs, Point position) : base(objs, position) { }

		public CutAction(byte[,] objs, int x, int y) : base(objs, x, y) { }

		public override string Name => "Cut";
	}

	[Serializable]
	class PasteOnceAction : AreaFillAction
	{
		public PasteOnceAction(byte[,] objs, Point position) : base(objs, position) { }

		public PasteOnceAction(byte[,] objs, int x, int y) : base(objs, x, y) { }

		public override string Name => "Paste Once";
	}

	[Serializable]
	class PasteRepeatingAction : AreaFillAction
	{
		public PasteRepeatingAction(byte[,] objs, Point position) : base(objs, position) { }

		public PasteRepeatingAction(byte[,] objs, int x, int y) : base(objs, x, y) { }

		public override string Name => "Paste Repeating";
	}

	[Serializable]
	class FlipHorizontallyAction : Action
	{
		public FlipHorizontallyAction(Rectangle area)
		{
			this.area = area;
		}

		Rectangle area;

		public override void Do(byte[,] layout)
		{
			byte[,] copy = (byte[,])layout.Clone();
			for (int y = 0; y < area.Height; y++)
				for (int x = 0; x < area.Width; x++)
					layout[area.X + x, area.Y + y] = copy[area.Right - x - 1, area.Y + y];
		}

		public override string Name => "Flip Horizontally";
	}

	[Serializable]
	class FlipVerticallyAction : Action
	{
		public FlipVerticallyAction(Rectangle area)
		{
			this.area = area;
		}

		Rectangle area;

		public override void Do(byte[,] layout)
		{
			byte[,] copy = (byte[,])layout.Clone();
			for (int y = 0; y < area.Height; y++)
				for (int x = 0; x < area.Width; x++)
					layout[area.X + x, area.Y + y] = copy[area.X + x, area.Bottom - y - 1];
		}

		public override string Name => "Flip Vertically";
	}

	[Serializable]
	class RotateLeftAction : RotateAction
	{
		public RotateLeftAction(Rectangle area) : base(area, false) { }

		public override string Name => "Rotate Left";
	}

	[Serializable]
	class RotateRightAction : RotateAction
	{
		public RotateRightAction(Rectangle area) : base(area, true) { }

		public override string Name => "Rotate Right";
	}
}
