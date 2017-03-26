using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using SonicRetro.SonLVL.API;
using SonicRetro.SonLVL.API.XMLDef;
using XMLEnum = SonicRetro.SonLVL.API.XMLDef.Enum;
using XMLImage = SonicRetro.SonLVL.API.XMLDef.Image;
using XMLImageList = SonicRetro.SonLVL.API.XMLDef.ImageList;

namespace ObjDefEditor
{
	public partial class MainForm : Form
	{
		public static MainForm Instance { get; private set; }
		private readonly Properties.Settings Settings = Properties.Settings.Default;

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
			File.WriteAllLines("ObjDefEditor.log", LogFile.ToArray());
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
		bool suppressEvents;
		Dictionary<string, ToolStripMenuItem> levelMenuItems;
		List<string> objectLists;
		Dictionary<string, ObjectData> objlist;
		ObjDef Definition;
		string fileName = string.Empty;
		List<Sprite> images = new List<Sprite>();
		TypeCode[] propertyTypes;

		private void MainForm_Load(object sender, EventArgs e)
		{
			if (Settings.MRUList == null)
				Settings.MRUList = new System.Collections.Specialized.StringCollection();
			System.Collections.Specialized.StringCollection mru = new System.Collections.Specialized.StringCollection();
			foreach (string item in Settings.MRUList)
				if (File.Exists(item))
				{
					mru.Add(item);
					recentProjectsToolStripMenuItem.DropDownItems.Add(item.Replace("&", "&&"));
				}
			Settings.MRUList = mru;
			if (mru.Count > 0) recentProjectsToolStripMenuItem.DropDownItems.Remove(noneToolStripMenuItem);
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (Definition != null)
				switch (MessageBox.Show(this, "Do you want to save?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
				{
					case DialogResult.Yes:
						saveToolStripMenuItem_Click(this, EventArgs.Empty);
						break;
					case DialogResult.Cancel:
						e.Cancel = true;
						return;
				}
			Settings.Save();
		}

		#region Menu
		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (Definition != null)
				switch (MessageBox.Show(this, "Do you want to save?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
				{
					case DialogResult.Yes:
						saveToolStripMenuItem_Click(this, EventArgs.Empty);
						break;
					case DialogResult.Cancel:
						return;
				}
			using (OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = "ini", Filter = "SonLVL INI Files|*.ini" })
			{
				if (!string.IsNullOrEmpty(fileName))
				{
					dlg.InitialDirectory = Path.GetDirectoryName(fileName);
					dlg.FileName = Path.GetFileName(fileName);
				}
				if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{
					fileName = dlg.FileName;
					LoadINI();
				}
			}
		}

		private void LoadINI()
		{
			Environment.CurrentDirectory = Path.GetDirectoryName(fileName);
			listPanel.Enabled = editorTabs.Enabled = saveToolStripMenuItem.Enabled = false;
			Definition = null;
			LevelData.LoadGame(fileName);
			changeLevelToolStripMenuItem.DropDownItems.Clear();
			levelMenuItems = new Dictionary<string, ToolStripMenuItem>();
			foreach (KeyValuePair<string, LevelInfo> item in LevelData.Game.Levels)
			{
				if (!string.IsNullOrEmpty(item.Key))
				{
					string[] itempath = item.Key.Split('\\');
					ToolStripMenuItem parent = changeLevelToolStripMenuItem;
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
				recentProjectsToolStripMenuItem.DropDownItems.Remove(noneToolStripMenuItem);
			if (Settings.MRUList.Contains(fileName))
			{
				recentProjectsToolStripMenuItem.DropDownItems.RemoveAt(Settings.MRUList.IndexOf(fileName));
				Settings.MRUList.Remove(fileName);
			}
			Settings.MRUList.Insert(0, fileName);
			recentProjectsToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem(fileName));
		}

		private void LevelToolStripMenuItem_Clicked(object sender, EventArgs e)
		{
			fileToolStripMenuItem.DropDown.Hide();
			if (Definition != null)
				switch (MessageBox.Show(this, "Do you want to save?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
				{
					case DialogResult.Yes:
						saveToolStripMenuItem_Click(this, EventArgs.Empty);
						break;
					case DialogResult.Cancel:
						return;
				}
			editorTabs.Enabled = saveToolStripMenuItem.Enabled = false;
			Definition = null;
			foreach (KeyValuePair<string, ToolStripMenuItem> item in levelMenuItems)
				item.Value.Checked = false;
			((ToolStripMenuItem)sender).Checked = true;
			Enabled = false;
#if !DEBUG
			backgroundWorker1.RunWorkerAsync(((ToolStripMenuItem)sender).Tag);
#else
			backgroundWorker1_DoWork(null, new DoWorkEventArgs(((ToolStripMenuItem)sender).Tag));
			backgroundWorker1_RunWorkerCompleted(null, null);
#endif
		}

		Exception initerror = null;
		private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
		{
#if !DEBUG
			try
			{
#endif
			LevelData.LoadLevel((string)e.Argument, true);
#if !DEBUG
			}
			catch (Exception ex) { initerror = ex; }
#endif
		}

		private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (initerror != null)
			{
				System.IO.File.WriteAllLines("ObjDefEditor.log", LogFile.ToArray());
				using (ErrorDialog ed = new ErrorDialog(initerror.GetType().Name + ": " + initerror.Message + "\nLog file has been saved to " + System.IO.Path.Combine(Environment.CurrentDirectory, "SonLVL.log") + ".\nSend this to MainMemory on the Sonic Retro forums.", true))
					if (ed.ShowDialog(this) == System.Windows.Forms.DialogResult.Cancel) Close();
				Enabled = true;
				return;
			}
			objectLists = new List<string>();
			if (LevelData.Game.ObjectList != null)
				objectLists.AddRange(LevelData.Game.ObjectList);
			if (LevelData.Level.ObjectList != null)
				objectLists.AddRange(LevelData.Level.ObjectList);
			objectListList.Items.Clear();
			objectListList.BeginUpdate();
			if (LevelData.Game.ObjectList != null)
				foreach (string item in LevelData.Game.ObjectList)
					objectListList.Items.Add(item + " (Game)");
			if (LevelData.Level.ObjectList != null)
				foreach (string item in LevelData.Level.ObjectList)
					objectListList.Items.Add(item + " (Level)");
			objectListList.EndUpdate();
			objectDefinitionList.Items.Clear();
			addDefinitionButton.Enabled = false;
			spriteNum.Maximum = Math.Max(LevelData.Sprites.Count - 1, 0);
			imageType.Items.Clear();
			imageType.Items.AddRange(new object[] { "Art+Mappings", "Bitmap" });
			if (LevelData.Sprites.Count > 0)
				imageType.Items.Add("Sprite");
			Enabled = listPanel.Enabled = true;
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			foreach (XMLImage item in Definition.Images.Items)
				if (item is ImageFromBitmap)
				{
					ImageFromBitmap img = (ImageFromBitmap)item;
					img.filename = GetRelativePath(img.filename);
				}
				else if (item is ImageFromMappings)
				{
					ImageFromMappings img = (ImageFromMappings)item;
					foreach (ArtFile art in img.ArtFiles)
						if (art.filename != "LevelArt")
							art.filename = GetRelativePath(art.filename);
					img.MapFile.filename = GetRelativePath(img.MapFile.filename);
					if (!string.IsNullOrEmpty(img.MapFile.dplcfile))
						img.MapFile.dplcfile = GetRelativePath(img.MapFile.dplcfile);
				}
			if (Definition.Display.DisplayOptions.Length == 0)
				Definition.Display = null;
			if (Definition.Enums.Items.Length == 0)
				Definition.Enums = null;
			if (Definition.Properties.Items.Length == 0)
				Definition.Properties = null;
			if (Definition.Subtypes.Items.Length == 0)
				Definition.Subtypes = null;
			IniSerializer.Serialize(objlist, objectLists[objectListList.SelectedIndex]);
			Definition.Save(objlist[(string)objectDefinitionList.SelectedItem].XMLFile);
			SetupDefinition();
		}

		private string GetRelativePath(string path)
		{
			return Uri.UnescapeDataString(new Uri(Path.GetDirectoryName(fileName) + Path.DirectorySeparatorChar).MakeRelativeUri(new Uri(path)).ToString());
		}

		private void recentFilesToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
		{
			fileToolStripMenuItem.DropDown.Close();
			if (Definition != null)
				switch (MessageBox.Show(this, "Do you want to save?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
				{
					case DialogResult.Yes:
						saveToolStripMenuItem_Click(this, EventArgs.Empty);
						break;
					case DialogResult.Cancel:
						return;
				}
			fileName = Settings.MRUList[recentProjectsToolStripMenuItem.DropDownItems.IndexOf(e.ClickedItem)];
			LoadINI();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void reportBugToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (BugReportDialog dlg = new BugReportDialog("Object Definition Editor", string.Join(Environment.NewLine, LogFile.ToArray())))
				dlg.ShowDialog(this);
		}
		#endregion

		#region List Controls
		private void objectListList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (objectListList.SelectedIndex == -1)
			{
				objectDefinitionList.Items.Clear();
				addDefinitionButton.Enabled = false;
				return;
			}
			objlist = IniSerializer.Deserialize<Dictionary<string, ObjectData>>(objectLists[objectListList.SelectedIndex]);
			objectDefinitionList.Items.Clear();
			objectDefinitionList.BeginUpdate();
			foreach (KeyValuePair<string, ObjectData> item in objlist)
				if (!string.IsNullOrEmpty(item.Value.XMLFile))
					objectDefinitionList.Items.Add(item.Key);
			objectDefinitionList.EndUpdate();
			addDefinitionButton.Enabled = true;
		}

		private void objectDefinitionList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (objectDefinitionList.SelectedIndex == -1)
			{
				editorTabs.Enabled = saveToolStripMenuItem.Enabled = false;
				return;
			}
			if (Definition != null)
				switch (MessageBox.Show(this, "Do you want to save?", Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
				{
					case DialogResult.Yes:
						saveToolStripMenuItem_Click(this, EventArgs.Empty);
						break;
					case DialogResult.Cancel:
						return;
				}
			Definition = ObjDef.Load(objlist[(string)objectDefinitionList.SelectedItem].XMLFile);
			SetupDefinition();
			ResetImageLists();
			defName.Text = Definition.Name;
			defNamespace.Text = Definition.Namespace;
			defType.Text = Definition.TypeName;
			switch (Definition.Language)
			{
				case "cs":
					defLanguage.SelectedIndex = 0;
					break;
				case "vb":
					defLanguage.SelectedIndex = 1;
					break;
				default:
					defLanguage.SelectedIndex = -1;
					break;
			}
			defImage.SelectedIndex = defImage.Items.IndexOf(Definition.Image);
			defRemember.Checked = Definition.RememberState;
			defDebug.Checked = Definition.Debug;
			imageControls.Enabled = deleteImageButton.Enabled = mappingsImageControls.Visible = bitmapImageControls.Visible = false;
			subtypeControls.Enabled = false;
			selectedSubtype.Items.Clear();
			selectedSubtype.BeginUpdate();
			foreach (Subtype sub in Definition.Subtypes.Items)
				selectedSubtype.Items.Add(sub.id + ": " + sub.name);
			selectedSubtype.EndUpdate();
			propertyValueType.Items.Clear();
			propertyValueType.BeginUpdate();
			propertyValueType.Items.AddRange(new object[] { "bool", "byte", "int" });
			selectedEnum.Items.Clear();
			selectedEnum.BeginUpdate();
			foreach (XMLEnum enu in Definition.Enums.Items)
			{
				selectedEnum.Items.Add(enu.name);
				propertyValueType.Items.Add(enu.name);
			}
			selectedEnum.EndUpdate();
			propertyValueType.EndUpdate();
			conditionProperty.Items.Clear();
			conditionProperty.BeginUpdate();
			List<TypeCode> codes = new List<TypeCode>();
			Type basetype = LevelData.ObjectFormat.ObjectType;
			foreach (System.Reflection.PropertyInfo info in basetype.GetProperties())
			{
				if (info.GetGetMethod() == null | info.GetSetMethod() == null) continue;
				TypeCode code = Type.GetTypeCode(info.PropertyType);
				switch (code)
				{
					case TypeCode.Boolean:
					case TypeCode.Byte:
					case TypeCode.Int16:
					case TypeCode.Int32:
					case TypeCode.SByte:
					case TypeCode.UInt16:
					case TypeCode.UInt32:
						conditionProperty.Items.Add(info.Name);
						codes.Add(code);
						break;
				}
			}
			propertyTypes = codes.ToArray();
			selectedProperty.Items.Clear();
			selectedProperty.BeginUpdate();
			foreach (Property prop in Definition.Properties.Items)
			{
				selectedProperty.Items.Add(prop.name);
				conditionProperty.Items.Add(prop.name);
			}
			selectedProperty.EndUpdate();
			conditionProperty.EndUpdate();
			selectedDisplayOption.Items.Clear();
			selectedDisplayOption.BeginUpdate();
			foreach (DisplayOption opt in Definition.Display.DisplayOptions)
				if (opt.Conditions.Length == 0)
					selectedDisplayOption.Items.Add("Always True");
				else
				{
					List<string> strs = new List<string>(opt.Conditions.Length);
					foreach (Condition cond in opt.Conditions)
						strs.Add(cond.property + " = " + cond.value);
					selectedDisplayOption.Items.Add(string.Join(" & ", strs.ToArray()));
				}
			deleteDefinitionButton.Enabled = editorTabs.Enabled = saveToolStripMenuItem.Enabled = true;
		}

		private void addDefinitionButton_Click(object sender, EventArgs e)
		{
			using (IDDialog dlg = new IDDialog())
				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					string id = dlg.ID.ToString("X2");
					if (objlist.ContainsKey(id))
					{
						if (!string.IsNullOrEmpty(objlist[id].XMLFile))
						{
							MessageBox.Show(this, "This object already has a definition.", Text);
							return;
						}
						if (MessageBox.Show(this, "This object already has a definition.\n\nOverwrite it?", Text, MessageBoxButtons.YesNo) != DialogResult.Yes)
							return;
					}
					using (SaveFileDialog sfd = new SaveFileDialog() { DefaultExt = "xml", Filter = "XML Files|*.xml", InitialDirectory = Environment.CurrentDirectory, RestoreDirectory = true })
						if (sfd.ShowDialog(this) == DialogResult.OK)
						{
							objlist[id] = new ObjectData() { XMLFile = GetRelativePath(sfd.FileName) };
							new ObjDef().Save(objlist[id].XMLFile);
							objectDefinitionList.Items.Add(id);
							objectDefinitionList.SelectedIndex = objectDefinitionList.Items.Count - 1;
						}
				}
		}

		private void deleteDefinitionButton_Click(object sender, EventArgs e)
		{
			objlist.Remove((string)objectDefinitionList.SelectedItem);
			objectDefinitionList.Items.RemoveAt(objectDefinitionList.SelectedIndex);
		}
		#endregion

		#region Helper Functions
		private void SetupDefinition()
		{
			Uri baseUri = new Uri(Environment.CurrentDirectory + Path.DirectorySeparatorChar);
			Definition.Display = Definition.Display ?? new Display();
			Definition.Display.DisplayOptions = Definition.Display.DisplayOptions ?? new DisplayOption[0];
			foreach (DisplayOption item in Definition.Display.DisplayOptions)
			{
				item.Conditions = item.Conditions ?? new Condition[0];
				item.Images = item.Images ?? new ImageRef[0];
				item.Lines = item.Lines ?? new Line[0];
			}
			Definition.Enums = Definition.Enums ?? new EnumList();
			Definition.Enums.Items = Definition.Enums.Items ?? new XMLEnum[0];
			foreach (XMLEnum item in Definition.Enums.Items)
				item.Items = item.Items ?? new EnumMember[0];
			Definition.Image = Definition.Image ?? string.Empty;
			Definition.Images = Definition.Images ?? new XMLImageList();
			Definition.Images.Items = Definition.Images.Items ?? new XMLImage[0];
			foreach (XMLImage item in Definition.Images.Items)
				if (item is ImageFromBitmap)
				{
					ImageFromBitmap img = (ImageFromBitmap)item;
					img.filename = Path.Combine(Environment.CurrentDirectory, img.filename);
				}
				else if (item is ImageFromMappings)
				{
					ImageFromMappings img = (ImageFromMappings)item;
					img.ArtFiles = img.ArtFiles ?? new ArtFile[0];
					foreach (ArtFile art in img.ArtFiles)
						if (art.filename != "LevelArt")
							art.filename = new Uri(baseUri, art.filename).LocalPath;
					img.MapFile.filename = new Uri(baseUri, img.MapFile.filename).LocalPath;
					if (!string.IsNullOrEmpty(img.MapFile.dplcfile))
						img.MapFile.dplcfile = new Uri(baseUri, img.MapFile.dplcfile).LocalPath;
				}
			Definition.Language = Definition.Language ?? "cs";
			Definition.Name = Definition.Name ?? string.Empty;
			Definition.Namespace = Definition.Namespace ?? string.Empty;
			Definition.Properties = Definition.Properties ?? new PropertyList();
			Definition.Properties.Items = Definition.Properties.Items ?? new Property[0];
			Definition.Subtypes = Definition.Subtypes ?? new SubtypeList();
			Definition.Subtypes.Items = Definition.Subtypes.Items ?? new Subtype[0];
			Definition.TypeName = Definition.TypeName ?? string.Empty;
		}

		private void ResetImageLists()
		{
			List<object> items = new List<object>(Definition.Images.Items.Length);
			images = new List<Sprite>(Definition.Images.Items.Length);
			for (int i = 0; i < Definition.Images.Items.Length; i++)
			{
				items.Add(Definition.Images.Items[i].id);
				images.Add(default(Sprite));
				RedrawImage(i);
			}
			object[] itemArray = items.ToArray();
			int tmp = defImage.SelectedIndex;
			defImage.Items.Clear();
			defImage.Items.AddRange(itemArray);
			defImage.SelectedIndex = Math.Min(tmp, itemArray.Length - 1);
			tmp = selectedImage.SelectedIndex;
			selectedImage.Items.Clear();
			selectedImage.Items.AddRange(itemArray);
			selectedImage.SelectedIndex = Math.Min(tmp, itemArray.Length - 1);
			tmp = subtypeImage.SelectedIndex;
			subtypeImage.Items.Clear();
			subtypeImage.Items.AddRange(itemArray);
			if (selectedSubtype.SelectedIndex > -1)
				subtypeImage.SelectedIndex = Math.Min(tmp, itemArray.Length - 1);
			tmp = imageRefImage.SelectedIndex;
			imageRefImage.Items.Clear();
			imageRefImage.Items.AddRange(itemArray);
			if (selectedDisplayOption.SelectedIndex > -1 & selectedImageRef.SelectedIndex > -1)
				imageRefImage.SelectedIndex = Math.Min(tmp, itemArray.Length - 1);
		}

		private void RedrawImage(int index)
		{
			XMLImage item = Definition.Images.Items[index];
			try
			{
				if (item is ImageFromBitmap)
					images[index] = new Sprite(new BitmapBits(((ImageFromBitmap)item).filename), item.offset.ToPoint());
				else if (item is ImageFromMappings)
				{
					ImageFromMappings img = (ImageFromMappings)item;
					MultiFileIndexer<byte> artfiles = new MultiFileIndexer<byte>();
					foreach (ArtFile artfile in img.ArtFiles)
						if (!string.IsNullOrEmpty(artfile.filename))
							artfiles.AddFile(new List<byte>(ObjectHelper.OpenArtFile(artfile.filename,
								artfile.compression == CompressionType.Invalid ? CompressionType.Nemesis : artfile.compression)),
								artfile.offsetSpecified ? artfile.offset : -1);
					Sprite result = ObjectHelper.UnknownObject;
					switch (img.MapFile.type)
					{
						case MapFileType.Binary:
							if (string.IsNullOrEmpty(img.MapFile.dplcfile))
								result = ObjectHelper.MapToBmp(artfiles.ToArray(), File.ReadAllBytes(img.MapFile.filename),
									img.MapFile.frame, img.MapFile.startpal, img.MapFile.version);
							else
								result = ObjectHelper.MapDPLCToBmp(artfiles.ToArray(), File.ReadAllBytes(img.MapFile.filename), img.MapFile.version,
									File.ReadAllBytes(img.MapFile.dplcfile), img.MapFile.dplcver, img.MapFile.frame, img.MapFile.startpal);
							break;
						case MapFileType.ASM:
							if (string.IsNullOrEmpty(img.MapFile.label))
							{
								if (string.IsNullOrEmpty(img.MapFile.dplcfile))
									result = ObjectHelper.MapASMToBmp(artfiles.ToArray(), img.MapFile.filename,
										img.MapFile.frame, img.MapFile.startpal, img.MapFile.version);
								else
									result = ObjectHelper.MapASMDPLCToBmp(artfiles.ToArray(), img.MapFile.filename, img.MapFile.version,
										img.MapFile.dplcfile, img.MapFile.dplcver, img.MapFile.frame, img.MapFile.startpal);
							}
							else
							{
								if (string.IsNullOrEmpty(img.MapFile.dplcfile))
									result = ObjectHelper.MapASMToBmp(artfiles.ToArray(), img.MapFile.filename,
										img.MapFile.label, img.MapFile.startpal, img.MapFile.version);
								else
									result = ObjectHelper.MapASMDPLCToBmp(artfiles.ToArray(), img.MapFile.filename, img.MapFile.label, img.MapFile.version,
										img.MapFile.dplcfile, img.MapFile.dplclabel, img.MapFile.dplcver, img.MapFile.startpal);
							}
							break;
					}
					if (!img.offset.IsEmpty)
						result.Offset = new Point(result.X + img.offset.X, result.Y + img.offset.Y);
					images[index] = result;
				}
				else if (item is ImageFromSprite)
				{
					Sprite result = LevelData.Sprites[((ImageFromSprite)item).frame];
					if (!item.offset.IsEmpty)
						result.Offset = new Point(result.X + item.offset.X, result.Y + item.offset.Y);
					images[index] = result;
				}
			}
			catch
			{
				Sprite result = ObjectHelper.UnknownObject;
				if (!item.offset.IsEmpty)
					result.Offset = new Point(result.X + item.offset.X, result.Y + item.offset.Y);
				images[index] = result;
			}
			if (defImage.SelectedIndex == index)
				ImagePreview(defImagePreview, index);
			if (selectedImage.SelectedIndex == index)
				ImagePreview(selectedImagePreview, index);
			if (subtypeImage.SelectedIndex == index)
				ImagePreview(subtypeImagePreview, index);
		}

		private void ImagePreview(PictureBox ctrl, int image)
		{
			BitmapBits bits = new BitmapBits(ctrl.Size);
			Point center = new Point(bits.Width / 2, bits.Height / 2);
			bits.DrawSprite(images[image], center);
			bits.DrawLine(64, center.X - 2, center.Y, center.X + 2, center.Y);
			bits.DrawLine(64, center.X, center.Y - 2, center.X, center.Y + 2);
			Color[] pal = new Color[256];
			Array.Copy(LevelData.BmpPal.Entries, pal, 256);
			pal[64] = Color.Fuchsia;
			ctrl.Image = bits.ToBitmap(pal);
		}
		#endregion

		#region Tabs
		#region General Tab
		private void defName_TextChanged(object sender, EventArgs e)
		{
			Definition.Name = defName.Text;
		}

		private string MakeIdentifier(string name)
		{
			StringBuilder result = new StringBuilder();
			foreach (char item in name)
				if ((item >= '0' & item <= '9') | (item >= 'A' & item <= 'Z') | (item >= 'a' & item <= 'z'))
					result.Append(item);
			if (result[0] >= '0' & result[0] <= '9')
				result.Insert(0, '_');
			return result.ToString();
		}

		private void defNamespace_TextChanged(object sender, EventArgs e)
		{
			defNamespace.Text = MakeNamespace(defNamespace.Text);
			Definition.Namespace = defNamespace.Text;
		}

		private string MakeNamespace(string name)
		{
			StringBuilder result = new StringBuilder();
			foreach (char item in name)
				if ((item >= '0' & item <= '9') | (item >= 'A' & item <= 'Z') | (item >= 'a' & item <= 'z') | item == '.')
					result.Append(item);
			if ((result[0] >= '0' & result[0] <= '9') | result[0] == '.')
				result.Insert(0, '_');
			return result.ToString();
		}

		private void defType_TextChanged(object sender, EventArgs e)
		{
			defType.Text = MakeIdentifier(defType.Text);
			Definition.TypeName = defType.Text;
		}

		private void defLanguage_SelectedIndexChanged(object sender, EventArgs e)
		{
			switch (defLanguage.SelectedIndex)
			{
				case 0:
					Definition.Language = "cs";
					break;
				case 1:
					Definition.Language = "vb";
					break;
			}
		}

		private void defImage_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (defImage.SelectedIndex == -1)
			{
				Definition.Image = string.Empty;
				defImagePreview.Image = null;
			}
			else
			{
				Definition.Image = Definition.Images.Items[defImage.SelectedIndex].id;
				ImagePreview(defImagePreview, defImage.SelectedIndex);
			}
		}

		private void defRemember_CheckedChanged(object sender, EventArgs e)
		{
			Definition.RememberState = defRemember.Checked;
		}

		private void defDebug_CheckedChanged(object sender, EventArgs e)
		{
			Definition.Debug = defDebug.Checked;
		}

		private void defSubtype_ValueChanged(object sender, EventArgs e)
		{
			Definition.DefaultSubtypeValue = (byte)defSubtype.Value;
		}
		#endregion

		#region Images Tab
		private void selectedImage_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (selectedImage.SelectedIndex == -1)
			{
				imageControls.Enabled = deleteImageButton.Enabled = mappingsImageControls.Visible = bitmapImageControls.Visible = false;
				return;
			}
			imageControls.Enabled = deleteImageButton.Enabled = true;
			XMLImage img = Definition.Images.Items[selectedImage.SelectedIndex];
			imageName.Text = img.id;
			imageOffsetX.Value = img.offset.X;
			imageOffsetY.Value = img.offset.Y;
			ImagePreview(selectedImagePreview, selectedImage.SelectedIndex);
			if (img is ImageFromBitmap)
				imageType.SelectedIndex = 1;
			else if (img is ImageFromSprite)
				imageType.SelectedIndex = 2;
			else
				imageType.SelectedIndex = 0;
			setupImageType();
		}

		private void setupImageType()
		{
			XMLImage img = Definition.Images.Items[selectedImage.SelectedIndex];
			switch (imageType.SelectedIndex)
			{
				case 1: // ImageFromBitmap
					mappingsImageControls.Visible = spriteImageControls.Visible = false;
					bitmapImageControls.Visible = true;
					bitmapFilename.FileName = ((ImageFromBitmap)img).filename;
					break;
				case 2: // ImageFromSprite
					mappingsImageControls.Visible = bitmapImageControls.Visible = false;
					spriteImageControls.Visible = true;
					spriteNum.Value = ((ImageFromSprite)img).frame;
					break;
				default: // ImageFromMappings
					ImageFromMappings mapimg = (ImageFromMappings)img;
					artRemoveButton.Enabled = artUpButton.Enabled = artDownButton.Enabled = useLevelArt.Enabled = artFilename.Enabled = artCompression.Enabled = artOffset.Enabled = defaultArtOffset.Enabled = false;
					bitmapImageControls.Visible = spriteImageControls.Visible = false;
					mappingsImageControls.Visible = true;
					artList.Items.Clear();
					artList.BeginUpdate();
					foreach (ArtFile art in mapimg.ArtFiles)
						artList.Items.Add(Path.GetFileName(art.filename) + " (" + (art.compression == CompressionType.Invalid ? "Default" : art.compression.ToString()) + ")");
					artList.EndUpdate();
					switch (mapimg.MapFile.type)
					{
						case MapFileType.ASM:
							mappingsASM.Checked = true;
							mappingsLabel.Enabled = dplcLabel.Enabled = true;
							break;
						default:
							mappingsBinary.Checked = true;
							mappingsLabel.Enabled = dplcLabel.Enabled = false;
							break;
					}
					mappingsFile.FileName = mapimg.MapFile.filename;
					mappingsLabel.Text = mapimg.MapFile.label;
					switch (mapimg.MapFile.version)
					{
						case EngineVersion.S1:
							mappingsFormat.SelectedIndex = 1;
							break;
						case EngineVersion.S2:
							mappingsFormat.SelectedIndex = 2;
							break;
						case EngineVersion.S3K:
							mappingsFormat.SelectedIndex = 3;
							break;
						default:
							mappingsFormat.SelectedIndex = 0;
							break;
					}
					dplcFile.FileName = mapimg.MapFile.dplcfile;
					dplcLabel.Text = mapimg.MapFile.dplclabel;
					switch (mapimg.MapFile.dplcver)
					{
						case EngineVersion.S1:
							dplcFormat.SelectedIndex = 1;
							break;
						case EngineVersion.S2:
							dplcFormat.SelectedIndex = 2;
							break;
						case EngineVersion.S3K:
							dplcFormat.SelectedIndex = 3;
							break;
						default:
							dplcFormat.SelectedIndex = 0;
							break;
					}
					mappingsFrame.Value = mapimg.MapFile.frame;
					mappingsPalette.Value = mapimg.MapFile.startpal;
					mappingsFrame.Enabled = (mappingsLabel.Text == string.Empty) | mappingsBinary.Checked;
					break;
			}
		}

		private void addImageButton_Click(object sender, EventArgs e)
		{
			List<XMLImage> images = new List<XMLImage>(Definition.Images.Items);
			images.Add(new ImageFromMappings() { ArtFiles = new ArtFile[0], MapFile = new MapFile() { filename = string.Empty, label = string.Empty, dplcfile = string.Empty, dplclabel = string.Empty }, id = "newImage" + (images.Count + 1).ToString(NumberFormatInfo.InvariantInfo) });
			Definition.Images.Items = images.ToArray();
			ResetImageLists();
			selectedImage.SelectedIndex = images.Count - 1;
		}

		private void deleteImageButton_Click(object sender, EventArgs e)
		{
			List<XMLImage> images = new List<XMLImage>(Definition.Images.Items);
			images.RemoveAt(selectedImage.SelectedIndex);
			Definition.Images.Items = images.ToArray();
			ResetImageLists();
		}

		private void imageName_TextChanged(object sender, EventArgs e)
		{
			imageName.Text = MakeIdentifier(imageName.Text);
			if (Definition.Images.Items[selectedImage.SelectedIndex].id != imageName.Text)
			{
				foreach (XMLImage img in Definition.Images.Items)
					if (imageName.Text == img.id)
					{
						MessageBox.Show(this, "ID already in use!");
						return;
					}
				Definition.Images.Items[selectedImage.SelectedIndex].id = imageName.Text;
				ResetImageLists();
			}
		}

		private void imageOffsetX_ValueChanged(object sender, EventArgs e)
		{
			XMLImage img = Definition.Images.Items[selectedImage.SelectedIndex];
			if (img.offset.X != imageOffsetX.Value)
			{
				img.offset = new XmlPoint((int)imageOffsetX.Value, img.offset.Y);
				ResetImageLists();
			}
		}

		private void imageOffsetY_ValueChanged(object sender, EventArgs e)
		{
			XMLImage img = Definition.Images.Items[selectedImage.SelectedIndex];
			if (img.offset.Y != imageOffsetY.Value)
			{
				img.offset = new XmlPoint(img.offset.X, (int)imageOffsetY.Value);
				ResetImageLists();
			}
		}

		private void imageType_SelectedIndexChanged(object sender, EventArgs e)
		{
			XMLImage img = Definition.Images.Items[selectedImage.SelectedIndex];
			XMLImage newimg;
			switch (imageType.SelectedIndex)
			{
				case 1:
					if (img is ImageFromBitmap) return;
					newimg = new ImageFromBitmap();
					break;
				case 2:
					if (img is ImageFromSprite) return;
					newimg = new ImageFromSprite();
					break;
				default:
					if (img is ImageFromMappings) return;
					newimg = new ImageFromMappings();
					break;
			}
			newimg.id = img.id;
			newimg.offset = img.offset;
			Definition.Images.Items[selectedImage.SelectedIndex] = newimg;
			setupImageType();
		}

		#region Mappings Image Controls
		private void artList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (artList.SelectedIndex == -1)
			{
				artRemoveButton.Enabled = artUpButton.Enabled = artDownButton.Enabled = useLevelArt.Enabled = artFilename.Enabled = artCompression.Enabled = artOffset.Enabled = defaultArtOffset.Enabled = false;
				return;
			}
			ImageFromMappings img = (ImageFromMappings)Definition.Images.Items[selectedImage.SelectedIndex];
			artRemoveButton.Enabled = useLevelArt.Enabled = defaultArtOffset.Enabled = true;
			artUpButton.Enabled = artList.SelectedIndex > 0;
			artDownButton.Enabled = artList.SelectedIndex < img.ArtFiles.Length - 1;
			ArtFile art = img.ArtFiles[artList.SelectedIndex];
			if (art.filename == "LevelArt")
			{
				useLevelArt.Checked = true;
				artFilename.Enabled = artCompression.Enabled = false;
			}
			else
			{
				useLevelArt.Checked = false;
				artFilename.Enabled = artCompression.Enabled = true;
			}
			artFilename.FileName = art.filename;
			artCompression.SelectedIndex = (int)art.compression;
			if (art.offset == -1 | !art.offsetSpecified)
			{
				defaultArtOffset.Checked = true;
				artOffset.Enabled = false;
				artOffset.Value = 0;
			}
			else
			{
				artOffset.Value = art.offset;
				artOffset.Enabled = true;
				defaultArtOffset.Checked = false;
			}
		}

		private void artAddButton_Click(object sender, EventArgs e)
		{
			ImageFromMappings img = (ImageFromMappings)Definition.Images.Items[selectedImage.SelectedIndex];
			List<ArtFile> artfiles = new List<ArtFile>(img.ArtFiles);
			artfiles.Add(new ArtFile() { filename = string.Empty, offsetSpecified = false });
			img.ArtFiles = artfiles.ToArray();
			artList.Items.Add(" (Default)");
			artList.SelectedIndex = artfiles.Count - 1;
		}

		private void artRemoveButton_Click(object sender, EventArgs e)
		{
			ImageFromMappings img = (ImageFromMappings)Definition.Images.Items[selectedImage.SelectedIndex];
			List<ArtFile> artfiles = new List<ArtFile>(img.ArtFiles);
			artfiles.RemoveAt(artList.SelectedIndex);
			img.ArtFiles = artfiles.ToArray();
			artList.Items.RemoveAt(artList.SelectedIndex);
			RedrawImage(selectedImage.SelectedIndex);
		}

		private void artUpButton_Click(object sender, EventArgs e)
		{
			ImageFromMappings img = (ImageFromMappings)Definition.Images.Items[selectedImage.SelectedIndex];
			List<ArtFile> artfiles = new List<ArtFile>(img.ArtFiles);
			int i = artList.SelectedIndex;
			ArtFile art = artfiles[i];
			artfiles.RemoveAt(i);
			artfiles.Insert(i - 1, art);
			img.ArtFiles = artfiles.ToArray();
			artList.Items.RemoveAt(i);
			artList.Items.Insert(i - 1, Path.GetFileName(art.filename) + " (" + (art.compression == CompressionType.Invalid ? "Default" : art.compression.ToString()) + ")");
			artList.SelectedIndex = i - 1;
			RedrawImage(selectedImage.SelectedIndex);
		}

		private void artDownButton_Click(object sender, EventArgs e)
		{
			ImageFromMappings img = (ImageFromMappings)Definition.Images.Items[selectedImage.SelectedIndex];
			List<ArtFile> artfiles = new List<ArtFile>(img.ArtFiles);
			int i = artList.SelectedIndex;
			ArtFile art = artfiles[i];
			artfiles.RemoveAt(i);
			artfiles.Insert(i + 1, art);
			img.ArtFiles = artfiles.ToArray();
			artList.Items.RemoveAt(i);
			artList.Items.Insert(i + 1, Path.GetFileName(art.filename) + " (" + (art.compression == CompressionType.Invalid ? "Default" : art.compression.ToString()) + ")");
			artList.SelectedIndex = i + 1;
			RedrawImage(selectedImage.SelectedIndex);
		}

		private void useLevelArt_CheckedChanged(object sender, EventArgs e)
		{
			ArtFile art = ((ImageFromMappings)Definition.Images.Items[selectedImage.SelectedIndex]).ArtFiles[artList.SelectedIndex];
			if (useLevelArt.Checked)
			{
				artFilename.Enabled = artCompression.Enabled = false;
				art.filename = artFilename.FileName = "LevelArt";
			}
			else
			{
				artFilename.Enabled = artCompression.Enabled = true;
				art.filename = artFilename.FileName = string.Empty;
			}
			artList.Items[artList.SelectedIndex] = Path.GetFileName(art.filename) + " (" + (art.compression == CompressionType.Invalid ? "Default" : art.compression.ToString()) + ")";
			RedrawImage(selectedImage.SelectedIndex);
		}

		private void artFilename_FileNameChanged(object sender, EventArgs e)
		{
			ArtFile art = ((ImageFromMappings)Definition.Images.Items[selectedImage.SelectedIndex]).ArtFiles[artList.SelectedIndex];
			art.filename = artFilename.FileName;
			artList.Items[artList.SelectedIndex] = Path.GetFileName(art.filename) + " (" + (art.compression == CompressionType.Invalid ? "Default" : art.compression.ToString()) + ")";
			RedrawImage(selectedImage.SelectedIndex);
		}

		private void artCompression_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (artCompression.SelectedIndex == -1) return;
			ArtFile art = ((ImageFromMappings)Definition.Images.Items[selectedImage.SelectedIndex]).ArtFiles[artList.SelectedIndex];
			art.compression = (CompressionType)artCompression.SelectedIndex;
			artList.Items[artList.SelectedIndex] = Path.GetFileName(art.filename) + " (" + (art.compression == CompressionType.Invalid ? "Default" : art.compression.ToString()) + ")";
			RedrawImage(selectedImage.SelectedIndex);
		}

		private void artOffset_ValueChanged(object sender, EventArgs e)
		{
			ArtFile art = ((ImageFromMappings)Definition.Images.Items[selectedImage.SelectedIndex]).ArtFiles[artList.SelectedIndex];
			art.offset = (int)artOffset.Value;
			RedrawImage(selectedImage.SelectedIndex);
		}

		private void defaultArtOffset_CheckedChanged(object sender, EventArgs e)
		{
			ArtFile art = ((ImageFromMappings)Definition.Images.Items[selectedImage.SelectedIndex]).ArtFiles[artList.SelectedIndex];
			if (defaultArtOffset.Checked)
			{
				art.offset = -1;
				art.offsetSpecified = false;
				artOffset.Enabled = false;
			}
			else
			{
				art.offset = (int)artOffset.Value;
				art.offsetSpecified = true;
				artOffset.Enabled = true;
			}
			RedrawImage(selectedImage.SelectedIndex);
		}

		private void mappingsType_CheckedChanged(object sender, EventArgs e)
		{
			if (mappingsASM.Checked)
			{
				mappingsLabel.Enabled = dplcLabel.Enabled = true;
				mappingsFrame.Enabled = mappingsLabel.Text == string.Empty;
				((ImageFromMappings)Definition.Images.Items[selectedImage.SelectedIndex]).MapFile.type = MapFileType.ASM;
			}
			else
			{
				mappingsLabel.Enabled = dplcLabel.Enabled = false;
				mappingsFrame.Enabled = true;
				((ImageFromMappings)Definition.Images.Items[selectedImage.SelectedIndex]).MapFile.type = MapFileType.Binary;
			}
			RedrawImage(selectedImage.SelectedIndex);
		}

		private void mappingsFile_FileNameChanged(object sender, EventArgs e)
		{
			((ImageFromMappings)Definition.Images.Items[selectedImage.SelectedIndex]).MapFile.filename = mappingsFile.FileName;
			RedrawImage(selectedImage.SelectedIndex);
		}

		private void mappingsLabel_TextChanged(object sender, EventArgs e)
		{
			mappingsLabel.Text = MakeIdentifier(mappingsLabel.Text);
			mappingsFrame.Enabled = (mappingsLabel.Text == string.Empty) | mappingsBinary.Checked;
			((ImageFromMappings)Definition.Images.Items[selectedImage.SelectedIndex]).MapFile.label = mappingsLabel.Text;
			RedrawImage(selectedImage.SelectedIndex);
		}

		private void mappingsFormat_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (mappingsFormat.SelectedIndex == -1) return;
			MapFile map = ((ImageFromMappings)Definition.Images.Items[selectedImage.SelectedIndex]).MapFile;
			switch (mappingsFormat.SelectedIndex)
			{
				case 1:
					map.version = EngineVersion.S1;
					break;
				case 2:
					map.version = EngineVersion.S2;
					break;
				case 3:
					map.version = EngineVersion.S3K;
					break;
				default:
					map.version = EngineVersion.Invalid;
					break;
			}
			RedrawImage(selectedImage.SelectedIndex);
		}

