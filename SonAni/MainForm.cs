using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SonicRetro.SonLVL.API;
using System.IO;
using System.Drawing.Imaging;

namespace SonAni
{
	public partial class MainForm : Form
	{
		SonAni.Properties.Settings Settings = SonAni.Properties.Settings.Default;

		public MainForm()
		{
			InitializeComponent();
		}

		ProjectFile project;
		AnimationInfo animInfo;
		Dictionary<string, ToolStripMenuItem> levelMenuItems;
		bool loaded;
		byte[] tiles;
		ColorPalette palette;
		List<MappingsFrame> mappings;
		List<DPLCFrame> dplc;
		string animlabel;
		List<Animation> animations;
		List<Sprite> sprites;
		Animation curanim;

		private void MainForm_Load(object sender, EventArgs e)
		{
			previewGraphics = previewPanel.CreateGraphics();
			previewGraphics.SetOptions();
			if (Settings.MRUList == null)
				Settings.MRUList = new System.Collections.Specialized.StringCollection();
			System.Collections.Specialized.StringCollection mru = new System.Collections.Specialized.StringCollection();
			foreach (string item in Settings.MRUList)
			{
				if (File.Exists(item))
				{
					mru.Add(item);
					recentProjectsToolStripMenuItem.DropDownItems.Add(item.Replace("&", "&&"));
				}
			}
			Settings.MRUList = mru;
			if (mru.Count > 0) recentProjectsToolStripMenuItem.DropDownItems.RemoveAt(0);
			if (Program.args.Length > 0)
				LoadINI(Program.args[0]);
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
			Settings.Save();
		}

