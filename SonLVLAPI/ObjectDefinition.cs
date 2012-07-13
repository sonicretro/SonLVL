using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.ComponentModel;

namespace SonicRetro.SonLVL.API
{
    public class ObjectData
    {
        [IniName("codefile")]
        public string CodeFile;
        [IniName("codetype")]
        public string CodeType;
        [IniName("xmlfile")]
        public string XMLFile;
        [IniName("name")]
        [DefaultValue("Unknown")]
        public string Name;
        [IniName("art")]
        public FileList Art;
        [IniName("artcmp")]
        [DefaultValue(CompressionType.Nemesis)]
        public CompressionType ArtCompression;
        [IniName("map")]
        public string MapFile;
        [IniName("mapcmp")]
        [DefaultValue(CompressionType.Uncompressed)]
        public CompressionType MapCompression;
        [IniName("mapasm")]
        public string MapFileAsm;
        [IniName("mapasmlbl")]
        public string MapAsmLabel;
        [IniName("mapver")]
        public EngineVersion MapVersion;
        [IniName("dplc")]
        public string DPLCFile;
        [IniName("dplccmp")]
        [DefaultValue(CompressionType.Uncompressed)]
        public CompressionType DPLCCompression;
        [IniName("dplcasm")]
        public string DPLCFileAsm;
        [IniName("dplcasmlbl")]
        public string DPLCAsmLabel;
        [IniName("dplcver")]
        public EngineVersion DPLCVersion;
        [IniName("frame")]
        public int Frame;
        [IniName("pal")]
        public int Palette;
        [IniName("image")]
        public string Image;
        [IniName("sprite")]
        [DefaultValue(-1)]
        public int Sprite;
        [IniName("offset")]
        public Size Offset;
        [IniName("rememberstate")]
        public bool RememberState;
        [IniName("debug")]
        public bool Debug;
        [IniName("subtypes")]
        public string Subtypes;
        [IniCollection]
        public Dictionary<string, string> CustomProperties;

        public ObjectData()
        {
            Sprite = -1;
        }

        public void Init()
        {
            if (DPLCVersion == EngineVersion.Invalid)
                DPLCVersion = MapVersion;
        }
    }

    public abstract class ObjectDefinition
    {
        public abstract void Init(ObjectData data);
        public abstract ReadOnlyCollection<byte> Subtypes();
        public abstract string Name();
        public abstract bool RememberState();
        public abstract string SubtypeName(byte subtype);
        public abstract BitmapBits Image();
        public abstract BitmapBits Image(byte subtype);
        public abstract Sprite GetSprite(ObjectEntry obj);
        public abstract Rectangle Bounds(ObjectEntry obj, Point camera);
        public virtual bool Debug { get { return false; } }

        public virtual Type ObjectType
        {
            get
            {
                switch (LevelData.Level.ObjectFormat)
                {
                    case EngineVersion.S1:
                        return typeof(S1ObjectEntry);
                    case EngineVersion.S2:
                    case EngineVersion.S2NA:
                    case EngineVersion.SBoom:
                        return typeof(S2ObjectEntry);
                    case EngineVersion.S3K:
                    case EngineVersion.SKC:
                        return typeof(S3KObjectEntry);
                    case EngineVersion.SCD:
                    case EngineVersion.SCDPC:
                        return typeof(SCDObjectEntry);
                    default:
                        return typeof(ObjectEntry);
                }
            }
        }
    }

    public class DefaultObjectDefinition : ObjectDefinition
    {
        private Sprite spr;
        private string name;
        private bool rememberstate;
        private List<byte> subtypes = new List<byte>();
        bool debug = false;

