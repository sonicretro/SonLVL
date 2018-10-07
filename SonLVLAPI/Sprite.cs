using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SonicRetro.SonLVL.API
{
	public class Sprite
	{
		private List<PixelStrip> strips;
		public System.Collections.ObjectModel.ReadOnlyCollection<PixelStrip> Strips => new System.Collections.ObjectModel.ReadOnlyCollection<PixelStrip>(strips);
		private Rectangle bounds;
		public Rectangle Bounds => bounds;
		public Point Location => bounds.Location;
		public Size Size => bounds.Size;
		public int X => bounds.X;
		public int Y => bounds.Y;
		public int Width => bounds.Width;
		public int Height => bounds.Height;
		public int Left => bounds.Left;
		public int Top => bounds.Top;
		public int Right => bounds.Right;
		public int Bottom => bounds.Bottom;

		public Sprite(BitmapBits lowImg, int xoff, int yoff)
		{
			strips = new List<PixelStrip>();
			LoadBitmap(lowImg, false, xoff, yoff);
			CalculateBounds();
		}

		public Sprite(BitmapBits lowImg, Point offset)
			: this(lowImg, offset.X, offset.Y) { }

		public Sprite(BitmapBits lowImg)
			: this(lowImg, 0, 0) { }

		public Sprite(BitmapBits lowImg, BitmapBits highImg, int xoff, int yoff)
		{
			strips = new List<PixelStrip>();
			LoadBitmap(lowImg, false, xoff, yoff);
			LoadBitmap(highImg, true, xoff, yoff);
			CalculateBounds();
		}

		public Sprite(BitmapBits lowImg, BitmapBits highImg, Point offset)
			: this(lowImg, highImg, offset.X, offset.Y) { }

		public Sprite(BitmapBits lowImg, BitmapBits highImg)
			: this(lowImg, highImg, 0, 0) { }

		public Sprite(BitmapBits img, bool priority, int xoff, int yoff)
		{
			strips = new List<PixelStrip>();
			LoadBitmap(img, priority, xoff, yoff);
			CalculateBounds();
		}

		public Sprite(BitmapBits img, bool priority, Point offset)
			: this(img, priority, offset.X, offset.Y) { }

		public Sprite(BitmapBits img, bool priority)
			: this(img, priority, 0, 0) { }

		public Sprite(Sprite sprite)
		{
			strips = new List<PixelStrip>(sprite.strips.Count);
			foreach (PixelStrip strip in sprite.strips)
				strips.Add(new PixelStrip(strip));
			bounds = sprite.bounds;
		}

		public Sprite(Sprite sprite, int xoff, int yoff)
			: this(sprite)
		{
			Offset(xoff, yoff);
		}

		public Sprite(Sprite sprite, Point offset)
			: this(sprite, offset.X, offset.Y) { }

		public Sprite(Sprite sprite, Size offset)
			: this(sprite, offset.Width, offset.Height) { }

		public Sprite(Sprite sprite, bool xflip, bool yflip)
			: this(sprite)
		{
			Flip(xflip, yflip);
		}

		public Sprite(Sprite sprite, int xoff, int yoff, bool xflip, bool yflip)
			: this(sprite, xflip, yflip)
		{
			Offset(xoff, yoff);
		}

		public Sprite(Sprite sprite, Point offset, bool xflip, bool yflip)
			: this(sprite, offset.X, offset.Y, xflip, yflip) { }

		public Sprite(Sprite sprite, Size offset, bool xflip, bool yflip)
			: this(sprite, offset.Width, offset.Height, xflip, yflip) { }

		public Sprite(params Sprite[] sprites)
			: this((IEnumerable<Sprite>)sprites)
		{
		}

		public Sprite(IEnumerable<Sprite> sprites)
		{
			strips = new List<PixelStrip>();
			bool first = true;
			foreach (Sprite spr in sprites)
				if (first)
				{
					strips = new List<PixelStrip>(spr.strips.Count);
					foreach (PixelStrip strip in spr.strips)
						strips.Add(new PixelStrip(strip));
					first = false;
				}
				else
				{
					List<PixelStrip> newstrips = new List<PixelStrip>();
					foreach (PixelStrip strip in spr.strips)
					{
						var overlap = new List<PixelStrip>(strips.Where(a => a.Priority == strip.Priority && a.Y == strip.Y && a.X < strip.X + strip.Width && strip.X < a.X + a.Width));
						if (overlap.Count > 0)
						{
							foreach (PixelStrip s in overlap)
								strips.Remove(s);
							overlap.Add(strip);
							newstrips.Add(new PixelStrip(overlap));
						}
						else
							newstrips.Add(new PixelStrip(strip));
					}
					strips.AddRange(newstrips);
					strips.Sort();
				}
			CalculateBounds();
		}

		private void LoadBitmap(BitmapBits source, bool priority, int xoff, int yoff)
		{
			int i = 0;
			for (int y = 0; y < source.Height; y++)
			{
				int? starti = null;
				int startx = 0;
				for (int x = 0; x < source.Width; x++)
				{
					if (source.Bits[i] != 0)
					{
						if (!starti.HasValue)
						{
							starti = i;
							startx = x;
						}
					}
					else if (starti.HasValue)
					{
						byte[] pix = new byte[i - starti.Value];
						Array.Copy(source.Bits, starti.Value, pix, 0, pix.Length);
						strips.Add(new PixelStrip(pix, priority, startx + xoff, y + yoff));
						starti = null;
					}
					i++;
				}
				if (starti.HasValue)
				{
					byte[] pix = new byte[i - starti.Value];
					Array.Copy(source.Bits, starti.Value, pix, 0, pix.Length);
					strips.Add(new PixelStrip(pix, priority, startx + xoff, y + yoff));
				}
			}
			strips.Sort();
		}

		private void CalculateBounds()
		{
			int l = int.MaxValue;
			int t = int.MaxValue;
			int r = int.MinValue;
			int b = int.MinValue;
			foreach (PixelStrip strip in strips)
			{
				l = Math.Min(l, strip.X);
				t = Math.Min(t, strip.Y);
				r = Math.Max(r, strip.X + strip.Width);
				b = Math.Max(b, strip.Y + 1);
			}
			bounds = Rectangle.FromLTRB(l, t, r, b);
		}

		public void Offset(int x, int y)
		{
			if (x == 0 && y == 0) return;
			foreach (PixelStrip strip in strips)
			{
				strip.X += x;
				strip.Y += y;
			}
			bounds.Offset(x, y);
		}

		public void Offset(Point pt) => Offset(pt.X, pt.Y);

		public void Offset(Size sz) => Offset(sz.Width, sz.Height);

		public void InvertPriority()
		{
			foreach (PixelStrip strip in strips)
				strip.Priority = !strip.Priority;
			strips.Sort();
		}

		public void Flip(bool xflip, bool yflip)
		{
			if (!xflip && !yflip) return;
			foreach (PixelStrip strip in strips)
				strip.Flip(xflip, yflip);
			bounds = bounds.Flip(xflip, yflip);
			strips.Sort();
		}

		public BitmapBits GetBitmap()
		{
			BitmapBits result = new BitmapBits(Size);
			result.DrawSprite(this, -X, -Y);
			return result;
		}

		public BitmapBits GetBitmapLow()
		{
			BitmapBits result = new BitmapBits(Size);
			result.DrawSpriteLow(this, -X, -Y);
			return result;
		}

		public BitmapBits GetBitmapHigh()
		{
			BitmapBits result = new BitmapBits(Size);
			result.DrawSpriteHigh(this, -X, -Y);
			return result;
		}
	}

	public class PixelStrip : IComparable<PixelStrip>
	{
		public byte[] Pixels { get; private set; }
		public bool Priority { get; set; }
		public Point Location { get => location; set => location = value; }
		public int X { get => location.X; set => location.X = value; }
		public int Y { get => location.Y; set => location.Y = value; }
		public int Width => Pixels.Length;
		public Rectangle Bounds => new Rectangle(X, Y, Width, 1);

		private Point location;

		public PixelStrip(byte[] pixels, bool priority, int x, int y)
		{
			Pixels = pixels;
			Priority = priority;
			location = new Point(x, y);
		}

		public PixelStrip(byte[] pixels, bool priority, Point location)
			: this(pixels, priority, location.X, location.Y) { }

		public PixelStrip(PixelStrip source)
		{
			Pixels = (byte[])source.Pixels.Clone();
			Priority = source.Priority;
			location = source.location;
		}

		public PixelStrip(params PixelStrip[] strips)
			: this((IEnumerable<PixelStrip>)strips) { }

		public PixelStrip(IEnumerable<PixelStrip> strips)
		{
			Priority = strips.First().Priority;
			Y = strips.First().Y;
			if (!strips.All(a => a.Priority == Priority && a.Y == Y)) throw new ArgumentException("All strips must have the same priority and Y position.");
			X = strips.Min(a => a.X);
			Pixels = new byte[strips.Max(a => a.X + a.Width) - X];
			foreach (PixelStrip strip in strips)
				Array.Copy(strip.Pixels, 0, Pixels, strip.X - X, strip.Width);
		}

		public void Flip(bool xflip, bool yflip)
		{
			if (xflip)
			{
				location.X = -(Pixels.Length + location.X);
				Array.Reverse(Pixels);
			}
			if (yflip) location.Y = -location.Y - 1;
		}

		public int CompareTo(PixelStrip other)
		{
			int result = Priority.CompareTo(other.Priority);
			if (result == 0)
			{
				result = Y.CompareTo(other.Y);
				if (result == 0)
					result = X.CompareTo(other.X);
			}
			return result;
		}
	}
}
