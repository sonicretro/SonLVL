using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace SonicRetro.SonLVL.GUI
{
    public static class Extensions
    {
        public static Bitmap To32bpp(this Bitmap bmp)
        {
            if (Program.IsWindows) return bmp.Clone(new Rectangle(Point.Empty, bmp.Size), PixelFormat.Format32bppArgb);
            else return new Bitmap(bmp);
        }

        /// <summary>
        /// Resizes the <see cref="Bitmap" />, maintaining the original aspect ratio.
        /// </summary>
        public static Bitmap Resize(this Bitmap image, Size newsize)
        {
            Bitmap bmp = new Bitmap(newsize.Width, newsize.Height);
            Graphics gfx = Graphics.FromImage(bmp);
            gfx.CompositingQuality = CompositingQuality.HighQuality;
            gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
            gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
            gfx.SmoothingMode = SmoothingMode.HighQuality;
            gfx.Clear(Color.Transparent);
            int mywidth = image.Width;
            int myheight = image.Height;
            while (myheight > newsize.Height | mywidth > newsize.Width)
            {
                if (mywidth > newsize.Width)
                {
                    mywidth = newsize.Width;
                    myheight = (int)(image.Height * ((double)newsize.Width / image.Width));
                }
                else if (myheight > newsize.Height)
                {
                    myheight = newsize.Height;
                    mywidth = (int)(image.Width * ((double)newsize.Height / image.Height));
                }
            }
            gfx.DrawImage(image, (int)(((double)newsize.Width - mywidth) / 2), (int)(((double)newsize.Height - myheight) / 2), mywidth, myheight);
            return bmp;
        }

        /// <summary>
        /// Sets options to enable faster rendering.
        /// </summary>
        public static void SetOptions(this Graphics gfx)
        {
            gfx.CompositingQuality = CompositingQuality.HighQuality;
            gfx.InterpolationMode = InterpolationMode.NearestNeighbor;
            gfx.PixelOffsetMode = PixelOffsetMode.None;
            gfx.SmoothingMode = SmoothingMode.HighSpeed;
        }

        public static void ShowHide(this System.Windows.Forms.Control ctrl)
        {
            if (ctrl.Visible)
                ctrl.Hide();
            else
                ctrl.Show();
        }

        public static Color Blend(this Color back, Color blend)
        {
            double A = blend.A / 255d;
            return Color.FromArgb(255, (int)(((1 - A) * back.R) + (A * blend.R)), (int)(((1 - A) * back.G) + (A * blend.G)), (int)(((1 - A) * back.B) + (A * blend.B)));
        }

        public static Rectangle Scale(this Rectangle r, int factor)
        {
            return new Rectangle(r.X * factor, r.Y * factor, r.Width * factor, r.Height * factor);
        }

		public static Rectangle Scale(this Rectangle r, int h, int v)
		{
			return new Rectangle(r.X * h, r.Y * v, r.Width * h, r.Height * v);
		}

		public static void Swap<T>(this IList<T> list, int a, int b)
		{
			T tmp = list[a];
			list[a] = list[b];
			list[b] = tmp;
		}

		public static void Move<T>(this IList<T> list, int src, int dst)
		{
			T tmp = list[src];
			list.Insert(dst, tmp);
			list.RemoveAt(src > dst ? src + 1 : src);
		}

		public static void AddOrSet<T>(this IList<T> list, int index, T item)
		{
			if (list.Count <= index)
				list.Add(item);
			else
				list[index] = item;
		}
    }
}