		private void LoadINI(string filename)
		{
			project = ProjectFile.Load(filename);
			Environment.CurrentDirectory = Path.GetDirectoryName(filename);
			animationToolStripMenuItem.DropDownItems.Clear();
			levelMenuItems = new Dictionary<string, ToolStripMenuItem>();
			foreach (KeyValuePair<string, AnimationInfo> item in project.Animations)
			{
				if (!string.IsNullOrEmpty(item.Key))
				{
					string[] itempath = item.Key.Split('\\');
					ToolStripMenuItem parent = animationToolStripMenuItem;
					for (int i = 0; i < itempath.Length - 1; i++)
					{
						string curpath = string.Empty;
						if (i - 1 >= 0)
							curpath = string.Join(@"\", itempath, 0, i - 1);
						if (!string.IsNullOrEmpty(curpath))
							parent = levelMenuItems[curpath];
						curpath += itempath[i];
						if (!levelMenuItems.ContainsKey(curpath))
						{
							ToolStripMenuItem it = new ToolStripMenuItem(itempath[i].Replace("&", "&&")) { Tag = curpath };
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
			if (Settings.MRUList.Count == 0)
				recentProjectsToolStripMenuItem.DropDownItems.RemoveAt(0);
			if (Settings.MRUList.Contains(filename))
			{
				recentProjectsToolStripMenuItem.DropDownItems.RemoveAt(Settings.MRUList.IndexOf(filename));
				Settings.MRUList.Remove(filename);
			}
			Settings.MRUList.Insert(0, filename);
			recentProjectsToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem(filename));
		}

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
			animInfo = project.GetAnimationInfo((string)((ToolStripMenuItem)sender).Tag);
			Text = "SonAni - Loading " + animInfo.DisplayName + "...";
#if !DEBUG
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
				LevelData.littleendian = false;
				MultiFileIndexer<byte> art = new MultiFileIndexer<byte>();
				foreach (SonicRetro.SonLVL.API.FileInfo file in animInfo.Art)
					art.AddFile(new List<byte>(Compression.Decompress(file.Filename, animInfo.ArtCompression)), file.Offset);
				tiles = art.ToArray();
				art.Clear();
				byte[] tmp = null;
				using (Bitmap bmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
					palette = bmp.Palette;
				for (int i = 0; i < 256; i++)
					palette.Entries[i] = Color.Black;
				foreach (PaletteInfo palent in animInfo.Palette)
				{
					tmp = File.ReadAllBytes(palent.Filename);
					SonLVLColor[] palfile;
					palfile = new SonLVLColor[tmp.Length / 2];
					for (int pi = 0; pi < tmp.Length; pi += 2)
						palfile[pi / 2] = new SonLVLColor(SonicRetro.SonLVL.API.ByteConverter.ToUInt16(tmp, pi));
					for (int pa = 0; pa < palent.Length; pa++)
						palette.Entries[pa + palent.Destination] = palfile[pa + palent.Source].RGBColor;
				}
				palette.Entries[0] = Color.Transparent;
				Dictionary<string, int> labels = new Dictionary<string, int>();
				tmp = null;
				switch (animInfo.MappingsFormat)
				{
					case MappingsFormat.Binary:
						tmp = File.ReadAllBytes(animInfo.MappingsFile);
						break;
					case MappingsFormat.ASM:
					case MappingsFormat.Macro:
						tmp = LevelData.ASMToBin(animInfo.MappingsFile, animInfo.MappingsGame, out labels);
						break;
				}
				mappings = MappingsFrame.Load(tmp, animInfo.MappingsGame, labels);
				if (!string.IsNullOrEmpty(animInfo.DPLCFile))
				{
					labels = new Dictionary<string, int>();
					tmp = null;
					switch (animInfo.DPLCFormat)
					{
						case MappingsFormat.Binary:
							tmp = File.ReadAllBytes(animInfo.DPLCFile);
							break;
						case MappingsFormat.ASM:
						case MappingsFormat.Macro:
							tmp = LevelData.ASMToBin(animInfo.DPLCFile, animInfo.DPLCGame, out labels);
							break;
					}
					dplc = DPLCFrame.Load(tmp, animInfo.DPLCGame, labels);
				}
				else
					dplc = null;
				labels = new Dictionary<string, int>();
				tmp = null;
				switch (animInfo.AnimationFormat)
				{
					case MappingsFormat.Binary:
						tmp = File.ReadAllBytes(animInfo.AnimationFile);
						break;
					case MappingsFormat.ASM:
					case MappingsFormat.Macro:
						tmp = LevelData.ASMToBin(animInfo.AnimationFile, project.Game, out labels);
						break;
				}
				animations = Animation.Load(tmp, labels);
				animlabel = Path.GetFileNameWithoutExtension(animInfo.AnimationFile).MakeIdentifier();
				foreach (KeyValuePair<string, int> label in labels)
					if (label.Value == 0)
					{
						animlabel = label.Key;
						break;
					}
				sprites = new List<Sprite>(mappings.Count);
				for (int i = 0; i < mappings.Count; i++)
				{
					if (dplc == null)
						sprites.Add(new Sprite(LevelData.MapFrameToBmp(tiles, mappings[i], animInfo.StartPalette)));
					else
						sprites.Add(new Sprite(LevelData.MapFrameToBmp(tiles, mappings[i], dplc[i], animInfo.StartPalette)));
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
				File.WriteAllText("SonAni.log", initerror.ToString());
				if (MessageBox.Show(this, initerror.GetType().Name + ": " + initerror.Message + "\nLog file has been saved to " + Path.Combine(Environment.CurrentDirectory, "SonAni.log") + ".\nSend this to MainMemory on the Sonic Retro forums.\n\nClose the program?", "SonAni Error", MessageBoxButtons.YesNo) == DialogResult.Yes)
					Close();
				Enabled = true;
				return;
			}
			animationListBox.Items.Clear();
			animationListBox.Items.AddRange(animations.ConvertAll((a) => a.Name).ToArray());
			mappingFrameList.Images.Clear();
			for (int i = 0; i < sprites.Count; i++)
				using (Bitmap image = sprites[i].GetBitmap().ToBitmap(palette))
					mappingFrameList.Images.Add(image.Resize(new Size(mappingFrameList.ImageSize, mappingFrameList.ImageSize)));
			mappingFrameList.ChangeSize();
			Text = "SonAni - " + animInfo.DisplayName;
			loaded = Enabled = animationListBox.Enabled = addAnimationButton.Enabled = true;
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (Animation item in animations)
				if (item.EndType == 0xFF)
					item.ExtraParam = null;
			switch (animInfo.AnimationFormat)
			{
				case MappingsFormat.Binary:
					File.WriteAllBytes(animInfo.AnimationFile, Animation.GetBytes(animations.ToArray()));
					break;
				case MappingsFormat.ASM:
					Animation.ToASM(animInfo.AnimationFile, animlabel, animations, false);
					break;
				case MappingsFormat.Macro:
					Animation.ToASM(animInfo.AnimationFile, animlabel, animations, true);
					break;
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

		private void animationListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (animationListBox.SelectedIndex == -1)
			{
				curanim = null;
				removeAnimationButton.Enabled = previewBox.Enabled = exportGIFButton.Enabled =
					mappingFrameList.Enabled = animationName.Enabled = animationSpeed.Enabled =
					animationFrameList.Enabled = endTypeBox.Enabled = false;
				animationFrameList.Images.Clear();
				previewTimer.Stop();
				playButton.Text = "4";
			}
			else
			{
				curanim = animations[animationListBox.SelectedIndex];
				removeAnimationButton.Enabled = true;
				previewBox.Enabled = exportGIFButton.Enabled = mappingFrameList.Enabled = true;
				previewTimer.Stop();
				PreviewFrame = 0;
				playButton.Text = "4";
				animationName.Text = curanim.Name;
				animationName.Enabled = true;
				animationSpeed.Value = curanim.Speed;
				animationSpeed.Enabled = true;
				animationFrameList.Images.Clear();
				for (int i = 0; i < curanim.Count; i++)
					animationFrameList.Images.Add(mappingFrameList.Images[curanim[i]]);
				animationFrameList.ChangeSize();
				animationFrameList.Enabled = true;
				endTypeFF.Checked = endTypeFE.Checked = endTypeFD.Checked = endFrameNum.Enabled = endAnimNum.Enabled
					= endAnimBox.Enabled = false;
				endAnimBox.SelectedIndex = -1;
				endFrameNum.Maximum = curanim.Count - 1;
				switch (curanim.EndType)
				{
					case 0xFF:
						endTypeFF.Checked = true;
						break;
					case 0xFE:
						endTypeFE.Checked = true;
						endFrameNum.Value = curanim.Count - curanim.ExtraParam.Value;
						break;
					case 0xFD:
						endTypeFD.Checked = true;
						endAnimNum.Value = curanim.ExtraParam.Value;
						break;
					case 0xFC:
						endTypeFC.Checked = true;
						break;
					case 0xFB:
						endTypeFB.Checked = true;
						break;
					case 0xFA:
						endTypeFA.Checked = true;
						break;
				}
				endTypeBox.Enabled = true;
			}
		}

		private int previewFrame;
		private int PreviewFrame
		{
			get { return previewFrame; }
			set
			{
				previewFrame = value;
				DrawPreview();
			}
		}

		private Graphics previewGraphics;

		private void DrawPreview()
		{
			if (!loaded || curanim == null) return;
			previewGraphics.Clear(previewPanel.BackColor);
			Sprite spr = sprites[curanim[previewFrame]];
			previewGraphics.DrawImage(spr.GetBitmap().ToBitmap(palette), (previewPanel.Width / 2) + spr.X, (previewPanel.Height / 2) + spr.Y, spr.Width, spr.Height);
		}

		private void previewPanel_Paint(object sender, PaintEventArgs e) { DrawPreview(); }

		private void stopButton_Click(object sender, EventArgs e)
		{
			previewTimer.Stop();
			PreviewFrame = 0;
			playButton.Text = "4";
		}

		private void playButton_Click(object sender, EventArgs e)
		{
			if (previewTimer.Enabled)
			{
				previewTimer.Stop();
				playButton.Text = "4";
			}
			else
			{
				previewTimer.Start();
				playButton.Text = ";";
			}
		}

		private void nextButton_Click(object sender, EventArgs e)
		{
			previewTimer.Stop();
			playButton.Text = "4";
			PreviewNextFrame();
		}

		int delayframes;
		private void previewTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (delayframes++ >= (curanim.Speed < 0xFD ? curanim.Speed : 4))
			{
				PreviewNextFrame();
				delayframes = 0;
			}
		}

		private void PreviewNextFrame()
		{
			if (PreviewFrame >= curanim.Count - 1)
			{
				switch (curanim.EndType)
				{
					case 0xFF:
					case 0xFB:
						PreviewFrame = 0;
						break;
					case 0xFE:
						PreviewFrame = curanim.Count - curanim.ExtraParam.Value;
						break;
					default:
						previewTimer.Stop();
						playButton.Text = "4";
						break;
				}
			}
			else
				PreviewFrame++;
		}

		public const double Frame = (1 / 60d) / (1 / 100d);

		private void exportGIFButton_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog fd = new SaveFileDialog() { DefaultExt = "gif", FileName = curanim.Name + ".gif", Filter = "GIF Images|*.gif" })
				if (fd.ShowDialog(this) == DialogResult.OK)
				{
					string file = fd.FileName;
					string filebase = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));
					string fileext = Path.GetExtension(file);
					Animation anim = curanim;
					List<Animation> anims = new List<Animation>() { anim };
					while (anims[anims.Count - 1].EndType == 0xFD)
						anims.Add(animations[anims[anims.Count - 1].ExtraParam.Value]);
					bool isFE = anims[anims.Count - 1].EndType == 0xFE;
					if (isFE)
						file = filebase + "_intro" + fileext;
					AnimatedGifEncoder gif = new AnimatedGifEncoder();
					gif.Start(file);
					if (anim.EndType == 0xFF || anim.EndType == 0xFB)
						gif.SetRepeat(0);
					else
						gif.SetRepeat(-1);
					double delay = 0;
					double leftover = 0;
					gif.SetDelay(delay);
					gif.SetPalette(palette.Entries);
					int left = 0;
					int right = 0;
					int top = 0;
					int bottom = 0;
					for (int a = 0; a < anims.Count; a++)
						for (int i = 0; i < anims[a].Count; i++)
						{
							left = Math.Min(sprites[anims[a][i]].Left, left);
							right = Math.Max(sprites[anims[a][i]].Right, right);
							top = Math.Min(sprites[anims[a][i]].Top, top);
							bottom = Math.Max(sprites[anims[a][i]].Bottom, bottom);
						}
					for (int a = 0; a < anims.Count; a++)
						for (int i = 0; i < anims[a].Count; i++)
						{
							delay = (anims[a].Speed < 0xFD ? anims[a].Speed : 4) * Frame;
							int samecnt = 0;
							while (i + samecnt < anims[a].Count && anims[a][i + samecnt] == anims[a][i])
								samecnt++;
							delay *= samecnt;
							leftover += delay % 1;
							if (leftover >= 1)
								delay++;
							leftover %= 1;
							gif.SetDelay(delay);
							BitmapBits image = new BitmapBits(right - left, bottom - top);
							image.DrawBitmapComposited(sprites[anims[a][i]].GetBitmap(), sprites[anims[a][i]].X - left, sprites[anims[a][i]].Y - top);
							gif.AddFrame(image);
							i += samecnt - 1;
						}
					gif.Finish();
					if (isFE)
					{
						anim = anims[anims.Count - 1];
						file = filebase + "_loop" + fileext;
						gif = new AnimatedGifEncoder();
						gif.Start(file);
						gif.SetRepeat(0);
						gif.SetPalette(palette.Entries);
						left = 0;
						right = 0;
						top = 0;
						bottom = 0;
						for (int i = anim.Count - anim.ExtraParam.Value; i < anim.Count; i++)
						{
							left = Math.Min(sprites[anim[i]].Left, left);
							right = Math.Max(sprites[anim[i]].Right, right);
							top = Math.Min(sprites[anim[i]].Top, top);
							bottom = Math.Max(sprites[anim[i]].Bottom, bottom);
						}
						for (int i = anim.Count - anim.ExtraParam.Value; i < anim.Count; i++)
						{
							delay = (anim.Speed < 0xFD ? anim.Speed : 4) * Frame;
							int samecnt = 0;
							while (i + samecnt < anim.Count && anim[i + samecnt] == anim[i])
								samecnt++;
							delay *= samecnt;
							leftover += delay % 1;
							if (leftover >= 1)
								delay++;
							leftover %= 1;
							gif.SetDelay(delay);
							BitmapBits image = new BitmapBits(right - left, bottom - top);
							image.DrawBitmapComposited(sprites[anim[i]].GetBitmap(), sprites[anim[i]].X - left, sprites[anim[i]].Y - top);
							gif.AddFrame(image);
							i += samecnt - 1;
						}
						gif.Finish();
					}
				}
		}

