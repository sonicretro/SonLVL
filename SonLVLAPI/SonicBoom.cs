using System;
using System.Collections.Generic;
using System.IO;

namespace SonicRetro.SonLVL.API
{
    public static class SonicBoom
    {
        const int FRONT_ROW = 0;
        const int BACK_ROW = 1;
        public const int MAX_ROW_LENGTH = 127;
        public const int MAX_NUMBER_OF_ROWS = 64;

        const int FORMAT_AST = 0;
        const int FORMAT_BALE = 1;
        const int FORMAT_WOLE = 2;
        const int FORMAT_ZLE = 3;
        const int FORMAT_BALE_4BIT = FORMAT_BALE + 16;
        const int FORMAT_WOLE_4BIT = FORMAT_WOLE + 16;
        const int FORMAT_ZLE_4BIT = FORMAT_ZLE + 16;

        private class ROW
        {
            public int format;
            public int attribute;
            public byte[] data = new byte[MAX_ROW_LENGTH];
            public int dataLength;
        }

        private class LEVEL
        {
            public int[] rowLength = new int[2];
            public int[] rowCount = new int[2];
            public ROW[][] row = new ROW[2][];

            public LEVEL()
            {
                for (int i = 0; i < 2; i++)
                {
                    row[i] = new ROW[MAX_NUMBER_OF_ROWS];
                    for (int j = 0; j < MAX_NUMBER_OF_ROWS; j++)
                        row[i][j] = new ROW();
                }
            }
        }

        static LEVEL level;
        static ROW[] testRow = { new ROW(), new ROW(), new ROW(), new ROW() };

        static ushort SwapEndianShort(ushort v)
        {
            return (ushort)unchecked((ushort)(v >> 8) | (ushort)(v << 8));
        }

