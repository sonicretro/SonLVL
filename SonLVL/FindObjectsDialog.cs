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
	public partial class FindObjectsDialog : Form
	{
		public FindObjectsDialog()
		{
			InitializeComponent();
		}

		private void findID_CheckedChanged(object sender, EventArgs e)
		{
			idSelect.Enabled = findID.Checked;
			if (findID.Checked)
				idSelect_ValueChanged(this, EventArgs.Empty);
			else
			{
				subtypeList.Items.Clear();
				imageList1.Images.Clear();
			}
		}

		private void findSubtype_CheckedChanged(object sender, EventArgs e)
		{
			panel1.Enabled = findSubtype.Checked;
		}

		private void idSelect_ValueChanged(object sender, EventArgs e)
		{
			subtypeList.BeginUpdate();
			subtypeList.Items.Clear();
			imageList1.Images.Clear();
			if (LevelData.ObjTypes.ContainsKey(idSelect.Value))
			{
				byte value = LevelData.ObjTypes[idSelect.Value].DefaultSubtype;
				foreach (byte item in LevelData.ObjTypes[idSelect.Value].Subtypes)
				{
					imageList1.Images.Add(LevelData.ObjTypes[idSelect.Value].SubtypeImage(item).GetBitmap().ToBitmap(LevelData.BmpPal).Resize(imageList1.ImageSize));
					subtypeList.Items.Add(new ListViewItem(LevelData.ObjTypes[idSelect.Value].SubtypeName(item), imageList1.Images.Count - 1) { Tag = item, Selected = item == value });
				}
				subtypeSelect.Value = value;
			}
			subtypeList.EndUpdate();
		}

		private void subtypeList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (subtypeList.SelectedIndices.Count > 0)
				subtypeSelect.Value = (byte)subtypeList.SelectedItems[0].Tag;
		}

		public byte? ID
		{
			get
			{
				if (findID.Checked)
					return idSelect.Value;
				else
					return null;
			}
		}

		public byte? SubType
		{
			get
			{
				if (findSubtype.Checked)
					return (byte)subtypeSelect.Value;
				else
					return null;
			}
		}

		public bool? XFlip
		{
			get
			{
				if (findXFlip.CheckState == CheckState.Indeterminate)
					return null;
				else
					return findXFlip.Checked;
			}
		}

		public bool? YFlip
		{
			get
			{
				if (findYFlip.CheckState == CheckState.Indeterminate)
					return null;
				else
					return findYFlip.Checked;
			}
		}
	}
}
