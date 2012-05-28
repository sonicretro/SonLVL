using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
        public static List<BitmapBits[]> ChunkBmpBits;
        public static List<BitmapBits> CompChunkBmpBits;
        public static List<Bitmap[]> ChunkBmps;
        public static List<Bitmap> CompChunkBmps;
        public static byte[,] FGLayout;
        public static bool[,] FGLoop;
        public static byte[,] BGLayout;
        public static bool[,] BGLoop;
        public static List<string> PalName;
        public static List<ushort[,]> Palette;
        public static List<byte[,]> PalNum;
        public static List<int[,]> PalAddr;
        public static int CurPal;
        public static ColorPalette BmpPal;
        public static List<ObjectEntry> Objects;
        public static List<RingEntry> Rings;
        public static List<CNZBumperEntry> Bumpers;
        public static List<StartPositionEntry> StartPositions;
        public static Dictionary<string, ObjectData> INIObjDefs;
        public static Dictionary<byte, ObjectDefinition> ObjTypes;
        public static ObjectDefinition unkobj;
        public static S2RingDefinition S2RingDef;
        public static S3KRingDefinition S3KRingDef;
        public static List<StartPositionDefinition> StartPosDefs;
        public static int chunksz;
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
        public static Bitmap UnknownImg;
        public static List<Sprite> Sprites;
        public delegate void LogEventHandler(params string[] message);
        public static event LogEventHandler LogEvent = delegate { };
        public delegate void ObjectTypeChangedEventHandler(ObjectEntry old, ObjectEntry @new);
        public static event ObjectTypeChangedEventHandler ObjectTypeChangedEvent = delegate { };
        public static event Action PaletteChangedEvent = delegate { };
        internal static readonly bool IsMonoRuntime = Type.GetType("Mono.Runtime") != null;
        internal static readonly bool IsWindows = !(Environment.OSVersion.Platform == PlatformID.MacOSX | Environment.OSVersion.Platform == PlatformID.Unix | Environment.OSVersion.Platform == PlatformID.Xbox);

        public static void LoadGame(string filename)
        {
            Log("Opening INI file \"" + filename + "\"...");
            Game = GameInfo.Load(filename);
            Environment.CurrentDirectory = Path.GetDirectoryName(filename);
            switch (Game.EngineVersion)
            {
                case EngineVersion.S1:
                    chunksz = 256;
                    UnknownImg = Properties.Resources.UnknownImg.Copy();
                    break;
                case EngineVersion.SCDPC:
                    chunksz = 256;
                    UnknownImg = Properties.Resources.UnknownImg.Copy();
                    littleendian = true;
                    break;
                case EngineVersion.S2:
                case EngineVersion.S2NA:
                case EngineVersion.SBoom:
                    chunksz = 128;
                    UnknownImg = Properties.Resources.UnknownImg.Copy();
                    break;
                case EngineVersion.S3K:
                    chunksz = 128;
                    UnknownImg = Properties.Resources.UnknownImg3K.Copy();
                    break;
                case EngineVersion.SKC:
                    chunksz = 128;
                    UnknownImg = Properties.Resources.UnknownImg3K.Copy();
                    littleendian = true;
                    break;
                default:
                    throw new NotImplementedException("Game type " + Game.EngineVersion.ToString() + " is not supported!");
            }
            Log("Game type is " + Game.EngineVersion.ToString() + ".");
        }

        public static void LoadLevel(string levelname, bool loadGraphics)
        {
            Level = Game.GetLevelInfo(levelname);
            Log("Loading " + Level.DisplayName + "...");
            switch (Level.ChunkFormat)
            {
                case EngineVersion.S1:
                case EngineVersion.SCDPC:
                    chunksz = 256;
                    break;
                case EngineVersion.S2:
                case EngineVersion.S2NA:
                case EngineVersion.S3K:
                case EngineVersion.SKC:
                case EngineVersion.SBoom:
                    chunksz = 128;
                    break;
            }
            byte[] tmp = null;
            List<byte> data = new List<byte>();
            Tiles = new MultiFileIndexer<byte[]>();
            if (Level.TileFormat != EngineVersion.SCDPC)
            {
                foreach (FileInfo tileent in Level.Tiles)
                {
                    tmp = null;
                    if (File.Exists(tileent.Filename))
                    {
                        Log("Loading 8x8 tiles from file \"" + tileent.Filename + "\", using compression " + Level.TileCompression.ToString() + "...");
                        tmp = Compression.Decompress(tileent.Filename, Level.TileCompression);
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
                Level.TileCompression = Compression.CompressionType.SZDD;
                if (File.Exists(Level.Tiles[0].Filename))
                {
                    Log("Loading 8x8 tiles from file \"" + Level.Tiles[0].Filename + "\", using compression SZDD...");
                    tmp = Compression.Decompress(Level.Tiles[0].Filename, Compression.CompressionType.SZDD);
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
            UpdateTileArray();
            Blocks = new MultiFileIndexer<Block>();
            foreach (FileInfo tileent in Level.Blocks)
            {
                if (File.Exists(tileent.Filename))
                {
                    Log("Loading 16x16 blocks from file \"" + tileent.Filename + "\", using compression " + Level.BlockCompression.ToString() + "...");
                    tmp = Compression.Decompress(tileent.Filename, Level.BlockCompression);
                    List<Block> tmpblk = new List<Block>();
                    if (Game.EngineVersion == EngineVersion.SKC)
                        littleendian = false;
                    for (int ba = 0; ba < tmp.Length; ba += Block.Size)
                        tmpblk.Add(new Block(tmp, ba));
                    if (Game.EngineVersion == EngineVersion.SKC)
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
            Chunks = new MultiFileIndexer<Chunk>();
            data = new List<byte>();
            int fileind = 0;
            foreach (FileInfo tileent in Level.Chunks)
            {
                if (File.Exists(tileent.Filename))
                {
                    Log("Loading " + chunksz + "x" + chunksz + " chunks from file \"" + tileent.Filename + "\", using compression " + Level.ChunkCompression.ToString() + "...");
                    tmp = Compression.Decompress(tileent.Filename, Level.ChunkCompression);
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
                    if (Game.EngineVersion == EngineVersion.SKC)
                        littleendian = false;
                    for (int ba = 0; ba < tmp.Length; ba += Chunk.Size)
                        tmpchnk.Add(new Chunk(tmp, ba));
                    if (Game.EngineVersion == EngineVersion.SKC)
                        littleendian = true;
                    Chunks.AddFile(tmpchnk, tileent.Offset == -1 ? tileent.Offset : tileent.Offset / Chunk.Size);
                    fileind++;
                }
                else
                {
                    Log(chunksz + "x" + chunksz + " chunk file \"" + tileent.Filename + "\" not found.");
                    Chunks.AddFile(new List<Chunk>() { new Chunk() }, tileent.Offset == -1 ? tileent.Offset : tileent.Offset / Chunk.Size);
                }
            }
            if (Chunks.Count == 0)
                Chunks.AddFile(new List<Chunk>() { new Chunk() }, -1);
            int fgw, fgh, bgw, bgh;
            FGLoop = null;
            BGLoop = null;
            switch (Level.LayoutFormat)
            {
                case EngineVersion.S1:
                    if (File.Exists(Level.FGLayout))
                    {
                        Log("Loading FG layout from file \"" + Level.FGLayout + "\", using compression " + Level.LayoutCompression.ToString() + "...");
                        tmp = Compression.Decompress(Level.FGLayout, Level.LayoutCompression);
                        fgw = tmp[0] + 1;
                        fgh = tmp[1] + 1;
                        FGLayout = new byte[fgw, fgh];
                        FGLoop = new bool[fgw, fgh];
                        for (int lr = 0; lr < fgh; lr++)
                            for (int lc = 0; lc < fgw; lc++)
                            {
                                if ((lr * fgw) + lc + 2 >= tmp.Length) break;
                                FGLayout[lc, lr] = (byte)(tmp[(lr * fgw) + lc + 2] & 0x7F);
                                FGLoop[lc, lr] = (tmp[(lr * fgw) + lc + 2] & 0x80) == 0x80;
                            }
                    }
                    else
                    {
                        Log("FG layout file \"" + Level.FGLayout + "\" not found.");
                        FGLayout = new byte[Game.LevelWidthMax, Game.LevelHeightMax];
                        FGLoop = new bool[Game.LevelWidthMax, Game.LevelHeightMax];
                    }
                    if (File.Exists(Level.BGLayout))
                    {
                        Log("Loading BG layout from file \"" + Level.BGLayout + "\", using compression " + Level.LayoutCompression.ToString() + "...");
                        tmp = Compression.Decompress(Level.BGLayout, Level.LayoutCompression);
                        bgw = tmp[0] + 1;
                        bgh = tmp[1] + 1;
                        BGLayout = new byte[bgw, bgh];
                        BGLoop = new bool[bgw, bgh];
                        for (int lr = 0; lr < bgh; lr++)
                            for (int lc = 0; lc < bgw; lc++)
                            {
                                BGLayout[lc, lr] = (byte)(tmp[(lr * bgw) + lc + 2] & 0x7F);
                                BGLoop[lc, lr] = (tmp[(lr * bgw) + lc + 2] & 0x80) == 0x80;
                            }
                    }
                    else
                    {
                        Log("BG layout file \"" + Level.BGLayout + "\" not found.");
                        BGLayout = new byte[Game.LevelWidthMax, Game.LevelHeightMax];
                        BGLoop = new bool[Game.LevelWidthMax, Game.LevelHeightMax];
                    }
                    break;
                case EngineVersion.S2NA:
                    if (File.Exists(Level.FGLayout))
                    {
                        Log("Loading FG layout from file \"" + Level.FGLayout + "\", using compression " + Level.LayoutCompression.ToString() + "...");
                        tmp = Compression.Decompress(Level.FGLayout, Level.LayoutCompression);
                        fgw = tmp[0] + 1;
                        fgh = tmp[1] + 1;
                        FGLayout = new byte[fgw, fgh];
                        for (int lr = 0; lr < fgh; lr++)
                            for (int lc = 0; lc < fgw; lc++)
                            {
                                if ((lr * fgw) + lc + 2 >= tmp.Length) break;
                                FGLayout[lc, lr] = tmp[(lr * fgw) + lc + 2];
                            }
                    }
                    else
                    {
                        Log("FG layout file \"" + Level.FGLayout + "\" not found.");
                        FGLayout = new byte[Game.LevelWidthMax, Game.LevelHeightMax];
                    }
                    if (File.Exists(Level.BGLayout))
                    {
                        Log("Loading BG layout from file \"" + Level.BGLayout + "\", using compression " + Level.LayoutCompression.ToString() + "...");
                        tmp = Compression.Decompress(Level.BGLayout, Level.LayoutCompression);
                        bgw = tmp[0] + 1;
                        bgh = tmp[1] + 1;
                        BGLayout = new byte[bgw, bgh];
                        for (int lr = 0; lr < bgh; lr++)
                            for (int lc = 0; lc < bgw; lc++)
                                BGLayout[lc, lr] = tmp[(lr * bgw) + lc + 2];
                    }
                    else
                    {
                        Log("BG layout file \"" + Level.BGLayout + "\" not found.");
                        BGLayout = new byte[Game.LevelWidthMax, Game.LevelHeightMax];
                    }
                    break;
                case EngineVersion.S2:
                    FGLayout = new byte[128, 16];
                    BGLayout = new byte[128, 16];
                    if (File.Exists(Level.Layout))
                    {
                        Log("Loading layout from file \"" + Level.Layout + "\", using compression " + Level.LayoutCompression.ToString() + "...");
                        tmp = Compression.Decompress(Level.Layout, Level.LayoutCompression);
                        for (int la = 0; la < tmp.Length; la += 256)
                        {
                            for (int laf = 0; laf < 128; laf++)
                                FGLayout[laf, la / 256] = tmp[la + laf];
                            for (int lab = 0; lab < 128; lab++)
                                BGLayout[lab, la / 256] = tmp[la + lab + 128];
                        }
                    }
                    else
                        Log("Layout file \"" + Level.Layout + "\" not found.");
                    break;
                case EngineVersion.S3K:
                    if (File.Exists(Level.Layout))
                    {
                        Log("Loading layout from file \"" + Level.Layout + "\", using compression " + Level.LayoutCompression.ToString() + "...");
                        tmp = Compression.Decompress(Level.Layout, Level.LayoutCompression);
                        fgw = ByteConverter.ToUInt16(tmp, 0);
                        bgw = ByteConverter.ToUInt16(tmp, 2);
                        fgh = ByteConverter.ToUInt16(tmp, 4);
                        bgh = ByteConverter.ToUInt16(tmp, 6);
                        FGLayout = new byte[fgw, fgh];
                        BGLayout = new byte[bgw, bgh];
                        for (int la = 0; la < Math.Max(fgh, bgh) * 4; la += 4)
                        {
                            ushort lfp = ByteConverter.ToUInt16(tmp, 8 + la);
                            if (lfp != 0)
                                for (int laf = 0; laf < fgw; laf++)
                                    FGLayout[laf, la / 4] = tmp[lfp - 0x8000 + laf];
                            ushort lbp = ByteConverter.ToUInt16(tmp, 8 + la + 2);
                            if (lbp != 0)
                                for (int lab = 0; lab < bgw; lab++)
                                    BGLayout[lab, la / 4] = tmp[lbp - 0x8000 + lab];
                        }
                    }
                    else
                    {
                        Log("Layout file \"" + Level.Layout + "\" not found.");
                        FGLayout = new byte[128, 16];
                        BGLayout = new byte[128, 16];
                    }
                    break;
                case EngineVersion.SKC:
                    if (File.Exists(Level.Layout))
                    {
                        Log("Loading layout from file \"" + Level.Layout + "\", using compression " + Level.LayoutCompression.ToString() + "...");
                        tmp = Compression.Decompress(Level.Layout, Level.LayoutCompression);
                        fgw = ByteConverter.ToUInt16(tmp, 0);
                        bgw = ByteConverter.ToUInt16(tmp, 2);
                        fgh = ByteConverter.ToUInt16(tmp, 4);
                        bgh = ByteConverter.ToUInt16(tmp, 6);
                        FGLayout = new byte[fgw, fgh];
                        BGLayout = new byte[bgw, bgh];
                        for (int la = 0; la < Math.Max(fgh, bgh) * 4; la += 4)
                        {
                            ushort lfp = ByteConverter.ToUInt16(tmp, 8 + la);
                            if (lfp != 0)
                                for (int laf = 0; laf < fgw; laf++)
                                    FGLayout[laf, la / 4] = tmp[(lfp - 0x8000 + laf) ^ 1];
                            ushort lbp = ByteConverter.ToUInt16(tmp, 8 + la + 2);
                            if (lbp != 0)
                                for (int lab = 0; lab < bgw; lab++)
                                    BGLayout[lab, la / 4] = tmp[(lbp - 0x8000 + lab) ^ 1];
                        }
                    }
                    else
                    {
                        Log("Layout file \"" + Level.Layout + "\" not found.");
                        FGLayout = new byte[128, 16];
                        BGLayout = new byte[128, 16];
                    }
                    break;
                case EngineVersion.SCDPC:
                    FGLayout = new byte[64, 8];
                    if (File.Exists(Level.FGLayout))
                    {
                        Log("Loading FG layout from file \"" + Level.FGLayout + "\", using compression " + Level.LayoutCompression.ToString() + "...");
                        tmp = Compression.Decompress(Level.FGLayout, Level.LayoutCompression);
                        for (int lr = 0; lr < 8; lr++)
                            for (int lc = 0; lc < 64; lc++)
                            {
                                if ((lr * 64) + lc >= tmp.Length) break;
                                FGLayout[lc, lr] = tmp[(lr * 64) + lc];
                            }
                    }
                    else
                        Log("FG layout file \"" + Level.FGLayout + "\" not found.");
                    BGLayout = new byte[64, 8];
                    if (File.Exists(Level.BGLayout))
                    {
                        Log("Loading BG layout from file \"" + Level.BGLayout + "\", using compression " + Level.LayoutCompression.ToString() + "...");
                        tmp = Compression.Decompress(Level.BGLayout, Level.LayoutCompression);
                        for (int lr = 0; lr < 8; lr++)
                            for (int lc = 0; lc < 64; lc++)
                            {
                                BGLayout[lc, lr] = tmp[(lr * 64) + lc];
                            }
                    }
                    else
                        Log("BG layout file \"" + Level.BGLayout + "\" not found.");
                    break;
                case EngineVersion.SBoom:
                    SonicBoom.ReadLayout();
                    break;
            }
            PalName = new List<string>();
            Palette = new List<ushort[,]>();
            PalNum = new List<byte[,]>();
            PalAddr = new List<int[,]>();
            byte palfilenum = 0;
            int palnum = 0;
            while (palnum < 9 && Level.Palettes[palnum] != null)
            {
                PalName.Add(Level.Palettes[palnum].Name);
                Palette.Add(new ushort[4, 16]);
                PalNum.Add(new byte[4, 16]);
                PalAddr.Add(new int[4, 16]);
                for (byte pn = 0; pn < Level.Palettes[palnum].Palettes.Collection.Length; pn++)
                {
                    PaletteInfo palent = Level.Palettes[palnum].Palettes[pn];
                    Log("Loading palette file \"" + palent.Filename + "\"...", "Source: " + palent.Source + " Destination: " + palent.Destination + " Length: " + palent.Length);
                    if (!File.Exists(palent.Filename)) throw new FileNotFoundException("Palette file could not be loaded! Have you set up your disassembly properly?", palent.Filename);
                    tmp = File.ReadAllBytes(palent.Filename);
                    ushort[] palfile;
                    if (Level.PaletteFormat != EngineVersion.SCDPC)
                    {
                        palfile = new ushort[tmp.Length / 2];
                        for (int pi = 0; pi < tmp.Length; pi += 2)
                            palfile[pi / 2] = ByteConverter.ToUInt16(tmp, pi);
                    }
                    else
                    {
                        palfile = new ushort[tmp.Length / 4];
                        for (int pi = 0; pi < tmp.Length; pi += 4)
                            palfile[pi / 4] = (ushort)((tmp[pi] >> 4) | (tmp[pi + 1] & 0xF0) | ((tmp[pi + 2] >> 4) << 8));
                    }
                    for (int pa = 0; pa < palent.Length; pa++)
                    {
                        Palette[palnum][(pa + palent.Destination) / 16, (pa + palent.Destination) % 16] = palfile[pa + palent.Source];
                        PalNum[palnum][(pa + palent.Destination) / 16, (pa + palent.Destination) % 16] = palfilenum;
                        PalAddr[palnum][(pa + palent.Destination) / 16, (pa + palent.Destination) % 16] = pa + palent.Source;
                    }
                    palfilenum++;
                }
                palnum++;
            }
            CurPal = 0;
            Sprites = new List<Sprite>();
            if (loadGraphics)
            {
                Bitmap palbmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed);
                BmpPal = palbmp.Palette;
                for (int i = 0; i < 64; i++)
                    BmpPal.Entries[i] = PaletteToColor(i / 16, i % 16, true);
                for (int i = 64; i < 256; i++)
                    BmpPal.Entries[i] = Color.Black;
                UnknownImg.Palette = BmpPal;
                if (Level.Sprites != null)
                {
                    tmp = Compression.Decompress(Level.Sprites, Compression.CompressionType.SZDD);
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
                if (!System.IO.Directory.Exists("dllcache"))
                {
                    System.IO.DirectoryInfo dir = System.IO.Directory.CreateDirectory("dllcache");
                    dir.Attributes |= System.IO.FileAttributes.Hidden;
                }
                S2RingDef = new DefS2RingDef();
                S2RingDef.Init(new ObjectData());
                S3KRingDef = new S3KRingDefinition();
                if (Game.ObjectList != null)
                    foreach (string item in Game.ObjectList)
                        LoadObjectDefinitionFile(item);
                if (Level.ObjectList != null)
                    foreach (string item in Level.ObjectList)
                        LoadObjectDefinitionFile(item);
                InitObjectDefinitions();
            }
            Objects = new List<ObjectEntry>();
            if (Level.Objects != null)
            {
                if (File.Exists(Level.Objects))
                {
                    Log("Loading objects from file \"" + Level.Objects + "\", using compression " + Level.ObjectCompression + "...");
                    tmp = Compression.Decompress(Level.Objects, Level.ObjectCompression);
                    switch (Level.ObjectFormat)
                    {
                        case EngineVersion.S1:
                            for (int oa = 0; oa < tmp.Length; oa += S1ObjectEntry.Size)
                            {
                                if (ByteConverter.ToUInt16(tmp, oa) == 0xFFFF) break;
                                Objects.Add(new S1ObjectEntry(tmp, oa));
                            }
                            break;
                        case EngineVersion.S2:
                        case EngineVersion.S2NA:
                        case EngineVersion.SBoom:
                            for (int oa = 0; oa < tmp.Length; oa += S2ObjectEntry.Size)
                            {
                                if (ByteConverter.ToUInt16(tmp, oa) == 0xFFFF) break;
                                Objects.Add(new S2ObjectEntry(tmp, oa));
                            }
                            break;
                        case EngineVersion.S3K:
                        case EngineVersion.SKC:
                            for (int oa = 0; oa < tmp.Length; oa += S3KObjectEntry.Size)
                            {
                                if (ByteConverter.ToUInt16(tmp, oa) == 0xFFFF) break;
                                Objects.Add(new S3KObjectEntry(tmp, oa));
                            }
                            break;
                        case EngineVersion.SCDPC:
                            for (int oa = 0; oa < tmp.Length; oa += SCDObjectEntry.Size)
                            {
                                if (ByteConverter.ToUInt64(tmp, oa) == 0xFFFFFFFFFFFFFFFF) break;
                                Objects.Add(new SCDObjectEntry(tmp, oa));
                            }
                            break;
                    }
                    if (loadGraphics)
                        for (int i = 0; i < Objects.Count; i++)
                        {
                            ChangeObjectType(Objects[i]);
                            Objects[i].UpdateSprite();
                        }
                }
                else
                    Log("Object file \"" + Level.Objects + "\" not found.");
            }
            Rings = new List<RingEntry>();
            if (Level.Rings != null)
            {
                switch (Level.RingFormat)
                {
                    case EngineVersion.S2:
                    case EngineVersion.S2NA:
                        if (File.Exists(Level.Rings))
                        {
                            Log("Loading rings from file \"" + Level.Rings + "\", using compression " + Level.RingCompression + "...");
                            tmp = Compression.Decompress(Level.Rings, Level.RingCompression);
                            for (int oa = 0; oa < tmp.Length; oa += S2RingEntry.Size)
                            {
                                if (ByteConverter.ToUInt16(tmp, oa) == 0xFFFF) break;
                                S2RingEntry ent = new S2RingEntry(tmp, oa);
                                Rings.Add(ent);
                                if (loadGraphics) ent.UpdateSprite();
                            }
                        }
                        else
                            Log("Ring file \"" + Level.Rings + "\" not found.");
                        break;
                    case EngineVersion.S3K:
                    case EngineVersion.SKC:
                        if (File.Exists(Level.Rings))
                        {
                            Log("Loading rings from file \"" + Level.Rings + "\", using compression " + Level.RingCompression + "...");
                            tmp = Compression.Decompress(Level.Rings, Level.RingCompression);
                            for (int oa = 4; oa < tmp.Length; oa += S3KRingEntry.Size)
                            {
                                if (ByteConverter.ToUInt16(tmp, oa) == 0xFFFF) break;
                                S3KRingEntry ent = new S3KRingEntry(tmp, oa);
                                Rings.Add(ent);
                                if (loadGraphics) ent.UpdateSprite();
                            }
                        }
                        else
                            Log("Ring file \"" + Level.Rings + "\" not found.");
                        break;
                    case EngineVersion.SBoom:
                        if (File.Exists(Level.Rings))
                        {
                            Log("Loading rings from file \"" + Level.Rings + "\", using compression " + Level.RingCompression + "...");
                            tmp = Compression.Decompress(Level.Rings, Level.RingCompression);
                            for (int oa = 0; oa < tmp.Length; oa += SonicBoom.SBoomRingEntry.Size)
                            {
                                if (ByteConverter.ToUInt16(tmp, oa) == 0xFFFF) break;
                                SonicBoom.SBoomRingEntry ent = new SonicBoom.SBoomRingEntry(tmp, oa);
                                Rings.Add(ent);
                                if (loadGraphics) ent.UpdateSprite();
                            }
                        }
                        else
                            Log("Ring file \"" + Level.Rings + "\" not found.");
                        break;
                }
            }
            if (Level.Bumpers != null)
            {
                Bumpers = new List<CNZBumperEntry>();
                if (File.Exists(Level.Bumpers))
                {
                    Log("Loading bumpers from file \"" + Level.Bumpers + "\", using compression " + Level.BumperCompression + "...");
                    tmp = Compression.Decompress(Level.Bumpers, Level.BumperCompression);
                    for (int i = 0; i < tmp.Length; i += CNZBumperEntry.Size)
                    {
                        if (ByteConverter.ToUInt16(tmp, i + 2) == 0xFFFF) break;
                        CNZBumperEntry ent = new CNZBumperEntry(tmp, i);
                        Bumpers.Add(ent);
                        if (loadGraphics) ent.UpdateSprite();
                    }
                }
                else
                    Log("Bumper file \"" + Level.Bumpers + "\" not found.");
            }
            else
            {
                Bumpers = null;
            }
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
                case EngineVersion.SBoom:
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
                    if (Level.CollisionIndex != null)
                    {
                        if (File.Exists(Level.CollisionIndex))
                        {
                            tmp = Compression.Decompress(Level.CollisionIndex, Level.CollisionIndexCompression);
                            switch (Level.CollisionIndexSize)
                            {
                                case 1:
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
                    }
                    break;
            }
            if (Game.EngineVersion != EngineVersion.S3K && Game.EngineVersion != EngineVersion.SKC)
            {
                if (ColInds1.Count < Blocks.Count)
                    ColInds1.AddRange(new byte[Blocks.Count - ColInds1.Count]);
                if (ColInds2.Count < Blocks.Count)
                    ColInds2.AddRange(new byte[Blocks.Count - ColInds2.Count]);
            }
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
            if (Level.Angles != null && File.Exists(Level.Angles))
                Angles = Compression.Decompress(Level.Angles, Level.AngleCompression);
            else
                Angles = new byte[256];
            if (loadGraphics)
            {
                BlockBmps = new List<Bitmap[]>();
                BlockBmpBits = new List<BitmapBits[]>();
                CompBlockBmps = new List<Bitmap>();
                CompBlockBmpBits = new List<BitmapBits>();
                Log("Drawing block bitmaps...");
                for (int bi = 0; bi < Blocks.Count; bi++)
                {
                    BlockBmps.Add(new Bitmap[2]);
                    BlockBmpBits.Add(new BitmapBits[2]);
                    CompBlockBmps.Add(null);
                    CompBlockBmpBits.Add(null);
                    RedrawBlock(bi, false);
                }
                ColBmps = new Bitmap[256];
                ColBmpBits = new BitmapBits[256];
                for (int ci = 0; ci < 256; ci++)
                    RedrawCol(ci, false);
                ChunkBmps = new List<Bitmap[]>();
                ChunkBmpBits = new List<BitmapBits[]>();
                ChunkColBmps = new List<Bitmap[]>();
                ChunkColBmpBits = new List<BitmapBits[]>();
                CompChunkBmps = new List<Bitmap>();
                CompChunkBmpBits = new List<BitmapBits>();
                Log("Drawing chunk bitmaps...");
                for (int ci = 0; ci < Chunks.Count; ci++)
                {
                    ChunkBmps.Add(new Bitmap[2]);
                    ChunkBmpBits.Add(new BitmapBits[2]);
                    ChunkColBmps.Add(new Bitmap[2]);
                    ChunkColBmpBits.Add(new BitmapBits[2]);
                    CompChunkBmps.Add(null);
                    CompChunkBmpBits.Add(null);
                    RedrawChunk(ci);
                }
            }
        }

        public static void SaveLevel()
        {
            Log("Saving " + Level.DisplayName + "...");
            int fileind = -1;
            List<byte> tmp;
            ReadOnlyCollection<ReadOnlyCollection<byte[]>> tilefiles = Tiles.GetFiles();
            if (Level.TileFormat != EngineVersion.SCDPC)
            {
                foreach (FileInfo tileent in Level.Tiles)
                {
                    fileind++;
                    tmp = new List<byte>();
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
                            if (!tilepals[blk.tiles[x, y].Palette].Contains(blk.tiles[x, y].Tile))
                                tilepals[blk.tiles[x, y].Palette].Add(blk.tiles[x, y].Tile);
                foreach (Block blk in Blocks)
                    for (int y = 0; y < 2; y++)
                        for (int x = 0; x < 2; x++)
                        {
                            byte pal = blk.tiles[x, y].Palette;
                            int c = 0;
                            for (int i = pal - 1; i >= 0; i--)
                                c += tilepals[i].Count;
                            blk.tiles[x, y].Tile = (ushort)(tilepals[pal].IndexOf(blk.tiles[x, y].Tile) + c);
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
                tmp = new List<byte>();
                tmp.Add(0x53);
                tmp.Add(0x43);
                tmp.Add(0x52);
                tmp.Add(0x4C);
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
                Compression.Compress(tmp.ToArray(), Level.Tiles[0].Filename, Compression.CompressionType.SZDD);
            }
            fileind = -1;
            ReadOnlyCollection<ReadOnlyCollection<Block>> blockfiles = Blocks.GetFiles();
            foreach (FileInfo tileent in Level.Blocks)
            {
                fileind++;
                tmp = new List<byte>();
                if (Level.BlockFormat == EngineVersion.SKC)
                    littleendian = false;
                foreach (Block b in blockfiles[fileind])
                    tmp.AddRange(b.GetBytes());
                if (Level.BlockFormat == EngineVersion.SKC)
                    littleendian = true;
                Compression.Compress(tmp.ToArray(), tileent.Filename, Level.BlockCompression);
            }
            fileind = -1;
            ReadOnlyCollection<ReadOnlyCollection<Chunk>> chunkfiles = Chunks.GetFiles();
            foreach (FileInfo tileent in Level.Chunks)
            {
                fileind++;
                tmp = new List<byte>();
                if (Level.ChunkFormat == EngineVersion.SKC)
                    littleendian = false;
                foreach (Chunk c in chunkfiles[fileind])
                    tmp.AddRange(c.GetBytes());
                if (Level.ChunkFormat == EngineVersion.SKC)
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
            switch (Level.LayoutFormat)
            {
                case EngineVersion.S1:
                    tmp = new List<byte>();
                    tmp.Add((byte)(FGLayout.GetLength(0) - 1));
                    tmp.Add((byte)(FGLayout.GetLength(1) - 1));
                    for (int lr = 0; lr < FGLayout.GetLength(1); lr++)
                        for (int lc = 0; lc < FGLayout.GetLength(0); lc++)
                            tmp.Add((byte)(FGLayout[lc, lr] | (FGLoop[lc, lr] ? 0x80 : 0)));
                    Compression.Compress(tmp.ToArray(), Level.FGLayout, Level.LayoutCompression);
                    tmp = new List<byte>();
                    tmp.Add((byte)(BGLayout.GetLength(0) - 1));
                    tmp.Add((byte)(BGLayout.GetLength(1) - 1));
                    for (int lr = 0; lr < BGLayout.GetLength(1); lr++)
                        for (int lc = 0; lc < BGLayout.GetLength(0); lc++)
                            tmp.Add((byte)(BGLayout[lc, lr] | (BGLoop[lc, lr] ? 0x80 : 0)));
                    Compression.Compress(tmp.ToArray(), Level.BGLayout, Level.LayoutCompression);
                    break;
                case EngineVersion.S2NA:
                    tmp = new List<byte>();
                    tmp.Add((byte)(FGLayout.GetLength(0) - 1));
                    tmp.Add((byte)(FGLayout.GetLength(1) - 1));
                    for (int lr = 0; lr < FGLayout.GetLength(1); lr++)
                        for (int lc = 0; lc < FGLayout.GetLength(0); lc++)
                            tmp.Add(FGLayout[lc, lr]);
                    Compression.Compress(tmp.ToArray(), Level.FGLayout, Level.LayoutCompression);
                    tmp = new List<byte>();
                    tmp.Add((byte)(BGLayout.GetLength(0) - 1));
                    tmp.Add((byte)(BGLayout.GetLength(1) - 1));
                    for (int lr = 0; lr < BGLayout.GetLength(1); lr++)
                        for (int lc = 0; lc < BGLayout.GetLength(0); lc++)
                            tmp.Add(BGLayout[lc, lr]);
                    Compression.Compress(tmp.ToArray(), Level.BGLayout, Level.LayoutCompression);
                    break;
                case EngineVersion.S2:
                    tmp = new List<byte>();
                    for (int la = 0; la < 16; la++)
                    {
                        for (int laf = 0; laf < 128; laf++)
                            tmp.Add(FGLayout[laf, la]);
                        for (int lab = 0; lab < 128; lab++)
                            tmp.Add(BGLayout[lab, la]);
                    }
                    Compression.Compress(tmp.ToArray(), Level.Layout, Level.LayoutCompression);
                    break;
                case EngineVersion.S3K:
                    tmp = new List<byte>();
                    ushort fgw = (ushort)FGLayout.GetLength(0);
                    ushort bgw = (ushort)BGLayout.GetLength(0);
                    ushort fgh = (ushort)FGLayout.GetLength(1);
                    ushort bgh = (ushort)BGLayout.GetLength(1);
                    tmp.AddRange(ByteConverter.GetBytes(fgw));
                    tmp.AddRange(ByteConverter.GetBytes(bgw));
                    tmp.AddRange(ByteConverter.GetBytes(fgh));
                    tmp.AddRange(ByteConverter.GetBytes(bgh));
                    for (int la = 0; la < 32; la++)
                    {
                        if (la < fgh)
                            tmp.AddRange(ByteConverter.GetBytes((ushort)(0x8088 + (la * fgw))));
                        else
                            tmp.AddRange(new byte[2]);
                        if (la < bgh)
                            tmp.AddRange(ByteConverter.GetBytes((ushort)(0x8088 + (fgh * fgw) + (la * bgw))));
                        else
                            tmp.AddRange(new byte[2]);
                    }
                    for (int y = 0; y < fgh; y++)
                        for (int x = 0; x < fgw; x++)
                            tmp.Add(FGLayout[x, y]);
                    for (int y = 0; y < bgh; y++)
                        for (int x = 0; x < bgw; x++)
                            tmp.Add(BGLayout[x, y]);
                    Compression.Compress(tmp.ToArray(), Level.Layout, Level.LayoutCompression);
                    break;
                case EngineVersion.SKC:
                    tmp = new List<byte>();
                    fgw = (ushort)FGLayout.GetLength(0);
                    bgw = (ushort)BGLayout.GetLength(0);
                    fgh = (ushort)FGLayout.GetLength(1);
                    bgh = (ushort)BGLayout.GetLength(1);
                    tmp.AddRange(ByteConverter.GetBytes(fgw));
                    tmp.AddRange(ByteConverter.GetBytes(bgw));
                    tmp.AddRange(ByteConverter.GetBytes(fgh));
                    tmp.AddRange(ByteConverter.GetBytes(bgh));
                    for (int la = 0; la < 32; la++)
                    {
                        if (la < fgh)
                            tmp.AddRange(ByteConverter.GetBytes((ushort)(0x8088 + (la * fgw))));
                        else
                            tmp.AddRange(new byte[2]);
                        if (la < bgh)
                            tmp.AddRange(ByteConverter.GetBytes((ushort)(0x8088 + (fgh * fgw) + (la * bgw))));
                        else
                            tmp.AddRange(new byte[2]);
                    }
                    List<byte> l = new List<byte>();
                    for (int y = 0; y < fgh; y++)
                        for (int x = 0; x < fgw; x++)
                            l.Add(FGLayout[x, y]);
                    for (int y = 0; y < bgh; y++)
                        for (int x = 0; x < bgw; x++)
                            l.Add(BGLayout[x, y]);
                    for (int i = 0; i < l.Count; i++)
                        tmp.Add(l[i ^ 1]);
                    Compression.Compress(tmp.ToArray(), Level.Layout, Level.LayoutCompression);
                    break;
                case EngineVersion.SCDPC:
                    tmp = new List<byte>();
                    for (int lr = 0; lr < 8; lr++)
                        for (int lc = 0; lc < 64; lc++)
                            tmp.Add(FGLayout[lc, lr]);
                    Compression.Compress(tmp.ToArray(), Level.FGLayout, Level.LayoutCompression);
                    tmp = new List<byte>();
                    for (int lr = 0; lr < 8; lr++)
                        for (int lc = 0; lc < 64; lc++)
                            tmp.Add(BGLayout[lc, lr]);
                    Compression.Compress(tmp.ToArray(), Level.BGLayout, Level.LayoutCompression);
                    break;
                case EngineVersion.SBoom:
                    SonicBoom.WriteLayout();
                    break;
            }
            if (Level.PaletteFormat != EngineVersion.SCDPC)
            {
                byte[] paltmp;
                List<ushort[]> palfiles = new List<ushort[]>();
                byte palfilenum = 0;
                int palnum = 0;
                while (Level.Palettes[palnum] != null)
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
                            palfiles[PalNum[palnum][pl, pi]][PalAddr[palnum][pl, pi]] = Palette[palnum][pl, pi];
                    for (byte pn = 0; pn < palent.Collection.Length; pn++)
                    {
                        tmp = new List<byte>();
                        for (int pi = 0; pi < palfiles[pn + palfilenum].Length; pi++)
                            tmp.AddRange(ByteConverter.GetBytes(palfiles[pn + palfilenum][pi]));
                        File.WriteAllBytes(palent[pn].Filename, tmp.ToArray());
                    }
                    palnum++;
                    palfilenum = (byte)palfiles.Count;
                }
            }
            else
            {
                List<byte[]> palfiles = new List<byte[]>();
                byte palfilenum = 0;
                if (Level.Palettes[0] != null)
                {
                    PaletteList palent = Level.Palettes[0].Palettes;
                    for (byte pn = 0; pn < palent.Collection.Length; pn++)
                        palfiles.Add(System.IO.File.ReadAllBytes(palent[pn].Filename));
                    for (int pl = 0; pl < 4; pl++)
                    {
                        for (int pi = 0; pi < 16; pi++)
                        {
                            palfiles[PalNum[0][pl, pi]][PalAddr[0][pl, pi] * 4] = (byte)((Palette[0][pl, pi] & 0xF) * 0x11);
                            palfiles[PalNum[0][pl, pi]][PalAddr[0][pl, pi] * 4 + 1] = (byte)(((Palette[0][pl, pi] & 0xF0) >> 4) * 0x11);
                            palfiles[PalNum[0][pl, pi]][PalAddr[0][pl, pi] * 4 + 2] = (byte)(((Palette[0][pl, pi] & 0xF00) >> 8) * 0x11);
                        }
                    }
                    for (byte pn = 0; pn < palent.Collection.Length; pn++)
                        File.WriteAllBytes(palent[pn].Filename, palfiles[pn]);
                    palfilenum = (byte)palfiles.Count;
                }
            }
            if (Level.Objects != null)
            {
                Objects.Sort();
                tmp = new List<byte>();
                switch (Level.ObjectFormat)
                {
                    case EngineVersion.S1:
                        for (int oi = 0; oi < Objects.Count; oi++)
                            tmp.AddRange(((S1ObjectEntry)Objects[oi]).GetBytes());
                        tmp.AddRange(new byte[] { 0xFF, 0xFF });
                        while (tmp.Count % S1ObjectEntry.Size > 0)
                            tmp.Add(0);
                        break;
                    case EngineVersion.S2:
                    case EngineVersion.S2NA:
                    case EngineVersion.SBoom:
                        for (int oi = 0; oi < Objects.Count; oi++)
                            tmp.AddRange(((S2ObjectEntry)Objects[oi]).GetBytes());
                        tmp.AddRange(new byte[] { 0xFF, 0xFF });
                        while (tmp.Count % S2ObjectEntry.Size > 0)
                            tmp.Add(0);
                        break;
                    case EngineVersion.S3K:
                    case EngineVersion.SKC:
                        for (int oi = 0; oi < Objects.Count; oi++)
                            tmp.AddRange(((S3KObjectEntry)Objects[oi]).GetBytes());
                        tmp.AddRange(new byte[] { 0xFF, 0xFF });
                        while (tmp.Count % S3KObjectEntry.Size > 0)
                            tmp.Add(0);
                        break;
                    case EngineVersion.SCDPC:
                        for (int oi = 0; oi < Objects.Count; oi++)
                            tmp.AddRange(((SCDObjectEntry)Objects[oi]).GetBytes());
                        tmp.Add(0xFF);
                        while (tmp.Count % SCDObjectEntry.Size > 0)
                            tmp.Add(0xFF);
                        break;
                }
                Compression.Compress(tmp.ToArray(), Level.Objects, Level.ObjectCompression);
            }
            if (Level.Rings != null)
            {
                Rings.Sort();
                switch (Level.RingFormat)
                {
                    case EngineVersion.S2:
                    case EngineVersion.S2NA:
                        tmp = new List<byte>();
                        for (int ri = 0; ri < Rings.Count; ri++)
                            tmp.AddRange(((S2RingEntry)Rings[ri]).GetBytes());
                        tmp.AddRange(new byte[] { 0xFF, 0xFF });
                        Compression.Compress(tmp.ToArray(), Level.Rings, Level.RingCompression);
                        break;
                    case EngineVersion.S3K:
                    case EngineVersion.SKC:
                        tmp = new List<byte>();
                        tmp.AddRange(new byte[] { 0, 0, 0, 0 });
                        for (int ri = 0; ri < Rings.Count; ri++)
                            tmp.AddRange(((S3KRingEntry)Rings[ri]).GetBytes());
                        tmp.AddRange(new byte[] { 0xFF, 0xFF });
                        Compression.Compress(tmp.ToArray(), Level.Rings, Level.RingCompression);
                        break;
                    case EngineVersion.SBoom:
                        tmp = new List<byte>();
                        for (int ri = 0; ri < Rings.Count; ri++)
                            tmp.AddRange(((SonicBoom.SBoomRingEntry)Rings[ri]).GetBytes());
                        tmp.AddRange(new byte[] { 0xFF, 0xFF });
                        Compression.Compress(tmp.ToArray(), Level.Rings, Level.RingCompression);
                        break;
                }
            }
            if (Bumpers != null)
            {
                Bumpers.Sort();
                tmp = new List<byte>();
                foreach (CNZBumperEntry item in Bumpers)
                    tmp.AddRange(item.GetBytes());
                tmp.AddRange(new byte[] { 0, 0, 0xFF, 0xFF, 0, 0 });
                Compression.Compress(tmp.ToArray(), Level.Bumpers, Level.BumperCompression);
            }
            if (Level.StartPositions != null)
            {
                int i = 0;
                foreach (StartPositionInfo item in Level.StartPositions)
                {
                    byte[] fc = new byte[4];
                    if (File.Exists(item.Filename))
                        fc = File.ReadAllBytes(item.Filename);
                    StartPositions[i++].GetBytes().CopyTo(fc, item.Offset == -1 ? 0 : item.Offset);
                    File.WriteAllBytes(item.Filename, fc);
                }
            }
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
                case EngineVersion.SBoom:
                    if (Level.CollisionIndex1 != null)
                        Compression.Compress(ColInds1.ToArray(), Level.CollisionIndex1, Level.CollisionIndexCompression);
                    if (Level.CollisionIndex2 != null)
                        Compression.Compress(ColInds2.ToArray(), Level.CollisionIndex2, Level.CollisionIndexCompression);
                    break;
                case EngineVersion.S3K:
                case EngineVersion.SKC:
                    if (Level.CollisionIndex != null)
                    {
                        tmp = new List<byte>();
                        switch (Level.CollisionIndexSize)
                        {
                            case 1:
                                for (int i = 0; i < 0x300; i++)
                                {
                                    tmp.Add(ColInds1[i]);
                                    tmp.Add(ColInds2[i]);
                                }
                                break;
                            case 2:
                                foreach (byte item in ColInds1)
                                    tmp.AddRange(ByteConverter.GetBytes((ushort)item));
                                foreach (byte item in ColInds2)
                                    tmp.AddRange(ByteConverter.GetBytes((ushort)item));
                                break;
                        }
                        Compression.Compress(tmp.ToArray(), Level.CollisionIndex, Level.CollisionIndexCompression);
                    }
                    break;
            }
            if (Level.CollisionArray1 != null)
            {
                tmp = new List<byte>();
                for (int i = 0; i < 256; i++)
                    for (int j = 0; j < 16; j++)
                        tmp.Add(unchecked((byte)ColArr1[i][j]));
                Compression.Compress(tmp.ToArray(), Level.CollisionArray1, Level.CollisionArrayCompression);
            }
            if (Level.CollisionArray2 != null)
            {
                sbyte[][] rotcol = GenerateRotatedCollision();
                tmp = new List<byte>();
                for (int i = 0; i < 256; i++)
                    for (int j = 0; j < 16; j++)
                        tmp.Add(unchecked((byte)rotcol[i][j]));
                Compression.Compress(tmp.ToArray(), Level.CollisionArray2, Level.CollisionArrayCompression);
            }
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
                for (int y = 0; y < FGLayout.GetLength(1); y++)
                    for (int x = 0; x < FGLayout.GetLength(0); x++)
                        if (FGLayout[x, y] > 0)
                        {
                            xend = Math.Max(xend, x);
                            yend = Math.Max(yend, y);
                        }
                xend++;
                yend++;
                bounds = new Rectangle(0, 0, xend * chunksz, yend * chunksz);
            }
            BitmapBits LevelImg8bpp = new BitmapBits(bounds.Width, bounds.Height);
            for (int y = Math.Max(bounds.Y / chunksz, 0); y <= Math.Min(bounds.Bottom / chunksz, FGLayout.GetLength(1) - 1); y++)
                for (int x = Math.Max(bounds.X / chunksz, 0); x <= Math.Min(bounds.Right / chunksz, FGLayout.GetLength(0) - 1); x++)
                {
                    if (FGLayout[x, y] < Chunks.Count & lowPlane)
                        LevelImg8bpp.DrawBitmapComposited(ChunkBmpBits[FGLayout[x, y]][0], new Point(x * chunksz - bounds.X, y * chunksz - bounds.Y));
                    if (objectsAboveHighPlane)
                        if (FGLayout[x, y] < Chunks.Count)
                        {
                            if (highPlane)
                                LevelImg8bpp.DrawBitmapComposited(ChunkBmpBits[FGLayout[x, y]][1], new Point(x * chunksz - bounds.X, y * chunksz - bounds.Y));
                            if (collisionPath1)
                                LevelImg8bpp.DrawBitmapComposited(ChunkColBmpBits[FGLayout[x, y]][0], new Point(x * chunksz - bounds.X, y * chunksz - bounds.Y));
                            else if (collisionPath2)
                                LevelImg8bpp.DrawBitmapComposited(ChunkColBmpBits[FGLayout[x, y]][1], new Point(x * chunksz - bounds.X, y * chunksz - bounds.Y));
                        }
                }
            for (int oi = 0; oi < Objects.Count; oi++)
            {
                ObjectEntry oe = Objects[oi];
                Sprite spr = oe.Sprite;
                Point pt = new Point(spr.Offset.X - bounds.X, spr.Offset.Y - bounds.Y);
                if (ObjectVisible(oe, allTimeZones))
                    LevelImg8bpp.DrawBitmapComposited(spr.Image, pt);
            }
            for (int ri = 0; ri < Rings.Count; ri++)
                switch (Level.RingFormat)
                {
                    case EngineVersion.S2:
                    case EngineVersion.S2NA:
                    case EngineVersion.SBoom:
                        S2RingEntry re = (S2RingEntry)Rings[ri];
                        Sprite spr = re.Sprite;
                        Point pt = new Point(spr.Offset.X - bounds.X, spr.Offset.Y - bounds.Y);
                        LevelImg8bpp.DrawBitmapComposited(spr.Image, pt);
                        break;
                    case EngineVersion.S3K:
                    case EngineVersion.SKC:
                        S3KRingEntry re3 = (S3KRingEntry)Rings[ri];
                        spr = re3.Sprite;
                        pt = new Point(spr.Offset.X - bounds.X, spr.Offset.Y - bounds.Y);
                        LevelImg8bpp.DrawBitmapComposited(spr.Image, pt);
                        break;
                }
            if (Bumpers != null)
                foreach (CNZBumperEntry item in Bumpers)
                {
                    Sprite spr = item.Sprite;
                    Point pt = new Point(spr.Offset.X - bounds.X, spr.Offset.Y - bounds.Y);
                    LevelImg8bpp.DrawBitmapComposited(spr.Image, pt);
                }
            foreach (StartPositionEntry item in StartPositions)
            {
                Sprite spr = item.Sprite;
                Point pt = new Point(spr.Offset.X - bounds.X, spr.Offset.Y - bounds.Y);
                LevelImg8bpp.DrawBitmapComposited(spr.Image, pt);
            }
            if (!objectsAboveHighPlane)
                for (int y = Math.Max(bounds.Y / chunksz, 0); y <= Math.Min(bounds.Bottom / chunksz, FGLayout.GetLength(1) - 1); y++)
                    for (int x = Math.Max(bounds.X / chunksz, 0); x <= Math.Min(bounds.Right / chunksz, FGLayout.GetLength(0) - 1); x++)
                        if (FGLayout[x, y] < Chunks.Count)
                        {
                            if (highPlane)
                                LevelImg8bpp.DrawBitmapComposited(ChunkBmpBits[FGLayout[x, y]][1], new Point(x * chunksz - bounds.X, y * chunksz - bounds.Y));
                            if (collisionPath1)
                                LevelImg8bpp.DrawBitmapComposited(ChunkColBmpBits[FGLayout[x, y]][0], new Point(x * chunksz - bounds.X, y * chunksz - bounds.Y));
                            else if (collisionPath2)
                                LevelImg8bpp.DrawBitmapComposited(ChunkColBmpBits[FGLayout[x, y]][1], new Point(x * chunksz - bounds.X, y * chunksz - bounds.Y));
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
                for (int y = 0; y < BGLayout.GetLength(1); y++)
                    for (int x = 0; x < BGLayout.GetLength(0); x++)
                        if (BGLayout[x, y] > 0)
                        {
                            xend = Math.Max(xend, x);
                            yend = Math.Max(yend, y);
                        }
                xend++;
                yend++;
                bounds = new Rectangle(0, 0, xend * chunksz, yend * chunksz);
            }
            BitmapBits LevelImg8bpp = new BitmapBits(bounds.Width, bounds.Height);
            for (int y = Math.Max(bounds.Y / chunksz, 0); y <= Math.Min(bounds.Bottom / chunksz, BGLayout.GetLength(1) - 1); y++)
                for (int x = Math.Max(bounds.X / chunksz, 0); x <= Math.Min(bounds.Right / chunksz, BGLayout.GetLength(0) - 1); x++)
                    if (BGLayout[x, y] < Chunks.Count)
                    {
                        if (lowPlane)
                            LevelImg8bpp.DrawBitmapComposited(ChunkBmpBits[BGLayout[x, y]][0], new Point(x * chunksz - bounds.X, y * chunksz - bounds.Y));
                        if (highPlane)
                            LevelImg8bpp.DrawBitmapComposited(ChunkBmpBits[BGLayout[x, y]][1], new Point(x * chunksz - bounds.X, y * chunksz - bounds.Y));
                        if (collisionPath1)
                            LevelImg8bpp.DrawBitmapComposited(ChunkColBmpBits[BGLayout[x, y]][0], new Point(x * chunksz - bounds.X, y * chunksz - bounds.Y));
                        else if (collisionPath2)
                            LevelImg8bpp.DrawBitmapComposited(ChunkColBmpBits[BGLayout[x, y]][1], new Point(x * chunksz - bounds.X, y * chunksz - bounds.Y));
                    }
            return LevelImg8bpp;
        }

        public static bool ObjectVisible(ObjectEntry obj, bool allTimeZones)
        {
            if (allTimeZones)
                return true;
            if (obj is SCDObjectEntry)
            {
                SCDObjectEntry scdobj = (SCDObjectEntry)obj;
                switch (Level.TimeZone)
                {
                    case API.TimeZone.Past:
                        return scdobj.ShowPast;
                    case API.TimeZone.Present:
                        return scdobj.ShowPresent;
                    case API.TimeZone.Future:
                        return scdobj.ShowFuture;
                    default:
                        return true;
                }
            }
            return true;
        }

        private static void LoadObjectDefinitionFile(string file)
        {
            Log("Loading object definition file \"" + file + "\".");
            Dictionary<string, ObjectData> obj = IniFile.Deserialize<Dictionary<string, ObjectData>>(file);
            foreach (KeyValuePair<string, ObjectData> group in obj)
            {
                group.Value.Init();
                INIObjDefs[group.Key] = group.Value;
            }
        }

        private static void InitObjectDefinitions()
        {
            foreach (KeyValuePair<string, ObjectData> group in INIObjDefs)
            {
                byte ID;
                if (group.Key == "Ring")
                {
                    switch (Level.RingFormat)
                    {
                        case EngineVersion.S2:
                        case EngineVersion.S2NA:
                        case EngineVersion.SBoom:
                            string ty = group.Value.CodeType;
                            string dllfile = System.IO.Path.Combine("dllcache", ty + ".dll");
                            DateTime modDate = DateTime.MinValue;
                            if (System.IO.File.Exists(dllfile))
                                modDate = System.IO.File.GetLastWriteTime(dllfile);
                            string fp = group.Value.CodeFile.Replace('/', System.IO.Path.DirectorySeparatorChar);
                            Log("Loading S2RingDefinition type " + ty + " from \"" + fp + "\"...");
                            if (modDate >= File.GetLastWriteTime(fp) & modDate > File.GetLastWriteTime(Application.ExecutablePath))
                            {
                                Log("Loading type from cached assembly \"" + dllfile + "\"...");
                                S2RingDef = (S2RingDefinition)Activator.CreateInstance(System.Reflection.Assembly.LoadFile(System.IO.Path.Combine(Environment.CurrentDirectory, dllfile)).GetType(ty));
                            }
                            else
                            {
                                Log("Compiling code file...");
                                string ext = System.IO.Path.GetExtension(fp);
                                CodeDomProvider pr = null;
                                switch (ext.ToLowerInvariant())
                                {
                                    case ".cs":
                                        pr = new Microsoft.CSharp.CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
                                        break;
                                    case ".vb":
                                        pr = new Microsoft.VisualBasic.VBCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
                                        break;
#if false
                                    case ".js":
                                        pr = new Microsoft.JScript.JScriptCodeProvider();
                                        break;
#endif
                                }
                                CompilerParameters para = new CompilerParameters(new string[] { "System.dll", "System.Core.dll", "System.Drawing.dll", System.Reflection.Assembly.GetExecutingAssembly().Location });
                                para.GenerateExecutable = false;
                                para.GenerateInMemory = false;
                                para.IncludeDebugInformation = true;
                                para.OutputAssembly = System.IO.Path.Combine(Environment.CurrentDirectory, dllfile);
                                CompilerResults res = pr.CompileAssemblyFromFile(para, fp);
                                if (res.Errors.HasErrors)
                                {
                                    Log("Compile failed.", "Errors:");
                                    foreach (CompilerError item in res.Errors)
                                        Log(item.ToString());
                                    Log(string.Empty);
                                    S2RingDef = new DefS2RingDef();
                                }
                                else
                                {
                                    Log("Compile succeeded.");
                                    S2RingDef = (S2RingDefinition)Activator.CreateInstance(res.CompiledAssembly.GetType(ty));
                                }
                            }
                            S2RingDef.Init(group.Value);
                            Log("S2 Ring Definition loaded.");
                            break;
                        case EngineVersion.S3K:
                        case EngineVersion.SKC:
                            Log("Loading S3K Ring Definition...");
                            S3KRingDef = new S3KRingDefinition(group.Value);
                            break;
                    }
                }
                else if (byte.TryParse(group.Key, System.Globalization.NumberStyles.HexNumber, System.Globalization.NumberFormatInfo.InvariantInfo, out ID))
                {
                    ObjectDefinition def = null;
                    if (group.Value.CodeFile != null)
                    {
                        string ty = group.Value.CodeType;
                        string dllfile = System.IO.Path.Combine("dllcache", ty + ".dll");
                        DateTime modDate = DateTime.MinValue;
                        if (System.IO.File.Exists(dllfile))
                            modDate = System.IO.File.GetLastWriteTime(dllfile);
                        string fp = group.Value.CodeFile.Replace('/', System.IO.Path.DirectorySeparatorChar);
                        Log("Loading ObjectDefinition type " + ty + " from \"" + fp + "\"...");
                        if (modDate >= File.GetLastWriteTime(fp) & modDate > File.GetLastWriteTime(Application.ExecutablePath))
                        {
                            Log("Loading type from cached assembly \"" + dllfile + "\"...");
                            def = (ObjectDefinition)Activator.CreateInstance(System.Reflection.Assembly.LoadFile(System.IO.Path.Combine(Environment.CurrentDirectory, dllfile)).GetType(ty));
                        }
                        else
                        {
                            Log("Compiling code file...");
                            string ext = System.IO.Path.GetExtension(fp);
                            CodeDomProvider pr = null;
                            switch (ext.ToLowerInvariant())
                            {
                                case ".cs":
                                    pr = new Microsoft.CSharp.CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
                                    break;
                                case ".vb":
                                    pr = new Microsoft.VisualBasic.VBCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
                                    break;
#if false
                                case ".js":
                                    pr = new Microsoft.JScript.JScriptCodeProvider();
                                    break;
#endif
                            }
                            if (pr != null)
                            {
                                CompilerParameters para = new CompilerParameters(new string[] { "System.dll", "System.Core.dll", "System.Drawing.dll", System.Reflection.Assembly.GetExecutingAssembly().Location });
                                para.GenerateExecutable = false;
                                para.GenerateInMemory = false;
                                para.IncludeDebugInformation = true;
                                para.OutputAssembly = System.IO.Path.Combine(Environment.CurrentDirectory, dllfile);
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
                                    def = (ObjectDefinition)Activator.CreateInstance(res.CompiledAssembly.GetType(ty));
                                }
                            }
                            else
                                def = new DefaultObjectDefinition();
                        }
                    }
                    else if (group.Value.XMLFile != null)
                    {
                        XMLDef.ObjDef xdef = XMLDef.ObjDef.Load(group.Value.XMLFile);
                        string ty = xdef.Namespace + "." + xdef.TypeName;
                        string dllfile = System.IO.Path.Combine("dllcache", ty + ".dll");
                        DateTime modDate = DateTime.MinValue;
                        if (System.IO.File.Exists(dllfile))
                            modDate = System.IO.File.GetLastWriteTime(dllfile);
                        Log("Loading ObjectDefinition type " + ty + " from \"" + group.Value.XMLFile + "\"...");
                        if (modDate >= File.GetLastWriteTime(group.Value.XMLFile) & modDate > File.GetLastWriteTime(Application.ExecutablePath))
                        {
                            Log("Loading type from cached assembly \"" + dllfile + "\"...");
                            def = (ObjectDefinition)Activator.CreateInstance(System.Reflection.Assembly.LoadFile(System.IO.Path.Combine(Environment.CurrentDirectory, dllfile)).GetType(ty));
                        }
                        else
                        {
                            Log("Building code file...");
                            Type basetype;
                            switch (Level.ObjectFormat)
                            {
                                case EngineVersion.S1:
                                    basetype = typeof(S1ObjectEntry);
                                    break;
                                case EngineVersion.S2:
                                case EngineVersion.S2NA:
                                case EngineVersion.SBoom:
                                    basetype = typeof(S2ObjectEntry);
                                    break;
                                case EngineVersion.S3K:
                                case EngineVersion.SKC:
                                    basetype = typeof(S3KObjectEntry);
                                    break;
                                case EngineVersion.SCD:
                                case EngineVersion.SCDPC:
                                    basetype = typeof(SCDObjectEntry);
                                    break;
                                default:
                                    basetype = typeof(ObjectEntry);
                                    break;
                            }
                            CodeTypeReferenceExpression objhelprefex = new CodeTypeReferenceExpression(typeof(ObjectHelper));
                            CodeThisReferenceExpression thisref = new CodeThisReferenceExpression();
                            List<CodeTypeMember> members = new List<CodeTypeMember>();
                            CodeMemberMethod method = new CodeMemberMethod();
                            method.Attributes = MemberAttributes.Override | MemberAttributes.Public;
                            method.Name = "Init";
                            method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(ObjectData), "data"));
                            method.ReturnType = new CodeTypeReference(typeof(void));
                            method.Statements.Add(new CodeVariableDeclarationStatement(typeof(MultiFileIndexer<byte>), "artfiles", new CodePrimitiveExpression(null)));
                            members.Add(new CodeMemberField(typeof(Sprite), "unkimg") { Attributes = MemberAttributes.Private });
                            method.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(thisref, "unkimg"), new CodePropertyReferenceExpression(objhelprefex, "UnknownObject")));
                            if (xdef.Images != null & xdef.Images.Items != null)
                            {
                                foreach (object item in xdef.Images.Items)
                                {
                                    if (item is XMLDef.ImageFromBitmap)
                                    {
                                        XMLDef.ImageFromBitmap img = (XMLDef.ImageFromBitmap)item;
                                        members.Add(new CodeMemberField(typeof(Sprite), img.id) { Attributes = MemberAttributes.Private });
                                        Point pnt = Point.Empty;
                                        if (img.offset != null)
                                            pnt = img.offset.ToPoint();
                                        method.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(thisref, img.id), new CodeObjectCreateExpression(typeof(Sprite),
                                            new CodeObjectCreateExpression(typeof(BitmapBits), new CodeObjectCreateExpression(typeof(Bitmap), new CodePrimitiveExpression(img.filename))),
                                            new CodeObjectCreateExpression(typeof(Point), new CodePrimitiveExpression(pnt.X), new CodePrimitiveExpression(pnt.Y)))));
                                    }
                                    else if (item is XMLDef.ImageFromMappings)
                                    {
                                        XMLDef.ImageFromMappings img = (XMLDef.ImageFromMappings)item;
                                        members.Add(new CodeMemberField(typeof(Sprite), img.id) { Attributes = MemberAttributes.Private });
                                        method.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("artfiles"), new CodeObjectCreateExpression(typeof(MultiFileIndexer<byte>))));
                                        foreach (XMLDef.ArtFile artfile in img.ArtFiles)
                                        {
                                            method.Statements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("artfiles"),
                                                "AddFile", new CodeObjectCreateExpression(typeof(List<byte>), new CodeMethodInvokeExpression(objhelprefex, "OpenArtFile",
                                                    new CodePrimitiveExpression(artfile.filename),
                                                    new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(Compression.CompressionType)), (artfile.compression == Compression.CompressionType.Invalid ? Compression.CompressionType.Nemesis : artfile.compression).ToString()))),
                                                    new CodePrimitiveExpression(artfile.offsetSpecified ? artfile.offset : -1)));
                                        }
                                        if (img.mappings is XMLDef.MapFileBin)
                                        {
                                            XMLDef.MapFileBin map = (XMLDef.MapFileBin)img.mappings;
                                            if (string.IsNullOrEmpty(map.dplcfile))
                                                method.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(thisref, img.id), new CodeMethodInvokeExpression(objhelprefex, "MapToBmp",
                                                    new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("artfiles"), "ToArray"),
                                                    new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(File)), "ReadAllBytes", new CodePrimitiveExpression(map.filename)),
                                                    new CodePrimitiveExpression(map.frame), new CodePrimitiveExpression(map.startpal), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(EngineVersion)), map.version.ToString()))));
                                            else
                                                method.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(thisref, img.id), new CodeMethodInvokeExpression(objhelprefex, "MapDPLCToBmp",
                                                    new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("artfiles"), "ToArray"),
                                                    new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(File)), "ReadAllBytes", new CodePrimitiveExpression(map.filename)), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(EngineVersion)), map.version.ToString()),
                                                    new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(File)), "ReadAllBytes", new CodePrimitiveExpression(map.dplcfile)),
                                                    new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(EngineVersion)), map.dplcver.ToString()),
                                                    new CodePrimitiveExpression(map.frame), new CodePrimitiveExpression(map.startpal))));
                                        }
                                        else if (img.mappings is XMLDef.MapFileAsm)
                                        {
                                            XMLDef.MapFileAsm map = (XMLDef.MapFileAsm)img.mappings;
                                            if (map.frameSpecified)
                                            {
                                                if (string.IsNullOrEmpty(map.dplcfile))
                                                    method.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(thisref, img.id), new CodeMethodInvokeExpression(objhelprefex, "MapASMToBmp",
                                                        new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("artfiles"), "ToArray"),
                                                        new CodePrimitiveExpression(map.filename), new CodePrimitiveExpression(map.frame),
                                                        new CodePrimitiveExpression(map.startpal), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(EngineVersion)), map.version.ToString()))));
                                                else
                                                    method.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(thisref, img.id), new CodeMethodInvokeExpression(objhelprefex, "MapDPLCASMToBmp",
                                                        new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("artfiles"), "ToArray"),
                                                        new CodePrimitiveExpression(map.filename), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(EngineVersion)), map.version.ToString()), new CodePrimitiveExpression(map.dplcfile),
                                                        new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(EngineVersion)), map.dplcver.ToString()),
                                                        new CodePrimitiveExpression(map.frame), new CodePrimitiveExpression(map.startpal))));
                                            }
                                            else
                                            {
                                                if (string.IsNullOrEmpty(map.dplcfile))
                                                    method.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(thisref, img.id), new CodeMethodInvokeExpression(objhelprefex, "MapASMToBmp",
                                                        new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("artfiles"), "ToArray"),
                                                        new CodePrimitiveExpression(map.filename), new CodePrimitiveExpression(map.label),
                                                        new CodePrimitiveExpression(map.startpal), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(EngineVersion)), map.version.ToString()))));
                                                else
                                                    method.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(thisref, img.id), new CodeMethodInvokeExpression(objhelprefex, "MapDPLCASMToBmp",
                                                        new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("artfiles"), "ToArray"),
                                                        new CodePrimitiveExpression(map.filename), new CodePrimitiveExpression(map.label), new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(EngineVersion)), map.version.ToString()),
                                                        new CodePrimitiveExpression(map.dplcfile), new CodePrimitiveExpression(map.dplclabel),
                                                        new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(EngineVersion)), map.dplcver.ToString()),
                                                        new CodePrimitiveExpression(map.startpal))));
                                            }
                                        }
                                        if (img.offset != null)
                                        {
                                            Point pnt = img.offset.ToPoint();
                                            method.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.id), "Offset"), new CodeObjectCreateExpression(typeof(Point), new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.id), "X"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(pnt.X)), new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.id), "Y"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(pnt.Y)))));
                                        }
                                    }
                                    else if (item is XMLDef.ImageFromSprite)
                                    {
                                        XMLDef.ImageFromSprite img = (XMLDef.ImageFromSprite)item;
                                        members.Add(new CodeMemberField(typeof(Sprite), img.id) { Attributes = MemberAttributes.Private });
                                        method.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(thisref, img.id), new CodeMethodInvokeExpression(objhelprefex, "GetSprite", new CodePrimitiveExpression(img.frame))));
                                        if (img.offset != null)
                                        {
                                            Point pnt = img.offset.ToPoint();
                                            method.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.id), "Offset"), new CodeObjectCreateExpression(typeof(Point), new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.id), "X"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(pnt.X)), new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.id), "Y"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(pnt.Y)))));
                                        }
                                    }
                                }
                            }
                            members.Add(method);
                            method = new CodeMemberMethod();
                            method.Attributes = MemberAttributes.Override | MemberAttributes.Public;
                            method.Name = "Name";
                            method.ReturnType = new CodeTypeReference(typeof(string));
                            method.Statements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(xdef.Name)));
                            members.Add(method);
                            method = new CodeMemberMethod();
                            method.Attributes = MemberAttributes.Override | MemberAttributes.Public;
                            method.Name = "Subtypes";
                            method.ReturnType = new CodeTypeReference(typeof(ReadOnlyCollection<byte>));
                            List<CodeExpression> subtypeexprs = new List<CodeExpression>();
                            if (xdef.Subtypes != null && xdef.Subtypes.Items != null)
                                foreach (XMLDef.Subtype item in xdef.Subtypes.Items)
                                    subtypeexprs.Add(new CodePrimitiveExpression(item.subtype));
                            method.Statements.Add(new CodeMethodReturnStatement(new CodeObjectCreateExpression(typeof(ReadOnlyCollection<byte>), new CodeArrayCreateExpression(typeof(byte), subtypeexprs.ToArray()))));
                            members.Add(method);
                            method = new CodeMemberMethod();
                            method.Attributes = MemberAttributes.Override | MemberAttributes.Public;
                            method.Name = "SubtypeName";
                            method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(byte), "subtype"));
                            method.ReturnType = new CodeTypeReference(typeof(string));
                            if (xdef.Subtypes != null && xdef.Subtypes.Items != null)
                                foreach (XMLDef.Subtype item in xdef.Subtypes.Items)
                                    method.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("subtype"), CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(item.subtype)), new CodeMethodReturnStatement(new CodePrimitiveExpression(item.name))));
                            method.Statements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(string.Empty)));
                            members.Add(method);
                            method = new CodeMemberMethod();
                            method.Attributes = MemberAttributes.Override | MemberAttributes.Public;
                            method.Name = "Image";
                            method.ReturnType = new CodeTypeReference(typeof(BitmapBits));
                            method.Statements.Add(new CodeMethodReturnStatement(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, string.IsNullOrEmpty(xdef.Image) ? "unkimg" : xdef.Image), "Image")));
                            members.Add(method);
                            method = new CodeMemberMethod();
                            method.Attributes = MemberAttributes.Override | MemberAttributes.Public;
                            method.Name = "Image";
                            method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(byte), "subtype"));
                            method.ReturnType = new CodeTypeReference(typeof(BitmapBits));
                            if (xdef.Subtypes != null && xdef.Subtypes.Items != null)
                                foreach (XMLDef.Subtype item in xdef.Subtypes.Items)
                                    method.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeArgumentReferenceExpression("subtype"), CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(item.subtype)), new CodeMethodReturnStatement(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, string.IsNullOrEmpty(item.image) ? "unkimg" : item.image), "Image"))));
                            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(thisref, "Image")));
                            members.Add(method);
                            method = new CodeMemberMethod();
                            method.Attributes = MemberAttributes.Override | MemberAttributes.Public;
                            method.Name = "GetSprite";
                            method.Parameters.AddRange(new CodeParameterDeclarationExpression[] { new CodeParameterDeclarationExpression(typeof(ObjectEntry), "obj") });
                            method.ReturnType = new CodeTypeReference(typeof(Sprite));
                            if (xdef.Display != null && !string.IsNullOrEmpty(xdef.Display.SpriteRoutine))
                                method.Statements.Add(new CodeSnippetStatement(xdef.Display.SpriteRoutine));
                            else if (xdef.Display != null && xdef.Display.DisplayOptions != null && xdef.Display.DisplayOptions.Length > 0)
                            {
                                Dictionary<string, string> props = new Dictionary<string, string>();
                                if (xdef.Properties != null && xdef.Properties.Items != null && xdef.Properties.Items.Length > 0)
                                    foreach (object item in xdef.Properties.Items)
                                        if (item is XMLDef.BitsProperty)
                                        {
                                            XMLDef.BitsProperty bp = (XMLDef.BitsProperty)item;
                                            props.Add(bp.name, bp.type);
                                        }
                                        else
                                        {
                                            XMLDef.CustomProperty cp = (XMLDef.CustomProperty)item;
                                            props.Add(cp.name, cp.type);
                                        }
                                if (props.Count > 0)
                                    method.Statements.Add(new CodeVariableDeclarationStatement(xdef.TypeName + basetype.Name, "obj2", new CodeCastExpression(xdef.TypeName + basetype.Name, new CodeArgumentReferenceExpression("obj"))));
                                else
                                    method.Statements.Add(new CodeVariableDeclarationStatement(basetype, "obj2", new CodeCastExpression(basetype, new CodeArgumentReferenceExpression("obj"))));
                                List<string> enums = new List<string>();
                                if (xdef.Enums != null && xdef.Enums.Items != null && xdef.Enums.Items.Length > 0)
                                    foreach (XMLDef.Enum item in xdef.Enums.Items)
                                        enums.Add(item.name);
                                foreach (XMLDef.DisplayOption opt in xdef.Display.DisplayOptions)
                                {
                                    CodeExpression condlist = null;
                                    if (opt.Conditions != null && opt.Conditions.Length > 0)
                                    {
                                        foreach (XMLDef.Condition item in opt.Conditions)
                                        {
                                            CodeBinaryOperatorExpression cond = new CodeBinaryOperatorExpression(null, CodeBinaryOperatorType.IdentityEquality, null);
                                            cond.Left = new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), item.property);
                                            if (props.ContainsKey(item.property))
                                            {
                                                if (enums.Contains(props[item.property]))
                                                    cond.Right = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(props[item.property]), item.value);
                                                else
                                                    switch (props[item.property])
                                                    {
                                                        case "bool":
                                                            cond.Right = new CodePrimitiveExpression(bool.Parse(item.value));
                                                            break;
                                                        case "byte":
                                                            cond.Right = new CodePrimitiveExpression(byte.Parse(item.value));
                                                            break;
                                                        case "int":
                                                            cond.Right = new CodePrimitiveExpression(int.Parse(item.value));
                                                            break;
                                                        default:
                                                            cond.Right = new CodePrimitiveExpression(item.value);
                                                            break;
                                                    }
                                            }
                                            else
                                            {
                                                Type t = basetype.GetProperty(item.property).PropertyType;
                                                if (t == typeof(bool))
                                                    cond.Right = new CodePrimitiveExpression(bool.Parse(item.value));
                                                if (t == typeof(byte) || t == typeof(ushort))
                                                    cond.Right = new CodePrimitiveExpression(int.Parse(item.value));
                                            }
                                            if (condlist == null)
                                                condlist = cond;
                                            else
                                                condlist = new CodeBinaryOperatorExpression(condlist, CodeBinaryOperatorType.BooleanAnd, cond);
                                        }
                                    }
                                    else
                                        condlist = new CodePrimitiveExpression(true);
                                    CodeConditionStatement ifstatement = new CodeConditionStatement(condlist);
                                    ifstatement.TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(BitmapBits), "bits"));
                                    ifstatement.TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(int), "xoff"));
                                    ifstatement.TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(int), "yoff"));
                                    ifstatement.TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(List<Sprite>), "sprs", new CodeObjectCreateExpression(typeof(List<Sprite>))));
                                    if (opt.Images != null)
                                        foreach (XMLDef.ImageRef img in opt.Images)
                                        {
                                            int xoff = img.Offset != null ? img.Offset.X : 0;
                                            int yoff = img.Offset != null ? img.Offset.Y : 0;
                                            ifstatement.TrueStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("xoff"), new CodePrimitiveExpression(img.xflip ? -xoff : xoff)));
                                            if (!img.xflipSpecified)
                                                ifstatement.TrueStatements.Add(new CodeConditionStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "XFlip"), new CodeAssignStatement(new CodeVariableReferenceExpression("xoff"), new CodeBinaryOperatorExpression(new CodePrimitiveExpression(0), CodeBinaryOperatorType.Subtract, new CodeVariableReferenceExpression("xoff")))));
                                            ifstatement.TrueStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("yoff"), new CodePrimitiveExpression(img.yflip ? -yoff : yoff)));
                                            if (!img.yflipSpecified)
                                                ifstatement.TrueStatements.Add(new CodeConditionStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "YFlip"), new CodeAssignStatement(new CodeVariableReferenceExpression("yoff"), new CodeBinaryOperatorExpression(new CodePrimitiveExpression(0), CodeBinaryOperatorType.Subtract, new CodeVariableReferenceExpression("yoff")))));
                                            ifstatement.TrueStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("bits"), new CodeObjectCreateExpression(typeof(BitmapBits), new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.image), "Image"))));
                                            ifstatement.TrueStatements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("bits"), "Flip", new CodePrimitiveExpression(img.xflip), new CodePrimitiveExpression(img.yflip)));
                                            if (!img.xflipSpecified)
                                                if (!img.yflipSpecified)
                                                    ifstatement.TrueStatements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("bits"), "Flip", new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "XFlip"), new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "YFlip")));
                                                else
                                                    ifstatement.TrueStatements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("bits"), "Flip", new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "XFlip"), new CodePrimitiveExpression(false)));
                                            else if (!img.yflipSpecified)
                                                ifstatement.TrueStatements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("bits"), "Flip", new CodePrimitiveExpression(false), new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "YFlip")));
                                            ifstatement.TrueStatements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("sprs"), "Add", new CodeObjectCreateExpression(typeof(Sprite), new CodeVariableReferenceExpression("bits"), new CodeObjectCreateExpression(typeof(Point), new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.image), "X"), CodeBinaryOperatorType.Add, new CodeVariableReferenceExpression("xoff")), new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.image), "Y"), CodeBinaryOperatorType.Add, new CodeVariableReferenceExpression("yoff"))))));
                                        }
                                    ifstatement.TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(Sprite), "spr", new CodeObjectCreateExpression(typeof(Sprite), new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("sprs"), "ToArray"))));
                                    ifstatement.TrueStatements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("spr"), "Offset"), new CodeObjectCreateExpression(typeof(Point), new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "X"), CodeBinaryOperatorType.Add, new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("spr"), "X")), new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "Y"), CodeBinaryOperatorType.Add, new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("spr"), "Y")))));
                                    ifstatement.TrueStatements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("spr")));
                                    method.Statements.Add(ifstatement);
                                }
                                method.Statements.Add(new CodeVariableDeclarationStatement(typeof(BitmapBits), "unkbits", new CodeObjectCreateExpression(typeof(BitmapBits), new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, "unkimg"), "Image"))));
                                method.Statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("unkbits"), "Flip", new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "XFlip"), new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "YFlip"))));
                                method.Statements.Add(new CodeVariableDeclarationStatement(typeof(Sprite), "unkspr", new CodeObjectCreateExpression(typeof(Sprite), new CodeVariableReferenceExpression("unkbits"), new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, "unkimg"), "Offset"))));
                                method.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("unkspr"), "Offset"), new CodeObjectCreateExpression(typeof(Point), new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "X"), CodeBinaryOperatorType.Add, new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("unkspr"), "X")), new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "Y"), CodeBinaryOperatorType.Add, new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("unkspr"), "Y")))));
                                method.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("unkspr")));
                            }
                            else
                            {
                                method.Statements.Add(new CodeVariableDeclarationStatement(basetype, "obj2", new CodeCastExpression(basetype, new CodeArgumentReferenceExpression("obj"))));
                                if (xdef.Subtypes != null && xdef.Subtypes.Items != null)
                                {
                                    foreach (XMLDef.Subtype item in xdef.Subtypes.Items)
                                    {
                                        CodeConditionStatement ifstatement = new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "SubType"), CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(item.subtype)));
                                        ifstatement.TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(BitmapBits), "bits", new CodeObjectCreateExpression(typeof(BitmapBits), new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, item.image), "Image"))));
                                        ifstatement.TrueStatements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("bits"), "Flip", new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "XFlip"), new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "YFlip"))));
                                        ifstatement.TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(Sprite), "spr", new CodeObjectCreateExpression(typeof(Sprite), new CodeVariableReferenceExpression("bits"), new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, item.image), "Offset"))));
                                        ifstatement.TrueStatements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("spr"), "Offset"), new CodeObjectCreateExpression(typeof(Point), new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "X"), CodeBinaryOperatorType.Add, new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("spr"), "X")), new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "Y"), CodeBinaryOperatorType.Add, new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("spr"), "Y")))));
                                        ifstatement.TrueStatements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("spr")));
                                        method.Statements.Add(ifstatement);
                                    }
                                }
                                method.Statements.Add(new CodeVariableDeclarationStatement(typeof(BitmapBits), "unkbits", new CodeObjectCreateExpression(typeof(BitmapBits), new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, "unkimg"), "Image"))));
                                method.Statements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("unkbits"), "Flip", new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "XFlip"), new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "YFlip"))));
                                method.Statements.Add(new CodeVariableDeclarationStatement(typeof(Sprite), "unkspr", new CodeObjectCreateExpression(typeof(Sprite), new CodeVariableReferenceExpression("unkbits"), new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, "unkimg"), "Offset"))));
                                method.Statements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("unkspr"), "Offset"), new CodeObjectCreateExpression(typeof(Point), new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "X"), CodeBinaryOperatorType.Add, new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("unkspr"), "X")), new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "Y"), CodeBinaryOperatorType.Add, new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("unkspr"), "Y")))));
                                method.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("unkspr")));
                            }
                            members.Add(method);
                            method = new CodeMemberMethod();
                            method.Attributes = MemberAttributes.Override | MemberAttributes.Public;
                            method.Name = "Bounds";
                            method.Parameters.AddRange(new CodeParameterDeclarationExpression[] { new CodeParameterDeclarationExpression(typeof(ObjectEntry), "obj"), new CodeParameterDeclarationExpression(typeof(Point), "camera") });
                            method.ReturnType = new CodeTypeReference(typeof(Rectangle));
                            if (xdef.Display != null && !string.IsNullOrEmpty(xdef.Display.BoundsRoutine))
                                method.Statements.Add(new CodeSnippetStatement(xdef.Display.BoundsRoutine));
                            else if (xdef.Display != null && xdef.Display.DisplayOptions != null && xdef.Display.DisplayOptions.Length > 0)
                            {
                                Dictionary<string, string> props = new Dictionary<string, string>();
                                if (xdef.Properties != null && xdef.Properties.Items != null && xdef.Properties.Items.Length > 0)
                                    foreach (object item in xdef.Properties.Items)
                                        if (item is XMLDef.BitsProperty)
                                        {
                                            XMLDef.BitsProperty bp = (XMLDef.BitsProperty)item;
                                            props.Add(bp.name, bp.type);
                                        }
                                        else
                                        {
                                            XMLDef.CustomProperty cp = (XMLDef.CustomProperty)item;
                                            props.Add(cp.name, cp.type);
                                        }
                                if (props.Count > 0)
                                    method.Statements.Add(new CodeVariableDeclarationStatement(xdef.TypeName + basetype.Name, "obj2", new CodeCastExpression(xdef.TypeName + basetype.Name, new CodeArgumentReferenceExpression("obj"))));
                                else
                                    method.Statements.Add(new CodeVariableDeclarationStatement(basetype, "obj2", new CodeCastExpression(basetype, new CodeArgumentReferenceExpression("obj"))));
                                List<string> enums = new List<string>();
                                if (xdef.Enums != null && xdef.Enums.Items != null && xdef.Enums.Items.Length > 0)
                                    foreach (XMLDef.Enum item in xdef.Enums.Items)
                                        enums.Add(item.name);
                                foreach (XMLDef.DisplayOption opt in xdef.Display.DisplayOptions)
                                {
                                    CodeExpression condlist = null;
                                    if (opt.Conditions != null && opt.Conditions.Length > 0)
                                    {
                                        foreach (XMLDef.Condition item in opt.Conditions)
                                        {
                                            CodeBinaryOperatorExpression cond = new CodeBinaryOperatorExpression(null, CodeBinaryOperatorType.IdentityEquality, null);
                                            cond.Left = new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), item.property);
                                            if (props.ContainsKey(item.property))
                                            {
                                                if (enums.Contains(props[item.property]))
                                                    cond.Right = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(props[item.property]), item.value);
                                                else
                                                    switch (props[item.property])
                                                    {
                                                        case "bool":
                                                            cond.Right = new CodePrimitiveExpression(bool.Parse(item.value));
                                                            break;
                                                        case "byte":
                                                            cond.Right = new CodePrimitiveExpression(byte.Parse(item.value));
                                                            break;
                                                        case "int":
                                                            cond.Right = new CodePrimitiveExpression(int.Parse(item.value));
                                                            break;
                                                        default:
                                                            cond.Right = new CodePrimitiveExpression(item.value);
                                                            break;
                                                    }
                                            }
                                            else
                                            {
                                                Type t = basetype.GetProperty(item.property).PropertyType;
                                                if (t == typeof(bool))
                                                    cond.Right = new CodePrimitiveExpression(bool.Parse(item.value));
                                                if (t == typeof(byte) || t == typeof(ushort))
                                                    cond.Right = new CodePrimitiveExpression(int.Parse(item.value));
                                            }
                                            if (condlist == null)
                                                condlist = cond;
                                            else
                                                condlist = new CodeBinaryOperatorExpression(condlist, CodeBinaryOperatorType.BooleanAnd, cond);
                                        }
                                    }
                                    else
                                        condlist = new CodePrimitiveExpression(true);
                                    CodeConditionStatement ifstatement = new CodeConditionStatement(condlist, new CodeVariableDeclarationStatement(typeof(Rectangle), "rect", new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(Rectangle)), "Empty")));
                                    ifstatement.TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(int), "xoff"));
                                    ifstatement.TrueStatements.Add(new CodeVariableDeclarationStatement(typeof(int), "yoff"));
                                    bool first = true;
                                    foreach (XMLDef.ImageRef img in opt.Images)
                                    {
                                        int xoff = img.Offset != null ? img.Offset.X : 0;
                                        int yoff = img.Offset != null ? img.Offset.Y : 0;
                                        ifstatement.TrueStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("xoff"), new CodePrimitiveExpression(img.xflip ? -xoff : xoff)));
                                        if (!img.xflipSpecified)
                                            ifstatement.TrueStatements.Add(new CodeConditionStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "XFlip"), new CodeAssignStatement(new CodeVariableReferenceExpression("xoff"), new CodeBinaryOperatorExpression(new CodePrimitiveExpression(0), CodeBinaryOperatorType.Subtract, new CodeVariableReferenceExpression("xoff")))));
                                        ifstatement.TrueStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("xoff"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("xoff"), CodeBinaryOperatorType.Subtract, new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("camera"), "X"))));
                                        ifstatement.TrueStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("yoff"), new CodePrimitiveExpression(img.yflip ? -yoff : yoff)));
                                        if (!img.yflipSpecified)
                                            ifstatement.TrueStatements.Add(new CodeConditionStatement(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "YFlip"), new CodeAssignStatement(new CodeVariableReferenceExpression("yoff"), new CodeBinaryOperatorExpression(new CodePrimitiveExpression(0), CodeBinaryOperatorType.Subtract, new CodeVariableReferenceExpression("yoff")))));
                                        ifstatement.TrueStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("yoff"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("yoff"), CodeBinaryOperatorType.Subtract, new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("camera"), "Y"))));
                                        if (first)
                                        {
                                            ifstatement.TrueStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("rect"), new CodeObjectCreateExpression(typeof(Rectangle),
                                            new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "X"), CodeBinaryOperatorType.Add, new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.image), "Offset"), "X"), CodeBinaryOperatorType.Add, new CodeVariableReferenceExpression("xoff"))),
                                            new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "Y"), CodeBinaryOperatorType.Add, new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.image), "Offset"), "Y"), CodeBinaryOperatorType.Add, new CodeVariableReferenceExpression("yoff"))),
                                            new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.image), "Image"), "Width"),
                                            new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.image), "Image"), "Height"))));
                                            first = false;
                                        }
                                        else
                                            ifstatement.TrueStatements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("rect"), new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(Rectangle)), "Union", new CodeVariableReferenceExpression("rect"), new CodeObjectCreateExpression(typeof(Rectangle),
                                                new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "X"), CodeBinaryOperatorType.Add, new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.image), "Offset"), "X"), CodeBinaryOperatorType.Add, new CodeVariableReferenceExpression("xoff"))),
                                            new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "Y"), CodeBinaryOperatorType.Add, new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.image), "Offset"), "Y"), CodeBinaryOperatorType.Add, new CodeVariableReferenceExpression("yoff"))),
                                                new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.image), "Image"), "Width"),
                                                new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, img.image), "Image"), "Height")))));
                                    }
                                    ifstatement.TrueStatements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("rect")));
                                    method.Statements.Add(ifstatement);
                                }
                                method.Statements.Add(new CodeMethodReturnStatement(new CodeObjectCreateExpression(typeof(Rectangle), new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "X"), CodeBinaryOperatorType.Add, new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, "unkimg"), "Offset"), "X")), CodeBinaryOperatorType.Subtract, new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("camera"), "X")), new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "Y"), CodeBinaryOperatorType.Add, new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, "unkimg"), "Offset"), "Y")), CodeBinaryOperatorType.Subtract, new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("camera"), "Y")), new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, "unkimg"), "Image"), "Width"), new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, "unkimg"), "Image"), "Height"))));
                            }
                            else
                            {
                                method.Statements.Add(new CodeVariableDeclarationStatement(basetype, "obj2", new CodeCastExpression(basetype, new CodeArgumentReferenceExpression("obj"))));
                                if (xdef.Subtypes != null && xdef.Subtypes.Items != null)
                                    foreach (XMLDef.Subtype item in xdef.Subtypes.Items)
                                        method.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "SubType"), CodeBinaryOperatorType.ValueEquality, new CodePrimitiveExpression(item.subtype)), new CodeMethodReturnStatement(new CodeObjectCreateExpression(typeof(Rectangle), new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "X"), CodeBinaryOperatorType.Add, new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, item.image), "Offset"), "X")), CodeBinaryOperatorType.Subtract, new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("camera"), "X")), new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "Y"), CodeBinaryOperatorType.Add, new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, item.image), "Offset"), "Y")), CodeBinaryOperatorType.Subtract, new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("camera"), "Y")), new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, item.image), "Image"), "Width"), new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, item.image), "Image"), "Height")))));
                                method.Statements.Add(new CodeMethodReturnStatement(new CodeObjectCreateExpression(typeof(Rectangle), new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "X"), CodeBinaryOperatorType.Add, new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, "unkimg"), "Offset"), "X")), CodeBinaryOperatorType.Subtract, new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("camera"), "X")), new CodeBinaryOperatorExpression(new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("obj2"), "Y"), CodeBinaryOperatorType.Add, new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, "unkimg"), "Offset"), "Y")), CodeBinaryOperatorType.Subtract, new CodePropertyReferenceExpression(new CodeArgumentReferenceExpression("camera"), "Y")), new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, "unkimg"), "Image"), "Width"), new CodePropertyReferenceExpression(new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(thisref, "unkimg"), "Image"), "Height"))));
                            }
                            members.Add(method);
                            method = new CodeMemberMethod();
                            method.Attributes = MemberAttributes.Override | MemberAttributes.Public;
                            method.Name = "RememberState";
                            method.ReturnType = new CodeTypeReference(typeof(bool));
                            method.Statements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(xdef.RememberState)));
                            members.Add(method);
                            CodeMemberProperty prop = new CodeMemberProperty();
                            prop.Attributes = MemberAttributes.Override | MemberAttributes.Public;
                            prop.Name = "Debug";
                            prop.GetStatements.Add(new CodeMethodReturnStatement(new CodePrimitiveExpression(xdef.Debug)));
                            prop.HasGet = true;
                            prop.HasSet = false;
                            prop.Type = new CodeTypeReference(typeof(bool));
                            members.Add(prop);
                            if (xdef.Properties != null && xdef.Properties.Items != null && xdef.Properties.Items.Length > 0)
                            {
                                prop = new CodeMemberProperty();
                                prop.Attributes = MemberAttributes.Override | MemberAttributes.Public;
                                prop.Name = "ObjectType";
                                prop.GetStatements.Add(new CodeMethodReturnStatement(new CodeTypeOfExpression(xdef.TypeName + basetype.Name)));
                                prop.HasGet = true;
                                prop.HasSet = false;
                                prop.Type = new CodeTypeReference(typeof(Type));
                                members.Add(prop);
                            }
                            CodeTypeDeclaration ctd = new CodeTypeDeclaration(xdef.TypeName);
                            ctd.BaseTypes.Add(typeof(ObjectDefinition));
                            ctd.IsClass = true;
                            ctd.Members.AddRange(members.ToArray());
                            CodeNamespace cn = new CodeNamespace(xdef.Namespace);
                            cn.Imports.Add(new CodeNamespaceImport("System.Drawing"));
                            cn.Types.Add(ctd);
                            if (xdef.Properties != null && xdef.Properties.Items != null && xdef.Properties.Items.Length > 0)
                            {
                                members = new List<CodeTypeMember>();
                                CodeConstructor ctor = new CodeConstructor();
                                ctor.Attributes = MemberAttributes.Public;
                                ctor.BaseConstructorArgs.Add(new CodeSnippetExpression(string.Empty));
                                members.Add(ctor);
                                ctor = new CodeConstructor();
                                ctor.Attributes = MemberAttributes.Public;
                                ctor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression("file"));
                                ctor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression("address"));
                                ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(byte[]), "file"));
                                ctor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "address"));
                                members.Add(ctor);
                                foreach (object item in xdef.Properties.Items)
                                {
                                    if (item is XMLDef.BitsProperty)
                                    {
                                        XMLDef.BitsProperty bp = (XMLDef.BitsProperty)item;
                                        int mask = 0;
                                        for (int i = 0; i < bp.length; i++)
                                            mask += (int)Math.Pow(2, bp.startbit + i);
                                        prop = new CodeMemberProperty();
                                        prop.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(CategoryAttribute)), new CodeAttributeArgument(new CodePrimitiveExpression("Extended"))));
                                        if (!string.IsNullOrEmpty(bp.description))
                                            prop.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(DescriptionAttribute)), new CodeAttributeArgument(new CodePrimitiveExpression(bp.description))));
                                        prop.Attributes = MemberAttributes.Public;
                                        prop.Name = bp.name;
                                        if (ExpandTypeName(bp.type) != typeof(bool).FullName)
                                        {
                                            prop.GetStatements.Add(new CodeMethodReturnStatement(new CodeCastExpression(bp.type, new CodeMethodInvokeExpression(objhelprefex, "ShiftRight", new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(thisref, "SubType"), CodeBinaryOperatorType.BitwiseAnd, new CodePrimitiveExpression(mask)), new CodePrimitiveExpression(bp.startbit)))));
                                            prop.SetStatements.Add(new CodeAssignStatement(new CodePropertyReferenceExpression(thisref, "SubType"), new CodeMethodInvokeExpression(objhelprefex, "SetSubtypeMask", new CodePropertyReferenceExpression(thisref, "SubType"), new CodeCastExpression(typeof(byte), new CodeMethodInvokeExpression(objhelprefex, "ShiftLeft", new CodeCastExpression(typeof(byte), new CodePropertySetValueReferenceExpression()), new CodePrimitiveExpression(bp.startbit))), new CodePrimitiveExpression(mask))));
                                        }
                                        else
                                        {
                                            prop.GetStatements.Add(new CodeMethodReturnStatement(new CodeBinaryOperatorExpression(new CodeMethodInvokeExpression(objhelprefex, "ShiftRight", new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(thisref, "SubType"), CodeBinaryOperatorType.BitwiseAnd, new CodePrimitiveExpression(mask)), new CodePrimitiveExpression(bp.startbit)), CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(0))));
                                            prop.SetStatements.Add(new CodeConditionStatement(new CodePropertySetValueReferenceExpression(),
                                                new CodeStatement[] { new CodeAssignStatement(new CodePropertyReferenceExpression(thisref, "SubType"), new CodeMethodInvokeExpression(objhelprefex, "SetSubtypeMask", new CodePropertyReferenceExpression(thisref, "SubType"), new CodeCastExpression(typeof(byte), new CodePrimitiveExpression(mask)), new CodePrimitiveExpression(mask))) },
                                                new CodeStatement[] { new CodeAssignStatement(new CodePropertyReferenceExpression(thisref, "SubType"), new CodeMethodInvokeExpression(objhelprefex, "SetSubtypeMask", new CodePropertyReferenceExpression(thisref, "SubType"), new CodeCastExpression(typeof(byte), new CodePrimitiveExpression(0)), new CodePrimitiveExpression(mask))) }));
                                        }
                                        prop.HasGet = true;
                                        prop.HasSet = true;
                                        prop.Type = new CodeTypeReference(ExpandTypeName(bp.type));
                                        members.Add(prop);
                                    }
                                    else
                                    {
                                        XMLDef.CustomProperty cp = (XMLDef.CustomProperty)item;
                                        prop = new CodeMemberProperty();
                                        prop.Attributes = MemberAttributes.Public;
                                        if (cp.@override) prop.Attributes |= MemberAttributes.Override;
                                        prop.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(CategoryAttribute)), new CodeAttributeArgument(new CodePrimitiveExpression("Extended"))));
                                        if (!string.IsNullOrEmpty(cp.description))
                                            prop.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(DescriptionAttribute)), new CodeAttributeArgument(new CodePrimitiveExpression(cp.description))));
                                        prop.Name = cp.name;
                                        prop.GetStatements.Add(new CodeSnippetStatement(cp.get));
                                        prop.HasGet = true;
                                        prop.HasSet = true;
                                        prop.SetStatements.Add(new CodeSnippetStatement(cp.set));
                                        prop.Type = new CodeTypeReference(ExpandTypeName(cp.type));
                                        members.Add(prop);
                                    }
                                }
                                ctd = new CodeTypeDeclaration(xdef.TypeName + basetype.Name);
                                ctd.Attributes = MemberAttributes.Public;
                                ctd.BaseTypes.Add(basetype);
                                ctd.IsClass = true;
                                ctd.Members.AddRange(members.ToArray());
                                cn.Types.Add(ctd);
                            }
                            if (xdef.Enums != null && xdef.Enums.Items != null)
                            {
                                foreach (XMLDef.Enum item in xdef.Enums.Items)
                                {
                                    ctd = new CodeTypeDeclaration(item.name);
                                    ctd.Attributes = MemberAttributes.Public;
                                    ctd.BaseTypes.Add(typeof(int));
                                    ctd.IsEnum = true;
                                    foreach (XMLDef.EnumMember mem in item.Items)
                                    {
                                        CodeMemberField mf = new CodeMemberField(typeof(int), mem.name);
                                        if (mem.valueSpecified)
                                            mf.InitExpression = new CodePrimitiveExpression(mem.value);
                                        ctd.Members.Add(mf);
                                    }
                                    cn.Types.Add(ctd);
                                }
                            }
                            CodeCompileUnit ccu = new CodeCompileUnit();
                            ccu.Namespaces.Add(cn);
                            ccu.ReferencedAssemblies.AddRange(new string[] { "System.dll", "System.Core.dll", "System.Drawing.dll", System.Reflection.Assembly.GetExecutingAssembly().Location });
                            Log("Compiling code file...");
                            CodeDomProvider pr = null;
                            switch (xdef.Language.ToLowerInvariant())
                            {
                                case "cs":
                                    pr = new Microsoft.CSharp.CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
                                    break;
                                case "vb":
                                    pr = new Microsoft.VisualBasic.VBCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
                                    break;
#if false
                                case "js":
                                    pr = new Microsoft.JScript.JScriptCodeProvider();
                                    break;
#endif
                            }
                            if (pr != null)
                            {
#if DEBUG
                                StreamWriter sw = new StreamWriter(xdef.Namespace + "." + xdef.TypeName + "." + pr.FileExtension);
                                pr.GenerateCodeFromCompileUnit(ccu, sw, new CodeGeneratorOptions() { BlankLinesBetweenMembers = true, BracingStyle = "C", VerbatimOrder = true });
                                sw.Close();
#endif
                                CompilerParameters para = new CompilerParameters(new string[] { "System.dll", "System.Core.dll", "System.Drawing.dll", System.Reflection.Assembly.GetExecutingAssembly().Location });
                                para.GenerateExecutable = false;
                                para.GenerateInMemory = false;
                                para.IncludeDebugInformation = true;
                                para.OutputAssembly = System.IO.Path.Combine(Environment.CurrentDirectory, dllfile);
                                CompilerResults res = pr.CompileAssemblyFromDom(para, ccu);
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
                                    def = (ObjectDefinition)Activator.CreateInstance(res.CompiledAssembly.GetType(ty));
                                }
                            }
                            else
                                def = new DefaultObjectDefinition();
                        }
                    }
                    else
                        def = new DefaultObjectDefinition();
                    ObjTypes[ID] = def;
                    def.Init(group.Value);
                }
            }
        }

        private static string ExpandTypeName(string type)
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

        public static byte[] ReadFile(string file, Compression.CompressionType cmp)
        {
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

        public static Bitmap TileToBmp4bpp(byte[] file, int index, int palette)
        {
            Bitmap bmp = new Bitmap(8, 8, PixelFormat.Format4bppIndexed);
            if (file != null && index * 32 + 32 <= file.Length)
            {
                BitmapData bmpd = bmp.LockBits(new Rectangle(0, 0, 8, 8), ImageLockMode.WriteOnly, PixelFormat.Format4bppIndexed);
                System.Runtime.InteropServices.Marshal.Copy(file, index * 32, bmpd.Scan0, 32);
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
            BitmapBits bmp = new BitmapBits(8, 8);
            if (file != null && index * 32 + 32 <= file.Length)
            {
                for (int i = 0; i < 32; i++)
                {
                    bmp.Bits[i * 2] = (byte)((file[i + (index * 32)] >> 4) + (pal * 16));
                    bmp.Bits[(i * 2) + 1] = (byte)((file[i + (index * 32)] & 0xF) + (pal * 16));
                    if (bmp.Bits[i * 2] % 16 == 0) bmp.Bits[i * 2] = 0;
                    if (bmp.Bits[(i * 2) + 1] % 16 == 0) bmp.Bits[(i * 2) + 1] = 0;
                }
            }
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
                    endlist = Math.Min(endlist, val);
                }
            }
            return addresses.ToArray();
        }

        public static byte[] ASMToBin(string file)
        {
            return ASMToBin(file, 0);
        }

        public static byte[] ASMToBin(string file, string label)
        {
            string[] fc = System.IO.File.ReadAllLines(file);
            int sti = -1;
            for (int i = 0; i < fc.Length; i++)
            {
                if (fc[i].StartsWith(label + ":"))
                {
                    sti = i;
                    fc[i] = fc[i].Substring(label.Length + 1);
                }
            }
            if (sti == -1) return new byte[0];
            return ASMToBin(file, sti);
        }

        public static byte[] ASMToBin(string file, int sti)
        {
            Dictionary<string, int> labels = GetASMLabels(file, sti);
            string[] fc = System.IO.File.ReadAllLines(file);
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
                    if (result.Count % 2 == 1)
                        result.Add(0);
                }
                else if (ln[0].Equals("align"))
                {
                    uint alignment = ParseASMLong(ln[1]);
                    if (result.Count % alignment != 0)
                        result.AddRange(new byte[alignment - (result.Count % alignment)]);
                }
                else if (ln[0].Equals("offsetTable"))
                    offsetLabel = lastlabel;
                else if (ln[0].StartsWith("offsetTableEntry."))
                    switch (ln[0].Split('.')[1])
                    {
                        case "b":
                            result.Add(unchecked((byte)ParseASMOffset(ln[1] + "-" + offsetLabel, labels)));
                            break;
                        case "w":
                            result.AddRange(ByteConverter.GetBytes((short)ParseASMOffset(ln[1] + "-" + offsetLabel, labels)));
                            break;
                        case "l":
                            result.AddRange(ByteConverter.GetBytes(ParseASMOffset(ln[1] + "-" + offsetLabel, labels)));
                            break;
                    }
                else if (ln[0].StartsWith("dc."))
                {
                    string d = string.Empty;
                    for (int i = 1; i < ln.Length; i++)
                    {
                        d += ln[i];
                    }
                    string[] dats = d.Split(',');
                    switch (ln[0].Split('.')[1])
                    {
                        case "b":
                            foreach (string item in dats)
                                if (item.Contains("-"))
                                    if (item.StartsWith("-"))
                                        result.Add(ParseASMByte(item));
                                    else
                                        result.Add(unchecked((byte)ParseASMOffset(item, labels)));
                                else
                                    result.Add(ParseASMByte(item));
                            break;
                        case "w":
                            foreach (string item in dats)
                                if (item.Contains("-"))
                                    if (item.StartsWith("-"))
                                        result.AddRange(ByteConverter.GetBytes(ParseASMWord(item)));
                                    else
                                        result.AddRange(ByteConverter.GetBytes((short)ParseASMOffset(item, labels)));
                                else
                                    result.AddRange(ByteConverter.GetBytes(ParseASMWord(item)));
                            break;
                        case "l":
                            foreach (string item in dats)
                                if (item.Contains("-"))
                                    if (item.StartsWith("-"))
                                        result.AddRange(ByteConverter.GetBytes(ParseASMLong(item)));
                                    else
                                        result.AddRange(ByteConverter.GetBytes(ParseASMOffset(item, labels)));
                                else
                                    result.AddRange(ByteConverter.GetBytes(ParseASMLong(item)));
                            break;
                    }
                }
                else
                    break;
            }
            return result.ToArray();
        }

        public static byte ParseASMByte(string data)
        {
            if (data.StartsWith("-"))
            {
                if (data.StartsWith("-$"))
                    return unchecked((byte)-sbyte.Parse(data.Substring(2), System.Globalization.NumberStyles.HexNumber));
                else
                    return unchecked((byte)-sbyte.Parse(data, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo));
            }
            else
            {
                if (data.StartsWith("$"))
                    return byte.Parse(data.Substring(1), System.Globalization.NumberStyles.HexNumber);
                else
                    return byte.Parse(data, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
            }
        }

        public static ushort ParseASMWord(string data)
        {
            if (data.StartsWith("-"))
            {
                if (data.StartsWith("-$"))
                    return unchecked((ushort)-short.Parse(data.Substring(2), System.Globalization.NumberStyles.HexNumber));
                else
                    return unchecked((ushort)-short.Parse(data, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo));
            }
            else
            {
                if (data.StartsWith("$"))
                    return ushort.Parse(data.Substring(1), System.Globalization.NumberStyles.HexNumber);
                else
                    return ushort.Parse(data, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
            }
        }

        public static uint ParseASMLong(string data)
        {
            if (data.StartsWith("-"))
            {
                if (data.StartsWith("-$"))
                    return unchecked((uint)-int.Parse(data.Substring(2), System.Globalization.NumberStyles.HexNumber));
                else
                    return unchecked((uint)-int.Parse(data, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo));
            }
            else
            {
                if (data.StartsWith("$"))
                    return uint.Parse(data.Substring(1), System.Globalization.NumberStyles.HexNumber);
                else
                    return uint.Parse(data, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
            }
        }

        public static int ParseASMOffset(string data, Dictionary<string, int> labels)
        {
            int label1 = 0;
            if (labels.ContainsKey(data.Split('-')[0]))
                label1 = labels[data.Split('-')[0]];
            int label2 = 0;
            if (labels.ContainsKey(data.Split('-')[1]))
                label2 = labels[data.Split('-')[1]];
            return label1 - label2;
        }

        public static Dictionary<string, int> GetASMLabels(string file)
        {
            return GetASMLabels(file, 0);
        }

        public static Dictionary<string, int> GetASMLabels(string file, string label)
        {
            string[] fc = System.IO.File.ReadAllLines(file);
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
            return GetASMLabels(file, sti);
        }


        public static Dictionary<string, int> GetASMLabels(string file, int sti)
        {
            string[] fc = System.IO.File.ReadAllLines(file);
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
                    uint alignment = ParseASMLong(ln[1]);
                    if (curaddr % alignment != 0)
                        curaddr += (int)(alignment - (curaddr % alignment));
                }
                else if (ln[0].Equals("offsetTable"))
                    continue;
                else if (ln[0].StartsWith("offsetTableEntry."))
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

        public static Sprite[] MapFrameToBmp(byte[] art, MappingsFrame map, int startpal)
        {
            int left = 0;
            int right = 0;
            int top = 0;
            int bottom = 0;
            for (int i = 0; i < map.TileCount; i++)
            {
                left = Math.Min(map[i].X, left);
                right = Math.Max(map[i].X + (map[i].Width * 8), right);
                top = Math.Min(map[i].Y, top);
                bottom = Math.Max(map[i].Y + (map[i].Height * 8), bottom);
            }
            Point offset = new Point(left, top);
            BitmapBits[] bmp = new BitmapBits[] { new BitmapBits(right - left, bottom - top), new BitmapBits(right - left, bottom - top) };
            for (int i = map.TileCount - 1; i >= 0; i--)
            {
                BitmapBits pcbmp = new BitmapBits(map[i].Width * 8, map[i].Height * 8);
                int ti = 0;
                int pr = map[i].Tile.Priority ? 1 : 0;
                for (int x = 0; x < map[i].Width; x++)
                {
                    for (int y = 0; y < map[i].Height; y++)
                    {
                        pcbmp.DrawBitmapComposited(
                            TileToBmp8bpp(art, map[i].Tile.Tile + ti, (map[i].Tile.Palette + startpal) & 3),
                            new Point(x * 8, y * 8));
                        ti++;
                    }
                }
                pcbmp.Flip(map[i].Tile.XFlip, map[i].Tile.YFlip);
                bmp[pr].DrawBitmapComposited(pcbmp, new Point(map[i].X - left, map[i].Y - top));
            }
            return new Sprite[] { new Sprite(bmp[0], offset), new Sprite(bmp[1], offset) };
        }

        public static Sprite[] MapFrameDPLCToBmp(byte[] file, MappingsFrame map, DPLCFrame dplc, int startpal)
        {
            return MapFrameToBmp(ProcessDPLC(file, dplc), map, startpal);
        }

        public static Color PaletteToColor(int line, int index, bool acceptTransparent)
        {
            if (acceptTransparent && index == 0)
                return Color.Transparent;
            return Color.FromArgb(
                (Palette[CurPal][line, index] & 0xE) * 0x11,
                ((Palette[CurPal][line, index] & 0xE0) >> 4) * 0x11,
                ((Palette[CurPal][line, index] & 0xE00) >> 8) * 0x11
                );
        }

        public static void ColorToPalette(int line, int index, Color color)
        {
            Palette[CurPal][line, index] = (ushort)(((color.R / 0x11) & 0xE) | (((color.G / 0x11) << 4) & 0xE0) | (((color.B / 0x11) << 8) & 0xE00));
        }

        public static void RedrawBlock(int block, bool drawChunks)
        {
            BlockBmpBits[block][0] = new BitmapBits(16, 16);
            BlockBmpBits[block][1] = new BitmapBits(16, 16);
            CompBlockBmpBits[block] = new BitmapBits(16, 16);
            for (int by = 0; by < 2; by++)
            {
                for (int bx = 0; bx < 2; bx++)
                {
                    PatternIndex pt = Blocks[block].tiles[bx, by];
                    int pr = pt.Priority ? 1 : 0;
                    BitmapBits tile = TileToBmp8bpp(Tiles[pt.Tile], 0, pt.Palette);
                    tile.Flip(pt.XFlip, pt.YFlip);
                    BlockBmpBits[block][pr].DrawBitmap(
                        tile,
                        new Point(bx * 8, by * 8)
                        );
                }
            }
            CompBlockBmpBits[block].DrawBitmapComposited(BlockBmpBits[block][0], Point.Empty);
            CompBlockBmpBits[block].DrawBitmapComposited(BlockBmpBits[block][1], Point.Empty);
            BlockBmps[block][0] = BlockBmpBits[block][0].ToBitmap(BmpPal);
            BlockBmps[block][1] = BlockBmpBits[block][1].ToBitmap(BmpPal);
            CompBlockBmps[block] = CompBlockBmpBits[block].ToBitmap(BmpPal);
            if (drawChunks)
            {
                for (int i = 0; i < Chunks.Count; i++)
                {
                    bool dr = false;
                    for (int k = 0; k < 8; k++)
                        for (int j = 0; j < 8; j++)
                            if (Chunks[i].blocks[j, k].Block == block)
                                dr = true;
                    if (dr)
                        RedrawChunk(i);
                }
            }
        }

        public static void RedrawChunk(int chunk)
        {
            ChunkBmpBits[chunk][0] = new BitmapBits(chunksz, chunksz);
            ChunkBmpBits[chunk][1] = new BitmapBits(chunksz, chunksz);
            ChunkColBmpBits[chunk][0] = new BitmapBits(chunksz, chunksz);
            ChunkColBmpBits[chunk][1] = new BitmapBits(chunksz, chunksz);
            CompChunkBmpBits[chunk] = new BitmapBits(chunksz, chunksz);
            for (int by = 0; by < chunksz / 16; by++)
            {
                for (int bx = 0; bx < chunksz / 16; bx++)
                {
                    ChunkBlock blk = Chunks[chunk].blocks[bx, by];
                    if (blk.Block < BlockBmpBits.Count)
                    {
                        BitmapBits bmp = new BitmapBits(BlockBmpBits[blk.Block][0]);
                        bmp.Flip(blk.XFlip, blk.YFlip);
                        ChunkBmpBits[chunk][0].DrawBitmap(
                            bmp,
                            new Point(bx * 16, by * 16));
                        bmp = new BitmapBits(BlockBmps[blk.Block][1]);
                        bmp.Flip(blk.XFlip, blk.YFlip);
                        ChunkBmpBits[chunk][1].DrawBitmap(
                            bmp,
                            new Point(bx * 16, by * 16));
                        if (ColInds1.Count > 0)
                        {
                            bmp = new BitmapBits(ColBmpBits[ColInds1[blk.Block]]);
                            bmp.IncrementIndexes((int)blk.Solid1 - 1);
                            bmp.Flip(blk.XFlip, blk.YFlip);
                            ChunkColBmpBits[chunk][0].DrawBitmap(bmp, new Point(bx * 16, by * 16));
                            if (blk is S2ChunkBlock)
                            {
                                bmp = new BitmapBits(ColBmpBits[ColInds2[blk.Block]]);
                                bmp.IncrementIndexes((int)((S2ChunkBlock)blk).Solid2 - 1);
                                bmp.Flip(blk.XFlip, blk.YFlip);
                                ChunkColBmpBits[chunk][1].DrawBitmap(bmp, new Point(bx * 16, by * 16));
                            }
                        }
                    }
                }
            }
            CompChunkBmpBits[chunk].DrawBitmapComposited(ChunkBmpBits[chunk][0], Point.Empty);
            CompChunkBmpBits[chunk].DrawBitmapComposited(ChunkBmpBits[chunk][1], Point.Empty);
            ChunkBmps[chunk][0] = ChunkBmpBits[chunk][0].ToBitmap(BmpPal);
            ChunkBmps[chunk][1] = ChunkBmpBits[chunk][1].ToBitmap(BmpPal);
            ChunkColBmps[chunk][0] = ChunkColBmpBits[chunk][0].ToBitmap(Color.Transparent, Color.White, Color.Yellow, Color.Black);
            ChunkColBmps[chunk][1] = ChunkColBmpBits[chunk][1].ToBitmap(Color.Transparent, Color.White, Color.Yellow, Color.Black);
            ChunkColBmpBits[chunk][0].IncrementIndexes(63);
            ChunkColBmpBits[chunk][1].IncrementIndexes(63);
            CompChunkBmps[chunk] = CompChunkBmpBits[chunk].ToBitmap(BmpPal);
        }

        public static ObjectDefinition GetObjectDefinition(byte ID)
        {
            if (ObjTypes != null && ObjTypes.ContainsKey(ID))
                return ObjTypes[ID];
            else
                return unkobj;
        }

        public static ObjectEntry ChangeObjectType(ObjectEntry entry)
        {
            if (ObjTypes == null) return entry;
            Type t = GetObjectDefinition(entry.ID).ObjectType;
            if (entry.GetType() == t) return entry;
            byte[] entb = entry.GetBytes();
            ObjectEntry oe = (ObjectEntry)Activator.CreateInstance(t, new object[] { entb, 0 });
            int i = Objects.IndexOf(entry);
            if (i > -1)
                Objects[i] = oe;
            ObjectTypeChangedEvent(entry, oe);
            return oe;
        }

        public static ObjectEntry GetBaseObjectType(ObjectEntry entry)
        {
            byte[] entb = entry.GetBytes();
            if (entry is S1ObjectEntry)
                return new S1ObjectEntry(entb, 0);
            else if (entry is S2ObjectEntry)
                return new S2ObjectEntry(entb, 0);
            else if (entry is S3KObjectEntry)
                return new S3KObjectEntry(entb, 0);
            else if (entry is SCDObjectEntry)
                return new SCDObjectEntry(entb, 0);
            else
                return entry;
        }

        public static ObjectEntry CreateObject(byte ID)
        {
            Type t = GetObjectDefinition(ID).ObjectType;
            ObjectEntry oe = (ObjectEntry)Activator.CreateInstance(t, new object[] { });
            oe.ID = ID;
            return oe;
        }

        public static void PaletteChanged()
        {
            for (int i = 0; i < 64; i++)
                BmpPal.Entries[i] = PaletteToColor(i / 16, i % 16, true);
            foreach (Bitmap[] item in BlockBmps)
            {
                item[0].Palette = BmpPal;
                item[1].Palette = BmpPal;
            }
            foreach (Bitmap[] item in ChunkBmps)
            {
                item[0].Palette = BmpPal;
                item[1].Palette = BmpPal;
            }
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
                    for (int k = 0; k < 8; k++)
                        for (int j = 0; j < 8; j++)
                            if (ColInds1[Chunks[i].blocks[j, k].Block] == block | ColInds2[Chunks[i].blocks[j, k].Block] == block)
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

        public static byte[] BmpToTile(Bitmap bmp, out int palette)
        {
            switch (bmp.PixelFormat)
            {
                case PixelFormat.Format1bppIndexed:
                case PixelFormat.Format32bppArgb:
                case PixelFormat.Format4bppIndexed:
                case PixelFormat.Format8bppIndexed:
                    break;
                default:
                    bmp = bmp.To32bpp();
                    break;
            }
            BitmapBits bmpbits = new BitmapBits(8, 8);
            BitmapData bmpd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
            int stride = bmpd.Stride;
            byte[] Bits = new byte[Math.Abs(stride) * bmpd.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmpd.Scan0, Bits, 0, Bits.Length);
            bmp.UnlockBits(bmpd);
            switch (bmpd.PixelFormat)
            {
                case PixelFormat.Format1bppIndexed:
                    LoadBitmap1BppIndexed(bmpbits, Bits, stride);
                    palette = 0;
                    return bmpbits.ToTile();
                case PixelFormat.Format32bppArgb:
                    LoadBitmap32BppArgb(bmpbits, Bits, stride, BmpPal.Entries);
                    break;
                case PixelFormat.Format4bppIndexed:
                    LoadBitmap4BppIndexed(bmpbits, Bits, stride);
                    palette = 0;
                    return bmpbits.ToTile();
                case PixelFormat.Format8bppIndexed:
                    LoadBitmap8BppIndexed(bmpbits, Bits, stride);
                    break;
            }
            int[] palcnt = new int[4];
            for (int y = 0; y < 8; y++)
                for (int x = 0; x < 8; x++)
                    if ((bmpbits[x, y] & 15) > 0)
                        palcnt[bmpbits[x, y] / 16]++;
            palette = 0;
            if (palcnt[1] > palcnt[palette])
                palette = 1;
            if (palcnt[2] > palcnt[palette])
                palette = 2;
            if (palcnt[3] > palcnt[palette])
                palette = 3;
            Color[] newpal = new Color[16];
            for (int i = 0; i < 16; i++)
                newpal[i] = BmpPal.Entries[(palette * 16) + i];
            switch (bmpd.PixelFormat)
            {
                case PixelFormat.Format32bppArgb:
                    LoadBitmap32BppArgb(bmpbits, Bits, stride, newpal);
                    break;
                case PixelFormat.Format8bppIndexed:
                    for (int y = 0; y < 8; y++)
                        for (int x = 0; x < 8; x++)
                            bmpbits[x, y] = (byte)(bmpbits[x, y] & 15);
                            break;
            }
            return bmpbits.ToTile();
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

        public static void LoadBitmap4BppIndexed(BitmapBits bmp, byte[] Bits, int Stride)
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
        SBoom
    }

    public enum TimeZone
    {
        None,
        Present,
        Past,
        Future
    }
}