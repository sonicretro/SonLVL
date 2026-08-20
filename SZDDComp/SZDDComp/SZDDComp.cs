using System;
using System.IO;

namespace SZDDComp
{
    public partial class SZDDComp
    {

        public static byte[] Decompress(string sourceFilePath)
        {
            using (FileStream input = File.OpenRead(sourceFilePath))
            {
                using (MemoryStream output = new MemoryStream())
                {
                    Decompress(input, output);
                    return output.ToArray();
                }
            }
        }

        public static void Decompress(byte[] sourceData, string destinationFilePath)
        {
            using (MemoryStream input = new MemoryStream(sourceData))
            {
                using (FileStream output = File.Create(destinationFilePath))
                {
                    Decompress(input, output);
                }
            }
        }

        public static void Decompress(string sourceFilePath, string destinationFilePath)
        {
            using (FileStream input = File.OpenRead(sourceFilePath))
            {
                using (FileStream output = File.Create(destinationFilePath))
                {
                    Decompress(input, output);
                }
            }
        }

        public static byte[] Decompress(byte[] sourceData)
        {
            using (MemoryStream input = new MemoryStream(sourceData))
            {
                using (MemoryStream output = new MemoryStream())
                {
                    Decompress(input, output);
                    return output.ToArray();
                }
            }
        }

        public static void Decompress(Stream input, Stream output)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            if (output == null)
            {
                throw new ArgumentNullException("output");
            }

            Decode(input, output);
        }

        public static byte[] Compress(string sourceFilePath)
        {
            using (FileStream input = File.OpenRead(sourceFilePath))
            {
                using (MemoryStream output = new MemoryStream())
                {
                    Compress(input, output);
                    return output.ToArray();
                }
            }
        }

        public static void Compress(byte[] sourceData, string destinationFilePath)
        {
            using (MemoryStream input = new MemoryStream(sourceData))
            {
                using (FileStream output = File.Create(destinationFilePath))
                {
                    Compress(input, output);
                }
            }
        }

        public static void Compress(string sourceFilePath, string destinationFilePath)
        {
            using (FileStream input = File.OpenRead(sourceFilePath))
            {
                using (FileStream output = File.Create(destinationFilePath))
                {
                    Compress(input, output);
                }
            }
        }

        public static byte[] Compress(byte[] sourceData)
        {
            using (MemoryStream input = new MemoryStream(sourceData))
            {
                using (MemoryStream output = new MemoryStream())
                {
                    Compress(input, output);
                    return output.ToArray();
                }
            }
        }

        public static void Compress(Stream input, Stream output)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            if (output == null)
            {
                throw new ArgumentNullException("output");
            }

            Encode(input, output);
        }
    }
}
