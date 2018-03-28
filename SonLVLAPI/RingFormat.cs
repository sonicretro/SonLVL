using System.Collections.Generic;
using System.Drawing;
using System.IO;
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

		public abstract List<RingEntry> ReadLayout(byte[] rawdata, out bool startterm, out bool endterm);

		public List<RingEntry> ReadLayout(byte[] rawdata) { return ReadLayout(rawdata, out bool startterm, out bool endterm); }

		public List<RingEntry> ReadLayout(string filename, CompressionType compression, out bool startterm, out bool endterm)
		{
			if (compression == CompressionType.Invalid) compression = DefaultCompression;
			LevelData.Log("Loading rings from file \"" + filename + "\", using compression " + compression + "...");
			return ReadLayout(Compression.Decompress(filename, compression), out startterm, out endterm);
		}

		public List<RingEntry> ReadLayout(string filename, CompressionType compression) { return ReadLayout(filename, compression, out bool startterm, out bool endterm); }

		public List<RingEntry> ReadLayout(string filename, out bool startterm, out bool endterm) { return ReadLayout(filename, DefaultCompression, out startterm, out endterm); }

		public List<RingEntry> ReadLayout(string filename) { return ReadLayout(filename, DefaultCompression); }

		public List<RingEntry> TryReadLayout(string filename, CompressionType compression, out bool startterm, out bool endterm)
		{
			if (File.Exists(filename))
				return ReadLayout(filename, compression, out startterm, out endterm);
			else
			{
				LevelData.Log("Ring file \"" + filename + "\" not found.");
				startterm = false;
				endterm = false;
				return new List<RingEntry>();
			}
		}

		public List<RingEntry> TryReadLayout(string filename, CompressionType compression) { return TryReadLayout(filename, compression, out bool startterm, out bool endterm); }

		public List<RingEntry> TryReadLayout(string filename, out bool startterm, out bool endterm) { return TryReadLayout(filename, DefaultCompression, out startterm, out endterm); }

		public List<RingEntry> TryReadLayout(string filename) { return TryReadLayout(filename, DefaultCompression); }

		public abstract byte[] WriteLayout(List<RingEntry> rings, bool startterm, bool endterm);

		public byte[] WriteLayout(List<RingEntry> rings) { return WriteLayout(rings, false, true); }

		public void WriteLayout(List<RingEntry> rings, CompressionType compression, string filename, bool startterm, bool endterm)
		{
			if (compression == CompressionType.Invalid) compression = DefaultCompression;
			Compression.Compress(WriteLayout(rings, startterm, endterm), filename, compression);
		}

		public void WriteLayout(List<RingEntry> rings, CompressionType compression, string filename) { WriteLayout(rings, compression, filename, false, true); }

		public void WriteLayout(List<RingEntry> rings, string filename, bool startterm, bool endterm) { WriteLayout(rings, DefaultCompression, filename, startterm, endterm); }

		public void WriteLayout(List<RingEntry> rings, string filename) { WriteLayout(rings, DefaultCompression, filename); }

		public abstract void Init(ObjectData data);

		public abstract string Name { get; }

		public abstract Sprite Image { get; }

		public abstract Sprite GetSprite(RingEntry rng);

#pragma warning disable CS0618 // Type or member is obsolete
		public virtual Rectangle GetBounds(RingEntry rng) { return GetBounds(rng, Point.Empty); }
#pragma warning restore CS0618 // Type or member is obsolete
		[System.Obsolete("The two-argument version of this function is obsolete. Please change your code to use the single-argument version instead.")]
		public virtual Rectangle GetBounds(RingEntry rng, Point camera) { return Rectangle.Empty; }

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
