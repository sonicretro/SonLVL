using SonicRetro.SonLVL.API;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SonicRetro.SonLVL.SonPLN
{
	public partial class MainForm : Form
	{
		public static MainForm Instance { get; private set; }
		Settings Settings;
		int pid;

		public MainForm()
		{
			Application.ThreadException += Application_ThreadException;
			Instance = this;
			pid = System.Diagnostics.Process.GetCurrentProcess().Id;
			if (Program.IsMonoRuntime)
				Log("Mono runtime detected.");
			Log("Operating system: " + Environment.OSVersion.ToString());
			LevelData.LogEvent += Log;
			InitializeComponent();
		}

		const int ColorGrid = 64;

		void PaletteChanged()
		{
			for (int i = 0; i < 64; i++)
				LevelData.BmpPal.Entries[i] = LevelData.PaletteToColor(i / 16, i % 16, true);
			LevelData.BmpPal.Entries.CopyTo(LevelImgPalette.Entries, 0);
			LevelImgPalette.Entries[LevelData.ColorTransparent] = LevelData.PaletteToColor(level.BgPalLine & 3, level.BgPalIndex & 0xF, false);
			LevelImgPalette.Entries[ColorGrid] = Settings.GridColor;
			curpal = new Color[16];
			for (int i = 0; i < 16; i++)
				curpal[i] = LevelData.PaletteToColor(SelectedColor.Y, i, false);
			DrawPalette();
			DrawTilePicture();
			RefreshTileSelector();
			TileSelector.Invalidate();
			DrawLevel();
		}

		void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			Log(e.Exception.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
			File.WriteAllLines("SonLVL.log", LogFile.ToArray());
			using (ErrorDialog ed = new ErrorDialog("Unhandled Exception " + e.Exception.GetType().Name + "\nLog file has been saved.\n\nDo you want to try to continue running?", true))
			{
				if (ed.ShowDialog(this) == DialogResult.Cancel)
					Close();
			}
		}

		ImageAttributes imageTransparency = new ImageAttributes();
		Bitmap LevelBmp;
		Graphics LevelGfx, PalettePanelGfx;
		bool loaded;
		Rectangle FGSelection;
		ColorPalette LevelImgPalette;
		double ZoomLevel = 1;
		Point dragpoint;
		bool selecting = false;
		Point lastchunkpoint;
		Point lastmouse;
		internal List<string> LogFile = new List<string>();
		Dictionary<string, ToolStripMenuItem> levelMenuItems;
		string inifilename;
		GameInfo game;
		string levelname;
		LevelInfo level;
		PatternIndex[,] planemap;
		ReplaceTilesDialog replaceTilesDialog;
		TextMapping textMapping;
		List<bool[,]> PalValid = new List<bool[,]>();

		internal void Log(params string[] lines)
		{
			lock (LogFile)
			{
				LogFile.AddRange(lines);
			}
		}

		Tab CurrentTab
		{
			get { return (Tab)tabControl1.SelectedIndex; }
			set { tabControl1.SelectedIndex = (int)value; }
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			Settings = Settings.Load();
			imageTransparency.SetColorMatrix(new ColorMatrix() { Matrix33 = 0.75f }, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
			PalettePanelGfx = PalettePanel.CreateGraphics();
			enableGridToolStripMenuItem.Checked = Settings.ShowGrid;
			foreach (ToolStripMenuItem item in zoomToolStripMenuItem.DropDownItems)
				if (item.Text == Settings.ZoomLevel)
				{
					zoomToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(item));
					break;
				}
			transparentBackgroundToolStripMenuItem.Checked = Settings.TransparentBackgroundExport;
			exportArtcollisionpriorityToolStripMenuItem.Checked = Settings.ExportArtCollisionPriority;
			CurrentTab = Settings.CurrentTab;
			switch (Settings.WindowMode)
			{
				case WindowMode.Maximized:
					WindowState = FormWindowState.Maximized;
					break;
				case WindowMode.Fullscreen:
					prevbnds = Bounds;
					prevstate = WindowState;
					TopMost = true;
					WindowState = FormWindowState.Normal;
					FormBorderStyle = FormBorderStyle.None;
					Bounds = Screen.FromControl(this).Bounds;
					break;
			}
			mainMenuStrip.Visible = Settings.ShowMenu;
			enableDraggingPaletteButton.Checked = Settings.EnableDraggingPalette;
			enableDraggingTilesButton.Checked = Settings.EnableDraggingTiles;
			if (Settings.MRUList == null)
				Settings.MRUList = new List<string>();
			List<string> mru = new List<string>();
			foreach (string item in Settings.MRUList)
			{
				if (File.Exists(item))
				{
					mru.Add(item);
					recentProjectsToolStripMenuItem.DropDownItems.Add(item.Replace("&", "&&"));
				}
			}
			Settings.MRUList = mru;
			if (mru.Count > 0) recentProjectsToolStripMenuItem.DropDownItems.Remove(noneToolStripMenuItem2);
			replaceTilesDialog = new ReplaceTilesDialog();
			if (Program.Arguments.Length > 0)
				LoadINI(Path.GetFullPath(Program.Arguments[0]));
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (loaded)
			{
				switch (MessageBox.Show(this, "Do you want to save?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
				{
					case DialogResult.Yes:
						saveToolStripMenuItem_Click(this, EventArgs.Empty);
						break;
					case DialogResult.Cancel:
						e.Cancel = true;
						break;
				}
			}
			if (Settings != null)
			{
				Settings.ShowGrid = enableGridToolStripMenuItem.Checked;
				Settings.ZoomLevel = zoomToolStripMenuItem.DropDownItems.Cast<ToolStripMenuItem>().Single((a) => a.Checked).Text;
				Settings.CurrentTab = CurrentTab;
				if (TopMost)
					Settings.WindowMode = WindowMode.Fullscreen;
				else if (WindowState == FormWindowState.Maximized)
					Settings.WindowMode = WindowMode.Maximized;
				else
					Settings.WindowMode = WindowMode.Normal;
				Settings.ShowMenu = mainMenuStrip.Visible;
				Settings.EnableDraggingPalette = enableDraggingPaletteButton.Checked;
				Settings.EnableDraggingTiles = enableDraggingTilesButton.Checked;
				Settings.Save();
			}
		}

		private void LoadINI(string filename)
		{
			try
			{
				Log($"Opening INI file \"{filename}\"...");
				game = GameInfo.Load(filename);
				Environment.CurrentDirectory = Path.GetDirectoryName(filename);
				switch (game.EngineVersion)
				{
					case EngineVersion.S1:
					case EngineVersion.SCD:
					case EngineVersion.S2:
					case EngineVersion.S2NA:
					case EngineVersion.S3K:
					case EngineVersion.Chaotix:
					case EngineVersion.Custom:		// assuming it's a Mega Drive engine...
						break;
					case EngineVersion.SKC:
						LevelData.littleendian = true;
						break;
					default:
						throw new NotImplementedException($"Game type {game.EngineVersion} is not supported!");
				}
				Log($"Game type is {game.EngineVersion}.");
			}
			catch (Exception ex)
			{
				using (LoadErrorDialog ed = new LoadErrorDialog(ex.GetType().Name + ": " + ex.Message))
					ed.ShowDialog(this);
				return;
			}
			changeLevelToolStripMenuItem.DropDownItems.Clear();
			levelMenuItems = new Dictionary<string, ToolStripMenuItem>();
			foreach (KeyValuePair<string, LevelInfo> item in game.Levels)
			{
				if (!string.IsNullOrEmpty(item.Key))
				{
					string[] itempath = item.Key.Split('\\');
					ToolStripMenuItem parent = changeLevelToolStripMenuItem;
					string curpath = string.Empty;
					for (int i = 0; i < itempath.Length - 1; i++)
					{
						if (i - 1 >= 0)
							curpath += @"\";
						curpath += itempath[i];
						if (!levelMenuItems.ContainsKey(curpath))
						{
							ToolStripMenuItem it = new ToolStripMenuItem(itempath[i].Replace("&", "&&"));
							levelMenuItems.Add(curpath, it);
							parent.DropDownItems.Add(it);
							parent = it;
						}
						else
							parent = levelMenuItems[curpath];
					}
					ToolStripMenuItem ts = new ToolStripMenuItem(itempath[itempath.Length - 1].Replace("&", "&&"), null, new EventHandler(LevelToolStripMenuItem_Clicked)) { Tag = item.Key };
					levelMenuItems.Add(item.Key, ts);
					parent.DropDownItems.Add(ts);
				}
			}
			Text = "SonPLN - " + game.EngineVersion.ToString();
			if (Settings.MRUList.Count == 0)
				recentProjectsToolStripMenuItem.DropDownItems.Remove(noneToolStripMenuItem2);
			if (Settings.MRUList.Contains(filename))
			{
				recentProjectsToolStripMenuItem.DropDownItems.RemoveAt(Settings.MRUList.IndexOf(filename));
				Settings.MRUList.Remove(filename);
			}
			Settings.MRUList.Insert(0, filename);
			recentProjectsToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem(filename));
			inifilename = filename;
		}

		#region Main Menu
		#region File Menu
		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (loaded)
			{
				switch (MessageBox.Show(this, "Do you want to save?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
				{
					case DialogResult.Yes:
						saveToolStripMenuItem_Click(this, EventArgs.Empty);
						break;
					case DialogResult.Cancel:
						return;
				}
			}
			using (OpenFileDialog a = new OpenFileDialog()
			{
				DefaultExt = "ini",
				Filter = "INI Files|*.ini|All Files|*.*"
			})
				if (a.ShowDialog(this) == DialogResult.OK)
				{
					loaded = false;
					LoadINI(a.FileName);
				}
		}

		private void LevelToolStripMenuItem_Clicked(object sender, EventArgs e)
		{
			if (loaded)
			{
				fileToolStripMenuItem.DropDown.Hide();
				switch (MessageBox.Show(this, "Do you want to save?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
				{
					case DialogResult.Yes:
						saveToolStripMenuItem_Click(this, EventArgs.Empty);
						break;
					case DialogResult.Cancel:
						return;
				}
			}
			loaded = false;
			foreach (KeyValuePair<string, ToolStripMenuItem> item in levelMenuItems)
				item.Value.Checked = false;
			((ToolStripMenuItem)sender).Checked = true;
			Enabled = false;
			UseWaitCursor = true;
			levelname = (string)((ToolStripMenuItem)sender).Tag;
			level = game.GetLevelInfo(levelname);
			Text = $"SonPLN - {game.EngineVersion} - Loading {level.DisplayName}...";
#if !DEBUG
			initerror = null;
			backgroundLevelLoader.RunWorkerAsync();
#else
			backgroundLevelLoader_DoWork(null, null);
			backgroundLevelLoader_RunWorkerCompleted(null, null);
#endif
		}

		Exception initerror = null;
		private void backgroundLevelLoader_DoWork(object sender, DoWorkEventArgs e)
		{
#if !DEBUG
			try
			{
#endif
				Log("Loading " + level.DisplayName + "...");
#if !DEBUG
				System.Threading.Tasks.Parallel.Invoke(LoadLevelTiles, LoadLevelLayout, LoadLevelPalette, ProcessTextMapping);
#else
				LoadLevelTiles();
				LoadLevelLayout();
				LoadLevelPalette();
				ProcessTextMapping();
#endif
				using (Bitmap palbmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
					LevelData.BmpPal = palbmp.Palette;
				for (int i = 0; i < 64; i++)
					LevelData.BmpPal.Entries[i] = LevelData.PaletteToColor(i / 16, i % 16, true);
				for (int i = 64; i < 256; i++)
					LevelData.BmpPal.Entries[i] = Color.Black;
				using (Bitmap palbmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
					LevelImgPalette = palbmp.Palette;
				LevelData.BmpPal.Entries.CopyTo(LevelImgPalette.Entries, 0);
				LevelImgPalette.Entries[LevelData.ColorTransparent] = LevelData.PaletteToColor(level.BgPalLine & 3, level.BgPalIndex & 0xF, false);
				LevelImgPalette.Entries[ColorGrid] = Settings.GridColor;
				curpal = new Color[16];
#if !DEBUG
			}
			catch (Exception ex) { initerror = ex; }
#endif
		}

		private void LoadLevelTiles()
		{
			LevelData.Tiles = new MultiFileIndexer<byte[]>(() => new byte[32]);
			if (level.Tiles == null || string.IsNullOrWhiteSpace(level.Tiles[0].Filename))
				throw new FormatException("Level must contain at least one Tiles file!");
			foreach (API.FileInfo tileent in level.Tiles)
			{
				if (tileent.Filename.Equals("dummy", StringComparison.OrdinalIgnoreCase))
				{
					Log("Loading dummy tile data.");
					LevelData.Tiles.AddFile(new List<byte[]>() { new byte[32] }, tileent.Offset == -1 ? tileent.Offset : tileent.Offset / 32);
				}
				else if (File.Exists(tileent.Filename))
				{
					if (LevelData.Tiles.FileCount == 0 && (tileent.Offset > 0 || level.TileOffset > 0))
					{
						Log("Loading dummy tile data.");
						LevelData.Tiles.AddFile(new List<byte[]>() { new byte[32] }, -1);
					}
					Log("Loading 8x8 tiles from file \"" + tileent.Filename + "\", using compression " + level.TileCompression.ToString() + "...");
					byte[] tmp = Compression.Decompress(tileent.Filename, level.TileCompression);
					LevelData.Pad(ref tmp, 32);
					List<byte[]> tiles = new List<byte[]>();
					for (int i = 0; i < tmp.Length; i += 32)
					{
						byte[] tile = new byte[32];
						Array.Copy(tmp, i, tile, 0, 32);
						tiles.Add(tile);
					}
					LevelData.Tiles.AddFile(tiles, tileent.Offset == -1 ? tileent.Offset : tileent.Offset / 32);
				}
				else
				{
					Log("8x8 tile file \"" + tileent.Filename + "\" not found.");
					LevelData.Tiles.AddFile(new List<byte[]>() { new byte[32] }, tileent.Offset == -1 ? tileent.Offset : tileent.Offset / 32);
				}
			}
			LevelData.Tiles.FillGaps();
			LevelData.UpdateTileArray();
		}

		private void LoadLevelLayout()
		{
			planemap = new PatternIndex[level.Width, level.Height];
			for (int y = 0; y < level.Height; y++)
				for (int x = 0; x < level.Width; x++)
					planemap[x, y] = new PatternIndex();
			if (level.Mappings == null)
				throw new FormatException("Level must contain Mappings file!");
			if (File.Exists(level.Mappings))
			{
				Log("Loading plane mappings from file \"" + level.Mappings + "\", using compression " + level.MappingsCompression.ToString() + "...");
				byte[] tmp = Compression.Decompress(level.Mappings, level.MappingsCompression);
				if (tmp.Length / 2 != level.Width * level.Height)
					Log($"Mappings data size does not match given width/height. Expected: {level.Width * level.Height * 2} ({level.Width}x{level.Height}*2) Actual: {tmp.Length}.");
				int x = 0, y = 0;
				for (int i = 0; i < tmp.Length; i += PatternIndex.Size)
				{
					planemap[x, y] = new PatternIndex(tmp, i);
					if (level.TileOffset > 0 && planemap[x, y].Tile > 0)
						planemap[x, y].Tile -= (ushort)(level.TileOffset - 1);
					if (++x == level.Width)
					{
						x = 0;
						if (++y == level.Height)
							break;
					}
				}
			}
			else
				Log($"Plane mappings file \"{level.Mappings}\" not found.");
		}

		private void LoadLevelPalette()
		{
			LevelData.PalName = new List<string>();
			LevelData.Palette = new List<SonLVLColor[,]>();
			LevelData.PalNum = new List<byte[,]>();
			LevelData.PalAddr = new List<int[,]>();
			PalValid = new List<bool[,]>();
			if (level.Palette == null || string.IsNullOrWhiteSpace(level.Palette[0].Filename))
				throw new FormatException("Level must contain at least one Palette file!");
			byte palfilenum = 0;
			for (int palnum = 0; palnum < level.Palettes.Length; palnum++)
			{
				LevelData.PalName.Add(level.Palettes[palnum].Name);
				LevelData.Palette.Add(new SonLVLColor[4, 16]);
				LevelData.PalNum.Add(new byte[4, 16]);
				LevelData.PalAddr.Add(new int[4, 16]);
				PalValid.Add(new bool[4, 16]);
				for (byte pn = 0; pn < level.Palettes[palnum].Palettes.Collection.Length; pn++)
				{
					PaletteInfo palent = level.Palettes[palnum].Palettes[pn];
					Log("Loading palette file \"" + palent.Filename + "\"...", "Source: " + palent.Source + " Destination: " + palent.Destination + " Length: " + palent.Length);
					if (!File.Exists(palent.Filename)) throw new FileNotFoundException("Palette file could not be loaded! Have you set up your disassembly properly?", palent.Filename);
					SonLVLColor[] palfile = SonLVLColor.Load(palent.Filename, level.PaletteFormat);
					int ind = palent.Destination % 16;
					int line = palent.Destination / 16;
					int src = palent.Source;
					for (int pa = 0; pa < palent.Length; pa++)
					{
						LevelData.Palette[palnum][line, ind] = palfile[src];
						LevelData.PalNum[palnum][line, ind] = palfilenum;
						LevelData.PalAddr[palnum][line, ind] = src;
						PalValid[palnum][line, ind] = true;
						if (++ind == 16)
						{
							ind = 0;
							if (++line == 4)
								line = 0;
						}
						if (++src == palfile.Length)
							src = 0;
					}
					palfilenum++;
				}
			}
			LevelData.CurPal = 0;
		}

		private void ProcessTextMapping()
		{
			textMapping = null;
			if (level.TextMapping != null)
				textMapping = new TextMapping(level.TextMapping);
			else if (level.TextMappingFile != null && File.Exists(level.TextMappingFile))
				textMapping = IniSerializer.Deserialize<TextMapping>(level.TextMappingFile);
			if (textMapping != null && level.TileOffset > 0)
				foreach (var cm in textMapping.Characters)
					for (int y = 0; y < textMapping.Height; y++)
						for (int x = 0; x < (cm.Value.Width ?? textMapping.DefaultWidth); x++)
							if (cm.Value.Map[x, y] > 0)
								cm.Value.Map[x, y] -= (ushort)(level.TileOffset - 1);
		}

		private void backgroundLevelLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (initerror != null)
			{
				Log(initerror.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
				File.WriteAllLines("SonPLN.log", LogFile.ToArray());
				string msg = initerror.GetType().Name + ": " + initerror.Message;
				if (initerror is AggregateException ae)
				{
					msg += " =>";
					foreach (Exception ex in ae.InnerExceptions)
						msg += Environment.NewLine + ex.GetType().Name + ": " + ex.Message;
				}
				using (LoadErrorDialog ed = new LoadErrorDialog(msg))
					ed.ShowDialog(this);
				Text = "SonPLN - " + game.EngineVersion.ToString();
				Enabled = true;
				return;
			}
			Log("Load completed.");
			importTilesToolStripButton.Enabled = LevelData.Tiles.Count < 0x800;
			drawTileToolStripButton.Enabled = importTilesToolStripButton.Enabled;
			RefreshTileSelector();
			TileSelector.SelectedIndex = 0;
			TileSelector.ChangeSize();
			Text = "SonPLN - " + game.EngineVersion + " - " + level.DisplayName;
			UpdateScrollBars();
			foregroundPanel.HScrollValue = 0;
			foregroundPanel.HScrollSmallChange = 8;
			foregroundPanel.HScrollLargeChange = 128;
			foregroundPanel.VScrollValue = 0;
			foregroundPanel.VScrollSmallChange = 8;
			foregroundPanel.VScrollLargeChange = 128;
			foregroundPanel.HScrollEnabled = true;
			foregroundPanel.VScrollEnabled = true;
			switch (level.PaletteFormat)
			{
				case EngineVersion.SKC:
					colorRed.Maximum = colorGreen.Maximum = colorBlue.Maximum = 0xF;
					colorRed.Increment = colorGreen.Increment = colorBlue.Increment = 1;
					colorRed.Hexadecimal = colorGreen.Hexadecimal = colorBlue.Hexadecimal = true;
					break;
				default:
					colorRed.Maximum = colorGreen.Maximum = colorBlue.Maximum = 0xE;
					colorRed.Increment = colorGreen.Increment = colorBlue.Increment = 2;
					colorRed.Hexadecimal = colorGreen.Hexadecimal = colorBlue.Hexadecimal = true;
					break;
			}
			colorEditingPanel.Enabled = true;
			paletteToolStrip.Enabled = true;
			panel2.Enabled = true;
			copiedTile = new PatternIndex();
			xFlip.Checked = false;
			yFlip.Checked = false;
			priority.Checked = false;
			loaded = true;
			saveToolStripMenuItem.Enabled = true;
			editToolStripMenuItem.Enabled = true;
			exportToolStripMenuItem.Enabled = true;
			paletteToolStripDropDownButton.DropDownItems.Clear();
			foreach (string item in LevelData.PalName)
				paletteToolStripDropDownButton.DropDownItems.Add(new ToolStripMenuItem(item));
			((ToolStripMenuItem)paletteToolStripDropDownButton.DropDownItems[0]).Checked = true;
			paletteToolStripDropDownButton.Text = "Palette: " + LevelData.PalName[0];
			for (int i = 0; i < 16; i++)
				curpal[i] = LevelData.PaletteToColor(SelectedColor.Y, i, false);
			TileID.Maximum = LevelData.Tiles.Count - 1;
			TileCount.Text = LevelData.Tiles.Count.ToString("X") + " / 800";
			deleteUnusedTilesToolStripButton.Enabled = removeDuplicateTilesToolStripButton.Enabled =
				replaceForegroundToolStripButton.Enabled = clearForegroundToolStripButton.Enabled =
				importToolStripButton.Enabled = usageCountsToolStripMenuItem.Enabled = true;
			Enabled = true;
			UseWaitCursor = false;
			DrawLevel();
		}

		private void RefreshTileSelector()
		{
			TileSelector.Images.Clear();
			for (int i = 0; i < LevelData.Tiles.Count; i++)
				TileSelector.Images.Add(LevelData.TileToBmp4bpp(LevelData.Tiles[i], 0, SelectedColor.Y, false));
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Threading.Tasks.Parallel.Invoke(SaveLevelTiles, SaveLevelLayout, SaveLevelPalette);
			game.Levels[levelname].Width = planemap.GetLength(0);
			game.Levels[levelname].Height = planemap.GetLength(1);
			game.Save(inifilename);
		}

		private void SaveLevelTiles()
		{
			int fileind = -1;
			ReadOnlyCollection<ReadOnlyCollection<byte[]>> tilefiles = LevelData.Tiles.GetFiles();
			foreach (API.FileInfo tileent in level.Tiles)
			{
				fileind++;
				if (tileent.Filename.Equals("dummy", StringComparison.OrdinalIgnoreCase)) continue;
				if (fileind == 0 && (tileent.Offset > 0 || level.TileOffset > 0))
					fileind++; // skip dummy data
				List<byte> tmp = new List<byte>(tilefiles[fileind].Count * 32);
				foreach (byte[] item in tilefiles[fileind])
					tmp.AddRange(item);
				Compression.Compress(tmp.ToArray(), tileent.Filename, level.TileCompression);
			}
		}

		private void SaveLevelLayout()
		{
			List<byte> tmp = new List<byte>(planemap.GetLength(0) * planemap.GetLength(1) * PatternIndex.Size);
			for (int y = 0; y < planemap.GetLength(1); y++)
				for (int x = 0; x < planemap.GetLength(0); x++)
				{
					if (level.TileOffset > 0 && planemap[x,y].Tile > 0)
					{
						PatternIndex pi = planemap[x, y].Clone();
						pi.Tile += (ushort)(level.TileOffset - 1);
						tmp.AddRange(pi.GetBytes());
					}
					else
						tmp.AddRange(planemap[x, y].GetBytes());
				}
			Compression.Compress(tmp.ToArray(), level.Mappings, level.MappingsCompression);
		}

		private void SaveLevelPalette()
		{
			byte[] paltmp;
			List<ushort[]> palfiles = new List<ushort[]>();
			byte palfilenum = 0;
			for (int palnum = 0; palnum < level.Palettes.Length; palnum++)
			{
				PaletteList palent = level.Palettes[palnum].Palettes;
				for (byte pn = 0; pn < palent.Collection.Length; pn++)
				{
					paltmp = File.ReadAllBytes(palent[pn].Filename);
					ushort[] palfile = new ushort[paltmp.Length / 2];
					for (int pi = 0; pi < paltmp.Length; pi += 2)
						palfile[pi / 2] = API.ByteConverter.ToUInt16(paltmp, pi);
					palfiles.Add(palfile);
				}
				for (int pl = 0; pl < 4; pl++)
					for (int pi = 0; pi < 16; pi++)
						if (PalValid[palnum][pl, pi])
							palfiles[LevelData.PalNum[palnum][pl, pi]][LevelData.PalAddr[palnum][pl, pi]] = LevelData.Palette[palnum][pl, pi].MDColor;
				for (byte pn = 0; pn < palent.Collection.Length; pn++)
				{
					List<byte> tmp = new List<byte>();
					for (int pi = 0; pi < palfiles[pn + palfilenum].Length; pi++)
						tmp.AddRange(API.ByteConverter.GetBytes(palfiles[pn + palfilenum][pi]));
					File.WriteAllBytes(palent[pn].Filename, tmp.ToArray());
				}
				palfilenum = (byte)palfiles.Count;
			}
		}

		private void recentProjectsToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			loaded = false;
			LoadINI(Settings.MRUList[recentProjectsToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem)]);
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}
		#endregion

		#region Edit Menu
		private void resizeLevelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(this, "Resizing the mappings requires editing the code that loads the mappings. Are you sure you want to do this?", "SonPLN", MessageBoxButtons.OKCancel) == DialogResult.OK)
				using (ResizeLevelDialog dg = new ResizeLevelDialog())
				{
					dg.levelHeight.Value = planemap.GetLength(1);
					dg.levelWidth.Value = planemap.GetLength(0);
					if (dg.ShowDialog(this) == DialogResult.OK)
					{
						ResizeMap((int)dg.levelWidth.Value, (int)dg.levelHeight.Value);
						loaded = false;
						UpdateScrollBars();
						loaded = true;
						DrawLevel();
					}
				}
		}

		private void ResizeMap(int width, int height)
		{
			int oldwidth = planemap.GetLength(0);
			int oldheight = planemap.GetLength(1);
			PatternIndex[,] newFG = new PatternIndex[width, height];
			for (int y = 0; y < height; y++)
				for (int x = 0; x < width; x++)
					if (x < oldwidth && y < oldheight)
						newFG[x, y] = planemap[x, y];
					else
						newFG[x, y] = new PatternIndex();
			planemap = newFG;
		}
		#endregion

		#region View Menu
		private void paletteToolStripDropDownButton_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			foreach (ToolStripMenuItem item in paletteToolStripDropDownButton.DropDownItems)
				item.Checked = false;
			((ToolStripMenuItem)e.ClickedItem).Checked = true;
			LevelData.CurPal = paletteToolStripDropDownButton.DropDownItems.IndexOf(e.ClickedItem);
			paletteToolStripDropDownButton.Text = "Palette: " + e.ClickedItem.Text;
			PaletteChanged();
			DrawLevel();
		}

		private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ColorDialog a = new ColorDialog
			{
				AllowFullOpen = true,
				AnyColor = true,
				FullOpen = true,
				SolidColorOnly = true,
				Color = Settings.GridColor
			};
			if (cols != null)
				a.CustomColors = cols;
			if (a.ShowDialog() == DialogResult.OK)
			{
				Settings.GridColor = a.Color;
				if (loaded)
				{
					LevelImgPalette.Entries[ColorGrid] = a.Color;
					DrawLevel();
				}
			}
			cols = a.CustomColors;
			a.Dispose();
		}

		private void zoomToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			foreach (ToolStripMenuItem item in zoomToolStripMenuItem.DropDownItems)
				item.Checked = false;
			((ToolStripMenuItem)e.ClickedItem).Checked = true;
			switch (zoomToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem))
			{
				case 0: // 1/8x
					ZoomLevel = 0.125;
					break;
				case 1: // 1/4x
					ZoomLevel = 0.25;
					break;
				case 2: // 1/2x
					ZoomLevel = 0.5;
					break;
				default:
					ZoomLevel = zoomToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem) - 2;
					break;
			}
			if (!loaded) return;
			loaded = false;
			UpdateScrollBars();
			loaded = true;
			DrawLevel();
		}
		#endregion

		#region Export Menu
		private void pNGToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			exportToolStripMenuItem.DropDown.Hide();
			using (SaveFileDialog a = new SaveFileDialog() { DefaultExt = "png", Filter = "PNG Files|*.png", RestoreDirectory = true })
				if (a.ShowDialog(this) == DialogResult.OK)
				{
					int line = pNGToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem);
					if (line < 4)
					{
						BitmapBits bmp = new BitmapBits(16 * 8, 8);
						Color[] pal = new Color[16];
						for (int i = 0; i < 16; i++)
						{
							pal[i] = LevelData.PaletteToColor(line, i, false);
							bmp.FillRectangle((byte)i, i * 8, 0, 8, 8);
						}
						bmp.ToBitmap4bpp(pal).Save(a.FileName);
					}
					else
					{
						BitmapBits bmp = new BitmapBits(16 * 8, 4 * 8);
						Color[] pal = new Color[256];
						for (int i = 0; i < 64; i++)
							pal[i] = LevelData.PaletteToColor(i / 16, i % 16, false);
						pal.Fill(Color.Black, 64, 192);
						for (int y = 0; y < 4; y++)
							for (int x = 0; x < 16; x++)
								bmp.FillRectangle((byte)((y * 16) + x), x * 8, y * 8, 8, 8);
						bmp.ToBitmap(pal).Save(a.FileName);
					}
				}
		}

		private void yYCHRToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog a = new SaveFileDialog() { DefaultExt = "pal", Filter = "Palette Files|*.pal", RestoreDirectory = true })
				if (a.ShowDialog(this) == DialogResult.OK)
					using (FileStream str = File.Create(a.FileName))
					using (BinaryWriter bw = new BinaryWriter(str))
					{
						int cnt = Math.Min(LevelData.Palette.Count, 4);
						for (int i = 0; i < cnt; i++)
						{
							SonLVLColor[,] pal = LevelData.Palette[(LevelData.CurPal + i) % LevelData.Palette.Count];
							for (int y = 0; y < 4; y++)
								for (int x = 0; x < 16; x++)
								{
									bw.Write(pal[y, x].R);
									bw.Write(pal[y, x].G);
									bw.Write(pal[y, x].B);
								}
						}
						if (cnt != 4)
							bw.Write(new byte[0xC0 * (4 - cnt)]);
					}
		}

		private void jASCPALToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			exportToolStripMenuItem.DropDown.Hide();
			using (SaveFileDialog a = new SaveFileDialog() { DefaultExt = "pal", Filter = "JASC-PAL Files|*.pal;*.PspPalette", RestoreDirectory = true })
				if (a.ShowDialog(this) == DialogResult.OK)
					using (StreamWriter writer = File.CreateText(a.FileName))
					{
						writer.WriteLine("JASC-PAL");
						writer.WriteLine("0100");
						int line = jASCPALToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem);
						if (line < 4)
						{
							writer.WriteLine("16");
							for (int i = 0; i < 16; i++)
							{
								SonLVLColor col = LevelData.Palette[LevelData.CurPal][line, i];
								writer.WriteLine("{0} {1} {2}", col.R, col.G, col.B);
							}
						}
						else
						{
							writer.WriteLine("256");
							for (int y = 0; y < 4; y++)
								for (int x = 0; x < 16; x++)
								{
									SonLVLColor col = LevelData.Palette[LevelData.CurPal][y, x];
									writer.WriteLine("{0} {1} {2}", col.R, col.G, col.B);
								}
							for (int i = 64; i < 256; i++)
								writer.WriteLine("0 0 0");
						}
						writer.Close();
					}
		}

		private void tilesToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			exportToolStripMenuItem.DropDown.Hide();
			using (FolderBrowserDialog a = new FolderBrowserDialog() { SelectedPath = Environment.CurrentDirectory })
				if (a.ShowDialog() == DialogResult.OK)
					for (int i = 0; i < LevelData.Tiles.Count; i++)
						LevelData.TileToBmp4bpp(LevelData.Tiles[i], 0, tilesToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem), transparentBackgroundToolStripMenuItem.Checked)
							.Save(Path.Combine(a.SelectedPath,
							(useHexadecimalIndexesToolStripMenuItem.Checked ? i.ToString("X2") : i.ToString()) + ".png"));
		}

		private void foregroundToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog a = new SaveFileDialog()
						{
							DefaultExt = "png",
							Filter = "PNG Files|*.png",
							RestoreDirectory = true
						})
				if (a.ShowDialog() == DialogResult.OK)
				{
					if (exportArtcollisionpriorityToolStripMenuItem.Checked)
					{
						string pathBase = Path.Combine(Path.GetDirectoryName(a.FileName), Path.GetFileNameWithoutExtension(a.FileName));
						string pathExt = Path.GetExtension(a.FileName);
						BitmapBits bmp = DrawPlane(null, true, true);
						using (Bitmap res = bmp.ToBitmap())
						{
							ColorPalette pal = res.Palette;
							for (int i = 0; i < 64; i++)
								pal.Entries[i] = LevelData.PaletteToColor(i / 16, i % 16, transparentBackgroundToolStripMenuItem.Checked);
							pal.Entries.Fill(Color.Black, 64, 192);
							res.Palette = pal;
							res.Save(a.FileName);
						}
						bmp.Clear();
						for (int ly = 0; ly < planemap.GetLength(1); ly++)
							for (int lx = 0; lx < planemap.GetLength(0); lx++)
								if (planemap[lx, ly].Priority)
									bmp.FillRectangle(1, lx * 8, ly * 8, 8, 8);
						bmp.ToBitmap1bpp(Color.Black, Color.White).Save(pathBase + "_pri" + pathExt);
					}
					else
					{
						BitmapBits bmp = DrawPlane(null, true, true);
						using (Bitmap res = bmp.ToBitmap(LevelImgPalette))
							res.Save(a.FileName);
					}
				}
		}

		private void transparentBackgroundToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			Settings.TransparentBackgroundExport = transparentBackgroundToolStripMenuItem.Checked;
		}

		private void useHexadecimalIndexesToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			Settings.UseHexadecimalIndexesExport = useHexadecimalIndexesToolStripMenuItem.Checked;
		}

		private void exportArtcollisionpriorityToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			Settings.ExportArtCollisionPriority = exportArtcollisionpriorityToolStripMenuItem.Checked;
		}
		#endregion

		#region Help Menu
		private void reportBugToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (BugReportDialog err = new BugReportDialog("SonLVL", string.Join(Environment.NewLine, LogFile.ToArray())))
				err.ShowDialog();
		}
		#endregion
		#endregion

		public BitmapBits DrawPlane(Rectangle? section, bool lowPlane, bool highPlane)
		{
			Rectangle bounds;
			if (section.HasValue)
				bounds = section.Value;
			else
				bounds = new Rectangle(0, 0, planemap.GetLength(0) * 8, planemap.GetLength(1) * 8);
			BitmapBits levelImg8bpp = new BitmapBits(bounds.Size);
			for (int y = Math.Max(bounds.Y / 8, 0); y <= Math.Min((bounds.Bottom - 1) / 8, planemap.GetLength(1) - 1); y++)
				for (int x = Math.Max(bounds.X / 8, 0); x <= Math.Min((bounds.Right - 1) / 8, planemap.GetLength(0) - 1); x++)
					if (planemap[x, y].Tile < LevelData.Tiles.Count && ((!planemap[x, y].Priority && lowPlane) || (planemap[x, y].Priority && highPlane)))
						levelImg8bpp.DrawBitmapBounded(LevelData.TileToBmp8bpp(LevelData.TileArray, planemap[x, y]), x * 8 - bounds.X, y * 8 - bounds.Y);
			return levelImg8bpp;
		}

		BitmapBits LevelImg8bpp;
		static readonly Pen selectionPen = new Pen(Color.FromArgb(128, Color.Black)) { DashStyle = DashStyle.Dot };
		static readonly SolidBrush selectionBrush = new SolidBrush(Color.FromArgb(128, Color.White));
		internal void DrawLevel()
		{
			if (!loaded) return;
			ScrollingPanel panel;
			Rectangle selection;
			switch (CurrentTab)
			{
				case Tab.Foreground:
					panel = foregroundPanel;
					selection = FGSelection;
					break;
				default:
					return;
			}
			Point camera = new Point(panel.HScrollValue, panel.VScrollValue);
			Rectangle dispRect = new Rectangle(camera.X, camera.Y, (int)(panel.PanelWidth / ZoomLevel), (int)(panel.PanelHeight / ZoomLevel));
			LevelImg8bpp = DrawPlane(dispRect, true, true);
			if (enableGridToolStripMenuItem.Checked)
			{
				for (int x = (8 - (camera.X % 8)) % 8; x < LevelImg8bpp.Width; x += 8)
					LevelImg8bpp.DrawLine(ColorGrid, x, 0, x, LevelImg8bpp.Height - 1);
				for (int y = (8 - (camera.Y % 8)) % 8; y < LevelImg8bpp.Height; y += 8)
					LevelImg8bpp.DrawLine(ColorGrid, 0, y, LevelImg8bpp.Width - 1, y);
			}
			LevelBmp = LevelImg8bpp.ToBitmap(LevelImgPalette).To32bpp();
			LevelGfx = Graphics.FromImage(LevelBmp);
			LevelGfx.SetOptions();
			Point pnlcur = panel.PanelPointToClient(Cursor.Position);
			if (!selecting && SelectedTile < LevelData.Tiles.Count)
				LevelGfx.DrawImage(tile.ToBitmap(curpal),
				new Rectangle(((((int)(pnlcur.X / ZoomLevel) + camera.X) / 8) * 8) - camera.X, ((((int)(pnlcur.Y / ZoomLevel) + camera.Y) / 8) * 8) - camera.Y, 8, 8),
				0, 0, 8, 8,
				GraphicsUnit.Pixel, imageTransparency);
			if (!selection.IsEmpty)
			{
				Rectangle selbnds = selection.Scale(8, 8);
				selbnds.Offset(-camera.X, -camera.Y);
				LevelGfx.FillRectangle(selectionBrush, selbnds);
				selbnds.Width--; selbnds.Height--;
				LevelGfx.DrawRectangle(selectionPen, selbnds);
			}
			panel.PanelGraphics.DrawImage(LevelBmp, 0, 0, panel.PanelWidth, panel.PanelHeight);
		}

		private void panel_Paint(object sender, PaintEventArgs e)
		{
			DrawLevel();
		}

		private void UpdateScrollBars()
		{
			foregroundPanel.HScrollMaximum = (int)Math.Max((planemap.GetLength(0) * 8) + foregroundPanel.HScrollLargeChange - (foregroundPanel.PanelWidth / ZoomLevel), 0);
			foregroundPanel.VScrollMaximum = (int)Math.Max((planemap.GetLength(1) * 8) + foregroundPanel.VScrollLargeChange - (foregroundPanel.PanelHeight / ZoomLevel), 0);
		}

		Rectangle prevbnds;
		FormWindowState prevstate;
		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Enter:
					if (e.Alt)
					{
						if (!TopMost)
						{
							prevbnds = Bounds;
							prevstate = WindowState;
							TopMost = true;
							WindowState = FormWindowState.Normal;
							FormBorderStyle = FormBorderStyle.None;
							Bounds = Screen.FromControl(this).Bounds;
						}
						else
						{
							TopMost = false;
							WindowState = prevstate;
							FormBorderStyle = FormBorderStyle.Sizable;
							Bounds = prevbnds;
						}
					}
					break;
				case Keys.F5:
					mainMenuStrip.ShowHide();
					break;
				case Keys.D1:
				case Keys.NumPad1:
					if (e.Control)
						CurrentTab = Tab.Foreground;
					break;
				case Keys.D2:
				case Keys.NumPad2:
					if (e.Control)
						CurrentTab = Tab.Art;
					break;
			}
		}

		private void foregroundPanel_KeyDown(object sender, KeyEventArgs e)
		{
			long step = e.Control ? int.MaxValue : e.Shift ? 8 : 16;
			switch (e.KeyCode)
			{
				case Keys.Up:
					if (!loaded) return;
					foregroundPanel.VScrollValue = (int)Math.Max(foregroundPanel.VScrollValue - step, foregroundPanel.VScrollMinimum);
					break;
				case Keys.Down:
					if (!loaded) return;
					foregroundPanel.VScrollValue = (int)Math.Min(foregroundPanel.VScrollValue + step, foregroundPanel.VScrollMaximum - 8 + 1);
					break;
				case Keys.Left:
					if (!loaded) return;
					foregroundPanel.HScrollValue = (int)Math.Max(foregroundPanel.HScrollValue - step, foregroundPanel.HScrollMinimum);
					break;
				case Keys.Right:
					if (!loaded) return;
					foregroundPanel.HScrollValue = (int)Math.Min(foregroundPanel.HScrollValue + step, foregroundPanel.HScrollMaximum - 8 + 1);
					break;
				case Keys.A:
					if (!loaded) return;
					SelectedTile = SelectedTile == 0 ? LevelData.Tiles.Count - 1 : SelectedTile - 1;
					DrawLevel();
					break;
				case Keys.Z:
					if (!loaded) return;
					SelectedTile = SelectedTile == LevelData.Tiles.Count - 1 ? 0 : SelectedTile + 1;
					DrawLevel();
					break;
				case Keys.I:
					enableGridToolStripMenuItem.Checked = !enableGridToolStripMenuItem.Checked;
					DrawLevel();
					break;
				case Keys.OemMinus:
				case Keys.Subtract:
					for (int i = 1; i < zoomToolStripMenuItem.DropDownItems.Count; i++)
						if (((ToolStripMenuItem)zoomToolStripMenuItem.DropDownItems[i]).Checked)
						{
							zoomToolStripMenuItem_DropDownItemClicked(sender, new ToolStripItemClickedEventArgs(zoomToolStripMenuItem.DropDownItems[i - 1]));
							break;
						}
					break;
				case Keys.Oemplus:
				case Keys.Add:
					for (int i = 0; i < zoomToolStripMenuItem.DropDownItems.Count - 1; i++)
						if (((ToolStripMenuItem)zoomToolStripMenuItem.DropDownItems[i]).Checked)
						{
							zoomToolStripMenuItem_DropDownItemClicked(sender, new ToolStripItemClickedEventArgs(zoomToolStripMenuItem.DropDownItems[i + 1]));
							break;
						}
					break;
			}
		}

		private void foregroundPanel_MouseDown(object sender, MouseEventArgs e)
		{
			if (!loaded) return;
			Point chunkpoint = new Point(((int)(e.X / ZoomLevel) + foregroundPanel.HScrollValue) / 8, ((int)(e.Y / ZoomLevel) + foregroundPanel.VScrollValue) / 8);
			if (chunkpoint.X >= planemap.GetLength(0) | chunkpoint.Y >= planemap.GetLength(1)) return;
			switch (e.Button)
			{
				case MouseButtons.Left:
					FGSelection = Rectangle.Empty;
					planemap[chunkpoint.X, chunkpoint.Y] = copiedTile.Clone();
					DrawLevel();
					break;
				case MouseButtons.Right:
					menuLoc = chunkpoint;
					if (!FGSelection.Contains(chunkpoint))
					{
						FGSelection = Rectangle.Empty;
						DrawLevel();
					}
					lastmouse = new Point((int)(e.X / ZoomLevel) + foregroundPanel.HScrollValue, (int)(e.Y / ZoomLevel) + foregroundPanel.VScrollValue);
					break;
			}
		}

		private void foregroundPanel_MouseMove(object sender, MouseEventArgs e)
		{
			if (!loaded) return;
			if (e.X < 0 || e.Y < 0 || e.X > foregroundPanel.PanelWidth || e.Y > foregroundPanel.PanelHeight) return;
			Point mouse = new Point((int)(e.X / ZoomLevel) + foregroundPanel.HScrollValue, (int)(e.Y / ZoomLevel) + foregroundPanel.VScrollValue);
			Point chunkpoint = new Point(mouse.X / 8, mouse.Y / 8);
			if (chunkpoint.X >= planemap.GetLength(0) | chunkpoint.Y >= planemap.GetLength(1)) return;
			switch (e.Button)
			{
				case MouseButtons.Left:
					planemap[chunkpoint.X, chunkpoint.Y] = copiedTile.Clone();
					DrawLevel();
					break;
				case MouseButtons.Right:
					if (!selecting)
						if (Math.Sqrt(Math.Pow(e.X - lastmouse.X, 2) + Math.Pow(e.Y - lastmouse.Y, 2)) > 5)
							selecting = true;
						else
							break;
					if (FGSelection.IsEmpty)
						FGSelection = new Rectangle(chunkpoint, new Size(1, 1));
					else
					{
						int l = Math.Min(FGSelection.Left, chunkpoint.X);
						int t = Math.Min(FGSelection.Top, chunkpoint.Y);
						int r = Math.Max(FGSelection.Right, chunkpoint.X + 1);
						int b = Math.Max(FGSelection.Bottom, chunkpoint.Y + 1);
						if (FGSelection.Width > 1 && lastchunkpoint.X == l && chunkpoint.X > lastchunkpoint.X)
							l = chunkpoint.X;
						if (FGSelection.Height > 1 && lastchunkpoint.Y == t && chunkpoint.Y > lastchunkpoint.Y)
							t = chunkpoint.Y;
						if (FGSelection.Width > 1 && lastchunkpoint.X == r - 1 && chunkpoint.X < lastchunkpoint.X)
							r = chunkpoint.X + 1;
						if (FGSelection.Height > 1 && lastchunkpoint.Y == b - 1 && chunkpoint.Y < lastchunkpoint.Y)
							b = chunkpoint.Y + 1;
						FGSelection = Rectangle.FromLTRB(l, t, r, b);
					}
					DrawLevel();
					break;
				default:
					if (chunkpoint != lastchunkpoint)
						DrawLevel();
					break;
			}
			lastchunkpoint = chunkpoint;
		}

		private void foregroundPanel_MouseUp(object sender, MouseEventArgs e)
		{
			switch (e.Button)
			{
				case MouseButtons.Right:
					Point mouse = new Point((int)(e.X / ZoomLevel) + foregroundPanel.HScrollValue, (int)(e.Y / ZoomLevel) + foregroundPanel.VScrollValue);
					Point chunkpoint = new Point(mouse.X / 8, mouse.Y / 8);
					if (chunkpoint.X < 0 || chunkpoint.Y < 0 || chunkpoint.X >= planemap.GetLength(0) || chunkpoint.Y >= planemap.GetLength(1)) return;
					if (FGSelection.IsEmpty)
					{
						SelectedTile = planemap[chunkpoint.X, chunkpoint.Y].Tile;
						if (SelectedTile < LevelData.Tiles.Count)
							TileSelector.SelectedIndex = SelectedTile;
						copiedTile = planemap[chunkpoint.X, chunkpoint.Y].Clone();
						SetSelectedColor(new Point(SelectedColor.X, copiedTile.Palette));
						xFlip.Checked = copiedTile.XFlip;
						yFlip.Checked = copiedTile.YFlip;
						priority.Checked = copiedTile.Priority;
						DrawLevel();
					}
					else if (!selecting)
					{
						pasteOnceToolStripMenuItem.Enabled = pasteRepeatingToolStripMenuItem.Enabled = Clipboard.ContainsData(typeof(PatternIndex[,]).AssemblyQualifiedName);
						editTextToolStripMenuItem.Enabled = textMapping != null && FGSelection.Height % textMapping.Height == 0;
						layoutContextMenuStrip.Show(foregroundPanel, e.Location);
					}
					selecting = false;
					break;
			}
		}

		private void ScrollBar_ValueChanged(object sender, EventArgs e)
		{
			if (!loaded) return;
			DrawLevel();
		}

		private void panel_Resize(object sender, EventArgs e)
		{
			if (!loaded) return;
			loaded = false;
			UpdateScrollBars();
			loaded = true;
			DrawLevel();
		}

		Point menuLoc;
		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			selecting = false;
			switch (CurrentTab)
			{
				case Tab.Foreground:
					tableLayoutPanel1.Controls.Add(TileSelector, 0, 1);
					TileSelector.AllowDrop = false;
					foregroundPanel.Focus();
					break;
				case Tab.Art:
					panel1.Controls.Add(TileSelector);
					TileSelector.BringToFront();
					TileSelector.AllowDrop = enableDraggingTilesButton.Checked;
					break;
			}
			DrawLevel();
		}

		int SelectedTile;
		Point SelectedColor;

		Color[,] disppal = null;
		private void DrawPalette()
		{
			if (!loaded) return;
			Color[,] pal = disppal;
			if (pal == null)
			{
				pal = new Color[4, 16];
				for (int y = 0; y < 4; y++)
					for (int x = 0; x < 16; x++)
						pal[y, x] = LevelData.PaletteToColor(y, x, false);
			}
			for (int y = 0; y < 4; y++)
				for (int x = 0; x < 16; x++)
				{
					PalettePanelGfx.FillRectangle(new SolidBrush(pal[y, x]), x * 20, y * 20, 20, 20);
					PalettePanelGfx.DrawRectangle(Pens.White, x * 20, y * 20, 19, 19);
					if (!PalValid[LevelData.CurPal][y, x])
					{
						PalettePanelGfx.DrawLine(Pens.Red, x * 20, y * 20, x * 20 + 20, y * 20 + 20);
						PalettePanelGfx.DrawLine(Pens.Red, x * 20, y * 20 + 20, x * 20 + 20, y * 20);
					}
				}
			if (disppal == null)
				PalettePanelGfx.DrawRectangle(new Pen(Color.Yellow, 2), SelectedColor.X * 20, SelectedColor.Y * 20, 20, 20);
			else if (lastmouse.Y == SelectedColor.Y)
				PalettePanelGfx.DrawRectangle(new Pen(Color.Yellow, 2), lastmouse.X * 20, lastmouse.Y * 20, 20, 20);
			else
				PalettePanelGfx.DrawRectangle(new Pen(Color.Yellow, 2), 0, lastmouse.Y * 20, 320, 20);
		}

		private void PalettePanel_Paint(object sender, PaintEventArgs e)
		{
			DrawPalette();
		}

		int[] cols;
		private void PalettePanel_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (!loaded || e.Button != MouseButtons.Left) return;
			int line = e.Y / 20;
			int index = e.X / 20;
			if (index < 0 || index > 15 || line < 0 || line > 3) return;
			if (!PalValid[LevelData.CurPal][line, index]) return;
			SelectedColor = new Point(index, line);
			using (ColorDialog a = new ColorDialog
			{
				AllowFullOpen = true,
				AnyColor = true,
				FullOpen = true,
				Color = LevelData.PaletteToColor(line, index, false)
			})
			{
				if (cols != null)
					a.CustomColors = cols;
				if (a.ShowDialog() == DialogResult.OK)
				{
					LevelData.ColorToPalette(line, index, a.Color);
					PaletteChanged();
				}
				cols = a.CustomColors;
			}
			loaded = false;
			ushort md = LevelData.Palette[LevelData.CurPal][line, index].MDColor;
			colorRed.Value = md & 0xF;
			colorGreen.Value = (md >> 4) & 0xF;
			colorBlue.Value = (md >> 8) & 0xF;
			loaded = true;
		}

		private void color_ValueChanged(object sender, EventArgs e)
		{
			if (!loaded) return;
			LevelData.Palette[LevelData.CurPal][SelectedColor.Y, SelectedColor.X] = new SonLVLColor((ushort)((int)colorRed.Value | (int)colorGreen.Value << 4 | (int)colorBlue.Value << 8));
			PaletteChanged();
		}

		private Color[] curpal;
		private void PalettePanel_MouseDown(object sender, MouseEventArgs e)
		{
			if (!loaded) return;
			Point mouseColor = new Point(e.X / 20, e.Y / 20);
			if (mouseColor.X < 0 || mouseColor.X > 15 || mouseColor.Y < 0 || mouseColor.Y > 3) return;
			if (mouseColor == SelectedColor) return;
			if (!PalValid[LevelData.CurPal][mouseColor.Y, mouseColor.X]) return;
			bool newpal = mouseColor.Y != SelectedColor.Y;
			switch (e.Button)
			{
				case MouseButtons.Left:
					SetSelectedColor(mouseColor);
					if (newpal)
					{
						curpal = new Color[16];
						for (int i = 0; i < 16; i++)
							curpal[i] = LevelData.PaletteToColor(SelectedColor.Y, i, false);
						DrawTilePicture();
						RefreshTileSelector();
					}
					break;
				case MouseButtons.Right:
					if (!newpal)
					{
						int start = Math.Min(SelectedColor.X, mouseColor.X);
						int end = Math.Max(SelectedColor.X, mouseColor.X);
						if (end - start == 1) return;
						Color startcol = LevelData.PaletteToColor(SelectedColor.Y, start, false);
						Color endcol = LevelData.PaletteToColor(SelectedColor.Y, end, false);
						double r = startcol.R;
						double g = startcol.G;
						double b = startcol.B;
						double radd = (endcol.R - startcol.R) / (double)(end - start);
						double gadd = (endcol.G - startcol.G) / (double)(end - start);
						double badd = (endcol.B - startcol.B) / (double)(end - start);
						for (int x = start + 1; x < end; x++)
						{
							r += radd;
							g += gadd;
							b += badd;
							LevelData.ColorToPalette(SelectedColor.Y, x, Color.FromArgb((int)Math.Round(r, MidpointRounding.AwayFromZero), (int)Math.Round(g, MidpointRounding.AwayFromZero), (int)Math.Round(b, MidpointRounding.AwayFromZero)));
						}
						PaletteChanged();
					}
					break;
			}
		}

		private void SetSelectedColor(Point color)
		{
			SelectedColor = color;
			lastmouse = color;
			DrawPalette();
			loaded = false;
			ushort md = LevelData.Palette[LevelData.CurPal][SelectedColor.Y, SelectedColor.X].MDColor;
			colorRed.Value = md & 0xF;
			colorGreen.Value = (md >> 4) & 0xF;
			colorBlue.Value = (md >> 8) & 0xF;
			loaded = true;
			copiedTile.Palette = (byte)color.Y;
		}

		private void PalettePanel_MouseMove(object sender, MouseEventArgs e)
		{
			if (!loaded || e.Button != MouseButtons.Left || !enableDraggingPaletteButton.Checked) return;
			Point mouseColor = new Point(e.X / 20, e.Y / 20);
			if (mouseColor == lastmouse) return;
			if (mouseColor == SelectedColor)
			{
				disppal = null;
				lastmouse = mouseColor;
				DrawPalette();
			}
			if (mouseColor.X < 0 || mouseColor.Y < 0 || mouseColor.X > 15 || mouseColor.Y > 3) return;
			List<List<Point>> palidxs = new List<List<Point>>();
			for (int y = 0; y < 4; y++)
			{
				List<Point> l = new List<Point>();
				for (int x = 0; x < 16; x++)
					l.Add(new Point(x, y));
				palidxs.Add(l);
			}
			if (mouseColor.Y != SelectedColor.Y)
			{
				if (mouseColor.Y == lastmouse.Y)
				{
					lastmouse = mouseColor;
					return;
				}
				if ((ModifierKeys & Keys.Control) == Keys.Control)
					palidxs.Swap(SelectedColor.Y, mouseColor.Y);
				else
					palidxs.Move(SelectedColor.Y, mouseColor.Y > SelectedColor.Y ? mouseColor.Y + 1 : mouseColor.Y);
			}
			else
			{
				if ((ModifierKeys & Keys.Control) == Keys.Control)
					palidxs[mouseColor.Y].Swap(SelectedColor.X, mouseColor.X);
				else
					palidxs[mouseColor.Y].Move(SelectedColor.X, mouseColor.X > SelectedColor.X ? mouseColor.X + 1 : mouseColor.X);
			}
			disppal = new Color[4, 16];
			for (int y = 0; y < 4; y++)
				for (int x = 0; x < 16; x++)
					disppal[y, x] = LevelData.PaletteToColor(palidxs[y][x].Y, palidxs[y][x].X, false);
			lastmouse = mouseColor;
			DrawPalette();
		}

		private void PalettePanel_MouseUp(object sender, MouseEventArgs e)
		{
			if (!loaded || e.Button != MouseButtons.Left || !enableDraggingPaletteButton.Checked) return;
			Point mouseColor = lastmouse;
			if (mouseColor == SelectedColor) return;
			if (mouseColor.X < 0 || mouseColor.Y < 0 || mouseColor.X > 15 || mouseColor.Y > 3) return;
			disppal = null;
			List<List<Point>> palidxs = new List<List<Point>>();
			for (int y = 0; y < 4; y++)
			{
				List<Point> l = new List<Point>();
				for (int x = 0; x < 16; x++)
					l.Add(new Point(x, y));
				palidxs.Add(l);
			}
			if (mouseColor.Y != SelectedColor.Y)
			{
				if ((ModifierKeys & Keys.Control) == Keys.Control)
					palidxs.Swap(SelectedColor.Y, mouseColor.Y);
				else
					palidxs.Move(SelectedColor.Y, mouseColor.Y > SelectedColor.Y ? mouseColor.Y + 1 : mouseColor.Y);
				for (int y = 0; y < planemap.GetLength(1); y++)
					for (int x = 0; x < planemap.GetLength(0); x++)
						planemap[x, y].Palette = (byte)palidxs.FindIndex(a => a[0].Y == planemap[x, y].Palette);
			}
			else
			{
				if ((ModifierKeys & Keys.Control) == Keys.Control)
					palidxs[mouseColor.Y].Swap(SelectedColor.X, mouseColor.X);
				else
					palidxs[mouseColor.Y].Move(SelectedColor.X, mouseColor.X > SelectedColor.X ? mouseColor.X + 1 : mouseColor.X);
				List<int> tiles = new List<int>();
				for (int y = 0; y < planemap.GetLength(1); y++)
					for (int x = 0; x < planemap.GetLength(0); x++)
						if (planemap[x, y].Palette == mouseColor.Y && !tiles.Contains(planemap[x, y].Tile))
						{
							int t = planemap[x, y].Tile;
							byte[] til = LevelData.Tiles[t];
							if (til != null)
							{
								BitmapBits bmp = BitmapBits.FromTile(til, 0);
								for (int i = 0; i < bmp.Bits.Length; i++)
									bmp.Bits[i] = (byte)palidxs[mouseColor.Y].FindIndex((a) => a.X == bmp.Bits[i]);
								LevelData.Tiles[t] = bmp.ToTile();
							}
							tiles.Add(t);
						}
				if (tiles.Count > 0)
					LevelData.UpdateTileArray();
			}
			Color[,] newpal = new Color[4, 16];
			for (int y = 0; y < 4; y++)
				for (int x = 0; x < 16; x++)
					newpal[y, x] = LevelData.PaletteToColor(palidxs[y][x].Y, palidxs[y][x].X, false);
			for (int y = 0; y < 4; y++)
				for (int x = 0; x < 16; x++)
					LevelData.ColorToPalette(y, x, newpal[y, x]);
			SelectedColor = mouseColor;
			PaletteChanged();
		}

		private void importPaletteToolStripButton_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog a = new OpenFileDialog())
			{
				a.DefaultExt = "bin";
				a.Filter = "MD Palettes|*.bin|Image Files|*.bmp;*.png;*.jpg;*.gif";
				a.RestoreDirectory = true;
				if (a.ShowDialog(this) == DialogResult.OK)
				{
					int l = SelectedColor.Y;
					int x = SelectedColor.X;
					switch (Path.GetExtension(a.FileName))
					{
						case ".bin":
							SonLVLColor[] colors = SonLVLColor.Load(a.FileName, level.PaletteFormat);
							for (int i = 0; i < colors.Length; i++)
							{
								LevelData.Palette[LevelData.CurPal][l, x] = colors[i];
								x++;
								if (x == 16)
								{
									x = 0;
									l++;
									if (l == 4)
										break;
								}
							}
							break;
						case ".bmp":
						case ".png":
						case ".jpg":
						case ".gif":
							using (Bitmap bmp = new Bitmap(a.FileName))
							{
								if ((bmp.PixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed)
								{
									Color[] pal = bmp.Palette.Entries;
									for (int i = 0; i < pal.Length; i++)
									{
										LevelData.ColorToPalette(l, x++, pal[i]);
										if (x == 16)
										{
											x = 0;
											l++;
											if (l == 4)
												break;
										}
									}
								}
								else
									for (int y = 0; y < bmp.Height; y += 8)
									{
										for (int ix = 0; ix < bmp.Width; ix += 8)
										{
											LevelData.ColorToPalette(l, x++, bmp.GetPixel(ix, y));
											if (x == 16)
											{
												x = 0;
												l++;
												if (l == 4)
													break;
											}
										}
										if (l == 4)
											break;
									}
							}
							break;
					}
				}
			}
			PaletteChanged();
		}

		private BitmapBits tile;
		private PatternIndex copiedTile = new PatternIndex();
		private void TileSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			importTilesToolStripButton.Enabled = LevelData.Tiles.Count < 0x800;
			drawTileToolStripButton.Enabled = importTilesToolStripButton.Enabled;
			if (TileSelector.SelectedIndex > -1)
			{
				rotateTileRightButton.Enabled = flipTileHButton.Enabled = flipTileVButton.Enabled = true;
				SelectedTile = TileSelector.SelectedIndex;
				tile = BitmapBits.FromTile(LevelData.Tiles[SelectedTile], 0);
				TileID.Value = SelectedTile;
				TileCount.Text = LevelData.Tiles.Count.ToString("X") + " / 800";
				DrawTilePicture();
				copiedTile.Tile = (ushort)SelectedTile;
			}
			else
				rotateTileRightButton.Enabled = flipTileHButton.Enabled = flipTileVButton.Enabled = false;
		}

		private void TilePicture_Paint(object sender, PaintEventArgs e)
		{
			DrawTilePicture();
		}

		private void DrawTilePicture()
		{
			if (TileSelector.SelectedIndex == -1) return;
			using (Graphics gfx = TilePicture.CreateGraphics())
			{
				gfx.SetOptions();
				gfx.DrawImage(tile.Scale(16).ToBitmap(curpal), 0, 0, TilePicture.Width, TilePicture.Height);
			}
		}

		private void TilePicture_MouseDown(object sender, MouseEventArgs e)
		{
			if (TileSelector.SelectedIndex == -1) return;
			if (e.Button == MouseButtons.Left)
			{
				tile[e.X / 16, e.Y / 16] = (byte)SelectedColor.X;
				DrawTilePicture();
			}
			else if (e.Button == MouseButtons.Right)
				SetSelectedColor(new Point(tile[e.X / 16, e.Y / 16], SelectedColor.Y));
		}

		private void TilePicture_MouseMove(object sender, MouseEventArgs e)
		{
			if (TileSelector.SelectedIndex == -1) return;
			if (e.Button == MouseButtons.Left && new Rectangle(Point.Empty, TilePicture.Size).Contains(e.Location))
			{
				tile[e.X / 16, e.Y / 16] = (byte)SelectedColor.X;
				DrawTilePicture();
			}
		}

		private void TilePicture_MouseUp(object sender, MouseEventArgs e)
		{
			if (TileSelector.SelectedIndex == -1 || e.Button != MouseButtons.Left) return;
			LevelData.Tiles[SelectedTile] = tile.ToTile();
			LevelData.Tiles[SelectedTile].CopyTo(LevelData.TileArray, SelectedTile * 32);
			TileSelector.Images[SelectedTile] = LevelData.TileToBmp4bpp(LevelData.Tiles[SelectedTile], 0, SelectedColor.Y, false);
			TileSelector.Invalidate();
		}

		private void TileSelector_MouseDown(object sender, MouseEventArgs e)
		{
			if (!loaded) return;
			if (e.Button == MouseButtons.Right)
			{
				pasteOverToolStripMenuItem.Enabled = Clipboard.ContainsData("SonLVLTile");
				pasteBeforeToolStripMenuItem.Enabled = pasteOverToolStripMenuItem.Enabled && LevelData.Tiles.Count < 0x800;
				pasteAfterToolStripMenuItem.Enabled = pasteBeforeToolStripMenuItem.Enabled;
				insertAfterToolStripMenuItem.Enabled = LevelData.Tiles.Count < 0x800;
				insertBeforeToolStripMenuItem.Enabled = insertAfterToolStripMenuItem.Enabled;
				duplicateTilesToolStripMenuItem.Enabled = insertAfterToolStripMenuItem.Enabled;
				deleteTilesToolStripMenuItem.Enabled = TileSelector.Images.Count > 1;
				cutTilesToolStripMenuItem.Enabled = deleteTilesToolStripMenuItem.Enabled;
				tileContextMenuStrip.Show(TileSelector, e.Location);
			}
		}

		private void cutTilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Clipboard.SetData("SonLVLTile", LevelData.Tiles[SelectedTile]);
			DeleteTile();
		}

		private void DeleteTile()
		{
			LevelData.Tiles.RemoveAt(SelectedTile);
			LevelData.UpdateTileArray();
			for (int y = 0; y < planemap.GetLength(1); y++)
				for (int x = 0; x < planemap.GetLength(0); x++)
					if (planemap[x, y].Tile > SelectedTile && planemap[x, y].Tile < LevelData.Tiles.Count + 1)
						planemap[x, y].Tile--;
			TileSelector.Images.RemoveAt(SelectedTile);
			TileID.Maximum = LevelData.Tiles.Count - 1;
			TileSelector.SelectedIndex = Math.Min(TileSelector.SelectedIndex, TileSelector.Images.Count - 1);
			importTilesToolStripButton.Enabled = true;
			drawTileToolStripButton.Enabled = importTilesToolStripButton.Enabled;
			DrawLevel();
		}

		private void copyTilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Clipboard.SetData("SonLVLTile", LevelData.Tiles[SelectedTile]);
		}

		private void InsertTile()
		{
			LevelData.UpdateTileArray();
			TileSelector.Images.Insert(SelectedTile, LevelData.TileToBmp4bpp(LevelData.Tiles[SelectedTile], 0, SelectedColor.Y, false));
				for (int y = 0; y < planemap.GetLength(1); y++)
					for (int x = 0; x < planemap.GetLength(0); x++)
						if (planemap[x, y].Tile >= SelectedTile && planemap[x, y].Tile < LevelData.Tiles.Count)
							planemap[x, y].Tile++;
			TileID.Maximum = LevelData.Tiles.Count - 1;
			TileSelector.SelectedIndex = SelectedTile;
			importTilesToolStripButton.Enabled = LevelData.Tiles.Count < 0x800;
			drawTileToolStripButton.Enabled = importTilesToolStripButton.Enabled;
		}

		private void pasteBeforeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LevelData.Tiles.InsertBefore(SelectedTile, (byte[])Clipboard.GetData("SonLVLTile"));
			InsertTile();
		}

		private void pasteAfterToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LevelData.Tiles.InsertAfter(SelectedTile, (byte[])Clipboard.GetData("SonLVLTile"));
			SelectedTile++;
			InsertTile();
		}

		private void duplicateTilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LevelData.Tiles.InsertAfter(SelectedTile, (byte[])LevelData.Tiles[SelectedTile].Clone());
			SelectedTile++;
			InsertTile();
		}

		private void insertBeforeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LevelData.Tiles.InsertBefore(SelectedTile, new byte[32]);
			InsertTile();
		}

		private void insertAfterToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LevelData.Tiles.InsertAfter(SelectedTile, new byte[32]);
			SelectedTile++;
			InsertTile();
		}

		private void deleteTilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DeleteTile();
		}

		private void importToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog opendlg = new OpenFileDialog())
			{
				opendlg.DefaultExt = "png";
				opendlg.Filter = "Image Files|*.bmp;*.png;*.jpg;*.gif";
				opendlg.RestoreDirectory = true;
				if (opendlg.ShowDialog(this) == DialogResult.OK)
				{
					Bitmap bmp = new Bitmap(opendlg.FileName);
					if (bmp.Width < 8 || bmp.Height < 8)
					{
						MessageBox.Show(this, $"The image you have selected is too small ({bmp.Width}x{bmp.Height}). It must be at least as large as one tile (8x8)", "SonPLN Tile Importer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
						bmp.Dispose();
						return;
					}
					ImportImage(bmp, null, out _);
				}
			}
		}

		private bool ImportImage(Bitmap bmp, Bitmap pribmp, out PatternIndex[,] layout)
		{
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			sw.Start();
			int w = bmp.Width;
			int h = bmp.Height;
			Enabled = false;
			UseWaitCursor = true;
			Application.DoEvents();
			BitmapInfo bmpi = new BitmapInfo(bmp);
			Application.DoEvents();
			bmp.Dispose();
			bool[,] priority = new bool[w / 8, h / 8];
			if (pribmp != null)
			{
				using (pribmp)
					LevelData.GetPriMap(pribmp, priority);
				Application.DoEvents();
			}
			byte? forcepal = bmpi.PixelFormat == PixelFormat.Format1bppIndexed || bmpi.PixelFormat == PixelFormat.Format4bppIndexed ? (byte)SelectedColor.Y : (byte?)null;
			Application.DoEvents();
			List<byte[]> tiles = new List<byte[]>(LevelData.Tiles.Count);
			for (int i = 0; i < LevelData.Tiles.Count; i++)
				tiles.Add(LevelData.Tiles[i]);
			Application.DoEvents();
			object proglock = new object();
			ImportResult ir = LevelData.BitmapToTiles(bmpi, priority, forcepal, tiles, false, true);
			List<byte[]> newTiles = ir.Art;
			layout = ir.Mappings;
			if (newTiles.Count > 0 && LevelData.Tiles.Count + newTiles.Count > 0x800)
			{
				Enabled = true;
				UseWaitCursor = false;
				MessageBox.Show(this, "There are " + (LevelData.Tiles.Count + newTiles.Count - 0x800) + " tiles over the limit.\nImport cannot proceed.", "SonPLN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}
			if (newTiles.Count > 0)
			{
				foreach (byte[] t in newTiles)
					LevelData.Tiles.Add(t);
				LevelData.UpdateTileArray();
				RefreshTileSelector();
				TileID.Maximum = LevelData.Tiles.Count - 1;
				TileSelector.SelectedIndex = TileSelector.Images.Count - 1;
			}
			sw.Stop();
			System.Text.StringBuilder msg = new System.Text.StringBuilder();
			msg.AppendFormat("New tiles: {0:X}\n", newTiles.Count);
			msg.Append("\nCompleted in ");
			if (sw.Elapsed.Hours > 0)
			{
				msg.AppendFormat("{0}:{1:00}:{2:00}", sw.Elapsed.Hours, sw.Elapsed.Minutes, sw.Elapsed.Seconds);
				if (sw.Elapsed.Milliseconds > 0)
					msg.AppendFormat(".{000}", sw.Elapsed.Milliseconds);
			}
			else if (sw.Elapsed.Minutes > 0)
			{
				msg.AppendFormat("{0}:{1:00}", sw.Elapsed.Minutes, sw.Elapsed.Seconds);
				if (sw.Elapsed.Milliseconds > 0)
					msg.AppendFormat(".{000}", sw.Elapsed.Milliseconds);
			}
			else
			{
				msg.AppendFormat("{0}", sw.Elapsed.Seconds);
				if (sw.Elapsed.Milliseconds > 0)
					msg.AppendFormat(".{000}", sw.Elapsed.Milliseconds);
			}
			MessageBox.Show(this, msg.ToString(), "Import Results");
			Enabled = true;
			UseWaitCursor = false;
			return true;
		}

		private void rotateTileRightButton_Click(object sender, EventArgs e)
		{
			tile.Rotate(3);
			LevelData.Tiles[SelectedTile] = tile.ToTile();
			LevelData.Tiles[SelectedTile].CopyTo(LevelData.TileArray, SelectedTile * 32);
			TileSelector.Images[SelectedTile] = LevelData.TileToBmp4bpp(LevelData.Tiles[SelectedTile], 0, SelectedColor.Y, false);
			DrawTilePicture();
			DrawLevel();
		}

		private void drawToolStripButton_Click(object sender, EventArgs e)
		{
			using (DrawTileDialog dlg = new DrawTileDialog())
			{
				dlg.tile = new BitmapBits(8, 8);
				if (dlg.ShowDialog(this) == DialogResult.OK)
					ImportImage(dlg.tile.ToBitmap(LevelData.BmpPal), null, out _);
			}
		}

		private void TileList_KeyDown(object sender, KeyEventArgs e)
		{
			if (CurrentTab == Tab.Art)
			{
				switch (e.KeyCode)
				{
					case Keys.C:
						if (e.Control)
							copyTilesToolStripMenuItem_Click(sender, EventArgs.Empty);
						break;
					case Keys.D:
						if (e.Control && LevelData.Tiles.Count < 0x800)
							duplicateTilesToolStripMenuItem_Click(sender, EventArgs.Empty);
						break;
					case Keys.Delete:
						if (TileSelector.Images.Count > 1)
							deleteTilesToolStripMenuItem_Click(sender, EventArgs.Empty);
						break;
					case Keys.Insert:
						if (LevelData.Tiles.Count < 0x800)
							insertBeforeToolStripMenuItem_Click(sender, EventArgs.Empty);
						break;
					case Keys.V:
						if (e.Control && Clipboard.ContainsData("SonLVLTile") && LevelData.Tiles.Count < 0x800)
							pasteAfterToolStripMenuItem_Click(sender, EventArgs.Empty);
						break;
					case Keys.X:
						if (e.Control && TileSelector.Images.Count > 1)
							cutTilesToolStripMenuItem_Click(sender, EventArgs.Empty);
						break;
					case Keys.A:
						if (e.Control)
							LevelData.Tiles.InsertAfter(SelectedTile, new byte[32]);
						SelectedTile = LevelData.Tiles.Count - 1;
						InsertTile();
						break;
				}
			}
		}

		private void cutToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			PatternIndex[,] layoutsection = new PatternIndex[FGSelection.Width, FGSelection.Height];
			for (int y = 0; y < FGSelection.Height; y++)
				for (int x = 0; x < FGSelection.Width; x++)
				{
					layoutsection[x, y] = planemap[x + FGSelection.X, y + FGSelection.Y].Clone();
					planemap[x + FGSelection.X, y + FGSelection.Y] = new PatternIndex();
				}
			Clipboard.SetData(typeof(PatternIndex[,]).AssemblyQualifiedName, layoutsection);
			DrawLevel();
		}

		private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			PatternIndex[,] layoutsection = new PatternIndex[FGSelection.Width, FGSelection.Height];
			for (int y = 0; y < FGSelection.Height; y++)
				for (int x = 0; x < FGSelection.Width; x++)
					layoutsection[x, y] = planemap[x + FGSelection.X, y + FGSelection.Y].Clone();
			Clipboard.SetData(typeof(PatternIndex[,]).AssemblyQualifiedName, layoutsection);
		}

		private void pasteOnceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PatternIndex[,] section = (PatternIndex[,])Clipboard.GetData(typeof(PatternIndex[,]).AssemblyQualifiedName);
			int w = Math.Min(section.GetLength(0), planemap.GetLength(0) - menuLoc.X);
			int h = Math.Min(section.GetLength(1), planemap.GetLength(1) - menuLoc.Y);
			for (int y = 0; y < h; y++)
				for (int x = 0; x < w; x++)
					planemap[x + menuLoc.X, y + menuLoc.Y] = section[x, y].Clone();
			DrawLevel();
		}

		private void pasteRepeatingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PatternIndex[,] section = (PatternIndex[,])Clipboard.GetData(typeof(PatternIndex[,]).AssemblyQualifiedName);
			int width = section.GetLength(0);
			int height = section.GetLength(1);
			for (int y = 0; y < FGSelection.Height; y++)
				for (int x = 0; x < FGSelection.Width; x++)
					planemap[x + FGSelection.X, y + FGSelection.Y] = section[x % width, y % height].Clone();
			DrawLevel();
		}

		private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			for (int y = FGSelection.Top; y < FGSelection.Bottom; y++)
				for (int x = FGSelection.Left; x < FGSelection.Right; x++)
					planemap[x, y] = new PatternIndex();
			DrawLevel();
		}

		private void fillToolStripMenuItem_Click(object sender, EventArgs e)
		{
			for (int y = FGSelection.Top; y < FGSelection.Bottom; y++)
				for (int x = FGSelection.Left; x < FGSelection.Right; x++)
					planemap[x, y] = copiedTile.Clone();
			DrawLevel();
		}

		private void insertLayoutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (InsertDeleteDialog dlg = new InsertDeleteDialog())
			{
				dlg.Text = "Insert";
				if (dlg.ShowDialog(this) != DialogResult.OK) return;
				if (dlg.shiftH.Checked)
				{
					if (planemap.GetLength(0) < 65535)
						ResizeMap(Math.Min(65535, planemap.GetLength(0) + FGSelection.Width), planemap.GetLength(1));
					for (int y = FGSelection.Top; y < FGSelection.Bottom; y++)
						for (int x = planemap.GetLength(0) - FGSelection.Width - 1; x >= FGSelection.Left; x--)
							planemap[x + FGSelection.Width, y] = planemap[x, y];
					for (int y = FGSelection.Top; y < FGSelection.Bottom; y++)
						for (int x = FGSelection.Left; x < FGSelection.Right; x++)
							planemap[x, y] = new PatternIndex();
				}
				else if (dlg.shiftV.Checked)
				{
					if (planemap.GetLength(1) < 65535)
						ResizeMap(planemap.GetLength(0), Math.Min(65535, planemap.GetLength(1) + FGSelection.Height));
					for (int x = FGSelection.Left; x < FGSelection.Right; x++)
						for (int y = planemap.GetLength(1) - FGSelection.Height - 1; y >= FGSelection.Top; y--)
							planemap[x, y + FGSelection.Height] = planemap[x, y];
					for (int x = FGSelection.Left; x < FGSelection.Right; x++)
						for (int y = FGSelection.Top; y < FGSelection.Bottom; y++)
							planemap[x, y] = new PatternIndex();
				}
				else if (dlg.entireRow.Checked)
				{
					if (planemap.GetLength(1) < 65535)
						ResizeMap(planemap.GetLength(0), Math.Min(65535, planemap.GetLength(1) + FGSelection.Height));
					for (int x = 0; x < planemap.GetLength(0); x++)
						for (int y = planemap.GetLength(1) - FGSelection.Height - 1; y >= FGSelection.Top; y--)
							planemap[x, y + FGSelection.Height] = planemap[x, y];
					for (int x = 0; x < planemap.GetLength(0); x++)
						for (int y = FGSelection.Top; y < FGSelection.Bottom; y++)
							planemap[x, y] = new PatternIndex();
				}
				else if (dlg.entireColumn.Checked)
				{
					if (planemap.GetLength(0) < 65535)
						ResizeMap(Math.Min(65535, planemap.GetLength(0) + FGSelection.Width), planemap.GetLength(1));
					for (int y = 0; y < planemap.GetLength(1); y++)
						for (int x = planemap.GetLength(0) - FGSelection.Width - 1; x >= FGSelection.Left; x--)
							planemap[x + FGSelection.Width, y] = planemap[x, y];
					for (int y = 0; y < planemap.GetLength(1); y++)
						for (int x = FGSelection.Left; x < FGSelection.Right; x++)
							planemap[x, y] = new PatternIndex();
				}
			}
		}

		private void deleteLayoutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (InsertDeleteDialog dlg = new InsertDeleteDialog())
			{
				dlg.Text = "Delete";
				dlg.shiftH.Text = "Shift cells left";
				dlg.shiftV.Text = "Shift cells up";
				if (dlg.ShowDialog(this) != DialogResult.OK) return;
				if (dlg.shiftH.Checked)
				{
					for (int y = FGSelection.Top; y < FGSelection.Bottom; y++)
						for (int x = FGSelection.Left; x < planemap.GetLength(0) - FGSelection.Width; x++)
							planemap[x, y] = planemap[x + FGSelection.Width, y];
					for (int y = FGSelection.Top; y < FGSelection.Bottom; y++)
						for (int x = planemap.GetLength(0) - FGSelection.Width; x < planemap.GetLength(0); x++)
							planemap[x, y] = new PatternIndex();
				}
				else if (dlg.shiftV.Checked)
				{
					for (int x = FGSelection.Left; x < FGSelection.Right; x++)
						for (int y = FGSelection.Top; y < planemap.GetLength(1) - FGSelection.Height; y++)
							planemap[x, y] = planemap[x, y + FGSelection.Height];
					for (int x = FGSelection.Left; x < FGSelection.Right; x++)
						for (int y = planemap.GetLength(1) - FGSelection.Height; y < planemap.GetLength(1); y++)
							planemap[x, y] = new PatternIndex();
				}
				else if (dlg.entireRow.Checked)
				{
					for (int x = 0; x < planemap.GetLength(0); x++)
						for (int y = FGSelection.Top; y < planemap.GetLength(1) - FGSelection.Height; y++)
							planemap[x, y] = planemap[x, y + FGSelection.Height];
					for (int x = 0; x < planemap.GetLength(0); x++)
						for (int y = planemap.GetLength(1) - FGSelection.Height; y < planemap.GetLength(1); y++)
							planemap[x, y] = new PatternIndex();
					if (planemap.GetLength(1) > FGSelection.Height)
						ResizeMap(planemap.GetLength(0), planemap.GetLength(1) - FGSelection.Height);
				}
				else if (dlg.entireColumn.Checked)
				{
					for (int y = 0; y < planemap.GetLength(1); y++)
						for (int x = FGSelection.Left; x < planemap.GetLength(0) - FGSelection.Width; x++)
							planemap[x, y] = planemap[x + FGSelection.Width, y];
					for (int y = 0; y < planemap.GetLength(1); y++)
						for (int x = planemap.GetLength(0) - FGSelection.Width; x < planemap.GetLength(0); x++)
							planemap[x, y] = new PatternIndex();
					if (planemap.GetLength(0) > FGSelection.Width)
						ResizeMap(planemap.GetLength(0) - FGSelection.Width, planemap.GetLength(1));
				}
			}
		}

		private void TileSelector_ItemDrag(object sender, EventArgs e)
		{
			if (enableDraggingTilesButton.Checked)
				DoDragDrop(new DataObject("SonPLNTileIndex_" + pid, TileSelector.SelectedIndex), DragDropEffects.Move);
		}

		bool tile_dragdrop;
		int tile_dragobj;
		Point tile_dragpoint;
		private void TileSelector_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent("SonPLNTileIndex_" + pid))
			{
				e.Effect = DragDropEffects.Move;
				tile_dragdrop = true;
				tile_dragobj = (int)e.Data.GetData("SonPLNTileIndex_" + pid);
				tile_dragpoint = TileSelector.PointToClient(new Point(e.X, e.Y));
				TileSelector.Invalidate();
			}
			else
				tile_dragdrop = false;
		}

		private void TileSelector_DragOver(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent("SonPLNTileIndex_" + pid))
			{
				e.Effect = DragDropEffects.Move;
				tile_dragdrop = true;
				tile_dragobj = (int)e.Data.GetData("SonPLNTileIndex_" + pid);
				tile_dragpoint = TileSelector.PointToClient(new Point(e.X, e.Y));
				if (tile_dragpoint.Y < 8)
					TileSelector.ScrollValue -= 8 - dragpoint.Y;
				else if (dragpoint.Y > TileSelector.Height - 8)
					TileSelector.ScrollValue += dragpoint.Y - (TileSelector.Height - 8);
				TileSelector.Invalidate();
			}
			else
				tile_dragdrop = false;
		}

		private void TileSelector_DragLeave(object sender, EventArgs e)
		{
			tile_dragdrop = false;
			TileSelector.Invalidate();
		}

		private void TileSelector_Paint(object sender, PaintEventArgs e)
		{
			if (tile_dragdrop)
			{
				e.Graphics.DrawImage(TileSelector.Images[tile_dragobj], tile_dragpoint.X - (TileSelector.ImageWidth / 2),
					tile_dragpoint.Y - (TileSelector.ImageHeight / 2), TileSelector.ImageWidth, TileSelector.ImageHeight);
				Rectangle r = TileSelector.GetItemBounds(TileSelector.GetItemAtPoint(tile_dragpoint));
				if ((ModifierKeys & Keys.Control) == Keys.Control)
					e.Graphics.DrawRectangle(new Pen(Color.Black, 2), r);
				else
					e.Graphics.DrawLine(new Pen(Color.Black, 2), r.Left + 1, r.Top, r.Left + 1, r.Bottom);
			}
		}

		private void TileSelector_DragDrop(object sender, DragEventArgs e)
		{
			tile_dragdrop = false;
			if (e.Data.GetDataPresent("SonPLNTileIndex_" + pid))
			{
				Point clientPoint = TileSelector.PointToClient(new Point(e.X, e.Y));
				ushort newindex = (ushort)TileSelector.GetItemAtPoint(clientPoint);
				ushort oldindex = (ushort)(int)e.Data.GetData("SonPLNTileIndex_" + pid);
				if (newindex == oldindex) return;
				if ((ModifierKeys & Keys.Control) == Keys.Control)
				{
					if (newindex == TileSelector.Images.Count) return;
						LevelData.Tiles.Swap(oldindex, newindex);
					TileSelector.Images.Swap(oldindex, newindex);
					LevelData.UpdateTileArray();
					for (int y = 0; y < planemap.GetLength(1); y++)
						for (int x = 0; x < planemap.GetLength(0); x++)
						{
							if (planemap[x, y].Tile == newindex)
								planemap[x, y].Tile = oldindex;
							else if (planemap[x, y].Tile == oldindex)
								planemap[x, y].Tile = newindex;
						}
					TileSelector.SelectedIndex = newindex;
				}
				else
				{
					if (newindex == oldindex + 1) return;
						LevelData.Tiles.Move(oldindex, newindex);
					TileSelector.Images.Move(oldindex, newindex);
					LevelData.UpdateTileArray();
					for (int y = 0; y < planemap.GetLength(1); y++)
						for (int x = 0; x < planemap.GetLength(0); x++)
						{
							ushort t = planemap[x, y].Tile;
							if (newindex > oldindex)
							{
								if (t == oldindex)
									planemap[x, y].Tile = (ushort)(newindex - 1);
								else if (t > oldindex && t < newindex)
									planemap[x, y].Tile = (ushort)(t - 1);
							}
							else
							{
								if (t == oldindex)
									planemap[x, y].Tile = newindex;
								else if (t >= newindex && t < oldindex)
									planemap[x, y].Tile = (ushort)(t + 1);
							}
						}
					if (newindex > oldindex)
						TileSelector.SelectedIndex = newindex - 1;
					else
						TileSelector.SelectedIndex = newindex;
				}
			}
		}

		private void remapTilesButton_Click(object sender, EventArgs e)
		{
			using (TileRemappingDialog dlg = new TileRemappingDialog(TileSelector.Images, TileSelector.ImageWidth, TileSelector.ImageHeight))
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					List<byte[]> oldtiles = LevelData.Tiles.ToList();
					List<Bitmap> oldimages = new List<Bitmap>(TileSelector.Images);
					Dictionary<ushort, ushort> ushortdict = new Dictionary<ushort, ushort>(dlg.TileMap.Count);
					foreach (KeyValuePair<int, int> item in dlg.TileMap)
					{
						LevelData.Tiles[item.Value] = oldtiles[item.Key];
						TileSelector.Images[item.Value] = oldimages[item.Key];
						ushortdict.Add((ushort)item.Key, (ushort)item.Value);
					}
					LevelData.UpdateTileArray();
					TileSelector.ChangeSize();
					TileSelector_SelectedIndexChanged(this, EventArgs.Empty);
				}
		}

		private void flipTileHButton_Click(object sender, EventArgs e)
		{
			tile.Flip(true, false);
			LevelData.Tiles[SelectedTile] = tile.ToTile();
			LevelData.Tiles[SelectedTile].CopyTo(LevelData.TileArray, SelectedTile * 32);
			TileSelector.Images[SelectedTile] = LevelData.TileToBmp4bpp(LevelData.Tiles[SelectedTile], 0, SelectedColor.Y, false);
			DrawTilePicture();
			TileSelector.Invalidate();
		}

		private void flipTileVButton_Click(object sender, EventArgs e)
		{
			tile.Flip(false, true);
			LevelData.Tiles[SelectedTile] = tile.ToTile();
			LevelData.Tiles[SelectedTile].CopyTo(LevelData.TileArray, SelectedTile * 32);
			TileSelector.Images[SelectedTile] = LevelData.TileToBmp4bpp(LevelData.Tiles[SelectedTile], 0, SelectedColor.Y, false);
			DrawTilePicture();
			TileSelector.Invalidate();
		}

		private void pasteOverToolStripMenuItem_Click(object sender, EventArgs e)
		{
			byte[] t = (byte[])Clipboard.GetData("SonLVLTile");
			LevelData.Tiles[SelectedTile] = t;
			t.CopyTo(LevelData.TileArray, SelectedTile * 32);
			TileSelector.Images[SelectedTile] = LevelData.TileToBmp4bpp(LevelData.Tiles[SelectedTile], 0, SelectedColor.Y, false);
		}

		private void importToolStripMenuItem2_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog opendlg = new OpenFileDialog()
			{
				DefaultExt = "png",
				Filter = "Image Files|*.bmp;*.png;*.jpg;*.gif",
				RestoreDirectory = true
			})
				if (opendlg.ShowDialog(this) == DialogResult.OK)
					using (Bitmap bmp = new Bitmap(opendlg.FileName))
					{
						if (bmp.Width < 8 || bmp.Height < 8)
						{
							MessageBox.Show(this, $"The image you have selected is too small ({bmp.Width}x{bmp.Height}). It must be at least as large as one tile (8x8)", "SonPLN Mappings Importer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						Bitmap pribmp = null;
						string fmt = Path.Combine(Path.GetDirectoryName(opendlg.FileName),
							Path.GetFileNameWithoutExtension(opendlg.FileName) + "_{0}" + Path.GetExtension(opendlg.FileName));
						if (File.Exists(string.Format(fmt, "pri")))
							pribmp = new Bitmap(string.Format(fmt, "pri"));
						PatternIndex[,] section;
						if (!ImportImage(bmp, pribmp, out section))
							return;
						int w, h;
						w = Math.Min(section.GetLength(0), planemap.GetLength(0) - menuLoc.X);
						h = Math.Min(section.GetLength(1), planemap.GetLength(1) - menuLoc.Y);
						for (int y = 0; y < h; y++)
							for (int x = 0; x < w; x++)
								planemap[x + menuLoc.X, y + menuLoc.Y] = section[x, y];
					}
		}

		private void deleteUnusedTilesToolStripButton_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(this, "Are you sure you want to delete all tiles not used in these mappings?", "Delete Unused Tiles", MessageBoxButtons.OKCancel) != DialogResult.OK)
				return;
			bool[] tilesused = new bool[LevelData.Tiles.Count];
			foreach (PatternIndex pat in planemap)
				if (pat.Tile < tilesused.Length)
						tilesused[pat.Tile] = true;
			ushort c = 0;
			Dictionary<ushort, ushort> tilemap = new Dictionary<ushort, ushort>();
			for (ushort i = 0; i < tilesused.Length; i++)
				if (tilesused[i])
					tilemap[i] = c++;
			foreach (PatternIndex pat in planemap)
				if (tilemap.ContainsKey(pat.Tile))
					pat.Tile = tilemap[pat.Tile];
			int numdel = 0;
			for (int i = tilesused.Length - 1; i >= 0; i--)
			{
				if (tilesused[i]) continue;
				LevelData.Tiles.RemoveAt(i);
				numdel++;
			}
			LevelData.UpdateTileArray();
			TileID.Maximum = LevelData.Tiles.Count - 1;
			RefreshTileSelector();
			TileSelector.SelectedIndex = Math.Min(TileSelector.SelectedIndex, TileSelector.Images.Count - 1);
			MessageBox.Show(this, "Deleted " + numdel + " unused tiles.", "SonPLN");
		}

		private void clearForegroundToolStripButton_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(this, "Are you sure you want to clear the plane?", "Clear Plane", MessageBoxButtons.OKCancel) == DialogResult.OK)
			{
				for (int y = 0; y < planemap.GetLength(1); y++)
					for (int x = 0; x < planemap.GetLength(0); x++)
						planemap[x, y] = new PatternIndex();
			}
		}

		private void usageCountsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (StatisticsDialog dlg = new StatisticsDialog(planemap))
				dlg.ShowDialog(this);
		}

		private void replaceForegroundToolStripButton_Click(object sender, EventArgs e)
		{
			if (replaceTilesDialog.ShowDialog(this) == DialogResult.OK)
			{
				var list = planemap.OfType<PatternIndex>().ToList();
				ushort? tile = replaceTilesDialog.findTile.Tile;
				if (tile.HasValue)
					list = list.Where(a => a.Tile == tile.Value).ToList();
				bool? xflip = replaceTilesDialog.findTile.XFlip;
				if (xflip.HasValue)
					list = list.Where(a => a.XFlip == xflip.Value).ToList();
				bool? yflip = replaceTilesDialog.findTile.YFlip;
				if (yflip.HasValue)
					list = list.Where(a => a.YFlip = yflip.Value).ToList();
				bool? priority = replaceTilesDialog.findTile.Priority;
				if (priority.HasValue)
					list = list.Where(a => a.Priority == priority.Value).ToList();
				byte? palette = replaceTilesDialog.findTile.Palette;
				if (palette.HasValue)
					list = list.Where(a => a.Palette == palette.Value).ToList();
				tile = replaceTilesDialog.replaceTile.Tile;
				xflip = replaceTilesDialog.replaceTile.XFlip;
				yflip = replaceTilesDialog.replaceTile.YFlip;
				priority = replaceTilesDialog.replaceTile.Priority;
				palette = replaceTilesDialog.replaceTile.Palette;
				foreach (PatternIndex blk in list)
				{
					if (tile.HasValue)
						blk.Tile = tile.Value;
					if (xflip.HasValue)
						blk.XFlip = xflip.Value;
					if (yflip.HasValue)
						blk.YFlip = yflip.Value;
					if (priority.HasValue)
						blk.Priority = priority.Value;
					if (palette.HasValue)
						blk.Palette = palette.Value;
				}
				DrawLevel();
				MessageBox.Show(this, "Replaced " + list.Count + " tiles.", "SonPLN");
			}
		}

		private void importToolStripButton_Click(object sender, EventArgs e)
		{
			menuLoc = new Point();
			importToolStripMenuItem2_Click(sender, e);
		}

		private void TileID_ValueChanged(object sender, EventArgs e)
		{
			TileSelector.SelectedIndex = (int)TileID.Value;
		}

		private void ExportTileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog a = new SaveFileDialog() { FileName = (useHexadecimalIndexesToolStripMenuItem.Checked ? SelectedTile.ToString("X2") : SelectedTile.ToString()) + ".png", Filter = "PNG Images|*.png" })
				if (a.ShowDialog() == DialogResult.OK)
					LevelData.TileToBmp4bpp(LevelData.Tiles[SelectedTile], 0, SelectedColor.Y, transparentBackgroundToolStripMenuItem.Checked)
						.Save(a.FileName);
		}

		private void removeDuplicateTilesToolStripButton_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(this, "Are you sure you want to remove all duplicate tiles?", "SonPLN", MessageBoxButtons.OKCancel) != DialogResult.OK)
				return;
			Dictionary<ushort, byte[]> tiles = new Dictionary<ushort, byte[]>(LevelData.Tiles.Count);
			Dictionary<ushort, PatternIndex> tileMap = new Dictionary<ushort, PatternIndex>(LevelData.Tiles.Count);
			Stack<int> deleted = new Stack<int>();
			for (int i = 0; i < LevelData.Tiles.Count; i++)
			{
				byte[] tile = LevelData.Tiles[i];
				byte[] tileh = LevelData.FlipTile(tile, true, false);
				byte[] tilev = LevelData.FlipTile(tile, false, true);
				byte[] tilehv = LevelData.FlipTile(tile, true, true);
				foreach (var item in tiles)
				{
					if (tile.FastArrayEqual(item.Value))
					{
						tileMap[(ushort)i] = new PatternIndex() { Tile = item.Key };
						deleted.Push(i);
						break;
					}
					if (tileh.FastArrayEqual(item.Value))
					{
						tileMap[(ushort)i] = new PatternIndex() { Tile = item.Key, XFlip = true };
						deleted.Push(i);
						break;
					}
					if (tilev.FastArrayEqual(item.Value))
					{
						tileMap[(ushort)i] = new PatternIndex() { Tile = item.Key, YFlip = true };
						deleted.Push(i);
						break;
					}
					if (tilehv.FastArrayEqual(item.Value))
					{
						tileMap[(ushort)i] = new PatternIndex() { Tile = item.Key, XFlip = true, YFlip = true };
						deleted.Push(i);
						break;
					}
				}
				if (!tileMap.ContainsKey((ushort)i))
				{
					tileMap[(ushort)i] = new PatternIndex() { Tile = (ushort)tiles.Count };
					tiles[(ushort)tiles.Count] = tile;
				}
			}
			if (deleted.Count > 0)
			{
				foreach (int i in deleted)
				{
					LevelData.Tiles.RemoveAt(i);
					TileSelector.Images.RemoveAt(i);
				}
				LevelData.UpdateTileArray();
				TileID.Maximum = LevelData.Tiles.Count - 1;
				TileSelector.SelectedIndex = Math.Min(TileSelector.SelectedIndex, LevelData.Tiles.Count - 1);
				foreach (PatternIndex cb in planemap)
					if (tileMap.ContainsKey(cb.Tile))
					{
						PatternIndex nb = tileMap[cb.Tile];
						cb.Tile = nb.Tile;
						cb.XFlip = !nb.XFlip;
						cb.YFlip = !nb.YFlip;
					}
				DrawLevel();
			}
			MessageBox.Show(this, "Removed " + deleted.Count + " duplicate tiles.", "SonPLN");
		}

		private void XFlip_CheckedChanged(object sender, EventArgs e)
		{
			if (loaded)
				copiedTile.XFlip = xFlip.Checked;
		}

		private void YFlip_CheckedChanged(object sender, EventArgs e)
		{
			if (loaded)
				copiedTile.YFlip = yFlip.Checked;
		}

		private void Priority_CheckedChanged(object sender, EventArgs e)
		{
			if (loaded)
				copiedTile.Priority = priority.Checked;
		}

		private void MirrorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PatternIndex[,] res = (PatternIndex[,])planemap.Clone();
			for (int y = 0; y < FGSelection.Height; y++)
				for (int x = 0; x < FGSelection.Width; x++)
				{
					PatternIndex tmp = planemap[x + FGSelection.Left, y + FGSelection.Top];
					tmp.XFlip = !tmp.XFlip;
					res[FGSelection.Width - 1 - x + FGSelection.Left, y + FGSelection.Top] = tmp;
				}
			planemap = res;
		}

		private void FlipToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PatternIndex[,] res = (PatternIndex[,])planemap.Clone();
			for (int y = 0; y < FGSelection.Height; y++)
				for (int x = 0; x < FGSelection.Width; x++)
				{
					PatternIndex tmp = planemap[x + FGSelection.Left, y + FGSelection.Top];
					tmp.YFlip = !tmp.YFlip;
					res[x + FGSelection.Left, FGSelection.Height - 1 - y + FGSelection.Top] = tmp;
				}
			planemap = res;
		}

		private void TogglePriorityToolStripMenuItem_Click(object sender, EventArgs e)
		{
			for (int y = 0; y < FGSelection.Height; y++)
				for (int x = 0; x < FGSelection.Width; x++)
				{
					bool tmp = planemap[x + FGSelection.Left, y + FGSelection.Top].Priority;
					planemap[x + FGSelection.Left, y + FGSelection.Top].Priority = !tmp;
				}
		}

		private void CyclePaletteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			for (int y = 0; y < FGSelection.Height; y++)
				for (int x = 0; x < FGSelection.Width; x++)
					planemap[x + FGSelection.Left, y + FGSelection.Top].Palette++;
		}

		private void EditTextToolStripMenuItem_Click(object sender, EventArgs e)
		{
			bool unmapped = false;
			int[] pals = new int[4];
			string[] strs = new string[FGSelection.Height / textMapping.Height];
			for (int y = 0; y < FGSelection.Height; y += textMapping.Height)
			{
				StringBuilder sb = new StringBuilder(FGSelection.Width / textMapping.DefaultWidth);
				for (int x = 0; x < FGSelection.Width; )
				{
					KeyValuePair<char, CharMapInfo>? found = null;
					foreach (KeyValuePair<char, CharMapInfo> cm in textMapping.Characters.Where(a => (a.Value.Width ?? textMapping.DefaultWidth) <= FGSelection.Width - x))
					{
						for (int y2 = 0; y2 < textMapping.Height; y2++)
							for (int x2 = 0; x2 < (cm.Value.Width ?? textMapping.DefaultWidth); x2++)
								if (planemap[x + FGSelection.Left + x2, y + FGSelection.Top + y2].Tile != cm.Value.Map[x2, y2])
									goto next;
						found = cm;
						break;
						next:;
					}
					if (!found.HasValue)
					{
						unmapped = true;
						sb.Append(' ');
						x++;
					}
					else
					{
						sb.Append(found.Value.Key);
						if (found.Value.Key != ' ')
							for (int y2 = 0; y2 < textMapping.Height; y2++)
								for (int x2 = 0; x2 < (found.Value.Value.Width ?? textMapping.DefaultWidth); x2++)
									pals[planemap[x + FGSelection.Left + x2, y + FGSelection.Top + y2].Palette]++;
						x += found.Value.Value.Width ?? textMapping.DefaultWidth;
					}
				}
				strs[y / textMapping.Height] = sb.ToString();
			}
			if (unmapped && MessageBox.Show(this, "Selection contains tiles that aren't mapped to characters. These tiles will be converted to spaces.", "SonPLN", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
				return;
			int pal = 0;
			for (int i = 1; i < 4; i++)
				if (pals[i] > pals[pal])
					pal = i;
			using (TextDialog dlg = new TextDialog(textMapping, FGSelection.Width, strs, pal))
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					byte pal2 = dlg.Palette;
					int y = FGSelection.Top;
					for (int l = 0; l < dlg.Lines.Length; l++)
					{
						int x = FGSelection.Left;
						for (int i = 0; i < dlg.Lines[l].Length; i++)
						{
							CharMapInfo cm = textMapping.Characters[dlg.Lines[l][i]];
							int w = cm.Width ?? textMapping.DefaultWidth;
							for (int y2 = 0; y2 < textMapping.Height; y2++)
								for (int x2 = 0; x2 < w; x2++)
									planemap[x + x2, y+y2] = new PatternIndex(cm.Map[x2, y2], false, false, pal2, false);
							x += w;
						}
						y += textMapping.Height;
					}
					DrawLevel();
				}
		}

		private void importOverToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog opendlg = new OpenFileDialog())
			{
				opendlg.DefaultExt = "png";
				opendlg.Filter = "Image Files|*.bmp;*.png;*.jpg;*.gif";
				opendlg.RestoreDirectory = true;
				if (opendlg.ShowDialog(this) == DialogResult.OK)
				{
					BitmapInfo bmpi;
					using (Bitmap bmp = new Bitmap(opendlg.FileName))
						bmpi = new BitmapInfo(bmp);
							if (bmpi.Width < 8 || bmpi.Height < 8)
							{
								MessageBox.Show(this, "Image must be at least 8x8 to import tile.", "SonPLN");
								return;
							}
					ImportResult res = LevelData.BitmapToTiles(bmpi, new bool[bmpi.Width / 8, bmpi.Height / 8], null, new List<byte[]>(), false, false, Application.DoEvents);
					List<int> editedTiles = new List<int>();
							LevelData.Tiles[SelectedTile] = res.Art[res.Mappings[0, 0].Tile];
							editedTiles.Add(SelectedTile);
					LevelData.UpdateTileArray();
					RefreshTileSelector();
					TileSelector.Invalidate();
					if (editedTiles.Contains(SelectedTile))
						TileSelector_SelectedIndexChanged(this, EventArgs.Empty);
					DrawLevel();
				}
			}
		}
	}
}
