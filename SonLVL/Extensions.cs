using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SonicRetro.SonLVL
{
    public static class Extensions
    {
        public static void Flip(this Bitmap bmp, bool XFlip, bool YFlip)
        {
            if (!XFlip & !YFlip) return;
            if (XFlip & YFlip)
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipXY);
            else if (XFlip)
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
            else if (YFlip)
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
        }

        public static void DrawImageFlipped(this Graphics gfx, Bitmap bmp, int x, int y, bool XFlip, bool YFlip)
        {
            int bmpw = bmp.Width;
            int bmph = bmp.Height;
            Rectangle rect = new Rectangle(x, y, bmpw, bmph);
            Rectangle blockrect = new Rectangle();
            if (XFlip)
            {
                blockrect.X = bmpw;
                blockrect.Width = -bmpw;
            }
            else
            {
                blockrect.X = 0;
                blockrect.Width = bmpw;
            }
            if (YFlip)
            {
                blockrect.Y = bmph;
                blockrect.Height = -bmph;
            }
            else
            {
                blockrect.Y = 0;
                blockrect.Height = bmph;
            }
            gfx.DrawImage(bmp, rect, blockrect, GraphicsUnit.Pixel);
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

        public static Bitmap Copy(this Bitmap bmp)
        {
            BitmapData bmpd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
            Bitmap newbmp = new Bitmap(bmpd.Width, bmpd.Height, bmpd.PixelFormat);
            BitmapData newbmpd = newbmp.LockBits(new Rectangle(0, 0, bmpd.Width, bmpd.Height), ImageLockMode.WriteOnly, bmpd.PixelFormat);
            byte[] bytes = new byte[Math.Abs(bmpd.Stride) * bmpd.Height];
            Marshal.Copy(bmpd.Scan0, bytes, 0, bytes.Length);
            Marshal.Copy(bytes, 0, newbmpd.Scan0, bytes.Length);
            bmp.UnlockBits(bmpd);
            newbmp.UnlockBits(newbmpd);
            newbmp.Palette = bmp.Palette;
            return newbmp;
        }

        internal static void IncrementIndexes(this SonicRetro.SonLVL.BitmapBits bmp, int amount)
        {
            for (int i = 0; i < bmp.Bits.Length; i++)
                if (bmp.Bits[i] > 0) bmp.Bits[i] = (byte)(bmp.Bits[i] + amount);
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue @default)
        {
            TValue output;
            if (dict.TryGetValue(key, out output))
                return output;
            return @default;
        }

        public static Color Blend(this Color back, Color blend)
        {
            double A = blend.A / 255d;
            return Color.FromArgb(255, (int)(((1 - A) * back.R) + (A * blend.R)), (int)(((1 - A) * back.G) + (A * blend.G)), (int)(((1 - A) * back.B) + (A * blend.B)));
        }

        public static Color FindNearestMatch(this Color col, params Color[] palette)
        {
            Color nearest_color = Color.Empty;
            double distance = 250000;
            foreach (Color o in palette)
            {
                double dbl_test_red = Math.Pow(o.R - col.R, 2.0);
                double dbl_test_green = Math.Pow(o.G - col.G, 2.0);
                double dbl_test_blue = Math.Pow(o.B - col.B, 2.0);
                double temp = dbl_test_blue + dbl_test_green + dbl_test_red;
                if (temp == 0.0)
                {
                    nearest_color = o;
                    break;
                }
                else if (temp < distance)
                {
                    distance = temp;
                    nearest_color = o;
                }
            }
            return nearest_color;
        }

        public static Rectangle Scale(this Rectangle r, int factor)
        {
            return new Rectangle(r.X * factor, r.Y * factor, r.Width * factor, r.Height * factor);
        }
    }
}