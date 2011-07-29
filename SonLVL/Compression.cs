using SonicRetro.KensSharp;

namespace SonicRetro.SonLVL
{
    public class Compression
    {
        public static byte[] Decompress(string file, CompressionType cmp)
        {
            byte[] ret = new byte[0];
            switch (cmp)
            {
                case CompressionType.Uncompressed:
                    ret = System.IO.File.ReadAllBytes(file);
                    break;
                case CompressionType.Kosinski:
                    ret = Kosinski.Decompress(file);
                    break;
                case CompressionType.KosinskiM:
                    ret = ModuledKosinski.Decompress(file, LevelData.littleendian ? Endianness.LittleEndian : Endianness.BigEndian);
                    break;
                case CompressionType.Nemesis:
                    ret = Nemesis.Decompress(file);
                    break;
                case CompressionType.Enigma:
                    ret = Enigma.Decompress(file, LevelData.littleendian ? Endianness.LittleEndian : Endianness.BigEndian);
                    break;
                case CompressionType.SZDD:
                    ret = SZDDComp.SZDDComp.Decompress(file);
                    break;
                default:
                    break;
            }
            return ret;
        }

        public static void Compress(byte[] file, string destination, CompressionType cmp)
        {
            switch (cmp)
            {
                case CompressionType.Uncompressed:
                    System.IO.File.WriteAllBytes(destination, file);
                    break;
                case CompressionType.Kosinski:
                    Kosinski.Compress(file, destination);
                    break;
                case CompressionType.KosinskiM:
                    ModuledKosinski.Compress(file, destination, LevelData.littleendian ? Endianness.LittleEndian : Endianness.BigEndian);
                    break;
                case CompressionType.Nemesis:
                    Nemesis.Compress(file, destination);
                    break;
                case CompressionType.Enigma:
                    Enigma.Compress(file, destination, LevelData.littleendian ? Endianness.LittleEndian : Endianness.BigEndian);
                    break;
                case CompressionType.SZDD:
                    SZDDComp.SZDDComp.Compress(file, destination);
                    break;
                default:
                    break;
            }
        }

        public enum CompressionType
        {
            Uncompressed,
            Kosinski,
            KosinskiM,
            Nemesis,
            Enigma,
            SZDD
        }
    }
}