using System.Collections.Generic;
using System.IO;
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
                    ret = File.ReadAllBytes(file);
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
            List<byte> outfile = new List<byte>();
            switch (cmp)
            {
                case CompressionType.Uncompressed:
                    outfile.AddRange(file);
                    break;
                case CompressionType.Kosinski:
                    outfile.AddRange(Kosinski.Compress(file));
                    if (outfile.Count % 2 == 1)
                        outfile.Add(0);
                    break;
                case CompressionType.KosinskiM:
                    outfile.AddRange(ModuledKosinski.Compress(file, LevelData.littleendian ? Endianness.LittleEndian : Endianness.BigEndian));
                    if (outfile.Count % 2 == 1)
                        outfile.Add(0);
                    break;
                case CompressionType.Nemesis:
                    outfile.AddRange(Nemesis.Compress(file));
                    break;
                case CompressionType.Enigma:
                    outfile.AddRange(Enigma.Compress(file, LevelData.littleendian ? Endianness.LittleEndian : Endianness.BigEndian));
                    break;
                case CompressionType.SZDD:
                    outfile.AddRange(SZDDComp.SZDDComp.Compress(file));
                    break;
                default:
                    return;
            }
            File.WriteAllBytes(destination, outfile.ToArray());
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