using SonicRetro.SonLVL.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ObjectLayoutMerge
{
	public partial class FileSelectDialog : Form
	{
		public FileSelectDialog()
		{
			InitializeComponent();
		}

		GameInfo project;
		List<string> levels;
		EngineVersion ringformat;
		bool hasrings;

		private void FileSelectDialog_Load(object sender, EventArgs e)
		{
			layoutAFileGameSelector.SelectedIndex = 0;
		}

		private void layoutAProjectButton_CheckedChanged(object sender, EventArgs e)
		{
			layoutAProjectPanel.Enabled = layoutAProjectButton.Checked;
			layoutAFilePanel.Enabled = layoutAFileButton.Checked;
			SetRingFormat();
			CheckOKButton();
		}

		private void layoutAProjectFileSelector_FileNameChanged(object sender, EventArgs e)
		{
			layoutAProjectLevelSelector.Items.Clear();
			ringformat = EngineVersion.Invalid;
			hasrings = false;
			if (File.Exists(layoutAProjectFileSelector.FileName))
			{
				project = GameInfo.Load(layoutAProjectFileSelector.FileName);
				levels = new List<string>(project.Levels.Count);
				layoutAProjectLevelSelector.BeginUpdate();
				foreach (var level in project.Levels)
				{
					levels.Add(level.Key);
					LevelInfo info = project.GetLevelInfo(level.Key);
					layoutAProjectLevelSelector.Items.Add(info.DisplayName);
				}
				layoutAProjectLevelSelector.EndUpdate();
				layoutAProjectLevelSelector.SelectedIndex = 0;
				layoutAProjectLevelSelector.Enabled = true;
			}
			else
			{
				project = null;
				layoutAProjectLevelSelector.Enabled = false;
			}
			CheckOKButton();
		}

		private void layoutAProjectLevelSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			LevelInfo level = project.GetLevelInfo(levels[layoutAProjectLevelSelector.SelectedIndex]);
			ringformat = level.RingFormat;
			SetRingFormat();
			CheckOKButton();
		}

		private void layoutAFileGameSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (layoutAFileButton.Checked)
			{
				SetRingFormat();
				CheckOKButton();
			}
		}

		private void SetRingFormat()
		{
			EngineVersion fmt;
			if (layoutAProjectButton.Checked)
				fmt = ringformat;
			else
				fmt = (EngineVersion)(layoutAFileGameSelector.SelectedIndex + 1);
			switch (fmt)
			{
				case EngineVersion.S1:
				case EngineVersion.SCD:
				case EngineVersion.SCDPC:
					hasrings = false;
					break;
				case EngineVersion.S2:
				case EngineVersion.S2NA:
				case EngineVersion.S3K:
				case EngineVersion.SKC:
					hasrings = true;
					break;
			}
			layoutAFileRingSelector.Enabled = hasrings;
			layoutBRingSelector.Enabled = hasrings;
			outputFileRingSelector.Enabled = hasrings;
		}

		private void generic_FileNameChanged(object sender, EventArgs e)
		{
			CheckOKButton();
		}

		private void outputFileButton_CheckedChanged(object sender, EventArgs e)
		{
			outputFilePanel.Enabled = outputFileButton.Checked;
			CheckOKButton();
		}

		private void CheckOKButton()
		{
			okButton.Enabled = false;
			if (layoutAProjectButton.Checked)
			{
				if (project == null) return;
			}
			else
			{
				if (!File.Exists(layoutAFileObjectSelector.FileName)) return;
			}
			if (!File.Exists(layoutBObjectSelector.FileName)) return;
			if (outputFileButton.Checked)
				if (string.IsNullOrWhiteSpace(outputFileObjectSelector.FileName)) return;
			okButton.Enabled = true;
		}

		public LayoutAMode LayoutAMode { get { return layoutAProjectButton.Checked ? LayoutAMode.Project : LayoutAMode.File; } }

		public string LayoutAProjectFile { get { return layoutAProjectFileSelector.FileName; } }

		public string LayoutAProjectLevel { get { return levels[layoutAProjectLevelSelector.SelectedIndex]; } }

		public EngineVersion LayoutAFileGame { get { return (EngineVersion)(layoutAFileGameSelector.SelectedIndex + 1); } }

		public string LayoutAFileObjects { get { return layoutAFileObjectSelector.FileName; } }

		public string LayoutAFileRings { get { return layoutAFileRingSelector.FileName; } }

		public string LayoutBObjects { get { return layoutBObjectSelector.FileName; } }

		public string LayoutBRings { get { return layoutBRingSelector.FileName; } }

		public OutputMode OutputMode
		{
			get
			{
				if (outputAButton.Checked) return OutputMode.LayoutA;
				else if (outputBButton.Checked) return OutputMode.LayoutB;
				else return OutputMode.File;
			}
		}

		public string OutputFileObjects { get { return outputFileObjectSelector.FileName; } }

		public string OutputFileRings { get { return outputFileRingSelector.FileName; } }
	}

	public enum LayoutAMode { Project, File }

	public enum OutputMode { LayoutA, LayoutB, File }
}
