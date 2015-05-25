using System;
using System.ComponentModel;
using System.Windows.Forms;
using SonicRetro.SonLVL.API;

namespace SonicRetro.SonLVL
{
	public partial class PatternIndexEditor : UserControl
	{
		public PatternIndexEditor()
		{
			InitializeComponent();
		}

		private void PatternIndexEditor_Load(object sender, EventArgs e)
		{
			Enabled = false;
		}

		public event EventHandler PropertyValueChanged = delegate { };

		bool initializing;

		private PatternIndex selectedObject;
		[Browsable(false)]
		public PatternIndex SelectedObject
		{
			get { return selectedObject; }
			set
			{
				initializing = true;
				if (Enabled = (selectedObject = value) != null)
				{
					xFlip.Checked = value.XFlip;
					yFlip.Checked = value.YFlip;
					priority.Checked = value.Priority;
					palette.Value = value.Palette;
					tile.Value = value.Tile;
					tileList.Images.Clear();
					for (int i = 0; i < LevelData.Tiles.Count; i++)
						tileList.Images.Add(LevelData.TileToBmp4bpp(LevelData.Tiles[i], 0, value.Palette));
					tileList.ChangeSize();
					tileList.SelectedIndex = value.Tile >= LevelData.Tiles.Count ? -1 : value.Tile;
				}
				initializing = false;
			}
		}

		private void xFlip_CheckedChanged(object sender, EventArgs e)
		{
			if (!initializing)
			{
				selectedObject.XFlip = xFlip.Checked;
				PropertyValueChanged(xFlip, EventArgs.Empty);
			}
		}

		private void yFlip_CheckedChanged(object sender, EventArgs e)
		{
			if (!initializing)
			{
				selectedObject.YFlip = yFlip.Checked;
				PropertyValueChanged(yFlip, EventArgs.Empty);
			}
		}

		private void priority_CheckedChanged(object sender, EventArgs e)
		{
			if (!initializing)
			{
				selectedObject.Priority = priority.Checked;
				PropertyValueChanged(priority, EventArgs.Empty);
			}
		}

		private void palette_ValueChanged(object sender, EventArgs e)
		{
			if (!initializing)
			{
				selectedObject.Palette = (byte)palette.Value;
				PropertyValueChanged(palette, EventArgs.Empty);
				initializing = true;
				tileList.Images.Clear();
				for (int i = 0; i < LevelData.Tiles.Count; i++)
					tileList.Images.Add(LevelData.TileToBmp4bpp(LevelData.Tiles[i], 0, (int)palette.Value));
				tileList.SelectedIndex = selectedObject.Tile;
				initializing = false;
			}
		}

		private void tile_ValueChanged(object sender, EventArgs e)
		{
			if (!initializing)
			{
				selectedObject.Tile = (ushort)tile.Value;
				PropertyValueChanged(tile, EventArgs.Empty);
				initializing = true;
				tileList.SelectedIndex = tile.Value >= LevelData.Tiles.Count ? -1 : (int)tile.Value;
				initializing = false;
			}
		}

		private void tileList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!initializing && tileList.SelectedIndex > -1)
				tile.Value = tileList.SelectedIndex;
		}
	}
}
