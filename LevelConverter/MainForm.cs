using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SonicRetro.SonLVL.API;
using System.Linq;

namespace SonicRetro.SonLVL.LevelConverter
{
	public partial class MainForm : Form
	{
		public static MainForm Instance { get; private set; }

		public MainForm()
		{
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
			Instance = this;
			LevelData.LogEvent += new LevelData.LogEventHandler(Log);
			InitializeComponent();
		}

		void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			Log(e.Exception.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
			File.WriteAllLines("LevelConverter.log", LogFile.ToArray());
			using (ErrorDialog ed = new ErrorDialog("Unhandled Exception " + e.Exception.GetType().Name + "\nLog file has been saved.\n\nDo you want to try to continue running?", true))
			{
				if (ed.ShowDialog(this) == System.Windows.Forms.DialogResult.Cancel)
					Close();
			}
		}

		internal void Log(params string[] lines)
		{
			LogFile.AddRange(lines);
		}

		internal List<string> LogFile = new List<string>();
		string Dir = Environment.CurrentDirectory;
		bool ConvertKnownObjs = true;
		bool DontChangeObjects;
		List<string> Levels;

		private void fileSelector1_FileNameChanged(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(fileSelector1.FileName)) return;
			if (!File.Exists(fileSelector1.FileName)) return;
			try
			{
				LevelData.LoadGame(fileSelector1.FileName);
			}
			catch (ArgumentException)
			{
				return;
			}
			catch (IOException)
			{
				return;
			}
			catch (UnauthorizedAccessException)
			{
				return;
			}
			catch (NotSupportedException)
			{
				return;
			}
			catch (System.Security.SecurityException)
			{
				return;
			}
			comboBox1.Items.Clear();
			Levels = new List<string>();
			foreach (KeyValuePair<string, LevelInfo> item in LevelData.Game.Levels)
			{
				Levels.Add(item.Key);
				comboBox1.Items.Add(LevelData.Game.GetLevelInfo(item.Key).DisplayName);
			}
		}

