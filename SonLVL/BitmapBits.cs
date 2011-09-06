using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SonicRetro.SonLVL
{
    /// <summary>
    /// Represents the pixel data of an 8bpp Bitmap.
    /// </summary>
    public class BitmapBits
    {
        public byte[] Bits { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Size Size
        {
            get
            {
                return new Size(Width, Height);
            }
        }

        public byte this[int x, int y]
        {
            get
            {
                return Bits[(y * Width) + x];
            }
            set
            {
                Bits[(y * Width) + x] = value;
            }
        }

        public BitmapBits(int width, int height)
        {
            Width = width;
            Height = height;
            Bits = new byte[width * height];
        }

        public BitmapBits(Bitmap bmp)
        {
            if (bmp.PixelFormat != PixelFormat.Format8bppIndexed)
                throw new ArgumentException();
            BitmapData bmpd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
            Width = bmpd.Width;
            Height = bmpd.Height;
            byte[] tmpbits = new byte[Math.Abs(bmpd.Stride) * bmpd.Height];
            Marshal.Copy(bmpd.Scan0, tmpbits, 0, tmpbits.Length);
            Bits = new byte[Width * Height];
            int j = 0;
            for (int i = 0; i < Bits.Length; i += Width)
            {
                Array.Copy(tmpbits, j, Bits, i, Width);
                j += Math.Abs(bmpd.Stride);
            }
            bmp.UnlockBits(bmpd);
        }

        public BitmapBits(BitmapBits source)
        {
            Width = source.Width;
            Height = source.Height;
            Bits = new byte[source.Bits.Length];
            Array.Copy(source.Bits, Bits, Bits.Length);
        }

        public Bitmap ToBitmap()
        {
            Bitmap newbmp = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed);
            BitmapData newbmpd = newbmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            byte[] bmpbits = new byte[Math.Abs(newbmpd.Stride) * newbmpd.Height];
            for (int y = 0; y < Height; y++)
                Array.Copy(Bits, y * Width, bmpbits, y * Math.Abs(newbmpd.Stride), Width);
            Marshal.Copy(bmpbits, 0, newbmpd.Scan0, bmpbits.Length);
            newbmp.UnlockBits(newbmpd);
            return newbmp;
        }

        public Bitmap ToBitmap(ColorPalette palette)
        {
            Bitmap newbmp = ToBitmap();
            newbmp.Palette = palette;
            return newbmp;
        }

        public Bitmap ToBitmap(params Color[] palette)
        {
            Bitmap newbmp = ToBitmap();
            ColorPalette pal = newbmp.Palette;
            for (int i = 0; i < Math.Min(palette.Length, 256); i++)
                pal.Entries[i] = palette[i];
            newbmp.Palette = pal;
            return newbmp;
        }

        public void DrawBitmap(BitmapBits source, Point location)
        {
            int dstx = location.X; int dsty = location.Y;
            for (int i = 0; i < source.Height; i++)
            {
                int di = ((dsty + i) * Width) + dstx;
                int si = i * source.Width;
                Array.Copy(source.Bits, si, Bits, di, source.Width);
            }
        }

        public void DrawBitmapComposited(BitmapBits source, Point location)
        {
            int srcl = 0;
            if (location.X < 0)
                srcl = -location.X;
            int srct = 0;
            if (location.Y < 0)
                srct = -location.Y;
            int srcr = source.Width;
            if (srcr > Width - location.X)
                srcr = Width - location.X;
            int srcb = source.Height;
            if (srcb > Height - location.Y)
                srcb = Height - location.Y;
            for (int i = srct; i < srcb; i++)
                for (int x = srcl; x < srcr; x++)
                    if (source[x, i] != 0)
                        this[location.X + x, location.Y + i] = source[x, i];
        }

        public void Flip(bool XFlip, bool YFlip)
        {
            if (!XFlip & !YFlip)
                return;
            if (XFlip)
            {
                for (int y = 0; y < Height; y++)
                {
                    int addr = y * Width;
                    Array.Reverse(Bits, addr, Width);
                }
            }
            if (YFlip)
            {
                byte[] tmppix = new byte[Bits.Length];
                for (int y = 0; y < Height; y++)
                {
                    int srcaddr = y * Width;
                    int dstaddr = (Height - y - 1) * Width;
                    Array.Copy(Bits, srcaddr, tmppix, dstaddr, Width);
                }
                Bits = tmppix;
            }
        }

        public void Clear()
        {
            Array.Clear(Bits, 0, Bits.Length);
        }

        public static BitmapBits FromTile(byte[] art, int index)
        {
            BitmapBits bmp = new BitmapBits(8, 8);
            if (index * 32 + 32 <= art.Length)
            {
                for (int i = 0; i < 32; i++)
                {
                    bmp.Bits[i * 2] = (byte)(art[i + (index * 32)] >> 4);
                    bmp.Bits[(i * 2) + 1] = (byte)(art[i + (index * 32)] & 0xF);
                }
            }
            return bmp;
        }

        public byte[] ToTile()
        {
            List<byte> res = new List<byte>();
            for (int y = 0; y < 8; y++)
                for (int x = 0; x < 8; x += 2)
                    res.Add((byte)(((this[x, y] & 0xF) << 4) | (this[x + 1, y] & 0xF)));
            return res.ToArray();
        }

        public void Rotate(int R)
        {
            byte[] tmppix = new byte[Bits.Length];
            switch (R)
            {
                case 1:
                    for (int y = 0; y < Height; y++)
                    {
                        int srcaddr = y * Width;
                        int dstaddr = (Width * (Width - 1)) + y;
                        for (int x = 0; x < Width; x++)
                        {
                            tmppix[dstaddr] = Bits[srcaddr + x];
                            dstaddr -= Width;
                        }
                    }
                    Bits = tmppix;
                    int h = Height;
                    Height = Width;
                    Width = h;
                    break;
                case 2:
                    Flip(true, true);
                    break;
                case 3:
                    for (int y = 0; y < Height; y++)
                    {
                        int srcaddr = y * Width;
                        int dstaddr = Height - 1 - y;
                        for (int x = 0; x < Width; x++)
                        {
                            tmppix[dstaddr] = Bits[srcaddr + x];
                            dstaddr += Width;
                        }
                    }
                    Bits = tmppix;
                    h = Height;
                    Height = Width;
                    Width = h;
                    break;
            }
        }

        public BitmapBits Scale(int factor)
        {
            BitmapBits res = new BitmapBits(Width * factor, Height * factor);
            for (int y = 0; y < res.Height; y++)
                for (int x = 0; x < res.Width; x++)
                    res[x, y] = this[(x / factor), (y / factor)];
            return res;
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj)) return true;
            BitmapBits other = obj as BitmapBits;
            if (other == null) return false;
            if (Width != other.Width | Height != other.Height) return false;
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    if (this[x, y] != other[x, y])
                        return false;
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}