using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace SonLVLINIEditor
{
    public class GameData
    {
        public EngineVersion EngineVersion { get; set; }
        public EngineVersion TileFormat { get; set; }
        public CompressionType TileCompression { get; set; }
        public EngineVersion BlockFormat { get; set; }
        public CompressionType BlockCompression { get; set; }
        public EngineVersion ChunkFormat { get; set; }
        public CompressionType ChunkCompression { get; set; }
        public EngineVersion LayoutFormat { get; set; }
        public CompressionType LayoutCompression { get; set; }
        public EngineVersion PaletteFormat { get; set; }
        public EngineVersion ObjectFormat { get; set; }
        public EngineVersion RingFormat { get; set; }
        public CompressionType CollisionIndexCompression { get; set; }
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string CollisionArray1 { get; set; }
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string CollisionArray2 { get; set; }
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string Angles { get; set; }
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string ObjectDefinitions { get; set; }

        public List<LevelData> levels;
        private int chunksz;

        public GameData(string filename)
        {
            Dictionary<string, Dictionary<string, string>> ini = IniFile.Load(filename);
            Dictionary<string, string> gr = ini[string.Empty];
            EngineVersion = (EngineVersion)Enum.Parse(typeof(EngineVersion), gr.GetValueOrDefault("version", "S2"));
            TileFormat = gr.ContainsKey("tile8fmt") ? (EngineVersion)Enum.Parse(typeof(EngineVersion), gr["tile8fmt"]) : EngineVersion;
            TileCompression = gr.ContainsKey("tile8cmp") ? (CompressionType)Enum.Parse(typeof(CompressionType), gr["tile8cmp"]) : CompressionType.Default;
            BlockFormat = gr.ContainsKey("block16fmt") ? (EngineVersion)Enum.Parse(typeof(EngineVersion), gr["block16fmt"]) : EngineVersion;
            BlockCompression = gr.ContainsKey("block16cmp") ? (CompressionType)Enum.Parse(typeof(CompressionType), gr["block16cmp"]) : CompressionType.Default;
            ChunkFormat = gr.ContainsKey("chunkfmt") ? (EngineVersion)Enum.Parse(typeof(EngineVersion), gr["chunkfmt"]) : EngineVersion;
            switch (ChunkFormat)
            {
                case EngineVersion.S1:
                case EngineVersion.SCDPC:
                    chunksz = 256;
                    break;
                case EngineVersion.S2:
                case EngineVersion.S3K:
                case EngineVersion.SKC:
                    chunksz = 128;
                    break;
            }
            ChunkCompression = gr.ContainsKey("chunk" + chunksz + "cmp") ? (CompressionType)Enum.Parse(typeof(CompressionType), gr["chunk" + chunksz + "cmp"]) : CompressionType.Default;
            LayoutFormat = gr.ContainsKey("layoutfmt") ? (EngineVersion)Enum.Parse(typeof(EngineVersion), gr["layoutfmt"]) : EngineVersion;
            LayoutCompression = gr.ContainsKey("layoutcmp") ? (CompressionType)Enum.Parse(typeof(CompressionType), gr["layoutcmp"]) : CompressionType.Default;
            PaletteFormat = gr.ContainsKey("palettefmt") ? (EngineVersion)Enum.Parse(typeof(EngineVersion), gr["palettefmt"]) : EngineVersion;
            ObjectFormat = gr.ContainsKey("objectsfmt") ? (EngineVersion)Enum.Parse(typeof(EngineVersion), gr["objectsfmt"]) : EngineVersion;
            RingFormat = gr.ContainsKey("ringsfmt") ? (EngineVersion)Enum.Parse(typeof(EngineVersion), gr["ringsfmt"]) : EngineVersion;
            CollisionIndexCompression = gr.ContainsKey("colindcmp") ? (CompressionType)Enum.Parse(typeof(CompressionType), gr["colindcmp"]) : CompressionType.Default;
            CollisionArray1 = gr.GetValueOrDefault("colarr1", string.Empty);
            CollisionArray2 = gr.GetValueOrDefault("colarr2", string.Empty);
            Angles = gr.GetValueOrDefault("angles", string.Empty);
            ObjectDefinitions = gr.GetValueOrDefault("objlst", string.Empty);
        }

        public void Save(string filename)
        {
            Dictionary<string, Dictionary<string, string>> ini = new Dictionary<string,Dictionary<string,string>>();
            Dictionary<string, string> gr = new Dictionary<string,string>();
            gr.Add("version", EngineVersion.ToString());
            if (TileFormat != EngineVersion)
                gr.Add("tile8fmt", TileFormat.ToString());
            if (TileCompression != CompressionType.Default)
                gr.Add("tile8cmp", TileCompression.ToString());
            if (BlockFormat != EngineVersion)
                gr.Add("block16fmt", BlockFormat.ToString());
            if (BlockCompression != CompressionType.Default)
                gr.Add("block16cmp", BlockCompression.ToString());
            if (ChunkFormat != EngineVersion)
                gr.Add("chunkfmt", ChunkFormat.ToString());
            switch (ChunkFormat)
            {
                case EngineVersion.S1:
                case EngineVersion.SCDPC:
                    chunksz = 256;
                    break;
                case EngineVersion.S2:
                case EngineVersion.S3K:
                case EngineVersion.SKC:
                    chunksz = 128;
                    break;
            }
            if (ChunkCompression != CompressionType.Default)
                gr.Add("chunk" + chunksz + "cmp", ChunkCompression.ToString());
            if (LayoutFormat != EngineVersion)
                gr.Add("layoutfmt", LayoutFormat.ToString());
            if (LayoutCompression != CompressionType.Default)
                gr.Add("layoutcmp", LayoutCompression.ToString());
            if (PaletteFormat != EngineVersion)
                gr.Add("palettefmt", PaletteFormat.ToString());
            if (ObjectFormat != EngineVersion)
                gr.Add("objectsfmt", ObjectFormat.ToString());
            if (RingFormat != EngineVersion)
                gr.Add("ringsfmt", RingFormat.ToString());
            if (CollisionIndexCompression != CompressionType.Default)
                gr.Add("colindcmp", CollisionIndexCompression.ToString());
            if (!string.IsNullOrEmpty(CollisionArray1))
                gr.Add("colarr1", CollisionArray1);
            if (!string.IsNullOrEmpty(CollisionArray2))
                gr.Add("colarr2", CollisionArray2);
            if (!string.IsNullOrEmpty(Angles))
                gr.Add("angles", Angles);
            if (!string.IsNullOrEmpty(ObjectDefinitions))
                gr.Add("objlst", ObjectDefinitions);
        }
    }

    public class LevelData
    {
        public string Name { get; set; }
        public List<FileInfo> Tiles { get; set; }
        public EngineVersion TileFormat { get; set; }
        public CompressionType TileCompression { get; set; }
        public List<FileInfo> Blocks { get; set; }
        public List<Bitmap[]> BlockBmps { get; set; }
        public EngineVersion BlockFormat { get; set; }
        public CompressionType BlockCompression { get; set; }
        public List<FileInfo> Chunks { get; set; }
        public List<Bitmap[]> ChunkBmps { get; set; }
        public EngineVersion ChunkFormat { get; set; }
        public CompressionType ChunkCompression { get; set; }
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string Layout { get; set; }
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string FGLayout { get; set; }
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string BGLayout { get; set; }
        public EngineVersion LayoutFormat { get; set; }
        public CompressionType LayoutCompression { get; set; }
        public List<string> PalName { get; set; }
        public List<ushort[,]> Palette { get; set; }
        public List<byte[,]> PalNum { get; set; }
        public List<int[,]> PalAddr { get; set; }
        public EngineVersion PaletteFormat { get; set; }
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string Objects { get; set; }
        public EngineVersion ObjectFormat { get; set; }
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string Rings { get; set; }
        public EngineVersion RingFormat { get; set; }
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string Bumpers { get; set; }
        //public List<StartPositionEntry> StartPositions { get; set; }
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string CollisionIndexes1 { get; set; }
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string CollisionIndexes2 { get; set; }
        public CompressionType CollisionIndexCompression { get; set; }
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string CollisionArray1 { get; set; }
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string CollisionArray2 { get; set; }
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string Angles { get; set; }
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string Sprites { get; set; }
        public TimeZone TimeZone { get; set; }
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string ObjectDefinitions { get; set; }
    }

    public class FileInfo
    {
        public string FileName { get; set; }
        public string Offset { get; set; }

        public FileInfo(string data)
        {
            FileName = data.Split(':')[0];
            if (data.Contains(":"))
                Offset = data.Split(':')[1];
            else
                Offset = "-1";
        }

        public override string ToString()
        {
            if (Offset != "-1")
                return FileName + ':' + Offset;
            else
                return FileName;
        }
    }

    public enum EngineVersion
    {
        Invalid,
        S1,
        S2,
        S3K,
        SCD,
        SCDPC,
        SKC
    }

    public enum TimeZone
    {
        None,
        Present,
        Past,
        Future
    }

    public enum CompressionType
    {
        Default,
        Uncompressed,
        Kosinski,
        KosinskiM,
        Nemesis,
        Enigma,
        SZDD
    }
}