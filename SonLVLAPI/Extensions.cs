using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace SonicRetro.SonLVL.API
{
    public static class Extensions
    {
        public static T GetValueOrDefault<T>(this T[,] array, int x, int y)
        {
            if (x < array.GetLength(0) & y < array.GetLength(1))
                return array[x, y];
            return default(T);
        }

        public static byte[] ReadNybbles(this BinaryReader br, int count)
        {
            int bytes = count / 2;
            if ((count & 1) == 1) bytes++;
            return br.ReadBytes(bytes);
        }

        public static void IncrementIndexes(this BitmapBits bmp, int amount)
        {
            for (int i = 0; i < bmp.Bits.Length; i++)
                if (bmp.Bits[i] > 0) bmp.Bits[i] = (byte)(bmp.Bits[i] + amount);
        }

        public static Bitmap To32bpp(this Bitmap bmp)
        {
            if (LevelData.IsWindows) return bmp.Clone(new Rectangle(Point.Empty, bmp.Size), PixelFormat.Format32bppArgb);
            else return new Bitmap(bmp);
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

        public static TValue GetValueOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue @default)
        {
            TValue output;
            if (dict.TryGetValue(key, out output))
                return output;
            return @default;
        }
    }
}