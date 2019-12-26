using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SonicRetro.SonLVL.API.S3K
{
	public class Object : ObjectLayoutFormat
	{
		public override Type ObjectType { get { return typeof(S3KObjectEntry); } }

		public override List<ObjectEntry> ReadLayout(byte[] rawdata, out bool terminator)
		{
			List<ObjectEntry> objs = new List<ObjectEntry>();
			terminator = false;
			for (int oa = 0; oa < rawdata.Length; oa += S3KObjectEntry.Size)
			{
				if (ByteConverter.ToUInt16(rawdata, oa) == 0xFFFF) { terminator = true; break; }
				objs.Add(new S3KObjectEntry(rawdata, oa));
			}
			return objs;
		}

		public override byte[] WriteLayout(List<ObjectEntry> objects, bool terminator)
		{
			List<byte> tmp = new List<byte>((objects.Count + 1) + S3KObjectEntry.Size);
			foreach (ObjectEntry obj in objects)
				tmp.AddRange(obj.GetBytes());
			if (terminator)
				tmp.AddRange(new S3KObjectEntry() { X = 0xFFFF }.GetBytes());
			return tmp.ToArray();
		}
	}


	[DefaultProperty("ID")]
	[Serializable]
	public class S3KObjectEntry : ObjectEntry
	{
		[DefaultValue(false)]
		[Description("If true, the object will be loaded when it is in horizontal range of the screen, regardless of its Y position.")]
		[DisplayName("Load at any Y position")]
		public bool LoadAtAnyYPos { get; set; }

		public static int Size { get { return 6; } }

		public S3KObjectEntry() { pos = new Position(this); isLoaded = true; }

		public S3KObjectEntry(byte[] file, int address)
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
			if (XFlip) val |= 0x2000;
			if (YFlip) val |= 0x4000;
			if (LoadAtAnyYPos) val |= 0x8000;
			ret.AddRange(ByteConverter.GetBytes(val));
			ret.Add(ID);
			ret.Add(SubType);
			return ret.ToArray();
		}

		public override void FromBytes(byte[] bytes)
		{
			X = ByteConverter.ToUInt16(bytes, 0);
			ushort val = ByteConverter.ToUInt16(bytes, 2);
			LoadAtAnyYPos = (val & 0x8000) == 0x8000;
			YFlip = (val & 0x4000) == 0x4000;
			XFlip = (val & 0x2000) == 0x2000;
			Y = (ushort)(val & 0xFFF);
			ID = bytes[4];
			SubType = bytes[5];
		}
	}
}
