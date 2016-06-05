using SonicRetro.SonLVL.API;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace SonicRetro.SonLVL
{
	public partial class ChunkBlockSearchControl : UserControl
	{
		public ChunkBlockSearchControl()
		{
			InitializeComponent();
			solidity1.SelectedIndex = solidity2.SelectedIndex = 0;
		}

		bool initializing;

		public void UpdateStuff()
		{
			initializing = true;
			switch (LevelData.Level.ChunkFormat)
			{
				case EngineVersion.S2NA:
				case EngineVersion.S2:
				case EngineVersion.S3K:
				case EngineVersion.SKC:
					solidity2.Visible = true;
					break;
				default:
					solidity2.SelectedIndex = 0;
					solidity2.Visible = false;
					break;
			}
			block.Maximum = Math.Max(LevelData.GetBlockMax(), LevelData.Blocks.Count) - 1;
			blockList.Images = LevelData.CompBlockBmps;
			blockList.ChangeSize();
			blockList.SelectedIndex = block.Value >= LevelData.Blocks.Count ? -1 : (int)block.Value;
			initializing = false;
		}

		private void block_ValueChanged(object sender, EventArgs e)
		{
			if (!initializing)
			{
				initializing = true;
				blockList.SelectedIndex = block.Value >= LevelData.Blocks.Count ? -1 : (int)block.Value;
				initializing = false;
			}
		}

		private void blockList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!initializing && blockList.SelectedIndex > -1)
				block.Value = blockList.SelectedIndex;
		}

		private void searchBlock_CheckedChanged(object sender, EventArgs e)
		{
			blockList.Enabled = block.Enabled = searchBlock.Checked;
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
		public Solidity? Solidity1
		{
			get
			{
				if (solidity1.SelectedIndex == 0)
					return null;
				else
					return (Solidity)(solidity1.SelectedIndex - 1);
			}
		}

		[Browsable(false)]
		public Solidity? Solidity2
		{
			get
			{
				if (solidity2.SelectedIndex == 0)
					return null;
				else
					return (Solidity)(solidity2.SelectedIndex - 1);
			}
		}

		[Browsable(false)]
		public ushort? Block
		{
			get
			{
				if (searchBlock.Checked)
					return (ushort)block.Value;
				else
					return null;
			}
		}
	}
}