        public static void ReadLayout()
        {
            if (!File.Exists(LevelData.Level.Layout))
            {
                LevelData.Log("Layout file \"" + LevelData.Level.Layout + "\" not found.");
                LevelData.FGLayout = new byte[MAX_ROW_LENGTH, MAX_NUMBER_OF_ROWS];
                LevelData.BGLayout = new byte[MAX_ROW_LENGTH, MAX_NUMBER_OF_ROWS];
                return;
            }
            LevelData.Log("Loading layout from file \"" + LevelData.Level.Layout + "\", using compression " + LevelData.Level.LayoutCompression.ToString() + "...");
            MemoryStream ms = new MemoryStream(Compression.Decompress(LevelData.Level.Layout, LevelData.Level.LayoutCompression));
            BinaryReader br = new BinaryReader(ms);
            byte fgrows = br.ReadByte();
            byte bgrows = br.ReadByte();
            LevelData.FGLayout = new byte[MAX_ROW_LENGTH, fgrows];
            LevelData.BGLayout = new byte[MAX_ROW_LENGTH, bgrows];
            ushort[] fgpointers = new ushort[fgrows];
            ushort[] fgcolpointers = new ushort[fgrows];
            for (int i = 0; i < fgrows; i++)
            {
                fgpointers[i] = SwapEndianShort(br.ReadUInt16());
                fgcolpointers[i] = SwapEndianShort(br.ReadUInt16());
            }
            ushort[] bgpointers = new ushort[bgrows];
            for (int i = 0; i < bgrows; i++)
                bgpointers[i] = SwapEndianShort(br.ReadUInt16());
            for (int plane = 0; plane < 2; plane++)
            {
                int rows = fgrows;
                ushort[] pointers = fgpointers;
                byte[,] layout = LevelData.FGLayout;
                if (plane == 1)
                {
                    rows = bgrows;
                    pointers = bgpointers;
                    layout = LevelData.BGLayout;
                }
                for (int r = 0; r < rows; r++)
                {
                    int format = pointers[r] >> 11;
                    int pointer = pointers[r] & 0x7FF;
                    byte[] rowData;
                    byte[] blockList = new byte[16];
                    switch (format)
                    {
                        case FORMAT_BALE:
                            pointer <<= 1;
                            ms.Seek(pointer, SeekOrigin.Begin);
                            byte mask = br.ReadByte();
                            rowData = br.ReadBytes(mask + 1);
                            for (int c = 0; c < MAX_ROW_LENGTH; c++)
                                layout[c, r] = rowData[c & mask];
                            break;
                        case FORMAT_BALE_4BIT:
                            pointer <<= 1;
                            ms.Seek(pointer, SeekOrigin.Begin);
                            mask = br.ReadByte();
                            rowData = br.ReadNybbles(mask + 1);
                            ms.Read(blockList, 0, 16);
                            for (int c = 0; c < MAX_ROW_LENGTH; c++)
                            {
                                byte blockRef = rowData[(c / 2) & mask];
                                if (c % 2 == 0) blockRef >>= 4;
                                blockRef &= 0xF;
                                layout[c, r] = blockList[blockRef];
                            }
                            break;
                        case FORMAT_WOLE:
                            pointer <<= 1;
                            ms.Seek(pointer, SeekOrigin.Begin);
                            byte width = br.ReadByte();
                            rowData = br.ReadBytes(width);
                            for (int c = 0; c < MAX_ROW_LENGTH; c++)
                                layout[c, r] = rowData[c % width];
                            break;
                        case FORMAT_WOLE_4BIT:
                            pointer <<= 1;
                            ms.Seek(pointer, SeekOrigin.Begin);
                            width = br.ReadByte();
                            rowData = br.ReadNybbles(width);
                            ms.Read(blockList, 0, 16);
                            for (int c = 0; c < MAX_ROW_LENGTH; c++)
                            {
                                byte blockRef = rowData[(c % width) / 2];
                                if (c % 2 == 0) blockRef >>= 4;
                                blockRef &= 0xF;
                                layout[c, r] = blockList[blockRef];
                            }
                            break;
                        case FORMAT_ZLE:
                            pointer <<= 1;
                            ms.Seek(pointer, SeekOrigin.Begin);
                            width = br.ReadByte();
                            rowData = br.ReadBytes(width);
                            for (int c = 0; c < Math.Min(MAX_ROW_LENGTH, (int)width); c++)
                                layout[c, r] = rowData[c];
                            break;
                        case FORMAT_ZLE_4BIT:
                            pointer <<= 1;
                            ms.Seek(pointer, SeekOrigin.Begin);
                            width = br.ReadByte();
                            rowData = br.ReadNybbles(width);
                            ms.Read(blockList, 0, 16);
                            for (int c = 0; c < Math.Min(MAX_ROW_LENGTH, (int)width); c++)
                            {
                                byte blockRef = rowData[c / 2];
                                if (c % 2 == 0) blockRef >>= 4;
                                blockRef &= 0xF;
                                layout[c, r] = blockList[blockRef];
                            }
                            break;
                        default:
                            for (int c = 0; c < MAX_ROW_LENGTH; c++)
                                layout[c, r] = (byte)(pointer & 0xFF);
                            break;
                    }
                }
            }
        }