		private void dplcFile_FileNameChanged(object sender, EventArgs e)
		{
			((ImageFromMappings)Definition.Images.Items[selectedImage.SelectedIndex]).MapFile.dplcfile = dplcFile.FileName;
			RedrawImage(selectedImage.SelectedIndex);
		}

		private void dplcLabel_TextChanged(object sender, EventArgs e)
		{
			dplcLabel.Text = MakeIdentifier(dplcLabel.Text);
			((ImageFromMappings)Definition.Images.Items[selectedImage.SelectedIndex]).MapFile.dplclabel = dplcLabel.Text;
			RedrawImage(selectedImage.SelectedIndex);
		}

		private void dplcFormat_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (dplcFormat.SelectedIndex == -1) return;
			MapFile map = ((ImageFromMappings)Definition.Images.Items[selectedImage.SelectedIndex]).MapFile;
			switch (dplcFormat.SelectedIndex)
			{
				case 1:
					map.dplcver = EngineVersion.S1;
					break;
				case 2:
					map.dplcver = EngineVersion.S2;
					break;
				case 3:
					map.dplcver = EngineVersion.S3K;
					break;
				default:
					map.dplcver = EngineVersion.Invalid;
					break;
			}
			RedrawImage(selectedImage.SelectedIndex);
		}

