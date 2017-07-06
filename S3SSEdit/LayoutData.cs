using SonicRetro.SonLVL.API;

namespace S3SSEdit
{
	public abstract class LayoutData
	{
		public abstract class LayoutAccessor
		{
			public abstract SphereType this[int x, int y] { get; set; }
			public abstract int Size { get; }
			public abstract SphereType[,] Clone();
		}

		public LayoutAccessor Layout { get; protected set; }
	}

	public class SSLayoutData : LayoutData
	{
		class SSLayoutAccessor : LayoutAccessor
		{
			SSLayoutData data;

			public SSLayoutAccessor(SSLayoutData data)
			{
				this.data = data;
			}

			public override SphereType this[int x, int y] { get => data.layout[x, y]; set => data.layout[x, y] = value; }

			public override int Size => 32;

			public override SphereType[,] Clone()
			{
				return (SphereType[,])data.layout.Clone();
			}
		}

		protected SphereType[,] layout = new SphereType[32, 32];
		public ushort Angle { get; set; }
		public ushort StartX { get; set; } = 0xF00;
		public ushort StartY { get; set; } = 0xF00;
		public short Perfect { get; set; }

		public static int Size => 1032;

		public SSLayoutData()
		{
			Layout = new SSLayoutAccessor(this);
		}

		public SSLayoutData(byte[] data, int offset = 0)
			: this()
		{
			for (int i = 0; i < 1024; i++)
				layout[i % 32, i / 32] = (SphereType)data[i + offset];
			Angle = ByteConverter.ToUInt16(data, 1024 + offset);
			StartX = ByteConverter.ToUInt16(data, 1026 + offset);
			StartY = ByteConverter.ToUInt16(data, 1028 + offset);
			Perfect = ByteConverter.ToInt16(data, 1030 + offset);
		}

		public byte[] GetBytes()
		{
			byte[] result = new byte[Size];
			for (int i = 0; i < 1024; i++)
				result[i] = (byte)layout[i % 32, i / 32];
			ByteConverter.GetBytes(Angle).CopyTo(result, 1024);
			ByteConverter.GetBytes(StartX).CopyTo(result, 1026);
			ByteConverter.GetBytes(StartY).CopyTo(result, 1028);
			ByteConverter.GetBytes(Perfect).CopyTo(result, 1030);
			return result;
		}
	}

	public class BSChunkLayoutData : LayoutData
	{
		class BSChunkLayoutAccessor : LayoutAccessor
		{
			BSChunkLayoutData data;

			public BSChunkLayoutAccessor(BSChunkLayoutData data)
			{
				this.data = data;
			}

			public override SphereType this[int x, int y] { get => data.layout[x, y]; set => data.layout[x, y] = value; }

			public override int Size => 16;

			public override SphereType[,] Clone()
			{
				return (SphereType[,])data.layout.Clone();
			}
		}

		protected SphereType[,] layout = new SphereType[16, 16];
		public byte Perfect { get; set; }
		public byte Difficulty { get; set; }

		public BSChunkLayoutData()
		{
			Layout = new BSChunkLayoutAccessor(this);
		}

		public BSChunkLayoutData(byte[] data, int index)
			: this()
		{
			index &= 0x7F;
			Perfect = data[index];
			Difficulty = data[index + 0x80];
			for (int i = 0; i < 256; i++)
				layout[i % 16, i / 16] = (SphereType)data[i + (index << 8) + 0x100];
		}

		public void WriteBytes(byte[] data, int index)
		{
			index &= 0x7F;
			data[index] = Perfect;
			data[index + 0x80] = Difficulty;
			for (int i = 0; i < 256; i++)
				data[i + (index << 8) + 0x100] = (byte)layout[i % 16, i / 16];
		}
	}

	public enum SphereType { Empty, Red, Blue, Bumper, Ring, Yellow }
}
