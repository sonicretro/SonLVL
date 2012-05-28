using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using SonicRetro.SonLVL.API;

namespace S12005ObjectDefinitions.GHZ
{
    class Platform : ObjectDefinition
    {
        private List<Sprite> imgs = new List<Sprite>();

        public override void Init(ObjectData data)
        {
            byte[] artfile = ObjectHelper.LevelArt;
            imgs.Add(ObjectHelper.MapASMToBmp(artfile, "../_maps/obj18ghz.asm", 0, 2));
            imgs.Add(ObjectHelper.MapASMToBmp(artfile, "../_maps/obj18ghz.asm", 1, 2));
        }

        public override ReadOnlyCollection<byte> Subtypes()
        {
            return new ReadOnlyCollection<byte>(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C });
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

        public override BitmapBits Image()
        {
            return imgs[0].Image;
        }

        private int imgindex(byte SubType)
        {
            switch ((PlatformMovement)(SubType & 0xF))
            {
                case PlatformMovement.Large:
                    return 1;
                default:
                    return 0;
            }
        }

        public override BitmapBits Image(byte subtype)
        {
            return imgs[imgindex(subtype)].Image;
        }

        public override Rectangle Bounds(ObjectEntry obj, Point camera)
        {
            return new Rectangle(obj.X + imgs[imgindex(obj.SubType)].X - camera.X, obj.Y + imgs[imgindex(obj.SubType)].Y - camera.Y, imgs[imgindex(obj.SubType)].Width, imgs[imgindex(obj.SubType)].Height);
        }

        public override Sprite GetSprite(ObjectEntry obj)
        {
            BitmapBits bits = new BitmapBits(imgs[imgindex(obj.SubType)].Image);
            bits.Flip(obj.XFlip, obj.YFlip);
            return new Sprite(bits, new Point(obj.X + imgs[imgindex(obj.SubType)].X, obj.Y + imgs[imgindex(obj.SubType)].Y));
        }

        public override Type ObjectType { get { return typeof(PlatformS1ObjectEntry); } }
    }

    public class PlatformS1ObjectEntry : S1ObjectEntry
    {
        public PlatformS1ObjectEntry() : base() { }
        public PlatformS1ObjectEntry(byte[] file, int address) : base(file, address) { }

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
        Large,
        DownUpSlow,
        UpDownSlow,
        Invalid1,
        Invalid2,
        Invalid3
    }
}