		private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (comboBox1.SelectedIndex == -1 || comboBox2.SelectedIndex == -1)
				button1.Enabled = false;
			else
				button1.Enabled = true;
			button2.Enabled = File.Exists(fileSelector1.FileName) & comboBox2.SelectedIndex != -1;
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			ConvertKnownObjs = checkBox1.Checked;
		}

		private void checkBox2_CheckedChanged(object sender, EventArgs e)
		{
			DontChangeObjects = checkBox2.Checked;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			ConvertLevel();
			string level = (string)comboBox1.SelectedItem;
			string OutDir = Path.Combine(Dir, level);
			System.Diagnostics.Process.Start(OutDir);
		}

		private void ConvertLevel()
		{
			string level = (string)comboBox1.SelectedItem;
			string OutDir = Path.Combine(Dir, level);
			if (Directory.Exists(OutDir)) { Directory.Delete(OutDir, true); System.Threading.Thread.Sleep(100); }
			Directory.CreateDirectory(OutDir);
			EngineVersion OutFmt = EngineVersion.Invalid;
			switch (comboBox2.SelectedIndex)
			{
				case 0:
					OutFmt = EngineVersion.S1;
					break;
				case 1:
					OutFmt = EngineVersion.S2;
					break;
				case 2:
					OutFmt = EngineVersion.S3K;
					break;
				case 3:
					OutFmt = EngineVersion.SKC;
					break;
				case 4:
					OutFmt = EngineVersion.SCDPC;
					break;
				case 5:
					OutFmt = EngineVersion.S2NA;
					break;
			}
			LevelData.LoadLevel(Levels[comboBox1.SelectedIndex], false);
			if (LevelData.Level.LayoutFormat == EngineVersion.S2 | LevelData.Level.LayoutFormat == EngineVersion.SCDPC)
			{
				int xend = 0;
				int yend = 0;
				for (int y = 0; y < LevelData.FGHeight; y++)
					for (int x = 0; x < LevelData.FGWidth; x++)
						if (LevelData.Layout.FGLayout[x, y] > 0)
						{
							xend = Math.Max(xend, x);
							yend = Math.Max(yend, y);
						}
				xend++;
				yend++;
				byte[,] tmp = new byte[xend, yend];
				for (int y = 0; y < yend; y++)
					for (int x = 0; x < xend; x++)
						tmp[x, y] = LevelData.Layout.FGLayout[x, y];
				LevelData.Layout.FGLayout = tmp;
				xend = 0;
				yend = 0;
				for (int y = 0; y < LevelData.BGHeight; y++)
					for (int x = 0; x < LevelData.BGWidth; x++)
						if (LevelData.Layout.BGLayout[x, y] > 0)
						{
							xend = Math.Max(xend, x);
							yend = Math.Max(yend, y);
						}
				xend++;
				yend++;
				tmp = new byte[xend, yend];
				for (int y = 0; y < yend; y++)
					for (int x = 0; x < xend; x++)
						tmp[x, y] = LevelData.Layout.BGLayout[x, y];
				LevelData.Layout.BGLayout = tmp;
			}
			GameInfo Output = new GameInfo() { EngineVersion = OutFmt };
			LevelInfo Level = new LevelInfo();
			Output.Levels = new Dictionary<string, LevelInfo>() { { level, Level } };
			bool LE = LevelData.littleendian;
			LevelData.littleendian = false;
			switch (OutFmt)
			{
				case EngineVersion.SCDPC:
				case EngineVersion.SKC:
					LevelData.littleendian = true;
					break;
			}
			CompressionType cmp = CompressionType.Uncompressed;
			List<byte> tmp2 = new List<byte>();
			if (OutFmt != EngineVersion.SCDPC)
			{
				switch (OutFmt)
				{
					case EngineVersion.S1:
					case EngineVersion.S2NA:
						cmp = CompressionType.Nemesis;
						break;
					case EngineVersion.S2:
						cmp = CompressionType.Kosinski;
						break;
					case EngineVersion.S3K:
					case EngineVersion.SKC:
						cmp = CompressionType.KosinskiM;
						break;
					default:
						cmp = CompressionType.Uncompressed;
						break;
				}
				foreach (byte[] tile in LevelData.Tiles)
					tmp2.AddRange(tile);
				Compression.Compress(tmp2.ToArray(), Path.Combine(OutDir, "Tiles.bin"), cmp);
			}
			else
			{
				List<ushort>[] tilepals = new List<ushort>[4];
				for (int i = 0; i < 4; i++)
					tilepals[i] = new List<ushort>();
				foreach (Block blk in LevelData.Blocks)
					for (int y = 0; y < 2; y++)
						for (int x = 0; x < 2; x++)
							if (!tilepals[blk.Tiles[x, y].Palette].Contains(blk.Tiles[x, y].Tile))
								tilepals[blk.Tiles[x, y].Palette].Add(blk.Tiles[x, y].Tile);
				foreach (Block blk in LevelData.Blocks)
					for (int y = 0; y < 2; y++)
						for (int x = 0; x < 2; x++)
						{
							byte pal = blk.Tiles[x, y].Palette;
							int c = 0;
							for (int i = pal - 1; i >= 0; i--)
								c += tilepals[i].Count;
							blk.Tiles[x, y].Tile = (ushort)(tilepals[pal].IndexOf(blk.Tiles[x, y].Tile) + c);
						}
				List<byte[]> tiles = new List<byte[]>();
				for (int p = 0; p < 4; p++)
					foreach (ushort item in tilepals[p])
						if (LevelData.Tiles[item] != null)
							tiles.Add(LevelData.Tiles[item]);
						else
							tiles.Add(new byte[32]);
				LevelData.Tiles.Clear();
				LevelData.Tiles.AddFile(tiles, -1);
				tmp2 = new List<byte>();
				tmp2.Add(0x53);
				tmp2.Add(0x43);
				tmp2.Add(0x52);
				tmp2.Add(0x4C);
				tmp2.AddRange(ByteConverter.GetBytes(0x18 + (LevelData.Tiles.Count * 4) + (LevelData.Tiles.Count * 32)));
				tmp2.AddRange(ByteConverter.GetBytes(LevelData.Tiles.Count));
				tmp2.AddRange(ByteConverter.GetBytes(0x18 + (LevelData.Tiles.Count * 4)));
				for (int i = 0; i < 4; i++)
					tmp2.AddRange(ByteConverter.GetBytes((ushort)tilepals[i].Count));
				for (int i = 0; i < LevelData.Tiles.Count; i++)
				{
					tmp2.AddRange(ByteConverter.GetBytes((ushort)8));
					tmp2.AddRange(ByteConverter.GetBytes((ushort)8));
				}
				foreach (byte[] tile in LevelData.Tiles)
					tmp2.AddRange(tile);
				Compression.Compress(tmp2.ToArray(), Path.Combine(OutDir, "Tiles.bin"), CompressionType.SZDD);
			}
			Level.Tiles = new[] { new SonicRetro.SonLVL.API.FileInfo("Tiles.bin") };
			tmp2 = new List<byte>();
			if (OutFmt == EngineVersion.SKC)
				LevelData.littleendian = false;
			foreach (Block b in LevelData.Blocks)
			{
				tmp2.AddRange(b.GetBytes());
			}
			if (OutFmt == EngineVersion.SKC)
				LevelData.littleendian = true;
			switch (OutFmt)
			{
				case EngineVersion.S1:
					cmp = CompressionType.Enigma;
					break;
				case EngineVersion.S2:
				case EngineVersion.S3K:
				case EngineVersion.SKC:
					cmp = CompressionType.Kosinski;
					break;
				default:
					cmp = CompressionType.Uncompressed;
					break;
			}
			Compression.Compress(tmp2.ToArray(), Path.Combine(OutDir, "Blocks.bin"), cmp);
			Level.Blocks = new[] { new SonicRetro.SonLVL.API.FileInfo("Blocks.bin") };
			byte chunktypes = 0;
			int chunksz = 16;
			switch (LevelData.Level.ChunkFormat)
			{
				case EngineVersion.S1:
				case EngineVersion.SCDPC:
					chunktypes = 0;
					break;
				case EngineVersion.S2:
				case EngineVersion.S2NA:
				case EngineVersion.S3K:
				case EngineVersion.SKC:
					chunktypes = 1;
					break;
			}
			switch (OutFmt)
			{
				case EngineVersion.S2:
				case EngineVersion.S2NA:
				case EngineVersion.S3K:
				case EngineVersion.SKC:
					chunktypes |= 2;
					chunksz = 8;
					break;
			}
			LevelData.Level.ChunkWidth = LevelData.Level.ChunkHeight = chunksz * 16;
			LevelData.Level.ChunkFormat = OutFmt;
			List<Chunk> tmpchnk = new List<Chunk>();
			switch (chunktypes)
			{
				case 0: // S1 -> S1
					tmpchnk = LevelData.Chunks.ToList();
					tmpchnk.RemoveAt(0);
					break;
				case 1: // S2 -> S1
					tmpchnk = new List<Chunk>() { new Chunk() };
					List<int> chnks = new List<int>() { 0 };
					int chnk;
					byte[,] newFG1 = new byte[(int)Math.Ceiling(LevelData.FGWidth / 2d), (int)Math.Ceiling(LevelData.FGHeight / 2d)];
					LevelData.Layout.FGLoop = new bool[newFG1.GetLength(0), newFG1.GetLength(1)];
					for (int y = 0; y < LevelData.FGHeight; y += 2)
					{
						for (int x = 0; x < LevelData.FGWidth; x += 2)
						{
							chnk = LevelData.Layout.FGLayout[x, y];
							chnk |= (x + 1 < LevelData.FGWidth ? LevelData.Layout.FGLayout[x + 1, y] : 0) << 8;
							chnk |= (y + 1 < LevelData.FGHeight ? LevelData.Layout.FGLayout[x, y + 1] : 0) << 16;
							chnk |= (x + 1 < LevelData.FGWidth & y + 1 < LevelData.FGHeight ? LevelData.Layout.FGLayout[x + 1, y + 1] : 0) << 24;
							if (chnks.IndexOf(chnk) > -1)
								newFG1[x / 2, y / 2] = (byte)chnks.IndexOf(chnk);
							else
							{
								newFG1[x / 2, y / 2] = (byte)chnks.Count;
								Chunk newchnk = new Chunk();
								for (int i = 0; i < 8; i++)
								{
									for (int j = 0; j < 8; j++)
									{
										newchnk.Blocks[i, j].Block = LevelData.Chunks[chnk & 0xFF].Blocks[i, j].Block;
										newchnk.Blocks[i, j].Solid1 = LevelData.Chunks[chnk & 0xFF].Blocks[i, j].Solid1;
										newchnk.Blocks[i, j].XFlip = LevelData.Chunks[chnk & 0xFF].Blocks[i, j].XFlip;
										newchnk.Blocks[i, j].YFlip = LevelData.Chunks[chnk & 0xFF].Blocks[i, j].YFlip;
									}
								}
								for (int i = 0; i < 8; i++)
								{
									for (int j = 0; j < 8; j++)
									{
										newchnk.Blocks[i + 8, j].Block = LevelData.Chunks[(chnk >> 8) & 0xFF].Blocks[i, j].Block;
										newchnk.Blocks[i + 8, j].Solid1 = LevelData.Chunks[(chnk >> 8) & 0xFF].Blocks[i, j].Solid1;
										newchnk.Blocks[i + 8, j].XFlip = LevelData.Chunks[(chnk >> 8) & 0xFF].Blocks[i, j].XFlip;
										newchnk.Blocks[i + 8, j].YFlip = LevelData.Chunks[(chnk >> 8) & 0xFF].Blocks[i, j].YFlip;
									}
								}
								for (int i = 0; i < 8; i++)
								{
									for (int j = 0; j < 8; j++)
									{
										newchnk.Blocks[i, j + 8].Block = LevelData.Chunks[(chnk >> 16) & 0xFF].Blocks[i, j].Block;
										newchnk.Blocks[i, j + 8].Solid1 = LevelData.Chunks[(chnk >> 16) & 0xFF].Blocks[i, j].Solid1;
										newchnk.Blocks[i, j + 8].XFlip = LevelData.Chunks[(chnk >> 16) & 0xFF].Blocks[i, j].XFlip;
										newchnk.Blocks[i, j + 8].YFlip = LevelData.Chunks[(chnk >> 16) & 0xFF].Blocks[i, j].YFlip;
									}
								}
								for (int i = 0; i < 8; i++)
								{
									for (int j = 0; j < 8; j++)
									{
										newchnk.Blocks[i + 8, j + 8].Block = LevelData.Chunks[(chnk >> 24) & 0xFF].Blocks[i, j].Block;
										newchnk.Blocks[i + 8, j + 8].Solid1 = LevelData.Chunks[(chnk >> 24) & 0xFF].Blocks[i, j].Solid1;
										newchnk.Blocks[i + 8, j + 8].XFlip = LevelData.Chunks[(chnk >> 24) & 0xFF].Blocks[i, j].XFlip;
										newchnk.Blocks[i + 8, j + 8].YFlip = LevelData.Chunks[(chnk >> 24) & 0xFF].Blocks[i, j].YFlip;
									}
								}
								tmpchnk.Add(newchnk);
								chnks.Add(chnk);
							}
						}
					}
					LevelData.Layout.FGLayout = newFG1;
					byte[,] newBG1 = new byte[(int)Math.Ceiling(LevelData.BGWidth / 2d), (int)Math.Ceiling(LevelData.BGHeight / 2d)];
					LevelData.Layout.BGLoop = new bool[newBG1.GetLength(0), newBG1.GetLength(1)];
					for (int y = 0; y < LevelData.BGHeight; y += 2)
					{
						for (int x = 0; x < LevelData.BGWidth; x += 2)
						{
							chnk = LevelData.Layout.BGLayout[x, y];
							chnk |= (x + 1 < LevelData.BGWidth ? LevelData.Layout.BGLayout[x + 1, y] : 0) << 8;
							chnk |= (y + 1 < LevelData.BGHeight ? LevelData.Layout.BGLayout[x, y + 1] : 0) << 16;
							chnk |= (x + 1 < LevelData.BGWidth & y + 1 < LevelData.BGHeight ? LevelData.Layout.BGLayout[x + 1, y + 1] : 0) << 24;
							if (chnks.IndexOf(chnk) > -1)
								newBG1[x / 2, y / 2] = (byte)chnks.IndexOf(chnk);
							else
							{
								newBG1[x / 2, y / 2] = (byte)chnks.Count;
								Chunk newchnk = new Chunk();
								for (int i = 0; i < 8; i++)
								{
									for (int j = 0; j < 8; j++)
									{
										newchnk.Blocks[i, j].Block = LevelData.Chunks[chnk & 0xFF].Blocks[i, j].Block;
										newchnk.Blocks[i, j].Solid1 = LevelData.Chunks[chnk & 0xFF].Blocks[i, j].Solid1;
										newchnk.Blocks[i, j].XFlip = LevelData.Chunks[chnk & 0xFF].Blocks[i, j].XFlip;
										newchnk.Blocks[i, j].YFlip = LevelData.Chunks[chnk & 0xFF].Blocks[i, j].YFlip;
									}
								}
								for (int i = 0; i < 8; i++)
								{
									for (int j = 0; j < 8; j++)
									{
										newchnk.Blocks[i + 8, j].Block = LevelData.Chunks[(chnk >> 8) & 0xFF].Blocks[i, j].Block;
										newchnk.Blocks[i + 8, j].Solid1 = LevelData.Chunks[(chnk >> 8) & 0xFF].Blocks[i, j].Solid1;
										newchnk.Blocks[i + 8, j].XFlip = LevelData.Chunks[(chnk >> 8) & 0xFF].Blocks[i, j].XFlip;
										newchnk.Blocks[i + 8, j].YFlip = LevelData.Chunks[(chnk >> 8) & 0xFF].Blocks[i, j].YFlip;
									}
								}
								for (int i = 0; i < 8; i++)
								{
									for (int j = 0; j < 8; j++)
									{
										newchnk.Blocks[i, j + 8].Block = LevelData.Chunks[(chnk >> 16) & 0xFF].Blocks[i, j].Block;
										newchnk.Blocks[i, j + 8].Solid1 = LevelData.Chunks[(chnk >> 16) & 0xFF].Blocks[i, j].Solid1;
										newchnk.Blocks[i, j + 8].XFlip = LevelData.Chunks[(chnk >> 16) & 0xFF].Blocks[i, j].XFlip;
										newchnk.Blocks[i, j + 8].YFlip = LevelData.Chunks[(chnk >> 16) & 0xFF].Blocks[i, j].YFlip;
									}
								}
								for (int i = 0; i < 8; i++)
								{
									for (int j = 0; j < 8; j++)
									{
										newchnk.Blocks[i + 8, j + 8].Block = LevelData.Chunks[(chnk >> 24) & 0xFF].Blocks[i, j].Block;
										newchnk.Blocks[i + 8, j + 8].Solid1 = LevelData.Chunks[(chnk >> 24) & 0xFF].Blocks[i, j].Solid1;
										newchnk.Blocks[i + 8, j + 8].XFlip = LevelData.Chunks[(chnk >> 24) & 0xFF].Blocks[i, j].XFlip;
										newchnk.Blocks[i + 8, j + 8].YFlip = LevelData.Chunks[(chnk >> 24) & 0xFF].Blocks[i, j].YFlip;
									}
								}
								tmpchnk.Add(newchnk);
								chnks.Add(chnk);
							}
						}
					}
					LevelData.Layout.BGLayout = newBG1;
					tmpchnk.RemoveAt(0);
					break;
				case 2: // S1 -> S2
					tmpchnk = new List<Chunk>() { new Chunk() };
					Dictionary<byte, byte[]> cnkinds = new Dictionary<byte, byte[]>() { { 0, new byte[4] } };
					List<byte> usedcnks =
						LevelData.Layout.FGLayout.Cast<byte>().Concat(LevelData.Layout.BGLayout.Cast<byte>()).Distinct().ToList();
					if (usedcnks.Contains(0))
						usedcnks.Remove(0);
					foreach (byte usedcnk in usedcnks)
					{
						Chunk item = LevelData.Chunks[usedcnk];
						Chunk[] newchnk = new Chunk[4];
						for (int i = 0; i < 4; i++)
							newchnk[i] = new Chunk();
						for (int y = 0; y < chunksz; y++)
							for (int x = 0; x < chunksz; x++)
							{
								S2ChunkBlock blk = (S2ChunkBlock)newchnk[0].Blocks[x, y];
								blk.Block = item.Blocks[x, y].Block;
								blk.Solid1 = item.Blocks[x, y].Solid1;
								blk.Solid2 = blk.Solid1;
								blk.XFlip = item.Blocks[x, y].XFlip;
								blk.YFlip = item.Blocks[x, y].YFlip;
								blk = (S2ChunkBlock)newchnk[1].Blocks[x, y];
								blk.Block = item.Blocks[x + chunksz, y].Block;
								blk.Solid1 = item.Blocks[x + chunksz, y].Solid1;
								blk.Solid2 = blk.Solid1;
								blk.XFlip = item.Blocks[x + chunksz, y].XFlip;
								blk.YFlip = item.Blocks[x + chunksz, y].YFlip;
								blk = (S2ChunkBlock)newchnk[2].Blocks[x, y];
								blk.Block = item.Blocks[x, y + chunksz].Block;
								blk.Solid1 = item.Blocks[x, y + chunksz].Solid1;
								blk.Solid2 = blk.Solid1;
								blk.XFlip = item.Blocks[x, y + chunksz].XFlip;
								blk.YFlip = item.Blocks[x, y + chunksz].YFlip;
								blk = (S2ChunkBlock)newchnk[3].Blocks[x, y];
								blk.Block = item.Blocks[x + chunksz, y + chunksz].Block;
								blk.Solid1 = item.Blocks[x + chunksz, y + chunksz].Solid1;
								blk.Solid2 = blk.Solid1;
								blk.XFlip = item.Blocks[x + chunksz, y + chunksz].XFlip;
								blk.YFlip = item.Blocks[x + chunksz, y + chunksz].YFlip;
							}
						byte[] ids = new byte[4];
						for (int i = 0; i < 4; i++)
						{
							byte[] b = newchnk[i].GetBytes();
							int match = -1;
							for (int c = 0; c < tmpchnk.Count; c++)
								if (b.ArrayEqual(tmpchnk[c].GetBytes()))
								{
									match = c;
									break;
								}
							if (match != -1)
								ids[i] = (byte)match;
							else
							{
								ids[i] = (byte)tmpchnk.Count;
								tmpchnk.Add(newchnk[i]);
							}
						}
						cnkinds.Add(usedcnk, ids);
					}
					byte[,] newFG = new byte[LevelData.FGWidth * 2, LevelData.FGHeight * 2];
					for (int y = 0; y < LevelData.FGHeight; y++)
						for (int x = 0; x < LevelData.FGWidth; x++)
							if (LevelData.Layout.FGLayout[x, y] != 0)
							{
								byte[] ids = cnkinds[LevelData.Layout.FGLayout[x, y]];
								newFG[x * 2, y * 2] = ids[0];
								newFG[(x * 2) + 1, y * 2] = ids[1];
								newFG[x * 2, (y * 2) + 1] = ids[2];
								newFG[(x * 2) + 1, (y * 2) + 1] = ids[3];
							}
					LevelData.Layout.FGLayout = newFG;
					byte[,] newBG = new byte[LevelData.BGWidth * 2, LevelData.BGHeight * 2];
					for (int y = 0; y < LevelData.BGHeight; y++)
						for (int x = 0; x < LevelData.BGWidth; x++)
							if (LevelData.Layout.BGLayout[x, y] != 0)
							{
								byte[] ids = cnkinds[LevelData.Layout.BGLayout[x, y]];
								newBG[x * 2, y * 2] = ids[0];
								newBG[(x * 2) + 1, y * 2] = ids[1];
								newBG[x * 2, (y * 2) + 1] = ids[2];
								newBG[(x * 2) + 1, (y * 2) + 1] = ids[3];
							}
					LevelData.Layout.BGLayout = newBG;
					break;
				case 3: // S2 -> S2
					tmpchnk = LevelData.Chunks.ToList();
					break;
			}
			tmp2 = new List<byte>();
			if (OutFmt == EngineVersion.SKC)
				LevelData.littleendian = false;
			foreach (Chunk c in tmpchnk)
			{
				tmp2.AddRange(c.GetBytes());
			}
			if (OutFmt == EngineVersion.SKC)
				LevelData.littleendian = true;
			switch (OutFmt)
			{
				case EngineVersion.S1:
				case EngineVersion.S2:
				case EngineVersion.S3K:
				case EngineVersion.SKC:
					cmp = CompressionType.Kosinski;
					break;
				default:
					cmp = CompressionType.Uncompressed;
					break;
			}
			Compression.Compress(tmp2.ToArray(), Path.Combine(OutDir, "Chunks.bin"), cmp);
			Level.Chunks = new[] { new SonicRetro.SonLVL.API.FileInfo("Chunks.bin") };
			ushort fgw = (ushort)LevelData.FGWidth;
			ushort bgw = (ushort)LevelData.BGWidth;
			ushort fgh = (ushort)LevelData.FGHeight;
			ushort bgh = (ushort)LevelData.BGHeight;
			switch (OutFmt)
			{
				case EngineVersion.S1:
					tmp2 = new List<byte>();
					tmp2.Add((byte)(LevelData.FGWidth - 1));
					tmp2.Add((byte)(LevelData.FGHeight - 1));
					for (int lr = 0; lr < LevelData.FGHeight; lr++)
						for (int lc = 0; lc < LevelData.FGWidth; lc++)
							tmp2.Add((byte)(LevelData.Layout.FGLayout[lc, lr] | (LevelData.Layout.FGLoop[lc, lr] ? 0x80 : 0)));
					Compression.Compress(tmp2.ToArray(), Path.Combine(OutDir, "FGLayout.bin"), CompressionType.Uncompressed);
					Level.FGLayout = "FGLayout.bin";
					tmp2 = new List<byte>();
					tmp2.Add((byte)(LevelData.BGWidth - 1));
					tmp2.Add((byte)(LevelData.BGHeight - 1));
					for (int lr = 0; lr < LevelData.BGHeight; lr++)
						for (int lc = 0; lc < LevelData.BGWidth; lc++)
							tmp2.Add((byte)(LevelData.Layout.BGLayout[lc, lr] | (LevelData.Layout.BGLoop[lc, lr] ? 0x80 : 0)));
					Compression.Compress(tmp2.ToArray(), Path.Combine(OutDir, "BGLayout.bin"), CompressionType.Uncompressed);
					Level.BGLayout = "BGLayout.bin";
					break;
				case EngineVersion.S2NA:
					tmp2 = new List<byte>();
					tmp2.Add((byte)(LevelData.FGWidth - 1));
					tmp2.Add((byte)(LevelData.FGHeight - 1));
					for (int lr = 0; lr < LevelData.FGHeight; lr++)
						for (int lc = 0; lc < LevelData.FGWidth; lc++)
							tmp2.Add(LevelData.Layout.FGLayout[lc, lr]);
					Compression.Compress(tmp2.ToArray(), Path.Combine(OutDir, "FGLayout.bin"), CompressionType.Uncompressed);
					Level.FGLayout = "FGLayout.bin";
					tmp2 = new List<byte>();
					tmp2.Add((byte)(LevelData.BGWidth - 1));
					tmp2.Add((byte)(LevelData.BGHeight - 1));
					for (int lr = 0; lr < LevelData.BGHeight; lr++)
						for (int lc = 0; lc < LevelData.BGWidth; lc++)
							tmp2.Add(LevelData.Layout.BGLayout[lc, lr]);
					Compression.Compress(tmp2.ToArray(), Path.Combine(OutDir, "BGLayout.bin"), CompressionType.Uncompressed);
					Level.BGLayout = "BGLayout.bin";
					break;
				case EngineVersion.S2:
					tmp2 = new List<byte>();
					for (int la = 0; la < 16; la++)
					{
						if (LevelData.FGHeight > la)
							for (int laf = 0; laf < 128; laf++)
								if (LevelData.FGWidth > laf)
									tmp2.Add(LevelData.Layout.FGLayout[laf, la]);
								else
									tmp2.Add(0);
						else
							tmp2.AddRange(new byte[128]);
						if (LevelData.BGHeight > la)
							for (int lab = 0; lab < 128; lab++)
								if (LevelData.BGWidth > lab)
									tmp2.Add(LevelData.Layout.BGLayout[lab, la]);
								else
									tmp2.Add(0);
						else
							tmp2.AddRange(new byte[128]);
					}
					Compression.Compress(tmp2.ToArray(), Path.Combine(OutDir, "Layout.bin"), CompressionType.Kosinski);
					Level.Layout = "Layout.bin";
					break;
				case EngineVersion.S3K:
					tmp2 = new List<byte>();
					tmp2.AddRange(ByteConverter.GetBytes(fgw));
					tmp2.AddRange(ByteConverter.GetBytes(bgw));
					tmp2.AddRange(ByteConverter.GetBytes(fgh));
					tmp2.AddRange(ByteConverter.GetBytes(bgh));
					for (int la = 0; la < 32; la++)
					{
						if (la < fgh)
							tmp2.AddRange(ByteConverter.GetBytes((ushort)(0x8088 + (la * fgw))));
						else
							tmp2.AddRange(new byte[2]);
						if (la < bgh)
							tmp2.AddRange(ByteConverter.GetBytes((ushort)(0x8088 + (fgh * fgw) + (la * bgw))));
						else
							tmp2.AddRange(new byte[2]);
					}
					for (int y = 0; y < fgh; y++)
						for (int x = 0; x < fgw; x++)
							tmp2.Add(LevelData.Layout.FGLayout[x, y]);
					for (int y = 0; y < bgh; y++)
						for (int x = 0; x < bgw; x++)
							tmp2.Add(LevelData.Layout.BGLayout[x, y]);
					Compression.Compress(tmp2.ToArray(), Path.Combine(OutDir, "Layout.bin"), CompressionType.Uncompressed);
					Level.Layout = "Layout.bin";
					break;
				case EngineVersion.SKC:
					tmp2 = new List<byte>();
					tmp2.AddRange(ByteConverter.GetBytes(fgw));
					tmp2.AddRange(ByteConverter.GetBytes(bgw));
					tmp2.AddRange(ByteConverter.GetBytes(fgh));
					tmp2.AddRange(ByteConverter.GetBytes(bgh));
					for (int la = 0; la < 32; la++)
					{
						if (la < fgh)
							tmp2.AddRange(ByteConverter.GetBytes((ushort)(0x8088 + (la * fgw))));
						else
							tmp2.AddRange(new byte[2]);
						if (la < bgh)
							tmp2.AddRange(ByteConverter.GetBytes((ushort)(0x8088 + (fgh * fgw) + (la * bgw))));
						else
							tmp2.AddRange(new byte[2]);
					}
					List<byte> l = new List<byte>();
					for (int y = 0; y < fgh; y++)
						for (int x = 0; x < fgw; x++)
							l.Add(LevelData.Layout.FGLayout[x, y]);
					for (int y = 0; y < bgh; y++)
						for (int x = 0; x < bgw; x++)
							l.Add(LevelData.Layout.BGLayout[x, y]);
					for (int i = 0; i < l.Count; i++)
						tmp2.Add(l[i ^ 1]);
					Compression.Compress(tmp2.ToArray(), Path.Combine(OutDir, "Layout.bin"), CompressionType.Uncompressed);
					Level.Layout = "Layout.bin";
					break;
				case EngineVersion.SCDPC:
					tmp2 = new List<byte>();
					for (int lr = 0; lr < 8; lr++)
						for (int lc = 0; lc < 64; lc++)
							if (lc < fgw & lr < fgh)
								tmp2.Add(LevelData.Layout.FGLayout[lc, lr]);
							else
								tmp2.Add(0);
					Compression.Compress(tmp2.ToArray(), Path.Combine(OutDir, "FGLayout.bin"), CompressionType.Uncompressed);
					Level.FGLayout = "FGLayout.bin";
					tmp2 = new List<byte>();
					for (int lr = 0; lr < 8; lr++)
						for (int lc = 0; lc < 64; lc++)
							if (lc < bgw & lr < bgh)
								tmp2.Add(LevelData.Layout.BGLayout[lc, lr]);
							else
								tmp2.Add(0);
					Compression.Compress(tmp2.ToArray(), Path.Combine(OutDir, "BGLayout.bin"), CompressionType.Uncompressed);
					Level.BGLayout = "BGLayout.bin";
					break;
			}
			tmp2 = new List<byte>();
			if (OutFmt != EngineVersion.SCDPC)
			{
				for (int pl = 0; pl < 4; pl++)
					for (int pi = 0; pi < 16; pi++)
						tmp2.AddRange(ByteConverter.GetBytes(LevelData.Palette[0][pl, pi].MDColor));
			}
			else
			{
				for (int pl = 0; pl < 4; pl++)
					for (int pi = 0; pi < 16; pi++)
					{
						tmp2.Add(LevelData.Palette[0][pl, pi].R);
						tmp2.Add(LevelData.Palette[0][pl, pi].G);
						tmp2.Add(LevelData.Palette[0][pl, pi].B);
						tmp2.Add((byte)1);
					}
			}
			Compression.Compress(tmp2.ToArray(), Path.Combine(OutDir, "Palette.bin"), CompressionType.Uncompressed);
			Level.Palette = new PaletteList("Palette.bin:0:0:64");
			switch (LevelData.Level.ObjectFormat)
			{
				case EngineVersion.S1:
					switch (OutFmt)
					{
						case EngineVersion.S2:
						case EngineVersion.S2NA:
							ObjS1ToS2();
							break;
						case EngineVersion.S3K:
						case EngineVersion.SKC:
							ObjS1ToS3K();
							break;
						case EngineVersion.SCDPC:
							ObjS1ToSCD();
							break;
					}
					break;
				case EngineVersion.S2:
				case EngineVersion.S2NA:
					switch (OutFmt)
					{
						case EngineVersion.S1:
							ObjS2ToS1();
							break;
						case EngineVersion.S3K:
						case EngineVersion.SKC:
							ObjS2ToS3K();
							break;
						case EngineVersion.SCDPC:
							ObjS2ToSCD();
							break;
					}
					break;
				case EngineVersion.S3K:
				case EngineVersion.SKC:
					switch (OutFmt)
					{
						case EngineVersion.S1:
							ObjS3KToS1();
							break;
						case EngineVersion.S2:
						case EngineVersion.S2NA:
							ObjS3KToS2();
							break;
						case EngineVersion.SCDPC:
							ObjS3KToSCD();
							break;
					}
					break;
				case EngineVersion.SCDPC:
					switch (OutFmt)
					{
						case EngineVersion.S1:
							ObjSCDToS1();
							break;
						case EngineVersion.S2:
						case EngineVersion.S2NA:
							ObjSCDToS2();
							break;
						case EngineVersion.S3K:
						case EngineVersion.SKC:
							ObjSCDToS3K();
							break;
					}
					break;
			}
			tmp2 = new List<byte>();
			switch (OutFmt)
			{
				case EngineVersion.S1:
					for (int oi = 0; oi < LevelData.Objects.Count; oi++)
					{
						tmp2.AddRange(((S1ObjectEntry)LevelData.Objects[oi]).GetBytes());
					}
					tmp2.AddRange(new byte[] { 0xFF, 0xFF });
					while (tmp2.Count % S1ObjectEntry.Size > 0)
					{
						tmp2.Add(0);
					}
					break;
				case EngineVersion.S2:
				case EngineVersion.S2NA:
					for (int oi = 0; oi < LevelData.Objects.Count; oi++)
					{
						tmp2.AddRange(((S2ObjectEntry)LevelData.Objects[oi]).GetBytes());
					}
					tmp2.AddRange(new byte[] { 0xFF, 0xFF });
					while (tmp2.Count % S2ObjectEntry.Size > 0)
					{
						tmp2.Add(0);
					}
					break;
				case EngineVersion.S3K:
				case EngineVersion.SKC:
					for (int oi = 0; oi < LevelData.Objects.Count; oi++)
					{
						tmp2.AddRange(((S3KObjectEntry)LevelData.Objects[oi]).GetBytes());
					}
					tmp2.AddRange(new byte[] { 0xFF, 0xFF });
					while (tmp2.Count % S3KObjectEntry.Size > 0)
					{
						tmp2.Add(0);
					}
					break;
				case EngineVersion.SCDPC:
					for (int oi = 0; oi < LevelData.Objects.Count; oi++)
					{
						tmp2.AddRange(((SCDObjectEntry)LevelData.Objects[oi]).GetBytes());
					}
					tmp2.Add(0xFF);
					while (tmp2.Count % SCDObjectEntry.Size > 0)
					{
						tmp2.Add(0xFF);
					}
					break;
			}
			Compression.Compress(tmp2.ToArray(), Path.Combine(OutDir, "Objects.bin"), CompressionType.Uncompressed);
			Level.Objects = "Objects.bin";
			if (LevelData.Rings != null)
			{
				switch (OutFmt)
				{
					case EngineVersion.S2:
					case EngineVersion.S2NA:
						new API.S2.Ring().WriteLayout(LevelData.Rings, Path.Combine(OutDir, "Rings.bin"));
						Level.Rings = "Rings.bin";
						break;
					case EngineVersion.S3K:
					case EngineVersion.SKC:
						new API.S3K.Ring().WriteLayout(LevelData.Rings, Path.Combine(OutDir, "Rings.bin"));
						Level.Rings = "Rings.bin";
						break;
				}
			}
			if (LevelData.ColInds1 != null)
				switch (OutFmt)
				{
					case EngineVersion.S1:
					case EngineVersion.SCD:
					case EngineVersion.SCDPC:
						Compression.Compress(LevelData.ColInds1.ToArray(), Path.Combine(OutDir, "Indexes.bin"), CompressionType.Uncompressed);
						Level.CollisionIndex = "Indexes.bin";
						break;
					case EngineVersion.S2:
					case EngineVersion.S2NA:
						Compression.Compress(LevelData.ColInds1.ToArray(), Path.Combine(OutDir, "Indexes1.bin"), CompressionType.Kosinski);
						Level.CollisionIndex1 = "Indexes1.bin";
						Compression.Compress(LevelData.ColInds2.ToArray(), Path.Combine(OutDir, "Indexes2.bin"), CompressionType.Kosinski);
						Level.CollisionIndex2 = "Indexes2.bin";
						break;
					case EngineVersion.S3K:
					case EngineVersion.SKC:
						tmp2 = new List<byte>();
						for (int i = 0; i < LevelData.ColInds1.Count; i++)
						{
							tmp2.Add(LevelData.ColInds1[i]);
							tmp2.Add(LevelData.ColInds2[i]);
						}
						Compression.Compress(tmp2.ToArray(), Path.Combine(OutDir, "IndexesSK.bin"), CompressionType.Uncompressed);
						tmp2 = new List<byte>();
						foreach (byte item in LevelData.ColInds1)
							tmp2.AddRange(ByteConverter.GetBytes((ushort)item));
						foreach (byte item in LevelData.ColInds2)
							tmp2.AddRange(ByteConverter.GetBytes((ushort)item));
						Compression.Compress(tmp2.ToArray(), Path.Combine(OutDir, "IndexesS3.bin"), CompressionType.Uncompressed);
						Level.CollisionIndex = "IndexesSK.bin";
						Level.CollisionIndexSize = 1;
						break;
				}
			if (LevelData.ColArr1 != null)
			{
				tmp2 = new List<byte>();
				for (int i = 0; i < 256; i++)
					for (int j = 0; j < 16; j++)
						tmp2.Add(unchecked((byte)LevelData.ColArr1[i][j]));
				Compression.Compress(tmp2.ToArray(), Path.Combine(OutDir, "Collision.bin"), CompressionType.Uncompressed);
				Level.CollisionArray1 = "Collision.bin";
				sbyte[][] rotcol = LevelData.GenerateRotatedCollision();
				tmp2 = new List<byte>();
				for (int i = 0; i < 256; i++)
					for (int j = 0; j < 16; j++)
						tmp2.Add(unchecked((byte)rotcol[i][j]));
				Compression.Compress(tmp2.ToArray(), Path.Combine(OutDir, "CollisionR.bin"), CompressionType.Uncompressed);
				Level.CollisionArray2 = "CollisionR.bin";
			}
			if (LevelData.Angles != null)
			{
				Compression.Compress(LevelData.Angles, Path.Combine(OutDir, "Angles.bin"), CompressionType.Uncompressed);
				Level.Angles = "Angles.bin";
			}
			Output.Save(Path.Combine(OutDir, OutFmt.ToString() + "LVL.ini"));
			LevelData.littleendian = LE;
		}

		private Size[] RingSpacing = {
                                     new Size(0x10, 0), // horizontal tight
                                     new Size(0x18, 0), // horizontal normal
                                     new Size(0x20, 0), // horizontal wide
                                     new Size(0, 0x10), // vertical tight
                                     new Size(0, 0x18), // vertical normal
                                     new Size(0, 0x20), // vertical wide
                                     new Size(0x10, 0x10), // diagonal
                                     new Size(0x18, 0x18),
                                     new Size(0x20, 0x20),
                                     new Size(-0x10, 0x10),
                                     new Size(-0x18, 0x18),
                                     new Size(-0x20, 0x20),
                                     new Size(0x10, 8),
                                     new Size(0x18, 0x10),
                                     new Size(-0x10, 8),
                                     new Size(-0x18, 0x10)
                                 };

		private void ObjS1ToS2()
		{
			List<ObjectEntry> Objs = new List<ObjectEntry>();
			LevelData.Rings = new List<RingEntry>();
			for (int i = 0; i < LevelData.Objects.Count; i++)
			{
				S1ObjectEntry item1 = LevelData.Objects[i] as S1ObjectEntry;
				if (!DontChangeObjects)
				{
					if (item1.ID == 0x25)
					{
						byte rcnt = (byte)(Math.Min(6, item1.SubType & 7) + 1);
						int rtyp = item1.SubType >> 4;
						switch (rtyp)
						{
							case 1:
								LevelData.Rings.Add(new API.S2.S2RingEntry() { Direction = Direction.Horizontal, Count = rcnt, X = item1.X, Y = item1.Y });
								break;
							case 4:
								LevelData.Rings.Add(new API.S2.S2RingEntry() { Direction = Direction.Vertical, Count = rcnt, X = item1.X, Y = item1.Y });
								break;
							default:
								Point rpos = new Point(item1.X, item1.Y);
								for (int r = 0; r < rcnt; r++)
								{
									LevelData.Rings.Add(new API.S2.S2RingEntry() { X = (ushort)rpos.X, Y = (ushort)rpos.Y });
									rpos += RingSpacing[rtyp];
								}
								break;
						}
					}
					else
					{
						S2ObjectEntry item = new S2ObjectEntry();
						item.ID = item1.ID;
						item.RememberState = item1.RememberState;
						item.SubType = item1.SubType;
						item.X = item1.X;
						item.XFlip = item1.XFlip;
						item.Y = item1.Y;
						item.YFlip = item1.YFlip;
						bool known = true;
						switch (item.ID)
						{
							case 0x0D:
							case 0x11:
								break;
							case 0x18:
								switch (item.SubType & 0xF)
								{
									case 0: // Stationary
										item.SubType = 0;
										break;
									case 1: // Right/Left
										item.SubType = 1;
										break;
									case 2: // Down/Up
										item.SubType = 2;
										break;
									case 3: // Falling
									case 4:
										item.SubType = 3;
										break;
									case 5: // Left/Right
										item.SubType = 1;
										break;
									case 6: // Up/Down
									case 7:
									case 8:
										item.SubType = 2;
										break;
									case 9:
										item.SubType = 0;
										break;
									case 0xA:
										item.SubType = 0x9A;
										break;
									case 0xB:
									case 0xC:
										item.SubType = 2;
										break;
								}
								break;
							case 0x1C:
								if (item.SubType != 3)
									known = false;
								else
									item.SubType = 2;
								break;
							case 0x26:
								switch (item.SubType)
								{
									case 1:
										item.SubType = 3;
										break;
									case 2:
										item.SubType = 1;
										break;
									case 3:
										item.SubType = 5;
										break;
									case 4:
										item.SubType = 6;
										break;
									case 5:
										item.SubType = 7;
										break;
									case 6:
										item.SubType = 4;
										break;
									case 7:
										item.SubType = 8;
										break;
									case 8:
										item.SubType = 9;
										break;
									case 9:
										item.SubType = 10;
										break;
								}
								break;
							case 0x3E:
								if (item.SubType == 1) continue;
								break;
							case 0x41:
								break;
							case 0x71:
								item.ID = 0x74;
								break;
							case 0x79:
								break;
							default:
								known = false;
								break;
						}
						if (!ConvertKnownObjs | known)
							Objs.Add(item);
					}
				}
				else
				{
					S2ObjectEntry item = new S2ObjectEntry();
					item.ID = item1.ID;
					item.RememberState = item1.RememberState;
					item.SubType = item1.SubType;
					item.X = item1.X;
					item.XFlip = item1.XFlip;
					item.Y = item1.Y;
					item.YFlip = item1.YFlip;
					Objs.Add(item);
				}
			}
			LevelData.Objects = Objs;
		}

		private void ObjS1ToS3K()
		{
			List<ObjectEntry> Objs = new List<ObjectEntry>();
			LevelData.Rings = new List<RingEntry>();
			for (int i = 0; i < LevelData.Objects.Count; i++)
			{
				S1ObjectEntry item1 = LevelData.Objects[i] as S1ObjectEntry;
				if (!DontChangeObjects)
				{
					if (item1.ID == 0x25)
					{
						byte rcnt = (byte)(Math.Min(6, item1.SubType & 7) + 1);
						int rtyp = item1.SubType >> 4;
						Point rpos = new Point(item1.X, item1.Y);
						for (int r = 0; r < rcnt; r++)
						{
							LevelData.Rings.Add(new API.S3K.S3KRingEntry() { X = (ushort)rpos.X, Y = (ushort)rpos.Y });
							rpos += RingSpacing[rtyp];
						}
					}
					else
					{
						S3KObjectEntry item = new S3KObjectEntry();
						item.ID = item1.ID;
						item.SubType = item1.SubType;
						item.X = item1.X;
						item.XFlip = item1.XFlip;
						item.Y = item1.Y;
						item.YFlip = item1.YFlip;
						bool known = true;
						switch (item.ID)
						{
							case 0x26:
								item.ID = 1;
								switch (item.SubType)
								{
									case 1:
										item.SubType = 2;
										break;
									case 2:
										item.SubType = 1;
										break;
									case 3:
										item.SubType = 4;
										break;
									case 4:
										item.SubType = 7;
										break;
									case 5:
										item.SubType = 8;
										break;
									case 6:
										item.SubType = 3;
										break;
									case 7:
										item.SubType = 9;
										break;
									case 8:
										item.SubType = 9;
										break;
									case 9:
										item.SubType = 10;
										break;
								}
								break;
							default:
								known = false;
								break;
						}
						if (!ConvertKnownObjs | known)
							Objs.Add(item);
					}
				}
				else
				{
					S3KObjectEntry item = new S3KObjectEntry();
					item.ID = item1.ID;
					item.SubType = item1.SubType;
					item.X = item1.X;
					item.XFlip = item1.XFlip;
					item.Y = item1.Y;
					item.YFlip = item1.YFlip;
					Objs.Add(item);
				}
			}
			LevelData.Objects = Objs;
		}

		private void ObjS1ToSCD()
		{
			List<ObjectEntry> Objs = new List<ObjectEntry>();
			for (int i = 0; i < LevelData.Objects.Count; i++)
			{
				S1ObjectEntry item1 = LevelData.Objects[i] as S1ObjectEntry;
				if (!DontChangeObjects)
				{
					SCDObjectEntry item = new SCDObjectEntry();
					item.ID = item1.ID;
					item.SubType = item1.SubType;
					item.X = item1.X;
					item.XFlip = item1.XFlip;
					item.Y = item1.Y;
					item.YFlip = item1.YFlip;
					item.RememberState = item1.RememberState;
					item.ShowPast = true;
					item.ShowPresent = true;
					item.ShowFuture = true;
					bool known = true;
					switch (item.ID)
					{
						default:
							known = false;
							break;
					}
					if (!ConvertKnownObjs | known)
						Objs.Add(item);
				}
				else
				{
					SCDObjectEntry item = new SCDObjectEntry();
					item.ID = item1.ID;
					item.SubType = item1.SubType;
					item.X = item1.X;
					item.XFlip = item1.XFlip;
					item.Y = item1.Y;
					item.YFlip = item1.YFlip;
					item.RememberState = item1.RememberState;
					item.ShowPast = true;
					item.ShowPresent = true;
					item.ShowFuture = true;
					Objs.Add(item);
				}
			}
			LevelData.Objects = Objs;
		}

		private void ObjS2ToS1()
		{
			List<ObjectEntry> Objs = new List<ObjectEntry>();
			for (int i = 0; i < LevelData.Objects.Count; i++)
			{
				S2ObjectEntry item2 = LevelData.Objects[i] as S2ObjectEntry;
				S1ObjectEntry item = new S1ObjectEntry();
				item.ID = item2.ID;
				item.RememberState = item2.RememberState;
				item.SubType = item2.SubType;
				item.X = item2.X;
				item.XFlip = item2.XFlip;
				item.Y = item2.Y;
				item.YFlip = item2.YFlip;
				if (!DontChangeObjects)
				{
					bool known = true;
					switch (item.ID)
					{
						case 0x26:
							switch (item.SubType)
							{
								case 1:
									item.SubType = 2;
									break;
								case 2:
									item.SubType = 2;
									break;
								case 3:
									item.SubType = 1;
									break;
								case 4:
									item.SubType = 6;
									break;
								case 5:
									item.SubType = 3;
									break;
								case 6:
									item.SubType = 4;
									break;
								case 7:
									item.SubType = 5;
									break;
								case 8:
									item.SubType = 7;
									break;
								case 9:
									item.SubType = 8;
									break;
								case 10:
									item.SubType = 9;
									break;
							}
							break;
						default:
							known = false;
							break;
					}
					if (!ConvertKnownObjs | known)
						Objs.Add(item);
				}
				else
					Objs.Add(item);
			}
			if (!DontChangeObjects)
			{
				foreach (RingEntry item in LevelData.Rings)
				{
					API.S2.S2RingEntry ring = item as API.S2.S2RingEntry;
					Point rpos = new Point(ring.X, ring.Y);
					int rtyp = 0;
					switch (ring.Direction)
					{
						case Direction.Horizontal:
							rtyp = 1;
							break;
						case Direction.Vertical:
							rtyp = 4;
							break;
					}
					Objs.Add(new S1ObjectEntry() { ID = 0x25, SubType = (byte)((ring.Count - 1) | (rtyp << 4)), RememberState = true, X = item.X, Y = item.Y });
				}
				LevelData.Rings = null;
			}
			LevelData.Objects = Objs;
		}

		private void ObjS2ToS3K()
		{
			List<ObjectEntry> Objs = new List<ObjectEntry>();
			for (int i = 0; i < LevelData.Objects.Count; i++)
			{
				S2ObjectEntry item2 = LevelData.Objects[i] as S2ObjectEntry;
				S3KObjectEntry item = new S3KObjectEntry();
				item.ID = item2.ID;
				item.SubType = item2.SubType;
				item.X = item2.X;
				item.XFlip = item2.XFlip;
				item.Y = item2.Y;
				item.YFlip = item2.YFlip;
				if (!DontChangeObjects)
				{
					bool known = true;
					switch (item.ID)
					{
						case 0x26:
							item.ID = 1;
							switch (item.SubType)
							{
								case 1:
									item.SubType = 1;
									break;
								case 2:
									item.SubType = 1;
									break;
								case 3:
									item.SubType = 2;
									break;
								case 4:
									item.SubType = 3;
									break;
								case 5:
									item.SubType = 4;
									break;
								case 6:
									item.SubType = 7;
									break;
								case 7:
									item.SubType = 8;
									break;
								case 8:
									item.SubType = 9;
									break;
								case 9:
									item.SubType = 9;
									break;
								case 10:
									item.SubType = 10;
									break;
							}
							break;
						default:
							known = false;
							break;
					}
					if (!ConvertKnownObjs | known)
						Objs.Add(item);
				}
				else
					Objs.Add(item);
			}
			LevelData.Objects = Objs;
			if (!DontChangeObjects)
			{
				List<RingEntry> Rngs = new List<RingEntry>();
				foreach (RingEntry item in LevelData.Rings)
				{
					API.S2.S2RingEntry ring = item as API.S2.S2RingEntry;
					Point rpos = new Point(ring.X, ring.Y);
					int rtyp = 0;
					switch (ring.Direction)
					{
						case Direction.Horizontal:
							rtyp = 1;
							break;
						case Direction.Vertical:
							rtyp = 4;
							break;
					}
					for (int r = 0; r < ring.Count; r++)
					{
						Rngs.Add(new API.S3K.S3KRingEntry() { X = (ushort)rpos.X, Y = (ushort)rpos.Y });
						rpos += RingSpacing[rtyp];
					}
				}
				LevelData.Rings = Rngs;
			}
		}

		private void ObjS2ToSCD()
		{
			List<ObjectEntry> Objs = new List<ObjectEntry>();
			for (int i = 0; i < LevelData.Objects.Count; i++)
			{
				S2ObjectEntry item2 = LevelData.Objects[i] as S2ObjectEntry;
				SCDObjectEntry item = new SCDObjectEntry();
				item.ID = item2.ID;
				item.RememberState = item2.RememberState;
				item.SubType = item2.SubType;
				item.X = item2.X;
				item.XFlip = item2.XFlip;
				item.Y = item2.Y;
				item.YFlip = item2.YFlip;
				item.ShowPast = true;
				item.ShowPresent = true;
				item.ShowFuture = true;
				if (!DontChangeObjects)
				{
					bool known = true;
					switch (item.ID)
					{
						default:
							known = false;
							break;
					}
					if (!ConvertKnownObjs | known)
						Objs.Add(item);
				}
				else
					Objs.Add(item);
			}
			LevelData.Objects = Objs;
		}

		private void ObjS3KToS1()
		{
			List<ObjectEntry> Objs = new List<ObjectEntry>();
			for (int i = 0; i < LevelData.Objects.Count; i++)
			{
				S3KObjectEntry item3k = LevelData.Objects[i] as S3KObjectEntry;
				S1ObjectEntry item = new S1ObjectEntry();
				item.ID = item3k.ID;
				item.SubType = item3k.SubType;
				item.X = item3k.X;
				item.XFlip = item3k.XFlip;
				item.Y = item3k.Y;
				item.YFlip = item3k.YFlip;
				if (!DontChangeObjects)
				{
					bool known = true;
					switch (item.ID)
					{
						case 1:
							item.ID = 0x26;
							switch (item.SubType)
							{
								case 1:
									item.SubType = 2;
									break;
								case 2:
									item.SubType = 1;
									break;
								case 3:
									item.SubType = 6;
									break;
								case 4:
									item.SubType = 3;
									break;
								case 5:
									item.SubType = 4;
									break;
								case 6:
									item.SubType = 4;
									break;
								case 7:
									item.SubType = 4;
									break;
								case 8:
									item.SubType = 5;
									break;
								case 9:
									item.SubType = 7;
									break;
								case 10:
									item.SubType = 9;
									break;
							}
							break;
						default:
							known = false;
							break;
					}
					if (!ConvertKnownObjs | known)
						Objs.Add(item);
				}
				else
					Objs.Add(item);
			}
			if (!DontChangeObjects)
			{
				foreach (RingEntry item in LevelData.Rings)
				{
					Objs.Add(new S1ObjectEntry() { ID = 0x25, RememberState = true, X = item.X, Y = item.Y });
				}
				LevelData.Rings = null;
			}
			LevelData.Objects = Objs;
		}

		private void ObjS3KToS2()
		{
			List<ObjectEntry> Objs = new List<ObjectEntry>();
			for (int i = 0; i < LevelData.Objects.Count; i++)
			{
				S3KObjectEntry item3k = LevelData.Objects[i] as S3KObjectEntry;
				S2ObjectEntry item = new S2ObjectEntry();
				item.ID = item3k.ID;
				item.SubType = item3k.SubType;
				item.X = item3k.X;
				item.XFlip = item3k.XFlip;
				item.Y = item3k.Y;
				item.YFlip = item3k.YFlip;
				if (!DontChangeObjects)
				{
					bool known = true;
					switch (item.ID)
					{
						case 1:
							item.ID = 0x26;
							switch (item.SubType)
							{
								case 1:
									item.SubType = 1;
									break;
								case 2:
									item.SubType = 3;
									break;
								case 3:
									item.SubType = 4;
									break;
								case 4:
									item.SubType = 5;
									break;
								case 5:
									item.SubType = 6;
									break;
								case 6:
									item.SubType = 6;
									break;
								case 7:
									item.SubType = 6;
									break;
								case 8:
									item.SubType = 7;
									break;
								case 9:
									item.SubType = 9;
									break;
								case 10:
									item.SubType = 10;
									break;
							}
							break;
						default:
							known = false;
							break;
					}
					if (!ConvertKnownObjs | known)
						Objs.Add(item);
				}
				else
					Objs.Add(item);
			}
			LevelData.Objects = Objs;
			if (!DontChangeObjects)
			{
				List<RingEntry> Rngs = new List<RingEntry>();
				foreach (RingEntry item in LevelData.Rings)
				{
					Rngs.Add(new API.S2.S2RingEntry() { X = item.X, Y = item.Y });
				}
				LevelData.Rings = Rngs;
			}
		}

		private void ObjS3KToSCD()
		{
			List<ObjectEntry> Objs = new List<ObjectEntry>();
			for (int i = 0; i < LevelData.Objects.Count; i++)
			{
				S3KObjectEntry item3k = LevelData.Objects[i] as S3KObjectEntry;
				SCDObjectEntry item = new SCDObjectEntry();
				item.ID = item3k.ID;
				item.SubType = item3k.SubType;
				item.X = item3k.X;
				item.XFlip = item3k.XFlip;
				item.Y = item3k.Y;
				item.YFlip = item3k.YFlip;
				item.ShowPast = true;
				item.ShowPresent = true;
				item.ShowFuture = true;
				if (!DontChangeObjects)
				{
					bool known = true;
					switch (item.ID)
					{
						default:
							known = false;
							break;
					}
					if (!ConvertKnownObjs | known)
						Objs.Add(item);
				}
				else
					Objs.Add(item);
			}
			LevelData.Objects = Objs;
		}

		private void ObjSCDToS1()
		{
			List<ObjectEntry> Objs = new List<ObjectEntry>();
			for (int i = 0; i < LevelData.Objects.Count; i++)
			{
				SCDObjectEntry itemcd = LevelData.Objects[i] as SCDObjectEntry;
				S1ObjectEntry item = new S1ObjectEntry();
				item.ID = itemcd.ID;
				item.SubType = itemcd.SubType;
				item.X = itemcd.X;
				item.XFlip = itemcd.XFlip;
				item.Y = itemcd.Y;
				item.YFlip = itemcd.YFlip;
				item.RememberState = itemcd.RememberState;
				if (!DontChangeObjects)
				{
					bool known = true;
					switch (item.ID)
					{
						default:
							known = false;
							break;
					}
					if (!ConvertKnownObjs | known)
						Objs.Add(item);
				}
				else
					Objs.Add(item);
			}
			LevelData.Objects = Objs;
		}

		private void ObjSCDToS2()
		{
			List<ObjectEntry> Objs = new List<ObjectEntry>();
			for (int i = 0; i < LevelData.Objects.Count; i++)
			{
				SCDObjectEntry itemcd = LevelData.Objects[i] as SCDObjectEntry;
				S2ObjectEntry item = new S2ObjectEntry();
				item.ID = itemcd.ID;
				item.SubType = itemcd.SubType;
				item.X = itemcd.X;
				item.XFlip = itemcd.XFlip;
				item.Y = itemcd.Y;
				item.YFlip = itemcd.YFlip;
				item.RememberState = itemcd.RememberState;
				if (!DontChangeObjects)
				{
					bool known = true;
					switch (item.ID)
					{
						default:
							known = false;
							break;
					}
					if (!ConvertKnownObjs | known)
						Objs.Add(item);
				}
				else
					Objs.Add(item);
			}
			LevelData.Objects = Objs;
			LevelData.Rings = new List<RingEntry>();
		}

		private void ObjSCDToS3K()
		{
			List<ObjectEntry> Objs = new List<ObjectEntry>();
			for (int i = 0; i < LevelData.Objects.Count; i++)
			{
				SCDObjectEntry itemcd = LevelData.Objects[i] as SCDObjectEntry;
				S3KObjectEntry item = new S3KObjectEntry();
				item.ID = itemcd.ID;
				item.SubType = itemcd.SubType;
				item.X = itemcd.X;
				item.XFlip = itemcd.XFlip;
				item.Y = itemcd.Y;
				item.YFlip = itemcd.YFlip;
				if (!DontChangeObjects)
				{
					bool known = true;
					switch (item.ID)
					{
						default:
							known = false;
							break;
					}
					if (!ConvertKnownObjs | known)
						Objs.Add(item);
				}
				else
					Objs.Add(item);
			}
			LevelData.Objects = Objs;
			LevelData.Rings = new List<RingEntry>();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < comboBox1.Items.Count; i++)
			{
				comboBox1.SelectedIndex = i;
				ConvertLevel();
				Application.DoEvents();
			}
			System.Diagnostics.Process.Start(Dir);
		}
	}
}