using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SonicRetro.SonLVL.API.S3K
{
	public class Ring : RingLayoutFormat
	{
		Sprite spr;

		public Ring()
		{
			spr = ObjectHelper.UnknownObject;
		}

		public override List<RingEntry> ReadLayout(byte[] rawdata)
		{
			List<RingEntry> rings = new List<RingEntry>();
			for (int i = 0; i < rawdata.Length; i += S3KRingEntry.Size)
			{
				if (ByteConverter.ToUInt16(rawdata, i) == 0xFFFF) break;
				S3KRingEntry ent = new S3KRingEntry(rawdata, i);
				rings.Add(ent);
			}
			return rings;
		}

		public override byte[] WriteLayout(List<RingEntry> rings)
		{
			rings.Sort();
			List<byte> tmp = new List<byte>(S3KRingEntry.Size * (rings.Count + 1));
			foreach (RingEntry item in rings)
				tmp.AddRange(item.GetBytes());
			tmp.AddRange(new byte[] { 0xFF, 0xFF, 0, 0 });
			return tmp.ToArray();
		}

		public override void Init(ObjectData data)
		{
			try
			{
				if (data.Art != null)
				{
					MultiFileIndexer<byte> art = new MultiFileIndexer<byte>();
					foreach (FileInfo file in data.Art)
						art.AddFile(new List<byte>(ObjectHelper.OpenArtFile(file.Filename, data.ArtCompression)), file.Offset);
					byte[] artfile = art.ToArray();
					if (data.MapFile != null)
					{
						if (data.DPLCFile != null)
							spr = ObjectHelper.MapDPLCToBmp(artfile, LevelData.ReadFile(data.MapFile, data.MapCompression), data.MapVersion, LevelData.ReadFile(data.DPLCFile, data.DPLCCompression), data.DPLCVersion, data.Frame, data.Palette);
						else
							spr = ObjectHelper.MapToBmp(artfile, LevelData.ReadFile(data.MapFile, data.MapCompression), data.Frame, data.Palette, data.MapVersion);
					}
					else if (data.MapFileAsm != null)
					{
						if (data.MapAsmLabel != null)
						{
							if (data.DPLCFileAsm != null)
								spr = ObjectHelper.MapASMDPLCToBmp(artfile, data.MapFileAsm, data.MapAsmLabel, data.MapVersion, data.DPLCFileAsm, data.DPLCAsmLabel, data.DPLCVersion, data.Palette);
							else
								spr = ObjectHelper.MapASMToBmp(artfile, data.MapFileAsm, data.MapAsmLabel, data.Palette, data.MapVersion);
						}
						else
						{
							if (data.DPLCFileAsm != null)
								spr = ObjectHelper.MapASMDPLCToBmp(artfile, data.MapFileAsm, data.MapVersion, data.DPLCFileAsm, data.DPLCVersion, data.Frame, data.Palette);
							else
								spr = ObjectHelper.MapASMToBmp(artfile, data.MapFileAsm, data.Frame, data.Palette, data.MapVersion);
						}
					}
					else
						spr = ObjectHelper.UnknownObject;
					if (data.Offset != Size.Empty)
						spr.Offset = spr.Offset + data.Offset;
				}
				else if (data.Image != null)
				{
					BitmapBits img = new BitmapBits(data.Image);
					spr = new Sprite(img, new Point(data.Offset));
				}
				else if (data.Sprite > -1)
					spr = ObjectHelper.GetSprite(data.Sprite);
				else
					spr = ObjectHelper.UnknownObject;
			}
			catch (Exception ex)
			{
				LevelData.Log("Error loading S3K ring definition:", ex.ToString());
				spr = ObjectHelper.UnknownObject;
			}
		}

		public override Sprite Image { get { return spr; } }

		public override Rectangle GetBounds(RingEntry rng, Point camera)
		{
			return new Rectangle((rng.X + spr.Offset.X) - camera.X, (rng.Y + spr.Offset.Y) - camera.Y, spr.Image.Width, spr.Image.Height);
		}

		public override Sprite GetSprite(RingEntry rng)
		{
			return new Sprite(spr.Image, new Point(rng.X + spr.Offset.X, rng.Y + spr.Offset.Y));
		}

		public override Entry CreateRing()
		{
			return new S3KRingEntry();
		}

		public override string Name
		{
			get { return "Ring"; }
		}

		public override int CountRings(IEnumerable<RingEntry> rings)
		{
			return rings.OfType<S3KRingEntry>().Count();
		}
	}

	[Serializable]
	public class S3KRingEntry : RingEntry
	{
		public static int Size { get { return 4; } }

		public S3KRingEntry() { pos = new Position(this); }

		public S3KRingEntry(byte[] file, int address)
		{
			byte[] bytes = new byte[Size];
			Array.Copy(file, address, bytes, 0, Size);
			FromBytes(bytes);
			pos = new Position(this);
		}

		public override byte[] GetBytes()
		{
			List<byte> ret = new List<byte>();
			ret.AddRange(ByteConverter.GetBytes(X));
			ret.AddRange(ByteConverter.GetBytes(Y));
			return ret.ToArray();
		}

		public override void FromBytes(byte[] bytes)
		{
			X = ByteConverter.ToUInt16(bytes, 0);
			Y = ByteConverter.ToUInt16(bytes, 2);
		}

		public override void UpdateSprite()
		{
			Sprite = ((RingLayoutFormat)LevelData.RingFormat).GetSprite(this);
		}
	}
}
