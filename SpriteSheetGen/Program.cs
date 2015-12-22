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
							background = Color.FromName(getopt.Optarg);
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
			using (Bitmap bmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
				palette = bmp.Palette;
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
			List<Sprite> sprites = new List<Sprite>(map.Count);
			if (gridsize == 0)
			{
				for (int i = 0; i < map.Count; i++)
				{
					if (map[i].TileCount == 0)
					{
						//File.AppendAllText("log.txt", "Frame " + i + " empty.\r\n");
						continue;
					}
					Sprite spr;
					if (dplc != null)
						spr = new Sprite(LevelData.MapFrameToBmp(art, map[i], dplc[i], spriteInfo.StartPalette));
					else
						spr = new Sprite(LevelData.MapFrameToBmp(art, map[i], spriteInfo.StartPalette));
					gridsize = Math.Max(gridsize, Math.Max(spr.Width, spr.Height));
				}
				if (!fixedwidth)
					width = (padding * 2 + gridsize) * columns;
			}
			int x = padding;
			int y = padding;
			int height = 0;
			int rowcnt = 0;
			int rowheight = 0;
			for (int i = 0; i < map.Count; i++)
			{
				if (map[i].TileCount == 0)
				{
					//File.AppendAllText("log.txt", "Frame " + i + " empty.\r\n");
					continue;
				}
				Sprite spr;
				if (dplc != null)
					spr = new Sprite(LevelData.MapFrameToBmp(art, map[i], dplc[i], spriteInfo.StartPalette));
				else
					spr = new Sprite(LevelData.MapFrameToBmp(art, map[i], spriteInfo.StartPalette));
				if (gridsize == -1)
				{
					if (fixedwidth && x + spr.Width + padding > width)
					{
						x = padding;
						y += rowheight;
						rowheight = 0;
					}
					spr.Offset = new System.Drawing.Point(x, y);
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
					spr.Offset = new System.Drawing.Point(x + (gridsize - spr.Width) / 2, y + (gridsize - spr.Height) / 2);
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
				sprites.Add(spr);
			}
			BitmapBits bmp2 = new BitmapBits(width, height);
			foreach (Sprite spr in sprites)
				bmp2.DrawSprite(spr, 0, 0);
			Console.Write("Save as: ");
			if (getopt.Optind + 1 >= args.Length)
				filename = Console.ReadLine();
			else
			{
				filename = args[getopt.Optind + 1];
				Console.WriteLine(filename);
			}
			bmp2.ToBitmap(palette).Save(filename);
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