        public override void Init(ObjectData data)
        {
            name = data.Name ?? "Unknown";
            try
            {
                if (data.Art != null)
                {
                    MultiFileIndexer<byte> art = new MultiFileIndexer<byte>();
                    foreach (FileInfo file in data.Art)
                        art.AddFile(new List<byte>(ObjectHelper.OpenArtFile(file.Filename, data.ArtCompression)), file.Offset);
                    byte[] artfile = art.ToArray();
                    if (data.MapFile != null)
                    {
                        if (data.DPLCFile != null)
                            spr = ObjectHelper.MapDPLCToBmp(artfile, LevelData.ReadFile(data.MapFile, data.MapCompression), data.MapVersion, LevelData.ReadFile(data.DPLCFile, data.DPLCCompression), data.DPLCVersion, data.Frame, data.Palette);
                        else
                            spr = ObjectHelper.MapToBmp(artfile, LevelData.ReadFile(data.MapFile, data.MapCompression), data.Frame, data.Palette, data.MapVersion);
                    }
                    else if (data.MapFileAsm != null)
                    {
                        if (data.MapAsmLabel != null)
                        {
                            if (data.DPLCFileAsm != null)
                                spr = ObjectHelper.MapASMDPLCToBmp(artfile, data.MapFileAsm, data.MapAsmLabel, data.MapVersion, data.DPLCFileAsm, data.DPLCAsmLabel, data.DPLCVersion, data.Palette);
                            else
                                spr = ObjectHelper.MapASMToBmp(artfile, data.MapFileAsm, data.MapAsmLabel, data.Palette, data.MapVersion);
                        }
                        else
                        {
                            if (data.DPLCFileAsm != null)
                                spr = ObjectHelper.MapASMDPLCToBmp(artfile, data.MapFileAsm, data.MapVersion, data.DPLCFileAsm, data.DPLCVersion, data.Frame, data.Palette);
                            else
                                spr = ObjectHelper.MapASMToBmp(artfile, data.MapFileAsm, data.Frame, data.Palette, data.MapVersion);
                        }
                    }
                    else
                        spr = ObjectHelper.UnknownObject;
                    if (data.Offset != Size.Empty)
                        spr.Offset = spr.Offset + data.Offset;
                }
                else if (data.Image != null)
                {
                    BitmapBits img = new BitmapBits(new Bitmap(data.Image));
                    spr = new Sprite(img, new Point(data.Offset));
                    debug = true;
                }
                else if (data.Sprite > -1)
                    spr = ObjectHelper.GetSprite(data.Sprite);
                else
                {
                    spr = ObjectHelper.UnknownObject;
                    debug = true;
                }
            }
            catch (Exception ex)
            {
                LevelData.Log("Error loading object definition " + name + ":", ex.ToString());
                spr = ObjectHelper.UnknownObject;
                debug = true;
            }
            rememberstate = data.RememberState;
            debug = debug | data.Debug;
            string[] subs = (data.Subtypes ?? string.Empty).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in subs)
                subtypes.Add(byte.Parse(item, System.Globalization.NumberStyles.HexNumber));
        }

        public override ReadOnlyCollection<byte> Subtypes()
        {
            return new ReadOnlyCollection<byte>(subtypes);
        }

        public override string Name()
        {
            return name;
        }

        public override bool RememberState()
        {
            return rememberstate;
        }

        public override string SubtypeName(byte subtype)
        {
            return string.Empty;
        }

        public override BitmapBits Image()
        {
            return spr.Image;
        }

        public override BitmapBits Image(byte subtype)
        {
            return spr.Image;
        }

        public override Rectangle Bounds(ObjectEntry obj, Point camera)
        {
            return new Rectangle((obj.X + spr.Offset.X) - camera.X, (obj.Y + spr.Offset.Y) - camera.Y, spr.Image.Width, spr.Image.Height);
        }

        public override Sprite GetSprite(ObjectEntry obj)
        {
            BitmapBits bits = new BitmapBits(spr.Image);
            bits.Flip(obj.XFlip, obj.YFlip);
            return new Sprite(bits, new Point(obj.X + spr.Offset.X, obj.Y + spr.Offset.Y));
        }

