using System;
using System.Collections.Generic;
using System.IO;

namespace SonicRetro.SonLVL.API
{
	/// <summary>
	/// Represents an object layout format.
	/// </summary>
	public abstract class ObjectLayoutFormat
	{
		public abstract Type ObjectType { get; }

		public virtual ObjectEntry CreateObject() { return (ObjectEntry)Activator.CreateInstance(ObjectType); }

		/// <summary>
		/// The default compression used for layout files.
		/// </summary>
		public virtual CompressionType DefaultCompression { get { return CompressionType.Uncompressed; } }

		public abstract List<ObjectEntry> ReadLayout(byte[] rawdata, out bool terminator);

		public List<ObjectEntry> ReadLayout(byte[] rawdata) { return ReadLayout(rawdata, out bool terminator); }

		public List<ObjectEntry> ReadLayout(string filename, CompressionType compression, out bool terminator)
		{
			if (compression == CompressionType.Invalid) compression = DefaultCompression;
			LevelData.Log("Loading objects from file \"" + filename + "\", using compression " + compression + "...");
			return ReadLayout(Compression.Decompress(filename, compression), out terminator);
		}

		public List<ObjectEntry> ReadLayout(string filename, CompressionType compression) { return ReadLayout(filename, compression, out bool terminator); }

		public List<ObjectEntry> ReadLayout(string filename, out bool terminator) { return ReadLayout(filename, DefaultCompression, out terminator); }

		public List<ObjectEntry> ReadLayout(string filename) { return ReadLayout(filename, DefaultCompression); }

		public List<ObjectEntry> TryReadLayout(string filename, CompressionType compression, out bool terminator)
		{
			if (File.Exists(filename))
				return ReadLayout(filename, compression, out terminator);
			else
			{
				LevelData.Log("Object file \"" + filename + "\" not found.");
				terminator = true;
				return new List<ObjectEntry>();
			}
		}

		public List<ObjectEntry> TryReadLayout(string filename, CompressionType compression) { return TryReadLayout(filename, compression, out bool terminator); }

		public List<ObjectEntry> TryReadLayout(string filename, out bool terminator) { return TryReadLayout(filename, DefaultCompression, out terminator); }

		public List<ObjectEntry> TryReadLayout(string filename) { return TryReadLayout(filename, DefaultCompression); }

		public abstract byte[] WriteLayout(List<ObjectEntry> objects, bool terminator);

		public byte[] WriteLayout(List<ObjectEntry> objects) { return WriteLayout(objects, true); }

		public void WriteLayout(List<ObjectEntry> objects, CompressionType compression, string filename, bool terminator)
		{
			if (compression == CompressionType.Invalid) compression = DefaultCompression;
			Compression.Compress(WriteLayout(objects, terminator), filename, compression);
		}

		public void WriteLayout(List<ObjectEntry> objects, CompressionType compression, string filename) { WriteLayout(objects, compression, filename, true); }

		public void WriteLayout(List<ObjectEntry> objects, string filename, bool terminator) { WriteLayout(objects, DefaultCompression, filename, terminator); }

		public void WriteLayout(List<ObjectEntry> objects, string filename) { WriteLayout(objects, DefaultCompression, filename); }
	}
}
