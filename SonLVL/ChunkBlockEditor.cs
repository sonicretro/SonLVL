using SonicRetro.SonLVL.API;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace SonicRetro.SonLVL
{
	public partial class ChunkBlockEditor : UserControl
	{
		public ChunkBlockEditor()
		{
			InitializeComponent();
		}

		private void ChunkBlockEditor_Load(object sender, EventArgs e)
		{
			Enabled = false;
		}

		public event EventHandler PropertyValueChanged = delegate { };

		bool initializing;

		private ChunkBlock[] selectedObjects;
		[Browsable(false)]
		public ChunkBlock[] SelectedObjects
		{
			get { return selectedObjects; }
			set
			{
				initializing = true;
				if (Enabled = (selectedObjects = value) != null)
				{
					ChunkBlock first = value[0];
					if (value.All(a => a.XFlip == first.XFlip))
						xFlip.CheckState = first.XFlip ? CheckState.Checked : CheckState.Unchecked;
					else
						xFlip.CheckState = CheckState.Indeterminate;
					if (value.All(a => a.YFlip == first.YFlip))
						yFlip.CheckState = first.YFlip ? CheckState.Checked : CheckState.Unchecked;
					else
						yFlip.CheckState = CheckState.Indeterminate;
					if (value.All(a => a.Solid1 == first.Solid1))
						solidity1.SelectedIndex = (int)first.Solid1;
					else
						solidity1.SelectedIndex = -1;
					if (first is S2ChunkBlock)
					{
						solidity2.Visible = true;
						if (value.All(a => ((S2ChunkBlock)a).Solid2 == ((S2ChunkBlock)first).Solid2))
							solidity2.SelectedIndex = (int)((S2ChunkBlock)first).Solid2;
						else
							solidity2.SelectedIndex = -1;
					}
					else
						solidity2.Visible = false;
					block.Maximum = Math.Max(LevelData.GetBlockMax(), LevelData.Blocks.Count) - 1;
					if (value.All(a => a.Block == first.Block))
					{
						block.Minimum = 0;
						block.Value = first.Block;
					}
					else
					{
						block.Minimum = -1;
						block.Value = -1;
					}
				}
				initializing = false;
			}
		}

		private void xFlip_CheckedChanged(object sender, EventArgs e)
		{
			if (!initializing && xFlip.CheckState != CheckState.Indeterminate)
			{
				foreach (ChunkBlock item in selectedObjects)
					item.XFlip = xFlip.Checked;
				PropertyValueChanged(xFlip, EventArgs.Empty);
			}
		}

		private void yFlip_CheckedChanged(object sender, EventArgs e)
		{
			if (!initializing && yFlip.CheckState != CheckState.Indeterminate)
			{
				foreach (ChunkBlock item in selectedObjects)
					item.YFlip = yFlip.Checked;
				PropertyValueChanged(yFlip, EventArgs.Empty);
			}
		}

		private void solidity1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!initializing && solidity1.SelectedIndex > -1)
			{
				foreach (ChunkBlock item in selectedObjects)
					item.Solid1 = (Solidity)solidity1.SelectedIndex;
				PropertyValueChanged(solidity1, EventArgs.Empty);
			}
		}

		private void solidity2_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!initializing && solidity2.SelectedIndex > -1)
			{
				foreach (ChunkBlock item in selectedObjects)
					((S2ChunkBlock)item).Solid2 = (Solidity)solidity2.SelectedIndex;
				PropertyValueChanged(solidity2, EventArgs.Empty);
			}
		}

		private void block_ValueChanged(object sender, EventArgs e)
		{
			if (!initializing && block.Value > -1)
			{
				block.Minimum = 0;
				foreach (ChunkBlock item in selectedObjects)
					item.Block = (ushort)block.Value;
				PropertyValueChanged(block, EventArgs.Empty);
			}
		}
	}
}
