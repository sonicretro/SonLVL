using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;

namespace SonicRetro.SonLVL.API
{
    [Serializable]
    /// <summary>
    /// Represents the pixel data of an 8bpp Bitmap.
    /// </summary>
    public class BitmapBits
    {
        public byte[] Bits { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Size Size { get { return new Size(Width, Height); } }
		public PixelFormat OriginalFormat { get; private set; }

		public int GetPixelIndex(int x, int y)
		{
			return (y * Width) + x;
		}

        public byte this[int x, int y]
        {
			get { return Bits[GetPixelIndex(x, y)]; }
			set { Bits[GetPixelIndex(x, y)] = value; }
        }

        public BitmapBits(int width, int height)
        {
            Width = width;
            Height = height;
            Bits = new byte[width * height];
			OriginalFormat = PixelFormat.Format8bppIndexed;
        }

        public BitmapBits(Size size)
            : this(size.Width, size.Height) { }

		public BitmapBits(Bitmap bmp)
		{
			LoadBitmap(bmp);
		}

		public BitmapBits(string filename)
		{
			using (Bitmap bmp = new Bitmap(filename))
				LoadBitmap(bmp);
		}

		private void LoadBitmap(Bitmap bmp)
		{
			switch (bmp.PixelFormat)
			{
				case PixelFormat.Format1bppIndexed:
					LoadBitmap1bpp(bmp);
					break;
				case PixelFormat.Format4bppIndexed:
					LoadBitmap4bpp(bmp);
					break;
				case PixelFormat.Format8bppIndexed:
					LoadBitmap8bpp(bmp);
					break;
				default:
					throw new ArgumentException("Only indexed-color bitmaps are supported.");
			}
			OriginalFormat = bmp.PixelFormat;
		}

		private void LoadBitmap1bpp(Bitmap bmp)
		{
			BitmapData bmpd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format1bppIndexed);
			Width = bmpd.Width;
			Height = bmpd.Height;
			Bits = new byte[Width * Height];
			byte[] tmpbits = new byte[Math.Abs(bmpd.Stride) * bmpd.Height];
			Marshal.Copy(bmpd.Scan0, tmpbits, 0, tmpbits.Length);
			int dstaddr = 0;
			for (int y = 0; y < Height; y++)
			{
				int srcaddr = y * Math.Abs(bmpd.Stride);
				for (int x = 0; x < Width; x += 8)
				{
					Bits[dstaddr++] = (byte)((tmpbits[srcaddr + (x / 8)] >> 7) & 1);
					if (x + 1 == Width) break;
					Bits[dstaddr++] = (byte)((tmpbits[srcaddr + (x / 8)] >> 6) & 1);
					if (x + 2 == Width) break;
					Bits[dstaddr++] = (byte)((tmpbits[srcaddr + (x / 8)] >> 5) & 1);
					if (x + 3 == Width) break;
					Bits[dstaddr++] = (byte)((tmpbits[srcaddr + (x / 8)] >> 4) & 1);
					if (x + 4 == Width) break;
					Bits[dstaddr++] = (byte)((tmpbits[srcaddr + (x / 8)] >> 3) & 1);
					if (x + 5 == Width) break;
					Bits[dstaddr++] = (byte)((tmpbits[srcaddr + (x / 8)] >> 2) & 1);
					if (x + 6 == Width) break;
					Bits[dstaddr++] = (byte)((tmpbits[srcaddr + (x / 8)] >> 1) & 1);
					if (x + 7 == Width) break;
					Bits[dstaddr++] = (byte)(tmpbits[srcaddr + (x / 8)] & 1);
				}
			}
			bmp.UnlockBits(bmpd);
		}

