using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace S1SSEdit
{
	public partial class LayoutSectionNameDialog : Form
	{
		public LayoutSectionNameDialog()
		{
			InitializeComponent();
		}

		public string Value
		{
			get { return textBox1.Text; }
			set { textBox1.Text = value; }
		}
	}
}
