using SonicRetro.SonLVL.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ChaotixSpriteEdit
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		ColorPalette palette;
		int selectedColor;
		Sprite sprite = new Sprite(new BitmapBits(32, 32), new Point(-16, -16));
		string filename;

		private void MainForm_Load(object sender, EventArgs e)
		{
			using (Bitmap tmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
				palette = tmp.Palette;
			Environment.CurrentDirectory = Application.StartupPath;
			if (File.Exists("Palette.dat"))
				using (FileStream fs = File.OpenRead("Palette.dat"))
				using (BinaryReader br = new BinaryReader(fs))
					for (int i = 0; i < 256; i++)
						palette.Entries[i] = Color.FromArgb(br.ReadByte(), br.ReadByte(), br.ReadByte());
			if (Program.Arguments.Count > 0)
				LoadSprite(Program.Arguments[0]);
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			Environment.CurrentDirectory = Application.StartupPath;
			using (FileStream fs = File.Create("Palette.dat"))
			using (BinaryWriter bw = new BinaryWriter(fs))
				for (int i = 0; i < 256; i++)
				{
					bw.Write(palette.Entries[i].R);
					bw.Write(palette.Entries[i].G);
					bw.Write(palette.Entries[i].B);
				}
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			sprite = new Sprite(new BitmapBits(32, 32), new Point(-16, -16));
			spriteImagePanel.Size = new Size(128, 128);
			filename = null;
			saveToolStripMenuItem.Enabled = false;
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "bin", Filter = "Binary Files|*.bin|All Files|*.*", RestoreDirectory = true })
				if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
					LoadSprite(dlg.FileName);
		}

		private void LoadSprite(string filename)
		{
			sprite = Sprite.LoadChaotixSprites(filename)[0];
			spriteImagePanel.Size = new Size(sprite.Width * 4, sprite.Height * 4);
			offsetXNumericUpDown.Value = sprite.X;
			offsetYNumericUpDown.Value = sprite.Y;
			this.filename = filename;
			saveToolStripMenuItem.Enabled = true;
			spriteImagePanel.Invalidate();
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Sprite.SaveChaotixSprites(filename, new[] { sprite });
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "bin", Filter = "Binary Files|*.bin|All Files|*.*", RestoreDirectory = true })
				if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{
					filename = dlg.FileName;
					Sprite.SaveChaotixSprites(filename, new[] { sprite });
				}
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void palettePanel_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.Clear(Color.Black);
			int x = 0, y = 0;
			for (int i = 0; i < 256; i++)
			{
				e.Graphics.FillRectangle(new SolidBrush(palette.Entries[i]), x * 32, y * 32, 32, 32);
				e.Graphics.DrawRectangle(Pens.White, x * 32, y * 32, 31, 31);
				if (++x == 16)
				{
					x = 0;
					++y;
				}
			}
			e.Graphics.DrawRectangle(new Pen(Color.Yellow, 2), (selectedColor % 16) * 32, (selectedColor / 16) * 32, 32, 32);
		}

		private void palettePanel_MouseDown(object sender, MouseEventArgs e)
		{
			selectedColor = ((e.Y / 32) * 16) + (e.X / 32);
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
				paletteContextMenuStrip.Show(palettePanel, e.Location);
			palettePanel.Invalidate();
		}

		private void importPaletteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog a = new OpenFileDialog() { DefaultExt = "bin", Filter = "32X Palettes|*.bin|Image Files|*.bmp;*.png;*.jpg;*.gif", RestoreDirectory = true })
				if (a.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{
					switch (Path.GetExtension(a.FileName))
					{
						case ".bin":
							SonLVLColor[] colors = SonLVLColor.Load(a.FileName, EngineVersion.Chaotix);
							for (int i = 0; i < colors.Length && selectedColor + i < palette.Entries.Length; i++)
								palette.Entries[selectedColor + i] = colors[i].RGBColor;
							break;
						case ".bmp":
						case ".png":
						case ".jpg":
						case ".gif":
							using (Bitmap bmp = new Bitmap(a.FileName))
								if ((bmp.PixelFormat & System.Drawing.Imaging.PixelFormat.Indexed) == System.Drawing.Imaging.PixelFormat.Indexed)
								{
									Color[] pal = bmp.Palette.Entries;
									for (int i = 0; i < pal.Length && selectedColor + i < palette.Entries.Length; i++)
										palette.Entries[selectedColor + i] = pal[i];
								}
							break;
					}
					palettePanel.Invalidate();
					spriteImagePanel.Invalidate();
				}
		}

		private void palettePanel_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			using (ColorDialog a = new ColorDialog { AllowFullOpen = true, AnyColor = true, FullOpen = true, Color = palette.Entries[selectedColor] })
				if (a.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					palette.Entries[selectedColor] = a.Color;
					palettePanel.Invalidate();
					spriteImagePanel.Invalidate();
				}
		}

		private void offsetXNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			sprite.X = (int)offsetXNumericUpDown.Value;
			if (showCenterCheckBox.Checked)
				spriteImagePanel.Invalidate();
		}

		private void offsetYNumericUpDown_ValueChanged(object sender, EventArgs e)
		{
			sprite.Y = (int)offsetYNumericUpDown.Value;
			if (showCenterCheckBox.Checked)
				spriteImagePanel.Invalidate();
		}

		private void showCenterCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			spriteImagePanel.Invalidate();
		}

		private void spriteImagePanel_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.SetOptions();
			using (Bitmap bmp = sprite.Image.ToBitmap(palette))
				e.Graphics.DrawImage(bmp, 0, 0, spriteImagePanel.Width, spriteImagePanel.Height);
			if (showCenterCheckBox.Checked)
			{
				e.Graphics.DrawLine(Pens.LimeGreen, (-sprite.X * 4) - 4, -sprite.Y * 4, (-sprite.X * 4) + 4, -sprite.Y * 4);
				e.Graphics.DrawLine(Pens.Fuchsia, -sprite.X * 4, (-sprite.Y * 4) - 4, -sprite.X * 4, (-sprite.Y * 4) + 4);
			}
		}

		private void spriteImagePanel_MouseDown(object sender, MouseEventArgs e)
		{
			switch (e.Button)
			{
				case MouseButtons.Left:
					sprite.Image[e.X / 4, e.Y / 4] = (byte)selectedColor;
					spriteImagePanel.Invalidate();
					break;
				case MouseButtons.Right:
					selectedColor = sprite.Image[e.X / 4, e.Y / 4];
					palettePanel.Invalidate();
					break;
			}
		}

		private void spriteImagePanel_MouseMove(object sender, MouseEventArgs e)
		{
			if (new Rectangle(Point.Empty, spriteImagePanel.Size).Contains(e.Location))
				switch (e.Button)
				{
					case MouseButtons.Left:
						sprite.Image[e.X / 4, e.Y / 4] = (byte)selectedColor;
						spriteImagePanel.Invalidate();
						break;
					case MouseButtons.Right:
						selectedColor = sprite.Image[e.X / 4, e.Y / 4];
						palettePanel.Invalidate();
						break;
				}
		}

		private void importButton_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "png", Filter = "Image Files|*.png;*.bmp;*.gif;*.jpg", RestoreDirectory = true })
				if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
					using (Bitmap bmp = new Bitmap(dlg.FileName))
					{
						if ((bmp.PixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed)
							sprite.Image = new BitmapBits(bmp);
						else
							using (Bitmap tmp = bmp.To32bpp())
							{
								BitmapBits bmpbits = new BitmapBits(tmp.Width, tmp.Height);
								BitmapData bmpd = tmp.LockBits(new Rectangle(0, 0, tmp.Width, tmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
								int stride = bmpd.Stride;
								byte[] Bits = new byte[Math.Abs(stride) * bmpd.Height];
								System.Runtime.InteropServices.Marshal.Copy(bmpd.Scan0, Bits, 0, Bits.Length);
								tmp.UnlockBits(bmpd);
								for (int y = 0; y < bmpbits.Height; y++)
								{
									int srcaddr = y * Math.Abs(stride);
									for (int x = 0; x < bmpbits.Width; x++)
									{
										Color col = Color.FromArgb(BitConverter.ToInt32(Bits, srcaddr + (x * 4)));
										bmpbits[x, y] = (byte)Array.IndexOf(palette.Entries, col.FindNearestMatch(palette.Entries));
										if (col.A < 128)
											bmpbits[x, y] = 0;
									}
								}
								sprite.Image = bmpbits;
							}
						spriteImagePanel.Size = new Size(sprite.Width * 4, sprite.Height * 4);
						spriteImagePanel.Invalidate();
					}
		}

		private void exportButton_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "png", Filter = "PNG Files|*.png", RestoreDirectory = true })
				if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
					using (Bitmap bmp = sprite.Image.ToBitmap(palette))
						bmp.Save(dlg.FileName);
		}
	}
}