        public override bool Debug { get { return debug; } }
    }

    public abstract class S2RingDefinition
    {
        public abstract void Init(ObjectData data);
        public abstract string Name();
        public abstract BitmapBits Image();
        public abstract Sprite GetSprite(S2RingEntry rng);
        public abstract Rectangle Bounds(S2RingEntry rng, Point camera);
        public virtual bool Debug { get { return false; } }
    }

    public class DefS2RingDef : S2RingDefinition
    {
        private Sprite spr;

        public override void Init(ObjectData data)
        {
            spr = ObjectHelper.UnknownObject;
        }

        public override string Name()
        {
            return "Rings";
        }

        public override BitmapBits Image()
        {
            return spr.Image;
        }

        public override Rectangle Bounds(S2RingEntry rng, Point camera)
        {
            return new Rectangle((rng.X + spr.Offset.X) - camera.X, (rng.Y + spr.Offset.Y) - camera.Y, spr.Image.Width, spr.Image.Height);
        }

        public override Sprite GetSprite(S2RingEntry rng)
        {
            return new Sprite(spr.Image, new Point(rng.X + spr.Offset.X, rng.Y + spr.Offset.Y));
        }

        public override bool Debug { get { return true; } }
    }

    public class S3KRingDefinition
    {
        private Sprite spr;
        private bool debug = false;

        public S3KRingDefinition()
        {
            spr = ObjectHelper.UnknownObject;
        }

        public S3KRingDefinition(ObjectData data)
        {
            try
            {
                if (data.Art != null)
                {
                    MultiFileIndexer<byte> art = new MultiFileIndexer<byte>();
                    foreach (FileInfo file in data.Art)
                        art.AddFile(new List<byte>(ObjectHelper.OpenArtFile(file.Filename, data.ArtCompression)), file.Offset);
                    byte[] artfile = art.ToArray();
                    if (data.MapFile != null)
                    {
                        if (data.DPLCFile != null)
                            spr = ObjectHelper.MapDPLCToBmp(artfile, LevelData.ReadFile(data.MapFile, data.MapCompression), data.MapVersion, LevelData.ReadFile(data.DPLCFile, data.DPLCCompression), data.DPLCVersion, data.Frame, data.Palette);
                        else
                            spr = ObjectHelper.MapToBmp(artfile, LevelData.ReadFile(data.MapFile, data.MapCompression), data.Frame, data.Palette, data.MapVersion);
                    }
                    else if (data.MapFileAsm != null)
                    {
                        if (data.MapAsmLabel != null)
                        {
                            if (data.DPLCFileAsm != null)
                                spr = ObjectHelper.MapASMDPLCToBmp(artfile, data.MapFileAsm, data.MapAsmLabel, data.MapVersion, data.DPLCFileAsm, data.DPLCAsmLabel, data.DPLCVersion, data.Palette);
                            else
                                spr = ObjectHelper.MapASMToBmp(artfile, data.MapFileAsm, data.MapAsmLabel, data.Palette, data.MapVersion);
                        }
                        else
                        {
                            if (data.DPLCFileAsm != null)
                                spr = ObjectHelper.MapASMDPLCToBmp(artfile, data.MapFileAsm, data.MapVersion, data.DPLCFileAsm, data.DPLCVersion, data.Frame, data.Palette);
                            else
                                spr = ObjectHelper.MapASMToBmp(artfile, data.MapFileAsm, data.Frame, data.Palette, data.MapVersion);
                        }
                    }
                    else
                        spr = ObjectHelper.UnknownObject;
                    if (data.Offset != Size.Empty)
                        spr.Offset = spr.Offset + data.Offset;
                }
                else if (data.Image != null)
                {
                    BitmapBits img = new BitmapBits(new Bitmap(data.Image));
                    spr = new Sprite(img, new Point(data.Offset));
                }
                else if (data.Sprite > -1)
                    spr = ObjectHelper.GetSprite(data.Sprite);
                else
                {
                    spr = ObjectHelper.UnknownObject;
                    debug = true;
                }
            }
            catch (Exception ex)
            {
                LevelData.Log("Error loading S3K ring definition:", ex.ToString());
                spr = ObjectHelper.UnknownObject;
                debug = true;
            }
        }

        public BitmapBits Image()
        {
            return spr.Image;
        }

        public Rectangle Bounds(S3KRingEntry rng, Point camera)
        {
            return new Rectangle((rng.X + spr.Offset.X) - camera.X, (rng.Y + spr.Offset.Y) - camera.Y, spr.Image.Width, spr.Image.Height);
        }

        public Sprite GetSprite(S3KRingEntry rng)
        {
            return new Sprite(spr.Image, new Point(rng.X + spr.Offset.X, rng.Y + spr.Offset.Y));
        }

        public bool Debug { get { return debug; } }
    }

    public class StartPositionDefinition
    {
        private Sprite spr;
        private string name;
        bool debug = false;

        public StartPositionDefinition(string name)
        {
            this.name = name;
            spr = ObjectHelper.UnknownObject;
        }

        public StartPositionDefinition(ObjectData data, string name)
            : this(name)
        {
            try
            {
                if (data.Art != null)
                {
                    MultiFileIndexer<byte> art = new MultiFileIndexer<byte>();
                    foreach (FileInfo file in data.Art)
                        art.AddFile(new List<byte>(ObjectHelper.OpenArtFile(file.Filename, data.ArtCompression)), file.Offset);
                    byte[] artfile = art.ToArray();
                    if (data.MapFile != null)
                    {
                        if (data.DPLCFile != null)
                            spr = ObjectHelper.MapDPLCToBmp(artfile, LevelData.ReadFile(data.MapFile, data.MapCompression), data.MapVersion, LevelData.ReadFile(data.DPLCFile, data.DPLCCompression), data.DPLCVersion == EngineVersion.Invalid & LevelData.Game.DPLCVersion == EngineVersion.S3K ? EngineVersion.S2 : data.DPLCVersion, data.Frame, data.Palette);
                        else
                            spr = ObjectHelper.MapToBmp(artfile, LevelData.ReadFile(data.MapFile, data.MapCompression), data.Frame, data.Palette, data.MapVersion);
                    }
                    else if (data.MapFileAsm != null)
                    {
                        if (data.MapAsmLabel != null)
                        {
                            if (data.DPLCFileAsm != null)
                                spr = ObjectHelper.MapASMDPLCToBmp(artfile, data.MapFileAsm, data.MapAsmLabel, data.MapVersion, data.DPLCFileAsm, data.DPLCAsmLabel, data.DPLCVersion == EngineVersion.Invalid & LevelData.Game.DPLCVersion == EngineVersion.S3K ? EngineVersion.S2 : data.DPLCVersion, data.Palette);
                            else
                                spr = ObjectHelper.MapASMToBmp(artfile, data.MapFileAsm, data.MapAsmLabel, data.Palette, data.MapVersion);
                        }
                        else
                        {
                            if (data.DPLCFileAsm != null)
                                spr = ObjectHelper.MapASMDPLCToBmp(artfile, data.MapFileAsm, data.MapVersion, data.DPLCFileAsm, data.DPLCVersion == EngineVersion.Invalid & LevelData.Game.DPLCVersion == EngineVersion.S3K ? EngineVersion.S2 : data.DPLCVersion, data.Frame, data.Palette);
                            else
                                spr = ObjectHelper.MapASMToBmp(artfile, data.MapFileAsm, data.Frame, data.Palette, data.MapVersion);
                        }
                    }
                    else
                        spr = ObjectHelper.UnknownObject;
                    if (data.Offset != Size.Empty)
                        spr.Offset = spr.Offset + data.Offset;
                }
                else if (data.Image != null)
                {
                    BitmapBits img = new BitmapBits(new Bitmap(data.Image));
                    spr = new Sprite(img, new Point(data.Offset));
                }
                else if (data.Sprite > -1)
                    spr = ObjectHelper.GetSprite(data.Sprite);
                else
                    spr = ObjectHelper.UnknownObject;
            }
            catch (Exception ex)
            {
                LevelData.Log("Error loading start position definition " + this.name + ":", ex.ToString());
                spr = ObjectHelper.UnknownObject;
                debug = true;
            }
        }

        public string Name()
        {
            return name;
        }
        public BitmapBits Image()
        {
            return spr.Image;
        }

        public Rectangle Bounds(StartPositionEntry st, Point camera)
        {
            return new Rectangle((st.X + spr.Offset.X) - camera.X, (st.Y + spr.Offset.Y) - camera.Y, spr.Image.Width, spr.Image.Height);
        }

        public Sprite GetSprite(StartPositionEntry st)
        {
            return new Sprite(spr.Image, new Point(st.X + spr.Offset.X, st.Y + spr.Offset.Y));
        }

        public bool Debug { get { return debug; } }
    }
}