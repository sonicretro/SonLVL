using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace SonicRetro.SonLVL
{
    internal static class LevelData
    {
        internal static MainForm MainForm;
        internal static MultiFileIndexer<byte> Tiles;
        internal static byte[] TilesArray;
        internal static EngineVersion TileFmt;
        internal static MultiFileIndexer<Block> Blocks;
        internal static List<BitmapBits[]> BlockBmpBits;
        internal static List<Bitmap[]> BlockBmps;
        internal static EngineVersion BlockFmt;
        internal static MultiFileIndexer<Chunk> Chunks;
        internal static List<BitmapBits[]> ChunkBmpBits;
        internal static List<Bitmap[]> ChunkBmps;
        internal static EngineVersion ChunkFmt;
        internal static byte[,] FGLayout;
        internal static bool[,] FGLoop;
        internal static byte[,] BGLayout;
        internal static bool[,] BGLoop;
        internal static EngineVersion LayoutFmt;
        internal static List<string> PalName;
        internal static List<ushort[,]> Palette;
        internal static List<byte[,]> PalNum;
        internal static List<int[,]> PalAddr;
        internal static int CurPal;
        internal static ColorPalette BmpPal;
        internal static EngineVersion PaletteFmt;
        internal static List<ObjectEntry> Objects;
        internal static EngineVersion ObjectFmt;
        internal static List<RingEntry> Rings;
        internal static EngineVersion RingFmt;
        internal static List<CNZBumperEntry> Bumpers;
        internal static List<StartPositionEntry> StartPositions;
        internal static Dictionary<byte, ObjectDefinition> ObjTypes;
        internal static ObjectDefinition unkobj;
        internal static S2RingDefinition S2RingDef;
        internal static S3KRingDefinition S3KRingDef;
        internal static List<StartPositionDefinition> StartPosDefs;
        internal static EngineVersion EngineVersion;
        internal static int chunksz;
        internal static bool littleendian;
        internal static Dictionary<string, byte[]> filecache;
        internal static Rectangle? LevelBounds;
        internal static List<byte> ColInds1;
        internal static List<byte> ColInds2;
        internal static sbyte[][] ColArr1;
        internal static byte[] Angles;
        internal static BitmapBits[] ColBmpBits;
        internal static Bitmap[] ColBmps;
        internal static List<BitmapBits[]> ChunkColBmpBits;
        internal static List<Bitmap[]> ChunkColBmps;
        internal static Bitmap UnknownImg;

        internal static byte[] ReadFile(string file, Compression.CompressionType cmp)
        {
            if (file == "LevelArt")
                return TilesArray;
            else if (filecache.ContainsKey(file))
                return filecache[file];
            else
            {
                byte[] val = Compression.Decompress(file, cmp);
                filecache.Add(file, val);
                return val;
            }
        }

        internal static Bitmap TileToBmp4bpp(byte[] file, int index, int palette)
        {
            Bitmap bmp = new Bitmap(8, 8, PixelFormat.Format4bppIndexed);
            if (index * 32 + 32 <= file.Length)
            {
                BitmapData bmpd = bmp.LockBits(new Rectangle(0, 0, 8, 8), ImageLockMode.WriteOnly, PixelFormat.Format4bppIndexed);
                System.Runtime.InteropServices.Marshal.Copy(file, index * 32, bmpd.Scan0, 32);
                bmp.UnlockBits(bmpd);
            }
            ColorPalette pal = bmp.Palette;
            for (int i = 0; i < 16; i++)
            {
                pal.Entries[i] = PaletteToColor(palette, i, true);
            }
            bmp.Palette = pal;
            return bmp;
        }

        internal static BitmapBits TileToBmp8bpp(byte[] file, int index, int pal)
        {
            BitmapBits bmp = new BitmapBits(8, 8);
            if (index * 32 + 32 <= file.Length)
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

        internal static byte[] ASMToBin(string file)
        {
            if (!System.IO.Directory.Exists("mapcache"))
            {
                System.IO.DirectoryInfo dir = System.IO.Directory.CreateDirectory("mapcache");
                dir.Attributes |= System.IO.FileAttributes.Hidden;
            }
            string outfile = "mapcache/" + System.IO.Path.ChangeExtension(file, "bin").Replace('/', '!');
            DateTime modDate = DateTime.MinValue;
            if (System.IO.File.Exists(outfile))
                modDate = System.IO.File.GetLastWriteTime(outfile);
            if (modDate >= System.IO.File.GetLastWriteTime(file))
                return System.IO.File.ReadAllBytes(outfile);
            System.Diagnostics.Process proc = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "asm68k.exe"), "/k /p /o ae- \"" + file + "\", \"" + outfile + "\"") { UseShellExecute = false, CreateNoWindow = true });
            proc.WaitForExit();
            return System.IO.File.ReadAllBytes(outfile);
        }

        internal static byte[] ASMToBin(string file, string label)
        {
            string[] fc = System.IO.File.ReadAllLines(file);
            int st = -1;
            for (int i = 0; i < fc.Length; i++)
            {
                if (fc[i].StartsWith(label + ":"))
                {
                    st = i;
                    fc[i] = fc[i].Substring(label.Length + 1);
                }
            }
            if (st == -1) return new byte[0];
            List<byte> result = new List<byte>();
            for (; st < fc.Length; st++)
            {
                int ts = Math.Max(fc[st].IndexOf(' '), fc[st].IndexOf('\t'));
                string[] ln = fc[st].Trim().Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (ln.Length == 0) continue;
                if (!ln[0].StartsWith("dc.")) break;
                string d = string.Empty;
                for (int i = 1; i < ln.Length; i++)
                {
                    d += ln[i];
                }
                if (d.IndexOf(';') > -1)
                    d = d.Remove(d.IndexOf(';'));
                string[] dats = d.Split(',');
                switch (ln[0].Split('.')[1])
                {
                    case "b":
                        foreach (string item in dats)
                        {
                            result.Add(ParseASMByte(item));
                        }
                        break;
                    case "w":
                        foreach (string item in dats)
                        {
                            result.AddRange(ByteConverter.GetBytes(ParseASMWord(item)));
                        }
                        break;
                    case "l":
                        foreach (string item in dats)
                        {
                            result.AddRange(ByteConverter.GetBytes(ParseASMLong(item)));
                        }
                        break;
                }
            }
            return result.ToArray();
        }

        internal static byte ParseASMByte(string data)
        {
            if (data.StartsWith("$"))
                return byte.Parse(data.Substring(1), System.Globalization.NumberStyles.HexNumber);
            else
                return byte.Parse(data, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        internal static ushort ParseASMWord(string data)
        {
            if (data.StartsWith("$"))
                return ushort.Parse(data.Substring(1), System.Globalization.NumberStyles.HexNumber);
            else
                return ushort.Parse(data, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        internal static uint ParseASMLong(string data)
        {
            if (data.StartsWith("$"))
                return uint.Parse(data.Substring(1), System.Globalization.NumberStyles.HexNumber);
            else
                return uint.Parse(data, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        internal static byte[] ProcessDPLC(byte[] artfile, DPLC dplc)
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

        internal static BitmapBits[] S2MapFrameToBmp(byte[] file, S2Mappings map, int startpal, out Point offset)
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
            offset = new Point(left, top);
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
                            TileToBmp8bpp(file, map[i].Tile.Tile + ti, (map[i].Tile.Palette + startpal) & 3),
                            new Point(x * 8, y * 8));
                        ti++;
                    }
                }
                pcbmp.Flip(map[i].Tile.XFlip, map[i].Tile.YFlip);
                bmp[pr].DrawBitmapComposited(pcbmp, new Point(map[i].X - left, map[i].Y - top));
            }
            return bmp;
        }

        internal static BitmapBits[] S1MapFrameToBmp(byte[] file, S1Mappings map, int startpal, out Point offset)
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
            offset = new Point(left, top);
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
                            TileToBmp8bpp(file, map[i].Tile.Tile + ti, (map[i].Tile.Palette + startpal) & 3),
                            new Point(x * 8, y * 8));
                        ti++;
                    }
                }
                pcbmp.Flip(map[i].Tile.XFlip, map[i].Tile.YFlip);
                bmp[pr].DrawBitmapComposited(pcbmp, new Point(map[i].X - left, map[i].Y - top));
            }
            return bmp;
        }

        internal static BitmapBits[] S3KMapFrameToBmp(byte[] file, S3KMappings map, int startpal, out Point offset)
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
            offset = new Point(left, top);
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
                            TileToBmp8bpp(file, map[i].Tile.Tile + ti, (map[i].Tile.Palette + startpal) & 3),
                            new Point(x * 8, y * 8));
                        ti++;
                    }
                }
                pcbmp.Flip(map[i].Tile.XFlip, map[i].Tile.YFlip);
                bmp[pr].DrawBitmapComposited(pcbmp, new Point(map[i].X - left, map[i].Y - top));
            }
            return bmp;
        }

        internal static BitmapBits[] S2MapFrameDPLCToBmp(byte[] file, S2Mappings map, DPLC dplc, int startpal, out Point offset)
        {
            byte[] art = ProcessDPLC(file, dplc);
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
            offset = new Point(left, top);
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
            return bmp;
        }

        internal static BitmapBits[] S3KMapFrameDPLCToBmp(byte[] file, S3KMappings map, DPLC dplc, int startpal, out Point offset)
        {
            byte[] art = ProcessDPLC(file, dplc);
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
            offset = new Point(left, top);
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
            return bmp;
        }

        internal static Color PaletteToColor(int line, int index, bool acceptTransparent)
        {
            if (acceptTransparent && index == 0)
                return Color.Transparent;
            return Color.FromArgb(
                (Palette[CurPal][line, index] & 0xF) * 0x11,
                ((Palette[CurPal][line, index] & 0xF0) >> 4) * 0x11,
                ((Palette[CurPal][line, index] & 0xF00) >> 8) * 0x11
                );
        }

        internal static void ColorToPalette(int line, int index, Color color)
        {
            Palette[CurPal][line, index] = (ushort)(((color.R / 0x11) & 0xE) | (((color.G / 0x11) << 4) & 0xE0) | (((color.B / 0x11) << 8) & 0xE00));
        }

        internal static void RedrawBlock(int block, bool drawChunks)
        {
            BlockBmpBits[block][0] = new BitmapBits(16, 16);
            BlockBmpBits[block][1] = new BitmapBits(16, 16);
            for (int by = 0; by < 2; by++)
            {
                for (int bx = 0; bx < 2; bx++)
                {
                    PatternIndex pt = Blocks[block].tiles[bx, by];
                    int pr = pt.Priority ? 1 : 0;
                    BitmapBits tile = TileToBmp8bpp(TilesArray, pt.Tile, pt.Palette);
                    tile.Flip(pt.XFlip, pt.YFlip);
                    BlockBmpBits[block][pr].DrawBitmap(
                        tile,
                        new Point(bx * 8, by * 8)
                        );
                }
            }
            BlockBmps[block][0] = BlockBmpBits[block][0].ToBitmap(BmpPal);
            BlockBmps[block][1] = BlockBmpBits[block][1].ToBitmap(BmpPal);
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

        internal static void RedrawChunk(int chunk)
        {
            ChunkBmpBits[chunk][0] = new BitmapBits(chunksz, chunksz);
            ChunkBmpBits[chunk][1] = new BitmapBits(chunksz, chunksz);
            ChunkColBmpBits[chunk][0] = new BitmapBits(chunksz, chunksz);
            ChunkColBmpBits[chunk][1] = new BitmapBits(chunksz, chunksz);
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
            ChunkBmps[chunk][0] = ChunkBmpBits[chunk][0].ToBitmap(BmpPal);
            ChunkBmps[chunk][1] = ChunkBmpBits[chunk][1].ToBitmap(BmpPal);
            ChunkColBmps[chunk][0] = ChunkColBmpBits[chunk][0].ToBitmap(Color.Transparent, Color.White, Color.Yellow, Color.Black);
            ChunkColBmps[chunk][1] = ChunkColBmpBits[chunk][1].ToBitmap(Color.Transparent, Color.White, Color.Yellow, Color.Black);
            ChunkColBmpBits[chunk][0].IncrementIndexes(64);
            ChunkColBmpBits[chunk][1].IncrementIndexes(64);
        }

        internal static ObjectDefinition getObjectDefinition(byte ID)
        {
            if (ObjTypes.ContainsKey(ID))
                return ObjTypes[ID];
            else
                return unkobj;
        }

        internal static string getFullObjectName(byte ID, byte SubType)
        {
            string ret = unkobj.Name();
            if (ObjTypes.ContainsKey(ID))
                ret = ObjTypes[ID].FullName(SubType);
            return ret;
        }

        internal static void ChangeObjectType(ObjectEntry entry)
        {
            if (Objects.IndexOf(entry) == -1) return;
            Type t = getObjectDefinition(entry.ID).ObjectType;
            if (entry.GetType() == t) return;
            byte[] entb = entry.GetBytes();
            ObjectEntry oe = (ObjectEntry)Activator.CreateInstance(t, new object[] { entb, 0 });
            int i = Objects.IndexOf(entry);
            Objects[i] = oe;
            if (MainForm.SelectedItems != null)
            {
                i = MainForm.SelectedItems.IndexOf(entry);
                if (i > -1)
                {
                    MainForm.SelectedItems[i] = oe;
                    MainForm.EditControls.propertyGrid1.SelectedObjects = MainForm.SelectedItems.ToArray();
                }
            }
            if (MainForm.loaded)
                MainForm.AddUndo(new ObjectIDChangedUndoAction(entry, oe));
        }

        internal static ObjectEntry GetBaseObjectType(ObjectEntry entry)
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

        internal static ObjectEntry CreateObject(byte ID)
        {
            Type t = getObjectDefinition(ID).ObjectType;
            ObjectEntry oe = (ObjectEntry)Activator.CreateInstance(t, new object[] { });
            oe.ID = ID;
            return oe;
        }

        internal static void PaletteChanged()
        {
            for (int i = 0; i < 64; i++)
                BmpPal.Entries[i] = PaletteToColor(i / 16, i % 16, true);
            for (int i = 0; i < 64; i++)
                MainForm.LevelImgPalette.Entries[i] = PaletteToColor(i / 16, i % 16, true);
            MainForm.LevelImgPalette.Entries[0] = PaletteToColor(2, 0, false);
            MainForm.LevelImgPalette.Entries[64] = Color.White;
            MainForm.LevelImgPalette.Entries[65] = Color.Yellow;
            MainForm.LevelImgPalette.Entries[66] = Color.Black;
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
            MainForm.EditControls.ChunkSelector.BackColor = PaletteToColor(2, 0, false);
            MainForm.DrawLevel();
        }

        internal static void RedrawCol(int block, bool drawChunks)
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

        internal static sbyte[][] GenerateRotatedCollision()
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
            BitmapBits bmpbits = new BitmapBits(8, 8);
            BitmapData bmpd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
            byte[] Bits = new byte[Math.Abs(bmpd.Stride) * bmpd.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmpd.Scan0, Bits, 0, Bits.Length);
            bmp.UnlockBits(bmpd);
            switch (bmpd.PixelFormat)
            {
                case PixelFormat.Format16bppArgb1555:
                    LoadBitmap16BppArgb1555(bmpbits, Bits, bmpd.Stride);
                    break;
                case PixelFormat.Format16bppGrayScale:
                    LoadBitmap16BppGrayScale(bmpbits, Bits, bmpd.Stride);
                    break;
                case PixelFormat.Format16bppRgb555:
                    LoadBitmap16BppRgb555(bmpbits, Bits, bmpd.Stride);
                    break;
                case PixelFormat.Format16bppRgb565:
                    LoadBitmap16BppRgb565(bmpbits, Bits, bmpd.Stride);
                    break;
                case PixelFormat.Format1bppIndexed:
                    LoadBitmap1BppIndexed(bmpbits, Bits, bmpd.Stride);
                    palette = 0;
                    return bmpbits.ToTile();
                case PixelFormat.Format24bppRgb:
                    LoadBitmap24BppRgb(bmpbits, Bits, bmpd.Stride);
                    break;
                case PixelFormat.Format32bppArgb:
                    LoadBitmap32BppArgb(bmpbits, Bits, bmpd.Stride);
                    break;
                case PixelFormat.Format32bppPArgb:
                    throw new NotImplementedException();
                case PixelFormat.Format32bppRgb:
                    LoadBitmap32BppRgb(bmpbits, Bits, bmpd.Stride);
                    break;
                case PixelFormat.Format48bppRgb:
                    LoadBitmap48BppRgb(bmpbits, Bits, bmpd.Stride);
                    break;
                case PixelFormat.Format4bppIndexed:
                    LoadBitmap4BppIndexed(bmpbits, Bits, bmpd.Stride);
                    palette = 0;
                    return bmpbits.ToTile();
                case PixelFormat.Format64bppArgb:
                    LoadBitmap64BppArgb(bmpbits, Bits, bmpd.Stride);
                    break;
                case PixelFormat.Format64bppPArgb:
                    throw new NotImplementedException();
                case PixelFormat.Format8bppIndexed:
                    LoadBitmap8BppIndexed(bmpbits, Bits, bmpd.Stride);
                    break;
            }
            int[] palcnt = new int[4];
            for (int y = 0; y < 8; y++)
                for (int x = 0; x < 8; x++)
                {
                    palcnt[bmpbits[x, y] / 16]++;
                    bmpbits[x, y] &= 15;
                }
            palette = 0;
            if (palcnt[1] > palcnt[palette])
                palette = 1;
            if (palcnt[2] > palcnt[palette])
                palette = 2;
            if (palcnt[3] > palcnt[palette])
                palette = 3;
            return bmpbits.ToTile();
        }

        private static void LoadBitmap16BppArgb1555(BitmapBits bmp, byte[] Bits, int Stride)
        {
            for (int y = 0; y < bmp.Height; y++)
            {
                int srcaddr = y * Math.Abs(Stride);
                for (int x = 0; x < bmp.Width; x++)
                {
                    ushort pix = BitConverter.ToUInt16(Bits, srcaddr + (x * 2));
                    int A = (pix >> 15) * 255;
                    int R = (pix >> 10) & 0x1F;
                    R = R << 3 | R >> 2;
                    int G = (pix >> 5) & 0x1F;
                    G = G << 3 | G >> 2;
                    int B = pix & 0x1F;
                    B = B << 3 | B >> 2;
                    bmp[x, y] = (byte)Array.IndexOf(BmpPal.Entries, Color.FromArgb(A, R, G, B).FindNearestMatch(BmpPal.Entries));
                    if (A < 128)
                        bmp[x, y] = 0;
                }
            }
        }

        private static void LoadBitmap16BppGrayScale(BitmapBits bmp, byte[] Bits, int Stride)
        {
            for (int y = 0; y < bmp.Height; y++)
            {
                int srcaddr = y * Math.Abs(Stride);
                for (int x = 0; x < bmp.Width; x++)
                {
                    ushort pix = BitConverter.ToUInt16(Bits, srcaddr + (x * 2));
                    bmp[x, y] = (byte)Array.IndexOf(BmpPal.Entries, Color.FromArgb(pix >> 8, pix >> 8, pix >> 8).FindNearestMatch(BmpPal.Entries));
                }
            }
        }

        private static void LoadBitmap16BppRgb555(BitmapBits bmp, byte[] Bits, int Stride)
        {
            for (int y = 0; y < bmp.Height; y++)
            {
                int srcaddr = y * Math.Abs(Stride);
                for (int x = 0; x < bmp.Width; x++)
                {
                    ushort pix = BitConverter.ToUInt16(Bits, srcaddr + (x * 2));
                    int R = (pix >> 10) & 0x1F;
                    R = R << 3 | R >> 2;
                    int G = (pix >> 5) & 0x1F;
                    G = G << 3 | G >> 2;
                    int B = pix & 0x1F;
                    B = B << 3 | B >> 2;
                    bmp[x, y] = (byte)Array.IndexOf(BmpPal.Entries, Color.FromArgb(R, G, B).FindNearestMatch(BmpPal.Entries));
                }
            }
        }

        private static void LoadBitmap16BppRgb565(BitmapBits bmp, byte[] Bits, int Stride)
        {
            for (int y = 0; y < bmp.Height; y++)
            {
                int srcaddr = y * Math.Abs(Stride);
                for (int x = 0; x < bmp.Width; x++)
                {
                    ushort pix = BitConverter.ToUInt16(Bits, srcaddr + (x * 2));
                    int R = (pix >> 11) & 0x1F;
                    R = R << 3 | R >> 2;
                    int G = (pix >> 5) & 0x3F;
                    G = G << 2 | G >> 4;
                    int B = pix & 0x1F;
                    B = B << 3 | B >> 2;
                    bmp[x, y] = (byte)Array.IndexOf(BmpPal.Entries, Color.FromArgb(R, G, B).FindNearestMatch(BmpPal.Entries));
                }
            }
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

        private static void LoadBitmap24BppRgb(BitmapBits bmp, byte[] Bits, int Stride)
        {
            for (int y = 0; y < bmp.Height; y++)
            {
                int srcaddr = y * Math.Abs(Stride);
                for (int x = 0; x < bmp.Width; x++)
                    bmp[x, y] = (byte)Array.IndexOf(BmpPal.Entries, Color.FromArgb(Bits[srcaddr + (x * 3) + 2], Bits[srcaddr + (x * 3) + 1], Bits[srcaddr + (x * 3)]).FindNearestMatch(BmpPal.Entries));
            }
        }

        private static void LoadBitmap32BppArgb(BitmapBits bmp, byte[] Bits, int Stride)
        {
            for (int y = 0; y < bmp.Height; y++)
            {
                int srcaddr = y * Math.Abs(Stride);
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color col = Color.FromArgb(BitConverter.ToInt32(Bits, srcaddr + (x * 4)));
                    bmp[x, y] = (byte)Array.IndexOf(BmpPal.Entries, col.FindNearestMatch(BmpPal.Entries));
                }
            }
        }

        private static void LoadBitmap32BppRgb(BitmapBits bmp, byte[] Bits, int Stride)
        {
            for (int y = 0; y < bmp.Height; y++)
            {
                int srcaddr = y * Math.Abs(Stride);
                for (int x = 0; x < bmp.Width; x++)
                    bmp[x, y] = (byte)Array.IndexOf(BmpPal.Entries, Color.FromArgb(Bits[srcaddr + (x * 4) + 2], Bits[srcaddr + (x * 4) + 1], Bits[srcaddr + (x * 4)]).FindNearestMatch(BmpPal.Entries));
            }
        }

        private static void LoadBitmap48BppRgb(BitmapBits bmp, byte[] Bits, int Stride)
        {
            for (int y = 0; y < bmp.Height; y++)
            {
                int srcaddr = y * Math.Abs(Stride);
                for (int x = 0; x < bmp.Width; x++)
                    bmp[x, y] = (byte)Array.IndexOf(BmpPal.Entries, Color.FromArgb(BitConverter.ToUInt16(Bits, srcaddr + (x * 6) + 4) / 255, BitConverter.ToUInt16(Bits, srcaddr + (x * 6) + 2) / 255, BitConverter.ToUInt16(Bits, srcaddr + (x * 6)) / 255).FindNearestMatch(BmpPal.Entries));
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

        private static void LoadBitmap64BppArgb(BitmapBits bmp, byte[] Bits, int Stride)
        {
            for (int y = 0; y < bmp.Height; y++)
            {
                int srcaddr = y * Math.Abs(Stride);
                for (int x = 0; x < bmp.Width; x++)
                    bmp[x, y] = (byte)Array.IndexOf(BmpPal.Entries, Color.FromArgb(BitConverter.ToUInt16(Bits, srcaddr + (x * 8) + 6) / 255, BitConverter.ToUInt16(Bits, srcaddr + (x * 8) + 4) / 255, BitConverter.ToUInt16(Bits, srcaddr + (x * 8) + 2) / 255, BitConverter.ToUInt16(Bits, srcaddr + (x * 8)) / 255).FindNearestMatch(BmpPal.Entries));
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
    }

    public enum EngineVersion
    {
        Invalid,
        S1,
        S2,
        S3K,
        SCD,
        SCDPC,
        SKC
    }
}