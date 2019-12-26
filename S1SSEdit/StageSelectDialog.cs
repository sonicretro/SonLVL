using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace S1SSEdit
{
	public partial class StageSelectDialog : Form
	{
		string path;
		Dictionary<string, ProjectStage> project;
		List<string> names;

		internal StageSelectDialog(string filename, Dictionary<string, ProjectStage> project)
		{
			InitializeComponent();
			path = Path.GetDirectoryName(filename);
			this.project = project;
			names = new List<string>(project.Keys);
			stageList.BeginUpdate();
			foreach (string item in names)
				stageList.Items.Add(item);
			stageList.EndUpdate();
		}

		internal string StageName { get; set; }

		private void StageSelectDialog_Load(object sender, EventArgs e)
		{
			if (stageList.SelectedIndex == -1)
				stageList.SelectedIndex = 0;
			else
				DrawPreview();
		}

		private void stageList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (stageList.SelectedIndex != -1)
			{
				StageName = names[stageList.SelectedIndex];
				DrawPreview();
			}
		}

		private void DrawPreview()
		{
			pictureBox1.Image = LayoutDrawer.DrawLayout(project[StageName].LoadStage(path), false).ToBitmap(LayoutDrawer.Palette);
		}
	}
}