        public static void WriteLayout()
        {
            // ----------------------------------------
            // Initialize data
            // ----------------------------------------
            level = new LEVEL();
            level.rowCount[FRONT_ROW] = LevelData.FGLayout.GetLength(1);
            level.rowCount[BACK_ROW] = LevelData.BGLayout.GetLength(1);

            for (int r = 0; r < MAX_NUMBER_OF_ROWS; r++)
            {
                level.row[FRONT_ROW][r].format = -1;
                level.row[BACK_ROW][r].format = -1;
            }

            // ----------------------------------------
            // Get row length information
            // ----------------------------------------

            for (int r = 0; r < LevelData.FGLayout.GetLength(1); r++)
            {
                for (int i = LevelData.FGLayout.GetLength(0) - 1; i >= 0; i--)
                {
                    if (LevelData.FGLayout[i, r] != 0 && i + 1 > level.rowLength[FRONT_ROW])
                    {
                        level.rowLength[FRONT_ROW] = i + 1;
                        i = 0;
                    }
                }
            }

            for (int r = 0; r < LevelData.BGLayout.GetLength(1); r++)
            {
                for (int i = LevelData.BGLayout.GetLength(0) - 1; i >= 0; i--)
                {
                    if (LevelData.BGLayout[i, r] != 0 && i + 1 > level.rowLength[BACK_ROW])
                    {
                        level.rowLength[BACK_ROW] = i + 1;
                        i = 0;
                    }
                }
            }

            // ----------------------------------------
            // Test data and store the smallest
            // ----------------------------------------
            //Console.Write("[ 0x{0:X2}, 0x{1:X2} ]\n\n", level.rowCount[FRONT_ROW], level.rowCount[BACK_ROW]);

            for (int r = 0; r < level.rowCount[FRONT_ROW]; r++)
            {
                TestAST(testRow[0], r, FRONT_ROW);
                TestBALE(testRow[1], r, FRONT_ROW);
                TestWOLE(testRow[2], r, FRONT_ROW);
                TestZLE(testRow[3], r, FRONT_ROW);

                /*Console.Write("\n[{0:X2}, {1:X2}, {2:X2}, {3:X2}]\n",
                        testRow[0].dataLength,
                        testRow[1].dataLength,
                        testRow[2].dataLength,
                        testRow[3].dataLength
                );*/

                int format = testRow[3].format;
                int newDataLength = testRow[3].dataLength;

                if (testRow[0].dataLength < newDataLength)
                {
                    format = testRow[0].format;
                    newDataLength = testRow[0].dataLength;
                }

                if (testRow[1].dataLength < newDataLength)
                {
                    format = testRow[1].format;
                    newDataLength = testRow[1].dataLength;
                }

                if (testRow[2].dataLength < newDataLength)
                {
                    format = testRow[2].format;
                    newDataLength = testRow[2].dataLength;
                }

                level.row[FRONT_ROW][r].format = testRow[format & 0xF].format;
                level.row[FRONT_ROW][r].attribute = testRow[format & 0xF].attribute;
                level.row[FRONT_ROW][r].dataLength = testRow[format & 0xF].dataLength;

                if (format != 0)
                {
                    for (int i = 0; i < level.row[FRONT_ROW][r].dataLength - 1; i++)
                    {
                        level.row[FRONT_ROW][r].data[i] = testRow[format & 0xF].data[i];
                    }
                }

                /*Console.Write("F 0x{0:X2} :\tf 0x{1:X2}\ta 0x{2:X2}\tdl 0x{3:X2}\n",
                        r,
                        level.row[FRONT_ROW][r].format,
                        level.row[FRONT_ROW][r].attribute,
                        level.row[FRONT_ROW][r].dataLength
                );*/
            }

            //Console.Write("\n");

            for (int r = 0; r < level.rowCount[BACK_ROW]; r++)
            {
                TestAST(testRow[0], r, BACK_ROW);
                TestBALE(testRow[1], r, BACK_ROW);
                TestWOLE(testRow[2], r, BACK_ROW);
                TestZLE(testRow[3], r, BACK_ROW);

                /*Console.Write("\n[{0:X2}, {1:X2}, {2:X2}, {3:X2}]\n",
                        testRow[0].dataLength,
                        testRow[1].dataLength,
                        testRow[2].dataLength,
                        testRow[3].dataLength
                );*/

                int format = testRow[3].format;
                int newDataLength = testRow[3].dataLength;

                if (testRow[0].dataLength < newDataLength)
                {
                    format = testRow[0].format;
                    newDataLength = testRow[0].dataLength;
                }

                if (testRow[1].dataLength < newDataLength)
                {
                    format = testRow[1].format;
                    newDataLength = testRow[1].dataLength;
                }

                if (testRow[2].dataLength < newDataLength)
                {
                    format = testRow[2].format;
                    newDataLength = testRow[2].dataLength;
                }

                level.row[BACK_ROW][r].format = testRow[format & 0xF].format;
                level.row[BACK_ROW][r].attribute = testRow[format & 0xF].attribute;
                level.row[BACK_ROW][r].dataLength = testRow[format & 0xF].dataLength;

                if (format != 0)
                {
                    for (int i = 0; i < level.row[BACK_ROW][r].dataLength - 1; i++)
                        level.row[BACK_ROW][r].data[i] = testRow[format & 0xF].data[i];
                }

                /*Console.Write("B 0x{0:X2} :\tf 0x{1:X2}\ta 0x{2:X2}\tdl 0x{3:X2}\n",
                        r,
                        level.row[BACK_ROW][r].format,
                        level.row[BACK_ROW][r].attribute,
                        level.row[BACK_ROW][r].dataLength
                );*/
            }

            // ----------------------------------------
            // Construct output file
            // ----------------------------------------
            ushort ptr = (ushort)((((level.rowCount[FRONT_ROW] * 2) + level.rowCount[BACK_ROW]) * 2) + 2);

            MemoryStream file = new MemoryStream();
            file.WriteByte((byte)level.rowCount[FRONT_ROW]);
            file.WriteByte((byte)level.rowCount[BACK_ROW]);

            for (int r = 0; r < level.rowCount[FRONT_ROW]; r++)
            {
                file.Seek(2 + ((r * 2) << 1), SeekOrigin.Begin);
                file.Write(BitConverter.GetBytes(ptr), 0, 2);
                file.Write(BitConverter.GetBytes(ptr), 0, 2);
                file.Seek(ptr, SeekOrigin.Begin);

                if (level.row[FRONT_ROW][r].format != 0)
                {
                    file.WriteByte((byte)level.row[FRONT_ROW][r].attribute);
                    file.Write(level.row[FRONT_ROW][r].data, 0, level.row[FRONT_ROW][r].dataLength - 1);
                    if ((level.row[FRONT_ROW][r].dataLength & 1) != 0)
                    {
                        file.WriteByte(0);
                    }

                    ptr = (ushort)file.Position;
                }
            }

            for (int r = 0; r < level.rowCount[BACK_ROW]; r++)
            {
                file.Seek(2 + ((level.rowCount[FRONT_ROW] * 2) << 1) + (r << 1), SeekOrigin.Begin);
                file.Write(BitConverter.GetBytes(ptr), 0, 2);
                file.Seek(ptr, SeekOrigin.Begin);

                if (level.row[BACK_ROW][r].format != 0)
                {
                    file.WriteByte((byte)level.row[BACK_ROW][r].attribute);
                    file.Write(level.row[BACK_ROW][r].data, 0, level.row[BACK_ROW][r].dataLength - 1);
                    if ((level.row[BACK_ROW][r].dataLength & 1) != 0)
                    {
                        file.WriteByte(0);
                    }

                    ptr = (ushort)file.Position;
                }
            }

            for (int r = 0; r < level.rowCount[FRONT_ROW]; r++)
            {
                byte[] buf = new byte[2];
                file.Seek(2 + ((r * 2) << 1), SeekOrigin.Begin);
                file.Read(buf, 0, 2);
                ptr = BitConverter.ToUInt16(buf, 0);
                ptr >>= 1;

                if (level.row[FRONT_ROW][r].format == 0)
                    ptr = (ushort)level.row[FRONT_ROW][r].attribute;
                else
                    ptr |= (ushort)(level.row[FRONT_ROW][r].format << 11);

                ptr = SwapEndianShort(ptr);
                file.Seek(-2, SeekOrigin.Current);
                file.Write(BitConverter.GetBytes(ptr), 0, 2);
            }

            for (int r = 0; r < level.rowCount[BACK_ROW]; r++)
            {
                byte[] buf = new byte[2];
                file.Seek(2 + ((level.rowCount[FRONT_ROW] * 2) << 1) + (r << 1), SeekOrigin.Begin);
                file.Read(buf, 0, 2);
                ptr = BitConverter.ToUInt16(buf, 0);
                ptr >>= 1;

                if (level.row[BACK_ROW][r].format == 0)
                    ptr = (ushort)level.row[BACK_ROW][r].attribute;
                else
                    ptr |= (ushort)(level.row[BACK_ROW][r].format << 11);

                ptr = SwapEndianShort(ptr);
                file.Seek(-2, SeekOrigin.Current);
                file.Write(BitConverter.GetBytes(ptr), 0, 2);
            }
            Compression.Compress(file.ToArray(), LevelData.Level.Layout, LevelData.Level.LayoutCompression);
            file.Close();
        }