		private void mappingFrameList_ItemDrag(object sender, EventArgs e)
		{
			DoDragDrop(new DataObject("SonAniMapFrame", mappingFrameList.SelectedIndex), DragDropEffects.Copy);          
		}

		bool dragdrop;
		int dragobj;
		Point dragpoint;
		private void animationFrameList_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent("SonAniMapFrame"))
			{
				e.Effect = DragDropEffects.All;
				dragdrop = true;
				dragobj = (int)e.Data.GetData("SonAniMapFrame");
				dragpoint = animationFrameList.PointToClient(new Point(e.X, e.Y));
				animationFrameList.Invalidate();
			}
			else if (e.Data.GetDataPresent("SonAniAnimFrame"))
			{
				e.Effect = DragDropEffects.All;
				dragdrop = true;
				dragobj = curanim[(int)e.Data.GetData("SonAniAnimFrame")];
				dragpoint = animationFrameList.PointToClient(new Point(e.X, e.Y));
				animationFrameList.Invalidate();
			}
			else
				dragdrop = false;
		}

		private void animationFrameList_DragOver(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent("SonAniMapFrame"))
			{
				e.Effect = DragDropEffects.All;
				dragdrop = true;
				dragobj = (int)e.Data.GetData("SonAniMapFrame");
				dragpoint = animationFrameList.PointToClient(new Point(e.X, e.Y));
				if (dragpoint.X < 8)
					animationFrameList.ScrollValue -= 8 - dragpoint.X;
				else if (dragpoint.X > animationFrameList.Width - 8)
					animationFrameList.ScrollValue += dragpoint.X - (animationFrameList.Width - 8);
				animationFrameList.Invalidate();
			}
			else if (e.Data.GetDataPresent("SonAniAnimFrame"))
			{
				e.Effect = DragDropEffects.All;
				dragdrop = true;
				dragobj = curanim[(int)e.Data.GetData("SonAniAnimFrame")];
				dragpoint = animationFrameList.PointToClient(new Point(e.X, e.Y));
				if (dragpoint.X < 8)
					animationFrameList.ScrollValue -= 8 - dragpoint.X;
				else if (dragpoint.X > animationFrameList.Width - 8)
					animationFrameList.ScrollValue += dragpoint.X - (animationFrameList.Width - 8);
				animationFrameList.Invalidate();
			}
			else
				dragdrop = false;
		}

		private void animationFrameList_DragLeave(object sender, EventArgs e)
		{
			dragdrop = false;
			animationFrameList.Invalidate();
		}

		private void animationFrameList_Paint(object sender, PaintEventArgs e)
		{
			if (dragdrop)
			{
				e.Graphics.DrawImage(mappingFrameList.Images[dragobj], dragpoint.X - (mappingFrameList.ImageSize / 2),
					dragpoint.Y - (mappingFrameList.ImageSize / 2), mappingFrameList.ImageSize, mappingFrameList.ImageSize);
				Rectangle r = animationFrameList.GetItemBounds(animationFrameList.GetItemAtPoint(dragpoint));
				e.Graphics.DrawLine(new Pen(Color.Black, 2), r.Left + 1, r.Top, r.Left + 1, r.Bottom);
			}
		}

		private void animationFrameList_DragDrop(object sender, DragEventArgs e)
		{
			dragdrop = false;
			if (e.Data.GetDataPresent("SonAniMapFrame"))
			{
				Point clientPoint = animationFrameList.PointToClient(new Point(e.X, e.Y));
				int index = animationFrameList.GetItemAtPoint(clientPoint);
				int obj = (int)e.Data.GetData("SonAniMapFrame");
				curanim.Frames.Insert(index, (byte)obj);
				animationFrameList.Images.Insert(index, mappingFrameList.Images[obj]);
				animationFrameList.SelectedIndex = index;
				endFrameNum.Maximum = animationFrameList.Images.Count - 1;
			}
			else if (e.Data.GetDataPresent("SonAniAnimFrame"))
			{
				Point clientPoint = animationFrameList.PointToClient(new Point(e.X, e.Y));
				int newindex = animationFrameList.GetItemAtPoint(clientPoint);
				int oldindex = (int)e.Data.GetData("SonAniAnimFrame");
				if (newindex > oldindex)
					newindex--;
				byte frame = curanim[oldindex];
				curanim.Frames.RemoveAt(oldindex);
				curanim.Frames.Insert(newindex, frame);
				animationFrameList.Images.RemoveAt(oldindex);
				animationFrameList.Images.Insert(newindex, mappingFrameList.Images[frame]);
				animationFrameList.SelectedIndex = newindex;
			}
		}

		private void animationFrameList_KeyDown(object sender, KeyEventArgs e)
		{
			if (animationFrameList.SelectedIndex > -1 && e.KeyCode == Keys.Delete)
			{
				curanim.Frames.RemoveAt(animationFrameList.SelectedIndex);
				animationFrameList.Images.RemoveAt(animationFrameList.SelectedIndex);
				animationFrameList.SelectedIndex = Math.Min(animationFrameList.SelectedIndex, animationFrameList.Images.Count - 1);
				endFrameNum.Maximum = animationFrameList.Images.Count - 1;
			}
		}

		private void animationFrameList_ItemDrag(object sender, EventArgs e)
		{
			DoDragDrop(new DataObject("SonAniAnimFrame", animationFrameList.SelectedIndex), DragDropEffects.Copy);
		}

		private void endTypeFF_CheckedChanged(object sender, EventArgs e)
		{
			if (endTypeFF.Checked) curanim.EndType = 0xFF;
		}
 
		private void endTypeFE_CheckedChanged(object sender, EventArgs e)
		{
			endFrameNum.Enabled = endTypeFE.Checked;
			if (endTypeFE.Checked) curanim.EndType = 0xFE;
		}

		private void endTypeFD_CheckedChanged(object sender, EventArgs e)
		{
			endAnimNum.Enabled = endAnimBox.Enabled = endTypeFD.Checked;
			if (endTypeFD.Checked) curanim.EndType = 0xFD;
		}

		private void endTypeFC_CheckedChanged(object sender, EventArgs e)
		{
			if (endTypeFC.Checked) curanim.EndType = 0xFC;
		}

		private void endTypeFB_CheckedChanged(object sender, EventArgs e)
		{
			if (endTypeFB.Checked) curanim.EndType = 0xFB;
		}

		private void endTypeFA_CheckedChanged(object sender, EventArgs e)
		{
			if (endTypeFA.Checked) curanim.EndType = 0xFA;
		}

		private void endAnimBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (endAnimBox.SelectedIndex != -1)
				endAnimNum.Value = endAnimBox.SelectedIndex;
		}

		private void endFrameNum_ValueChanged(object sender, EventArgs e)
		{
			curanim.ExtraParam = (byte)(curanim.Count - endFrameNum.Value);
		}

		private void endAnimNum_ValueChanged(object sender, EventArgs e)
		{
			curanim.ExtraParam = (byte)endAnimNum.Value;
		}
	}

	public class ProjectFile
	{
		[DefaultValue(EngineVersion.S2)]
		[IniName("game")]
		public EngineVersion Game { get; set; }
		[IniName("mapgame")]
		public EngineVersion MappingsGame { get; set; }
		[DefaultValue(MappingsFormat.Binary)]
		[IniName("mapfmt")]
		public MappingsFormat MappingsFormat { get; set; }
		[IniName("dplcgame")]
		public EngineVersion DPLCGame { get; set; }
		[DefaultValue(MappingsFormat.Binary)]
		[IniName("dplcfmt")]
		public MappingsFormat DPLCFormat { get; set; }
		[DefaultValue(MappingsFormat.ASM)]
		[IniName("animfmt")]
		public MappingsFormat AnimationFormat { get; set; }
		[IniCollection(IniCollectionMode.IndexOnly)]
		public Dictionary<string, AnimationInfo> Animations { get; set; }

		public static ProjectFile Load(string filename)
		{
			ProjectFile result = IniSerializer.Deserialize<ProjectFile>(filename);
			if (result.MappingsGame == EngineVersion.Invalid)
				result.MappingsGame = result.Game;
			if (result.DPLCGame == EngineVersion.Invalid)
				result.DPLCGame = result.MappingsGame;
			return result;
		}

		public AnimationInfo GetAnimationInfo(string animName)
		{
			AnimationInfo info = Animations[animName];
			if (animName.Contains("\\"))
				animName = animName.Substring(animName.LastIndexOf('\\') + 1);
			AnimationInfo result = new AnimationInfo();
			result.DisplayName = info.DisplayName ?? animName;
			foreach (string item in new string[] { "MappingsGame", "DPLCGame" })
			{
				System.Reflection.PropertyInfo gam = typeof(ProjectFile).GetProperty(item);
				System.Reflection.PropertyInfo lvl = typeof(AnimationInfo).GetProperty(item);
				lvl.SetValue(result, Game, null);
				if ((EngineVersion)gam.GetValue(this, null) != EngineVersion.Invalid)
					lvl.SetValue(result, gam.GetValue(this, null), null);
				if ((EngineVersion)lvl.GetValue(info, null) != EngineVersion.Invalid)
					lvl.SetValue(result, lvl.GetValue(info, null), null);
			}
			foreach (string item in new string[] { "MappingsFormat", "DPLCFormat", "AnimationFormat" })
			{
				System.Reflection.PropertyInfo gam = typeof(ProjectFile).GetProperty(item);
				System.Reflection.PropertyInfo lvl = typeof(AnimationInfo).GetProperty(item);
				lvl.SetValue(result, MappingsFormat.Binary, null);
				if ((MappingsFormat)gam.GetValue(this, null) != MappingsFormat.Invalid)
					lvl.SetValue(result, gam.GetValue(this, null), null);
				if ((MappingsFormat)lvl.GetValue(info, null) != MappingsFormat.Invalid)
					lvl.SetValue(result, lvl.GetValue(info, null), null);
			}
			result.AnimationFile = info.AnimationFile;
			result.Art = info.Art;
			result.ArtCompression = info.ArtCompression;
			result.DPLCFile = info.DPLCFile;
			result.MappingsFile = info.MappingsFile;
			result.Palette = info.Palette;
			return result;
		}
	}

	public class AnimationInfo
	{
		[IniName("displayname")]
		public string DisplayName { get; set; }
		[IniName("art")]
		[IniCollection(IniCollectionMode.SingleLine, Format = "|")]
		public SonicRetro.SonLVL.API.FileInfo[] Art { get; set; }
		[IniName("artcmp")]
		[DefaultValue(CompressionType.Nemesis)]
		public CompressionType ArtCompression { get; set; }
		[IniName("palette")]
		public PaletteList Palette { get; set; }
		[IniName("map")]
		public string MappingsFile { get; set; }
		[IniName("mapgame")]
		public EngineVersion MappingsGame { get; set; }
		[IniName("mapfmt")]
		public MappingsFormat MappingsFormat { get; set; }
		[IniName("dplc")]
		public string DPLCFile { get; set; }
		[IniName("dplcgame")]
		public EngineVersion DPLCGame { get; set; }
		[IniName("dplcfmt")]
		public MappingsFormat DPLCFormat { get; set; }
		[IniName("anim")]
		public string AnimationFile { get; set; }
		[IniName("animfmt")]
		public MappingsFormat AnimationFormat { get; set; }
		[IniName("startpal")]
		public int StartPalette { get; set; }
	}
}
