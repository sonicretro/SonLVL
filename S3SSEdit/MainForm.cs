using SonicRetro.SonLVL.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;

namespace S3SSEdit
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		const int gridsize = 28;
		Bitmap[] startbmps32 = new Bitmap[4];
		List<LayoutSection> layoutSections = new List<LayoutSection>();
		List<Bitmap> layoutSectionImages = new List<Bitmap>();
		Settings settings = new Settings();
		LayoutData layout = new SSLayoutData();
		ProjectFile project = null;
		LayoutMode layoutmode = LayoutMode.S3;
		int stagenum = 0;
		string stgname = "New Stage";
		string filename = null;
		int gridpalnum = 0;
		StageSelectDialog stageseldlg;
		SphereType fgsphere = SphereType.Blue;
		SphereType bgsphere = SphereType.Empty;
		Graphics layoutgfx;
		ImageAttributes imageTransparency = new ImageAttributes();
		Tool tool = Tool.Pencil;
		ShapeMode rectmode = ShapeMode.Edge;
		ShapeMode diammode = ShapeMode.Edge;
		ShapeMode ovalmode = ShapeMode.Edge;
		Stack<Action> undoList = new Stack<Action>();
		Stack<Action> redoList = new Stack<Action>();
		int lastSaveUndoCount = 0;
		bool drawing = false;
		Point firstloc, prevloc;
		Rectangle selection;
		List<SphereLoc> drawlist;
		SphereType?[,] drawrect;
		SphereType[,] fillrect;
		Point startloc = new Point(15, 15);

		private void MainForm_Load(object sender, EventArgs e)
		{
			imageTransparency.SetColorMatrix(new ColorMatrix() { Matrix33 = 0.75f }, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
			LayoutDrawer.Init();
			for (int i = 0; i < LayoutDrawer.StartBmps.Length; i++)
				startbmps32[i] = LayoutDrawer.StartBmps[i].ToBitmap(LayoutDrawer.Palette).To32bpp();
			paletteRed.Image = LayoutDrawer.SphereBmps[SphereType.Red].ToBitmap(LayoutDrawer.Palette).To32bpp();
			foreSpherePicture.Image = paletteBlue.Image = LayoutDrawer.SphereBmps[SphereType.Blue].ToBitmap(LayoutDrawer.Palette).To32bpp();
			paletteBumper.Image = LayoutDrawer.SphereBmps[SphereType.Bumper].ToBitmap(LayoutDrawer.Palette).To32bpp();
			paletteRing.Image = LayoutDrawer.SphereBmps[SphereType.Ring].ToBitmap(LayoutDrawer.Palette).To32bpp();
			paletteYellow.Image = LayoutDrawer.SphereBmps[SphereType.Yellow].ToBitmap(LayoutDrawer.Palette).To32bpp();
			LayoutDrawer.Palette.Entries[0] = LayoutDrawer.Palette.Entries[1] = SystemColors.Control;
			if (File.Exists("LayoutSections.sls"))
				layoutSections = DeserializeCompressed<List<LayoutSection>>("LayoutSections.sls");
			layoutSectionListBox.Items.Clear();
			layoutSectionListBox.BeginUpdate();
			foreach (LayoutSection sec in layoutSections)
			{
				layoutSectionListBox.Items.Add(sec.Name);
				layoutSectionImages.Add(MakeLayoutSectionImage(sec));
			}
			layoutSectionListBox.EndUpdate();
			if (File.Exists("S3SSEdit.ini"))
				settings = Settings.Load("S3SSEdit.ini");
			if (settings.RecentFiles.Count > 0)
			{
				List<string> mru = new List<string>();
				foreach (string item in settings.RecentFiles)
					if (File.Exists(item))
					{
						mru.Add(item);
						recentFilesToolStripMenuItem.DropDownItems.Add(item.Replace("&", "&&"));
					}
				settings.RecentFiles = mru;
				recentFilesToolStripMenuItem.Enabled = mru.Count > 0;
			}
			saveUndoHistoryToolStripMenuItem.Checked = settings.SaveUndoHistory;
			showGridToolStripMenuItem.Checked = settings.ShowGrid;
			if (showGridToolStripMenuItem.Checked)
			{
				SonLVLColor[] tmp = SonLVLColor.Load(Properties.Resources.gridpal, 0, 2, EngineVersion.S3K);
				LayoutDrawer.Palette.Entries[0] = tmp[0].RGBColor;
				LayoutDrawer.Palette.Entries[1] = tmp[1].RGBColor;
			}
			layoutgfx = layoutPanel.CreateGraphics();
			layoutgfx.SetOptions();
		}

		private static T DeserializeCompressed<T>(string fn)
		{
			using (FileStream fs = File.OpenRead(fn))
			using (DeflateStream ds = new DeflateStream(fs, CompressionMode.Decompress))
				return (T)new BinaryFormatter().Deserialize(ds);
		}

		private Bitmap MakeLayoutSectionImage(LayoutSection section)
		{
			LayoutDrawer.Palette.Entries[0] = LayoutDrawer.Palette.Entries[1] = SystemColors.Control;
			Bitmap result = LayoutDrawer.DrawLayout(section.Spheres, 24).ToBitmap(LayoutDrawer.Palette).To32bpp();
			if (showGridToolStripMenuItem.Checked)
			{
				SonLVLColor[] tmp = SonLVLColor.Load(Properties.Resources.gridpal, gridpalnum * 4, 2, EngineVersion.S3K);
				LayoutDrawer.Palette.Entries[0] = tmp[0].RGBColor;
				LayoutDrawer.Palette.Entries[1] = tmp[1].RGBColor;
			}
			return result;
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (undoList.Count != lastSaveUndoCount)
				switch (MessageBox.Show(this, "Do you want to save before exiting?", "S3SSEdit", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning))
				{
					case DialogResult.Cancel:
						e.Cancel = true;
						return;
					case DialogResult.Yes:
						saveToolStripMenuItem_Click(this, EventArgs.Empty);
						break;
				}
			settings.SaveUndoHistory = saveUndoHistoryToolStripMenuItem.Checked;
			settings.ShowGrid = showGridToolStripMenuItem.Checked;
			settings.Save("S3SSEdit.ini");
		}

		private void UpdateText()
		{
			StringBuilder sb = new StringBuilder("S3SSEdit - ");
			sb.Append(stgname);
			if (undoList.Count != lastSaveUndoCount)
				sb.Append(" *");
			Text = sb.ToString();
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(this, $"Unload the current {(project != null ? "project" : "stage")} and start a new {(project != null ? "stage" : "one")}?", "S3SSEdit", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.OK)
			{
				layout = new SSLayoutData();
				filename = null;
				stgname = "New Stage";
				gridpalnum = 0;
				LoadStage();
			}
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (undoList.Count != lastSaveUndoCount)
				switch (MessageBox.Show(this, "Do you want to save the current file?", "S3SSEdit", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning))
				{
					case DialogResult.Cancel:
						return;
					case DialogResult.Yes:
						saveToolStripMenuItem_Click(this, EventArgs.Empty);
						break;
				}
			using (OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "bin", Filter = "All Supported Files|*.bin;*.ini|Binary Files|*.bin|Project Files|*.ini|All Files|*.*" })
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					filename = dlg.FileName;
					LoadFile();
				}
		}

		private void LoadFile()
		{
			if (Path.GetExtension(filename).Equals(".ini", StringComparison.OrdinalIgnoreCase))
			{
				project = ProjectFile.Load(filename);
				changeStageToolStripMenuItem.Enabled = true;
				if (stageseldlg != null)
					stageseldlg.Dispose();
				stageseldlg = new StageSelectDialog(filename, project);
				if (!SelectProjectStage())
				{
					layout = new SSLayoutData();
					filename = null;
					stgname = "New Stage";
					gridpalnum = 0;
					LoadStage();
				}
				else
					AddRecentFile();
			}
			else
			{
				layout = new SSLayoutData(File.ReadAllBytes(filename));
				stgname = Path.GetFileNameWithoutExtension(filename);
				if (stgname[stgname.Length - 1] >= '1' && stgname[stgname.Length - 1] <= '8')
					gridpalnum = stgname[stgname.Length - 1] - '1';
				AddRecentFile();
				LoadStage();
			}
		}

		private void AddRecentFile()
		{
			if (settings.RecentFiles.Contains(filename))
			{
				recentFilesToolStripMenuItem.DropDownItems.RemoveAt(settings.RecentFiles.IndexOf(filename));
				settings.RecentFiles.Remove(filename);
			}
			settings.RecentFiles.Insert(0, filename);
			recentFilesToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem(filename.Replace("&", "&&")));
			while (settings.RecentFiles.Count > 10)
			{
				settings.RecentFiles.RemoveAt(10);
				recentFilesToolStripMenuItem.DropDownItems.RemoveAt(10);
			}
			recentFilesToolStripMenuItem.Enabled = true;
		}

		private bool SelectProjectStage()
		{
			Color[] tmpcol = { LayoutDrawer.Palette.Entries[0], LayoutDrawer.Palette.Entries[1] };
			LayoutDrawer.Palette.Entries[0] = LayoutDrawer.Palette.Entries[1] = SystemColors.Control;
			bool cancel = stageseldlg.ShowDialog(this) != DialogResult.OK;
			LayoutDrawer.Palette.Entries[0] = tmpcol[0];
			LayoutDrawer.Palette.Entries[1] = tmpcol[1];
			if (cancel)
				return false;
			string path = Path.GetDirectoryName(filename);
			layoutmode = stageseldlg.Category;
			stagenum = stageseldlg.StageNumber;
			switch (layoutmode)
			{
				case LayoutMode.S3:
					layout = new SSLayoutData(File.ReadAllBytes(Path.Combine(path, project.S3Stages[stagenum])));
					stgname = $"Sonic 3 Stage {stagenum + 1}";
					gridpalnum = stagenum & 7;
					startButton.Visible = true;
					bsChunkOptionsPanel.Visible = false;
					bsStageOptionsPanel.Visible = false;
					stageLayoutOptionsPanel.Visible = true;
					break;
				case LayoutMode.SK:
					layout = new SSLayoutData(Compression.Decompress(Path.Combine(path, project.SKStageSet), CompressionType.Kosinski), stagenum * SSLayoutData.Size);
					stgname = $"Sonic & Knuckles Stage {stagenum + 1}";
					gridpalnum = (stagenum & 7) + 8;
					startButton.Visible = true;
					bsChunkOptionsPanel.Visible = false;
					bsStageOptionsPanel.Visible = false;
					stageLayoutOptionsPanel.Visible = true;
					break;
				case LayoutMode.BSChunk:
					layout = new BSChunkLayoutData(Compression.Decompress(Path.Combine(path, project.BlueSphereChunkSet), CompressionType.Kosinski), stagenum);
					stgname = $"Blue Sphere Chunk {stagenum}";
					gridpalnum = stagenum & 15;
					startButton.Visible = false;
					if (startButton.Checked)
						pencilButton.Checked = true;
					stageLayoutOptionsPanel.Visible = false;
					bsStageOptionsPanel.Visible = false;
					bsChunkOptionsPanel.Visible = true;
					break;
				case LayoutMode.BSLayout:
					layout = new BSStageLayoutData(Compression.Decompress(Path.Combine(path, project.BlueSphereChunkSet), CompressionType.Kosinski), stageseldlg.BSChunks);
					stgname = $"Blue Sphere {stageseldlg.BSMode} {stagenum}";
					gridpalnum = stageseldlg.BSChunks[2] & 15;
					startButton.Visible = false;
					if (startButton.Checked)
						pencilButton.Checked = true;
					stageLayoutOptionsPanel.Visible = false;
					bsChunkOptionsPanel.Visible = false;
					bsStageOptionsPanel.Visible = true;
					bsStageSection.SelectedIndex = 0;
					break;
			}
			if (showGridToolStripMenuItem.Checked)
			{
				SonLVLColor[] tmp = SonLVLColor.Load(Properties.Resources.gridpal, gridpalnum * 4, 2, EngineVersion.S3K);
				LayoutDrawer.Palette.Entries[0] = tmp[0].RGBColor;
				LayoutDrawer.Palette.Entries[1] = tmp[1].RGBColor;
			}
			layoutPanel.Width = layoutPanel.Height = layout.Layout.Size * gridsize;
			layoutgfx = layoutPanel.CreateGraphics();
			layoutgfx.SetOptions();
			lastSaveUndoCount = 0;
			undoList.Clear();
			redoList.Clear();
			undoToolStripMenuItem.DropDownItems.Clear();
			undoToolStripMenuItem.Enabled = false;
			redoToolStripMenuItem.DropDownItems.Clear();
			redoToolStripMenuItem.Enabled = false;
			if (saveUndoHistoryToolStripMenuItem.Checked && File.Exists(Path.ChangeExtension(filename, ".undo")))
			{
				Dictionary<LayoutMode, Dictionary<int, Stack<Action>>> undodict = DeserializeCompressed<Dictionary<LayoutMode, Dictionary<int, Stack<Action>>>>(Path.ChangeExtension(filename, ".undo"));
				if (undodict.ContainsKey(layoutmode) && undodict[layoutmode].ContainsKey(stagenum))
				{
					undoList = undodict[layoutmode][stagenum];
					foreach (Action a in undoList)
						undoToolStripMenuItem.DropDownItems.Add(a.Name);
					lastSaveUndoCount = undoList.Count;
					if (undoList.Count > 0)
						undoToolStripMenuItem.Enabled = true;
				}
			}
			UpdateText();
			UpdateControls();
			DrawLayout();
			return true;
		}

		private void LoadStage()
		{
			changeStageToolStripMenuItem.Enabled = false;
			project = null;
			stagenum = 0;
			layoutmode = LayoutMode.S3;
			startButton.Visible = true;
			bsChunkOptionsPanel.Visible = false;
			bsStageOptionsPanel.Visible = false;
			stageLayoutOptionsPanel.Visible = true;
			if (showGridToolStripMenuItem.Checked)
			{
				SonLVLColor[] tmp = SonLVLColor.Load(Properties.Resources.gridpal, gridpalnum * 4, 2, EngineVersion.S3K);
				LayoutDrawer.Palette.Entries[0] = tmp[0].RGBColor;
				LayoutDrawer.Palette.Entries[1] = tmp[1].RGBColor;
			}
			layoutPanel.Width = layoutPanel.Height = layout.Layout.Size * gridsize;
			layoutgfx = layoutPanel.CreateGraphics();
			layoutgfx.SetOptions();
			lastSaveUndoCount = 0;
			undoList.Clear();
			redoList.Clear();
			undoToolStripMenuItem.DropDownItems.Clear();
			undoToolStripMenuItem.Enabled = false;
			redoToolStripMenuItem.DropDownItems.Clear();
			redoToolStripMenuItem.Enabled = false;
			if (saveUndoHistoryToolStripMenuItem.Checked && filename != null && File.Exists(Path.ChangeExtension(filename, ".undo")))
			{
				undoList = DeserializeCompressed<Stack<Action>>(Path.ChangeExtension(filename, ".undo"));
				foreach (Action a in undoList)
					undoToolStripMenuItem.DropDownItems.Add(a.Name);
				lastSaveUndoCount = undoList.Count;
				if (undoList.Count > 0)
					undoToolStripMenuItem.Enabled = true;
			}
			UpdateText();
			UpdateControls();
			DrawLayout();
		}

		private void UpdateControls()
		{
			switch (layout)
			{
				case SSLayoutData ss:
					startloc = new Point(ss.StartX / 0x100, ss.StartY / 0x100);
					stageLayoutPerfectCount.Value = ss.Perfect;
					break;
				case BSChunkLayoutData bsc:
					bsChunkPerfectCount.Value = bsc.Perfect;
					bsChunkDifficulty.Value = bsc.Difficulty;
					break;
				case BSStageLayoutData bss:
					if (bsStageSection.SelectedIndex == 0)
					{
						bsStagePerfectCount.Value = bss.Perfect;
						bsStageDifficulty.Value = bss.Difficulty;
					}
					else
					{
						bsStagePerfectCount.Value = bss.GetPerfect(bsStageSection.SelectedIndex - 1);
						bsStageDifficulty.Value = bss.GetDifficulty(bsStageSection.SelectedIndex - 1);
					}
					break;
			}
		}

		private void changeStageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SelectProjectStage();
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (filename == null)
				saveAsToolStripMenuItem_Click(sender, e);
			else
				SaveLayout();
		}

		private void SaveLayout()
		{
			if (project != null)
			{
				string path = Path.GetDirectoryName(filename);
				switch (layoutmode)
				{
					case LayoutMode.S3:
						File.WriteAllBytes(Path.Combine(path, project.S3Stages[stagenum]), ((SSLayoutData)layout).GetBytes());
						break;
					case LayoutMode.SK:
						{
							byte[] tmp = Compression.Decompress(Path.Combine(path, project.SKStageSet), CompressionType.Kosinski);
							((SSLayoutData)layout).GetBytes().CopyTo(tmp, stagenum * SSLayoutData.Size);
							Compression.Compress(tmp, Path.Combine(path, project.SKStageSet), CompressionType.Kosinski);
						}
						break;
					case LayoutMode.BSChunk:
						{
							byte[] tmp = Compression.Decompress(Path.Combine(path, project.BlueSphereChunkSet), CompressionType.Kosinski);
							((BSChunkLayoutData)layout).WriteBytes(tmp, stagenum);
							Compression.Compress(tmp, Path.Combine(path, project.BlueSphereChunkSet), CompressionType.Kosinski);
						}
						break;
					case LayoutMode.BSLayout:
						{
							byte[] tmp = Compression.Decompress(Path.Combine(path, project.BlueSphereChunkSet), CompressionType.Kosinski);
							((BSStageLayoutData)layout).WriteBytes(tmp);
							Compression.Compress(tmp, Path.Combine(path, project.BlueSphereChunkSet), CompressionType.Kosinski);
						}
						break;
				}
				if (saveUndoHistoryToolStripMenuItem.Checked)
				{
					string fn = Path.ChangeExtension(filename, ".undo");
					Dictionary<LayoutMode, Dictionary<int, Stack<Action>>> undodict = new Dictionary<LayoutMode, Dictionary<int, Stack<Action>>>();
					if (File.Exists(fn))
						undodict = DeserializeCompressed<Dictionary<LayoutMode, Dictionary<int, Stack<Action>>>>(fn);
					if (undoList.Count > 0)
					{
						if (!undodict.ContainsKey(layoutmode))
							undodict.Add(layoutmode, new Dictionary<int, Stack<Action>>());
						undodict[layoutmode][stagenum] = undoList;
					}
					else if (undodict.ContainsKey(layoutmode))
					{
						if (undodict[layoutmode].ContainsKey(stagenum))
							undodict[layoutmode].Remove(stagenum);
						if (undodict[layoutmode].Count == 0)
							undodict.Remove(layoutmode);
					}
					if (undodict.Count > 0)
						SerializeCompressed(fn, undodict);
					else if (File.Exists(fn))
						File.Delete(fn);
				}
			}
			else
			{
				File.WriteAllBytes(filename, ((SSLayoutData)layout).GetBytes());
				if (saveUndoHistoryToolStripMenuItem.Checked)
				{
					string fn = Path.ChangeExtension(filename, ".undo");
					if (undoList.Count > 0)
						SerializeCompressed(fn, undoList);
					else if (File.Exists(fn))
						File.Delete(fn);
				}
			}
			lastSaveUndoCount = undoList.Count;
			UpdateText();
		}

		private static void SerializeCompressed(string fn, object obj)
		{
			using (FileStream fs = File.Create(fn))
			using (DeflateStream ds = new DeflateStream(fs, CompressionMode.Compress))
				new BinaryFormatter().Serialize(ds, obj);
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (project != null)
				switch (MessageBox.Show(this, "Do you want to unload the current project?", "S3SSEdit", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2))
				{
					case DialogResult.Cancel:
						return;
					case DialogResult.Yes:
						project = null;
						stagenum = 0;
						layoutmode = LayoutMode.S3;
						break;
				}
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "bin", Filter = "Binary Files|*.bin|All Files|*.*", FileName = stgname + ".bin" })
				if (dlg.ShowDialog(this) == DialogResult.OK)
					if (project != null)
						File.WriteAllBytes(dlg.FileName, ((SSLayoutData)layout).GetBytes());
					else
					{
						filename = dlg.FileName;
						stgname = Path.GetFileNameWithoutExtension(filename);
						AddRecentFile();
						SaveLayout();
					}
		}

		private void exportImageToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog dlg = new SaveFileDialog() { DefaultExt = "png", Filter = "PNG Files|*.png" })
				if (dlg.ShowDialog(this) == DialogResult.OK)
					using (Bitmap bmp = LayoutDrawer.DrawLayout(layout, gridsize).ToBitmap(LayoutDrawer.Palette))
						bmp.Save(dlg.FileName);
		}

		private void recentFilesToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			filename = settings.RecentFiles[recentFilesToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem)];
			LoadFile();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void undoToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			DoUndo(undoToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem) + 1);
		}

		private void redoToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			DoRedo(redoToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem) + 1);
		}

		private void DoUndo(int count = 1)
		{
			if (drawing) return;
			for (int i = 0; i < count; i++)
			{
				undoToolStripMenuItem.DropDownItems.RemoveAt(0);
				Action act = undoList.Pop();
				act.Do(layout);
				redoList.Push(act);
				redoToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem(act.Name));
				redoToolStripMenuItem.Enabled = true;
			}
			if (undoList.Count == 0)
				undoToolStripMenuItem.Enabled = false;
			UpdateText();
			UpdateControls();
			DrawLayout();
		}

		private void DoRedo(int count = 1)
		{
			if (drawing) return;
			for (int i = 0; i < count; i++)
			{
				redoToolStripMenuItem.DropDownItems.RemoveAt(0);
				Action act = redoList.Pop();
				act.Do(layout);
				undoList.Push(act);
				undoToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem(act.Name));
				undoToolStripMenuItem.Enabled = true;
			}
			if (redoList.Count == 0)
				redoToolStripMenuItem.Enabled = false;
			UpdateText();
			UpdateControls();
			DrawLayout();
		}

		private void showGridToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
		{
			if (showGridToolStripMenuItem.Checked)
			{
				SonLVLColor[] tmp = SonLVLColor.Load(Properties.Resources.gridpal, gridpalnum * 4, 2, EngineVersion.S3K);
				LayoutDrawer.Palette.Entries[0] = tmp[0].RGBColor;
				LayoutDrawer.Palette.Entries[1] = tmp[1].RGBColor;
			}
			else
				LayoutDrawer.Palette.Entries[0] = LayoutDrawer.Palette.Entries[1] = SystemColors.Control;
			DrawLayout();
		}

		private void MainForm_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Y:
					if (e.Control && !drawing && redoList.Count > 0)
					{
						DoRedo();
						e.SuppressKeyPress = true;
					}

					break;
				case Keys.Z:
					if (e.Control && !drawing && undoList.Count > 0)
					{
						DoUndo();
						e.SuppressKeyPress = true;
					}

					break;
			}
		}