        static void TestAST(ROW testRow, int row, int plane)
        {
            // "mode" is not used with this function

            byte[,] layout = LevelData.FGLayout;
            if (plane == BACK_ROW) layout = LevelData.BGLayout;

            int tile = layout[0, row];

            for (int i = 0; i < level.rowLength[plane]; i++)
            {
                if (tile != layout[i, row])
                {
                    testRow.dataLength = 255;
                    return;
                }
            }

            testRow.format = FORMAT_AST;
            testRow.attribute = tile;
            testRow.dataLength = 0;
        }

        static void TestBALE(ROW testRow, int row, int plane)
        {
            int i;
            byte[] usedTiles = new byte[256];
            byte[] tileRef = new byte[16 + 112];	// used for 4-bit mode
            byte[] tileCrossRef = new byte[256];	// used for 4-bit mode
            int totalUsedTiles = 0;
            byte[,] layout = LevelData.FGLayout;
            if (plane == BACK_ROW) layout = LevelData.BGLayout;

            for (int mask = 2; mask <= level.rowLength[plane]; mask <<= 1)
            {
                for (i = 0; i < level.rowLength[plane] - mask; i++)
                {
                    if (layout[i, row] != layout[i + mask, row])
                    {
                        break;
                    }
                }

                if (i >= level.rowLength[plane] - mask)
                {
                    testRow.attribute = mask - 1;

                    for (i = 0; i < 256; i++)
                        usedTiles[i] = 0;

                    for (i = 0; i < level.rowLength[plane]; i++)
                        usedTiles[layout[i, row]] = 1;

                    for (i = 0; i < 256; i++)
                    {
                        if (usedTiles[i] != 0)
                        {
                            tileRef[totalUsedTiles] = (byte)i;
                            tileCrossRef[i] = (byte)totalUsedTiles;
                            totalUsedTiles++;
                        }
                    }

                    if (totalUsedTiles < 16 && totalUsedTiles <= (mask >> 1))
                    {
                        // 4-bit
                        testRow.format = FORMAT_BALE_4BIT;
                        testRow.dataLength = 1 + (mask >> 1) + totalUsedTiles;

                        for (i = 0; i < (mask >> 1); i++)
                        {
                            testRow.data[i] =
                                (byte)((tileCrossRef[layout[(i << 1) + 0, row]] << 4) |
                                tileCrossRef[layout.GetValueOrDefault((i << 1) + 1, row)]);
                        }
                        for (i = 0; i < totalUsedTiles; i++)
                        {
                            testRow.data[(mask >> 1) + i] = tileRef[i];
                        }
                    }
                    else
                    {
                        // 8-bit
                        testRow.format = FORMAT_BALE;
                        testRow.dataLength = 1 + mask;

                        for (i = 0; i < mask; i++)
                        {
                            testRow.data[i] = layout[i, row];
                        }
                    }

                    return;
                }
            }

            testRow.dataLength = 255;
        }

