using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SonicRetro.SonLVL.API;

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

		private ChunkBlock selectedObject;
		[Browsable(false)]
		public ChunkBlock SelectedObject
		{
			get { return selectedObject; }
			set
			{
				initializing = true;
				if (Enabled = (selectedObject = value) != null)
				{
					xFlip.Checked = value.XFlip;
					yFlip.Checked = value.YFlip;
					solidity1.SelectedIndex = (int)value.Solid1;
					if (value is S2ChunkBlock)
					{
						solidity2.Visible = true;
						solidity2.SelectedIndex = (int)((S2ChunkBlock)value).Solid2;
					}
					else
						solidity2.Visible = false;
					blockList.Images = LevelData.CompBlockBmps;
					blockList.ChangeSize();
					blockList.SelectedIndex = value.Block;
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
				PropertyValueChanged(xFlip, EventArgs.Empty);
			}
		}

		private void solidity1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!initializing)
			{
				selectedObject.Solid1 = (Solidity)solidity1.SelectedIndex;
				PropertyValueChanged(xFlip, EventArgs.Empty);
			}
		}

		private void solidity2_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!initializing)
			{
				((S2ChunkBlock)selectedObject).Solid2 = (Solidity)solidity2.SelectedIndex;
				PropertyValueChanged(xFlip, EventArgs.Empty);
			}
		}

		private void blockList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!initializing)
			{
				selectedObject.Block = (ushort)blockList.SelectedIndex;
				PropertyValueChanged(xFlip, EventArgs.Empty);
			}
		}
	}
}