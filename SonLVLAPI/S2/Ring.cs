using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace SonicRetro.SonLVL.API.S2
{
	public class Ring : RingLayoutFormat
	{
		Sprite spr;
		int spacing = 24;

		public Ring()
		{
			spr = ObjectHelper.UnknownObject;
		}

		public override void Init(ObjectData data)
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
						spr = ObjectHelper.MapDPLCToBmp(artfile, ObjectHelper.OpenArtFile(data.MapFile, data.MapCompression), data.MapVersion, ObjectHelper.OpenArtFile(data.DPLCFile, data.DPLCCompression), data.MapVersion, data.Frame, data.Palette, data.Priority);
					else
						spr = ObjectHelper.MapToBmp(artfile, ObjectHelper.OpenArtFile(data.MapFile, data.MapCompression), data.Frame, data.Palette, data.Priority, data.MapVersion);
				}
				else if (data.MapFileAsm != null)
				{
					if (data.MapAsmLabel != null)
					{
						if (data.DPLCFileAsm != null)
							spr = ObjectHelper.MapASMDPLCToBmp(artfile, data.MapFileAsm, data.MapAsmLabel, data.MapVersion, data.DPLCFileAsm, data.DPLCAsmLabel, data.MapVersion, data.Palette, data.Priority);
						else
							spr = ObjectHelper.MapASMToBmp(artfile, data.MapFileAsm, data.MapAsmLabel, data.Palette, data.Priority, data.MapVersion);
					}
					else
					{
						if (data.DPLCFileAsm != null)
							spr = ObjectHelper.MapASMDPLCToBmp(artfile, data.MapFileAsm, data.MapVersion, data.DPLCFileAsm, data.MapVersion, data.Frame, data.Palette, data.Priority);
						else
							spr = ObjectHelper.MapASMToBmp(artfile, data.MapFileAsm, data.Frame, data.Palette, data.Priority, data.MapVersion);
					}
				}
				else
					spr = ObjectHelper.UnknownObject;
				if (data.Offset != Size.Empty)
					spr.Offset(data.Offset);
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
			spacing = int.Parse(data.CustomProperties.GetValueOrDefault("spacing", "24"), System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo);
		}

		public override List<RingEntry> ReadLayout(byte[] rawdata, out bool startterm, out bool endterm)
		{
			startterm = false;
			endterm = false;
			List<RingEntry> rings = new List<RingEntry>();
			for (int i = 0; i < rawdata.Length; i += S2RingEntry.Size)
			{
				if (ByteConverter.ToUInt16(rawdata, i) == 0xFFFF) { endterm = true; break; }
				S2RingEntry ent = new S2RingEntry(rawdata, i);
				rings.Add(ent);
			}
			return rings;
		}

		public override byte[] WriteLayout(List<RingEntry> rings, bool startterm, bool endterm)
		{
			rings.Sort();
			List<byte> tmp = new List<byte>(S2RingEntry.Size * (rings.Count + 1));
			foreach (RingEntry item in rings)
				tmp.AddRange(item.GetBytes());
			if (endterm)
				tmp.AddRange(new byte[] { 0xFF, 0xFF, 0, 0 });
			return tmp.ToArray();
		}

		public override string Name
		{
			get { return "Rings"; }
		}

		public override Sprite Image
		{
			get { return spr; }
		}

		public override Sprite GetSprite(RingEntry rng)
		{
			S2RingEntry rng2 = (S2RingEntry)rng;
			List<Sprite> sprs = new List<Sprite>();
			for (int i = 0; i < rng2.Count; i++)
			{
				Sprite tmp = new Sprite(spr);
				switch (rng2.Direction)
				{
					case Direction.Horizontal:
						tmp.Offset(i * spacing, 0);
						break;
					case Direction.Vertical:
						tmp.Offset(0, i * spacing);
						break;
				}
				sprs.Add(tmp);
			}
			return new Sprite(sprs.ToArray());
		}

		public override Rectangle GetBounds(RingEntry rng)
		{
			S2RingEntry rng2 = (S2RingEntry)rng;
			switch (rng2.Direction)
			{
				case Direction.Horizontal:
					return new Rectangle(rng2.X + spr.X, rng2.Y + spr.Y, ((rng2.Count - 1) * spacing) + spr.Width, spr.Height);
				case Direction.Vertical:
					return new Rectangle(rng2.X + spr.X, rng2.Y + spr.Y, spr.Width, ((rng2.Count - 1) * spacing) + spr.Height);
			}
			return Rectangle.Empty;
		}

		public override Entry CreateRing()
		{
			return new S2RingEntry();
		}

		public override int CountRings(IEnumerable<RingEntry> rings)
		{
			return rings.OfType<S2RingEntry>().Sum(a => a.Count);
		}
	}

	[DefaultProperty("Count")]
	[Serializable]
	public class S2RingEntry : RingEntry
	{
		[DefaultValue(Direction.Horizontal)]
		[Description("The direction of the ring group.")]
		public Direction Direction { get; set; }
		private byte _count;
		[DefaultValue(1)]
		[Description("The number of rings in this group.")]
		public byte Count
		{
			get
			{
				return _count;
			}
			set
			{
				_count = Math.Max(Math.Min(value, (byte)8), (byte)1);
			}
		}

		public static int Size { get { return 4; } }

		public S2RingEntry() { pos = new Position(this); Count = 1; }

		public S2RingEntry(byte[] file, int address)
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
			ushort val = (ushort)(Y & 0xFFF);
			val |= (ushort)((Count - 1) << 12);
			if (Direction == Direction.Vertical) val |= 0x8000;
			ret.AddRange(ByteConverter.GetBytes(val));
			return ret.ToArray();
		}

		public override void FromBytes(byte[] bytes)
		{
			X = ByteConverter.ToUInt16(bytes, 0);
			ushort val = ByteConverter.ToUInt16(bytes, 2);
			Direction = (val & 0x8000) == 0x8000 ? Direction.Vertical : Direction.Horizontal;
			Count = (byte)(((val & 0x7000) >> 12) + 1);
			Y = (ushort)(val & 0xFFF);
		}

		public override void UpdateSprite()
		{
			_sprite = ((RingLayoutFormat)LevelData.RingFormat).GetSprite(this);
			_bounds = ((RingLayoutFormat)LevelData.RingFormat).GetBounds(this);
			if (_bounds.IsEmpty)
			{
				_bounds = _sprite.Bounds;
				_bounds.Offset(X, Y);
			}
		}

		public override string Name
		{
			get { return ((RingLayoutFormat)LevelData.RingFormat).Name; }
		}
	}
}
