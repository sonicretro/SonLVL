using System;
using System.Collections.Generic;
using System.Drawing;
using SonicRetro.SonLVL;

namespace S2ObjectDefinitions.Common
{
    class RingGroup : S2RingDefinition
    {
        private Sprite img;
        private int spacing;

        public override void Init(Dictionary<string, string> data)
        {
            if (data.ContainsKey("art"))
            {
                byte[] artfile = ObjectHelper.OpenArtFile(data["art"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), data.GetValueOrDefault("artcmp", "Nemesis")));
                if (data.ContainsKey("map"))
                    img = ObjectHelper.MapToBmp(artfile, Compression.Decompress(data["map"], (Compression.CompressionType)Enum.Parse(typeof(Compression.CompressionType), data.GetValueOrDefault("mapcmp", "Uncompressed"))), int.Parse(data["frame"], System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture), int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture));
                else if (data.ContainsKey("mapasm"))
                    img = ObjectHelper.MapASMToBmp(artfile, data["mapasm"], data["mapasmlbl"], int.Parse(data.GetValueOrDefault("pal", "0"), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture));
            }
            spacing = int.Parse(data.GetValueOrDefault("spacing", "24"), System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        public override string Name()
        {
            return "Rings";
        }

        public override BitmapBits Image()
        {
            return img.Image;
        }

        public override System.Drawing.Rectangle Bounds(S2RingEntry rng, Point camera)
        {
            switch (rng.Direction)
            {
                case Direction.Horizontal:
                    return new Rectangle(rng.X + img.X - camera.X, rng.Y + img.Y - camera.Y, ((rng.Count - 1) * spacing) + img.Width, img.Height);
                case Direction.Vertical:
                    return new Rectangle(rng.X + img.X - camera.X, rng.Y + img.Y - camera.Y, img.Width, ((rng.Count - 1) * spacing) + img.Height);
            }
            return Rectangle.Empty;
        }

        public override Sprite GetSprite(S2RingEntry rng)
        {
            List<Sprite> sprs = new List<Sprite>();
            for (int i = 0; i < rng.Count; i++)
            {
                switch (rng.Direction)
                {
                    case SonicRetro.SonLVL.Direction.Horizontal:
                        sprs.Add(new Sprite(img.Image, new Point(img.X + (i * spacing), img.Y)));
                        break;
                    case SonicRetro.SonLVL.Direction.Vertical:
                        sprs.Add(new Sprite(img.Image, new Point(img.X, img.Y + (i * spacing))));
                        break;
                }
            }
            Sprite spr = new Sprite(sprs.ToArray());
            spr.Offset = new Point(spr.X + rng.X, spr.Y + rng.Y);
            return spr;
        }
    }
}