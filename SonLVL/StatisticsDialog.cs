using SonicRetro.SonLVL.API;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SonicRetro.SonLVL
{
	public partial class StatisticsDialog : Form
	{
		public StatisticsDialog()
		{
			InitializeComponent();
			listView1.ListViewItemSorter = new ListViewColumnSorter();
			listView2.ListViewItemSorter = new ListViewColumnSorter();
			listView3.ListViewItemSorter = new ListViewColumnSorter();
			listView4.ListViewItemSorter = new ListViewColumnSorter();
			listView5.ListViewItemSorter = new ListViewColumnSorter();
		}

		private void StatisticsDialog_Load(object sender, EventArgs e)
		{
			Dictionary<int, int> counts = new Dictionary<int, int>();
			foreach (var item in LevelData.ObjTypes)
				counts.Add(item.Key, 0);
			foreach (ObjectEntry item in LevelData.Objects)
				if (counts.ContainsKey(item.ID))
					counts[item.ID]++;
				else
					counts.Add(item.ID, 1);
			listView1.BeginUpdate();
			foreach (KeyValuePair<int, int> item in counts)
			{
				ListViewItem lvi = new ListViewItem(item.Key.ToString("X2"));
				lvi.SubItems[0].Tag = item.Key;
				lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, item.Value.ToString()) { Tag = item.Value });
				listView1.Items.Add(lvi);
			}
			listView1.Sort();
			listView1.EndUpdate();
			counts.Clear();
			for (int i = 0; i < LevelData.Chunks.Count; i++)
				counts.Add(i, 0);
			for (int y = 0; y < LevelData.FGHeight; y++)
				for (int x = 0; x < LevelData.FGWidth; x++)
					if (counts.ContainsKey(LevelData.Layout.FGLayout[x, y]))
						counts[LevelData.Layout.FGLayout[x, y]]++;
					else
						counts.Add(LevelData.Layout.FGLayout[x, y], 1);
			for (int y = 0; y < LevelData.BGHeight; y++)
				for (int x = 0; x < LevelData.BGWidth; x++)
					if (counts.ContainsKey(LevelData.Layout.BGLayout[x, y]))
						counts[LevelData.Layout.BGLayout[x, y]]++;
					else
						counts.Add(LevelData.Layout.BGLayout[x, y], 1);
			listView2.BeginUpdate();
			foreach (KeyValuePair<int, int> item in counts)
			{
				ListViewItem lvi = new ListViewItem(item.Key.ToString("X2"));
				lvi.SubItems[0].Tag = item.Key;
				lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, item.Value.ToString()) { Tag = item.Value });
				listView2.Items.Add(lvi);
			}
			listView2.Sort();
			listView2.EndUpdate();
			counts.Clear();
			for (int i = 0; i < LevelData.Blocks.Count; i++)
				counts.Add(i, 0);
			for (int i = 0; i < LevelData.Chunks.Count; i++)
				for (int y = 0; y < LevelData.Level.ChunkHeight / 16; y++)
					for (int x = 0; x < LevelData.Level.ChunkWidth / 16; x++)
						if (counts.ContainsKey(LevelData.Chunks[i].Blocks[x, y].Block))
							counts[LevelData.Chunks[i].Blocks[x, y].Block]++;
						else
							counts.Add(LevelData.Chunks[i].Blocks[x, y].Block, 1);
			listView3.BeginUpdate();
			foreach (KeyValuePair<int, int> item in counts)
			{
				ListViewItem lvi = new ListViewItem(item.Key.ToString("X3"));
				lvi.SubItems[0].Tag = item.Key;
				lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, item.Value.ToString()) { Tag = item.Value });
				listView3.Items.Add(lvi);
			}
			listView3.Sort();
			listView3.EndUpdate();
			counts.Clear();
			for (int i = 0; i < LevelData.Tiles.Count; i++)
				counts.Add(i, 0);
			for (int i = 0; i < LevelData.Blocks.Count; i++)
				for (int y = 0; y < 2; y++)
					for (int x = 0; x < 2; x++)
						if (counts.ContainsKey(LevelData.Blocks[i].Tiles[x, y].Tile))
							counts[LevelData.Blocks[i].Tiles[x, y].Tile]++;
						else
							counts.Add(LevelData.Blocks[i].Tiles[x, y].Tile, 1);
			listView4.BeginUpdate();
			foreach (KeyValuePair<int, int> item in counts)
			{
				ListViewItem lvi = new ListViewItem(item.Key.ToString("X3"));
				lvi.SubItems[0].Tag = item.Key;
				lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, item.Value.ToString()) { Tag = item.Value });
				listView4.Items.Add(lvi);
			}
			listView4.Sort();
			listView4.EndUpdate();
			if (LevelData.ColInds1 == null)
			{
				tabPage5.Hide();
				return;
			}
			counts.Clear();
			for (int i = 0; i < 256; i++)
				counts.Add(i, 0);
			for (int i = 0; i < LevelData.ColInds1.Count; i++)
			{
				if (counts.ContainsKey(LevelData.ColInds1[i]))
					counts[LevelData.ColInds1[i]]++;
				else
					counts.Add(LevelData.ColInds1[i], 1);
				if (LevelData.ColInds2 != null && LevelData.ColInds2 != LevelData.ColInds1)
				{
					if (counts.ContainsKey(LevelData.ColInds2[i]))
						counts[LevelData.ColInds2[i]]++;
					else
						counts.Add(LevelData.ColInds2[i], 1);
				}
			}
			listView5.BeginUpdate();
			foreach (KeyValuePair<int, int> item in counts)
			{
				ListViewItem lvi = new ListViewItem(item.Key.ToString("X2"));
				lvi.SubItems[0].Tag = item.Key;
				lvi.SubItems.Add(new ListViewItem.ListViewSubItem(lvi, item.Value.ToString()) { Tag = item.Value });
				listView5.Items.Add(lvi);
			}
			listView5.Sort();
			listView5.EndUpdate();
		}

		private void listView_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			ListView list = (ListView)sender;
			ListViewColumnSorter sort = (ListViewColumnSorter)list.ListViewItemSorter;
			if (sort.Column == e.Column)
				sort.Order = sort.Order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
			else
			{
				sort.Column = e.Column;
				sort.Order = e.Column == 0 ? SortOrder.Ascending : SortOrder.Descending;
			}
			list.Sort();
		}
	}

	public class ListViewColumnSorter : System.Collections.IComparer
	{
		public int Column { get; set; }
		public SortOrder Order { get; set; }

		public ListViewColumnSorter()
		{
			Column = 1;
			Order = SortOrder.Descending;
		}

		public int Compare(object x, object y)
		{
			ListViewItem it1 = (ListViewItem)x;
			ListViewItem it2 = (ListViewItem)y;
			int result = ((int)it1.SubItems[Column].Tag).CompareTo(it2.SubItems[Column].Tag);
			return Order == SortOrder.Ascending ? result : -result;
		}
	}
}
