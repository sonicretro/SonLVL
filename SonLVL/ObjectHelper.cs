using System.Drawing;

namespace SonicRetro.SonLVL
{
    public static class ObjectHelper
    {
        public static byte[] OpenArtFile(string file, Compression.CompressionType comp) { return LevelData.ReadFile(file, comp); }

        public static Sprite MapToBmp(byte[] artfile, byte[] mapfile, int frame, int startpal)
        {
            BitmapBits[] bmp = null;
            Point off = new Point();
            switch (LevelData.EngineVersion)
            {
                case EngineVersion.S1:
                    S1Mappings s1map = new S1Mappings(mapfile, ByteConverter.ToInt16(mapfile, frame * 2));
                    bmp = LevelData.S1MapFrameToBmp(artfile, s1map, startpal, out off);
                    break;
                case EngineVersion.S2:
                case EngineVersion.S2NA:
                    S2Mappings s2map = new S2Mappings(mapfile, ByteConverter.ToInt16(mapfile, frame * 2));
                    bmp = LevelData.S2MapFrameToBmp(artfile, s2map, startpal, out off);
                    break;
                case EngineVersion.S3K:
                case EngineVersion.SKC:
                    S3KMappings s3kmap = new S3KMappings(mapfile, ByteConverter.ToInt16(mapfile, frame * 2));
                    bmp = LevelData.S3KMapFrameToBmp(artfile, s3kmap, startpal, out off);
                    break;
            }
            BitmapBits Image = new BitmapBits(bmp[0].Width, bmp[0].Height);
            Image.DrawBitmapComposited(bmp[0], Point.Empty);
            Image.DrawBitmapComposited(bmp[1], Point.Empty);
            return new Sprite(Image, off);
        }

        public static Sprite MapASMToBmp(byte[] artfile, string mapfileloc, int frame, int startpal)
        {
            return MapToBmp(artfile, LevelData.ASMToBin(mapfileloc), frame, startpal);
        }

        public static Sprite MapASMToBmp(byte[] artfile, string mapfileloc, string label, int startpal)
        {
            byte[] mapfile = LevelData.ASMToBin(mapfileloc, label);
            BitmapBits[] bmp = null;
            Point off = new Point();
            switch (LevelData.EngineVersion)
            {
                case EngineVersion.S1:
                    bmp = LevelData.S1MapFrameToBmp(artfile, new S1Mappings(mapfile, 0), startpal, out off);
                    break;
                case EngineVersion.S2:
                case EngineVersion.S2NA:
                    bmp = LevelData.S2MapFrameToBmp(artfile, new S2Mappings(mapfile, 0), startpal, out off);
                    break;
                case EngineVersion.S3K:
                case EngineVersion.SKC:
                    bmp = LevelData.S3KMapFrameToBmp(artfile, new S3KMappings(mapfile, 0), startpal, out off);
                    break;
            }
            BitmapBits Image = new BitmapBits(bmp[0].Width, bmp[0].Height);
            Image.DrawBitmapComposited(bmp[0], Point.Empty);
            Image.DrawBitmapComposited(bmp[1], Point.Empty);
            return new Sprite(Image, off);
        }

        public static Sprite MapDPLCToBmp(byte[] artfile, byte[] mapfile, byte[] dplc, int frame, int startpal)
        {
            BitmapBits[] bmp = null;
            Point off = new Point();
            DPLC dp = new DPLC(dplc, ByteConverter.ToInt16(dplc, frame * 2), LevelData.EngineVersion);
            switch (LevelData.EngineVersion)
            {
                case EngineVersion.S2:
                case EngineVersion.S2NA:
                    S2Mappings s2map = new S2Mappings(mapfile, ByteConverter.ToInt16(mapfile, frame * 2));
                    bmp = LevelData.S2MapFrameDPLCToBmp(artfile, s2map, dp, startpal, out off);
                    break;
                case EngineVersion.S3K:
                case EngineVersion.SKC:
                    S3KMappings s3kmap = new S3KMappings(mapfile, ByteConverter.ToInt16(mapfile, frame * 2));
                    bmp = LevelData.S3KMapFrameDPLCToBmp(artfile, s3kmap, dp, startpal, out off);
                    break;
            }
            BitmapBits Image = new BitmapBits(bmp[0].Width, bmp[0].Height);
            Image.DrawBitmapComposited(bmp[0], Point.Empty);
            Image.DrawBitmapComposited(bmp[1], Point.Empty);
            return new Sprite(Image, off);
        }

