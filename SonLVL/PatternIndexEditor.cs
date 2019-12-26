using System;
using System.ComponentModel;
using System.Windows.Forms;
using SonicRetro.SonLVL.API;
using System.Linq;

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

		private PatternIndex[] selectedObjects;
		[Browsable(false)]
		public PatternIndex[] SelectedObjects
		{
			get { return selectedObjects; }
			set
			{
				initializing = true;
				if (Enabled = (selectedObjects = value) != null)
				{
					PatternIndex first = value[0];
					if (value.All(a => a.XFlip == first.XFlip))
						xFlip.CheckState = first.XFlip ? CheckState.Checked : CheckState.Unchecked;
					else
						xFlip.CheckState = CheckState.Indeterminate;
					if (value.All(a => a.YFlip == first.YFlip))
						yFlip.CheckState = first.YFlip ? CheckState.Checked : CheckState.Unchecked;
					else
						yFlip.CheckState = CheckState.Indeterminate;
					if (value.All(a => a.Priority == first.Priority))
						priority.CheckState = first.Priority ? CheckState.Checked : CheckState.Unchecked;
					else
						priority.CheckState = CheckState.Indeterminate;
					if (value.All(a => a.Palette == first.Palette))
					{
						palette.Minimum = 0;
						palette.Value = first.Palette;
					}
					else
					{
						palette.Minimum = -1;
						palette.Value = -1;
					}
					if (value.All(a => a.Tile == first.Tile))
					{
						if (LevelData.Level.TwoPlayerCompatible)
						{
							tile.Increment = 2;
							tile.Value = first.Tile & ~1;
						}
						else
						{
							tile.Increment = 1;
							tile.Value = first.Tile;
						}
						tile.Minimum = 0;
					}
					else
					{
						tile.Increment = 1;
						tile.Minimum = -1;
						tile.Value = -1;
					}
				}
				initializing = false;
			}
		}

		private void xFlip_CheckedChanged(object sender, EventArgs e)
		{
			if (!initializing && xFlip.CheckState != CheckState.Indeterminate)
			{
				foreach (PatternIndex item in selectedObjects)
					item.XFlip = xFlip.Checked;
				PropertyValueChanged(xFlip, EventArgs.Empty);
			}
		}

		private void yFlip_CheckedChanged(object sender, EventArgs e)
		{
			if (!initializing && yFlip.CheckState != CheckState.Indeterminate)
			{
				foreach (PatternIndex item in selectedObjects)
				{
					item.YFlip = yFlip.Checked;
					if (LevelData.Level.TwoPlayerCompatible)
						item.Tile = (ushort)(item.Tile & ~1 | (item.YFlip ? 1 : 0));
				}
				PropertyValueChanged(yFlip, EventArgs.Empty);
			}
		}

		private void priority_CheckedChanged(object sender, EventArgs e)
		{
			if (!initializing && priority.CheckState != CheckState.Indeterminate)
			{
				foreach (PatternIndex item in selectedObjects)
					item.Priority = priority.Checked;
				PropertyValueChanged(priority, EventArgs.Empty);
			}
		}

		private void palette_ValueChanged(object sender, EventArgs e)
		{
			if (!initializing && palette.Value > -1)
			{
				palette.Minimum = 0;
				foreach (PatternIndex item in selectedObjects)
					item.Palette = (byte)palette.Value;
				PropertyValueChanged(palette, EventArgs.Empty);
			}
		}

		private void tile_ValueChanged(object sender, EventArgs e)
		{
			if (!initializing && tile.Value > -1)
			{
				tile.Increment = LevelData.Level.TwoPlayerCompatible ? 2 : 1;
				tile.Minimum = 0;
				foreach (PatternIndex item in selectedObjects)
					if (LevelData.Level.TwoPlayerCompatible && item.YFlip)
						item.Tile = (ushort)((int)tile.Value ^ 1);
					else
						item.Tile = (ushort)tile.Value;
				PropertyValueChanged(tile, EventArgs.Empty);
			}
		}
	}
}
