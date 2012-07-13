using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace SonicRetro.SonLVL.API
{
    public class GameInfo
    {
        [DefaultValue(EngineVersion.S2)]
        [IniName("version")]
        public EngineVersion EngineVersion;
        [IniName("levelwidthmax")]
        public int LevelWidthMax;
        [IniName("levelheightmax")]
        public int LevelHeightMax;
        [IniName("objlst")]
        public StringList ObjectList;
        [IniName("mapver")]
        public EngineVersion MappingsVersion;
        [IniName("dplcver")]
        public EngineVersion DPLCVersion;
        [IniName("tilefmt")]
        public EngineVersion TileFormat;
        [IniName("tilecmp")]
        public CompressionType TileCompression;
        [IniName("blockfmt")]
        public EngineVersion BlockFormat;
        [IniName("blockcmp")]
        public CompressionType BlockCompression;
        [IniName("chunkfmt")]
        public EngineVersion ChunkFormat;
        [IniName("chunkcmp")]
        public CompressionType ChunkCompression;
        [IniName("layoutfmt")]
        public EngineVersion LayoutFormat;
        [IniName("layoutcmp")]
        public CompressionType LayoutCompression;
        [IniName("palettefmt")]
        public EngineVersion PaletteFormat;
        [IniName("objectfmt")]
        public EngineVersion ObjectFormat;
        [IniName("objectcmp")]
        public CompressionType ObjectCompression;
        [IniName("ringfmt")]
        public EngineVersion RingFormat;
        [IniName("ringcmp")]
        public CompressionType RingCompression;
        [IniName("colindfmt")]
        public EngineVersion CollisionIndexFormat;
        [IniName("colindcmp")]
        public CompressionType CollisionIndexCompression;
        [IniName("colind")]
        public string CollisionIndex;
        [IniName("colind1")]
        public string CollisionIndex1;
        [IniName("colind2")]
        public string CollisionIndex2;
        [IniName("colarrfmt")]
        public EngineVersion CollisionArrayFormat;
        [IniName("colarrcmp")]
        public CompressionType CollisionArrayCompression;
        [IniName("colarr1")]
        public string CollisionArray1;
        [IniName("colarr2")]
        public string CollisionArray2;
        [IniName("anglefmt")]
        public EngineVersion AngleFormat;
        [IniName("anglecmp")]
        public CompressionType AngleCompression;
        [IniName("angles")]
        public string Angles;
        [IniName("buildscr")]
        public string BuildScript;
        [IniName("romfile")]
        public string ROMFile;
        [IniName("runcmd")]
        public string RunCommand;
        [IniName("useemu")]
        [DefaultValue(true)]
        public bool UseEmulator;
        [IniCollection]
        public Dictionary<string, LevelInfo> Levels;

        public static GameInfo Load(string filename)
        {
            Dictionary<string, Dictionary<string, string>> ini = IniFile.Load(filename);
            string userfile = Path.Combine(Path.GetDirectoryName(filename), Path.GetFileNameWithoutExtension(filename) + ".user" + Path.GetExtension(filename));
            if (File.Exists(userfile))
                ini = IniFile.Combine(ini, IniFile.Load(userfile));
            GameInfo result = IniFile.Deserialize<GameInfo>(ini);
            if (result.MappingsVersion == EngineVersion.Invalid)
                result.MappingsVersion = result.EngineVersion;
            if (result.DPLCVersion == EngineVersion.Invalid)
                result.DPLCVersion = result.MappingsVersion;
            return result;
        }

        public void Save(string filename)
        {
            IniFile.Serialize(this, filename);
        }

        private string[] formatFields = { "TileFormat", "BlockFormat", "ChunkFormat", "LayoutFormat", "PaletteFormat", "ObjectFormat", "RingFormat", "CollisionIndexFormat", "CollisionArrayFormat", "AngleFormat" };
        public LevelInfo GetLevelInfo(string levelName)
        {
            LevelInfo info = Levels[levelName];
            if (levelName.Contains("\\"))
                levelName = levelName.Substring(levelName.LastIndexOf('\\') + 1);
            LevelInfo result = new LevelInfo();
            result.DisplayName = info.DisplayName ?? levelName;
            foreach (string item in formatFields)
            {
                System.Reflection.FieldInfo gam = typeof(GameInfo).GetField(item);
                System.Reflection.FieldInfo lvl = typeof(LevelInfo).GetField(item);
                lvl.SetValue(result, EngineVersion);
                if ((EngineVersion)gam.GetValue(this) != EngineVersion.Invalid)
                    lvl.SetValue(result, gam.GetValue(this));
                if ((EngineVersion)lvl.GetValue(info) != EngineVersion.Invalid)
                    lvl.SetValue(result, lvl.GetValue(info));
            }
            switch (result.TileFormat)
            {
                case EngineVersion.S1:
                case EngineVersion.S2NA:
                    result.TileCompression = CompressionType.Nemesis;
                    break;
                case EngineVersion.S2:
                case EngineVersion.SBoom:
                    result.TileCompression = CompressionType.Kosinski;
                    break;
                case EngineVersion.S3K:
                case EngineVersion.SKC:
                    result.TileCompression = CompressionType.KosinskiM;
                    break;
                case EngineVersion.SCDPC:
                    result.TileCompression = CompressionType.SZDD;
                    break;
                default:
                    result.TileCompression = CompressionType.Uncompressed;
                    break;
            }
            if (TileCompression != CompressionType.Invalid)
                result.TileCompression = TileCompression;
            if (info.TileCompression != CompressionType.Invalid)
                result.TileCompression = info.TileCompression;
            result.Tiles = info.Tiles;
            switch (result.BlockFormat)
            {
                case EngineVersion.S1:
                    result.BlockCompression = CompressionType.Enigma;
                    break;
                case EngineVersion.S2:
                case EngineVersion.S3K:
                case EngineVersion.SKC:
                case API.EngineVersion.SBoom:
                    result.BlockCompression = CompressionType.Kosinski;
                    break;
                default:
                    result.BlockCompression = CompressionType.Uncompressed;
                    break;
            }
            if (BlockCompression != CompressionType.Invalid)
                result.BlockCompression = BlockCompression;
            if (info.BlockCompression != CompressionType.Invalid)
                result.BlockCompression = info.BlockCompression;
            result.Blocks = info.Blocks;
            switch (result.ChunkFormat)
            {
                case EngineVersion.S1:
                case EngineVersion.S2:
                case EngineVersion.S3K:
                case EngineVersion.SKC:
                case API.EngineVersion.SBoom:
                    result.ChunkCompression = CompressionType.Kosinski;
                    break;
                default:
                    result.ChunkCompression = CompressionType.Uncompressed;
                    break;
            }
            if (ChunkCompression != CompressionType.Invalid)
                result.ChunkCompression = ChunkCompression;
            if (info.ChunkCompression != CompressionType.Invalid)
                result.ChunkCompression = info.ChunkCompression;
            result.Chunks = info.Chunks;
            switch (result.LayoutFormat)
            {
                case EngineVersion.S2:
                case API.EngineVersion.SBoom:
                    result.LayoutCompression = CompressionType.Kosinski;
                    break;
                default:
                    result.LayoutCompression = CompressionType.Uncompressed;
                    break;
            }
            if (LayoutCompression != CompressionType.Invalid)
                result.LayoutCompression = LayoutCompression;
            if (info.LayoutCompression != CompressionType.Invalid)
                result.LayoutCompression = info.LayoutCompression;
            result.Layout = info.Layout;
            result.FGLayout = info.FGLayout;
            result.BGLayout = info.BGLayout;
            result.Palettes = new NamedPaletteList[9];
            result.Palettes[0] = new NamedPaletteList("Normal", info.Palette);
            result.Palettes[1] = info.Palette2;
            result.Palettes[2] = info.Palette3;
            result.Palettes[3] = info.Palette4;
            result.Palettes[4] = info.Palette5;
            result.Palettes[5] = info.Palette6;
            result.Palettes[6] = info.Palette7;
            result.Palettes[7] = info.Palette8;
            result.Palettes[8] = info.Palette9;
            result.Sprites = info.Sprites;
            result.ObjectList = info.ObjectList;
            result.ObjectCompression = CompressionType.Uncompressed;
            if (ObjectCompression != CompressionType.Invalid)
                result.ObjectCompression = ObjectCompression;
            if (info.ObjectCompression != CompressionType.Invalid)
                result.ObjectCompression = info.ObjectCompression;
            result.Objects = info.Objects;
            result.RingCompression = CompressionType.Uncompressed;
            if (RingCompression != CompressionType.Invalid)
                result.RingCompression = RingCompression;
            if (info.RingCompression != CompressionType.Invalid)
                result.RingCompression = info.RingCompression;
            result.Rings = info.Rings;
            result.BumperCompression = CompressionType.Uncompressed;
            if (info.BumperCompression != CompressionType.Invalid)
                result.BumperCompression = info.BumperCompression;
            result.Bumpers = info.Bumpers;
            result.StartPositions = info.StartPositions;
            switch (result.CollisionIndexFormat)
            {
                case EngineVersion.S2:
                case API.EngineVersion.SBoom:
                    result.CollisionIndexCompression = CompressionType.Kosinski;
                    break;
                default:
                    result.CollisionIndexCompression = CompressionType.Uncompressed;
                    break;
            }
            if (CollisionIndexCompression != CompressionType.Invalid)
                result.CollisionIndexCompression = CollisionIndexCompression;
            if (info.CollisionIndexCompression != CompressionType.Invalid)
                result.CollisionIndexCompression = info.CollisionIndexCompression;
            result.CollisionIndex = info.CollisionIndex ?? CollisionIndex;
            result.CollisionIndex1 = info.CollisionIndex1 ?? CollisionIndex1;
            result.CollisionIndex2 = info.CollisionIndex2 ?? CollisionIndex2;
            result.CollisionIndexSize = info.CollisionIndexSize;
            result.CollisionArrayCompression = CompressionType.Uncompressed;
            if (CollisionArrayCompression != CompressionType.Invalid)
                result.CollisionArrayCompression = CollisionArrayCompression;
            if (info.CollisionArrayCompression != CompressionType.Invalid)
                result.CollisionArrayCompression = info.CollisionArrayCompression;
            result.CollisionArray1 = info.CollisionArray1 ?? CollisionArray1;
            result.CollisionArray2 = info.CollisionArray2 ?? CollisionArray2;
            result.AngleCompression = CompressionType.Uncompressed;
            if (AngleCompression != CompressionType.Invalid)
                result.AngleCompression = AngleCompression;
            if (info.AngleCompression != CompressionType.Invalid)
                result.AngleCompression = info.AngleCompression;
            result.Angles = info.Angles ?? Angles;
            result.TimeZone = info.TimeZone;
            return result;
        }
    }

    public class LevelInfo
    {
        [IniName("displayname")]
        public string DisplayName;
        [IniName("tilefmt")]
        public EngineVersion TileFormat;
        [IniName("tilecmp")]
        public CompressionType TileCompression;
        [IniName("tiles")]
        public FileList Tiles;
        [IniName("blockfmt")]
        public EngineVersion BlockFormat;
        [IniName("blockcmp")]
        public CompressionType BlockCompression;
        [IniName("blocks")]
        public FileList Blocks;
        [IniName("chunkfmt")]
        public EngineVersion ChunkFormat;
        [IniName("chunkcmp")]
        public CompressionType ChunkCompression;
        [IniName("chunks")]
        public FileList Chunks;
        [IniName("layoutfmt")]
        public EngineVersion LayoutFormat;
        [IniName("layoutcmp")]
        public CompressionType LayoutCompression;
        [IniName("layout")]
        public string Layout;
        [IniName("fglayout")]
        public string FGLayout;
        [IniName("bglayout")]
        public string BGLayout;
        [IniName("palettefmt")]
        public EngineVersion PaletteFormat;
        [IniIgnore]
        public NamedPaletteList[] Palettes;
        [IniName("palette")]
        public PaletteList Palette;
        [IniName("palette2")]
        public NamedPaletteList Palette2;
        [IniName("palette3")]
        public NamedPaletteList Palette3;
        [IniName("palette4")]
        public NamedPaletteList Palette4;
        [IniName("palette5")]
        public NamedPaletteList Palette5;
        [IniName("palette6")]
        public NamedPaletteList Palette6;
        [IniName("palette7")]
        public NamedPaletteList Palette7;
        [IniName("palette8")]
        public NamedPaletteList Palette8;
        [IniName("palette9")]
        public NamedPaletteList Palette9;
        [IniName("objectfmt")]
        public EngineVersion ObjectFormat;
        [IniName("objectcmp")]
        public CompressionType ObjectCompression;
        [IniName("objects")]
        public string Objects;
        [IniName("ringfmt")]
        public EngineVersion RingFormat;
        [IniName("ringcmp")]
        public CompressionType RingCompression;
        [IniName("rings")]
        public string Rings;
        [IniName("bumpercmp")]
        public CompressionType BumperCompression;
        [IniName("bumpers")]
        public string Bumpers;
        [IniName("startpos")]
        public StartPositionList StartPositions;
        [IniName("colindfmt")]
        public EngineVersion CollisionIndexFormat;
        [IniName("colindcmp")]
        public CompressionType CollisionIndexCompression;
        [IniName("colind")]
        public string CollisionIndex;
        [IniName("colind1")]
        public string CollisionIndex1;
        [IniName("colind2")]
        public string CollisionIndex2;
        [IniName("colindsz")]
        [DefaultValue(1)]
        public int CollisionIndexSize;
        [IniName("colarrfmt")]
        public EngineVersion CollisionArrayFormat;
        [IniName("colarrcmp")]
        public CompressionType CollisionArrayCompression;
        [IniName("colarr1")]
        public string CollisionArray1;
        [IniName("colarr2")]
        public string CollisionArray2;
        [IniName("anglefmt")]
        public EngineVersion AngleFormat;
        [IniName("anglecmp")]
        public CompressionType AngleCompression;
        [IniName("angles")]
        public string Angles;
        [IniName("timezone")]
        public TimeZone TimeZone;
        [IniName("sprites")]
        public string Sprites;
        [IniName("objlst")]
        public StringList ObjectList;
    }

    [TypeConverter(typeof(StringConverter<FileList>))]
    public class FileList : IEnumerable<FileInfo>
    {
        public FileInfo[] Collection;

        public FileList(string data)
        {
            string[] files = data.Split('|');
            List<FileInfo> filelist = new List<FileInfo>();
            foreach (string item in files)
                filelist.Add(new FileInfo(item));
            Collection = filelist.ToArray();
        }

        public override string ToString()
        {
            List<string> data = new List<string>(Collection.Length);
            foreach (FileInfo item in Collection)
                data.Add(item.ToString());
            return string.Join("|", data.ToArray());
        }

        IEnumerator<FileInfo> IEnumerable<FileInfo>.GetEnumerator()
        {
            return new List<FileInfo>(Collection).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Collection.GetEnumerator();
        }

        public FileInfo this[int index]
        {
            get { return Collection[index]; }
            set { Collection[index] = value; }
        }
    }

    public class FileInfo
    {
        public string Filename;
        public int Offset;

        public FileInfo(string data)
        {
            string[] split = data.Split(':');
            Filename = split[0];
            Offset = -1;
            if (split.Length > 1)
            {
                string offstr = split[1];
                if (offstr.StartsWith("0x"))
                    Offset = int.Parse(offstr.Substring(2), NumberStyles.HexNumber);
                else
                    Offset = int.Parse(offstr, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
            }
        }

        public override string ToString()
        {
            if (Offset != -1)
                return Filename + ":0x" + Offset.ToString("X");
            else
                return Filename;
        }
    }

    [TypeConverter(typeof(StringConverter<NamedPaletteList>))]
    public class NamedPaletteList : IEnumerable<PaletteInfo>
    {
        public string Name;
        public PaletteList Palettes;

        public NamedPaletteList(string data)
        {
            string[] files = data.Split('|');
            Name = files[0];
            Palettes = new PaletteList(string.Join("|", files, 1, files.Length - 1));
        }

        public NamedPaletteList(string name, PaletteList paletteList)
        {
            Name = name;
            Palettes = paletteList;
        }

        public override string ToString()
        {
            return Name + "|" + Palettes.ToString();
        }

        IEnumerator<PaletteInfo> IEnumerable<PaletteInfo>.GetEnumerator()
        {
            return new List<PaletteInfo>(Palettes.Collection).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Palettes.Collection.GetEnumerator();
        }

        public PaletteInfo this[int index]
        {
            get { return Palettes.Collection[index]; }
            set { Palettes.Collection[index] = value; }
        }
    }

    [TypeConverter(typeof(StringConverter<PaletteList>))]
    public class PaletteList : IEnumerable<PaletteInfo>
    {
        public PaletteInfo[] Collection;

        public PaletteList(string data)
        {
            string[] files = data.Split('|');
            List<PaletteInfo> filelist = new List<PaletteInfo>();
            foreach (string item in files)
                filelist.Add(new PaletteInfo(item));
            Collection = filelist.ToArray();
        }

        public override string ToString()
        {
            List<string> data = new List<string>(Collection.Length);
            foreach (PaletteInfo item in Collection)
                data.Add(item.ToString());
            return string.Join("|", data.ToArray());
        }

        IEnumerator<PaletteInfo> IEnumerable<PaletteInfo>.GetEnumerator()
        {
            return new List<PaletteInfo>(Collection).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Collection.GetEnumerator();
        }

        public PaletteInfo this[int index]
        {
            get { return Collection[index]; }
            set { Collection[index] = value; }
        }
    }

    public class PaletteInfo
    {
        public string Filename;
        public int Source, Destination, Length;

        public PaletteInfo(string data)
        {
            string[] split = data.Split(':');
            Filename = split[0];
            Source = int.Parse(split[1], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
            Destination = int.Parse(split[2], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
            Length = int.Parse(split[3], NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
        }

        public override string ToString()
        {
            return Filename + ":" + Source.ToString(NumberFormatInfo.InvariantInfo) + ":" + Destination.ToString(NumberFormatInfo.InvariantInfo) + ":" + Length.ToString(NumberFormatInfo.InvariantInfo);
        }
    }

    [TypeConverter(typeof(StringConverter<StartPositionList>))]
    public class StartPositionList : IEnumerable<StartPositionInfo>
    {
        public StartPositionInfo[] Collection;

        public StartPositionList(string data)
        {
            string[] files = data.Split('|');
            List<StartPositionInfo> filelist = new List<StartPositionInfo>();
            foreach (string item in files)
                filelist.Add(new StartPositionInfo(item));
            Collection = filelist.ToArray();
        }

        public override string ToString()
        {
            List<string> data = new List<string>(Collection.Length);
            foreach (StartPositionInfo item in Collection)
                data.Add(item.ToString());
            return string.Join("|", data.ToArray());
        }

        IEnumerator<StartPositionInfo> IEnumerable<StartPositionInfo>.GetEnumerator()
        {
            return new List<StartPositionInfo>(Collection).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Collection.GetEnumerator();
        }

        public StartPositionInfo this[int index]
        {
            get { return Collection[index]; }
            set { Collection[index] = value; }
        }
    }

    public class StartPositionInfo
    {
        public string Filename, Sprite, Name;
        public int Offset = -1;

        public StartPositionInfo(string data)
        {
            string[] split = data.Split(':');
            Filename = split[0];
            Sprite = split[1];
            Name = split[2];
            if (split.Length > 3)
                Offset = int.Parse(split[3], NumberStyles.HexNumber);
        }

        public override string ToString()
        {
            return Filename + ":" + Sprite + ":" + Name + (Offset == -1 ? "" : ":" + Offset.ToString("X"));
        }
    }

    [TypeConverter(typeof(StringConverter<StringList>))]
    public class StringList : IEnumerable<string>
    {
        public string[] Collection;

        public StringList(string data)
        {
            Collection = data.Split('|');
        }

        public override string ToString()
        {
            return string.Join("|", Collection);
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            return new List<string>(Collection).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Collection.GetEnumerator();
        }

        public string this[int index]
        {
            get { return Collection[index]; }
            set { Collection[index] = value; }
        }
    }

    /// <summary>
    /// Converts between <see cref="System.String"/> and <typeparamref name="T"/>
    /// </summary>
    public class StringConverter<T> : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(T))
                return true;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is T)
                return ((T)value).ToString();
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
            if (value is string)
                return Activator.CreateInstance(typeof(T), (string)value);
            return base.ConvertFrom(context, culture, value);
        }
    }
}