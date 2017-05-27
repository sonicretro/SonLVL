using SonicRetro.SonLVL.API;

namespace S3SSEdit
{
	public class LayoutData
	{
		public SphereType[,] Layout { get; private set; } = new SphereType[32, 32];
		public ushort Angle { get; set; }
		public ushort StartX { get; set; } = 0xF00;
		public ushort StartY { get; set; } = 0xF00;
		public short Perfect { get; set; }

		public static int Size => 1032;

		public LayoutData() { }

		public LayoutData(byte[] data, int offset = 0)
		{
			for (int i = 0; i < 1024; i++)
				Layout[i % 32, i / 32] = (SphereType)data[i + offset];
			Angle = ByteConverter.ToUInt16(data, 1024 + offset);
			StartX = ByteConverter.ToUInt16(data, 1026 + offset);
			StartY = ByteConverter.ToUInt16(data, 1028 + offset);
			Perfect = ByteConverter.ToInt16(data, 1030 + offset);
		}

		public byte[] GetBytes()
		{
			byte[] result = new byte[Size];
			for (int i = 0; i < 1024; i++)
				result[i] = (byte)Layout[i % 32, i / 32];
			ByteConverter.GetBytes(Angle).CopyTo(result, 1024);
			ByteConverter.GetBytes(StartX).CopyTo(result, 1026);
			ByteConverter.GetBytes(StartY).CopyTo(result, 1028);
			ByteConverter.GetBytes(Perfect).CopyTo(result, 1030);
			return result;
		}
	}

	public enum SphereType { Empty, Red, Blue, Bumper, Ring, Yellow }
}
