using System;
using System.IO;

/*
 *  SZDDComp.impl.cs: Microsoft "compress.exe/expand.exe" compatible compressor
 *
 *  Copyright (c) 2000 Martin Hinner <mhi@penguin.cz>
 *  Algorithm & data structures by M. Winterhoff <100326.2776@compuserve.com>
 *  C# port Copyright (c) 2011 Francis Gagné <fragag@hotmail.com> (Compressor) / Michael Ratzlaff <sonicmike2@yahoo.com> (Decompressor)
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2, or (at your option)
 *  any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
 */

namespace SZDDComp
{

    public static partial class SZDDComp
    {
        private const int N = 0x1000;
        private const int F = 0x10;
        private const int THRESHOLD = 3;
        private const int NIL = -1;

        private static readonly byte[] Magic = new byte[] { 0x53, 0x5A, 0x44, 0x44, 0x88, 0xF0, 0x27, 0x33, 0x41, 0x00 };

        private static void Encode(Stream input, Stream output)
        {
            int ch, i, run, len, match, size, mask;
            byte[] buf = new byte[17];
            byte[] byteBuffer = new byte[8];

            Buffer buffer = new Buffer();
            output.Write(Magic, 0, Magic.Length);
            Int32ToBytesLE(checked((int)(input.Length - input.Position)), byteBuffer);
            output.Write(byteBuffer, 0, 4);

            size = mask = 1;
            buf[0] = 0;
            i = N - F - F;
            for (len = 0; len < F && (ch = input.ReadByte()) != -1; len++)
            {
                buffer.SetHead(i + F, (byte)ch);
                i = (i + 1) & (N - 1);
            }

            run = len;
            do
            {
                ch = input.ReadByte();
                if (i >= N - F)
                {
                    buffer.Delete(i + F - N);
                    buffer.SetHead(i + F, (byte)ch);
                    buffer.SetHead(i + F - N, (byte)ch);
                }
                else
                {
                    buffer.Delete(i + F);
                    buffer.SetHead(i + F, (byte)ch);
                }

                match = buffer.Insert(i, run);
                if (ch == -1)
                {
                    run--;
                    len--;
                }

                if (len++ >= run)
                {
                    if (match >= THRESHOLD)
                    {
                        buf[size++] = (byte)buffer.Position;
                        buf[size++] = (byte)(((buffer.Position >> 4) & 0xF0) + (match - 3));
                        len -= match;
                    }
                    else
                    {
                        buf[0] |= (byte)mask;
                        buf[size++] = buffer.GetHead(i);
                        len--;
                    }

                    mask += mask;
                    if ((mask & 0xFF) == 0)
                    {
                        output.Write(buf, 0, size);
                        size = mask = 1;
                        buf[0] = 0;
                    }
                }

                i = (i + 1) & (N - 1);
            } while (len > 0);

            if (size > 1)
            {
                output.Write(buf, 0, size);
            }
        }

        private static void Int32ToBytesLE(int value, byte[] bytes)
        {
            bytes[0] = (byte)value;
            bytes[1] = (byte)(value >> 8);
            bytes[2] = (byte)(value >> 16);
            bytes[3] = (byte)(value >> 24);
        }

        private class Buffer
        {
            private const int DadOffset = 1;
            private const int LeftSonOffset = 1 + N;
            private const int RightSonOffset = 1 + N + N;
            private const int RootOffset = 1 + N + N + N;

            int pos;
            byte[] head = new byte[N + F];
            int[] node = new int[N + 1 + N + N + 256];

            /// <summary>
            /// Initializes a new instance of the <see cref="Buffer" /> class.
            /// </summary>
            public Buffer()
            {
                for (int i = 0; i < 256; i++)
                {
                    SetRoot(i, NIL);
                }

                for (int i = NIL; i < N; i++)
                {
                    SetDad(i, NIL);
                }
            }

            public int Position
            {
                get { return this.pos; }
            }

            public int Insert(int i, int run)
            {
                int c = 0, j, k, l, n, match;
                int idx;

                k = l = 1;
                match = THRESHOLD - 1;
                idx = RootOffset + head[i];
                SetLeftSon(i, NIL);
                SetRightSon(i, NIL);

                while ((j = node[idx]) != NIL)
                {
                    n = Math.Min(k, l);
                    while (n < run && (c = (head[j + n] - head[i + n])) == 0)
                    {
                        n++;
                    }

                    if (n > match)
                    {
                        match = n;
                        pos = j;
                    }

                    if (c < 0)
                    {
                        idx = LeftSonOffset + j;
                        k = n;
                    }
                    else if (c > 0)
                    {
                        idx = RightSonOffset + j;
                        l = n;
                    }
                    else
                    {
                        SetDad(j, NIL);
                        SetDad(GetLeftSon(j), LeftSonOffset + i);
                        SetDad(GetRightSon(j), RightSonOffset + i);
                        SetLeftSon(i, GetLeftSon(j));
                        SetRightSon(i, GetRightSon(j));
                        break;
                    }
                }

                SetDad(i, idx);
                node[idx] = i;
                return match;
            }

