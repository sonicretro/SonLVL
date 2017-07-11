using SonicRetro.SonLVL.API;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace S3SSEdit
{
	public partial class InsertTextDialog : Form
	{
		public InsertTextDialog(SphereType fgsphere, SphereType bgsphere)
		{
			InitializeComponent();
			font = Font;
			this.fgsphere = fgsphere;
			this.bgsphere = bgsphere;
		}

		SphereType fgsphere, bgsphere;
		Font font;
		public SphereType?[,] Section { get; set; }

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
			Size size;
			byte[] bits;
			int stride;
			using (Bitmap bmp = new Bitmap(32, 32))
			using (Graphics g = Graphics.FromImage(bmp))
			{
				g.SetOptions();
				g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
				size = g.MeasureString(textBox1.Text, font).ToSize();
				size = new Size(Math.Min(size.Width, 32), Math.Min(size.Height, 32));
				g.DrawString(textBox1.Text, font, Brushes.Black, new PointF());
				BitmapData bmpd = bmp.LockBits(new Rectangle(new Point(), bmp.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
				bits = new byte[bmpd.Height * Math.Abs(bmpd.Stride)];
				System.Runtime.InteropServices.Marshal.Copy(bmpd.Scan0, bits, 0, bits.Length);
				stride = Math.Abs(bmpd.Stride);
				bmp.UnlockBits(bmpd);
			}
			Section = new SphereType?[size.Width, size.Height];
			for (int y = 0; y < size.Height; y++)
			{
				int srcaddr = y * stride;
				for (int x = 0; x < size.Width; x++)
					if (BitConverter.ToUInt32(bits, srcaddr + (x * 4)) == 0xFF000000)
						Section[x, y] = fgsphere;
					else if (!transparentBGCheckBox.Checked)
						Section[x, y] = bgsphere;
			}
			pictureBox1.Image = LayoutDrawer.DrawLayout(Section, 24).ToBitmap(LayoutDrawer.Palette);
		}
	}
}