#region Sphere Palette
		// Also handles backSpherePicture.Click
		private void foreSpherePicture_Click(object sender, EventArgs e)
		{
			SphereType a = fgsphere;
			fgsphere = bgsphere;
			bgsphere = a;
			Image b = foreSpherePicture.Image;
			foreSpherePicture.Image = backSpherePicture.Image;
			backSpherePicture.Image = b;
			PictureBoxSizeMode c = foreSpherePicture.SizeMode;
			foreSpherePicture.SizeMode = backSpherePicture.SizeMode;
			backSpherePicture.SizeMode = c;
		}

		private void ChangeSelectedSphere(MouseEventArgs e, SphereType type, Image pic, PictureBoxSizeMode size)
		{
			switch (e.Button)
			{
				case MouseButtons.Left:
					fgsphere = type;
					foreSpherePicture.Image = pic;
					foreSpherePicture.SizeMode = size;
					break;
				case MouseButtons.Right:
					bgsphere = type;
					backSpherePicture.Image = pic;
					backSpherePicture.SizeMode = size;
					break;
			}
		}

		private void paletteErase_MouseClick(object sender, MouseEventArgs e)
		{
			ChangeSelectedSphere(e, SphereType.Empty, paletteErase.Image, PictureBoxSizeMode.CenterImage);
		}

		private void paletteRed_MouseClick(object sender, MouseEventArgs e)
		{
			ChangeSelectedSphere(e, SphereType.Red, paletteRed.Image, PictureBoxSizeMode.Zoom);
		}

		private void paletteBlue_MouseClick(object sender, MouseEventArgs e)
		{
			ChangeSelectedSphere(e, SphereType.Blue, paletteBlue.Image, PictureBoxSizeMode.Zoom);
		}

		private void paletteBumper_MouseClick(object sender, MouseEventArgs e)
		{
			ChangeSelectedSphere(e, SphereType.Bumper, paletteBumper.Image, PictureBoxSizeMode.Zoom);
		}

		private void paletteRing_MouseClick(object sender, MouseEventArgs e)
		{
			ChangeSelectedSphere(e, SphereType.Ring, paletteRing.Image, PictureBoxSizeMode.CenterImage);
		}

		private void paletteYellow_MouseClick(object sender, MouseEventArgs e)
		{
			ChangeSelectedSphere(e, SphereType.Yellow, paletteYellow.Image, PictureBoxSizeMode.Zoom);
		}
		#endregion