		private void LoadBitmap4bpp(Bitmap bmp)
		{
			BitmapData bmpd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format4bppIndexed);
			Width = bmpd.Width;
			Height = bmpd.Height;
			Bits = new byte[Width * Height];
			byte[] tmpbits = new byte[Math.Abs(bmpd.Stride) * bmpd.Height];
			Marshal.Copy(bmpd.Scan0, tmpbits, 0, tmpbits.Length);
			int dstaddr = 0;
			for (int y = 0; y < Height; y++)
			{
				int srcaddr = y * Math.Abs(bmpd.Stride);
				for (int x = 0; x < Width; x += 2)
				{
					Bits[dstaddr++] = (byte)(tmpbits[srcaddr + (x / 2)] >> 4);
					if (x + 1 == Width) break;
					Bits[dstaddr++] = (byte)(tmpbits[srcaddr + (x / 2)] & 0xF);
				}
			}
			bmp.UnlockBits(bmpd);
		}

		private void LoadBitmap8bpp(Bitmap bmp)
		{
			BitmapData bmpd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);
			Width = bmpd.Width;
			Height = bmpd.Height;
			byte[] tmpbits = new byte[Math.Abs(bmpd.Stride) * bmpd.Height];
			Marshal.Copy(bmpd.Scan0, tmpbits, 0, tmpbits.Length);
			if (bmpd.Stride == bmpd.Width)
				Bits = tmpbits;
			else
			{
				Bits = new byte[Width * Height];
				int j = 0;
				for (int i = 0; i < Bits.Length; i += Width)
				{
					Array.Copy(tmpbits, j, Bits, i, Width);
					j += Math.Abs(bmpd.Stride);
				}
			}
			bmp.UnlockBits(bmpd);
		}

        public BitmapBits(BitmapBits source)
        {
            Width = source.Width;
            Height = source.Height;
            Bits = new byte[source.Bits.Length];
            Array.Copy(source.Bits, Bits, Bits.Length);
			OriginalFormat = PixelFormat.Format8bppIndexed;
        }

        public Bitmap ToBitmap()
        {
            if (Size.IsEmpty) return new Bitmap(1, 1, PixelFormat.Format8bppIndexed);
            Bitmap newbmp = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed);
            BitmapData newbmpd = newbmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            if (newbmpd.Stride == newbmpd.Width)
                Marshal.Copy(Bits, 0, newbmpd.Scan0, Bits.Length);
            else
            {
                byte[] bmpbits = new byte[Math.Abs(newbmpd.Stride) * newbmpd.Height];
                for (int y = 0; y < Height; y++)
                    Array.Copy(Bits, y * Width, bmpbits, y * Math.Abs(newbmpd.Stride), Width);
                Marshal.Copy(bmpbits, 0, newbmpd.Scan0, bmpbits.Length);
            }
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

		public Bitmap ToBitmap4bpp()
		{
			if (Size.IsEmpty) return new Bitmap(1, 1, PixelFormat.Format4bppIndexed);
			Bitmap newbmp = new Bitmap(Width, Height, PixelFormat.Format4bppIndexed);
			BitmapData newbmpd = newbmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format4bppIndexed);
			byte[] bmpbits = new byte[Math.Abs(newbmpd.Stride) * newbmpd.Height];
			int srcaddr = 0;
			for (int y = 0; y < Height; y++)
			{
				int dstaddr = y * Math.Abs(newbmpd.Stride);
				for (int x = 0; x < Width; x += 2)
				{
					byte px = (byte)((Bits[srcaddr++] & 0xF) << 4);
					if (x + 1 != Width)
						px |= (byte)(Bits[srcaddr++] & 0xF);
					bmpbits[dstaddr++] = px;
				}
			}
			Marshal.Copy(bmpbits, 0, newbmpd.Scan0, bmpbits.Length);
			newbmp.UnlockBits(newbmpd);
			return newbmp;
		}

		public Bitmap ToBitmap4bpp(ColorPalette palette)
		{
			Bitmap newbmp = ToBitmap4bpp();
			newbmp.Palette = palette;
			return newbmp;
		}

		public Bitmap ToBitmap4bpp(params Color[] palette)
		{
			Bitmap newbmp = ToBitmap4bpp();
			ColorPalette pal = newbmp.Palette;
			for (int i = 0; i < Math.Min(palette.Length, 16); i++)
				pal.Entries[i] = palette[i];
			newbmp.Palette = pal;
			return newbmp;
		}

		public Bitmap ToBitmap1bpp()
		{
			if (Size.IsEmpty) return new Bitmap(1, 1, PixelFormat.Format1bppIndexed);
			Bitmap newbmp = new Bitmap(Width, Height, PixelFormat.Format1bppIndexed);
			BitmapData newbmpd = newbmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);
			byte[] bmpbits = new byte[Math.Abs(newbmpd.Stride) * newbmpd.Height];
			int srcaddr = 0;
			for (int y = 0; y < Height; y++)
			{
				int dstaddr = y * Math.Abs(newbmpd.Stride);
				for (int x = 0; x < Width; x += 8)
				{
					byte px = (byte)((Bits[srcaddr++] & 1) << 7);
					if (x + 1 == Width) goto writepx;
					px |= (byte)((Bits[srcaddr++] & 1) << 6);
					if (x + 2 == Width) goto writepx;
					px |= (byte)((Bits[srcaddr++] & 1) << 5);
					if (x + 3 == Width) goto writepx;
					px |= (byte)((Bits[srcaddr++] & 1) << 4);
					if (x + 4 == Width) goto writepx;
					px |= (byte)((Bits[srcaddr++] & 1) << 3);
					if (x + 5 == Width) goto writepx;
					px |= (byte)((Bits[srcaddr++] & 1) << 2);
					if (x + 6 == Width) goto writepx;
					px |= (byte)((Bits[srcaddr++] & 1) << 1);
					if (x + 7 == Width) goto writepx;
					px |= (byte)(Bits[srcaddr++] & 1);
				writepx:
					bmpbits[dstaddr++] = px;
				}
			}
			Marshal.Copy(bmpbits, 0, newbmpd.Scan0, bmpbits.Length);
			newbmp.UnlockBits(newbmpd);
			return newbmp;
		}

		public Bitmap ToBitmap1bpp(ColorPalette palette)
		{
			Bitmap newbmp = ToBitmap1bpp();
			newbmp.Palette = palette;
			return newbmp;
		}

		public Bitmap ToBitmap1bpp(params Color[] palette)
		{
			Bitmap newbmp = ToBitmap1bpp();
			ColorPalette pal = newbmp.Palette;
			for (int i = 0; i < Math.Min(palette.Length, 2); i++)
				pal.Entries[i] = palette[i];
			newbmp.Palette = pal;
			return newbmp;
		}

		public void DrawBitmap(BitmapBits source, int x, int y)
        {
            for (int i = 0; i < source.Height; i++)
            {
                int di = GetPixelIndex(x, y + i);
                int si = i * source.Width;
                Array.Copy(source.Bits, si, Bits, di, source.Width);
            }
        }

        public void DrawBitmap(BitmapBits source, Point location) { DrawBitmap(source, location.X, location.Y); }

        public void DrawBitmapComposited(BitmapBits source, int x, int y)
        {
            int srcl = 0;
            if (x < 0)
                srcl = -x;
            int srct = 0;
            if (y < 0)
                srct = -y;
            int srcr = source.Width;
            if (srcr > Width - x)
                srcr = Width - x;
            int srcb = source.Height;
            if (srcb > Height - y)
                srcb = Height - y;
            for (int c = srct; c < srcb; c++)
                for (int r = srcl; r < srcr; r++)
                    if (source[r, c] != 0)
                        this[x + r, y + c] = source[r, c];
        }

        public void DrawBitmapComposited(BitmapBits source, Point location)
        {
            DrawBitmapComposited(source, location.X, location.Y);
        }

        public void DrawBitmapBounded(BitmapBits source, int x, int y)
        {
            int srcl = 0;
            if (x < 0)
                srcl = -x;
            int srct = 0;
            if (y < 0)
                srct = -y;
            int srcr = source.Width;
            if (srcr > Width - x)
                srcr = Width - x;
            int srcb = source.Height;
            if (srcb > Height - y)
                srcb = Height - y;
            for (int c = srct; c < srcb; c++)
                Array.Copy(source.Bits, source.GetPixelIndex(srcl, c), Bits, GetPixelIndex(x + srcl, y + c), srcr - srcl);
        }

        public void DrawBitmapBounded(BitmapBits source, Point location) { DrawBitmapComposited(source, location.X, location.Y); }

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

        public void DrawLine(byte index, int x1, int y1, int x2, int y2)
        {
			if (y1 == y2)
			{
				if (y1 >= Height || y1 < 0)
					return;
				if (x1 > x2)
				{
					int tmp = x1;
					x1 = x2;
					x2 = tmp;
				}
				int end = y1 * Width + Math.Min(x2, Width - 1);
				for (int i = y1 * Width + Math.Max(x1, 0); i <= end; i++)
					Bits[i] = index;
				return;
			}
			if (x1 == x2)
			{
				if (x1 >= Width || x1 < 0)
					return;
				if (y1 > y2)
				{
					int tmp = y1;
					y1 = y2;
					y2 = tmp;
				}
				int end = Math.Min(y2, Height - 1) * Width + x1;
				for (int i = Math.Max(y1, 0) * Width + x1; i <= end; i += Width)
					Bits[i] = index;
				return;
			}
			bool steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);
            if (steep)
            {
                int tmp = x1;
                x1 = y1;
                y1 = tmp;
                tmp = x2;
                x2 = y2;
                y2 = tmp;
            }
            if (x1 > x2)
            {
                int tmp = x1;
                x1 = x2;
                x2 = tmp;
                tmp = y1;
                y1 = y2;
                y2 = tmp;
            }
            int deltax = x2 - x1;
            int deltay = Math.Abs(y2 - y1);
            double error = 0;
            double deltaerr = (double)deltay / (double)deltax;
            int ystep;
            int y = y1;
            if (y1 < y2) ystep = 1; else ystep = -1;
            for (int x = x1; x <= x2; x++)
            {
                if (steep)
                {
                    if (x >= 0 & x < Height & y >= 0 & y < Width)
                        this[y, x] = index;
                }
                else
                {
                    if (y >= 0 & y < Height & x >= 0 & x < Width)
                        this[x, y] = index;
                }
                error = error + deltaerr;
                if (error >= 0.5)
                {
                    y = y + ystep;
                    error = error - 1.0;
                }
            }
        }

        public void DrawLine(byte index, Point p1, Point p2) { DrawLine(index, p1.X, p1.Y, p2.X, p2.Y); }

        public void DrawRectangle(byte index, int x, int y, int width, int height)
        {
            DrawLine(index, x, y, x + width, y);
            DrawLine(index, x, y, x, y + height);
            DrawLine(index, x + width, y, x + width, y + height);
            DrawLine(index, x, y + height, x + width, y + height);
        }

        public void DrawRectangle(byte index, Rectangle rect) { DrawRectangle(index, rect.X, rect.Y, rect.Width, rect.Height); }

        public void FillRectangle(byte index, int x, int y, int width, int height)
        {
            int srcl = Math.Max(x, 0);
            int srct = Math.Max(y, 0);
            int srcr = Math.Min(x + width, Width);
			int srcb = Math.Min(y + height, Height);
            for (int cy = srct; cy < srcb; cy++)
                for (int cx = srcl; cx < srcr; cx++)
                    this[cx, cy] = index;
        }

        public void FillRectangle(byte index, Rectangle rect) { DrawRectangle(index, rect.X, rect.Y, rect.Width, rect.Height); }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj)) return true;
            BitmapBits other = obj as BitmapBits;
            if (other == null) return false;
            if (Width != other.Width | Height != other.Height) return false;
            return System.Linq.Enumerable.SequenceEqual(Bits, other.Bits);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public BitmapBits GetSection(int x, int y, int width, int height)
        {
            BitmapBits result = new BitmapBits(width, height);
            for (int v = 0; v < height; v++)
                Array.Copy(this.Bits, GetPixelIndex(x, y + v), result.Bits, v * width, width);
            return result;
        }

        public void DrawSprite(Sprite sprite, int x, int y)
        {
            DrawBitmapComposited(sprite.Image, sprite.X + x, sprite.Y + y);
        }

        public void DrawSprite(Sprite sprite, Point location)
        {
            DrawBitmapComposited(sprite.Image, location + new Size(sprite.Offset));
        }

        public void ReplaceColor(byte old, byte @new)
        {
            for (int i = 0; i < Bits.Length; i++)
                if (Bits[i] == old)
                    Bits[i] = @new;
        }

		/// <summary>
		/// Scrolls the image horizontally to the left by <paramref name="amount"/> pixels.
		/// </summary>
		/// <param name="amount">The number of pixels to scroll by. Positive is left, negative is right.</param>
		public void ScrollHorizontal(int amount)
		{
			byte[] newBits = new byte[Bits.Length];
			while (amount < 0)
				amount += Width;
			amount %= Width;
			if (amount == 0) return;
			int copy1src = amount;
			int copy1dst = 0;
			int copy1len = Width - amount;
			int copy2src = 0;
			int copy2dst = Width - amount;
			int copy2len = amount;
			for (int y = 0; y < Height; y++)
			{
				Array.Copy(Bits, copy1src, newBits, copy1dst, copy1len);
				Array.Copy(Bits, copy2src, newBits, copy2dst, copy2len);
				copy1src += Width;
				copy1dst += Width;
				copy2src += Width;
				copy2dst += Width;
			}
			Bits = newBits;
		}

		/// <summary>
		/// Scrolls each row in the image horizontally to the left by <paramref name="amounts"/> pixels.
		/// </summary>
		/// <param name="amounts">The number of pixels to scroll each row by. Positive is left, negative is right.</param>
		public void ScrollHorizontal(int[] amounts)
		{
			byte[] newBits = new byte[Bits.Length];
			for (int i = 0; i < amounts.Length; i++)
			{
				while (amounts[i] < 0)
					amounts[i] += Width;
				amounts[i] %= Width;
			}
			int rowStart = 0;
			for (int y = 0; y < Height; y++)
			{
				int amount = amounts[y % amounts.Length];
				Array.Copy(Bits, rowStart + amount, newBits, rowStart, Width - amount);
				if (amount != 0)
					Array.Copy(Bits, rowStart, newBits, rowStart + Width - amount, amount);
				rowStart += Width;
			}
			Bits = newBits;
		}

        public static BitmapBits ReadPCX(string filename) { Color[] palette; return ReadPCX(filename, out palette); }

        public static BitmapBits ReadPCX(string filename, out Color[] palette)
        {
            using (FileStream fs = File.OpenRead(filename))
                return ReadPCX(fs, out palette);
        }

        public static BitmapBits ReadPCX(Stream stream) { Color[] palette; return ReadPCX(stream, out palette); }

        public static BitmapBits ReadPCX(Stream stream, out Color[] palette)
        {
            BinaryReader br = new BinaryReader(stream);
            stream.Seek(8, SeekOrigin.Current);
            BitmapBits pix = new BitmapBits(br.ReadUInt16() + 1, br.ReadUInt16() + 1);
            stream.Seek(0x36, SeekOrigin.Current);
            int stride = br.ReadUInt16();
            byte[] buffer = new byte[stride];
            stream.Seek(0x3C, SeekOrigin.Current);
            for (int y = 0; y < pix.Height; y++)
            {
                int i = 0;
                while (i < stride)
                {
                    int run = 1;
                    byte val = br.ReadByte();
                    if ((val & 0xC0) == 0xC0)
                    {
                        run = val & 0x3F;
                        val = br.ReadByte();
                    }
                    for (int r = 0; r < run; r++)
                        buffer[i++] = val;
                }
                for (int x = 0; x < pix.Width; x++)
                    pix[x, y] = buffer[x];
            }
            palette = new Color[256];
            if (br.ReadByte() != 0xC) return pix;
            for (int i = 0; i < 256; i++)
                palette[i] = Color.FromArgb(br.ReadByte(), br.ReadByte(), br.ReadByte());
            return pix;
        }
    }
}