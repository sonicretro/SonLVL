using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SonicRetro.SonLVL.API
{
	public abstract class TileFormat
	{
		public virtual List<byte[]> ReadTiles(byte[] rawdata)
		{
			byte[] tmp = new byte[rawdata.Length];
			LevelData.Pad(ref tmp, 32);
			rawdata.CopyTo(tmp, 0);
			List<byte[]> tiles = new List<byte[]>();
			for (int i = 0; i < tmp.Length; i += 32)
			{
				byte[] tile = new byte[32];
				Array.Copy(tmp, i, tile, 0, 32);
				tiles.Add(tile);
			}
			return tiles;
		}

		public List<byte[]> ReadTiles(string filename, CompressionType compression)
		{
			if (compression == CompressionType.Invalid) compression = DefaultCompression;
			LevelData.Log("Loading 8x8 tiles from file \"" + filename + "\", using compression " + compression.ToString() + "...");
			return ReadTiles(Compression.Decompress(filename, compression));
		}

		public List<byte[]> ReadTiles(string filename) { return ReadTiles(filename, DefaultCompression); }

		public List<byte[]> TryReadTiles(string filename, CompressionType compression)
		{
			if (System.IO.File.Exists(filename))
				return ReadTiles(filename, compression);
			else
			{
				LevelData.Log("8x8 tile file \"" + filename + "\" not found.");
				return new List<byte[]>() { new byte[32] };
			}
		}

		public virtual MultiFileIndexer<byte[]> ReadTiles(FileInfo[] files, CompressionType compression)
		{
			MultiFileIndexer<byte[]> result = new MultiFileIndexer<byte[]>();
			foreach (FileInfo file in files)
				result.AddFile(ReadTiles(file.Filename, compression), file.Offset == -1 ? file.Offset : file.Offset / 32);
			return result;
		}

		public MultiFileIndexer<byte[]> ReadTiles(FileInfo[] files) { return ReadTiles(files, DefaultCompression); }

		public virtual MultiFileIndexer<byte[]> TryReadTiles(FileInfo[] files, CompressionType compression)
		{
			MultiFileIndexer<byte[]> result = new MultiFileIndexer<byte[]>();
			foreach (FileInfo file in files)
				result.AddFile(TryReadTiles(file.Filename, compression), file.Offset == -1 ? file.Offset : file.Offset / 32);
			return result;
		}

		public MultiFileIndexer<byte[]> TryReadTiles(FileInfo[] files) { return TryReadTiles(files, DefaultCompression); }

		public virtual CompressionType DefaultCompression { get { return CompressionType.Uncompressed; } }
	}
}
