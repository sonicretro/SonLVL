using SonicRetro.SonLVL.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace SonicRetro.SonLVL.SonPLN
{
	public class GameInfo
	{
		[IniAlwaysInclude]
		[IniName("version")]
		public EngineVersion EngineVersion { get; set; }
		[IniName("tilecmp")]
		[DefaultValue(CompressionType.Nemesis)]
		public CompressionType TileCompression { get; set; }
		[IniName("mapcmp")]
		[DefaultValue(CompressionType.Enigma)]
		public CompressionType MappingsCompression { get; set; }
		[IniName("palettefmt")]
		public EngineVersion PaletteFormat { get; set; }
		[IniCollection(IniCollectionMode.IndexOnly)]
		public Dictionary<string, LevelInfo> Levels { get; set; }

		public static GameInfo Load(string filename)
		{
			return IniSerializer.Deserialize<GameInfo>(filename);
		}

		public void Save(string filename)
		{
			IniSerializer.Serialize(this, filename);
		}

		public LevelInfo GetLevelInfo(string levelName)
		{
			LevelInfo info = Levels[levelName];
			if (levelName.Contains("\\"))
				levelName = levelName.Substring(levelName.LastIndexOf('\\') + 1);
			LevelInfo result = new LevelInfo();
			foreach (PropertyInfo prop in typeof(LevelInfo).GetProperties())
			{
				if (prop.PropertyType == typeof(EngineVersion))
					prop.SetValue(result, EngineVersion, null);
				object val;
				PropertyInfo gam = typeof(GameInfo).GetProperty(prop.Name, prop.PropertyType);
				if (gam != null)
				{
					val = gam.GetValue(this, null);
					if (!Equals(val, prop.PropertyType.GetDefaultValue()))
						prop.SetValue(result, val, null);
				}
				val = prop.GetValue(info, null);
				if (!Equals(val, prop.PropertyType.GetDefaultValue()))
					prop.SetValue(result, val, null);
			}
			if (result.DisplayName == null) result.DisplayName = levelName;
			result.Palettes = new NamedPaletteList[info.ExtraPalettes.Length + 1];
			result.Palettes[0] = new NamedPaletteList("Normal", info.Palette);
			if (info.ExtraPalettes.Length > 0)
				info.ExtraPalettes.CopyTo(result.Palettes, 1);
			return result;
		}
	}

	public class LevelInfo
	{
		[IniName("displayname")]
		public string DisplayName { get; set; }
		[IniName("version")]
		public EngineVersion EngineVersion { get; set; }
		[IniName("tilecmp")]
		public CompressionType TileCompression { get; set; }
		[IniName("tiles")]
		[IniCollection(IniCollectionMode.SingleLine, Format = "|")]
		public API.FileInfo[] Tiles { get; set; }
		[IniName("mapcmp")]
		public CompressionType MappingsCompression { get; set; }
		[IniName("mappings")]
		public string Mappings { get; set; }
		[IniName("tileoff")]
		[TypeConverter(typeof(Int32HexConverter))]
		public int TileOffset { get; set; }
		[IniName("palettefmt")]
		public EngineVersion PaletteFormat { get; set; }
		[IniIgnore]
		public NamedPaletteList[] Palettes { get; set; }
		[IniName("palette")]
		public PaletteList Palette { get; set; }
		[IniName("palette")]
		[IniCollection(IniCollectionMode.NoSquareBrackets, StartIndex = 2)]
		public NamedPaletteList[] ExtraPalettes { get; set; }
		[IniName("width")]
		public int Width { get; set; }
		[IniName("height")]
		public int Height { get; set; }
		[IniName("textmap")]
		public string TextMapping { get; set; }
		[IniName("textmapfile")]
		public string TextMappingFile { get; set; }
		[IniName("bgcolorline")]
		public int BgPalLine { get; set; }
		[IniName("bgcolorindex")]
		public int BgPalIndex { get; set; }
	}

	public class TextMapping
	{
		public TextMapping() { }

		public TextMapping(string map)
		{
			Height = 1;
			DefaultWidth = 1;
			Characters = new Dictionary<char, CharMapInfo>();
			int i = 0;
			ushort tile = 0;
			while (i < map.Length)
			{
				char start = map[i++];
				char? end = null;
				if (i < map.Length && map[i] == '-')
				{
					++i;
					end = map[i++];
				}
				if (i < map.Length && map[i] == ':')
				{
					++i;
					int pipe = map.IndexOf('|', i);
					string num;
					if (pipe == -1)
						num = map.Substring(i);
					else
						num = map.Substring(i, pipe - i);
					i += num.Length;
					if (num.StartsWith("0x"))
						tile = ushort.Parse(num.Substring(2), NumberStyles.HexNumber);
					else
						tile = ushort.Parse(num, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
				}
				if (end.HasValue)
					for (char c = start; c <= end.Value; c++)
						Characters[c] = new CharMapInfo() { Map = new ushort[1, 1] { { tile++ } } };
				else
					Characters[start] = new CharMapInfo() { Map = new ushort[1, 1] { { tile++ } } };
				if (i < map.Length && map[i] != '|')
					throw new System.FormatException("Invalid text mapping string.");
				++i;
			}
		}

		public int Height { get; set; }
		public int DefaultWidth { get; set; }
		[IniCollection(IniCollectionMode.IndexOnly)]
		public Dictionary<char, CharMapInfo> Characters { get; set; }
	}

	public class CharMapInfo
	{
		public int? Width { get; set; }
		[IniIgnore]
		public ushort[,] Map { get; set; }
		[IniName("Row")]
		[IniCollection(IniCollectionMode.NoSquareBrackets, StartIndex = 1)]
		public string[] RowsText
		{
			get
			{
				string[] result = new string[Map.GetLength(1)];
				for (int y = 0; y < Map.GetLength(1); y++)
				{
					string[] tmp = new string[Map.GetLength(0)];
					for (int x = 0; x < Map.GetLength(0); x++)
						tmp[x] = $"0x{Map[x, y]:X}";
					result[y] = string.Join(", ", tmp);
				}
				return result;
			}
			set
			{
				Map = new ushort[value[0].Split(new[] { ", " }, StringSplitOptions.None).Length, value.Length];
				for (int y = 0; y < Map.GetLength(1); y++)
				{
					string[] tmp = value[y].Split(new[] { ", " }, StringSplitOptions.None);
					for (int x = 0; x < Map.GetLength(0); x++)
						if (tmp[x].StartsWith("0x"))
							Map[x, y] = ushort.Parse(tmp[x].Substring(2), NumberStyles.HexNumber);
						else
							Map[x, y] = ushort.Parse(tmp[x], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
				}
			}
		}
	}

	public class Int32HexConverter : TypeConverter
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string) && value is int ival)
			{
				if (ival < 10)
					return ival.ToString(NumberFormatInfo.InvariantInfo);
				return $"0x{ival:X}";
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string str)
			{
				if (str.StartsWith("0x"))
					return int.Parse(str.Substring(2), NumberStyles.HexNumber);
				else
					return int.Parse(str, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override bool IsValid(ITypeDescriptorContext context, object value)
		{
			if (value is int)
				return true;
			if (value is string)
				return int.TryParse((string)value, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out int i);
			return base.IsValid(context, value);
		}
	}
}
