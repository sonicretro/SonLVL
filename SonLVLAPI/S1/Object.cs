using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SonicRetro.SonLVL.API.S1
{
	public class Object : ObjectLayoutFormat
	{
		public override Type ObjectType { get { return typeof(S1ObjectEntry); } }

		public override List<ObjectEntry> ReadLayout(byte[] rawdata, out bool terminator)
		{
			List<ObjectEntry> objs = new List<ObjectEntry>();
			terminator = false;
			for (int oa = 0; oa < rawdata.Length; oa += S1ObjectEntry.Size)
			{
				if (ByteConverter.ToUInt16(rawdata, oa) == 0xFFFF) { terminator = true; break; }
				objs.Add(new S1ObjectEntry(rawdata, oa));
			}
			return objs;
		}

		public override byte[] WriteLayout(List<ObjectEntry> objects, bool terminator)
		{
			List<byte> tmp = new List<byte>((objects.Count + 1) + S1ObjectEntry.Size);
			foreach (ObjectEntry obj in objects)
				tmp.AddRange(obj.GetBytes());
			if (terminator)
				tmp.AddRange(new S1ObjectEntry() { X = 0xFFFF }.GetBytes());
			return tmp.ToArray();
		}
	}

	[DefaultProperty("ID")]
	[Serializable]
	public class S1ObjectEntry : RememberStateObjectEntry
	{
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

		public static int Size { get { return 6; } }

		public S1ObjectEntry() { pos = new Position(this); isLoaded = true; }

		public S1ObjectEntry(byte[] file, int address)
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
		}
	}
}
