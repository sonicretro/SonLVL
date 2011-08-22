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
        public abstract string FullName(byte subtype);
        public abstract BitmapBits Image();
        public abstract BitmapBits Image(byte subtype);
        public abstract void Draw(BitmapBits bmp, Point loc, byte subtype, bool XFlip, bool YFlip, bool includeDebug);
        public abstract Rectangle Bounds(Point loc, byte subtype);

        public virtual Type ObjectType
        {
            get
            {
                switch (LevelData.ObjectFmt)
                {
                    case EngineVersion.S1:
                        return typeof(S1ObjectEntry);
                    case EngineVersion.S2:
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
        private Point offset;
        private BitmapBits img;
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
                        img = ObjectHelper.MapDPLCToBmp(artfile, LevelData.ReadFile(data["map"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), data.GetValueOrDefault("mapcmp", "Uncompressed"))), LevelData.ReadFile(data["dplc"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), data.GetValueOrDefault("dplccmp", "Uncompressed"))), int.Parse(data["frame"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), out offset);
                    else
                        img = ObjectHelper.MapToBmp(artfile, LevelData.ReadFile(data["map"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), data.GetValueOrDefault("mapcmp", "Uncompressed"))), int.Parse(data["frame"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), out offset);
                }
                else if (data.ContainsKey("mapasm"))
                {
                    if (data.ContainsKey("mapasmlbl"))
                    {
                        if (data.ContainsKey("dplcasm"))
                            img = ObjectHelper.MapASMDPLCToBmp(artfile, data["mapasm"], data["mapasmlbl"], data["dplcasm"], data["dplcasmlbl"], int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), out offset);
                        else
                            img = ObjectHelper.MapASMToBmp(artfile, data["mapasm"], data["mapasmlbl"], int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), out offset);
                    }
                    else
                    {
                        if (data.ContainsKey("dplcasm"))
                            img = ObjectHelper.MapASMDPLCToBmp(artfile, data["mapasm"], data["dplcasm"], int.Parse(data["frame"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), out offset);
                        else
                            img = ObjectHelper.MapASMToBmp(artfile, data["mapasm"], int.Parse(data["frame"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), out offset);
                    }
                }
                else
                {
                    img = ObjectHelper.UnknownObject(out offset);
                    debug = true;
                }
                if (data.ContainsKey("offset"))
                {
                    string[] off = data["offset"].Split(',');
                    Size delta = new Size(int.Parse(off[0], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo), int.Parse(off[1], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo));
                    offset = offset + delta;
                }
            }
            else if (data.ContainsKey("image"))
            {
                img = new BitmapBits(new Bitmap(data["image"]));
                string[] off = data["offset"].Split(',');
                offset = new Point(int.Parse(off[0], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo), int.Parse(off[1], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo));
                debug = true;
            }
            else if (data.ContainsKey("sprite"))
            {
                int spr = int.Parse(data["sprite"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                img = LevelData.Sprites[spr].sprite;
                offset = LevelData.Sprites[spr].offset;
            }
            else
            {
                img = ObjectHelper.UnknownObject(out offset);
                debug = true;
            }
            rememberstate = bool.Parse(data.GetValueOrDefault("rememberstate", "False"));
            debug = debug | bool.Parse(data.GetValueOrDefault("debug", "False"));
            string[] subs = data.GetValueOrDefault("subtypes", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in subs)
            {
                subtypes.Add(byte.Parse(item, System.Globalization.NumberStyles.HexNumber));
            }
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

        public override string FullName(byte subtype)
        {
            return name;
        }

        public override BitmapBits Image()
        {
            return img;
        }

        public override BitmapBits Image(byte subtype)
        {
            return img;
        }

        public override Rectangle Bounds(Point loc, byte subtype)
        {
            return new Rectangle(loc.X + offset.X, loc.Y + offset.Y, img.Width, img.Height);
        }

        public override void Draw(BitmapBits bmp, Point loc, byte subtype, bool XFlip, bool YFlip, bool includeDebug)
        {
            if (!includeDebug & debug) return;
            BitmapBits bits = new BitmapBits(img);
            bits.Flip(XFlip, YFlip);
            bmp.DrawBitmapComposited(bits, new Point(loc.X + offset.X, loc.Y + offset.Y));
        }
    }

    public abstract class S2RingDefinition
    {
        public abstract void Init(Dictionary<string, string> data);
        public abstract string Name();
        public abstract BitmapBits Image();
        public abstract void Draw(BitmapBits bmp, Point loc, Direction direction, byte count, bool includeDebug);
        public abstract Rectangle Bounds(Point loc, Direction direction, byte count);
    }

    internal class DefS2RingDef : S2RingDefinition
    {
        private Point offset;
        private BitmapBits img;
        private int imgw, imgh;

        public override void Init(Dictionary<string, string> data)
        {
            img = ObjectHelper.UnknownObject(out offset);
            imgw = img.Width;
            imgh = img.Height;
        }

        public override string Name()
        {
            return "Rings";
        }

        public override BitmapBits Image()
        {
            return img;
        }

        public override Rectangle Bounds(Point loc, Direction direction, byte count)
        {
            return new Rectangle(loc.X + offset.X, loc.Y + offset.Y, imgw, imgh);
        }

        public override void Draw(BitmapBits bmp, Point loc, Direction direction, byte count, bool includeDebug)
        {
            if (!includeDebug) return;
            bmp.DrawBitmapComposited(img, new Point(loc.X + offset.X, loc.Y + offset.Y));
        }
    }

    internal class S3KRingDefinition
    {
        private Point offset;
        private BitmapBits img;
        private bool debug = false;

        public S3KRingDefinition()
        {
            img = ObjectHelper.UnknownObject(out offset);
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
                        img = ObjectHelper.MapDPLCToBmp(artfile, LevelData.ReadFile(data["map"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), data.GetValueOrDefault("mapcmp", "Uncompressed"))), LevelData.ReadFile(data["dplc"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), data.GetValueOrDefault("dplccmp", "Uncompressed"))), int.Parse(data["frame"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), out offset);
                    else
                        img = ObjectHelper.MapToBmp(artfile, LevelData.ReadFile(data["map"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), data.GetValueOrDefault("mapcmp", "Uncompressed"))), int.Parse(data["frame"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), out offset);
                }
                else if (data.ContainsKey("mapasm"))
                {
                    if (data.ContainsKey("mapasmlbl"))
                    {
                        if (data.ContainsKey("dplcasm"))
                            img = ObjectHelper.MapASMDPLCToBmp(artfile, data["mapasm"], data["mapasmlbl"], data["dplcasm"], data["dplcasmlbl"], int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), out offset);
                        else
                            img = ObjectHelper.MapASMToBmp(artfile, data["mapasm"], data["mapasmlbl"], int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), out offset);
                    }
                    else
                    {
                        if (data.ContainsKey("dplcasm"))
                            img = ObjectHelper.MapASMDPLCToBmp(artfile, data["mapasm"], data["dplcasm"], int.Parse(data["frame"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), out offset);
                        else
                            img = ObjectHelper.MapASMToBmp(artfile, data["mapasm"], int.Parse(data["frame"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), out offset);
                    }
                }
                else
                    img = ObjectHelper.UnknownObject(out offset);
                if (data.ContainsKey("offset"))
                {
                    string[] off = data["offset"].Split(',');
                    Size delta = new Size(int.Parse(off[0], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo), int.Parse(off[1], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo));
                    offset = offset + delta;
                }
            }
            else if (data.ContainsKey("image"))
            {
                img = new BitmapBits(new Bitmap(data["image"]));
                string[] off = data["offset"].Split(',');
                offset = new Point(int.Parse(off[0], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo), int.Parse(off[1], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo));
            }
            else if (data.ContainsKey("sprite"))
            {
                int spr = int.Parse(data["sprite"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                img = LevelData.Sprites[spr].sprite;
                offset = LevelData.Sprites[spr].offset;
            }
            else
                img = ObjectHelper.UnknownObject(out offset);
        }

        public BitmapBits Image()
        {
            return img;
        }

        public Rectangle Bounds(Point loc)
        {
            return new Rectangle(loc.X + offset.X, loc.Y + offset.Y, img.Width, img.Height);
        }

        public void Draw(BitmapBits bmp, Point loc, bool includeDebug)
        {
            if (!includeDebug & debug) return;
            bmp.DrawBitmapComposited(img, new Point(loc.X + offset.X, loc.Y + offset.Y));
        }
    }

    internal class StartPositionDefinition
    {
        private Point offset;
        private BitmapBits img;
        private string name;
        bool debug = false;

        public StartPositionDefinition(string name)
        {
            this.name = name;
            img = ObjectHelper.UnknownObject(out offset);
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
                        img = ObjectHelper.MapDPLCToBmp(artfile, LevelData.ReadFile(data["map"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), data.GetValueOrDefault("mapcmp", "Uncompressed"))), LevelData.ReadFile(data["dplc"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), data.GetValueOrDefault("dplccmp", "Uncompressed"))), EngineVersion.S2, int.Parse(data["frame"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), out offset);
                    else
                        img = ObjectHelper.MapToBmp(artfile, LevelData.ReadFile(data["map"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), data.GetValueOrDefault("mapcmp", "Uncompressed"))), int.Parse(data["frame"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), out offset);
                }
                else if (data.ContainsKey("mapasm"))
                {
                    if (data.ContainsKey("mapasmlbl"))
                    {
                        if (data.ContainsKey("dplcasm"))
                            img = ObjectHelper.MapASMDPLCToBmp(artfile, data["mapasm"], data["mapasmlbl"], data["dplcasm"], data["dplcasmlbl"], EngineVersion.S2, int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), out offset);
                        else
                            img = ObjectHelper.MapASMToBmp(artfile, data["mapasm"], data["mapasmlbl"], int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), out offset);
                    }
                    else
                    {
                        if (data.ContainsKey("dplcasm"))
                            img = ObjectHelper.MapASMDPLCToBmp(artfile, data["mapasm"], data["dplcasm"], EngineVersion.S2, int.Parse(data["frame"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), out offset);
                        else
                            img = ObjectHelper.MapASMToBmp(artfile, data["mapasm"], int.Parse(data["frame"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), out offset);
                    }
                }
                else
                    img = ObjectHelper.UnknownObject(out offset);
                if (data.ContainsKey("offset"))
                {
                    string[] off = data["offset"].Split(',');
                    Size delta = new Size(int.Parse(off[0], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo), int.Parse(off[1], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo));
                    offset = offset + delta;
                }
            }
            else if (data.ContainsKey("image"))
            {
                img = new BitmapBits(new Bitmap(data["image"]));
                string[] off = data["offset"].Split(',');
                offset = new Point(int.Parse(off[0], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo), int.Parse(off[1], System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo));
            }
            else if (data.ContainsKey("sprite"))
            {
                int spr = int.Parse(data["sprite"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                img = LevelData.Sprites[spr].sprite;
                offset = LevelData.Sprites[spr].offset;
            }
            else
                img = ObjectHelper.UnknownObject(out offset);
        }

        public string Name()
        {
            return name;
        }
        public BitmapBits Image()
        {
            return img;
        }

        public Rectangle Bounds(Point loc)
        {
            return new Rectangle(loc.X + offset.X, loc.Y + offset.Y, img.Width, img.Height);
        }

        public void Draw(BitmapBits bmp, Point loc, bool includeDebug)
        {
            if (!includeDebug & debug) return;
            bmp.DrawBitmapComposited(img, new Point(loc.X + offset.X, loc.Y + offset.Y));
        }
    }
}