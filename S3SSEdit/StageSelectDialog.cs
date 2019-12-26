using SonicRetro.SonLVL.API;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace S3SSEdit
{
	public partial class StageSelectDialog : Form
	{
		string path;
		ProjectFile project;

		internal StageSelectDialog(string filename, ProjectFile project)
		{
			InitializeComponent();
			path = Path.GetDirectoryName(filename);
			this.project = project;
		}

		internal LayoutMode Category { get; private set; } = LayoutMode.S3;
		internal int StageNumber { get; private set; } = 0;
		internal byte[] BSChunks { get; private set; } = new byte[4];
		internal BSMode BSMode { get; private set; } = BSMode.Level;

		private void StageSelectDialog_Load(object sender, EventArgs e)
		{
			if (stageList.SelectedIndex == -1)
				stageList.SelectedIndex = 0;
			else
				DrawPreview();
		}

		private void categoryS3_CheckedChanged(object sender, EventArgs e)
		{
			if (categoryS3.Checked)
			{
				Category = LayoutMode.S3;
				chunkSelector.Visible = false;
				bsStageControlPanel.Visible = false;
				stageList.Visible = true;
				if (stageList.SelectedIndex != 0)
					stageList.SelectedIndex = 0;
				else
					DrawPreview();
			}
		}

		private void categorySK_CheckedChanged(object sender, EventArgs e)
		{
			if (categorySK.Checked)
			{
				Category = LayoutMode.SK;
				chunkSelector.Visible = false;
				bsStageControlPanel.Visible = false;
				stageList.Visible = true;
				if (stageList.SelectedIndex != 0)
					stageList.SelectedIndex = 0;
				else
					DrawPreview();
			}
		}

		private void categoryBSChunk_CheckedChanged(object sender, EventArgs e)
		{
			if (categoryBSChunk.Checked)
			{
				Category = LayoutMode.BSChunk;
				stageList.Visible = false;
				bsStageControlPanel.Visible = false;
				chunkSelector.Visible = true;
				if (chunkSelector.Value != 0)
					chunkSelector.Value = 0;
				else
					DrawPreview();
			}
		}

		private void categoryBSLayout_CheckedChanged(object sender, EventArgs e)
		{
			if (categoryBSLayout.Checked)
			{
				Category = LayoutMode.BSLayout;
				stageList.Visible = false;
				chunkSelector.Visible = false;
				bsStageControlPanel.Visible = true;
				if (bsLevelNum.Value != 1)
					bsLevelNum.Value = 1;
				GenerateCode(0, false);
				LevelChanged(0);
			}
		}

		private void stageList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (stageList.SelectedIndex != -1)
			{
				StageNumber = stageList.SelectedIndex;
				DrawPreview();
			}
		}

		private void chunkSelector_ValueChanged(object sender, EventArgs e)
		{
			StageNumber = (int)chunkSelector.Value;
			DrawPreview();
		}

		private void bsChunksButton_Click(object sender, EventArgs e)
		{
			BSMode = BSMode.Stage;
			bsLevelNum.Value = 0;
			BSChunks[0] = (byte)bsStageChunk1.Value;
			BSChunks[1] = (byte)bsStageChunk2.Value;
			BSChunks[2] = (byte)bsStageChunk3.Value;
			BSChunks[3] = (byte)bsStageChunk4.Value;
			StageChanged();
		}

		private void bsStageButton_Click(object sender, EventArgs e)
		{
			BSMode = BSMode.Stage;
			bsLevelNum.Value = 0;
			uint stg = (uint)bsStageNum.Value;
			bsStageChunk1.Value = BSChunks[0] = (byte)(stg & 0x7F);
			bsStageChunk2.Value = BSChunks[1] = (byte)((stg >> 7) & 0x7F);
			bsStageChunk3.Value = BSChunks[2] = (byte)((stg >> 14) & 0x7F);
			bsStageChunk4.Value = BSChunks[3] = (byte)((stg >> 21) & 0x7F);
			DrawPreview();
		}

		private void bsLevelNum_ValueChanged(object sender, EventArgs e)
		{
			bsLevelButton.Enabled = bsLevelNum.Value != 0;
		}

		private void bsLevelButton_Click(object sender, EventArgs e)
		{
			uint level = (uint)(bsLevelNum.Value - 1);
			GenerateCode(level, false);
			LevelChanged(level);
		}

		private void bsCode_TextChanged(object sender, EventArgs e)
		{
			if (bsCode.Text.Length < 12)
				bsCode.Text = bsCode.Text.PadRight(12, '0');
		}

		private void bsCodeButton_Click(object sender, EventArgs e)
		{
			ulong input = ulong.Parse(bsCode.Text);
			BWL d0 = (uint)(input >> 32);
			BWL d1 = (uint)(input & uint.MaxValue);
			d0.w ^= 0x55;
			BWL d2 = d0.w;
			d0.w &= 0x3F;
			d1.l ^= 0xAAAAAAAA;
			d1.RotateLeftL(6);
			BWL d3 = d1.hw;
			d3.w &= 0xF800;
			d3.w >>= 5;
			d3.w |= d0.w;
			d3.w &= 0x7FF;
			d1.l -= 0x1234567;
			d2.w &= 0x40;
			if (d2.w != 0)
				d1.l += 0x7654321;
			d1.l &= 0x7FFFFFF;
			BWL d4 = d1.hw;
			bool c = (d4.w & 1) == 1;
			d4.w >>= 1;
			if (c)
				d4.w ^= 0x8810;
			d4.w ^= d1.w;
			c = (d4.w & 1) == 1;
			d4.w >>= 1;
			if (c)
				d4.w ^= 0x8810;
			d4.w &= 0x7FF;
			if (d4.w != d3.w)
			{
				MessageBox.Show(this, "Invalid code.", "S3SSEdit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			if (d2.w != 0)
			{
				d1.l -= 0x7654321;
				d1.l &= 0x7FFFFFF;
			}
			bsLevelNum.Value = d1.l + 1;
			LevelChanged(d1.l);
		}

		readonly string[] headerconsoles = { "SEGA GENESIS    ", "SEGA MEGA DRIVE " };
		readonly string[] headermonths = {
			".JAN",
			".FEB",
			".MAR",
			".APR",
			".MAY",
			".JUN",
			".JUL",
			".AUG",
			".SEP",
			".OCT",
			".NOV",
			".DEC",
			".   ",
			"    ",
			"...."
		};
		private void bsROMButton_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "bin", Filter = "ROM Files|*.bin;*.gen;*.md|All Files|*.*" })
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					byte[] fc = File.ReadAllBytes(dlg.FileName);
					if (fc.Length < 0x1A6) goto fail;
					int bsheader = Array.IndexOf(headerconsoles, Encoding.ASCII.GetString(fc, 0x100, 16));
					if (bsheader == -1) goto fail;
					BWL serialnum = 0;
					for (int a1 = 0x180; a1 < 0x18C; a1++)
						if (fc[a1] >= '0' && fc[a1] <= '9')
							serialnum = serialnum.w * 10 + (fc[a1] - '0');
					serialnum.l &= 0x7FFF;
					if (serialnum.w == 0) goto fail;
					BWL levelnum = serialnum.w & 0x3F;
					int month = Array.IndexOf(headermonths, Encoding.ASCII.GetString(fc, 0x11C, 4));
					if (month == -1) month = 0xF;
					levelnum.w <<= 4;
					levelnum.b |= (byte)month;
					levelnum.w <<= 4;
					levelnum.b |= (byte)((serialnum.w >> 6) & 0xF);
					levelnum.l <<= 1;
					levelnum.b |= (byte)(fc[0x150] & 1);
					levelnum.l <<= 3;
					levelnum.b |= (byte)((((fc[0x11A] - '0') * 10) + (fc[0x11B] - '0') - 88) & 7);
					serialnum.RotateLeftW(6);
					serialnum.w &= 0x1F;
					levelnum.l <<= 5;
					levelnum.b |= serialnum.b;
					levelnum.l <<= 2;
					levelnum.b |= (byte)((ByteConverter.ToUInt16(fc, 0x1A4) >> 3) & 3);
					levelnum.l <<= 1;
					if (Encoding.ASCII.GetString(fc, 0x114, 2) == "EG")
						levelnum.b |= 1;
					levelnum.l <<= 1;
					levelnum.b |= (byte)bsheader;
					levelnum.l &= 0x7FFFFFF;
					bsLevelNum.Value = levelnum.l + 1;
					GenerateCode(levelnum.l, true);
					LevelChanged(levelnum.l);
				}
			return;
			fail:
			MessageBox.Show(this, "File is not a Sega Mega Drive/Genesis ROM.", "S3SSEdit", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private void GenerateCode(uint levelnum, bool lockon)
		{
			BWL a2 = levelnum;
			if (lockon)
				a2.l += 0x7654321;
			a2.l &= 0x7FFFFFF;
			BWL d4 = a2.hw;
			bool c = (d4.w & 1) == 1;
			d4.w >>= 1;
			if (c)
				d4.w ^= 0x8810;
			d4.w ^= a2.w;
			c = (d4.w & 1) == 1;
			d4.w >>= 1;
			if (c)
				d4.w ^= 0x8810;
			BWL d0 = d4.w;
			d0.w &= 0x3F;
			if (lockon)
				d0.w |= 0x40;
			d0.w ^= 0x55;
			BWL d1 = levelnum;
			d1.l += 0x1234567;
			d1.l &= 0x7FFFFFF;
			d4.w <<= 5;
			d4.w &= 0xF800;
			d1.hw |= d4.w;
			d1.RotateRightL(6);
			d1.l ^= 0xAAAAAAAA;
			bsCode.Text = (((ulong)d0.l << 32) | d1.l).ToString("000000000000");
		}

		private void LevelChanged(uint levelnum)
		{
			BSMode = BSMode.Level;
			bsStageChunk1.Value = BSChunks[0] = (byte)(levelnum & 0x7F);
			BWL d0 = levelnum * 3 + 1;
			BWL d2 = new BWL(d0.w, (ushort)(d0.hw % 0x7F));
			bsStageChunk2.Value = BSChunks[1] = (byte)(d0.l % 0x7F);
			d0 = levelnum * 5 + 2;
			d2 = new BWL(d0.w, (ushort)(d0.hw % 0x7E));
			bsStageChunk3.Value = BSChunks[2] = (byte)(d0.l % 0x7E);
			d0 = levelnum * 7 + 3;
			d2 = new BWL(d0.w, (ushort)(d0.hw % 0x7D));
			bsStageChunk4.Value = BSChunks[3] = (byte)(d0.l % 0x7D);
			StageChanged();
		}

		private void StageChanged()
		{
			bsStageNum.Value = (BSChunks[3] | (BSChunks[2] << 7) | (BSChunks[1] << 14) | (BSChunks[0] << 21)) & 0xFFFFFFF;
			DrawPreview();
		}

		private void DrawPreview()
		{
			LayoutData layout = null;
			switch (Category)
			{
				case LayoutMode.S3:
					layout = new SSLayoutData(File.ReadAllBytes(Path.Combine(path, project.S3Stages[StageNumber])));
					break;
				case LayoutMode.SK:
					layout = new SSLayoutData(Compression.Decompress(Path.Combine(path, project.SKStageSet), CompressionType.Kosinski), StageNumber * SSLayoutData.Size);
					break;
				case LayoutMode.BSChunk:
					layout = new BSChunkLayoutData(Compression.Decompress(Path.Combine(path, project.BlueSphereChunkSet), CompressionType.Kosinski), StageNumber);
					break;
				case LayoutMode.BSLayout:
					layout = new BSStageLayoutData(Compression.Decompress(Path.Combine(path, project.BlueSphereChunkSet), CompressionType.Kosinski), BSChunks);
					break;
			}
			pictureBox1.Image = LayoutDrawer.DrawLayout(layout, 28).ToBitmap(LayoutDrawer.Palette);
		}
	}

	[StructLayout(LayoutKind.Explicit, Size = 4)]
	struct BWL
	{
		[FieldOffset(0)]
		public byte b;
		[FieldOffset(0)]
		public sbyte sb;
		[FieldOffset(1)]
		public byte b1;
		[FieldOffset(1)]
		public sbyte sb1;
		[FieldOffset(2)]
		public byte b2;
		[FieldOffset(2)]
		public sbyte sb2;
		[FieldOffset(3)]
		public byte b3;
		[FieldOffset(3)]
		public sbyte sb3;
		[FieldOffset(0)]
		public ushort w;
		[FieldOffset(0)]
		public short sw;
		[FieldOffset(2)]
		public ushort hw;
		[FieldOffset(2)]
		public short hsw;
		[FieldOffset(0)]
		public uint l;
		[FieldOffset(0)]
		public int sl;

		public BWL(byte b)
			: this()
		{
			this.b = b;
		}

		public BWL(sbyte sb)
			: this()
		{
			this.sb = sb;
		}

		public BWL(byte b0, byte b1, byte b2, byte b3)
			: this()
		{
			b = b0;
			this.b1 = b1;
			this.b2 = b2;
			this.b3 = b3;
		}

		public BWL(sbyte sb0, sbyte sb1, sbyte sb2, sbyte sb3)
			: this()
		{
			sb = sb0;
			this.sb1 = sb1;
			this.sb2 = sb2;
			this.sb3 = sb3;
		}

		public BWL(ushort w)
			: this()
		{
			this.w = w;
		}

		public BWL(short sw)
			: this()
		{
			this.sw = sw;
		}

		public BWL(ushort w, ushort hw)
			: this()
		{
			this.w = w;
			this.hw = hw;
		}

		public BWL(short sw, short hsw)
			: this()
		{
			this.sw = sw;
			this.hsw = hsw;
		}

		public BWL(uint l)
			: this()
		{
			this.l = l;
		}

		public BWL(int sl)
			: this()
		{
			this.sl = sl;
		}

		public void Swap()
		{
			ushort tmp = w;
			w = hw;
			hw = tmp;
		}

		public void ExtendW()
		{
			sw = sb;
		}

		public void ExtendL()
		{
			sl = sw;
		}

		public void RotateLeftL(int amount)
		{
			l = (l << amount) | (l >> (32 - amount));
		}

		public void RotateRightL(int amount)
		{
			l = (l >> amount) | (l << (32 - amount));
		}

		public void RotateLeftW(int amount)
		{
			w = (ushort)((w << amount) | (w >> (16 - amount)));
		}

		public void RotateRightW(int amount)
		{
			w = (ushort)((w >> amount) | (w << (16 - amount)));
		}

		public void RotateLeftB(int amount)
		{
			b = (byte)((b << amount) | (b >> (16 - amount)));
		}

		public void RotateRightB(int amount)
		{
			b = (byte)((b >> amount) | (b << (16 - amount)));
		}

		public static implicit operator BWL(byte b)
		{
			return new BWL(b);
		}

		public static implicit operator BWL(sbyte sb)
		{
			return new BWL(sb);
		}

		public static implicit operator BWL(ushort w)
		{
			return new BWL(w);
		}

		public static implicit operator BWL(short sw)
		{
			return new BWL(sw);
		}

		public static implicit operator BWL(uint l)
		{
			return new BWL(l);
		}

		public static implicit operator BWL(int sl)
		{
			return new BWL(sl);
		}
	}
}
