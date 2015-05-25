namespace SonAni
{
	partial class MainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.ToolStripMenuItem noneToolStripMenuItem;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
			System.Windows.Forms.ToolStripMenuItem noneToolStripMenuItem1;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
			System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
			System.Windows.Forms.Label label5;
			this.animationListBox = new System.Windows.Forms.ListBox();
			this.addAnimationButton = new System.Windows.Forms.Button();
			this.removeAnimationButton = new System.Windows.Forms.Button();
			this.animationFrameList = new SonicRetro.SonLVL.API.TileList();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.animationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.recentProjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.backgroundLevelLoader = new System.ComponentModel.BackgroundWorker();
			this.playButton = new System.Windows.Forms.Button();
			this.stopButton = new System.Windows.Forms.Button();
			this.nextButton = new System.Windows.Forms.Button();
			this.previewPanel = new System.Windows.Forms.Panel();
			this.previewBox = new System.Windows.Forms.GroupBox();
			this.previewTimer = new System.Timers.Timer();
			this.exportGIFButton = new System.Windows.Forms.Button();
			this.animationName = new System.Windows.Forms.TextBox();
			this.animationSpeed = new System.Windows.Forms.NumericUpDown();
			this.endTypeBox = new System.Windows.Forms.GroupBox();
			this.endTypeFA = new System.Windows.Forms.RadioButton();
			this.endTypeFB = new System.Windows.Forms.RadioButton();
			this.endTypeFC = new System.Windows.Forms.RadioButton();
			this.endAnimBox = new System.Windows.Forms.ComboBox();
			this.endAnimNum = new System.Windows.Forms.NumericUpDown();
			this.endTypeFD = new System.Windows.Forms.RadioButton();
			this.endFrameNum = new System.Windows.Forms.NumericUpDown();
			this.endTypeFE = new System.Windows.Forms.RadioButton();
			this.endTypeFF = new System.Windows.Forms.RadioButton();
			this.mappingFrameList = new SonicRetro.SonLVL.API.TileList();
			noneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			noneToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			label1 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			label5 = new System.Windows.Forms.Label();
			tableLayoutPanel1.SuspendLayout();
			tableLayoutPanel3.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.previewBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.previewTimer)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.animationSpeed)).BeginInit();
			this.endTypeBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.endAnimNum)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.endFrameNum)).BeginInit();
			this.SuspendLayout();
			// 
			// noneToolStripMenuItem
			// 
			noneToolStripMenuItem.Enabled = false;
			noneToolStripMenuItem.Name = "noneToolStripMenuItem";
			noneToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
			noneToolStripMenuItem.Text = "(none)";
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new System.Drawing.Size(152, 6);
			// 
			// noneToolStripMenuItem1
			// 
			noneToolStripMenuItem1.Enabled = false;
			noneToolStripMenuItem1.Name = "noneToolStripMenuItem1";
			noneToolStripMenuItem1.Size = new System.Drawing.Size(109, 22);
			noneToolStripMenuItem1.Text = "(none)";
			// 
			// toolStripSeparator2
			// 
			toolStripSeparator2.Name = "toolStripSeparator2";
			toolStripSeparator2.Size = new System.Drawing.Size(152, 6);
			// 
			// tableLayoutPanel1
			// 
			tableLayoutPanel1.AutoSize = true;
			tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			tableLayoutPanel1.ColumnCount = 2;
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			tableLayoutPanel1.Controls.Add(this.animationListBox, 0, 1);
			tableLayoutPanel1.Controls.Add(label1, 0, 0);
			tableLayoutPanel1.Controls.Add(this.addAnimationButton, 0, 2);
			tableLayoutPanel1.Controls.Add(this.removeAnimationButton, 1, 2);
			tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Left;
			tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 3;
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel1.Size = new System.Drawing.Size(126, 538);
			tableLayoutPanel1.TabIndex = 0;
			// 
			// animationListBox
			// 
			tableLayoutPanel1.SetColumnSpan(this.animationListBox, 2);
			this.animationListBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.animationListBox.Enabled = false;
			this.animationListBox.Location = new System.Drawing.Point(3, 16);
			this.animationListBox.Name = "animationListBox";
			this.animationListBox.Size = new System.Drawing.Size(120, 490);
			this.animationListBox.TabIndex = 1;
			this.animationListBox.SelectedIndexChanged += new System.EventHandler(this.animationListBox_SelectedIndexChanged);
			// 
			// label1
			// 
			label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left)));
			label1.AutoSize = true;
			tableLayoutPanel1.SetColumnSpan(label1, 2);
			label1.Location = new System.Drawing.Point(3, 0);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(61, 13);
			label1.TabIndex = 2;
			label1.Text = "Animations:";
			label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// addAnimationButton
			// 
			this.addAnimationButton.Enabled = false;
			this.addAnimationButton.Location = new System.Drawing.Point(3, 512);
			this.addAnimationButton.Name = "addAnimationButton";
			this.addAnimationButton.Size = new System.Drawing.Size(57, 23);
			this.addAnimationButton.TabIndex = 3;
			this.addAnimationButton.Text = "Add";
			this.addAnimationButton.UseVisualStyleBackColor = true;
			// 
			// removeAnimationButton
			// 
			this.removeAnimationButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.removeAnimationButton.Enabled = false;
			this.removeAnimationButton.Location = new System.Drawing.Point(66, 512);
			this.removeAnimationButton.Name = "removeAnimationButton";
			this.removeAnimationButton.Size = new System.Drawing.Size(57, 23);
			this.removeAnimationButton.TabIndex = 4;
			this.removeAnimationButton.Text = "Remove";
			this.removeAnimationButton.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(132, 30);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(38, 13);
			label3.TabIndex = 9;
			label3.Text = "Name:";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(132, 55);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(41, 13);
			label4.TabIndex = 11;
			label4.Text = "Speed:";
			// 
			// tableLayoutPanel3
			// 
			tableLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			tableLayoutPanel3.ColumnCount = 1;
			tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			tableLayoutPanel3.Controls.Add(label5, 0, 0);
			tableLayoutPanel3.Controls.Add(this.animationFrameList, 0, 1);
			tableLayoutPanel3.Location = new System.Drawing.Point(132, 79);
			tableLayoutPanel3.Name = "tableLayoutPanel3";
			tableLayoutPanel3.RowCount = 2;
			tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel3.Size = new System.Drawing.Size(546, 119);
			tableLayoutPanel3.TabIndex = 13;
			// 
			// label5
			// 
			label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left)));
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(3, 0);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(44, 13);
			label5.TabIndex = 3;
			label5.Text = "Frames:";
			label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// animationFrameList
			// 
			this.animationFrameList.AllowDrop = true;
			this.animationFrameList.BackColor = System.Drawing.SystemColors.Window;
			this.animationFrameList.Direction = SonicRetro.SonLVL.API.Direction.Horizontal;
			this.animationFrameList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.animationFrameList.Enabled = false;
			this.animationFrameList.ImageSize = 64;
			this.animationFrameList.Location = new System.Drawing.Point(3, 16);
			this.animationFrameList.Name = "animationFrameList";
			this.animationFrameList.ScrollValue = 0;
			this.animationFrameList.SelectedIndex = -1;
			this.animationFrameList.Size = new System.Drawing.Size(540, 100);
			this.animationFrameList.TabIndex = 1;
			this.animationFrameList.ItemDrag += new System.EventHandler(this.animationFrameList_ItemDrag);
			this.animationFrameList.DragDrop += new System.Windows.Forms.DragEventHandler(this.animationFrameList_DragDrop);
			this.animationFrameList.DragEnter += new System.Windows.Forms.DragEventHandler(this.animationFrameList_DragEnter);
			this.animationFrameList.DragOver += new System.Windows.Forms.DragEventHandler(this.animationFrameList_DragOver);
			this.animationFrameList.DragLeave += new System.EventHandler(this.animationFrameList_DragLeave);
			this.animationFrameList.Paint += new System.Windows.Forms.PaintEventHandler(this.animationFrameList_Paint);
			this.animationFrameList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.animationFrameList_KeyDown);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.fileToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(784, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.openToolStripMenuItem,
			this.animationToolStripMenuItem,
			this.saveToolStripMenuItem,
			toolStripSeparator1,
			this.recentProjectsToolStripMenuItem,
			toolStripSeparator2,
			this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			this.openToolStripMenuItem.Text = "&Open...";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// animationToolStripMenuItem
			// 
			this.animationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			noneToolStripMenuItem});
			this.animationToolStripMenuItem.Name = "animationToolStripMenuItem";
			this.animationToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			this.animationToolStripMenuItem.Text = "&Animation";
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			this.saveToolStripMenuItem.Text = "&Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// recentProjectsToolStripMenuItem
			// 
			this.recentProjectsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			noneToolStripMenuItem1});
			this.recentProjectsToolStripMenuItem.Name = "recentProjectsToolStripMenuItem";
			this.recentProjectsToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			this.recentProjectsToolStripMenuItem.Text = "&Recent Projects";
			this.recentProjectsToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.recentProjectsToolStripMenuItem_DropDownItemClicked);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// backgroundLevelLoader
			// 
			this.backgroundLevelLoader.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundLevelLoader_DoWork);
			this.backgroundLevelLoader.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundLevelLoader_RunWorkerCompleted);
			// 
			// playButton
			// 
			this.playButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.playButton.AutoSize = true;
			this.playButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.playButton.Font = new System.Drawing.Font("Webdings", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
			this.playButton.Location = new System.Drawing.Point(53, 153);
			this.playButton.Name = "playButton";
			this.playButton.Size = new System.Drawing.Size(35, 29);
			this.playButton.TabIndex = 3;
			this.playButton.Text = "4";
			this.playButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.playButton.UseVisualStyleBackColor = true;
			this.playButton.Click += new System.EventHandler(this.playButton_Click);
			// 
			// stopButton
			// 
			this.stopButton.AutoSize = true;
			this.stopButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.stopButton.Font = new System.Drawing.Font("Webdings", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
			this.stopButton.Location = new System.Drawing.Point(6, 153);
			this.stopButton.Name = "stopButton";
			this.stopButton.Size = new System.Drawing.Size(35, 29);
			this.stopButton.TabIndex = 4;
			this.stopButton.Text = "<";
			this.stopButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.stopButton.UseVisualStyleBackColor = true;
			this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
			// 
			// nextButton
			// 
			this.nextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.nextButton.AutoSize = true;
			this.nextButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.nextButton.Font = new System.Drawing.Font("Webdings", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
			this.nextButton.Location = new System.Drawing.Point(99, 153);
			this.nextButton.Name = "nextButton";
			this.nextButton.Size = new System.Drawing.Size(35, 29);
			this.nextButton.TabIndex = 5;
			this.nextButton.Text = ":";
			this.nextButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.nextButton.UseVisualStyleBackColor = true;
			this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
			// 
			// previewPanel
			// 
			this.previewPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.previewPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.previewPanel.Location = new System.Drawing.Point(6, 19);
			this.previewPanel.Name = "previewPanel";
			this.previewPanel.Size = new System.Drawing.Size(128, 128);
			this.previewPanel.TabIndex = 6;
			this.previewPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.previewPanel_Paint);
			// 
			// previewBox
			// 
			this.previewBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.previewBox.AutoSize = true;
			this.previewBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.previewBox.Controls.Add(this.previewPanel);
			this.previewBox.Controls.Add(this.playButton);
			this.previewBox.Controls.Add(this.nextButton);
			this.previewBox.Controls.Add(this.stopButton);
			this.previewBox.Enabled = false;
			this.previewBox.Location = new System.Drawing.Point(532, 204);
			this.previewBox.Name = "previewBox";
			this.previewBox.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this.previewBox.Size = new System.Drawing.Size(140, 198);
			this.previewBox.TabIndex = 7;
			this.previewBox.TabStop = false;
			this.previewBox.Text = "Preview";
			// 
			// previewTimer
			// 
			this.previewTimer.Interval = 16.6666D;
			this.previewTimer.SynchronizingObject = this;
			this.previewTimer.Elapsed += new System.Timers.ElapsedEventHandler(this.previewTimer_Elapsed);
			// 
			// exportGIFButton
			// 
			this.exportGIFButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.exportGIFButton.AutoSize = true;
			this.exportGIFButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.exportGIFButton.Enabled = false;
			this.exportGIFButton.Location = new System.Drawing.Point(532, 408);
			this.exportGIFButton.Name = "exportGIFButton";
			this.exportGIFButton.Size = new System.Drawing.Size(67, 23);
			this.exportGIFButton.TabIndex = 8;
			this.exportGIFButton.Text = "Export GIF";
			this.exportGIFButton.UseVisualStyleBackColor = true;
			this.exportGIFButton.Click += new System.EventHandler(this.exportGIFButton_Click);
			// 
			// animationName
			// 
			this.animationName.Enabled = false;
			this.animationName.Location = new System.Drawing.Point(176, 27);
			this.animationName.Name = "animationName";
			this.animationName.Size = new System.Drawing.Size(116, 20);
			this.animationName.TabIndex = 10;
			// 
			// animationSpeed
			// 
			this.animationSpeed.Enabled = false;
			this.animationSpeed.Hexadecimal = true;
			this.animationSpeed.Location = new System.Drawing.Point(179, 53);
			this.animationSpeed.Maximum = new decimal(new int[] {
			255,
			0,
			0,
			0});
			this.animationSpeed.Name = "animationSpeed";
			this.animationSpeed.Size = new System.Drawing.Size(50, 20);
			this.animationSpeed.TabIndex = 12;
			// 
			// endTypeBox
			// 
			this.endTypeBox.AutoSize = true;
			this.endTypeBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.endTypeBox.Controls.Add(this.endTypeFA);
			this.endTypeBox.Controls.Add(this.endTypeFB);
			this.endTypeBox.Controls.Add(this.endTypeFC);
			this.endTypeBox.Controls.Add(this.endAnimBox);
			this.endTypeBox.Controls.Add(this.endAnimNum);
			this.endTypeBox.Controls.Add(this.endTypeFD);
			this.endTypeBox.Controls.Add(this.endFrameNum);
			this.endTypeBox.Controls.Add(this.endTypeFE);
			this.endTypeBox.Controls.Add(this.endTypeFF);
			this.endTypeBox.Enabled = false;
			this.endTypeBox.Location = new System.Drawing.Point(132, 204);
			this.endTypeBox.Name = "endTypeBox";
			this.endTypeBox.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this.endTypeBox.Size = new System.Drawing.Size(306, 173);
			this.endTypeBox.TabIndex = 14;
			this.endTypeBox.TabStop = false;
			this.endTypeBox.Text = "End Type";
			// 
			// endTypeFA
			// 
			this.endTypeFA.AutoSize = true;
			this.endTypeFA.Location = new System.Drawing.Point(6, 140);
			this.endTypeFA.Name = "endTypeFA";
			this.endTypeFA.Size = new System.Drawing.Size(192, 17);
			this.endTypeFA.TabIndex = 19;
			this.endTypeFA.TabStop = true;
			this.endTypeFA.Text = "Increment Second Routine Counter";
			this.endTypeFA.UseVisualStyleBackColor = true;
			this.endTypeFA.CheckedChanged += new System.EventHandler(this.endTypeFA_CheckedChanged);
			// 
			// endTypeFB
			// 
			this.endTypeFB.AutoSize = true;
			this.endTypeFB.Location = new System.Drawing.Point(6, 117);
			this.endTypeFB.Name = "endTypeFB";
			this.endTypeFB.Size = new System.Drawing.Size(243, 17);
			this.endTypeFB.TabIndex = 18;
			this.endTypeFB.TabStop = true;
			this.endTypeFB.Text = "Reset Animation and Second Routine Counter";
			this.endTypeFB.UseVisualStyleBackColor = true;
			this.endTypeFB.CheckedChanged += new System.EventHandler(this.endTypeFB_CheckedChanged);
			// 
			// endTypeFC
			// 
			this.endTypeFC.AutoSize = true;
			this.endTypeFC.Location = new System.Drawing.Point(6, 94);
			this.endTypeFC.Name = "endTypeFC";
			this.endTypeFC.Size = new System.Drawing.Size(152, 17);
			this.endTypeFC.TabIndex = 17;
			this.endTypeFC.TabStop = true;
			this.endTypeFC.Text = "Increment Routine Counter";
			this.endTypeFC.UseVisualStyleBackColor = true;
			this.endTypeFC.CheckedChanged += new System.EventHandler(this.endTypeFC_CheckedChanged);
			// 
			// endAnimBox
			// 
			this.endAnimBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.endAnimBox.Enabled = false;
			this.endAnimBox.FormattingEnabled = true;
			this.endAnimBox.Location = new System.Drawing.Point(179, 67);
			this.endAnimBox.Name = "endAnimBox";
			this.endAnimBox.Size = new System.Drawing.Size(121, 21);
			this.endAnimBox.TabIndex = 16;
			this.endAnimBox.SelectedIndexChanged += new System.EventHandler(this.endAnimBox_SelectedIndexChanged);
			// 
			// endAnimNum
			// 
			this.endAnimNum.Enabled = false;
			this.endAnimNum.Hexadecimal = true;
			this.endAnimNum.Location = new System.Drawing.Point(123, 68);
			this.endAnimNum.Maximum = new decimal(new int[] {
			255,
			0,
			0,
			0});
			this.endAnimNum.Name = "endAnimNum";
			this.endAnimNum.Size = new System.Drawing.Size(50, 20);
			this.endAnimNum.TabIndex = 15;
			this.endAnimNum.ValueChanged += new System.EventHandler(this.endAnimNum_ValueChanged);
			// 
			// endTypeFD
			// 
			this.endTypeFD.AutoSize = true;
			this.endTypeFD.Location = new System.Drawing.Point(6, 68);
			this.endTypeFD.Name = "endTypeFD";
			this.endTypeFD.Size = new System.Drawing.Size(111, 17);
			this.endTypeFD.TabIndex = 14;
			this.endTypeFD.TabStop = true;
			this.endTypeFD.Text = "Change Animation";
			this.endTypeFD.UseVisualStyleBackColor = true;
			this.endTypeFD.CheckedChanged += new System.EventHandler(this.endTypeFD_CheckedChanged);
			// 
			// endFrameNum
			// 
			this.endFrameNum.Enabled = false;
			this.endFrameNum.Hexadecimal = true;
			this.endFrameNum.Location = new System.Drawing.Point(99, 42);
			this.endFrameNum.Maximum = new decimal(new int[] {
			255,
			0,
			0,
			0});
			this.endFrameNum.Name = "endFrameNum";
			this.endFrameNum.Size = new System.Drawing.Size(50, 20);
			this.endFrameNum.TabIndex = 13;
			this.endFrameNum.ValueChanged += new System.EventHandler(this.endFrameNum_ValueChanged);
			// 
			// endTypeFE
			// 
			this.endTypeFE.AutoSize = true;
			this.endTypeFE.Location = new System.Drawing.Point(6, 42);
			this.endTypeFE.Name = "endTypeFE";
			this.endTypeFE.Size = new System.Drawing.Size(87, 17);
			this.endTypeFE.TabIndex = 1;
			this.endTypeFE.TabStop = true;
			this.endTypeFE.Text = "Go To Frame";
			this.endTypeFE.UseVisualStyleBackColor = true;
			this.endTypeFE.CheckedChanged += new System.EventHandler(this.endTypeFE_CheckedChanged);
			// 
			// endTypeFF
			// 
			this.endTypeFF.AutoSize = true;
			this.endTypeFF.Location = new System.Drawing.Point(6, 19);
			this.endTypeFF.Name = "endTypeFF";
			this.endTypeFF.Size = new System.Drawing.Size(128, 17);
			this.endTypeFF.TabIndex = 0;
			this.endTypeFF.TabStop = true;
			this.endTypeFF.Text = "Loop Entire Animation";
			this.endTypeFF.UseVisualStyleBackColor = true;
			this.endTypeFF.CheckedChanged += new System.EventHandler(this.endTypeFF_CheckedChanged);
			// 
			// mappingFrameList
			// 
			this.mappingFrameList.BackColor = System.Drawing.SystemColors.Window;
			this.mappingFrameList.Dock = System.Windows.Forms.DockStyle.Right;
			this.mappingFrameList.Enabled = false;
			this.mappingFrameList.ImageSize = 64;
			this.mappingFrameList.Location = new System.Drawing.Point(684, 24);
			this.mappingFrameList.Name = "mappingFrameList";
			this.mappingFrameList.ScrollValue = 0;
			this.mappingFrameList.SelectedIndex = -1;
			this.mappingFrameList.Size = new System.Drawing.Size(100, 538);
			this.mappingFrameList.TabIndex = 1;
			this.mappingFrameList.ItemDrag += new System.EventHandler(this.mappingFrameList_ItemDrag);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 562);
			this.Controls.Add(this.endTypeBox);
			this.Controls.Add(tableLayoutPanel3);
			this.Controls.Add(this.mappingFrameList);
			this.Controls.Add(this.animationSpeed);
			this.Controls.Add(label4);
			this.Controls.Add(this.animationName);
			this.Controls.Add(label3);
			this.Controls.Add(this.exportGIFButton);
			this.Controls.Add(this.previewBox);
			this.Controls.Add(tableLayoutPanel1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainForm";
			this.Text = "SonAni";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Load += new System.EventHandler(this.MainForm_Load);
			tableLayoutPanel1.ResumeLayout(false);
			tableLayoutPanel1.PerformLayout();
			tableLayoutPanel3.ResumeLayout(false);
			tableLayoutPanel3.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.previewBox.ResumeLayout(false);
			this.previewBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.previewTimer)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.animationSpeed)).EndInit();
			this.endTypeBox.ResumeLayout(false);
			this.endTypeBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.endAnimNum)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.endFrameNum)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem animationToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem recentProjectsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.ComponentModel.BackgroundWorker backgroundLevelLoader;
		private System.Windows.Forms.ListBox animationListBox;
		private System.Windows.Forms.Button addAnimationButton;
		private System.Windows.Forms.Button removeAnimationButton;
		private SonicRetro.SonLVL.API.TileList mappingFrameList;
		private System.Windows.Forms.Button playButton;
		private System.Windows.Forms.Button stopButton;
		private System.Windows.Forms.Button nextButton;
		private System.Windows.Forms.Panel previewPanel;
		private System.Windows.Forms.GroupBox previewBox;
		private System.Timers.Timer previewTimer;
		private System.Windows.Forms.Button exportGIFButton;
		private System.Windows.Forms.TextBox animationName;
		private System.Windows.Forms.NumericUpDown animationSpeed;
		private SonicRetro.SonLVL.API.TileList animationFrameList;
		private System.Windows.Forms.GroupBox endTypeBox;
		private System.Windows.Forms.RadioButton endTypeFF;
		private System.Windows.Forms.RadioButton endTypeFE;
		private System.Windows.Forms.NumericUpDown endFrameNum;
		private System.Windows.Forms.NumericUpDown endAnimNum;
		private System.Windows.Forms.RadioButton endTypeFD;
		private System.Windows.Forms.ComboBox endAnimBox;
		private System.Windows.Forms.RadioButton endTypeFA;
		private System.Windows.Forms.RadioButton endTypeFB;
		private System.Windows.Forms.RadioButton endTypeFC;
	}
}