        static void TestWOLE(ROW testRow, int row, int plane)
        {
            int i;
            byte[] usedTiles = new byte[256];
            byte[] tileRef = new byte[16 + 112];	// used for 4-bit mode
            byte[] tileCrossRef = new byte[256];	// used for 4-bit mode
            int totalUsedTiles = 0;
            byte[,] layout = LevelData.FGLayout;
            if (plane == BACK_ROW) layout = LevelData.BGLayout;

            for (int mask = (level.rowLength[plane] >> 1) + (level.rowLength[plane] & 1); mask <= level.rowLength[plane]; mask++)
            {
                for (i = 0; i < level.rowLength[plane] - mask; i++)
                {
                    if (layout[i, row] != layout[i + mask, row])
                    {
                        break;
                    }
                }

                if (i >= level.rowLength[plane] - mask)
                {
                    testRow.attribute = mask;

                    for (i = 0; i < 256; i++)
                        usedTiles[i] = 0;

                    for (i = 0; i < level.rowLength[plane]; i++)
                        usedTiles[layout[i, row]] = 1;

                    for (i = 0; i < 256; i++)
                    {
                        if (usedTiles[i] != 0)
                        {
                            tileRef[totalUsedTiles] = (byte)i;
                            tileCrossRef[i] = (byte)totalUsedTiles;
                            totalUsedTiles++;
                        }
                    }

                    if (totalUsedTiles < 16 && totalUsedTiles <= ((mask + 1) >> 1))
                    {
                        // 4-bit
                        testRow.format = FORMAT_WOLE_4BIT;
                        testRow.dataLength = 1 + ((mask + 1) >> 1) + totalUsedTiles;

                        for (i = 0; i < ((mask + 1) >> 1); i++)
                        {
                            testRow.data[i] =
                                (byte)((tileCrossRef[layout[(i << 1) + 0, row]] << 4) |
                                tileCrossRef[layout.GetValueOrDefault((i << 1) + 1, row)]);
                        }
                        for (i = 0; i < totalUsedTiles; i++)
                        {
                            testRow.data[((mask + 1) >> 1) + i] = tileRef[i];
                        }
                    }
                    else
                    {
                        // 8-bit
                        testRow.format = FORMAT_WOLE;
                        testRow.dataLength = 1 + mask;

                        for (i = 0; i < mask; i++)
                        {
                            testRow.data[i] = layout[i, row];
                        }
                    }

                    return;
                }
            }

            testRow.dataLength = 255;
        }