#region Tool Palette
		private void selectButton_CheckedChanged(object sender, EventArgs e)
		{
			if (selectButton.Checked)
			{
				tool = Tool.Select;
				selection = Rectangle.Empty;
				toolOptionsPanel.Invalidate();
			}
		}

		private void pencilButton_CheckedChanged(object sender, EventArgs e)
		{
			if (pencilButton.Checked)
			{
				tool = Tool.Pencil;
				toolOptionsPanel.Invalidate();
			}
		}

		private void fillButton_CheckedChanged(object sender, EventArgs e)
		{
			if (fillButton.Checked)
			{
				tool = Tool.Fill;
				toolOptionsPanel.Invalidate();
			}
		}

		private void lineButton_CheckedChanged(object sender, EventArgs e)
		{
			if (lineButton.Checked)
			{
				tool = Tool.Line;
				toolOptionsPanel.Invalidate();
			}
		}

		private void rectangleButton_CheckedChanged(object sender, EventArgs e)
		{
			if (rectangleButton.Checked)
			{
				tool = Tool.Rectangle;
				toolOptionsPanel.Invalidate();
			}
		}

		private void diamondButton_CheckedChanged(object sender, EventArgs e)
		{
			if (diamondButton.Checked)
			{
				tool = Tool.Diamond;
				toolOptionsPanel.Invalidate();
			}
		}

		private void ovalButton_CheckedChanged(object sender, EventArgs e)
		{
			if (ovalButton.Checked)
			{
				tool = Tool.Oval;
				toolOptionsPanel.Invalidate();
			}
		}

		private void startButton_CheckedChanged(object sender, EventArgs e)
		{
			if (startButton.Checked)
			{
				tool = Tool.Start;
				toolOptionsPanel.Invalidate();
			}
		}

		private void toolOptionsPanel_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.Clear(toolOptionsPanel.BackColor);
			ShapeMode mode;
			switch (tool)
			{
				case Tool.Rectangle:
					mode = rectmode;
					break;
				case Tool.Diamond:
					mode = diammode;
					break;
				case Tool.Oval:
					mode = ovalmode;
					break;
				default:
					return;
			}
			e.Graphics.FillRectangle(SystemBrushes.Highlight, 1, 2 + 20 * (int)mode, 36, 18);
			e.Graphics.DrawRectangle(mode == ShapeMode.Edge ? SystemPens.HighlightText : SystemPens.ControlText, 5, 6, 27, 9);
			e.Graphics.DrawRectangle(mode == ShapeMode.FillEdge ? SystemPens.HighlightText : SystemPens.ControlText, 5, 26, 27, 9);
			e.Graphics.FillRectangle(Brushes.DarkGray, 6, 27, 26, 8);
			e.Graphics.FillRectangle(Brushes.DarkGray, 5, 46, 28, 10);
		}

		private void toolOptionsPanel_MouseClick(object sender, MouseEventArgs e)
		{
			switch (tool)
			{
				case Tool.Rectangle:
					if (e.Y <= 20)
						rectmode = ShapeMode.Edge;
					else if (e.Y <= 40)
						rectmode = ShapeMode.FillEdge;
					else
						rectmode = ShapeMode.Fill;
					break;
				case Tool.Diamond:
					if (e.Y <= 20)
						diammode = ShapeMode.Edge;
					else if (e.Y <= 40)
						diammode = ShapeMode.FillEdge;
					else
						diammode = ShapeMode.Fill;
					break;
				case Tool.Oval:
					if (e.Y <= 20)
						ovalmode = ShapeMode.Edge;
					else if (e.Y <= 40)
						ovalmode = ShapeMode.FillEdge;
					else
						ovalmode = ShapeMode.Fill;
					break;
				default:
					return;
			}
			toolOptionsPanel.Invalidate();
		}
