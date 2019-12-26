using SonicRetro.SonLVL.API;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace ChaotixSpriteEdit
{
	[Serializable]
	public struct Sprite
	{
		public Point Offset;
		public BitmapBits Image;
		public int X { get { return Offset.X; } set { Offset.X = value; } }
		public int Y { get { return Offset.Y; } set { Offset.Y = value; } }
		public int Width => Image.Width;
		public int Height => Image.Height;
		public Size Size => Image.Size;
		public int Left => X;
		public int Top => Y;
		public int Right => X + Width;
		public int Bottom => Y + Height;
		public Rectangle Bounds => new Rectangle(Offset, Size);

		public Sprite(BitmapBits spr, Point off)
		{
			Image = spr;
			Offset = off;
		}

		public Sprite(Sprite sprite)
		{
			Image = new BitmapBits(sprite.Image);
			Offset = sprite.Offset;
		}

		public Sprite(params Sprite[] sprites)
			: this((IEnumerable<Sprite>)sprites)
		{
		}

		public Sprite(IEnumerable<Sprite> sprites)
		{
			List<Sprite> sprlst = new List<Sprite>(sprites);
			int left = 0;
			int right = 0;
			int top = 0;
			int bottom = 0;
			bool first = true;
			foreach (Sprite spr in sprlst)
				if (spr.Image != null)
					if (first)
					{
						left = spr.Left;
						right = spr.Right;
						top = spr.Top;
						bottom = spr.Bottom;
						first = false;
					}
					else
					{
						left = Math.Min(spr.Left, left);
						right = Math.Max(spr.Right, right);
						top = Math.Min(spr.Top, top);
						bottom = Math.Max(spr.Bottom, bottom);
					}
			Offset = new Point(left, top);
			Image = new BitmapBits(right - left, bottom - top);
			for (int i = 0; i < sprlst.Count; i++)
				if (sprlst[i].Image != null)
				{
					bool comp = false;
					for (int j = 0; j < i; j++)
						if (sprlst[j].Image != null && sprlst[i].Bounds.IntersectsWith(sprlst[j].Bounds))
						{
							comp = true;
							break;
						}
					if (comp)
						Image.DrawBitmapComposited(sprlst[i].Image, new Point(sprlst[i].X - left, sprlst[i].Y - top));
					else
						Image.DrawBitmap(sprlst[i].Image, new Point(sprlst[i].X - left, sprlst[i].Y - top));
				}
		}

		private Rectangle GetUsedRange()
		{
			int x;
			int y;
			Rectangle result = new Rectangle();

			// get first used pixel at left side
			for (x = 0; x < Width; x++)
			{
				for (y = 0; y < Height; y++)
				{
					if (Image[x, y] > 0)
						break;
				}
				if (y < Height)
					break;
			}
			result.X = x;

			// get first used pixel at right side
			for (x = Width - 1; x >= result.X; x--)
			{
				for (y = 0; y < Height; y++)
				{
					if (Image[x, y] > 0)
						break;
				}
				if (y < Height)
					break;
			}
			result.Width = x + 1 - result.X;

			// get first used pixel at top side
			for (y = 0; y < Height; y++)
			{
				for (x = 0; x < Width; x++)
				{
					if (Image[x, y] > 0)
						break;
				}
				if (x < Width)
					break;
			}
			result.Y = y;

			// get first used pixel at bottom side
			for (y = Height - 1; y >= result.Y; y--)
			{
				for (x = 0; x < Width; x++)
				{
					if (Image[x, y] > 0)
						break;
				}
				if (x < Width)
					break;
			}
			result.Height = y + 1 - result.Y;
			return result;
		}

		public Sprite Crop(Rectangle rect)
		{
			if (rect.Width == Width && rect.Height == Height)
				return this;
			if (rect.Width == 0 && rect.Height == 0)
				return new Sprite();    // return empty sprite

			BitmapBits newimg = new BitmapBits(rect.Width, rect.Height);
			newimg.DrawBitmapBounded(Image, -rect.X, -rect.Y);
			return new Sprite(newimg, new Point(X + rect.X, Y + rect.Y));
		}

		public Sprite Trim()
		{
			Rectangle used = GetUsedRange();
			if (used.Width == 0 || used.Height == 0)
				return this;    // don't trim fully transparent images (else we won't be able to select them)
			else
				return Crop(used);
		}

		public void Flip(bool xflip, bool yflip)
		{
			Image.Flip(xflip, yflip);
			if (xflip) X = -(Width + X);
			if (yflip) Y = -(Height + Y);
		}

		public static Sprite LoadChaotixSprite(string filename) { return LoadChaotixSprite(File.ReadAllBytes(filename), 0); }

		public static Sprite LoadChaotixSprite(byte[] file, int addr)
		{
			short left = ByteConverter.ToInt16(file, addr);
			addr += 2;
			short right = ByteConverter.ToInt16(file, addr);
			addr += 2;
			sbyte top = (sbyte)file[addr];
			addr += 2;
			sbyte bottom = (sbyte)file[addr];
			addr += 2;
			BitmapBits bmp = new BitmapBits(right - left + 1, bottom - top + 1);
			sbyte y;
			while (true)
			{
				sbyte xl = (sbyte)file[addr++];
				sbyte xr = (sbyte)file[addr++];
				if (xl == 0 && xr == 0) break;
				y = (sbyte)file[addr];
				addr += 2;
				Array.Copy(file, addr, bmp.Bits, bmp.GetPixelIndex(xl - left, y - top), xr - xl);
				addr += xr - xl;
			}
			return new Sprite(bmp, new Point(left, top));
		}

		public void SaveChaotixSprite(string filename)
		{
			List<byte> result = new List<byte>();
			int left = Left & ~1;
			int right = (Right & 1) == 1 ? Right + 1 : Right;
			result.AddRange(ByteConverter.GetBytes((short)left));
			result.AddRange(ByteConverter.GetBytes((short)(right - 1)));
			result.Add((byte)(sbyte)Top);
			result.Add((byte)0);
			result.Add((byte)(sbyte)(Bottom - 1));
			result.Add((byte)0);
			for (int y = 0; y < Height; y++)
			{
				int xl = -1;
				for (int x = 0; x < Width; x++)
					if (Image[x, y] != 0)
					{
						xl = x;
						break;
					}
				if (xl == -1) continue;
				int xr = 0;
				for (int x = Width - 1; x >= xl; x--)
					if (Image[x, y] != 0)
					{
						xr = x + 1;
						break;
					}
				xl &= ~1;
				if ((xr - xl) % 2 != 0)
					++xr;
				result.Add((byte)(sbyte)(xl + left));
				result.Add((byte)(sbyte)(xr + left));
				result.Add((byte)(sbyte)(y + Top));
				result.Add((byte)0);
				for (int x = xl; x < xr; x++)
					result.Add((byte)(x < Width ? Image[x, y] : 0));
			}
			result.AddRange(ByteConverter.GetBytes((short)0));
			if (result.Count % 4 != 0)
				result.AddRange(new byte[4 - (result.Count % 4)]);
			File.WriteAllBytes(filename, result.ToArray());
		}
	}
}
