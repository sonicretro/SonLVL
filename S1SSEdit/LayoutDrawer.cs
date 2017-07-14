using SonicRetro.SonLVL.API;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace S1SSEdit
{
	static class LayoutDrawer
	{
		public static ColorPalette Palette { get; private set; }
		public static Dictionary<byte, BitmapBits> ObjectBmps { get; private set; } = new Dictionary<byte, BitmapBits>();
		public static Dictionary<byte, BitmapBits> ObjectBmpsNoNum { get; private set; }
		public static BitmapBits StartPosBmp { get; private set; }

		public static void Init()
		{
			using (Bitmap tmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
				Palette = tmp.Palette;
			SonLVLColor[] pal = SonLVLColor.Load(Properties.Resources.Palette, EngineVersion.S1);
			for (int i = 0; i < pal.Length; i++)
				Palette.Entries[i] = pal[i].RGBColor;
			Palette.Entries[0] = Color.Transparent;
			BitmapBits bmp = new BitmapBits(Properties.Resources.GFX);
			BitmapBits[] sprites = new BitmapBits[bmp.Width / 24];
			for (int i = 0; i < sprites.Length; i++)
				sprites[i] = bmp.GetSection(i * 24, 0, 24, 24);
			bmp = new BitmapBits(Properties.Resources.Font);
			BitmapBits[] font = new BitmapBits[11];
			for (int i = 0; i < 10; i++)
				font[i] = bmp.GetSection(i * 8, 0, 8, 8);
			font[10] = bmp.GetSection(10 * 8, 0, 16, 8);
			ObjectBmps[0] = new BitmapBits(24, 24);
			byte ind = 1;
			// walls
			for (int p = 0; p < 4; p++)
			{
				BitmapBits tmp = new BitmapBits(sprites[0]);
				ObjectBmps[ind++] = tmp;
				for (int i = 1; i < 9; i++)
				{
					BitmapBits t2 = new BitmapBits(tmp);
					t2.DrawBitmapComposited(font[i], 16, 16);
					ObjectBmps[ind++] = t2;
				}
				sprites[0].IncrementIndexes(16);
			}
			// misc
			for (int i = 1; i < 7; i++)
				ObjectBmps[ind++] = sprites[i];
			// "R"
			sprites[7].IncrementIndexes(16);
			ObjectBmps[ind++] = sprites[7];
			// red/white
			ObjectBmps[ind++] = sprites[8];
			// diamonds
			for (int p = 0; p < 4; p++)
			{
				BitmapBits tmp = new BitmapBits(sprites[9]);
				ObjectBmps[ind++] = tmp;
				sprites[9].IncrementIndexes(16);
			}
			// unused blocks
			ind += 3;
			// "ZONE" blocks
			for (int i = 10; i < 16; i++)
				ObjectBmps[ind++] = sprites[i];
			// ring
			sprites[16].IncrementIndexes(16);
			ObjectBmps[ind++] = sprites[16];
			// blue/yellow/pink/green emerald
			for (int p = 0; p < 4; p++)
			{
				BitmapBits tmp = new BitmapBits(sprites[19]);
				ObjectBmps[ind++] = tmp;
				sprites[19].IncrementIndexes(16);
			}
			// red emerald
			ObjectBmps[ind++] = sprites[17];
			// gray emerald
			ObjectBmps[ind++] = sprites[18];
			// pass-through barrier
			ObjectBmps[ind++] = sprites[20];
			ObjectBmps[0x4A] = new BitmapBits(sprites[20]);
			ObjectBmps[0x4A].DrawBitmapComposited(font[10], 4, 8);
			ObjectBmpsNoNum = new Dictionary<byte, BitmapBits>(ObjectBmps);
			for (int p = 0; p < 4; p++)
			{
				BitmapBits tmp = ObjectBmps[(byte)(p * 9 + 1)];
				for (int i = 1; i < 9; i++)
					ObjectBmpsNoNum[(byte)(p * 9 + 1 + i)] = tmp;
			}
			StartPosBmp = new BitmapBits(Properties.Resources.StartPos);
		}

		public static BitmapBits DrawLayout(byte?[,] layout, bool shownum)
		{
			int width = layout.GetLength(0);
			int height = layout.GetLength(1);
			BitmapBits layoutbmp = new BitmapBits(width * 24, height * 24);
			for (int y = 0; y < height; y++)
				for (int x = 0; x < width; x++)
				{
					byte? sp = layout[x, y];
					if (sp.HasValue && sp.Value != 0 && ObjectBmps.ContainsKey(sp.Value))
						layoutbmp.DrawBitmapComposited(shownum ? ObjectBmps[sp.Value] : ObjectBmpsNoNum[sp.Value], x * 24, y * 24);
				}
			return layoutbmp;
		}

		public static BitmapBits DrawLayout(byte[,] layout, bool shownum)
		{
			int width = layout.GetLength(0);
			int height = layout.GetLength(1);
			BitmapBits layoutbmp = new BitmapBits(width * 24, height * 24);
			for (int y = 0; y < height; y++)
				for (int x = 0; x < width; x++)
				{
					byte sp = layout[x, y];
					if (sp != 0 && ObjectBmps.ContainsKey(sp))
						layoutbmp.DrawBitmapComposited(shownum ? ObjectBmps[sp] : ObjectBmpsNoNum[sp], x * 24, y * 24);
				}
			return layoutbmp;
		}

		public static BitmapBits DrawLayout(LayoutData layout, bool shownum)
		{
			BitmapBits layoutbmp = DrawLayout(layout.Layout, shownum);
			if (layout.StartPosition != null)
				layoutbmp.DrawBitmapComposited(StartPosBmp, layout.StartPosition.X - 736 - (StartPosBmp.Width / 2), layout.StartPosition.Y - 688 - (StartPosBmp.Height / 2));
			return layoutbmp;
		}
	}
}
