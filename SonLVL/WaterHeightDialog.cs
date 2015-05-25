using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SonicRetro.SonLVL
{
	public partial class WaterHeightDialog : Form
	{
		public WaterHeightDialog(ushort height)
		{
			InitializeComponent();
			numericUpDown1.Value = height;
		}

		public ushort Value { get { return (ushort)numericUpDown1.Value; } }
	}
}
