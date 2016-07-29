using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SonicRetro.SonLVL.API.S2
{
	public class Object : ObjectLayoutFormat
	{
		public override Type ObjectType { get { return typeof(S2ObjectEntry); } }

		public override List<ObjectEntry> ReadLayout(byte[] rawdata, out bool terminator)
		{
			List<ObjectEntry> objs = new List<ObjectEntry>();
			terminator = false;
			for (int oa = 0; oa < rawdata.Length; oa += S2ObjectEntry.Size)
			{
				if (ByteConverter.ToUInt16(rawdata, oa) == 0xFFFF) { terminator = true; break; }
				objs.Add(new S2ObjectEntry(rawdata, oa));
			}
			return objs;
		}

		public override byte[] WriteLayout(List<ObjectEntry> objects, bool terminator)
		{
			List<byte> tmp = new List<byte>((objects.Count + 1) + S2ObjectEntry.Size);
			foreach (ObjectEntry obj in objects)
				tmp.AddRange(obj.GetBytes());
			if (terminator)
				tmp.AddRange(new S2ObjectEntry() { X = 0xFFFF }.GetBytes());
			return tmp.ToArray();
		}
	}

	[DefaultProperty("ID")]
	[Serializable]
	public class S2ObjectEntry : RememberStateObjectEntry
	{
		[DefaultValue(false)]
		[DisplayName("Long Distance")]
		public bool LongDistance { get; set; }

		public static int Size { get { return 6; } }

		public S2ObjectEntry() { pos = new Position(this); isLoaded = true; }

		public S2ObjectEntry(byte[] file, int address)
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
			if (LongDistance) val |= 0x1000;
			if (XFlip) val |= 0x2000;
			if (YFlip) val |= 0x4000;
			if (RememberState) val |= 0x8000;
			ret.AddRange(ByteConverter.GetBytes(val));
			ret.Add(ID);
			ret.Add(SubType);
			return ret.ToArray();
		}

		public override void FromBytes(byte[] bytes)
		{
			X = ByteConverter.ToUInt16(bytes, 0);
			ushort val = ByteConverter.ToUInt16(bytes, 2);
			RememberState = (val & 0x8000) == 0x8000;
			YFlip = (val & 0x4000) == 0x4000;
			XFlip = (val & 0x2000) == 0x2000;
			LongDistance = (val & 0x1000) == 0x1000;
			Y = (ushort)(val & 0xFFF);
			ID = bytes[4];
			SubType = bytes[5];
		}
	}
}
