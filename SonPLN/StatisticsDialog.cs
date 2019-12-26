using SonicRetro.SonLVL.API;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SonicRetro.SonLVL.SonPLN
{
	public partial class StatisticsDialog : Form
	{
		private PatternIndex[,] plane;
		public StatisticsDialog(PatternIndex[,] plane)
		{
			InitializeComponent();
			this.plane = plane;
			listView4.ListViewItemSorter = new ListViewColumnSorter();
		}

		private void StatisticsDialog_Load(object sender, EventArgs e)
		{
			Dictionary<int, int> counts = new Dictionary<int, int>();
			counts.Clear();
			for (int i = 0; i < LevelData.Tiles.Count; i++)
				counts.Add(i, 0);
			for (int y = 0; y < plane.GetLength(1); y++)
				for (int x = 0; x < plane.GetLength(0); x++)
					if (counts.ContainsKey(plane[x, y].Tile))
						counts[plane[x, y].Tile]++;
					else
						counts.Add(plane[x, y].Tile, 1);
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
