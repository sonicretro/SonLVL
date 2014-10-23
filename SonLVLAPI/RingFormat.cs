using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Linq;

namespace SonicRetro.SonLVL.API
{
	/// <summary>
	/// Represents a ring layout format.
	/// </summary>
	public abstract class RingFormat
	{
		public abstract Entry CreateRing();
	}

	public abstract class RingLayoutFormat : RingFormat
	{
		/// <summary>
		/// The default compression used for layout files.
		/// </summary>
		public virtual CompressionType DefaultCompression { get { return CompressionType.Uncompressed; } }

		public abstract List<RingEntry> ReadLayout(byte[] rawdata);

		public List<RingEntry> ReadLayout(string filename, CompressionType compression)
		{
			if (compression == CompressionType.Invalid) compression = DefaultCompression;
			LevelData.Log("Loading rings from file \"" + filename + "\", using compression " + compression + "...");
			return ReadLayout(Compression.Decompress(filename, compression));
		}

		public List<RingEntry> ReadLayout(string filename) { return ReadLayout(filename, DefaultCompression); }

		public List<RingEntry> TryReadLayout(string filename, CompressionType compression)
		{
			if (File.Exists(filename))
				return ReadLayout(filename, compression);
			else
			{
				LevelData.Log("Ring file \"" + filename + "\" not found.");
				return new List<RingEntry>();
			}
		}

		public List<RingEntry> TryReadLayout(string filename) { return TryReadLayout(filename, DefaultCompression); }

		public abstract byte[] WriteLayout(List<RingEntry> rings);

		public void WriteLayout(List<RingEntry> rings, CompressionType compression, string filename)
		{
			if (compression == CompressionType.Invalid) compression = DefaultCompression;
			Compression.Compress(WriteLayout(rings), filename, compression);
		}

		public void WriteLayout(List<RingEntry> rings, string filename) { WriteLayout(rings, DefaultCompression, filename); }

		public abstract void Init(ObjectData data);

		public abstract string Name { get; }

		public abstract Sprite Image { get; }

		public abstract Sprite GetSprite(RingEntry rng);

		public abstract Rectangle GetBounds(RingEntry rng, Point camera);

		public abstract int CountRings(IEnumerable<RingEntry> rings);
	}

	public abstract class RingObjectFormat : RingFormat
	{
		public abstract byte ObjectID { get; }

		public override Entry CreateRing()
		{
			return LevelData.CreateObject(ObjectID);
		}

		public int CountRings(IEnumerable<ObjectEntry> objects)
		{
			PropertySpec prop = LevelData.GetObjectDefinition(ObjectID).CustomProperties.SingleOrDefault(a => a.Name == "Count");
			if (prop != null)
				return objects.Where(a => a.ID == ObjectID).Sum(a => (int)prop.GetValue(a));
			else
				return objects.Count(a => a.ID == ObjectID);
		}
	}
}