#endregion

		private void layoutPanel_Paint(object sender, PaintEventArgs e)
		{
			DrawLayout();
		}

		private void DrawLayout()
		{
			Point gridloc = layoutPanel.PointToClient(Cursor.Position);
			gridloc = new Point(gridloc.X / gridsize, gridloc.Y / gridsize);
			LayoutData tmplayout = layout.Clone();
			if (drawing)
				switch (tool)
				{
					case Tool.Pencil:
					case Tool.Line:
						foreach (SphereLoc loc in drawlist)
							tmplayout.Layout[loc.X, loc.Y] = loc.Sphere;
						break;
					case Tool.Rectangle:
						gridloc = new Point(Math.Min(gridloc.X, firstloc.X / gridsize), Math.Min(gridloc.Y, firstloc.Y / gridsize));
						switch (rectmode)
						{
							case ShapeMode.Edge:
								foreach (SphereLoc loc in drawlist)
									tmplayout.Layout[loc.X, loc.Y] = loc.Sphere;
								break;
							case ShapeMode.FillEdge:
							case ShapeMode.Fill:
								for (int y = 0; y < fillrect.GetLength(1); y++)
									for (int x = 0; x < fillrect.GetLength(0); x++)
										tmplayout.Layout[(x + gridloc.X) & (layout.Layout.Size - 1), (y + gridloc.Y) & (layout.Layout.Size - 1)] = fillrect[x, y];
								break;
						}
						break;
					case Tool.Diamond:
						gridloc = new Point(Math.Min(gridloc.X, firstloc.X / gridsize), Math.Min(gridloc.Y, firstloc.Y / gridsize));
						switch (diammode)
						{
							case ShapeMode.Edge:
								foreach (SphereLoc loc in drawlist)
									tmplayout.Layout[loc.X, loc.Y] = loc.Sphere;
								break;
							case ShapeMode.FillEdge:
							case ShapeMode.Fill:
								for (int y = 0; y < drawrect.GetLength(1); y++)
									for (int x = 0; x < drawrect.GetLength(0); x++)
										if (drawrect[x, y].HasValue)
											tmplayout.Layout[(x + gridloc.X) & (layout.Layout.Size - 1), (y + gridloc.Y) & (layout.Layout.Size - 1)] = drawrect[x, y].Value;
								break;
						}
						break;
					case Tool.Oval:
						gridloc = new Point(Math.Min(gridloc.X, firstloc.X / gridsize), Math.Min(gridloc.Y, firstloc.Y / gridsize));
						for (int y = 0; y < drawrect.GetLength(1); y++)
							for (int x = 0; x < drawrect.GetLength(0); x++)
								if (drawrect[x, y].HasValue)
									tmplayout.Layout[(x + gridloc.X) & (layout.Layout.Size - 1), (y + gridloc.Y) & (layout.Layout.Size - 1)] = drawrect[x, y].Value;
						break;
					case Tool.Start:
						SSLayoutData ss = (SSLayoutData)tmplayout;
						ss.StartX = (ushort)(startloc.X * 0x100);
						ss.StartY = (ushort)(startloc.Y * 0x100);
						break;
				}
			BitmapBits layoutbmp = LayoutDrawer.DrawLayout(tmplayout, gridsize);
			using (Bitmap bmp = layoutbmp.ToBitmap(LayoutDrawer.Palette).To32bpp())
			{
				Graphics gfx = Graphics.FromImage(bmp);
				if (tool == Tool.Select)
				{
					if (!selection.IsEmpty)
					{
						Rectangle selbnds = new Rectangle(selection.X * gridsize, selection.Y * gridsize, selection.Width * gridsize, selection.Height * gridsize);
						using (SolidBrush brush = new SolidBrush(Color.FromArgb(128, SystemColors.Highlight)))
							gfx.FillRectangle(brush, selbnds);
						selbnds.Width--; selbnds.Height--;
						using (Pen pen = new Pen(Color.FromArgb(128, Color.Black)) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot })
							gfx.DrawRectangle(pen, selbnds);
					}
				}
				else if (!drawing)
				{
					if (tool == Tool.Start)
						gfx.DrawImage(startbmps32[((SSLayoutData)layout).Angle >> 14], new Rectangle(gridloc.X * gridsize + 2, gridloc.Y * gridsize + 2, 24, 24), 0, 0, 24, 24, GraphicsUnit.Pixel, imageTransparency);
					else
					{
						if (fgsphere != SphereType.Empty)
							gfx.DrawImage(foreSpherePicture.Image, new Rectangle(gridloc.X * gridsize + 2, gridloc.Y * gridsize + 2, 24, 24), 0, 0, 24, 24, GraphicsUnit.Pixel, imageTransparency);
						if (fgsphere == SphereType.Yellow)
							using (SolidBrush brush = new SolidBrush(Color.FromArgb(128, Color.Yellow)))
							{
								gfx.FillRectangle(brush, gridloc.X * gridsize, ((gridloc.Y - 6) & (layout.Layout.Size - 1)) * gridsize, gridsize, gridsize);
								gfx.FillRectangle(brush, ((gridloc.X + 6) & (layout.Layout.Size - 1)) * gridsize, gridloc.Y * gridsize, gridsize, gridsize);
								gfx.FillRectangle(brush, gridloc.X * gridsize, ((gridloc.Y + 6) & (layout.Layout.Size - 1)) * gridsize, gridsize, gridsize);
								gfx.FillRectangle(brush, ((gridloc.X - 6) & (layout.Layout.Size - 1)) * gridsize, gridloc.Y * gridsize, gridsize, gridsize);
							}
					}
				}
				layoutgfx.DrawImage(bmp, 0, 0, layoutPanel.Width, layoutPanel.Height);
			}
		}

		private void DoAction(Action action)
		{
			if (redoList.Count > 0)
			{
				redoList.Clear();
				redoToolStripMenuItem.DropDownItems.Clear();
				redoToolStripMenuItem.Enabled = false;
				if (lastSaveUndoCount > undoList.Count)
					lastSaveUndoCount = -1;
			}
			undoList.Push(action);
			undoToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem(action.Name));
			undoToolStripMenuItem.Enabled = true;
			action.Do(layout);
			UpdateText();
		}

		private void layoutPanel_MouseDown(object sender, MouseEventArgs e)
		{
			Point loc = e.Location;
			Point gridloc = new Point(loc.X / gridsize, loc.Y / gridsize);
			if (tool == Tool.Select)
			{
				if (e.Button == MouseButtons.Left)
				{
					drawing = true;
					selection = new Rectangle(gridloc.X, gridloc.Y, 1, 1);
					firstloc = prevloc = loc;
					DrawLayout();
				}
				return;
			}
			drawing = true;
			SphereType sphere = SphereType.Empty;
			switch (e.Button)
			{
				case MouseButtons.Left:
					sphere = fgsphere;
					break;
				case MouseButtons.Right:
					sphere = bgsphere;
					break;
				default:
					return;
			}
			switch (tool)
			{
				case Tool.Pencil:
				case Tool.Line:
					drawlist = new List<SphereLoc>() { new SphereLoc(sphere, gridloc) };
					break;
				case Tool.Fill:
					{
						drawing = false;
						SphereType oldind = layout.Layout[gridloc.X, gridloc.Y];
						if (oldind == sphere) return;
						Queue<Point> pts = new Queue<Point>(layout.Layout.Size * layout.Layout.Size / 2);
						pts.Enqueue(gridloc);
						SphereType?[,] fillgrid = new SphereType?[layout.Layout.Size, layout.Layout.Size];
						fillgrid[gridloc.X, gridloc.Y] = sphere;
						while (pts.Count > 0)
						{
							Point pt = pts.Dequeue();
							int tmp = (pt.X + (layout.Layout.Size - 1)) % layout.Layout.Size;
							if (layout.Layout[tmp, pt.Y] == oldind && !fillgrid[tmp, pt.Y].HasValue)
							{
								fillgrid[tmp, pt.Y] = sphere;
								pts.Enqueue(new Point(tmp, pt.Y));
							}
							tmp = (pt.X + 1) % layout.Layout.Size;
							if (layout.Layout[tmp, pt.Y] == oldind && !fillgrid[tmp, pt.Y].HasValue)
							{
								fillgrid[tmp, pt.Y] = sphere;
								pts.Enqueue(new Point(tmp, pt.Y));
							}
							tmp = (pt.Y + (layout.Layout.Size - 1)) % layout.Layout.Size;
							if (layout.Layout[pt.X, tmp] == oldind && !fillgrid[pt.X, tmp].HasValue)
							{
								fillgrid[pt.X, tmp] = sphere;
								pts.Enqueue(new Point(pt.X, tmp));
							}
							tmp = (pt.Y + 1) % layout.Layout.Size;
							if (layout.Layout[pt.X, tmp] == oldind && !fillgrid[pt.X, tmp].HasValue)
							{
								fillgrid[pt.X, tmp] = sphere;
								pts.Enqueue(new Point(pt.X, tmp));
							}
						}
						DoAction(new FillAction(fillgrid, layout.Layout.Size));
					}
					break;
				case Tool.Rectangle:
					switch (rectmode)
					{
						case ShapeMode.Edge:
							drawlist = new List<SphereLoc>() { new SphereLoc(sphere, gridloc) };
							break;
						case ShapeMode.FillEdge:
						case ShapeMode.Fill:
							fillrect = new SphereType[1, 1] { { sphere } };
							break;
					}
					break;
				case Tool.Diamond:
					switch (diammode)
					{
						case ShapeMode.Edge:
							drawlist = new List<SphereLoc>() { new SphereLoc(sphere, gridloc) };
							break;
						case ShapeMode.FillEdge:
						case ShapeMode.Fill:
							drawrect = new SphereType?[1, 1] { { sphere } };
							break;
					}
					break;
				case Tool.Oval:
					drawrect = new SphereType?[1, 1] { { sphere } };
					break;
				case Tool.Start:
					startloc = gridloc;
					break;
			}
			firstloc = prevloc = loc;
			DrawLayout();
		}

		private void DrawLine(SphereType sphere, int x1, int y1, int x2, int y2)
		{
			if (y1 == y2)
			{
				if (x1 > x2)
				{
					int tmp = x1;
					x1 = x2;
					x2 = tmp;
				}
				for (int x = x1; x <= x2; x++)
				{
					SphereLoc s = new SphereLoc(sphere, x & (layout.Layout.Size - 1), y1 & (layout.Layout.Size - 1));
					if (!drawlist.Contains(s))
						drawlist.Add(s);
				}
			}
			else if (x1 == x2)
			{
				if (y1 > y2)
				{
					int tmp = y1;
					y1 = y2;
					y2 = tmp;
				}
				for (int y = y1; y <= y2; y++)
				{
					SphereLoc s = new SphereLoc(sphere, x1 & (layout.Layout.Size - 1), y & (layout.Layout.Size - 1));
					if (!drawlist.Contains(s))
						drawlist.Add(s);
				}
			}
			else
			{
				bool steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);
				if (steep)
				{
					int tmp = x1;
					x1 = y1;
					y1 = tmp;
					tmp = x2;
					x2 = y2;
					y2 = tmp;
				}
				if (x1 > x2)
				{
					int tmp = x1;
					x1 = x2;
					x2 = tmp;
					tmp = y1;
					y1 = y2;
					y2 = tmp;
				}
				int deltax = x2 - x1;
				int deltay = Math.Abs(y2 - y1);
				double error = 0;
				double deltaerr = deltay / (double)deltax;
				int ystep;
				int y = y1;
				if (y1 < y2)
					ystep = 1;
				else
					ystep = -1;
				for (int x = x1; x <= x2; x++)
				{
					if (steep)
					{
						SphereLoc s = new SphereLoc(sphere, y & (layout.Layout.Size - 1), x & (layout.Layout.Size - 1));
						if (!drawlist.Contains(s))
							drawlist.Add(s);
					}
					else
					{
						SphereLoc s = new SphereLoc(sphere, x & (layout.Layout.Size - 1), y & (layout.Layout.Size - 1));
						if (!drawlist.Contains(s))
							drawlist.Add(s);
					}
					error += deltaerr;
					if (error >= 0.5)
					{
						y += ystep;
						error -= 1.0;
					}
				}
			}
		}

		private void DrawLine(SphereType?[,] rect, SphereType sphere, int x1, int y1, int x2, int y2)
		{
			if (y1 == y2)
			{
				if (x1 > x2)
				{
					int tmp = x1;
					x1 = x2;
					x2 = tmp;
				}
				for (int x = x1; x <= x2; x++)
					rect[x, y1] = sphere;
			}
			else if (x1 == x2)
			{
				if (y1 > y2)
				{
					int tmp = y1;
					y1 = y2;
					y2 = tmp;
				}
				for (int y = y1; y <= y2; y++)
					rect[x1, y] = sphere;
			}
			else
			{
				bool steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);
				if (steep)
				{
					int tmp = x1;
					x1 = y1;
					y1 = tmp;
					tmp = x2;
					x2 = y2;
					y2 = tmp;
				}
				if (x1 > x2)
				{
					int tmp = x1;
					x1 = x2;
					x2 = tmp;
					tmp = y1;
					y1 = y2;
					y2 = tmp;
				}
				int deltax = x2 - x1;
				int deltay = Math.Abs(y2 - y1);
				double error = 0;
				double deltaerr = deltay / (double)deltax;
				int ystep;
				int y = y1;
				if (y1 < y2)
					ystep = 1;
				else
					ystep = -1;
				for (int x = x1; x <= x2; x++)
				{
					if (steep)
						rect[y, x] = sphere;
					else
						rect[x, y] = sphere;
					error += deltaerr;
					if (error >= 0.5)
					{
						y += ystep;
						error -= 1.0;
					}
				}
			}
		}

		private void layoutPanel_MouseMove(object sender, MouseEventArgs e)
		{
			Point loc = e.Location;
			if (!drawing)
			{
				DrawLayout();
				prevloc = loc;
				return;
			}
			Point gridloc = new Point(loc.X / gridsize, loc.Y / gridsize);
			if (gridloc == new Point(prevloc.X / gridsize, prevloc.Y / gridsize))
				return;
			SphereType sphere = SphereType.Empty;
			SphereType bgsphere = SphereType.Empty;
			switch (e.Button)
			{
				case MouseButtons.Left:
					sphere = fgsphere;
					bgsphere = this.bgsphere;
					break;
				case MouseButtons.Right:
					sphere = this.bgsphere;
					bgsphere = fgsphere;
					break;
			}
			switch (tool)
			{
				case Tool.Select:
					selection = new Rectangle(
						Math.Min(gridloc.X, firstloc.X / gridsize),
						Math.Min(gridloc.Y, firstloc.Y / gridsize),
						Math.Abs(gridloc.X - firstloc.X / gridsize) + 1,
						Math.Abs(gridloc.Y - firstloc.Y / gridsize) + 1);
					break;
				case Tool.Pencil:
					{
						int x1 = prevloc.X;
						int y1 = prevloc.Y;
						int x2 = loc.X;
						int y2 = loc.Y;
						if (y1 == y2)
						{
							if (y1 >= layout.Layout.Size * gridsize || y1 < 0)
								return;
							if (x1 > x2)
							{
								int tmp = x1;
								x1 = x2;
								x2 = tmp;
							}
							if (x1 >= layout.Layout.Size * gridsize || x2 < 0)
								return;
							x1 = Math.Max(x1, 0);
							x2 = Math.Min(x2, layout.Layout.Size * gridsize - 1);
							for (int x = x1; x <= x2; x++)
							{
								SphereLoc s = new SphereLoc(sphere, x / gridsize, y1 / gridsize);
								if (!drawlist.Contains(s))
									drawlist.Add(s);
							}
						}
						else if (x1 == x2)
						{
							if (x1 >= layout.Layout.Size * gridsize || x1 < 0)
								return;
							if (y1 > y2)
							{
								int tmp = y1;
								y1 = y2;
								y2 = tmp;
							}
							if (y1 >= layout.Layout.Size * gridsize || y2 < 0)
								return;
							y1 = Math.Max(y1, 0);
							y2 = Math.Min(y2, layout.Layout.Size * gridsize - 1);
							for (int y = y1; y <= y2; y++)
							{
								SphereLoc s = new SphereLoc(sphere, x1 / gridsize, y / gridsize);
								if (!drawlist.Contains(s))
									drawlist.Add(s);
							}
						}
						else
						{
							bool steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);
							if (steep)
							{
								int tmp = x1;
								x1 = y1;
								y1 = tmp;
								tmp = x2;
								x2 = y2;
								y2 = tmp;
							}
							if (x1 > x2)
							{
								int tmp = x1;
								x1 = x2;
								x2 = tmp;
								tmp = y1;
								y1 = y2;
								y2 = tmp;
							}
							int deltax = x2 - x1;
							int deltay = Math.Abs(y2 - y1);
							double error = 0;
							double deltaerr = deltay / (double)deltax;
							int ystep;
							int y = y1;
							if (y1 < y2)
								ystep = 1;
							else
								ystep = -1;
							for (int x = x1; x <= x2; x++)
							{
								if (steep)
								{
									if (x >= 0 && x < layout.Layout.Size * gridsize && y >= 0 && y < layout.Layout.Size * gridsize)
									{
										SphereLoc s = new SphereLoc(sphere, y / gridsize, x / gridsize);
										if (!drawlist.Contains(s))
											drawlist.Add(s);
									}
								}
								else
								{
									if (y >= 0 && y < layout.Layout.Size * gridsize && x >= 0 && x < layout.Layout.Size * gridsize)
									{
										SphereLoc s = new SphereLoc(sphere, x / gridsize, y / gridsize);
										if (!drawlist.Contains(s))
											drawlist.Add(s);
									}
								}
								error += deltaerr;
								if (error >= 0.5)
								{
									y += ystep;
									error -= 1.0;
								}
							}
						}
					}
					break;
				case Tool.Fill:
					return;
				case Tool.Line:
					drawlist = new List<SphereLoc>();
					DrawLine(sphere, firstloc.X / gridsize, firstloc.Y / gridsize, gridloc.X, gridloc.Y);
					break;
				case Tool.Rectangle:
					{
						int width = Math.Abs(gridloc.X - firstloc.X / gridsize) + 1;
						int height = Math.Abs(gridloc.Y - firstloc.Y / gridsize) + 1;
						if (ModifierKeys == Keys.Control)
							height = width = Math.Max(width, height);
						switch (rectmode)
						{
							case ShapeMode.Edge:
								{
									drawlist = new List<SphereLoc>();
									int x = Math.Min(gridloc.X, firstloc.X / gridsize);
									int y = Math.Min(gridloc.Y, firstloc.Y / gridsize);
									DrawLine(sphere, x, y, x + width - 1, y);
									DrawLine(sphere, x, y, x, y + height - 1);
									DrawLine(sphere, x, y + height - 1, x + width - 1, y + height - 1);
									DrawLine(sphere, x + width - 1, y, x + width - 1, y + height - 1);
								}
								break;
							case ShapeMode.FillEdge:
								fillrect = new SphereType[width, height];
								if (sphere != SphereType.Empty)
								{
									for (int x = 0; x < width; x++)
									{
										fillrect[x, 0] = sphere;
										fillrect[x, height - 1] = sphere;
									}
									for (int y = 1; y < height - 1; y++)
									{
										fillrect[0, y] = sphere;
										fillrect[width - 1, y] = sphere;
									}
								}
								if (bgsphere != SphereType.Empty)
									for (int y = 1; y < height - 1; y++)
										for (int x = 1; x < width - 1; x++)
											fillrect[x, y] = bgsphere;
								break;
							case ShapeMode.Fill:
								fillrect = new SphereType[width, height];
								if (sphere != SphereType.Empty)
									for (int y = 0; y < height; y++)
										for (int x = 0; x < width; x++)
											fillrect[x, y] = sphere;
								break;
						}
					}
					break;
				case Tool.Diamond:
					{
						int width = Math.Abs(gridloc.X - firstloc.X / gridsize) + 1;
						int height = Math.Abs(gridloc.Y - firstloc.Y / gridsize) + 1;
						if (ModifierKeys == Keys.Control)
							height = width = Math.Max(width, height);
						switch (diammode)
						{
							case ShapeMode.Edge:
								{
									drawlist = new List<SphereLoc>();
									int x = Math.Min(gridloc.X, firstloc.X / gridsize);
									int y = Math.Min(gridloc.Y, firstloc.Y / gridsize);
									DrawLine(sphere, x + width / 2, y, x + width - 1, y + height / 2);
									DrawLine(sphere, x + width - 1, y + height / 2, x + width / 2, y + height - 1);
									DrawLine(sphere, x + width / 2, y + height - 1, x, y + height / 2);
									DrawLine(sphere, x, y + height / 2, x + width / 2, y);
								}
								break;
							case ShapeMode.FillEdge:
								drawrect = new SphereType?[width, height];
								DrawLine(drawrect, sphere, width / 2, 0, width - 1, height / 2);
								DrawLine(drawrect, sphere, width - 1, height / 2, width / 2, height - 1);
								DrawLine(drawrect, sphere, width / 2, height - 1, 0, height / 2);
								DrawLine(drawrect, sphere, 0, height / 2, width / 2, 0);
								for (int y = 0; y < height; y++)
								{
									int minX = int.MaxValue;
									int maxX = int.MinValue;
									for (int x = 0; x < width; x++)
										if (drawrect[x, y].HasValue)
										{
											minX = Math.Min(minX, x);
											maxX = Math.Max(maxX, x);
										}
									for (int x = minX + 1; x < maxX; x++)
										if (!drawrect[x, y].HasValue)
											drawrect[x, y] = bgsphere;
								}
								break;
							case ShapeMode.Fill:
								drawrect = new SphereType?[width, height];
								DrawLine(drawrect, sphere, width / 2, 0, width - 1, height / 2);
								DrawLine(drawrect, sphere, width - 1, height / 2, width / 2, height - 1);
								DrawLine(drawrect, sphere, width / 2, height - 1, 0, height / 2);
								DrawLine(drawrect, sphere, 0, height / 2, width / 2, 0);
								for (int y = 0; y < height; y++)
								{
									int minX = int.MaxValue;
									int maxX = int.MinValue;
									for (int x = 0; x < width; x++)
										if (drawrect[x, y].HasValue)
										{
											minX = Math.Min(minX, x);
											maxX = Math.Max(maxX, x);
										}
									for (int x = minX + 1; x < maxX; x++)
										drawrect[x, y] = sphere;
								}
								break;
						}
					}
					break;
				case Tool.Oval:
					{
						int width = Math.Abs(gridloc.X - firstloc.X / gridsize) + 1;
						int height = Math.Abs(gridloc.Y - firstloc.Y / gridsize) + 1;
						if (ModifierKeys == Keys.Control)
							height = width = Math.Max(width, height);
						drawrect = new SphereType?[width, height];
						for (double a = 0; a < 2 * Math.PI; a += 0.04)
						{
							int x = (int)(Math.Cos(a) * (width / 2.01) + (width / 2.0));
							int y = (int)(Math.Sin(a) * (height / 2.01) + (height / 2.0));
							drawrect[x, y] = sphere;
						}
						switch (ovalmode)
						{
							case ShapeMode.FillEdge:
								for (int y = 0; y < height; y++)
								{
									int minX = int.MaxValue;
									int maxX = int.MinValue;
									for (int x = 0; x < width; x++)
										if (drawrect[x, y].HasValue)
										{
											minX = Math.Min(minX, x);
											maxX = Math.Max(maxX, x);
										}
									for (int x = minX + 1; x < maxX; x++)
										if (!drawrect[x, y].HasValue)
											drawrect[x, y] = bgsphere;
								}
								break;
							case ShapeMode.Fill:
								for (int y = 0; y < height; y++)
								{
									int minX = int.MaxValue;
									int maxX = int.MinValue;
									for (int x = 0; x < width; x++)
										if (drawrect[x, y].HasValue)
										{
											minX = Math.Min(minX, x);
											maxX = Math.Max(maxX, x);
										}
									for (int x = minX + 1; x < maxX; x++)
										drawrect[x, y] = sphere;
								}
								break;
						}
					}
					break;
				case Tool.Start:
					startloc = gridloc;
					break;
			}
			prevloc = loc;
			DrawLayout();
		}

		private void layoutPanel_MouseUp(object sender, MouseEventArgs e)
		{
			if (tool == Tool.Select && e.Button == MouseButtons.Right)
			{
				pasteOnceToolStripMenuItem.Enabled = pasteRepeatingToolStripMenuItem.Enabled = Clipboard.ContainsData(typeof(SphereType[,]).AssemblyQualifiedName);
				pasteSectionOnceToolStripMenuItem.Enabled = pasteSectionRepeatingToolStripMenuItem.Enabled = layoutSectionListBox.SelectedIndex != -1;
				rotateLeftToolStripMenuItem.Enabled = rotateRightToolStripMenuItem.Enabled = selection.Width == selection.Height;
				layoutContextMenuStrip.Show(layoutPanel, e.Location);
			}

			if (!drawing) return;
			drawing = false;
			Point gridloc = new Point(e.X / gridsize, e.Y / gridsize);
			switch (tool)
			{
				case Tool.Pencil:
					DoAction(new PencilAction(drawlist));
					break;
				case Tool.Fill:
					return;
				case Tool.Line:
					DoAction(new LineAction(drawlist));
					break;
				case Tool.Rectangle:
					switch (rectmode)
					{
						case ShapeMode.Edge:
							DoAction(new RectangleEdgeAction(drawlist));
							break;
						case ShapeMode.FillEdge:
						case ShapeMode.Fill:
							DoAction(new RectangleFillAction(fillrect, Math.Min(gridloc.X, firstloc.X / gridsize), Math.Min(gridloc.Y, firstloc.Y / gridsize)));
							break;
					}
					break;
				case Tool.Diamond:
					switch (diammode)
					{
						case ShapeMode.Edge:
							DoAction(new DiamondEdgeAction(drawlist));
							break;
						case ShapeMode.FillEdge:
						case ShapeMode.Fill:
							DoAction(new DiamondFillAction(drawrect, Math.Min(gridloc.X, firstloc.X / gridsize), Math.Min(gridloc.Y, firstloc.Y / gridsize)));
							break;
					}
					break;
				case Tool.Oval:
					DoAction(new OvalAction(drawrect, Math.Min(gridloc.X, firstloc.X / gridsize), Math.Min(gridloc.Y, firstloc.Y / gridsize)));
					break;
				case Tool.Start:
					if (gridloc.X == ((SSLayoutData)layout).StartX / 0x100 && gridloc.Y == ((SSLayoutData)layout).StartY / 0x100)
						DoAction(new StartAngleAction((ushort)(((SSLayoutData)layout).Angle + 0x4000)));
					else
						DoAction(new StartPositionAction(gridloc));
					break;
			}
			DrawLayout();
		}

		private void stageLayoutPerfectCount_ValueChanged(object sender, EventArgs e)
		{
			if (stageLayoutPerfectCount.Value != ((SSLayoutData)layout).Perfect)
				DoAction(new SSLayoutPerfectCountAction((short)stageLayoutPerfectCount.Value));
		}

		private void bsChunkPerfectCount_ValueChanged(object sender, EventArgs e)
		{
			if (bsChunkPerfectCount.Value != ((BSChunkLayoutData)layout).Perfect)
				DoAction(new BSChunkPerfectCountAction((byte)bsChunkPerfectCount.Value));
		}

		private void bsChunkDifficulty_ValueChanged(object sender, EventArgs e)
		{
			if (bsChunkDifficulty.Value != ((BSChunkLayoutData)layout).Difficulty)
				DoAction(new BSChunkDifficultyAction((byte)bsChunkDifficulty.Value));
		}

		private void bsStageSection_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (bsStageSection.SelectedIndex == 0)
			{
				bsStagePerfectCount.Enabled = false;
				bsStagePerfectCount.Maximum = 1020;
				bsStagePerfectCount.Value = ((BSStageLayoutData)layout).Perfect;
				bsStageDifficulty.Enabled = false;
				bsStageDifficulty.Maximum = 52;
				bsStageDifficulty.Value = ((BSStageLayoutData)layout).Difficulty;
			}
			else
			{
				bsStagePerfectCount.Enabled = true;
				bsStagePerfectCount.Value = ((BSStageLayoutData)layout).GetPerfect(bsStageSection.SelectedIndex - 1);
				bsStagePerfectCount.Maximum = 255;
				bsStageDifficulty.Enabled = true;
				bsStageDifficulty.Value = ((BSStageLayoutData)layout).GetDifficulty(bsStageSection.SelectedIndex - 1);
				bsStageDifficulty.Maximum = 13;
			}
		}

		private void bsStagePerfectCount_ValueChanged(object sender, EventArgs e)
		{
			if (bsStageSection.SelectedIndex != 0 && bsStagePerfectCount.Value != ((BSStageLayoutData)layout).GetPerfect(bsStageSection.SelectedIndex - 1))
				DoAction(new BSStagePerfectCountAction((byte)bsStagePerfectCount.Value, bsStageSection.SelectedIndex - 1));
		}

		private void bsStageDifficulty_ValueChanged(object sender, EventArgs e)
		{
			if (bsStageSection.SelectedIndex != 0 && bsStageDifficulty.Value != ((BSStageLayoutData)layout).GetDifficulty(bsStageSection.SelectedIndex - 1))
				DoAction(new BSStageDifficultyAction((byte)bsStageDifficulty.Value, bsStageSection.SelectedIndex - 1));
		}

		private void cutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Rectangle area = selection;
			if (area.IsEmpty)
				area = new Rectangle(0, 0, layout.Layout.Size, layout.Layout.Size);
			SphereType[,] sect = new SphereType[area.Width, area.Height];
			for (int y = 0; y < area.Height; y++)
				for (int x = 0; x < area.Width; x++)
					sect[x, y] = layout.Layout[area.X + x, area.Y + y];
			Clipboard.SetData(sect.GetType().AssemblyQualifiedName, sect);
			DoAction(new CutAction(new SphereType[area.Width, area.Height], area.Location));
			DrawLayout();
		}

		private void copyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Rectangle area = selection;
			if (area.IsEmpty)
				area = new Rectangle(0, 0, layout.Layout.Size, layout.Layout.Size);
			SphereType[,] sect = new SphereType[area.Width, area.Height];
			for (int y = 0; y < area.Height; y++)
				for (int x = 0; x < area.Width; x++)
					sect[x, y] = layout.Layout[area.X + x, area.Y + y];
			Clipboard.SetData(sect.GetType().AssemblyQualifiedName, sect);
		}

		private void pasteOnceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SphereType[,] data = (SphereType[,])Clipboard.GetData(typeof(SphereType[,]).AssemblyQualifiedName);
			DoAction(new PasteOnceAction(data, selection.Location));
			selection.Size = new Size(data.GetLength(0), data.GetLength(1));
			DrawLayout();
		}

		private void pasteRepeatingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Rectangle area = selection;
			if (area.IsEmpty)
				area = new Rectangle(0, 0, layout.Layout.Size, layout.Layout.Size);
			SphereType[,] sect = new SphereType[area.Width, area.Height];
			SphereType[,] copy = (SphereType[,])Clipboard.GetData(typeof(SphereType[,]).AssemblyQualifiedName);
			int copywidth = copy.GetLength(0);
			int copyheight = copy.GetLength(1);
			for (int y = 0; y < area.Height; y++)
				for (int x = 0; x < area.Width; x++)
					sect[x, y] = copy[x % copywidth, y % copyheight];
			DoAction(new PasteRepeatingAction(sect, selection.Location));
			DrawLayout();
		}

		private void importToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void insertTextToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (InsertTextDialog dlg = new InsertTextDialog(layout.Layout.Size, fgsphere, bgsphere))
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					DoAction(new TextAction(dlg.Section, selection.Location));
					DrawLayout();
				}
		}

		private void replaceFGToBGToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Rectangle area = selection;
			if (area.IsEmpty)
				area = new Rectangle(0, 0, layout.Layout.Size, layout.Layout.Size);
			SphereType?[,] sect = new SphereType?[area.Width, area.Height];
			bool found = false;
			for (int y = 0; y < area.Height; y++)
				for (int x = 0; x < area.Width; x++)
					if (layout.Layout[area.X + x, area.Y + y] == fgsphere)
					{
						sect[x, y] = bgsphere;
						found = true;
					}
			if (found)
			{
				DoAction(new ReplaceFGToBGAction(sect, selection.Location));
				DrawLayout();
			}
		}

		private void replaceBGToFGToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Rectangle area = selection;
			if (area.IsEmpty)
				area = new Rectangle(0, 0, layout.Layout.Size, layout.Layout.Size);
			SphereType?[,] sect = new SphereType?[area.Width, area.Height];
			bool found = false;
			for (int y = 0; y < area.Height; y++)
				for (int x = 0; x < area.Width; x++)
					if (layout.Layout[area.X + x, area.Y + y] == bgsphere)
					{
						sect[x, y] = fgsphere;
						found = true;
					}
			if (found)
			{
				DoAction(new ReplaceBGToFGAction(sect, selection.Location));
				DrawLayout();
			}
		}

		private void swapFGAndBGToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Rectangle area = selection;
			if (area.IsEmpty)
				area = new Rectangle(0, 0, layout.Layout.Size, layout.Layout.Size);
			SphereType?[,] sect = new SphereType?[area.Width, area.Height];
			bool found = false;
			for (int y = 0; y < area.Height; y++)
				for (int x = 0; x < area.Width; x++)
					if (layout.Layout[area.X + x, area.Y + y] == fgsphere)
					{
						sect[x, y] = bgsphere;
						found = true;
					}
					else if (layout.Layout[area.X + x, area.Y + y] == bgsphere)
					{
						sect[x, y] = fgsphere;
						found = true;
					}
			if (found)
			{
				DoAction(new SwapFGAndBGAction(sect, selection.Location));
				DrawLayout();
			}
		}

		private void flipHorizontallyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Rectangle area = selection;
			if (area.IsEmpty)
				area = new Rectangle(0, 0, layout.Layout.Size, layout.Layout.Size);
			switch (layout)
			{
				case BSStageLayoutData bss:
					SphereType[,] sect = new SphereType[area.Width, area.Height];
					for (int y = 0; y < area.Height; y++)
						for (int x = 0; x < area.Width; x++)
							sect[x, y] = layout.Layout[area.Right - x - 1, area.Y + y];
					DoAction(new BSFlipHorizontallyAction(sect, area.Location));
					break;
				default:
					DoAction(new FlipHorizontallyAction(area));
					break;
			}
			DrawLayout();
		}

		private void flipVerticallyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Rectangle area = selection;
			if (area.IsEmpty)
				area = new Rectangle(0, 0, layout.Layout.Size, layout.Layout.Size);
			switch (layout)
			{
				case BSStageLayoutData bss:
					SphereType[,] sect = new SphereType[area.Width, area.Height];
					for (int y = 0; y < area.Height; y++)
						for (int x = 0; x < area.Width; x++)
							sect[x, y] = layout.Layout[area.X + x, area.Bottom - y - 1];
					DoAction(new BSFlipVerticallyAction(sect, area.Location));
					break;
				default:
					DoAction(new FlipVerticallyAction(area));
					break;
			}
			DrawLayout();
		}

		private void rotateLeftToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Rectangle area = selection;
			if (area.IsEmpty)
				area = new Rectangle(0, 0, layout.Layout.Size, layout.Layout.Size);
			switch (layout)
			{
				case BSStageLayoutData bss:
					SphereType[,] sect = new SphereType[area.Width, area.Height];
					for (int y = 0; y < area.Height; y++)
						for (int x = 0; x < area.Width; x++)
							sect[x, y] = layout.Layout[area.X + area.Height - y - 1, area.Y + x];
					DoAction(new BSRotateLeftAction(sect, area.Location));
					break;
				default:
					DoAction(new RotateLeftAction(area));
					break;
			}
			DrawLayout();
		}

		private void rotateRightToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Rectangle area = selection;
			if (area.IsEmpty)
				area = new Rectangle(0, 0, layout.Layout.Size, layout.Layout.Size);
			switch (layout)
			{
				case BSStageLayoutData bss:
					SphereType[,] sect = new SphereType[area.Width, area.Height];
					for (int y = 0; y < area.Height; y++)
						for (int x = 0; x < area.Width; x++)
							sect[x, y] = layout.Layout[area.X + y, area.Y + area.Width - x - 1];
					DoAction(new BSRotateRightAction(sect, area.Location));
					break;
				default:
					DoAction(new RotateRightAction(area));
					break;
			}
			DrawLayout();
		}

		private void saveSectionToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (LayoutSectionNameDialog dlg = new LayoutSectionNameDialog())
			{
				dlg.Value = "Section " + (layoutSections.Count + 1);
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					Rectangle area = selection;
					if (area.IsEmpty)
						area = new Rectangle(0, 0, layout.Layout.Size, layout.Layout.Size);
					SphereType[,] sect = new SphereType[area.Width, area.Height];
					for (int y = 0; y < area.Height; y++)
						for (int x = 0; x < area.Width; x++)
							sect[x, y] = layout.Layout[area.X + x, area.Y + y];
					LayoutSection sec = new LayoutSection(dlg.Value, sect);
					layoutSections.Add(sec);
					layoutSectionImages.Add(MakeLayoutSectionImage(sec));
					layoutSectionListBox.Items.Add(sec.Name);
					layoutSectionListBox.SelectedIndex = layoutSections.Count - 1;
					SerializeCompressed("LayoutSections.sls", layoutSections);
				}
			}
		}

		private void pasteSectionOnceToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DoAction(new PasteOnceAction(layoutSections[layoutSectionListBox.SelectedIndex].Spheres, selection.Location));
			selection.Size = new Size(layoutSections[layoutSectionListBox.SelectedIndex].Spheres.GetLength(0), layoutSections[layoutSectionListBox.SelectedIndex].Spheres.GetLength(1));
			DrawLayout();
		}

		private void pasteSectionRepeatingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Rectangle area = selection;
			if (area.IsEmpty)
				area = new Rectangle(0, 0, layout.Layout.Size, layout.Layout.Size);
			SphereType[,] sect = new SphereType[area.Width, area.Height];
			SphereType[,] copy = layoutSections[layoutSectionListBox.SelectedIndex].Spheres;
			int copywidth = copy.GetLength(0);
			int copyheight = copy.GetLength(1);
			for (int y = 0; y < area.Height; y++)
				for (int x = 0; x < area.Width; x++)
					sect[x, y] = copy[x % copywidth, y % copyheight];
			DoAction(new PasteRepeatingAction(sect, selection.Location));
			DrawLayout();
		}

		private void layoutSectionListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (layoutSectionListBox.SelectedIndex == -1)
				layoutSectionPreview.Image = null;
			else
				layoutSectionPreview.Image = layoutSectionImages[layoutSectionListBox.SelectedIndex];
		}

		private void layoutSectionListBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (layoutSectionListBox.SelectedIndex != -1 && e.KeyCode == Keys.Delete
				&& MessageBox.Show(this, "Are you sure you want to delete layout section \"" + layoutSections[layoutSectionListBox.SelectedIndex].Name + "\"?", "S3SSEdit", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
			{
				layoutSections.RemoveAt(layoutSectionListBox.SelectedIndex);
				layoutSectionImages.RemoveAt(layoutSectionListBox.SelectedIndex);
				layoutSectionListBox.Items.RemoveAt(layoutSectionListBox.SelectedIndex);
				SerializeCompressed("LayoutSections.sls", layoutSections);
			}
		}
	}

	enum LayoutMode { S3, SK, BSChunk, BSLayout }

	enum BSMode { Level, Stage }

	enum Tool { Select, Pencil, Fill, Line, Rectangle, Diamond, Oval, Start }

	enum ShapeMode { Edge, FillEdge, Fill }

	public class Settings
	{
		[IniName("RecentFile")]
		[IniCollection(IniCollectionMode.NoSquareBrackets, StartIndex = 1)]
		public List<string> RecentFiles { get; set; } = new List<string>();
		[IniAlwaysInclude]
		[DefaultValue(true)]
		public bool SaveUndoHistory { get; set; } = true;
		[IniAlwaysInclude]
		[DefaultValue(true)]
		public bool ShowGrid { get; set; } = true;

		public static Settings Load(string filename)
		{
			return IniSerializer.Deserialize<Settings>(filename);
		}

		public void Save(string filename)
		{
			IniSerializer.Serialize(this, filename);
		}
	}

	[Serializable]
	public class LayoutSection
	{
		public string Name { get; set; }
		public SphereType[,] Spheres { get; set; }

		public LayoutSection() { }

		public LayoutSection(string name, SphereType[,] spheres)
		{
			Name = name;
			Spheres = spheres;
		}
	}

	[Serializable]
	class SphereLoc : IEquatable<SphereLoc>
	{
		public SphereType Sphere { get; set; }
		public int X { get; set; }
		public int Y { get; set; }

		public SphereLoc(SphereType sphere, int x, int y)
		{
			Sphere = sphere;
			X = x;
			Y = y;
		}

		public SphereLoc(SphereType sphere, Point position)
			: this(sphere, position.X, position.Y)
		{ }

		public bool Equals(SphereLoc other)
		{
			return Sphere == other.Sphere && X == other.X && Y == other.Y;
		}

		public override bool Equals(object obj)
		{
			return obj is SphereLoc && Equals((SphereLoc)obj);
		}

		public override int GetHashCode()
		{
			return Sphere.GetHashCode() ^ X.GetHashCode() ^ Y.GetHashCode();
		}

		public override string ToString()
		{
			return $"{Sphere}, {X}, {Y}";
		}
	}

	[Serializable]
	abstract class Action
	{
		public abstract string Name { get; }
		public abstract void Do(LayoutData layout);
	}

	[Serializable]
	abstract class SphereListAction : Action
	{
		public SphereListAction(List<SphereLoc> spheres)
		{
			this.spheres = spheres;
		}

		List<SphereLoc> spheres;

		public override void Do(LayoutData layout)
		{
			foreach (SphereLoc s in spheres)
			{
				SphereType sp = layout.Layout[s.X, s.Y];
				layout.Layout[s.X, s.Y] = s.Sphere;
				s.Sphere = sp;
			}
			spheres.Reverse();
		}
	}

	[Serializable]
	abstract class AreaFillAction : Action
	{
		public AreaFillAction(SphereType[,] spheres, Point position)
		{
			this.spheres = spheres;
			this.position = position;
		}

		public AreaFillAction(SphereType[,] spheres, int x, int y)
			: this(spheres, new Point(x, y))
		{ }

		SphereType[,] spheres;
		Point position;

		public override void Do(LayoutData layout)
		{
			SphereType[,] tmp = new SphereType[spheres.GetLength(0), spheres.GetLength(1)];
			for (int y = 0; y < spheres.GetLength(1); y++)
				for (int x = 0; x < spheres.GetLength(0); x++)
					tmp[x, y] = layout.Layout[(x + position.X) & (layout.Layout.Size - 1), (y + position.Y) & (layout.Layout.Size - 1)];
			for (int y = 0; y < spheres.GetLength(1); y++)
				for (int x = 0; x < spheres.GetLength(0); x++)
					layout.Layout[(x + position.X) & (layout.Layout.Size - 1), (y + position.Y) & (layout.Layout.Size - 1)] = spheres[x, y];
			spheres = tmp;
		}
	}

	[Serializable]
	abstract class AreaAction : Action
	{
		public AreaAction(SphereType?[,] spheres, int size)
		{
			int minX = size;
			int minY = size;
			int maxX = -1;
			int maxY = -1;
			for (int y = 0; y < size; y++)
				for (int x = 0; x < size; x++)
					if (spheres[x, y].HasValue)
					{
						minX = Math.Min(minX, x);
						minY = Math.Min(minY, y);
						maxX = Math.Max(maxX, x);
						maxY = Math.Max(maxY, y);
					}
			if (minX == 0 && minY == 0 && maxX == size - 1 && maxY == size - 1)
				this.spheres = spheres;
			else
			{
				this.spheres = new SphereType?[maxX - minX + 1, maxY - minY + 1];
				for (int y = minY; y <= maxY; y++)
					for (int x = minX; x <= maxX; x++)
						this.spheres[x - minX, y - minY] = spheres[x, y];
			}
			position = new Point(minX, minY);
		}

		public AreaAction(SphereType?[,] spheres, Point position)
		{
			this.spheres = spheres;
			this.position = position;
		}

		public AreaAction(SphereType?[,] spheres, int x, int y)
			: this(spheres, new Point(x, y))
		{ }

		SphereType?[,] spheres;
		Point position;

		public override void Do(LayoutData layout)
		{
			SphereType?[,] tmp = new SphereType?[spheres.GetLength(0), spheres.GetLength(1)];
			for (int y = 0; y < spheres.GetLength(1); y++)
				for (int x = 0; x < spheres.GetLength(0); x++)
					if (spheres[x, y].HasValue)
						tmp[x, y] = layout.Layout[(x + position.X) & (layout.Layout.Size - 1), (y + position.Y) & (layout.Layout.Size - 1)];
			for (int y = 0; y < spheres.GetLength(1); y++)
				for (int x = 0; x < spheres.GetLength(0); x++)
					if (spheres[x, y].HasValue)
						layout.Layout[(x + position.X) & (layout.Layout.Size - 1), (y + position.Y) & (layout.Layout.Size - 1)] = spheres[x, y].Value;
			spheres = tmp;
		}
	}

	[Serializable]
	abstract class RotateAction : Action
	{
		public RotateAction(Rectangle area, bool right)
		{
			this.area = area;
			this.right = right;
		}

		Rectangle area;
		bool right;

		public override void Do(LayoutData layout)
		{
			SphereType[,] copy = layout.Layout.ToArray();
			if (right)
			{
				for (int y = 0; y < area.Height; y++)
					for (int x = 0; x < area.Width; x++)
						layout.Layout[area.X + area.Height - y - 1, area.Y + x] = copy[area.X + x, area.Y + y];
			}
			else
			{
				for (int y = 0; y < area.Height; y++)
					for (int x = 0; x < area.Width; x++)
						layout.Layout[area.X + y, area.Y + area.Width - x - 1] = copy[area.X + x, area.Y + y];
			}
			right = !right;
		}
	}

	[Serializable]
	class PencilAction : SphereListAction
	{
		public PencilAction(List<SphereLoc> spheres) : base(spheres) { }

		public override string Name => "Draw";
	}

	[Serializable]
	class FillAction : AreaAction
	{
		public FillAction(SphereType?[,] spheres, int size) : base(spheres, size) { }

		public override string Name => "Fill";
	}

	[Serializable]
	class LineAction : SphereListAction
	{
		public LineAction(List<SphereLoc> spheres) : base(spheres) { }

		public override string Name => "Line";
	}

	[Serializable]
	class RectangleEdgeAction : SphereListAction
	{
		public RectangleEdgeAction(List<SphereLoc> spheres) : base(spheres) { }

		public override string Name => "Rectangle";
	}

	[Serializable]
	class RectangleFillAction : AreaFillAction
	{
		public RectangleFillAction(SphereType[,] spheres, Point position) : base(spheres, position) { }

		public RectangleFillAction(SphereType[,] spheres, int x, int y) : base(spheres, x, y) { }

		public override string Name => "Rectangle";
	}

	[Serializable]
	class DiamondEdgeAction : SphereListAction
	{
		public DiamondEdgeAction(List<SphereLoc> spheres) : base(spheres) { }

		public override string Name => "Diamond";
	}

	[Serializable]
	class DiamondFillAction : AreaAction
	{
		public DiamondFillAction(SphereType?[,] spheres, Point position) : base(spheres, position) { }

		public DiamondFillAction(SphereType?[,] spheres, int x, int y) : base(spheres, x, y) { }

		public override string Name => "Diamond";
	}

	[Serializable]
	class OvalAction : AreaAction
	{
		public OvalAction(SphereType?[,] spheres, Point position) : base(spheres, position) { }

		public OvalAction(SphereType?[,] spheres, int x, int y) : base(spheres, x, y) { }

		public override string Name => "Oval";
	}

	[Serializable]
	class StartPositionAction : Action
	{
		public StartPositionAction(Point position)
		{
			this.position = position;
		}

		Point position;

		public override void Do(LayoutData layout)
		{
			Point tmp = new Point(((SSLayoutData)layout).StartX / 0x100, ((SSLayoutData)layout).StartY / 0x100);
			((SSLayoutData)layout).StartX = (ushort)(position.X * 0x100);
			((SSLayoutData)layout).StartY = (ushort)(position.Y * 0x100);
			position = tmp;
		}

		public override string Name => "Start Position";
	}

	[Serializable]
	class StartAngleAction : Action
	{
		public StartAngleAction(ushort angle)
		{
			this.angle = angle;
		}

		ushort angle;

		public override void Do(LayoutData layout)
		{
			ushort tmp = ((SSLayoutData)layout).Angle;
			((SSLayoutData)layout).Angle = angle;
			angle = tmp;
		}

		public override string Name => "Start Angle";
	}

	[Serializable]
	class SSLayoutPerfectCountAction : Action
	{
		public SSLayoutPerfectCountAction(short count)
		{
			this.count = count;
		}

		short count;

		public override void Do(LayoutData layout)
		{
			short tmp = ((SSLayoutData)layout).Perfect;
			((SSLayoutData)layout).Perfect = count;
			count = tmp;
		}

		public override string Name => "Perfect Count";
	}

	[Serializable]
	class BSChunkPerfectCountAction : Action
	{
		public BSChunkPerfectCountAction(byte count)
		{
			this.count = count;
		}

		byte count;

		public override void Do(LayoutData layout)
		{
			byte tmp = ((BSChunkLayoutData)layout).Perfect;
			((BSChunkLayoutData)layout).Perfect = count;
			count = tmp;
		}

		public override string Name => "Perfect Count";
	}

	[Serializable]
	class BSChunkDifficultyAction : Action
	{
		public BSChunkDifficultyAction(byte diff)
		{
			this.diff = diff;
		}

		byte diff;

		public override void Do(LayoutData layout)
		{
			byte tmp = ((BSChunkLayoutData)layout).Difficulty;
			((BSChunkLayoutData)layout).Difficulty = diff;
			diff = tmp;
		}

		public override string Name => "Difficulty";
	}

	[Serializable]
	class BSStagePerfectCountAction : Action
	{
		public BSStagePerfectCountAction(byte count, int chunk)
		{
			this.count = count;
			this.chunk = chunk;
		}

		byte count;
		int chunk;

		public override void Do(LayoutData layout)
		{
			byte tmp = ((BSStageLayoutData)layout).GetPerfect(chunk);
			((BSStageLayoutData)layout).SetPerfect(chunk, count);
			count = tmp;
		}

		public override string Name => "Perfect Count";
	}

	[Serializable]
	class BSStageDifficultyAction : Action
	{
		public BSStageDifficultyAction(byte diff, int chunk)
		{
			this.diff = diff;
			this.chunk = chunk;
		}

		byte diff;
		int chunk;

		public override void Do(LayoutData layout)
		{
			byte tmp = ((BSStageLayoutData)layout).GetDifficulty(chunk);
			((BSStageLayoutData)layout).SetDifficulty(chunk, diff);
			diff = tmp;
		}

		public override string Name => "Difficulty";
	}

	[Serializable]
	class CutAction : AreaFillAction
	{
		public CutAction(SphereType[,] spheres, Point position) : base(spheres, position) { }

		public CutAction(SphereType[,] spheres, int x, int y) : base(spheres, x, y) { }

		public override string Name => "Cut";
	}

	[Serializable]
	class PasteOnceAction : AreaFillAction
	{
		public PasteOnceAction(SphereType[,] spheres, Point position) : base(spheres, position) { }

		public PasteOnceAction(SphereType[,] spheres, int x, int y) : base(spheres, x, y) { }

		public override string Name => "Paste Once";
	}

	[Serializable]
	class PasteRepeatingAction : AreaFillAction
	{
		public PasteRepeatingAction(SphereType[,] spheres, Point position) : base(spheres, position) { }

		public PasteRepeatingAction(SphereType[,] spheres, int x, int y) : base(spheres, x, y) { }

		public override string Name => "Paste Repeating";
	}

	[Serializable]
	class TextAction : AreaAction
	{
		public TextAction(SphereType?[,] spheres, Point position) : base(spheres, position) { }

		public TextAction(SphereType?[,] spheres, int x, int y) : base(spheres, x, y) { }

		public override string Name => "Text";
	}

	[Serializable]
	class ReplaceFGToBGAction : AreaAction
	{
		public ReplaceFGToBGAction(SphereType?[,] spheres, Point position) : base(spheres, position) { }

		public ReplaceFGToBGAction(SphereType?[,] spheres, int x, int y) : base(spheres, x, y) { }

		public override string Name => "Replace FG -> BG";
	}

	[Serializable]
	class ReplaceBGToFGAction : AreaAction
	{
		public ReplaceBGToFGAction(SphereType?[,] spheres, Point position) : base(spheres, position) { }

		public ReplaceBGToFGAction(SphereType?[,] spheres, int x, int y) : base(spheres, x, y) { }

		public override string Name => "Replace BG -> FG";
	}

	[Serializable]
	class SwapFGAndBGAction : AreaAction
	{
		public SwapFGAndBGAction(SphereType?[,] spheres, Point position) : base(spheres, position) { }

		public SwapFGAndBGAction(SphereType?[,] spheres, int x, int y) : base(spheres, x, y) { }

		public override string Name => "Swap FG <-> BG";
	}

	[Serializable]
	class FlipHorizontallyAction : Action
	{
		public FlipHorizontallyAction(Rectangle area)
		{
			this.area = area;
		}

		Rectangle area;

		public override void Do(LayoutData layout)
		{
			SphereType[,] copy = layout.Layout.ToArray();
			for (int y = 0; y < area.Height; y++)
				for (int x = 0; x < area.Width; x++)
					layout.Layout[area.X + x, area.Y + y] = copy[area.Right - x - 1, area.Y + y];
		}

		public override string Name => "Flip Horizontally";
	}

	[Serializable]
	class FlipVerticallyAction : Action
	{
		public FlipVerticallyAction(Rectangle area)
		{
			this.area = area;
		}

		Rectangle area;

		public override void Do(LayoutData layout)
		{
			SphereType[,] copy = layout.Layout.ToArray();
			for (int y = 0; y < area.Height; y++)
				for (int x = 0; x < area.Width; x++)
					layout.Layout[area.X + x, area.Y + y] = copy[area.X + x, area.Bottom - y - 1];
		}

		public override string Name => "Flip Vertically";
	}

	[Serializable]
	class RotateLeftAction : RotateAction
	{
		public RotateLeftAction(Rectangle area) : base(area, false) { }

		public override string Name => "Rotate Left";
	}

	[Serializable]
	class RotateRightAction : RotateAction
	{
		public RotateRightAction(Rectangle area) : base(area, true) { }

		public override string Name => "Rotate Right";
	}

	[Serializable]
	class BSFlipHorizontallyAction : AreaFillAction
	{
		public BSFlipHorizontallyAction(SphereType[,] spheres, Point position) : base(spheres, position) { }

		public BSFlipHorizontallyAction(SphereType[,] spheres, int x, int y) : base(spheres, x, y) { }

		public override string Name => "Flip Horizontally";
	}

	[Serializable]
	class BSFlipVerticallyAction : AreaFillAction
	{
		public BSFlipVerticallyAction(SphereType[,] spheres, Point position) : base(spheres, position) { }

		public BSFlipVerticallyAction(SphereType[,] spheres, int x, int y) : base(spheres, x, y) { }

		public override string Name => "Flip Vertically";
	}

	[Serializable]
	class BSRotateLeftAction : AreaFillAction
	{
		public BSRotateLeftAction(SphereType[,] spheres, Point position) : base(spheres, position) { }

		public BSRotateLeftAction(SphereType[,] spheres, int x, int y) : base(spheres, x, y) { }

		public override string Name => "Rotate Left";
	}

	[Serializable]
	class BSRotateRightAction : AreaFillAction
	{
		public BSRotateRightAction(SphereType[,] spheres, Point position) : base(spheres, position) { }

		public BSRotateRightAction(SphereType[,] spheres, int x, int y) : base(spheres, x, y) { }

		public override string Name => "Rotate Right";
	}
}