		private void mappingsFrame_ValueChanged(object sender, EventArgs e)
		{
			((ImageFromMappings)Definition.Images.Items[selectedImage.SelectedIndex]).MapFile.frame = (int)mappingsFrame.Value;
			RedrawImage(selectedImage.SelectedIndex);
		}

		private void mappingsPalette_ValueChanged(object sender, EventArgs e)
		{
			((ImageFromMappings)Definition.Images.Items[selectedImage.SelectedIndex]).MapFile.startpal = (int)mappingsPalette.Value;
			RedrawImage(selectedImage.SelectedIndex);
		}
		#endregion

		#region Bitmap Image Controls
		private void bitmapFilename_FileNameChanged(object sender, EventArgs e)
		{
			((ImageFromBitmap)Definition.Images.Items[selectedImage.SelectedIndex]).filename = bitmapFilename.FileName;
			RedrawImage(selectedImage.SelectedIndex);
		}
		#endregion

		#region Sprite Image Controls
		private void spriteNum_ValueChanged(object sender, EventArgs e)
		{
			((ImageFromSprite)Definition.Images.Items[selectedImage.SelectedIndex]).frame = (int)spriteNum.Value;
			RedrawImage(selectedImage.SelectedIndex);
		}
		#endregion
		#endregion

		#region Subtypes Tab
		private void selectedSubtype_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (selectedSubtype.SelectedIndex == -1)
			{
				deleteSubtypeButton.Enabled = subtypeControls.Enabled = false;
				return;
			}
			deleteSubtypeButton.Enabled = subtypeControls.Enabled = true;
			Subtype sub = Definition.Subtypes.Items[selectedSubtype.SelectedIndex];
			subtypeID.Value = sub.subtype;
			subtypeName.Text = sub.name;
			subtypeImage.SelectedIndex = subtypeImage.Items.IndexOf(sub.image);
		}

		private void addSubtypeButton_Click(object sender, EventArgs e)
		{
			List<Subtype> subtypes = new List<Subtype>(Definition.Subtypes.Items);
			subtypes.Add(new Subtype() { name = string.Empty, image = string.Empty });
			Definition.Subtypes.Items = subtypes.ToArray();
			selectedSubtype.Items.Add("00: ");
			selectedSubtype.SelectedIndex = subtypes.Count - 1;
		}

		private void deleteSubtypeButton_Click(object sender, EventArgs e)
		{
			List<Subtype> subtypes = new List<Subtype>(Definition.Subtypes.Items);
			subtypes.RemoveAt(selectedSubtype.SelectedIndex);
			Definition.Subtypes.Items = subtypes.ToArray();
			selectedSubtype.Items.RemoveAt(selectedSubtype.SelectedIndex);
		}

		private void subtypeID_ValueChanged(object sender, EventArgs e)
		{
			Subtype sub = Definition.Subtypes.Items[selectedSubtype.SelectedIndex];
			sub.subtype = (byte)subtypeID.Value;
			selectedSubtype.Items[selectedSubtype.SelectedIndex] = sub.id + ": " + sub.name;
		}

		private void subtypeName_TextChanged(object sender, EventArgs e)
		{
			Subtype sub = Definition.Subtypes.Items[selectedSubtype.SelectedIndex];
			sub.name = subtypeName.Text;
			selectedSubtype.Items[selectedSubtype.SelectedIndex] = sub.id + ": " + sub.name;
		}

		private void subtypeImage_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (subtypeImage.SelectedIndex == -1) return;
			Definition.Subtypes.Items[selectedSubtype.SelectedIndex].image = Definition.Images.Items[subtypeImage.SelectedIndex].id;
			ImagePreview(subtypeImagePreview, subtypeImage.SelectedIndex);
		}
		#endregion

		#region Enums Tab
		private void selectedEnum_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (selectedEnum.SelectedIndex == -1)
			{
				enumControls.Enabled = false;
				return;
			}
			XMLEnum enu = Definition.Enums.Items[selectedEnum.SelectedIndex];
			enumMemberRemoveButton.Enabled = enumMemberUpButton.Enabled = enumMemberDownButton.Enabled = enumMemberName.Enabled = enumMemberValue.Enabled = enumMemberDefault.Enabled = false;
			enumName.Text = enu.name;
			enumMemberList.Items.Clear();
			enumMemberList.BeginUpdate();
			foreach (EnumMember mem in enu.Items)
				enumMemberList.Items.Add(mem.name);
			enumMemberList.EndUpdate();
			enumControls.Enabled = true;
		}

		private void addEnumButton_Click(object sender, EventArgs e)
		{
			List<XMLEnum> enums = new List<XMLEnum>(Definition.Enums.Items);
			enums.Add(new XMLEnum() { Items = new EnumMember[0], name = "enum" + (enums.Count + 1) });
			Definition.Enums.Items = enums.ToArray();
			selectedEnum.Items.Add("enum" + enums.Count);
			propertyValueType.Items.Add("enum" + enums.Count);
			selectedEnum.SelectedIndex = enums.Count - 1;
		}

		private void deleteEnumButton_Click(object sender, EventArgs e)
		{
			List<XMLEnum> enums = new List<XMLEnum>(Definition.Enums.Items);
			enums.RemoveAt(selectedEnum.SelectedIndex);
			Definition.Enums.Items = enums.ToArray();
			propertyValueType.Items.RemoveAt(selectedEnum.SelectedIndex);
			selectedEnum.Items.RemoveAt(selectedEnum.SelectedIndex);
			if (selectedProperty.SelectedIndex > -1)
				propertyValueType.SelectedIndex = propertyValueType.Items.IndexOf(Definition.Properties.Items[selectedProperty.SelectedIndex].type);
		}

		private void enumName_TextChanged(object sender, EventArgs e)
		{
			enumName.Text = MakeIdentifier(enumName.Text);
			Definition.Enums.Items[selectedEnum.SelectedIndex].name = enumName.Text;
			selectedEnum.Items[selectedEnum.SelectedIndex] = enumName.Text;
			propertyValueType.Items[selectedEnum.SelectedIndex + 3] = enumName.Text;
		}

		private void enumMemberList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (suppressEvents) return;
			if (enumMemberList.SelectedIndex == -1)
			{
				enumMemberRemoveButton.Enabled = enumMemberUpButton.Enabled = enumMemberDownButton.Enabled = useLevelArt.Enabled = enumMemberName.Enabled = enumMemberValue.Enabled = enumMemberDefault.Enabled = false;
				return;
			}
			XMLEnum enu = Definition.Enums.Items[selectedEnum.SelectedIndex];
			enumMemberRemoveButton.Enabled = useLevelArt.Enabled = defaultArtOffset.Enabled = true;
			enumMemberUpButton.Enabled = enumMemberList.SelectedIndex > 0;
			enumMemberDownButton.Enabled = enumMemberList.SelectedIndex < enu.Items.Length - 1;
			EnumMember enumMember = enu.Items[enumMemberList.SelectedIndex];
			enumMemberName.Text = enumMember.name;
			enumMemberValue.Value = enumMember.value;
			enumMemberDefault.Checked = !enumMember.valueSpecified;
			enumMemberValue.Enabled = enumMember.valueSpecified;
			enumMemberName.Enabled = enumMemberDefault.Enabled = true;
		}

		private void enumMemberAddButton_Click(object sender, EventArgs e)
		{
			XMLEnum enu = Definition.Enums.Items[selectedEnum.SelectedIndex];
			List<EnumMember> enumMembers = new List<EnumMember>(enu.Items);
			enumMembers.Add(new EnumMember() { name = "enumMember" + (enumMembers.Count + 1), valueSpecified = false });
			enu.Items = enumMembers.ToArray();
			enumMemberList.Items.Add("enumMember" + enumMembers.Count);
			enumMemberList.SelectedIndex = enumMembers.Count - 1;
		}

		private void enumMemberRemoveButton_Click(object sender, EventArgs e)
		{
			XMLEnum enu = Definition.Enums.Items[selectedEnum.SelectedIndex];
			List<EnumMember> enumMembers = new List<EnumMember>(enu.Items);
			enumMembers.RemoveAt(enumMemberList.SelectedIndex);
			enu.Items = enumMembers.ToArray();
			enumMemberList.Items.RemoveAt(enumMemberList.SelectedIndex);
		}

		private void enumMemberUpButton_Click(object sender, EventArgs e)
		{
			XMLEnum enu = Definition.Enums.Items[selectedEnum.SelectedIndex];
			List<EnumMember> enumMembers = new List<EnumMember>(enu.Items);
			int i = enumMemberList.SelectedIndex;
			EnumMember enumMember = enumMembers[i];
			enumMembers.RemoveAt(i);
			enumMembers.Insert(i - 1, enumMember);
			enu.Items = enumMembers.ToArray();
			enumMemberList.Items.RemoveAt(i);
			enumMemberList.Items.Insert(i - 1, enumMember.name);
			enumMemberList.SelectedIndex = i - 1;
		}

		private void enumMemberDownButton_Click(object sender, EventArgs e)
		{
			XMLEnum enu = Definition.Enums.Items[selectedEnum.SelectedIndex];
			List<EnumMember> enumMembers = new List<EnumMember>(enu.Items);
			int i = enumMemberList.SelectedIndex;
			EnumMember enumMember = enumMembers[i];
			enumMembers.RemoveAt(i);
			enumMembers.Insert(i + 1, enumMember);
			enu.Items = enumMembers.ToArray();
			enumMemberList.Items.RemoveAt(i);
			enumMemberList.Items.Insert(i + 1, enumMember.name);
			enumMemberList.SelectedIndex = i + 1;
		}

		private void enumMemberName_TextChanged(object sender, EventArgs e)
		{
			Definition.Enums.Items[selectedEnum.SelectedIndex].Items[enumMemberList.SelectedIndex].name = enumMemberName.Text;
			suppressEvents = true;
			enumMemberList.Items[enumMemberList.SelectedIndex] = enumMemberName.Text;
			suppressEvents = false;
		}

		private void enumMemberValue_ValueChanged(object sender, EventArgs e)
		{
			Definition.Enums.Items[selectedEnum.SelectedIndex].Items[enumMemberList.SelectedIndex].value = (byte)enumMemberValue.Value;
		}

		private void enumMemberDefault_CheckedChanged(object sender, EventArgs e)
		{
			Definition.Enums.Items[selectedEnum.SelectedIndex].Items[enumMemberList.SelectedIndex].valueSpecified = enumMemberValue.Enabled = !enumMemberDefault.Checked;
		}
		#endregion

		#region Properties Tab
		private void selectedProperty_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (selectedProperty.SelectedIndex == -1)
			{
				propertyControls.Enabled = deletePropertyButton.Enabled = bitsPropertyControls.Visible = customPropertyControls.Visible = false;
				return;
			}
			propertyControls.Enabled = deletePropertyButton.Enabled = true;
			Property prop = Definition.Properties.Items[selectedProperty.SelectedIndex];
			propertyName.Text = prop.name;
			displayName.Text = prop.displayname ?? prop.name;
			useDisplayName.Checked = !string.IsNullOrEmpty(prop.displayname);
			propertyValueType.SelectedIndex = propertyValueType.Items.IndexOf(prop.type);
			propertyDescription.Text = prop.description;
			if (prop is CustomProperty)
				propertyType.SelectedIndex = 1;
			else
				propertyType.SelectedIndex = 0;
			setupPropertyType();
		}

		private void setupPropertyType()
		{
			Property prop = Definition.Properties.Items[selectedProperty.SelectedIndex];
			switch (propertyType.SelectedIndex)
			{
				case 1: // CustomProperty
					bitsPropertyControls.Visible = false;
					customPropertyControls.Visible = true;
					CustomProperty cust = (CustomProperty)prop;
					getMethod.Text = cust.get;
					setMethod.Text = cust.set;
					break;
				default:
					customPropertyControls.Visible = false;
					bitsPropertyControls.Visible = true;
					BitsProperty bits = (BitsProperty)prop;
					bitLength.Maximum = 8;
					bitLength.Value = bits.length;
					startBit.Value = bits.startbit;
					break;
			}
		}

		private void addPropertyButton_Click(object sender, EventArgs e)
		{
			List<Property> properties = new List<Property>(Definition.Properties.Items);
			properties.Add(new BitsProperty() { name = "property" + (properties.Count + 1), type = "int", length = 1 });
			Definition.Properties.Items = properties.ToArray();
			selectedProperty.Items.Add("property" + properties.Count);
			conditionProperty.Items.Add("property" + properties.Count);
			selectedProperty.SelectedIndex = properties.Count - 1;
		}

		private void deletePropertyButton_Click(object sender, EventArgs e)
		{
			List<Property> properties = new List<Property>(Definition.Properties.Items);
			properties.RemoveAt(selectedProperty.SelectedIndex);
			Definition.Properties.Items = properties.ToArray();
			conditionProperty.Items.RemoveAt(selectedProperty.SelectedIndex + propertyTypes.Length);
			selectedProperty.Items.RemoveAt(selectedProperty.SelectedIndex);
			if (selectedDisplayOption.SelectedIndex > -1 & selectedCondition.SelectedIndex > -1)
				conditionProperty.SelectedIndex = conditionProperty.Items.IndexOf(Definition.Display.DisplayOptions[selectedDisplayOption.SelectedIndex].Conditions[selectedCondition.SelectedIndex].property);
		}

		private void propertyName_TextChanged(object sender, EventArgs e)
		{
			propertyName.Text = MakeIdentifier(propertyName.Text);
			if (!useDisplayName.Checked) displayName.Text = propertyName.Text;
			string oldname = Definition.Properties.Items[selectedProperty.SelectedIndex].name;
			Definition.Properties.Items[selectedProperty.SelectedIndex].name = propertyName.Text;
			selectedProperty.Items[selectedProperty.SelectedIndex] = propertyName.Text;
			conditionProperty.Items[selectedProperty.SelectedIndex + propertyTypes.Length] = propertyName.Text;
			if (Definition.Display != null && Definition.Display.DisplayOptions != null)
				foreach (DisplayOption option in Definition.Display.DisplayOptions)
					if (option.Conditions != null)
						foreach (Condition cond in option.Conditions)
							if (cond.property == oldname)
								cond.property = propertyName.Text;
		}

		private void useDisplayName_CheckedChanged(object sender, EventArgs e)
		{
			if (displayName.Enabled = useDisplayName.Checked)
				Definition.Properties.Items[selectedProperty.SelectedIndex].displayname = displayName.Text;
			else
				Definition.Properties.Items[selectedProperty.SelectedIndex].displayname = null;
		}

		private void displayName_TextChanged(object sender, EventArgs e)
		{
			if (useDisplayName.Checked)
				Definition.Properties.Items[selectedProperty.SelectedIndex].displayname = displayName.Text;
		}

		private void propertyValueType_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (propertyValueType.SelectedIndex == -1) return;
			Definition.Properties.Items[selectedProperty.SelectedIndex].type = (string)propertyValueType.SelectedItem;
		}

		private void propertyDescription_TextChanged(object sender, EventArgs e)
		{
			Definition.Properties.Items[selectedProperty.SelectedIndex].description = propertyDescription.Text;
		}

		private void propertyType_SelectedIndexChanged(object sender, EventArgs e)
		{
			Property prop = Definition.Properties.Items[selectedProperty.SelectedIndex];
			Property newprop;
			switch (propertyType.SelectedIndex)
			{
				case 1:
					if (prop is CustomProperty) return;
					newprop = new CustomProperty();
					break;
				default:
					if (prop is BitsProperty) return;
					newprop = new BitsProperty() { length = 1 };
					break;
			}
			newprop.name = prop.name;
			newprop.type = prop.type;
			newprop.description = prop.description;
			Definition.Properties.Items[selectedProperty.SelectedIndex] = newprop;
			setupPropertyType();
		}

		#region Bits Property Controls
		private void startBit_ValueChanged(object sender, EventArgs e)
		{
			BitsProperty prop = (BitsProperty)Definition.Properties.Items[selectedProperty.SelectedIndex];
			prop.startbit = (int)startBit.Value;
			bitLength.Maximum = 8 - prop.startbit;
		}

		private void bitLength_ValueChanged(object sender, EventArgs e)
		{
			((BitsProperty)Definition.Properties.Items[selectedProperty.SelectedIndex]).length = (int)bitLength.Value;
		}
		#endregion

		#region Custom Property Controls
		private void getMethod_TextChanged(object sender, EventArgs e)
		{
			((CustomProperty)Definition.Properties.Items[selectedProperty.SelectedIndex]).get = getMethod.Text;
		}

		private void setMethod_TextChanged(object sender, EventArgs e)
		{
			((CustomProperty)Definition.Properties.Items[selectedProperty.SelectedIndex]).set = setMethod.Text;
		}
		#endregion
		#endregion

		#region Display Tab
		private void selectedDisplayOption_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (suppressEvents) return;
			if (selectedDisplayOption.SelectedIndex == -1)
			{
				displayOptionControls.Enabled = deleteDisplayOptionButton.Enabled = false;
				return;
			}
			conditionProperty.Enabled = conditionValueEnum.Enabled = conditionValueNum.Enabled = removeConditionButton.Enabled = imageRefControls.Enabled = removeImageRefButton.Enabled = false;
			displayOptionControls.Enabled = deleteDisplayOptionButton.Enabled = true;
			DisplayOption option = Definition.Display.DisplayOptions[selectedDisplayOption.SelectedIndex];
			selectedCondition.Items.Clear();
			selectedCondition.BeginUpdate();
			foreach (Condition cond in option.Conditions)
				selectedCondition.Items.Add(cond.property + " = " + cond.value);
			selectedCondition.EndUpdate();
			selectedImageRef.Items.Clear();
			selectedImageRef.BeginUpdate();
			foreach (ImageRef img in option.Images)
				selectedImageRef.Items.Add(img.image);
			selectedImageRef.EndUpdate();
			UpdateDisplayPreview();
		}

		private void UpdateDisplayPreview()
		{
			List<Sprite> sprites = new List<Sprite>();
			foreach (ImageRef img in Definition.Display.DisplayOptions[selectedDisplayOption.SelectedIndex].Images)
			{
				Sprite spr = new Sprite();
				for (int i = 0; i < Definition.Images.Items.Length; i++)
					if (Definition.Images.Items[i].id == img.image)
					{
						spr = new Sprite(images[i]);
						break;
					}
				int xoff = 0, yoff = 0;
				bool xflip = false, yflip = false;
				switch (img.xflip)
				{
					case FlipType.NormalFlip:
					case FlipType.NeverFlip:
						xoff = img.Offset.X;
						xflip = false;
						break;
					case FlipType.ReverseFlip:
					case FlipType.AlwaysFlip:
						xoff = -img.Offset.X;
						xflip = true;
						break;
				}
				switch (img.yflip)
				{
					case FlipType.NormalFlip:
					case FlipType.NeverFlip:
						yoff = img.Offset.Y;
						yflip = false;
						break;
					case FlipType.ReverseFlip:
					case FlipType.AlwaysFlip:
						yoff = -img.Offset.Y;
						yflip = true;
						break;
				}
				spr.X += xoff;
				spr.Y += yoff;
				spr.Image.Flip(xflip, yflip);
				sprites.Add(spr);
			}
			Sprite result = new Sprite(sprites);
			BitmapBits bits = new BitmapBits(displayPreview.Size);
			Point center = new Point(bits.Width / 2, bits.Height / 2);
			bits.DrawSprite(result, center);
			bits.DrawLine(64, center.X - 2, center.Y, center.X + 2, center.Y);
			bits.DrawLine(64, center.X, center.Y - 2, center.X, center.Y + 2);
			Color[] pal = new Color[256];
			Array.Copy(LevelData.BmpPal.Entries, pal, 256);
			pal[64] = Color.Fuchsia;
			displayPreview.Image = bits.ToBitmap(pal);
		}

		private void addDisplayOptionButton_Click(object sender, EventArgs e)
		{
			List<DisplayOption> options = new List<DisplayOption>(Definition.Display.DisplayOptions);
			options.Add(new DisplayOption() { Conditions = new Condition[0], Images = new ImageRef[0], Lines = new Line[0] });
			Definition.Display.DisplayOptions = options.ToArray();
			selectedDisplayOption.Items.Add("Always True");
			selectedDisplayOption.SelectedIndex = options.Count - 1;
		}

		private void deleteDisplayOptionButton_Click(object sender, EventArgs e)
		{
			List<DisplayOption> options = new List<DisplayOption>(Definition.Display.DisplayOptions);
			options.RemoveAt(selectedDisplayOption.SelectedIndex);
			Definition.Display.DisplayOptions = options.ToArray();
			selectedDisplayOption.Items.RemoveAt(selectedDisplayOption.SelectedIndex);
		}

		#region Conditions
		private void selectedCondition_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (selectedCondition.SelectedIndex == -1)
			{
				conditionProperty.Enabled = conditionValueEnum.Enabled = conditionValueNum.Enabled = removeConditionButton.Enabled = false;
				return;
			}
			conditionProperty.Enabled = conditionValueEnum.Enabled = conditionValueNum.Enabled = removeConditionButton.Enabled = true;
			Condition cond = Definition.Display.DisplayOptions[selectedDisplayOption.SelectedIndex].Conditions[selectedCondition.SelectedIndex];
			string val = cond.value;
			int condType = conditionProperty.Items.IndexOf(cond.property);
			conditionProperty.SelectedIndex = condType;
			if (string.IsNullOrEmpty(val)) return;
			if (condType < propertyTypes.Length)
			{
				switch (propertyTypes[condType])
				{
					case TypeCode.Boolean:
						conditionValueEnum.SelectedIndex = bool.Parse(val) ? 1 : 0;
						break;
					case TypeCode.Byte:
					case TypeCode.Int16:
					case TypeCode.Int32:
					case TypeCode.SByte:
					case TypeCode.UInt16:
					case TypeCode.UInt32:
						conditionValueNum.Value = decimal.Parse(val, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
						break;
				}
			}
			else
			{
				string propType = Definition.Properties.Items[condType - propertyTypes.Length].type;
				switch (propType)
				{
					case "bool":
						conditionValueEnum.SelectedIndex = bool.Parse(val) ? 1 : 0;
						break;
					case "byte":
					case "int":
						conditionValueNum.Value = decimal.Parse(val, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
						break;
					default:
						foreach (XMLEnum item in Definition.Enums.Items)
							if (item.name == propType)
							{
								for (int i = 0; i < item.Items.Length; i++)
									if (val == item.Items[i].name)
									{
										conditionValueEnum.SelectedIndex = i;
										break;
									}
								break;
							}
						break;
				}
			}
		}

		private void addConditionButton_Click(object sender, EventArgs e)
		{
			List<Condition> conditions = new List<Condition>(Definition.Display.DisplayOptions[selectedDisplayOption.SelectedIndex].Conditions);
			conditions.Add(new Condition() { property = (string)conditionProperty.Items[0], value = string.Empty });
			Definition.Display.DisplayOptions[selectedDisplayOption.SelectedIndex].Conditions = conditions.ToArray();
			selectedCondition.Items.Add((string)conditionProperty.Items[0] + " = ");
			selectedCondition.SelectedIndex = conditions.Count - 1;
		}

		private void removeConditionButton_Click(object sender, EventArgs e)
		{
			List<Condition> conditions = new List<Condition>(Definition.Display.DisplayOptions[selectedDisplayOption.SelectedIndex].Conditions);
			conditions.RemoveAt(selectedCondition.SelectedIndex);
			Definition.Display.DisplayOptions[selectedDisplayOption.SelectedIndex].Conditions = conditions.ToArray();
			selectedCondition.Items.RemoveAt(selectedCondition.SelectedIndex);
			UpdateDisplayOptionName();
		}

		private void UpdateDisplayOptionName()
		{
			suppressEvents = true;
			DisplayOption opt = Definition.Display.DisplayOptions[selectedDisplayOption.SelectedIndex];
			if (opt.Conditions.Length == 0)
				selectedDisplayOption.Items[selectedDisplayOption.SelectedIndex] = "Always True";
			else
			{
				List<string> strs = new List<string>(opt.Conditions.Length);
				foreach (Condition cond in opt.Conditions)
					strs.Add(cond.property + " = " + cond.value);
				selectedDisplayOption.Items[selectedDisplayOption.SelectedIndex] = string.Join(" & ", strs.ToArray());
			}
			suppressEvents = false;
		}

		private void conditionProperty_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (conditionProperty.SelectedIndex == -1)
				return;
			Condition cond = Definition.Display.DisplayOptions[selectedDisplayOption.SelectedIndex].Conditions[selectedCondition.SelectedIndex];
			cond.property = (string)conditionProperty.SelectedItem;
			if (conditionProperty.SelectedIndex < propertyTypes.Length)
			{
				switch (propertyTypes[conditionProperty.SelectedIndex])
				{
					case TypeCode.Boolean:
						conditionValueNum.Visible = false;
						conditionValueEnum.Visible = true;
						conditionValueEnum.Items.Clear();
						conditionValueEnum.Items.AddRange(new object[] { "False", "True" });
						conditionValueEnum.SelectedIndex = 0;
						break;
					case TypeCode.Byte:
						conditionValueEnum.Visible = false;
						conditionValueNum.Visible = true;
						cond.value = "0";
						conditionValueNum.Value = 0;
						conditionValueNum.Minimum = byte.MinValue;
						conditionValueNum.Maximum = byte.MaxValue;
						break;
					case TypeCode.Int16:
						conditionValueEnum.Visible = false;
						conditionValueNum.Visible = true;
						cond.value = "0";
						conditionValueNum.Value = 0;
						conditionValueNum.Minimum = short.MinValue;
						conditionValueNum.Maximum = short.MaxValue;
						break;
					case TypeCode.Int32:
						conditionValueEnum.Visible = false;
						conditionValueNum.Visible = true;
						cond.value = "0";
						conditionValueNum.Value = 0;
						conditionValueNum.Minimum = int.MinValue;
						conditionValueNum.Maximum = int.MaxValue;
						break;
					case TypeCode.SByte:
						conditionValueEnum.Visible = false;
						conditionValueNum.Visible = true;
						cond.value = "0";
						conditionValueNum.Value = 0;
						conditionValueNum.Minimum = sbyte.MinValue;
						conditionValueNum.Maximum = sbyte.MaxValue;
						break;
					case TypeCode.UInt16:
						conditionValueEnum.Visible = false;
						conditionValueNum.Visible = true;
						cond.value = "0";
						conditionValueNum.Value = 0;
						conditionValueNum.Minimum = ushort.MinValue;
						conditionValueNum.Maximum = ushort.MaxValue;
						break;
					case TypeCode.UInt32:
						conditionValueEnum.Visible = false;
						conditionValueNum.Visible = true;
						cond.value = "0";
						conditionValueNum.Value = 0;
						conditionValueNum.Minimum = uint.MinValue;
						conditionValueNum.Maximum = uint.MaxValue;
						break;
				}
			}
			else
			{
				string propType = Definition.Properties.Items[conditionProperty.SelectedIndex - propertyTypes.Length].type;
				switch (propType)
				{
					case "bool":
						conditionValueNum.Visible = false;
						conditionValueEnum.Visible = true;
						conditionValueEnum.Items.Clear();
						conditionValueEnum.Items.AddRange(new object[] { "False", "True" });
						conditionValueEnum.SelectedIndex = 0;
						break;
					case "byte":
						conditionValueEnum.Visible = false;
						conditionValueNum.Visible = true;
						cond.value = "0";
						conditionValueNum.Value = 0;
						conditionValueNum.Minimum = byte.MinValue;
						conditionValueNum.Maximum = byte.MaxValue;
						break;
					case "int":
						conditionValueEnum.Visible = false;
						conditionValueNum.Visible = true;
						cond.value = "0";
						conditionValueNum.Value = 0;
						conditionValueNum.Minimum = int.MinValue;
						conditionValueNum.Maximum = int.MaxValue;
						break;
					default:
						conditionValueNum.Visible = false;
						conditionValueEnum.Visible = true;
						conditionValueEnum.Items.Clear();
						conditionValueEnum.BeginUpdate();
						foreach (XMLEnum item in Definition.Enums.Items)
							if (item.name == propType)
							{
								foreach (EnumMember member in item.Items)
									conditionValueEnum.Items.Add(member.name);
								break;
							}
						conditionValueEnum.EndUpdate();
						conditionValueEnum.SelectedIndex = 0;
						break;
				}
			}
			UpdateConditionName();
		}

		private void UpdateConditionName()
		{
			Condition cond = Definition.Display.DisplayOptions[selectedDisplayOption.SelectedIndex].Conditions[selectedCondition.SelectedIndex];
			selectedCondition.Items[selectedCondition.SelectedIndex] = cond.property + " = " + cond.value;
			UpdateDisplayOptionName();
		}

		private void conditionValueEnum_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (conditionValueEnum.SelectedIndex == -1) return;
			Definition.Display.DisplayOptions[selectedDisplayOption.SelectedIndex].Conditions[selectedCondition.SelectedIndex].value = (string)conditionValueEnum.SelectedItem;
			UpdateConditionName();
		}

		private void conditionValueNum_ValueChanged(object sender, EventArgs e)
		{
			Definition.Display.DisplayOptions[selectedDisplayOption.SelectedIndex].Conditions[selectedCondition.SelectedIndex].value = conditionValueNum.Value.ToString();
			UpdateConditionName();
		}
		#endregion

		#region Images
		private void selectedImageRef_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (selectedImageRef.SelectedIndex == -1)
			{
				imageRefControls.Enabled = removeImageRefButton.Enabled = false;
				return;
			}
			imageRefControls.Enabled = removeImageRefButton.Enabled = true;
			ImageRef cond = Definition.Display.DisplayOptions[selectedDisplayOption.SelectedIndex].Images[selectedImageRef.SelectedIndex];
			imageRefImage.SelectedIndex = imageRefImage.Items.IndexOf(cond.image);
			imageRefOffsetX.Value = cond.Offset.X;
			imageRefOffsetY.Value = cond.Offset.Y;
			imageRefXFlip.SelectedIndex = (int)cond.xflip;
			imageRefYFlip.SelectedIndex = (int)cond.yflip;
		}

		private void addImageRefButton_Click(object sender, EventArgs e)
		{
			List<ImageRef> refs = new List<ImageRef>(Definition.Display.DisplayOptions[selectedDisplayOption.SelectedIndex].Images);
			refs.Add(new ImageRef() { image = (string)imageRefImage.Items[0] });
			Definition.Display.DisplayOptions[selectedDisplayOption.SelectedIndex].Images = refs.ToArray();
			selectedImageRef.Items.Add((string)imageRefImage.Items[0]);
			selectedImageRef.SelectedIndex = refs.Count - 1;
			UpdateDisplayPreview();
		}

		private void removeImageRefButton_Click(object sender, EventArgs e)
		{
			List<ImageRef> refs = new List<ImageRef>(Definition.Display.DisplayOptions[selectedDisplayOption.SelectedIndex].Images);
			refs.RemoveAt(selectedImageRef.SelectedIndex);
			Definition.Display.DisplayOptions[selectedDisplayOption.SelectedIndex].Images = refs.ToArray();
			selectedImageRef.Items.RemoveAt(selectedImageRef.SelectedIndex);
			UpdateDisplayPreview();
		}

		private void imageRefImage_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (imageRefImage.SelectedIndex == -1) return;
			Definition.Display.DisplayOptions[selectedDisplayOption.SelectedIndex].Images[selectedImageRef.SelectedIndex].image = (string)imageRefImage.SelectedItem;
			selectedImageRef.Items[selectedImageRef.SelectedIndex] = imageRefImage.SelectedItem;
			UpdateDisplayPreview();
		}

		private void imageRefOffsetX_ValueChanged(object sender, EventArgs e)
		{
			Definition.Display.DisplayOptions[selectedDisplayOption.SelectedIndex].Images[selectedImageRef.SelectedIndex].Offset = new XmlPoint((int)imageRefOffsetX.Value, Definition.Display.DisplayOptions[selectedDisplayOption.SelectedIndex].Images[selectedImageRef.SelectedIndex].Offset.Y);
			UpdateDisplayPreview();
		}

		private void imageRefOffsetY_ValueChanged(object sender, EventArgs e)
		{
			Definition.Display.DisplayOptions[selectedDisplayOption.SelectedIndex].Images[selectedImageRef.SelectedIndex].Offset = new XmlPoint(Definition.Display.DisplayOptions[selectedDisplayOption.SelectedIndex].Images[selectedImageRef.SelectedIndex].Offset.X, (int)imageRefOffsetY.Value);
			UpdateDisplayPreview();
		}

		private void imageRefXFlip_SelectedIndexChanged(object sender, EventArgs e)
		{
			Definition.Display.DisplayOptions[selectedDisplayOption.SelectedIndex].Images[selectedImageRef.SelectedIndex].xflip = (FlipType)imageRefXFlip.SelectedIndex;
			UpdateDisplayPreview();
		}

		private void imageRefYFlip_SelectedIndexChanged(object sender, EventArgs e)
		{
			Definition.Display.DisplayOptions[selectedDisplayOption.SelectedIndex].Images[selectedImageRef.SelectedIndex].yflip = (FlipType)imageRefYFlip.SelectedIndex;
			UpdateDisplayPreview();
		}
		#endregion
		#endregion
		#endregion
	}
}
