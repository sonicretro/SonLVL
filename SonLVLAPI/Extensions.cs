using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Globalization;

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

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue @default)
        {
            TValue output;
            if (dict.TryGetValue(key, out output))
                return output;
            return @default;
        }

        public static TKey GetKey<TKey, TValue>(this IDictionary<TKey, TValue> dict, TValue value)
        {
            bool found = false;
            TKey result = default(TKey);
            foreach (KeyValuePair<TKey, TValue> item in dict)
                if (item.Value.Equals(value))
                {
                    found = true;
                    result = item.Key;
                }
            if (found) return result;
            throw new KeyNotFoundException();
        }

        public static TKey GetKeyOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TValue value, TKey @default)
        {
            foreach (KeyValuePair<TKey, TValue> item in dict)
                if (item.Value.Equals(value))
                    @default = item.Key;
            return @default;
        }

        public static Dictionary<TValue, TKey> Swap<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            Dictionary<TValue, TKey> result = new Dictionary<TValue, TKey>(dict.Count);
            foreach (KeyValuePair<TKey, TValue> item in dict)
                result.Add(item.Value, item.Key);
            return result;
        }

        public static string MakeIdentifier(this string name)
        {
            StringBuilder result = new StringBuilder();
            foreach (char item in name)
                if ((item >= '0' & item <= '9') | (item >= 'A' & item <= 'Z') | (item >= 'a' & item <= 'z') | item == '_')
                    result.Append(item);
            if (result[0] >= '0' & result[0] <= '9')
                result.Insert(0, '_');
            return result.ToString();
        }

        public static string ToHex68k(this byte number)
        {
            if (number < 10)
                return number.ToString(NumberFormatInfo.InvariantInfo);
            else
                return "$" + number.ToString("X");
        }

        public static string ToHex68k(this sbyte number)
        {
            if (number > -1)
                if (number < 10)
                    return number.ToString(NumberFormatInfo.InvariantInfo);
                else
                    return "$" + number.ToString("X");
            else if (number == sbyte.MinValue)
                return "$80";
            else
                return "-" + Math.Abs(number).ToHex68k();
        }

        public static string ToHex68k(this ushort number)
        {
            if (number < 10)
                return number.ToString(NumberFormatInfo.InvariantInfo);
            else
                return "$" + number.ToString("X");
        }

        public static string ToHex68k(this short number)
        {
            if (number > -1)
                if (number < 10)
                    return number.ToString(NumberFormatInfo.InvariantInfo);
                else
                    return "$" + number.ToString("X");
            else if (number == short.MinValue)
                return "$8000";
            else
                return "-" + Math.Abs(number).ToHex68k();
        }

		public static bool ArrayEqual<T>(this T[] arr1, T[] arr2)
		{
			if (arr1 == arr2) return true;
			if (arr1.Length != arr2.Length) return false;
			for (int i = 0; i < arr1.Length; i++)
				if (!arr1[i].Equals(arr2[i]))
					return false;
			return true;
		}

		private static unsafe bool FastArrayEqualInternal(void* fp1, void* fp2, int length)
		{
			ulong* lp1 = (ulong*)fp1;
			ulong* lp2 = (ulong*)fp2;
			int longlen = length / 8;
			for (int i = 0; i < longlen; i++)
				if (*lp1++ != *lp2++) return false;
			if ((length & 7) != 0)
			{
				byte* bp1 = (byte*)lp1;
				byte* bp2 = (byte*)lp2;
				if ((length & 4) == 4)
					if (*(uint*)bp1 != *(uint*)bp2)
						return false;
					else
					{
						bp1 += 4;
						bp2 += 4;
					}
				if ((length & 2) == 2)
					if (*(ushort*)bp1 != *(ushort*)bp2)
						return false;
					else
					{
						bp1 += 2;
						bp2 += 2;
					}
				if ((length & 1) == 1)
					return *bp1 != *bp2;
			}
			return true;
		}

		public static unsafe bool FastArrayEqual(this byte[] arr1, byte[] arr2)
		{
			if (arr1 == arr2) return true;
			if (arr1.Length != arr2.Length) return false;
			fixed (byte* fp1 = arr1, fp2 = arr2)
				return FastArrayEqualInternal(fp1, fp2, arr1.Length);
		}

		public static unsafe bool FastArrayEqual(this sbyte[] arr1, sbyte[] arr2)
		{
			if (arr1 == arr2) return true;
			if (arr1.Length != arr2.Length) return false;
			fixed (sbyte* fp1 = arr1, fp2 = arr2)
				return FastArrayEqualInternal(fp1, fp2, arr1.Length);
		}

		public static unsafe bool FastArrayEqual(this ushort[] arr1, ushort[] arr2)
		{
			if (arr1 == arr2) return true;
			if (arr1.Length != arr2.Length) return false;
			fixed (ushort* fp1 = arr1, fp2 = arr2)
				return FastArrayEqualInternal(fp1, fp2, arr1.Length * 2);
		}

		public static unsafe bool FastArrayEqual(this short[] arr1, short[] arr2)
		{
			if (arr1 == arr2) return true;
			if (arr1.Length != arr2.Length) return false;
			fixed (short* fp1 = arr1, fp2 = arr2)
				return FastArrayEqualInternal(fp1, fp2, arr1.Length * 2);
		}

		public static unsafe bool FastArrayEqual(this uint[] arr1, uint[] arr2)
		{
			if (arr1 == arr2) return true;
			if (arr1.Length != arr2.Length) return false;
			fixed (uint* fp1 = arr1, fp2 = arr2)
				return FastArrayEqualInternal(fp1, fp2, arr1.Length * 4);
		}

		public static unsafe bool FastArrayEqual(this int[] arr1, int[] arr2)
		{
			if (arr1 == arr2) return true;
			if (arr1.Length != arr2.Length) return false;
			fixed (int* fp1 = arr1, fp2 = arr2)
				return FastArrayEqualInternal(fp1, fp2, arr1.Length * 4);
		}

		public static unsafe bool FastArrayEqual(this char[] arr1, char[] arr2)
		{
			if (arr1 == arr2) return true;
			if (arr1.Length != arr2.Length) return false;
			fixed (char* fp1 = arr1, fp2 = arr2)
				return FastArrayEqualInternal(fp1, fp2, arr1.Length * 2);
		}

		public static bool ListEqual<T>(this IList<T> lst1, IList<T> lst2)
		{
			if (lst1 == lst2) return true;
			if (lst1.Count != lst2.Count) return false;
			for (int i = 0; i < lst1.Count; i++)
				if (!lst1[i].Equals(lst2[i]))
					return false;
			return true;
		}

		public static void Fill<T>(this T[] arr, T item, int startIndex, int length)
		{
			if (length == 0) return;
			if (startIndex < 0 || startIndex >= arr.Length) throw new ArgumentOutOfRangeException("startIndex");
			if (length < 0 || startIndex + length >= arr.Length) throw new ArgumentOutOfRangeException("length");
			for (int i = startIndex; i < startIndex + length; i++)
				arr[i] = item;
		}

		public static void Fill<T>(this T[] arr, T item)
		{
			for (int i = 0; i < arr.Length; i++)
				arr[i] = item;
		}

		private static unsafe void FastFillInternal(void* fp, ulong item, int length)
		{
			ulong* lp = (ulong*)fp;
			int longlen = length / 8;
			for (int i = 0; i < longlen; i++)
				*lp++ = item;
			if ((length & 7) != 0)
			{
				byte* bp = (byte*)lp;
				if ((length & 4) == 4)
				{
					*(uint*)bp = (uint)item;
					bp += 4;
				}
				if ((length & 2) == 2)
				{
					*(ushort*)bp = (ushort)item;
					bp += 2;
				}
				if ((length & 1) == 1)
					*bp = (byte)item;
			}
		}

		public static unsafe void FastFill(this byte[] arr, byte value, int startIndex, int length)
		{
			if (length == 0) return;
			if (startIndex < 0 || startIndex >= arr.Length) throw new ArgumentOutOfRangeException("startIndex");
			if (length < 0 || startIndex + length >= arr.Length) throw new ArgumentOutOfRangeException("length");
			ulong longval = (ulong)value;
			longval |= longval << 8;
			longval |= longval << 16;
			longval |= longval << 32;
			fixed (byte* fp = arr)
				FastFillInternal(fp + startIndex, longval, length);
		}

		public static unsafe void FastFill(this byte[] arr, byte value)
		{
			ulong longval = (ulong)value;
			longval |= longval << 8;
			longval |= longval << 16;
			longval |= longval << 32;
			fixed (byte* fp = arr)
				FastFillInternal(fp, longval, arr.Length);
		}

		public static unsafe void FastFill(this sbyte[] arr, sbyte value, int startIndex, int length)
		{
			if (length == 0) return;
			if (startIndex < 0 || startIndex >= arr.Length) throw new ArgumentOutOfRangeException("startIndex");
			if (length < 0 || startIndex + length >= arr.Length) throw new ArgumentOutOfRangeException("length");
			ulong longval = (ulong)value;
			longval |= longval << 8;
			longval |= longval << 16;
			longval |= longval << 32;
			fixed (sbyte* fp = arr)
				FastFillInternal(fp + startIndex, longval, length);
		}

		public static unsafe void FastFill(this sbyte[] arr, sbyte value)
		{
			ulong longval = (ulong)value;
			longval |= longval << 8;
			longval |= longval << 16;
			longval |= longval << 32;
			fixed (sbyte* fp = arr)
				FastFillInternal(fp, longval, arr.Length);
		}

		public static unsafe void FastFill(this ushort[] arr, ushort value, int startIndex, int length)
		{
			if (length == 0) return;
			if (startIndex < 0 || startIndex >= arr.Length) throw new ArgumentOutOfRangeException("startIndex");
			if (length < 0 || startIndex + length >= arr.Length) throw new ArgumentOutOfRangeException("length");
			ulong longval = (ulong)value;
			longval |= longval << 8;
			longval |= longval << 16;
			longval |= longval << 32;
			fixed (ushort* fp = arr)
				FastFillInternal(fp + startIndex, longval, length * 2);
		}

		public static unsafe void FastFill(this ushort[] arr, ushort value)
		{
			ulong longval = (ulong)value;
			longval |= longval << 8;
			longval |= longval << 16;
			longval |= longval << 32;
			fixed (ushort* fp = arr)
				FastFillInternal(fp, longval, arr.Length * 2);
		}

		public static unsafe void FastFill(this short[] arr, short value, int startIndex, int length)
		{
			if (length == 0) return;
			if (startIndex < 0 || startIndex >= arr.Length) throw new ArgumentOutOfRangeException("startIndex");
			if (length < 0 || startIndex + length >= arr.Length) throw new ArgumentOutOfRangeException("length");
			ulong longval = (ulong)value;
			longval |= longval << 8;
			longval |= longval << 16;
			longval |= longval << 32;
			fixed (short* fp = arr)
				FastFillInternal(fp + startIndex, longval, length * 2);
		}

		public static unsafe void FastFill(this short[] arr, short value)
		{
			ulong longval = (ulong)value;
			longval |= longval << 8;
			longval |= longval << 16;
			longval |= longval << 32;
			fixed (short* fp = arr)
				FastFillInternal(fp, longval, arr.Length * 2);
		}

		public static unsafe void FastFill(this uint[] arr, uint value, int startIndex, int length)
		{
			if (length == 0) return;
			if (startIndex < 0 || startIndex >= arr.Length) throw new ArgumentOutOfRangeException("startIndex");
			if (length < 0 || startIndex + length >= arr.Length) throw new ArgumentOutOfRangeException("length");
			ulong longval = (ulong)value;
			longval |= longval << 8;
			longval |= longval << 16;
			longval |= longval << 32;
			fixed (uint* fp = arr)
				FastFillInternal(fp + startIndex, longval, length * 4);
		}

		public static unsafe void FastFill(this uint[] arr, uint value)
		{
			ulong longval = (ulong)value;
			longval |= longval << 8;
			longval |= longval << 16;
			longval |= longval << 32;
			fixed (uint* fp = arr)
				FastFillInternal(fp, longval, arr.Length * 4);
		}

		public static unsafe void FastFill(this int[] arr, int value, int startIndex, int length)
		{
			if (length == 0) return;
			if (startIndex < 0 || startIndex >= arr.Length) throw new ArgumentOutOfRangeException("startIndex");
			if (length < 0 || startIndex + length >= arr.Length) throw new ArgumentOutOfRangeException("length");
			ulong longval = (ulong)value;
			longval |= longval << 8;
			longval |= longval << 16;
			longval |= longval << 32;
			fixed (int* fp = arr)
				FastFillInternal(fp + startIndex, longval, length * 4);
		}

		public static unsafe void FastFill(this int[] arr, int value)
		{
			ulong longval = (ulong)value;
			longval |= longval << 8;
			longval |= longval << 16;
			longval |= longval << 32;
			fixed (int* fp = arr)
				FastFillInternal(fp, longval, arr.Length * 4);
		}

		public static unsafe void FastFill(this char[] arr, char value, int startIndex, int length)
		{
			if (length == 0) return;
			if (startIndex < 0 || startIndex >= arr.Length) throw new ArgumentOutOfRangeException("startIndex");
			if (length < 0 || startIndex + length >= arr.Length) throw new ArgumentOutOfRangeException("length");
			ulong longval = (ulong)value;
			longval |= longval << 8;
			longval |= longval << 16;
			longval |= longval << 32;
			fixed (char* fp = arr)
				FastFillInternal(fp + startIndex, longval, length * 2);
		}

		public static unsafe void FastFill(this char[] arr, char value)
		{
			ulong longval = (ulong)value;
			longval |= longval << 8;
			longval |= longval << 16;
			longval |= longval << 32;
			fixed (char* fp = arr)
				FastFillInternal(fp, longval, arr.Length * 2);
		}
	}
}