using SonicRetro.SonLVL.API;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace S3SSEdit
{
	public partial class StatisticsDialog : Form
	{
		private LayoutData layout;
		public StatisticsDialog(LayoutData layout)
		{
			InitializeComponent();
			this.layout = layout;
			listView4.ListViewItemSorter = new ListViewColumnSorter();
		}

		private void StatisticsDialog_Load(object sender, EventArgs e)
		{
			Dictionary<SphereType, int> counts = new Dictionary<SphereType, int>();
			counts.Clear();
			foreach (SphereType t in Enum.GetValues(typeof(SphereType)))
				if (t != SphereType.Empty)
					counts.Add(t, 0);
			for (int y = 0; y < layout.Layout.Size; y++)
				for (int x = 0; x < layout.Layout.Size; x++)
					if (counts.ContainsKey(layout.Layout[x, y]))
						counts[layout.Layout[x, y]]++;
					else
						counts.Add(layout.Layout[x, y], 1);
			listView4.BeginUpdate();
			foreach (KeyValuePair<SphereType, int> item in counts)
			{
				ListViewItem lvi = new ListViewItem(item.Key.ToString());
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
