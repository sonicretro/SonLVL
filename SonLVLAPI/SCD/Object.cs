using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SonicRetro.SonLVL.API.SCD
{
	public class Object : ObjectLayoutFormat
	{
		public override Type ObjectType { get { return typeof(SCDObjectEntry); } }

		public override List<ObjectEntry> ReadLayout(byte[] rawdata, out bool terminator)
		{
			List<ObjectEntry> objs = new List<ObjectEntry>();
			terminator = false;
			for (int oa = 0; oa < rawdata.Length; oa += SCDObjectEntry.Size)
			{
				if (ByteConverter.ToUInt64(rawdata, oa) == 0xFFFFFFFFFFFFFFFF) { terminator = true; break; }
				objs.Add(new SCDObjectEntry(rawdata, oa));
			}
			return objs;
		}

		public override byte[] WriteLayout(List<ObjectEntry> objects, bool terminator)
		{
			List<byte> tmp = new List<byte>((objects.Count + 1) + SCDObjectEntry.Size);
			foreach (ObjectEntry obj in objects)
				tmp.AddRange(obj.GetBytes());
			if (terminator)
			{
				tmp.Add(0xFF);
				while (tmp.Count % SCDObjectEntry.Size > 0)
					tmp.Add(0xFF);
			}
			return tmp.ToArray();
		}
	}

	[DefaultProperty("ID")]
	[Serializable]
	public class SCDObjectEntry : RememberStateObjectEntry
	{
		[Description("If true, the object should be loaded in the Present time zone.")]
		[DisplayName("Show in Present")]
		public virtual bool ShowPresent { get; set; }
		[Description("If true, the object should be loaded in the Past time zone.")]
		[DisplayName("Show in Past")]
		public virtual bool ShowPast { get; set; }
		[Description("If true, the object should be loaded in the Future time zone.")]
		[DisplayName("Show in Future")]
		public virtual bool ShowFuture { get; set; }

		public override byte ID
		{
			get
			{
				return base.ID;
			}
			set
			{
				base.ID = (byte)(value & 0x7F);
			}
		}

		[Browsable(false)]
		public byte SubType2 { get; set; }

		[DisplayName("SubType2")]
		public string _SubType2
		{
			get { return SubType2.ToString("X2"); }
			set { SubType2 = byte.Parse(value, System.Globalization.NumberStyles.HexNumber); }
		}

		public static int Size { get { return 8; } }

		public SCDObjectEntry() { pos = new Position(this); isLoaded = true; }

		public SCDObjectEntry(byte[] file, int address)
		{
			byte[] bytes = new byte[Size];
			Array.Copy(file, address, bytes, 0, Size);
			FromBytes(bytes);
			pos = new Position(this);
			isLoaded = true;
		}

		public override byte[] GetBytes()
		{
			List<byte> ret = new List<byte>();
			ret.AddRange(ByteConverter.GetBytes(X));
			ushort val = (ushort)(Y & 0xFFF);
			if (XFlip) val |= 0x4000;
			if (YFlip) val |= 0x8000;
			ret.AddRange(ByteConverter.GetBytes(val));
			ret.Add((byte)(ID | (RememberState ? 0x80 : 0)));
			ret.Add(SubType);
			byte b = 0;
			if (ShowPresent) b |= 0x40;
			if (ShowPast) b |= 0x20;
			if (ShowFuture) b |= 0x80;
			ret.Add(b);
			ret.Add(SubType2);
			return ret.ToArray();
		}

		public override void FromBytes(byte[] bytes)
		{
			X = ByteConverter.ToUInt16(bytes, 0);
			ushort val = ByteConverter.ToUInt16(bytes, 2);
			YFlip = (val & 0x8000) == 0x8000;
			XFlip = (val & 0x4000) == 0x4000;
			Y = (ushort)(val & 0xFFF);
			ID = bytes[4];
			RememberState = (bytes[4] & 0x80) == 0x80;
			SubType = bytes[5];
			ShowPresent = (bytes[6] & 0x40) == 0x40;
			ShowPast = (bytes[6] & 0x20) == 0x20;
			ShowFuture = (bytes[6] & 0x80) == 0x80;
			SubType2 = bytes[7];
		}
	}
}
