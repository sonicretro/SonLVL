using System.Windows.Forms;

namespace SonicRetro.SonLVL
{
	internal class NumericUpDownMulti : NumericUpDown
	{
		protected override void UpdateEditText()
		{
			if (Hexadecimal && Value == -1)
			{
				ChangingText = true;
				Text = "-1";
			}
			else
				base.UpdateEditText();
		}
	}
}