        public static Sprite MapDPLCToBmp(byte[] artfile, byte[] mapfile, byte[] dplc, EngineVersion dplcversion, int frame, int startpal)
        {
            BitmapBits[] bmp = null;
            Point off = new Point();
            DPLC dp = new DPLC(dplc, ByteConverter.ToInt16(dplc, frame * 2), dplcversion);
            switch (LevelData.EngineVersion)
            {
                case EngineVersion.S2:
                case EngineVersion.S2NA:
                    S2Mappings s2map = new S2Mappings(mapfile, ByteConverter.ToInt16(mapfile, frame * 2));
                    bmp = LevelData.S2MapFrameDPLCToBmp(artfile, s2map, dp, startpal, out off);
                    break;
                case EngineVersion.S3K:
                case EngineVersion.SKC:
                    S3KMappings s3kmap = new S3KMappings(mapfile, ByteConverter.ToInt16(mapfile, frame * 2));
                    bmp = LevelData.S3KMapFrameDPLCToBmp(artfile, s3kmap, dp, startpal, out off);
                    break;
            }
            BitmapBits Image = new BitmapBits(bmp[0].Width, bmp[0].Height);
            Image.DrawBitmapComposited(bmp[0], Point.Empty);
            Image.DrawBitmapComposited(bmp[1], Point.Empty);
            return new Sprite(Image, off);
        }

        public static Sprite MapASMDPLCToBmp(byte[] artfile, string mapfileloc, string label, string dplcloc, string dplclabel, int startpal)
        {
            byte[] mapfile = LevelData.ASMToBin(mapfileloc, label);
            byte[] dplcfile = LevelData.ASMToBin(dplcloc, dplclabel);
            BitmapBits[] bmp = null;
            Point off = new Point();
            switch (LevelData.EngineVersion)
            {
                case EngineVersion.S2:
                case EngineVersion.S2NA:
                    bmp = LevelData.S2MapFrameDPLCToBmp(artfile, new S2Mappings(mapfile, 0), new DPLC(dplcfile, 0, EngineVersion.S2), startpal, out off);
                    break;
                case EngineVersion.S3K:
                case EngineVersion.SKC:
                    bmp = LevelData.S3KMapFrameDPLCToBmp(artfile, new S3KMappings(mapfile, 0), new DPLC(dplcfile, 0, EngineVersion.S3K), startpal, out off);
                    break;
            }
            BitmapBits Image = new BitmapBits(bmp[0].Width, bmp[0].Height);
            Image.DrawBitmapComposited(bmp[0], Point.Empty);
            Image.DrawBitmapComposited(bmp[1], Point.Empty);
            return new Sprite(Image, off);
        }

        public static Sprite MapASMDPLCToBmp(byte[] artfile, string mapfileloc, string label, string dplcloc, string dplclabel, EngineVersion dplcversion, int startpal)
        {
            byte[] mapfile = LevelData.ASMToBin(mapfileloc, label);
            byte[] dplcfile = LevelData.ASMToBin(dplcloc, dplclabel);
            BitmapBits[] bmp = null;
            Point off = new Point();
            switch (LevelData.EngineVersion)
            {
                case EngineVersion.S2:
                case EngineVersion.S2NA:
                    bmp = LevelData.S2MapFrameDPLCToBmp(artfile, new S2Mappings(mapfile, 0), new DPLC(dplcfile, 0, dplcversion), startpal, out off);
                    break;
                case EngineVersion.S3K:
                case EngineVersion.SKC:
                    bmp = LevelData.S3KMapFrameDPLCToBmp(artfile, new S3KMappings(mapfile, 0), new DPLC(dplcfile, 0, dplcversion), startpal, out off);
                    break;
            }
            BitmapBits Image = new BitmapBits(bmp[0].Width, bmp[0].Height);
            Image.DrawBitmapComposited(bmp[0], Point.Empty);
            Image.DrawBitmapComposited(bmp[1], Point.Empty);
            return new Sprite(Image, off);
        }

        public static Sprite MapASMDPLCToBmp(byte[] artfile, string mapfileloc, string dplcloc, int frame, int startpal)
        {
            return MapDPLCToBmp(artfile, LevelData.ASMToBin(mapfileloc), LevelData.ASMToBin(dplcloc), frame, startpal);
        }

        public static Sprite MapASMDPLCToBmp(byte[] artfile, string mapfileloc, string dplcloc, EngineVersion dplcversion, int frame, int startpal)
        {
            return MapDPLCToBmp(artfile, LevelData.ASMToBin(mapfileloc), LevelData.ASMToBin(dplcloc), dplcversion, frame, startpal);
        }

        public static Sprite UnknownObject { get { return new Sprite(new BitmapBits(LevelData.UnknownImg), new Point(-8, -7)); } }

        public static Sprite GetSprite(int index) { return LevelData.Sprites[index]; }

        public static byte[] LevelArt { get { return LevelData.TileArray; } }

        public static int ShiftLeft(int value, int num) { return value << num; }

        public static int ShiftRight(int value, int num) { return value >> num; }

        public static byte SetSubtypeMask(byte subtype, byte value, int mask) { return (byte)((subtype & ~mask) | (value & mask)); }
    }
}