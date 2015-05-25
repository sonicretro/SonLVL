using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SonicRetro.SonLVL.API;

namespace SonicRetro.SonLVL.GUI
{
	public partial class CollisionSelector : Form
	{
		public CollisionSelector()
		{
			InitializeComponent();
		}

		public int Selection { get; private set; }

		private void CollisionSelector_Load(object sender, EventArgs e)
		{
			tileList1.Images = new List<Bitmap>(LevelData.ColBmps);
			tileList1.ChangeSize();
		}

		private void tileList1_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			Selection = tileList1.SelectedIndex;
			Close();
		}
	}
}
