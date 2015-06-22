using System;
using System.Windows.Forms;

namespace ChaotixSpriteEdit
{
	public partial class SpriteAddressDialog : Form
	{
		public SpriteAddressDialog(int length)
		{
			InitializeComponent();
			numericUpDown1.Maximum = length - 4;
		}

		private void okButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		public int Address { get { return (int)numericUpDown1.Value; } }
	}
}
