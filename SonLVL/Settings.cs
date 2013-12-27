using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SonicRetro.SonLVL.API;
using System.ComponentModel;

namespace SonicRetro.SonLVL
{
    public class Settings
    {
        private static string filename;

        public bool ShowHUD { get; set; }
        public string Emulator { get; set; }
        [IniCollection(IniCollectionMode.SingleLine, Format = "|")]
        public List<string> MRUList { get; set; }
        public bool ShowGrid { get; set; }
        public Color GridColor { get; set; }
        public string Username { get; set; }
        public bool IncludeObjectsInForegroundSelection { get; set; }

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
                return result;
            }
        }

        public void Save()
        {
            IniFile.Serialize(this, filename);
        }
    }
}