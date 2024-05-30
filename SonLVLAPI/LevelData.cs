using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SonicRetro.SonLVL.API
{
	public static class LevelData
	{
		public static GameInfo Game;
		public static LevelInfo Level;
		public static MultiFileIndexer<byte[]> Tiles;
		public static byte[] TileArray;
		public static MultiFileIndexer<Block> Blocks;
		public static List<BitmapBits[]> BlockBmpBits;
		public static List<BitmapBits> CompBlockBmpBits;
		public static List<Bitmap[]> BlockBmps;
		public static List<Bitmap> CompBlockBmps;
		public static MultiFileIndexer<Chunk> Chunks;
		public static List<Sprite> ChunkSprites;
		public static List<Bitmap[]> ChunkBmps;
		public static List<Bitmap> CompChunkBmps;
		public static LayoutData Layout;
		public static Dictionary<string, KeyValuePair<CompressionType, LayoutData>> AdditionalLayouts;
		public static LayoutFormat LayoutFormat;
		public static List<string> PalName;
		public static List<SonLVLColor[,]> Palette;
		public static List<byte[,]> PalNum;
		public static List<int[,]> PalAddr;
		public static int CurPal;
		public static ColorPalette BmpPal;
		public static List<ObjectEntry> Objects;
		public static bool objectterm;
		public static ObjectLayoutFormat ObjectFormat;
		public static List<RingEntry> Rings;
		public static bool ringstartterm, ringendterm;
		public static RingFormat RingFormat;
		public static List<ExtraObjEntry> ExtraObjects;
		public static List<StartPositionEntry> StartPositions;
		public static Dictionary<string, ObjectData> INIObjDefs;
		public static Dictionary<byte, ObjectDefinition> ObjTypes;
		public static ObjectDefinition unkobj;
		public static List<StartPositionDefinition> StartPosDefs;
		public static bool littleendian;
		public static Dictionary<string, byte[]> filecache;
		public static List<byte> ColInds1;
		public static List<byte> ColInds2;
		public static sbyte[][] ColArr1;
		public static byte[] Angles;
		public static BitmapBits[] ColBmpBits;
		public static Bitmap[] ColBmps;
		public static List<BitmapBits[]> ChunkColBmpBits;
		public static List<Bitmap[]> ChunkColBmps;
		public static MultiFileIndexer<byte[]> AnimatedTiles;
		public static List<BitmapBits[]> AnimatedBlockBmpBits;
		public static int AnimatedBlockOffset;
		public static Bitmap UnknownImg;
		public static Sprite UnknownSprite;
		public static List<Sprite> Sprites;
		public static int WaterPalette;
		public static ushort WaterHeight = 0x600;
		public delegate void LogEventHandler(params string[] message);
		public static event LogEventHandler LogEvent = delegate { };
		public static event Action PaletteChangedEvent = delegate { };
		internal static readonly bool IsMonoRuntime = Type.GetType("Mono.Runtime") != null;
		internal static readonly bool IsWindows = !(Environment.OSVersion.Platform == PlatformID.MacOSX | Environment.OSVersion.Platform == PlatformID.Unix | Environment.OSVersion.Platform == PlatformID.Xbox);
		private static readonly BitmapBits InvalidTile = new BitmapBits(8, 8);
		private static readonly BitmapBits[] InvalidBlock = new BitmapBits[] { new BitmapBits(16, 16), new BitmapBits(16, 16) };
		public const int ColorTransparent = 0;
		public const int ColorWhite = 16;
		public const int ColorYellow = 32;
		public const int ColorBlack = 48;

		static LevelData()
		{
			InvalidTile.DrawLine(15, 0, 0, 7, 0);
			InvalidTile.DrawLine(15, 0, 0, 0, 7);
			InvalidTile.DrawLine(15, 7, 7, 0, 7);
			InvalidTile.DrawLine(15, 7, 7, 7, 0);
			InvalidTile.DrawLine(15, 0, 0, 7, 7);
			InvalidTile.DrawLine(15, 0, 7, 7, 0);
			InvalidBlock[0].DrawLine(15, 0, 0, 15, 0);
			InvalidBlock[0].DrawLine(15, 0, 0, 0, 15);
			InvalidBlock[0].DrawLine(15, 15, 15, 0, 15);
			InvalidBlock[0].DrawLine(15, 15, 15, 15, 0);
			InvalidBlock[0].DrawLine(15, 0, 0, 15, 15);
			InvalidBlock[0].DrawLine(15, 0, 15, 15, 0);
		}

		public static void LoadGame(string filename)
		{
			Log("Opening INI file \"" + filename + "\"...");
			Game = GameInfo.Load(filename);
			Environment.CurrentDirectory = Path.GetDirectoryName(filename);
			if(Game.UnknownImgPath != null)
			{
				UnknownImg = new Bitmap(Game.UnknownImgPath);
			}
			else switch (Game.EngineVersion)
			{
				case EngineVersion.S1:
				case EngineVersion.SCD:
				case EngineVersion.S2:
				case EngineVersion.S2NA:
					UnknownImg = Properties.Resources.UnknownImg.Copy();
					break;
				case EngineVersion.SCDPC:
					UnknownImg = Properties.Resources.UnknownImg.Copy();
					littleendian = true;
					break;
				case EngineVersion.S3K:
					UnknownImg = Properties.Resources.UnknownImg3K.Copy();
					break;
				case EngineVersion.SKC:
					UnknownImg = Properties.Resources.UnknownImg3K.Copy();
					littleendian = true;
					break;
				case EngineVersion.Custom:
					throw new NotImplementedException("Engine is custom, and \"unknownimg\" is missing in INI file.");
				default:
					throw new NotImplementedException("Game type " + Game.EngineVersion.ToString() + " is not supported!");
			}
			UnknownSprite = new Sprite(new BitmapBits(UnknownImg), true, -8, -7);
			Log("Game type is " + Game.EngineVersion.ToString() + ".");
		}

		public static void LoadLevel(string levelname, bool loadGraphics)
		{
			System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
			stopwatch.Start();
			Level = Game.GetLevelInfo(levelname);
			switch (Level.EngineVersion)
			{
				case EngineVersion.S1:
				case EngineVersion.SCD:
				case EngineVersion.S2:
				case EngineVersion.S2NA:
					UnknownImg = Properties.Resources.UnknownImg.Copy();
					littleendian = false;
					break;
				case EngineVersion.SCDPC:
					UnknownImg = Properties.Resources.UnknownImg.Copy();
					littleendian = true;
					break;
				case EngineVersion.S3K:
					UnknownImg = Properties.Resources.UnknownImg3K.Copy();
					littleendian = false;
					break;
				case EngineVersion.SKC:
					UnknownImg = Properties.Resources.UnknownImg3K.Copy();
					littleendian = true;
					break;
				case EngineVersion.Custom:
					if (Game.UnknownImgPath == null)
						throw new NotImplementedException("Engine is custom, and \"unknownimg\" is missing in INI file.");
					break;
				default:
					throw new NotImplementedException("Game type " + Level.EngineVersion.ToString() + " is not supported!");
			}
			// support for custom unknown images
			if(Game.UnknownImgPath != null)
				UnknownImg = new Bitmap(Game.UnknownImgPath);
			UnknownSprite = new Sprite(new BitmapBits(UnknownImg), true, -8, -7);
			Log("Loading " + Level.DisplayName + "...");
			if ((Level.ChunkWidth & 15) != 0)
				throw new ArgumentException("Chunk width must be divisible by 16!");
			if ((Level.ChunkHeight & 15) != 0)
				throw new ArgumentException("Chunk height must be divisible by 16!");
				if (!Directory.Exists("dllcache"))
				{
					DirectoryInfo dir = Directory.CreateDirectory("dllcache");
					dir.Attributes |= FileAttributes.Hidden;
				}
			byte[] tmp = null;
#if !DEBUG
			if (Level.EngineVersion != EngineVersion.SKC)
				Parallel.Invoke(LoadLevelTiles, LoadLevelBlocks, LoadLevelChunks, () => LoadLevelLayout(levelname), LoadLevelPalette,
					LoadLevelColInds, LoadLevelColArr, LoadLevelAngles);
			else
			{
				Parallel.Invoke(LoadLevelTiles, () => LoadLevelLayout(levelname), LoadLevelPalette,
					LoadLevelColInds, LoadLevelColArr, LoadLevelAngles);
				LoadLevelBlocks();
				LoadLevelChunks();
			}
#else
			LoadLevelTiles();
			LoadLevelBlocks();
			LoadLevelChunks();
			LoadLevelLayout(levelname);
			LoadLevelPalette();
			LoadLevelColInds();
			LoadLevelColArr();
			LoadLevelAngles();
#endif
			if (ColInds1 != null && Level.EngineVersion != EngineVersion.S3K && Level.EngineVersion != EngineVersion.SKC)
			{
				if (ColInds1.Count < Blocks.Count)
					ColInds1.AddRange(new byte[Blocks.Count - ColInds1.Count]);
				if (ColInds2.Count < Blocks.Count)
					ColInds2.AddRange(new byte[Blocks.Count - ColInds2.Count]);
			}
			switch (Level.ObjectFormat)
			{
				case EngineVersion.S1:
					ObjectFormat = new S1.Object();
					break;
				case EngineVersion.S2:
				case EngineVersion.S2NA:
					ObjectFormat = new S2.Object();
					break;
				case EngineVersion.S3K:
				case EngineVersion.SKC:
					ObjectFormat = new S3K.Object();
					break;
				case EngineVersion.SCD:
				case EngineVersion.SCDPC:
					ObjectFormat = new SCD.Object();
					break;
				case EngineVersion.Chaotix:
					ObjectFormat = new Chaotix.Object();
					break;
				case EngineVersion.Custom:
					ObjectFormat = CompileCodeFile<ObjectLayoutFormat>(Level.ObjectCodeFile, Level.ObjectCodeType);
					break;
			}
			switch (Level.RingFormat)
			{
				case EngineVersion.S1:
					RingFormat = new S1.Ring();
					break;
				case EngineVersion.SCD:
				case EngineVersion.SCDPC:
					RingFormat = new SCD.Ring();
					break;
				case EngineVersion.S2:
				case EngineVersion.S2NA:
					RingFormat = new S2.Ring();
					break;
				case EngineVersion.S3K:
				case EngineVersion.SKC:
					RingFormat = new S3K.Ring();
					break;
				case EngineVersion.Custom:
					RingFormat = CompileCodeFile<RingFormat>(Level.RingCodeFile, Level.RingCodeType);
					break;
			}
			Sprites = new List<Sprite>();
			if (loadGraphics)
			{
				Bitmap palbmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed);
				BmpPal = palbmp.Palette;
				for (int i = 0; i < 64; i++)
					BmpPal.Entries[i] = PaletteToColor(i / 16, i % 16, true);
				for (int i = 64; i < 256; i++)
					BmpPal.Entries[i] = Color.Black;
				BmpPal.Entries[ColorWhite] = Color.White;
				BmpPal.Entries[ColorYellow] = Color.Yellow;
				BmpPal.Entries[ColorBlack] = Color.Black;
				BmpPal.Entries[ColorWhite | 0x80] = Color.White;
				BmpPal.Entries[ColorYellow | 0x80] = Color.Yellow;
				BmpPal.Entries[ColorBlack | 0x80] = Color.Black;
				if (Level.ExtraColors != null)
					foreach (PaletteInfo palent in Level.ExtraColors)
						if (File.Exists(palent.Filename))
						{
							Log("Loading extra color file \"" + palent.Filename + "\"...", "Source: " + palent.Source + " Destination: " + palent.Destination + " Length: " + palent.Length);
							SonLVLColor[] palfile = SonLVLColor.Load(palent.Filename, Level.PaletteFormat);
							for (int pa = 0; pa < palent.Length; pa++)
								BmpPal.Entries[pa + palent.Destination + 0x40] = palfile[pa + palent.Source].RGBColor;
						}
				if (Level.ExtraWaterColors != null)
					foreach (PaletteInfo palent in Level.ExtraWaterColors)
						if (File.Exists(palent.Filename))
						{
							Log("Loading extra water color file \"" + palent.Filename + "\"...", "Source: " + palent.Source + " Destination: " + palent.Destination + " Length: " + palent.Length);
							SonLVLColor[] palfile = SonLVLColor.Load(palent.Filename, Level.PaletteFormat);
							for (int pa = 0; pa < palent.Length; pa++)
								BmpPal.Entries[pa + palent.Destination + 0xC0] = palfile[pa + palent.Source].RGBColor;
						}
				UnknownImg.Palette = BmpPal;
				if (Level.Sprites != null)
				{
					tmp = Compression.Decompress(Level.Sprites, CompressionType.SZDD);
					int numspr = ByteConverter.ToInt32(tmp, 8);
					int taddr = ByteConverter.ToInt32(tmp, 0xC);
					for (int i = 0; i < numspr; i++)
					{
						ushort width = ByteConverter.ToUInt16(tmp, 0x10 + (i * 0xC) + 4);
						if ((width & 4) == 4)
							width += 4;
						ushort height = ByteConverter.ToUInt16(tmp, 0x10 + (i * 0xC) + 6);
						ushort startcol = (ushort)(ByteConverter.ToUInt16(tmp, 0x10 + (i * 0xC) + 8) - 0x10);
						BitmapBits bmp = new BitmapBits(width, height);
						byte[] til = new byte[height * (width / 2)];
						Array.Copy(tmp, taddr, til, 0, til.Length);
						taddr += til.Length;
						LoadBitmap4BppIndexed(bmp, til, width / 2);
						bmp.IncrementIndexes(startcol);
						Sprites.Add(new Sprite(bmp, new Point(ByteConverter.ToInt16(tmp, 0x10 + (i * 0xC) + 0), ByteConverter.ToInt16(tmp, 0x10 + (i * 0xC) + 2))));
					}
				}
				INIObjDefs = new Dictionary<string, ObjectData>();
				ObjTypes = new Dictionary<byte, ObjectDefinition>();
				filecache = new Dictionary<string, byte[]>();
				unkobj = new DefaultObjectDefinition();
				unkobj.Init(new ObjectData());
				if (Game.ObjectList != null)
					foreach (string item in Game.ObjectList)
						LoadObjectDefinitionFile(item);
				if (Level.ObjectList != null)
					foreach (string item in Level.ObjectList)
						LoadObjectDefinitionFile(item);
				InitObjectDefinitions();
			}
#if !DEBUG
			Parallel.Invoke(() => LoadLevelObjects(loadGraphics), () => LoadLevelRings(loadGraphics),
				() => LoadLevelExtraObjects(loadGraphics), () => LoadLevelStartPositions(loadGraphics));
#else
			LoadLevelObjects(loadGraphics);
			LoadLevelRings(loadGraphics);
			LoadLevelExtraObjects(loadGraphics);
			LoadLevelStartPositions(loadGraphics);
#endif
			if (loadGraphics)
			{
				if (Level.AnimatedTiles != null)
				{
					AnimatedTiles = new MultiFileIndexer<byte[]>();
					foreach (AnimatedTileInfo info in Level.AnimatedTiles)
					{
						tmp = File.ReadAllBytes(info.Filename);
						List<byte[]> tiles = new List<byte[]>();
						for (int i = 0; i < info.Length * 32; i += 32)
						{
							byte[] tile = new byte[32];
							Array.Copy(tmp, i + (info.Source * 32), tile, 0, 32);
							tiles.Add(tile);
						}
						AnimatedTiles.AddFile(tiles, info.Destination);
					}
				}
				else
					AnimatedTiles = null;
				if (Level.AnimatedBlocks != null)
				{
					AnimatedBlockBmpBits = new List<BitmapBits[]>();
					tmp = File.ReadAllBytes(Level.AnimatedBlocks);
					AnimatedBlockOffset = ByteConverter.ToUInt16(tmp, 0) / Block.Size;
					int end = ((ByteConverter.ToUInt16(tmp, 2) + 1) * 2) + 4;
					for (int i = 4; i < end; i += Block.Size)
					{
						Block blk = new Block(tmp, i);
						BitmapBits[] bmp = new BitmapBits[2];
						bmp[0] = new BitmapBits(16, 16);
						bmp[1] = new BitmapBits(16, 16);
						for (int by = 0; by < 2; by++)
							for (int bx = 0; bx < 2; bx++)
							{
								PatternIndex pt = blk.Tiles[bx, by];
								int pr = pt.Priority ? 1 : 0;
								BitmapBits tile = TileToBmp8bpp(AnimatedTiles?[pt.Tile] ?? Tiles[pt.Tile], 0, pt.Palette);
								tile.Flip(pt.XFlip, pt.YFlip);
								bmp[pr].DrawBitmap(tile, bx * 8, by * 8);
							}
						AnimatedBlockBmpBits.Add(bmp);
					}
				}
				else
					AnimatedBlockBmpBits = null;
#if !DEBUG
				Parallel.Invoke(() =>
				{
#endif
					BlockBmps = new List<Bitmap[]>(Blocks.Count);
					BlockBmpBits = new List<BitmapBits[]>(Blocks.Count);
					CompBlockBmps = new List<Bitmap>(Blocks.Count);
					CompBlockBmpBits = new List<BitmapBits>(Blocks.Count);
					for (int bi = 0; bi < Blocks.Count; bi++)
					{
						BlockBmps.Add(new Bitmap[2]);
						BlockBmpBits.Add(new BitmapBits[2]);
						CompBlockBmps.Add(null);
						CompBlockBmpBits.Add(null);
					}
#if !DEBUG
				},
				() =>
				{
#endif
					ColBmps = new Bitmap[256];
					ColBmpBits = new BitmapBits[256];
#if !DEBUG
				},
				() =>
				{
#endif
					ChunkBmps = new List<Bitmap[]>(Chunks.Count);
					ChunkSprites = new List<Sprite>(Chunks.Count);
					ChunkColBmps = new List<Bitmap[]>(Chunks.Count);
					ChunkColBmpBits = new List<BitmapBits[]>(Chunks.Count);
					CompChunkBmps = new List<Bitmap>(Chunks.Count);
					for (int ci = 0; ci < Chunks.Count; ci++)
					{
						ChunkBmps.Add(new Bitmap[2]);
						ChunkSprites.Add(null);
						ChunkColBmps.Add(new Bitmap[2]);
						ChunkColBmpBits.Add(new BitmapBits[2]);
						CompChunkBmps.Add(null);
					}
#if !DEBUG
				});
#endif
				Log("Drawing block bitmaps...");
#if !DEBUG
				Parallel.ForEach(Partitioner.Create(0, Blocks.Count), range =>
				{
					for (int bi = range.Item1; bi < range.Item2; bi++)
#else
				for (int bi = 0; bi < Blocks.Count; bi++)
#endif
						RedrawBlock(bi, false);
#if !DEBUG
				});
				Parallel.ForEach(Partitioner.Create(0, 256), range =>
				{
					for (int ci = range.Item1; ci < range.Item2; ci++)
#else
				for (int ci = 0; ci < 256; ci++)
#endif
						RedrawCol(ci, false);
#if !DEBUG
				});
#endif
				Log("Drawing chunk bitmaps...");
#if !DEBUG
				Parallel.ForEach(Partitioner.Create(0, Chunks.Count), range =>
				{
					for (int ci = range.Item1; ci < range.Item2; ci++)
#else
				for (int ci = 0; ci < Chunks.Count; ci++)
#endif
						RedrawChunk(ci);
#if !DEBUG
				});
#endif
			}
			stopwatch.Stop();
			Log($"Level loaded in {stopwatch.Elapsed.TotalSeconds} second(s).");
		}

		private static void LoadLevelTiles()
		{
			Tiles = new MultiFileIndexer<byte[]>(() => new byte[32]);
			if (Level.Tiles == null || string.IsNullOrWhiteSpace(Level.Tiles[0].Filename))
				throw new FormatException("Level must contain at least one Tiles file!");
			if (Level.TileFormat != EngineVersion.SCDPC)
			{
				foreach (FileInfo tileent in Level.Tiles)
				{
					if (File.Exists(tileent.Filename))
					{
						Log("Loading 8x8 tiles from file \"" + tileent.Filename + "\", using compression " + Level.TileCompression.ToString() + "...");
						byte[] tmp = Compression.Decompress(tileent.Filename, Level.TileCompression);
						Pad(ref tmp, 32);
						List<byte[]> tiles = new List<byte[]>();
						for (int i = 0; i < tmp.Length; i += 32)
						{
							byte[] tile = new byte[32];
							Array.Copy(tmp, i, tile, 0, 32);
							tiles.Add(tile);
						}
						Tiles.AddFile(tiles, tileent.Offset == -1 ? tileent.Offset : tileent.Offset / 32);
					}
					else
					{
						Log("8x8 tile file \"" + tileent.Filename + "\" not found.");
						Tiles.AddFile(new List<byte[]>() { new byte[32] }, tileent.Offset == -1 ? tileent.Offset : tileent.Offset / 32);
					}
				}
			}
			else
			{
				Level.TileCompression = CompressionType.SZDD;
				if (File.Exists(Level.Tiles[0].Filename))
				{
					Log("Loading 8x8 tiles from file \"" + Level.Tiles[0].Filename + "\", using compression SZDD...");
					byte[] tmp = Compression.Decompress(Level.Tiles[0].Filename, CompressionType.SZDD);
					int sta = ByteConverter.ToInt32(tmp, 0xC);
					int numt = ByteConverter.ToInt32(tmp, 8);
					List<byte[]> tiles = new List<byte[]>();
					for (int i = 0; i < numt; i++)
					{
						byte[] tile = new byte[32];
						Array.Copy(tmp, sta, tile, 0, 32);
						tiles.Add(tile);
						sta += 32;
					}
					Tiles.AddFile(tiles, -1);
				}
				else
				{
					Log("8x8 tile file \"" + Level.Tiles[0].Filename + "\" not found.");
					Tiles.AddFile(new List<byte[]>() { new byte[32] }, -1);
				}
			}
			Tiles.FillGaps();
			UpdateTileArray();
		}

		private static void LoadLevelBlocks()
		{
			Blocks = new MultiFileIndexer<Block>(() => new Block());
			if (Level.Blocks == null || string.IsNullOrWhiteSpace(Level.Blocks[0].Filename))
				throw new FormatException("Level must contain at least one Blocks file!");
			foreach (FileInfo tileent in Level.Blocks)
			{
				if (File.Exists(tileent.Filename))
				{
					Log("Loading 16x16 blocks from file \"" + tileent.Filename + "\", using compression " + Level.BlockCompression.ToString() + "...");
					byte[] tmp = Compression.Decompress(tileent.Filename, Level.BlockCompression);
					Pad(ref tmp, Block.Size);
					List<Block> tmpblk = new List<Block>();
					if (Level.EngineVersion == EngineVersion.SKC)
						littleendian = false;
					for (int ba = 0; ba < tmp.Length; ba += Block.Size)
						tmpblk.Add(new Block(tmp, ba));
					if (Level.EngineVersion == EngineVersion.SKC)
						littleendian = true;
					Blocks.AddFile(tmpblk, tileent.Offset == -1 ? tileent.Offset : tileent.Offset / Block.Size);
				}
				else
				{
					Log("16x16 block file \"" + tileent.Filename + "\" not found.");
					Blocks.AddFile(new List<Block>() { new Block() }, tileent.Offset == -1 ? tileent.Offset : tileent.Offset / Block.Size);
				}
			}
			if (Blocks.Count == 0)
				Blocks.AddFile(new List<Block>() { new Block() }, -1);
			Blocks.FillGaps();
		}

		private static void LoadLevelChunks()
		{
			Chunks = new MultiFileIndexer<Chunk>(() => new Chunk());
			if (Level.Chunks == null || string.IsNullOrWhiteSpace(Level.Chunks[0].Filename))
				throw new FormatException("Level must contain at least one Chunks file!");
			int fileind = 0;
			foreach (FileInfo tileent in Level.Chunks)
			{
				if (File.Exists(tileent.Filename))
				{
					Log("Loading " + Level.ChunkWidth + "x" + Level.ChunkHeight + " chunks from file \"" + tileent.Filename + "\", using compression " + Level.ChunkCompression.ToString() + "...");
					byte[] tmp = Compression.Decompress(tileent.Filename, Level.ChunkCompression);
					Pad(ref tmp, Chunk.Size);
					List<Chunk> tmpchnk = new List<Chunk>();
					if (fileind == 0)
					{
						switch (Level.ChunkFormat)
						{
							case EngineVersion.S1:
							case EngineVersion.SCD:
							case EngineVersion.SCDPC:
								tmpchnk.Add(new Chunk());
								break;
						}
					}
					if (Level.EngineVersion == EngineVersion.SKC)
						littleendian = false;
					for (int ba = 0; ba < tmp.Length; ba += Chunk.Size)
						tmpchnk.Add(new Chunk(tmp, ba));
					if (Level.EngineVersion == EngineVersion.SKC)
						littleendian = true;
					Chunks.AddFile(tmpchnk, tileent.Offset == -1 ? tileent.Offset : tileent.Offset / Chunk.Size);
					fileind++;
				}
				else
				{
					Log(Level.ChunkWidth + "x" + Level.ChunkHeight + " chunk file \"" + tileent.Filename + "\" not found.");
					Chunks.AddFile(new List<Chunk>() { new Chunk() }, tileent.Offset == -1 ? tileent.Offset : tileent.Offset / Chunk.Size);
				}
			}
			if (Chunks.Count == 0)
				Chunks.AddFile(new List<Chunk>() { new Chunk() }, -1);
			Chunks.FillGaps();
		}

		private static void LoadLevelLayout(string levelname)
		{
			Layout = new LayoutData();
			AdditionalLayouts = new Dictionary<string, KeyValuePair<CompressionType, LayoutData>>();
			switch (Level.LayoutFormat)
			{
				case EngineVersion.S1:
				case EngineVersion.SCD:
					LayoutFormat = new S1.Layout();
					break;
				case EngineVersion.S2NA:
					LayoutFormat = new S2NA.Layout();
					break;
				case EngineVersion.S2:
					LayoutFormat = new S2.Layout();
					break;
				case EngineVersion.S3K:
					LayoutFormat = new S3K.Layout();
					break;
				case EngineVersion.SKC:
					LayoutFormat = new SKC.Layout();
					break;
				case EngineVersion.SCDPC:
					LayoutFormat = new SCDPC.Layout();
					break;
				case EngineVersion.Custom:
					LayoutFormat = CompileCodeFile<LayoutFormat>(Level.LayoutCodeFile, Level.LayoutCodeType);
					break;
			}
			if (LayoutFormat.IsCombinedLayout)
			{
				if (string.IsNullOrWhiteSpace(Level.Layout))
					throw new FormatException("Level must contain a Layout file!");
				LayoutFormatCombined lfc = (LayoutFormatCombined)LayoutFormat;
				lfc.TryReadLayout(Level.Layout, Level.LayoutCompression, Layout);
				foreach (string lvlname in Game.Levels.Keys)
					if (lvlname != levelname)
					{
						LevelInfo lvlinf = Game.GetLevelInfo(lvlname);
						if (Level.Layout != lvlinf.Layout && !AdditionalLayouts.ContainsKey(lvlinf.Layout)
							&& Level.LayoutFormat == lvlinf.LayoutFormat && Level.Chunks.ArrayEqual(lvlinf.Chunks))
						{
							LayoutData ld = new LayoutData();
							lfc.TryReadLayout(lvlinf.Layout, lvlinf.LayoutCompression, ld);
							AdditionalLayouts.Add(lvlinf.Layout, new KeyValuePair<CompressionType, LayoutData>(lvlinf.LayoutCompression, ld));
						}
					}
			}
			else
			{
				if (string.IsNullOrWhiteSpace(Level.FGLayout) || string.IsNullOrWhiteSpace(Level.BGLayout))
					throw new FormatException("Level must contain an FGLayout and BGLayout file!");
				LayoutFormatSeparate lfs = (LayoutFormatSeparate)LayoutFormat;
				lfs.TryReadLayout(Level.FGLayout, Level.BGLayout, Level.FGLayoutCompression, Level.BGLayoutCompression, Layout);
				foreach (string lvlname in Game.Levels.Keys)
					if (lvlname != levelname)
					{
						LevelInfo lvlinf = Game.GetLevelInfo(lvlname);
						if (Level.LayoutFormat == lvlinf.LayoutFormat && Level.Chunks.ArrayEqual(lvlinf.Chunks))
						{
							if (Level.FGLayout != lvlinf.FGLayout && !AdditionalLayouts.ContainsKey(lvlinf.FGLayout))
							{
								LayoutData ld = new LayoutData();
								lfs.TryReadFG(lvlinf.FGLayout, lvlinf.FGLayoutCompression, ld);
								AdditionalLayouts.Add(lvlinf.FGLayout, new KeyValuePair<CompressionType, LayoutData>(lvlinf.FGLayoutCompression, ld));
							}
							if (Level.BGLayout != lvlinf.BGLayout && !AdditionalLayouts.ContainsKey(lvlinf.BGLayout))
							{
								LayoutData ld = new LayoutData();
								lfs.TryReadBG(lvlinf.BGLayout, lvlinf.BGLayoutCompression, ld);
								AdditionalLayouts.Add(lvlinf.BGLayout, new KeyValuePair<CompressionType, LayoutData>(lvlinf.BGLayoutCompression, ld));
							}
						}
					}
			}
		}

		private static void LoadLevelPalette()
		{
			PalName = new List<string>();
			Palette = new List<SonLVLColor[,]>();
			PalNum = new List<byte[,]>();
			PalAddr = new List<int[,]>();
			if (Level.Palette == null || string.IsNullOrWhiteSpace(Level.Palette[0].Filename))
				throw new FormatException("Level must contain at least one Palette file!");
			byte palfilenum = 0;
			for (int palnum = 0; palnum < Level.Palettes.Length; palnum++)
			{
				PalName.Add(Level.Palettes[palnum].Name);
				Palette.Add(new SonLVLColor[4, 16]);
				PalNum.Add(new byte[4, 16]);
				PalAddr.Add(new int[4, 16]);
				for (byte pn = 0; pn < Level.Palettes[palnum].Palettes.Collection.Length; pn++)
				{
					PaletteInfo palent = Level.Palettes[palnum].Palettes[pn];
					Log("Loading palette file \"" + palent.Filename + "\"...", "Source: " + palent.Source + " Destination: " + palent.Destination + " Length: " + palent.Length);
					if (!File.Exists(palent.Filename)) throw new FileNotFoundException("Palette file could not be loaded! Have you set up your disassembly properly?", palent.Filename);
					SonLVLColor[] palfile = SonLVLColor.Load(palent.Filename, Level.PaletteFormat);
					for (int pa = 0; pa < palent.Length; pa++)
					{
						Palette[palnum][(pa + palent.Destination) / 16, (pa + palent.Destination) % 16] = palfile[pa + palent.Source];
						PalNum[palnum][(pa + palent.Destination) / 16, (pa + palent.Destination) % 16] = palfilenum;
						PalAddr[palnum][(pa + palent.Destination) / 16, (pa + palent.Destination) % 16] = pa + palent.Source;
					}
					palfilenum++;
				}
			}
			CurPal = 0;
		}

		private static void LoadLevelColInds()
		{
			ColInds1 = new List<byte>();
			ColInds2 = new List<byte>();
			switch (Level.CollisionIndexFormat)
			{
				case EngineVersion.S1:
				case EngineVersion.SCD:
				case EngineVersion.SCDPC:
					if (Level.CollisionIndex != null && File.Exists(Level.CollisionIndex))
						ColInds1.AddRange(Compression.Decompress(Level.CollisionIndex, Level.CollisionIndexCompression));
					ColInds2 = ColInds1;
					break;
				case EngineVersion.S2:
				case EngineVersion.S2NA:
					if (Level.CollisionIndex1 != null && File.Exists(Level.CollisionIndex1))
						ColInds1.AddRange(Compression.Decompress(Level.CollisionIndex1, Level.CollisionIndexCompression));
					if (Level.CollisionIndex2 != null)
					{
						if (File.Exists(Level.CollisionIndex2))
							ColInds2.AddRange(Compression.Decompress(Level.CollisionIndex2, Level.CollisionIndexCompression));
					}
					else
						ColInds2 = ColInds1;
					break;
				case EngineVersion.S3K:
				case EngineVersion.SKC:
					if (Level.CollisionIndex != null && File.Exists(Level.CollisionIndex))
					{
						byte[] tmp = Compression.Decompress(Level.CollisionIndex, Level.CollisionIndexCompression);
						switch (Level.CollisionIndexSize)
						{
							case 0:
							case 1:
								Array.Resize(ref tmp, 0x600);
								for (int i = 0; i < 0x600; i += 2)
								{
									ColInds1.Add(tmp[i]);
									ColInds2.Add(tmp[i + 1]);
								}
								break;
							case 2:
								for (int i = 0; i < 0x600; i += 2)
									ColInds1.Add((byte)ByteConverter.ToUInt16(tmp, i));
								for (int i = 0x600; i < 0xC00; i += 2)
									ColInds2.Add((byte)ByteConverter.ToUInt16(tmp, i));
								break;
						}
					}
					else
					{
						ColInds1.AddRange(new byte[0x300]);
						ColInds2.AddRange(new byte[0x300]);
					}
					break;
			}
		}

		private static void LoadLevelColArr()
		{
			byte[] tmp;
			ColArr1 = new sbyte[256][];
			if (Level.CollisionArray1 != null && File.Exists(Level.CollisionArray1))
				tmp = Compression.Decompress(Level.CollisionArray1, Level.CollisionArrayCompression);
			else
				tmp = new byte[256 * 16];
			for (int i = 0; i < 256; i++)
			{
				ColArr1[i] = new sbyte[16];
				for (int j = 0; j < 16; j++)
					ColArr1[i][j] = unchecked((sbyte)tmp[(i * 16) + j]);
			}
		}

		private static void LoadLevelAngles()
		{
			if (Level.Angles != null && File.Exists(Level.Angles))
				Angles = Compression.Decompress(Level.Angles, Level.AngleCompression);
			else
				Angles = new byte[256];
		}

		private static void LoadLevelObjects(bool loadGraphics)
		{
			if (Level.Objects != null)
			{
				Objects = ObjectFormat.TryReadLayout(Level.Objects, Level.ObjectCompression, out objectterm);
				if (loadGraphics)
					for (int i = 0; i < Objects.Count; i++)
						Objects[i].UpdateSprite();
			}
			else
				Objects = new List<ObjectEntry>();
		}

		private static void LoadLevelRings(bool loadGraphics)
		{
			if (Level.Rings != null && RingFormat is RingLayoutFormat)
			{
				Rings = ((RingLayoutFormat)RingFormat).TryReadLayout(Level.Rings, Level.RingCompression, out ringstartterm, out ringendterm);
				if (loadGraphics)
					foreach (RingEntry ring in Rings)
						ring.UpdateSprite();
			}
			else
				Rings = new List<RingEntry>();
		}

		private static void LoadLevelExtraObjects(bool loadGraphics)
		{
			if (Level.ExtraObjects != null)
			{
				ExtraObjects = new List<ExtraObjEntry>();
				if (File.Exists(Level.ExtraObjects.Filename))
				{
					var tmp = File.ReadAllBytes(Level.ExtraObjects.Filename);
					if (tmp.Length < 10 || ByteConverter.ToInt16(tmp, 2) < 0) return;

					var ent = CompileCodeFile<ExtraObjEntry>(Level.ExtraObjects.CodeFile, Level.ExtraObjects.CodeType);
					if (loadGraphics) ent.Init();

					for (var i = 0; true; i += 2)
					{
						ent.ID = ByteConverter.ToUInt16(tmp, i);
						ent.Position = new Position(ByteConverter.ToUInt16(tmp, i += 2), ByteConverter.ToUInt16(tmp, i += 2));
						ExtraObjects.Add(ent);
						if (loadGraphics) ent.UpdateSprite();

						if (ByteConverter.ToInt16(tmp, i + 4) < 0) return;
						ent = (ExtraObjEntry)Activator.CreateInstance(ent.GetType());
					}
				}
			}
			else if (Level.Bumpers != null)
			{
				ExtraObjects = new List<ExtraObjEntry>();
				if (File.Exists(Level.Bumpers))
				{
					Log("Loading bumpers from file \"" + Level.Bumpers + "\", using compression " + Level.BumperCompression + "...");
					byte[] tmp = Compression.Decompress(Level.Bumpers, Level.BumperCompression);
					for (int i = 0; i < tmp.Length; i += ExtraObjEntry.Size)
					{
						if (ByteConverter.ToUInt16(tmp, i + 2) == 0xFFFF) break;
						ExtraObjEntry ent = new ActualCNZBumperEntry(tmp, i);
						ExtraObjects.Add(ent);
						if (loadGraphics) ent.UpdateSprite();
					}
				}
				else
					Log("Bumper file \"" + Level.Bumpers + "\" not found.");
			}
			else
			{
				ExtraObjects = null;
			}
		}

		private static void LoadLevelStartPositions(bool loadGraphics)
		{
			StartPositions = new List<StartPositionEntry>();
			StartPosDefs = new List<StartPositionDefinition>();
			if (Level.StartPositions != null)
			{
				foreach (StartPositionInfo item in Level.StartPositions)
				{
					StartPositionEntry ent;
					if (File.Exists(item.Filename))
					{
						Log("Loading start position \"" + item.Name + "\" from file \"" + item.Filename + "\"...");
						ent = new StartPositionEntry(File.ReadAllBytes(item.Filename), item.Offset == -1 ? 0 : item.Offset);
					}
					else
					{
						Log("Start position file \"" + item.Filename + "\" not found.");
						ent = new StartPositionEntry();
					}
					StartPositions.Add(ent);
					if (loadGraphics)
					{
						if (!string.IsNullOrEmpty(item.Sprite))
							StartPosDefs.Add(new StartPositionDefinition(INIObjDefs[item.Sprite], item.Name));
						else
							StartPosDefs.Add(new StartPositionDefinition(item.Name));
						ent.UpdateSprite();
					}
				}
			}
		}

		private static T CompileCodeFile<T>(string codefile, string typename)
		{
			string dllfile = Path.Combine("dllcache", typename + ".dll");
			DateTime modDate = DateTime.MinValue;
			if (File.Exists(dllfile))
				modDate = File.GetLastWriteTime(dllfile);
			string fp = codefile.Replace('/', Path.DirectorySeparatorChar);
			Log("Loading type " + typename + " from \"" + fp + "\"...");
			if (modDate >= File.GetLastWriteTime(fp) & modDate > File.GetLastWriteTime(Application.ExecutablePath))
			{
				Log("Loading type from cached assembly \"" + dllfile + "\"...");
				return (T)Activator.CreateInstance(System.Reflection.Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, dllfile)).GetType(typename));
			}
			else
			{
				Log("Compiling code file...");
				string ext = Path.GetExtension(fp);
				CodeDomProvider pr = null;
				switch (ext.ToLowerInvariant())
				{
					case ".cs":
						pr = new Microsoft.CSharp.CSharpCodeProvider();
						break;
					case ".vb":
						pr = new Microsoft.VisualBasic.VBCodeProvider();
						break;
				}
				CompilerParameters para = new CompilerParameters(new string[] { "System.dll", "System.Core.dll", "System.Drawing.dll", System.Reflection.Assembly.GetExecutingAssembly().Location })
				{
					GenerateExecutable = false,
					GenerateInMemory = false,
					IncludeDebugInformation = true,
					OutputAssembly = Path.Combine(Environment.CurrentDirectory, dllfile)
				};
				CompilerResults res = pr.CompileAssemblyFromFile(para, fp);
				if (res.Errors.HasErrors)
				{
					Log("Compile failed.", "Errors:");
					foreach (CompilerError item in res.Errors)
						Log(item.ToString());
					Log(string.Empty);
					throw new Exception("Failed compiling file.");
				}
				else
				{
					Log("Compile succeeded.");
					return (T)Activator.CreateInstance(res.CompiledAssembly.GetType(typename));
				}
			}
		}

		public static void SaveLevel()
		{
			Log("Saving " + Level.DisplayName + "...");
			switch (Level.EngineVersion)
			{
				case EngineVersion.SCDPC:
					Parallel.Invoke(SaveLevelTiles, SaveLevelChunks, SaveLevelLayout, SaveLevelPalette,
						SaveLevelObjects, SaveLevelRings, SaveLevelExtraObjects, SaveLevelStartPositions,
						SaveLevelColInds, SaveLevelColArr1, SaveLevelColArr2, SaveLevelAngles);
					SaveLevelBlocks(); // SCDPC...
					break;
				case EngineVersion.SKC:
					Parallel.Invoke(SaveLevelTiles, SaveLevelLayout, SaveLevelPalette,
						SaveLevelObjects, SaveLevelRings, SaveLevelExtraObjects, SaveLevelStartPositions,
						SaveLevelColInds, SaveLevelColArr1, SaveLevelColArr2, SaveLevelAngles);
					SaveLevelBlocks();
					SaveLevelChunks();
					break;
				default:
					Parallel.Invoke(SaveLevelTiles, SaveLevelBlocks, SaveLevelChunks, SaveLevelLayout, SaveLevelPalette,
						SaveLevelObjects, SaveLevelRings, SaveLevelExtraObjects, SaveLevelStartPositions,
						SaveLevelColInds, SaveLevelColArr1, SaveLevelColArr2, SaveLevelAngles);
					break;
			}
		}

		private static void SaveLevelTiles()
		{
			int fileind = -1;
			ReadOnlyCollection<ReadOnlyCollection<byte[]>> tilefiles = Tiles.GetFiles();
			if (Level.TileFormat != EngineVersion.SCDPC)
			{
				foreach (FileInfo tileent in Level.Tiles)
				{
					fileind++;
					List<byte> tmp = new List<byte>();
					foreach (byte[] item in tilefiles[fileind])
						tmp.AddRange(item);
					Compression.Compress(tmp.ToArray(), tileent.Filename, Level.TileCompression);
				}
			}
			else
			{
				List<ushort>[] tilepals = new List<ushort>[4];
				for (int i = 0; i < 4; i++)
					tilepals[i] = new List<ushort>();
				foreach (Block blk in Blocks)
					for (int y = 0; y < 2; y++)
						for (int x = 0; x < 2; x++)
							if (!tilepals[blk.Tiles[x, y].Palette].Contains(blk.Tiles[x, y].Tile))
								tilepals[blk.Tiles[x, y].Palette].Add(blk.Tiles[x, y].Tile);
				foreach (Block blk in Blocks)
					for (int y = 0; y < 2; y++)
						for (int x = 0; x < 2; x++)
						{
							byte pal = blk.Tiles[x, y].Palette;
							int c = 0;
							for (int i = pal - 1; i >= 0; i--)
								c += tilepals[i].Count;
							blk.Tiles[x, y].Tile = (ushort)(tilepals[pal].IndexOf(blk.Tiles[x, y].Tile) + c);
						}
				List<byte[]> tiles = new List<byte[]>();
				for (int p = 0; p < 4; p++)
					foreach (ushort item in tilepals[p])
						if (Tiles[item] != null)
							tiles.Add(Tiles[item]);
						else
							tiles.Add(new byte[32]);
				Tiles.Clear();
				Tiles.AddFile(tiles, -1);
				UpdateTileArray();
				List<byte> tmp = new List<byte> { 0x53, 0x43, 0x52, 0x4C };
				tmp.AddRange(ByteConverter.GetBytes(0x18 + (Tiles.Count * 4) + (Tiles.Count * 32)));
				tmp.AddRange(ByteConverter.GetBytes(Tiles.Count));
				tmp.AddRange(ByteConverter.GetBytes(0x18 + (Tiles.Count * 4)));
				for (int i = 0; i < 4; i++)
					tmp.AddRange(ByteConverter.GetBytes((ushort)tilepals[i].Count));
				for (int i = 0; i < Tiles.Count; i++)
				{
					tmp.AddRange(ByteConverter.GetBytes((ushort)8));
					tmp.AddRange(ByteConverter.GetBytes((ushort)8));
				}
				tmp.AddRange(TileArray);
				Compression.Compress(tmp.ToArray(), Level.Tiles[0].Filename, CompressionType.SZDD);
			}
		}

		private static void SaveLevelBlocks()
		{
			int fileind = -1;
			ReadOnlyCollection<ReadOnlyCollection<Block>> blockfiles = Blocks.GetFiles();
			foreach (FileInfo tileent in Level.Blocks)
			{
				fileind++;
				List<byte> tmp = new List<byte>();
				if (Level.EngineVersion == EngineVersion.SKC)
					littleendian = false;
				foreach (Block b in blockfiles[fileind])
					tmp.AddRange(b.GetBytes());
				if (Level.EngineVersion == EngineVersion.SKC)
					littleendian = true;
				Compression.Compress(tmp.ToArray(), tileent.Filename, Level.BlockCompression);
			}
		}

		private static void SaveLevelChunks()
		{
			int fileind = -1;
			ReadOnlyCollection<ReadOnlyCollection<Chunk>> chunkfiles = Chunks.GetFiles();
			foreach (FileInfo tileent in Level.Chunks)
			{
				fileind++;
				List<byte> tmp = new List<byte>();
				if (Level.EngineVersion == EngineVersion.SKC)
					littleendian = false;
				foreach (Chunk c in chunkfiles[fileind])
					tmp.AddRange(c.GetBytes());
				if (Level.EngineVersion == EngineVersion.SKC)
					littleendian = true;
				if (fileind == 0)
					switch (Level.ChunkFormat)
					{
						case EngineVersion.S1:
						case EngineVersion.SCD:
						case EngineVersion.SCDPC:
							tmp.RemoveRange(0, Chunk.Size);
							break;
					}
				Compression.Compress(tmp.ToArray(), tileent.Filename, Level.ChunkCompression);
			}
		}

		private static void SaveLevelLayout()
		{
			switch (LayoutFormat)
			{
				case LayoutFormatCombined lfc:
					lfc.WriteLayout(Layout, Level.LayoutCompression, Level.Layout);
					foreach (var item in AdditionalLayouts)
						lfc.WriteLayout(item.Value.Value, item.Value.Key, item.Key);
					break;
				case LayoutFormatSeparate lfs:
					lfs.WriteLayout(Layout, Level.FGLayoutCompression, Level.BGLayoutCompression, Level.FGLayout, Level.BGLayout);
					foreach (var item in AdditionalLayouts)
						if (item.Value.Value.FGLayout != null)
							lfs.WriteFG(item.Value.Value, item.Value.Key, item.Key);
						else
							lfs.WriteBG(item.Value.Value, item.Value.Key, item.Key);
					break;
			}
		}

		private static void SaveLevelPalette()
		{
			if (Level.PaletteFormat != EngineVersion.SCDPC)
			{
				byte[] paltmp;
				List<ushort[]> palfiles = new List<ushort[]>();
				byte palfilenum = 0;
				for (int palnum = 0; palnum < Level.Palettes.Length; palnum++)
				{
					PaletteList palent = Level.Palettes[palnum].Palettes;
					for (byte pn = 0; pn < palent.Collection.Length; pn++)
					{
						paltmp = File.ReadAllBytes(palent[pn].Filename);
						ushort[] palfile = new ushort[paltmp.Length / 2];
						for (int pi = 0; pi < paltmp.Length; pi += 2)
							palfile[pi / 2] = ByteConverter.ToUInt16(paltmp, pi);
						palfiles.Add(palfile);
					}
					for (int pl = 0; pl < 4; pl++)
						for (int pi = 0; pi < 16; pi++)
							palfiles[PalNum[palnum][pl, pi]][PalAddr[palnum][pl, pi]] = Palette[palnum][pl, pi].MDColor;
					for (byte pn = 0; pn < palent.Collection.Length; pn++)
					{
						List<byte> tmp = new List<byte>();
						for (int pi = 0; pi < palfiles[pn + palfilenum].Length; pi++)
							tmp.AddRange(ByteConverter.GetBytes(palfiles[pn + palfilenum][pi]));
						File.WriteAllBytes(palent[pn].Filename, tmp.ToArray());
					}
					palfilenum = (byte)palfiles.Count;
				}
			}
			else
			{
				List<byte[]> palfiles = new List<byte[]>();
				byte palfilenum = 0;
				if (Level.Palettes.Length > 0)
				{
					PaletteList palent = Level.Palettes[0].Palettes;
					for (byte pn = 0; pn < palent.Collection.Length; pn++)
						palfiles.Add(File.ReadAllBytes(palent[pn].Filename));
					for (int pl = 0; pl < 4; pl++)
					{
						for (int pi = 0; pi < 16; pi++)
						{
							palfiles[PalNum[0][pl, pi]][PalAddr[0][pl, pi] * 4] = Palette[0][pl, pi].R;
							palfiles[PalNum[0][pl, pi]][PalAddr[0][pl, pi] * 4 + 1] = Palette[0][pl, pi].G;
							palfiles[PalNum[0][pl, pi]][PalAddr[0][pl, pi] * 4 + 2] = Palette[0][pl, pi].B;
						}
					}
					for (byte pn = 0; pn < palent.Collection.Length; pn++)
						File.WriteAllBytes(palent[pn].Filename, palfiles[pn]);
					palfilenum = (byte)palfiles.Count;
				}
			}
		}

		private static void SaveLevelObjects()
		{
			if (Level.Objects != null)
			{
				Objects.Sort();
				ObjectFormat.WriteLayout(Objects, Level.ObjectCompression, Level.Objects, objectterm);
			}
		}

		private static void SaveLevelRings()
		{
			if (Level.Rings != null && RingFormat is RingLayoutFormat)
			{
				Rings.Sort();
				((RingLayoutFormat)RingFormat).WriteLayout(Rings, Level.RingCompression, Level.Rings, ringstartterm, ringendterm);
			}
		}

		private static void SaveLevelExtraObjects()
		{
			if (ExtraObjects != null)
			{
				ExtraObjects.Sort();
				List<byte> tmp = new List<byte>();
				foreach (ExtraObjEntry item in ExtraObjects)
					tmp.AddRange(item.GetBytes());

				if (Level.ExtraObjects != null)
				{
					tmp.AddRange(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
					File.WriteAllBytes(Level.ExtraObjects.Filename, tmp.ToArray());
				}
				else
				{
					tmp.AddRange(new byte[] { 0, 0, 0xFF, 0xFF, 0, 0 });
					Compression.Compress(tmp.ToArray(), Level.Bumpers, Level.BumperCompression);
				}
			}
		}

		private static void SaveLevelStartPositions()
		{
			if (Level.StartPositions != null)
			{
				int i = 0;
				foreach (StartPositionInfo item in Level.StartPositions)
				{
					byte[] fc = new byte[4];
					if (File.Exists(item.Filename))
						fc = File.ReadAllBytes(item.Filename);
					StartPositions[i++].GetBytes().CopyTo(fc, item.Offset == -1 ? 0 : item.Offset);
					Directory.CreateDirectory(Path.GetDirectoryName(item.Filename));
					File.WriteAllBytes(item.Filename, fc);
				}
			}
		}

		private static void SaveLevelColInds()
		{
			switch (Level.CollisionIndexFormat)
			{
				case EngineVersion.S1:
				case EngineVersion.SCD:
				case EngineVersion.SCDPC:
					if (Level.CollisionIndex != null)
						Compression.Compress(ColInds1.ToArray(), Level.CollisionIndex, Level.CollisionIndexCompression);
					break;
				case EngineVersion.S2:
				case EngineVersion.S2NA:
					if (Level.CollisionIndex1 != null)
						Compression.Compress(ColInds1.ToArray(), Level.CollisionIndex1, Level.CollisionIndexCompression);
					if (Level.CollisionIndex2 != null)
						Compression.Compress(ColInds2.ToArray(), Level.CollisionIndex2, Level.CollisionIndexCompression);
					break;
				case EngineVersion.S3K:
				case EngineVersion.SKC:
					if (Level.CollisionIndex != null)
					{
						List<byte> tmp = new List<byte>();
						byte[] cif = null;
						switch (Level.CollisionIndexSize)
						{
							case 0:
							case 1:
								cif = new byte[0x600];
								for (int i = 0; i < ColInds1.Count; i++)
								{
									tmp.Add(ColInds1[i]);
									tmp.Add(ColInds2[i]);
								}
								tmp.CopyTo(0, cif, 0, Math.Min(tmp.Count, cif.Length));
								break;
							case 2:
								cif = new byte[0xC00];
								foreach (byte item in ColInds1)
									tmp.AddRange(ByteConverter.GetBytes((ushort)item));
								tmp.CopyTo(0, cif, 0, Math.Min(tmp.Count, 0x600));
								tmp.Clear();
								foreach (byte item in ColInds2)
									tmp.AddRange(ByteConverter.GetBytes((ushort)item));
								tmp.CopyTo(0, cif, 0x600, Math.Min(tmp.Count, 0x600));
								break;
						}
						Compression.Compress(cif, Level.CollisionIndex, Level.CollisionIndexCompression);
					}
					break;
			}
		}

		private static void SaveLevelColArr1()
		{
			if (Level.CollisionArray1 != null)
			{
				List<byte> tmp = new List<byte>(0x1000);
				for (int i = 0; i < 256; i++)
					for (int j = 0; j < 16; j++)
						tmp.Add(unchecked((byte)ColArr1[i][j]));
				Compression.Compress(tmp.ToArray(), Level.CollisionArray1, Level.CollisionArrayCompression);
			}
		}

		private static void SaveLevelColArr2()
		{
			if (Level.CollisionArray2 != null)
			{
				sbyte[][] rotcol = GenerateRotatedCollision();
				List<byte> tmp = new List<byte>(0x1000);
				for (int i = 0; i < 256; i++)
					for (int j = 0; j < 16; j++)
						tmp.Add(unchecked((byte)rotcol[i][j]));
				Compression.Compress(tmp.ToArray(), Level.CollisionArray2, Level.CollisionArrayCompression);
			}
		}

		private static void SaveLevelAngles()
		{
			if (Level.Angles != null)
				Compression.Compress(Angles, Level.Angles, Level.AngleCompression);
		}

		public static BitmapBits DrawForeground(Rectangle? section, bool includeObjects, bool includeDebugObjects, bool objectsAboveHighPlane, bool lowPlane, bool highPlane, bool collisionPath1, bool collisionPath2, bool allTimeZones)
		{
			Rectangle bounds;
			if (section.HasValue)
				bounds = section.Value;
			else
			{
				int xend = 0;
				int yend = 0;
				for (int y = 0; y < FGHeight; y++)
					for (int x = 0; x < FGWidth; x++)
						if (Layout.FGLayout[x, y] > 0)
						{
							xend = Math.Max(xend, x);
							yend = Math.Max(yend, y);
						}
				xend++;
				yend++;
				bounds = new Rectangle(0, 0, xend * Level.ChunkWidth, yend * Level.ChunkHeight);
			}
			BitmapBits LevelImg8bpp = new BitmapBits(bounds.Size);
			int cl = Math.Max(bounds.X / Level.ChunkWidth, 0);
			int ct = Math.Max(bounds.Y / Level.ChunkHeight, 0);
			int cr = Math.Min((bounds.Right - 1) / Level.ChunkWidth, FGWidth - 1);
			int cb = Math.Min((bounds.Bottom - 1) / Level.ChunkHeight, FGHeight - 1);
			for (int y = ct; y <= cb; y++)
				for (int x = cl; x <= cr; x++)
					if (Layout.FGLayout[x, y] < Chunks.Count)
					{
						bool xflip = false;
						bool yflip = false;
						if (Layout.FGXFlip != null)
							xflip = Layout.FGXFlip[x, y];
						if (Layout.FGYFlip != null)
							yflip = Layout.FGYFlip[x, y];

						ChunkSprites[Layout.FGLayout[x, y]].Flip(xflip, yflip);
						ChunkSprites[Layout.FGLayout[x, y]].Offset(Level.ChunkWidth * (xflip ? 1 : 0), Level.ChunkHeight * (yflip ? 1 : 0));
						ChunkColBmpBits[Layout.FGLayout[x, y]][0].Flip(xflip, yflip);
						ChunkColBmpBits[Layout.FGLayout[x, y]][1].Flip(xflip, yflip);

						if ((!includeObjects || objectsAboveHighPlane) && lowPlane && highPlane)
						{
							LevelImg8bpp.DrawSprite(ChunkSprites[Layout.FGLayout[x, y]], x * Level.ChunkWidth - bounds.X, y * Level.ChunkHeight - bounds.Y);
							if (collisionPath1)
								LevelImg8bpp.DrawBitmapComposited(ChunkColBmpBits[Layout.FGLayout[x, y]][0], x * Level.ChunkWidth - bounds.X, y * Level.ChunkHeight - bounds.Y);
							else if (collisionPath2)
								LevelImg8bpp.DrawBitmapComposited(ChunkColBmpBits[Layout.FGLayout[x, y]][1], x * Level.ChunkWidth - bounds.X, y * Level.ChunkHeight - bounds.Y);
						}
						else
						{
							if (lowPlane)
								LevelImg8bpp.DrawSpriteLow(ChunkSprites[Layout.FGLayout[x, y]], x * Level.ChunkWidth - bounds.X, y * Level.ChunkHeight - bounds.Y);
							if (!includeObjects || objectsAboveHighPlane)
							{
								if (highPlane)
									LevelImg8bpp.DrawSpriteHigh(ChunkSprites[Layout.FGLayout[x, y]], x * Level.ChunkWidth - bounds.X, y * Level.ChunkHeight - bounds.Y);
								if (collisionPath1)
									LevelImg8bpp.DrawBitmapComposited(ChunkColBmpBits[Layout.FGLayout[x, y]][0], x * Level.ChunkWidth - bounds.X, y * Level.ChunkHeight - bounds.Y);
								else if (collisionPath2)
									LevelImg8bpp.DrawBitmapComposited(ChunkColBmpBits[Layout.FGLayout[x, y]][1], x * Level.ChunkWidth - bounds.X, y * Level.ChunkHeight - bounds.Y);
							}
						}

						ChunkColBmpBits[Layout.FGLayout[x, y]][0].Flip(xflip, yflip);
						ChunkColBmpBits[Layout.FGLayout[x, y]][1].Flip(xflip, yflip);
						ChunkSprites[Layout.FGLayout[x, y]].Offset(-Level.ChunkWidth * (xflip ? 1 : 0), -Level.ChunkHeight * (yflip ? 1 : 0));
						ChunkSprites[Layout.FGLayout[x, y]].Flip(xflip, yflip);
					}
			if (includeObjects)
			{
				List<Entry> objs = new List<Entry>(Objects.Where(o => ObjectVisible(o, allTimeZones)));
				if (RingFormat is RingLayoutFormat)
					objs.AddRange(Rings);
				objs.Sort((a, b) =>
				{
					int result = -a.Depth.CompareTo(b.Depth);
					if (result == 0)
						result = ((IComparable<Entry>)a).CompareTo(b);
					return result;
				});
				if (objectsAboveHighPlane)
				{
					foreach (Entry item in objs)
						if (item is RingEntry || !(!includeDebugObjects && GetObjectDefinition(((ObjectEntry)item).ID).Debug))
							LevelImg8bpp.DrawSprite(item.Sprite, item.X - bounds.X, item.Y - bounds.Y);
					if (ExtraObjects != null)
						foreach (ExtraObjEntry item in ExtraObjects.Where(obj => includeDebugObjects || !obj.Debug))
						{
							LevelImg8bpp.DrawSpriteHigh(item.Sprite, item.X - bounds.X, item.Y - bounds.Y);
							if (includeDebugObjects)
							{
								var rect = item.Bounds;
								LevelImg8bpp.DrawRectangle(ColorWhite, rect.X - bounds.X, rect.Y - bounds.Y, rect.Width - 1, rect.Height - 1);
							}
						}
					for (int si = StartPositions.Count - 1; si >= 0; si--)
						LevelImg8bpp.DrawSprite(StartPositions[si].Sprite, StartPositions[si].X - bounds.X, StartPositions[si].Y - bounds.Y);
				}
				else
				{
					BitmapBits objbmplow = new BitmapBits(bounds.Size);
					BitmapBits objbmplevel = new BitmapBits(bounds.Size);
					BitmapBits objbmphigh = new BitmapBits(bounds.Size);
					int curdepth = int.MinValue;
					foreach (Entry item in objs)
						if (item is RingEntry || !(!includeDebugObjects && GetObjectDefinition(((ObjectEntry)item).ID).Debug))
						{
							if (item.Depth != curdepth)
							{
								curdepth = item.Depth;
								objbmphigh.DrawBitmapComposited(objbmplevel, 0, 0);
								objbmplevel.Clear();
							}
							objbmplow.DrawSpriteLow(item.Sprite, item.X - bounds.X, item.Y - bounds.Y);
							objbmphigh.ClearSpriteLow(item.Sprite, item.X - bounds.X, item.Y - bounds.Y);
							objbmplevel.DrawSpriteHigh(item.Sprite, item.X - bounds.X, item.Y - bounds.Y);
						}
					LevelImg8bpp.DrawBitmapComposited(objbmplow, 0, 0);
					if (ExtraObjects != null)
						foreach (ExtraObjEntry item in ExtraObjects.Where(obj => includeDebugObjects || !obj.Debug))
							LevelImg8bpp.DrawSpriteLow(item.Sprite, item.X - bounds.X, item.Y - bounds.Y);
					for (int si = StartPositions.Count - 1; si >= 0; si--)
						LevelImg8bpp.DrawSpriteLow(StartPositions[si].Sprite, StartPositions[si].X - bounds.X, StartPositions[si].Y - bounds.Y);
					for (int y = ct; y <= cb; y++)
						for (int x = cl; x <= cr; x++)
							if (Layout.FGLayout[x, y] < Chunks.Count)
							{
								bool xflip = false;
								bool yflip = false;
								if (Layout.FGXFlip != null)
									xflip = Layout.FGXFlip[x, y];
								if (Layout.FGYFlip != null)
									yflip = Layout.FGYFlip[x, y];

								ChunkSprites[Layout.FGLayout[x, y]].Flip(xflip, yflip);
								ChunkSprites[Layout.FGLayout[x, y]].Offset(Level.ChunkWidth * (xflip ? 1 : 0), Level.ChunkHeight * (yflip ? 1 : 0));
								ChunkColBmpBits[Layout.FGLayout[x, y]][0].Flip(xflip, yflip);
								ChunkColBmpBits[Layout.FGLayout[x, y]][1].Flip(xflip, yflip);

								if (highPlane)
									LevelImg8bpp.DrawSpriteHigh(ChunkSprites[Layout.FGLayout[x, y]], x * Level.ChunkWidth - bounds.X, y * Level.ChunkHeight - bounds.Y);
								if (collisionPath1)
									LevelImg8bpp.DrawBitmapComposited(ChunkColBmpBits[Layout.FGLayout[x, y]][0], x * Level.ChunkWidth - bounds.X, y * Level.ChunkHeight - bounds.Y);
								else if (collisionPath2)
									LevelImg8bpp.DrawBitmapComposited(ChunkColBmpBits[Layout.FGLayout[x, y]][1], x * Level.ChunkWidth - bounds.X, y * Level.ChunkHeight - bounds.Y);

								ChunkColBmpBits[Layout.FGLayout[x, y]][0].Flip(xflip, yflip);
								ChunkColBmpBits[Layout.FGLayout[x, y]][1].Flip(xflip, yflip);
								ChunkSprites[Layout.FGLayout[x, y]].Offset(-Level.ChunkWidth * (xflip ? 1 : 0), -Level.ChunkHeight * (yflip ? 1 : 0));
								ChunkSprites[Layout.FGLayout[x, y]].Flip(xflip, yflip);
							}
					LevelImg8bpp.DrawBitmapComposited(objbmphigh, 0, 0);
					LevelImg8bpp.DrawBitmapComposited(objbmplevel, 0, 0);
					if (ExtraObjects != null)
						foreach (ExtraObjEntry item in ExtraObjects.Where(obj => includeDebugObjects || !obj.Debug))
						{
							LevelImg8bpp.DrawSpriteHigh(item.Sprite, item.X - bounds.X, item.Y - bounds.Y);
							if (includeDebugObjects)
							{
								var rect = item.Bounds;
								LevelImg8bpp.DrawRectangle(ColorWhite, rect.X - bounds.X, rect.Y - bounds.Y, rect.Width - 1, rect.Height - 1);
							}
						}
					for (int si = StartPositions.Count - 1; si >= 0; si--)
						LevelImg8bpp.DrawSpriteHigh(StartPositions[si].Sprite, StartPositions[si].X - bounds.X, StartPositions[si].Y - bounds.Y);
				}
				if (includeDebugObjects)
					foreach (ObjectEntry item in objs.OfType<ObjectEntry>())
						if (item.DebugOverlay != null)
							LevelImg8bpp.DrawSprite(item.DebugOverlay, item.X - bounds.X, item.Y - bounds.Y);
			}
			return LevelImg8bpp;
		}

		public static BitmapBits DrawBackground(Rectangle? section, bool lowPlane, bool highPlane, bool collisionPath1, bool collisionPath2)
		{
			Rectangle bounds;
			if (section.HasValue)
				bounds = section.Value;
			else
			{
				int xend = 0;
				int yend = 0;
				for (int y = 0; y < Layout.BGLayout.GetLength(1); y++)
					for (int x = 0; x < Layout.BGLayout.GetLength(0); x++)
						if (Layout.BGLayout[x, y] > 0)
						{
							xend = Math.Max(xend, x);
							yend = Math.Max(yend, y);
						}
				xend++;
				yend++;
				bounds = new Rectangle(0, 0, xend * Level.ChunkWidth, yend * Level.ChunkHeight);
			}
			BitmapBits LevelImg8bpp = new BitmapBits(bounds.Size);
			for (int y = Math.Max(bounds.Y / Level.ChunkHeight, 0); y <= Math.Min((bounds.Bottom - 1) / Level.ChunkHeight, Layout.BGLayout.GetLength(1) - 1); y++)
				for (int x = Math.Max(bounds.X / Level.ChunkWidth, 0); x <= Math.Min((bounds.Right - 1) / Level.ChunkWidth, Layout.BGLayout.GetLength(0) - 1); x++)
					if (Layout.BGLayout[x, y] < Chunks.Count)
					{
						bool xflip = false;
						bool yflip = false;
						if (Layout.BGXFlip != null)
							xflip = Layout.BGXFlip[x, y];
						if (Layout.BGYFlip != null)
							yflip = Layout.BGYFlip[x, y];

						ChunkSprites[Layout.BGLayout[x, y]].Flip(xflip, yflip);
						ChunkSprites[Layout.BGLayout[x, y]].Offset(Level.ChunkWidth * (xflip ? 1 : 0), Level.ChunkHeight * (yflip ? 1 : 0));
						ChunkColBmpBits[Layout.BGLayout[x, y]][0].Flip(xflip, yflip);
						ChunkColBmpBits[Layout.BGLayout[x, y]][1].Flip(xflip, yflip);

						if (lowPlane && highPlane)
							LevelImg8bpp.DrawSprite(ChunkSprites[Layout.BGLayout[x, y]], x * Level.ChunkWidth - bounds.X, y * Level.ChunkHeight - bounds.Y);
						else if (lowPlane)
							LevelImg8bpp.DrawSpriteLow(ChunkSprites[Layout.BGLayout[x, y]], x * Level.ChunkWidth - bounds.X, y * Level.ChunkHeight - bounds.Y);
						else if (highPlane)
							LevelImg8bpp.DrawSpriteHigh(ChunkSprites[Layout.BGLayout[x, y]], x * Level.ChunkWidth - bounds.X, y * Level.ChunkHeight - bounds.Y);
						if (collisionPath1)
							LevelImg8bpp.DrawBitmapComposited(ChunkColBmpBits[Layout.BGLayout[x, y]][0], x * Level.ChunkWidth - bounds.X, y * Level.ChunkHeight - bounds.Y);
						else if (collisionPath2)
							LevelImg8bpp.DrawBitmapComposited(ChunkColBmpBits[Layout.BGLayout[x, y]][1], x * Level.ChunkWidth - bounds.X, y * Level.ChunkHeight - bounds.Y);

						ChunkColBmpBits[Layout.BGLayout[x, y]][0].Flip(xflip, yflip);
						ChunkColBmpBits[Layout.BGLayout[x, y]][1].Flip(xflip, yflip);
						ChunkSprites[Layout.BGLayout[x, y]].Offset(-Level.ChunkWidth * (xflip ? 1 : 0), -Level.ChunkHeight * (yflip ? 1 : 0));
						ChunkSprites[Layout.BGLayout[x, y]].Flip(xflip, yflip);
					}
			return LevelImg8bpp;
		}

		public static bool ObjectVisible(ObjectEntry obj, bool allTimeZones)
		{
			if (allTimeZones)
				return true;
			if (obj is SCD.SCDObjectEntry scdobj)
				switch (Level.TimeZone)
				{
					case TimeZone.Past:
						return scdobj.ShowPast;
					case TimeZone.Present:
						return scdobj.ShowPresent;
					case TimeZone.Future:
						return scdobj.ShowFuture;
					default:
						return true;
				}
			return true;
		}

		private static void LoadObjectDefinitionFile(string file)
		{
			Log("Loading object definition file \"" + file + "\".");
			Dictionary<string, ObjectData> obj = IniSerializer.Deserialize<Dictionary<string, ObjectData>>(file);
			foreach (KeyValuePair<string, ObjectData> group in obj)
			{
				group.Value.Init();
				INIObjDefs[group.Key] = group.Value;
			}
		}

		private static void InitObjectDefinitions()
		{
			List<KeyValuePair<byte, ObjectDefinition>> objdefs = new List<KeyValuePair<byte, ObjectDefinition>>();
#if !DEBUG
			Parallel.ForEach(INIObjDefs, (KeyValuePair<string, ObjectData> group) =>
#else
			foreach (KeyValuePair<string, ObjectData> group in INIObjDefs)
#endif
			{
				if (group.Value.ArtCompression == CompressionType.Invalid)
					group.Value.ArtCompression = Game.ObjectArtCompression;
				if (group.Key == "Ring" && RingFormat is RingLayoutFormat)
					((RingLayoutFormat)RingFormat).Init(group.Value);
				else if (byte.TryParse(group.Key, System.Globalization.NumberStyles.HexNumber, System.Globalization.NumberFormatInfo.InvariantInfo, out byte ID))
				{
					ObjectDefinition def = null;
					if (group.Value.CodeFile != null)
					{
						string fulltypename = group.Value.CodeType;
						string dllfile = Path.Combine("dllcache", fulltypename + ".dll");
						DateTime modDate = DateTime.MinValue;
						if (File.Exists(dllfile))
							modDate = File.GetLastWriteTime(dllfile);
						string fp = group.Value.CodeFile.Replace('/', Path.DirectorySeparatorChar);
						Log("Loading ObjectDefinition type " + fulltypename + " from \"" + fp + "\"...");
						if (modDate >= File.GetLastWriteTime(fp) & modDate > File.GetLastWriteTime(Application.ExecutablePath))
						{
							Log("Loading type from cached assembly \"" + dllfile + "\"...");
							def = (ObjectDefinition)Activator.CreateInstance(System.Reflection.Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, dllfile)).GetType(fulltypename));
						}
						else
						{
							Log("Compiling code file...");
							string ext = Path.GetExtension(fp);
							CodeDomProvider pr = null;
							switch (ext.ToLowerInvariant())
							{
								case ".cs":
									pr = new Microsoft.CSharp.CSharpCodeProvider();
									break;
								case ".vb":
									pr = new Microsoft.VisualBasic.VBCodeProvider();
									break;
							}
							if (pr != null)
							{
								CompilerParameters para = new CompilerParameters(new string[] { "System.dll", "System.Core.dll", "System.Drawing.dll", System.Reflection.Assembly.GetExecutingAssembly().Location })
								{
									GenerateExecutable = false,
									GenerateInMemory = false,
									IncludeDebugInformation = true,
									OutputAssembly = Path.Combine(Environment.CurrentDirectory, dllfile)
								};
								CompilerResults res = pr.CompileAssemblyFromFile(para, fp);
								if (res.Errors.HasErrors)
								{
									Log("Compile failed.", "Errors:");
									foreach (CompilerError item in res.Errors)
										Log(item.ToString());
									Log(string.Empty);
									def = new DefaultObjectDefinition();
								}
								else
								{
									Log("Compile succeeded.");
									def = (ObjectDefinition)Activator.CreateInstance(res.CompiledAssembly.GetType(fulltypename));
								}
							}
							else
								def = new DefaultObjectDefinition();
						}
					}
					else if (group.Value.XMLFile != null)
						def = new XMLObjectDefinition();
					else
						def = new DefaultObjectDefinition();
					lock (objdefs)
						objdefs.Add(new KeyValuePair<byte, ObjectDefinition>(ID, def));
#if !DEBUG
					try
#endif
					{
						def.Init(group.Value);
					}
#if !DEBUG
					catch (Exception ex)
					{
						Log("Object definition " + ID.ToString("X2") + " failed to initialize!");
						Log(ex.ToString());
						def = new DefaultObjectDefinition();
						lock (objdefs)
							objdefs.Add(new KeyValuePair<byte, ObjectDefinition>(ID, def));
						def.Init(new ObjectData());
					}
#endif
				}
#if !DEBUG
			});
