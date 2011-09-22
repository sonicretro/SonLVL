using System;
using System.Windows.Forms;

namespace SonicRetro.SonLVL
{
    public partial class ResizeLevelDialog : Form
    {
        public ResizeLevelDialog()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void levelHeight_ValueChanged(object sender, EventArgs e)
        {
            if (LevelData.LayoutFmt == EngineVersion.S3K || LevelData.LayoutFmt == EngineVersion.SKC)
            {
                switch (LevelData.MainForm.EditingMode)
                {
                    case EditingModes.Objects:
                    case EditingModes.PlaneA:
                        levelWidth.Maximum = Math.Floor((3960 - (LevelData.BGLayout.Length)) / levelHeight.Value);
                        break;
                    case EditingModes.PlaneB:
                        levelWidth.Maximum = Math.Floor((3960 - (LevelData.FGLayout.Length)) / levelHeight.Value);
                        break;
                }
            }
        }
    }
}
