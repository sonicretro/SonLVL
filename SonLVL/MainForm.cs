using SonicRetro.SonLVL.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using ChaotixObjectEntry = SonicRetro.SonLVL.API.Chaotix.ChaotixObjectEntry;
using S1ObjectEntry = SonicRetro.SonLVL.API.S1.S1ObjectEntry;
using S2ObjectEntry = SonicRetro.SonLVL.API.S2.S2ObjectEntry;
using SCDObjectEntry = SonicRetro.SonLVL.API.SCD.SCDObjectEntry;

namespace SonicRetro.SonLVL.GUI
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
			LevelData.PaletteChangedEvent += LevelData_PaletteChangedEvent;
			InitializeComponent();
			if (Program.IsMonoRuntime)
			{
				BlockCollision1.TextChanged += BlockCollision1_TextChanged;
				BlockCollision2.TextChanged += BlockCollision2_TextChanged;
				ColAngle.TextChanged += ColAngle_TextChanged;
			}
		}

		void LevelData_PaletteChangedEvent()
		{
			for (int i = 0; i < 64; i++)
				LevelImgPalette.Entries[i] = LevelData.PaletteToColor(i / 16, i % 16, false);
			if (waterPalette != -1)
			{
				LevelImgPalette.Entries[64] = LevelData.Palette[waterPalette][2, 0].RGBColor;
				for (int i = 65; i < 128; i++)
					LevelImgPalette.Entries[i] = LevelData.Palette[waterPalette][(i - 64) / 16, i % 16].RGBColor;
			}
			LevelImgPalette.Entries[LevelData.ColorTransparent] = LevelData.PaletteToColor(2, 0, false);
			LevelImgPalette.Entries[LevelData.ColorWhite] = Color.White;
			LevelImgPalette.Entries[LevelData.ColorYellow] = Color.Yellow;
			LevelImgPalette.Entries[LevelData.ColorBlack] = Color.Black;
			LevelImgPalette.Entries[131] = Settings.GridColor;
			DrawChunkPicture();
			chunkBlockEditor.Invalidate();
			DrawBlockPicture();
			blockTileEditor.SelectedObjects = blockTileEditor.SelectedObjects;
			curpal = new Color[16];
			for (int i = 0; i < 16; i++)
				curpal[i] = LevelData.PaletteToColor(SelectedColor.Y, i, false);
			PalettePanel.Invalidate();
			TilePicture.Invalidate();
			RefreshTileSelector();
			TileSelector.Invalidate();
		}

		void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			Log(e.Exception.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
			File.WriteAllLines("SonLVL.log", LogFile.ToArray());
			using (ErrorDialog ed = new ErrorDialog("Unhandled Exception " + e.Exception.GetType().Name + "\nLog file has been saved.\n\nDo you want to try to continue running?", true))
			{
				if (ed.ShowDialog(this) == System.Windows.Forms.DialogResult.Cancel)
					Close();
			}
		}

		ImageAttributes imageTransparency = new ImageAttributes();
		Bitmap LevelBmp;
		Graphics LevelGfx, Panel1Gfx, Panel2Gfx, Panel3Gfx;
		internal bool loaded;
		internal byte SelectedChunk;
		internal List<Entry> SelectedItems;
		ObjectList ObjectSelect;
		Rectangle FGSelection, BGSelection;
		internal ColorPalette LevelImgPalette;
		double ZoomLevel = 1;
		byte ObjGrid = 0;
		bool objdrag = false;
		bool dragdrop = false;
		byte dragobj;
		Point dragpoint;
		bool selecting = false;
		Point selpoint;
		List<Point> locs = new List<Point>();
		List<byte> tiles = new List<byte>();
		Point lastchunkpoint;
		Point lastmouse;
		Stack<UndoAction> UndoList;
		Stack<UndoAction> RedoList;
		internal LogWindow LogWindow;
		internal List<string> LogFile = new List<string>();
		Dictionary<string, ToolStripMenuItem> levelMenuItems;
		Dictionary<char, BitmapBits> HUDLetters, HUDNumbers;
		FindObjectsDialog findObjectsDialog;
		FindChunksDialog findFGChunksDialog;
		FindChunksDialog findBGChunksDialog;
		ReplaceChunksDialog replaceFGChunksDialog;
		ReplaceChunksDialog replaceBGChunksDialog;
		ReplaceChunkBlocksDialog replaceChunkBlocksDialog;
		ReplaceBlockTilesDialog replaceBlockTilesDialog;
		List<LayoutSection> savedLayoutSections;
		List<Bitmap> savedLayoutSectionImages;
		int waterPalette;
		ushort waterHeight = 0x600;
		MouseButtons chunkblockMouseDraw = MouseButtons.Left;
		MouseButtons chunkblockMouseSelect = MouseButtons.Right;

		internal void Log(params string[] lines)
		{
			LogFile.AddRange(lines);
			if (LogWindow != null)
				LogWindow.Invoke(new MethodInvoker(LogWindow.UpdateLines));
		}

		Tab CurrentTab
		{
			get { return (Tab)tabControl1.SelectedIndex; }
			set { tabControl1.SelectedIndex = (int)value; }
		}

		ArtTab CurrentArtTab
		{
			get { return (ArtTab)tabControl4.SelectedIndex; }
			set { tabControl4.SelectedIndex = (int)value; }
		}

		private class UpdateInfo
		{
			[IniName("revision")]
			public int Revision { get; set; }
			[IniName("description")]
			public string Description { get; set; }
			[IniName("file")]
			public string File { get; set; }
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			if (File.Exists("SonLVL Updater.exe"))
			{
				Dictionary<string, int> downloaded;
				Dictionary<string, UpdateInfo> updini;
				if (File.Exists("Updater.ini"))
					downloaded = IniSerializer.Deserialize<Dictionary<string, int>>("Updater.ini");
				else
					downloaded = new Dictionary<string, int>();
#if !DEBUG
				try
				{
#endif
					using (System.Net.WebClient cli = new System.Net.WebClient())
					{
						string updatefile = Path.GetTempFileName();
						cli.DownloadFile("http://mm.reimuhakurei.net/SonLVL/update.ini", updatefile);
						updini = IniSerializer.Deserialize<Dictionary<string, UpdateInfo>>(updatefile);
						File.Delete(updatefile);
					}
					List<string> updates = new List<string>();
					foreach (KeyValuePair<string, UpdateInfo> item in updini)
					{
						if (downloaded.ContainsKey(item.Key))
							if (downloaded[item.Key] < item.Value.Revision)
								updates.Add(item.Key);
					}
					if (updates.Count > 0)
					{
						List<string> message = new List<string>();
						message.Add("The following components have updates available:");
						foreach (string item in updates)
							message.Add('\t' + item);
						message.Add(string.Empty);
						message.Add("Do you want to run the updater?");
						if (MessageBox.Show(this, string.Join(Environment.NewLine, message.ToArray()), "SonLVL Updates", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == System.Windows.Forms.DialogResult.Yes)
						{
							System.Diagnostics.Process.Start("SonLVL Updater.exe");
							Close();
							return;
						}
					}
#if !DEBUG
				}
				catch (Exception ex)
				{
					MessageBox.Show(this, ex.ToString(), "SonLVL Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
#endif
			}
			Settings = Settings.Load();
			System.Drawing.Imaging.ColorMatrix x = new System.Drawing.Imaging.ColorMatrix();
			x.Matrix33 = 0.75f;
			imageTransparency.SetColorMatrix(x, System.Drawing.Imaging.ColorMatrixFlag.Default, System.Drawing.Imaging.ColorAdjustType.Bitmap);
			string HUDpath = Path.Combine(Application.StartupPath, "HUD");
			HUDLetters = new Dictionary<char, BitmapBits>();
			Dictionary<char, string> huditems = IniSerializer.Deserialize<Dictionary<char, string>>(Path.Combine(HUDpath, "HUD.ini"));
			foreach (KeyValuePair<char, string> item in huditems)
			{
				BitmapBits bmp = new BitmapBits(Path.Combine(HUDpath, item.Value + ".png"));
				if (bmp.OriginalFormat != PixelFormat.Format8bppIndexed)
					bmp.IncrementIndexes(LevelData.ColorWhite - 1);
				HUDLetters.Add(item.Key, bmp);
			}
			HUDNumbers = new Dictionary<char, BitmapBits>();
			huditems = IniSerializer.Deserialize<Dictionary<char, string>>(Path.Combine(HUDpath, "HUDnum.ini"));
			foreach (KeyValuePair<char, string> item in huditems)
			{
				BitmapBits bmp = new BitmapBits(Path.Combine(HUDpath, item.Value + ".png"));
				if (bmp.OriginalFormat != PixelFormat.Format8bppIndexed)
					bmp.IncrementIndexes(LevelData.ColorWhite - 1);
				HUDNumbers.Add(item.Key, bmp);
			}
			objectsAboveHighPlaneToolStripMenuItem.Checked = Settings.ObjectsAboveHighPlane;
			hUDToolStripMenuItem.Checked = Settings.ShowHUD;
			lowToolStripMenuItem.Checked = Settings.ViewLowPlane;
			highToolStripMenuItem.Checked = Settings.ViewHighPlane;
			switch (Settings.ViewCollision)
			{
				case CollisionPath.Path1:
					noneToolStripMenuItem1.Checked = false;
					path1ToolStripMenuItem.Checked = true;
					break;
				case CollisionPath.Path2:
					noneToolStripMenuItem1.Checked = false;
					path2ToolStripMenuItem.Checked = true;
					break;
			}
			anglesToolStripMenuItem.Checked = Settings.ViewAngles;
			if (Settings.ViewAllTimeZones)
			{
				currentOnlyToolStripMenuItem.Checked = false;
				allToolStripMenuItem.Checked = true;
			}
			enableGridToolStripMenuItem.Checked = Settings.ShowGrid;
			foreach (ToolStripMenuItem item in zoomToolStripMenuItem.DropDownItems)
				if (item.Text == Settings.ZoomLevel)
				{
					zoomToolStripMenuItem_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(item));
					break;
				}
			objGridSizeDropDownButton_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(objGridSizeDropDownButton.DropDownItems[Settings.ObjectGridSize]));
			includeObjectsWithForegroundSelectionToolStripMenuItem.Checked = Settings.IncludeObjectsInForegroundSelection;
			transparentBackFGBGToolStripMenuItem.Checked = Settings.TransparentBackFGBGExport;
			includeobjectsWithFGToolStripMenuItem.Checked = Settings.IncludeObjectsFGExport;
			hideDebugObjectsToolStripMenuItem.Checked = Settings.HideDebugObjectsExport;
			exportArtcollisionpriorityToolStripMenuItem.Checked = Settings.ExportArtCollisionPriority;
			CurrentTab = Settings.CurrentTab;
			CurrentArtTab = Settings.CurrentArtTab;
			switchMouseButtonsInChunkAndBlockEditorsToolStripMenuItem.Checked = Settings.SwitchChunkBlockMouseButtons;
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
			menuStrip1.Visible = Settings.ShowMenu;
			if (System.Diagnostics.Debugger.IsAttached)
				logToolStripMenuItem_Click(sender, e);
			if (!string.IsNullOrEmpty(Settings.Emulator))
			{
				if (File.Exists(Settings.Emulator))
					setupEmulatorToolStripMenuItem.Checked = true;
				else
					Settings.Emulator = null;
			}
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
			findObjectsDialog = new FindObjectsDialog();
			findFGChunksDialog = new FindChunksDialog();
			findBGChunksDialog = new FindChunksDialog();
			replaceFGChunksDialog = new ReplaceChunksDialog();
			replaceBGChunksDialog = new ReplaceChunksDialog();
			replaceChunkBlocksDialog = new ReplaceChunkBlocksDialog();
			replaceBlockTilesDialog = new ReplaceBlockTilesDialog();
			if (Program.Arguments.Length > 0)
				LoadINI(Program.Arguments[0]);
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
				Settings.ShowHUD = hUDToolStripMenuItem.Checked;
				if (path1ToolStripMenuItem.Checked)
					Settings.ViewCollision = CollisionPath.Path1;
				else if (path2ToolStripMenuItem.Checked)
					Settings.ViewCollision = CollisionPath.Path2;
				else
					Settings.ViewCollision = CollisionPath.None;
				Settings.ViewAngles = anglesToolStripMenuItem.Checked;
				Settings.ViewAllTimeZones = allToolStripMenuItem.Checked;
				Settings.ShowGrid = enableGridToolStripMenuItem.Checked;
				Settings.ZoomLevel = zoomToolStripMenuItem.DropDownItems.Cast<ToolStripMenuItem>().Single((a) => a.Checked).Text;
				Settings.ObjectGridSize = ObjGrid;
				Settings.IncludeObjectsInForegroundSelection = includeObjectsWithForegroundSelectionToolStripMenuItem.Checked;
				Settings.CurrentTab = CurrentTab;
				Settings.CurrentArtTab = CurrentArtTab;
				if (TopMost)
					Settings.WindowMode = WindowMode.Fullscreen;
				else if (WindowState == FormWindowState.Maximized)
					Settings.WindowMode = WindowMode.Maximized;
				else
					Settings.WindowMode = WindowMode.Normal;
				Settings.ShowMenu = menuStrip1.Visible;
				Settings.Save();
			}
		}

		private void LoadINI(string filename)
		{
			Panel1Gfx = objectPanel.CreateGraphics();
			Panel1Gfx.SetOptions();
			Panel2Gfx = foregroundPanel.CreateGraphics();
			Panel2Gfx.SetOptions();
			Panel3Gfx = backgroundPanel.CreateGraphics();
			Panel3Gfx.SetOptions();
			LevelData.LoadGame(filename);
			changeLevelToolStripMenuItem.DropDownItems.Clear();
			levelMenuItems = new Dictionary<string, ToolStripMenuItem>();
			foreach (KeyValuePair<string, LevelInfo> item in LevelData.Game.Levels)
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
					ToolStripMenuItem ts = new ToolStripMenuItem(itempath[itempath.Length - 1], null, new EventHandler(LevelToolStripMenuItem_Clicked)) { Tag = item.Key };
					levelMenuItems.Add(item.Key, ts);
					parent.DropDownItems.Add(ts);
				}
			}
			timeZoneToolStripMenuItem.Visible = false;
			switch (LevelData.Game.EngineVersion)
			{
				case EngineVersion.S1:
					Icon = Properties.Resources.gogglemon;
					break;
				case EngineVersion.SCD:
				case EngineVersion.SCDPC:
					Icon = Properties.Resources.clockmon;
					timeZoneToolStripMenuItem.Visible = true;
					break;
				case EngineVersion.S2:
				case EngineVersion.S2NA:
					Icon = Properties.Resources.Tailsmon2;
					break;
				case EngineVersion.S3K:
					Icon = Properties.Resources.watermon;
					break;
				case EngineVersion.SKC:
					Icon = Properties.Resources.lightningmon;
					break;
				default:
					throw new NotImplementedException("Game type " + LevelData.Game.EngineVersion.ToString() + " is not supported!");
			}
			Text = "SonLVL - " + LevelData.Game.EngineVersion.ToString();
			buildAndRunToolStripMenuItem.Enabled = LevelData.Game.BuildScript != null & (LevelData.Game.ROMFile != null | LevelData.Game.RunCommand != null);
			if (Settings.MRUList.Count == 0)
				recentProjectsToolStripMenuItem.DropDownItems.Remove(noneToolStripMenuItem2);
			if (Settings.MRUList.Contains(filename))
			{
				recentProjectsToolStripMenuItem.DropDownItems.RemoveAt(Settings.MRUList.IndexOf(filename));
				Settings.MRUList.Remove(filename);
			}
			Settings.MRUList.Insert(0, filename);
			recentProjectsToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem(filename));
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
			Text = "SonLVL - " + LevelData.Game.EngineVersion + " - Loading " + LevelData.Game.GetLevelInfo((string)((ToolStripMenuItem)sender).Tag).DisplayName + "...";
			LevelData.littleendian = false;
			string anipath = Path.Combine(Application.StartupPath, "loadanim");
			Dictionary<string, AnimationInfo> animini = AnimationInfo.Load(Path.Combine(anipath, "anims.ini"));
			AnimationInfo[] anims = animini.Where((item) => item.Value.Game == LevelData.Game.EngineVersion)
				.Select((item) => item.Value).ToArray();
			AnimationInfo anim = animini["Default"];
			if (anims.Length > 0)
			{
				Random r = new Random();
				anim = anims[r.Next(anims.Length)];
			}
			MultiFileIndexer<byte> tiles = new MultiFileIndexer<byte>();
			foreach (SonicRetro.SonLVL.API.FileInfo tileent in anim.Art)
			{
				byte[] tmp = Compression.Decompress(Path.Combine(anipath, tileent.Filename), anim.ArtCompression);
				LevelData.Pad(ref tmp, 32);
				tiles.AddFile(new List<byte>(tmp), tileent.Offset == -1 ? tileent.Offset : tileent.Offset / 32);
			}
			List<MappingsFrame> map;
			if (anim.MappingsFormat == MappingsFormat.Binary)
				map = MappingsFrame.Load(File.ReadAllBytes(Path.Combine(anipath, anim.MappingsFile)), anim.MappingsGame);
			else
				map = MappingsFrame.LoadASM(Path.Combine(anipath, anim.MappingsFile), anim.MappingsGame);
			List<DPLCFrame> dplc;
			if (anim.DPLCFormat == MappingsFormat.Binary)
				dplc = DPLCFrame.Load(File.ReadAllBytes(Path.Combine(anipath, anim.DPLCFile)), anim.DPLCGame);
			else
				dplc = DPLCFrame.LoadASM(Path.Combine(anipath, anim.DPLCFile), anim.DPLCGame);
			Animation ani;
			if (anim.AnimationFormat == MappingsFormat.Binary)
				ani = new Animation(File.ReadAllBytes(Path.Combine(anipath, anim.AnimationFile)), 0, "Animation");
			else
				ani = new Animation(LevelData.ASMToBin(Path.Combine(anipath, anim.AnimationFile), anim.Game), 0, "Animation");
			Color[] pal = new Color[64];
			foreach (PaletteInfo item in anim.Palette)
			{
				SonLVLColor[] c = SonLVLColor.Load(File.ReadAllBytes(Path.Combine(anipath, item.Filename)), item.Source,
					item.Length, anim.Game);
				for (int i = 0; i < item.Length; i++)
					pal[item.Destination + i] = c[i].RGBColor;
			}
			pal[0] = Color.Transparent;
			loadingAnimation1.ChangeAnimation(tiles.ToArray(), map.ToArray(), dplc.ToArray(), ani, pal);
			importProgressControl1.ChangeAnimation(tiles.ToArray(), map.ToArray(), dplc.ToArray(), ani, pal);
#if !DEBUG
			loadingAnimation1.BringToFront();
			loadingAnimation1.Show();
			initerror = null;
			backgroundLevelLoader.RunWorkerAsync(((ToolStripMenuItem)sender).Tag);
#else
			backgroundLevelLoader_DoWork(null, new DoWorkEventArgs(((ToolStripMenuItem)sender).Tag));
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
				SelectedChunk = 0;
				UndoList = new Stack<UndoAction>();
				RedoList = new Stack<UndoAction>();
				LevelData.LoadLevel((string)e.Argument, true);
				if (LevelData.Level.TwoPlayerCompatible && LevelData.Tiles.Count % 2 == 1)
				{
					LevelData.Tiles.Add(new byte[32]);
					LevelData.UpdateTileArray();
				}
				LevelImgPalette = new Bitmap(1, 1, PixelFormat.Format8bppIndexed).Palette;
				for (int i = 0; i < 64; i++)
					LevelImgPalette.Entries[i] = LevelData.PaletteToColor(i / 16, i % 16, false);
				LevelImgPalette.Entries.Fill(Color.Black, 64, 192);
				LevelImgPalette.Entries[LevelData.ColorTransparent] = LevelData.PaletteToColor(2, 0, false);
				LevelImgPalette.Entries[LevelData.ColorWhite] = Color.White;
				LevelImgPalette.Entries[LevelData.ColorYellow] = Color.Yellow;
				LevelImgPalette.Entries[LevelData.ColorBlack] = Color.Black;
				LevelImgPalette.Entries[131] = Settings.GridColor;
				curpal = new Color[16];
				for (int i = 0; i < 16; i++)
					curpal[i] = LevelData.PaletteToColor(0, i, false);
				switch (LevelData.Level.ChunkFormat)
				{
					case EngineVersion.S1:
					case EngineVersion.SCD:
					case EngineVersion.SCDPC:
						copiedChunkBlock = new S1ChunkBlock();
						break;
					case EngineVersion.S2NA:
					case EngineVersion.S2:
					case EngineVersion.S3K:
					case EngineVersion.SKC:
						copiedChunkBlock = new S2ChunkBlock();
						break;
				}
#if !DEBUG
			}
			catch (Exception ex) { initerror = ex; }
#endif
		}

		private void backgroundLevelLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (initerror != null)
			{
				Log(initerror.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
				System.IO.File.WriteAllLines("SonLVL.log", LogFile.ToArray());
				using (ErrorDialog ed = new ErrorDialog(initerror.GetType().Name + ": " + initerror.Message + "\nLog file has been saved to " + System.IO.Path.Combine(Environment.CurrentDirectory, "SonLVL.log") + ".\nSend this to MainMemory on the Sonic Retro forums.", true))
					if (ed.ShowDialog(this) == System.Windows.Forms.DialogResult.Cancel) Close();
				Text = "SonLVL - " + LevelData.Game.EngineVersion.ToString();
				Enabled = true;
				loadingAnimation1.Hide();
				return;
			}
			Log("Load completed.");
			ChunkSelector.Images = LevelData.CompChunkBmps;
			ChunkSelector.ImageWidth = LevelData.Level.ChunkWidth;
			ChunkSelector.ImageHeight = LevelData.Level.ChunkHeight;
			BlockSelector.Images = LevelData.CompBlockBmps;
			BlockSelector.ChangeSize();
			CollisionSelector.Images = new List<Bitmap>(LevelData.ColBmps);
			CollisionSelector.ChangeSize();
			ChunkSelector.SelectedIndex = 0;
			ChunkPicture.Size = new Size(LevelData.Level.ChunkWidth, LevelData.Level.ChunkHeight);
			flipChunkHButton.Enabled = flipChunkVButton.Enabled = true;
			remapChunksButton.Enabled = remapBlocksButton.Enabled = remapTilesButton.Enabled = true;
			importChunksToolStripButton.Enabled = LevelData.Chunks.Count < 256;
			drawChunkToolStripButton.Enabled = importChunksToolStripButton.Enabled;
			importBlocksToolStripButton.Enabled = LevelData.Blocks.Count < LevelData.GetBlockMax();
			drawBlockToolStripButton.Enabled = importBlocksToolStripButton.Enabled;
			importTilesToolStripButton.Enabled = LevelData.Tiles.Count < 0x800;
			drawTileToolStripButton.Enabled = importTilesToolStripButton.Enabled;
			BlockSelector.SelectedIndex = 0;
			if (LevelData.Level.TwoPlayerCompatible)
			{
				TilePicture.Height = 256;
				TileSelector.ImageHeight = 128;
				rotateTileRightButton.Visible = false;
			}
			else
			{
				TilePicture.Height = 128;
				TileSelector.ImageHeight = 64;
				rotateTileRightButton.Visible = true;
			}
			RefreshTileSelector();
			TileSelector.SelectedIndex = 0;
			TileSelector.ChangeSize();
			switch (LevelData.Level.ChunkFormat)
			{
				case EngineVersion.S1:
				case EngineVersion.SCD:
				case EngineVersion.SCDPC:
					BlockCollision2.Visible = false;
					button2.Visible = false;
					path2ToolStripMenuItem.Visible = false;
					if (path2ToolStripMenuItem.Checked)
						path2ToolStripMenuItem.Checked = !(path1ToolStripMenuItem.Checked = true);
					break;
				case EngineVersion.S2:
				case EngineVersion.S2NA:
				case EngineVersion.S3K:
				case EngineVersion.SKC:
					BlockCollision2.Visible = true;
					button2.Visible = true;
					path2ToolStripMenuItem.Visible = true;
					break;
			}
			if (ObjectSelect == null)
			{
				ObjectSelect = new ObjectList();
				ObjectSelect.listView1.SelectedIndexChanged += new EventHandler(ObjectSelect_listView1_SelectedIndexChanged);
				ObjectSelect.listView2.SelectedIndexChanged += new EventHandler(ObjectSelect_listView2_SelectedIndexChanged);
			}
			ObjectSelect.listView1.Items.Clear();
			ObjectSelect.imageList1.Images.Clear();
			objectTypeList.Items.Clear();
			objectTypeImages.Images.Clear();
			foreach (KeyValuePair<byte, ObjectDefinition> item in LevelData.ObjTypes)
			{
				Bitmap image = item.Value.Image.Image.ToBitmap(LevelData.BmpPal);
				ObjectSelect.imageList1.Images.Add(image.Resize(ObjectSelect.imageList1.ImageSize));
				ObjectSelect.listView1.Items.Add(new ListViewItem(item.Value.Name, ObjectSelect.imageList1.Images.Count - 1) { Tag = item.Key });
				objectTypeImages.Images.Add(image.Resize(objectTypeImages.ImageSize));
				objectTypeList.Items.Add(new ListViewItem(item.Value.Name, objectTypeImages.Images.Count - 1) { Tag = item.Key });
			}
			ObjectSelect.listView2.Items.Clear();
			ObjectSelect.imageList2.Images.Clear();
			ColIndBox.Visible = collisionToolStripMenuItem.Visible = LevelData.ColInds1.Count > 0;
			Text = "SonLVL - " + LevelData.Game.EngineVersion + " - " + LevelData.Level.DisplayName;
			UpdateScrollBars();
			hScrollBar1.Value = 0;
			hScrollBar1.SmallChange = 16;
			hScrollBar1.LargeChange = 128;
			vScrollBar1.Value = 0;
			vScrollBar1.SmallChange = 16;
			vScrollBar1.LargeChange = 128;
			hScrollBar1.Enabled = true;
			vScrollBar1.Enabled = true;
			hScrollBar2.Value = 0;
			hScrollBar2.SmallChange = 16;
			hScrollBar2.LargeChange = 128;
			vScrollBar2.Value = 0;
			vScrollBar2.SmallChange = 16;
			vScrollBar2.LargeChange = 128;
			hScrollBar2.Enabled = true;
			vScrollBar2.Enabled = true;
			hScrollBar3.Value = 0;
			hScrollBar3.SmallChange = 16;
			hScrollBar3.LargeChange = 128;
			vScrollBar3.Value = 0;
			vScrollBar3.SmallChange = 16;
			vScrollBar3.LargeChange = 128;
			hScrollBar3.Enabled = true;
			vScrollBar3.Enabled = true;
			switch (LevelData.Level.PaletteFormat)
			{
				case EngineVersion.SCDPC:
					colorRed.Maximum = colorGreen.Maximum = colorBlue.Maximum = 255;
					colorRed.Increment = colorGreen.Increment = colorBlue.Increment = 1;
					colorRed.Hexadecimal = colorGreen.Hexadecimal = colorBlue.Hexadecimal = false;
					break;
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
			loaded = true;
			SelectedItems = new List<Entry>();
			undoCtrlZToolStripMenuItem.DropDownItems.Clear();
			redoCtrlYToolStripMenuItem.DropDownItems.Clear();
			saveToolStripMenuItem.Enabled = true;
			editToolStripMenuItem.Enabled = true;
			exportToolStripMenuItem.Enabled = true;
			paletteToolStripMenuItem2.DropDownItems.Clear();
			foreach (string item in LevelData.PalName)
				paletteToolStripMenuItem2.DropDownItems.Add(new ToolStripMenuItem(item));
			((ToolStripMenuItem)paletteToolStripMenuItem2.DropDownItems[0]).Checked = true;
			if (LevelData.Palette.Count > 1)
			{
				blendAlternatePaletteToolStripMenuItem.Enabled = waterPaletteToolStripMenuItem.Visible = true;
				selectPaletteToolStripMenuItem.DropDownItems.Clear();
				selectPaletteToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem("None") { Checked = LevelData.Level.WaterPalette < 2, Tag = -1 });
				for (int i = 1; i < LevelData.PalName.Count; i++)
					selectPaletteToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem(LevelData.PalName[i]) { Checked = LevelData.Level.WaterPalette - 1 == i, Tag = i });
				waterPalette = LevelData.Level.WaterPalette < 2 ? -1 : LevelData.Level.WaterPalette - 1;
				waterHeight = (ushort)LevelData.Level.WaterHeight;
				if (waterPalette != -1)
				{
					LevelImgPalette.Entries[64] = LevelData.Palette[waterPalette][2, 0].RGBColor;
					for (int i = 65; i < 128; i++)
						LevelImgPalette.Entries[i] = LevelData.Palette[waterPalette][(i - 64) / 16, i % 16].RGBColor;
				}
			}
			else
			{
				blendAlternatePaletteToolStripMenuItem.Enabled = waterPaletteToolStripMenuItem.Visible = false;
				waterPalette = -1;
			}
			timeZoneToolStripMenuItem.Visible = LevelData.Level.TimeZone != API.TimeZone.None;
			findNextToolStripMenuItem.Enabled = findPreviousToolStripMenuItem.Enabled = false;
			string levelname = LevelData.Level.DisplayName;
			foreach (char c in Path.GetInvalidFileNameChars())
				levelname = levelname.Replace(c, '_');
			if (File.Exists(levelname + ".sls"))
				using (FileStream fs = File.OpenRead(levelname + ".sls"))
					savedLayoutSections = (List<LayoutSection>)new BinaryFormatter().Deserialize(fs);
			else
				savedLayoutSections = new List<LayoutSection>();
			savedLayoutSectionImages = new List<Bitmap>();
			layoutSectionListBox.Items.Clear();
			layoutSectionListBox.BeginUpdate();
			foreach (LayoutSection sec in savedLayoutSections)
			{
				layoutSectionListBox.Items.Add(sec.Name);
				savedLayoutSectionImages.Add(MakeLayoutSectionImage(sec));
			}
			layoutSectionListBox.EndUpdate();
			foundobjs = null;
			SelectedObjectChanged();
			ChunkCount.Text = LevelData.Chunks.Count.ToString("X") + " / 100";
			BlockCount.Text = LevelData.Blocks.Count.ToString("X") + " / " + LevelData.GetBlockMax().ToString("X");
			TileCount.Text = LevelData.Tiles.Count.ToString("X") + " / 800";
			deleteUnusedTilesToolStripButton.Enabled = deleteUnusedBlocksToolStripButton.Enabled = deleteUnusedChunksToolStripButton.Enabled =
				removeDuplicateTilesToolStripButton.Enabled = removeDuplicateBlocksToolStripButton.Enabled = removeDuplicateChunksToolStripButton.Enabled =
				replaceBlockTilesToolStripButton.Enabled = replaceChunkBlocksToolStripButton.Enabled = replaceBackgroundToolStripButton.Enabled = replaceForegroundToolStripButton.Enabled =
				clearBackgroundToolStripButton.Enabled = clearForegroundToolStripButton.Enabled = usageCountsToolStripMenuItem.Enabled = true;
#if !DEBUG
			loadingAnimation1.Hide();
#endif
			Enabled = true;
			UseWaitCursor = false;
			DrawLevel();
		}

		private void RefreshTileSelector()
		{
			TileSelector.Images.Clear();
			if (LevelData.Level.TwoPlayerCompatible)
				for (int i = 0; i < LevelData.Tiles.Count - 1; i += 2)
					TileSelector.Images.Add(LevelData.InterlacedTileToBmp4bpp(LevelData.TileArray, i, SelectedColor.Y));
			else
				for (int i = 0; i < LevelData.Tiles.Count; i++)
					TileSelector.Images.Add(LevelData.TileToBmp4bpp(LevelData.Tiles[i], 0, SelectedColor.Y));
		}

		private Bitmap MakeLayoutSectionImage(LayoutSection sec)
		{
			int w = sec.Layout.GetLength(0), h = sec.Layout.GetLength(1);
			BitmapBits bmp = new BitmapBits(w * LevelData.Level.ChunkWidth, h * LevelData.Level.ChunkHeight);
			for (int y = 0; y < h; y++)
				for (int x = 0; x < w; x++)
					if (sec.Layout[x, y] < LevelData.Chunks.Count)
						bmp.DrawBitmapComposited(LevelData.ChunkBmpBits[sec.Layout[x, y]][0], x * LevelData.Level.ChunkWidth, y * LevelData.Level.ChunkHeight);
			foreach (Entry ent in sec.Objects)
			{
				ent.UpdateSprite();
				if (!(ent is ObjectEntry) || LevelData.ObjectVisible((ObjectEntry)ent, allToolStripMenuItem.Checked))
					bmp.DrawSprite(ent.Sprite, 0, 0);
			}
			for (int y = 0; y < h; y++)
				for (int x = 0; x < w; x++)
					if (sec.Layout[x, y] < LevelData.Chunks.Count)
						bmp.DrawBitmapComposited(LevelData.ChunkBmpBits[sec.Layout[x, y]][1], x * LevelData.Level.ChunkWidth, y * LevelData.Level.ChunkHeight);
			return LevelData.BitmapBitsToBitmap(bmp);
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LevelData.SaveLevel();
		}

		private void buildAndRunToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(Path.Combine(Environment.CurrentDirectory, LevelData.Game.BuildScript)) { WorkingDirectory = Path.GetDirectoryName(Path.Combine(Environment.CurrentDirectory, LevelData.Game.BuildScript)) }).WaitForExit();
			string romfile = LevelData.Game.ROMFile ?? LevelData.Game.RunCommand;
			if (LevelData.Game.UseEmulator)
				if (!string.IsNullOrEmpty(Settings.Emulator))
					System.Diagnostics.Process.Start(Settings.Emulator, '"' + Path.GetFullPath(romfile) + '"');
				else
					MessageBox.Show("You must set up an emulator before you can run the ROM, use File -> Setup Emulator.");
			else
				System.Diagnostics.Process.Start(Path.GetFullPath(romfile));
		}

		private void setupEmulatorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog opn = new OpenFileDialog() { DefaultExt = "exe", Filter = "EXE Files|*.exe|All Files|*.*", RestoreDirectory = true })
			{
				if (!string.IsNullOrEmpty(Settings.Emulator))
				{
					opn.FileName = Path.GetFileName(Settings.Emulator);
					opn.InitialDirectory = Path.GetDirectoryName(Settings.Emulator);
				}
				if (opn.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					Settings.Emulator = opn.FileName;
					setupEmulatorToolStripMenuItem.Checked = true;
				}
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
		public void AddUndo(UndoAction action)
		{
			UndoList.Push(action);
			undoCtrlZToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem(action.Name));
			if (UndoList.Count > 100)
			{
				UndoAction[] l = UndoList.ToArray();
				UndoList.Clear();
				for (int i = 99; i >= 0; i--)
				{
					UndoList.Push(l[i]);
				}
				undoCtrlZToolStripMenuItem.DropDownItems.RemoveAt(100);
			}
			RedoList.Clear();
			redoCtrlYToolStripMenuItem.DropDownItems.Clear();
		}

		private void undoCtrlZToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			DoUndo(undoCtrlZToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem) + 1);
		}

		private void redoCtrlYToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			DoRedo(redoCtrlYToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem) + 1);
		}

		private void DoUndo(int num)
		{
			for (int i = 0; i < num; i++)
			{
				undoCtrlZToolStripMenuItem.DropDownItems.RemoveAt(0);
				UndoAction act = UndoList.Pop();
				act.Undo();
				RedoList.Push(act);
				redoCtrlYToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem(act.Name));
			}
			SelectedItems.Clear();
			SelectedObjectChanged();
			DrawLevel();
		}

		private void DoRedo(int num)
		{
			for (int i = 0; i < num; i++)
			{
				redoCtrlYToolStripMenuItem.DropDownItems.RemoveAt(0);
				UndoAction act = RedoList.Pop();
				act.Redo();
				UndoList.Push(act);
				undoCtrlZToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem(act.Name));
			}
			SelectedItems.Clear();
			SelectedObjectChanged();
			DrawLevel();
		}

		private void blendAlternatePaletteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (AlternatePaletteDialog dlg = new AlternatePaletteDialog())
				if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{
					int underwater = LevelData.CurPal;
					LevelData.CurPal = 0;
					Color[,] pal = new Color[4, 16];
					for (int l = 0; l < 4; l++)
					{
						bool doblend = false;
						switch (l)
						{
							case 0:
								doblend = dlg.Line1;
								break;
							case 1:
								doblend = dlg.Line2;
								break;
							case 2:
								doblend = dlg.Line3;
								break;
							case 3:
								doblend = dlg.Line4;
								break;
						}
						if (!doblend) continue;
						for (int i = 0; i < 16; i++)
						{
							Color col = LevelData.PaletteToColor(l, i, false);
							if (dlg.radioButton1.Checked)
								LevelData.Palette[dlg.paletteIndex.SelectedIndex + 1][l, i] = new SonLVLColor(col.Blend(dlg.BlendColor));
							else if (dlg.radioButton2.Checked)
								LevelData.Palette[dlg.paletteIndex.SelectedIndex + 1][l, i] = new SonLVLColor(Color.FromArgb(Math.Min(col.R + dlg.BlendColor.R, 255), Math.Min(col.G + dlg.BlendColor.G, 255), Math.Min(col.B + dlg.BlendColor.B, 255)));
							else
								LevelData.Palette[dlg.paletteIndex.SelectedIndex + 1][l, i] = new SonLVLColor(Color.FromArgb(Math.Max(col.R - dlg.BlendColor.R, 0), Math.Max(col.G - dlg.BlendColor.G, 0), Math.Max(col.B - dlg.BlendColor.B, 0)));
						}
					}
					LevelData.CurPal = underwater;
					LevelData.PaletteChanged();
				}
		}
		#endregion

		#region View Menu
		private void objectsAboveHighPlaneToolStripMenuItem_Click(object sender, EventArgs e)
		{
			objectsAboveHighPlaneToolStripMenuItem.Checked = !objectsAboveHighPlaneToolStripMenuItem.Checked;
			DrawLevel();
		}

		private void paletteToolStripMenuItem2_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			foreach (ToolStripMenuItem item in paletteToolStripMenuItem2.DropDownItems)
				item.Checked = false;
			((ToolStripMenuItem)e.ClickedItem).Checked = true;
			LevelData.CurPal = paletteToolStripMenuItem2.DropDownItems.IndexOf(e.ClickedItem);
			if (waterPalette == LevelData.CurPal)
				waterPalette = -1;
			selectPaletteToolStripMenuItem.DropDownItems.Clear();
			selectPaletteToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem("None") { Checked = waterPalette == -1, Tag = -1 });
			for (int i = 0; i < LevelData.PalName.Count; i++)
				if (i != LevelData.CurPal)
					selectPaletteToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem(LevelData.PalName[i]) { Checked = waterPalette == i, Tag = i });
			LevelData.PaletteChanged();
			DrawLevel();
		}

		private void lowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DrawLevel();
		}

		private void highToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DrawLevel();
		}

		private void collisionToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			if (collisionToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem) < 3)
			{
				bool angles = anglesToolStripMenuItem.Checked;
				foreach (ToolStripItem item in collisionToolStripMenuItem.DropDownItems)
					if (item is ToolStripMenuItem)
						((ToolStripMenuItem)item).Checked = false;
				((ToolStripMenuItem)e.ClickedItem).Checked = true;
				anglesToolStripMenuItem.Checked = angles;
				DrawLevel();
			}
		}

		private void anglesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			anglesToolStripMenuItem.Checked = !anglesToolStripMenuItem.Checked;
			DrawLevel();
		}

		private void currentOnlyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			currentOnlyToolStripMenuItem.Checked = true;
			allToolStripMenuItem.Checked = false;
			DrawLevel();
		}

		private void allToolStripMenuItem_Click(object sender, EventArgs e)
		{
			allToolStripMenuItem.Checked = true;
			currentOnlyToolStripMenuItem.Checked = false;
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
			if (a.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				Settings.GridColor = a.Color;
				if (loaded)
				{
					LevelImgPalette.Entries[131] = a.Color;
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

		private void logToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LogWindow = new LogWindow();
			LogWindow.Show(this);
			logToolStripMenuItem.Enabled = false;
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
						LevelData.TileToBmp4bpp(LevelData.Tiles[i], 0, tilesToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem))
							.Save(Path.Combine(a.SelectedPath,
							(useHexadecimalIndexesToolStripMenuItem.Checked ? i.ToString("X2") : i.ToString()) + ".png"));
		}

		private void blocksToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!highToolStripMenuItem.Checked && !lowToolStripMenuItem.Checked && !path1ToolStripMenuItem.Checked && !path2ToolStripMenuItem.Checked)
			{
				MessageBox.Show(this, "Cannot export blocks with nothing visible.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			using (FolderBrowserDialog a = new FolderBrowserDialog() { SelectedPath = Environment.CurrentDirectory })
				if (a.ShowDialog() == DialogResult.OK)
					for (int i = 0; i < LevelData.Blocks.Count; i++)
					{
						BitmapBits bits = null;
						string pathBase = Path.Combine(a.SelectedPath, useHexadecimalIndexesToolStripMenuItem.Checked ? i.ToString("X2") : i.ToString());
						if (exportArtcollisionpriorityToolStripMenuItem.Checked)
						{
							LevelData.CompBlockBmpBits[i].ToBitmap(LevelImgPalette).Save(pathBase + ".png");
							bool dualPath = false;
							switch (LevelData.Level.ChunkFormat)
							{
								case EngineVersion.S2:
								case EngineVersion.S2NA:
								case EngineVersion.S3K:
								case EngineVersion.SKC:
									dualPath = !Object.ReferenceEquals(LevelData.ColInds1, LevelData.ColInds2);
									break;
							}
							if (dualPath)
							{
								LevelData.ColBmpBits[LevelData.GetColInd1(i)].ToBitmap1bpp(Color.Black, Color.White).Save(pathBase + "_col1.png");
								LevelData.ColBmpBits[LevelData.GetColInd2(i)].ToBitmap1bpp(Color.Black, Color.White).Save(pathBase + "_col2.png");
							}
							else
								LevelData.ColBmpBits[LevelData.GetColInd1(i)].ToBitmap1bpp(Color.Black, Color.White).Save(pathBase + "_col.png");
							bits = new BitmapBits(16, 16);
							for (int y = 0; y < 2; y++)
								for (int x = 0; x < 2; x++)
									if (LevelData.Blocks[i].Tiles[x, y].Priority)
										bits.FillRectangle(1, x * 8, y * 8, 8, 8);
							bits.ToBitmap1bpp(Color.Black, Color.White).Save(pathBase + "_pri.png");
						}
						else
						{
							if (highToolStripMenuItem.Checked & lowToolStripMenuItem.Checked)
								bits = new BitmapBits(LevelData.CompBlockBmpBits[i]);
							else if (lowToolStripMenuItem.Checked)
								bits = new BitmapBits(LevelData.BlockBmpBits[i][0]);
							else if (highToolStripMenuItem.Checked)
								bits = new BitmapBits(LevelData.BlockBmpBits[i][1]);
							else
								bits = new BitmapBits(16, 16);
							if (path1ToolStripMenuItem.Checked)
							{
								BitmapBits bmp = new BitmapBits(LevelData.ColBmpBits[LevelData.GetColInd1(i)]);
								bmp.IncrementIndexes(LevelData.ColorWhite - 1);
								bits.DrawBitmapComposited(bmp, 0, 0);
							}
							else if (path2ToolStripMenuItem.Checked)
							{
								BitmapBits bmp = new BitmapBits(LevelData.ColBmpBits[LevelData.GetColInd2(i)]);
								bmp.IncrementIndexes(LevelData.ColorWhite - 1);
								bits.DrawBitmapComposited(bmp, 0, 0);
							}
							bits.ToBitmap(LevelImgPalette).Save(pathBase + ".png");
						}
					}
		}

		private void chunksToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!highToolStripMenuItem.Checked && !lowToolStripMenuItem.Checked && !path1ToolStripMenuItem.Checked && !path2ToolStripMenuItem.Checked)
			{
				MessageBox.Show(this, "Cannot export chunks with nothing visible.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			using (FolderBrowserDialog a = new FolderBrowserDialog() { SelectedPath = Environment.CurrentDirectory })
				if (a.ShowDialog() == DialogResult.OK)
					for (int i = 0; i < LevelData.Chunks.Count; i++)
					{
						BitmapBits bits = null;
						string pathBase = Path.Combine(a.SelectedPath, useHexadecimalIndexesToolStripMenuItem.Checked ? i.ToString("X2") : i.ToString());
						if (exportArtcollisionpriorityToolStripMenuItem.Checked)
						{
							LevelData.CompChunkBmpBits[i].ToBitmap(LevelImgPalette).Save(pathBase + ".png");
							bool dualPath = false;
							switch (LevelData.Level.ChunkFormat)
							{
								case EngineVersion.S2:
								case EngineVersion.S2NA:
								case EngineVersion.S3K:
								case EngineVersion.SKC:
									dualPath = !Object.ReferenceEquals(LevelData.ColInds1, LevelData.ColInds2);
									break;
							}
							if (dualPath)
							{
								bits = new BitmapBits(LevelData.ChunkColBmpBits[i][0]);
								bits.IncrementIndexes(-LevelData.ColorWhite + 1);
								bits.ToBitmap4bpp(Color.Magenta, Color.White, Color.Yellow, Color.Black).Save(pathBase + "_col1.png");
								bits = new BitmapBits(LevelData.ChunkColBmpBits[i][1]);
								bits.IncrementIndexes(-LevelData.ColorWhite + 1);
								bits.ToBitmap4bpp(Color.Magenta, Color.White, Color.Yellow, Color.Black).Save(pathBase + "_col2.png");
							}
							else
							{
								bits = new BitmapBits(LevelData.ChunkColBmpBits[i][0]);
								bits.IncrementIndexes(-LevelData.ColorWhite + 1);
								bits.ToBitmap4bpp(Color.Magenta, Color.White, Color.Yellow, Color.Black).Save(pathBase + "_col.png");
							}
							bits = new BitmapBits(LevelData.Level.ChunkWidth, LevelData.Level.ChunkHeight);
							for (int cy = 0; cy < LevelData.Level.ChunkHeight / 16; cy++)
								for (int cx = 0; cx < LevelData.Level.ChunkWidth / 16; cx++)
								{
									if (LevelData.Chunks[i].Blocks[cx, cy].Block >= LevelData.Blocks.Count) continue;
									Block blk = LevelData.Blocks[LevelData.Chunks[i].Blocks[cx, cy].Block];
									for (int by = 0; by < 2; by++)
										for (int bx = 0; bx < 2; bx++)
											if (blk.Tiles[bx, by].Priority)
												bits.FillRectangle(1, cx * 16 + bx * 8, cy * 16 + by * 8, 8, 8);
								}
							bits.ToBitmap1bpp(Color.Black, Color.White).Save(pathBase + "_pri.png");
						}
						else
						{
							if (highToolStripMenuItem.Checked & lowToolStripMenuItem.Checked)
								bits = new BitmapBits(LevelData.CompChunkBmpBits[i]);
							else if (lowToolStripMenuItem.Checked)
								bits = new BitmapBits(LevelData.ChunkBmpBits[i][0]);
							else if (highToolStripMenuItem.Checked)
								bits = new BitmapBits(LevelData.ChunkBmpBits[i][1]);
							else
								bits = new BitmapBits(LevelData.Level.ChunkWidth, LevelData.Level.ChunkHeight);
							if (path1ToolStripMenuItem.Checked)
								bits.DrawBitmapComposited(LevelData.ChunkColBmpBits[i][0], 0, 0);
							else if (path2ToolStripMenuItem.Checked)
								bits.DrawBitmapComposited(LevelData.ChunkColBmpBits[i][1], 0, 0);
							bits.ToBitmap(LevelImgPalette).Save(pathBase + ".png");
						}
					}
		}

		private void solidityMapsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (FolderBrowserDialog a = new FolderBrowserDialog() { SelectedPath = Environment.CurrentDirectory })
				if (a.ShowDialog() == DialogResult.OK)
					for (int i = 0; i < LevelData.ColBmpBits.Length; i++)
						LevelData.ColBmpBits[i].ToBitmap1bpp(Color.Transparent, Color.White).Save(Path.Combine(a.SelectedPath,
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
						BitmapBits bmp = LevelData.DrawForeground(null, false, false, false, true, true, false, false, false);
						for (int i = 0; i < bmp.Bits.Length; i++)
							if (bmp.Bits[i] == 0)
								bmp.Bits[i] = 32;
						if (waterPalette != -1 && bmp.Height > waterHeight)
							bmp.ApplyWaterPalette(waterHeight);
						Bitmap res = bmp.ToBitmap();
						ColorPalette pal = res.Palette;
						for (int i = 0; i < 64; i++)
							pal.Entries[i] = LevelData.PaletteToColor(i / 16, i % 16, transparentBackFGBGToolStripMenuItem.Checked);
						pal.Entries.Fill(Color.Black, 64, 192);
						if (waterPalette != -1)
							for (int i = 64; i < 128; i++)
								if (transparentBackFGBGToolStripMenuItem.Checked && i % 16 == 0)
									pal.Entries[i] = Color.Transparent;
								else
									pal.Entries[i] = LevelData.Palette[waterPalette][(i - 64) / 16, i % 16].RGBColor;
						res.Palette = pal;
						res.Save(a.FileName);
						bool dualPath = false;
						switch (LevelData.Level.ChunkFormat)
						{
							case EngineVersion.S2:
							case EngineVersion.S2NA:
							case EngineVersion.S3K:
							case EngineVersion.SKC:
								dualPath = !Object.ReferenceEquals(LevelData.ColInds1, LevelData.ColInds2);
								break;
						}
						if (dualPath)
						{
							bmp = LevelData.DrawForeground(null, false, false, false, false, false, true, false, false);
							bmp.IncrementIndexes(-LevelData.ColorWhite + 1);
							bmp.ToBitmap4bpp(Color.Magenta, Color.White, Color.Yellow, Color.Black).Save(pathBase + "_col1" + pathExt);
							bmp = LevelData.DrawForeground(null, false, false, false, false, false, false, true, false);
							bmp.IncrementIndexes(-LevelData.ColorWhite + 1);
							bmp.ToBitmap4bpp(Color.Magenta, Color.White, Color.Yellow, Color.Black).Save(pathBase + "_col2" + pathExt);
						}
						else if (LevelData.LayoutFormat.HasLoopFlag && LevelData.Layout.FGLoop.OfType<bool>().Any(b => b))
						{
							bmp = LevelData.DrawForeground(null, false, false, false, false, false, true, false, false);
							bmp.IncrementIndexes(-LevelData.ColorWhite + 1);
							bmp.ToBitmap4bpp(Color.Magenta, Color.White, Color.Yellow, Color.Black).Save(pathBase + "_col1" + pathExt);
							byte[,] copy = (byte[,])LevelData.Layout.FGLayout.Clone();
							for (int y = 0; y < LevelData.FGHeight; y++)
								for (int x = 0; x < LevelData.FGWidth; x++)
									if (LevelData.Layout.FGLoop[x, y])
										LevelData.Layout.FGLayout[x, y]++;
							bmp = LevelData.DrawForeground(null, false, false, false, false, false, true, false, false);
							bmp.IncrementIndexes(-LevelData.ColorWhite + 1);
							bmp.ToBitmap4bpp(Color.Magenta, Color.White, Color.Yellow, Color.Black).Save(pathBase + "_col2" + pathExt);
							LevelData.Layout.FGLayout = copy;
						}
						else
						{
							bmp = LevelData.DrawForeground(null, false, false, false, false, false, true, false, false);
							bmp.IncrementIndexes(-LevelData.ColorWhite + 1);
							bmp.ToBitmap4bpp(Color.Magenta, Color.White, Color.Yellow, Color.Black).Save(pathBase + "_col" + pathExt);
						}
						bmp.Clear();
						for (int ly = 0; ly < LevelData.FGHeight; ly++)
							for (int lx = 0; lx < LevelData.FGWidth; lx++)
							{
								if (LevelData.Layout.FGLayout[lx, ly] >= LevelData.Chunks.Count) continue;
								Chunk cnk = LevelData.Chunks[LevelData.Layout.FGLayout[lx, ly]];
								for (int cy = 0; cy < LevelData.Level.ChunkHeight / 16; cy++)
									for (int cx = 0; cx < LevelData.Level.ChunkWidth / 16; cx++)
									{
										if (cnk.Blocks[cx, cy].Block >= LevelData.Blocks.Count) continue;
										Block blk = LevelData.Blocks[cnk.Blocks[cx, cy].Block];
										for (int by = 0; by < 2; by++)
											for (int bx = 0; bx < 2; bx++)
												if (blk.Tiles[bx, by].Priority)
													bmp.FillRectangle(1, lx * LevelData.Level.ChunkWidth + cx * 16 + bx * 8, ly * LevelData.Level.ChunkHeight + cy * 16 + by * 8, 8, 8);
									}
							}
						bmp.ToBitmap1bpp(Color.Black, Color.White).Save(pathBase + "_pri" + pathExt);
					}
					else
					{
						BitmapBits bmp = LevelData.DrawForeground(null, includeobjectsWithFGToolStripMenuItem.Checked, !hideDebugObjectsToolStripMenuItem.Checked, objectsAboveHighPlaneToolStripMenuItem.Checked, lowToolStripMenuItem.Checked, highToolStripMenuItem.Checked, path1ToolStripMenuItem.Checked, path2ToolStripMenuItem.Checked, allToolStripMenuItem.Checked);
						for (int i = 0; i < bmp.Bits.Length; i++)
							if (bmp.Bits[i] == 0)
								bmp.Bits[i] = 32;
						if (waterPalette != -1 && bmp.Height > waterHeight)
							for (int i = waterHeight * bmp.Width; i < bmp.Bits.Length; i++)
								if (bmp.Bits[i] < 64)
									bmp.Bits[i] += 64;
						Bitmap res = bmp.ToBitmap();
						ColorPalette pal = res.Palette;
						for (int i = 0; i < 64; i++)
							pal.Entries[i] = LevelData.PaletteToColor(i / 16, i % 16, transparentBackFGBGToolStripMenuItem.Checked);
						pal.Entries.Fill(Color.Black, 64, 192);
						if (waterPalette != -1)
							for (int i = 64; i < 128; i++)
								if (transparentBackFGBGToolStripMenuItem.Checked && i % 16 == 0)
									pal.Entries[i] = Color.Transparent;
								else
									pal.Entries[i] = LevelData.Palette[waterPalette][(i - 64) / 16, i % 16].RGBColor;
						pal.Entries[LevelData.ColorWhite] = Color.White;
						pal.Entries[LevelData.ColorYellow] = Color.Yellow;
						pal.Entries[LevelData.ColorBlack] = Color.Black;
						res.Palette = pal;
						res.Save(a.FileName);
					}
				}
		}

		private void backgroundToolStripMenuItem_Click(object sender, EventArgs e)
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
						BitmapBits bmp = LevelData.DrawBackground(null, true, true, false, false);
						for (int i = 0; i < bmp.Bits.Length; i++)
							if (bmp.Bits[i] == 0)
								bmp.Bits[i] = 32;
						Bitmap res = bmp.ToBitmap();
						ColorPalette pal = res.Palette;
						for (int i = 0; i < 64; i++)
							pal.Entries[i] = LevelData.PaletteToColor(i / 16, i % 16, transparentBackFGBGToolStripMenuItem.Checked);
						pal.Entries.Fill(Color.Black, 64, 192);
						res.Palette = pal;
						res.Save(a.FileName);
						bool dualPath = false;
						switch (LevelData.Level.ChunkFormat)
						{
							case EngineVersion.S2:
							case EngineVersion.S2NA:
							case EngineVersion.S3K:
							case EngineVersion.SKC:
								dualPath = !Object.ReferenceEquals(LevelData.ColInds1, LevelData.ColInds2);
								break;
						}
						if (dualPath)
						{
							bmp = LevelData.DrawBackground(null, false, false, true, false);
							bmp.IncrementIndexes(-LevelData.ColorWhite + 1);
							bmp.ToBitmap4bpp(Color.Magenta, Color.White, Color.Yellow, Color.Black).Save(pathBase + "_col1" + pathExt);
							bmp = LevelData.DrawBackground(null, false, false, false, true);
							bmp.IncrementIndexes(-LevelData.ColorWhite + 1);
							bmp.ToBitmap4bpp(Color.Magenta, Color.White, Color.Yellow, Color.Black).Save(pathBase + "_col2" + pathExt);
						}
						else if (LevelData.LayoutFormat.HasLoopFlag && LevelData.Layout.BGLoop.OfType<bool>().Any(b => b))
						{
							bmp = LevelData.DrawBackground(null, false, false, true, false);
							bmp.IncrementIndexes(-LevelData.ColorWhite + 1);
							bmp.ToBitmap4bpp(Color.Magenta, Color.White, Color.Yellow, Color.Black).Save(pathBase + "_col1" + pathExt);
							byte[,] copy = (byte[,])LevelData.Layout.BGLayout.Clone();
							for (int y = 0; y < LevelData.BGHeight; y++)
								for (int x = 0; x < LevelData.BGWidth; x++)
									if (LevelData.Layout.BGLoop[x, y])
										LevelData.Layout.BGLayout[x, y]++;
							bmp = LevelData.DrawBackground(null, false, false, true, false);
							bmp.IncrementIndexes(-LevelData.ColorWhite + 1);
							bmp.ToBitmap4bpp(Color.Magenta, Color.White, Color.Yellow, Color.Black).Save(pathBase + "_col2" + pathExt);
							LevelData.Layout.BGLayout = copy;
						}
						else
						{
							bmp = LevelData.DrawBackground(null, false, false, true, false);
							bmp.IncrementIndexes(-LevelData.ColorWhite + 1);
							bmp.ToBitmap4bpp(Color.Magenta, Color.White, Color.Yellow, Color.Black).Save(pathBase + "_col" + pathExt);
						}
						bmp.Clear();
						for (int ly = 0; ly < LevelData.BGHeight; ly++)
							for (int lx = 0; lx < LevelData.BGWidth; lx++)
							{
								if (LevelData.Layout.BGLayout[lx, ly] >= LevelData.Chunks.Count) continue;
								Chunk cnk = LevelData.Chunks[LevelData.Layout.BGLayout[lx, ly]];
								for (int cy = 0; cy < LevelData.Level.ChunkHeight / 16; cy++)
									for (int cx = 0; cx < LevelData.Level.ChunkWidth / 16; cx++)
									{
										if (cnk.Blocks[cx, cy].Block >= LevelData.Blocks.Count) continue;
										Block blk = LevelData.Blocks[cnk.Blocks[cx, cy].Block];
										for (int by = 0; by < 2; by++)
											for (int bx = 0; bx < 2; bx++)
												if (blk.Tiles[bx, by].Priority)
													bmp.FillRectangle(1, lx * LevelData.Level.ChunkWidth + cx * 16 + bx * 8, ly * LevelData.Level.ChunkHeight + cy * 16 + by * 8, 8, 8);
									}
							}
						bmp.ToBitmap1bpp(Color.Black, Color.White).Save(pathBase + "_pri" + pathExt);
					}
					else
					{
						BitmapBits bmp = LevelData.DrawBackground(null, lowToolStripMenuItem.Checked, highToolStripMenuItem.Checked, path1ToolStripMenuItem.Checked, path2ToolStripMenuItem.Checked);
						for (int i = 0; i < bmp.Bits.Length; i++)
							if (bmp.Bits[i] == 0)
								bmp.Bits[i] = 32;
						Bitmap res = bmp.ToBitmap();
						ColorPalette pal = res.Palette;
						for (int i = 0; i < 64; i++)
							pal.Entries[i] = LevelData.PaletteToColor(i / 16, i % 16, transparentBackFGBGToolStripMenuItem.Checked);
						pal.Entries.Fill(Color.Black, 64, 192);
						pal.Entries[LevelData.ColorWhite] = Color.White;
						pal.Entries[LevelData.ColorYellow] = Color.Yellow;
						pal.Entries[LevelData.ColorBlack] = Color.Black;
						pal.Entries[LevelData.ColorTransparent] = LevelData.PaletteToColor(2, 0, transparentBackFGBGToolStripMenuItem.Checked);
						res.Palette = pal;
						res.Save(a.FileName);
					}
				}
		}

		private void transparentBackFGBGToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			Settings.TransparentBackFGBGExport = transparentBackFGBGToolStripMenuItem.Checked;
		}

		private void includeObjectsWithFGToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			Settings.IncludeObjectsFGExport = includeobjectsWithFGToolStripMenuItem.Checked;
		}

		private void hideDebugObjectsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			Settings.HideDebugObjectsExport = hideDebugObjectsToolStripMenuItem.Checked;
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
		private void viewReadmeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "readme.txt"));
		}

		private void reportBugToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (BugReportDialog err = new BugReportDialog())
				err.ShowDialog();
		}
		#endregion
		#endregion

		void ObjectSelect_listView2_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!loaded) return;
			if (ObjectSelect.listView1.SelectedIndices.Count == 0) return;
			if (ObjectSelect.listView2.SelectedIndices.Count == 0) return;
			ObjectSelect.numericUpDown2.Value = (byte)ObjectSelect.listView2.SelectedItems[0].Tag;
		}

		void ObjectSelect_listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!loaded) return;
			if (ObjectSelect.listView1.SelectedIndices.Count == 0) return;
			byte ID = (byte)ObjectSelect.listView1.SelectedItems[0].Tag;
			ObjectSelect.numericUpDown1.Value = ID;
			ObjectSelect.numericUpDown2.Value = LevelData.ObjTypes[ID].DefaultSubtype;
			ObjectSelect.listView2.Items.Clear();
			ObjectSelect.imageList2.Images.Clear();
			foreach (byte item in LevelData.ObjTypes[ID].Subtypes)
			{
				ObjectSelect.imageList2.Images.Add(LevelData.ObjTypes[ID].SubtypeImage(item).Image.ToBitmap(LevelData.BmpPal).Resize(ObjectSelect.imageList2.ImageSize));
				ObjectSelect.listView2.Items.Add(new ListViewItem(LevelData.ObjTypes[ID].SubtypeName(item), ObjectSelect.imageList2.Images.Count - 1) { Tag = item, Selected = item == LevelData.ObjTypes[ID].DefaultSubtype });
			}
		}

		//List<object> oldvalues;
		void ObjectProperties_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
		{
			/*if (e.NewSelection.PropertyDescriptor == null || e.NewSelection.PropertyDescriptor.IsReadOnly) return;
			oldvalues = new List<object>();
			if (SelectedItems.Count > 1)
				oldvalues.Add(e.NewSelection.PropertyDescriptor.GetValue(SelectedItems.ToArray()));
			else
				oldvalues.Add(e.NewSelection.PropertyDescriptor.GetValue(SelectedItems[0]));*/
		}

		void ObjectProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			/*AddUndo(new ObjectPropertyChangedUndoAction(new List<Entry>(SelectedItems), oldvalues, e.ChangedItem.PropertyDescriptor));
			oldvalues = new List<object>();*/
			foreach (Entry item in SelectedItems)
			{
				//oldvalues.Add(e.ChangedItem.PropertyDescriptor.GetValue(item));
				item.UpdateSprite();
			}
			DrawLevel();
		}

		BitmapBits LevelImg8bpp;
		internal void DrawLevel()
		{
			if (!loaded) return;
			Point pnlcur;
			Point camera;
			switch (CurrentTab)
			{
				case Tab.Objects:
					pnlcur = objectPanel.PointToClient(Cursor.Position);
					camera = new Point(hScrollBar1.Value, vScrollBar1.Value);
					LevelImg8bpp = LevelData.DrawForeground(new Rectangle(camera.X, camera.Y, (int)(objectPanel.Width / ZoomLevel), (int)(objectPanel.Height / ZoomLevel)), true, true, objectsAboveHighPlaneToolStripMenuItem.Checked, lowToolStripMenuItem.Checked, highToolStripMenuItem.Checked, path1ToolStripMenuItem.Checked, path2ToolStripMenuItem.Checked, allToolStripMenuItem.Checked);
					if (waterPalette != -1 && camera.Y + LevelImg8bpp.Height > waterHeight)
						for (int i = Math.Max(waterHeight - camera.Y, 0) * LevelImg8bpp.Width; i < LevelImg8bpp.Bits.Length; i++)
							LevelImg8bpp.Bits[i] += 64;
					if (enableGridToolStripMenuItem.Checked && ObjGrid > 0)
					{
						int gs = 1 << ObjGrid;
						for (int x = (gs - (camera.X % gs)) % gs; x < LevelImg8bpp.Width; x += gs)
							LevelImg8bpp.DrawLine(131, x, 0, x, LevelImg8bpp.Height - 1);
						for (int y = (gs - (camera.Y % gs)) % gs; y < LevelImg8bpp.Height; y += gs)
							LevelImg8bpp.DrawLine(131, 0, y, LevelImg8bpp.Width - 1, y);
					}
					if (anglesToolStripMenuItem.Checked && !noneToolStripMenuItem1.Checked)
						for (int y = Math.Max(camera.Y / LevelData.Level.ChunkHeight, 0); y <= Math.Min(((camera.Y + (objectPanel.Height - 1) / ZoomLevel)) / LevelData.Level.ChunkHeight, LevelData.FGHeight - 1); y++)
							for (int x = Math.Max(camera.X / LevelData.Level.ChunkWidth, 0); x <= Math.Min(((camera.X + (objectPanel.Width - 1) / ZoomLevel)) / LevelData.Level.ChunkWidth, LevelData.FGWidth - 1); x++)
								for (int b = 0; b < LevelData.Level.ChunkHeight / 16; b++)
									for (int a = 0; a < LevelData.Level.ChunkWidth / 16; a++)
										if (LevelData.Layout.FGLayout[x, y] < LevelData.Chunks.Count)
										{
											ChunkBlock blk = LevelData.Chunks[LevelData.Layout.FGLayout[x, y]].Blocks[a, b];
											if (blk.Block > LevelData.Blocks.Count) continue;
											Solidity solid = path2ToolStripMenuItem.Checked ? ((S2ChunkBlock)blk).Solid2 : blk.Solid1;
											if (solid == Solidity.NotSolid) continue;
											byte coli = path2ToolStripMenuItem.Checked ? LevelData.GetColInd2(blk.Block) : LevelData.GetColInd1(blk.Block);
											byte angle = LevelData.Angles[coli];
											if (angle != 0xFF)
											{
												if (blk.XFlip)
													angle = (byte)(-angle & 0xFF);
												if (blk.YFlip)
													angle = (byte)((-(angle + 0x40) - 0x40) & 0xFF);
											}
											DrawHUDNum(x * LevelData.Level.ChunkWidth + a * 16 - camera.X, y * LevelData.Level.ChunkHeight + b * 16 - camera.Y, angle.ToString("X2"));
										}
					Rectangle hudbnd = Rectangle.Empty;
					Rectangle tmpbnd;
					int ringcnt;
					if (LevelData.RingFormat is RingLayoutFormat)
						ringcnt = ((RingLayoutFormat)LevelData.RingFormat).CountRings(LevelData.Rings);
					else
						ringcnt = ((RingObjectFormat)LevelData.RingFormat).CountRings(LevelData.Objects);
					if (hUDToolStripMenuItem.Checked)
					{
						tmpbnd = hudbnd = DrawHUDStr(8, 8, "Screen Pos: ");
						hudbnd = Rectangle.Union(hudbnd, DrawHUDNum(tmpbnd.Right, tmpbnd.Top, camera.X.ToString("X4") + ' ' + camera.Y.ToString("X4")));
						tmpbnd = DrawHUDStr(hudbnd.Left, hudbnd.Bottom, "Level Size: ");
						hudbnd = Rectangle.Union(hudbnd, tmpbnd);
						hudbnd = Rectangle.Union(hudbnd, DrawHUDNum(tmpbnd.Right, tmpbnd.Top, (LevelData.FGWidth * LevelData.Level.ChunkWidth).ToString("X4") + ' ' + (LevelData.FGHeight * LevelData.Level.ChunkHeight).ToString("X4")));
						hudbnd = Rectangle.Union(hudbnd, DrawHUDStr(hudbnd.Left, hudbnd.Bottom,
							"Objects: " + LevelData.Objects.Count + '\n' +
							"Rings: " + ringcnt));
						if (path1ToolStripMenuItem.Checked)
							hudbnd = Rectangle.Union(hudbnd, DrawHUDStr(hudbnd.Left, hudbnd.Bottom, "Path 1"));
						else if (path2ToolStripMenuItem.Checked)
							hudbnd = Rectangle.Union(hudbnd, DrawHUDStr(hudbnd.Left, hudbnd.Bottom, "Path 2"));
					}
					if (dragdrop)
						LevelImg8bpp.DrawSprite(LevelData.GetObjectDefinition(dragobj).Image, dragpoint);
					LevelBmp = LevelImg8bpp.ToBitmap(LevelImgPalette).To32bpp();
					LevelGfx = Graphics.FromImage(LevelBmp);
					LevelGfx.SetOptions();
					foreach (Entry item in SelectedItems)
					{
						if (item is ObjectEntry)
						{
							ObjectEntry objitem = (ObjectEntry)item;
							Rectangle objbnd = LevelData.GetObjectDefinition(objitem.ID).GetBounds(objitem, camera);
							LevelGfx.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Cyan)), objbnd);
							LevelGfx.DrawRectangle(new Pen(Color.FromArgb(128, Color.Black)) { DashStyle = DashStyle.Dot }, objbnd);
						}
						else if (item is RingEntry)
						{
							RingEntry rngitem = (RingEntry)item;
							Rectangle bnd = ((RingLayoutFormat)LevelData.RingFormat).GetBounds(rngitem, camera);
							LevelGfx.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Yellow)), bnd);
							LevelGfx.DrawRectangle(new Pen(Color.FromArgb(128, Color.Black)) { DashStyle = DashStyle.Dot }, bnd);
						}
						else if (item is CNZBumperEntry)
						{
							LevelGfx.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Cyan)), LevelData.unkobj.GetBounds(new S2ObjectEntry() { X = item.X, Y = item.Y }, new Point(camera.X, camera.Y)));
							LevelGfx.DrawRectangle(new Pen(Color.FromArgb(128, Color.Black)) { DashStyle = DashStyle.Dot }, LevelData.unkobj.GetBounds(new S2ObjectEntry() { X = item.X, Y = item.Y }, new Point(camera.X, camera.Y)));
						}
						else if (item is StartPositionEntry)
						{
							StartPositionEntry strtitem = (StartPositionEntry)item;
							Rectangle bnd = LevelData.StartPosDefs[LevelData.StartPositions.IndexOf(strtitem)].GetBounds(strtitem, camera);
							LevelGfx.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Red)), bnd);
							LevelGfx.DrawRectangle(new Pen(Color.FromArgb(128, Color.Black)) { DashStyle = DashStyle.Dot }, bnd);
						}
					}
					if (LevelData.LayoutFormat.HasLoopFlag)
						for (int y = Math.Max(camera.Y / LevelData.Level.ChunkHeight, 0); y <= Math.Min(((camera.Y + (objectPanel.Height - 1) / ZoomLevel)) / LevelData.Level.ChunkHeight, LevelData.FGHeight - 1); y++)
							for (int x = Math.Max(camera.X / LevelData.Level.ChunkWidth, 0); x <= Math.Min(((camera.X + (objectPanel.Width - 1) / ZoomLevel)) / LevelData.Level.ChunkWidth, LevelData.FGWidth - 1); x++)
								if (LevelData.Layout.FGLoop[x, y])
									LevelGfx.DrawRectangle(new Pen(Color.FromArgb(128, Color.Yellow)) { Width = 3 }, x * LevelData.Level.ChunkWidth - camera.X, y * LevelData.Level.ChunkHeight - camera.Y, LevelData.Level.ChunkWidth, LevelData.Level.ChunkHeight);
					if (selecting)
					{
						Rectangle selbnds = Rectangle.FromLTRB(
						Math.Min(selpoint.X, lastmouse.X) - camera.X,
						Math.Min(selpoint.Y, lastmouse.Y) - camera.Y,
						Math.Max(selpoint.X, lastmouse.X) - camera.X,
						Math.Max(selpoint.Y, lastmouse.Y) - camera.Y);
						LevelGfx.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.White)), selbnds);
						LevelGfx.DrawRectangle(new Pen(Color.FromArgb(128, Color.Black)) { DashStyle = DashStyle.Dot }, selbnds);
					}
					Panel1Gfx.DrawImage(LevelBmp, 0, 0, objectPanel.Width, objectPanel.Height);
					break;
				case Tab.Foreground:
					pnlcur = foregroundPanel.PointToClient(Cursor.Position);
					camera = new Point(hScrollBar2.Value, vScrollBar2.Value);
					LevelImg8bpp = LevelData.DrawForeground(new Rectangle(camera.X, camera.Y, (int)(foregroundPanel.Width / ZoomLevel), (int)(foregroundPanel.Height / ZoomLevel)), true, true, objectsAboveHighPlaneToolStripMenuItem.Checked, lowToolStripMenuItem.Checked, highToolStripMenuItem.Checked, path1ToolStripMenuItem.Checked, path2ToolStripMenuItem.Checked, allToolStripMenuItem.Checked);
					if (waterPalette != -1 && camera.Y + LevelImg8bpp.Height > waterHeight)
						for (int i = Math.Max(waterHeight - camera.Y, 0) * LevelImg8bpp.Width; i < LevelImg8bpp.Bits.Length; i++)
							LevelImg8bpp.Bits[i] += 64;
					if (enableGridToolStripMenuItem.Checked)
					{
						for (int x = (LevelData.Level.ChunkWidth - (camera.X % LevelData.Level.ChunkWidth)) % LevelData.Level.ChunkWidth; x < LevelImg8bpp.Width; x += LevelData.Level.ChunkWidth)
							LevelImg8bpp.DrawLine(131, x, 0, x, LevelImg8bpp.Height - 1);
						for (int y = (LevelData.Level.ChunkHeight - (camera.Y % LevelData.Level.ChunkHeight)) % LevelData.Level.ChunkHeight; y < LevelImg8bpp.Height; y += LevelData.Level.ChunkHeight)
							LevelImg8bpp.DrawLine(131, 0, y, LevelImg8bpp.Width - 1, y);
					}
					if (anglesToolStripMenuItem.Checked & !noneToolStripMenuItem1.Checked)
						for (int y = Math.Max(camera.Y / LevelData.Level.ChunkHeight, 0); y <= Math.Min(((camera.Y + (foregroundPanel.Height - 1) / ZoomLevel)) / LevelData.Level.ChunkHeight, LevelData.FGHeight - 1); y++)
							for (int x = Math.Max(camera.X / LevelData.Level.ChunkWidth, 0); x <= Math.Min(((camera.X + (foregroundPanel.Width - 1) / ZoomLevel)) / LevelData.Level.ChunkWidth, LevelData.FGWidth - 1); x++)
								for (int b = 0; b < LevelData.Level.ChunkHeight / 16; b++)
									for (int a = 0; a < LevelData.Level.ChunkWidth / 16; a++)
										if (LevelData.Layout.FGLayout[x, y] < LevelData.Chunks.Count)
										{
											ChunkBlock blk = LevelData.Chunks[LevelData.Layout.FGLayout[x, y]].Blocks[a, b];
											if (blk.Block > LevelData.Blocks.Count) continue;
											Solidity solid = path2ToolStripMenuItem.Checked ? ((S2ChunkBlock)blk).Solid2 : blk.Solid1;
											if (solid == Solidity.NotSolid) continue;
											byte coli = path2ToolStripMenuItem.Checked ? LevelData.GetColInd2(blk.Block) : LevelData.GetColInd1(blk.Block);
											byte angle = LevelData.Angles[coli];
											if (angle != 0xFF)
											{
												if (blk.XFlip)
													angle = (byte)(-angle & 0xFF);
												if (blk.YFlip)
													angle = (byte)((-(angle + 0x40) - 0x40) & 0xFF);
											}
											DrawHUDNum(x * LevelData.Level.ChunkWidth + a * 16 - camera.X, y * LevelData.Level.ChunkHeight + b * 16 - camera.Y, angle.ToString("X2"));
										}
					if (LevelData.RingFormat is RingLayoutFormat)
						ringcnt = ((RingLayoutFormat)LevelData.RingFormat).CountRings(LevelData.Rings);
					else
						ringcnt = ((RingObjectFormat)LevelData.RingFormat).CountRings(LevelData.Objects);
					if (hUDToolStripMenuItem.Checked)
					{
						tmpbnd = hudbnd = DrawHUDStr(8, 8, "Screen Pos: ");
						hudbnd = Rectangle.Union(hudbnd, DrawHUDNum(tmpbnd.Right, tmpbnd.Top, camera.X.ToString("X4") + ' ' + camera.Y.ToString("X4")));
						tmpbnd = DrawHUDStr(hudbnd.Left, hudbnd.Bottom, "Level Size: ");
						hudbnd = Rectangle.Union(hudbnd, tmpbnd);
						hudbnd = Rectangle.Union(hudbnd, DrawHUDNum(tmpbnd.Right, tmpbnd.Top, (LevelData.FGWidth * LevelData.Level.ChunkWidth).ToString("X4") + ' ' + (LevelData.FGHeight * LevelData.Level.ChunkHeight).ToString("X4")));
						hudbnd = Rectangle.Union(hudbnd, DrawHUDStr(hudbnd.Left, hudbnd.Bottom,
							"Objects: " + LevelData.Objects.Count + '\n' +
							"Rings: " + ringcnt));
						tmpbnd = DrawHUDStr(hudbnd.Left, hudbnd.Bottom, "Chunk: ");
						hudbnd = Rectangle.Union(hudbnd, tmpbnd);
						hudbnd = Rectangle.Union(hudbnd, DrawHUDNum(tmpbnd.Right, tmpbnd.Top, SelectedChunk.ToString("X2")));
						if (path1ToolStripMenuItem.Checked)
							hudbnd = Rectangle.Union(hudbnd, DrawHUDStr(hudbnd.Left, hudbnd.Bottom, "Path 1"));
						else if (path2ToolStripMenuItem.Checked)
							hudbnd = Rectangle.Union(hudbnd, DrawHUDStr(hudbnd.Left, hudbnd.Bottom, "Path 2"));
					}
					LevelBmp = LevelImg8bpp.ToBitmap(LevelImgPalette).To32bpp();
					LevelGfx = Graphics.FromImage(LevelBmp);
					LevelGfx.SetOptions();
					if (LevelData.LayoutFormat.HasLoopFlag)
						for (int y = Math.Max(camera.Y / LevelData.Level.ChunkHeight, 0); y <= Math.Min(((camera.Y + (foregroundPanel.Height - 1) / ZoomLevel)) / LevelData.Level.ChunkHeight, LevelData.FGHeight - 1); y++)
							for (int x = Math.Max(camera.X / LevelData.Level.ChunkWidth, 0); x <= Math.Min(((camera.X + (foregroundPanel.Width - 1) / ZoomLevel)) / LevelData.Level.ChunkWidth, LevelData.FGWidth - 1); x++)
								if (LevelData.Layout.FGLoop[x, y])
									LevelGfx.DrawRectangle(new Pen(Color.FromArgb(128, Color.Yellow)) { Width = (int)(3 * ZoomLevel) }, x * LevelData.Level.ChunkWidth - camera.X, y * LevelData.Level.ChunkHeight - camera.Y, LevelData.Level.ChunkWidth, LevelData.Level.ChunkHeight);
					if (!selecting && SelectedChunk < LevelData.Chunks.Count)
						LevelGfx.DrawImage(LevelData.CompChunkBmps[SelectedChunk],
						new Rectangle(((((int)(pnlcur.X / ZoomLevel) + camera.X) / LevelData.Level.ChunkWidth) * LevelData.Level.ChunkWidth) - camera.X, ((((int)(pnlcur.Y / ZoomLevel) + camera.Y) / LevelData.Level.ChunkHeight) * LevelData.Level.ChunkHeight) - camera.Y, LevelData.Level.ChunkWidth, LevelData.Level.ChunkHeight),
						0, 0, LevelData.Level.ChunkWidth, LevelData.Level.ChunkHeight,
						GraphicsUnit.Pixel, imageTransparency);
					if (!FGSelection.IsEmpty)
					{
						Rectangle selbnds = FGSelection.Scale(LevelData.Level.ChunkWidth, LevelData.Level.ChunkHeight);
						selbnds.Offset(-camera.X, -camera.Y);
						LevelGfx.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.White)), selbnds);
						LevelGfx.DrawRectangle(new Pen(Color.FromArgb(128, Color.Black)) { DashStyle = DashStyle.Dot }, selbnds);
					}
					Panel2Gfx.DrawImage(LevelBmp, 0, 0, foregroundPanel.Width, foregroundPanel.Height);
					break;
				case Tab.Background:
					pnlcur = backgroundPanel.PointToClient(Cursor.Position);
					camera = new Point(hScrollBar3.Value, vScrollBar3.Value);
					LevelImg8bpp = LevelData.DrawBackground(new Rectangle(camera.X, camera.Y, (int)(backgroundPanel.Width / ZoomLevel), (int)(backgroundPanel.Height / ZoomLevel)), lowToolStripMenuItem.Checked, highToolStripMenuItem.Checked, path1ToolStripMenuItem.Checked, path2ToolStripMenuItem.Checked);
					if (enableGridToolStripMenuItem.Checked)
					{
						for (int x = (LevelData.Level.ChunkWidth - (camera.X % LevelData.Level.ChunkWidth)) % LevelData.Level.ChunkWidth; x < LevelImg8bpp.Width; x += LevelData.Level.ChunkWidth)
							LevelImg8bpp.DrawLine(131, x, 0, x, LevelImg8bpp.Height - 1);
						for (int y = (LevelData.Level.ChunkHeight - (camera.Y % LevelData.Level.ChunkHeight)) % LevelData.Level.ChunkHeight; y < LevelImg8bpp.Height; y += LevelData.Level.ChunkHeight)
							LevelImg8bpp.DrawLine(131, 0, y, LevelImg8bpp.Width - 1, y);
					}
					if (anglesToolStripMenuItem.Checked & !noneToolStripMenuItem1.Checked)
						for (int y = Math.Max(camera.Y / LevelData.Level.ChunkHeight, 0); y <= Math.Min(((camera.Y + (backgroundPanel.Height - 1) / ZoomLevel)) / LevelData.Level.ChunkHeight, LevelData.BGHeight - 1); y++)
							for (int x = Math.Max(camera.X / LevelData.Level.ChunkWidth, 0); x <= Math.Min(((camera.X + (backgroundPanel.Width - 1) / ZoomLevel)) / LevelData.Level.ChunkWidth, LevelData.BGWidth - 1); x++)
								for (int b = 0; b < LevelData.Level.ChunkHeight / 16; b++)
									for (int a = 0; a < LevelData.Level.ChunkWidth / 16; a++)
										if (LevelData.Layout.BGLayout[x, y] < LevelData.Chunks.Count)
										{
											ChunkBlock blk = LevelData.Chunks[LevelData.Layout.BGLayout[x, y]].Blocks[a, b];
											if (blk.Block > LevelData.Blocks.Count) continue;
											Solidity solid = path2ToolStripMenuItem.Checked ? ((S2ChunkBlock)blk).Solid2 : blk.Solid1;
											if (solid == Solidity.NotSolid) continue;
											byte coli = path2ToolStripMenuItem.Checked ? LevelData.GetColInd2(blk.Block) : LevelData.GetColInd1(blk.Block);
											byte angle = LevelData.Angles[coli];
											if (angle != 0xFF)
											{
												if (blk.XFlip)
													angle = (byte)(-angle & 0xFF);
												if (blk.YFlip)
													angle = (byte)((-(angle + 0x40) - 0x40) & 0xFF);
											}
											DrawHUDNum(x * LevelData.Level.ChunkWidth + a * 16 - camera.X, y * LevelData.Level.ChunkHeight + b * 16 - camera.Y, angle.ToString("X2"));
										}
					if (hUDToolStripMenuItem.Checked)
					{
						tmpbnd = hudbnd = DrawHUDStr(8, 8, "Screen Pos: ");
						hudbnd = Rectangle.Union(hudbnd, DrawHUDNum(tmpbnd.Right, tmpbnd.Top, camera.X.ToString("X4") + ' ' + camera.Y.ToString("X4")));
						tmpbnd = DrawHUDStr(hudbnd.Left, hudbnd.Bottom, "Level Size: ");
						hudbnd = Rectangle.Union(hudbnd, tmpbnd);
						hudbnd = Rectangle.Union(hudbnd, DrawHUDNum(tmpbnd.Right, tmpbnd.Top, (LevelData.BGWidth * LevelData.Level.ChunkWidth).ToString("X4") + ' ' + (LevelData.BGHeight * LevelData.Level.ChunkHeight).ToString("X4")));
						tmpbnd = DrawHUDStr(hudbnd.Left, hudbnd.Bottom, "Chunk: ");
						hudbnd = Rectangle.Union(hudbnd, tmpbnd);
						hudbnd = Rectangle.Union(hudbnd, DrawHUDNum(tmpbnd.Right, tmpbnd.Top, SelectedChunk.ToString("X2")));
						if (path1ToolStripMenuItem.Checked)
							hudbnd = Rectangle.Union(hudbnd, DrawHUDStr(hudbnd.Left, hudbnd.Bottom, "Path 1"));
						else if (path2ToolStripMenuItem.Checked)
							hudbnd = Rectangle.Union(hudbnd, DrawHUDStr(hudbnd.Left, hudbnd.Bottom, "Path 2"));
					}
					LevelBmp = LevelImg8bpp.ToBitmap(LevelImgPalette).To32bpp();
					LevelGfx = Graphics.FromImage(LevelBmp);
					LevelGfx.SetOptions();
					if (LevelData.LayoutFormat.HasLoopFlag)
						for (int y = Math.Max(camera.Y / LevelData.Level.ChunkHeight, 0); y <= Math.Min(((camera.Y + (backgroundPanel.Height - 1) / ZoomLevel)) / LevelData.Level.ChunkHeight, LevelData.BGHeight - 1); y++)
							for (int x = Math.Max(camera.X / LevelData.Level.ChunkWidth, 0); x <= Math.Min(((camera.X + (backgroundPanel.Width - 1) / ZoomLevel)) / LevelData.Level.ChunkWidth, LevelData.BGWidth - 1); x++)
								if (LevelData.Layout.BGLoop[x, y])
									LevelGfx.DrawRectangle(new Pen(Color.FromArgb(128, Color.Yellow)) { Width = (int)(3 * ZoomLevel) }, x * LevelData.Level.ChunkWidth - camera.X, y * LevelData.Level.ChunkHeight - camera.Y, LevelData.Level.ChunkWidth, LevelData.Level.ChunkHeight);
					if (!selecting && SelectedChunk < LevelData.Chunks.Count)
						LevelGfx.DrawImage(LevelData.CompChunkBmps[SelectedChunk],
						new Rectangle(((((int)(pnlcur.X / ZoomLevel) + camera.X) / LevelData.Level.ChunkWidth) * LevelData.Level.ChunkWidth) - camera.X, ((((int)(pnlcur.Y / ZoomLevel) + camera.Y) / LevelData.Level.ChunkHeight) * LevelData.Level.ChunkHeight) - camera.Y, LevelData.Level.ChunkWidth, LevelData.Level.ChunkHeight),
						0, 0, LevelData.Level.ChunkWidth, LevelData.Level.ChunkHeight,
						GraphicsUnit.Pixel, imageTransparency);
					if (!BGSelection.IsEmpty)
					{
						Rectangle selbnds = BGSelection.Scale(LevelData.Level.ChunkWidth, LevelData.Level.ChunkHeight);
						selbnds.Offset(-camera.X, -camera.Y);
						LevelGfx.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.White)), selbnds);
						LevelGfx.DrawRectangle(new Pen(Color.FromArgb(128, Color.Black)) { DashStyle = DashStyle.Dot }, selbnds);
					}
					Panel3Gfx.DrawImage(LevelBmp, 0, 0, backgroundPanel.Width, backgroundPanel.Height);
					break;
			}
		}

		public Rectangle DrawHUDStr(int X, int Y, string str)
		{
			BitmapBits curimg;
			int curX = X;
			int curY = Y;
			Rectangle bounds = new Rectangle();
			bounds.X = X;
			bounds.Y = Y;
			int maxX = X;
			foreach (string line in str.Split(new char[] { '\n' }, StringSplitOptions.None))
			{
				int maxY = 0;
				foreach (char ch in line)
				{
					if (HUDLetters.ContainsKey(char.ToUpperInvariant(ch)))
						curimg = HUDLetters[char.ToUpperInvariant(ch)];
					else
						curimg = HUDLetters[' '];
					LevelImg8bpp.DrawBitmapComposited(curimg, new Point(curX, curY));
					curX += curimg.Width;
					maxX = Math.Max(maxX, curX);
					maxY = Math.Max(maxY, curimg.Height);
				}
				curY += maxY;
				curX = X;
			}
			bounds.Width = maxX - X;
			bounds.Height = curY - Y;
			return bounds;
		}

		public Rectangle DrawHUDNum(int X, int Y, string str)
		{
			BitmapBits curimg;
			int curX = X;
			int curY = Y;
			Rectangle bounds = new Rectangle();
			bounds.X = X;
			bounds.Y = Y;
			int maxX = X;
			foreach (string line in str.Split(new char[] { '\n' }, StringSplitOptions.None))
			{
				int maxY = 0;
				foreach (char ch in line)
				{
					if (HUDNumbers.ContainsKey(char.ToUpperInvariant(ch)))
						curimg = HUDNumbers[char.ToUpperInvariant(ch)];
					else
						curimg = HUDNumbers[' '];
					LevelImg8bpp.DrawBitmapComposited(curimg, new Point(curX, curY));
					curX += curimg.Width;
					maxX = Math.Max(maxX, curX);
					maxY = Math.Max(maxY, curimg.Height);
				}
				curY += maxY;
				curX = X;
			}
			bounds.Height = curY - Y;
			return bounds;
		}

		private void panel_Paint(object sender, PaintEventArgs e)
		{
			DrawLevel();
		}

		private void UpdateScrollBars()
		{
			hScrollBar1.Maximum = (int)Math.Max(((LevelData.FGWidth + 1) * LevelData.Level.ChunkWidth) - (objectPanel.Width / ZoomLevel), 0);
			vScrollBar1.Maximum = (int)Math.Max(((LevelData.FGHeight + 1) * LevelData.Level.ChunkHeight) - (objectPanel.Height / ZoomLevel), 0);
			hScrollBar2.Maximum = (int)Math.Max(((LevelData.FGWidth + 1) * LevelData.Level.ChunkWidth) - (foregroundPanel.Width / ZoomLevel), 0);
			vScrollBar2.Maximum = (int)Math.Max(((LevelData.FGHeight + 1) * LevelData.Level.ChunkHeight) - (foregroundPanel.Height / ZoomLevel), 0);
			hScrollBar3.Maximum = (int)Math.Max(((LevelData.BGWidth + 1) * LevelData.Level.ChunkWidth) - (backgroundPanel.Width / ZoomLevel), 0);
			vScrollBar3.Maximum = (int)Math.Max(((LevelData.BGHeight + 1) * LevelData.Level.ChunkHeight) - (backgroundPanel.Height / ZoomLevel), 0);
		}

		Rectangle prevbnds;
		FormWindowState prevstate;
		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Y:
					if (!loaded) return;
					if (e.Control && RedoList.Count > 0)
						DoRedo(1);
					break;
				case Keys.Z:
					if (!loaded) return;
					if (e.Control && UndoList.Count > 0)
						DoUndo(1);
					break;
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
					menuStrip1.ShowHide();
					break;
				case Keys.D1:
				case Keys.NumPad1:
					if (e.Control)
						CurrentTab = Tab.Objects;
					break;
				case Keys.D2:
				case Keys.NumPad2:
					if (e.Control)
						CurrentTab = Tab.Foreground;
					break;
				case Keys.D3:
				case Keys.NumPad3:
					if (e.Control)
						CurrentTab = Tab.Background;
					break;
				case Keys.D4:
				case Keys.NumPad4:
					if (e.Control)
						CurrentTab = Tab.Art;
					break;
			}
		}

		private void objectPanel_KeyDown(object sender, KeyEventArgs e)
		{
			long hstep = e.Control ? int.MaxValue : e.Shift ? LevelData.Level.ChunkWidth : 16;
			long vstep = e.Control ? int.MaxValue : e.Shift ? LevelData.Level.ChunkHeight : 16;
			switch (e.KeyCode)
			{
				case Keys.Up:
					if (!loaded) return;
					vScrollBar1.Value = (int)Math.Max(vScrollBar1.Value - vstep, vScrollBar1.Minimum);
					break;
				case Keys.Down:
					if (!loaded) return;
					vScrollBar1.Value = (int)Math.Min(vScrollBar1.Value + vstep, vScrollBar1.Maximum - LevelData.Level.ChunkHeight + 1);
					break;
				case Keys.Left:
					if (!loaded) return;
					hScrollBar1.Value = (int)Math.Max(hScrollBar1.Value - hstep, hScrollBar1.Minimum);
					break;
				case Keys.Right:
					if (!loaded) return;
					hScrollBar1.Value = (int)Math.Min(hScrollBar1.Value + hstep, hScrollBar1.Maximum - LevelData.Level.ChunkWidth + 1);
					break;
				case Keys.Delete:
					if (!loaded) return;
					if (SelectedItems.Count > 0)
						deleteToolStripMenuItem_Click(sender, EventArgs.Empty);
					break;
				case Keys.A:
					if (!loaded) return;
					for (int i = 0; i < SelectedItems.Count; i++)
					{
						if (SelectedItems[i] is S1ObjectEntry)
						{
							S1ObjectEntry oi = SelectedItems[i] as S1ObjectEntry;
							oi.ID = (byte)(oi.ID == 0 ? 0x7F : oi.ID - 1);
							SelectedItems[i].UpdateSprite();
						}
						else if (SelectedItems[i] is SCDObjectEntry)
						{
							SCDObjectEntry oi = SelectedItems[i] as SCDObjectEntry;
							oi.ID = (byte)(oi.ID == 0 ? 0x7F : oi.ID - 1);
							SelectedItems[i].UpdateSprite();
						}
						else if (SelectedItems[i] is ObjectEntry)
						{
							ObjectEntry oi = SelectedItems[i] as ObjectEntry;
							oi.ID = (byte)(oi.ID == 0 ? 255 : oi.ID - 1);
							SelectedItems[i].UpdateSprite();
						}
						else if (SelectedItems[i] is CNZBumperEntry)
						{
							CNZBumperEntry ci = SelectedItems[i] as CNZBumperEntry;
							ci.ID = (ushort)(ci.ID == 0 ? 65535 : ci.ID - 1);
							ci.UpdateSprite();
						}
					}
					DrawLevel();
					break;
				case Keys.C:
					if (!loaded) return;
					if (e.Control)
						if (SelectedItems.Count > 0)
							copyToolStripMenuItem_Click(sender, EventArgs.Empty);
					break;
				case Keys.J:
					int gs = ObjGrid + 1;
					if (gs < objGridSizeDropDownButton.DropDownItems.Count)
						objGridSizeDropDownButton_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(objGridSizeDropDownButton.DropDownItems[gs]));
					break;
				case Keys.M:
					if (ObjGrid > 0)
						objGridSizeDropDownButton_DropDownItemClicked(this, new ToolStripItemClickedEventArgs(objGridSizeDropDownButton.DropDownItems[ObjGrid - 1]));
					break;
				case Keys.S:
					if (!loaded) return;
					if (!e.Control)
					{
						foreach (ObjectEntry item in SelectedItems.OfType<ObjectEntry>())
						{
							unchecked
							{
								if (item is ChaotixObjectEntry)
									--((ChaotixObjectEntry)item).FullSubType;
								else
									--item.SubType;
							}
							item.UpdateSprite();
						}
						DrawLevel();
					}
					break;
				case Keys.V:
					if (!loaded) return;
					if (e.Control)
					{
						menuLoc = new Point(objectPanel.Width / 2, objectPanel.Height / 2);
						pasteToolStripMenuItem_Click(sender, EventArgs.Empty);
					}
					break;
				case Keys.X:
					if (!loaded) return;
					if (e.Control)
					{
						if (SelectedItems.Count > 0)
							cutToolStripMenuItem_Click(sender, EventArgs.Empty);
					}
					else
					{
						foreach (ObjectEntry item in SelectedItems.OfType<ObjectEntry>())
						{
							if (item is ChaotixObjectEntry)
								++((ChaotixObjectEntry)item).FullSubType;
							else
								++item.SubType;
							item.UpdateSprite();
						}
						DrawLevel();
					}
					break;
				case Keys.Z:
					if (!loaded) return;
					if (!e.Control)
					{
						for (int i = 0; i < SelectedItems.Count; i++)
						{
							if (SelectedItems[i] is S1ObjectEntry)
							{
								S1ObjectEntry oi = SelectedItems[i] as S1ObjectEntry;
								oi.ID = (byte)(oi.ID == 0x7F ? 0 : oi.ID + 1);
								SelectedItems[i].UpdateSprite();
							}
							else if (SelectedItems[i] is SCDObjectEntry)
							{
								SCDObjectEntry oi = SelectedItems[i] as SCDObjectEntry;
								oi.ID = (byte)(oi.ID == 0x7F ? 0 : oi.ID + 1);
								SelectedItems[i].UpdateSprite();
							}
							else if (SelectedItems[i] is ObjectEntry)
							{
								ObjectEntry oi = SelectedItems[i] as ObjectEntry;
								oi.ID = (byte)(oi.ID == 255 ? 0 : oi.ID + 1);
								SelectedItems[i].UpdateSprite();
							}
							else if (SelectedItems[i] is CNZBumperEntry)
							{
								CNZBumperEntry ci = SelectedItems[i] as CNZBumperEntry;
								ci.ID = (ushort)(ci.ID == 0 ? 65535 : ci.ID - 1);
								ci.UpdateSprite();
							}
						}
						DrawLevel();
					}
					break;
			}
			panel_KeyDown(sender, e);
		}

		private void foregroundPanel_KeyDown(object sender, KeyEventArgs e)
		{
			long hstep = e.Control ? int.MaxValue : e.Shift ? LevelData.Level.ChunkWidth : 16;
			long vstep = e.Control ? int.MaxValue : e.Shift ? LevelData.Level.ChunkHeight : 16;
			switch (e.KeyCode)
			{
				case Keys.Up:
					if (!loaded) return;
					vScrollBar2.Value = (int)Math.Max(vScrollBar2.Value - vstep, vScrollBar2.Minimum);
					break;
				case Keys.Down:
					if (!loaded) return;
					vScrollBar2.Value = (int)Math.Min(vScrollBar2.Value + vstep, vScrollBar2.Maximum - LevelData.Level.ChunkHeight + 1);
					break;
				case Keys.Left:
					if (!loaded) return;
					hScrollBar2.Value = (int)Math.Max(hScrollBar2.Value - hstep, hScrollBar2.Minimum);
					break;
				case Keys.Right:
					if (!loaded) return;
					hScrollBar2.Value = (int)Math.Min(hScrollBar2.Value + hstep, hScrollBar2.Maximum - LevelData.Level.ChunkWidth + 1);
					break;
				case Keys.A:
					if (!loaded) return;
					SelectedChunk = (byte)(SelectedChunk == 0 ? LevelData.Chunks.Count - 1 : SelectedChunk - 1);
					DrawLevel();
					break;
				case Keys.Z:
					if (!loaded) return;
					if (!e.Control)
					{
						SelectedChunk = (byte)(SelectedChunk == LevelData.Chunks.Count - 1 ? 0 : SelectedChunk + 1);
						DrawLevel();
					}
					break;
			}
			panel_KeyDown(sender, e);
		}

		private void backgroundPanel_KeyDown(object sender, KeyEventArgs e)
		{
			long hstep = e.Control ? int.MaxValue : e.Shift ? LevelData.Level.ChunkWidth : 16;
			long vstep = e.Control ? int.MaxValue : e.Shift ? LevelData.Level.ChunkHeight : 16;
			switch (e.KeyCode)
			{
				case Keys.Up:
					if (!loaded) return;
					vScrollBar3.Value = (int)Math.Max(vScrollBar3.Value - vstep, vScrollBar3.Minimum);
					break;
				case Keys.Down:
					if (!loaded) return;
					vScrollBar3.Value = (int)Math.Min(vScrollBar3.Value + vstep, vScrollBar3.Maximum - LevelData.Level.ChunkHeight + 1);
					break;
				case Keys.Left:
					if (!loaded) return;
					hScrollBar3.Value = (int)Math.Max(hScrollBar3.Value - hstep, hScrollBar3.Minimum);
					break;
				case Keys.Right:
					if (!loaded) return;
					hScrollBar3.Value = (int)Math.Min(hScrollBar3.Value + hstep, hScrollBar3.Maximum - LevelData.Level.ChunkWidth + 1);
					break;
				case Keys.A:
					if (!loaded) return;
					SelectedChunk = (byte)(SelectedChunk == 0 ? LevelData.Chunks.Count - 1 : SelectedChunk - 1);
					DrawLevel();
					break;
				case Keys.Z:
					if (!loaded) return;
					if (!e.Control)
					{
						SelectedChunk = (byte)(SelectedChunk == LevelData.Chunks.Count - 1 ? 0 : SelectedChunk + 1);
						DrawLevel();
					}
					break;
			}
			panel_KeyDown(sender, e);
		}

		private void panel_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Q:
					bool angles = anglesToolStripMenuItem.Checked;
					foreach (ToolStripItem item in collisionToolStripMenuItem.DropDownItems)
						if (item is ToolStripMenuItem)
							((ToolStripMenuItem)item).Checked = false;
					noneToolStripMenuItem1.Checked = true;
					anglesToolStripMenuItem.Checked = angles;
					DrawLevel();
					break;
				case Keys.W:
					angles = anglesToolStripMenuItem.Checked;
					foreach (ToolStripItem item in collisionToolStripMenuItem.DropDownItems)
						if (item is ToolStripMenuItem)
							((ToolStripMenuItem)item).Checked = false;
					path1ToolStripMenuItem.Checked = true;
					anglesToolStripMenuItem.Checked = angles;
					DrawLevel();
					break;
				case Keys.E:
					switch (LevelData.Level.ChunkFormat)
					{
						case EngineVersion.S1:
						case EngineVersion.SCD:
						case EngineVersion.SCDPC:
							break;
						case EngineVersion.S2:
						case EngineVersion.S2NA:
						case EngineVersion.S3K:
						case EngineVersion.SKC:
							angles = anglesToolStripMenuItem.Checked;
							foreach (ToolStripItem item in collisionToolStripMenuItem.DropDownItems)
								if (item is ToolStripMenuItem)
									((ToolStripMenuItem)item).Checked = false;
							path2ToolStripMenuItem.Checked = true;
							anglesToolStripMenuItem.Checked = angles;
							DrawLevel();
							break;
					}
					break;
				case Keys.R:
					if (!(e.Alt & e.Control))
					{
						anglesToolStripMenuItem.Checked = !anglesToolStripMenuItem.Checked;
						DrawLevel();
					}
					break;
				case Keys.T:
					objectsAboveHighPlaneToolStripMenuItem.Checked = !objectsAboveHighPlaneToolStripMenuItem.Checked;
					DrawLevel();
					break;
				case Keys.Y:
					if (!e.Control)
					{
						lowToolStripMenuItem.Checked = !lowToolStripMenuItem.Checked;
						DrawLevel();
					}
					break;
				case Keys.U:
					highToolStripMenuItem.Checked = !highToolStripMenuItem.Checked;
					DrawLevel();
					break;
				case Keys.I:
					enableGridToolStripMenuItem.Checked = !enableGridToolStripMenuItem.Checked;
					DrawLevel();
					break;
				case Keys.O:
					if (!e.Control)
					{
						hUDToolStripMenuItem.Checked = !hUDToolStripMenuItem.Checked;
						DrawLevel();
					}
					break;
				case Keys.P:
					switch (LevelData.Game.EngineVersion)
					{
						case EngineVersion.SCD:
						case EngineVersion.SCDPC:
							currentOnlyToolStripMenuItem.Checked = !currentOnlyToolStripMenuItem.Checked;
							allToolStripMenuItem.Checked = !allToolStripMenuItem.Checked;
							DrawLevel();
							break;
					}
					break;
				case Keys.OemOpenBrackets:
					LevelData.CurPal--;
					if (LevelData.CurPal == -1)
						LevelData.CurPal = LevelData.Palette.Count - 1;
					LevelData.PaletteChanged();
					DrawLevel();
					break;
				case Keys.OemCloseBrackets:
					LevelData.CurPal++;
					if (LevelData.CurPal == LevelData.Palette.Count)
						LevelData.CurPal = 0;
					LevelData.PaletteChanged();
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

		private void objectPanel_MouseDown(object sender, MouseEventArgs e)
		{
			if (!loaded) return;
			double gs = 1 << ObjGrid;
			int curx = (int)(e.X / ZoomLevel) + hScrollBar1.Value;
			int cury = (int)(e.Y / ZoomLevel) + vScrollBar1.Value;
			ushort gridx = (ushort)(Math.Round(curx / gs, MidpointRounding.AwayFromZero) * gs);
			ushort gridy = (ushort)(Math.Round(cury / gs, MidpointRounding.AwayFromZero) * gs);
			switch (e.Button)
			{
				case MouseButtons.Left:
					if (e.Clicks == 2)
					{
						if (ModifierKeys != Keys.Shift)
						{
							if (ModifierKeys != Keys.Control || LevelData.Bumpers == null)
							{
								if (typeof(ChaotixObjectEntry).IsAssignableFrom(LevelData.ObjectFormat.ObjectType))
									ObjectSelect.numericUpDown2.Maximum = 0x1FFF;
								else
									ObjectSelect.numericUpDown2.Maximum = 0xFF;
								if (ObjectSelect.ShowDialog(this) == DialogResult.OK)
								{
									ObjectEntry ent = LevelData.CreateObject((byte)ObjectSelect.numericUpDown1.Value);
									LevelData.Objects.Add(ent);
									if (ent is ChaotixObjectEntry)
										((ChaotixObjectEntry)ent).FullSubType = (ushort)ObjectSelect.numericUpDown2.Value;
									else
										ent.SubType = (byte)ObjectSelect.numericUpDown2.Value;
									ent.X = (ushort)gridx;
									ent.Y = (ushort)gridy;
									if (ent is SonicRetro.SonLVL.API.SCD.SCDObjectEntry)
									{
										SCDObjectEntry entcd = (SCDObjectEntry)ent;
										switch (LevelData.Level.TimeZone)
										{
											case API.TimeZone.Past:
												entcd.ShowPast = true;
												break;
											case API.TimeZone.Present:
												entcd.ShowPresent = true;
												break;
											case API.TimeZone.Future:
												entcd.ShowFuture = true;
												break;
										}
									}
									ent.UpdateSprite();
									SelectedItems.Clear();
									SelectedItems.Add(ent);
									SelectedObjectChanged();
									AddUndo(new ObjectAddedUndoAction(ent));
									LevelData.Objects.Sort();
									DrawLevel();
								}
							}
							else
							{
								LevelData.Bumpers.Add(new CNZBumperEntry() { X = (ushort)gridx, Y = (ushort)gridy });
								LevelData.Bumpers[LevelData.Bumpers.Count - 1].UpdateSprite();
								SelectedItems.Clear();
								SelectedItems.Add(LevelData.Bumpers[LevelData.Bumpers.Count - 1]);
								SelectedObjectChanged();
								AddUndo(new ObjectAddedUndoAction(LevelData.Bumpers[LevelData.Bumpers.Count - 1]));
								LevelData.Bumpers.Sort();
								DrawLevel();
							}
						}
						else
						{
							Entry ent = LevelData.RingFormat.CreateRing();
							ent.X = gridx;
							ent.Y = gridy;
							if (ent is ObjectEntry)
							{
								LevelData.Objects.Add((ObjectEntry)ent);
								LevelData.Objects.Sort();
							}
							else
							{
								LevelData.Rings.Add((RingEntry)ent);
								LevelData.Rings.Sort();
							}
							ent.UpdateSprite();
							SelectedItems.Clear();
							SelectedItems.Add(ent);
							SelectedObjectChanged();
							AddUndo(new ObjectAddedUndoAction(ent));
							DrawLevel();
						}
					}
					foreach (ObjectEntry item in LevelData.Objects)
					{
						ObjectDefinition dat = LevelData.GetObjectDefinition(item.ID);
						Rectangle bound = dat.GetBounds(item, Point.Empty);
						if (LevelData.ObjectVisible(item, allToolStripMenuItem.Checked) && bound.Contains(curx, cury))
						{
							if (ModifierKeys == Keys.Control)
							{
								if (SelectedItems.Contains(item))
									SelectedItems.Remove(item);
								else
									SelectedItems.Add(item);
							}
							else if (!SelectedItems.Contains(item))
							{
								SelectedItems.Clear();
								SelectedItems.Add(item);
							}
							SelectedObjectChanged();
							objdrag = true;
							DrawLevel();
							break;
						}
					}
					if (!objdrag)
						foreach (RingEntry item in LevelData.Rings)
						{
							if (((RingLayoutFormat)LevelData.RingFormat).GetBounds(item, Point.Empty).Contains(curx, cury))
							{
								if (ModifierKeys == Keys.Control)
								{
									if (SelectedItems.Contains(item))
										SelectedItems.Remove(item);
									else
										SelectedItems.Add(item);
								}
								else if (!SelectedItems.Contains(item))
								{
									SelectedItems.Clear();
									SelectedItems.Add(item);
								}
								SelectedObjectChanged();
								objdrag = true;
								DrawLevel();
								break;
							}
						}
					if (!objdrag && LevelData.Bumpers != null)
						foreach (CNZBumperEntry item in LevelData.Bumpers)
						{
							Rectangle bound = LevelData.unkobj.GetBounds(new S2ObjectEntry() { X = item.X, Y = item.Y }, Point.Empty);
							if (bound.Contains(curx, cury))
							{
								if (ModifierKeys == Keys.Control)
								{
									if (SelectedItems.Contains(item))
										SelectedItems.Remove(item);
									else
										SelectedItems.Add(item);
								}
								if (!SelectedItems.Contains(item))
								{
									SelectedItems.Clear();
									SelectedItems.Add(item);
								}
								SelectedObjectChanged();
								objdrag = true;
								DrawLevel();
								break;
							}
						}
					if (!objdrag)
						foreach (StartPositionEntry item in LevelData.StartPositions)
						{
							Rectangle bound = LevelData.StartPosDefs[LevelData.StartPositions.IndexOf(item)].GetBounds(item, Point.Empty);
							if (bound.Contains(curx, cury))
							{
								if (ModifierKeys == Keys.Control)
								{
									if (SelectedItems.Contains(item))
										SelectedItems.Remove(item);
									else
										SelectedItems.Add(item);
								}
								if (!SelectedItems.Contains(item))
								{
									SelectedItems.Clear();
									SelectedItems.Add(item);
								}
								SelectedObjectChanged();
								objdrag = true;
								DrawLevel();
								break;
							}
						}
					if (!objdrag)
					{
						selecting = true;
						selpoint = new Point(curx, cury);
						SelectedItems.Clear();
						SelectedObjectChanged();
					}
					else
					{
						locs = new List<Point>();
						foreach (Entry item in SelectedItems)
							locs.Add(new Point(item.X, item.Y));
					}
					break;
				case MouseButtons.Right:
					menuLoc = e.Location;
					foreach (ObjectEntry item in LevelData.Objects)
					{
						ObjectDefinition dat = LevelData.GetObjectDefinition(item.ID);
						Rectangle bound = dat.GetBounds(item, Point.Empty);
						if (LevelData.ObjectVisible(item, allToolStripMenuItem.Checked) && bound.Contains(curx, cury))
						{
							if (!SelectedItems.Contains(item))
							{
								SelectedItems.Clear();
								SelectedItems.Add(item);
							}
							SelectedObjectChanged();
							objdrag = true;
							DrawLevel();
							break;
						}
					}
					if (!objdrag)
						foreach (RingEntry item in LevelData.Rings)
						{
							if (((RingLayoutFormat)LevelData.RingFormat).GetBounds(item, Point.Empty).Contains(curx, cury))
							{
								if (!SelectedItems.Contains(item))
								{
									SelectedItems.Clear();
									SelectedItems.Add(item);
								}
								SelectedObjectChanged();
								objdrag = true;
								DrawLevel();
								break;
							}
						}
					if (!objdrag && LevelData.Bumpers != null)
						foreach (CNZBumperEntry item in LevelData.Bumpers)
						{
							item.UpdateSprite();
							Rectangle bound = LevelData.unkobj.GetBounds(new SonicRetro.SonLVL.API.S2.S2ObjectEntry() { X = item.X, Y = item.Y }, Point.Empty);
							if (bound.Contains(curx, cury))
							{
								if (!SelectedItems.Contains(item))
								{
									SelectedItems.Clear();
									SelectedItems.Add(item);
								}
								SelectedObjectChanged();
								objdrag = true;
								DrawLevel();
								break;
							}
						}
					if (!objdrag)
						foreach (StartPositionEntry item in LevelData.StartPositions)
						{
							Rectangle bound = LevelData.StartPosDefs[LevelData.StartPositions.IndexOf(item)].GetBounds(item, Point.Empty);
							if (bound.Contains(curx, cury))
							{
								if (!SelectedItems.Contains(item))
								{
									SelectedItems.Clear();
									SelectedItems.Add(item);
								}
								SelectedObjectChanged();
								objdrag = true;
								DrawLevel();
								break;
							}
						}
					objdrag = false;
					if (SelectedItems.Count > 0)
					{
						cutToolStripMenuItem.Enabled = true;
						copyToolStripMenuItem.Enabled = true;
						deleteToolStripMenuItem.Enabled = true;
					}
					else
					{
						cutToolStripMenuItem.Enabled = false;
						copyToolStripMenuItem.Enabled = false;
						deleteToolStripMenuItem.Enabled = false;
					}
					pasteToolStripMenuItem.Enabled = Clipboard.ContainsData(typeof(List<Entry>).AssemblyQualifiedName);
					objectContextMenuStrip.Show(objectPanel, menuLoc);
					break;
			}
		}

		private void objectPanel_MouseMove(object sender, MouseEventArgs e)
		{
			if (!loaded) return;
			Rectangle bnd = objectPanel.Bounds;
			bnd.Offset(-objectPanel.Location.X, -objectPanel.Location.Y);
			if (!bnd.Contains(objectPanel.PointToClient(Cursor.Position))) return;
			Point mouse = new Point((int)(e.X / ZoomLevel) + hScrollBar1.Value, (int)(e.Y / ZoomLevel) + vScrollBar1.Value);
			bool redraw = false;
			switch (e.Button)
			{
				case MouseButtons.Left:
					if (objdrag)
					{
						foreach (Entry item in SelectedItems)
						{
							item.X = (ushort)(item.X + (mouse.X - lastmouse.X));
							item.Y = (ushort)(item.Y + (mouse.Y - lastmouse.Y));
							item.UpdateSprite();
						}
						redraw = true;
					}
					else if (selecting)
					{
						int selobjs = SelectedItems.Count;
						SelectedItems.Clear();
						Rectangle selbnds = Rectangle.FromLTRB(
						Math.Min(selpoint.X, mouse.X),
						Math.Min(selpoint.Y, mouse.Y),
						Math.Max(selpoint.X, mouse.X),
						Math.Max(selpoint.Y, mouse.Y));
						foreach (ObjectEntry item in LevelData.Objects)
						{
							ObjectDefinition dat = LevelData.GetObjectDefinition(item.ID);
							Rectangle bound = dat.GetBounds(item, Point.Empty);
							if (LevelData.ObjectVisible(item, allToolStripMenuItem.Checked) && bound.IntersectsWith(selbnds))
								SelectedItems.Add(item);
						}
						foreach (RingEntry item in LevelData.Rings)
							if (((RingLayoutFormat)LevelData.RingFormat).GetBounds(item, Point.Empty).IntersectsWith(selbnds))
								SelectedItems.Add(item);
						if (LevelData.Bumpers != null)
							foreach (CNZBumperEntry item in LevelData.Bumpers)
							{
								Rectangle bound = LevelData.unkobj.GetBounds(new S2ObjectEntry() { X = item.X, Y = item.Y }, Point.Empty);
								if (bound.IntersectsWith(selbnds))
									SelectedItems.Add(item);
							}
						foreach (StartPositionEntry item in LevelData.StartPositions)
						{
							Rectangle bound = LevelData.StartPosDefs[LevelData.StartPositions.IndexOf(item)].GetBounds(item, Point.Empty);
							if (bound.IntersectsWith(selbnds))
								SelectedItems.Add(item);
						}
						if (selobjs != SelectedItems.Count) SelectedObjectChanged();
						redraw = true;
					}
					break;
			}
			Cursor cur = Cursors.Default;
			foreach (ObjectEntry item in LevelData.Objects)
			{
				ObjectDefinition dat = LevelData.GetObjectDefinition(item.ID);
				Rectangle bound = dat.GetBounds(item, Point.Empty);
				if (LevelData.ObjectVisible(item, allToolStripMenuItem.Checked) && bound.Contains(mouse))
				{
					cur = Cursors.SizeAll;
					break;
				}
			}
			foreach (RingEntry item in LevelData.Rings)
				if (((RingLayoutFormat)LevelData.RingFormat).GetBounds(item, Point.Empty).Contains(mouse))
				{
					cur = Cursors.SizeAll;
					break;
				}
			if (LevelData.Bumpers != null)
				foreach (CNZBumperEntry item in LevelData.Bumpers)
				{
					Rectangle bound = LevelData.unkobj.GetBounds(new S2ObjectEntry() { X = item.X, Y = item.Y }, Point.Empty);
					if (bound.Contains(mouse))
					{
						cur = Cursors.SizeAll;
						break;
					}
				}
			foreach (StartPositionEntry item in LevelData.StartPositions)
			{
				Rectangle bound = LevelData.StartPosDefs[LevelData.StartPositions.IndexOf(item)].GetBounds(item, Point.Empty);
				if (bound.Contains(mouse))
				{
					cur = Cursors.SizeAll;
					break;
				}
			}
			objectPanel.Cursor = cur;
			if (redraw) DrawLevel();
			lastmouse = mouse;
		}

		private void objectPanel_MouseUp(object sender, MouseEventArgs e)
		{
			if (objdrag)
			{
				double gs = 1 << ObjGrid;
				foreach (Entry item in SelectedItems)
				{
					item.X = (ushort)(Math.Round(item.X / gs, MidpointRounding.AwayFromZero) * gs);
					item.Y = (ushort)(Math.Round(item.Y / gs, MidpointRounding.AwayFromZero) * gs);
					item.UpdateSprite();
				}
				bool moved = false;
				for (int i = 0; i < SelectedItems.Count; i++)
					if (SelectedItems[i].X != locs[i].X | SelectedItems[i].Y != locs[i].Y)
						moved = true;
				if (moved)
					AddUndo(new ObjectMoveUndoAction(new List<Entry>(SelectedItems), locs));
				ObjectProperties.SelectedObjects = SelectedItems.ToArray();
				LevelData.Objects.Sort();
				LevelData.Rings.Sort();
				if (LevelData.Bumpers != null) LevelData.Bumpers.Sort();
			}
			objdrag = false;
			selecting = false;
			DrawLevel();
		}

		private void foregroundPanel_MouseDown(object sender, MouseEventArgs e)
		{
			if (!loaded) return;
			Point chunkpoint = new Point(((int)(e.X / ZoomLevel) + hScrollBar2.Value) / LevelData.Level.ChunkWidth, ((int)(e.Y / ZoomLevel) + vScrollBar2.Value) / LevelData.Level.ChunkHeight);
			if (chunkpoint.X >= LevelData.FGWidth | chunkpoint.Y >= LevelData.FGHeight) return;
			switch (e.Button)
			{
				case MouseButtons.Left:
					FGSelection = Rectangle.Empty;
					if (LevelData.LayoutFormat.HasLoopFlag && e.Clicks >= 2)
						LevelData.Layout.FGLoop[chunkpoint.X, chunkpoint.Y] = !LevelData.Layout.FGLoop[chunkpoint.X, chunkpoint.Y];
					else
					{
						locs = new List<Point>();
						tiles = new List<byte>();
						byte t = LevelData.Layout.FGLayout[chunkpoint.X, chunkpoint.Y];
						if (t != SelectedChunk)
						{
							locs.Add(chunkpoint);
							tiles.Add(t);
							LevelData.Layout.FGLayout[chunkpoint.X, chunkpoint.Y] = SelectedChunk;
							if (LevelData.LayoutFormat.HasLoopFlag)
								LevelData.Layout.FGLoop[chunkpoint.X, chunkpoint.Y] = LevelData.Level.LoopChunks.Contains(SelectedChunk);
						}
					}
					DrawLevel();
					break;
				case MouseButtons.Right:
					menuLoc = chunkpoint;
					if (!FGSelection.Contains(chunkpoint))
					{
						FGSelection = Rectangle.Empty;
						DrawLevel();
					}
					break;
			}
		}

		private void foregroundPanel_MouseMove(object sender, MouseEventArgs e)
		{
			if (!loaded) return;
			Rectangle bnd = foregroundPanel.Bounds;
			bnd.Offset(-foregroundPanel.Location.X, -foregroundPanel.Location.Y);
			if (!bnd.Contains(foregroundPanel.PointToClient(Cursor.Position))) return;
			Point mouse = new Point((int)(e.X / ZoomLevel) + hScrollBar2.Value, (int)(e.Y / ZoomLevel) + vScrollBar2.Value);
			Point chunkpoint = new Point(mouse.X / LevelData.Level.ChunkWidth, mouse.Y / LevelData.Level.ChunkHeight);
			if (chunkpoint.X >= LevelData.FGWidth | chunkpoint.Y >= LevelData.FGHeight) return;
			switch (e.Button)
			{
				case MouseButtons.Left:
						byte t = LevelData.Layout.FGLayout[chunkpoint.X, chunkpoint.Y];
						if (t != SelectedChunk)
						{
							locs.Add(chunkpoint);
							tiles.Add(t);
							LevelData.Layout.FGLayout[chunkpoint.X, chunkpoint.Y] = SelectedChunk;
							if (LevelData.LayoutFormat.HasLoopFlag)
								LevelData.Layout.FGLoop[chunkpoint.X, chunkpoint.Y] = LevelData.Level.LoopChunks.Contains(SelectedChunk);
							DrawLevel();
						}
					break;
				case MouseButtons.Right:
					selecting = true;
					if (FGSelection.IsEmpty)
						FGSelection = new Rectangle(chunkpoint, new Size(1, 1));
					else
						FGSelection = Rectangle.FromLTRB(Math.Min(FGSelection.Left, chunkpoint.X), Math.Min(FGSelection.Top, chunkpoint.Y), Math.Max(FGSelection.Right, chunkpoint.X + 1), Math.Max(FGSelection.Bottom, chunkpoint.Y + 1));
					DrawLevel();
					break;
				default:
					if (chunkpoint != lastchunkpoint)
						DrawLevel();
					break;
			}
			lastchunkpoint = chunkpoint;
			lastmouse = mouse;
		}

		private void foregroundPanel_MouseUp(object sender, MouseEventArgs e)
		{
			switch (e.Button)
			{
				case MouseButtons.Left:
					if (locs.Count > 0) AddUndo(new LayoutEditUndoAction(1, locs, tiles));
					DrawLevel();
					break;
				case MouseButtons.Right:
					Point mouse = new Point((int)(e.X / ZoomLevel) + hScrollBar2.Value, (int)(e.Y / ZoomLevel) + vScrollBar2.Value);
					Point chunkpoint = new Point(mouse.X / LevelData.Level.ChunkWidth, mouse.Y / LevelData.Level.ChunkHeight);
					if (FGSelection.IsEmpty)
					{
						SelectedChunk = LevelData.Layout.FGLayout[chunkpoint.X, chunkpoint.Y];
						if (SelectedChunk < LevelData.Chunks.Count)
							ChunkSelector.SelectedIndex = SelectedChunk;
						DrawLevel();
					}
					else if (!selecting)
					{
						pasteOnceToolStripMenuItem.Enabled = pasteRepeatingToolStripMenuItem.Enabled = Clipboard.ContainsData(typeof(LayoutSection).AssemblyQualifiedName);
						pasteSectionOnceToolStripMenuItem.Enabled = pasteSectionRepeatingToolStripMenuItem.Enabled = layoutSectionListBox.SelectedIndex != -1;
						layoutContextMenuStrip.Show(foregroundPanel, e.Location);
					}
					selecting = false;
					break;
			}
		}

		private void backgroundPanel_MouseDown(object sender, MouseEventArgs e)
		{
			if (!loaded) return;
			Point chunkpoint = new Point(((int)(e.X / ZoomLevel) + hScrollBar3.Value) / LevelData.Level.ChunkWidth, ((int)(e.Y / ZoomLevel) + vScrollBar3.Value) / LevelData.Level.ChunkHeight);
			if (chunkpoint.X >= LevelData.BGWidth | chunkpoint.Y >= LevelData.BGHeight) return;
			switch (e.Button)
			{
				case MouseButtons.Left:
					BGSelection = Rectangle.Empty;
					if (LevelData.LayoutFormat.HasLoopFlag && e.Clicks >= 2)
						LevelData.Layout.BGLoop[chunkpoint.X, chunkpoint.Y] = !LevelData.Layout.BGLoop[chunkpoint.X, chunkpoint.Y];
					else
					{
						locs = new List<Point>();
						tiles = new List<byte>();
						byte t = LevelData.Layout.BGLayout[chunkpoint.X, chunkpoint.Y];
						if (t != SelectedChunk)
						{
							locs.Add(chunkpoint);
							tiles.Add(t);
							LevelData.Layout.BGLayout[chunkpoint.X, chunkpoint.Y] = SelectedChunk;
							if (LevelData.LayoutFormat.HasLoopFlag)
								LevelData.Layout.BGLoop[chunkpoint.X, chunkpoint.Y] = LevelData.Level.LoopChunks.Contains(SelectedChunk);
						}
					}
					DrawLevel();
					break;
				case MouseButtons.Right:
					menuLoc = chunkpoint;
					if (!BGSelection.Contains(chunkpoint))
					{
						BGSelection = Rectangle.Empty;
						DrawLevel();
					}
					break;
			}
		}

		private void backgroundPanel_MouseMove(object sender, MouseEventArgs e)
		{
			if (!loaded) return;
			Rectangle bnd = backgroundPanel.Bounds;
			bnd.Offset(-backgroundPanel.Location.X, -backgroundPanel.Location.Y);
			if (!bnd.Contains(backgroundPanel.PointToClient(Cursor.Position))) return;
			Point mouse = new Point((int)(e.X / ZoomLevel) + hScrollBar3.Value, (int)(e.Y / ZoomLevel) + vScrollBar3.Value);
			Point chunkpoint = new Point(mouse.X / LevelData.Level.ChunkWidth, mouse.Y / LevelData.Level.ChunkHeight);
			if (chunkpoint.X >= LevelData.BGWidth | chunkpoint.Y >= LevelData.BGHeight) return;
			switch (e.Button)
			{
				case MouseButtons.Left:
					byte t = LevelData.Layout.BGLayout[chunkpoint.X, chunkpoint.Y];
					if (t != SelectedChunk)
					{
						locs.Add(chunkpoint);
						tiles.Add(t);
						LevelData.Layout.BGLayout[chunkpoint.X, chunkpoint.Y] = SelectedChunk;
						if (LevelData.LayoutFormat.HasLoopFlag)
							LevelData.Layout.BGLoop[chunkpoint.X, chunkpoint.Y] = LevelData.Level.LoopChunks.Contains(SelectedChunk);
						DrawLevel();
					}
					break;
				case MouseButtons.Right:
					selecting = true;
					if (BGSelection.IsEmpty)
						BGSelection = new Rectangle(chunkpoint, new Size(1, 1));
					else
						BGSelection = Rectangle.FromLTRB(Math.Min(BGSelection.Left, chunkpoint.X), Math.Min(BGSelection.Top, chunkpoint.Y), Math.Max(BGSelection.Right, chunkpoint.X + 1), Math.Max(BGSelection.Bottom, chunkpoint.Y + 1));
					DrawLevel();
					break;
				default:
					if (chunkpoint != lastchunkpoint)
						DrawLevel();
					break;
			}
			lastchunkpoint = chunkpoint;
			lastmouse = mouse;
		}

		private void backgroundPanel_MouseUp(object sender, MouseEventArgs e)
		{
			switch (e.Button)
			{
				case MouseButtons.Left:
					if (locs.Count > 0) AddUndo(new LayoutEditUndoAction(2, locs, tiles));
					DrawLevel();
					break;
				case MouseButtons.Right:
					Point mouse = new Point((int)(e.X / ZoomLevel) + hScrollBar3.Value, (int)(e.Y / ZoomLevel) + vScrollBar3.Value);
					Point chunkpoint = new Point(mouse.X / LevelData.Level.ChunkWidth, mouse.Y / LevelData.Level.ChunkHeight);
					if (BGSelection.IsEmpty)
					{
						SelectedChunk = LevelData.Layout.BGLayout[chunkpoint.X, chunkpoint.Y];
						if (SelectedChunk < LevelData.Chunks.Count)
							ChunkSelector.SelectedIndex = SelectedChunk;
						DrawLevel();
					}
					else if (!selecting)
					{
						pasteOnceToolStripMenuItem.Enabled = pasteRepeatingToolStripMenuItem.Enabled = Clipboard.ContainsData(typeof(LayoutSection).AssemblyQualifiedName);
						pasteSectionOnceToolStripMenuItem.Enabled = pasteSectionRepeatingToolStripMenuItem.Enabled = layoutSectionListBox.SelectedIndex != -1;
						layoutContextMenuStrip.Show(backgroundPanel, e.Location);
					}
					selecting = false;
					break;
			}
		}

		private void ChunkSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (ChunkSelector.SelectedIndex == -1 || ChunkSelector.SelectedIndex >= LevelData.Chunks.Count) return;
			importChunksToolStripButton.Enabled = LevelData.Chunks.Count < 256;
			drawChunkToolStripButton.Enabled = importChunksToolStripButton.Enabled;
			SelectedChunk = (byte)ChunkSelector.SelectedIndex;
			SelectedChunkBlock = new Rectangle(0, 0, 1, 1);
			chunkBlockEditor.SelectedObjects = new[] { LevelData.Chunks[SelectedChunk].Blocks[0, 0] };
			ChunkPicture.Invalidate();
			ChunkID.Text = SelectedChunk.ToString("X2");
			ChunkCount.Text = LevelData.Chunks.Count.ToString("X") + " / 100";
			DrawLevel();
		}

		private void SelectedObjectChanged()
		{
			ObjectProperties.SelectedObjects = SelectedItems.ToArray();
			alignLeftWallToolStripButton.Enabled = alignRightWallToolStripButton.Enabled = alignGroundToolStripButton.Enabled =
				alignCeilingToolStripButton.Enabled = SelectedItems.Count > 0;
			alignBottomsToolStripButton.Enabled = alignCentersToolStripButton.Enabled = alignLeftsToolStripButton.Enabled =
				alignMiddlesToolStripButton.Enabled = alignRightsToolStripButton.Enabled = alignTopsToolStripButton.Enabled =
				SelectedItems.Count > 1;
		}

		private void ScrollBar_ValueChanged(object sender, EventArgs e)
		{
			if (!loaded) return;
			loaded = false;
			switch (CurrentTab)
			{
				case Tab.Objects:
					hScrollBar2.Value = Math.Min(hScrollBar1.Value, hScrollBar2.Maximum);
					vScrollBar2.Value = Math.Min(vScrollBar1.Value, vScrollBar2.Maximum);
					break;
				case Tab.Foreground:
					hScrollBar1.Value = Math.Min(hScrollBar2.Value, hScrollBar1.Maximum);
					vScrollBar1.Value = Math.Min(vScrollBar2.Value, vScrollBar1.Maximum);
					break;
			}
			loaded = true;
			DrawLevel();
		}

		private void panel_Resize(object sender, EventArgs e)
		{
			Panel1Gfx = objectPanel.CreateGraphics();
			Panel1Gfx.SetOptions();
			Panel2Gfx = foregroundPanel.CreateGraphics();
			Panel2Gfx.SetOptions();
			Panel3Gfx = backgroundPanel.CreateGraphics();
			Panel3Gfx.SetOptions();
			if (!loaded) return;
			loaded = false;
			UpdateScrollBars();
			loaded = true;
			DrawLevel();
		}

		Point menuLoc;
		private void addObjectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (LevelData.Level.ObjectFormat == EngineVersion.Chaotix)
				ObjectSelect.numericUpDown2.Maximum = 0x1FFF;
			else
				ObjectSelect.numericUpDown2.Maximum = 0xFF;
			if (ObjectSelect.ShowDialog(this) == DialogResult.OK)
			{
				ObjectEntry ent = LevelData.CreateObject((byte)ObjectSelect.numericUpDown1.Value);
				LevelData.Objects.Add(ent);
				if (ent is ChaotixObjectEntry)
					((ChaotixObjectEntry)ent).FullSubType = (ushort)ObjectSelect.numericUpDown2.Value;
				else
					ent.SubType = (byte)ObjectSelect.numericUpDown2.Value;
				double gs = 1 << ObjGrid;
				ent.X = (ushort)(Math.Round((menuLoc.X * ZoomLevel + hScrollBar1.Value) / gs, MidpointRounding.AwayFromZero) * gs);
				ent.Y = (ushort)(Math.Round((menuLoc.Y * ZoomLevel + vScrollBar1.Value) / gs, MidpointRounding.AwayFromZero) * gs);
				if (ent is SCDObjectEntry)
				{
					SCDObjectEntry entcd = (SCDObjectEntry)ent;
					switch (LevelData.Level.TimeZone)
					{
						case API.TimeZone.Past:
							entcd.ShowPast = true;
							break;
						case API.TimeZone.Present:
							entcd.ShowPresent = true;
							break;
						case API.TimeZone.Future:
							entcd.ShowFuture = true;
							break;
					}
				}
				ent.UpdateSprite();
				SelectedItems.Clear();
				SelectedItems.Add(ent);
				SelectedObjectChanged();
				AddUndo(new ObjectAddedUndoAction(ent));
				LevelData.Objects.Sort();
				DrawLevel();
			}
		}

		private void addRingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			double gs = 1 << ObjGrid;
			Entry ent = LevelData.RingFormat.CreateRing();
			ent.X = (ushort)(Math.Round((menuLoc.X * ZoomLevel + hScrollBar1.Value) / gs, MidpointRounding.AwayFromZero) * gs);
			ent.Y = (ushort)(Math.Round((menuLoc.Y * ZoomLevel + vScrollBar1.Value) / gs, MidpointRounding.AwayFromZero) * gs);
			if (ent is ObjectEntry)
			{
				LevelData.Objects.Add((ObjectEntry)ent);
				LevelData.Objects.Sort();
			}
			else
			{
				LevelData.Rings.Add((RingEntry)ent);
				LevelData.Rings.Sort();
			}
			ent.UpdateSprite();
			SelectedItems.Clear();
			SelectedItems.Add(ent);
			SelectedObjectChanged();
			AddUndo(new ObjectAddedUndoAction(ent));
			DrawLevel();
		}

		private void addGroupOfObjectsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (ObjectSelect.ShowDialog(this) == DialogResult.OK)
			{
				byte ID = (byte)ObjectSelect.numericUpDown1.Value;
				byte sub = (byte)ObjectSelect.numericUpDown2.Value;
				using (AddGroupDialog dlg = new AddGroupDialog())
				{
					ObjectEntry tmp = LevelData.ObjectFormat.CreateObject();
					tmp.SubType = sub;
					Rectangle bnd = LevelData.GetObjectDefinition(ID).GetBounds(tmp, Point.Empty);
					dlg.Text = "Add Group of Objects";
					dlg.XDist.Value = bnd.Width;
					dlg.YDist.Value = bnd.Height;
					if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
					{
						double gs = 1 << ObjGrid;
						Point pt = new Point(
							(ushort)(Math.Round((menuLoc.X * ZoomLevel + hScrollBar1.Value) / gs, MidpointRounding.AwayFromZero) * gs),
							(ushort)(Math.Round((menuLoc.Y * ZoomLevel + vScrollBar1.Value) / gs, MidpointRounding.AwayFromZero) * gs)
							);
						int xst = pt.X;
						Size xsz = new Size((int)dlg.XDist.Value, 0);
						Size ysz = new Size(0, (int)dlg.YDist.Value);
						SelectedItems.Clear();
						for (int y = 0; y < dlg.Rows.Value; y++)
						{
							for (int x = 0; x < dlg.Columns.Value; x++)
							{
								ObjectEntry ent = LevelData.CreateObject(ID);
								LevelData.Objects.Add(ent);
								ent.SubType = sub;
								ent.X = (ushort)(pt.X);
								ent.Y = (ushort)(pt.Y);
								if (ent is SCDObjectEntry)
								{
									SCDObjectEntry entcd = (SCDObjectEntry)ent;
									switch (LevelData.Level.TimeZone)
									{
										case API.TimeZone.Past:
											entcd.ShowPast = true;
											break;
										case API.TimeZone.Present:
											entcd.ShowPresent = true;
											break;
										case API.TimeZone.Future:
											entcd.ShowFuture = true;
											break;
									}
								}
								ent.UpdateSprite();
								SelectedItems.Add(ent);
								pt += xsz;
							}
							pt.X = xst;
							pt += ysz;
						}
						SelectedObjectChanged();
						LevelData.Objects.Sort();
						DrawLevel();
					}
				}
			}
		}

		private void addGroupOfRingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (AddGroupDialog dlg = new AddGroupDialog())
			{
				dlg.Text = "Add Group of Rings";
				dlg.XDist.Value = 24;
				dlg.YDist.Value = 24;
				if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{
					double gs = 1 << ObjGrid;
					Point pt = new Point(
						(ushort)(Math.Round((menuLoc.X * ZoomLevel + hScrollBar1.Value) / gs, MidpointRounding.AwayFromZero) * gs),
						(ushort)(Math.Round((menuLoc.Y * ZoomLevel + vScrollBar1.Value) / gs, MidpointRounding.AwayFromZero) * gs)
						);
					int xst = pt.X;
					Size xsz = new Size((int)dlg.XDist.Value, 0);
					Size ysz = new Size(0, (int)dlg.YDist.Value);
					SelectedItems.Clear();
					for (int y = 0; y < dlg.Rows.Value; y++)
					{
						for (int x = 0; x < dlg.Columns.Value; x++)
						{
							Entry ent = LevelData.RingFormat.CreateRing();
							ent.X = (ushort)pt.X;
							ent.Y = (ushort)pt.Y;
							if (ent is ObjectEntry)
								LevelData.Objects.Add((ObjectEntry)ent);
							else
								LevelData.Rings.Add((RingEntry)ent);
							ent.UpdateSprite();
							SelectedItems.Add(ent);
							pt += xsz;
						}
						pt.X = xst;
						pt += ysz;
					}
					SelectedObjectChanged();
					if (LevelData.RingFormat is RingObjectFormat)
						LevelData.Objects.Sort();
					else
						LevelData.Rings.Sort();
					DrawLevel();
				}
			}
		}

		private void cutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<Entry> selitems = new List<Entry>();
			foreach (Entry item in SelectedItems)
			{
				if (item is ObjectEntry)
				{
					LevelData.Objects.Remove((ObjectEntry)item);
					selitems.Add(item);
				}
				else if (item is RingEntry)
				{
					LevelData.Rings.Remove((RingEntry)item);
					selitems.Add(item);
				}
				else if (item is CNZBumperEntry)
				{
					LevelData.Bumpers.Remove((CNZBumperEntry)item);
					selitems.Add(item);
				}
			}
			if (selitems.Count == 0) return;
			Clipboard.SetData(typeof(List<Entry>).AssemblyQualifiedName, selitems);
			AddUndo(new ObjectsDeletedUndoAction(selitems));
			SelectedItems.Clear();
			SelectedObjectChanged();
			DrawLevel();
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<Entry> selitems = new List<Entry>();
			foreach (Entry item in SelectedItems)
			{
				if (item is ObjectEntry)
					selitems.Add(item);
				else if (item is RingEntry)
					selitems.Add(item);
				else if (item is CNZBumperEntry)
					selitems.Add(item);
			}
			if (selitems.Count == 0) return;
			Clipboard.SetData(typeof(List<Entry>).AssemblyQualifiedName, selitems);
		}

		private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<Entry> objs = Clipboard.GetData(typeof(List<Entry>).AssemblyQualifiedName) as List<Entry>;
			Point upleft = new Point(int.MaxValue, int.MaxValue);
			foreach (Entry item in objs)
			{
				upleft.X = Math.Min(upleft.X, item.X);
				upleft.Y = Math.Min(upleft.Y, item.Y);
			}
			Size off = new Size(((int)(menuLoc.X / ZoomLevel) + hScrollBar1.Value) - upleft.X, ((int)(menuLoc.Y / ZoomLevel) + vScrollBar1.Value) - upleft.Y);
			SelectedItems = new List<Entry>(objs);
			double gs = 1 << ObjGrid;
			foreach (Entry item in objs)
			{
				item.X += (ushort)off.Width;
				item.Y += (ushort)off.Height;
				item.X = (ushort)(Math.Round(item.X / gs, MidpointRounding.AwayFromZero) * gs);
				item.Y = (ushort)(Math.Round(item.Y / gs, MidpointRounding.AwayFromZero) * gs);
				item.ResetPos();
				if (item is ObjectEntry)
					LevelData.Objects.Add((ObjectEntry)item);
				else if (item is RingEntry)
					LevelData.Rings.Add((RingEntry)item);
				else if (item is CNZBumperEntry)
					LevelData.Bumpers.Add((CNZBumperEntry)item);
				item.UpdateSprite();
			}
			AddUndo(new ObjectsPastedUndoAction(new List<Entry>(SelectedItems)));
			SelectedObjectChanged();
			LevelData.Objects.Sort();
			LevelData.Rings.Sort();
			if (LevelData.Bumpers != null) LevelData.Bumpers.Sort();
			DrawLevel();
		}

		private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<Entry> selitems = new List<Entry>();
			foreach (Entry item in SelectedItems)
			{
				if (item is ObjectEntry)
				{
					LevelData.Objects.Remove((ObjectEntry)item);
					selitems.Add(item);
				}
				else if (item is RingEntry)
				{
					LevelData.Rings.Remove((RingEntry)item);
					selitems.Add(item);
				}
				else if (item is CNZBumperEntry)
				{
					LevelData.Bumpers.Remove((CNZBumperEntry)item);
					selitems.Add(item);
				}
			}
			if (selitems.Count == 0) return;
			AddUndo(new ObjectsDeletedUndoAction(selitems));
			SelectedItems.Clear();
			SelectedObjectChanged();
			DrawLevel();
		}

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
		{
			selecting = false;
			switch (CurrentTab)
			{
				case Tab.Objects:
					findToolStripMenuItem.Enabled = true;
					findNextToolStripMenuItem.Enabled = findPreviousToolStripMenuItem.Enabled = foundobjs == null;
					objectPanel.Focus();
					break;
				case Tab.Foreground:
					findToolStripMenuItem.Enabled = true;
					findNextToolStripMenuItem.Enabled = findPreviousToolStripMenuItem.Enabled = lastfoundfgchunk.HasValue;
					tabPage8.Controls.Add(ChunkSelector);
					tabPage9.Controls.Add(layoutSectionSplitContainer);
					ChunkSelector.AllowDrop = false;
					foregroundPanel.Focus();
					break;
				case Tab.Background:
					findToolStripMenuItem.Enabled = true;
					findNextToolStripMenuItem.Enabled = findPreviousToolStripMenuItem.Enabled = lastfoundbgchunk.HasValue;
					tabPage10.Controls.Add(ChunkSelector);
					tabPage11.Controls.Add(layoutSectionSplitContainer);
					ChunkSelector.AllowDrop = false;
					backgroundPanel.Focus();
					break;
				case Tab.Art:
					findToolStripMenuItem.Enabled = findNextToolStripMenuItem.Enabled = findPreviousToolStripMenuItem.Enabled = false;
					panel10.Controls.Add(ChunkSelector);
					ChunkSelector.AllowDrop = true;
					break;
				default:
					findToolStripMenuItem.Enabled = findNextToolStripMenuItem.Enabled = findPreviousToolStripMenuItem.Enabled = false;
					break;
			}
			DrawLevel();
		}

		int SelectedBlock, SelectedTile;
		Rectangle SelectedChunkBlock, SelectedBlockTile;
		Point SelectedColor;
		PatternIndex copiedBlockTile = new PatternIndex();
		ChunkBlock copiedChunkBlock;

		private void ChunkPicture_MouseMove(object sender, MouseEventArgs e)
		{
			if (!loaded) return;
			if (e.X > 0 && e.Y > 0 && e.X < LevelData.Level.ChunkWidth && e.Y < LevelData.Level.ChunkHeight)
				if (e.Button == chunkblockMouseDraw)
				{
					SelectedChunkBlock = new Rectangle(e.X / 16, e.Y / 16, 1, 1);
					ChunkBlock destBlock = LevelData.Chunks[SelectedChunk].Blocks[SelectedChunkBlock.X, SelectedChunkBlock.Y];
					destBlock.Block = copiedChunkBlock.Block;
					destBlock.Solid1 = copiedChunkBlock.Solid1;
					destBlock.XFlip = copiedChunkBlock.XFlip;
					destBlock.YFlip = copiedChunkBlock.YFlip;
					if (copiedChunkBlock is S2ChunkBlock)
						((S2ChunkBlock)destBlock).Solid2 = ((S2ChunkBlock)copiedChunkBlock).Solid2;
					chunkBlockEditor.SelectedObjects = new[] { destBlock };
					LevelData.RedrawChunk(SelectedChunk);
					DrawChunkPicture();
					ChunkSelector.Invalidate();
				}
				else if (e.Button == chunkblockMouseSelect)
				{
					SelectedChunkBlock = Rectangle.FromLTRB(Math.Min(SelectedChunkBlock.Left, e.X / 16), Math.Min(SelectedChunkBlock.Top, e.Y / 16), Math.Max(SelectedChunkBlock.Right, e.X / 16 + 1), Math.Max(SelectedChunkBlock.Bottom, e.Y / 16 + 1));
					copiedChunkBlock = (chunkBlockEditor.SelectedObjects = GetSelectedChunkBlocks())[0];
					DrawChunkPicture();
				}
		}

		private ChunkBlock[] GetSelectedChunkBlocks()
		{
			ChunkBlock[] blocks = new ChunkBlock[SelectedChunkBlock.Width * SelectedChunkBlock.Height];
			int i = 0;
			for (int y = SelectedChunkBlock.Top; y < SelectedChunkBlock.Bottom; y++)
				for (int x = SelectedChunkBlock.Left; x < SelectedChunkBlock.Right; x++)
					blocks[i++] = LevelData.Chunks[SelectedChunk].Blocks[x, y];
			return blocks;
		}

		private void ChunkPicture_MouseDown(object sender, MouseEventArgs e)
		{
			if (!loaded) return;
			if (e.Button == chunkblockMouseDraw)
				ChunkPicture_MouseMove(sender, e);
			else if (e.Button == chunkblockMouseSelect)
			{
				SelectedChunkBlock = new Rectangle(e.X / 16, e.Y / 16, 1, 1);
				copiedChunkBlock = LevelData.Chunks[SelectedChunk].Blocks[SelectedChunkBlock.X, SelectedChunkBlock.Y];
				if (copiedChunkBlock.Block < LevelData.Blocks.Count)
					BlockSelector.SelectedIndex = copiedChunkBlock.Block;
				chunkBlockEditor.SelectedObjects = new[] { copiedChunkBlock };
				DrawChunkPicture();
				ChunkSelector.Invalidate();
			}
		}

		private void ChunkPicture_MouseUp(object sender, MouseEventArgs e)
		{
			if (loaded && e.Button == chunkblockMouseDraw)
			{
				LevelData.RedrawChunk(SelectedChunk);
				DrawLevel();
				DrawChunkPicture();
				ChunkSelector.Invalidate();
			}
		}

		private void ChunkPicture_KeyDown(object sender, KeyEventArgs e)
		{
			if (!loaded) return;
			ChunkBlock[] blocks = GetSelectedChunkBlocks();
			switch (e.KeyCode)
			{
				case Keys.B:
					foreach (ChunkBlock item in blocks)
						if (e.Shift)
							item.Block = (ushort)(item.Block == 0 ? LevelData.Blocks.Count - 1 : item.Block - 1);
						else
							item.Block = (ushort)((item.Block + 1) % LevelData.Blocks.Count);
					break;
				case Keys.Down:
					if (SelectedChunkBlock.Y < (LevelData.Level.ChunkHeight / 16) - 1)
					{
						SelectedChunkBlock = new Rectangle(SelectedChunkBlock.X, SelectedChunkBlock.Y + 1, 1, 1);
						copiedChunkBlock = LevelData.Chunks[SelectedChunk].Blocks[SelectedChunkBlock.X, SelectedChunkBlock.Y];
						blocks = new[] { copiedChunkBlock };
						if (copiedChunkBlock.Block < LevelData.Blocks.Count)
							BlockSelector.SelectedIndex = copiedChunkBlock.Block;
					}
					else
						return;
					break;
				case Keys.Left:
					if (SelectedChunkBlock.X > 0)
					{
						SelectedChunkBlock = new Rectangle(SelectedChunkBlock.X - 1, SelectedChunkBlock.Y, 1, 1);
						copiedChunkBlock = LevelData.Chunks[SelectedChunk].Blocks[SelectedChunkBlock.X, SelectedChunkBlock.Y];
						blocks = new[] { copiedChunkBlock };
						if (copiedChunkBlock.Block < LevelData.Blocks.Count)
							BlockSelector.SelectedIndex = copiedChunkBlock.Block;
					}
					else
						return;
					break;
				case Keys.Right:
					if (SelectedChunkBlock.X < (LevelData.Level.ChunkWidth / 16) - 1)
					{
						SelectedChunkBlock = new Rectangle(SelectedChunkBlock.X + 1, SelectedChunkBlock.Y, 1, 1);
						copiedChunkBlock = LevelData.Chunks[SelectedChunk].Blocks[SelectedChunkBlock.X, SelectedChunkBlock.Y];
						blocks = new[] { copiedChunkBlock };
						if (copiedChunkBlock.Block < LevelData.Blocks.Count)
							BlockSelector.SelectedIndex = copiedChunkBlock.Block;
					}
					else
						return;
					break;
				case Keys.S:
					foreach (ChunkBlock item in blocks)
						if (e.Shift)
							item.Solid1--;
						else
							item.Solid1++;
					break;
				case Keys.T:
					if (!(blocks[0] is S2ChunkBlock))
						return;
					foreach (ChunkBlock item in blocks)
					{
						S2ChunkBlock cur2 = (S2ChunkBlock)item;
						if (e.Shift)
							cur2.Solid2--;
						else
							cur2.Solid2++;
					}
					break;
				case Keys.Up:
					if (SelectedChunkBlock.Y > 0)
					{
						SelectedChunkBlock = new Rectangle(SelectedChunkBlock.X, SelectedChunkBlock.Y - 1, 1, 1);
						copiedChunkBlock = LevelData.Chunks[SelectedChunk].Blocks[SelectedChunkBlock.X, SelectedChunkBlock.Y];
						blocks = new[] { copiedChunkBlock };
						if (copiedChunkBlock.Block < LevelData.Blocks.Count)
							BlockSelector.SelectedIndex = copiedChunkBlock.Block;
					}
					else
						return;
					break;
				case Keys.X:
					foreach (ChunkBlock item in blocks)
						item.XFlip = !item.XFlip;
					break;
				case Keys.Y:
					foreach (ChunkBlock item in blocks)
						item.YFlip = !item.YFlip;
					break;
				default:
					return;
			}
			LevelData.RedrawChunk(SelectedChunk);
			DrawLevel();
			DrawChunkPicture();
			ChunkSelector.Invalidate();
			chunkBlockEditor.SelectedObjects = blocks;
		}

		private void chunkBlockEditor_PropertyValueChanged(object sender, EventArgs e)
		{
			LevelData.RedrawChunk(SelectedChunk);
			DrawLevel();
			DrawChunkPicture();
		}

		private void DrawChunkPicture()
		{
			if (!loaded) return;
			BitmapBits bmp = new BitmapBits(LevelData.Level.ChunkWidth, LevelData.Level.ChunkHeight);
			bmp.Bits.FastFill(0x20);
			if (lowToolStripMenuItem.Checked)
				bmp.DrawBitmap(LevelData.ChunkBmpBits[SelectedChunk][0], 0, 0);
			if (highToolStripMenuItem.Checked)
				bmp.DrawBitmapComposited(LevelData.ChunkBmpBits[SelectedChunk][1], 0, 0);
			if (path1ToolStripMenuItem.Checked)
				bmp.DrawBitmapComposited(LevelData.ChunkColBmpBits[SelectedChunk][0], 0, 0);
			if (path2ToolStripMenuItem.Checked)
				bmp.DrawBitmapComposited(LevelData.ChunkColBmpBits[SelectedChunk][1], 0, 0);
			bmp.DrawRectangle(LevelData.ColorWhite, SelectedChunkBlock.X * 16 - 1, SelectedChunkBlock.Y * 16 - 1, SelectedChunkBlock.Width * 16 + 1, SelectedChunkBlock.Height * 16 + 1);
			using (Graphics gfx = ChunkPicture.CreateGraphics())
			{
				gfx.SetOptions();
				gfx.DrawImage(bmp.ToBitmap(LevelImgPalette), 0, 0, LevelData.Level.ChunkWidth, LevelData.Level.ChunkHeight);
			}
		}

		private void ChunkPicture_Paint(object sender, PaintEventArgs e)
		{
			DrawChunkPicture();
		}

		private void flipChunkHButton_Click(object sender, EventArgs e)
		{
			Chunk newcnk = LevelData.Chunks[SelectedChunk].Flip(true, false);
			LevelData.Chunks[SelectedChunk] = newcnk;
			LevelData.RedrawChunk(SelectedChunk);
			copiedChunkBlock = (chunkBlockEditor.SelectedObjects = GetSelectedChunkBlocks())[0];
			if (newcnk.Blocks[SelectedChunkBlock.X, SelectedChunkBlock.Y].Block < LevelData.Blocks.Count)
				BlockSelector.SelectedIndex = newcnk.Blocks[SelectedChunkBlock.X, SelectedChunkBlock.Y].Block;
			DrawChunkPicture();
			ChunkSelector.Invalidate();
		}

		private void flipChunkVButton_Click(object sender, EventArgs e)
		{
			Chunk newcnk = LevelData.Chunks[SelectedChunk].Flip(false, true);
			LevelData.Chunks[SelectedChunk] = newcnk;
			LevelData.RedrawChunk(SelectedChunk);
			copiedChunkBlock = (chunkBlockEditor.SelectedObjects = GetSelectedChunkBlocks())[0];
			if (newcnk.Blocks[SelectedChunkBlock.X, SelectedChunkBlock.Y].Block < LevelData.Blocks.Count)
				BlockSelector.SelectedIndex = newcnk.Blocks[SelectedChunkBlock.X, SelectedChunkBlock.Y].Block;
			DrawChunkPicture();
			ChunkSelector.Invalidate();
		}

		private void BlockPicture_MouseMove(object sender, MouseEventArgs e)
		{
			if (!loaded) return;
			int y = e.Y / 64;
			if (LevelData.Level.TwoPlayerCompatible)
				y = 0;
			if (e.X > 0 && e.Y > 0 && e.X < 128 && e.Y < 128)
				if (e.Button == chunkblockMouseDraw)
				{
					SelectedBlockTile = new Rectangle(e.X / 64, y, 1, 1);
					PatternIndex destTile = LevelData.Blocks[SelectedBlock].Tiles[SelectedBlockTile.X, SelectedBlockTile.Y]
						= copiedBlockTile.Clone();
					if (LevelData.Level.TwoPlayerCompatible)
						LevelData.Blocks[SelectedBlock].MakeInterlacedCompatible();
					blockTileEditor.SelectedObjects = new[] { destTile };
					LevelData.RedrawBlock(SelectedBlock, false);
					DrawBlockPicture();
					BlockSelector.Invalidate();
				}
				else if (e.Button == chunkblockMouseSelect)
				{
					SelectedBlockTile = Rectangle.FromLTRB(Math.Min(SelectedBlockTile.Left, e.X / 64), Math.Min(SelectedBlockTile.Top, y), Math.Max(SelectedBlockTile.Right, e.X / 64 + 1), Math.Max(SelectedBlockTile.Bottom, y + 1));
					copiedBlockTile = (blockTileEditor.SelectedObjects = GetSelectedBlockTiles())[0];
					DrawBlockPicture();
				}
		}

		private PatternIndex[] GetSelectedBlockTiles()
		{
			PatternIndex[] tiles = new PatternIndex[SelectedBlockTile.Width * SelectedBlockTile.Height];
			int i = 0;
			for (int y = SelectedBlockTile.Top; y < SelectedBlockTile.Bottom; y++)
				for (int x = SelectedBlockTile.Left; x < SelectedBlockTile.Right; x++)
					tiles[i++] = LevelData.Blocks[SelectedBlock].Tiles[x, y];
			return tiles;
		}

		private void BlockPicture_MouseDown(object sender, MouseEventArgs e)
		{
			if (!loaded) return;
			if (e.Button == chunkblockMouseDraw)
				BlockPicture_MouseMove(sender, e);
			else if (e.Button == chunkblockMouseSelect)
			{
				int y = LevelData.Level.TwoPlayerCompatible ? 0 : e.Y / 64;
				SelectedBlockTile = new Rectangle(e.X / 64, y, 1, 1);
				copiedBlockTile = LevelData.Blocks[SelectedBlock].Tiles[e.X / 64, y];
				if (copiedBlockTile.Tile < LevelData.Tiles.Count)
					TileSelector.SelectedIndex = LevelData.Level.TwoPlayerCompatible ? copiedBlockTile.Tile / 2 : copiedBlockTile.Tile;
				blockTileEditor.SelectedObjects = new[] { copiedBlockTile };
				DrawBlockPicture();
			}
		}

		private void BlockPicture_MouseUp(object sender, MouseEventArgs e)
		{
			if (loaded && e.Button == chunkblockMouseDraw)
			{
				LevelData.RedrawBlock(SelectedBlock, true);
				DrawLevel();
				DrawBlockPicture();
				BlockSelector.Invalidate();
			}
		}

		private void blockTileEditor_PropertyValueChanged(object sender, EventArgs e)
		{
			if (LevelData.Level.TwoPlayerCompatible)
				LevelData.Blocks[SelectedBlock].MakeInterlacedCompatible();
			LevelData.RedrawBlock(SelectedBlock, true);
			DrawLevel();
			DrawBlockPicture();
			BlockSelector.Invalidate();
		}

		private void BlockSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			importBlocksToolStripButton.Enabled = LevelData.Blocks.Count < LevelData.GetBlockMax();
			drawBlockToolStripButton.Enabled = importBlocksToolStripButton.Enabled;
			if (BlockSelector.SelectedIndex > -1)
			{
				SelectedBlock = BlockSelector.SelectedIndex;
				flipBlockHButton.Enabled = flipBlockVButton.Enabled = true;
				SelectedBlockTile = new Rectangle(0, 0, 1, 1);
				blockTileEditor.SelectedObjects = new[] { LevelData.Blocks[SelectedBlock].Tiles[0, 0] };
				if (LevelData.ColInds1.Count > 0)
					if (SelectedBlock < LevelData.ColInds1.Count)
					{
						ColIndBox.Enabled = true;
						BlockCollision1.Value = LevelData.GetColInd1(SelectedBlock);
						BlockCollision2.Value = LevelData.GetColInd2(SelectedBlock);
					}
					else
						ColIndBox.Enabled = false;
				BlockID.Text = SelectedBlock.ToString("X3");
				BlockCount.Text = LevelData.Blocks.Count.ToString("X") + " / " + LevelData.GetBlockMax().ToString("X");
				DrawBlockPicture();
				if (copiedChunkBlock.Block != SelectedBlock)
				{
					copiedChunkBlock = copiedChunkBlock.Clone();
					copiedChunkBlock.Block = (ushort)SelectedBlock;
				}
			}
			else
				flipBlockHButton.Enabled = flipBlockVButton.Enabled = false;
		}

		private void DrawBlockPicture()
		{
			if (!loaded) return;
			BitmapBits bmp = new BitmapBits(16, 16);
			bmp.Bits.FastFill(0x20);
			if (lowToolStripMenuItem.Checked)
				bmp.DrawBitmap(LevelData.BlockBmpBits[SelectedBlock][0], 0, 0);
			if (highToolStripMenuItem.Checked)
				bmp.DrawBitmapComposited(LevelData.BlockBmpBits[SelectedBlock][1], 0, 0);
			if (path1ToolStripMenuItem.Checked)
			{
				BitmapBits tmp = new BitmapBits(LevelData.ColBmpBits[LevelData.GetColInd1(SelectedBlock)]);
				tmp.IncrementIndexes(LevelData.ColorWhite - 1);
				bmp.DrawBitmapComposited(tmp, 0, 0);
			}
			if (path2ToolStripMenuItem.Checked)
			{
				BitmapBits tmp = new BitmapBits(LevelData.ColBmpBits[LevelData.GetColInd2(SelectedBlock)]);
				tmp.IncrementIndexes(LevelData.ColorWhite - 1);
				bmp.DrawBitmapComposited(tmp, 0, 0);
			}
			bmp = bmp.Scale(8);
			bmp.DrawRectangle(LevelData.ColorWhite, SelectedBlockTile.X * 64 - 1, SelectedBlockTile.Y * 64 - 1, SelectedBlockTile.Width * 64 + 1, LevelData.Level.TwoPlayerCompatible ? 130 : SelectedBlockTile.Height * 64 + 1);
			using (Graphics gfx = BlockPicture.CreateGraphics())
			{
				gfx.SetOptions();
				gfx.DrawImage(bmp.ToBitmap(LevelImgPalette), 0, 0, 128, 128);
			}
		}

		private void BlockPicture_Paint(object sender, PaintEventArgs e)
		{
			DrawBlockPicture();
		}

		private void BlockPicture_KeyDown(object sender, KeyEventArgs e)
		{
			if (!loaded) return;
			PatternIndex[] tiles = GetSelectedBlockTiles();
			switch (e.KeyCode)
			{
				case Keys.C:
					foreach (PatternIndex item in tiles)
						if (e.Shift)
							item.Palette--;
						else
							item.Palette++;
					break;
				case Keys.Down:
					if (!LevelData.Level.TwoPlayerCompatible && SelectedBlockTile.Y < 1)
					{
						SelectedBlockTile = new Rectangle(SelectedBlockTile.X, SelectedBlockTile.Y + 1, 1, 1);
						copiedBlockTile = LevelData.Blocks[SelectedBlock].Tiles[SelectedBlockTile.X, SelectedBlockTile.Y];
						tiles = new[] { copiedBlockTile };
						if (copiedBlockTile.Tile < LevelData.Tiles.Count)
							TileSelector.SelectedIndex = LevelData.Level.TwoPlayerCompatible ? copiedBlockTile.Tile / 2 : copiedBlockTile.Tile;
					}
					else
						return;
					break;
				case Keys.Left:
					if (SelectedBlockTile.X > 0)
					{
						SelectedBlockTile = new Rectangle(SelectedBlockTile.X - 1, SelectedBlockTile.Y, 1, 1);
						copiedBlockTile = LevelData.Blocks[SelectedBlock].Tiles[SelectedBlockTile.X, SelectedBlockTile.Y];
						tiles = new[] { copiedBlockTile };
						if (copiedBlockTile.Tile < LevelData.Tiles.Count)
							TileSelector.SelectedIndex = LevelData.Level.TwoPlayerCompatible ? copiedBlockTile.Tile / 2 : copiedBlockTile.Tile;
					}
					else
						return;
					break;
				case Keys.P:
					foreach (PatternIndex item in tiles)
						item.Priority = !item.Priority;
					break;
				case Keys.Right:
					if (SelectedBlockTile.X < 1)
					{
						SelectedBlockTile = new Rectangle(SelectedBlockTile.X + 1, SelectedBlockTile.Y, 1, 1);
						copiedBlockTile = LevelData.Blocks[SelectedBlock].Tiles[SelectedBlockTile.X, SelectedBlockTile.Y];
						tiles = new[] { copiedBlockTile };
						if (copiedBlockTile.Tile < LevelData.Tiles.Count)
							TileSelector.SelectedIndex = LevelData.Level.TwoPlayerCompatible ? copiedBlockTile.Tile / 2 : copiedBlockTile.Tile;
					}
					else
						return;
					break;
				case Keys.T:
					foreach (PatternIndex item in tiles)
						if (e.Shift)
							if (LevelData.Level.TwoPlayerCompatible)
								item.Tile = (ushort)(item.Tile < 2 ? LevelData.Tiles.Count - 1 : item.Tile - 2);
							else
								item.Tile = (ushort)(item.Tile == 0 ? LevelData.Tiles.Count - 1 : item.Tile - 1);
						else if (LevelData.Level.TwoPlayerCompatible)
							item.Tile = (ushort)((item.Tile + 2) % LevelData.Tiles.Count);
						else
							item.Tile = (ushort)((item.Tile + 1) % LevelData.Tiles.Count);
					break;
				case Keys.Up:
					if (!LevelData.Level.TwoPlayerCompatible && SelectedBlockTile.Y > 0)
					{
						SelectedBlockTile = new Rectangle(SelectedBlockTile.X, SelectedBlockTile.Y - 1, 1, 1);
						copiedBlockTile = LevelData.Blocks[SelectedBlock].Tiles[SelectedBlockTile.X, SelectedBlockTile.Y];
						tiles = new[] { copiedBlockTile };
						if (copiedBlockTile.Tile < LevelData.Tiles.Count)
							TileSelector.SelectedIndex = LevelData.Level.TwoPlayerCompatible ? copiedBlockTile.Tile / 2 : copiedBlockTile.Tile;
					}
					else
						return;
					break;
				case Keys.X:
					foreach (PatternIndex item in tiles)
						item.XFlip = !item.XFlip;
					break;
				case Keys.Y:
					foreach (PatternIndex item in tiles)
						item.YFlip = !item.YFlip;
					break;
				default:
					return;
			}
			if (LevelData.Level.TwoPlayerCompatible)
			LevelData.Blocks[SelectedBlock].MakeInterlacedCompatible();
			LevelData.RedrawBlock(SelectedBlock, true);
			DrawLevel();
			DrawBlockPicture();
			BlockSelector.Invalidate();
			copiedBlockTile = (blockTileEditor.SelectedObjects = tiles)[0];
		}

		private void PalettePanel_Paint(object sender, PaintEventArgs e)
		{
			if (!loaded) return;
			e.Graphics.Clear(Color.Black);
			for (int y = 0; y <= 3; y++)
				for (int x = 0; x <= 15; x++)
				{
					e.Graphics.FillRectangle(new SolidBrush(LevelData.PaletteToColor(y, x, false)), x * 20, y * 20, 20, 20);
					e.Graphics.DrawRectangle(Pens.White, x * 20, y * 20, 19, 19);
				}
			e.Graphics.DrawRectangle(new Pen(Color.Yellow, 2), SelectedColor.X * 20, SelectedColor.Y * 20, 20, 20);
		}

		int[] cols;
		private void PalettePanel_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (!loaded) return;
			int line = e.Y / 20;
			int index = e.X / 20;
			SelectedColor = new Point(index, line);
			ColorDialog a = new ColorDialog
			{
				AllowFullOpen = true,
				AnyColor = true,
				FullOpen = true,
				Color = LevelData.PaletteToColor(line, index, false)
			};
			if (cols != null)
				a.CustomColors = cols;
			if (a.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				LevelData.ColorToPalette(line, index, a.Color);
				PalettePanel.Invalidate();
				LevelData.PaletteChanged();
				ChunkSelector.Invalidate();
				ChunkPicture.Invalidate();
				BlockSelector.Invalidate();
				BlockPicture.Invalidate();
				TileSelector.Invalidate();
				TilePicture.Invalidate();
				DrawLevel();
			}
			cols = a.CustomColors;
			loaded = false;
			if (LevelData.Level.PaletteFormat == EngineVersion.SCDPC)
			{
				colorRed.Value = LevelData.Palette[LevelData.CurPal][line, index].R;
				colorGreen.Value = LevelData.Palette[LevelData.CurPal][line, index].G;
				colorBlue.Value = LevelData.Palette[LevelData.CurPal][line, index].B;
			}
			else
			{
				ushort md = LevelData.Palette[LevelData.CurPal][line, index].MDColor;
				colorRed.Value = md & 0xF;
				colorGreen.Value = (md >> 4) & 0xF;
				colorBlue.Value = (md >> 8) & 0xF;
			}
			loaded = true;
		}

		private void color_ValueChanged(object sender, EventArgs e)
		{
			if (!loaded) return;
			if (LevelData.Level.PaletteFormat == EngineVersion.SCDPC)
				LevelData.Palette[LevelData.CurPal][SelectedColor.Y, SelectedColor.X] = new SonLVLColor((byte)colorRed.Value, (byte)colorGreen.Value, (byte)colorBlue.Value);
			else
				LevelData.Palette[LevelData.CurPal][SelectedColor.Y, SelectedColor.X] = new SonLVLColor((ushort)((int)colorRed.Value | (int)colorGreen.Value << 4 | (int)colorBlue.Value << 8));
			PalettePanel.Invalidate();
			LevelData.PaletteChanged();
			ChunkSelector.Invalidate();
			ChunkPicture.Invalidate();
			BlockSelector.Invalidate();
			BlockPicture.Invalidate();
			TileSelector.Invalidate();
			TilePicture.Invalidate();
			DrawLevel();
		}

		private void BlockCollision1_ValueChanged(object sender, EventArgs e)
		{
			if (!loaded) return;
			LevelData.ColInds1[SelectedBlock] = (byte)BlockCollision1.Value;
			if (Object.ReferenceEquals(LevelData.ColInds1, LevelData.ColInds2))
				BlockCollision2.Value = BlockCollision1.Value;
		}

		void BlockCollision1_TextChanged(object sender, EventArgs e)
		{
			if (!loaded) return;
			byte value;
			if (byte.TryParse(BlockCollision1.Text, System.Globalization.NumberStyles.HexNumber, null, out value))
				BlockCollision1.Value = value;
		}

		private void BlockCollision2_ValueChanged(object sender, EventArgs e)
		{
			if (!loaded) return;
			LevelData.ColInds2[SelectedBlock] = (byte)BlockCollision2.Value;
			if (Object.ReferenceEquals(LevelData.ColInds1, LevelData.ColInds2))
				BlockCollision1.Value = BlockCollision2.Value;
		}

		void BlockCollision2_TextChanged(object sender, EventArgs e)
		{
			if (!loaded) return;
			byte value;
			if (byte.TryParse(BlockCollision2.Text, System.Globalization.NumberStyles.HexNumber, null, out value))
				BlockCollision2.Value = value;
		}

		private Color[] curpal;
		private void PalettePanel_MouseDown(object sender, MouseEventArgs e)
		{
			if (!loaded) return;
			bool newpal = e.Y / 20 != SelectedColor.Y;
			SelectedColor = new Point(e.X / 20, e.Y / 20);
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
				paletteContextMenuStrip.Show(PalettePanel, e.Location);
			PalettePanel.Invalidate();
			if (newpal)
			{
				curpal = new Color[16];
				for (int i = 0; i < 16; i++)
					curpal[i] = LevelData.PaletteToColor(SelectedColor.Y, i, false);
			}
			TilePicture.Invalidate();
			RefreshTileSelector();
			TileSelector.Invalidate();
			loaded = false;
			if (LevelData.Level.PaletteFormat == EngineVersion.SCDPC)
			{
				colorRed.Value = LevelData.Palette[LevelData.CurPal][SelectedColor.Y, SelectedColor.X].R;
				colorGreen.Value = LevelData.Palette[LevelData.CurPal][SelectedColor.Y, SelectedColor.X].G;
				colorBlue.Value = LevelData.Palette[LevelData.CurPal][SelectedColor.Y, SelectedColor.X].B;
			}
			else
			{
				ushort md = LevelData.Palette[LevelData.CurPal][SelectedColor.Y, SelectedColor.X].MDColor;
				colorRed.Value = md & 0xF;
				colorGreen.Value = (md >> 4) & 0xF;
				colorBlue.Value = (md >> 8) & 0xF;
			}
			loaded = true;
		}

		private void importToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog a = new OpenFileDialog())
			{
				a.DefaultExt = "bin";
				a.Filter = "MD Palettes|*.bin|Image Files|*.bmp;*.png;*.jpg;*.gif";
				a.RestoreDirectory = true;
				if (a.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{
					int l = SelectedColor.Y;
					int x = SelectedColor.X;
					switch (System.IO.Path.GetExtension(a.FileName))
					{
						case ".bin":
							SonLVLColor[] colors = SonLVLColor.Load(a.FileName, LevelData.Level.PaletteFormat);
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
			LevelData.PaletteChanged();
			ChunkSelector.Invalidate();
			ChunkPicture.Invalidate();
			BlockSelector.Invalidate();
			BlockPicture.Invalidate();
			TileSelector.Invalidate();
			TilePicture.Invalidate();
			DrawLevel();
		}

		private BitmapBits tile;
		private void TileSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			importTilesToolStripButton.Enabled = LevelData.Tiles.Count < 0x800;
			drawTileToolStripButton.Enabled = importTilesToolStripButton.Enabled;
			if (TileSelector.SelectedIndex > -1)
			{
				rotateTileRightButton.Enabled = flipTileHButton.Enabled = flipTileVButton.Enabled = true;
				if (LevelData.Level.TwoPlayerCompatible)
				{
					SelectedTile = TileSelector.SelectedIndex * 2;
					tile = BitmapBits.FromTileInterlaced(LevelData.TileArray, SelectedTile);
					TileID.Text = SelectedTile.ToString("X3");
				}
				else
				{
					SelectedTile = TileSelector.SelectedIndex;
					tile = BitmapBits.FromTile(LevelData.Tiles[SelectedTile], 0);
					TileID.Text = SelectedTile.ToString("X3");
				}
				TileCount.Text = LevelData.Tiles.Count.ToString("X") + " / 800";
				DrawTilePicture();
				if (copiedBlockTile.Tile != SelectedTile)
				{
					copiedBlockTile = copiedBlockTile.Clone();
					copiedBlockTile.Tile = (ushort)SelectedTile;
					copiedBlockTile.Palette = (byte)SelectedColor.Y;
				}
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
				gfx.DrawImage(tile.Scale(16).ToBitmap(curpal), 0, 0, 128, TilePicture.Height);
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
			{
				SelectedColor = new Point(tile[e.X / 16, e.Y / 16], SelectedColor.Y);
				PalettePanel.Invalidate();
			}
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
			if (LevelData.Level.TwoPlayerCompatible)
			{
				byte[] td = tile.ToTileInterlaced();
				Array.Copy(td, 0, LevelData.Tiles[SelectedTile], 0, 32);
				Array.Copy(td, 32, LevelData.Tiles[SelectedTile + 1], 0, 32);
				td.CopyTo(LevelData.TileArray, SelectedTile * 32);
			}
			else
			{
				LevelData.Tiles[SelectedTile] = tile.ToTile();
				LevelData.Tiles[SelectedTile].CopyTo(LevelData.TileArray, SelectedTile * 32);
			}
			for (int i = 0; i < LevelData.Blocks.Count; i++)
			{
				bool dr = false;
				for (int y = 0; y < 2; y++)
					for (int x = 0; x < 2; x++)
						if (LevelData.Blocks[i].Tiles[x, y].Tile == SelectedTile)
							dr = true;
				if (dr)
					LevelData.RedrawBlock(i, true);
			}
			if (LevelData.Level.TwoPlayerCompatible)
				TileSelector.Images[SelectedTile / 2] = LevelData.InterlacedTileToBmp4bpp(LevelData.TileArray, SelectedTile, SelectedColor.Y);
			else
				TileSelector.Images[SelectedTile] = LevelData.TileToBmp4bpp(LevelData.Tiles[SelectedTile], 0, SelectedColor.Y);
			TileSelector.Invalidate();
			blockTileEditor.SelectedObjects = blockTileEditor.SelectedObjects;
		}

		private void ChunkSelector_MouseDown(object sender, MouseEventArgs e)
		{
			if (!loaded) return;
			if (CurrentTab == Tab.Art & e.Button == MouseButtons.Right)
			{
				pasteOverToolStripMenuItem.Enabled = Clipboard.ContainsData(typeof(Chunk).AssemblyQualifiedName) || Clipboard.ContainsData(typeof(ChunkCopyData).AssemblyQualifiedName);
				pasteBeforeToolStripMenuItem.Enabled = pasteOverToolStripMenuItem.Enabled && LevelData.Chunks.Count < 256;
				pasteAfterToolStripMenuItem.Enabled = pasteBeforeToolStripMenuItem.Enabled;
				insertAfterToolStripMenuItem.Enabled = LevelData.Chunks.Count < 256;
				insertBeforeToolStripMenuItem.Enabled = insertAfterToolStripMenuItem.Enabled;
				duplicateTilesToolStripMenuItem.Enabled = insertAfterToolStripMenuItem.Enabled;
				deleteTilesToolStripMenuItem.Enabled = LevelData.Chunks.Count > 1;
				cutTilesToolStripMenuItem.Enabled = deleteTilesToolStripMenuItem.Enabled;
				deepCopyToolStripMenuItem.Visible = true;
				tileContextMenuStrip.Show(ChunkSelector, e.Location);
			}
		}

		private void BlockSelector_MouseDown(object sender, MouseEventArgs e)
		{
			if (!loaded) return;
			if (e.Button == MouseButtons.Right)
			{
				int blockmax = LevelData.GetBlockMax();
				pasteOverToolStripMenuItem.Enabled = Clipboard.ContainsData(typeof(Block).AssemblyQualifiedName) || Clipboard.ContainsData(typeof(BlockCopyData).AssemblyQualifiedName);
				pasteBeforeToolStripMenuItem.Enabled = pasteOverToolStripMenuItem.Enabled && LevelData.Blocks.Count <  blockmax;
				pasteAfterToolStripMenuItem.Enabled = pasteBeforeToolStripMenuItem.Enabled;
				insertAfterToolStripMenuItem.Enabled = LevelData.Blocks.Count < blockmax;
				insertBeforeToolStripMenuItem.Enabled = insertAfterToolStripMenuItem.Enabled;
				duplicateTilesToolStripMenuItem.Enabled = insertAfterToolStripMenuItem.Enabled;
				deleteTilesToolStripMenuItem.Enabled = LevelData.Blocks.Count > 1;
				cutTilesToolStripMenuItem.Enabled = deleteTilesToolStripMenuItem.Enabled;
				deepCopyToolStripMenuItem.Visible = true;
				tileContextMenuStrip.Show(BlockSelector, e.Location);
			}
		}

		private void TileSelector_MouseDown(object sender, MouseEventArgs e)
		{
			if (!loaded) return;
			if (e.Button == MouseButtons.Right)
			{
				pasteOverToolStripMenuItem.Enabled = Clipboard.ContainsData(LevelData.Level.TwoPlayerCompatible ? "SonLVLTileInterlaced" : "SonLVLTile");
				pasteBeforeToolStripMenuItem.Enabled = pasteOverToolStripMenuItem.Enabled && LevelData.Tiles.Count < 0x800;
				pasteAfterToolStripMenuItem.Enabled = pasteBeforeToolStripMenuItem.Enabled;
				insertAfterToolStripMenuItem.Enabled = LevelData.Tiles.Count < 0x800;
				insertBeforeToolStripMenuItem.Enabled = insertAfterToolStripMenuItem.Enabled;
				duplicateTilesToolStripMenuItem.Enabled = insertAfterToolStripMenuItem.Enabled;
				deleteTilesToolStripMenuItem.Enabled = TileSelector.Images.Count > 1;
				cutTilesToolStripMenuItem.Enabled = deleteTilesToolStripMenuItem.Enabled;
				deepCopyToolStripMenuItem.Visible = false;
				tileContextMenuStrip.Show(TileSelector, e.Location);
			}
		}

		private void cutTilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			switch (CurrentArtTab)
			{
				case ArtTab.Chunks:
					Clipboard.SetData(typeof(Chunk).AssemblyQualifiedName, LevelData.Chunks[SelectedChunk].GetBytes());
					DeleteChunk();
					break;
				case ArtTab.Blocks:
					Clipboard.SetData(typeof(Block).AssemblyQualifiedName, LevelData.Blocks[SelectedBlock].GetBytes());
					DeleteBlock();
					break;
				case ArtTab.Tiles:
					if (LevelData.Level.TwoPlayerCompatible)
					{
						byte[][] data = new byte[2][];
						data[0] = LevelData.Tiles[SelectedTile];
						data[1] = LevelData.Tiles[SelectedTile + 1];
						Clipboard.SetData("SonLVLTileInterlaced", data);
					}
					else
						Clipboard.SetData("SonLVLTile", LevelData.Tiles[SelectedTile]);
					DeleteTile();
					break;
			}
		}

		private void DeleteChunk()
		{
			LevelData.Chunks.RemoveAt(SelectedChunk);
			LevelData.ChunkBmpBits.RemoveAt(SelectedChunk);
			LevelData.ChunkBmps.RemoveAt(SelectedChunk);
			LevelData.ChunkColBmpBits.RemoveAt(SelectedChunk);
			LevelData.ChunkColBmps.RemoveAt(SelectedChunk);
			LevelData.CompChunkBmps.RemoveAt(SelectedChunk);
			LevelData.CompChunkBmpBits.RemoveAt(SelectedChunk);
			SelectedChunk = (byte)Math.Min(SelectedChunk, LevelData.Chunks.Count - 1);
			LevelData.RemapLayouts((layout, x, y) =>
			{
				if (layout[x, y] > SelectedChunk && layout[x, y] < LevelData.Chunks.Count + 1)
					layout[x, y]--;
			});
			ChunkSelector.SelectedIndex = Math.Min(ChunkSelector.SelectedIndex, LevelData.Chunks.Count - 1);
			importChunksToolStripButton.Enabled = true;
			drawChunkToolStripButton.Enabled = importChunksToolStripButton.Enabled;
		}

		private void DeleteBlock()
		{
			LevelData.Blocks.RemoveAt(SelectedBlock);
			LevelData.BlockBmps.RemoveAt(SelectedBlock);
			LevelData.BlockBmpBits.RemoveAt(SelectedBlock);
			LevelData.CompBlockBmps.RemoveAt(SelectedBlock);
			LevelData.CompBlockBmpBits.RemoveAt(SelectedBlock);
			LevelData.ColInds1.RemoveAt(SelectedBlock);
			if (LevelData.ColInds2 != null && LevelData.ColInds2 != LevelData.ColInds1)
				LevelData.ColInds2.RemoveAt(SelectedBlock);
			for (int i = 0; i < LevelData.Chunks.Count; i++)
			{
				bool dr = false;
				for (int y = 0; y < LevelData.Level.ChunkHeight / 16; y++)
					for (int x = 0; x < LevelData.Level.ChunkWidth / 16; x++)
						if (LevelData.Chunks[i].Blocks[x, y].Block == SelectedBlock)
							dr = true;
						else if (LevelData.Chunks[i].Blocks[x, y].Block > SelectedBlock && LevelData.Chunks[i].Blocks[x, y].Block < LevelData.Blocks.Count + 1)
							LevelData.Chunks[i].Blocks[x, y].Block--;
				if (dr)
					LevelData.RedrawChunk(i);
			}
			BlockSelector.SelectedIndex = Math.Min(BlockSelector.SelectedIndex, LevelData.Blocks.Count - 1);
			importBlocksToolStripButton.Enabled = true;
			drawBlockToolStripButton.Enabled = importBlocksToolStripButton.Enabled;
		}

		private void DeleteTile()
		{
			LevelData.Tiles.RemoveAt(SelectedTile);
			if (LevelData.Level.TwoPlayerCompatible)
				LevelData.Tiles.RemoveAt(SelectedTile);
			LevelData.UpdateTileArray();
			TileSelector.Images.RemoveAt(LevelData.Level.TwoPlayerCompatible ? SelectedTile / 2 : SelectedTile);
			blockTileEditor.SelectedObjects = blockTileEditor.SelectedObjects;
			for (int i = 0; i < LevelData.Blocks.Count; i++)
			{
				bool dr = false;
				if (LevelData.Level.TwoPlayerCompatible)
				{
					for (int x = 0; x < 2; x++)
						if ((LevelData.Blocks[i].Tiles[x, 0].Tile & ~1) == SelectedTile)
							dr = true;
						else if (LevelData.Blocks[i].Tiles[x, 0].Tile > SelectedTile + 1 && LevelData.Blocks[i].Tiles[x, 0].Tile < LevelData.Tiles.Count + 2)
						{
							LevelData.Blocks[i].Tiles[x, 0].Tile -= 2;
							LevelData.Blocks[i].Tiles[x, 1].Tile -= 2;
						}
				}
				else
				{
					for (int y = 0; y < 2; y++)
						for (int x = 0; x < 2; x++)
							if (LevelData.Blocks[i].Tiles[x, y].Tile == SelectedTile)
								dr = true;
							else if (LevelData.Blocks[i].Tiles[x, y].Tile > SelectedTile && LevelData.Blocks[i].Tiles[x, y].Tile < LevelData.Tiles.Count + 1)
								LevelData.Blocks[i].Tiles[x, y].Tile--;
				}
				if (dr)
					LevelData.RedrawBlock(i, true);
			}
			TileSelector.SelectedIndex = Math.Min(TileSelector.SelectedIndex, TileSelector.Images.Count - 1);
			importTilesToolStripButton.Enabled = true;
			drawTileToolStripButton.Enabled = importTilesToolStripButton.Enabled;
		}

		private void copyTilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			switch (CurrentArtTab)
			{
				case ArtTab.Chunks:
					Clipboard.SetData(typeof(Chunk).AssemblyQualifiedName, LevelData.Chunks[SelectedChunk].GetBytes());
					break;
				case ArtTab.Blocks:
					Clipboard.SetData(typeof(Block).AssemblyQualifiedName, LevelData.Blocks[SelectedBlock].GetBytes());
					break;
				case ArtTab.Tiles:
					if (LevelData.Level.TwoPlayerCompatible)
					{
						byte[][] data = new byte[2][];
						data[0] = LevelData.Tiles[SelectedTile];
						data[1] = LevelData.Tiles[SelectedTile + 1];
						Clipboard.SetData("SonLVLTileInterlaced", data);
					}
					else
						Clipboard.SetData("SonLVLTile", LevelData.Tiles[SelectedTile]);
					break;
			}
		}

		private void InsertChunk()
		{
			LevelData.ChunkBmpBits.Insert(SelectedChunk, new BitmapBits[2]);
			LevelData.ChunkBmps.Insert(SelectedChunk, new Bitmap[2]);
			LevelData.ChunkColBmpBits.Insert(SelectedChunk, new BitmapBits[2]);
			LevelData.ChunkColBmps.Insert(SelectedChunk, new Bitmap[2]);
			LevelData.CompChunkBmps.Insert(SelectedChunk, null);
			LevelData.CompChunkBmpBits.Insert(SelectedChunk, null);
			LevelData.RemapLayouts((layout, x, y) =>
			{
				if (layout[x, y] >= SelectedChunk && layout[x, y] < LevelData.Chunks.Count)
					layout[x, y]++;
			});
			LevelData.RedrawChunk(SelectedChunk);
			ChunkSelector.SelectedIndex = SelectedChunk;
			importChunksToolStripButton.Enabled = LevelData.Chunks.Count < 256;
			drawChunkToolStripButton.Enabled = importChunksToolStripButton.Enabled;
		}

		private void InsertBlock()
		{
			LevelData.BlockBmps.Insert(SelectedBlock, new Bitmap[2]);
			LevelData.BlockBmpBits.Insert(SelectedBlock, new BitmapBits[2]);
			LevelData.CompBlockBmps.Insert(SelectedBlock, null);
			LevelData.CompBlockBmpBits.Insert(SelectedBlock, null);
			LevelData.ColInds1.Insert(SelectedBlock, 0);
			if (LevelData.ColInds2 != null && LevelData.ColInds2 != LevelData.ColInds1)
				LevelData.ColInds2.Insert(SelectedBlock, 0);
			for (int i = 0; i < LevelData.Chunks.Count; i++)
				for (int y = 0; y < LevelData.Level.ChunkHeight / 16; y++)
					for (int x = 0; x < LevelData.Level.ChunkWidth / 16; x++)
						if (LevelData.Chunks[i].Blocks[x, y].Block >= SelectedBlock && LevelData.Chunks[i].Blocks[x, y].Block < LevelData.Blocks.Count)
							LevelData.Chunks[i].Blocks[x, y].Block++;
			LevelData.RedrawBlock(SelectedBlock, false);
			BlockSelector.SelectedIndex = SelectedBlock;
			importBlocksToolStripButton.Enabled = LevelData.Blocks.Count < LevelData.GetBlockMax();
			drawBlockToolStripButton.Enabled = importBlocksToolStripButton.Enabled;
		}

		private void InsertTile()
		{
			LevelData.UpdateTileArray();
			if (LevelData.Level.TwoPlayerCompatible)
				TileSelector.Images.Insert(SelectedTile / 2, LevelData.InterlacedTileToBmp4bpp(LevelData.TileArray, SelectedTile, SelectedColor.Y));
			else
				TileSelector.Images.Insert(SelectedTile, LevelData.TileToBmp4bpp(LevelData.Tiles[SelectedTile], 0, SelectedColor.Y));
			blockTileEditor.SelectedObjects = blockTileEditor.SelectedObjects;
			for (int i = 0; i < LevelData.Blocks.Count; i++)
				if (LevelData.Level.TwoPlayerCompatible)
				{
					for (int x = 0; x < 2; x++)
						if ((LevelData.Blocks[i].Tiles[x, 0].Tile & ~1) >= SelectedTile && LevelData.Blocks[i].Tiles[x, 0].Tile < LevelData.Tiles.Count)
						{
							LevelData.Blocks[i].Tiles[x, 0].Tile += 2;
							LevelData.Blocks[i].Tiles[x, 1].Tile += 2;
						}
					TileSelector.SelectedIndex = SelectedTile / 2;
				}
				else
				{
					for (int y = 0; y < 2; y++)
						for (int x = 0; x < 2; x++)
							if (LevelData.Blocks[i].Tiles[x, y].Tile >= SelectedTile && LevelData.Blocks[i].Tiles[x, y].Tile < LevelData.Tiles.Count)
								LevelData.Blocks[i].Tiles[x, y].Tile++;
					TileSelector.SelectedIndex = SelectedTile;
				}
			importTilesToolStripButton.Enabled = LevelData.Tiles.Count < 0x800;
			drawTileToolStripButton.Enabled = importTilesToolStripButton.Enabled;
		}

		private void pasteBeforeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			switch (CurrentArtTab)
			{
				case ArtTab.Chunks:
					if (Clipboard.ContainsData(typeof(ChunkCopyData).AssemblyQualifiedName))
					{
						ChunkCopyData cnkcpy = (ChunkCopyData)Clipboard.GetData(typeof(ChunkCopyData).AssemblyQualifiedName);
						bool isS2 = false;
						switch (LevelData.Level.ChunkFormat)
						{
							case EngineVersion.S2NA:
							case EngineVersion.S2:
							case EngineVersion.S3K:
							case EngineVersion.SKC:
								isS2 = true;
								break;
						}
						if (cnkcpy.IsS2 != isS2)
						{
							MessageBox.Show(this, "Copied chunk data does not match current level's format.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						if (LevelData.Level.TwoPlayerCompatible && !cnkcpy.IsInterlacedCompatible)
						{
							MessageBox.Show(this, "Copied chunk data is not 2P compatible.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						if (LevelData.Blocks.Count + cnkcpy.Blocks.Count > LevelData.GetBlockMax())
						{
							MessageBox.Show(this, "Level does not have enough free blocks.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						if (LevelData.Tiles.Count + cnkcpy.Tiles.Count > 0x8000)
						{
							MessageBox.Show(this, "Level does not have enough free tiles.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						List<ushort> tiles = new List<ushort>(cnkcpy.Tiles.Count);
						foreach (byte[] tile in cnkcpy.Tiles)
						{
							ushort ti = (ushort)LevelData.Tiles.Count;
							for (ushort j = 0; j < LevelData.Tiles.Count; j++)
								if (tile.FastArrayEqual(LevelData.Tiles[j]))
								{
									ti = j;
									break;
								}
							if (ti == LevelData.Tiles.Count)
								LevelData.Tiles.Add(tile);
							tiles.Add(ti);
						}
						LevelData.UpdateTileArray();
						RefreshTileSelector();
						blockTileEditor.SelectedObjects = blockTileEditor.SelectedObjects;
						List<ushort> blocks = new List<ushort>(cnkcpy.Blocks.Count);
						for (int i = 0; i < cnkcpy.Blocks.Count; i++)
						{
							Block block = cnkcpy.Blocks[i];
							for (int y = 0; y < 2; y++)
								for (int x = 0; x < 2; x++)
									if (block.Tiles[x, y].Tile < tiles.Count)
										block.Tiles[x, y].Tile = tiles[block.Tiles[x, y].Tile];
							ushort bi = (ushort)LevelData.Blocks.Count;
							for (ushort j = 0; j < LevelData.Blocks.Count; j++)
								if (block.Equals(LevelData.Blocks[j]))
								{
									bi = j;
									break;
								}
							if (bi == LevelData.Blocks.Count)
							{
								LevelData.Blocks.Add(block);
								LevelData.ColInds1.AddOrSet(bi, cnkcpy.ColInds1[i]);
								if (LevelData.ColInds2 != null && LevelData.ColInds2 != LevelData.ColInds1)
									LevelData.ColInds2.AddOrSet(bi, cnkcpy.ColInds2[i]);
								LevelData.BlockBmps.Add(new Bitmap[2]);
								LevelData.BlockBmpBits.Add(new BitmapBits[2]);
								LevelData.CompBlockBmps.Add(null);
								LevelData.CompBlockBmpBits.Add(null);
								LevelData.RedrawBlock(bi, false);
							}
							blocks.Add(bi);
						}
						for (int y = 0; y < LevelData.Level.ChunkHeight / 16; y++)
							for (int x = 0; x < LevelData.Level.ChunkWidth / 16; x++)
								if (cnkcpy.Chunk.Blocks[x, y].Block < blocks.Count)
									cnkcpy.Chunk.Blocks[x, y].Block = blocks[cnkcpy.Chunk.Blocks[x, y].Block];
						LevelData.Chunks.InsertBefore(SelectedChunk, cnkcpy.Chunk);
					}
					else
					{
						LevelData.Chunks.InsertBefore(SelectedChunk, new Chunk((byte[])Clipboard.GetData(typeof(Chunk).AssemblyQualifiedName), 0));
					}
					InsertChunk();
					break;
				case ArtTab.Blocks:
					if (Clipboard.ContainsData(typeof(BlockCopyData).AssemblyQualifiedName))
					{
						BlockCopyData blkcpy = (BlockCopyData)Clipboard.GetData(typeof(BlockCopyData).AssemblyQualifiedName);
						if (LevelData.Level.TwoPlayerCompatible && !blkcpy.Block.IsInterlacedCompatible)
						{
							MessageBox.Show(this, "Copied block data is not 2P compatible.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						if (LevelData.Tiles.Count + blkcpy.Tiles.Count > 0x8000)
						{
							MessageBox.Show(this, "Level does not have enough free tiles.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						List<ushort> tiles = new List<ushort>(blkcpy.Tiles.Count);
						foreach (byte[] tile in blkcpy.Tiles)
						{
							ushort ti = (ushort)LevelData.Tiles.Count;
							for (ushort j = 0; j < LevelData.Tiles.Count; j++)
								if (tile.FastArrayEqual(LevelData.Tiles[j]))
								{
									ti = j;
									break;
								}
							if (ti == LevelData.Tiles.Count)
								LevelData.Tiles.Add(tile);
							tiles.Add(ti);
						}
						LevelData.UpdateTileArray();
						RefreshTileSelector();
						for (int y = 0; y < 2; y++)
							for (int x = 0; x < 2; x++)
								if (blkcpy.Block.Tiles[x, y].Tile < tiles.Count)
									blkcpy.Block.Tiles[x, y].Tile = tiles[blkcpy.Block.Tiles[x, y].Tile];
						LevelData.Blocks.InsertBefore(SelectedBlock, blkcpy.Block);
					}
					else
					{
						LevelData.Blocks.InsertBefore(SelectedBlock, new Block((byte[])Clipboard.GetData(typeof(Block).AssemblyQualifiedName), 0));
					}
					InsertBlock();
					break;
				case ArtTab.Tiles:
					if (LevelData.Level.TwoPlayerCompatible)
					{
						byte[][] t = (byte[][])Clipboard.GetData("SonLVLTileInterlaced");
						LevelData.Tiles.InsertBefore(SelectedTile, t[1]);
						LevelData.Tiles.InsertBefore(SelectedTile, t[0]);
					}
					else
					{
						byte[] t = (byte[])Clipboard.GetData("SonLVLTile");
						LevelData.Tiles.InsertBefore(SelectedTile, t);
					}
					InsertTile();
					break;
			}
		}

		private void pasteAfterToolStripMenuItem_Click(object sender, EventArgs e)
		{
			switch (CurrentArtTab)
			{
				case ArtTab.Chunks:
					if (Clipboard.ContainsData(typeof(ChunkCopyData).AssemblyQualifiedName))
					{
						ChunkCopyData cnkcpy = (ChunkCopyData)Clipboard.GetData(typeof(ChunkCopyData).AssemblyQualifiedName);
						bool isS2 = false;
						switch (LevelData.Level.ChunkFormat)
						{
							case EngineVersion.S2NA:
							case EngineVersion.S2:
							case EngineVersion.S3K:
							case EngineVersion.SKC:
								isS2 = true;
								break;
						}
						if (cnkcpy.IsS2 != isS2)
						{
							MessageBox.Show(this, "Copied chunk data does not match current level's format.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						if (LevelData.Level.TwoPlayerCompatible && !cnkcpy.IsInterlacedCompatible)
						{
							MessageBox.Show(this, "Copied chunk data is not 2P compatible.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						if (LevelData.Blocks.Count + cnkcpy.Blocks.Count > LevelData.GetBlockMax())
						{
							MessageBox.Show(this, "Level does not have enough free blocks.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						if (LevelData.Tiles.Count + cnkcpy.Tiles.Count > 0x8000)
						{
							MessageBox.Show(this, "Level does not have enough free tiles.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						List<ushort> tiles = new List<ushort>(cnkcpy.Tiles.Count);
						foreach (byte[] tile in cnkcpy.Tiles)
						{
							ushort ti = (ushort)LevelData.Tiles.Count;
							for (ushort j = 0; j < LevelData.Tiles.Count; j++)
								if (tile.FastArrayEqual(LevelData.Tiles[j]))
								{
									ti = j;
									break;
								}
							if (ti == LevelData.Tiles.Count)
								LevelData.Tiles.Add(tile);
							tiles.Add(ti);
						}
						LevelData.UpdateTileArray();
						RefreshTileSelector();
						blockTileEditor.SelectedObjects = blockTileEditor.SelectedObjects;
						List<ushort> blocks = new List<ushort>(cnkcpy.Blocks.Count);
						for (int i = 0; i < cnkcpy.Blocks.Count; i++)
						{
							Block block = cnkcpy.Blocks[i];
							for (int y = 0; y < 2; y++)
								for (int x = 0; x < 2; x++)
									block.Tiles[x, y].Tile = tiles[block.Tiles[x, y].Tile];
							ushort bi = (ushort)LevelData.Blocks.Count;
							for (ushort j = 0; j < LevelData.Blocks.Count; j++)
								if (block.Equals(LevelData.Blocks[j]))
								{
									bi = j;
									break;
								}
							if (bi == LevelData.Blocks.Count)
							{
								LevelData.Blocks.Add(block);
								LevelData.ColInds1.AddOrSet(bi, cnkcpy.ColInds1[i]);
								if (LevelData.ColInds2 != null && LevelData.ColInds2 != LevelData.ColInds1)
									LevelData.ColInds2.AddOrSet(bi, cnkcpy.ColInds2[i]);
								LevelData.BlockBmps.Add(new Bitmap[2]);
								LevelData.BlockBmpBits.Add(new BitmapBits[2]);
								LevelData.CompBlockBmps.Add(null);
								LevelData.CompBlockBmpBits.Add(null);
								LevelData.RedrawBlock(bi, false);
							}
							blocks.Add(bi);
						}
						for (int y = 0; y < LevelData.Level.ChunkHeight / 16; y++)
							for (int x = 0; x < LevelData.Level.ChunkWidth / 16; x++)
								cnkcpy.Chunk.Blocks[x, y].Block = blocks[cnkcpy.Chunk.Blocks[x, y].Block];
						LevelData.Chunks.InsertAfter(SelectedChunk, cnkcpy.Chunk);
					}
					else
					{
						LevelData.Chunks.InsertAfter(SelectedChunk, new Chunk((byte[])Clipboard.GetData(typeof(Chunk).AssemblyQualifiedName), 0));
					}
					SelectedChunk++;
					InsertChunk();
					break;
				case ArtTab.Blocks:
					if (Clipboard.ContainsData(typeof(BlockCopyData).AssemblyQualifiedName))
					{
						BlockCopyData blkcpy = (BlockCopyData)Clipboard.GetData(typeof(BlockCopyData).AssemblyQualifiedName);
						if (LevelData.Level.TwoPlayerCompatible && !blkcpy.Block.IsInterlacedCompatible)
						{
							MessageBox.Show(this, "Copied block data is not 2P compatible.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						if (LevelData.Tiles.Count + blkcpy.Tiles.Count > 0x8000)
						{
							MessageBox.Show(this, "Level does not have enough free tiles.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						List<ushort> tiles = new List<ushort>(blkcpy.Tiles.Count);
						foreach (byte[] tile in blkcpy.Tiles)
						{
							ushort ti = (ushort)LevelData.Tiles.Count;
							for (ushort j = 0; j < LevelData.Tiles.Count; j++)
								if (tile.FastArrayEqual(LevelData.Tiles[j]))
								{
									ti = j;
									break;
								}
							if (ti == LevelData.Tiles.Count)
								LevelData.Tiles.Add(tile);
							tiles.Add(ti);
						}
						LevelData.UpdateTileArray();
						RefreshTileSelector();
						for (int y = 0; y < 2; y++)
							for (int x = 0; x < 2; x++)
								blkcpy.Block.Tiles[x, y].Tile = tiles[blkcpy.Block.Tiles[x, y].Tile];
						LevelData.Blocks.InsertAfter(SelectedBlock, blkcpy.Block);
					}
					else
					{
						LevelData.Blocks.InsertAfter(SelectedBlock, new Block((byte[])Clipboard.GetData(typeof(Block).AssemblyQualifiedName), 0));
					}
					SelectedBlock++;
					InsertBlock();
					break;
				case ArtTab.Tiles:
					if (LevelData.Level.TwoPlayerCompatible)
					{
						byte[][] t = (byte[][])Clipboard.GetData("SonLVLTileInterlaced");
						LevelData.Tiles.InsertAfter(SelectedTile, t[1]);
						LevelData.Tiles.InsertAfter(SelectedTile, t[0]);
						SelectedTile += 2;
					}
					else
					{
						byte[] t = (byte[])Clipboard.GetData("SonLVLTile");
						LevelData.Tiles.InsertAfter(SelectedTile, t);
						SelectedTile++;
					}
					InsertTile();
					break;
			}
		}

		private void duplicateTilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			switch (CurrentArtTab)
			{
				case ArtTab.Chunks:
					LevelData.Chunks.InsertAfter(SelectedChunk, LevelData.Chunks[SelectedChunk].Clone());
					SelectedChunk++;
					InsertChunk();
					break;
				case ArtTab.Blocks:
					LevelData.Blocks.InsertAfter(SelectedBlock, LevelData.Blocks[SelectedBlock].Clone());
					SelectedBlock++;
					InsertBlock();
					break;
				case ArtTab.Tiles:
					if (LevelData.Level.TwoPlayerCompatible)
					{
						LevelData.Tiles.InsertAfter(SelectedTile + 1, (byte[])LevelData.Tiles[SelectedTile + 1].Clone());
						LevelData.Tiles.InsertAfter(SelectedTile + 1, (byte[])LevelData.Tiles[SelectedTile].Clone());
						SelectedTile += 2;
					}
					else
						LevelData.Tiles.InsertAfter(SelectedTile++, (byte[])LevelData.Tiles[SelectedTile].Clone());
					InsertTile();
					break;
			}
		}

		private void insertBeforeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			switch (CurrentArtTab)
			{
				case ArtTab.Chunks:
					LevelData.Chunks.InsertBefore(SelectedChunk, new Chunk());
					InsertChunk();
					break;
				case ArtTab.Blocks:
					LevelData.Blocks.InsertBefore(SelectedBlock, new Block());
					InsertBlock();
					break;
				case ArtTab.Tiles:
					if (LevelData.Level.TwoPlayerCompatible)
						LevelData.Tiles.InsertAfter(SelectedTile, new byte[32]);
					LevelData.Tiles.InsertAfter(SelectedTile, new byte[32]);
					InsertTile();
					break;
			}
		}

		private void insertAfterToolStripMenuItem_Click(object sender, EventArgs e)
		{
			switch (CurrentArtTab)
			{
				case ArtTab.Chunks:

					LevelData.Chunks.InsertAfter(SelectedChunk, new Chunk());
					SelectedChunk++;
					InsertChunk();
					break;
				case ArtTab.Blocks:
					LevelData.Blocks.InsertAfter(SelectedBlock, new Block());
					SelectedBlock++;
					InsertBlock();
					break;
				case ArtTab.Tiles:
					if (LevelData.Level.TwoPlayerCompatible)
					{
						LevelData.Tiles.InsertAfter(SelectedTile, new byte[32]);
						LevelData.Tiles.InsertAfter(SelectedTile, new byte[32]);
						SelectedTile += 2;
					}
					else
					{
						LevelData.Tiles.InsertAfter(SelectedTile, new byte[32]);
						SelectedTile++;
					}
					InsertTile();
					break;
			}
		}

		private void deleteTilesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			switch (CurrentArtTab)
			{
				case ArtTab.Chunks:
					DeleteChunk();
					break;
				case ArtTab.Blocks:
					DeleteBlock();
					break;
				case ArtTab.Tiles:
					DeleteTile();
					break;
			}
		}

		private void importToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog opendlg = new OpenFileDialog())
			{
				opendlg.DefaultExt = "png";
				opendlg.Filter = "Image Files|*.bmp;*.png;*.jpg;*.gif";
				opendlg.RestoreDirectory = true;
				if (opendlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{
					Bitmap colbmp1 = null, colbmp2 = null, pribmp = null;
					if (CurrentArtTab != ArtTab.Tiles)
					{
						string fmt = Path.Combine(Path.GetDirectoryName(opendlg.FileName),
							Path.GetFileNameWithoutExtension(opendlg.FileName) + "_{0}" + Path.GetExtension(opendlg.FileName));
						if (File.Exists(string.Format(fmt, "col1")))
						{
							colbmp1 = new Bitmap(string.Format(fmt, "col1"));
							if (File.Exists(string.Format(fmt, "col2")))
								colbmp2 = new Bitmap(string.Format(fmt, "col2"));
						}
						else if (File.Exists(string.Format(fmt, "col")))
							colbmp1 = new Bitmap(string.Format(fmt, "col"));
						if (File.Exists(string.Format(fmt, "pri")))
							pribmp = new Bitmap(string.Format(fmt, "pri"));
					}
					ImportImage(new Bitmap(opendlg.FileName), colbmp1, colbmp2, pribmp, null);
				}
			}
		}

		private bool ImportImage(Bitmap bmp, Bitmap colbmp1, Bitmap colbmp2, Bitmap pribmp, byte[,] layout)
		{
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			sw.Start();
			int w = bmp.Width;
			int h = bmp.Height;
			Enabled = false;
			UseWaitCursor = true;
			importProgressControl1_SizeChanged(this, EventArgs.Empty);
			importProgressControl1.CurrentProgress = 0;
			importProgressControl1.MaximumProgress = (w / 8) * (h / (LevelData.Level.TwoPlayerCompatible ? 16 : 8));
			importProgressControl1.BringToFront();
			importProgressControl1.Show();
			Application.DoEvents();
			BitmapInfo bmpi = new BitmapInfo(bmp);
			Application.DoEvents();
			bmp.Dispose();
			BlockColInfo[,] blockcoldata = null;
			if (colbmp1 != null)
			{
				blockcoldata = ProcessColBmps(colbmp1, colbmp2, w, h);
				Application.DoEvents();
			}
			bool[,] priority = new bool[w / 8, h / 8];
			if (pribmp != null)
			{
				using (pribmp)
					LevelData.GetPriMap(pribmp, priority);
				Application.DoEvents();
			}
			byte? forcepal = bmpi.PixelFormat == PixelFormat.Format1bppIndexed || bmpi.PixelFormat == PixelFormat.Format4bppIndexed ? (byte)SelectedColor.Y : (byte?)null;
			List<byte[]> tiles = new List<byte[]>(LevelData.Tiles.Count);
			if (LevelData.Level.TwoPlayerCompatible)
				for (int i = 0; i < LevelData.Tiles.Count; i += 2)
				{
					byte[] t = new byte[64];
					Array.Copy(LevelData.TileArray, i * 32, t, 0, 64);
					tiles.Add(t);
				}
			else
				for (int i = 0; i < LevelData.Tiles.Count; i++)
					tiles.Add(LevelData.Tiles[i]);
			Application.DoEvents();
			List<byte[]> blocks = new List<byte[]>(LevelData.Blocks.Count);
			for (int i = 0; i < LevelData.Blocks.Count; i++)
				blocks.Add(LevelData.Blocks[i].GetBytes());
			Application.DoEvents();
			List<byte> colInds1 = new List<byte>(LevelData.ColInds1);
			List<byte> colInds2 = new List<byte>(LevelData.ColInds2);
			Application.DoEvents();
			List<byte[]> chunks = new List<byte[]>(LevelData.Chunks.Count);
			for (int i = 0; i < LevelData.Chunks.Count; i++)
				chunks.Add(LevelData.Chunks[i].GetBytes());
			Application.DoEvents();
			ImportResult ir = LevelData.BitmapToTiles(bmpi, priority, forcepal, tiles, LevelData.Level.TwoPlayerCompatible, true, () => { importProgressControl1.CurrentProgress++; Application.DoEvents(); });
			List<byte[]> newTiles = ir.Art;
			List<Block> newBlocks = new List<Block>();
			List<byte> newColInds1 = new List<byte>();
			List<byte> newColInds2 = new List<byte>();
			List<Chunk> newChunks = new List<Chunk>();
			switch (CurrentTab)
			{
				case Tab.Foreground:
				case Tab.Background:
					for (int cy = 0; cy < h / LevelData.Level.ChunkHeight; cy++)
						for (int cx = 0; cx < w / LevelData.Level.ChunkWidth; cx++)
							ImportChunk(ir.Mappings, blockcoldata, blocks, chunks, colInds1, colInds2, newBlocks, newChunks, newColInds1, newColInds2, layout, cx, cy);
					break;
				case Tab.Art:
					switch (CurrentArtTab)
					{
						case ArtTab.Chunks:
							for (int cy = 0; cy < h / LevelData.Level.ChunkHeight; cy++)
								for (int cx = 0; cx < w / LevelData.Level.ChunkWidth; cx++)
									ImportChunk(ir.Mappings, blockcoldata, blocks, chunks, colInds1, colInds2, newBlocks, newChunks, newColInds1, newColInds2, layout, cx, cy);
							break;
						case ArtTab.Blocks:
							for (int by = 0; by < h / 16; by++)
								for (int bx = 0; bx < w / 16; bx++)
									ImportBlock(ir.Mappings, blockcoldata, blocks, colInds1, colInds2, newBlocks, newColInds1, newColInds2, null, 0, 0, bx, by);
							break;
						case ArtTab.Tiles:
							break;
					}
					break;
			}
			if (newTiles.Count > 0 && LevelData.Tiles.Count + newTiles.Count > 0x800)
			{
				importProgressControl1.Hide();
				Enabled = true;
				UseWaitCursor = false;
				MessageBox.Show(this, "There are " + (LevelData.Tiles.Count + newTiles.Count - 0x800) + " tiles over the limit.\nImport cannot proceed.", "SonLVL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}
			if (newBlocks.Count > 0 && LevelData.Blocks.Count + newBlocks.Count > LevelData.GetBlockMax())
			{
				importProgressControl1.Hide();
				Enabled = true;
				UseWaitCursor = false;
				MessageBox.Show(this, "There are " + (LevelData.Blocks.Count + newBlocks.Count - LevelData.GetBlockMax()) + " blocks over the limit.\nImport cannot proceed.", "SonLVL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}
			if (newChunks.Count > 0 && LevelData.Chunks.Count + newChunks.Count > 256)
			{
				importProgressControl1.Hide();
				Enabled = true;
				UseWaitCursor = false;
				MessageBox.Show(this, "There are " + (LevelData.Chunks.Count + newChunks.Count - 256) + " chunks over the limit.\nImport cannot proceed.", "SonLVL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}
			if (newTiles.Count > 0)
			{
				foreach (byte[] t in newTiles)
					LevelData.Tiles.Add(t);
				LevelData.UpdateTileArray();
				RefreshTileSelector();
				TileSelector.SelectedIndex = TileSelector.Images.Count - 1;
			}
			if (newBlocks.Count > 0)
			{
				for (int i = 0; i < newBlocks.Count; i++)
				{
					Application.DoEvents();
					LevelData.Blocks.Add(newBlocks[i]);
					LevelData.ColInds1.AddOrSet(LevelData.Blocks.Count - 1, newColInds1[i]);
					switch (LevelData.Level.ChunkFormat)
					{
						case EngineVersion.S2NA:
						case EngineVersion.S2:
						case EngineVersion.S3K:
						case EngineVersion.SKC:
							if (!Object.ReferenceEquals(LevelData.ColInds1, LevelData.ColInds2))
								LevelData.ColInds2.AddOrSet(LevelData.Blocks.Count - 1, newColInds2[i]);
							break;
					}
					LevelData.BlockBmps.Add(new Bitmap[2]);
					LevelData.BlockBmpBits.Add(new BitmapBits[2]);
					LevelData.CompBlockBmps.Add(null);
					LevelData.CompBlockBmpBits.Add(null);
					LevelData.RedrawBlock(LevelData.Blocks.Count - 1, false);
				}
				SelectedBlock = LevelData.Blocks.Count - 1;
				BlockSelector.SelectedIndex = SelectedBlock;
			}
			else if (newTiles.Count > 0)
				blockTileEditor.SelectedObjects = blockTileEditor.SelectedObjects;
			if (newChunks.Count > 0)
			{
				foreach (Chunk c in newChunks)
				{
					Application.DoEvents();
					LevelData.Chunks.Add(c);
					LevelData.ChunkBmpBits.Add(new BitmapBits[2]);
					LevelData.ChunkBmps.Add(new Bitmap[2]);
					LevelData.ChunkColBmpBits.Add(new BitmapBits[2]);
					LevelData.ChunkColBmps.Add(new Bitmap[2]);
					LevelData.CompChunkBmps.Add(null);
					LevelData.CompChunkBmpBits.Add(null);
					LevelData.RedrawChunk(LevelData.Chunks.Count - 1);
				}
				SelectedChunk = (byte)(LevelData.Chunks.Count - 1);
				ChunkSelector.SelectedIndex = SelectedChunk;
			}
			else if (newBlocks.Count > 0)
				chunkBlockEditor.SelectedObjects = chunkBlockEditor.SelectedObjects;
			sw.Stop();
			System.Text.StringBuilder msg = new System.Text.StringBuilder();
			msg.AppendFormat("New tiles: {0:X}\n", newTiles.Count);
			msg.AppendFormat("New blocks: {0:X}\n", newBlocks.Count);
			msg.AppendFormat("New chunks: {0:X}\n", newChunks.Count);
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
			importProgressControl1.Hide();
			Enabled = true;
			UseWaitCursor = false;
			return true;
		}

		private void ImportChunk(PatternIndex[,] map, BlockColInfo[,] blockcoldata, List<byte[]> blocks, List<byte[]> chunks, List<byte> colInds1, List<byte> colInds2, List<Block> newBlocks, List<Chunk> newChunks, List<byte> newColInds1, List<byte> newColInds2, byte[,] layout, int cx, int cy)
		{
			Chunk cnk = new Chunk();
			for (int by = 0; by < LevelData.Level.ChunkHeight / 16; by++)
				for (int bx = 0; bx < LevelData.Level.ChunkWidth / 16; bx++)
					ImportBlock(map, blockcoldata, blocks, colInds1, colInds2, newBlocks, newColInds1, newColInds2, cnk, cx * LevelData.Level.ChunkWidth, cy * LevelData.Level.ChunkHeight, bx, by);
			byte[] cnkb = cnk.GetBytes();
			for (int i = 0; i < chunks.Count; i++)
			{
				Application.DoEvents();
				if (cnkb.FastArrayEqual(chunks[i]))
				{
					if (layout != null)
						layout[cx, cy] = (byte)i;
					return;
				}
			}
			chunks.Add(cnkb);
			newChunks.Add(cnk);
			if (layout != null)
				layout[cx, cy] = (byte)(LevelData.Chunks.Count + newChunks.Count - 1);
			return;
		}

		private void ImportBlock(PatternIndex[,] map, BlockColInfo[,] blockcoldata, List<byte[]> blocks, List<byte> colInds1, List<byte> colInds2, List<Block> newBlocks, List<byte> newColInds1, List<byte> newColInds2, Chunk cnk, int left, int top, int bx, int by)
		{
			BlockColInfo col = new BlockColInfo(0, 0, Solidity.NotSolid, Solidity.NotSolid, false, false);
			Block blk = new Block();
			if (blockcoldata != null)
			{
				col = blockcoldata[(left / 16) + bx, (top / 16) + by];
				if (cnk != null)
				{
					cnk.Blocks[bx, by].Solid1 = col.Solidity1;
					if (cnk.Blocks[bx, by] is S2ChunkBlock)
						((S2ChunkBlock)cnk.Blocks[bx, by]).Solid2 = col.Solidity2;
					cnk.Blocks[bx, by].XFlip = col.XFlip;
					cnk.Blocks[bx, by].YFlip = col.YFlip;
				}
			}
			for (int x = 0; x < 2; x++)
				for (int y = 0; y < 2; y++)
					blk.Tiles[x, y] = map[(left / 8) + (bx * 2) + x, (top / 8) + (by * 2) + y];
			if (LevelData.Level.TwoPlayerCompatible)
			{
				if (blk.Tiles[0, 0].Tile == 0 && blk.Tiles[1, 0].Tile == 0 && blk.Tiles[0, 1].Tile == 1 && blk.Tiles[1, 1].Tile == 1)
					return;
			}
			else if (blk.Tiles[0, 0].Tile == 0 && blk.Tiles[1, 0].Tile == 0 && blk.Tiles[0, 1].Tile == 0 && blk.Tiles[1, 1].Tile == 0)
				return;
			blk = blk.Flip(col.XFlip, col.YFlip);
			byte[] blkb = blk.GetBytes();
			byte[] blkh = blk.Flip(true, false).GetBytes();
			byte[] blkv = blk.Flip(false, true).GetBytes();
			byte[] blkhv = blk.Flip(true, true).GetBytes();
			for (int i = 0; i < blocks.Count; i++)
			{
				Application.DoEvents();
				if (blockcoldata != null)
				{
					if (colInds1[i] != col.ColInd1)
						continue;
					if (!Object.ReferenceEquals(LevelData.ColInds1, LevelData.ColInds2) && colInds2[i] != col.ColInd2)
						continue;
				}
				if (blkb.FastArrayEqual(blocks[i]))
				{
					if (cnk != null)
						cnk.Blocks[bx, by].Block = (ushort)i;
					return;
				}
				if (blkh.FastArrayEqual(blocks[i]))
				{
					if (cnk != null)
					{
						cnk.Blocks[bx, by].Block = (ushort)i;
						cnk.Blocks[bx, by].XFlip ^= true;
					}
					return;
				}
				if (blkv.FastArrayEqual(blocks[i]))
				{
					if (cnk != null)
					{
						cnk.Blocks[bx, by].Block = (ushort)i;
						cnk.Blocks[bx, by].YFlip ^= true;
					}
					return;
				}
				if (blkhv.FastArrayEqual(blocks[i]))
				{
					if (cnk != null)
					{
						cnk.Blocks[bx, by].Block = (ushort)i;
						cnk.Blocks[bx, by].XFlip ^= true;
						cnk.Blocks[bx, by].YFlip ^= true;
					}
					return;
				}
			}
			blocks.Add(blkb);
			colInds1.AddOrSet(blocks.Count - 1, col.ColInd1);
			colInds2.AddOrSet(blocks.Count - 1, col.ColInd2);
			newBlocks.Add(blk);
			newColInds1.Add(col.ColInd1);
			newColInds2.Add(col.ColInd2);
			if (cnk != null)
			{
				cnk.Blocks[bx, by].Block = (ushort)(LevelData.Blocks.Count + newBlocks.Count - 1);
				cnk.Blocks[bx, by].XFlip = col.XFlip;
				cnk.Blocks[bx, by].YFlip = col.YFlip;
			}
		}

		private int SelectedCol;
		private void CollisionSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (CollisionSelector.SelectedIndex > -1)
			{
				SelectedCol = CollisionSelector.SelectedIndex;
				ColAngle.Value = LevelData.Angles[SelectedCol];
				ColID.Text = SelectedCol.ToString("X2");
				DrawColPicture();
			}
		}

		private void DrawColPicture()
		{
			if (CollisionSelector.SelectedIndex == -1) return;
			using (Graphics gfx = ColPicture.CreateGraphics())
			{
				gfx.SetOptions();
				if (showBlockBehindCollisionCheckBox.Checked)
				{
					BitmapBits bmp = new BitmapBits(16, 16);
					bmp.Bits.FastFill(0x20);
					bmp.DrawBitmapComposited(LevelData.CompBlockBmpBits[SelectedBlock], 0, 0);
					BitmapBits tmp = new BitmapBits(LevelData.ColBmpBits[SelectedCol]);
					tmp.IncrementIndexes(LevelData.ColorWhite - 1);
					bmp.DrawBitmapComposited(tmp, 0, 0);
					gfx.DrawImage(bmp.Scale(8).ToBitmap(LevelImgPalette), 0, 0, 128, 128);
				}
				else
					gfx.DrawImage(LevelData.ColBmpBits[SelectedCol].Scale(8).ToBitmap(Color.Black, Color.White), 0, 0, 128, 128);
			}
		}

		private void ColPicture_Paint(object sender, PaintEventArgs e)
		{
			DrawColPicture();
		}

		private void ColPicture_MouseDown(object sender, MouseEventArgs e)
		{
			if (CollisionSelector.SelectedIndex == -1) return;
			int x = e.X / 8;
			int y = e.Y / 8;
			switch (e.Button)
			{
				case MouseButtons.Left:
					LevelData.ColArr1[SelectedCol][x] = (sbyte)(16 - y);
					break;
				case MouseButtons.Right:
					if (y == 16)
						LevelData.ColArr1[SelectedCol][x] = 0;
					else
						LevelData.ColArr1[SelectedCol][x] = (sbyte)(-y - 1);
					break;
			}
			LevelData.RedrawCol(SelectedCol, false);
			DrawColPicture();
			CollisionSelector.Images[SelectedCol] = LevelData.ColBmps[SelectedCol];
		}

		private void ColPicture_MouseMove(object sender, MouseEventArgs e)
		{
			if (CollisionSelector.SelectedIndex == -1) return;
			int x = e.X / 8;
			if (x < 0 | x > 15) return;
			int y = e.Y / 8;
			if (y < 0 | y > 16) return;
			switch (e.Button)
			{
				case MouseButtons.Left:
					LevelData.ColArr1[SelectedCol][x] = (sbyte)(16 - y);
					LevelData.RedrawCol(SelectedCol, false);
					DrawColPicture();
					CollisionSelector.Images[SelectedCol] = LevelData.ColBmps[SelectedCol];
					break;
				case MouseButtons.Right:
					if (y == 16)
						LevelData.ColArr1[SelectedCol][x] = 0;
					else
						LevelData.ColArr1[SelectedCol][x] = (sbyte)(-y - 1);
					LevelData.RedrawCol(SelectedCol, false);
					DrawColPicture();
					CollisionSelector.Images[SelectedCol] = LevelData.ColBmps[SelectedCol];
					break;
			}
		}

		private void ColPicture_MouseUp(object sender, MouseEventArgs e)
		{
			if (CollisionSelector.SelectedIndex == -1) return;
			LevelData.RedrawCol(SelectedCol, true);
			DrawColPicture();
			CollisionSelector.Images[SelectedCol] = LevelData.ColBmps[SelectedCol];
		}

		private void ColAngle_ValueChanged(object sender, EventArgs e)
		{
			if (CollisionSelector.SelectedIndex == -1) return;
			LevelData.Angles[SelectedCol] = (byte)ColAngle.Value;
		}

		private void ColAngle_TextChanged(object sender, EventArgs e)
		{
			if (!loaded) return;
			byte value;
			if (byte.TryParse(ColAngle.Text, System.Globalization.NumberStyles.HexNumber, null, out value))
				ColAngle.Value = value;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (!loaded) return;
			using (CollisionSelector sel = new CollisionSelector())
				if (sel.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
					BlockCollision1.Value = sel.Selection;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			if (!loaded) return;
			using (CollisionSelector sel = new CollisionSelector())
				if (sel.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
					BlockCollision2.Value = sel.Selection;
		}

		private void rotateTileRightButton_Click(object sender, EventArgs e)
		{
			tile.Rotate(3);
			LevelData.Tiles[SelectedTile] = tile.ToTile();
			LevelData.Tiles[SelectedTile].CopyTo(LevelData.TileArray, SelectedTile * 32);
			for (int i = 0; i < LevelData.Blocks.Count; i++)
			{
				bool dr = false;
				for (int y = 0; y < 2; y++)
					for (int x = 0; x < 2; x++)
						if (LevelData.Blocks[i].Tiles[x, y].Tile == SelectedTile)
							dr = true;
				if (dr)
					LevelData.RedrawBlock(i, true);
			}
			TileSelector.Images[SelectedTile] = LevelData.TileToBmp4bpp(LevelData.Tiles[SelectedTile], 0, SelectedColor.Y);
			blockTileEditor.SelectedObjects = blockTileEditor.SelectedObjects;
			TilePicture.Invalidate();
		}

		private void drawToolStripButton_Click(object sender, EventArgs e)
		{
			using (DrawTileDialog dlg = new DrawTileDialog())
			{
				switch (CurrentArtTab)
				{
					case ArtTab.Chunks:
						dlg.tile = new BitmapBits(LevelData.Level.ChunkWidth, LevelData.Level.ChunkHeight);
						break;
					case ArtTab.Blocks:
						dlg.tile = new BitmapBits(16, 16);
						break;
					case ArtTab.Tiles:
						dlg.tile = new BitmapBits(8, LevelData.Level.TwoPlayerCompatible ? 16 : 8);
						break;
				}
				if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
					ImportImage(dlg.tile.ToBitmap(LevelData.BmpPal), null, null, null, null);
			}
		}

		private void TileList_KeyDown(object sender, KeyEventArgs e)
		{
			if (CurrentTab > Tab.Background)
			{
				switch (e.KeyCode)
				{
					case Keys.C:
						if (e.Control)
							copyTilesToolStripMenuItem_Click(sender, EventArgs.Empty);
						break;
					case Keys.D:
						if (e.Control)
							switch (CurrentArtTab)
							{
								case ArtTab.Chunks:
									if (LevelData.Chunks.Count < 0x100)
										duplicateTilesToolStripMenuItem_Click(sender, EventArgs.Empty);
									break;
								case ArtTab.Blocks:
									if (LevelData.Blocks.Count < LevelData.GetBlockMax())
										duplicateTilesToolStripMenuItem_Click(sender, EventArgs.Empty);
									break;
								case ArtTab.Tiles:
									if (LevelData.Tiles.Count < 0x800)
										duplicateTilesToolStripMenuItem_Click(sender, EventArgs.Empty);
									break;
							}
						break;
					case Keys.Delete:
						switch (CurrentArtTab)
						{
							case ArtTab.Chunks:
								if (LevelData.Chunks.Count > 1)
									deleteTilesToolStripMenuItem_Click(sender, EventArgs.Empty);
								break;
							case ArtTab.Blocks:
								if (LevelData.Blocks.Count > 1)
									deleteTilesToolStripMenuItem_Click(sender, EventArgs.Empty);
								break;
							case ArtTab.Tiles:
								if (TileSelector.Images.Count > 1)
									deleteTilesToolStripMenuItem_Click(sender, EventArgs.Empty);
								break;
						}
						break;
					case Keys.Insert:
						switch (CurrentArtTab)
						{
							case ArtTab.Chunks:
								if (LevelData.Chunks.Count < 0x100)
									insertBeforeToolStripMenuItem_Click(sender, EventArgs.Empty);
								break;
							case ArtTab.Blocks:
								if (LevelData.Blocks.Count < LevelData.GetBlockMax())
									insertBeforeToolStripMenuItem_Click(sender, EventArgs.Empty);
								break;
							case ArtTab.Tiles:
								if (LevelData.Tiles.Count < 0x800)
									insertBeforeToolStripMenuItem_Click(sender, EventArgs.Empty);
								break;
						}
						break;
					case Keys.V:
						if (e.Control)
							switch (CurrentArtTab)
							{
								case ArtTab.Chunks:
									if ((Clipboard.ContainsData(typeof(ChunkCopyData).AssemblyQualifiedName) || Clipboard.ContainsData(typeof(Chunk).AssemblyQualifiedName)) && LevelData.Chunks.Count < 0x100)
										pasteAfterToolStripMenuItem_Click(sender, EventArgs.Empty);
									break;
								case ArtTab.Blocks:
									if ((Clipboard.ContainsData(typeof(BlockCopyData).AssemblyQualifiedName) || Clipboard.ContainsData(typeof(Block).AssemblyQualifiedName)) && LevelData.Blocks.Count < LevelData.GetBlockMax())
										pasteAfterToolStripMenuItem_Click(sender, EventArgs.Empty);
									break;
								case ArtTab.Tiles:
									if (Clipboard.ContainsData(LevelData.Level.TwoPlayerCompatible ? "SonLVLTileInterlaced" : "SonLVLTile") & LevelData.Tiles.Count < 0x800)
										pasteAfterToolStripMenuItem_Click(sender, EventArgs.Empty);
									break;
							}
						break;
					case Keys.X:
						if (e.Control)
							switch (CurrentArtTab)
							{
								case ArtTab.Chunks:
									if (LevelData.Chunks.Count > 1)
										cutTilesToolStripMenuItem_Click(sender, EventArgs.Empty);
									break;
								case ArtTab.Blocks:
									if (LevelData.Blocks.Count > 1)
										cutTilesToolStripMenuItem_Click(sender, EventArgs.Empty);
									break;
								case ArtTab.Tiles:
									if (TileSelector.Images.Count > 1)
										cutTilesToolStripMenuItem_Click(sender, EventArgs.Empty);
									break;
							}
						break;
				}
			}
		}

		private void selectAllObjectsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SelectedItems = new List<Entry>(LevelData.Objects.Cast<Entry>());
			SelectedObjectChanged();
		}

		private void selectAllRingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			List<Entry> items = new List<Entry>();
			if (LevelData.RingFormat is RingLayoutFormat)
				items.AddRange(LevelData.Rings.Cast<Entry>());
			else
				items.AddRange(LevelData.Objects.Where(a => a.ID == ((RingObjectFormat)LevelData.RingFormat).ObjectID).Cast<Entry>());
			SelectedItems = items;
			SelectedObjectChanged();
		}

		private void cutToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			byte[,] layout;
			bool[,] loop;
			Rectangle selection;
			if (CurrentTab == Tab.Background)
			{
				layout = LevelData.Layout.BGLayout;
				loop = LevelData.Layout.BGLoop;
				selection = BGSelection;
			}
			else
			{
				layout = LevelData.Layout.FGLayout;
				loop = LevelData.Layout.FGLoop;
				selection = FGSelection;
			}
			byte[,] layoutsection = new byte[selection.Width, selection.Height];
			bool[,] loopsection = loop == null ? null : new bool[selection.Width, selection.Height];
			for (int y = 0; y < selection.Height; y++)
				for (int x = 0; x < selection.Width; x++)
				{
					layoutsection[x, y] = layout[x + selection.X, y + selection.Y];
					layout[x + selection.X, y + selection.Y] = 0;
					if (loop != null)
					{
						loopsection[x, y] = loop[x + selection.X, y + selection.Y];
						loop[x + selection.X, y + selection.Y] = false;
					}
				}
			List<Entry> objectselection = new List<Entry>();
			List<Entry> objstodelete = new List<Entry>();
			if (includeObjectsWithForegroundSelectionToolStripMenuItem.Checked & CurrentTab == Tab.Foreground)
			{
				int x = selection.Left * LevelData.Level.ChunkWidth;
				int y = selection.Top * LevelData.Level.ChunkHeight;
				if (LevelData.Objects != null)
					foreach (ObjectEntry item in LevelData.Objects)
						if (item.Y >= y & item.Y < selection.Bottom * LevelData.Level.ChunkHeight
							& item.X >= x & item.X < selection.Right * LevelData.Level.ChunkWidth)
						{
							Entry ent = item.Clone();
							ent.X -= (ushort)x;
							ent.Y -= (ushort)y;
							objectselection.Add(ent);
							objstodelete.Add(item);
						}
				if (LevelData.Rings != null)
					foreach (RingEntry item in LevelData.Rings)
						if (item.Y >= y & item.Y < selection.Bottom * LevelData.Level.ChunkHeight
							& item.X >= x & item.X < selection.Right * LevelData.Level.ChunkWidth)
						{
							Entry ent = item.Clone();
							ent.X -= (ushort)x;
							ent.Y -= (ushort)y;
							objectselection.Add(ent);
							objstodelete.Add(item);
						}
				if (LevelData.Bumpers != null)
					foreach (CNZBumperEntry item in LevelData.Bumpers)
						if (item.Y >= y & item.Y < selection.Bottom * LevelData.Level.ChunkHeight
							& item.X >= x & item.X < selection.Right * LevelData.Level.ChunkWidth)
						{
							Entry ent = item.Clone();
							ent.X -= (ushort)x;
							ent.Y -= (ushort)y;
							objectselection.Add(ent);
							objstodelete.Add(item);
						}
				foreach (Entry item in objstodelete)
				{
					if (item is ObjectEntry)
						LevelData.Objects.Remove((ObjectEntry)item);
					if (item is RingEntry)
						LevelData.Rings.Remove((RingEntry)item);
					if (item is CNZBumperEntry)
						LevelData.Bumpers.Remove((CNZBumperEntry)item);
					if (SelectedItems.Contains(item))
						SelectedItems.Remove(item);
				}
				SelectedObjectChanged();
			}
			Clipboard.SetData(typeof(LayoutSection).AssemblyQualifiedName, new LayoutSection(layoutsection, loopsection, objectselection));
			DrawLevel();
		}

		private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			Clipboard.SetData(typeof(LayoutSection).AssemblyQualifiedName, CreateLayoutSection());
		}

		private LayoutSection CreateLayoutSection()
		{
			byte[,] layout;
			bool[,] loop;
			Rectangle selection;
			if (CurrentTab == Tab.Background)
			{
				layout = LevelData.Layout.BGLayout;
				loop = LevelData.Layout.BGLoop;
				selection = BGSelection;
			}
			else
			{
				layout = LevelData.Layout.FGLayout;
				loop = LevelData.Layout.FGLoop;
				selection = FGSelection;
			}
			byte[,] layoutsection = new byte[selection.Width, selection.Height];
			bool[,] loopsection = loop == null ? null : new bool[selection.Width, selection.Height];
			for (int y = 0; y < selection.Height; y++)
				for (int x = 0; x < selection.Width; x++)
				{
					layoutsection[x, y] = layout[x + selection.X, y + selection.Y];
					if (loop != null)
						loopsection[x, y] = loop[x + selection.X, y + selection.Y];
				}
			List<Entry> objectselection = new List<Entry>();
			if (includeObjectsWithForegroundSelectionToolStripMenuItem.Checked & CurrentTab == Tab.Foreground)
			{
				int x = selection.Left * LevelData.Level.ChunkWidth;
				int y = selection.Top * LevelData.Level.ChunkHeight;
				if (LevelData.Objects != null)
					foreach (ObjectEntry item in LevelData.Objects)
						if (item.Y >= y & item.Y < selection.Bottom * LevelData.Level.ChunkHeight
							& item.X >= x & item.X < selection.Right * LevelData.Level.ChunkWidth)
						{
							Entry ent = item.Clone();
							ent.X -= (ushort)x;
							ent.Y -= (ushort)y;
							objectselection.Add(ent);
						}
				if (LevelData.Rings != null)
					foreach (RingEntry item in LevelData.Rings)
						if (item.Y >= y & item.Y < selection.Bottom * LevelData.Level.ChunkHeight
							& item.X >= x & item.X < selection.Right * LevelData.Level.ChunkWidth)
						{
							Entry ent = item.Clone();
							ent.X -= (ushort)x;
							ent.Y -= (ushort)y;
							objectselection.Add(ent);
						}
				if (LevelData.Bumpers != null)
					foreach (CNZBumperEntry item in LevelData.Bumpers)
						if (item.Y >= y & item.Y < selection.Bottom * LevelData.Level.ChunkHeight
							& item.X >= x & item.X < selection.Right * LevelData.Level.ChunkWidth)
						{
							Entry ent = item.Clone();
							ent.X -= (ushort)x;
							ent.Y -= (ushort)y;
							objectselection.Add(ent);
						}
			}
			return new LayoutSection(layoutsection, loopsection, objectselection);
		}

		private void pasteOnceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LayoutSection section = (LayoutSection)Clipboard.GetData(typeof(LayoutSection).AssemblyQualifiedName);
			PasteLayoutSectionOnce(section);
		}

		private void PasteLayoutSectionOnce(LayoutSection section)
		{
			byte[,] layout;
			bool[,] loop;
			int w, h;
			if (CurrentTab == Tab.Background)
			{
				layout = LevelData.Layout.BGLayout;
				loop = LevelData.Layout.BGLoop;
				w = Math.Min(section.Layout.GetLength(0), LevelData.BGWidth - menuLoc.X);
				h = Math.Min(section.Layout.GetLength(1), LevelData.BGHeight - menuLoc.Y);
			}
			else
			{
				layout = LevelData.Layout.FGLayout;
				loop = LevelData.Layout.FGLoop;
				w = Math.Min(section.Layout.GetLength(0), LevelData.FGWidth - menuLoc.X);
				h = Math.Min(section.Layout.GetLength(1), LevelData.FGHeight - menuLoc.Y);
			}
			for (int y = 0; y < h; y++)
				for (int x = 0; x < w; x++)
				{
					layout[x + menuLoc.X, y + menuLoc.Y] = section.Layout[x, y];
					if (loop != null)
					{
						if (section.Loop == null)
							loop[x + menuLoc.X, y + menuLoc.Y] = false;
						else
							loop[x + menuLoc.X, y + menuLoc.Y] = section.Loop[x, y];
					}
				}
			if (includeObjectsWithForegroundSelectionToolStripMenuItem.Checked & CurrentTab == Tab.Foreground)
			{
				Size off = new Size(menuLoc.X * LevelData.Level.ChunkWidth, menuLoc.Y * LevelData.Level.ChunkHeight);
				foreach (Entry item in section.Objects)
				{
					Entry newent = item.Clone();
					newent.X = (ushort)(newent.X + off.Width);
					newent.Y = (ushort)(newent.Y + off.Height);
					if (newent is ObjectEntry)
						LevelData.Objects.Add((ObjectEntry)newent);
					else if (newent is RingEntry)
						LevelData.Rings.Add((RingEntry)newent);
					else if (newent is CNZBumperEntry)
						LevelData.Bumpers.Add((CNZBumperEntry)newent);
					newent.UpdateSprite();
				}
				LevelData.Objects.Sort();
				LevelData.Rings.Sort();
				if (LevelData.Bumpers != null)
					LevelData.Bumpers.Sort();
			}
			DrawLevel();
		}

		private void pasteRepeatingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			LayoutSection section = (LayoutSection)Clipboard.GetData(typeof(LayoutSection).AssemblyQualifiedName);
			PasteLayoutSectionRepeating(section);
		}

		private void PasteLayoutSectionRepeating(LayoutSection section)
		{
			byte[,] layout;
			bool[,] loop;
			Rectangle selection;
			if (CurrentTab == Tab.Background)
			{
				layout = LevelData.Layout.BGLayout;
				loop = LevelData.Layout.BGLoop;
				selection = BGSelection;
			}
			else
			{
				layout = LevelData.Layout.FGLayout;
				loop = LevelData.Layout.FGLoop;
				selection = FGSelection;
			}
			int width = section.Layout.GetLength(0);
			int height = section.Layout.GetLength(1);
			for (int y = 0; y < selection.Height; y++)
				for (int x = 0; x < selection.Width; x++)
				{
					layout[x + selection.X, y + selection.Y] = section.Layout[x % width, y % height];
					if (loop != null)
					{
						if (section.Loop == null)
							loop[x + selection.X, y + selection.Y] = false;
						else
							loop[x + selection.X, y + selection.Y] = section.Loop[x % width, y % height];
					}
				}
			if (includeObjectsWithForegroundSelectionToolStripMenuItem.Checked & CurrentTab == Tab.Foreground)
			{
				int w = (int)Math.Ceiling(selection.Width / (double)width);
				int h = (int)Math.Ceiling(selection.Height / (double)height);
				Point bottomright = new Point(selection.Right * LevelData.Level.ChunkWidth, selection.Bottom * LevelData.Level.ChunkHeight);
				for (int y = 0; y < h; y++)
					for (int x = 0; x < w; x++)
					{
						Size off = new Size((selection.X + (x * width)) * LevelData.Level.ChunkWidth, (selection.Y + (y * height)) * LevelData.Level.ChunkHeight);
						foreach (Entry item in section.Objects)
						{
							Entry it2 = item.Clone();
							it2.FromBytes(item.GetBytes());
							it2.X = (ushort)(it2.X + off.Width);
							it2.Y = (ushort)(it2.Y + off.Height);
							if (it2.X < bottomright.X & it2.Y < bottomright.Y)
							{
								if (it2 is ObjectEntry)
									LevelData.Objects.Add((ObjectEntry)it2);
								else if (it2 is RingEntry)
									LevelData.Rings.Add((RingEntry)it2);
								else if (it2 is CNZBumperEntry)
									LevelData.Bumpers.Add((CNZBumperEntry)it2);
								it2.UpdateSprite();
							}
						}
					}
				LevelData.Objects.Sort();
				LevelData.Rings.Sort();
				if (LevelData.Bumpers != null)
					LevelData.Bumpers.Sort();
			}
			DrawLevel();
		}

		private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			byte[,] layout;
			bool[,] loop;
			Rectangle selection;
			if (CurrentTab == Tab.Background)
			{
				layout = LevelData.Layout.BGLayout;
				loop = LevelData.Layout.BGLoop;
				selection = BGSelection;
			}
			else
			{
				layout = LevelData.Layout.FGLayout;
				loop = LevelData.Layout.FGLoop;
				selection = FGSelection;
			}
			for (int y = selection.Top; y < selection.Bottom; y++)
				for (int x = selection.Left; x < selection.Right; x++)
				{
					layout[x, y] = 0;
					if (loop != null)
						loop[x, y] = false;
				}
			if (includeObjectsWithForegroundSelectionToolStripMenuItem.Checked & CurrentTab == Tab.Foreground)
			{
				List<Entry> objectselection = new List<Entry>();
				if (LevelData.Objects != null)
					foreach (ObjectEntry item in LevelData.Objects)
						if (item.Y >= selection.Top * LevelData.Level.ChunkHeight & item.Y < selection.Bottom * LevelData.Level.ChunkHeight
							& item.X >= selection.Left * LevelData.Level.ChunkWidth & item.X < selection.Right * LevelData.Level.ChunkWidth)
							objectselection.Add(item);
				if (LevelData.Rings != null)
					foreach (RingEntry item in LevelData.Rings)
						if (item.Y >= selection.Top * LevelData.Level.ChunkHeight & item.Y < selection.Bottom * LevelData.Level.ChunkHeight
							& item.X >= selection.Left * LevelData.Level.ChunkWidth & item.X < selection.Right * LevelData.Level.ChunkWidth)
							objectselection.Add(item);
				if (LevelData.Bumpers != null)
					foreach (CNZBumperEntry item in LevelData.Bumpers)
						if (item.Y >= selection.Top * LevelData.Level.ChunkHeight & item.Y < selection.Bottom * LevelData.Level.ChunkHeight
							& item.X >= selection.Left * LevelData.Level.ChunkWidth & item.X < selection.Right * LevelData.Level.ChunkWidth)
							objectselection.Add(item);
				foreach (Entry item in objectselection)
				{
					if (item is ObjectEntry)
						LevelData.Objects.Remove((ObjectEntry)item);
					if (item is RingEntry)
						LevelData.Rings.Remove((RingEntry)item);
					if (item is CNZBumperEntry)
						LevelData.Bumpers.Remove((CNZBumperEntry)item);
					if (SelectedItems.Contains(item))
						SelectedItems.Remove(item);
				}
				SelectedObjectChanged();
			}
			DrawLevel();
		}

		private void fillToolStripMenuItem_Click(object sender, EventArgs e)
		{
			byte[,] layout;
			bool[,] loop;
			Rectangle selection;
			if (CurrentTab == Tab.Background)
			{
				layout = LevelData.Layout.BGLayout;
				loop = LevelData.Layout.BGLoop;
				selection = BGSelection;
			}
			else
			{
				layout = LevelData.Layout.FGLayout;
				loop = LevelData.Layout.FGLoop;
				selection = FGSelection;
			}
			for (int y = selection.Top; y < selection.Bottom; y++)
				for (int x = selection.Left; x < selection.Right; x++)
				{
					layout[x, y] = SelectedChunk;
					if (loop != null)
						loop[x, y] = false;
				}
			DrawLevel();
		}

		private void resizeLevelToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (ResizeLevelDialog dg = new ResizeLevelDialog(CurrentTab != Tab.Background))
			{
				dg.levelWidth.Minimum = 1;
				dg.levelHeight.Minimum = 1;
				if (LevelData.LayoutFormat.IsResizable)
				{
					Size maxsize = LevelData.LayoutFormat.MaxSize;
					dg.levelWidth.Maximum = maxsize.Width;
					dg.levelHeight.Maximum = maxsize.Height;
					Size cursize;
					if (CurrentTab == Tab.Background)
						cursize = LevelData.BGSize;
					else
						cursize = LevelData.FGSize;
					dg.levelHeight.Value = cursize.Height;
					dg.levelWidth.Value = cursize.Width;
					if (dg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
					{
						if (CurrentTab == Tab.Background)
							LevelData.ResizeBG((int)dg.levelWidth.Value, (int)dg.levelHeight.Value);
						else
							LevelData.ResizeFG((int)dg.levelWidth.Value, (int)dg.levelHeight.Value);
						loaded = false;
						UpdateScrollBars();
						loaded = true;
						DrawLevel();
					}
				}
				else
					MessageBox.Show(this, "The current game does not allow you to resize levels!", Text);
			}
		}

		private void objectTypeList_ItemDrag(object sender, ItemDragEventArgs e)
		{
			objectTypeList.DoDragDrop(new DataObject("SonicRetro.SonLVL.GUI.ObjectDrop", ((ListViewItem)e.Item).Tag), DragDropEffects.Copy);
		}

		private void objectPanel_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent("SonicRetro.SonLVL.GUI.ObjectDrop"))
			{
				e.Effect = DragDropEffects.All;
				dragdrop = true;
				dragobj = (byte)e.Data.GetData("SonicRetro.SonLVL.GUI.ObjectDrop");
				dragpoint = objectPanel.PointToClient(new Point(e.X, e.Y));
				dragpoint = new Point((int)(dragpoint.X / ZoomLevel), (int)(dragpoint.Y / ZoomLevel));
				DrawLevel();
			}
			else
				dragdrop = false;
		}

		private void objectPanel_DragOver(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent("SonicRetro.SonLVL.GUI.ObjectDrop"))
			{
				e.Effect = DragDropEffects.All;
				dragdrop = true;
				dragobj = (byte)e.Data.GetData("SonicRetro.SonLVL.GUI.ObjectDrop");
				dragpoint = objectPanel.PointToClient(new Point(e.X, e.Y));
				dragpoint = new Point((int)(dragpoint.X / ZoomLevel), (int)(dragpoint.Y / ZoomLevel));
				DrawLevel();
			}
			else
				dragdrop = false;
		}

		private void objectPanel_DragLeave(object sender, EventArgs e)
		{
			dragdrop = false;
			DrawLevel();
		}

		private void objectPanel_DragDrop(object sender, DragEventArgs e)
		{
			dragdrop = false;
			if (e.Data.GetDataPresent("SonicRetro.SonLVL.GUI.ObjectDrop"))
			{
				double gs = 1 << ObjGrid;
				Point clientPoint = objectPanel.PointToClient(new Point(e.X, e.Y));
				clientPoint = new Point((int)(clientPoint.X / ZoomLevel), (int)(clientPoint.Y / ZoomLevel));
				ObjectEntry obj = LevelData.CreateObject((byte)e.Data.GetData("SonicRetro.SonLVL.GUI.ObjectDrop"));
				obj.X = (ushort)(Math.Round((clientPoint.X + hScrollBar1.Value) / gs, MidpointRounding.AwayFromZero) * gs);
				obj.Y = (ushort)(Math.Round((clientPoint.Y + vScrollBar1.Value) / gs, MidpointRounding.AwayFromZero) * gs);
				obj.UpdateSprite();
				LevelData.Objects.Add(obj);
				LevelData.Objects.Sort();
				SelectedItems = new List<Entry>(1) { obj };
				SelectedObjectChanged();
				DrawLevel();
			}
		}

		private void insertLayoutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (InsertDeleteDialog dlg = new InsertDeleteDialog())
			{
				dlg.Text = "Insert";
				dlg.moveObjects.Visible = dlg.moveObjects.Checked = CurrentTab == Tab.Foreground;
				if (dlg.ShowDialog(this) != DialogResult.OK) return;
				Rectangle selection;
				if (CurrentTab == Tab.Background)
					selection = BGSelection;
				else
					selection = FGSelection;
				Size maxsize = LevelData.LayoutFormat.MaxSize;
				if (dlg.shiftH.Checked)
				{
					byte[,] layout;
					bool[,] loop;
					if (CurrentTab == Tab.Background)
					{
						if (LevelData.LayoutFormat.IsResizable && maxsize.Width > LevelData.BGWidth)
							LevelData.ResizeBG(Math.Min(maxsize.Width, LevelData.BGWidth + selection.Width), LevelData.BGHeight);
						layout = LevelData.Layout.BGLayout;
						loop = LevelData.Layout.BGLoop;
					}
					else
					{
						if (LevelData.LayoutFormat.IsResizable && maxsize.Width > LevelData.FGWidth)
							LevelData.ResizeFG(Math.Min(maxsize.Width, LevelData.FGWidth + selection.Width), LevelData.FGHeight);
						layout = LevelData.Layout.FGLayout;
						loop = LevelData.Layout.FGLoop;
					}
					for (int y = selection.Top; y < selection.Bottom; y++)
						for (int x = layout.GetLength(0) - selection.Width - 1; x >= selection.Left; x--)
						{
							layout[x + selection.Width, y] = layout[x, y];
							if (loop != null)
								loop[x + selection.Width, y] = loop[x, y];
						}
					for (int y = selection.Top; y < selection.Bottom; y++)
						for (int x = selection.Left; x < selection.Right; x++)
						{
							layout[x, y] = 0;
							if (loop != null)
								loop[x, y] = false;
						}
					if (dlg.moveObjects.Checked)
					{
						if (LevelData.Objects != null)
							foreach (ObjectEntry item in LevelData.Objects)
								if (item.Y >= selection.Top * LevelData.Level.ChunkHeight & item.Y < selection.Bottom * LevelData.Level.ChunkHeight & item.X >= selection.Left * LevelData.Level.ChunkWidth)
								{
									item.X += (ushort)(selection.Width * LevelData.Level.ChunkWidth);
									item.UpdateSprite();
								}
						if (LevelData.Rings != null)
							foreach (RingEntry item in LevelData.Rings)
								if (item.Y >= selection.Top * LevelData.Level.ChunkHeight & item.Y < selection.Bottom * LevelData.Level.ChunkHeight & item.X >= selection.Left * LevelData.Level.ChunkWidth)
								{
									item.X += (ushort)(selection.Width * LevelData.Level.ChunkWidth);
									item.UpdateSprite();
								}
						if (LevelData.Bumpers != null)
							foreach (CNZBumperEntry item in LevelData.Bumpers)
								if (item.Y >= selection.Top * LevelData.Level.ChunkHeight & item.Y < selection.Bottom * LevelData.Level.ChunkHeight & item.X >= selection.Left * LevelData.Level.ChunkWidth)
								{
									item.X += (ushort)(selection.Width * LevelData.Level.ChunkWidth);
									item.UpdateSprite();
								}
						if (LevelData.StartPositions != null)
							foreach (StartPositionEntry item in LevelData.StartPositions)
								if (item.Y >= selection.Top * LevelData.Level.ChunkHeight & item.Y < selection.Bottom * LevelData.Level.ChunkHeight & item.X >= selection.Left * LevelData.Level.ChunkWidth)
								{
									item.X += (ushort)(selection.Width * LevelData.Level.ChunkWidth);
									item.UpdateSprite();
								}
					}
				}
				else if (dlg.shiftV.Checked)
				{
					byte[,] layout;
					bool[,] loop;
					if (CurrentTab == Tab.Background)
					{
						if (LevelData.LayoutFormat.IsResizable && maxsize.Height > LevelData.BGHeight)
							LevelData.ResizeBG(LevelData.BGWidth, Math.Min(maxsize.Height, LevelData.BGHeight + selection.Height));
						layout = LevelData.Layout.BGLayout;
						loop = LevelData.Layout.BGLoop;
					}
					else
					{
						if (LevelData.LayoutFormat.IsResizable && maxsize.Height > LevelData.FGHeight)
							LevelData.ResizeFG(LevelData.FGWidth, Math.Min(maxsize.Height, LevelData.FGHeight + selection.Height));
						layout = LevelData.Layout.FGLayout;
						loop = LevelData.Layout.FGLoop;
					}
					for (int x = selection.Left; x < selection.Right; x++)
						for (int y = layout.GetLength(1) - selection.Height - 1; y >= selection.Top; y--)
						{
							layout[x, y + selection.Height] = layout[x, y];
							if (loop != null)
								loop[x, y + selection.Height] = loop[x, y];
						}
					for (int x = selection.Left; x < selection.Right; x++)
						for (int y = selection.Top; y < selection.Bottom; y++)
						{
							layout[x, y] = 0;
							if (loop != null)
								loop[x, y] = false;
						}
					if (dlg.moveObjects.Checked)
					{
						if (LevelData.Objects != null)
							foreach (ObjectEntry item in LevelData.Objects)
								if (item.X >= selection.Left * LevelData.Level.ChunkWidth & item.X < selection.Right * LevelData.Level.ChunkWidth & item.Y >= selection.Top * LevelData.Level.ChunkHeight)
								{
									item.Y += (ushort)(selection.Height * LevelData.Level.ChunkHeight);
									item.UpdateSprite();
								}
						if (LevelData.Rings != null)
							foreach (RingEntry item in LevelData.Rings)
								if (item.X >= selection.Left * LevelData.Level.ChunkWidth & item.X < selection.Right * LevelData.Level.ChunkWidth & item.Y >= selection.Top * LevelData.Level.ChunkHeight)
								{
									item.Y += (ushort)(selection.Height * LevelData.Level.ChunkHeight);
									item.UpdateSprite();
								}
						if (LevelData.Bumpers != null)
							foreach (CNZBumperEntry item in LevelData.Bumpers)
								if (item.X >= selection.Left * LevelData.Level.ChunkWidth & item.X < selection.Right * LevelData.Level.ChunkWidth & item.Y >= selection.Top * LevelData.Level.ChunkHeight)
								{
									item.Y += (ushort)(selection.Height * LevelData.Level.ChunkHeight);
									item.UpdateSprite();
								}
						if (LevelData.StartPositions != null)
							foreach (StartPositionEntry item in LevelData.StartPositions)
								if (item.X >= selection.Left * LevelData.Level.ChunkWidth & item.X < selection.Right * LevelData.Level.ChunkWidth & item.Y >= selection.Top * LevelData.Level.ChunkHeight)
								{
									item.Y += (ushort)(selection.Height * LevelData.Level.ChunkHeight);
									item.UpdateSprite();
								}
					}
				}
				else if (dlg.entireRow.Checked)
				{
					byte[,] layout;
					bool[,] loop;
					if (CurrentTab == Tab.Background)
					{
						if (LevelData.LayoutFormat.IsResizable && maxsize.Height > LevelData.BGHeight)
							LevelData.ResizeBG(LevelData.BGWidth, Math.Min(maxsize.Height, LevelData.BGHeight + selection.Height));
						layout = LevelData.Layout.BGLayout;
						loop = LevelData.Layout.BGLoop;
					}
					else
					{
						if (LevelData.LayoutFormat.IsResizable && maxsize.Height > LevelData.FGHeight)
							LevelData.ResizeFG(LevelData.FGWidth, Math.Min(maxsize.Height, LevelData.FGHeight + selection.Height));
						layout = LevelData.Layout.FGLayout;
						loop = LevelData.Layout.FGLoop;
					}
					for (int x = 0; x < layout.GetLength(0); x++)
						for (int y = layout.GetLength(1) - selection.Height - 1; y >= selection.Top; y--)
						{
							layout[x, y + selection.Height] = layout[x, y];
							if (loop != null)
								loop[x, y + selection.Height] = loop[x, y];
						}
					for (int x = 0; x < layout.GetLength(0); x++)
						for (int y = selection.Top; y < selection.Bottom; y++)
						{
							layout[x, y] = 0;
							if (loop != null)
								loop[x, y] = false;
						}
					if (dlg.moveObjects.Checked)
					{
						if (LevelData.Objects != null)
							foreach (ObjectEntry item in LevelData.Objects)
								if (item.Y >= selection.Top * LevelData.Level.ChunkHeight)
								{
									item.Y += (ushort)(selection.Height * LevelData.Level.ChunkHeight);
									item.UpdateSprite();
								}
						if (LevelData.Rings != null)
							foreach (RingEntry item in LevelData.Rings)
								if (item.Y >= selection.Top * LevelData.Level.ChunkHeight)
								{
									item.Y += (ushort)(selection.Height * LevelData.Level.ChunkHeight);
									item.UpdateSprite();
								}
						if (LevelData.Bumpers != null)
							foreach (CNZBumperEntry item in LevelData.Bumpers)
								if (item.Y >= selection.Top * LevelData.Level.ChunkHeight)
								{
									item.Y += (ushort)(selection.Height * LevelData.Level.ChunkHeight);
									item.UpdateSprite();
								}
						if (LevelData.StartPositions != null)
							foreach (StartPositionEntry item in LevelData.StartPositions)
								if (item.Y >= selection.Top * LevelData.Level.ChunkHeight)
								{
									item.Y += (ushort)(selection.Height * LevelData.Level.ChunkHeight);
									item.UpdateSprite();
								}
					}
				}
				else if (dlg.entireColumn.Checked)
				{
					byte[,] layout;
					bool[,] loop;
					if (CurrentTab == Tab.Background)
					{
						if (LevelData.LayoutFormat.IsResizable && maxsize.Width > LevelData.BGWidth)
							LevelData.ResizeBG(Math.Min(maxsize.Width, LevelData.BGWidth + selection.Width), LevelData.BGHeight);
						layout = LevelData.Layout.BGLayout;
						loop = LevelData.Layout.BGLoop;
					}
					else
					{
						if (LevelData.LayoutFormat.IsResizable && maxsize.Width > LevelData.FGWidth)
							LevelData.ResizeFG(Math.Min(maxsize.Width, LevelData.FGWidth + selection.Width), LevelData.FGHeight);
						layout = LevelData.Layout.FGLayout;
						loop = LevelData.Layout.FGLoop;
					}
					for (int y = 0; y < layout.GetLength(1); y++)
						for (int x = layout.GetLength(0) - selection.Width - 1; x >= selection.Left; x--)
						{
							layout[x + selection.Width, y] = layout[x, y];
							if (loop != null)
								loop[x + selection.Width, y] = loop[x, y];
						}
					for (int y = 0; y < layout.GetLength(1); y++)
						for (int x = selection.Left; x < selection.Right; x++)
						{
							layout[x, y] = 0;
							if (loop != null)
								loop[x, y] = false;
						}
					if (dlg.moveObjects.Checked)
					{
						if (LevelData.Objects != null)
							foreach (ObjectEntry item in LevelData.Objects)
								if (item.X >= selection.Left * LevelData.Level.ChunkWidth)
								{
									item.X += (ushort)(selection.Width * LevelData.Level.ChunkWidth);
									item.UpdateSprite();
								}
						if (LevelData.Rings != null)
							foreach (RingEntry item in LevelData.Rings)
								if (item.X >= selection.Left * LevelData.Level.ChunkWidth)
								{
									item.X += (ushort)(selection.Width * LevelData.Level.ChunkWidth);
									item.UpdateSprite();
								}
						if (LevelData.Bumpers != null)
							foreach (CNZBumperEntry item in LevelData.Bumpers)
								if (item.X >= selection.Left * LevelData.Level.ChunkWidth)
								{
									item.X += (ushort)(selection.Width * LevelData.Level.ChunkWidth);
									item.UpdateSprite();
								}
						if (LevelData.StartPositions != null)
							foreach (StartPositionEntry item in LevelData.StartPositions)
								if (item.X >= selection.Left * LevelData.Level.ChunkWidth)
								{
									item.X += (ushort)(selection.Width * LevelData.Level.ChunkWidth);
									item.UpdateSprite();
								}
					}
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
				dlg.moveObjects.Visible = dlg.moveObjects.Checked = CurrentTab == Tab.Foreground;
				if (dlg.ShowDialog(this) != DialogResult.OK) return;
				Rectangle selection;
				if (CurrentTab == Tab.Background)
					selection = BGSelection;
				else
					selection = FGSelection;
				if (dlg.shiftH.Checked)
				{
					byte[,] layout;
					bool[,] loop;
					if (CurrentTab == Tab.Background)
					{
						layout = LevelData.Layout.BGLayout;
						loop = LevelData.Layout.BGLoop;
					}
					else
					{
						layout = LevelData.Layout.FGLayout;
						loop = LevelData.Layout.FGLoop;
					}
					for (int y = selection.Top; y < selection.Bottom; y++)
						for (int x = selection.Left; x < layout.GetLength(0) - selection.Width; x++)
						{
							layout[x, y] = layout[x + selection.Width, y];
							if (loop != null)
								loop[x, y] = loop[x + selection.Width, y];
						}
					for (int y = selection.Top; y < selection.Bottom; y++)
						for (int x = layout.GetLength(0) - selection.Width; x < layout.GetLength(0); x++)
						{
							layout[x, y] = 0;
							if (loop != null)
								loop[x, y] = false;
						}
					if (dlg.moveObjects.Checked)
					{
						if (LevelData.Objects != null)
							foreach (ObjectEntry item in LevelData.Objects)
								if (item.Y >= selection.Top * LevelData.Level.ChunkHeight & item.Y < selection.Bottom * LevelData.Level.ChunkHeight & item.X >= selection.Right * LevelData.Level.ChunkWidth)
								{
									item.X -= (ushort)(selection.Width * LevelData.Level.ChunkWidth);
									item.UpdateSprite();
								}
						if (LevelData.Rings != null)
							foreach (RingEntry item in LevelData.Rings)
								if (item.Y >= selection.Top * LevelData.Level.ChunkHeight & item.Y < selection.Bottom * LevelData.Level.ChunkHeight & item.X >= selection.Right * LevelData.Level.ChunkWidth)
								{
									item.X -= (ushort)(selection.Width * LevelData.Level.ChunkWidth);
									item.UpdateSprite();
								}
						if (LevelData.Bumpers != null)
							foreach (CNZBumperEntry item in LevelData.Bumpers)
								if (item.Y >= selection.Top * LevelData.Level.ChunkHeight & item.Y < selection.Bottom * LevelData.Level.ChunkHeight & item.X >= selection.Right * LevelData.Level.ChunkWidth)
								{
									item.X -= (ushort)(selection.Width * LevelData.Level.ChunkWidth);
									item.UpdateSprite();
								}
						if (LevelData.StartPositions != null)
							foreach (StartPositionEntry item in LevelData.StartPositions)
								if (item.Y >= selection.Top * LevelData.Level.ChunkHeight & item.Y < selection.Bottom * LevelData.Level.ChunkHeight & item.X >= selection.Right * LevelData.Level.ChunkWidth)
								{
									item.X -= (ushort)(selection.Width * LevelData.Level.ChunkWidth);
									item.UpdateSprite();
								}
					}
				}
				else if (dlg.shiftV.Checked)
				{
					byte[,] layout;
					bool[,] loop;
					if (CurrentTab == Tab.Background)
					{
						layout = LevelData.Layout.BGLayout;
						loop = LevelData.Layout.BGLoop;
					}
					else
					{
						layout = LevelData.Layout.FGLayout;
						loop = LevelData.Layout.FGLoop;
					}
					for (int x = selection.Left; x < selection.Right; x++)
						for (int y = selection.Top; y < layout.GetLength(1) - selection.Height; y++)
						{
							layout[x, y] = layout[x, y + selection.Height];
							if (loop != null)
								loop[x, y] = loop[x, y + selection.Height];
						}
					for (int x = selection.Left; x < selection.Right; x++)
						for (int y = layout.GetLength(1) - selection.Height; y < layout.GetLength(1); y++)
						{
							layout[x, y] = 0;
							if (loop != null)
								loop[x, y] = false;
						}
					if (dlg.moveObjects.Checked)
					{
						if (LevelData.Objects != null)
							foreach (ObjectEntry item in LevelData.Objects)
								if (item.X >= selection.Left * LevelData.Level.ChunkWidth & item.X < selection.Right * LevelData.Level.ChunkWidth & item.Y >= selection.Bottom * LevelData.Level.ChunkHeight)
								{
									item.Y -= (ushort)(selection.Height * LevelData.Level.ChunkHeight);
									item.UpdateSprite();
								}
						if (LevelData.Rings != null)
							foreach (RingEntry item in LevelData.Rings)
								if (item.X >= selection.Left * LevelData.Level.ChunkWidth & item.X < selection.Right * LevelData.Level.ChunkWidth & item.Y >= selection.Bottom * LevelData.Level.ChunkHeight)
								{
									item.Y -= (ushort)(selection.Height * LevelData.Level.ChunkHeight);
									item.UpdateSprite();
								}
						if (LevelData.Bumpers != null)
							foreach (CNZBumperEntry item in LevelData.Bumpers)
								if (item.X >= selection.Left * LevelData.Level.ChunkWidth & item.X < selection.Right * LevelData.Level.ChunkWidth & item.Y >= selection.Bottom * LevelData.Level.ChunkHeight)
								{
									item.Y -= (ushort)(selection.Height * LevelData.Level.ChunkHeight);
									item.UpdateSprite();
								}
						if (LevelData.StartPositions != null)
							foreach (StartPositionEntry item in LevelData.StartPositions)
								if (item.X >= selection.Left * LevelData.Level.ChunkWidth & item.X < selection.Right * LevelData.Level.ChunkWidth & item.Y >= selection.Bottom * LevelData.Level.ChunkHeight)
								{
									item.Y -= (ushort)(selection.Height * LevelData.Level.ChunkHeight);
									item.UpdateSprite();
								}
					}
				}
				else if (dlg.entireRow.Checked)
				{
					byte[,] layout;
					bool[,] loop;
					if (CurrentTab == Tab.Background)
					{
						layout = LevelData.Layout.BGLayout;
						loop = LevelData.Layout.BGLoop;
					}
					else
					{
						layout = LevelData.Layout.FGLayout;
						loop = LevelData.Layout.FGLoop;
					}
					for (int x = 0; x < layout.GetLength(0); x++)
						for (int y = selection.Top; y < layout.GetLength(1) - selection.Height; y++)
						{
							layout[x, y] = layout[x, y + selection.Height];
							if (loop != null)
								loop[x, y] = loop[x, y + selection.Height];
						}
					for (int x = 0; x < layout.GetLength(0); x++)
						for (int y = layout.GetLength(1) - selection.Height; y < layout.GetLength(1); y++)
						{
							layout[x, y] = 0;
							if (loop != null)
								loop[x, y] = false;
						}
					if (dlg.moveObjects.Checked)
					{
						if (LevelData.Objects != null)
							foreach (ObjectEntry item in LevelData.Objects)
								if (item.Y >= selection.Bottom * LevelData.Level.ChunkHeight)
								{
									item.Y -= (ushort)(selection.Height * LevelData.Level.ChunkHeight);
									item.UpdateSprite();
								}
						if (LevelData.Rings != null)
							foreach (RingEntry item in LevelData.Rings)
								if (item.Y >= selection.Bottom * LevelData.Level.ChunkHeight)
								{
									item.Y -= (ushort)(selection.Height * LevelData.Level.ChunkHeight);
									item.UpdateSprite();
								}
						if (LevelData.Bumpers != null)
							foreach (CNZBumperEntry item in LevelData.Bumpers)
								if (item.Y >= selection.Bottom * LevelData.Level.ChunkHeight)
								{
									item.Y -= (ushort)(selection.Height * LevelData.Level.ChunkHeight);
									item.UpdateSprite();
								}
						if (LevelData.StartPositions != null)
							foreach (StartPositionEntry item in LevelData.StartPositions)
								if (item.Y >= selection.Bottom * LevelData.Level.ChunkHeight)
								{
									item.Y -= (ushort)(selection.Height * LevelData.Level.ChunkHeight);
									item.UpdateSprite();
								}
					}
					if (CurrentTab == Tab.Background)
					{
						if (LevelData.LayoutFormat.IsResizable && LevelData.BGHeight > selection.Height)
							LevelData.ResizeBG(LevelData.BGHeight - selection.Height, LevelData.BGWidth);
					}
					else
					{
						if (LevelData.LayoutFormat.IsResizable && LevelData.FGHeight > selection.Height)
							LevelData.ResizeFG(LevelData.FGHeight - selection.Height, LevelData.FGWidth);
					}
				}
				else if (dlg.entireColumn.Checked)
				{
					byte[,] layout;
					bool[,] loop;
					if (CurrentTab == Tab.Background)
					{
						layout = LevelData.Layout.BGLayout;
						loop = LevelData.Layout.BGLoop;
					}
					else
					{
						layout = LevelData.Layout.FGLayout;
						loop = LevelData.Layout.FGLoop;
					}
					for (int y = 0; y < layout.GetLength(1); y++)
						for (int x = selection.Left; x < layout.GetLength(0) - selection.Width; x++)
						{
							layout[x, y] = layout[x + selection.Width, y];
							if (loop != null)
								loop[x, y] = loop[x + selection.Width, y];
						}
					for (int y = 0; y < layout.GetLength(1); y++)
						for (int x = layout.GetLength(0) - selection.Width; x < layout.GetLength(0); x++)
						{
							layout[x, y] = 0;
							if (loop != null)
								loop[x, y] = false;
						}
					if (dlg.moveObjects.Checked)
					{
						if (LevelData.Objects != null)
							foreach (ObjectEntry item in LevelData.Objects)
								if (item.X >= selection.Right * LevelData.Level.ChunkWidth)
								{
									item.X -= (ushort)(selection.Width * LevelData.Level.ChunkWidth);
									item.UpdateSprite();
								}
						if (LevelData.Rings != null)
							foreach (RingEntry item in LevelData.Rings)
								if (item.X >= selection.Right * LevelData.Level.ChunkWidth)
								{
									item.X -= (ushort)(selection.Width * LevelData.Level.ChunkWidth);
									item.UpdateSprite();
								}
						if (LevelData.Bumpers != null)
							foreach (CNZBumperEntry item in LevelData.Bumpers)
								if (item.X >= selection.Right * LevelData.Level.ChunkWidth)
								{
									item.X -= (ushort)(selection.Width * LevelData.Level.ChunkWidth);
									item.UpdateSprite();
								}
						if (LevelData.StartPositions != null)
							foreach (StartPositionEntry item in LevelData.StartPositions)
								if (item.X >= selection.Right * LevelData.Level.ChunkWidth)
								{
									item.X -= (ushort)(selection.Width * LevelData.Level.ChunkWidth);
									item.UpdateSprite();
								}
					}
					if (CurrentTab == Tab.Background)
					{
						if (LevelData.LayoutFormat.IsResizable && LevelData.BGWidth > selection.Width)
							LevelData.ResizeBG(LevelData.BGWidth - selection.Width, LevelData.BGHeight);
					}
					else
					{
						if (LevelData.LayoutFormat.IsResizable && LevelData.FGWidth > selection.Width)
							LevelData.ResizeFG(LevelData.FGWidth - selection.Width, LevelData.FGHeight);
					}
				}
			}
		}

		private Rectangle GetBounds(Entry item)
		{
			if (item is ObjectEntry)
				return LevelData.GetObjectDefinition(((ObjectEntry)item).ID).GetBounds((ObjectEntry)item, Point.Empty);
			else if (item is RingEntry)
				return ((RingLayoutFormat)LevelData.RingFormat).GetBounds((RingEntry)item, Point.Empty);
			else if (item is StartPositionEntry)
				return LevelData.StartPosDefs[LevelData.StartPositions.IndexOf((StartPositionEntry)item)].GetBounds((StartPositionEntry)item, Point.Empty);
			else
				return LevelData.unkobj.GetBounds(new S2ObjectEntry() { X = item.X, Y = item.Y }, Point.Empty);
		}

		private bool alignWall_common(int x, int y, Solidity solidity)
		{
			int cnkx = x / LevelData.Level.ChunkWidth;
			int cnky = y / LevelData.Level.ChunkHeight;
			int blkx = (x % LevelData.Level.ChunkWidth) / 16;
			int blky = (y % LevelData.Level.ChunkHeight) / 16;
			int colx = x % 16;
			int coly = y % 16;
			
			ChunkBlock blk = LevelData.Chunks[LevelData.Layout.FGLayout[cnkx, cnky]].Blocks[blkx, blky];
			Solidity solid;
			int colind;
			if (path2ToolStripMenuItem.Checked)
			{
				solid = ((S2ChunkBlock)blk).Solid2;
				colind = LevelData.GetColInd2(blk.Block);
			}
			else
			{
				solid = blk.Solid1;
				colind = LevelData.GetColInd1(blk.Block);
			}
			if ((solid & solidity) == solidity)
			{
				sbyte height = LevelData.ColArr1[colind][colx];
				if (height < 0)
				{
					if (coly < -height)
						return true;
				}
				else if (15 - coly < height)
					return true;
			}
			return false;
		}

		private void alignLeftWallToolStripButton_Click(object sender, EventArgs e)
		{
			foreach (Entry item in SelectedItems)
			{
				Rectangle bounds = GetBounds(item);
				int x = bounds.Left - 1;
				int y = bounds.Top + (bounds.Height / 2);
				while (x > 0)
				{
					if (alignWall_common(x, y, Solidity.LRBSolid))
						break;
					x--;
				}
				item.X = (ushort)(x + 1 + (item.X - bounds.Left));
				item.UpdateSprite();
			}
			DrawLevel();
		}

		private void alignGroundToolStripButton_Click(object sender, EventArgs e)
		{
			foreach (Entry item in SelectedItems)
			{
				Rectangle bounds = GetBounds(item);
				int x = bounds.Left + (bounds.Width / 2);
				int y = bounds.Bottom + 1;
				while (y < LevelData.FGHeight * LevelData.Level.ChunkHeight - 1)
				{
					if (alignWall_common(x, y, Solidity.TopSolid))
						break;
					y++;
				}
				item.Y = (ushort)(y + (item.Y - bounds.Bottom));
				item.UpdateSprite();
			}
			DrawLevel();
		}

		private void alignRightWallToolStripButton_Click(object sender, EventArgs e)
		{
			foreach (Entry item in SelectedItems)
			{
				Rectangle bounds = GetBounds(item);
				int x = bounds.Right + 1;
				int y = bounds.Top + (bounds.Height / 2);
				while (x < LevelData.FGWidth * LevelData.Level.ChunkWidth - 1)
				{
					if (alignWall_common(x, y, Solidity.LRBSolid))
						break;
					x++;
				}
				item.X = (ushort)(x + (item.X - bounds.Right));
				item.UpdateSprite();
			}
			DrawLevel();
		}

		private void alignCeilingToolStripButton_Click(object sender, EventArgs e)
		{
			foreach (Entry item in SelectedItems)
			{
				Rectangle bounds = GetBounds(item);
				int x = bounds.Left + (bounds.Width / 2);
				int y = bounds.Top - 1;
				while (y > 0)
				{
					if (alignWall_common(x, y, Solidity.LRBSolid))
						break;
					y--;
				}
				item.Y = (ushort)(y + 1 + (item.Y - bounds.Top));
				item.UpdateSprite();
			}
			DrawLevel();
		}

		private void alignLeftsToolStripButton_Click(object sender, EventArgs e)
		{
			int left = int.MaxValue;
			foreach (Entry item in SelectedItems)
				left = Math.Min(left, GetBounds(item).Left);
			foreach (Entry item in SelectedItems)
			{
				item.X = (ushort)(left + (item.X - GetBounds(item).Left));
				item.UpdateSprite();
			}
			DrawLevel();
		}

		private void alignCentersToolStripButton_Click(object sender, EventArgs e)
		{
			int left = int.MaxValue;
			foreach (Entry item in SelectedItems)
			{
				Rectangle bounds = GetBounds(item);
				left = Math.Min(left, bounds.Left + (bounds.Width / 2));
			}
			foreach (Entry item in SelectedItems)
			{
				Rectangle bounds = GetBounds(item);
				item.X = (ushort)(left + (item.X - (bounds.Left + (bounds.Width / 2))));
				item.UpdateSprite();
			}
			DrawLevel();
		}

		private void alignRightsToolStripButton_Click(object sender, EventArgs e)
		{
			int right = int.MinValue;
			foreach (Entry item in SelectedItems)
				right = Math.Max(right, GetBounds(item).Right);
			foreach (Entry item in SelectedItems)
			{
				item.X = (ushort)(right + (item.X - GetBounds(item).Right));
				item.UpdateSprite();
			}
			DrawLevel();
		}

		private void alignTopsToolStripButton_Click(object sender, EventArgs e)
		{
			int top = int.MaxValue;
			foreach (Entry item in SelectedItems)
				top = Math.Min(top, GetBounds(item).Top);
			foreach (Entry item in SelectedItems)
			{
				item.Y = (ushort)(top + (item.Y - GetBounds(item).Top));
				item.UpdateSprite();
			}
			DrawLevel();
		}

		private void alignMiddlesToolStripButton_Click(object sender, EventArgs e)
		{
			int top = int.MaxValue;
			foreach (Entry item in SelectedItems)
			{
				Rectangle bounds = GetBounds(item);
				top = Math.Min(top, bounds.Top + (bounds.Height / 2));
			}
			foreach (Entry item in SelectedItems)
			{
				Rectangle bounds = GetBounds(item);
				item.Y = (ushort)(top + (item.Y - (bounds.Top + (bounds.Height / 2))));
				item.UpdateSprite();
			}
			DrawLevel();
		}

		private void alignBottomsToolStripButton_Click(object sender, EventArgs e)
		{
			int bottom = int.MinValue;
			foreach (Entry item in SelectedItems)
				bottom = Math.Max(bottom, GetBounds(item).Bottom);
			foreach (Entry item in SelectedItems)
			{
				item.Y = (ushort)(bottom + (item.Y - GetBounds(item).Bottom));
				item.UpdateSprite();
			}
			DrawLevel();
		}

		List<ObjectEntry> foundobjs;
		int lastfoundobj;
		Point? lastfoundfgchunk;
		byte searchfgchunk;
		Point? lastfoundbgchunk;
		byte searchbgchunk;
		private void findToolStripMenuItem_Click(object sender, EventArgs e)
		{
			switch (CurrentTab)
			{
				case Tab.Objects:
					DialogResult res = findObjectsDialog.ShowDialog(this);
					if (res != System.Windows.Forms.DialogResult.Yes && res != System.Windows.Forms.DialogResult.OK)
						return;
					foundobjs = new List<ObjectEntry>(LevelData.Objects);
					byte? id = findObjectsDialog.ID;
					if (id.HasValue)
						foundobjs = new List<ObjectEntry>(foundobjs.Where(a => a.ID == id.Value));
					byte? sub = findObjectsDialog.SubType;
					if (sub.HasValue)
						foundobjs = new List<ObjectEntry>(foundobjs.Where(a => a.SubType == sub.Value));
					bool? xf = findObjectsDialog.XFlip;
					if (xf.HasValue)
						foundobjs = new List<ObjectEntry>(foundobjs.Where(a => a.XFlip == xf.Value));
					bool? yf = findObjectsDialog.YFlip;
					if (yf.HasValue)
						foundobjs = new List<ObjectEntry>(foundobjs.Where(a => a.YFlip == yf.Value));
					SelectedItems.Clear();
					switch (res)
					{
						case DialogResult.Yes:
							SelectedItems.AddRange(foundobjs.OfType<Entry>());
							if (SelectedItems.Count > 0)
								MessageBox.Show(this, SelectedItems.Count + " object" + (SelectedItems.Count > 1 ? "s" : "") + " found.",
									"SonLVL");
							break;
						case DialogResult.OK:
							if (foundobjs.Count > 0)
								SelectedItems.Add(foundobjs[0]);
							break;
					}
					if (SelectedItems.Count > 0)
					{
						ScrollToObject(SelectedItems[0]);
						lastfoundobj = 0;
						findNextToolStripMenuItem.Enabled = foundobjs.Count > 1;
						findPreviousToolStripMenuItem.Enabled = false;
					}
					else
					{
						MessageBox.Show(this, "No matching objects found.", "SonLVL");
						findNextToolStripMenuItem.Enabled = findPreviousToolStripMenuItem.Enabled = false;
						foundobjs = null;
					}
					SelectedObjectChanged();
					DrawLevel();
					break;
				case Tab.Foreground:
					switch (findFGChunksDialog.ShowDialog(this))
					{
						case DialogResult.Yes:
							int count = 0;
							for (int x = 0; x < LevelData.FGWidth; x++)
								for (int y = 0; y < LevelData.FGHeight; y++)
									if (LevelData.Layout.FGLayout[x, y] == findFGChunksDialog.chunkSelect.Value)
										count++;
							MessageBox.Show(this, count + " chunk" + (count != 1 ? "s" : "") + " found.",
								"SonLVL");
							break;
						case DialogResult.OK:
							for (int x = 0; x < LevelData.FGWidth; x++)
								for (int y = 0; y < LevelData.FGHeight; y++)
									if (LevelData.Layout.FGLayout[x, y] == findFGChunksDialog.chunkSelect.Value)
									{
										lastfoundfgchunk = new Point(x, y);
										searchfgchunk = (byte)findFGChunksDialog.chunkSelect.Value;
										findNextToolStripMenuItem.Enabled = true;
										findPreviousToolStripMenuItem.Enabled = false;
										FGSelection = new Rectangle(x, y, 1, 1);
										loaded = false;
										hScrollBar2.Value = Math.Max(0, Math.Min(hScrollBar2.Maximum, (x * LevelData.Level.ChunkWidth)
											+ (LevelData.Level.ChunkWidth / 2) - (foregroundPanel.Width / 2)));
										vScrollBar2.Value = Math.Max(0, Math.Min(vScrollBar2.Maximum, (y * LevelData.Level.ChunkHeight)
											+ (LevelData.Level.ChunkHeight / 2) - (foregroundPanel.Height / 2)));
										loaded = true;
										DrawLevel();
										return;
									}
							MessageBox.Show(this, "No matching chunks found.", "SonLVL");
							lastfoundfgchunk = null;
							findNextToolStripMenuItem.Enabled = findPreviousToolStripMenuItem.Enabled = false;
							break;
					}
					break;
				case Tab.Background:
					switch (findBGChunksDialog.ShowDialog(this))
					{
						case DialogResult.Yes:
							int count = 0;
							for (int x = 0; x < LevelData.BGWidth; x++)
								for (int y = 0; y < LevelData.BGHeight; y++)
									if (LevelData.Layout.BGLayout[x, y] == findBGChunksDialog.chunkSelect.Value)
										count++;
							MessageBox.Show(this, count + " chunk" + (count != 1 ? "s" : "") + " found.",
								"SonLVL");
							break;
						case DialogResult.OK:
							for (int x = 0; x < LevelData.BGWidth; x++)
								for (int y = 0; y < LevelData.BGHeight; y++)
									if (LevelData.Layout.BGLayout[x, y] == findBGChunksDialog.chunkSelect.Value)
									{
										lastfoundbgchunk = new Point(x, y);
										searchbgchunk = (byte)findBGChunksDialog.chunkSelect.Value;
										findNextToolStripMenuItem.Enabled = true;
										findPreviousToolStripMenuItem.Enabled = false;
										BGSelection = new Rectangle(x, y, 1, 1);
										loaded = false;
										hScrollBar3.Value = Math.Max(0, Math.Min(hScrollBar3.Maximum, (x * LevelData.Level.ChunkWidth)
											+ (LevelData.Level.ChunkWidth / 2) - (backgroundPanel.Width / 2)));
										vScrollBar3.Value = Math.Max(0, Math.Min(vScrollBar3.Maximum, (y * LevelData.Level.ChunkHeight)
											+ (LevelData.Level.ChunkHeight / 2) - (backgroundPanel.Height / 2)));
										loaded = true;
										DrawLevel();
										return;
									}
							MessageBox.Show(this, "No matching chunks found.", "SonLVL");
							lastfoundbgchunk = null;
							findNextToolStripMenuItem.Enabled = findPreviousToolStripMenuItem.Enabled = false;
							break;
					}
					break;
			}
		}

		private void ScrollToObject(Entry item)
		{
			loaded = false;
			hScrollBar1.Value = Math.Max(0, Math.Min(hScrollBar1.Maximum, item.X - (objectPanel.Width / 2)));
			vScrollBar1.Value = Math.Max(0, Math.Min(vScrollBar1.Maximum, item.Y - (objectPanel.Height / 2)));
			loaded = true;
		}

		private void findNextToolStripMenuItem_Click(object sender, EventArgs e)
		{
			switch (CurrentTab)
			{
				case Tab.Objects:
					if (lastfoundobj < foundobjs.Count - 1)
					{
						SelectedItems.Clear();
						SelectedItems.Add(foundobjs[++lastfoundobj]);
						ScrollToObject(SelectedItems[0]);
						findPreviousToolStripMenuItem.Enabled = true;
						SelectedObjectChanged();
						DrawLevel();
					}
					else
					{
						MessageBox.Show(this, "No more objects found.", "SonLVL");
						findNextToolStripMenuItem.Enabled = false;
					}
					break;
				case Tab.Foreground:
					for (int x = 0; x < LevelData.FGWidth; x++)
						for (int y = 0; y < LevelData.FGHeight; y++)
						{
							if (x == 0 && y == 0)
							{
								x = lastfoundfgchunk.Value.X;
								y = lastfoundfgchunk.Value.Y;
							}
							else if (LevelData.Layout.FGLayout[x, y] == searchfgchunk)
							{
								lastfoundfgchunk = new Point(x, y);
								findPreviousToolStripMenuItem.Enabled = true;
								FGSelection = new Rectangle(x, y, 1, 1);
								loaded = false;
								hScrollBar2.Value = Math.Max(0, Math.Min(hScrollBar2.Maximum, (x * LevelData.Level.ChunkWidth)
									+ (LevelData.Level.ChunkWidth / 2) - (foregroundPanel.Width / 2)));
								vScrollBar2.Value = Math.Max(0, Math.Min(vScrollBar2.Maximum, (y * LevelData.Level.ChunkHeight)
									+ (LevelData.Level.ChunkHeight / 2) - (foregroundPanel.Height / 2)));
								loaded = true;
								DrawLevel();
								return;
							}
						}
					MessageBox.Show(this, "No more chunks found.", "SonLVL");
					findNextToolStripMenuItem.Enabled = false;
					break;
				case Tab.Background:
					for (int x = 0; x < LevelData.BGWidth; x++)
						for (int y = 0; y < LevelData.BGHeight; y++)
						{
							if (x == 0 && y == 0)
							{
								x = lastfoundbgchunk.Value.X;
								y = lastfoundbgchunk.Value.Y;
							}
							else if (LevelData.Layout.BGLayout[x, y] == searchbgchunk)
							{
								lastfoundbgchunk = new Point(x, y);
								findPreviousToolStripMenuItem.Enabled = true;
								BGSelection = new Rectangle(x, y, 1, 1);
								loaded = false;
								hScrollBar3.Value = Math.Max(0, Math.Min(hScrollBar3.Maximum, (x * LevelData.Level.ChunkWidth)
									+ (LevelData.Level.ChunkWidth / 2) - (backgroundPanel.Width / 2)));
								vScrollBar3.Value = Math.Max(0, Math.Min(vScrollBar3.Maximum, (y * LevelData.Level.ChunkHeight)
									+ (LevelData.Level.ChunkHeight / 2) - (backgroundPanel.Height / 2)));
								loaded = true;
								DrawLevel();
								return;
							}
						}
					MessageBox.Show(this, "No more chunks found.", "SonLVL");
					findNextToolStripMenuItem.Enabled = false;
					break;
			}
		}

		private void findPreviousToolStripMenuItem_Click(object sender, EventArgs e)
		{
			switch (CurrentTab)
			{
				case Tab.Objects:
					if (lastfoundobj > 0)
					{
						SelectedItems.Clear();
						SelectedItems.Add(foundobjs[--lastfoundobj]);
						ScrollToObject(SelectedItems[0]);
						findNextToolStripMenuItem.Enabled = true;
						SelectedObjectChanged();
						DrawLevel();
					}
					else
					{
						MessageBox.Show(this, "No more objects found.", "SonLVL");
						findPreviousToolStripMenuItem.Enabled = false;
					}
					break;
				case Tab.Foreground:
					for (int x = (LevelData.FGWidth - 1); x >= 0; x--)
						for (int y = (LevelData.FGHeight - 1); y >= 0; y--)
						{
							if (x == (LevelData.FGWidth - 1) && y == (LevelData.FGHeight - 1))
							{
								x = lastfoundfgchunk.Value.X;
								y = lastfoundfgchunk.Value.Y;
							}
							else if (LevelData.Layout.FGLayout[x, y] == searchfgchunk)
							{
								lastfoundfgchunk = new Point(x, y);
								findNextToolStripMenuItem.Enabled = true;
								FGSelection = new Rectangle(x, y, 1, 1);
								loaded = false;
								hScrollBar2.Value = Math.Max(0, Math.Min(hScrollBar2.Maximum, (x * LevelData.Level.ChunkWidth)
									+ (LevelData.Level.ChunkWidth / 2) - (foregroundPanel.Width / 2)));
								vScrollBar2.Value = Math.Max(0, Math.Min(vScrollBar2.Maximum, (y * LevelData.Level.ChunkHeight)
									+ (LevelData.Level.ChunkHeight / 2) - (foregroundPanel.Height / 2)));
								loaded = true;
								DrawLevel();
								return;
							}
						}
					MessageBox.Show(this, "No more chunks found.", "SonLVL");
					findPreviousToolStripMenuItem.Enabled = false;
					break;
				case Tab.Background:
					for (int x = (LevelData.BGWidth - 1); x >= 0; x--)
						for (int y = (LevelData.BGHeight - 1); y >= 0; y--)
						{
							if (x == (LevelData.BGWidth - 1) && y == (LevelData.BGHeight - 1))
							{
								x = lastfoundbgchunk.Value.X;
								y = lastfoundbgchunk.Value.Y;
							}
							else if (LevelData.Layout.BGLayout[x, y] == searchbgchunk)
							{
								lastfoundbgchunk = new Point(x, y);
								findNextToolStripMenuItem.Enabled = true;
								BGSelection = new Rectangle(x, y, 1, 1);
								loaded = false;
								hScrollBar3.Value = Math.Max(0, Math.Min(hScrollBar3.Maximum, (x * LevelData.Level.ChunkWidth)
									+ (LevelData.Level.ChunkWidth / 2) - (backgroundPanel.Width / 2)));
								vScrollBar3.Value = Math.Max(0, Math.Min(vScrollBar3.Maximum, (y * LevelData.Level.ChunkHeight)
									+ (LevelData.Level.ChunkHeight / 2) - (backgroundPanel.Height / 2)));
								loaded = true;
								DrawLevel();
								return;
							}
						}
					MessageBox.Show(this, "No more chunks found.", "SonLVL");
					findPreviousToolStripMenuItem.Enabled = false;
					break;
			}
		}

		private void objectsAboveHighPlaneToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			Settings.ObjectsAboveHighPlane = objectsAboveHighPlaneToolStripMenuItem.Checked;
		}

		private void lowToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			Settings.ViewLowPlane = lowToolStripMenuItem.Checked;
		}

		private void highToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			Settings.ViewHighPlane = highToolStripMenuItem.Checked;
		}

		private void loadingAnimation1_SizeChanged(object sender, EventArgs e)
		{
			loadingAnimation1.Location = new Point((ClientSize.Width / 2) - (loadingAnimation1.Width / 2), (ClientSize.Height / 2) - loadingAnimation1.Height / 2);
		}

		private void objGridSizeDropDownButton_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			foreach (ToolStripMenuItem item in objGridSizeDropDownButton.DropDownItems)
				item.Checked = false;
			((ToolStripMenuItem)e.ClickedItem).Checked = true;
			objGridSizeDropDownButton.Text = "Grid Size: " + (1 << (ObjGrid = (byte)objGridSizeDropDownButton.DropDownItems.IndexOf(e.ClickedItem)));
			if (!loaded) return;
			DrawLevel();
		}

		private void ChunkSelector_ItemDrag(object sender, EventArgs e)
		{
			if (CurrentTab == Tab.Art && enableDraggingChunksButton.Checked)
				DoDragDrop(new DataObject("SonLVLChunkIndex_" + pid, ChunkSelector.SelectedIndex), DragDropEffects.Move);
		}

		bool chunk_dragdrop;
		int chunk_dragobj;
		Point chunk_dragpoint;
		private void ChunkSelector_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent("SonLVLChunkIndex_" + pid))
			{
				e.Effect = DragDropEffects.Move;
				chunk_dragdrop = true;
				chunk_dragobj = (int)e.Data.GetData("SonLVLChunkIndex_" + pid);
				chunk_dragpoint = ChunkSelector.PointToClient(new Point(e.X, e.Y));
				ChunkSelector.Invalidate();
			}
			else
				chunk_dragdrop = false;
		}

		private void ChunkSelector_DragOver(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent("SonLVLChunkIndex_" + pid))
			{
				e.Effect = DragDropEffects.Move;
				chunk_dragdrop = true;
				chunk_dragobj = (int)e.Data.GetData("SonLVLChunkIndex_" + pid);
				chunk_dragpoint = ChunkSelector.PointToClient(new Point(e.X, e.Y));
				if (chunk_dragpoint.Y < 8)
					ChunkSelector.ScrollValue -= 8 - dragpoint.Y;
				else if (dragpoint.Y > ChunkSelector.Height - 8)
					ChunkSelector.ScrollValue += dragpoint.Y - (ChunkSelector.Height - 8);
				ChunkSelector.Invalidate();
			}
			else
				chunk_dragdrop = false;
		}

		private void ChunkSelector_DragLeave(object sender, EventArgs e)
		{
			chunk_dragdrop = false;
			ChunkSelector.Invalidate();
		}

		private void ChunkSelector_Paint(object sender, PaintEventArgs e)
		{
			if (chunk_dragdrop)
			{
				e.Graphics.DrawImage(ChunkSelector.Images[chunk_dragobj], chunk_dragpoint.X - (ChunkSelector.ImageWidth / 2),
					chunk_dragpoint.Y - (ChunkSelector.ImageHeight / 2), ChunkSelector.ImageWidth, ChunkSelector.ImageHeight);
				Rectangle r = ChunkSelector.GetItemBounds(ChunkSelector.GetItemAtPoint(chunk_dragpoint));
				if ((ModifierKeys & Keys.Control) == Keys.Control)
					e.Graphics.DrawRectangle(new Pen(Color.Black, 2), r);
				else
					e.Graphics.DrawLine(new Pen(Color.Black, 2), r.Left + 1, r.Top, r.Left + 1, r.Bottom);
			}
		}

		private void ChunkSelector_DragDrop(object sender, DragEventArgs e)
		{
			chunk_dragdrop = false;
			if (e.Data.GetDataPresent("SonLVLChunkIndex_" + pid))
			{
				Point clientPoint = ChunkSelector.PointToClient(new Point(e.X, e.Y));
				byte newindex = (byte)ChunkSelector.GetItemAtPoint(clientPoint);
				byte oldindex = (byte)(int)e.Data.GetData("SonLVLChunkIndex_" + pid);
				if (newindex == oldindex) return;
				if ((ModifierKeys & Keys.Control) == Keys.Control)
				{
					if (newindex == LevelData.Chunks.Count) return;
					LevelData.Chunks.Swap(oldindex, newindex);
					LevelData.ChunkBmpBits.Swap(oldindex, newindex);
					LevelData.ChunkBmps.Swap(oldindex, newindex);
					LevelData.ChunkColBmpBits.Swap(oldindex, newindex);
					LevelData.ChunkColBmps.Swap(oldindex, newindex);
					LevelData.CompChunkBmpBits.Swap(oldindex, newindex);
					LevelData.CompChunkBmps.Swap(oldindex, newindex);
					LevelData.RemapLayouts((layout, x, y) =>
					{
						if (layout[x, y] == newindex)
							layout[x, y] = oldindex;
						else if (layout[x, y] == oldindex)
							layout[x, y] = newindex;
					});
					ChunkSelector.SelectedIndex = newindex;
				}
				else
				{
					if (newindex == oldindex + 1) return;
					LevelData.Chunks.Move(oldindex, newindex);
					LevelData.ChunkBmpBits.Move(oldindex, newindex);
					LevelData.ChunkBmps.Move(oldindex, newindex);
					LevelData.ChunkColBmpBits.Move(oldindex, newindex);
					LevelData.ChunkColBmps.Move(oldindex, newindex);
					LevelData.CompChunkBmpBits.Move(oldindex, newindex);
					LevelData.CompChunkBmps.Move(oldindex, newindex);
					LevelData.RemapLayouts((layout, x, y) =>
					{
						byte c = layout[x, y];
						if (newindex > oldindex)
						{
							if (c == oldindex)
								layout[x, y] = (byte)(newindex - 1);
							else if (c > oldindex && c < newindex)
								layout[x, y] = (byte)(c - 1);
						}
						else
						{
							if (c == oldindex)
								layout[x, y] = newindex;
							else if (c >= newindex && c < oldindex)
								layout[x, y] = (byte)(c + 1);
						}
					});
					if (newindex > oldindex)
						ChunkSelector.SelectedIndex = newindex - 1;
					else
						ChunkSelector.SelectedIndex = newindex;
				}
			}
		}

		private void BlockSelector_ItemDrag(object sender, EventArgs e)
		{
			if (enableDraggingBlocksButton.Checked)
				DoDragDrop(new DataObject("SonLVLBlockIndex_" + pid, BlockSelector.SelectedIndex), DragDropEffects.Move);
		}

		bool block_dragdrop;
		int block_dragobj;
		Point block_dragpoint;
		private void BlockSelector_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent("SonLVLBlockIndex_" + pid))
			{
				e.Effect = DragDropEffects.Move;
				block_dragdrop = true;
				block_dragobj = (int)e.Data.GetData("SonLVLBlockIndex_" + pid);
				block_dragpoint = BlockSelector.PointToClient(new Point(e.X, e.Y));
				BlockSelector.Invalidate();
			}
			else
				block_dragdrop = false;
		}

		private void BlockSelector_DragOver(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent("SonLVLBlockIndex_" + pid))
			{
				e.Effect = DragDropEffects.Move;
				block_dragdrop = true;
				block_dragobj = (int)e.Data.GetData("SonLVLBlockIndex_" + pid);
				block_dragpoint = BlockSelector.PointToClient(new Point(e.X, e.Y));
				if (block_dragpoint.Y < 8)
					BlockSelector.ScrollValue -= 8 - dragpoint.Y;
				else if (dragpoint.Y > BlockSelector.Height - 8)
					BlockSelector.ScrollValue += dragpoint.Y - (BlockSelector.Height - 8);
				BlockSelector.Invalidate();
			}
			else
				block_dragdrop = false;
		}

		private void BlockSelector_DragLeave(object sender, EventArgs e)
		{
			block_dragdrop = false;
			BlockSelector.Invalidate();
		}

		private void BlockSelector_Paint(object sender, PaintEventArgs e)
		{
			if (block_dragdrop)
			{
				e.Graphics.DrawImage(BlockSelector.Images[block_dragobj], block_dragpoint.X - (BlockSelector.ImageSize / 2),
					block_dragpoint.Y - (BlockSelector.ImageSize / 2), BlockSelector.ImageSize, BlockSelector.ImageSize);
				Rectangle r = BlockSelector.GetItemBounds(BlockSelector.GetItemAtPoint(block_dragpoint));
				if ((ModifierKeys & Keys.Control) == Keys.Control)
					e.Graphics.DrawRectangle(new Pen(Color.Black, 2), r);
				else
					e.Graphics.DrawLine(new Pen(Color.Black, 2), r.Left + 1, r.Top, r.Left + 1, r.Bottom);
			}
		}

		private void BlockSelector_DragDrop(object sender, DragEventArgs e)
		{
			block_dragdrop = false;
			if (e.Data.GetDataPresent("SonLVLBlockIndex_" + pid))
			{
				Point clientPoint = BlockSelector.PointToClient(new Point(e.X, e.Y));
				ushort newindex = (ushort)BlockSelector.GetItemAtPoint(clientPoint);
				ushort oldindex = (ushort)(int)e.Data.GetData("SonLVLBlockIndex_" + pid);
				if (newindex == oldindex) return;
				if ((ModifierKeys & Keys.Control) == Keys.Control)
				{
					if (newindex == LevelData.Blocks.Count) return;
					LevelData.Blocks.Swap(oldindex, newindex);
					LevelData.BlockBmpBits.Swap(oldindex, newindex);
					LevelData.BlockBmps.Swap(oldindex, newindex);
					LevelData.CompBlockBmpBits.Swap(oldindex, newindex);
					LevelData.CompBlockBmps.Swap(oldindex, newindex);
					if (LevelData.ColInds1 != null)
						LevelData.ColInds1.Swap(oldindex, newindex);
					if (LevelData.ColInds2 != null && LevelData.ColInds2 != LevelData.ColInds1)
						LevelData.ColInds2.Swap(oldindex, newindex);
					for (int i = 0; i < LevelData.Chunks.Count; i++)
						for (int y = 0; y < LevelData.Level.ChunkHeight / 16; y++)
							for (int x = 0; x < LevelData.Level.ChunkWidth / 16; x++)
							{
								if (LevelData.Chunks[i].Blocks[x, y].Block == newindex)
									LevelData.Chunks[i].Blocks[x, y].Block = oldindex;
								else if (LevelData.Chunks[i].Blocks[x, y].Block == oldindex)
									LevelData.Chunks[i].Blocks[x, y].Block = newindex;
							}
					BlockSelector.SelectedIndex = newindex;
				}
				else
				{
					if (newindex == oldindex + 1) return;
					LevelData.Blocks.Move(oldindex, newindex);
					LevelData.BlockBmpBits.Move(oldindex, newindex);
					LevelData.BlockBmps.Move(oldindex, newindex);
					LevelData.CompBlockBmpBits.Move(oldindex, newindex);
					LevelData.CompBlockBmps.Move(oldindex, newindex);
					if (LevelData.ColInds1 != null)
						LevelData.ColInds1.Move(oldindex, newindex);
					if (LevelData.ColInds2 != null && LevelData.ColInds2 != LevelData.ColInds1)
						LevelData.ColInds2.Move(oldindex, newindex);
					for (int i = 0; i < LevelData.Chunks.Count; i++)
						for (int y = 0; y < LevelData.Level.ChunkHeight / 16; y++)
							for (int x = 0; x < LevelData.Level.ChunkWidth / 16; x++)
							{
								ushort b = LevelData.Chunks[i].Blocks[x, y].Block;
								if (newindex > oldindex)
								{
									if (b == oldindex)
										LevelData.Chunks[i].Blocks[x, y].Block = (ushort)(newindex - 1);
									else if (b > oldindex && b < newindex)
										LevelData.Chunks[i].Blocks[x, y].Block = (ushort)(b - 1);
								}
								else
								{
									if (b == oldindex)
										LevelData.Chunks[i].Blocks[x, y].Block = newindex;
									else if (b >= newindex && b < oldindex)
										LevelData.Chunks[i].Blocks[x, y].Block = (ushort)(b + 1);
								}
							}
					if (newindex > oldindex)
						BlockSelector.SelectedIndex = newindex - 1;
					else
						BlockSelector.SelectedIndex = newindex;
				}
			}
		}

		private void TileSelector_ItemDrag(object sender, EventArgs e)
		{
			if (enableDraggingTilesButton.Checked)
				DoDragDrop(new DataObject("SonLVLTileIndex_" + pid, TileSelector.SelectedIndex), DragDropEffects.Move);
		}

		bool tile_dragdrop;
		int tile_dragobj;
		Point tile_dragpoint;
		private void TileSelector_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent("SonLVLTileIndex_" + pid))
			{
				e.Effect = DragDropEffects.Move;
				tile_dragdrop = true;
				tile_dragobj = (int)e.Data.GetData("SonLVLTileIndex_" + pid);
				tile_dragpoint = TileSelector.PointToClient(new Point(e.X, e.Y));
				TileSelector.Invalidate();
			}
			else
				tile_dragdrop = false;
		}

		private void TileSelector_DragOver(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent("SonLVLTileIndex_" + pid))
			{
				e.Effect = DragDropEffects.Move;
				tile_dragdrop = true;
				tile_dragobj = (int)e.Data.GetData("SonLVLTileIndex_" + pid);
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
			if (e.Data.GetDataPresent("SonLVLTileIndex_" + pid))
			{
				Point clientPoint = TileSelector.PointToClient(new Point(e.X, e.Y));
				ushort newindex = (ushort)TileSelector.GetItemAtPoint(clientPoint);
				ushort oldindex = (ushort)(int)e.Data.GetData("SonLVLTileIndex_" + pid);
				if (newindex == oldindex) return;
				if ((ModifierKeys & Keys.Control) == Keys.Control)
				{
					if (newindex == TileSelector.Images.Count) return;
					if (LevelData.Level.TwoPlayerCompatible)
					{
						LevelData.Tiles.Swap(oldindex * 2, newindex * 2);
						LevelData.Tiles.Swap(oldindex * 2 + 1, newindex * 2 + 1);
					}
					else
						LevelData.Tiles.Swap(oldindex, newindex);
					TileSelector.Images.Swap(oldindex, newindex);
					LevelData.UpdateTileArray();
					if (LevelData.Level.TwoPlayerCompatible)
						for (int i = 0; i < LevelData.Blocks.Count; i++)
							for (int x = 0; x < 2; x++)
							{
								if (LevelData.Blocks[i].Tiles[x, 0].Tile / 2 == newindex)
								{
									LevelData.Blocks[i].Tiles[x, 0].Tile = (ushort)(oldindex / 2);
									LevelData.Blocks[i].MakeInterlacedCompatible();
								}
								else if (LevelData.Blocks[i].Tiles[x, 0].Tile / 2 == oldindex)
								{
									LevelData.Blocks[i].Tiles[x, 0].Tile = (ushort)(newindex / 2);
									LevelData.Blocks[i].MakeInterlacedCompatible();
								}
							}
					else
						for (int i = 0; i < LevelData.Blocks.Count; i++)
							for (int y = 0; y < 2; y++)
								for (int x = 0; x < 2; x++)
								{
									if (LevelData.Blocks[i].Tiles[x, y].Tile == newindex)
										LevelData.Blocks[i].Tiles[x, y].Tile = oldindex;
									else if (LevelData.Blocks[i].Tiles[x, y].Tile == oldindex)
										LevelData.Blocks[i].Tiles[x, y].Tile = newindex;
								}
					TileSelector.SelectedIndex = newindex;
				}
				else
				{
					if (newindex == oldindex + 1) return;
					if (LevelData.Level.TwoPlayerCompatible)
					{
						byte[] t1 = LevelData.Tiles[oldindex * 2];
						byte[] t2 = LevelData.Tiles[oldindex * 2 + 1];
						LevelData.Tiles.Insert(newindex * 2, t2);
						LevelData.Tiles.Insert(newindex * 2, t1);
						LevelData.Tiles.RemoveAt(oldindex > newindex ? oldindex / 2 + 2 : oldindex);
						LevelData.Tiles.RemoveAt(oldindex > newindex ? oldindex / 2 + 2 : oldindex);
					}
					else
						LevelData.Tiles.Move(oldindex, newindex);
					TileSelector.Images.Move(oldindex, newindex);
					LevelData.UpdateTileArray();
					if (LevelData.Level.TwoPlayerCompatible)
						for (int i = 0; i < LevelData.Blocks.Count; i++)
							for (int x = 0; x < 2; x++)
							{
								int t = LevelData.Blocks[i].Tiles[x, 0].Tile / 2;
								if (newindex > oldindex)
								{
									if (t == oldindex)
									{
										LevelData.Blocks[i].Tiles[x, 0].Tile = (ushort)(newindex * 2 - 2);
										LevelData.Blocks[i].MakeInterlacedCompatible();
									}
									else if (t > oldindex && t < newindex)
									{
										LevelData.Blocks[i].Tiles[x, 0].Tile -= 2;
										LevelData.Blocks[i].Tiles[x, 1].Tile -= 2;
									}
								}
								else
								{
									if (t == oldindex)
									{
										LevelData.Blocks[i].Tiles[x, 0].Tile = (ushort)(newindex * 2);
									}
									else if (t >= newindex && t < oldindex)
									{
										LevelData.Blocks[i].Tiles[x, 0].Tile += 2;
										LevelData.Blocks[i].Tiles[x, 1].Tile += 2;
									}
								}
							}
					else
						for (int i = 0; i < LevelData.Blocks.Count; i++)
							for (int y = 0; y < 2; y++)
								for (int x = 0; x < 2; x++)
								{
									ushort t = LevelData.Blocks[i].Tiles[x, y].Tile;
									if (newindex > oldindex)
									{
										if (t == oldindex)
											LevelData.Blocks[i].Tiles[x, y].Tile = (ushort)(newindex - 1);
										else if (t > oldindex && t < newindex)
											LevelData.Blocks[i].Tiles[x, y].Tile = (ushort)(t - 1);
									}
									else
									{
										if (t == oldindex)
											LevelData.Blocks[i].Tiles[x, y].Tile = newindex;
										else if (t >= newindex && t < oldindex)
											LevelData.Blocks[i].Tiles[x, y].Tile = (ushort)(t + 1);
									}
								}
					if (newindex > oldindex)
						TileSelector.SelectedIndex = newindex - 1;
					else
						TileSelector.SelectedIndex = newindex;
				}
			}
		}

		private void remapChunksButton_Click(object sender, EventArgs e)
		{
			using (TileRemappingDialog dlg = new TileRemappingDialog("Chunks", LevelData.CompChunkBmps, ChunkSelector.ImageWidth, ChunkSelector.ImageHeight))
				if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{
					List<Chunk> oldchunks = LevelData.Chunks.ToList();
					List<BitmapBits[]> oldchunkbmpbits = new List<BitmapBits[]>(LevelData.ChunkBmpBits);
					List<Bitmap[]> oldchunkbmps = new List<Bitmap[]>(LevelData.ChunkBmps);
					List<BitmapBits[]> oldchunkcolbmpbits = new List<BitmapBits[]>(LevelData.ChunkColBmpBits);
					List<Bitmap[]> oldchunkcolbmps = new List<Bitmap[]>(LevelData.ChunkColBmps);
					List<BitmapBits> oldcompchunkbmpbits = new List<BitmapBits>(LevelData.CompChunkBmpBits);
					List<Bitmap> oldcompchunkbmps = new List<Bitmap>(LevelData.CompChunkBmps);
					Dictionary<byte, byte> bytedict = new Dictionary<byte, byte>(dlg.TileMap.Count);
					foreach (KeyValuePair<int, int> item in dlg.TileMap)
					{
						LevelData.Chunks[item.Value] = oldchunks[item.Key];
						LevelData.ChunkBmpBits[item.Value] = oldchunkbmpbits[item.Key];
						LevelData.ChunkBmps[item.Value] = oldchunkbmps[item.Key];
						LevelData.ChunkColBmpBits[item.Value] = oldchunkcolbmpbits[item.Key];
						LevelData.ChunkColBmps[item.Value] = oldchunkcolbmps[item.Key];
						LevelData.CompChunkBmpBits[item.Value] = oldcompchunkbmpbits[item.Key];
						LevelData.CompChunkBmps[item.Value] = oldcompchunkbmps[item.Key];
						bytedict.Add((byte)item.Key, (byte)item.Value);
					}
					LevelData.RemapLayouts((layout, x, y) =>
					{
						if (bytedict.ContainsKey(layout[x, y]))
							layout[x, y] = bytedict[layout[x, y]];
					});
					ChunkSelector.ChangeSize();
					ChunkSelector_SelectedIndexChanged(this, EventArgs.Empty);
				}
		}

		private void remapBlocksButton_Click(object sender, EventArgs e)
		{
			using (TileRemappingDialog dlg = new TileRemappingDialog("Blocks", LevelData.CompBlockBmps, BlockSelector.ImageWidth, BlockSelector.ImageHeight))
				if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{
					List<Block> oldblocks = LevelData.Blocks.ToList();
					List<BitmapBits[]> oldblockbmpbits = new List<BitmapBits[]>(LevelData.BlockBmpBits);
					List<Bitmap[]> oldblockbmps = new List<Bitmap[]>(LevelData.BlockBmps);
					List<BitmapBits> oldcompblockbmpbits = new List<BitmapBits>(LevelData.CompBlockBmpBits);
					List<Bitmap> oldcompblockbmps = new List<Bitmap>(LevelData.CompBlockBmps);
					List<byte> oldcolinds1 = null;
					if (LevelData.ColInds1 != null)
						oldcolinds1 = new List<byte>(LevelData.ColInds1);
					List<byte> oldcolinds2 = null;
					if (LevelData.ColInds2 != null && LevelData.ColInds2 != LevelData.ColInds1)
						oldcolinds2 = new List<byte>(LevelData.ColInds2);
					Dictionary<ushort, ushort> ushortdict = new Dictionary<ushort, ushort>(dlg.TileMap.Count);
					foreach (KeyValuePair<int, int> item in dlg.TileMap)
					{
						LevelData.Blocks[item.Value] = oldblocks[item.Key];
						LevelData.BlockBmpBits[item.Value] = oldblockbmpbits[item.Key];
						LevelData.BlockBmps[item.Value] = oldblockbmps[item.Key];
						LevelData.CompBlockBmpBits[item.Value] = oldcompblockbmpbits[item.Key];
						LevelData.CompBlockBmps[item.Value] = oldcompblockbmps[item.Key];
						if (oldcolinds1 != null)
							LevelData.ColInds1[item.Value] = oldcolinds1[item.Key];
						if (oldcolinds2 != null)
							LevelData.ColInds2[item.Value] = oldcolinds2[item.Key];
						ushortdict.Add((ushort)item.Key, (ushort)item.Value);
					}
					for (int c = 0; c < LevelData.Chunks.Count; c++)
					{
						bool redraw = false;
						for (int y = 0; y < LevelData.Level.ChunkHeight / 16; y++)
							for (int x = 0; x < LevelData.Level.ChunkWidth / 16; x++)
								if (ushortdict.ContainsKey(LevelData.Chunks[c].Blocks[x, y].Block))
								{
									redraw = true;
									LevelData.Chunks[c].Blocks[x, y].Block = ushortdict[LevelData.Chunks[c].Blocks[x, y].Block];
								}
						if (redraw)
							LevelData.RedrawChunk(c);
					}
					BlockSelector.ChangeSize();
					BlockSelector_SelectedIndexChanged(this, EventArgs.Empty);
				}
		}

		private void remapTilesButton_Click(object sender, EventArgs e)
		{
			using (TileRemappingDialog dlg = new TileRemappingDialog("Tiles", TileSelector.Images, TileSelector.ImageWidth, TileSelector.ImageHeight))
				if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{
					List<byte[]> oldtiles = LevelData.Tiles.ToList();
					List<Bitmap> oldimages = new List<Bitmap>(TileSelector.Images);
					Dictionary<ushort, ushort> ushortdict = new Dictionary<ushort, ushort>(LevelData.Level.TwoPlayerCompatible ? dlg.TileMap.Count * 2 : dlg.TileMap.Count);
					foreach (KeyValuePair<int, int> item in dlg.TileMap)
					{
						if (LevelData.Level.TwoPlayerCompatible)
						{
							LevelData.Tiles[item.Value * 2] = oldtiles[item.Key * 2];
							LevelData.Tiles[item.Value * 2 + 1] = oldtiles[item.Key * 2 + 1];
							TileSelector.Images[item.Value] = oldimages[item.Key];
							ushortdict.Add((ushort)(item.Key * 2), (ushort)(item.Value * 2));
							ushortdict.Add((ushort)(item.Key * 2 + 1), (ushort)(item.Value * 2 + 1));
						}
						else
						{
							LevelData.Tiles[item.Value] = oldtiles[item.Key];
							TileSelector.Images[item.Value] = oldimages[item.Key];
							ushortdict.Add((ushort)item.Key, (ushort)item.Value);
						}
					}
					LevelData.UpdateTileArray();
					for (int b = 0; b < LevelData.Blocks.Count; b++)
					{
						bool redraw = false;
						for (int y = 0; y < 2; y++)
							for (int x = 0; x < 2; x++)
								if (ushortdict.ContainsKey(LevelData.Blocks[b].Tiles[x, y].Tile))
								{
									redraw = true;
									LevelData.Blocks[b].Tiles[x, y].Tile = ushortdict[LevelData.Blocks[b].Tiles[x, y].Tile];
								}
						if (redraw)
							LevelData.RedrawBlock(b, true);
					}
					TileSelector.ChangeSize();
					TileSelector_SelectedIndexChanged(this, EventArgs.Empty);
				}
		}

		private void saveSectionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (LayoutSectionNameDialog dlg = new LayoutSectionNameDialog())
			{
				dlg.Value = "Section " + (savedLayoutSections.Count + 1);
				if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{
					LayoutSection sec = CreateLayoutSection();
					sec.Name = dlg.Value;
					savedLayoutSections.Add(sec);
					savedLayoutSectionImages.Add(MakeLayoutSectionImage(sec));
					layoutSectionListBox.Items.Add(sec.Name);
					layoutSectionListBox.SelectedIndex = savedLayoutSections.Count - 1;
					string levelname = LevelData.Level.DisplayName;
					foreach (char c in Path.GetInvalidFileNameChars())
						levelname = levelname.Replace(c, '_');
					using (FileStream fs = File.Create(levelname + ".sls"))
						new BinaryFormatter().Serialize(fs, savedLayoutSections);
				}
			}
		}

		private void layoutSectionListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (layoutSectionListBox.SelectedIndex == -1)
				layoutSectionPreview.Image = null;
			else
				layoutSectionPreview.Image = savedLayoutSectionImages[layoutSectionListBox.SelectedIndex];
		}

		private void layoutSectionListBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (layoutSectionListBox.SelectedIndex != -1 && e.KeyCode == Keys.Delete
				&& MessageBox.Show(this, "Are you sure you want to delete layout section \"" + savedLayoutSections[layoutSectionListBox.SelectedIndex].Name + "\"?", "SonLVL", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.OK)
			{
				savedLayoutSections.RemoveAt(layoutSectionListBox.SelectedIndex);
				savedLayoutSectionImages.RemoveAt(layoutSectionListBox.SelectedIndex);
				layoutSectionListBox.Items.RemoveAt(layoutSectionListBox.SelectedIndex);
				string levelname = LevelData.Level.DisplayName;
				foreach (char c in Path.GetInvalidFileNameChars())
					levelname = levelname.Replace(c, '_');
				using (FileStream fs = File.Create(levelname + ".sls"))
					new BinaryFormatter().Serialize(fs, savedLayoutSections);
			}
		}

		private void pasteSectionOnceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PasteLayoutSectionOnce(savedLayoutSections[layoutSectionListBox.SelectedIndex]);
		}

		private void pasteSectionRepeatingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			PasteLayoutSectionRepeating(savedLayoutSections[layoutSectionListBox.SelectedIndex]);
		}

		private void deepCopyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			switch (CurrentArtTab)
			{
				case ArtTab.Chunks:
					Clipboard.SetData(typeof(ChunkCopyData).AssemblyQualifiedName, new ChunkCopyData(LevelData.Chunks[SelectedChunk]));
					break;
				case ArtTab.Blocks:
					Clipboard.SetData(typeof(BlockCopyData).AssemblyQualifiedName, new BlockCopyData(LevelData.Blocks[SelectedBlock]));
					break;
			}
		}

		private void flipBlockHButton_Click(object sender, EventArgs e)
		{
			Block newblk = LevelData.Blocks[SelectedBlock].Flip(true, false);
			LevelData.Blocks[SelectedBlock] = newblk;
			LevelData.RedrawBlock(SelectedBlock, true);
			copiedBlockTile = (blockTileEditor.SelectedObjects = GetSelectedBlockTiles())[0];
			if (copiedBlockTile.Tile < LevelData.Tiles.Count)
				TileSelector.SelectedIndex = LevelData.Level.TwoPlayerCompatible ? copiedBlockTile.Tile / 2 : copiedBlockTile.Tile;
			BlockPicture.Invalidate();
			BlockSelector.Invalidate();
		}

		private void flipBlockVButton_Click(object sender, EventArgs e)
		{
			Block newblk = LevelData.Blocks[SelectedBlock].Flip(false, true);
			LevelData.Blocks[SelectedBlock] = newblk;
			LevelData.RedrawBlock(SelectedBlock, true);
			copiedBlockTile = (blockTileEditor.SelectedObjects = GetSelectedBlockTiles())[0];
			if (copiedBlockTile.Tile < LevelData.Tiles.Count)
				TileSelector.SelectedIndex = LevelData.Level.TwoPlayerCompatible ? copiedBlockTile.Tile / 2 : copiedBlockTile.Tile;
			BlockPicture.Invalidate();
			BlockSelector.Invalidate();
		}

		private void flipTileHButton_Click(object sender, EventArgs e)
		{
			tile.Flip(true, false);
			if (LevelData.Level.TwoPlayerCompatible)
			{
				byte[] td = tile.ToTileInterlaced();
				Array.Copy(td, 0, LevelData.Tiles[SelectedTile], 0, 32);
				Array.Copy(td, 32, LevelData.Tiles[SelectedTile + 1], 0, 32);
				td.CopyTo(LevelData.TileArray, SelectedTile * 32);
			}
			else
			{
				LevelData.Tiles[SelectedTile] = tile.ToTile();
				LevelData.Tiles[SelectedTile].CopyTo(LevelData.TileArray, SelectedTile * 32);
			}
			for (int i = 0; i < LevelData.Blocks.Count; i++)
			{
				bool dr = false;
				for (int y = 0; y < 2; y++)
					for (int x = 0; x < 2; x++)
						if (LevelData.Blocks[i].Tiles[x, y].Tile == SelectedTile)
							dr = true;
				if (dr)
					LevelData.RedrawBlock(i, true);
			}
			if (LevelData.Level.TwoPlayerCompatible)
				TileSelector.Images[SelectedTile / 2] = LevelData.InterlacedTileToBmp4bpp(LevelData.TileArray, SelectedTile, SelectedColor.Y);
			else
				TileSelector.Images[SelectedTile] = LevelData.TileToBmp4bpp(LevelData.Tiles[SelectedTile], 0, SelectedColor.Y);
			blockTileEditor.SelectedObjects = blockTileEditor.SelectedObjects;
			DrawTilePicture();
			TileSelector.Invalidate();
		}

		private void flipTileVButton_Click(object sender, EventArgs e)
		{
			tile.Flip(false, true);
			if (LevelData.Level.TwoPlayerCompatible)
			{
				byte[] td = tile.ToTileInterlaced();
				Array.Copy(td, 0, LevelData.Tiles[SelectedTile], 0, 32);
				Array.Copy(td, 32, LevelData.Tiles[SelectedTile + 1], 0, 32);
				td.CopyTo(LevelData.TileArray, SelectedTile * 32);
			}
			else
			{
				LevelData.Tiles[SelectedTile] = tile.ToTile();
				LevelData.Tiles[SelectedTile].CopyTo(LevelData.TileArray, SelectedTile * 32);
			}
			for (int i = 0; i < LevelData.Blocks.Count; i++)
			{
				bool dr = false;
				for (int y = 0; y < 2; y++)
					for (int x = 0; x < 2; x++)
						if (LevelData.Blocks[i].Tiles[x, y].Tile == SelectedTile)
							dr = true;
				if (dr)
					LevelData.RedrawBlock(i, true);
			}
			if (LevelData.Level.TwoPlayerCompatible)
				TileSelector.Images[SelectedTile / 2] = LevelData.InterlacedTileToBmp4bpp(LevelData.TileArray, SelectedTile, SelectedColor.Y);
			else
				TileSelector.Images[SelectedTile] = LevelData.TileToBmp4bpp(LevelData.Tiles[SelectedTile], 0, SelectedColor.Y);
			blockTileEditor.SelectedObjects = blockTileEditor.SelectedObjects;
			DrawTilePicture();
			TileSelector.Invalidate();
		}

		private void showBlockBehindCollisionCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			DrawColPicture();
		}

		private void pasteOverToolStripMenuItem_Click(object sender, EventArgs e)
		{
			switch (CurrentArtTab)
			{
				case ArtTab.Chunks:
					if (Clipboard.ContainsData(typeof(ChunkCopyData).AssemblyQualifiedName))
					{
						ChunkCopyData cnkcpy = (ChunkCopyData)Clipboard.GetData(typeof(ChunkCopyData).AssemblyQualifiedName);
						bool isS2 = false;
						switch (LevelData.Level.ChunkFormat)
						{
							case EngineVersion.S2NA:
							case EngineVersion.S2:
							case EngineVersion.S3K:
							case EngineVersion.SKC:
								isS2 = true;
								break;
						}
						if (cnkcpy.IsS2 != isS2)
						{
							MessageBox.Show(this, "Copied chunk data does not match current level's format.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						if (LevelData.Level.TwoPlayerCompatible && !cnkcpy.IsInterlacedCompatible)
						{
							MessageBox.Show(this, "Copied chunk data is not 2P compatible.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						if (LevelData.Blocks.Count + cnkcpy.Blocks.Count > LevelData.GetBlockMax())
						{
							MessageBox.Show(this, "Level does not have enough free blocks.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						if (LevelData.Tiles.Count + cnkcpy.Tiles.Count > 0x8000)
						{
							MessageBox.Show(this, "Level does not have enough free tiles.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						List<ushort> tiles = new List<ushort>(cnkcpy.Tiles.Count);
						foreach (byte[] tile in cnkcpy.Tiles)
						{
							ushort ti = (ushort)LevelData.Tiles.Count;
							for (ushort j = 0; j < LevelData.Tiles.Count; j++)
								if (tile.FastArrayEqual(LevelData.Tiles[j]))
								{
									ti = j;
									break;
								}
							if (ti == LevelData.Tiles.Count)
								LevelData.Tiles.Add(tile);
							tiles.Add(ti);
						}
						LevelData.UpdateTileArray();
						RefreshTileSelector();
						blockTileEditor.SelectedObjects = blockTileEditor.SelectedObjects;
						List<ushort> blocks = new List<ushort>(cnkcpy.Blocks.Count);
						for (int i = 0; i < cnkcpy.Blocks.Count; i++)
						{
							Block block = cnkcpy.Blocks[i];
							for (int y = 0; y < 2; y++)
								for (int x = 0; x < 2; x++)
									block.Tiles[x, y].Tile = tiles[block.Tiles[x, y].Tile];
							ushort bi = (ushort)LevelData.Blocks.Count;
							for (ushort j = 0; j < LevelData.Blocks.Count; j++)
								if (block.Equals(LevelData.Blocks[j]))
								{
									bi = j;
									break;
								}
							if (bi == LevelData.Blocks.Count)
							{
								LevelData.Blocks.Add(block);
								LevelData.ColInds1.AddOrSet(bi, cnkcpy.ColInds1[i]);
								if (LevelData.ColInds2 != null && LevelData.ColInds2 != LevelData.ColInds1)
									LevelData.ColInds2.AddOrSet(bi, cnkcpy.ColInds2[i]);
								LevelData.BlockBmps.Add(new Bitmap[2]);
								LevelData.BlockBmpBits.Add(new BitmapBits[2]);
								LevelData.CompBlockBmps.Add(null);
								LevelData.CompBlockBmpBits.Add(null);
								LevelData.RedrawBlock(bi, false);
							}
							blocks.Add(bi);
						}
						for (int y = 0; y < LevelData.Level.ChunkHeight / 16; y++)
							for (int x = 0; x < LevelData.Level.ChunkWidth / 16; x++)
								cnkcpy.Chunk.Blocks[x, y].Block = blocks[cnkcpy.Chunk.Blocks[x, y].Block];
						LevelData.Chunks[SelectedChunk] = cnkcpy.Chunk;
					}
					else
						LevelData.Chunks[SelectedChunk] = new Chunk((byte[])Clipboard.GetData(typeof(Chunk).AssemblyQualifiedName), 0);
					LevelData.RedrawChunk(SelectedChunk);
					break;
				case ArtTab.Blocks:
					if (Clipboard.ContainsData(typeof(BlockCopyData).AssemblyQualifiedName))
					{
						BlockCopyData blkcpy = (BlockCopyData)Clipboard.GetData(typeof(BlockCopyData).AssemblyQualifiedName);
						if (LevelData.Level.TwoPlayerCompatible && !blkcpy.Block.IsInterlacedCompatible)
						{
							MessageBox.Show(this, "Copied block data is not 2P compatible.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						if (LevelData.Tiles.Count + blkcpy.Tiles.Count > 0x8000)
						{
							MessageBox.Show(this, "Level does not have enough free tiles.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
							return;
						}
						List<ushort> tiles = new List<ushort>(blkcpy.Tiles.Count);
						foreach (byte[] tile in blkcpy.Tiles)
						{
							ushort ti = (ushort)LevelData.Tiles.Count;
							for (ushort j = 0; j < LevelData.Tiles.Count; j++)
								if (tile.FastArrayEqual(LevelData.Tiles[j]))
								{
									ti = j;
									break;
								}
							if (ti == LevelData.Tiles.Count)
								LevelData.Tiles.Add(tile);
							tiles.Add(ti);
						}
						LevelData.UpdateTileArray();
						RefreshTileSelector();
						for (int y = 0; y < 2; y++)
							for (int x = 0; x < 2; x++)
								blkcpy.Block.Tiles[x, y].Tile = tiles[blkcpy.Block.Tiles[x, y].Tile];
						LevelData.Blocks[SelectedBlock] = blkcpy.Block;
					}
					else
						LevelData.Blocks[SelectedBlock] = new Block((byte[])Clipboard.GetData(typeof(Block).AssemblyQualifiedName), 0);
					LevelData.RedrawBlock(SelectedBlock, true);
					break;
				case ArtTab.Tiles:
					if (LevelData.Level.TwoPlayerCompatible)
					{
						byte[][] t = (byte[][])Clipboard.GetData("SonLVLTileInterlaced");
						LevelData.Tiles[SelectedTile] = t[0];
						LevelData.Tiles[SelectedTile + 1] = t[1];
						t[0].CopyTo(LevelData.TileArray, SelectedTile * 32);
						t[1].CopyTo(LevelData.TileArray, SelectedTile * 32 + 32);
					}
					else
					{
						byte[] t = (byte[])Clipboard.GetData("SonLVLTile");
						LevelData.Tiles[SelectedTile] = t;
						t.CopyTo(LevelData.TileArray, SelectedTile * 32);
					}
					for (int i = 0; i < LevelData.Blocks.Count; i++)
					{
						bool dr = false;
						for (int y = 0; y < 2; y++)
							for (int x = 0; x < 2; x++)
								if (LevelData.Blocks[i].Tiles[x, y].Tile == SelectedTile)
									dr = true;
						if (dr)
							LevelData.RedrawBlock(i, true);
					}
					if (LevelData.Level.TwoPlayerCompatible)
						TileSelector.Images[SelectedTile / 2] = LevelData.InterlacedTileToBmp4bpp(LevelData.TileArray, SelectedTile, SelectedColor.Y);
					else
						TileSelector.Images[SelectedTile] = LevelData.TileToBmp4bpp(LevelData.Tiles[SelectedTile], 0, SelectedColor.Y);
					blockTileEditor.SelectedObjects = blockTileEditor.SelectedObjects;
					break;
			}
		}

		private BlockColInfo[,] ProcessColBmps(Bitmap colbmp1, Bitmap colbmp2, int w, int h)
		{
			ColInfo[,] coldata1;
			ColInfo[,] coldata2 = null;
			using (colbmp1)
			using (Bitmap tmp = new Bitmap(w, h))
			using (Graphics g = Graphics.FromImage(tmp))
			{
				g.SetOptions();
				g.DrawImage(colbmp1, 0, 0, colbmp1.Width, colbmp1.Height);
				coldata1 = LevelData.GetColMap(tmp);
			}
			Application.DoEvents();
			if (colbmp2 != null)
				using (colbmp2)
				using (Bitmap tmp = new Bitmap(w, h))
				using (Graphics g = Graphics.FromImage(tmp))
				{
					g.SetOptions();
					g.DrawImage(colbmp2, 0, 0, colbmp2.Width, colbmp2.Height);
					coldata2 = LevelData.GetColMap(tmp);
					Application.DoEvents();
				}
			BlockColInfo[,] blockcoldata = new BlockColInfo[w / 16, h / 16];
			for (int y = 0; y < h / 16; y++)
				for (int x = 0; x < w / 16; x++)
				{
					ColInfo blk1 = coldata1[x, y];
					ColInfo blk2 = new ColInfo(Solidity.NotSolid, new sbyte[16], 0);
					if (coldata2 != null)
						blk2 = coldata2[x, y];
					byte ind1 = 0, ind2 = 0;
					bool xflip = false, yflip = false;
					if ((blk1.HeightMap.FastArrayEqual(LevelData.ColArr1[0xFF]) || blk1.Solidity == Solidity.NotSolid) && coldata2 != null)
					{
						ind1 = (byte)(blk1.Solidity == Solidity.NotSolid ? 0 : 0xFF);
						if (blk2.Solidity != Solidity.NotSolid)
							MatchCol(blk2, out ind2, out xflip, out yflip);
					}
					else
					{
						if (blk1.Solidity != Solidity.NotSolid)
							MatchCol(blk1, out ind1, out xflip, out yflip);
						if (blk2.Solidity != Solidity.NotSolid)
						{
							if (blk2.HeightMap.FastArrayEqual(LevelData.ColArr1[0xFF]))
							{
								ind2 = 0xFF;
							}
							else
							{
								sbyte[] map = (sbyte[])blk2.HeightMap.Clone();
								byte angle = blk2.Angle;
								if (xflip)
								{
									Array.Reverse(map);
									if (angle != 0xFF)
										angle = (byte)(-angle & 0xFF);
								}
								if (yflip)
								{
									for (int i = 0; i < 16; i++)
										map[i] = (sbyte)-map[i];
									if (angle != 0xFF)
										angle = (byte)((-(angle + 0x40) - 0x40) & 0xFF);
								}
								byte? emptymap = null;
								bool found = false;
								for (int i = 0; i < LevelData.ColArr1.Length; i++)
								{
									if (map.FastArrayEqual(LevelData.ColArr1[i]))
									{
										ind2 = (byte)i;
										found = true;
										break;
									}
									if (i > 0 && !emptymap.HasValue && LevelData.ColArr1[i].All(a => a == 0))
										emptymap = (byte)i;
								}
								if (!found && emptymap.HasValue)
								{
									LevelData.ColArr1[emptymap.Value] = map;
									LevelData.Angles[emptymap.Value] = angle;
									LevelData.RedrawCol(emptymap.Value, false);
									CollisionSelector.Images[emptymap.Value] = LevelData.ColBmps[emptymap.Value];
									ind2 = emptymap.Value;
								}
							}
						}
					}
					blockcoldata[x, y] = new BlockColInfo(ind1, ind2, blk1.Solidity, blk2.Solidity, xflip, yflip);
					Application.DoEvents();
				}
			return blockcoldata;
		}

		private void MatchCol(ColInfo blk, out byte ind, out bool xflip, out bool yflip)
		{
			if (blk.HeightMap.FastArrayEqual(LevelData.ColArr1[0xFF]))
			{
				xflip = false;
				yflip = false;
				ind = 0xFF;
				return;
			}
			sbyte[] maph = (sbyte[])blk.HeightMap.Clone();
			Array.Reverse(maph);
			sbyte[] mapv = (sbyte[])blk.HeightMap.Clone();
			for (int i = 0; i < 16; i++)
				mapv[i] = (sbyte)-mapv[i];
			sbyte[] maphv = (sbyte[])mapv.Clone();
			Array.Reverse(maphv);
			byte? emptymap = null;
			for (int i = 0; i < LevelData.ColArr1.Length; i++)
			{
				Application.DoEvents();
				if (blk.HeightMap.FastArrayEqual(LevelData.ColArr1[i]))
				{
					xflip = false;
					yflip = false;
					ind = (byte)i;
					return;
				}
				if (maph.FastArrayEqual(LevelData.ColArr1[i]))
				{
					xflip = true;
					yflip = false;
					ind = (byte)i;
					return;
				}
				if (mapv.FastArrayEqual(LevelData.ColArr1[i]))
				{
					xflip = false;
					yflip = true;
					ind = (byte)i;
					return;
				}
				if (maphv.FastArrayEqual(LevelData.ColArr1[i]))
				{
					xflip = true;
					yflip = true;
					ind = (byte)i;
					return;
				}
				if (i > 0 && !emptymap.HasValue && LevelData.ColArr1[i].All(a => a == 0))
					emptymap = (byte)i;
			}
			if (emptymap.HasValue)
			{
				LevelData.ColArr1[emptymap.Value] = blk.HeightMap;
				LevelData.Angles[emptymap.Value] = blk.Angle;
				LevelData.RedrawCol(emptymap.Value, false);
				CollisionSelector.Images[emptymap.Value] = LevelData.ColBmps[emptymap.Value];
				ind = emptymap.Value;
				Application.DoEvents();
			}
			else
				ind = 0;
			xflip = false;
			yflip = false;
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
						Bitmap colbmp1 = null, colbmp2 = null, pribmp = null;
						string fmt = Path.Combine(Path.GetDirectoryName(opendlg.FileName),
							Path.GetFileNameWithoutExtension(opendlg.FileName) + "_{0}" + Path.GetExtension(opendlg.FileName));
						if (File.Exists(string.Format(fmt, "col1")))
						{
							colbmp1 = new Bitmap(string.Format(fmt, "col1"));
							if (File.Exists(string.Format(fmt, "col2")))
								colbmp2 = new Bitmap(string.Format(fmt, "col2"));
						}
						else if (File.Exists(string.Format(fmt, "col")))
							colbmp1 = new Bitmap(string.Format(fmt, "col"));
						if (File.Exists(string.Format(fmt, "pri")))
							pribmp = new Bitmap(string.Format(fmt, "pri"));
						byte[,] section = new byte[bmp.Width / LevelData.Level.ChunkWidth, bmp.Height / LevelData.Level.ChunkHeight];
						if (!ImportImage(bmp, colbmp1, colbmp2, pribmp, section))
							return;
						byte[,] layout;
						bool[,] loop;
						int w, h;
						if (CurrentTab == Tab.Background)
						{
							layout = LevelData.Layout.BGLayout;
							loop = LevelData.Layout.BGLoop;
							w = Math.Min(section.GetLength(0), LevelData.BGWidth - menuLoc.X);
							h = Math.Min(section.GetLength(1), LevelData.BGHeight - menuLoc.Y);
						}
						else
						{
							layout = LevelData.Layout.FGLayout;
							loop = LevelData.Layout.FGLoop;
							w = Math.Min(section.GetLength(0), LevelData.FGWidth - menuLoc.X);
							h = Math.Min(section.GetLength(1), LevelData.FGHeight - menuLoc.Y);
						}
						for (int y = 0; y < h; y++)
							for (int x = 0; x < w; x++)
							{
								layout[x + menuLoc.X, y + menuLoc.Y] = section[x, y];
								if (loop != null)
									loop[x + menuLoc.X, y + menuLoc.Y] = false;
							}
					}
		}

		private void importToolStripButton_Click(object sender, EventArgs e)
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
						Bitmap colbmp1 = null, colbmp2 = null, pribmp = null;
						string fmt = Path.Combine(Path.GetDirectoryName(opendlg.FileName),
							Path.GetFileNameWithoutExtension(opendlg.FileName) + "_{0}" + Path.GetExtension(opendlg.FileName));
						if (File.Exists(string.Format(fmt, "col1")))
						{
							colbmp1 = new Bitmap(string.Format(fmt, "col1"));
							if (File.Exists(string.Format(fmt, "col2")))
								colbmp2 = new Bitmap(string.Format(fmt, "col2"));
						}
						else if (File.Exists(string.Format(fmt, "col")))
							colbmp1 = new Bitmap(string.Format(fmt, "col"));
						if (File.Exists(string.Format(fmt, "pri")))
							pribmp = new Bitmap(string.Format(fmt, "pri"));
						byte[,] layout = new byte[bmp.Width / LevelData.Level.ChunkWidth, bmp.Height / LevelData.Level.ChunkHeight];
						if (!ImportImage(bmp, colbmp1, colbmp2, pribmp, layout))
							return;
						LayoutSection section = new LayoutSection(layout, LevelData.LayoutFormat.HasLoopFlag ? new bool[layout.GetLength(0), layout.GetLength(1)] : null, new List<Entry>());
						using (LayoutSectionNameDialog dlg = new LayoutSectionNameDialog() { Value = Path.GetFileNameWithoutExtension(opendlg.FileName) })
						{
							if (dlg.ShowDialog(this) == DialogResult.OK)
							{
								section.Name = dlg.Value;
								savedLayoutSections.Add(section);
								savedLayoutSectionImages.Add(MakeLayoutSectionImage(section));
								layoutSectionListBox.Items.Add(section.Name);
								layoutSectionListBox.SelectedIndex = savedLayoutSections.Count - 1;
								string levelname = LevelData.Level.DisplayName;
								foreach (char c in Path.GetInvalidFileNameChars())
									levelname = levelname.Replace(c, '_');
								using (FileStream fs = File.Create(levelname + ".sls"))
									new BinaryFormatter().Serialize(fs, savedLayoutSections);
							}
						}
					}
		}

		private void selectPaletteToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			foreach (ToolStripMenuItem item in selectPaletteToolStripMenuItem.DropDownItems)
				item.Checked = false;
			((ToolStripMenuItem)e.ClickedItem).Checked = true;
			waterPalette = (int)e.ClickedItem.Tag;
			selectPaletteToolStripMenuItem.DropDownItems.Clear();
			selectPaletteToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem("None") { Checked = waterPalette == -1, Tag = -1 });
			for (int i = 0; i < LevelData.PalName.Count; i++)
				if (i != LevelData.CurPal)
					selectPaletteToolStripMenuItem.DropDownItems.Add(new ToolStripMenuItem(LevelData.PalName[i]) { Checked = waterPalette == i, Tag = i });
			LevelData.PaletteChanged();
			DrawLevel();
		}

		private void setPositionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (WaterHeightDialog dlg = new WaterHeightDialog(waterHeight))
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					waterHeight = dlg.Value;
					DrawLevel();
				}
		}

		private void switchMouseButtonsInChunkAndBlockEditorsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			if (switchMouseButtonsInChunkAndBlockEditorsToolStripMenuItem.Checked)
			{
				chunkblockMouseDraw = MouseButtons.Right;
				chunkblockMouseSelect = MouseButtons.Left;
				chunkCtrlLabel.Text = "RMB: Paint w/ selected block\nLMB: Select block";
			}
			else
			{
				chunkblockMouseDraw = MouseButtons.Left;
				chunkblockMouseSelect = MouseButtons.Right;
				chunkCtrlLabel.Text = "LMB: Paint w/ selected block\nRMB: Select block";
			}
			Settings.SwitchChunkBlockMouseButtons = switchMouseButtonsInChunkAndBlockEditorsToolStripMenuItem.Checked;
		}

		private void importProgressControl1_SizeChanged(object sender, EventArgs e)
		{
			importProgressControl1.Location = new Point((ClientSize.Width / 2) - (importProgressControl1.Width / 2), (ClientSize.Height / 2) - (importProgressControl1.Height / 2));
		}

		private void deleteUnusedTilesToolStripButton_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(this, "This action may break other levels that share part of the same tile set, or objects that have their art in this set.\n\nAre you sure you want to delete all tiles not used in blocks?", "Delete Unused Tiles", MessageBoxButtons.OKCancel) != DialogResult.OK)
				return;
			bool[] tilesused = new bool[LevelData.Tiles.Count];
			foreach (Block blk in LevelData.Blocks)
				foreach (PatternIndex pat in blk.Tiles)
					if (pat.Tile < tilesused.Length)
						tilesused[pat.Tile] = true;
			ushort c = 0;
			Dictionary<ushort, ushort> tilemap = new Dictionary<ushort, ushort>();
			for (ushort i = 0; i < tilesused.Length; i++)
				if (tilesused[i])
					tilemap[i] = c++;
			foreach (Block blk in LevelData.Blocks)
				foreach (PatternIndex pat in blk.Tiles)
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
			RefreshTileSelector();
			TileSelector.SelectedIndex = Math.Min(TileSelector.SelectedIndex, TileSelector.Images.Count - 1);
			blockTileEditor.SelectedObjects = blockTileEditor.SelectedObjects;
			MessageBox.Show(this, "Deleted " + numdel + " unused tiles.", "SonLVL");
		}

		private void deleteUnusedBlocksToolStripButton_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(this, "This action may break other levels that share part of the same block set.\n\nAre you sure you want to delete all blocks not used in chunks?", "Delete Unused Blocks", MessageBoxButtons.OKCancel) != DialogResult.OK)
				return;
			bool[] blocksused = new bool[LevelData.Blocks.Count];
			foreach (Chunk cnk in LevelData.Chunks)
				foreach (ChunkBlock blk in cnk.Blocks)
					if (blk.Block < blocksused.Length)
						blocksused[blk.Block] = true;
			ushort c = 0;
			Dictionary<ushort, ushort> blockmap = new Dictionary<ushort, ushort>();
			for (ushort i = 0; i < blocksused.Length; i++)
				if (blocksused[i])
					blockmap[i] = c++;
			foreach (Chunk cnk in LevelData.Chunks)
				foreach (ChunkBlock blk in cnk.Blocks)
					if (blockmap.ContainsKey(blk.Block))
						blk.Block = blockmap[blk.Block];
			int numdel = 0;
			for (int i = blocksused.Length - 1; i >= 0; i--)
			{
				if (blocksused[i]) continue;
				LevelData.Blocks.RemoveAt(i);
				LevelData.BlockBmpBits.RemoveAt(i);
				LevelData.BlockBmps.RemoveAt(i);
				LevelData.CompBlockBmpBits.RemoveAt(i);
				LevelData.CompBlockBmps.RemoveAt(i);
				LevelData.ColInds1.RemoveAt(i);
				if (!Object.ReferenceEquals(LevelData.ColInds1, LevelData.ColInds2))
					LevelData.ColInds2.RemoveAt(i);
				numdel++;
			}
			SelectedBlock = BlockSelector.SelectedIndex = Math.Min(SelectedBlock, LevelData.Blocks.Count - 1);
			chunkBlockEditor.SelectedObjects = chunkBlockEditor.SelectedObjects;
			MessageBox.Show(this, "Deleted " + numdel + " unused blocks.", "SonLVL");
		}

		private void deleteUnusedChunksToolStripButton_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(this, "This action may break other levels that share part of the same chunk set, or levels that alter the level layout dynamically.\n\nAre you sure you want to delete all chunks not used in the layout?", "Delete Unused Chunks", MessageBoxButtons.OKCancel) != DialogResult.OK)
				return;
			bool[] chunksused = new bool[LevelData.Chunks.Count];
			// kind of hacky but whatever
			if (LevelData.Level.LoopChunks != null)
				foreach (byte ch in LevelData.Level.LoopChunks)
					chunksused[ch] = true;
			LevelData.RemapLayouts((layout, x, y) =>
			{
				if (layout[x, y] < chunksused.Length)
					chunksused[layout[x, y]] = true;
			});
			byte c = 0;
			Dictionary<byte, byte> chunkmap = new Dictionary<byte, byte>();
			for (int i = 0; i < chunksused.Length; i++)
				if (chunksused[i])
					chunkmap[(byte)i] = c++;
			LevelData.RemapLayouts((layout, x, y) =>
			{
				if (chunkmap.ContainsKey(layout[x, y]))
					layout[x, y] = chunkmap[layout[x, y]];
			});
			int numdel = 0;
			for (int i = chunksused.Length - 1; i >= 0; i--)
			{
				if (chunksused[i]) continue;
				LevelData.Chunks.RemoveAt(i);
				LevelData.ChunkBmpBits.RemoveAt(i);
				LevelData.ChunkBmps.RemoveAt(i);
				LevelData.ChunkColBmpBits.RemoveAt(i);
				LevelData.ChunkColBmps.RemoveAt(i);
				LevelData.CompChunkBmpBits.RemoveAt(i);
				LevelData.CompChunkBmps.RemoveAt(i);
				numdel++;
			}
			ChunkSelector.SelectedIndex = SelectedChunk = Math.Min(SelectedChunk, (byte)(LevelData.Chunks.Count - 1));
			MessageBox.Show(this, "Deleted " + numdel + " unused chunks.", "SonLVL");
		}

		private void clearForegroundToolStripButton_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(this, "Are you sure you want to clear the foreground layout?", "Clear Foreground", MessageBoxButtons.OKCancel) == DialogResult.OK)
			{
				Array.Clear(LevelData.Layout.FGLayout, 0, LevelData.FGWidth * LevelData.FGHeight);
				if (LevelData.Layout.FGLoop != null)
					Array.Clear(LevelData.Layout.FGLoop, 0, LevelData.FGWidth * LevelData.FGHeight);
			}
		}

		private void clearBackgroundToolStripButton_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(this, "Are you sure you want to clear the background layout?", "Clear Background", MessageBoxButtons.OKCancel) == DialogResult.OK)
			{
				Array.Clear(LevelData.Layout.BGLayout, 0, LevelData.BGWidth * LevelData.BGHeight);
				if (LevelData.Layout.BGLoop != null)
					Array.Clear(LevelData.Layout.BGLoop, 0, LevelData.BGWidth * LevelData.BGHeight);
			}
		}

		private void calculateAngleButton_Click(object sender, EventArgs e)
		{
			ColAngle.Value = LevelData.GetColMap(LevelData.ColBmps[SelectedCol])[0, 0].Angle; // super lazy
		}

		private void CollisionSelector_MouseDown(object sender, MouseEventArgs e)
		{
			if (!loaded) return;
			if (e.Button == MouseButtons.Right)
			{
				pasteSolidsToolStripMenuItem.Enabled = Clipboard.ContainsData(typeof(ColInfo).AssemblyQualifiedName);
				solidsContextMenuStrip.Show(CollisionSelector, e.Location);
			}
		}

		private void copySolidsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Clipboard.SetData(typeof(ColInfo).AssemblyQualifiedName, new ColInfo(Solidity.NotSolid, LevelData.ColArr1[CollisionSelector.SelectedIndex], LevelData.Angles[CollisionSelector.SelectedIndex]));
		}

		private void pasteSolidsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ColInfo col = (ColInfo)Clipboard.GetData(typeof(ColInfo).AssemblyQualifiedName);
			col.HeightMap.CopyTo(LevelData.ColArr1[CollisionSelector.SelectedIndex], 0);
			LevelData.Angles[CollisionSelector.SelectedIndex] = col.Angle;
			LevelData.RedrawCol(CollisionSelector.SelectedIndex, true);
			CollisionSelector.Images[SelectedCol] = LevelData.ColBmps[SelectedCol];
			CollisionSelector.Invalidate();
			CollisionSelector_SelectedIndexChanged(this, EventArgs.Empty);
		}

		private void clearSolidsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Array.Clear(LevelData.ColArr1[CollisionSelector.SelectedIndex], 0, 16);
			LevelData.Angles[CollisionSelector.SelectedIndex] = 0;
			LevelData.RedrawCol(CollisionSelector.SelectedIndex, true);
			CollisionSelector_SelectedIndexChanged(this, EventArgs.Empty);
		}

		private void usageCountsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (StatisticsDialog dlg = new StatisticsDialog())
				dlg.ShowDialog(this);
		}

		private void replaceForegroundToolStripButton_Click(object sender, EventArgs e)
		{
			if (replaceFGChunksDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				byte fc = (byte)replaceFGChunksDialog.findChunk.Value;
				byte rc = (byte)replaceFGChunksDialog.replaceChunk.Value;
				int cnt = 0;
				for (int y = 0; y < LevelData.FGHeight; y++)
					for (int x = 0; x < LevelData.FGWidth; x++)
						if (LevelData.Layout.FGLayout[x,y] == fc)
						{
							LevelData.Layout.FGLayout[x, y] = rc;
							cnt++;
						}
				MessageBox.Show(this, "Replaced " + cnt + " chunks.", "SonLVL");
				DrawLevel();
			}
		}

		private void replaceBackgroundToolStripButton_Click(object sender, EventArgs e)
		{
			if (replaceBGChunksDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				byte fc = (byte)replaceBGChunksDialog.findChunk.Value;
				byte rc = (byte)replaceBGChunksDialog.replaceChunk.Value;
				int cnt = 0;
				for (int y = 0; y < LevelData.BGHeight; y++)
					for (int x = 0; x < LevelData.BGWidth; x++)
						if (LevelData.Layout.BGLayout[x, y] == fc)
						{
							LevelData.Layout.BGLayout[x, y] = rc;
							cnt++;
						}
				MessageBox.Show(this, "Replaced " + cnt + " chunks.", "SonLVL");
				DrawLevel();
			}
		}

		private void replaceChunkBlocksToolStripButton_Click(object sender, EventArgs e)
		{
			if (replaceChunkBlocksDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				var list = LevelData.Chunks.SelectMany((a, b) => a.Blocks.OfType<ChunkBlock>().Select(c => new KeyValuePair<int, ChunkBlock>(b, c))).ToList();
				ushort? block = replaceChunkBlocksDialog.findBlock.Block;
				if (block.HasValue)
					list = list.Where(a => a.Value.Block == block.Value).ToList();
				bool? xflip = replaceChunkBlocksDialog.findBlock.XFlip;
				if (xflip.HasValue)
					list = list.Where(a => a.Value.XFlip == xflip.Value).ToList();
				bool? yflip = replaceChunkBlocksDialog.findBlock.YFlip;
				if (yflip.HasValue)
					list = list.Where(a => a.Value.YFlip = yflip.Value).ToList();
				Solidity? solid1 = replaceChunkBlocksDialog.findBlock.Solidity1;
				if (solid1.HasValue)
					list = list.Where(a => a.Value.Solid1 == solid1.Value).ToList();
				Solidity? solid2 = replaceChunkBlocksDialog.findBlock.Solidity2;
				if (solid2.HasValue)
					list = list.Where(a => ((S2ChunkBlock)a.Value).Solid2 == solid2.Value).ToList();
				block = replaceChunkBlocksDialog.replaceBlock.Block;
				xflip = replaceChunkBlocksDialog.replaceBlock.XFlip;
				yflip = replaceChunkBlocksDialog.replaceBlock.YFlip;
				solid1 = replaceChunkBlocksDialog.replaceBlock.Solidity1;
				solid2 = replaceChunkBlocksDialog.replaceBlock.Solidity2;
				foreach (ChunkBlock blk in list.Select(a => a.Value))
				{
					if (block.HasValue)
						blk.Block = block.Value;
					if (xflip.HasValue)
						blk.XFlip = xflip.Value;
					if (yflip.HasValue)
						blk.YFlip = yflip.Value;
					if (solid1.HasValue)
						blk.Solid1 = solid1.Value;
					if (solid2.HasValue)
						((S2ChunkBlock)blk).Solid2 = solid2.Value;
				}
				foreach (int i in list.Select(a => a.Key).Distinct())
					LevelData.RedrawChunk(i);
				ChunkSelector.Invalidate();
				DrawChunkPicture();
				chunkBlockEditor.SelectedObjects = chunkBlockEditor.SelectedObjects;
				MessageBox.Show(this, "Replaced " + list.Count + " chunk blocks.", "SonLVL");
			}
		}

		private void replaceBlockTilesToolStripButton_Click(object sender, EventArgs e)
		{
			if (replaceBlockTilesDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				var list = LevelData.Blocks.SelectMany((a, b) => a.Tiles.OfType<PatternIndex>().Select(c => new KeyValuePair<int, PatternIndex>(b, c))).ToList();
				ushort? tile = replaceBlockTilesDialog.findTile.Tile;
				if (tile.HasValue)
					list = list.Where(a => a.Value.Tile == tile.Value).ToList();
				bool? xflip = replaceBlockTilesDialog.findTile.XFlip;
				if (xflip.HasValue)
					list = list.Where(a => a.Value.XFlip == xflip.Value).ToList();
				bool? yflip = replaceBlockTilesDialog.findTile.YFlip;
				if (yflip.HasValue)
					list = list.Where(a => a.Value.YFlip = yflip.Value).ToList();
				bool? priority = replaceBlockTilesDialog.findTile.Priority;
				if (priority.HasValue)
					list = list.Where(a => a.Value.Priority == priority.Value).ToList();
				byte? palette = replaceBlockTilesDialog.findTile.Palette;
				if (palette.HasValue)
					list = list.Where(a => a.Value.Palette == palette.Value).ToList();
				tile = replaceBlockTilesDialog.replaceTile.Tile;
				xflip = replaceBlockTilesDialog.replaceTile.XFlip;
				yflip = replaceBlockTilesDialog.replaceTile.YFlip;
				priority = replaceBlockTilesDialog.replaceTile.Priority;
				palette = replaceBlockTilesDialog.replaceTile.Palette;
				foreach (PatternIndex blk in list.Select(a => a.Value))
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
				foreach (int i in list.Select(a => a.Key).Distinct())
					LevelData.RedrawBlock(i, true);
				BlockSelector.Invalidate();
				DrawBlockPicture();
				blockTileEditor.SelectedObjects = blockTileEditor.SelectedObjects;
				MessageBox.Show(this, "Replaced " + list.Count + " block tiles.", "SonLVL");
			}
		}

		private void removeDuplicateChunksToolStripButton_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(this, "This action may break other levels that share the same chunk set, or levels that alter the level layout dynamically.\n\nAre you sure you want to remove all duplicate chunks?", "SonLVL", MessageBoxButtons.OKCancel) != DialogResult.OK)
				return;
			Dictionary<byte, byte[]> chunks = new Dictionary<byte, byte[]>(LevelData.Chunks.Count);
			Dictionary<byte, byte> chunkMap = new Dictionary<byte, byte>(LevelData.Chunks.Count);
			Stack<int> deleted = new Stack<int>();
			for (int i = 0; i < LevelData.Chunks.Count; i++)
			{
				byte[] cnk = LevelData.Chunks[i].GetBytes();
				foreach (var item in chunks)
					if (cnk.FastArrayEqual(item.Value))
					{
						chunkMap[(byte)i] = item.Key;
						deleted.Push(i);
						break;
					}
				if (!chunkMap.ContainsKey((byte)i))
				{
					chunkMap[(byte)i] = (byte)chunks.Count;
					chunks[(byte)chunks.Count] = cnk;
				}
			}
			if (deleted.Count > 0)
			{
				foreach (int i in deleted)
				{
					LevelData.Chunks.RemoveAt(i);
					LevelData.ChunkBmpBits.RemoveAt(i);
					LevelData.ChunkBmps.RemoveAt(i);
					LevelData.ChunkColBmpBits.RemoveAt(i);
					LevelData.ChunkColBmps.RemoveAt(i);
					LevelData.CompChunkBmpBits.RemoveAt(i);
					LevelData.CompChunkBmps.RemoveAt(i);
				}
				ChunkSelector.SelectedIndex = Math.Min(ChunkSelector.SelectedIndex, LevelData.Chunks.Count - 1);
				LevelData.RemapLayouts((layout, x, y) =>
				{
					if (chunkMap.ContainsKey(layout[x, y]))
						layout[x, y] = chunkMap[layout[x, y]];
				});
				DrawLevel();
			}
			MessageBox.Show(this, "Removed " + deleted.Count + " duplicate chunks.", "SonLVL");
		}

		private void removeDuplicateBlocksToolStripButton_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(this, "This action may break other levels that share part of the same block set.\n\nAre you sure you want to remove all duplicate blocks?", "SonLVL", MessageBoxButtons.OKCancel) != DialogResult.OK)
				return;
			Dictionary<ushort, byte[]> blocks = new Dictionary<ushort, byte[]>(LevelData.Blocks.Count);
			Dictionary<ushort, ChunkBlock> blockMap = new Dictionary<ushort, ChunkBlock>(LevelData.Blocks.Count);
			Dictionary<int, int> colIndMap = new Dictionary<int, int>(LevelData.Blocks.Count);
			Stack<int> deleted = new Stack<int>();
			for (int i = 0; i < LevelData.Blocks.Count; i++)
			{
				byte[] blk = LevelData.Blocks[i].GetBytes();
				byte[] blkh = LevelData.Blocks[i].Flip(true, false).GetBytes();
				byte[] blkv = LevelData.Blocks[i].Flip(false, true).GetBytes();
				byte[] blkhv = LevelData.Blocks[i].Flip(true, true).GetBytes();
				foreach (var item in blocks)
				{
					if (LevelData.ColInds1[i] != LevelData.ColInds1[colIndMap[item.Key]])
						continue;
					if (!Object.ReferenceEquals(LevelData.ColInds1, LevelData.ColInds2) && LevelData.ColInds2[i] != LevelData.ColInds2[colIndMap[item.Key]])
						continue;
					if (blk.FastArrayEqual(item.Value))
					{
						blockMap[(ushort)i] = new S1ChunkBlock() { Block = item.Key };
						deleted.Push(i);
						break;
					}
					if (blkh.FastArrayEqual(item.Value))
					{
						blockMap[(ushort)i] = new S1ChunkBlock() { Block = item.Key, XFlip = true };
						deleted.Push(i);
						break;
					}
					if (blkv.FastArrayEqual(item.Value))
					{
						blockMap[(ushort)i] = new S1ChunkBlock() { Block = item.Key, YFlip = true };
						deleted.Push(i);
						break;
					}
					if (blkhv.FastArrayEqual(item.Value))
					{
						blockMap[(ushort)i] = new S1ChunkBlock() { Block = item.Key, XFlip = true, YFlip = true };
						deleted.Push(i);
						break;
					}
				}
				if (!blockMap.ContainsKey((ushort)i))
				{
					blockMap[(ushort)i] = new S1ChunkBlock() { Block = (ushort)blocks.Count };
					blocks[(ushort)blocks.Count] = blk;
					colIndMap[colIndMap.Count] = i;
				}
			}
			if (deleted.Count > 0)
			{
				foreach (int i in deleted)
				{
					LevelData.Blocks.RemoveAt(i);
					LevelData.BlockBmpBits.RemoveAt(i);
					LevelData.BlockBmps.RemoveAt(i);
					LevelData.CompBlockBmpBits.RemoveAt(i);
					LevelData.CompBlockBmps.RemoveAt(i);
					LevelData.ColInds1.RemoveAt(i);
					if (!Object.ReferenceEquals(LevelData.ColInds1, LevelData.ColInds2))
						LevelData.ColInds2.RemoveAt(i);
				}
				BlockSelector.SelectedIndex = Math.Min(BlockSelector.SelectedIndex, LevelData.Blocks.Count - 1);
				for (int i = 0; i < LevelData.Chunks.Count; i++)
					foreach (ChunkBlock cb in LevelData.Chunks[i].Blocks)
						if (blockMap.ContainsKey(cb.Block))
						{
							ChunkBlock nb = blockMap[cb.Block];
							cb.Block = nb.Block;
							cb.XFlip ^= nb.XFlip;
							cb.YFlip ^= nb.YFlip;
						}
				chunkBlockEditor.SelectedObjects = chunkBlockEditor.SelectedObjects;
				DrawLevel();
			}
			MessageBox.Show(this, "Removed " + deleted.Count + " duplicate blocks.", "SonLVL");
		}

		private void removeDuplicateTilesToolStripButton_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(this, "This action may break other levels that share part of the same tile set, or objects that have their art in this set.\n\nAre you sure you want to remove all duplicate tiles?", "SonLVL", MessageBoxButtons.OKCancel) != DialogResult.OK)
				return;
			if (LevelData.Level.TwoPlayerCompatible)
			{
				Dictionary<ushort, byte[]> tiles = new Dictionary<ushort, byte[]>(LevelData.Tiles.Count / 2);
				Dictionary<ushort, PatternIndex> tileMap = new Dictionary<ushort, PatternIndex>(LevelData.Tiles.Count / 2);
				Stack<int> deleted = new Stack<int>();
				for (int i = 0; i < LevelData.Tiles.Count; i += 2)
				{
					byte[] tile = new byte[64];
					Array.Copy(LevelData.TileArray, i * 32, tile, 0, 64);
					byte[] tileh = new byte[64];
					for (int ty = 0; ty < 16; ty++)
						for (int tx = 0; tx < 4; tx++)
						{
							byte px = tile[(ty * 4) + tx];
							tileh[(ty * 4) + (3 - tx)] = (byte)((px >> 4) | (px << 4));
						}
					byte[] tilev = new byte[64];
					for (int ty = 0; ty < 16; ty++)
						Array.Copy(tile, ty * 4, tilev, (15 - ty) * 4, 4);
					byte[] tilehv = new byte[64];
					for (int ty = 0; ty < 16; ty++)
						Array.Copy(tileh, ty * 4, tilehv, (15 - ty) * 4, 4);
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
						tileMap[(ushort)i] = new PatternIndex() { Tile = (ushort)(tiles.Count * 2) };
						tiles[(ushort)(tiles.Count * 2)] = tile;
					}
				}
				if (deleted.Count > 0)
				{
					foreach (int i in deleted)
					{
						LevelData.Tiles.RemoveAt(i);
						LevelData.Tiles.RemoveAt(i);
					}
					LevelData.UpdateTileArray();
					RefreshTileSelector();
					TileSelector.SelectedIndex = Math.Min(TileSelector.SelectedIndex, TileSelector.Images.Count - 1);
					for (int i = 0; i < LevelData.Blocks.Count; i++)
						for (int x = 0; x < 2; x++)
							if (tileMap.ContainsKey((ushort)(LevelData.Blocks[i].Tiles[x, 0].Tile & ~1)))
							{
								PatternIndex nb = tileMap[(ushort)(LevelData.Blocks[i].Tiles[x, 0].Tile & ~1)];
								LevelData.Blocks[i].Tiles[x, 0].Tile = nb.Tile;
								LevelData.Blocks[i].Tiles[x, 0].XFlip ^= nb.XFlip;
								LevelData.Blocks[i].Tiles[x, 0].YFlip ^= nb.YFlip;
								LevelData.Blocks[i].MakeInterlacedCompatible();
							}
					blockTileEditor.SelectedObjects = blockTileEditor.SelectedObjects;
					DrawLevel();
				}
				MessageBox.Show(this, "Removed " + deleted.Count + " duplicate tiles.", "SonLVL");
			}
			else
			{
				Dictionary<ushort, byte[]> tiles = new Dictionary<ushort, byte[]>(LevelData.Tiles.Count);
				Dictionary<ushort, PatternIndex> tileMap = new Dictionary<ushort, PatternIndex>(LevelData.Tiles.Count);
				Stack<int> deleted = new Stack<int>();
				for (int i = 0; i < LevelData.Tiles.Count; i++)
				{
					byte[] tile = LevelData.Tiles[i];
					byte[] tileh = new byte[32];
					for (int ty = 0; ty < 8; ty++)
						for (int tx = 0; tx < 4; tx++)
						{
							byte px = tile[(ty * 4) + tx];
							tileh[(ty * 4) + (3 - tx)] = (byte)((px >> 4) | (px << 4));
						}
					byte[] tilev = new byte[32];
					for (int ty = 0; ty < 8; ty++)
						Array.Copy(tile, ty * 4, tilev, (7 - ty) * 4, 4);
					byte[] tilehv = new byte[32];
					for (int ty = 0; ty < 8; ty++)
						Array.Copy(tileh, ty * 4, tilehv, (7 - ty) * 4, 4);
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
					TileSelector.SelectedIndex = Math.Min(TileSelector.SelectedIndex, LevelData.Tiles.Count - 1);
					for (int i = 0; i < LevelData.Blocks.Count; i++)
						foreach (PatternIndex cb in LevelData.Blocks[i].Tiles)
							if (tileMap.ContainsKey(cb.Tile))
							{
								PatternIndex nb = tileMap[cb.Tile];
								cb.Tile = nb.Tile;
								cb.XFlip ^= nb.XFlip;
								cb.YFlip ^= nb.YFlip;
							}
					blockTileEditor.SelectedObjects = blockTileEditor.SelectedObjects;
					DrawLevel();
				}
				MessageBox.Show(this, "Removed " + deleted.Count + " duplicate tiles.", "SonLVL");
			}
		}

		private void importOverToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog opendlg = new OpenFileDialog())
			{
				opendlg.DefaultExt = "png";
				opendlg.Filter = "Image Files|*.bmp;*.png;*.jpg;*.gif";
				opendlg.RestoreDirectory = true;
				if (opendlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{
					BitmapInfo bmpi;
					using (Bitmap bmp = new Bitmap(opendlg.FileName))
						bmpi = new BitmapInfo(bmp);
					switch (CurrentArtTab)
					{
						case ArtTab.Chunks:
							if (bmpi.Width < LevelData.Level.ChunkWidth || bmpi.Height < LevelData.Level.ChunkHeight)
							{
								MessageBox.Show(this, "Image must be at least " + LevelData.Level.ChunkWidth + "x" + LevelData.Level.ChunkHeight + " to import chunk.", "SonLVL");
								return;
							}
							break;
						case ArtTab.Blocks:
							if (bmpi.Width < 16 || bmpi.Height < 16)
							{
								MessageBox.Show(this, "Image must be at least 16x16 to import block.", "SonLVL");
								return;
							}
							break;
						case ArtTab.Tiles:
							if (bmpi.Width < 8 || bmpi.Height < (LevelData.Level.TwoPlayerCompatible ? 16 : 8))
							{
								MessageBox.Show(this, "Image must be at least 8x" + (LevelData.Level.TwoPlayerCompatible ? "16" : "8") + " to import tile.", "SonLVL");
								return;
							}
							break;
					}
					ImportResult res = LevelData.BitmapToTiles(bmpi, new bool[bmpi.Width / 8, bmpi.Height / 8], null, new List<byte[]>(), false, false, () => Application.DoEvents());
					List<int> editedTiles = new List<int>();
					switch (CurrentArtTab)
					{
						case ArtTab.Chunks:
							Chunk cnk = LevelData.Chunks[SelectedChunk];
							for (int by = 0; by < LevelData.Level.ChunkHeight / 16; by++)
								for (int bx = 0; bx < LevelData.Level.ChunkWidth / 16; bx++)
								{
									Block blk = LevelData.Blocks[cnk.Blocks[bx, by].Block].Flip(cnk.Blocks[bx, by].XFlip, cnk.Blocks[bx, by].YFlip);
									for (int y = 0; y < 2; y++)
										for (int x = 0; x < 2; x++)
											if (!editedTiles.Contains(blk.Tiles[x, y].Tile))
											{
												LevelData.Tiles[blk.Tiles[x, y].Tile] = LevelData.FlipTile(res.Art[res.Mappings[(bx * 2) + x, (by * 2) + y].Tile], blk.Tiles[x, y].XFlip, blk.Tiles[x, y].YFlip);
												editedTiles.Add(blk.Tiles[x, y].Tile);
											}
								}
							break;
						case ArtTab.Blocks:
							Block blk2 = LevelData.Blocks[SelectedBlock];
							for (int y = 0; y < 2; y++)
								for (int x = 0; x < 2; x++)
									if (!editedTiles.Contains(blk2.Tiles[x, y].Tile))
									{
										LevelData.Tiles[blk2.Tiles[x, y].Tile] = LevelData.FlipTile(res.Art[res.Mappings[x, y].Tile], blk2.Tiles[x, y].XFlip, blk2.Tiles[x, y].YFlip);
										editedTiles.Add(blk2.Tiles[x, y].Tile);
									}
							break;
						case ArtTab.Tiles:
							LevelData.Tiles[SelectedTile] = res.Art[res.Mappings[0, 0].Tile];
							editedTiles.Add(SelectedTile);
							if (LevelData.Level.TwoPlayerCompatible)
								LevelData.Tiles[SelectedTile + 1] = res.Art[res.Mappings[0, 1].Tile];
							break;
					}
					LevelData.UpdateTileArray();
					RefreshTileSelector();
					TileSelector.Invalidate();
					if (editedTiles.Contains(SelectedTile))
						TileSelector_SelectedIndexChanged(this, EventArgs.Empty);
					blockTileEditor.SelectedObjects = blockTileEditor.SelectedObjects;
					List<int> editedBlocks = new List<int>();
					for (int i = 0; i < LevelData.Blocks.Count; i++)
						if (LevelData.Blocks[i].Tiles.OfType<PatternIndex>().Any(a => editedTiles.Contains(a.Tile)))
						{
							editedBlocks.Add(i);
							LevelData.RedrawBlock(i, false);
						}
					if (editedBlocks.Contains(SelectedBlock))
						DrawBlockPicture();
					BlockSelector.Invalidate();
					chunkBlockEditor.SelectedObjects = chunkBlockEditor.SelectedObjects;
					for (int i = 0; i < LevelData.Chunks.Count; i++)
						if (LevelData.Chunks[i].Blocks.OfType<ChunkBlock>().Any(a => editedBlocks.Contains(a.Block)))
						{
							LevelData.RedrawChunk(i);
							if (i == SelectedChunk)
								DrawChunkPicture();
						}
					ChunkSelector.Invalidate();
				}
			}
		}

		private void BlockSelector_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (loaded && e.Button == MouseButtons.Left)
				foreach (ChunkBlock blk in GetSelectedChunkBlocks())
					blk.Block = (ushort)BlockSelector.SelectedIndex;
		}

		private void TileSelector_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (loaded && e.Button == MouseButtons.Left)
				foreach (PatternIndex til in GetSelectedBlockTiles())
					til.Tile = (ushort)TileSelector.SelectedIndex;
		}
	}

	public enum EditingMode { Draw, Select }

	[Serializable]
	public class LayoutSection
	{
		public string Name { get; set; }
		public byte[,] Layout { get; set; }
		public bool[,] Loop { get; set; }
		public List<Entry> Objects { get; set; }

		public LayoutSection(byte[,] layout, bool[,] loop, List<Entry> objects)
		{
			Layout = layout;
			Loop = loop;
			Objects = objects;
		}
	}

	[Serializable]
	public class ChunkCopyData
	{
		public List<byte[]> Tiles { get; set; }
		public List<Block> Blocks { get; set; }
		public List<byte> ColInds1 { get; set; }
		public List<byte> ColInds2 { get; set; }
		public Chunk Chunk { get; set; }
		public bool IsS2 { get; set; }
		public bool IsInterlacedCompatible { get; set; }

		public ChunkCopyData(Chunk chunk)
		{
			switch (LevelData.Level.ChunkFormat)
			{
				case EngineVersion.S2NA:
				case EngineVersion.S2:
				case EngineVersion.S3K:
				case EngineVersion.SKC:
					IsS2 = true;
					break;
			}
			Chunk = chunk.Clone();
			List<ushort> blocks = new List<ushort>();
			if (LevelData.ColInds1 != null)
			{
				ColInds1 = new List<byte>();
				if (LevelData.ColInds2 != null && LevelData.ColInds2 != LevelData.ColInds1)
					ColInds2 = new List<byte>();
			}
			for (int y = 0; y < LevelData.Level.ChunkHeight / 16; y++)
				for (int x = 0; x < LevelData.Level.ChunkWidth / 16; x++)
					if (chunk.Blocks[x, y].Block < LevelData.Blocks.Count)
					{
						int i = blocks.IndexOf(chunk.Blocks[x, y].Block);
						if (i == -1)
						{
							i = blocks.Count;
							blocks.Add(chunk.Blocks[x, y].Block);
							if (ColInds1 != null)
							{
								ColInds1.Add(LevelData.GetColInd1(chunk.Blocks[x, y].Block));
								if (ColInds2 != null)
									ColInds2.Add(LevelData.GetColInd2(chunk.Blocks[x, y].Block));
							}
						}
						Chunk.Blocks[x, y].Block = (ushort)i;
					}
			Blocks = new List<Block>(blocks.Count);
			IsInterlacedCompatible = true;
			List<ushort> tiles = new List<ushort>();
			foreach (ushort blkind in blocks)
			{
				Block block = LevelData.Blocks[blkind].Clone();
				if (!block.IsInterlacedCompatible)
					IsInterlacedCompatible = false;
				for (int y = 0; y < 2; y++)
					for (int x = 0; x < 2; x++)
						if (block.Tiles[x, y].Tile < LevelData.Tiles.Count)
						{
							int i = tiles.IndexOf(block.Tiles[x, y].Tile);
							if (i == -1)
							{
								i = tiles.Count;
								tiles.Add(block.Tiles[x, y].Tile);
							}
							block.Tiles[x, y].Tile = (ushort)i;
						}
				Blocks.Add(block);
			}
			Tiles = new List<byte[]>(tiles.Count);
			for (int i = 0; i < tiles.Count; i++)
				Tiles.Add(LevelData.Tiles[tiles[i]]);
		}
	}

	[Serializable]
	public class BlockCopyData
	{
		public List<byte[]> Tiles { get; set; }
		public Block Block { get; set; }

		public BlockCopyData(Block block)
		{
			Block = new Block(block.GetBytes(), 0);
			List<ushort> tiles = new List<ushort>();
			for (int y = 0; y < 2; y++)
				for (int x = 0; x < 2; x++)
					if (block.Tiles[x, y].Tile < LevelData.Tiles.Count)
					{
						int i = tiles.IndexOf(block.Tiles[x, y].Tile);
						if (i == -1)
						{
							i = tiles.Count;
							tiles.Add(block.Tiles[x, y].Tile);
						}
						Block.Tiles[x, y].Tile = (ushort)i;
					}
			Tiles = new List<byte[]>(tiles.Count);
			for (int i = 0; i < tiles.Count; i++)
				Tiles.Add(LevelData.Tiles[tiles[i]]);
		}
	}

	public class BlockColInfo
	{
		public byte ColInd1 { get; private set; }
		public byte ColInd2 { get; private set; }
		public Solidity Solidity1 { get; private set; }
		public Solidity Solidity2 { get; private set; }
		public bool XFlip { get; private set; }
		public bool YFlip { get; private set; }

		public BlockColInfo(byte colInd1, byte colInd2, Solidity solidity1, Solidity solidity2, bool xflip, bool yflip)
		{
			ColInd1 = colInd1;
			ColInd2 = colInd2;
			Solidity1 = solidity1;
			Solidity2 = solidity2;
			XFlip = xflip;
			YFlip = yflip;
		}
	}

	public abstract class UndoAction
	{
		public string Name { get; protected set; }
		public abstract void Undo();
		public abstract void Redo();
	}

	internal class LayoutEditUndoAction : UndoAction
	{
		private List<Point> locations;
		private List<byte> oldtiles;
		private int mode;

		public LayoutEditUndoAction(int Mode, List<Point> Locations, List<byte> OldTiles)
		{
			Name = "Layout Edit" + " (" + Locations.Count + " chunk" + (Locations.Count > 1 ? "s" : "") + ")";
			mode = Mode;
			locations = Locations;
			oldtiles = OldTiles;
		}

		public override void Undo()
		{
			byte t;
			switch (mode)
			{
				case 1:
					for (int i = 0; i < locations.Count; i++)
					{
						t = LevelData.Layout.FGLayout[locations[i].X, locations[i].Y];
						LevelData.Layout.FGLayout[locations[i].X, locations[i].Y] = oldtiles[i];
						oldtiles[i] = t;
					}
					break;
				case 2:
					for (int i = 0; i < locations.Count; i++)
					{
						t = LevelData.Layout.BGLayout[locations[i].X, locations[i].Y];
						LevelData.Layout.BGLayout[locations[i].X, locations[i].Y] = oldtiles[i];
						oldtiles[i] = t;
					}
					break;
			}
		}

		public override void Redo()
		{
			Undo();
		}
	}

	internal class ObjectPropertyChangedUndoAction : UndoAction
	{
		private List<Entry> objects;
		private List<object> values;
		private PropertyDescriptor prop;

		public ObjectPropertyChangedUndoAction(List<Entry> Objects, List<object> Values, PropertyDescriptor Property)
		{
			Name = "Change of property " + Property.DisplayName + " (" + Objects.Count + " object" + (Objects.Count > 1 ? "s" : "") + ")";
			objects = Objects;
			values = Values;
			prop = Property;
		}

		public override void Undo()
		{
			object val;
			for (int i = 0; i < objects.Count; i++)
			{
				val = prop.GetValue(objects[i]);
				prop.SetValue(objects[i], values[i]);
				values[i] = val;
				objects[i].UpdateSprite();
			}
		}

		public override void Redo()
		{
			Undo();
		}
	}

	internal class ObjectMoveUndoAction : UndoAction
	{
		private List<Entry> objects;
		private List<Point> locations;

		public ObjectMoveUndoAction(List<Entry> Objects, List<Point> Locations)
		{
			Name = "Moved " + Objects.Count + " object" + (Objects.Count > 1 ? "s" : "");
			objects = Objects;
			locations = Locations;
		}

		public override void Undo()
		{
			Point pt;
			for (int i = 0; i < objects.Count; i++)
			{
				pt = new Point(objects[i].X, objects[i].Y);
				objects[i].X = (ushort)locations[i].X;
				objects[i].Y = (ushort)locations[i].Y;
				locations[i] = pt;
				objects[i].UpdateSprite();
			}
		}

		public override void Redo()
		{
			Undo();
		}
	}

	internal class ObjectAddedUndoAction : UndoAction
	{
		private Entry obj;

		public ObjectAddedUndoAction(Entry Obj)
		{
			Name = "Added object";
			obj = Obj;
		}

		public override void Redo()
		{
			if (obj is ObjectEntry)
			{
				LevelData.Objects.Add((ObjectEntry)obj);
			}
			else if (obj is RingEntry)
			{
				LevelData.Rings.Add((RingEntry)obj);
			}
			else if (obj is CNZBumperEntry)
			{
				LevelData.Bumpers.Add((CNZBumperEntry)obj);
			}
		}

		public override void Undo()
		{
			if (obj is ObjectEntry)
			{
				LevelData.Objects.Remove((ObjectEntry)obj);
			}
			else if (obj is RingEntry)
			{
				LevelData.Rings.Remove((RingEntry)obj);
			}
			else if (obj is CNZBumperEntry)
			{
				LevelData.Bumpers.Remove((CNZBumperEntry)obj);
			}
		}
	}

	internal class ObjectsDeletedUndoAction : UndoAction
	{
		private List<Entry> objs;

		public ObjectsDeletedUndoAction(List<Entry> Objects)
		{
			Name = "Deleted " + Objects.Count + " object" + (Objects.Count > 1 ? "s" : "");
			objs = Objects;
		}

		public override void Undo()
		{
			foreach (Entry item in objs)
			{
				if (item is ObjectEntry)
				{
					LevelData.Objects.Add((ObjectEntry)item);
				}
				else if (item is RingEntry)
				{
					LevelData.Rings.Add((RingEntry)item);
				}
				else if (item is CNZBumperEntry)
				{
					LevelData.Bumpers.Add((CNZBumperEntry)item);
				}
			}
		}

		public override void Redo()
		{
			foreach (Entry item in objs)
			{
				if (item is ObjectEntry)
				{
					LevelData.Objects.Remove((ObjectEntry)item);
				}
				else if (item is RingEntry)
				{
					LevelData.Rings.Remove((RingEntry)item);
				}
				else if (item is CNZBumperEntry)
				{
					LevelData.Bumpers.Remove((CNZBumperEntry)item);
				}
			}
		}
	}


	internal class ObjectsPastedUndoAction : UndoAction
	{
		private List<Entry> objs;

		public ObjectsPastedUndoAction(List<Entry> Objects)
		{
			Name = "Pasted " + Objects.Count + " object" + (Objects.Count > 1 ? "s" : "");
			objs = Objects;
		}

		public override void Redo()
		{
			foreach (Entry item in objs)
			{
				if (item is ObjectEntry)
				{
					LevelData.Objects.Add((ObjectEntry)item);
				}
				else if (item is RingEntry)
				{
					LevelData.Rings.Add((RingEntry)item);
				}
				else if (item is CNZBumperEntry)
				{
					LevelData.Bumpers.Add((CNZBumperEntry)item);
				}
			}
		}

		public override void Undo()
		{
			foreach (Entry item in objs)
			{
				if (item is ObjectEntry)
				{
					LevelData.Objects.Remove((ObjectEntry)item);
				}
				else if (item is RingEntry)
				{
					LevelData.Rings.Remove((RingEntry)item);
				}
				else if (item is CNZBumperEntry)
				{
					LevelData.Bumpers.Remove((CNZBumperEntry)item);
				}
			}
		}
	}
}
