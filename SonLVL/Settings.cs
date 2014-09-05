using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SonicRetro.SonLVL.API;

namespace SonicRetro.SonLVL
{
    public class Settings
    {
        private static string filename;

        [DefaultValue(true)]
        public bool ShowHUD { get; set; }
        public string Emulator { get; set; }
        [IniCollection(IniCollectionMode.SingleLine, Format = "|")]
        public List<string> MRUList { get; set; }
        public bool ShowGrid { get; set; }
        [IniIgnore]
        public Color GridColor { get; set; }
        [IniName("GridColor")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Color? GridColorInternal
        {
            get { return GridColor; }
            set { GridColor = value ?? Color.Red; }
        }
		[IniName("ObjectGridSize")]
		public int ObjectGridSizeInternal
		{
			get { return 1 << ObjectGridSize; }
			set
			{
				ObjectGridSize = (byte)Math.Max(0, Math.Min(8, Math.Log(2, value)));
			}
		}
		[IniIgnore]
		public byte ObjectGridSize { get; set; }
        public string Username { get; set; }
        public bool IncludeObjectsInForegroundSelection { get; set; }
        [DefaultValue(true)]
        public bool TransparentBackFGBGExport { get; set; }
        public bool IncludeObjectsFGExport { get; set; }
        public bool HideDebugObjectsExport { get; set; }
        public bool UseHexadecimalIndexesExport { get; set; }
        public bool ObjectsAboveHighPlane { get; set; }
        [DefaultValue(true)]
        public bool ViewLowPlane { get; set; }
        [DefaultValue(true)]
        public bool ViewHighPlane { get; set; }
        public CollisionPath ViewCollision { get; set; }
        public bool ViewAngles { get; set; }
        public bool ViewAllTimeZones { get; set; }
        [DefaultValue("1x")]
        public string ZoomLevel { get; set; }
        public Tab CurrentTab { get; set; }
        public WindowMode WindowMode { get; set; }
        [DefaultValue(true)]
        public bool ShowMenu { get; set; }

        public static Settings Load()
        {
            filename = Path.Combine(Application.StartupPath, "SonLVL.ini");
            if (File.Exists(filename))
                return IniFile.Deserialize<Settings>(filename);
            else
            {
                Settings result = new Settings();
                // Import old style settings
                // Any new settings should not be added to the old settings class, and should have defaults applied here.
                Properties.Settings oldset = Properties.Settings.Default;
                result.ShowHUD = oldset.ShowHUD;
                result.Emulator = oldset.Emulator;
                result.MRUList = new List<string>();
                if (oldset.MRUList != null)
                    foreach (string item in oldset.MRUList)
                        if (!string.IsNullOrEmpty(item))
                            result.MRUList.Add(item);
                result.ShowGrid = oldset.ShowGrid;
                result.GridColor = oldset.GridColor;
                result.Username = oldset.Username;
                result.IncludeObjectsInForegroundSelection = oldset.IncludeObjectsInForegroundSelection;
                result.TransparentBackFGBGExport = true;
                result.ViewLowPlane = result.ViewHighPlane = true;
                result.ZoomLevel = "1x";
                result.ShowMenu = true;
                return result;
            }
        }

        public void Save()
        {
            IniFile.Serialize(this, filename);
        }
    }

    public enum CollisionPath
    {
        None,
        Path1,
        Path2
    }

    public enum Tab
    {
        Objects,
        Foreground,
        Background,
        Chunks,
        Blocks,
        Tiles,
        Solids
    }

    public enum WindowMode
    {
        Normal,
        Maximized,
        Fullscreen
    }
}