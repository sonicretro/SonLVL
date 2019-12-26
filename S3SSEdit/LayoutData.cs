using SonicRetro.SonLVL.API;
using System.Linq;

namespace S3SSEdit
{
	public abstract class LayoutData
	{
		public abstract class LayoutAccessor
		{
			public abstract SphereType this[int x, int y] { get; set; }
			public abstract int Size { get; }
			public abstract SphereType[,] ToArray();
		}

		public LayoutAccessor Layout { get; protected set; }
		public abstract LayoutData Clone();
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

			public override SphereType[,] ToArray()
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
			Perfect = System.Math.Min(ByteConverter.ToInt16(data, 1030 + offset), (short)1023);
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

		public override LayoutData Clone()
		{
			SSLayoutData result = new SSLayoutData();
			result.layout = (SphereType[,])layout.Clone();
			result.Angle = Angle;
			result.StartX = StartX;
			result.StartY = StartY;
			result.Perfect = Perfect;
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

			public override SphereType[,] ToArray()
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

		public override LayoutData Clone()
		{
			BSChunkLayoutData result = new BSChunkLayoutData();
			result.layout = (SphereType[,])layout.Clone();
			result.Perfect = Perfect;
			result.Difficulty = Difficulty;
			return result;
		}
	}

	public class BSStageLayoutData : LayoutData
	{
		class BSStageLayoutAccessor : LayoutAccessor
		{
			BSStageLayoutData data;

			public BSStageLayoutAccessor(BSStageLayoutData data)
			{
				this.data = data;
			}

			public override SphereType this[int x, int y]
			{
				get
				{
					int q = 0;
					if (x > 15) q = 1;
					if (y > 15) q += 2;
					switch (q)
					{
						case 0:
							return data.layouts[0].Layout[x, y];
						case 1:
							return data.layouts[1].Layout[15 - (x - 16), y];
						case 2:
							return data.layouts[2].Layout[x, 15 - (y - 16)];
						case 3:
							return data.layouts[3].Layout[15 - (x - 16), 15 - (y - 16)];
					}
					return 0;
				}
				set
				{
					int q = 0;
					if (x > 15) q = 1;
					if (y > 15) q += 2;
					switch (q)
					{
						case 0:
							data.layouts[0].Layout[x, y] = value;
							break;
						case 1:
							data.layouts[1].Layout[15 - (x - 16), y] = value;
							break;
						case 2:
							data.layouts[2].Layout[x, 15 - (y - 16)] = value;
							break;
						case 3:
							data.layouts[3].Layout[15 - (x - 16), 15 - (y - 16)] = value;
							break;
					}
				}
			}

			public override int Size => 32;

			public override SphereType[,] ToArray()
			{
				SphereType[,] result = new SphereType[32, 32];
				for (int y = 0; y < 32; y++)
					for (int x = 0; x < 32; x++)
						result[x, y] = this[x, y];
				return result;
			}
		}

		readonly byte[] chunknums = new byte[4];
		protected readonly BSChunkLayoutData[] layouts = new BSChunkLayoutData[4];
		public short Perfect { get => (short)layouts.Sum(a => a.Perfect); }
		public byte Difficulty { get => (byte)layouts.Sum(a => a.Difficulty); }

		private BSStageLayoutData()
		{
			Layout = new BSStageLayoutAccessor(this);
		}

		public BSStageLayoutData(byte[] data, byte[] chunknums)
			: this()
		{
			chunknums.CopyTo(this.chunknums, 0);
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < i; j++)
					if (chunknums[i] == chunknums[j])
					{
						layouts[i] = layouts[j];
						break;
					}
				if (layouts[i] == null)
					layouts[i] = new BSChunkLayoutData(data, chunknums[i]);
			}
		}

		public void WriteBytes(byte[] data)
		{
			for (int i = 0; i < 4; i++)
				layouts[i].WriteBytes(data, chunknums[i]);
		}

		public byte GetPerfect(int quadrant)
		{
			return layouts[quadrant].Perfect;
		}

		public void SetPerfect(int quadrant, byte count)
		{
			layouts[quadrant].Perfect = count;
		}

		public byte GetDifficulty(int quadrant)
		{
			return layouts[quadrant].Difficulty;
		}

		public void SetDifficulty(int quadrant, byte dif)
		{
			layouts[quadrant].Difficulty = dif;
		}

		public override LayoutData Clone()
		{
			BSStageLayoutData result = new BSStageLayoutData();
			chunknums.CopyTo(result.chunknums, 0);
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < i; j++)
					if (chunknums[i] == chunknums[j])
					{
						result.layouts[i] = result.layouts[j];
						break;
					}
				if (result.layouts[i] == null)
					result.layouts[i] = (BSChunkLayoutData)layouts[i].Clone();
			}
			return result;
		}
	}

	public enum SphereType { Empty, Red, Blue, Bumper, Ring, Yellow }
}
