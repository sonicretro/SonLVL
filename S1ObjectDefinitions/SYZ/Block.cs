using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using SonicRetro.SonLVL;

namespace S1ObjectDefinitions.SYZ
{
    class Block : ObjectDefinition
    {
        private Point offset;
        private BitmapBits img;
        private List<Point> offsets = new List<Point>();
        private List<BitmapBits> imgs = new List<BitmapBits>();

        public override void Init(Dictionary<string, string> data)
        {
            byte[] artfile = ObjectHelper.LevelArt;
            img = ObjectHelper.MapASMToBmp(artfile, "../_maps/Floating Blocks and Doors.asm", 0, 2, out offset);
            Point off;
            for (int i = 0; i < 8; i++)
            {
                imgs.Add(ObjectHelper.MapASMToBmp(artfile, "../_maps/Floating Blocks and Doors.asm", i, 2, out off));
                offsets.Add(off);
            }
        }

        public override ReadOnlyCollection<byte> Subtypes()
        {
            return new ReadOnlyCollection<byte>(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0B, 0x0C });
        }

        public override string Name()
        {
            return "Platform";
        }

        public override bool RememberState()
        {
            return false;
        }

        public override string SubtypeName(byte subtype)
        {
            return ((PlatformMovement)(subtype & 0xF)).ToString();
        }

        public override string FullName(byte subtype)
        {
            return Name() + " - " + SubtypeName(subtype);
        }

        public override BitmapBits Image()
        {
            return img;
        }

        public override BitmapBits Image(byte subtype)
        {
            return imgs[(subtype & 0xE) << 1];
        }

        public override Rectangle Bounds(Point loc, byte subtype)
        {
            return new Rectangle(loc.X + offsets[(subtype & 0xE) << 1].X, loc.Y + offsets[(subtype & 0xE) << 1].Y, imgs[(subtype & 0xE) << 1].Width, imgs[(subtype & 0xE) << 1].Height);
        }

        public override void Draw(BitmapBits bmp, Point loc, byte subtype, bool XFlip, bool YFlip, bool includeDebug)
        {
            BitmapBits bits = new BitmapBits(imgs[(subtype & 0xE) << 1]);
            bits.Flip(XFlip, YFlip);
            bmp.DrawBitmapComposited(bits, new Point(loc.X + offsets[(subtype & 0xE) << 1].X, loc.Y + offsets[(subtype & 0xE) << 1].Y));
        }

        public override Type ObjectType
        {
            get
            {
                return typeof(BlockS1ObjectEntry);
            }
        }
    }

    public class BlockS1ObjectEntry : S1ObjectEntry
    {
        public BlockS1ObjectEntry() : base() { }
        public BlockS1ObjectEntry(byte[] file, int address) : base(file, address) { }

        public PlatformMovement Movement
        {
            get
            {
                return (PlatformMovement)(SubType & 0xF);
            }
            set
            {
                SubType = (byte)((SubType & ~0xF) | (int)value);
            }
        }

        public byte SwitchID
        {
            get
            {
                return (byte)(SubType >> 4);
            }
            set
            {
                SubType = (byte)((SubType & ~0xF0) | (value << 4));
            }
        }
    }

    public enum PlatformMovement
    {
        Stationary,
        RightLeft,
        DownUp,
        FallStand,
        Fall,
        LeftRight,
        UpDown,
        SwitchUp,
        MoveUp,
        Stationary2,
        Invalid1,
        DownUpSlow,
        UpDownSlow,
        Invalid2,
        Invalid3,
        Invalid4
    }
}