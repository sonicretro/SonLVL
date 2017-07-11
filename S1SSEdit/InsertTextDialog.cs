using SonicRetro.SonLVL.API;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace S1SSEdit
{
	public partial class InsertTextDialog : Form
	{
		public InsertTextDialog(byte fgobj, byte bgobj)
		{
			InitializeComponent();
			font = Font;
			this.fgobj = fgobj;
			this.bgobj = bgobj;
		}

		byte fgobj, bgobj;
		Font font;
		public byte?[,] Section { get; set; }

		private void InsertTextDialog_Load(object sender, EventArgs e)
		{
			fontLabel.Text = font.Name + " " + font.SizeInPoints;
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			if (textBox1.TextLength > 0)
			{
				okButton.Enabled = true;
				DrawPreview();
			}
			else
				okButton.Enabled = false;
		}

		private void transparentBGCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			DrawPreview();
		}

		private void fontButton_Click(object sender, EventArgs e)
		{
			fontDialog1.Font = font;
			if (fontDialog1.ShowDialog(this) == DialogResult.OK)
			{
				textBox1.Font = font = fontDialog1.Font;
				fontLabel.Text = font.Name + " " + font.SizeInPoints;
				DrawPreview();
			}
		}

		private void DrawPreview()
		{
			if (textBox1.TextLength == 0)
			{
				pictureBox1.Image = null;
				return;
			}
			byte[] bits;
			int stride;
			using (Bitmap bmp = new Bitmap(0x40, 0x40))
			using (Graphics g = Graphics.FromImage(bmp))
			{
				g.SetOptions();
				g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
				g.DrawString(textBox1.Text, font, Brushes.Black, new PointF());
				BitmapData bmpd = bmp.LockBits(new Rectangle(new Point(), bmp.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
				bits = new byte[bmpd.Height * Math.Abs(bmpd.Stride)];
				System.Runtime.InteropServices.Marshal.Copy(bmpd.Scan0, bits, 0, bits.Length);
				stride = Math.Abs(bmpd.Stride);
				bmp.UnlockBits(bmpd);
			}
			int l = 0x40, t = 0x40, r = -1, b = -1;
			for (int y = 0; y < 0x40; y++)
			{
				int srcaddr = y * stride;
				for (int x = 0; x < 0x40; x++)
					if (BitConverter.ToUInt32(bits, srcaddr + (x * 4)) == 0xFF000000)
					{
						l = Math.Min(x, l);
						t = Math.Min(y, t);
						r = Math.Max(x, r);
						b = Math.Max(y, b);
					}
			}
			if (l == 0x40)
			{
				okButton.Enabled = false;
				pictureBox1.Image = null;
				return;
			}
			int w = r - l + 1;
			int h = b - t + 1;
			Section = new byte?[w, h];
			for (int y = 0; y < h; y++)
			{
				int srcaddr = (y + t) * stride;
				for (int x = 0; x < w; x++)
					if (BitConverter.ToUInt32(bits, srcaddr + ((x + l) * 4)) == 0xFF000000)
						Section[x, y] = fgobj;
					else if (!transparentBGCheckBox.Checked)
						Section[x, y] = bgobj;
			}
			pictureBox1.Image = LayoutDrawer.DrawLayout(Section, true).ToBitmap(LayoutDrawer.Palette);
		}
	}
}
