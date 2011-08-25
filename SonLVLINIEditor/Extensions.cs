using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SonLVLINIEditor
{
    public static class Extensions
    {
        public static void ShowHide(this System.Windows.Forms.Control ctrl)
        {
            if (ctrl.Visible)
                ctrl.Hide();
            else
                ctrl.Show();
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