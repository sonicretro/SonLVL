using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using SonicRetro.SonLVL.API;

namespace SCDPCObjectDefinitions.Common
{
    class Monitor : ObjectDefinition
    {
        private Sprite img;
		private Sprite postimg;
        private List<Sprite> imgs = new List<Sprite>();

        public override void Init(ObjectData data)
        {
            img = ObjectHelper.GetSprite(286);
			postimg = ObjectHelper.GetSprite(285);
            for (int i = 0; i < 8; i++)
				imgs.Add(ObjectHelper.GetSprite(272 + i));
            for (int i = 0; i < 2; i++)
				imgs.Add(ObjectHelper.GetSprite(281 + i));
        }

        public override ReadOnlyCollection<byte> Subtypes()
        {
            return new ReadOnlyCollection<byte>(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        }

        public override string Name()
        {
            return "Monitor/Timepost";
        }

        public override bool RememberState()
        {
            return true;
        }

        public override string SubtypeName(byte subtype)
        {
            switch (subtype)
            {
                case 0:
                    return "Sonic Monitor";
                case 1:
                    return "Rings Monitor";
                case 2:
                    return "Shield Monitor";
                case 3:
                    return "Invincibility Monitor";
                case 4:
                    return "Speed Shoes Monitor";
                case 5:
                    return "Clock Monitor";
                case 6:
                    return "Silver Ring Monitor";
                case 7:
                    return "S Monitor";
                case 8:
                    return "Past Timepost";
                case 9:
                    return "Future Timepost";
                default:
                    return "Unknown";
            }
        }

        public override BitmapBits Image()
        {
            return img.Image;
        }

        public override BitmapBits Image(byte subtype)
        {
            if (subtype <= 9)
                return imgs[subtype].Image;
            else
                return img.Image;
        }

        public override Rectangle Bounds(ObjectEntry obj, Point camera)
        {
            Rectangle r = GetSprite(obj).Bounds;
            r.Offset(-camera.X, -camera.Y);
            return r;
        }

        public override Sprite GetSprite(ObjectEntry obj)
        {
            byte subtype = obj.SubType;
            if (subtype <= 7 | subtype > 9)
            {
                BitmapBits bits = new BitmapBits(img.Image);
                bits.Flip(obj.XFlip, obj.YFlip);
                Sprite spr = new Sprite(bits, new Point(obj.X + img.Offset.X, obj.Y + img.Offset.Y));
				if (subtype <= 7)
				{
                    bits = new BitmapBits(imgs[subtype].Image);
                    bits.Flip(obj.XFlip, obj.YFlip);
                    spr = new Sprite(spr, new Sprite(bits, new Point(obj.X + imgs[subtype].Offset.X, obj.Y + imgs[subtype].Offset.Y)));
				}
                return spr;
		    }
			else
			{
                BitmapBits bits = new BitmapBits(postimg.Image);
                bits.Flip(obj.XFlip, obj.YFlip);
                Sprite spr = new Sprite(bits, new Point(obj.X + postimg.Offset.X, obj.Y + postimg.Offset.Y));
                bits = new BitmapBits(imgs[subtype].Image);
                bits.Flip(obj.XFlip, obj.YFlip);
                return new Sprite(spr, new Sprite(bits, new Point(obj.X + imgs[subtype].Offset.X, obj.Y + imgs[subtype].Offset.Y)));
            }
		}

        public override Type ObjectType
        {
            get
            {
                return typeof(MonitorSCDObjectEntry);
            }
        }
    }

    public class MonitorSCDObjectEntry : SCDObjectEntry
    {
        public MonitorSCDObjectEntry() : base() { }
        public MonitorSCDObjectEntry(byte[] file, int address) : base(file, address) { }

        public MonitorType Contents
        {
            get
            {
			    if (SubType > 9) return MonitorType.Invalid;
                return (MonitorType)SubType;
            }
            set
            {
                SubType = (byte)value;
            }
        }
		
		public enum MonitorType
		{
		    OneUp,
			Ring,
			Shield,
			Invincibility,
			Shoes,
			Clock,
			SilverRing,
			S,
			Past,
			Future,
			Invalid
		}
    }
}