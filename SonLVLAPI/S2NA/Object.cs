using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SonicRetro.SonLVL.API.S2NA
{
	public class Object : S2.Object
	{
		public override Type ObjectType { get { return typeof(S2NAObjectEntry); } }
	}

	[DefaultProperty("ID")]
	[Serializable]
	public class S2NAObjectEntry : S2.S2ObjectEntry
	{
		public S2NAObjectEntry() { pos = new Position(this); isLoaded = true; }

		public S2NAObjectEntry(byte[] file, int address)
		{
			byte[] bytes = new byte[Size];
			Array.Copy(file, address, bytes, 0, Size);
			FromBytes(bytes);
			pos = new Position(this);
			isLoaded = true;
		}

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

		public override byte[] GetBytes()
		{
			List<byte> ret = new List<byte>();
			ret.AddRange(ByteConverter.GetBytes(X));
			ushort val = (ushort)(Y & 0xFFF);
			if (LongDistance) val |= 0x2000;
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
			LongDistance = (val & 0x2000) == 0x2000;
			Y = (ushort)(val & 0xFFF);
			ID = bytes[4];
			RememberState = (bytes[4] & 0x80) == 0x80;
			SubType = bytes[5];
		}
	}
}