        static void TestZLE(ROW testRow, int row, int plane)
        {
            int i;
            byte[] usedTiles = new byte[256];
            byte[] tileRef = new byte[16 + 112];	// used for 4-bit mode
            byte[] tileCrossRef = new byte[256];	// used for 4-bit mode
            int totalUsedTiles = 0;
            byte[,] layout = LevelData.FGLayout;
            if (plane == BACK_ROW) layout = LevelData.BGLayout;

            for (i = level.rowLength[plane] - 1; i >= 0; i--)
            {
                if (layout[i, row] != 0)
                {
                    testRow.attribute = i + 1;

                    for (i = 0; i < 256; i++)
                        usedTiles[i] = 0;

                    for (i = 0; i < level.rowLength[plane]; i++)
                        usedTiles[layout[i, row]] = 1;

                    for (i = 0; i < 256; i++)
                    {
                        if (usedTiles[i] != 0)
                        {
                            tileRef[totalUsedTiles] = (byte)i;
                            tileCrossRef[i] = (byte)totalUsedTiles;
                            totalUsedTiles++;
                        }
                    }

                    if (totalUsedTiles < 16 && totalUsedTiles <= ((testRow.attribute + 1) >> 1))
                    {
                        // 4-bit
                        testRow.format = FORMAT_ZLE_4BIT;
                        testRow.dataLength = 1 + ((testRow.attribute + 1) >> 1) + totalUsedTiles;

                        //Console.Write("  %02X ", testRow->attribute);

                        for (i = 0; i < ((testRow.attribute + 1) >> 1); i++)
                        {
                            testRow.data[i] =
                                (byte)((tileCrossRef[layout[(i << 1) + 0, row]] << 4) |
                                tileCrossRef[layout.GetValueOrDefault((i << 1) + 1, row)]);
                            //Console.Write("  %02X ", testRow->newData[i]);
                        }
                        for (i = 0; i < totalUsedTiles; i++)
                        {
                            testRow.data[((testRow.attribute + 1) >> 1) + i] = tileRef[i];
                            //Console.Write("  %02X ", tileRef[i]);
                        }

                        //Console.Write("\n");
                    }
                    else
                    {
                        // 8-bit
                        testRow.format = FORMAT_ZLE;
                        testRow.dataLength = 1 + testRow.attribute;

                        for (i = 0; i < level.rowLength[plane]; i++)
                        {
                            testRow.data[i] = layout[i, row];
                        }
                    }

                    return;
                }
            }

            testRow.dataLength = 255;
        }

        public class SBoomRingEntry : S2RingEntry
        {
            public SBoomRingEntry() { pos = new Position(this); Count = 1; }

            public SBoomRingEntry(byte[] file, int address)
            {
                byte[] bytes = new byte[Size];
                Array.Copy(file, address, bytes, 0, Size);
                FromBytes(bytes);
                pos = new Position(this);
            }

            public override byte[] GetBytes()
            {
                List<byte> ret = new List<byte>();
                ushort val = (ushort)(X & 0x7FFF);
                if (Direction == API.Direction.Vertical) val |= 0x8000;
                ret.AddRange(ByteConverter.GetBytes(val));
                val = (ushort)(Y & 0x1FFF);
                val |= (ushort)((Count - 1) << 13);
                ret.AddRange(ByteConverter.GetBytes(val));
                return ret.ToArray();
            }

            public override void FromBytes(byte[] bytes)
            {
                ushort val = ByteConverter.ToUInt16(bytes, 0);
                Direction = (val & 0x8000) == 0x8000 ? Direction.Vertical : Direction.Horizontal;
                X = (ushort)(val & 0x7FFF);
                val = ByteConverter.ToUInt16(bytes, 2);
                Count = (byte)((val >> 13) + 1);
                Y = (ushort)(val & 0x1FFF);
            }
        }
    }
}