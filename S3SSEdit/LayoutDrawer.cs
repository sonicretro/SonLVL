using SonicRetro.SonLVL.API;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace S3SSEdit
{
	static class LayoutDrawer
	{
		public static ColorPalette Palette { get; private set; }
		public static Dictionary<SphereType, BitmapBits> SphereBmps { get; private set; } = new Dictionary<SphereType, BitmapBits>(5);
		public static BitmapBits[] StartBmps { get; private set; } = new BitmapBits[4];

		public static void Init()
		{
			using (Bitmap tmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
				Palette = tmp.Palette;
			Bitmap[] bmplist = { Properties.Resources.red, Properties.Resources.blue, Properties.Resources.bumper, Properties.Resources.ring, Properties.Resources.yellow };
			int palind = 1;
			for (int i = 0; i < bmplist.Length; i++)
			{
				bmplist[i].Palette.Entries.CopyTo(Palette.Entries, palind);
				BitmapBits bmp = new BitmapBits(bmplist[i]);
				bmp.IncrementIndexes(palind);
				SphereBmps[(SphereType)(i + 1)] = bmp;
				palind += 16;
			}
			Palette.Entries[0] = Palette.Entries[1] = Color.Transparent;
			bmplist = new[] { Properties.Resources.north, Properties.Resources.west, Properties.Resources.south, Properties.Resources.east };
			bmplist[0].Palette.Entries.CopyTo(Palette.Entries, palind);
			for (int i = 0; i < bmplist.Length; i++)
			{
				BitmapBits bmp = new BitmapBits(bmplist[i]);
				bmp.IncrementIndexes(palind);
				StartBmps[i] = bmp;
			}
		}

		public static BitmapBits DrawLayout(SphereType[,] layout, int gridsize)
		{
			int width = layout.GetLength(0);
			int height = layout.GetLength(1);
			int off = (gridsize - 24) / 2;
			BitmapBits layoutbmp = new BitmapBits(width * gridsize, height * gridsize);
			for (int y = -gridsize / 2; y < layoutbmp.Height; y += gridsize * 2)
			{
				bool row = false;
				for (int x = -gridsize / 2; x < layoutbmp.Width; x += gridsize)
				{
					layoutbmp.FillRectangle(1, x, row ? y : y + gridsize, gridsize, gridsize);
					row = !row;
				}
			}
			for (int y = 0; y < height; y++)
				for (int x = 0; x < width; x++)
				{
					SphereType sp = layout[x, y];
					if (sp != SphereType.Empty)
						layoutbmp.DrawBitmapComposited(SphereBmps[sp], x * gridsize + off, y * gridsize + off);
				}
			return layoutbmp;
		}

		public static BitmapBits DrawLayout(SphereType?[,] layout, int gridsize)
		{
			int width = layout.GetLength(0);
			int height = layout.GetLength(1);
			int off = (gridsize - 24) / 2;
			BitmapBits layoutbmp = new BitmapBits(width * gridsize, height * gridsize);
			for (int y = -gridsize / 2; y < layoutbmp.Height; y += gridsize * 2)
				for (int x = -gridsize / 2; x < layoutbmp.Width; x += gridsize * 2)
					layoutbmp.FillRectangle(1, x, y, gridsize, gridsize);
			for (int y = 0; y < height; y++)
				for (int x = 0; x < width; x++)
				{
					SphereType? sp = layout[x, y];
					if (sp.HasValue && sp.Value != SphereType.Empty)
						layoutbmp.DrawBitmapComposited(SphereBmps[sp.Value], x * gridsize + off, y * gridsize + off);
				}
			return layoutbmp;
		}

		public static BitmapBits DrawLayout(LayoutData layout, int gridsize)
		{
			BitmapBits layoutbmp = DrawLayout(layout.Layout.ToArray(), gridsize);
			int off = (gridsize - 24) / 2;
			switch (layout)
			{
				case SSLayoutData ss:
					layoutbmp.DrawBitmapComposited(StartBmps[ss.Angle >> 14], (ss.StartX / 0x100) * gridsize + off, (ss.StartY / 0x100) * gridsize + off);
					break;
				case BSChunkLayoutData bsc:
					layoutbmp.DrawBitmapComposited(StartBmps[3], 15 * gridsize + off, 3 * gridsize + off);
					break;
				case BSStageLayoutData bss:
					layoutbmp.DrawBitmapComposited(StartBmps[1], 16 * gridsize + off, 3 * gridsize + off);
					break;
			}
			return layoutbmp;
		}
	}
}
