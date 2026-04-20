using SonicRetro.SonLVL.API;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SonicRetro.SonLVL
{
	public partial class BackgroundColorDialog : Form
	{
		Graphics PalettePanelGfx;
		Bitmap palette;
		Point selection;

		public BackgroundColorDialog()
		{
			InitializeComponent();

			PalettePanelGfx = palettePanel.CreateGraphics();
			PalettePanelGfx.SetOptions();

			BitmapBits bitmap = new BitmapBits(512, 128);
			if (LevelData.Palette == null)
			{
				palette = bitmap.ToBitmap(new Color[] { Color.Black });
			}
			else
			{
				Color[] pal = new Color[64];

				for (int y = 0; y < 4; y++)
					for (int x = 0; x < 16; x++)
					{
						byte index = (byte)((y * 16) + x);
						pal[index] = LevelData.Palette[LevelData.CurPal][y, x].RGBColor;
						bitmap.FillRectangle(index, x * 32, y * 32, 32, 32);
					}

				palette = bitmap.ToBitmap(pal);
			}
		}

		Brush brush = new SolidBrush(Color.FromArgb(120, Color.Gray));
		private void DrawPalette()
		{
			PalettePanelGfx.DrawImage(palette, 0, 0, 256, 64);
			PalettePanelGfx.DrawRectangle(Pens.Yellow, selection.X * 16, selection.Y * 16, 15, 15);
			if (!useLevelColor.Checked)
				PalettePanelGfx.FillRectangle(brush, 0, 0, 256, 123);
		}
		
		private void palettePanel_MouseDown(object sender, MouseEventArgs e)
		{
			if (!useLevelColor.Checked) return;

			selection = e.Location;
			selection.X /= 16; selection.Y = Math.Min(selection.Y / 16, 15);
			index.Value = selection.X + (selection.Y * 16);
			DrawPalette();
		}

		private void palettePanel_Paint(object sender, PaintEventArgs e)
		{
			DrawPalette();
		}
	
		private void useLevelColor_CheckedChanged(object sender, EventArgs e)
		{
			index.Enabled = useLevelColor.Checked;
			DrawPalette();
		}

		private void index_ValueChanged(object sender, EventArgs e)
		{
			selection.X = (int)index.Value & 15;
			selection.Y = (int)index.Value / 16;
			DrawPalette();
		}

		private void customColor_Click(object sender, EventArgs e)
		{
			ColorDialog a = new ColorDialog
			{
				AllowFullOpen = true,
				AnyColor = true,
				FullOpen = true,
				SolidColorOnly = true,
				Color = customColorBox.BackColor
			};
			if (a.ShowDialog() == DialogResult.OK)
				customColorBox.BackColor = a.Color;
		}

		private void useCustomColor_CheckedChanged(object sender, EventArgs e)
		{
			colorChange.Enabled = useCustomColor.Checked;
			customColorOverlay.Visible = !useCustomColor.Checked;
		}
	}
}
