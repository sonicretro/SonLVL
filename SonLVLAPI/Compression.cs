using System;
using System.IO;
using SonicRetro.KensSharp;
using System.Runtime.InteropServices;

namespace SonicRetro.SonLVL.API
{
	[System.Diagnostics.DebuggerNonUserCode]
	public class Compression
	{
		private static class Comper
		{
			[DllImport("comper", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
			private static extern int compress_comper(byte[] src, int src_len, out IntPtr dst);

			[DllImport("comper", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
			private static extern int decompress_comper(byte[] src, int src_len, out IntPtr dst);

			[DllImport("comper", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
			private static extern void free_buffer(IntPtr buffer);

			static Comper()
			{
				string dir = Environment.CurrentDirectory;
				Environment.CurrentDirectory = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
				Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, IntPtr.Size == 8 ? "lib64" : "lib32");
				IntPtr dst;
				compress_comper(new byte[2], 2, out dst);
				free_buffer(dst);
				Environment.CurrentDirectory = dir;
			}

			public static void Compress(byte[] sourceData, string destinationFilePath)
			{
				IntPtr dst;
				int dst_len = compress_comper(sourceData, sourceData.Length, out dst);
				byte[] result = new byte[dst_len];
				Marshal.Copy(dst, result, 0, dst_len);
				free_buffer(dst);
				File.WriteAllBytes(destinationFilePath, result);
			}

			public static byte[] Decompress(string sourceFilePath)
			{
				byte[] sourceData = File.ReadAllBytes(sourceFilePath);
				IntPtr dst;
				int dst_len = decompress_comper(sourceData, sourceData.Length, out dst);
				byte[] result = new byte[dst_len];
				Marshal.Copy(dst, result, 0, dst_len);
				free_buffer(dst);
				return result;
			}
		}

		public static byte[] Decompress(string file, CompressionType cmp)
		{
			byte[] ret = new byte[0];
			try
			{
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
					case CompressionType.Comper:
						ret = Comper.Decompress(file);
						break;
					default:
						throw new ArgumentOutOfRangeException("cmp", "Invalid compression type " + cmp + "!");
				}
			}
			catch
			{
				LevelData.Log("Unable to read file \"" + file + "\" with compression " + cmp.ToString() + ":");
				throw;
			}
			return ret;
		}

		public static void Compress(byte[] file, string destination, CompressionType cmp)
		{
			try
			{
				switch (cmp)
				{
					case CompressionType.Uncompressed:
						File.WriteAllBytes(destination, file);
						break;
					case CompressionType.Kosinski:
						using (MemoryStream input = new MemoryStream(file))
						{
							using (FileStream output = File.Create(destination))
							{
								using (PaddedStream paddedOutput = new PaddedStream(output, 2, PaddedStreamMode.Write))
								{
									Kosinski.Compress(input, paddedOutput);
								}
							}
						}
						break;
					case CompressionType.KosinskiM:
						using (MemoryStream input = new MemoryStream(file))
						{
							using (FileStream output = File.Create(destination))
							{
								using (PaddedStream paddedOutput = new PaddedStream(output, 2, PaddedStreamMode.Write))
								{
									ModuledKosinski.Compress(input, paddedOutput, LevelData.littleendian ? Endianness.LittleEndian : Endianness.BigEndian);
								}
							}
						}
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
					case CompressionType.Comper:
						Comper.Compress(file, destination);
						break;
					default:
						throw new ArgumentOutOfRangeException("cmp", "Invalid compression type " + cmp + "!");
				}
			}
			catch
			{
				LevelData.Log("Unable to write file \"" + destination + "\" with compression " + cmp.ToString() + ":");
				throw;
			}
		}
	}

	public enum CompressionType
	{
		Invalid,
		Uncompressed,
		Kosinski,
		KosinskiM,
		Nemesis,
		Enigma,
		SZDD,
		Comper
	}
}
