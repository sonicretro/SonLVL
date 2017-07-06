using SonicRetro.SonLVL.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace S3SSEdit
{
	public partial class StageSelectDialog : Form
	{
		string path;
		ProjectFile project;

		internal StageSelectDialog(string filename, ProjectFile project)
		{
			InitializeComponent();
			path = Path.GetDirectoryName(filename);
			this.project = project;
		}

		internal LayoutMode Category { get; private set; } = LayoutMode.S3;
		internal int StageNumber { get; private set; } = 0;

		private void StageSelectDialog_Load(object sender, EventArgs e)
		{
			if (stageList.SelectedIndex == -1)
				stageList.SelectedIndex = 0;
			else
				DrawPreview();
		}

		private void categoryS3_CheckedChanged(object sender, EventArgs e)
		{
			if (categoryS3.Checked)
			{
				Category = LayoutMode.S3;
				chunkSelector.Visible = false;
				stageList.Visible = true;
				if (stageList.SelectedIndex != 0)
					stageList.SelectedIndex = 0;
				else
					DrawPreview();
			}
		}

		private void categorySK_CheckedChanged(object sender, EventArgs e)
		{
			if (categorySK.Checked)
			{
				Category = LayoutMode.SK;
				chunkSelector.Visible = false;
				stageList.Visible = true;
				if (stageList.SelectedIndex != 0)
					stageList.SelectedIndex = 0;
				else
					DrawPreview();
			}
		}

		private void categoryBSChunk_CheckedChanged(object sender, EventArgs e)
		{
			if (categoryBSChunk.Checked)
			{
				Category = LayoutMode.BSChunk;
				stageList.Visible = false;
				chunkSelector.Visible = true;
				if (stageList.SelectedIndex != 0)
					stageList.SelectedIndex = 0;
				else
					DrawPreview();
			}
		}

		private void categoryBSLayout_CheckedChanged(object sender, EventArgs e)
		{
			if (categoryBSLayout.Checked)
			{
				Category = LayoutMode.BSLayout;
				// stuff
			}
		}

		private void stageList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (stageList.SelectedIndex != -1)
			{
				StageNumber = stageList.SelectedIndex;
				DrawPreview();
			}
		}

		private void chunkSelector_ValueChanged(object sender, EventArgs e)
		{
			StageNumber = (int)chunkSelector.Value;
			DrawPreview();
		}

		private void DrawPreview()
		{
			switch (Category)
			{
				case LayoutMode.S3:
					pictureBox1.Image = LayoutDrawer.DrawLayout(new SSLayoutData(File.ReadAllBytes(Path.Combine(path, project.S3Stages[StageNumber]))), 28).ToBitmap(LayoutDrawer.Palette);
					break;
				case LayoutMode.SK:
					pictureBox1.Image = LayoutDrawer.DrawLayout(new SSLayoutData(Compression.Decompress(Path.Combine(path, project.SKStageSet), CompressionType.Kosinski), StageNumber * SSLayoutData.Size), 28).ToBitmap(LayoutDrawer.Palette);
					break;
				case LayoutMode.BSChunk:
					pictureBox1.Image = LayoutDrawer.DrawLayout(new BSChunkLayoutData(Compression.Decompress(Path.Combine(path, project.BlueSphereChunkSet), CompressionType.Kosinski), StageNumber), 28).ToBitmap(LayoutDrawer.Palette);
					break;
				case LayoutMode.BSLayout:
					break;
			}
		}
	}
}
