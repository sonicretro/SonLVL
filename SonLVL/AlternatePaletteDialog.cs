using System;
using System.Drawing;
using System.Windows.Forms;

namespace SonicRetro.SonLVL
{
    public partial class AlternatePaletteDialog : Form
    {
        public AlternatePaletteDialog()
        {
            InitializeComponent();
            BlendColor = Color.FromArgb(128, Color.Aqua);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        int palnum;
        public Color BlendColor { get; set; }

        private void UnderwaterPaletteDialog_Load(object sender, EventArgs e)
        {
            palnum = LevelData.CurPal;
            LevelData.CurPal = 0;
            Bitmap bmp = new Bitmap(256, 64);
            Graphics gfx = Graphics.FromImage(bmp);
            for (int l = 0; l < 4; l++)
                for (int i = 0; i < 16; i++)
                    gfx.FillRectangle(new SolidBrush(LevelData.PaletteToColor(l, i, false)), i * 16, l * 16, 16, 16);
            pictureBox1.Image = new Bitmap(bmp);
            LevelData.CurPal = 1;
            for (int l = 0; l < 4; l++)
                for (int i = 0; i < 16; i++)
                    gfx.FillRectangle(new SolidBrush(LevelData.PaletteToColor(l, i, false)), i * 16, l * 16, 16, 16);
            pictureBox2.Image = bmp;
            LevelData.CurPal = palnum;
            panel1.BackColor = BlendColor;
            paletteIndex.Items.Clear();
            paletteIndex.BeginUpdate();
            for (int i = 1; i < LevelData.Palette.Count; i++)
                paletteIndex.Items.Add(LevelData.PalName[i]);
            paletteIndex.EndUpdate();
            paletteIndex.SelectedIndex = 0;
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            using (ColorDialog dlg = new ColorDialog() { AllowFullOpen = true, AnyColor = true, Color = BlendColor, SolidColorOnly = false })
            {
                if (dlg.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                    BlendColor = Color.FromArgb((int)((numericUpDown1.Value / 100) * 255), dlg.Color);
            }
            panel1.BackColor = BlendColor;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            BlendColor = Color.FromArgb((int)((numericUpDown1.Value / 100) * 255), BlendColor);
            panel1.BackColor = BlendColor;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LevelData.CurPal = 0;
            Bitmap bmp = new Bitmap(256, 64);
            Graphics gfx = Graphics.FromImage(bmp);
            for (int l = 0; l < 4; l++)
                for (int i = 0; i < 16; i++)
                {
                    Color col = LevelData.PaletteToColor(l, i, false);
                    if (radioButton1.Checked)
                        col = col.Blend(BlendColor);
                    else if (radioButton2.Checked)
                        col = Color.FromArgb(Math.Min(col.R + BlendColor.R, 255), Math.Min(col.G + BlendColor.G, 255), Math.Min(col.B + BlendColor.B, 255));
                    else
                        col = Color.FromArgb(Math.Max(col.R - BlendColor.R, 0), Math.Max(col.G - BlendColor.G, 0), Math.Max(col.B - BlendColor.B, 0));
                    gfx.FillRectangle(new SolidBrush(col), i * 16, l * 16, 16, 16);
                }
            pictureBox2.Image = bmp;
            LevelData.CurPal = palnum;
        }

        private void paletteIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (paletteIndex.SelectedIndex == -1)
                return;
            Bitmap bmp = new Bitmap(256, 64);
            Graphics gfx = Graphics.FromImage(bmp);
            LevelData.CurPal = paletteIndex.SelectedIndex + 1;
            for (int l = 0; l < 4; l++)
                for (int i = 0; i < 16; i++)
                    gfx.FillRectangle(new SolidBrush(LevelData.PaletteToColor(l, i, false)), i * 16, l * 16, 16, 16);
            pictureBox2.Image = bmp;
            LevelData.CurPal = palnum;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = radioButton1.Checked;
        }
    }
}
