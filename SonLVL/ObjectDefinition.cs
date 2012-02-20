using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;

namespace SonicRetro.SonLVL
{
    public abstract class ObjectDefinition
    {
        public abstract void Init(Dictionary<string, string> data);
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
                switch (LevelData.ObjectFmt)
                {
                    case EngineVersion.S1:
                        return typeof(S1ObjectEntry);
                    case EngineVersion.S2:
                    case EngineVersion.S2NA:
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

    internal class DefaultObjectDefinition : ObjectDefinition
    {
        private Sprite spr;
        private string name;
        private bool rememberstate;
        private List<byte> subtypes = new List<byte>();
        bool debug = false;

        public override void Init(Dictionary<string, string> data)
        {
            name = data.GetValueOrDefault("name", "Unknown");
            if (data.ContainsKey("art"))
            {
                byte[] artfile = LevelData.ReadFile(data["art"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), data.GetValueOrDefault("artcmp", "Nemesis")));
                if (data.ContainsKey("map"))
                {
                    if (data.ContainsKey("dplc"))
                        spr = ObjectHelper.MapDPLCToBmp(artfile, LevelData.ReadFile(data["map"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), data.GetValueOrDefault("mapcmp", "Uncompressed"))), LevelData.ReadFile(data["dplc"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), data.GetValueOrDefault("dplccmp", "Uncompressed"))), int.Parse(data["frame"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture));
                    else
                        spr = ObjectHelper.MapToBmp(artfile, LevelData.ReadFile(data["map"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), data.GetValueOrDefault("mapcmp", "Uncompressed"))), int.Parse(data["frame"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture));
                }
                else if (data.ContainsKey("mapasm"))
                {
                    if (data.ContainsKey("mapasmlbl"))
                    {
                        if (data.ContainsKey("dplcasm"))
                            spr = ObjectHelper.MapASMDPLCToBmp(artfile, data["mapasm"], data["mapasmlbl"], data["dplcasm"], data["dplcasmlbl"], int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture));
                        else
                            spr = ObjectHelper.MapASMToBmp(artfile, data["mapasm"], data["mapasmlbl"], int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        if (data.ContainsKey("dplcasm"))
                            spr = ObjectHelper.MapASMDPLCToBmp(artfile, data["mapasm"], data["dplcasm"], int.Parse(data["frame"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture));
                        else
                            spr = ObjectHelper.MapASMToBmp(artfile, data["mapasm"], int.Parse(data["frame"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture));
                    }
                }
                else
                {
                    spr = ObjectHelper.UnknownObject;
                    debug = true;
                }
                if (data.ContainsKey("offset"))
                {
                    string[] off = data["offset"].Split(',');
                    Size delta = new Size(int.Parse(off[0], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo), int.Parse(off[1], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo));
                    spr.Offset = spr.Offset + delta;
                }
            }
            else if (data.ContainsKey("image"))
            {
                BitmapBits img = new BitmapBits(new Bitmap(data["image"]));
                string[] off = data["offset"].Split(',');
                Point offset = new Point(int.Parse(off[0], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo), int.Parse(off[1], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo));
                spr = new Sprite(img, offset);
                debug = true;
            }
            else if (data.ContainsKey("sprite"))
            {
                spr = ObjectHelper.GetSprite(int.Parse(data["sprite"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture));
            }
            else
            {
                spr = ObjectHelper.UnknownObject;
                debug = true;
            }
            rememberstate = bool.Parse(data.GetValueOrDefault("rememberstate", "False"));
            debug = debug | bool.Parse(data.GetValueOrDefault("debug", "False"));
            string[] subs = data.GetValueOrDefault("subtypes", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
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
        public abstract void Init(Dictionary<string, string> data);
        public abstract string Name();
        public abstract BitmapBits Image();
        public abstract Sprite GetSprite(S2RingEntry rng);
        public abstract Rectangle Bounds(S2RingEntry rng, Point camera);
        public virtual bool Debug { get { return false; } }
    }

    internal class DefS2RingDef : S2RingDefinition
    {
        private Sprite spr;

        public override void Init(Dictionary<string, string> data)
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

    internal class S3KRingDefinition
    {
        private Sprite spr;
        private bool debug = false;

        public S3KRingDefinition()
        {
            spr = ObjectHelper.UnknownObject;
            debug = true;
        }

        public S3KRingDefinition(Dictionary<string, string> data)
        {
            if (data.ContainsKey("art"))
            {
                byte[] artfile = LevelData.ReadFile(data["art"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), data.GetValueOrDefault("artcmp", "Nemesis")));
                if (data.ContainsKey("map"))
                {
                    if (data.ContainsKey("dplc"))
                        spr = ObjectHelper.MapDPLCToBmp(artfile, LevelData.ReadFile(data["map"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), data.GetValueOrDefault("mapcmp", "Uncompressed"))), LevelData.ReadFile(data["dplc"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), data.GetValueOrDefault("dplccmp", "Uncompressed"))), int.Parse(data["frame"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture));
                    else
                        spr = ObjectHelper.MapToBmp(artfile, LevelData.ReadFile(data["map"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), data.GetValueOrDefault("mapcmp", "Uncompressed"))), int.Parse(data["frame"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture));
                }
                else if (data.ContainsKey("mapasm"))
                {
                    if (data.ContainsKey("mapasmlbl"))
                    {
                        if (data.ContainsKey("dplcasm"))
                            spr = ObjectHelper.MapASMDPLCToBmp(artfile, data["mapasm"], data["mapasmlbl"], data["dplcasm"], data["dplcasmlbl"], int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture));
                        else
                            spr = ObjectHelper.MapASMToBmp(artfile, data["mapasm"], data["mapasmlbl"], int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        if (data.ContainsKey("dplcasm"))
                            spr = ObjectHelper.MapASMDPLCToBmp(artfile, data["mapasm"], data["dplcasm"], int.Parse(data["frame"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture));
                        else
                            spr = ObjectHelper.MapASMToBmp(artfile, data["mapasm"], int.Parse(data["frame"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture));
                    }
                }
                else
                    spr = ObjectHelper.UnknownObject;
                if (data.ContainsKey("offset"))
                {
                    string[] off = data["offset"].Split(',');
                    Size delta = new Size(int.Parse(off[0], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo), int.Parse(off[1], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo));
                    spr.Offset = spr.Offset + delta;
                }
            }
            else if (data.ContainsKey("image"))
            {
                BitmapBits img = new BitmapBits(new Bitmap(data["image"]));
                string[] off = data["offset"].Split(',');
                Point offset = new Point(int.Parse(off[0], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo), int.Parse(off[1], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo));
                spr = new Sprite(img, offset);
            }
            else if (data.ContainsKey("sprite"))
                spr = ObjectHelper.GetSprite(int.Parse(data["sprite"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture));
            else
                spr = ObjectHelper.UnknownObject;
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

    internal class StartPositionDefinition
    {
        private Sprite spr;
        private string name;
        bool debug = false;

        public StartPositionDefinition(string name)
        {
            this.name = name;
            spr = ObjectHelper.UnknownObject;
            debug = true;
        }

        public StartPositionDefinition(Dictionary<string, string> data, string name)
        {
            this.name = name;
            if (data.ContainsKey("art"))
            {
                byte[] artfile = LevelData.ReadFile(data["art"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), data.GetValueOrDefault("artcmp", "Uncompressed")));
                if (data.ContainsKey("map"))
                {
                    if (data.ContainsKey("dplc"))
                        spr = ObjectHelper.MapDPLCToBmp(artfile, LevelData.ReadFile(data["map"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), data.GetValueOrDefault("mapcmp", "Uncompressed"))), LevelData.ReadFile(data["dplc"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), data.GetValueOrDefault("dplccmp", "Uncompressed"))), int.Parse(data["frame"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture));
                    else
                        spr = ObjectHelper.MapToBmp(artfile, LevelData.ReadFile(data["map"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), data.GetValueOrDefault("mapcmp", "Uncompressed"))), int.Parse(data["frame"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture));
                }
                else if (data.ContainsKey("mapasm"))
                {
                    if (data.ContainsKey("mapasmlbl"))
                    {
                        if (data.ContainsKey("dplcasm"))
                            spr = ObjectHelper.MapASMDPLCToBmp(artfile, data["mapasm"], data["mapasmlbl"], data["dplcasm"], data["dplcasmlbl"], int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture));
                        else
                            spr = ObjectHelper.MapASMToBmp(artfile, data["mapasm"], data["mapasmlbl"], int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        if (data.ContainsKey("dplcasm"))
                            spr = ObjectHelper.MapASMDPLCToBmp(artfile, data["mapasm"], data["dplcasm"], int.Parse(data["frame"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture));
                        else
                            spr = ObjectHelper.MapASMToBmp(artfile, data["mapasm"], int.Parse(data["frame"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture));
                    }
                }
                else
                    spr = ObjectHelper.UnknownObject;
                if (data.ContainsKey("offset"))
                {
                    string[] off = data["offset"].Split(',');
                    Size delta = new Size(int.Parse(off[0], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo), int.Parse(off[1], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo));
                    spr.Offset = spr.Offset + delta;
                }
            }
            else if (data.ContainsKey("image"))
            {
                BitmapBits img = new BitmapBits(new Bitmap(data["image"]));
                string[] off = data["offset"].Split(',');
                Point offset = new Point(int.Parse(off[0], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo), int.Parse(off[1], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo));
                spr = new Sprite(img, offset);
            }
            else if (data.ContainsKey("sprite"))
                spr = ObjectHelper.GetSprite(int.Parse(data["sprite"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture));
            else
                spr = ObjectHelper.UnknownObject;
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