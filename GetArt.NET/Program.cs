using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using SonicRetro.SonLVL.API;

namespace GetArt.NET
{
	public static class Program
	{
		static string[] filetypes = { "bmp", "png", "gif", "jpg" };

		static void Main(string[] args)
		{
			LevelData.littleendian = false;
			string bmpfile = null;
			foreach (string extension in filetypes)
				if (File.Exists("Bitmap." + extension))
				{
					bmpfile = "Bitmap." + extension;
					break;
				}
			if (bmpfile == null)
			{
				Console.WriteLine("Bitmap file could not be found! Valid extensions are " + string.Join(", ", filetypes));
				return;
			}
			Bitmap img = new Bitmap(bmpfile);
			if (img.Width % 8 > 0 | img.Height % 8 > 0)
			{
				Bitmap dest = new Bitmap(img.Width % 8 == 0 ? img.Width : img.Width + (8 - (img.Width % 8)), img.Height % 8 == 0 ? img.Height : img.Height + (8 - (img.Height % 8)), img.PixelFormat);
				if ((img.PixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed)
					dest.Palette = img.Palette;
				using (Graphics gfx = Graphics.FromImage(dest))
					gfx.DrawImage(img, 0, 0, img.Width, img.Height);
				img.Dispose();
				img = dest;
			}
			SonLVLColor[] palette = new SonLVLColor[64];
			if ((img.PixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed)
			{
				for (int i = 0; i < Math.Min(64, img.Palette.Entries.Length); i++)
					palette[i] = new SonLVLColor(img.Palette.Entries[i]);
			}
			else
			{
				bmpfile = null;
				foreach (string extension in filetypes)
					if (File.Exists("Palette." + extension))
					{
						bmpfile = "Palette." + extension;
						break;
					}
				if (bmpfile == null)
				{
					Console.WriteLine("Palette file could not be found! Valid extensions are " + string.Join(", ", filetypes));
					return;
				}
				using (Bitmap palbmp = new Bitmap(bmpfile))
				{
					int i = 0;
					for (int y = 0; y < palbmp.Height; y += 8)
					{
						for (int x = 0; x < palbmp.Width; x += 8)
						{
							palette[i++] = new SonLVLColor(palbmp.GetPixel(x, y));
							if (i == 64)
								break;
						}
						if (i == 64)
							break;
					}
				}
			}
			using (Bitmap palbmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
				LevelData.BmpPal = palbmp.Palette;
			using (FileStream palfile = File.Create("Palette.bin"))
				for (int i = 0; i < 64; i++)
				{
					palfile.Write(ByteConverter.GetBytes(palette[i].MDColor), 0, 2);
					LevelData.BmpPal.Entries[i] = palette[i].RGBColor;
				}
			for (int i = 64; i < 256; i++)
				LevelData.BmpPal.Entries[i] = Color.Transparent;
			BitmapInfo bmpi = new BitmapInfo(img);
			img.Dispose();
			int w = bmpi.Width;
			int h = bmpi.Height;
			bool[,] priority = new bool[w / 8, h / 8];
			bmpfile = null;
			foreach (string extension in filetypes)
				if (File.Exists("Priority." + extension))
				{
					bmpfile = "Priority." + extension;
					break;
				}
			if (bmpfile != null)
				using (Bitmap pribmp = new Bitmap(bmpfile))
					LevelData.GetPriMap(pribmp, priority);
			ImportResult res = LevelData.BitmapToTiles(bmpi, priority, null, new List<byte[]>(), false, true);
			using (FileStream fs = File.Create("Art.bin"))
			using (BinaryWriter bw = new BinaryWriter(fs))
				foreach (byte[] t in res.Art)
					bw.Write(t);
			using (FileStream fs = File.Create("Map.bin"))
			using (BinaryWriter bw = new BinaryWriter(fs))
				for (int y = 0; y < h; y++)
					for (int x = 0; x < w; x++)
						bw.Write(res.Mappings[x, y].GetBytes());
		}
	}
}
