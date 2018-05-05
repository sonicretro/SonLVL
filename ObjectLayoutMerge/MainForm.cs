using SonicRetro.SonLVL.API;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Drawing2D;

namespace ObjectLayoutMerge
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		FileSelectDialog fileSelectDialog = new FileSelectDialog();
		ObjectLayoutFormat objectFormat;
		RingLayoutFormat ringFormat;
		bool objectTerm, ringStartTerm, ringEndTerm;
		string outputObjectFile;
		string outputRingFile;
		List<ObjectInfo> objectList;
		bool showPreview, suppressDraw;

		private void MainForm_Shown(object sender, EventArgs e)
		{
			if (fileSelectDialog.ShowDialog(this) == DialogResult.OK)
				LoadLevel();
			else
				Close();
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (fileSelectDialog.ShowDialog(this) == DialogResult.OK)
				LoadLevel();
		}

		private void LoadLevel()
		{
			List<ObjectEntry> objectsA, objectsB;
			List<RingEntry> ringsA = null, ringsB = null;
			if (fileSelectDialog.LayoutAMode == LayoutAMode.Project)
			{
				showPreview = true;
				viewToolStripMenuItem.Enabled = true;
				LevelData.LoadGame(fileSelectDialog.LayoutAProjectFile);
				LevelData.LoadLevel(fileSelectDialog.LayoutAProjectLevel, true);
				LevelData.BmpPal.Entries[0] = LevelData.PaletteToColor(2, 0, false);
				objectFormat = LevelData.ObjectFormat;
				ringFormat = LevelData.RingFormat as RingLayoutFormat;
				objectsA = LevelData.Objects;
				objectTerm = LevelData.objectterm;
				if (ringFormat != null)
				{
					ringsA = LevelData.Rings;
					ringStartTerm = LevelData.ringstartterm;
					ringEndTerm = LevelData.ringendterm;
				}
			}
			else
			{
				showPreview = false;
				viewToolStripMenuItem.Enabled = false;
				LevelData.littleendian = false;
				switch (fileSelectDialog.LayoutAFileGame)
				{
					case EngineVersion.S1:
						objectFormat = new SonicRetro.SonLVL.API.S1.Object();
						break;
					case EngineVersion.S2:
					case EngineVersion.S2NA:
						objectFormat = new SonicRetro.SonLVL.API.S2.Object();
						ringFormat = new SonicRetro.SonLVL.API.S2.Ring();
						break;
					case EngineVersion.S3K:
						objectFormat = new SonicRetro.SonLVL.API.S3K.Object();
						ringFormat = new SonicRetro.SonLVL.API.S3K.Ring();
						break;
					case EngineVersion.SKC:
						LevelData.littleendian = true;
						objectFormat = new SonicRetro.SonLVL.API.S3K.Object();
						ringFormat = new SonicRetro.SonLVL.API.S3K.Ring();
						break;
					case EngineVersion.SCD:
						objectFormat = new SonicRetro.SonLVL.API.SCD.Object();
						break;
					case EngineVersion.SCDPC:
						LevelData.littleendian = true;
						objectFormat = new SonicRetro.SonLVL.API.SCD.Object();
						break;
					case EngineVersion.Chaotix:
						objectFormat = new SonicRetro.SonLVL.API.Chaotix.Object();
						break;
				}
				objectsA = objectFormat.ReadLayout(fileSelectDialog.LayoutAFileObjects, out objectTerm);
				if (ringFormat != null && File.Exists(fileSelectDialog.LayoutAFileRings))
					ringsA = ringFormat.ReadLayout(fileSelectDialog.LayoutAFileRings, out ringStartTerm, out ringEndTerm);
			}
			objectsA.Sort();
			if (ringsA != null)
				ringsA.Sort();
			objectsB = objectFormat.ReadLayout(fileSelectDialog.LayoutBObjects);
			objectsB.Sort();
			if (ringFormat != null && File.Exists(fileSelectDialog.LayoutBRings))
			{
				ringsB = ringFormat.ReadLayout(fileSelectDialog.LayoutBRings);
				ringsB.Sort();
			}
			switch (fileSelectDialog.OutputMode)
			{
				case OutputMode.LayoutA:
					if (fileSelectDialog.LayoutAMode == LayoutAMode.Project)
					{
						outputObjectFile = LevelData.Level.Objects;
						outputRingFile = LevelData.Level.Rings;
					}
					else
					{
						outputObjectFile = fileSelectDialog.LayoutAFileObjects;
						outputRingFile = fileSelectDialog.LayoutAFileRings;
					}
					break;
				case OutputMode.LayoutB:
					outputObjectFile = fileSelectDialog.LayoutBObjects;
					outputRingFile = fileSelectDialog.LayoutBRings;
					break;
				case OutputMode.File:
					outputObjectFile = fileSelectDialog.OutputFileObjects;
					outputRingFile = fileSelectDialog.OutputFileRings;
					break;
			}
			if (ringsA == null || ringsB == null || string.IsNullOrEmpty(outputRingFile))
			{
				ringFormat = null;
				outputRingFile = null;
				if (showPreview)
					LevelData.Rings = new List<RingEntry>();
			}
			objectList = new List<ObjectInfo>(Math.Max(objectsA.Count, objectsB.Count));
			for (int i = 0; i < objectsA.Count; i++)
				for (int j = 0; j < objectsB.Count; j++)
					if (objectsA[i].GetBytes().FastArrayEqual(objectsB[j].GetBytes()))
					{
						objectList.Add(new ObjectInfo(true, Source.Both, objectsA[i]));
						objectsA.RemoveAt(i--);
						objectsB.RemoveAt(j);
						break;
					}
			objectList.AddRange(objectsA.Select(a => new ObjectInfo(false, Source.A, a)));
			if (showPreview)
				foreach (ObjectEntry obj in objectsB)
					obj.UpdateSprite();
			objectList.AddRange(objectsB.Select(b => new ObjectInfo(false, Source.B, b)));
			objectList.Sort();
			if (showPreview)
				LevelData.Objects = new List<ObjectEntry>(objectList.Select(a => (ObjectEntry)a.Entry));
			if (ringFormat != null)
			{
				for (int i = 0; i < ringsA.Count; i++)
					for (int j = 0; j < ringsB.Count; j++)
						if (ringsA[i].GetBytes().FastArrayEqual(ringsB[j].GetBytes()))
						{
							objectList.Add(new ObjectInfo(true, Source.Both, ringsA[i]));
							ringsA.RemoveAt(i--);
							ringsB.RemoveAt(j);
							break;
						}
				objectList.AddRange(ringsA.Select(a => new ObjectInfo(false, Source.A, a)));
				if (showPreview)
					foreach (RingEntry rng in ringsB)
						rng.UpdateSprite();
				objectList.AddRange(ringsB.Select(b => new ObjectInfo(false, Source.B, b)));
				objectList.Sort();
				if (showPreview)
					LevelData.Rings = new List<RingEntry>(objectList.Where(a => a.Entry is RingEntry).Select(a => (RingEntry)a.Entry));
			}
			objectListView.BeginUpdate();
			objectListView.Items.Clear();
			suppressDraw = true;
			foreach (ObjectInfo item in objectList)
				objectListView.Items.Add(new ListViewItem(new string[] { item.Source.ToString(), item.Entry.Data }) { Checked = item.Include });
			suppressDraw = false;
			objectListView.EndUpdate();
			nextAToolStripButton.Enabled = objectList.Any(a => a.Source == Source.A);
			nextBToolStripButton.Enabled = objectList.Any(a => a.Source == Source.B);
			toolStrip1.Visible = nextAToolStripButton.Enabled || nextBToolStripButton.Enabled;
			previewPanel.HScrollValue = 0;
			previewPanel.VScrollValue = 0;
			DrawPreview();
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			objectFormat.WriteLayout(objectList.Where(a => a.Include && a.Entry is ObjectEntry).Select(a => (ObjectEntry)a.Entry).ToList(), outputObjectFile, objectTerm);
			if (ringFormat != null)
				ringFormat.WriteLayout(objectList.Where(a => a.Include && a.Entry is RingEntry).Select(a => (RingEntry)a.Entry).ToList(), outputRingFile, ringStartTerm, ringEndTerm);
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void uncheckAllObjectsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			suppressDraw = true;
			foreach (ListViewItem item in objectListView.Items)
				item.Checked = false;
			suppressDraw = false;
			DrawPreview();
		}

		private void checkAllObjectsInBothToolStripMenuItem_Click(object sender, EventArgs e)
		{
			suppressDraw = true;
			for (int i = 0; i < objectList.Count; i++)
				if (objectList[i].Source == Source.Both)
					objectListView.Items[i].Checked = true;
			suppressDraw = false;
			DrawPreview();
		}

		private void checkAllObjectsInAToolStripMenuItem_Click(object sender, EventArgs e)
		{
			suppressDraw = true;
			for (int i = 0; i < objectList.Count; i++)
				if (objectList[i].Source == Source.A)
					objectListView.Items[i].Checked = true;
			suppressDraw = false;
			DrawPreview();
		}

		private void checkAllObjectsInBToolStripMenuItem_Click(object sender, EventArgs e)
		{
			suppressDraw = true;
			for (int i = 0; i < objectList.Count; i++)
				if (objectList[i].Source == Source.B)
					objectListView.Items[i].Checked = true;
			suppressDraw = false;
			DrawPreview();
		}

		private void checkAllObjectsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			suppressDraw = true;
			foreach (ListViewItem item in objectListView.Items)
				item.Checked = true;
			suppressDraw = false;
			DrawPreview();
		}

		private void showOnlySelectedObjectsToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			if (!showOnlySelectedObjectsToolStripMenuItem.Checked)
			{
				LevelData.Objects = new List<ObjectEntry>(objectList.Select(a => (ObjectEntry)a.Entry));
				LevelData.Rings = new List<RingEntry>(objectList.Where(a => a.Entry is RingEntry).Select(a => (RingEntry)a.Entry));
			}
			DrawPreview();
		}

		private void showOverlaysToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			DrawPreview();
		}

		private void nextDiffToolStripButton_Click(object sender, EventArgs e)
		{
			int ind = 0;
			if (objectListView.SelectedIndices.Count > 0 && objectListView.SelectedIndices[0] < objectList.Count - 1)
				ind = objectListView.SelectedIndices[0] + 1;
			ind = objectList.FindIndex(ind, a => a.Source != Source.Both);
			if (ind == -1)
				ind = objectList.FindIndex(a => a.Source != Source.Both);
			objectListView.SelectedIndices.Clear();
			objectListView.SelectedIndices.Add(ind);
			objectListView.EnsureVisible(ind);
		}

		private void nextAToolStripButton_Click(object sender, EventArgs e)
		{
			int ind = 0;
			if (objectListView.SelectedIndices.Count > 0 && objectListView.SelectedIndices[0] < objectList.Count - 1)
				ind = objectListView.SelectedIndices[0] + 1;
			ind = objectList.FindIndex(ind, a => a.Source == Source.A);
			if (ind == -1)
				ind = objectList.FindIndex(a => a.Source == Source.A);
			objectListView.SelectedIndices.Clear();
			objectListView.SelectedIndices.Add(ind);
			objectListView.EnsureVisible(ind);
		}

		private void nextBToolStripButton_Click(object sender, EventArgs e)
		{
			int ind = 0;
			if (objectListView.SelectedIndices.Count > 0 && objectListView.SelectedIndices[0] < objectList.Count - 1)
				ind = objectListView.SelectedIndices[0] + 1;
			ind = objectList.FindIndex(ind, a => a.Source == Source.B);
			if (ind == -1)
				ind = objectList.FindIndex(a => a.Source == Source.B);
			objectListView.SelectedIndices.Clear();
			objectListView.SelectedIndices.Add(ind);
			objectListView.EnsureVisible(ind);
		}

		private void objectListView_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (showPreview && objectListView.SelectedIndices.Count > 0)
			{
				Entry entry = objectList[objectListView.SelectedIndices[0]].Entry;
				previewPanel.HScrollValue = Math.Max(entry.X - (previewPanel.PanelWidth / 2), 0);
				previewPanel.VScrollValue = Math.Max(entry.Y - (previewPanel.PanelHeight / 2), 0);
				propertyGrid1.SelectedObject = new ReadOnlyWrapper(entry);
			}
		}

		private void objectListView_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			objectList[e.Item.Index].Include = e.Item.Checked;
			DrawPreview();
		}

		private void previewPanel_PanelPaint(object sender, PaintEventArgs e)
		{
			DrawPreview();
		}

		private void previewPanel_ScrollBarValueChanged(object sender, EventArgs e)
		{
			DrawPreview();
		}

		private void previewPanel_PanelMouseMove(object sender, MouseEventArgs e)
		{
			if (e.X < 0 || e.Y < 0 || e.X > previewPanel.PanelWidth || e.Y > previewPanel.PanelHeight) return;
			Point mouse = new Point(e.X + previewPanel.HScrollValue, e.Y + previewPanel.VScrollValue);
			Cursor cur = Cursors.Default;
			foreach (ObjectInfo obj in objectList)
				if (obj.Entry.Bounds.Contains(mouse))
				{
					cur = Cursors.Hand;
					break;
				}
			previewPanel.PanelCursor = cur;
		}

		private void previewPanel_PanelMouseDown(object sender, MouseEventArgs e)
		{
			if (e.X < 0 || e.Y < 0 || e.X > previewPanel.PanelWidth || e.Y > previewPanel.PanelHeight) return;
			Point mouse = new Point(e.X + previewPanel.HScrollValue, e.Y + previewPanel.VScrollValue);
			foreach (ObjectInfo obj in objectList)
				if (obj.Entry.Bounds.Contains(mouse))
				{
					objectListView.SelectedIndices.Clear();
					objectListView.SelectedIndices.Add(objectList.IndexOf(obj));
					objectListView.EnsureVisible(objectList.IndexOf(obj));
					return;
				}
		}

		private void previewPanel_PanelKeyDown(object sender, KeyEventArgs e)
		{
			long hstep = e.Control ? int.MaxValue : e.Shift ? LevelData.Level.ChunkWidth : 16;
			long vstep = e.Control ? int.MaxValue : e.Shift ? LevelData.Level.ChunkHeight : 16;
			switch (e.KeyCode)
			{
				case Keys.Up:
					previewPanel.VScrollValue = (int)Math.Max(previewPanel.VScrollValue - vstep, previewPanel.VScrollMinimum);
					break;
				case Keys.Down:
					previewPanel.VScrollValue = (int)Math.Min(previewPanel.VScrollValue + vstep, previewPanel.VScrollMaximum - LevelData.Level.ChunkHeight + 1);
					break;
				case Keys.Left:
					previewPanel.HScrollValue = (int)Math.Max(previewPanel.HScrollValue - hstep, previewPanel.HScrollMinimum);
					break;
				case Keys.Right:
					previewPanel.HScrollValue = (int)Math.Min(previewPanel.HScrollValue + hstep, previewPanel.HScrollMaximum - LevelData.Level.ChunkWidth + 1);
					break;
			}
		}

		static readonly SolidBrush brushA = new SolidBrush(Color.FromArgb(128, Color.Red));
		static readonly SolidBrush brushB = new SolidBrush(Color.FromArgb(128, Color.Lime));
		static readonly Pen selectionPen = new Pen(Color.FromArgb(128, Color.Black)) { DashStyle = DashStyle.Dot };
		static readonly SolidBrush selectionBrush = new SolidBrush(Color.FromArgb(128, Color.White));
		private void DrawPreview()
		{
			if (!showPreview || suppressDraw) return;
			List<ObjectInfo> selected = objectList;
			if (showOnlySelectedObjectsToolStripMenuItem.Checked)
			{
				selected = objectListView.CheckedIndices.OfType<int>().Select(a => objectList[a]).ToList();
				if (objectListView.SelectedIndices.Count > 0 && !selected.Contains(objectList[objectListView.SelectedIndices[0]]))
					selected.Add(objectList[objectListView.SelectedIndices[0]]);
				LevelData.Objects = new List<ObjectEntry>(selected.Where(a => a.Entry is ObjectEntry).Select(a => (ObjectEntry)a.Entry));
				LevelData.Rings = new List<RingEntry>(selected.Where(a => a.Entry is RingEntry).Select(a => (RingEntry)a.Entry));
			}
			Point camera = new Point(previewPanel.HScrollValue, previewPanel.VScrollValue);
			Rectangle dispRect = new Rectangle(camera.X, camera.Y, previewPanel.PanelWidth, previewPanel.PanelHeight);
			BitmapBits img8 = LevelData.DrawForeground(dispRect, true, true, true, true, true, false, false, false);
			Bitmap bmp = img8.ToBitmap(LevelData.BmpPal).To32bpp();
			Graphics gfx = Graphics.FromImage(bmp);
			gfx.SetOptions();
			if (showOverlaysToolStripMenuItem.Checked)
				foreach (ObjectInfo obj in selected)
				{
					Rectangle bnd = obj.Entry.Bounds;
					if (!dispRect.IntersectsWith(bnd)) continue;
					bnd.Offset(-camera.X, -camera.Y);
					SolidBrush brush = null;
					switch (obj.Source)
					{
						case Source.A:
							brush = brushA;
							break;
						case Source.B:
							brush = brushB;
							break;
					}
					if (brush != null)
						gfx.FillRectangle(brush, bnd);
					if (objectListView.SelectedIndices.Count > 0 && obj == objectList[objectListView.SelectedIndices[0]])
					{
						gfx.FillRectangle(selectionBrush, bnd);
						bnd.Width--; bnd.Height--;
						gfx.DrawRectangle(selectionPen, bnd);
					}
				}
			previewPanel.PanelGraphics.DrawImage(bmp, 0, 0, previewPanel.PanelWidth, previewPanel.PanelHeight);
		}
	}

	class ObjectInfo : IComparable<ObjectInfo>
	{
		public bool Include { get; set; }
		public Source Source { get; private set; }
		public Entry Entry { get; private set; }

		public ObjectInfo(bool include, Source source, Entry entry)
		{
			Include = include;
			Source = source;
			Entry = entry;
		}

		int IComparable<ObjectInfo>.CompareTo(ObjectInfo other)
		{
			return ((IComparable<Entry>)Entry).CompareTo(other.Entry);
		}
	}

	enum Source
	{
		A,
		B,
		Both
	}
}
