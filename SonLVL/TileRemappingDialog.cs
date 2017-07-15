using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace SonicRetro.SonLVL
{
	public partial class TileRemappingDialog : Form
	{
		public TileRemappingDialog(string title, List<Bitmap> images, int imageWidth, int imageHeight)
		{
			InitializeComponent();
			Text += title;
			SourceTileList.Images = images;
			DestinationTileList.Images = new List<Bitmap>(images);
			SourceTile.Maximum = DestinationTile.Maximum = images.Count - 1;
			SourceTileList.ImageWidth = DestinationTileList.ImageWidth = imageWidth;
			SourceTileList.ImageHeight = DestinationTileList.ImageHeight = imageHeight;
			SourceTileList.SelectedIndex = DestinationTileList.SelectedIndex = 0;
			TileMap = new Dictionary<int, int>();
		}

		public Dictionary<int, int> TileMap { get; private set; }

		private void SourceTileList_SelectedIndexChanged(object sender, EventArgs e)
		{
			SourceTile.Value = SourceTileList.SelectedIndex;
		}

		private void DestinationTileList_SelectedIndexChanged(object sender, EventArgs e)
		{
			DestinationTile.Value = DestinationTileList.SelectedIndex;
		}

		private void SourceTile_ValueChanged(object sender, EventArgs e)
		{
			SourceTileList.SelectedIndex = (int)SourceTile.Value;
			UpdateDestinationTiles();
		}

		private void DestinationTile_ValueChanged(object sender, EventArgs e)
		{
			DestinationTileList.SelectedIndex = (int)DestinationTile.Value;
			UpdateDestinationTiles();
		}

		private void UpdateDestinationTiles()
		{
			DestinationTileList.Images = new List<Bitmap>(SourceTileList.Images);
			foreach (KeyValuePair<int, int> item in TileMap)
				DestinationTileList.Images[item.Value] = SourceTileList.Images[item.Key];
			DestinationTileList.Images[(int)DestinationTile.Value] = SourceTileList.Images[(int)SourceTile.Value];
			DestinationTileList.ChangeSize();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			int src = (int)SourceTile.Value;
			int dst = (int)DestinationTile.Value;
			if (TileMap.ContainsValue(dst))
			{
				MessageBox.Show(this, "Destination item already has a replacement!", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			if (TileMap.ContainsKey(src))
			{
				if (MessageBox.Show(this, "Source item is already in list! Do you want to replace it?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.No)
					return;
				listBox1.Items[TileMap.Keys.ToList().IndexOf(src)] = src.ToString("X") + "=" + dst.ToString("X");
			}
			else
				listBox1.Items.Add(src.ToString("X") + "=" + dst.ToString("X"));
			TileMap[src] = dst;
		}

		private void button2_Click(object sender, EventArgs e)
		{
			TileMap.Remove(TileMap.Keys.ToList()[listBox1.SelectedIndex]);
			listBox1.Items.RemoveAt(listBox1.SelectedIndex);
			UpdateDestinationTiles();
		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			button2.Enabled = listBox1.SelectedIndex != -1;
		}

		private void TileRemappingDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (DialogResult != System.Windows.Forms.DialogResult.OK) return;
			bool warn = false;
			foreach (int item in TileMap.Keys)
				if (!TileMap.ContainsValue(item))
				{
					warn = true;
					break;
				}
			if (warn)
				if (MessageBox.Show(this, "Not all source items are replaced in the destination. All source items that are not replaced in the destination will be duplicated in both positions. Is this OK?", Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != System.Windows.Forms.DialogResult.OK)
					e.Cancel = true;
		}
	}
}