#else
			}
#endif
			foreach (var item in objdefs.OrderBy(a => a.Key))
				ObjTypes[item.Key] = item.Value;
		}

		internal static string ExpandTypeName(string type)
		{
			switch (type)
			{
				case "bool":
					return typeof(bool).FullName;
				case "byte":
					return typeof(byte).FullName;
				case "char":
					return typeof(char).FullName;
				case "decimal":
					return typeof(decimal).FullName;
				case "double":
					return typeof(double).FullName;
				case "float":
					return typeof(float).FullName;
				case "int":
					return typeof(int).FullName;
				case "long":
					return typeof(long).FullName;
				case "object":
					return typeof(object).FullName;
				case "sbyte":
					return typeof(sbyte).FullName;
				case "short":
					return typeof(short).FullName;
				case "string":
					return typeof(string).FullName;
				case "uint":
					return typeof(uint).FullName;
				case "ulong":
					return typeof(ulong).FullName;
				case "ushort":
					return typeof(ushort).FullName;
				default:
					return type;
			}
		}

		public static byte[] ReadFile(string file, CompressionType cmp)
		{
			lock (filecache)
				if (file == "LevelArt")
					return TileArray;
				else if (filecache.ContainsKey(file))
					return filecache[file];
				else
				{
					byte[] val = Compression.Decompress(file, cmp);
					filecache.Add(file, val);
					return val;
				}
		}

		public static Bitmap TileToBmp4bpp(byte[] file, int index, int palette, bool transparent)
		{
			Bitmap bmp = new Bitmap(8, 8, PixelFormat.Format4bppIndexed);
			if (file != null && index * 32 + 32 <= file.Length)
			{
				BitmapData bmpd = bmp.LockBits(new Rectangle(0, 0, 8, 8), ImageLockMode.WriteOnly, PixelFormat.Format4bppIndexed);
				System.Runtime.InteropServices.Marshal.Copy(file, index * 32, bmpd.Scan0, 32);
				bmp.UnlockBits(bmpd);
			}
			else
				InvalidTile.ToBitmap4bpp(bmp);
			ColorPalette pal = bmp.Palette;
			for (int i = 0; i < 16; i++)
				pal.Entries[i] = PaletteToColor(palette, i, transparent);
			bmp.Palette = pal;
			return bmp;
		}

		public static Bitmap InterlacedTileToBmp4bpp(byte[] file, int index, int palette)
		{
			Bitmap bmp = new Bitmap(8, 16, PixelFormat.Format4bppIndexed);
			if (file != null && index * 32 + 64 <= file.Length)
			{
				BitmapData bmpd = bmp.LockBits(new Rectangle(0, 0, 8, 16), ImageLockMode.WriteOnly, PixelFormat.Format4bppIndexed);
				System.Runtime.InteropServices.Marshal.Copy(file, index * 32, bmpd.Scan0, 64);
				bmp.UnlockBits(bmpd);
			}
			ColorPalette pal = bmp.Palette;
			for (int i = 0; i < 16; i++)
				pal.Entries[i] = PaletteToColor(palette, i, false);
			bmp.Palette = pal;
			return bmp;
		}

		public static BitmapBits TileToBmp8bpp(byte[] file, int index, int pal)
		{
			BitmapBits bmp;
			if (file != null && index * 32 + 32 <= file.Length)
			{
				bmp = new BitmapBits(8, 8);
				for (int i = 0; i < 32; i++)
				{
					bmp.Bits[i * 2] = (byte)((file[i + (index * 32)] >> 4) + (pal * 16));
					bmp.Bits[(i * 2) + 1] = (byte)((file[i + (index * 32)] & 0xF) + (pal * 16));
					if (bmp.Bits[i * 2] % 16 == 0) bmp.Bits[i * 2] = 0;
					if (bmp.Bits[(i * 2) + 1] % 16 == 0) bmp.Bits[(i * 2) + 1] = 0;
				}
			}
			else
			{
				bmp = new BitmapBits(InvalidTile);
				bmp.IncrementIndexes(pal * 16);
			}
			return bmp;
		}

		public static BitmapBits InterlacedTileToBmp8bpp(byte[] file, int index, int pal)
		{
			BitmapBits bmp = new BitmapBits(8, 16);
			if (file != null && index * 32 + 64 <= file.Length)
				for (int i = 0; i < 64; i++)
				{
					bmp.Bits[i * 2] = (byte)((file[i + (index * 32)] >> 4) + (pal * 16));
					bmp.Bits[(i * 2) + 1] = (byte)((file[i + (index * 32)] & 0xF) + (pal * 16));
					if (bmp.Bits[i * 2] % 16 == 0) bmp.Bits[i * 2] = 0;
					if (bmp.Bits[(i * 2) + 1] % 16 == 0) bmp.Bits[(i * 2) + 1] = 0;
				}
			else if (file != null && index * 32 + 32 <= file.Length)
			{
				for (int i = 0; i < 32; i++)
				{
					bmp.Bits[i * 2] = (byte)((file[i + (index * 32)] >> 4) + (pal * 16));
					bmp.Bits[(i * 2) + 1] = (byte)((file[i + (index * 32)] & 0xF) + (pal * 16));
					if (bmp.Bits[i * 2] % 16 == 0) bmp.Bits[i * 2] = 0;
					if (bmp.Bits[(i * 2) + 1] % 16 == 0) bmp.Bits[(i * 2) + 1] = 0;
				}
				BitmapBits tmp = new BitmapBits(InvalidTile);
				tmp.IncrementIndexes(pal * 16);
				bmp.DrawBitmap(tmp, 0, 8);
			}
			else
			{
				BitmapBits tmp = new BitmapBits(InvalidTile);
				tmp.IncrementIndexes(pal * 16);
				bmp.DrawBitmap(tmp, 0, 0);
				bmp.DrawBitmap(tmp, 0, 8);
			}
			return bmp;
		}

		public static BitmapBits TileToBmp8bpp(byte[] tiles, PatternIndex index)
		{
			BitmapBits bmp;
			if (tiles != null && index.Tile * 32 + 32 <= tiles.Length)
			{
				bmp = new BitmapBits(8, 8);
				for (int i = 0; i < 32; i++)
				{
					bmp.Bits[i * 2] = (byte)((tiles[i + (index.Tile * 32)] >> 4) + (index.Palette * 16));
					bmp.Bits[(i * 2) + 1] = (byte)((tiles[i + (index.Tile * 32)] & 0xF) + (index.Palette * 16));
					if (bmp.Bits[i * 2] % 16 == 0) bmp.Bits[i * 2] = 0;
					if (bmp.Bits[(i * 2) + 1] % 16 == 0) bmp.Bits[(i * 2) + 1] = 0;
				}
			}
			else
			{
				bmp = new BitmapBits(InvalidTile);
				bmp.IncrementIndexes(index.Palette * 16);
			}
			bmp.Flip(index.XFlip, index.YFlip);
			return bmp;
		}

		public static int[] GetOffsetList(byte[] file)
		{
			short endlist = short.MaxValue;
			List<int> addresses = new List<int>();
			for (int i = 0; i < endlist; i += 2)
			{
				short val = ByteConverter.ToInt16(file, i);
				if (val > -1)
				{
					addresses.Add(val);
					if (val > 0)
						endlist = Math.Min(endlist, val);
				}
			}
			return addresses.ToArray();
		}

		public static byte[] ASMToBin(string file, EngineVersion version) { return ASMToBin(file, version, out Dictionary<string, int> labels); }

		public static byte[] ASMToBin(string file, EngineVersion version, out Dictionary<string, int> labels) { return ASMToBin(file, 0, version, out labels); }

		public static byte[] ASMToBin(string file, string label, EngineVersion version) { return ASMToBin(file, label, version, out Dictionary<string, int> labels); }

		public static byte[] ASMToBin(string file, string label, EngineVersion version, out Dictionary<string, int> labels)
		{
			string[] fc = File.ReadAllLines(file);
			int sti = -1;
			for (int i = 0; i < fc.Length; i++)
			{
				if (fc[i].StartsWith(label + ":"))
				{
					sti = i;
					fc[i] = fc[i].Substring(label.Length + 1);
				}
			}
			if (sti == -1)
			{
				labels = new Dictionary<string, int>();
				return new byte[0];
			}
			return ASMToBin(file, sti, version, out labels);
		}

		public static byte[] ASMToBin(string file, int sti, EngineVersion version) { return ASMToBin(file, sti, version, out Dictionary<string, int> labels); }

		public static readonly Dictionary<string, int> asmlabels = new Dictionary<string, int>() {
			{ "afEnd", 0xFF }, // return to beginning of animation
			{ "afBack", 0xFE },// go back (specified number) bytes
			{ "afChange", 0xFD }, // run specified animation
			{ "afRoutine", 0xFC }, // increment routine counter
			{ "afReset", 0xFB }, // reset animation and 2nd object routine counter
			{ "af2ndRoutine", 0xFA } // increment 2nd routine counter
		};

		public static byte[] ASMToBin(string file, int sti, EngineVersion version, out Dictionary<string, int> labels)
		{
			labels = GetASMLabels(file, sti, version);
			foreach (KeyValuePair<string, int> item in asmlabels)
				if (!labels.ContainsKey(item.Key))
					labels.Add(item.Key, item.Value);
			string[] fc = File.ReadAllLines(file);
			List<byte> result = new List<byte>();
			string lastlabel = string.Empty;
			string offsetLabel = string.Empty;
			for (int st = sti; st < fc.Length; st++)
			{
				string[] ln = fc[st].Split(';')[0].Trim().Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
				if (ln.Length == 0) continue;
				if (!char.IsWhiteSpace(fc[st], 0))
				{
					string[] l = ln[0].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
					lastlabel = l[0];
					if (l.Length == 1)
					{
						string[] ln2 = new string[ln.Length - 1];
						for (int i = 0; i < ln2.Length; i++)
							ln2[i] = ln[i + 1];
						ln = ln2;
					}
					else
						ln[0] = l[1];
					if (ln.Length == 0) continue;
				}
				if (ln[0].Equals("even"))
				{
					if (result.Count % 2 == 1)
						result.Add(0);
				}
				else if (ln[0].Equals("align"))
				{
					uint alignment = (uint)ParseASMNum(ln[1], labels);
					if (result.Count % alignment != 0)
						result.AddRange(new byte[alignment - (result.Count % alignment)]);
				}
				else if (ln[0].Equals("offsetTable") | ln[0].Equals("mappingsTable"))
					offsetLabel = lastlabel;
				else if (ln[0].StartsWith("offsetTableEntry.") | ln[0].StartsWith("mappingsTableEntry."))
					switch (ln[0].Split('.')[1])
					{
						case "b":
							result.Add(unchecked((byte)LabelSubtract(ln[1], offsetLabel, labels)));
							break;
						case "w":
							result.AddRange(ByteConverter.GetBytes((short)LabelSubtract(ln[1], offsetLabel, labels)));
							break;
						case "l":
							result.AddRange(ByteConverter.GetBytes(LabelSubtract(ln[1], offsetLabel, labels)));
							break;
					}
				else if (ln[0].StartsWith("dc."))
				{
					string d = string.Empty;
					for (int i = 1; i < ln.Length; i++)
						d += ln[i];
					string[] dats = d.Split(',');
					switch (ln[0].Split('.')[1])
					{
						case "b":
							foreach (string item in dats)
								if (!item.StartsWith("-") && item.Contains("-"))
									result.Add(unchecked((byte)ParseASMOffset(item, labels)));
								else
									result.Add((byte)ParseASMNum(item, labels));
							break;
						case "w":
							foreach (string item in dats)
								if (!item.StartsWith("-") && item.Contains("-"))
									result.AddRange(ByteConverter.GetBytes((short)ParseASMOffset(item, labels)));
								else
									result.AddRange(ByteConverter.GetBytes((ushort)ParseASMNum(item, labels)));
							break;
						case "l":
							foreach (string item in dats)
								if (!item.StartsWith("-") && item.Contains("-"))
									result.AddRange(ByteConverter.GetBytes(ParseASMOffset(item, labels)));
								else
									result.AddRange(ByteConverter.GetBytes((uint)ParseASMNum(item, labels)));
							break;
					}
				}
				else if (ln[0].Equals("spriteHeader"))
				{
					switch (version)
					{
						case EngineVersion.S1:
							result.Add(unchecked((byte)((LabelSubtract(lastlabel + "_End", lastlabel, labels) - 1) / 5)));
							break;
						case EngineVersion.S2:
							result.AddRange(ByteConverter.GetBytes((short)((LabelSubtract(lastlabel + "_End", lastlabel, labels) - 2) / 8)));
							break;
						case EngineVersion.S3K:
							result.AddRange(ByteConverter.GetBytes((short)((LabelSubtract(lastlabel + "_End", lastlabel, labels) - 2) / 6)));
							break;
					}
				}
				else if (ln[0].Equals("spritePiece"))
				{
					string d = string.Empty;
					for (int i = 1; i < ln.Length; i++)
						d += ln[i];
					string[] dats = d.Split(',');
					unchecked { result.AddRange(new MappingsTile((short)ParseASMNum(dats[0], labels), (short)ParseASMNum(dats[1], labels), (byte)ParseASMNum(dats[2], labels), (byte)ParseASMNum(dats[3], labels), (ushort)ParseASMNum(dats[4], labels), (byte)ParseASMNum(dats[5], labels) == 1, (byte)ParseASMNum(dats[6], labels) == 1, (byte)ParseASMNum(dats[7], labels), (byte)ParseASMNum(dats[8], labels) == 1).GetBytes(version)); }
				}
				else if (ln[0].Equals("spritePiece2P"))
				{
					string d = string.Empty;
					for (int i = 1; i < ln.Length; i++)
						d += ln[i];
					string[] dats = d.Split(',');
					unchecked { result.AddRange(new MappingsTile((short)ParseASMNum(dats[0], labels), (short)ParseASMNum(dats[1], labels), (byte)ParseASMNum(dats[2], labels), (byte)ParseASMNum(dats[3], labels), (ushort)ParseASMNum(dats[4], labels), (byte)ParseASMNum(dats[5], labels) == 1, (byte)ParseASMNum(dats[6], labels) == 1, (byte)ParseASMNum(dats[7], labels), (byte)ParseASMNum(dats[8], labels) == 1, (ushort)ParseASMNum(dats[9], labels), (byte)ParseASMNum(dats[10], labels) == 1, (byte)ParseASMNum(dats[11], labels) == 1, (byte)ParseASMNum(dats[12], labels), (byte)ParseASMNum(dats[13], labels) == 1).GetBytes(version)); }
				}
				else if (ln[0].Equals("dplcHeader"))
				{
					switch (version)
					{
						case EngineVersion.S1:
							result.Add(unchecked((byte)((LabelSubtract(lastlabel + "_End", lastlabel, labels) - 1) / 2)));
							break;
						case EngineVersion.S2:
							result.AddRange(ByteConverter.GetBytes((short)((LabelSubtract(lastlabel + "_End", lastlabel, labels) - 2) / 2)));
							break;
						case EngineVersion.S3K:
							result.AddRange(ByteConverter.GetBytes((short)((LabelSubtract(lastlabel + "_End", lastlabel, labels) - 4) / 2)));
							break;
					}
				}
				else if (ln[0].Equals("s3kPlayerDplcHeader"))
					result.AddRange(ByteConverter.GetBytes((short)((LabelSubtract(lastlabel + "_End", lastlabel, labels) - 2) / 2)));
				else if (ln[0].Equals("dplcEntry"))
				{
					string d = string.Empty;
					for (int i = 1; i < ln.Length; i++)
						d += ln[i];
					string[] dats = d.Split(',');
					unchecked { result.AddRange(new DPLCEntry((byte)ParseASMNum(dats[0], labels), (ushort)ParseASMNum(dats[1], labels)).GetBytes(version)); }
				}
				else if (ln[0].Equals("s3kPlayerDplcEntry"))
				{
					string d = string.Empty;
					for (int i = 1; i < ln.Length; i++)
						d += ln[i];
					string[] dats = d.Split(',');
					unchecked { result.AddRange(new DPLCEntry((byte)ParseASMNum(dats[0], labels), (ushort)ParseASMNum(dats[1], labels)).GetBytes(EngineVersion.S2)); }
				}
				else if (ln[0].Equals("obj1E67Size"))
					result.AddRange(ByteConverter.GetBytes((short)(LabelSubtract(lastlabel + "_End", lastlabel, labels) - 2)));
				else
					break;
			}
			return result.ToArray();
		}

		public static long ParseASMNum(string data, Dictionary<string, int> labels)
		{
			data = data.Trim();
			bool neg = false;
			if (data.StartsWith("-"))
			{
				neg = true;
				data = data.Substring(1);
			}
			long result;
			if (labels.ContainsKey(data))
				result = labels[data];
			else if (data.StartsWith("$"))
				result = long.Parse(data.Substring(1), System.Globalization.NumberStyles.HexNumber);
			else if (data.StartsWith("%"))
				result = Convert.ToInt64(data.Substring(1), 2);
			else
				result = long.Parse(data, System.Globalization.NumberStyles.None, System.Globalization.NumberFormatInfo.InvariantInfo);
			if (neg)
				return -result;
			return result;
		}

		public static int ParseASMOffset(string data, Dictionary<string, int> labels)
		{
			return LabelSubtract(data.Split('-')[0], data.Split('-')[1], labels);
		}

		public static int LabelSubtract(string label1, string label2, Dictionary<string, int> labels)
		{
			return (labels.ContainsKey(label1) ? labels[label1] : 0) - (labels.ContainsKey(label2) ? labels[label2] : 0);
		}

		public static Dictionary<string, int> GetASMLabels(string file, EngineVersion version)
		{
			return GetASMLabels(file, 0, version);
		}

		public static Dictionary<string, int> GetASMLabels(string file, string label, EngineVersion version)
		{
			string[] fc = File.ReadAllLines(file);
			int sti = -1;
			for (int i = 0; i < fc.Length; i++)
			{
				if (fc[i].StartsWith(label + ":"))
				{
					sti = i;
					fc[i] = fc[i].Substring(label.Length + 1);
				}
			}
			if (sti == -1) return new Dictionary<string, int>();
			return GetASMLabels(file, sti, version);
		}


		public static Dictionary<string, int> GetASMLabels(string file, int sti, EngineVersion version)
		{
			string[] fc = File.ReadAllLines(file);
			Dictionary<string, int> labels = new Dictionary<string, int>();
			int curaddr = 0;
			for (int st = sti; st < fc.Length; st++)
			{
				string[] ln = fc[st].Split(';')[0].Trim().Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
				if (ln.Length == 0) continue;
				if (!char.IsWhiteSpace(fc[st], 0))
				{
					string[] l = ln[0].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
					labels.Add(l[0], curaddr);
					if (l.Length == 1)
					{
						string[] ln2 = new string[ln.Length - 1];
						for (int i = 0; i < ln2.Length; i++)
						{
							ln2[i] = ln[i + 1];
						}
						ln = ln2;
					}
					else
					{
						ln[0] = l[1];
					}
					if (ln.Length == 0) continue;
				}
				if (ln[0].Equals("even"))
				{
					if (curaddr % 2 == 1)
						curaddr++;
				}
				else if (ln[0].Equals("align"))
				{
					uint alignment = (uint)ParseASMNum(ln[1], labels);
					if (curaddr % alignment != 0)
						curaddr += (int)(alignment - (curaddr % alignment));
				}
				else if (ln[0].Equals("offsetTable") | ln[0].Equals("mappingsTable"))
					continue;
				else if (ln[0].StartsWith("offsetTableEntry.") | ln[0].StartsWith("mappingsTableEntry."))
					switch (ln[0].Split('.')[1])
					{
						case "b":
							curaddr++;
							break;
						case "w":
							curaddr += 2;
							break;
						case "l":
							curaddr += 4;
							break;
					}
				else if (ln[0].StartsWith("dc."))
				{
					string d = string.Empty;
					for (int i = 1; i < ln.Length; i++)
						d += ln[i];
					string[] dats = d.Split(',');
					switch (ln[0].Split('.')[1])
					{
						case "b":
							curaddr += dats.Length;
							break;
						case "w":
							curaddr += dats.Length * 2;
							break;
						case "l":
							curaddr += dats.Length * 4;
							break;
					}
				}
				else if (ln[0].Equals("spriteHeader") | ln[0].Equals("dplcHeader"))
					switch (version)
					{
						case EngineVersion.S1:
							curaddr++;
							break;
						case EngineVersion.S2:
						case EngineVersion.S3K:
							curaddr += 2;
							break;
					}
				else if (ln[0].Equals("s3kPlayerDplcHeader"))
					curaddr += 2;
				else if (ln[0].Equals("spritePiece") | ln[0].Equals("spritePiece2P"))
					switch (version)
					{
						case EngineVersion.S1:
							curaddr += 5;
							break;
						case EngineVersion.S2:
							curaddr += 8;
							break;
						case EngineVersion.S3K:
							curaddr += 6;
							break;
					}
				else if (ln[0].Equals("dplcEntry") | ln[0].Equals("s3kPlayerDplcEntry"))
					curaddr += 2;
				else
					break;
			}
			return labels;
		}

		public static byte[] ProcessDPLC(byte[] artfile, DPLCFrame dplc)
		{
			List<byte> result = new List<byte>();
			byte[] tmp;
			for (int i = 0; i < dplc.Count; i++)
			{
				tmp = new byte[dplc[i].TileCount * 0x20];
				Array.Copy(artfile, dplc[i].TileNum * 0x20, tmp, 0, tmp.Length);
				result.AddRange(tmp);
			}
			return result.ToArray();
		}

		public static Sprite MapFrameToBmp(byte[] art, MappingsFrame map, int startpal, bool priority = false)
		{
			return MapFrameToBmp(art, map, null, startpal, priority);
		}

		public static Sprite MapFrameToBmp(byte[] art, MappingsFrame map, DPLCFrame dplc, int startpal, bool priority = false)
		{
			if (dplc != null)
				art = ProcessDPLC(art, dplc);
			List<Sprite> sprites = new List<Sprite>(map.TileCount);
			for (int i = map.TileCount - 1; i >= 0; i--)
				sprites.Add(MapTileToBmp(art, map[i], startpal, priority));
			return new Sprite(sprites);
		}

		public static Sprite MapTileToBmp(byte[] art, MappingsTile map, int startpal, bool priority = false)
		{
			BitmapBits pcbmp = new BitmapBits(map.Width * 8, map.Height * 8);
			bool extrapal = startpal >= 4;
			startpal = (startpal & 3) + (priority ? 4 : 0);
			startpal += map.Tile.Palette + (map.Tile.Priority ? 4 : 0);
			priority = (startpal & 4) == 4;
			startpal &= 3;
			if (extrapal) startpal += 4;
			int ti = 0;
			for (int x = 0; x < map.Width; x++)
				for (int y = 0; y < map.Height; y++)
				{
					pcbmp.DrawBitmap(TileToBmp8bpp(art, map.Tile.Tile + ti, startpal), x * 8, y * 8);
					ti++;
				}
			pcbmp.Flip(map.Tile.XFlip, map.Tile.YFlip);
			return new Sprite(pcbmp, priority, map.X, map.Y);
		}

		public static Color PaletteToColor(int line, int index, bool acceptTransparent)
		{
			if (acceptTransparent && index == 0)
				return Color.Transparent;
			return Palette[CurPal][line, index].RGBColor;
		}

		public static void ColorToPalette(int line, int index, Color color)
		{
			Palette[CurPal][line, index] = new SonLVLColor(color);
		}

		public static void RedrawBlocksUsingTiles(IEnumerable<int> tiles, bool drawChunks)
		{
			List<int> blocks = new List<int>();
			foreach (int tile in tiles)
				for (int i = 0; i < Blocks.Count; i++)
				{
					if (blocks.Contains(i)) continue;
					for (int k = 0; k < 2; k++)
						for (int j = 0; j < 2; j++)
							if (Blocks[i].Tiles[j, k].Tile == tile)
							{
								blocks.Add(i);
								goto nextblock;
							}
					nextblock:;
				}
			RedrawBlocks(blocks, drawChunks);
		}

		public static void RedrawBlocks(IEnumerable<int> blocks, bool drawChunks)
		{
			List<int> chunks = new List<int>();
			foreach (int block in blocks)
			{
				RedrawBlock(block, false);
				if (drawChunks)
					for (int i = 0; i < Chunks.Count; i++)
					{
						if (chunks.Contains(i)) continue;
						for (int k = 0; k < Level.ChunkHeight / 16; k++)
							for (int j = 0; j < Level.ChunkWidth / 16; j++)
								if (Chunks[i].Blocks[j, k].Block == block)
								{
									chunks.Add(i);
									goto nextchunk;
								}
					nextchunk: ;
					}
			}
			if (drawChunks)
				RedrawChunks(chunks);
		}

		public static void RedrawBlock(int block, bool drawChunks)
		{
			BlockBmpBits[block][0] = new BitmapBits(16, 16);
			BlockBmpBits[block][1] = new BitmapBits(16, 16);
			CompBlockBmpBits[block] = new BitmapBits(16, 16);
			for (int by = 0; by < 2; by++)
				for (int bx = 0; bx < 2; bx++)
				{
					PatternIndex pt = Blocks[block].Tiles[bx, by];
					int pr = pt.Priority ? 1 : 0;
					BitmapBits tile = TileToBmp8bpp(AnimatedTiles?[pt.Tile] ?? Tiles[pt.Tile], 0, pt.Palette);
					tile.Flip(pt.XFlip, pt.YFlip);
					BlockBmpBits[block][pr].DrawBitmap(tile, bx * 8, by * 8);
				}
			CompBlockBmpBits[block].DrawBitmap(BlockBmpBits[block][0], Point.Empty);
			CompBlockBmpBits[block].DrawBitmapComposited(BlockBmpBits[block][1], Point.Empty);
			BlockBmps[block][0] = BlockBmpBits[block][0].ToBitmap(BmpPal);
			BlockBmps[block][1] = BlockBmpBits[block][1].ToBitmap(BmpPal);
			CompBlockBmps[block] = CompBlockBmpBits[block].ToBitmap(BmpPal);
			if (drawChunks)
				for (int i = 0; i < Chunks.Count; i++)
				{
					bool dr = false;
					for (int k = 0; k < Level.ChunkHeight / 16; k++)
						for (int j = 0; j < Level.ChunkWidth / 16; j++)
							if (Chunks[i].Blocks[j, k].Block == block)
								dr = true;
					if (dr)
						RedrawChunk(i);
				}
		}

		public static void RedrawChunks(IEnumerable<int> chunks)
		{
			foreach (int chunk in chunks)
				RedrawChunk(chunk);
		}

		public static void RedrawChunk(int chunk)
		{
			BitmapBits tmplow = new BitmapBits(Level.ChunkWidth, Level.ChunkHeight);
			BitmapBits tmphigh = new BitmapBits(Level.ChunkWidth, Level.ChunkHeight);
			ChunkColBmpBits[chunk][0] = new BitmapBits(Level.ChunkWidth, Level.ChunkHeight);
			ChunkColBmpBits[chunk][1] = new BitmapBits(Level.ChunkWidth, Level.ChunkHeight);
			for (int by = 0; by < Level.ChunkHeight / 16; by++)
				for (int bx = 0; bx < Level.ChunkWidth / 16; bx++)
				{
					ChunkBlock blk = Chunks[chunk].Blocks[bx, by];
					BitmapBits[] blkbmp;
					if (AnimatedBlockBmpBits != null && blk.Block >= AnimatedBlockOffset && blk.Block < AnimatedBlockOffset + AnimatedBlockBmpBits.Count)
						blkbmp = AnimatedBlockBmpBits[blk.Block - AnimatedBlockOffset];
					else if (blk.Block < BlockBmpBits.Count)
						blkbmp = BlockBmpBits[blk.Block];
					else
						blkbmp = InvalidBlock;
					BitmapBits bmp = new BitmapBits(blkbmp[0]);
					bmp.Flip(blk.XFlip, blk.YFlip);
					tmplow.DrawBitmap(bmp, bx * 16, by * 16);
					bmp = new BitmapBits(blkbmp[1]);
					bmp.Flip(blk.XFlip, blk.YFlip);
					tmphigh.DrawBitmap(bmp, bx * 16, by * 16);
					if (ColInds1.Count > 0)
					{
						bmp = new BitmapBits(ColBmpBits[GetColInd1(blk.Block)]);
						bmp.IncrementIndexes((int)blk.Solid1 - 1);
						bmp.Flip(blk.XFlip, blk.YFlip);
						ChunkColBmpBits[chunk][0].DrawBitmap(bmp, bx * 16, by * 16);
						if (blk is S2ChunkBlock)
						{
							bmp = new BitmapBits(ColBmpBits[GetColInd2(blk.Block)]);
							bmp.IncrementIndexes((int)((S2ChunkBlock)blk).Solid2 - 1);
							bmp.Flip(blk.XFlip, blk.YFlip);
							ChunkColBmpBits[chunk][1].DrawBitmap(bmp, bx * 16, by * 16);
						}
					}
				}
			ChunkSprites[chunk] = new Sprite(tmplow, tmphigh);
			ChunkBmps[chunk][0] = tmplow.ToBitmap(BmpPal);
			ChunkBmps[chunk][1] = tmphigh.ToBitmap(BmpPal);
			ChunkColBmps[chunk][0] = ChunkColBmpBits[chunk][0].ToBitmap(Color.Transparent, Color.White, Color.Yellow, Color.Black);
			ChunkColBmps[chunk][1] = ChunkColBmpBits[chunk][1].ToBitmap(Color.Transparent, Color.White, Color.Yellow, Color.Black);
			ChunkColBmpBits[chunk][0].FixUIColors();
			ChunkColBmpBits[chunk][1].FixUIColors();
			tmplow.DrawBitmapComposited(tmphigh, 0, 0);
			CompChunkBmps[chunk] = tmplow.ToBitmap(BmpPal);
		}

		public static ObjectDefinition GetObjectDefinition(byte ID)
		{
			if (ObjTypes != null && ObjTypes.ContainsKey(ID))
				return ObjTypes[ID];
			else
				return unkobj;
		}

		public static ObjectEntry CreateObject(byte ID)
		{
			ObjectDefinition def = GetObjectDefinition(ID);
			ObjectEntry oe = ObjectFormat.CreateObject();
			oe.ID = ID;
			oe.SubType = def.DefaultSubtype;
			if (oe is RememberStateObjectEntry)
				((RememberStateObjectEntry)oe).RememberState = def.RememberState;
			return oe;
		}

		public static void PaletteChanged()
		{
			for (int i = 0; i < 64; i++)
				BmpPal.Entries[i] = PaletteToColor(i / 16, i % 16, true);
			BmpPal.Entries[ColorWhite] = Color.White;
			BmpPal.Entries[ColorYellow] = Color.Yellow;
			BmpPal.Entries[ColorBlack] = Color.Black;
			BmpPal.Entries[ColorWhite | 0x80] = Color.White;
			BmpPal.Entries[ColorYellow | 0x80] = Color.Yellow;
			BmpPal.Entries[ColorBlack | 0x80] = Color.Black;
			foreach (Bitmap[] item in BlockBmps)
			{
				item[0].Palette = BmpPal;
				item[1].Palette = BmpPal;
			}
			foreach (Bitmap item in CompBlockBmps)
				item.Palette = BmpPal;
			foreach (Bitmap[] item in ChunkBmps)
			{
				item[0].Palette = BmpPal;
				item[1].Palette = BmpPal;
			}
			foreach (Bitmap item in CompChunkBmps)
				item.Palette = BmpPal;
			PaletteChangedEvent();
		}

		public static void RedrawCol(int block, bool drawChunks)
		{
			ColBmpBits[block] = new BitmapBits(16, 16);
			for (int by = 0; by < 16; by++)
			{
				for (int bx = 0; bx < 16; bx++)
				{
					switch (Math.Sign(ColArr1[block][bx]))
					{
						case -1:
							if (16 + ColArr1[block][bx] <= by)
								ColBmpBits[block].Bits[((15 - by) * 16) + bx] = 1;
							break;
						case 1:
							if (ColArr1[block][bx] > by)
								ColBmpBits[block].Bits[((15 - by) * 16) + bx] = 1;
							break;
					}
				}
			}
			ColBmps[block] = ColBmpBits[block].ToBitmap(Color.Transparent, Color.White);
			if (drawChunks)
			{
				for (int i = 0; i < Chunks.Count; i++)
				{
					bool dr = false;
					for (int k = 0; k < Level.ChunkHeight / 16; k++)
						for (int j = 0; j < Level.ChunkWidth / 16; j++)
							if (GetColInd1(Chunks[i].Blocks[j, k].Block) == block | GetColInd2(Chunks[i].Blocks[j, k].Block) == block)
								dr = true;
					if (dr)
						RedrawChunk(i);
				}
			}
		}

		public static sbyte[][] GenerateRotatedCollision()
		{
			sbyte[][] result = new sbyte[256][];
			for (int i = 0; i < 256; i++)
			{
				result[i] = new sbyte[16];
				for (int y = 0; y < 16; y++)
				{
					sbyte height = 0;
					int misses = 0;
					bool f = false;
					int st = 16;
					for (int x = 0; x < 16; x++)
					{
						switch (Math.Sign(ColArr1[i][15 - x]))
						{
							case 1:
								if (ColArr1[i][15 - x] >= 16 - y)
								{
									height = (sbyte)(x + 1);
									misses = 0;
									if (!f)
									{
										f = true;
										st = x;
									}
								}
								else
									misses++;
								break;
							case 0:
								misses++;
								break;
							case -1:
								if (ColArr1[i][15 - x] <= -y - 1)
								{
									height = (sbyte)(x + 1);
									misses = 0;
									if (!f)
									{
										f = true;
										st = x;
									}
								}
								else
									misses++;
								break;
						}
						if (x == 0 & misses == 1)
							break;
						if (misses == 3)
							break;
					}
					sbyte negheight = 0;
					misses = 0;
					f = false;
					int rst = 16;
					for (int x = 0; x < 16; x++)
					{
						switch (Math.Sign(ColArr1[i][x]))
						{
							case 1:
								if (ColArr1[i][x] >= 16 - y)
								{
									negheight = (sbyte)(-x - 1);
									misses = 0;
									if (!f)
									{
										f = true;
										rst = x;
									}
								}
								else
									misses++;
								break;
							case 0:
								misses++;
								break;
							case -1:
								if (ColArr1[i][x] <= -y - 1)
								{
									negheight = (sbyte)(-x - 1);
									misses = 0;
									if (!f)
									{
										f = true;
										rst = x;
									}
								}
								else
									misses++;
								break;
						}
						if (x == 0 & misses == 1)
							break;
						if (misses == 3)
							break;
					}
					result[i][y] = height;
					if (Math.Abs(negheight) > height || st > rst)
						result[i][y] = negheight;
					if (rst > st)
						result[i][y] = height;
				}
			}
			return result;
		}

		public static ImportResult BitmapToTiles(BitmapInfo bmpi, bool[,] priority, byte? forcepal, List<byte[]> tiles, bool interlaced, bool optimize, Action updateProgress = null)
		{
			int w = bmpi.Width / 8;
			int h = bmpi.Height / (interlaced ? 16 : 8);
			ImportResult result = new ImportResult(w, bmpi.Height / 8);
			if (!interlaced)
			{
				for (int y = 0; y < bmpi.Height / 8; y++)
					for (int x = 0; x < bmpi.Width / 8; x++)
					{
						PatternIndex map = new PatternIndex() { Priority = priority[x, y] };
						byte[] tile = BmpToTile(new BitmapInfo(bmpi, x * 8, y * 8, 8, 8), forcepal, out int pal);
						map.Palette = (byte)pal;
						bool match = false;
						if (optimize)
						{
							byte[] tileh = FlipTile(tile, true, false);
							byte[] tilev = FlipTile(tile, false, true);
							byte[] tilehv = FlipTile(tileh, false, true);
							for (int i = 0; i < tiles.Count; i++)
							{
								if (tiles[i].FastArrayEqual(tile))
								{
									match = true;
									map.Tile = (ushort)i;
									break;
								}
								if (tiles[i].FastArrayEqual(tileh))
								{
									match = true;
									map.Tile = (ushort)i;
									map.XFlip = true;
									break;
								}
								if (tiles[i].FastArrayEqual(tilev))
								{
									match = true;
									map.Tile = (ushort)i;
									map.YFlip = true;
									break;
								}
								if (tiles[i].FastArrayEqual(tilehv))
								{
									match = true;
									map.Tile = (ushort)i;
									map.XFlip = true;
									map.YFlip = true;
									break;
								}
							}
						}
						if (!match)
						{
							tiles.Add(tile);
							result.Art.Add(tile);
							map.Tile = (ushort)(tiles.Count - 1);
						}
						result.Mappings[x, y] = map;
						updateProgress?.Invoke();
					}
			}
			else
			{
				for (int y = 0; y < h; y++)
					for (int x = 0; x < w; x++)
					{
						PatternIndex map = new PatternIndex() { Priority = priority[x, y * 2] };
						byte[] tile = BmpToTileInterlaced(new BitmapInfo(bmpi, x * 8, y * 16, 8, 16), forcepal, out int pal);
						map.Palette = (byte)pal;
						bool match = false;
						if (optimize)
						{
							byte[] tileh = FlipTileInterlaced(tile, true, false);
							byte[] tilev = FlipTileInterlaced(tile, false, true);
							byte[] tilehv = FlipTileInterlaced(tileh, false, true);
							for (int i = 0; i < tiles.Count; i++)
							{
								if (tiles[i].FastArrayEqual(tile))
								{
									match = true;
									map.Tile = (ushort)i;
									break;
								}
								if (tiles[i].FastArrayEqual(tileh))
								{
									match = true;
									map.Tile = (ushort)i;
									map.XFlip = true;
									break;
								}
								if (tiles[i].FastArrayEqual(tilev))
								{
									match = true;
									map.Tile = (ushort)i;
									map.YFlip = true;
									break;
								}
								if (tiles[i].FastArrayEqual(tilehv))
								{
									match = true;
									map.Tile = (ushort)i;
									map.XFlip = true;
									map.YFlip = true;
									break;
								}
							}
						}

						if (!match)
						{
							tiles.Add(tile);
							byte[] t1 = new byte[32];
							Array.Copy(tile, 0, t1, 0, 32);
							result.Art.Add(t1);
							byte[] t2 = new byte[32];
							Array.Copy(tile, 32, t2, 0, 32);
							result.Art.Add(t2);
							map.Tile = (ushort)((tiles.Count - 1) * 2);
						}
						result.Mappings[x, y * 2] = map;
						result.Mappings[x, y * 2 + 1] = map.Clone();
						result.Mappings[x, y * 2 + 1].Tile ^= 1;
						updateProgress?.Invoke();
					}
			}
			return result;
		}

		public static byte[] BmpToTile(BitmapInfo bmp, byte? forcepal, out int palette)
		{
			BitmapBits bmpbits = new BitmapBits(8, 8);
			switch (bmp.PixelFormat)
			{
				case PixelFormat.Format1bppIndexed:
					LoadBitmap1BppIndexed(bmpbits, bmp.Pixels, bmp.Stride);
					palette = forcepal ?? 0;
					return bmpbits.ToTile();
				case PixelFormat.Format32bppArgb:
					Color[,] pixels = new Color[8, 8];
					for (int y = 0; y < bmp.Height; y++)
					{
						int srcaddr = y * Math.Abs(bmp.Stride);
						for (int x = 0; x < bmp.Width; x++)
							pixels[x, y] = Color.FromArgb(BitConverter.ToInt32(bmp.Pixels, srcaddr + (x * 4)));
					}
					palette = forcepal ?? 0;
					Color[] newpal = new Color[16];
					if (!forcepal.HasValue)
					{
						long mindist = long.MaxValue;
						for (int i = 0; i < 4; i++)
						{
							for (int j = 0; j < 16; j++)
								newpal[j] = PaletteToColor(i, j, false);
							long totdist = 0;
							for (int y = 0; y < 8; y++)
								for (int x = 0; x < 8; x++)
									if (pixels[x, y].A >= 128)
									{
										pixels[x, y].FindNearestMatch(out int dist, newpal);
										totdist += dist;
									}
							if (totdist < mindist)
							{
								palette = i;
								mindist = totdist;
							}
						}
					}
					for (int j = 0; j < 16; j++)
						newpal[j] = PaletteToColor(palette, j, false);
					for (int y = 0; y < 8; y++)
						for (int x = 0; x < 8; x++)
							if (pixels[x, y].A >= 128)
								bmpbits[x, y] = (byte)Array.IndexOf(newpal, pixels[x, y].FindNearestMatch(newpal));
					break;
				case PixelFormat.Format4bppIndexed:
					LoadBitmap4BppIndexed(bmpbits, bmp.Pixels, bmp.Stride);
					palette = forcepal ?? 0;
					break;
				case PixelFormat.Format8bppIndexed:
					LoadBitmap8BppIndexed(bmpbits, bmp.Pixels, bmp.Stride);
					int[] palcnt = new int[4];
					for (int y = 0; y < 8; y++)
						for (int x = 0; x < 8; x++)
						{
							if ((bmpbits[x, y] & 15) > 0)
								palcnt[bmpbits[x, y] / 16]++;
							bmpbits[x, y] &= 15;
						}
					palette = forcepal ?? 0;
					if (!forcepal.HasValue)
					{
						if (palcnt[1] > palcnt[palette])
							palette = 1;
						if (palcnt[2] > palcnt[palette])
							palette = 2;
						if (palcnt[3] > palcnt[palette])
							palette = 3;
					}
					break;
				default:
					throw new Exception("wat");
			}
			return bmpbits.ToTile();
		}

		public static byte[] BmpToTileInterlaced(BitmapInfo bmp, byte? forcepal, out int palette)
		{
			BitmapBits bmpbits = new BitmapBits(8, 16);
			switch (bmp.PixelFormat)
			{
				case PixelFormat.Format1bppIndexed:
					LoadBitmap1BppIndexed(bmpbits, bmp.Pixels, bmp.Stride);
					palette = forcepal ?? 0;
					return bmpbits.ToTile();
				case PixelFormat.Format32bppArgb:
					Color[,] pixels = new Color[8, 16];
					for (int y = 0; y < bmp.Height; y++)
					{
						int srcaddr = y * Math.Abs(bmp.Stride);
						for (int x = 0; x < bmp.Width; x++)
							pixels[x, y] = Color.FromArgb(BitConverter.ToInt32(bmp.Pixels, srcaddr + (x * 4)));
					}
					palette = forcepal ?? 0;
					Color[] newpal = new Color[16];
					if (!forcepal.HasValue)
					{
						int mindist = int.MaxValue;
						for (int i = 0; i < 4; i++)
						{
							for (int j = 0; j < 16; j++)
								newpal[j] = PaletteToColor(i, j, false);
							int totdist = 0;
							for (int y = 0; y < 16; y++)
								for (int x = 0; x < 8; x++)
									if (pixels[x, y].A >= 128)
									{
										pixels[x, y].FindNearestMatch(out int dist, newpal);
										totdist += dist;
									}
							if (totdist < mindist)
							{
								palette = i;
								mindist = totdist;
							}
						}
					}
					for (int j = 0; j < 16; j++)
						newpal[j] = PaletteToColor(palette, j, false);
					for (int y = 0; y < 16; y++)
						for (int x = 0; x < 8; x++)
							if (pixels[x, y].A >= 128)
								bmpbits[x, y] = (byte)Array.IndexOf(newpal, pixels[x, y].FindNearestMatch(newpal));
					break;
				case PixelFormat.Format4bppIndexed:
					LoadBitmap4BppIndexed(bmpbits, bmp.Pixels, bmp.Stride);
					palette = forcepal ?? 0;
					break;
				case PixelFormat.Format8bppIndexed:
					LoadBitmap8BppIndexed(bmpbits, bmp.Pixels, bmp.Stride);
					int[] palcnt = new int[4];
					for (int y = 0; y < 16; y++)
						for (int x = 0; x < 8; x++)
						{
							if ((bmpbits[x, y] & 15) > 0)
								palcnt[bmpbits[x, y] / 16]++;
							bmpbits[x, y] &= 15;
						}
					palette = forcepal ?? 0;
					if (!forcepal.HasValue)
					{
						if (palcnt[1] > palcnt[palette])
							palette = 1;
						if (palcnt[2] > palcnt[palette])
							palette = 2;
						if (palcnt[3] > palcnt[palette])
							palette = 3;
					}
					break;
				default:
					throw new Exception("wat");
			}
			return bmpbits.ToTileInterlaced();
		}

		public static ColInfo[,] GetColMap(Bitmap bmp)
		{
			if (bmp.PixelFormat != PixelFormat.Format32bppArgb)
				bmp = bmp.To32bpp();
			BitmapBits bmpbits = new BitmapBits(bmp.Width, bmp.Height);
			BitmapData bmpd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
			int stride = bmpd.Stride;
			byte[] Bits = new byte[Math.Abs(stride) * bmpd.Height];
			System.Runtime.InteropServices.Marshal.Copy(bmpd.Scan0, Bits, 0, Bits.Length);
			bmp.UnlockBits(bmpd);
			LoadBitmap32BppArgb(bmpbits, Bits, stride, new Color[] { Color.Magenta, Color.White, Color.Yellow, Color.Black });
			ColInfo[,] result = new ColInfo[bmpbits.Width / 16, bmpbits.Height / 16];
			Parallel.For(0, bmpbits.Height / 16, by =>
			{
				for (int bx = 0; bx < bmpbits.Width / 16; bx++)
				{
					ushort[] coltypes = new ushort[3];
					sbyte[] heightmap = new sbyte[16];
					Point? start = null;
					Point? end = null;
					bool inverted = false;
					for (int x = 0; x < 16; x++)
					{
						if (bmpbits[bx * 16 + x, by * 16 + 15] != 0) // solidity starts at bottom
						{
							for (int y = 15; y >= 0; y--)
								if (bmpbits[bx * 16 + x, by * 16 + y] != 0)
									coltypes[bmpbits[bx * 16 + x, by * 16 + y] - 1]++;
								else
								{
									if (!start.HasValue)
										start = new Point(x, y + 1);
									end = new Point(x, y + 1);
									heightmap[x] = (sbyte)(15 - y);
									break;
								}
							if (heightmap[x] == 0)
							{
								if (!start.HasValue || start.Value.Y == 0)
									start = new Point(x, 0);
								if (!end.HasValue || end.Value.Y != 0)
									end = new Point(x, 0);
								heightmap[x] = 16;
							}
						}
						else if (bmpbits[bx * 16 + x, by * 16] != 0) // solidity starts at top
						{
							inverted = true;
							for (int y = 0; y < 16; y++)
								if (bmpbits[bx * 16 + x, by * 16 + y] != 0)
									coltypes[bmpbits[bx * 16 + x, by * 16 + y] - 1]++;
								else
								{
									if (!start.HasValue)
										start = new Point(x, y - 1);
									end = new Point(x, y - 1);
									heightmap[x] = (sbyte)-y;
									break;
								}
						}
					}
					if (inverted)
					{
						if (start.HasValue && start.Value.Y == 0)
							start = new Point(start.Value.X, 15);
						if (end.HasValue && end.Value.Y == 0)
							end = new Point(end.Value.X, 15);
						for (int x = 0; x < 16; x++)
							if (heightmap[x] == 16)
								heightmap[x] = -16;
					}
					Solidity solid = Solidity.NotSolid;
					byte angle = 0;
					if (start.HasValue)
					{
						solid = Solidity.TopSolid;
						ushort max = coltypes[0];
						if (coltypes[1] > max)
						{
							solid = Solidity.LRBSolid;
							max = coltypes[1];
						}
						if (coltypes[2] > max)
							solid = Solidity.AllSolid;
						if (start.Value.Y == end.Value.Y)
							angle = 0xFF;
						else
							angle = (byte)((byte)(Math.Atan2(end.Value.Y - start.Value.Y, (end.Value.X - start.Value.X) * (inverted ? -1 : 1)) * (256 / (2 * Math.PI))) & 0xFC);
					}
					result[bx, by] = new ColInfo(solid, heightmap, angle);
				}
			});
			return result;
		}

		public static void GetPriMap(Bitmap bmp, bool[,] primap)
		{
			if (bmp.PixelFormat != PixelFormat.Format32bppArgb)
				bmp = bmp.To32bpp();
			BitmapBits bmpbits = new BitmapBits(bmp.Width, bmp.Height);
			BitmapData bmpd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
			int stride = bmpd.Stride;
			byte[] Bits = new byte[Math.Abs(stride) * bmpd.Height];
			System.Runtime.InteropServices.Marshal.Copy(bmpd.Scan0, Bits, 0, Bits.Length);
			bmp.UnlockBits(bmpd);
			LoadBitmap32BppArgb(bmpbits, Bits, stride, new Color[] { Color.Black, Color.White });
			int w = Math.Min(primap.GetLength(0), bmpbits.Width / 8);
			int h = Math.Min(primap.GetLength(1), bmpbits.Height / 8);
			Parallel.For(0, h, ty =>
			{
				for (int tx = 0; tx < w; tx++)
				{
					ushort[] cnt = new ushort[2];
					for (int y = 0; y < 8; y++)
						for (int x = 0; x < 8; x++)
							cnt[bmpbits[(tx * 8) + x, (ty * 8) + y]]++;
					primap[tx, ty] = cnt[0] < cnt[1];
				}
			});
		}

		private static void LoadBitmap1BppIndexed(BitmapBits bmp, byte[] Bits, int Stride)
		{
			for (int y = 0; y < bmp.Height; y++)
			{
				int srcaddr = y * Math.Abs(Stride);
				for (int x = 0; x < bmp.Width; x += 8)
				{
					bmp[x + 0, y] = (byte)((Bits[srcaddr + (x / 8)] >> 7) & 1);
					bmp[x + 1, y] = (byte)((Bits[srcaddr + (x / 8)] >> 6) & 1);
					bmp[x + 2, y] = (byte)((Bits[srcaddr + (x / 8)] >> 5) & 1);
					bmp[x + 3, y] = (byte)((Bits[srcaddr + (x / 8)] >> 4) & 1);
					bmp[x + 4, y] = (byte)((Bits[srcaddr + (x / 8)] >> 3) & 1);
					bmp[x + 5, y] = (byte)((Bits[srcaddr + (x / 8)] >> 2) & 1);
					bmp[x + 6, y] = (byte)((Bits[srcaddr + (x / 8)] >> 1) & 1);
					bmp[x + 7, y] = (byte)(Bits[srcaddr + (x / 8)] & 1);
				}
			}
		}

		private static void LoadBitmap32BppArgb(BitmapBits bmp, byte[] Bits, int Stride, Color[] palette)
		{
			for (int y = 0; y < bmp.Height; y++)
			{
				int srcaddr = y * Math.Abs(Stride);
				for (int x = 0; x < bmp.Width; x++)
				{
					Color col = Color.FromArgb(BitConverter.ToInt32(Bits, srcaddr + (x * 4)));
					bmp[x, y] = (byte)Array.IndexOf(palette, col.FindNearestMatch(palette));
					if (col.A < 128)
						bmp[x, y] = 0;
				}
			}
		}

		private static void LoadBitmap4BppIndexed(BitmapBits bmp, byte[] Bits, int Stride)
		{
			for (int y = 0; y < bmp.Height; y++)
			{
				int srcaddr = y * Math.Abs(Stride);
				for (int x = 0; x < bmp.Width; x += 2)
				{
					bmp[x, y] = (byte)(Bits[srcaddr + (x / 2)] >> 4);
					bmp[x + 1, y] = (byte)(Bits[srcaddr + (x / 2)] & 0xF);
				}
			}
		}

		private static void LoadBitmap8BppIndexed(BitmapBits bmp, byte[] Bits, int Stride)
		{
			for (int y = 0; y < bmp.Height; y++)
			{
				int srcaddr = y * Math.Abs(Stride);
				for (int x = 0; x < bmp.Width; x++)
					bmp[x, y] = (byte)(Bits[srcaddr + x] & 0x3F);
			}
		}

		public static void UpdateTileArray()
		{
			List<byte> tils = new List<byte>();
			foreach (byte[] item in Tiles)
				tils.AddRange(item);
			TileArray = tils.ToArray();
		}

		public static void Log(params string[] message) { LogEvent(message); }

		public static Bitmap BitmapBitsToBitmap(BitmapBits bmp)
		{
			return bmp.ToBitmap(BmpPal);
		}

		public static void Pad<T>(ref T[] array, int multiple)
		{
			int off = array.Length % multiple;
			if (off == 0) return;
			Array.Resize(ref array, array.Length + (multiple - off));
		}

		public static Size FGSize { get { return new Size(FGWidth, FGHeight); } }

		public static int FGWidth { get { return Layout.FGLayout.GetLength(0); } }
		public static int FGHeight { get { return Layout.FGLayout.GetLength(1); } }

		public static Size BGSize { get { return new Size(BGWidth, BGHeight); } }

		public static int BGWidth { get { return Layout.BGLayout.GetLength(0); } }
		public static int BGHeight { get { return Layout.BGLayout.GetLength(1); } }

		public static void ResizeFG(Size newSize) { ResizeFG(newSize.Width, newSize.Height); }

		public static void ResizeFG(int width, int height)
		{
			ushort[,] newFG = new ushort[width, height];
			bool[,] newFGLoop = Layout.FGLoop != null ? new bool[width, height] : null;
			bool[,] newFGXFlip = new bool[width, height];
			bool[,] newFGYFlip = new bool[width, height];
			Size oldsize = FGSize;
			for (int y = 0; y < Math.Min(height, oldsize.Height); y++)
				for (int x = 0; x < Math.Min(width, oldsize.Width); x++)
				{
					newFG[x, y] = Layout.FGLayout[x, y];
					if (newFGLoop != null)
						newFGLoop[x, y] = Layout.FGLoop[x, y];
					if (Layout.FGXFlip != null)
						newFGXFlip[x, y] = Layout.FGXFlip[x, y];
					if (Layout.FGYFlip != null)
						newFGYFlip[x, y] = Layout.FGYFlip[x, y];
				}
			Layout.FGLayout = newFG;
			Layout.FGLoop = newFGLoop;
			Layout.FGXFlip = newFGXFlip;
			Layout.FGYFlip = newFGYFlip;
		}

		public static void ResizeBG(Size newSize) { ResizeBG(newSize.Width, newSize.Height); }

		public static void ResizeBG(int width, int height)
		{
			ushort[,] newBG = new ushort[width, height];
			bool[,] newBGLoop = Layout.BGLoop != null ? new bool[width, height] : null;
			bool[,] newBGXFlip = new bool[width, height];
			bool[,] newBGYFlip = new bool[width, height];
			Size oldsize = BGSize;
			for (int y = 0; y < Math.Min(height, oldsize.Height); y++)
				for (int x = 0; x < Math.Min(width, oldsize.Width); x++)
				{
					newBG[x, y] = Layout.BGLayout[x, y];
					if (newBGLoop != null)
						newBGLoop[x, y] = Layout.BGLoop[x, y];
					if (Layout.BGXFlip != null)
						newBGXFlip[x, y] = Layout.BGXFlip[x, y];
					if (Layout.BGYFlip != null)
						newBGYFlip[x, y] = Layout.BGYFlip[x, y];
				}
			Layout.BGLayout = newBG;
			Layout.BGLoop = newBGLoop;
			Layout.BGXFlip = newBGXFlip;
			Layout.BGYFlip = newBGYFlip;
		}

		public static int GetBlockMax()
		{
			int blockmax = 0x300;
			if (Game.BlockMax.HasValue)
				blockmax = Game.BlockMax.Value;
			return blockmax;
		}

		public static int GetChunkMax()
		{
			int chunkmax = 0x100;
			if (Game.ChunkMax.HasValue)
				chunkmax = Game.ChunkMax.Value;
			return chunkmax;
		}

		public static byte GetColInd1(int index)
		{
			if (index >= ColInds1.Count)
				return 0;
			return ColInds1[index];
		}

		public static byte GetColInd2(int index)
		{
			if (index >= ColInds2.Count)
				return 0;
			return ColInds2[index];
		}

		public static byte[] FlipTile(byte[] tile, bool xflip, bool yflip)
		{
			int mode = (xflip ? 1 : 0) | (yflip ? 2 : 0);
			switch (mode)
			{
				default:
					return tile;
				case 1:
					byte[] tileh = new byte[32];
					for (int ty = 0; ty < 8; ty++)
						for (int tx = 0; tx < 4; tx++)
						{
							byte px = tile[(ty * 4) + tx];
							tileh[(ty * 4) + (3 - tx)] = (byte)((px >> 4) | (px << 4));
						}
					return tileh;
				case 2:
					byte[] tilev = new byte[32];
					for (int ty = 0; ty < 8; ty++)
						Array.Copy(tile, ty * 4, tilev, (7 - ty) * 4, 4);
					return tilev;
				case 3:
					byte[] tilehv = new byte[32];
					for (int ty = 0; ty < 8; ty++)
						for (int tx = 0; tx < 4; tx++)
						{
							byte px = tile[(ty * 4) + tx];
							tilehv[((7 - ty) * 4) + (3 - tx)] = (byte)((px >> 4) | (px << 4));
						}
					return tilehv;
			}
		}

		public static byte[] FlipTileInterlaced(byte[] tile, bool xflip, bool yflip)
		{
			int mode = (xflip ? 1 : 0) | (yflip ? 2 : 0);
			switch (mode)
			{
				default:
					return tile;
				case 1:
					byte[] tileh = new byte[64];
					for (int ty = 0; ty < 16; ty++)
						for (int tx = 0; tx < 4; tx++)
						{
							byte px = tile[(ty * 4) + tx];
							tileh[(ty * 4) + (3 - tx)] = (byte)((px >> 4) | (px << 4));
						}
					return tileh;
				case 2:
					byte[] tilev = new byte[64];
					for (int ty = 0; ty < 16; ty++)
						Array.Copy(tile, ty * 4, tilev, (15 - ty) * 4, 4);
					return tilev;
				case 3:
					byte[] tilehv = new byte[64];
					for (int ty = 0; ty < 16; ty++)
						for (int tx = 0; tx < 4; tx++)
						{
							byte px = tile[(ty * 4) + tx];
							tilehv[((15 - ty) * 4) + (3 - tx)] = (byte)((px >> 4) | (px << 4));
						}
					return tilehv;
			}
		}

		public static void RemapLayouts(Action<ushort[,], int, int> func)
		{
			for (int y = 0; y < FGHeight; y++)
				for (int x = 0; x < FGWidth; x++)
					func(Layout.FGLayout, x, y);
			for (int y = 0; y < BGHeight; y++)
				for (int x = 0; x < BGWidth; x++)
					func(Layout.BGLayout, x, y);
			foreach (var item in AdditionalLayouts.Values)
			{
				if (item.Value.FGLayout != null)
					for (int y = 0; y < item.Value.FGLayout.GetLength(1); y++)
						for (int x = 0; x < item.Value.FGLayout.GetLength(0); x++)
							func(item.Value.FGLayout, x, y);
				if (item.Value.BGLayout != null)
					for (int y = 0; y < item.Value.BGLayout.GetLength(1); y++)
						for (int x = 0; x < item.Value.BGLayout.GetLength(0); x++)
							func(item.Value.BGLayout, x, y);
			}
		}
	}

	public class LayoutData
	{
		public ushort[,] FGLayout;
		public bool[,] FGLoop;
		public bool[,] FGXFlip;
		public bool[,] FGYFlip;
		public ushort[,] BGLayout;
		public bool[,] BGLoop;
		public bool[,] BGXFlip;
		public bool[,] BGYFlip;
	}

	public enum EngineVersion
	{
		Invalid,
		S1,
		S2NA,
		S2,
		S3K,
		SCD,
		SCDPC,
		SKC,
		Chaotix,
		Custom
	}

	public enum TimeZone
	{
		None,
		Present,
		Past,
		Future
	}

	public enum MappingsFormat
	{
		Invalid,
		Binary,
		ASM,
		Macro
	}

	[Serializable]
	public class ColInfo
	{
		public Solidity Solidity { get; private set; }
		public sbyte[] HeightMap { get; private set; }
		public byte Angle { get; private set; }

		public ColInfo(Solidity solidity, sbyte[] heightMap, byte angle)
		{
			Solidity = solidity;
			HeightMap = heightMap;
			Angle = angle;
		}
	}

	public class BitmapInfo
	{
		public int Width { get; private set; }
		public int Height { get; private set; }
		public Size Size { get { return new Size(Width, Height); } }
		public PixelFormat PixelFormat { get; private set; }
		public int Stride { get; private set; }
		public byte[] Pixels { get; private set; }

		public BitmapInfo(Bitmap bitmap)
		{
			Width = bitmap.Width;
			Height = bitmap.Height;
			switch (bitmap.PixelFormat)
			{
				case PixelFormat.Format1bppIndexed:
				case PixelFormat.Format32bppArgb:
				case PixelFormat.Format4bppIndexed:
				case PixelFormat.Format8bppIndexed:
					PixelFormat = bitmap.PixelFormat;
					break;
				default:
					bitmap = bitmap.To32bpp();
					PixelFormat = PixelFormat.Format32bppArgb;
					break;
			}
			BitmapData bmpd = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly, PixelFormat);
			Stride = Math.Abs(bmpd.Stride);
			Pixels = new byte[Stride * Height];
			System.Runtime.InteropServices.Marshal.Copy(bmpd.Scan0, Pixels, 0, Pixels.Length);
			bitmap.UnlockBits(bmpd);
		}

		public BitmapInfo(BitmapInfo source, int x, int y, int width, int height)
		{
			switch (source.PixelFormat)
			{
				case PixelFormat.Format1bppIndexed:
					if (x % 8 != 0)
						throw new FormatException("X coordinate of 1bpp image section must be multiple of 8.");
					if (width % 8 != 0)
						throw new FormatException("Width of 1bpp image section must be multiple of 8.");
					break;
				case PixelFormat.Format4bppIndexed:
					if (x % 2 != 0)
						throw new FormatException("X coordinate of 4bpp image section must be multiple of 2.");
					if (width % 2 != 0)
						throw new FormatException("Width of 4bpp image section must be multiple of 2.");
					break;
			}
			Width = width;
			Height = height;
			PixelFormat = source.PixelFormat;
			switch (PixelFormat)
			{
				case PixelFormat.Format1bppIndexed:
					Stride = width / 8;
					x /= 8;
					break;
				case PixelFormat.Format4bppIndexed:
					Stride = width / 2;
					x /= 2;
					break;
				case PixelFormat.Format8bppIndexed:
					Stride = width;
					break;
				case PixelFormat.Format32bppArgb:
					Stride = width * 4;
					x *= 4;
					break;
			}
			Pixels = new byte[height * Stride];
			for (int v = 0; v < height; v++)
				Array.Copy(source.Pixels, ((y + v) * source.Stride) + x, Pixels, v * Stride, Stride);
		}

		public Bitmap ToBitmap()
		{
			Bitmap bitmap = new Bitmap(Width, Height, PixelFormat);
			BitmapData bmpd = bitmap.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat);
			byte[] bmpbits = new byte[Height * Math.Abs(bmpd.Stride)];
			for (int y = 0; y < Height; y++)
				Array.Copy(Pixels, y * Stride, bmpbits, y * Math.Abs(bmpd.Stride), Width);
			System.Runtime.InteropServices.Marshal.Copy(bmpbits, 0, bmpd.Scan0, bmpbits.Length);
			bitmap.UnlockBits(bmpd);
			return bitmap;
		}
	}

	public class ImportResult
	{
		public PatternIndex[,] Mappings { get; private set; }
		public List<byte[]> Art { get; private set; }

		public ImportResult(int width, int height)
		{
			Mappings = new PatternIndex[width, height];
			Art = new List<byte[]>();
		}
	}
}
