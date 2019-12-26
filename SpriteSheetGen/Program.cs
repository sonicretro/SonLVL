using GetoptNET;
using SonicRetro.SonLVL.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SpriteSheetGen
{
	class Program
	{
		static void Main(string[] args)
		{
			byte[] art;
			ColorPalette palette;
			List<MappingsFrame> map;
			List<DPLCFrame> dplc;
			SpriteInfo spriteInfo;
			LevelData.littleendian = false;
			LongOpt[] opts = new[] {
                new LongOpt("help", Argument.No, null, 'h'),
				new LongOpt("padding", Argument.Required, null, 'p'),
                new LongOpt("columns", Argument.Required, null, 'c'),
				new LongOpt("width", Argument.Required, null, 'w'),
				new LongOpt("background", Argument.Required, null, 'b'),
				new LongOpt("grid", Argument.No, null, 'g')
            };
			Getopt getopt = new Getopt("SpriteSheetGen", args, Getopt.digest(opts), opts);
			int padding = 2;
			int columns = 8;
			int width = 0;
			bool fixedwidth = false;
			int gridsize = -1;
			Color background = Color.Transparent;
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
					case 'c':
						columns = int.Parse(getopt.Optarg);
						break;
					case 'w':
						width = int.Parse(getopt.Optarg);
						columns = -1;
						fixedwidth = true;
						break;
					case 'g':
						gridsize = 0;
						break;
					case 'b':
						if (getopt.Optarg.StartsWith("#"))
							background = Color.FromArgb((int)(0xFF000000 | uint.Parse(getopt.Optarg.Substring(1), System.Globalization.NumberStyles.HexNumber)));
						else
							background = Color.FromName(getopt.Optarg.Replace(" ", ""));
						break;
				}
				opt = getopt.getopt();
			}
			string filename;
			Console.Write("File: ");
			if (getopt.Optind == args.Length)
				filename = Console.ReadLine();
			else
			{
				filename = args[getopt.Optind];
				Console.WriteLine(filename);
			}
			filename = Path.GetFullPath(filename);
			Environment.CurrentDirectory = Path.GetDirectoryName(filename);
			spriteInfo = IniSerializer.Deserialize<SpriteInfo>(filename);
			if (spriteInfo.MappingsGame == EngineVersion.Invalid)
				spriteInfo.MappingsGame = spriteInfo.Game;
			if (spriteInfo.DPLCGame == EngineVersion.Invalid)
				spriteInfo.DPLCGame = spriteInfo.Game;
			MultiFileIndexer<byte> tiles = new MultiFileIndexer<byte>();
			foreach (SonicRetro.SonLVL.API.FileInfo file in spriteInfo.Art)
				tiles.AddFile(new List<byte>(Compression.Decompress(file.Filename, spriteInfo.ArtCompression)), file.Offset);
			art = tiles.ToArray();
			tiles.Clear();
			byte[] tmp = null;
			using (Bitmap palbmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
				palette = palbmp.Palette;
			for (int i = 0; i < 256; i++)
				palette.Entries[i] = Color.Black;
			foreach (PaletteInfo palent in spriteInfo.Palette)
			{
				tmp = File.ReadAllBytes(palent.Filename);
				SonLVLColor[] palfile;
				palfile = new SonLVLColor[tmp.Length / 2];
				for (int pi = 0; pi < tmp.Length; pi += 2)
					palfile[pi / 2] = new SonLVLColor(SonicRetro.SonLVL.API.ByteConverter.ToUInt16(tmp, pi));
				for (int pa = 0; pa < palent.Length; pa++)
					palette.Entries[pa + palent.Destination] = palfile[pa + palent.Source].RGBColor;
			}
			palette.Entries[0] = background;
			Dictionary<string, int> labels = new Dictionary<string, int>();
			tmp = null;
			switch (spriteInfo.MappingsFormat)
			{
				case MappingsFormat.Binary:
					tmp = File.ReadAllBytes(spriteInfo.MappingsFile);
					break;
				case MappingsFormat.ASM:
				case MappingsFormat.Macro:
					tmp = LevelData.ASMToBin(spriteInfo.MappingsFile, spriteInfo.MappingsGame, out labels);
					break;
			}
			map = MappingsFrame.Load(tmp, spriteInfo.MappingsGame, labels);
			if (!string.IsNullOrEmpty(spriteInfo.DPLCFile))
			{
				labels = new Dictionary<string, int>();
				tmp = null;
				switch (spriteInfo.DPLCFormat)
				{
					case MappingsFormat.Binary:
						tmp = File.ReadAllBytes(spriteInfo.DPLCFile);
						break;
					case MappingsFormat.ASM:
					case MappingsFormat.Macro:
						tmp = LevelData.ASMToBin(spriteInfo.DPLCFile, spriteInfo.DPLCGame, out labels);
						break;
				}
				dplc = DPLCFrame.Load(tmp, spriteInfo.DPLCGame, labels);
			}
			else
				dplc = null;
			List<BitmapBits> spritesLow = new List<BitmapBits>(map.Count);
			List<BitmapBits> spritesHigh = new List<BitmapBits>(map.Count);
			List<BitmapBits> spritesMerged = new List<BitmapBits>(map.Count);
			List<Point> offsets = new List<Point>(map.Count);
			List<Point> centers = new List<Point>(map.Count);
			for (int i = 0; i < map.Count; i++)
			{
				if (map[i].TileCount == 0)
				{
					//File.AppendAllText("log.txt", "Frame " + i + " empty.\r\n");
					continue;
				}
				Sprite spr;
				if (dplc != null)
					spr = LevelData.MapFrameToBmp(art, map[i], dplc[i], spriteInfo.StartPalette);
				else
					spr = LevelData.MapFrameToBmp(art, map[i], spriteInfo.StartPalette);
				BitmapBits sprLow = spr.GetBitmapLow();
				BitmapBits sprHigh = spr.GetBitmapHigh();
				BitmapBits sprMerged = spr.GetBitmap();
				int cx = -spr.X;
				int cy = -spr.Y;
				for (int _x = 0; _x < sprMerged.Width; _x++)
					for (int _y = 0; _y < sprMerged.Height; _y++)
						if (sprMerged[_x, _y] != 0)
						{
							sprMerged = sprMerged.GetSection(_x, 0, sprMerged.Width - _x, sprMerged.Height);
							sprLow = sprLow.GetSection(_x, 0, sprLow.Width - _x, sprLow.Height);
							sprHigh = sprHigh.GetSection(_x, 0, sprHigh.Width - _x, sprHigh.Height);
							cx -= _x;
							goto checkright;
						}
			checkright:
				for (int _x = sprMerged.Width - 1; _x >= 0; _x--)
					for (int _y = 0; _y < sprMerged.Height; _y++)
						if (sprMerged[_x, _y] != 0)
						{
							sprMerged = sprMerged.GetSection(0, 0, _x + 1, sprMerged.Height);
							sprLow = sprLow.GetSection(0, 0, _x + 1, sprLow.Height);
							sprHigh = sprHigh.GetSection(0, 0, _x + 1, sprHigh.Height);
							goto checktop;
						}
			checktop:
				for (int _y = 0; _y < sprMerged.Height; _y++)
					for (int _x = 0; _x < sprMerged.Width; _x++)
						if (sprMerged[_x, _y] != 0)
						{
							sprMerged = sprMerged.GetSection(0, _y, sprMerged.Width, sprMerged.Height - _y);
							sprLow = sprLow.GetSection(0, _y, sprLow.Width, sprLow.Height - _y);
							sprHigh = sprHigh.GetSection(0, _y, sprHigh.Width, sprHigh.Height - _y);
							cy -= _y;
							goto checkbottom;
						}
			checkbottom:
				for (int _y = sprMerged.Height - 1; _y >= 0; _y--)
					for (int _x = 0; _x < sprMerged.Width; _x++)
						if (sprMerged[_x, _y] != 0)
						{
							sprMerged = sprMerged.GetSection(0, 0, sprMerged.Width, _y + 1);
							sprLow = sprLow.GetSection(0, 0, sprLow.Width, _y + 1);
							sprHigh = sprHigh.GetSection(0, 0, sprHigh.Width, _y + 1);
							goto checkdone;
						}
			checkdone:
				spritesMerged.Add(sprMerged);
				spritesLow.Add(sprLow);
				spritesHigh.Add(sprHigh);
				centers.Add(new Point(cx, cy));
			}
			if (gridsize == 0)
			{
				for (int i = 0; i < spritesMerged.Count; i++)
					gridsize = Math.Max(gridsize, Math.Max(spritesMerged[i].Width, spritesMerged[i].Height));
				if (!fixedwidth)
					width = (padding * 2 + gridsize) * columns;
			}
			int x = padding;
			int y = padding;
			int height = 0;
			int rowcnt = 0;
			int rowheight = 0;
			for (int i = 0; i < spritesMerged.Count; i++)
			{
				BitmapBits spr = spritesMerged[i];
				Point off;
				if (gridsize == -1)
				{
					if (fixedwidth && x + spr.Width + padding > width)
					{
						x = padding;
						y += rowheight;
						rowheight = 0;
					}
					off = new System.Drawing.Point(x, y);
					centers[i] = new Point(centers[i].X + off.X, centers[i].Y + off.Y);
					if (!fixedwidth)
						width = Math.Max(width, x + spr.Width + padding);
					height = Math.Max(height, y + spr.Height + padding);
					if (spr.Height + 2 * padding > rowheight)
						rowheight = spr.Height + 2 * padding;
					if (!fixedwidth && ++rowcnt == columns)
					{
						x = padding;
						y += rowheight;
						rowcnt = 0;
						rowheight = 0;
					}
					else
						x += spr.Width + 2 * padding;
				}
				else
				{
					if (fixedwidth && x + gridsize + padding > width)
					{
						x = padding;
						y += gridsize + 2 * padding;
					}
					off = new Point(x + (gridsize - spr.Width) / 2, y + (gridsize - spr.Height) / 2);
					centers[i] = new Point(centers[i].X + off.X, centers[i].Y + off.Y);
					height = Math.Max(height, y + gridsize + padding);
					if (!fixedwidth && ++rowcnt == columns)
					{
						x = padding;
						y += gridsize + 2 * padding;
						rowcnt = 0;
					}
					else
						x += gridsize + 2 * padding;
				}
				offsets.Add(off);
			}
			BitmapBits bmpMerged = new BitmapBits(width, height);
			for (int i = 0; i < spritesMerged.Count; i++)
				bmpMerged.DrawBitmap(spritesMerged[i], offsets[i]);
			BitmapBits bmpLow = new BitmapBits(width, height);
			for (int i = 0; i < spritesLow.Count; i++)
				bmpLow.DrawBitmap(spritesLow[i], offsets[i]);
			BitmapBits bmpHigh = new BitmapBits(width, height);
			for (int i = 0; i < spritesHigh.Count; i++)
				bmpHigh.DrawBitmap(spritesHigh[i], offsets[i]);
			BitmapBits bmpCenter = new BitmapBits(width, height);
			for (int i = 0; i < centers.Count; i++)
				bmpCenter[centers[i].X, centers[i].Y] = 1;
			Console.Write("Save as: ");
			if (getopt.Optind + 1 >= args.Length)
				filename = Console.ReadLine();
			else
			{
				filename = args[getopt.Optind + 1];
				Console.WriteLine(filename);
			}
			filename = Path.GetFullPath(filename);
			bmpMerged.ToBitmap(palette).Save(filename);
			if (!bmpLow.Bits.FastArrayEqual(0) && !bmpHigh.Bits.FastArrayEqual(0))
			{
				bmpLow.ToBitmap(palette).Save(Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename) + "_low" + Path.GetExtension(filename)));
				bmpHigh.ToBitmap(palette).Save(Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename) + "_high" + Path.GetExtension(filename)));
			}
			bmpCenter.ToBitmap1bpp(Color.Black, Color.White).Save(Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename) + "_center" + Path.GetExtension(filename)));
		}
	}

	public class SpriteInfo
	{
		[DefaultValue(EngineVersion.S2)]
		[IniName("game")]
		public EngineVersion Game { get; set; }
		[IniName("art")]
		[IniCollection(IniCollectionMode.SingleLine, Format = "|")]
		public SonicRetro.SonLVL.API.FileInfo[] Art { get; set; }
		[IniName("artcmp")]
		[DefaultValue(CompressionType.Nemesis)]
		public CompressionType ArtCompression { get; set; }
		[IniName("palette")]
		public PaletteList Palette { get; set; }
		[IniName("map")]
		public string MappingsFile { get; set; }
		[IniName("mapgame")]
		public EngineVersion MappingsGame { get; set; }
		[IniName("mapfmt")]
		public MappingsFormat MappingsFormat { get; set; }
		[IniName("dplc")]
		public string DPLCFile { get; set; }
		[IniName("dplcgame")]
		public EngineVersion DPLCGame { get; set; }
		[IniName("dplcfmt")]
		public MappingsFormat DPLCFormat { get; set; }
		[IniName("startpal")]
		public int StartPalette { get; set; }
	}
}
