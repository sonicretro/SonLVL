using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using SonicRetro.SonLVL.API;

namespace SCDPCObjectDefinitions.PPZ1
{
    public class Ring : ObjectDefinition
    {
        private Size[] Spacing = {
                                     new Size(0x10, 0), // horizontal tight
                                     new Size(0x18, 0), // horizontal normal
                                     new Size(0x20, 0), // horizontal wide
                                     new Size(0, 0x10), // vertical tight
                                     new Size(0, 0x18), // vertical normal
                                     new Size(0, 0x20), // vertical wide
                                     new Size(0x10, 0x10), // diagonal
                                     new Size(0x18, 0x18),
                                     new Size(0x20, 0x20),
                                     new Size(-0x10, 0x10),
                                     new Size(-0x18, 0x18),
                                     new Size(-0x20, 0x20),
                                     new Size(0x10, 8),
                                     new Size(0x18, 0x10),
                                     new Size(-0x10, 8),
                                     new Size(-0x18, 0x10)
                                 };

        private Sprite img;
        public override void Init(ObjectData data)
        {
            img = ObjectHelper.GetSprite(362);
        }

        public override ReadOnlyCollection<byte> Subtypes()
        {
            return new ReadOnlyCollection<byte>(new List<byte>());
        }

        public override string Name()
        {
            return "Ring";
        }

        public override bool RememberState()
        {
            return true;
        }

        public override string SubtypeName(byte subtype)
        {
            return string.Empty;
        }

        public override BitmapBits Image()
        {
            return img.Image;
        }

        public override BitmapBits Image(byte subtype)
        {
            return img.Image;
        }

        public override Rectangle Bounds(ObjectEntry obj, Point camera)
        {
            int count = Math.Min(6, obj.SubType & 7);
            Size space = Spacing[obj.SubType >> 4];
            return new Rectangle(obj.X + img.X - camera.X, obj.Y + img.Y - camera.Y, (space.Width * count) + img.Width, (space.Height * count) + img.Height);
        }

        public override Sprite GetSprite(ObjectEntry obj)
        {
            int count = Math.Min(6, obj.SubType & 7) + 1;
            Size space = Spacing[obj.SubType >> 4];
            Point loc = new Point(img.X, img.Y);
            List<Sprite> sprs = new List<Sprite>();
            for (int i = 0; i < count; i++)
            {
                sprs.Add(new Sprite(img.Image, loc));
                loc += space;
            }
            Sprite spr = new Sprite(sprs.ToArray());
            spr.Offset = new Point(spr.X + obj.X, spr.Y + obj.Y);
            return spr;
        }

        public override Type ObjectType
        {
            get
            {
                return typeof(RingSCDObjectEntry);
            }
        }
    }

    public class RingSCDObjectEntry : SCDObjectEntry
    {
        public RingSCDObjectEntry() : base() { }
        public RingSCDObjectEntry(byte[] file, int address) : base(file, address) { }

        public int Count
        {
            get
            {
                return Math.Min(6, SubType & 7) + 1;
            }
            set
            {
                SubType = (byte)((SubType & ~7) | (Math.Min(value, 7) - 1));
            }
        }

        public int Direction
        {
            get
            {
                return SubType >> 4;
            }
            set
            {
                SubType = (byte)((SubType & ~0xF0) | ((value & 0xF) << 4));
            }
        }
    }
}