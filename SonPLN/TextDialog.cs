using System;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using SonicRetro.SonLVL.API;

namespace SonicRetro.SonLVL.SonPLN
{
	public partial class TextDialog : Form
	{
		TextMapping texmap;
		int maxwidth;
		TextBox[] textBoxes;
		public TextDialog(TextMapping texmap, int width, string[] text, int palette)
		{
			InitializeComponent();
			this.texmap = texmap;
			charLabel.Text += new string(texmap.Characters.Keys.ToArray());
			maxwidth = width;
			Lines = text;
			textBoxes = new TextBox[text.Length];
			textBox1.Text = text[0];
			textBox1.TextChanged += TextBox_TextChanged;
			textBoxes[0] = textBox1;
			tableLayoutPanel2.SuspendLayout();
			for (int i = 1; i < text.Length; i++)
			{
				tableLayoutPanel2.RowCount++;
				tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.AutoSize));
				TextBox tb = new TextBox();
				tb.Anchor = AnchorStyles.Left | AnchorStyles.Right;
				tb.Text = text[i];
				tb.TextChanged += TextBox_TextChanged;
				tableLayoutPanel2.Controls.Add(tb, 0, i);
				textBoxes[i] = tb;
			}
			tableLayoutPanel2.ResumeLayout();
			if (palette != palSelector.Value)
				palSelector.Value = palette;
			else
				UpdatePreview();
		}

		private void UpdatePreview()
		{
			int pal = (int)palSelector.Value;
			BitmapBits levelImg8bpp = new BitmapBits(maxwidth * 8, Lines.Length * texmap.Height * 8);
			int y = 0;
			for (int l = 0; l < Lines.Length; l++)
			{
				int x = 0;
				for (int i = 0; i < Lines[l].Length; i++)
				{
					CharMapInfo cm = texmap.Characters[Lines[l][i]];
					int w = cm.Width ?? texmap.DefaultWidth;
					for (int y2 = 0; y2 < texmap.Height; y2++)
						for (int x2 = 0; x2 < w; x2++)
							levelImg8bpp.DrawBitmap(LevelData.TileToBmp8bpp(LevelData.TileArray, cm.Map[x2, y2], pal), (x + x2) * 8, (y + y2) * 8);
					x += w;
				}
				y += texmap.Height;
			}
			pictureBox1.Image = levelImg8bpp.ToBitmap(LevelData.BmpPal);
		}

		bool skip = false;
		private void TextBox_TextChanged(object sender, EventArgs e)
		{
			if (skip) return;
			TextBox tb = (TextBox)sender;
			StringBuilder sb = new StringBuilder(tb.Text.Length);
			int w = 0;
			for (int i = 0; i < tb.Text.Length; i++)
				if (texmap.Characters.ContainsKey(tb.Text[i]))
				{
					if (w + (texmap.Characters[tb.Text[i]].Width ?? texmap.DefaultWidth) > maxwidth)
						break;
					sb.Append(tb.Text[i]);
					w += texmap.Characters[tb.Text[i]].Width ?? texmap.DefaultWidth;
				}
			string str = sb.ToString();
			if (!str.Equals(tb.Text, StringComparison.Ordinal))
			{
				skip = true;
				int selst = tb.SelectionStart;
				int selle = tb.SelectionLength;
				tb.Text = str;
				if (selst > str.Length)
				{
					selst = str.Length;
					selle = 0;
				}
				else
					selle = Math.Min(selle, str.Length - selst);
				tb.SelectionStart = selst;
				tb.SelectionLength = selle;
				skip = false;
			}
			Lines[Array.IndexOf(textBoxes, tb)] = str;
			UpdatePreview();
		}

		private void PalSelector_ValueChanged(object sender, EventArgs e)
		{
			UpdatePreview();
		}

		public string[] Lines { get; private set; }

		public byte Palette => (byte)palSelector.Value;
	}
}
