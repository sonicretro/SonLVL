using GetoptNET;
using SonicRetro.SonLVL.API;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace SpritePlotter
{
	static class Program
	{
		static void Main(string[] args)
		{
			LongOpt[] opts = new[] {
				new LongOpt("help", Argument.No, null, 'h'),
				new LongOpt("padding", Argument.Required, null, 'p'),
				new LongOpt("startpal", Argument.Required, null, 's'),
				new LongOpt("nullfirst", Argument.No, null, 'n'),
				new LongOpt("twoplayer", Argument.No, null, '2'),
				new LongOpt("format", Argument.Required, null, 'f'),
				new LongOpt("game", Argument.Required, null, 'g'),
				new LongOpt("artcmp", Argument.Required, null, 'c'),
				new LongOpt("searchradius", Argument.Required, null, 'r'),
				new LongOpt("lowplane", Argument.Required, null, 0),
				new LongOpt("highplane", Argument.Required, null, 0),
				new LongOpt("palette", Argument.Required, null, 0),
				new LongOpt("center", Argument.Required, null, 0),
				new LongOpt("artfile", Argument.Required, null, 0),
				new LongOpt("mapfile", Argument.Required, null, 0),
				new LongOpt("dplcfile", Argument.Required, null, 0),
				new LongOpt("palfile", Argument.Required, null, 0)
			};
			if (args.Length == 0)
				args = new string[] { "-h" };
			Getopt getopt = new Getopt("SpritePlotter.NET", args, Getopt.digest(opts), opts);
			int padding = 2;
			byte startpal = 0;
			bool nullfirst = false;
			bool twoplayer = false;
			MappingsFormat mapfmt = MappingsFormat.Binary;
			EngineVersion mapgame = EngineVersion.Invalid;
			bool s3kp = false;
			CompressionType artcmp = CompressionType.Uncompressed;
			int searchradius = 0;
			Bitmap lowplanefile = null;
			Bitmap highplanefile = null;
			Color[] palette = null;
			Bitmap centerfile = null;
			string artfile = null;
			string mapfile = null;
			string dplcfile = null;
			string palfile = null;
			int opt = getopt.getopt();
			while (opt != -1)
			{
				switch (opt)
				{
					case 'h':
						Console.Write(Properties.Resources.HelpText);
						return;
					case 'p':
						padding = int.Parse(getopt.Optarg);
						break;
					case 's':
						startpal = byte.Parse(getopt.Optarg);
						break;
					case 'n':
						nullfirst = true;
						break;
					case '2':
						twoplayer = true;
						break;
					case 'f':
						mapfmt = (MappingsFormat)Enum.Parse(typeof(MappingsFormat), getopt.Optarg, true);
						break;
					case 'g':
						if (getopt.Optarg.Equals("s3kp", StringComparison.OrdinalIgnoreCase))
						{
							mapgame = EngineVersion.S3K;
							s3kp = true;
						}
						else
							mapgame = (EngineVersion)Enum.Parse(typeof(EngineVersion), getopt.Optarg, true);
						break;
					case 'c':
						artcmp = (CompressionType)Enum.Parse(typeof(CompressionType), getopt.Optarg, true);
						break;
					case 'r':
						searchradius = int.Parse(getopt.Optarg);
						break;
					case 0:
						switch (opts[getopt.Longind].Name)
						{
							case "lowplane":
								lowplanefile = new Bitmap(getopt.Optarg);
								if (palette == null && (lowplanefile.PixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed)
									palette = lowplanefile.Palette.Entries;
								break;
							case "highplane":
								highplanefile = new Bitmap(getopt.Optarg);
								if (palette == null && (highplanefile.PixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed)
									palette = highplanefile.Palette.Entries;
								break;
							case "palette":
								switch (System.IO.Path.GetExtension(getopt.Optarg))
								{
									case ".bin":
										palette = SonLVLColor.Load(getopt.Optarg, EngineVersion.S2).Select(a => a.RGBColor).ToArray();
										break;
									case ".bmp":
									case ".png":
									case ".jpg":
									case ".gif":
										using (Bitmap bmp = new Bitmap(getopt.Optarg))
										{
											if ((bmp.PixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed)
												palette = bmp.Palette.Entries;
											else
											{
												List<Color> pal = new List<Color>();
												for (int y = 0; y < bmp.Height; y += 8)
													for (int x = 0; x < bmp.Width; x += 8)
														pal.Add(bmp.GetPixel(x, y));
												palette = pal.ToArray();
											}
										}
										break;
								}
								break;
							case "center":
								centerfile = new Bitmap(getopt.Optarg);
								break;
							case "artfile":
								artfile = getopt.Optarg;
								break;
							case "mapfile":
								mapfile = getopt.Optarg;
								break;
							case "dplcfile":
								dplcfile = getopt.Optarg;
								break;
							case "palfile":
								palfile = getopt.Optarg;
								break;
						}
						break;
				}
				opt = getopt.getopt();
			}
			if (mapgame == EngineVersion.Invalid)
			{
				Console.Error.WriteLine("No game specified.");
				return;
			}
			if (lowplanefile == null && highplanefile == null)
			{
				Console.Error.WriteLine("No image file specified.");
				return;
			}
			if (palette == null)
			{
				Console.Error.WriteLine("No palette specified, and image(s) did not contain palette.");
				return;
			}
			if (artfile == null)
			{
				Console.Error.WriteLine("No output art file specified.");
				return;
			}
			if (mapfile == null)
			{
				Console.Error.WriteLine("No output mappings file specified.");
				return;
			}
			if (palette.Length > 64)
				palette = palette.Take(64).ToArray(); // lazy
			BitmapBits lowplane = null, highplane = null, combinedplanes = null;
			if (lowplanefile != null)
			{
				if ((lowplanefile.PixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed)
					lowplane = new BitmapBits(lowplanefile);
				else
					using (Bitmap bmp = lowplanefile.To32bpp())
						lowplane = LoadBitmap32BppArgb(bmp, palette);
				lowplanefile.Dispose();
				if (highplanefile == null)
					combinedplanes = lowplane;
			}
			if (highplanefile != null)
			{
				if ((highplanefile.PixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed)
					highplane = new BitmapBits(highplanefile);
				else
					using (Bitmap bmp = highplanefile.To32bpp())
						highplane = LoadBitmap32BppArgb(bmp, palette);
				highplanefile.Dispose();
				if (lowplanefile == null)
					combinedplanes = highplane;
			}
			if (combinedplanes == null)
			{
				combinedplanes = new BitmapBits(Math.Max(lowplane.Width, highplane.Width), Math.Max(lowplane.Height, highplane.Height));
				combinedplanes.DrawBitmap(lowplane, 0, 0);
				combinedplanes.DrawBitmapComposited(highplane, 0, 0);
			}
			List<Point> centers = null;
			if (centerfile != null)
			{
				centers = new List<Point>();
				BitmapBits cntr;
				if ((centerfile.PixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed)
					cntr = new BitmapBits(centerfile);
				else
					using (Bitmap bmp = centerfile.To32bpp())
						cntr = LoadBitmap32BppArgb(bmp, Color.Black, Color.White);
				centerfile.Dispose();
				for (int y = 0; y < cntr.Height; y++)
					for (int x = 0; x < cntr.Width; x++)
						if (cntr[x, y] != 0)
							centers.Add(new Point(x, y));
			}
			List<Rectangle> sprites = new List<Rectangle>();
			for (int y = 0; y < combinedplanes.Height; y++)
				for (int x = 0; x < combinedplanes.Width; x++)
				{
					if (combinedplanes[x, y] % 16 != 0)
					{
						Rectangle newrect = new Rectangle(x - padding, y - padding, (padding * 2) + 1, (padding * 2) + 1);
						for (int i = 0; i < sprites.Count; i++)
							if (newrect.IntersectsWith(sprites[i]))
							{
								newrect = Rectangle.Union(newrect, sprites[i]);
								sprites.RemoveAt(i);
								i = -1;
							}
						sprites.Add(newrect);
					}
				}
			List<Rectangle> rows = new List<Rectangle>();
			for (int i = 0; i < sprites.Count; i++)
			{
				Rectangle rect = sprites[i];
				rect.Inflate(-padding, -padding);
				sprites[i] = rect;
				if (rows.Count > 0 && sprites[i].IntersectsWith(rows[rows.Count - 1]))
					rows[rows.Count - 1] = Rectangle.Union(sprites[i], rows[rows.Count - 1]);
				else
					rows.Add(new Rectangle(0, sprites[i].Y, combinedplanes.Width, sprites[i].Height));
			}
			List<Rectangle> newsprites = new List<Rectangle>(sprites.Count);
			foreach (Rectangle rect in rows)
				newsprites.AddRange(sprites.Where(a => a.IntersectsWith(rect)).OrderBy(a => a.X));
			sprites = newsprites;
			List<byte[]> tiles = new List<byte[]>();
			NamedList<MappingsFrame> map = new NamedList<MappingsFrame>("Map_" + Path.GetFileNameWithoutExtension(mapfile), sprites.Count);
			NamedList<DPLCFrame> dplc = null;
			if (dplcfile != null)
				dplc = new NamedList<DPLCFrame>("DPLC_" + Path.GetFileNameWithoutExtension(dplcfile), sprites.Count);
			if (nullfirst)
			{
				map.Add(new MappingsFrame(map.Name + "_Null"));
				if (dplc != null)
					dplc.Add(new DPLCFrame(dplc.Name + "_Null"));
			}
			for (int i = 0; i < sprites.Count; i++)
			{
				Point center = new Point(sprites[i].Width / 2, sprites[i].Height / 2);
				if (centers != null)
				{
					Rectangle r = sprites[i];
					for (int j = 0; j <= searchradius; j++)
					{
						foreach (Point item in centers)
							if (sprites[i].Contains(item))
							{
								center = new Point(item.X - sprites[i].Left, item.Y - sprites[i].Top);
								goto foundcenter;
							}
						r.Inflate(1, 1);
					}
				}
				foundcenter:
				MappingsFrame mapframe = new MappingsFrame(map.Name + "_" + i);
				map.Add(mapframe);
				DPLCFrame dplcframe = null;
				if (dplc != null)
				{
					dplcframe = new DPLCFrame(dplc.Name + "_" + i);
					dplc.Add(dplcframe);
				}
				if (lowplane != null)
					SpriteToMap(sprites[i], center, lowplane, tiles, mapframe, dplcframe, startpal, false, twoplayer);
				if (highplane != null)
					SpriteToMap(sprites[i], center, highplane, tiles, mapframe, dplcframe, startpal, true, twoplayer);
			}
			using (MemoryStream ms = new MemoryStream(tiles.Count * 32))
			{
				using (BinaryWriter bw = new BinaryWriter(ms))
					foreach (byte[] b in tiles)
						bw.Write(b);
				Compression.Compress(ms.ToArray(), artfile, artcmp);
			}
			if (mapfmt == MappingsFormat.Binary)
			{
				File.WriteAllBytes(mapfile, MappingsFrame.GetBytes(map, mapgame));
				if (dplc != null)
					File.WriteAllBytes(dplcfile, DPLCFrame.GetBytes(dplc, s3kp ? EngineVersion.S2 : mapgame));
			}
			else
			{
				MappingsFrame.ToASM(mapfile, map, mapgame, mapfmt == MappingsFormat.Macro);
				if (dplc != null)
					DPLCFrame.ToASM(dplcfile, dplc, mapgame, mapfmt == MappingsFormat.Macro, s3kp);
			}
			if (palfile != null)
				using (FileStream fs = File.Create(palfile))
				using (BinaryWriter bw = new BinaryWriter(fs))
					foreach (Color c in palette)
						bw.Write(ByteConverter.GetBytes(new SonLVLColor(c).MDColor));
		}

		private static void SpriteToMap(Rectangle sprite, Point center, BitmapBits plane, List<byte[]> tiles, MappingsFrame mapframe, DPLCFrame dplcframe, byte startpal, bool pri, bool twoplayer)
		{
			BitmapBits bmp = plane.GetSection(sprite);
			if (bmp.Width % 8 != 0 || bmp.Height % 8 != 0)
			{
				int w = bmp.Width;
				if (w % 8 != 0)
					w = (w & ~7) + 8;
				int h = bmp.Height;
				if (h % 8 != 0)
					h = (h & ~7) + 8;
				BitmapBits tmp = new BitmapBits(w, h);
				tmp.DrawBitmapBounded(bmp, 0, 0);
				bmp = tmp;
			}
			int tw = bmp.Width / 8;
			int th = bmp.Height / 8;
			byte[,] tilepals = new byte[tw, th];
			for (int ty = 0; ty < th; ty++)
				for (int tx = 0; tx < tw; tx++)
				{
					tilepals[tx, ty] = 0xFF;
					int[] palcnt = new int[4];
					for (int y = 0; y < 8; y++)
						for (int x = 0; x < 8; x++)
							if (bmp[tx * 8 + x, ty * 8 + y] % 16 != 0)
								palcnt[bmp[tx * 8 + x, ty * 8 + y] / 16]++;
					if (!palcnt.FastArrayEqual(0))
					{
						tilepals[tx, ty] = 0;
						for (int i = 1; i < 4; i++)
							if (palcnt[i] > palcnt[tilepals[tx, ty]])
								tilepals[tx, ty] = (byte)i;
					}
				}
			int numtiles = 0;
			for (int ty = 0; ty < th; ty++)
				for (int tx = 0; tx < tw; tx++)
					if (tilepals[tx, ty] != 0xFF)
					{
						int w = Math.Min(4, (tw) - tx);
						for (int x = 1; x < w; x++)
							if (tilepals[tx + x, ty] != tilepals[tx, ty])
								w = x;
						int h = Math.Min(4, (th) - ty);
						for (int y = 1; y < h; y++)
							for (int x = 0; x < w; x++)
								if (tilepals[tx + x, ty + y] != tilepals[tx, ty])
									h = y;
						if (twoplayer && h % 2 == 1)
							h++;
						if (dplcframe != null)
						{
							mapframe.Tiles.Add(new MappingsTile((short)((tx * 8) - center.X), (short)((ty * 8) - center.Y), (byte)w, (byte)h, (ushort)numtiles, false, false, startpal, pri));
							dplcframe.Tiles.Add(new DPLCEntry((byte)(w * h), (ushort)tiles.Count));
						}
						else
							mapframe.Tiles.Add(new MappingsTile((short)((tx * 8) - center.X), (short)((ty * 8) - center.Y), (byte)w, (byte)h, (ushort)tiles.Count, false, false, startpal, pri));
						for (int x = 0; x < w; x++)
							for (int y = 0; y < h; y++)
								if (ty + y < th)
								{
									tiles.Add(bmp.ToTile((tx + x) * 8, (ty + y) * 8));
									tilepals[tx + x, ty + y] = 0xFF;
								}
								else
									tiles.Add(new byte[32]);
						numtiles += w * h;
						tx += w - 1;
					}
		}

		private static BitmapBits LoadBitmap32BppArgb(Bitmap bmp, params Color[] palette)
		{
			BitmapBits result = new BitmapBits(bmp.Width, bmp.Height);
			BitmapData bmpd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			byte[] bits = new byte[bmpd.Height * Math.Abs(bmpd.Stride)];
			System.Runtime.InteropServices.Marshal.Copy(bmpd.Scan0, bits, 0, bits.Length);
			for (int y = 0; y < bmpd.Height; y++)
			{
				int srcaddr = y * Math.Abs(bmpd.Stride);
				for (int x = 0; x < bmpd.Width; x++)
				{
					Color col = Color.FromArgb(BitConverter.ToInt32(bits, srcaddr + (x * 4)));
					result[x, y] = (byte)Array.IndexOf(palette, col.FindNearestMatch(palette));
					if (col.A < 128)
						result[x, y] = 0;
				}
			}
			bmp.UnlockBits(bmpd);
			return result;
		}
	}
}