            public void Delete(int z)
            {
                if (GetDad(z) != NIL)
                {
                    int j;

                    if (GetRightSon(z) == NIL)
                    {
                        j = GetLeftSon(z);
                    }
                    else if (GetLeftSon(z) == NIL)
                    {
                        j = GetRightSon(z);
                    }
                    else
                    {
                        j = GetLeftSon(z);
                        if (GetRightSon(j) != NIL)
                        {
                            do
                            {
                                j = GetRightSon(j);
                            } while (GetRightSon(j) != NIL);

                            node[GetDad(j)] = GetLeftSon(j);
                            SetDad(GetLeftSon(j), GetDad(j));
                            SetLeftSon(j, GetLeftSon(z));
                            SetDad(GetLeftSon(z), LeftSonOffset + j);
                        }

                        SetRightSon(j, GetRightSon(z));
                        SetDad(GetRightSon(z), RightSonOffset + j);
                    }

                    SetDad(j, GetDad(z));
                    node[GetDad(z)] = j;
                    SetDad(z, NIL);
                }
            }

            public byte GetHead(int index)
            {
                return head[index];
            }

            public void SetHead(int index, byte value)
            {
                head[index] = value;
            }

            private int GetDad(int index)
            {
                return node[DadOffset + index];
            }

            private int GetLeftSon(int index)
            {
                return node[LeftSonOffset + index];
            }

            private int GetRightSon(int index)
            {
                return node[RightSonOffset + index];
            }

            private void SetDad(int index, int value)
            {
                node[DadOffset + index] = value;
            }

            private void SetLeftSon(int index, int value)
            {
                node[LeftSonOffset + index] = value;
            }

            private void SetRightSon(int index, int value)
            {
                node[RightSonOffset + index] = value;
            }

            private void SetRoot(int index, int value)
            {
                node[RootOffset + index] = value;
            }
        }

        static int Decode(Stream infile, Stream outfile)
        {
            int bits, ch, i, j, len, mask;
            byte[] tmpbuf;
            byte[] buffer;

            uint magic1;
            uint magic2;
            uint magic3;
            ushort reserved;
            uint filesize;

            tmpbuf = new byte[4];
            if (infile.Read(tmpbuf, 0, 4) == -1)
            {
                throw new Exception();
            }
            magic1 = BitConverter.ToUInt32(tmpbuf, 0);

            if (magic1 == 0x44445A53U)
            {
                if (infile.Read(tmpbuf, 0, 4) == -1)
                {
                    throw new Exception();
                }
                magic2 = BitConverter.ToUInt32(tmpbuf, 0);

                if (infile.Read(tmpbuf, 0, 2) == -1)
                {
                    throw new Exception();
                }
                reserved = BitConverter.ToUInt16(tmpbuf, 0);

                if (infile.Read(tmpbuf, 0, 4) == -1)
                {
                    throw new Exception();
                }
                filesize = BitConverter.ToUInt32(tmpbuf, 0);

                if (magic2 != 0x3327F088L)
                {
                    throw new Exception("This is not a MS-compressed file!");
                }
            }
            else if (magic1 == 0x4A41574BU)
            {
                if (infile.Read(tmpbuf, 0, 4) == -1)
                {
                    throw new Exception();
                }
                magic2 = BitConverter.ToUInt32(tmpbuf, 0);

                if (infile.Read(tmpbuf, 0, 4) == -1)
                {
                    throw new Exception();
                }
                magic3 = BitConverter.ToUInt32(tmpbuf, 0);

                if (infile.Read(tmpbuf, 0, 2) == -1)
                {
                    throw new Exception();
                }
                reserved = BitConverter.ToUInt16(tmpbuf, 0);

                if (magic2 != 0xD127F088L || magic3 != 0x00120003L)
                {
                    throw new Exception("This is not a MS-compressed file!");
                }
                throw new Exception("Unsupported version 6.22!");
            }
            else
            {
                throw new Exception("This is not a MS-compressed file!");
            }


            buffer = new byte[N];

            for (int q = 0; q < buffer.Length; q++)
                buffer[q] = 0x20;

            i = N - F;
            while (true)
            {
                bits = infile.ReadByte();
                if (bits == -1)
                    break;

                for (mask = 0x01; (mask & 0xFF) != 0; mask <<= 1)
                {
                    if ((bits & mask) == 0)
                    {
                        j = infile.ReadByte();
                        if (j == -1)
                            break;
                        len = infile.ReadByte();
                        j += (len & 0xF0) << 4;
                        len = (len & 15) + 3;
                        while (len-- != 0)
                        {
                            buffer[i] = buffer[j];
                            outfile.WriteByte(buffer[i]);
                            j++;
                            j %= N;
                            i++;
                            i %= N;
                        }
                    }
                    else
                    {
                        ch = infile.ReadByte();
                        if (ch == -1)
                            break;
                        buffer[i] = (byte)ch;
                        outfile.WriteByte(buffer[i]);
                        i++;
                        i %= N;
                    }
                }
            }
            return 0;
        }
    }
}