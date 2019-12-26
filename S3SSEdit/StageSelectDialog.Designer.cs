namespace S3SSEdit
{
	partial class StageSelectDialog
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
			System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
			System.Windows.Forms.GroupBox groupBox1;
			System.Windows.Forms.GroupBox groupBox2;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Panel panel2;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label5;
			System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
			this.categoryBSLayout = new System.Windows.Forms.RadioButton();
			this.categoryBSChunk = new System.Windows.Forms.RadioButton();
			this.categorySK = new System.Windows.Forms.RadioButton();
			this.categoryS3 = new System.Windows.Forms.RadioButton();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this.bsStageControlPanel = new System.Windows.Forms.TableLayoutPanel();
			this.bsLevelButton = new System.Windows.Forms.Button();
			this.bsStageButton = new System.Windows.Forms.Button();
			this.bsStageChunk4 = new System.Windows.Forms.NumericUpDown();
			this.bsStageChunk3 = new System.Windows.Forms.NumericUpDown();
			this.bsStageChunk2 = new System.Windows.Forms.NumericUpDown();
			this.bsStageChunk1 = new System.Windows.Forms.NumericUpDown();
			this.bsStageNum = new System.Windows.Forms.NumericUpDown();
			this.bsLevelNum = new System.Windows.Forms.NumericUpDown();
			this.bsROMButton = new System.Windows.Forms.Button();
			this.bsChunksButton = new System.Windows.Forms.Button();
			this.bsCode = new System.Windows.Forms.MaskedTextBox();
			this.bsCodeButton = new System.Windows.Forms.Button();
			this.stageList = new System.Windows.Forms.ListBox();
			this.chunkSelector = new System.Windows.Forms.NumericUpDown();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			groupBox1 = new System.Windows.Forms.GroupBox();
			groupBox2 = new System.Windows.Forms.GroupBox();
			label4 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			panel2 = new System.Windows.Forms.Panel();
			label3 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			tableLayoutPanel1.SuspendLayout();
			groupBox1.SuspendLayout();
			groupBox2.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this.bsStageControlPanel.SuspendLayout();
			panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.bsStageChunk4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bsStageChunk3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bsStageChunk2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bsStageChunk1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bsStageNum)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bsLevelNum)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chunkSelector)).BeginInit();
			tableLayoutPanel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			tableLayoutPanel1.ColumnCount = 3;
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel1.Controls.Add(groupBox1, 0, 0);
			tableLayoutPanel1.Controls.Add(groupBox2, 1, 0);
			tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 2, 0);
			tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			tableLayoutPanel1.Name = "tableLayoutPanel1";
			tableLayoutPanel1.RowCount = 1;
			tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel1.Size = new System.Drawing.Size(584, 261);
			tableLayoutPanel1.TabIndex = 0;
			// 
			// groupBox1
			// 
			groupBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
			groupBox1.AutoSize = true;
			groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			groupBox1.Controls.Add(this.categoryBSLayout);
			groupBox1.Controls.Add(this.categoryBSChunk);
			groupBox1.Controls.Add(this.categorySK);
			groupBox1.Controls.Add(this.categoryS3);
			groupBox1.Location = new System.Drawing.Point(3, 68);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(156, 124);
			groupBox1.TabIndex = 0;
			groupBox1.TabStop = false;
			groupBox1.Text = "Category";
			// 
			// categoryBSLayout
			// 
			this.categoryBSLayout.AutoSize = true;
			this.categoryBSLayout.Location = new System.Drawing.Point(6, 88);
			this.categoryBSLayout.Name = "categoryBSLayout";
			this.categoryBSLayout.Size = new System.Drawing.Size(119, 17);
			this.categoryBSLayout.TabIndex = 3;
			this.categoryBSLayout.TabStop = true;
			this.categoryBSLayout.Text = "&Blue Sphere Stages";
			this.categoryBSLayout.UseVisualStyleBackColor = true;
			this.categoryBSLayout.CheckedChanged += new System.EventHandler(this.categoryBSLayout_CheckedChanged);
			// 
			// categoryBSChunk
			// 
			this.categoryBSChunk.AutoSize = true;
			this.categoryBSChunk.Location = new System.Drawing.Point(6, 65);
			this.categoryBSChunk.Name = "categoryBSChunk";
			this.categoryBSChunk.Size = new System.Drawing.Size(122, 17);
			this.categoryBSChunk.TabIndex = 2;
			this.categoryBSChunk.TabStop = true;
			this.categoryBSChunk.Text = "Blue Sphere &Chunks";
			this.categoryBSChunk.UseVisualStyleBackColor = true;
			this.categoryBSChunk.CheckedChanged += new System.EventHandler(this.categoryBSChunk_CheckedChanged);
			// 
			// categorySK
			// 
			this.categorySK.AutoSize = true;
			this.categorySK.Location = new System.Drawing.Point(6, 42);
			this.categorySK.Name = "categorySK";
			this.categorySK.Size = new System.Drawing.Size(144, 17);
			this.categorySK.TabIndex = 1;
			this.categorySK.Text = "Sonic && &Knuckles Stages";
			this.categorySK.UseVisualStyleBackColor = true;
			this.categorySK.CheckedChanged += new System.EventHandler(this.categorySK_CheckedChanged);
			// 
			// categoryS3
			// 
			this.categoryS3.AutoSize = true;
			this.categoryS3.Checked = true;
			this.categoryS3.Location = new System.Drawing.Point(6, 19);
			this.categoryS3.Name = "categoryS3";
			this.categoryS3.Size = new System.Drawing.Size(97, 17);
			this.categoryS3.TabIndex = 0;
			this.categoryS3.TabStop = true;
			this.categoryS3.Text = "Sonic &3 Stages";
			this.categoryS3.UseVisualStyleBackColor = true;
			this.categoryS3.CheckedChanged += new System.EventHandler(this.categoryS3_CheckedChanged);
			// 
			// groupBox2
			// 
			groupBox2.Anchor = System.Windows.Forms.AnchorStyles.None;
			groupBox2.AutoSize = true;
			groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			groupBox2.Controls.Add(this.tableLayoutPanel3);
			groupBox2.Location = new System.Drawing.Point(165, 3);
			groupBox2.Name = "groupBox2";
			groupBox2.Size = new System.Drawing.Size(202, 255);
			groupBox2.TabIndex = 1;
			groupBox2.TabStop = false;
			groupBox2.Text = "Stage";
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.AutoSize = true;
			this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel3.ColumnCount = 1;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel3.Controls.Add(this.bsStageControlPanel, 0, 2);
			this.tableLayoutPanel3.Controls.Add(this.stageList, 0, 0);
			this.tableLayoutPanel3.Controls.Add(this.chunkSelector, 0, 1);
			this.tableLayoutPanel3.Location = new System.Drawing.Point(6, 19);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 3;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.Size = new System.Drawing.Size(190, 296);
			this.tableLayoutPanel3.TabIndex = 1;
			// 
			// bsStageControlPanel
			// 
			this.bsStageControlPanel.AutoSize = true;
			this.bsStageControlPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.bsStageControlPanel.ColumnCount = 3;
			this.bsStageControlPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.bsStageControlPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.bsStageControlPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.bsStageControlPanel.Controls.Add(this.bsLevelButton, 2, 2);
			this.bsStageControlPanel.Controls.Add(this.bsStageButton, 2, 1);
			this.bsStageControlPanel.Controls.Add(label4, 0, 0);
			this.bsStageControlPanel.Controls.Add(label1, 0, 1);
			this.bsStageControlPanel.Controls.Add(panel2, 1, 0);
			this.bsStageControlPanel.Controls.Add(this.bsStageNum, 1, 1);
			this.bsStageControlPanel.Controls.Add(this.bsLevelNum, 1, 2);
			this.bsStageControlPanel.Controls.Add(label3, 0, 4);
			this.bsStageControlPanel.Controls.Add(label2, 0, 2);
			this.bsStageControlPanel.Controls.Add(this.bsROMButton, 1, 4);
			this.bsStageControlPanel.Controls.Add(this.bsChunksButton, 2, 0);
			this.bsStageControlPanel.Controls.Add(label5, 0, 3);
			this.bsStageControlPanel.Controls.Add(this.bsCode, 1, 3);
			this.bsStageControlPanel.Controls.Add(this.bsCodeButton, 2, 3);
			this.bsStageControlPanel.Location = new System.Drawing.Point(0, 128);
			this.bsStageControlPanel.Margin = new System.Windows.Forms.Padding(0);
			this.bsStageControlPanel.Name = "bsStageControlPanel";
			this.bsStageControlPanel.RowCount = 5;
			this.bsStageControlPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.bsStageControlPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.bsStageControlPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.bsStageControlPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.bsStageControlPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.bsStageControlPanel.Size = new System.Drawing.Size(190, 168);
			this.bsStageControlPanel.TabIndex = 2;
			this.bsStageControlPanel.Visible = false;
			// 
			// bsLevelButton
			// 
			this.bsLevelButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.bsLevelButton.AutoSize = true;
			this.bsLevelButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.bsLevelButton.Location = new System.Drawing.Point(156, 84);
			this.bsLevelButton.Name = "bsLevelButton";
			this.bsLevelButton.Size = new System.Drawing.Size(31, 23);
			this.bsLevelButton.TabIndex = 10;
			this.bsLevelButton.Text = "Go";
			this.bsLevelButton.UseVisualStyleBackColor = true;
			this.bsLevelButton.Click += new System.EventHandler(this.bsLevelButton_Click);
			// 
			// bsStageButton
			// 
			this.bsStageButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.bsStageButton.AutoSize = true;
			this.bsStageButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.bsStageButton.Location = new System.Drawing.Point(156, 55);
			this.bsStageButton.Name = "bsStageButton";
			this.bsStageButton.Size = new System.Drawing.Size(31, 23);
			this.bsStageButton.TabIndex = 9;
			this.bsStageButton.Text = "Go";
			this.bsStageButton.UseVisualStyleBackColor = true;
			this.bsStageButton.Click += new System.EventHandler(this.bsStageButton_Click);
			// 
			// label4
			// 
			label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(3, 19);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(46, 13);
			label4.TabIndex = 7;
			label4.Text = "Chunks:";
			label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label1
			// 
			label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(3, 60);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(38, 13);
			label1.TabIndex = 0;
			label1.Text = "Stage:";
			label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// panel2
			// 
			panel2.Anchor = System.Windows.Forms.AnchorStyles.None;
			panel2.AutoSize = true;
			panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			panel2.Controls.Add(this.bsStageChunk4);
			panel2.Controls.Add(this.bsStageChunk3);
			panel2.Controls.Add(this.bsStageChunk2);
			panel2.Controls.Add(this.bsStageChunk1);
			panel2.Location = new System.Drawing.Point(52, 0);
			panel2.Margin = new System.Windows.Forms.Padding(0);
			panel2.Name = "panel2";
			panel2.Size = new System.Drawing.Size(101, 52);
			panel2.TabIndex = 2;
			// 
			// bsStageChunk4
			// 
			this.bsStageChunk4.Location = new System.Drawing.Point(52, 29);
			this.bsStageChunk4.Maximum = new decimal(new int[] {
            127,
            0,
            0,
            0});
			this.bsStageChunk4.Name = "bsStageChunk4";
			this.bsStageChunk4.Size = new System.Drawing.Size(46, 20);
			this.bsStageChunk4.TabIndex = 11;
			// 
			// bsStageChunk3
			// 
			this.bsStageChunk3.Location = new System.Drawing.Point(0, 29);
			this.bsStageChunk3.Maximum = new decimal(new int[] {
            127,
            0,
            0,
            0});
			this.bsStageChunk3.Name = "bsStageChunk3";
			this.bsStageChunk3.Size = new System.Drawing.Size(46, 20);
			this.bsStageChunk3.TabIndex = 10;
			// 
			// bsStageChunk2
			// 
			this.bsStageChunk2.Location = new System.Drawing.Point(52, 3);
			this.bsStageChunk2.Maximum = new decimal(new int[] {
            127,
            0,
            0,
            0});
			this.bsStageChunk2.Name = "bsStageChunk2";
			this.bsStageChunk2.Size = new System.Drawing.Size(46, 20);
			this.bsStageChunk2.TabIndex = 9;
			// 
			// bsStageChunk1
			// 
			this.bsStageChunk1.Location = new System.Drawing.Point(0, 3);
			this.bsStageChunk1.Maximum = new decimal(new int[] {
            127,
            0,
            0,
            0});
			this.bsStageChunk1.Name = "bsStageChunk1";
			this.bsStageChunk1.Size = new System.Drawing.Size(46, 20);
			this.bsStageChunk1.TabIndex = 8;
			// 
			// bsStageNum
			// 
			this.bsStageNum.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.bsStageNum.Location = new System.Drawing.Point(55, 56);
			this.bsStageNum.Maximum = new decimal(new int[] {
            268435455,
            0,
            0,
            0});
			this.bsStageNum.Name = "bsStageNum";
			this.bsStageNum.Size = new System.Drawing.Size(85, 20);
			this.bsStageNum.TabIndex = 1;
			// 
			// bsLevelNum
			// 
			this.bsLevelNum.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.bsLevelNum.Location = new System.Drawing.Point(55, 85);
			this.bsLevelNum.Maximum = new decimal(new int[] {
            134217728,
            0,
            0,
            0});
			this.bsLevelNum.Name = "bsLevelNum";
			this.bsLevelNum.Size = new System.Drawing.Size(85, 20);
			this.bsLevelNum.TabIndex = 4;
			this.bsLevelNum.ValueChanged += new System.EventHandler(this.bsLevelNum_ValueChanged);
			// 
			// label3
			// 
			label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(3, 147);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(35, 13);
			label3.TabIndex = 6;
			label3.Text = "ROM:";
			label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(3, 89);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(36, 13);
			label2.TabIndex = 3;
			label2.Text = "Level:";
			label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// bsROMButton
			// 
			this.bsROMButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.bsROMButton.AutoSize = true;
			this.bsROMButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.bsROMButton.Location = new System.Drawing.Point(55, 142);
			this.bsROMButton.Name = "bsROMButton";
			this.bsROMButton.Size = new System.Drawing.Size(75, 23);
			this.bsROMButton.TabIndex = 5;
			this.bsROMButton.Text = "Select File...";
			this.bsROMButton.UseVisualStyleBackColor = true;
			this.bsROMButton.Click += new System.EventHandler(this.bsROMButton_Click);
			// 
			// bsChunksButton
			// 
			this.bsChunksButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.bsChunksButton.AutoSize = true;
			this.bsChunksButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.bsChunksButton.Location = new System.Drawing.Point(156, 14);
			this.bsChunksButton.Name = "bsChunksButton";
			this.bsChunksButton.Size = new System.Drawing.Size(31, 23);
			this.bsChunksButton.TabIndex = 8;
			this.bsChunksButton.Text = "Go";
			this.bsChunksButton.UseVisualStyleBackColor = true;
			this.bsChunksButton.Click += new System.EventHandler(this.bsChunksButton_Click);
			// 
			// label5
			// 
			label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(3, 118);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(35, 13);
			label5.TabIndex = 11;
			label5.Text = "Code:";
			label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// bsCode
			// 
			this.bsCode.AllowPromptAsInput = false;
			this.bsCode.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.bsCode.AsciiOnly = true;
			this.bsCode.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
			this.bsCode.Location = new System.Drawing.Point(55, 114);
			this.bsCode.Mask = "0000 0000 0000";
			this.bsCode.Name = "bsCode";
			this.bsCode.ResetOnPrompt = false;
			this.bsCode.ResetOnSpace = false;
			this.bsCode.Size = new System.Drawing.Size(90, 20);
			this.bsCode.TabIndex = 12;
			this.bsCode.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
			this.bsCode.TextChanged += new System.EventHandler(this.bsCode_TextChanged);
			// 
			// bsCodeButton
			// 
			this.bsCodeButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.bsCodeButton.AutoSize = true;
			this.bsCodeButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.bsCodeButton.Location = new System.Drawing.Point(156, 113);
			this.bsCodeButton.Name = "bsCodeButton";
			this.bsCodeButton.Size = new System.Drawing.Size(31, 23);
			this.bsCodeButton.TabIndex = 13;
			this.bsCodeButton.Text = "Go";
			this.bsCodeButton.UseVisualStyleBackColor = true;
			this.bsCodeButton.Click += new System.EventHandler(this.bsCodeButton_Click);
			// 
			// stageList
			// 
			this.stageList.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.stageList.FormattingEnabled = true;
			this.stageList.Items.AddRange(new object[] {
            "Stage 1",
            "Stage 2",
            "Stage 3",
            "Stage 4",
            "Stage 5",
            "Stage 6",
            "Stage 7",
            "Stage 8 (Secret)"});
			this.stageList.Location = new System.Drawing.Point(35, 0);
			this.stageList.Margin = new System.Windows.Forms.Padding(0);
			this.stageList.Name = "stageList";
			this.stageList.Size = new System.Drawing.Size(120, 108);
			this.stageList.TabIndex = 0;
			this.stageList.SelectedIndexChanged += new System.EventHandler(this.stageList_SelectedIndexChanged);
			// 
			// chunkSelector
			// 
			this.chunkSelector.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.chunkSelector.Location = new System.Drawing.Point(62, 108);
			this.chunkSelector.Margin = new System.Windows.Forms.Padding(0);
			this.chunkSelector.Maximum = new decimal(new int[] {
            127,
            0,
            0,
            0});
			this.chunkSelector.Name = "chunkSelector";
			this.chunkSelector.Size = new System.Drawing.Size(66, 20);
			this.chunkSelector.TabIndex = 1;
			this.chunkSelector.Visible = false;
			this.chunkSelector.ValueChanged += new System.EventHandler(this.chunkSelector_ValueChanged);
			// 
			// tableLayoutPanel2
			// 
			tableLayoutPanel2.ColumnCount = 1;
			tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel2.Controls.Add(this.pictureBox1, 0, 0);
			tableLayoutPanel2.Controls.Add(this.panel1, 0, 1);
			tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			tableLayoutPanel2.Location = new System.Drawing.Point(370, 0);
			tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
			tableLayoutPanel2.Name = "tableLayoutPanel2";
			tableLayoutPanel2.RowCount = 2;
			tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel2.Size = new System.Drawing.Size(214, 261);
			tableLayoutPanel2.TabIndex = 2;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox1.Location = new System.Drawing.Point(0, 3);
			this.pictureBox1.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(211, 229);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.AutoSize = true;
			this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel1.Controls.Add(this.okButton);
			this.panel1.Controls.Add(this.cancelButton);
			this.panel1.Location = new System.Drawing.Point(52, 232);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(162, 29);
			this.panel1.TabIndex = 1;
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(3, 3);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 1;
			this.okButton.Text = "&OK";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(84, 3);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 0;
			this.cancelButton.Text = "&Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// StageSelectDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(584, 261);
			this.Controls.Add(tableLayoutPanel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "StageSelectDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Select A Stage";
			this.Load += new System.EventHandler(this.StageSelectDialog_Load);
			tableLayoutPanel1.ResumeLayout(false);
			tableLayoutPanel1.PerformLayout();
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			this.bsStageControlPanel.ResumeLayout(false);
			this.bsStageControlPanel.PerformLayout();
			panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.bsStageChunk4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bsStageChunk3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bsStageChunk2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bsStageChunk1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bsStageNum)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bsLevelNum)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chunkSelector)).EndInit();
			tableLayoutPanel2.ResumeLayout(false);
			tableLayoutPanel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.RadioButton categorySK;
		private System.Windows.Forms.RadioButton categoryS3;
		private System.Windows.Forms.RadioButton categoryBSLayout;
		private System.Windows.Forms.RadioButton categoryBSChunk;
		private System.Windows.Forms.ListBox stageList;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private System.Windows.Forms.NumericUpDown chunkSelector;
		private System.Windows.Forms.NumericUpDown bsStageNum;
		private System.Windows.Forms.NumericUpDown bsLevelNum;
		private System.Windows.Forms.Button bsROMButton;
		private System.Windows.Forms.NumericUpDown bsStageChunk1;
		private System.Windows.Forms.NumericUpDown bsStageChunk2;
		private System.Windows.Forms.NumericUpDown bsStageChunk3;
		private System.Windows.Forms.NumericUpDown bsStageChunk4;
		private System.Windows.Forms.TableLayoutPanel bsStageControlPanel;
		private System.Windows.Forms.Button bsChunksButton;
		private System.Windows.Forms.Button bsLevelButton;
		private System.Windows.Forms.Button bsStageButton;
		private System.Windows.Forms.MaskedTextBox bsCode;
		private System.Windows.Forms.Button bsCodeButton;
	}
}