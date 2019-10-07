using SonicRetro.SonLVL.API;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SonicRetro.SonLVL.SonPLN
{
	public class Settings
	{
		private static string filename;

		[IniCollection(IniCollectionMode.SingleLine, Format = "|")]
		public List<string> MRUList { get; set; }
		public bool ShowGrid { get; set; }
		[IniIgnore]
		public Color GridColor { get; set; }
		[IniName("GridColor")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Color? GridColorInternal
		{
			get => GridColor;
			set => GridColor = value ?? Color.Red;
		}
		public string Username { get; set; }
		[DefaultValue(true)]
		public bool TransparentBackgroundExport { get; set; }
		public bool UseHexadecimalIndexesExport { get; set; }
		public bool ExportArtCollisionPriority { get; set; }
		[DefaultValue("1x")]
		public string ZoomLevel { get; set; }
		public Tab CurrentTab { get; set; }
		public WindowMode WindowMode { get; set; }
		[DefaultValue(true)]
		public bool ShowMenu { get; set; }
		[DefaultValue(true)]
		public bool EnableDraggingPalette { get; set; }
		[DefaultValue(true)]
		public bool EnableDraggingTiles { get; set; }

		public static Settings Load()
		{
			filename = Path.Combine(Application.StartupPath, "SonPLN.ini");
			if (File.Exists(filename))
				return IniSerializer.Deserialize<Settings>(filename);
			else
			{
				Settings result = new Settings
				{
					MRUList = new List<string>(),
					GridColor = Color.Red,
					TransparentBackgroundExport = true,
					ZoomLevel = "1x",
					ShowMenu = true,
					EnableDraggingPalette = true,
					EnableDraggingTiles = true
				};
				return result;
			}
		}

		public void Save()
		{
			IniSerializer.Serialize(this, filename);
		}
	}

	public enum Tab
	{
		Foreground,
		Art
	}

	public enum WindowMode
	{
		Normal,
		Maximized,
		Fullscreen
	}
}
