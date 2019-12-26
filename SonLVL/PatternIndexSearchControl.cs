using SonicRetro.SonLVL.API;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace SonicRetro.SonLVL
{
	public partial class PatternIndexSearchControl : UserControl
	{
		public PatternIndexSearchControl()
		{
			InitializeComponent();
		}

		bool initializing;

		public void UpdateStuff()
		{
			initializing = true;
			tileList.Images.Clear();
			if (LevelData.Level.TwoPlayerCompatible)
			{
				tile.Increment = 2;
				tileList.ImageHeight = 128;
				for (int i = 0; i < LevelData.Tiles.Count - 1; i += 2)
					tileList.Images.Add(LevelData.InterlacedTileToBmp4bpp(LevelData.TileArray, i, (int)palette.Value));
			}
			else
			{
				tile.Increment = 1;
				tileList.ImageHeight = 64;
				for (int i = 0; i < LevelData.Tiles.Count; i++)
					tileList.Images.Add(LevelData.TileToBmp4bpp(LevelData.Tiles[i], 0, (int)palette.Value, false));
			}
			tileList.ChangeSize();
			if (tile.Value >= LevelData.Tiles.Count)
				tileList.SelectedIndex = -1;
			else if (LevelData.Level.TwoPlayerCompatible)
				tileList.SelectedIndex = (int)tile.Value / 2;
			else
				tileList.SelectedIndex = (int)tile.Value;
			initializing = false;
		}

		private void tile_ValueChanged(object sender, EventArgs e)
		{
			if (!initializing)
			{
				initializing = true;
				if (tile.Value >= LevelData.Tiles.Count)
					tileList.SelectedIndex = -1;
				else if (LevelData.Level.TwoPlayerCompatible)
					tileList.SelectedIndex = (int)tile.Value / 2;
				else
					tileList.SelectedIndex = (int)tile.Value;
				initializing = false;
			}
		}

		private void tileList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!initializing && tileList.SelectedIndex > -1)
				tile.Value = LevelData.Level.TwoPlayerCompatible ? tileList.SelectedIndex * 2 : tileList.SelectedIndex;
		}

		private void searchTile_CheckedChanged(object sender, EventArgs e)
		{
			tileList.Enabled = tile.Enabled = searchTile.Checked;
		}

		private void searchPalette_CheckedChanged(object sender, EventArgs e)
		{
			palette.Enabled = searchPalette.Checked;
		}

		private void palette_ValueChanged(object sender, EventArgs e)
		{
			tileList.Images.Clear();
			if (LevelData.Level.TwoPlayerCompatible)
			{
				tileList.ImageHeight = 128;
				for (int i = 0; i < LevelData.Tiles.Count - 1; i += 2)
					tileList.Images.Add(LevelData.InterlacedTileToBmp4bpp(LevelData.TileArray, i, (int)palette.Value));
			}
			else
			{
				tileList.ImageHeight = 64;
				for (int i = 0; i < LevelData.Tiles.Count; i++)
					tileList.Images.Add(LevelData.TileToBmp4bpp(LevelData.Tiles[i], 0, (int)palette.Value, false));
			}
			tileList.ChangeSize();
		}

		[Browsable(false)]
		public bool? XFlip
		{
			get
			{
				if (xFlip.CheckState == CheckState.Indeterminate)
					return null;
				else
					return xFlip.Checked;
			}
		}

		[Browsable(false)]
		public bool? YFlip
		{
			get
			{
				if (yFlip.CheckState == CheckState.Indeterminate)
					return null;
				else
					return yFlip.Checked;
			}
		}

		[Browsable(false)]
		public bool? Priority
		{
			get
			{
				if (priority.CheckState == CheckState.Indeterminate)
					return null;
				else
					return priority.Checked;
			}
		}

		[Browsable(false)]
		public byte? Palette
		{
			get
			{
				if (searchPalette.Checked)
					return (byte)palette.Value;
				else
					return null;
			}
		}

		[Browsable(false)]
		public ushort? Tile
		{
			get
			{
				if (searchTile.Checked)
					return (ushort)tile.Value;
				else
					return null;
			}
		}
	}
}
