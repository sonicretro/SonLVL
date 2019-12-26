using SonicRetro.SonLVL.API;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace SonicRetro.SonLVL
{
	public class AnimationInfo
	{
		[DefaultValue(EngineVersion.S2)]
		[IniName("game")]
		public EngineVersion Game { get; set; }
		[IniName("art")]
		[IniCollection(IniCollectionMode.SingleLine, Format = "|")]
		public SonicRetro.SonLVL.API.FileInfo[] Art { get; set; }
		[IniName("artcmp")]
		[DefaultValue(CompressionType.Nemesis)]
		public CompressionType ArtCompression { get; set; }
		[IniName("palette")]
		public PaletteList Palette { get; set; }
		[IniName("map")]
		public string MappingsFile { get; set; }
		[IniName("mapgame")]
		public EngineVersion MappingsGame { get; set; }
		[IniName("mapfmt")]
		public MappingsFormat MappingsFormat { get; set; }
		[IniName("dplc")]
		public string DPLCFile { get; set; }
		[IniName("dplcgame")]
		public EngineVersion DPLCGame { get; set; }
		[IniName("dplcfmt")]
		public MappingsFormat DPLCFormat { get; set; }
		[IniName("anim")]
		public string AnimationFile { get; set; }
		[IniName("animfmt")]
		public MappingsFormat AnimationFormat { get; set; }
		[IniName("startpal")]
		public int StartPalette { get; set; }

		public static Dictionary<string, AnimationInfo> Load(string filename)
		{
			Dictionary<string, Dictionary<string, string>> ini = IniFile.Load(filename);
			string userfile = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename) + ".user" + Path.GetExtension(filename));
			if (File.Exists(userfile))
				ini = IniFile.Combine(ini, IniFile.Load(userfile));
			Dictionary<string, AnimationInfo> result = IniSerializer.Deserialize<Dictionary<string, AnimationInfo>>(ini);
			foreach (KeyValuePair<string, AnimationInfo> anim in result)
			{
				if (anim.Value.MappingsGame == EngineVersion.Invalid)
					anim.Value.MappingsGame = anim.Value.Game;
				if (anim.Value.DPLCGame == EngineVersion.Invalid)
					anim.Value.DPLCGame = anim.Value.Game;
				if (anim.Value.MappingsFormat == MappingsFormat.Invalid)
					anim.Value.MappingsFormat = MappingsFormat.Binary;
				if (anim.Value.DPLCFormat == MappingsFormat.Invalid)
					anim.Value.DPLCFormat = MappingsFormat.Binary;
				if (anim.Value.AnimationFormat == MappingsFormat.Invalid)
					anim.Value.AnimationFormat = MappingsFormat.Binary;
			}
			return result;
		}
	}
}
