using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;

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

		public int GetPixelIndex(int x, int y) => (y * Width) + x;

		public byte this[int x, int y]
		{
			get => Bits[GetPixelIndex(x, y)];
			set => Bits[GetPixelIndex(x, y)] = value;
		}

		public void SafeSetPixel(byte index, int x, int y)
		{
			if (x >= 0 && x < Width && y >= 0 && y < Height)
				this[x, y] = index;
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

		public BitmapBits(Bitmap bmp) => LoadBitmap(bmp);

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
			ToBitmapInternal(newbmp);
			return newbmp;
		}

		public void ToBitmap(Bitmap destination)
		{
			if (destination.Width != Width)
				throw new ArgumentException("Width of destination bitmap must be equal to current bitmap.");
			if (destination.Height != Height)
				throw new ArgumentException("Height of destination bitmap must be equal to current bitmap.");
			if (destination.PixelFormat != PixelFormat.Format8bppIndexed)
				throw new ArgumentException("Destination bitmap's pixel format must be 8bpp.");
			ToBitmapInternal(destination);
		}

		private void ToBitmapInternal(Bitmap destination)
		{
			BitmapData newbmpd = destination.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
			if (newbmpd.Stride == newbmpd.Width)
				Marshal.Copy(Bits, 0, newbmpd.Scan0, Bits.Length);
			else
			{
				byte[] bmpbits = new byte[Math.Abs(newbmpd.Stride) * newbmpd.Height];
				for (int y = 0; y < Height; y++)
					Array.Copy(Bits, y * Width, bmpbits, y * Math.Abs(newbmpd.Stride), Width);
				Marshal.Copy(bmpbits, 0, newbmpd.Scan0, bmpbits.Length);
			}
			destination.UnlockBits(newbmpd);
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
			ToBitmap4bppInternal(newbmp);
			return newbmp;
		}

		public void ToBitmap4bpp(Bitmap destination)
		{
			if (destination.Width != Width)
				throw new ArgumentException("Width of destination bitmap must be equal to current bitmap.");
			if (destination.Height != Height)
				throw new ArgumentException("Height of destination bitmap must be equal to current bitmap.");
			if (destination.PixelFormat != PixelFormat.Format4bppIndexed)
				throw new ArgumentException("Destination bitmap's pixel format must be 4bpp.");
			ToBitmap4bppInternal(destination);
		}

		private void ToBitmap4bppInternal(Bitmap destination)
		{
			BitmapData newbmpd = destination.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format4bppIndexed);
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
			destination.UnlockBits(newbmpd);
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
			ToBitmap1bppInternal(newbmp);
			return newbmp;
		}

		public void ToBitmap1bpp(Bitmap destination)
		{
			if (destination.Width != Width)
				throw new ArgumentException("Width of destination bitmap must be equal to current bitmap.");
			if (destination.Height != Height)
				throw new ArgumentException("Height of destination bitmap must be equal to current bitmap.");
			if (destination.PixelFormat != PixelFormat.Format1bppIndexed)
				throw new ArgumentException("Destination bitmap's pixel format must be 1bpp.");
			ToBitmap1bppInternal(destination);
		}

		private void ToBitmap1bppInternal(Bitmap destination)
		{
			BitmapData newbmpd = destination.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);
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
			destination.UnlockBits(newbmpd);
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
			if (x == 0 && source.Width == Width)
			{
				source.Bits.CopyTo(Bits, GetPixelIndex(0, y));
				return;
			}
			for (int i = 0; i < source.Height; i++)
			{
				int di = GetPixelIndex(x, y + i);
				int si = i * source.Width;
				Array.Copy(source.Bits, si, Bits, di, source.Width);
			}
		}

		public void DrawBitmap(BitmapBits source, Point location) => DrawBitmap(source, location.X, location.Y);

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

		public void DrawBitmapComposited(BitmapBits source, Point location) => DrawBitmapComposited(source, location.X, location.Y);

		public void DrawBitmapBehind(BitmapBits source, int x, int y)
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
					if (this[x + r, y + c] == 0)
						this[x + r, y + c] = source[r, c];
		}

		public void DrawBitmapBehind(BitmapBits source, Point location) => DrawBitmapBehind(source, location.X, location.Y);

		public void DrawBitmapBounded(BitmapBits source, int x, int y)
		{
			if (x >= 0 && y >= 0 && x + source.Width <= Width && y + source.Height <= Height)
			{
				DrawBitmap(source, x, y);
				return;
			}
			int srct = 0;
			if (y < 0)
				srct = -y;
			int srcb = source.Height;
			if (srcb > Height - y)
				srcb = Height - y;
			if (x == 0 && source.Width == Width)
			{
				Array.Copy(source.Bits, source.GetPixelIndex(0, srct), Bits, GetPixelIndex(0, y + srct), GetPixelIndex(0, srcb - srct));
				return;
			}
			int srcl = 0;
			if (x < 0)
				srcl = -x;
			int srcr = source.Width;
			if (srcr > Width - x)
				srcr = Width - x;
			if (srcr > srcl)
				for (int c = srct; c < srcb; c++)
					Array.Copy(source.Bits, source.GetPixelIndex(srcl, c), Bits, GetPixelIndex(x + srcl, y + c), srcr - srcl);
		}

		public void DrawBitmapBounded(BitmapBits source, Point location) => DrawBitmapComposited(source, location.X, location.Y);

		public void DrawSprite(Sprite sprite, int x, int y)
		{
			foreach (PixelStrip strip in sprite.Strips)
				DrawStrip(strip, x, y);
		}

		public void DrawSprite(Sprite sprite, Point location) => DrawSprite(sprite, location.X, location.Y);

		public void DrawSprite(Sprite sprite) => DrawSprite(sprite, 0, 0);

		public void DrawSpriteLow(Sprite sprite, int x, int y)
		{
			foreach (PixelStrip strip in sprite.Strips.Where(a => a.Priority == false))
				DrawStrip(strip, x, y);
		}

		public void DrawSpriteLow(Sprite sprite, Point location) => DrawSpriteLow(sprite, location.X, location.Y);

		public void DrawSpriteLow(Sprite sprite) => DrawSpriteLow(sprite, 0, 0);

		public void DrawSpriteHigh(Sprite sprite, int x, int y)
		{
			foreach (PixelStrip strip in sprite.Strips.Where(a => a.Priority == true))
				DrawStrip(strip, x, y);
		}

		public void DrawSpriteHigh(Sprite sprite, Point location) => DrawSpriteHigh(sprite, location.X, location.Y);

		public void DrawSpriteHigh(Sprite sprite) => DrawSpriteHigh(sprite, 0, 0);

		private void DrawStrip(PixelStrip strip, int x, int y)
		{
			int sty = strip.Y + y;
			if (sty < 0 || sty >= Height) return;
			int stx = strip.X + x;
			int srcl = 0;
			if (stx < 0)
				srcl = -stx;
			int srcr = strip.Width;
			if (srcr > Width - stx)
				srcr = Width - stx;
			if (srcr > srcl)
				Array.Copy(strip.Pixels, srcl, Bits, GetPixelIndex(stx + srcl, sty), srcr - srcl);
		}

		public void ReplaceColor(byte old, byte @new)
		{
			for (int i = 0; i < Bits.Length; i++)
				if (Bits[i] == old)
					Bits[i] = @new;
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

		public static BitmapBits FromTileInterlaced(byte[] art, int index)
		{
			BitmapBits bmp = new BitmapBits(8, 16);
			if (index * 32 + 64 <= art.Length)
			{
				for (int i = 0; i < 64; i++)
				{
					bmp.Bits[i * 2] = (byte)(art[i + (index * 32)] >> 4);
					bmp.Bits[(i * 2) + 1] = (byte)(art[i + (index * 32)] & 0xF);
				}
			}
			return bmp;
		}

		public byte[] ToTile(int x, int y)
		{
			List<byte> res = new List<byte>();
			for (int _y = 0; _y < 8; _y++)
				for (int _x = 0; _x < 8; _x += 2)
					res.Add((byte)(((this[x + _x, y + _y] & 0xF) << 4) | (this[x + _x + 1, y + _y] & 0xF)));
			return res.ToArray();
		}

		public byte[] ToTile(Point pnt) => ToTile(pnt.X, pnt.Y);

		public byte[] ToTile() => ToTile(0, 0);

		public byte[] ToTileInterlaced(int x, int y)
		{
			List<byte> res = new List<byte>();
			for (int _y = 0; _y < 16; _y++)
				for (int _x = 0; _x < 8; _x += 2)
					res.Add((byte)(((this[x + _x, y + _y] & 0xF) << 4) | (this[x + _x + 1, y +_y] & 0xF)));
			return res.ToArray();
		}

		public byte[] ToTileInterlaced(Point pnt) => ToTileInterlaced(pnt.X, pnt.Y);

		public byte[] ToTileInterlaced() => ToTileInterlaced(0, 0);

		public void Rotate(int R)
		{
			byte[] tmppix = new byte[Bits.Length];
			switch (R & 3)
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
			if (factor < 1)
				throw new ArgumentOutOfRangeException("factor", "Scaling factor must be 1 or greater.");
			if (factor == 1)
				return new BitmapBits(this);
			BitmapBits res = new BitmapBits(Width * factor, Height * factor);
			int srcaddr = 0;
			int dstaddr = 0;
			for (int y = 0; y < Height; y++)
			{
				int linestart = dstaddr;
				for (int x = 0; x < Width; x++)
				{
					res.Bits.FastFill(Bits[srcaddr++], dstaddr, factor);
					dstaddr += factor;
				}
				for (int i = 1; i < factor; i++)
				{
					Array.Copy(res.Bits, linestart, res.Bits, dstaddr, res.Width);
					dstaddr += res.Width;
				}
			}
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
				if (x1 >= Width || x2 < 0)
					return;
				x1 = Math.Max(x1, 0);
				x2 = Math.Min(x2, Width - 1);
				Bits.FastFill(index, GetPixelIndex(x1, y1), x2 - x1 + 1);
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
				if (y1 >= Height || y2 < 0)
					return;
				y1 = Math.Max(y1, 0);
				y2 = Math.Min(y2, Height - 1);
				int end = GetPixelIndex(x1, y2);
				for (int i = GetPixelIndex(x1, y1); i <= end; i += Width)
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
			if (y1 < y2)
				ystep = 1;
			else
				ystep = -1;
			for (int x = x1; x <= x2; x++)
			{
				if (steep)
					SafeSetPixel(index, y, x);
				else
					SafeSetPixel(index, x, y);
				error += deltaerr;
				if (error >= 0.5)
				{
					y += ystep;
					error -= 1.0;
				}
			}
		}

		public void DrawLine(byte index, Point p1, Point p2) => DrawLine(index, p1.X, p1.Y, p2.X, p2.Y);

		public void DrawRectangle(byte index, int x, int y, int width, int height)
		{
			DrawLine(index, x, y, x + width, y);
			DrawLine(index, x, y, x, y + height);
			DrawLine(index, x + width, y, x + width, y + height);
			DrawLine(index, x, y + height, x + width, y + height);
		}

		public void DrawRectangle(byte index, Rectangle rect) => DrawRectangle(index, rect.X, rect.Y, rect.Width, rect.Height);

		public void FillRectangle(byte index, int x, int y, int width, int height)
		{
			if (x > Width) return;
			if (y > Height) return;
			if (x + width <= 0) return;
			if (y + height <= 0) return;
			int srcl = Math.Max(x, 0);
			int srct = Math.Max(y, 0);
			int srcr = Math.Min(x + width, Width);
			int srcb = Math.Min(y + height, Height);
			int start = srct * Width + srcl;
			if (srcl == 0 && srcr == Width)
				Bits.FastFill(index, start, srcb * Width - start);
			else
			{
				int length = srcr - srcl;
				for (int cy = srct; cy < srcb; cy++)
				{
					Bits.FastFill(index, start, length);
					start += Width;
				}
			}
		}

		public void FillRectangle(byte index, Point loc, Size size) => FillRectangle(index, loc.X, loc.Y, size.Width, size.Height);

		public void FillRectangle(byte index, Rectangle rect) => FillRectangle(index, rect.X, rect.Y, rect.Width, rect.Height);

		public void DrawCircle(byte index, int x, int y, int radius)
		{
			int cx = -radius, cy = 0, err = 2 - 2 * radius; /* II. Quadrant */
			do
			{
				SafeSetPixel(index, x - cx, y + cy); /*   I. Quadrant */
				SafeSetPixel(index, x - cy, y - cx); /*  II. Quadrant */
				SafeSetPixel(index, x + cx, y - cy); /* III. Quadrant */
				SafeSetPixel(index, x + cy, y + cx); /*  IV. Quadrant */
				radius = err;
				if (radius <= cy) err += ++cy * 2 + 1;           /* e_xy+e_y < 0 */
				if (radius > cx || err > cy) err += ++cx * 2 + 1; /* e_xy+e_x > 0 or no 2nd y-step */
			} while (cx < 0);
		}

		public void DrawCircle(byte index, Point loc, int radius) => DrawCircle(index, loc.X, loc.Y, radius);

		public void DrawEllipse(byte index, int x1, int y1, int x2, int y2)
		{
			int a = Math.Abs(x2 - x1), b = Math.Abs(y2 - y1), b1 = b & 1; /* values of diameter */
			long dx = 4 * (1 - a) * b * b, dy = 4 * (b1 + 1) * a * a; /* error increment */
			long err = dx + dy + b1 * a * a, e2; /* error of 1.step */

			if (x1 > x2) { x1 = x2; x2 += a; } /* if called with swapped points */
			if (y1 > y2) y1 = y2; /* .. exchange them */
			y1 += (b + 1) / 2; y2 = y1 - b1;   /* starting pixel */
			a *= 8 * a; b1 = 8 * b * b;

			do
			{
				SafeSetPixel(index, x2, y1); /*   I. Quadrant */
				SafeSetPixel(index, x1, y1); /*  II. Quadrant */
				SafeSetPixel(index, x1, y2); /* III. Quadrant */
				SafeSetPixel(index, x2, y2); /*  IV. Quadrant */
				e2 = 2 * err;
				if (e2 <= dy) { y1++; y2--; err += dy += a; }  /* y step */
				if (e2 >= dx || 2 * err > dy) { x1++; x2--; err += dx += b1; } /* x step */
			} while (x1 <= x2);

			while (y1 - y2 < b)
			{  /* too early stop of flat ellipses a=1 */
				SafeSetPixel(index, x1 - 1, y1); /* -> finish tip of ellipse */
				SafeSetPixel(index, x2 + 1, y1++);
				SafeSetPixel(index, x1 - 1, y2);
				SafeSetPixel(index, x2 + 1, y2--);
			}
		}

		public void DrawEllipse(byte index, Point p1, Point p2) => DrawEllipse(index, p1.X, p1.Y, p2.X, p2.Y);

		public void DrawEllipse(byte index, Rectangle rect) => DrawEllipse(index, rect.Left, rect.Top, rect.Right, rect.Bottom);

		public override bool Equals(object obj)
		{
			if (base.Equals(obj)) return true;
			BitmapBits other = obj as BitmapBits;
			if (other == null) return false;
			if (Width != other.Width | Height != other.Height) return false;
			return Bits.FastArrayEqual(other.Bits);
		}

		public void DrawBezier(byte index, int x1, int y1, int x2, int y2, int x3, int y3)
		{
			int sx = x3 - x2, sy = y3 - y2;
			long xx = x1 - x2, yy = y1 - y2, xy;         /* relative values for checks */
			double dx, dy, err, cur = xx * sy - yy * sx;                    /* curvature */

			if (xx * sx <= 0 && yy * sy <= 0) throw new ArgumentOutOfRangeException();  /* sign of gradient must not change */

			if (sx * (long)sx + sy * (long)sy > xx * xx + yy * yy)
			{ /* begin with longer part */
				x3 = x1; x1 = sx + x2; y3 = y1; y1 = sy + y2; cur = -cur;  /* swap P0 P2 */
			}
			if (cur != 0)
			{                                    /* no straight line */
				xx += sx; xx *= sx = x1 < x3 ? 1 : -1;           /* x step direction */
				yy += sy; yy *= sy = y1 < y3 ? 1 : -1;           /* y step direction */
				xy = 2 * xx * yy; xx *= xx; yy *= yy;          /* differences 2nd degree */
				if (cur * sx * sy < 0)
				{                           /* negated curvature? */
					xx = -xx; yy = -yy; xy = -xy; cur = -cur;
				}
				dx = 4.0 * sy * cur * (x2 - x1) + xx - xy;             /* differences 1st degree */
				dy = 4.0 * sx * cur * (y1 - y2) + yy - xy;
				xx += xx; yy += yy; err = dx + dy + xy;                /* error 1st step */
				do
				{
					SafeSetPixel(index, x1, y1);                                     /* plot curve */
					if (x1 == x3 && y1 == y3) return;  /* last pixel -> curve finished */
					y2 = (2 * err < dx) ? 1 : 0;                  /* save value for test of y step */
					if (2 * err > dy) { x1 += sx; dx -= xy; err += dy += yy; } /* x step */
					if (y2 != 0) { y1 += sy; dy -= xy; err += dx += xx; } /* y step */
				} while (dy < dx);           /* gradient negates -> algorithm fails */
			}
			DrawLine(index, x1, y1, x3, y3);                  /* plot remaining part to end */
		}

		public void DrawBezier(byte index, Point p1, Point p2, Point p3) => DrawBezier(index, p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y);

		public void DrawGraphX(byte index, int xmin, int xmax, int yoff, Func<int, int> func)
		{
			Point? last = null;
			for (int x = xmin; x <= xmax; x++)
			{
				Point cur = new Point(x, func(x) + yoff);
				if (last.HasValue)
					DrawLine(index, last.Value, cur);
				SafeSetPixel(index, x, cur.Y);
				last = cur;
			}
		}

		public void DrawGraphY(byte index, int ymin, int ymax, int xoff, Func<int,int> func)
		{
			Point? last = null;
			for (int y = ymin; y <= ymax; y++)
			{
				Point cur = new Point(func(y) + xoff, y);
				if (last.HasValue)
					DrawLine(index, last.Value, cur);
				SafeSetPixel(index, cur.X, y);
				last = cur;
			}
		}

		public override int GetHashCode() => base.GetHashCode();

		public BitmapBits GetSection(int x, int y, int width, int height)
		{
			BitmapBits result = new BitmapBits(width, height);
			for (int v = 0; v < height; v++)
				Array.Copy(this.Bits, GetPixelIndex(x, y + v), result.Bits, v * width, width);
			return result;
		}

		public BitmapBits GetSection(Rectangle rect) => GetSection(rect.X, rect.Y, rect.Width, rect.Height);

		/// <summary>
		/// Scrolls the image horizontally to the left by <paramref name="amount"/> pixels.
		/// </summary>
		/// <param name="amount">The number of pixels to scroll by. Positive is left, negative is right.</param>
		public void ScrollHorizontal(int amount)
		{
			byte[] newBits = new byte[Bits.Length];
			amount %= Width;
			if (amount < 0)
				amount += Width;
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
		public void ScrollHorizontal(params int[] amounts)
		{
			byte[] newBits = new byte[Bits.Length];
			for (int i = 0; i < amounts.Length; i++)
			{
				amounts[i] %= Width;
				if (amounts[i] < 0)
					amounts[i] += Width;
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

		/// <summary>
		/// Scrolls the image vertically upwards by <paramref name="amount"/> pixels.
		/// </summary>
		/// <param name="amount">The number of pixels to scroll by. Positive is up, negative is down.</param>
		public void ScrollVertical(int amount)
		{
			byte[] newBits = new byte[Bits.Length];
			amount %= Height;
			if (amount < 0)
				amount += Height;
			if (amount == 0) return;
			int src = amount * Width;
			int dst = 0;
			for (int y = 0; y < Height; y++)
			{
				Array.Copy(Bits, src, newBits, dst, Width);
				src = (src + Width) % Bits.Length;
				dst += Width;
			}
			Bits = newBits;
		}

		/// <summary>
		/// Scrolls each column in the image vertically upwards by <paramref name="amounts"/> pixels.
		/// </summary>
		/// <param name="amounts">The number of pixels to scroll each column by. Positive is up, negative is down.</param>
		public void ScrollVertical(params int[] amounts)
		{
			byte[] newBits = new byte[Bits.Length];
			for (int i = 0; i < amounts.Length; i++)
			{
				amounts[i] %= Height;
				if (amounts[i] < 0)
					amounts[i] += Height;
			}
			for (int x = 0; x < Width; x++)
			{
				int amount = amounts[x % amounts.Length];
				int src = GetPixelIndex(x, amount);
				int dst = x;
				for (int y = 0; y < Height; y++)
				{
					newBits[dst] = Bits[src];
					src = (src + Width) % Bits.Length;
					dst += Width;
				}
			}
			Bits = newBits;
		}

		public void ScrollHV(BitmapBits destination, int dstY, int srcY, params int[] srcX)
		{
			if (dstY < 0 || dstY >= destination.Height) return;
			for (int i = 0; i < srcX.Length; i++)
			{
				srcX[i] %= Width;
				if (srcX[i] < 0)
					srcX[i] += Width;
			}
			srcY %= Height;
			if (srcY < 0)
				srcY += Height;
			int rowSrc = GetPixelIndex(0, srcY);
			int rowDst = destination.GetPixelIndex(0, dstY);
			for (int y = 0; y < destination.Height - dstY; y++)
			{
				int amount = srcX[(srcY + y) % srcX.Length];
				Array.Copy(Bits, rowSrc + amount, destination.Bits, rowDst, Math.Min(Width - amount, destination.Width));
				if (amount != 0 && Width - amount < destination.Width)
					Array.Copy(Bits, rowSrc, destination.Bits, rowDst + Width - amount, Math.Min(amount, destination.Width - (Width - amount)));
				rowSrc = (rowSrc + Width) % Bits.Length;
				rowDst += destination.Width;
			}
		}

		public unsafe void ApplyWaterPalette(int waterHeight)
		{
			if (waterHeight < 0 || waterHeight > Height) throw new ArgumentOutOfRangeException("waterHeight");
			int st = waterHeight * Width;
			int length = Bits.Length - st;
			fixed (byte* fp = Bits)
			{
				ulong* lp = (ulong*)(fp + st);
				int longlen = length / 8;
				for (int i = 0; i < longlen; i++)
					*lp++ |= 0x8080808080808080u;
				if ((length & 7) != 0)
				{
					byte* bp = (byte*)lp;
					if ((length & 4) == 4)
					{
						*(uint*)bp |= 0x80808080u;
						bp += 4;
					}
					if ((length & 2) == 2)
					{
						*(ushort*)bp |= 0x8080;
						bp += 2;
					}
					if ((length & 1) == 1)
						*bp |= 0x80;
				}
			}
		}

		public void FloodFill(byte index, int x, int y) => FloodFill(index, new Point(x, y));

		public void FloodFill(byte index, Point location)
		{
			byte oldind = this[location.X, location.Y];
			if (oldind == index) return;
			Queue<Point> pts = new Queue<Point>(Bits.Length / 2);
			pts.Enqueue(location);
			this[location.X, location.Y] = index;
			while (pts.Count > 0)
			{
				Point pt = pts.Dequeue();
				if (pt.X > 0 && this[pt.X - 1, pt.Y] == oldind)
				{
					this[pt.X - 1, pt.Y] = index;
					pts.Enqueue(new Point(pt.X - 1, pt.Y));
				}
				if (pt.X < Width - 1 && this[pt.X + 1, pt.Y] == oldind)
				{
					this[pt.X + 1, pt.Y] = index;
					pts.Enqueue(new Point(pt.X + 1, pt.Y));
				}
				if (pt.Y > 0 && this[pt.X, pt.Y - 1] == oldind)
				{
					this[pt.X, pt.Y - 1] = index;
					pts.Enqueue(new Point(pt.X, pt.Y - 1));
				}
				if (pt.Y < Height - 1 && this[pt.X, pt.Y + 1] == oldind)
				{
					this[pt.X, pt.Y + 1] = index;
					pts.Enqueue(new Point(pt.X, pt.Y + 1));
				}
			}
		}

		public static BitmapBits ReadPCX(string filename) => ReadPCX(filename, out Color[] palette);

		public static BitmapBits ReadPCX(string filename, out Color[] palette)
		{
			using (FileStream fs = File.OpenRead(filename))
				return ReadPCX(fs, out palette);
		}

		public static BitmapBits ReadPCX(Stream stream) => ReadPCX(stream, out Color[] palette);

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
