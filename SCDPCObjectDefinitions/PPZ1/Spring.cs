using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using SonicRetro.SonLVL.API;

namespace SCDPCObjectDefinitions.PPZ1
{
    class Spring : ObjectDefinition
    {
        private List<Sprite> imgs = new List<Sprite>();

        public override void Init(ObjectData data)
        {
            imgs.Add(ObjectHelper.GetSprite(479)); // 0x00
            imgs.Add(ObjectHelper.GetSprite(467)); // 0x02
            imgs.Add(ObjectHelper.GetSprite(482)); // 0x10
            imgs.Add(ObjectHelper.GetSprite(470)); // 0x12
            imgs.Add(ObjectHelper.GetSprite(485)); // 0x20
            imgs.Add(ObjectHelper.GetSprite(473)); // 0x22
            imgs.Add(imgs[0]); // 0x30
            imgs.Add(imgs[1]); // 0x32
        }

        public override ReadOnlyCollection<byte> Subtypes()
        {
            return new ReadOnlyCollection<byte>(new byte[] { 0x00, 0x02, 0x10, 0x12, 0x20, 0x22 });
        }

        public override string Name()
        {
            return "Spring";
        }

        public override bool RememberState()
        {
            return false;
        }

        public override string SubtypeName(byte subtype)
        {
            return ((SpringDirection)((subtype & 0x30) >> 4)).ToString() + " " + ((SpringColor)((subtype & 2) >> 1)).ToString();
        }

        public override BitmapBits Image()
        {
            return imgs[0].Image;
        }

        private int imgindex(byte subtype)
        {
            int result = (subtype & 2) >> 1;
            result |= (subtype & 0x30) >> 3;
            return result;
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
            byte subtype = obj.SubType;
            BitmapBits bits = new BitmapBits(imgs[imgindex(subtype)].Image);
            bits.Flip(obj.XFlip, obj.YFlip);
            return new Sprite(bits, new Point(obj.X + imgs[imgindex(subtype)].Offset.X, obj.Y + imgs[imgindex(subtype)].Offset.Y));
        }

        public override Type ObjectType
        {
            get
            {
                return typeof(SpringSCDObjectEntry);
            }
        }
    }

    public class SpringSCDObjectEntry : SCDObjectEntry
    {
        public SpringSCDObjectEntry() : base() { }
        public SpringSCDObjectEntry(byte[] file, int address) : base(file, address) { }

        public SpringDirection Direction
        {
            get
            {
                return (SpringDirection)((SubType & 0x30) >> 4);
            }
            set
            {
                SubType = (byte)((SubType & ~0x30) | ((int)value << 4));
            }
        }

        public SpringColor Color
        {
            get
            {
                return (SpringColor)((SubType & 2) >> 1);
            }
            set
            {
                SubType = (byte)((SubType & ~2) | ((int)value << 1));
            }
        }
    }

    public enum SpringDirection
    {
        Vertical,
        Horizontal,
        Diagonal,
        Invalid
    }

    public enum SpringColor
    {
        Red,
        Yellow
    }
}