using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SonicRetro.SonLVL.API.Chaotix
{
	public class Object : ObjectLayoutFormat
	{
		public override Type ObjectType { get { return typeof(ChaotixObjectEntry); } }

		public override List<ObjectEntry> ReadLayout(byte[] rawdata, out bool terminator)
		{
			List<ObjectEntry> objs = new List<ObjectEntry>();
			terminator = false;
			for (int oa = 0; oa < rawdata.Length; oa += ChaotixObjectEntry.Size)
			{
				if (ByteConverter.ToUInt16(rawdata, oa) == 0xFFFF) { terminator = true; break; }
				objs.Add(new ChaotixObjectEntry(rawdata, oa));
			}
			return objs;
		}

		public override byte[] WriteLayout(List<ObjectEntry> objects, bool terminator)
		{
			List<byte> tmp = new List<byte>((objects.Count + 1) + ChaotixObjectEntry.Size);
			foreach (ObjectEntry obj in objects)
				tmp.AddRange(obj.GetBytes());
			if (terminator)
				tmp.AddRange(new ChaotixObjectEntry() { X = 0xFFFF }.GetBytes());
			return tmp.ToArray();
		}
	}

	[DefaultProperty("ID")]
	[Serializable]
	public class ChaotixObjectEntry : ObjectEntry
	{
		[Browsable(false)]
		public override byte SubType
		{
			get
			{
				return (byte)fullSubType;
			}
			set
			{
				fullSubType = value;
			}
		}

		private ushort fullSubType;
		[DefaultValue(0)]
		[DisplayName("SubType")]
		[Description("The subtype of the object.")]
		[Editor(typeof(SubTypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
		[TypeConverter(typeof(UInt16HexConverter))]
		public ushort FullSubType
		{
			get { return fullSubType; }
			set { fullSubType = (ushort)(value & 0x1FFF); }
		}

		[DefaultValue(false)]
		[Description("If true, the object will be loaded when it is in horizontal range of the screen, regardless of its Y position.")]
		[DisplayName("Make object manager ignore Y position")]
		public virtual bool SomeFlag { get; set; }

		public static int Size { get { return 8; } }

		public ChaotixObjectEntry() { pos = new Position(this); isLoaded = true; }

		/* Format: 
		8 bytes per entry:
			x_pos (word)
			y_pos (word)
			4*ID (word)
			secondary subtype (byte), highest 3 bits are No-Y-Check, Y-Flip, X-Flip
			subtype (byte)
		*/

		public ChaotixObjectEntry(byte[] file, int address)
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
			ret.AddRange(ByteConverter.GetBytes(Y));
			ret.AddRange(ByteConverter.GetBytes((ushort)(ID << 2)));
			ushort val = fullSubType;
			if (XFlip) val |= 0x2000;
			if (YFlip) val |= 0x4000;
			if (SomeFlag) val |= 0x8000;
			ret.AddRange(ByteConverter.GetBytes(val));
			return ret.ToArray();
		}

		public override void FromBytes(byte[] bytes)
		{
			X = ByteConverter.ToUInt16(bytes, 0);
			Y = ByteConverter.ToUInt16(bytes, 2);
			ID = (byte)(ByteConverter.ToUInt16(bytes, 4) >> 2);

			ushort val = ByteConverter.ToUInt16(bytes, 6);

			SomeFlag = (val & 0x8000) == 0x8000;
			YFlip = (val & 0x4000) == 0x4000;
			XFlip = (val & 0x2000) == 0x2000;
			fullSubType = (ushort)(val & 0x1FFF);
		}
	}
}
