namespace S3SSEdit
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.MenuStrip menuStrip1;
            System.Windows.Forms.ImageList imageList1;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.Panel panel1;
            System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
            System.Windows.Forms.ToolTip toolTip1;
            System.Windows.Forms.Button countButton;
            System.Windows.Forms.Panel panel4;
            System.Windows.Forms.Panel panel5;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolOptionsPanel = new System.Windows.Forms.Panel();
            this.perfectCount = new System.Windows.Forms.NumericUpDown();
            this.layoutPanel = new System.Windows.Forms.UserControl();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.layoutContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.foreSpherePicture = new System.Windows.Forms.PictureBox();
            this.backSpherePicture = new System.Windows.Forms.PictureBox();
            this.paletteYellow = new System.Windows.Forms.PictureBox();
            this.paletteRing = new System.Windows.Forms.PictureBox();
            this.paletteBumper = new System.Windows.Forms.PictureBox();
            this.paletteBlue = new System.Windows.Forms.PictureBox();
            this.paletteRed = new System.Windows.Forms.PictureBox();
            this.paletteErase = new System.Windows.Forms.PictureBox();
            this.startButton = new System.Windows.Forms.RadioButton();
            this.lineButton = new System.Windows.Forms.RadioButton();
            this.fillButton = new System.Windows.Forms.RadioButton();
            this.pencilButton = new System.Windows.Forms.RadioButton();
            this.selectButton = new System.Windows.Forms.RadioButton();
            this.rectangleButton = new System.Windows.Forms.RadioButton();
            this.diamondButton = new System.Windows.Forms.RadioButton();
            this.ovalButton = new System.Windows.Forms.RadioButton();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteOnceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteRepeatingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flipHorizontallyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.flipVerticallyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotateLeftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rotateRightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteSectionOnceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteSectionRepeatingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveScreenshotDialog = new System.Windows.Forms.SaveFileDialog();
            this.saveImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            imageList1 = new System.Windows.Forms.ImageList(this.components);
            panel1 = new System.Windows.Forms.Panel();
            tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            countButton = new System.Windows.Forms.Button();
            panel4 = new System.Windows.Forms.Panel();
            panel5 = new System.Windows.Forms.Panel();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            menuStrip1.SuspendLayout();
            panel1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.perfectCount)).BeginInit();
            panel4.SuspendLayout();
            panel5.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.layoutContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.foreSpherePicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.backSpherePicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.paletteYellow)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.paletteRing)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.paletteBumper)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.paletteBlue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.paletteRed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.paletteErase)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            menuStrip1.Size = new System.Drawing.Size(779, 28);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.saveImageToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newToolStripMenuItem.Size = new System.Drawing.Size(182, 26);
            this.newToolStripMenuItem.Text = "&New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(182, 26);
            this.openToolStripMenuItem.Text = "&Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(182, 26);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(182, 26);
            this.saveAsToolStripMenuItem.Text = "Save &As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeyDisplayString = "Alt+F4";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(182, 26);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(47, 24);
            this.editToolStripMenuItem.Text = "&Edit";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Enabled = false;
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Z";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.undoToolStripMenuItem.Text = "&Undo";
            this.undoToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.undoToolStripMenuItem_DropDownItemClicked);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Enabled = false;
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Y";
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.redoToolStripMenuItem.Text = "&Redo";
            this.redoToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.redoToolStripMenuItem_DropDownItemClicked);
            // 
            // imageList1
            // 
            imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            imageList1.TransparentColor = System.Drawing.Color.Red;
            imageList1.Images.SetKeyName(0, "select");
            imageList1.Images.SetKeyName(1, "pencil");
            imageList1.Images.SetKeyName(2, "fill");
            imageList1.Images.SetKeyName(3, "line");
            imageList1.Images.SetKeyName(4, "rectangle");
            imageList1.Images.SetKeyName(5, "diamond");
            imageList1.Images.SetKeyName(6, "oval");
            imageList1.Images.SetKeyName(7, "start");
            // 
            // panel1
            // 
            panel1.AutoSize = true;
            panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            panel1.Controls.Add(this.toolOptionsPanel);
            panel1.Controls.Add(tableLayoutPanel1);
            panel1.Dock = System.Windows.Forms.DockStyle.Left;
            panel1.Location = new System.Drawing.Point(0, 28);
            panel1.Margin = new System.Windows.Forms.Padding(4);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(80, 664);
            panel1.TabIndex = 2;
            // 
            // toolOptionsPanel
            // 
            this.toolOptionsPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.toolOptionsPanel.Location = new System.Drawing.Point(12, 151);
            this.toolOptionsPanel.Margin = new System.Windows.Forms.Padding(4);
            this.toolOptionsPanel.Name = "toolOptionsPanel";
            this.toolOptionsPanel.Size = new System.Drawing.Size(55, 80);
            this.toolOptionsPanel.TabIndex = 1;
            this.toolOptionsPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.toolOptionsPanel_Paint);
            this.toolOptionsPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.toolOptionsPanel_MouseClick);
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.AutoSize = true;
            tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tableLayoutPanel1.Controls.Add(this.startButton, 0, 3);
            tableLayoutPanel1.Controls.Add(this.lineButton, 1, 1);
            tableLayoutPanel1.Controls.Add(this.fillButton, 0, 1);
            tableLayoutPanel1.Controls.Add(this.pencilButton, 1, 0);
            tableLayoutPanel1.Controls.Add(this.selectButton, 0, 0);
            tableLayoutPanel1.Controls.Add(this.rectangleButton, 0, 2);
            tableLayoutPanel1.Controls.Add(this.diamondButton, 1, 2);
            tableLayoutPanel1.Controls.Add(this.ovalButton, 0, 3);
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tableLayoutPanel1.Size = new System.Drawing.Size(80, 152);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // perfectCount
            // 
            this.perfectCount.Location = new System.Drawing.Point(71, 7);
            this.perfectCount.Margin = new System.Windows.Forms.Padding(4);
            this.perfectCount.Maximum = new decimal(new int[] {
            1023,
            0,
            0,
            0});
            this.perfectCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.perfectCount.Name = "perfectCount";
            this.perfectCount.Size = new System.Drawing.Size(69, 22);
            this.perfectCount.TabIndex = 6;
            toolTip1.SetToolTip(this.perfectCount, "The number of rings required to get a Perfect Bonus.");
            this.perfectCount.ValueChanged += new System.EventHandler(this.perfectCount_ValueChanged);
            // 
            // countButton
            // 
            countButton.AutoSize = true;
            countButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            countButton.Location = new System.Drawing.Point(148, 4);
            countButton.Margin = new System.Windows.Forms.Padding(4);
            countButton.Name = "countButton";
            countButton.Size = new System.Drawing.Size(55, 27);
            countButton.TabIndex = 7;
            countButton.Text = "Count";
            toolTip1.SetToolTip(countButton, "Automatically calculates the number of rings in the stage.");
            countButton.UseVisualStyleBackColor = true;
            countButton.Visible = false;
            countButton.Click += new System.EventHandler(this.countButton_Click);
            // 
            // panel4
            // 
            panel4.AutoScroll = true;
            panel4.BackColor = System.Drawing.SystemColors.ControlDark;
            panel4.Controls.Add(this.layoutPanel);
            panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            panel4.Location = new System.Drawing.Point(80, 75);
            panel4.Margin = new System.Windows.Forms.Padding(4);
            panel4.Name = "panel4";
            panel4.Size = new System.Drawing.Size(492, 617);
            panel4.TabIndex = 1;
            // 
            // layoutPanel
            // 
            this.layoutPanel.BackColor = System.Drawing.Color.White;
            this.layoutPanel.Location = new System.Drawing.Point(0, 0);
            this.layoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.layoutPanel.Name = "layoutPanel";
            this.layoutPanel.Size = new System.Drawing.Size(1024, 945);
            this.layoutPanel.TabIndex = 0;
            this.layoutPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.layoutPanel_Paint);
            this.layoutPanel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.layoutPanel_KeyDown);
            this.layoutPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.layoutPanel_MouseDown);
            this.layoutPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.layoutPanel_MouseMove);
            this.layoutPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.layoutPanel_MouseUp);
            // 
            // panel5
            // 
            panel5.AutoSize = true;
            panel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            panel5.Controls.Add(countButton);
            panel5.Controls.Add(this.perfectCount);
            panel5.Controls.Add(this.label3);
            panel5.Dock = System.Windows.Forms.DockStyle.Right;
            panel5.Location = new System.Drawing.Point(572, 28);
            panel5.Margin = new System.Windows.Forms.Padding(4);
            panel5.Name = "panel5";
            panel5.Size = new System.Drawing.Size(207, 664);
            panel5.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 10);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "Perfect:";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(239, 6);
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(239, 6);
            toolStripSeparator2.Visible = false;
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.paletteYellow);
            this.panel2.Controls.Add(this.paletteRing);
            this.panel2.Controls.Add(this.paletteBumper);
            this.panel2.Controls.Add(this.paletteBlue);
            this.panel2.Controls.Add(this.paletteRed);
            this.panel2.Controls.Add(this.paletteErase);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(80, 28);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(492, 47);
            this.panel2.TabIndex = 4;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.foreSpherePicture);
            this.panel3.Controls.Add(this.backSpherePicture);
            this.panel3.Location = new System.Drawing.Point(8, 4);
            this.panel3.Margin = new System.Windows.Forms.Padding(4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(43, 39);
            this.panel3.TabIndex = 0;
            // 
            // layoutContextMenuStrip
            // 
            this.layoutContextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.layoutContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteOnceToolStripMenuItem,
            this.pasteRepeatingToolStripMenuItem,
            this.importToolStripMenuItem,
            toolStripSeparator1,
            this.flipHorizontallyToolStripMenuItem,
            this.flipVerticallyToolStripMenuItem,
            this.rotateLeftToolStripMenuItem,
            this.rotateRightToolStripMenuItem,
            toolStripSeparator2,
            this.saveSectionToolStripMenuItem,
            this.pasteSectionOnceToolStripMenuItem,
            this.pasteSectionRepeatingToolStripMenuItem});
            this.layoutContextMenuStrip.Name = "layoutContextMenuStrip";
            this.layoutContextMenuStrip.Size = new System.Drawing.Size(243, 328);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(242, 26);
            this.importToolStripMenuItem.Text = "I&mport...";
            this.importToolStripMenuItem.Visible = false;
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // saveSectionToolStripMenuItem
            // 
            this.saveSectionToolStripMenuItem.Name = "saveSectionToolStripMenuItem";
            this.saveSectionToolStripMenuItem.Size = new System.Drawing.Size(242, 26);
            this.saveSectionToolStripMenuItem.Text = "&Save Section...";
            this.saveSectionToolStripMenuItem.Visible = false;
            this.saveSectionToolStripMenuItem.Click += new System.EventHandler(this.saveSectionToolStripMenuItem_Click);
            // 
            // foreSpherePicture
            // 
            this.foreSpherePicture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.foreSpherePicture.Location = new System.Drawing.Point(0, 0);
            this.foreSpherePicture.Margin = new System.Windows.Forms.Padding(0);
            this.foreSpherePicture.Name = "foreSpherePicture";
            this.foreSpherePicture.Size = new System.Drawing.Size(26, 24);
            this.foreSpherePicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.foreSpherePicture.TabIndex = 1;
            this.foreSpherePicture.TabStop = false;
            toolTip1.SetToolTip(this.foreSpherePicture, "Click To Swap");
            this.foreSpherePicture.Click += new System.EventHandler(this.foreSpherePicture_Click);
            // 
            // backSpherePicture
            // 
            this.backSpherePicture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.backSpherePicture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.backSpherePicture.Location = new System.Drawing.Point(16, 15);
            this.backSpherePicture.Margin = new System.Windows.Forms.Padding(0);
            this.backSpherePicture.Name = "backSpherePicture";
            this.backSpherePicture.Size = new System.Drawing.Size(26, 24);
            this.backSpherePicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.backSpherePicture.TabIndex = 0;
            this.backSpherePicture.TabStop = false;
            toolTip1.SetToolTip(this.backSpherePicture, "Click To Swap");
            this.backSpherePicture.Click += new System.EventHandler(this.foreSpherePicture_Click);
            // 
            // paletteYellow
            // 
            this.paletteYellow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.paletteYellow.Location = new System.Drawing.Point(241, 6);
            this.paletteYellow.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.paletteYellow.Name = "paletteYellow";
            this.paletteYellow.Size = new System.Drawing.Size(36, 34);
            this.paletteYellow.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.paletteYellow.TabIndex = 5;
            this.paletteYellow.TabStop = false;
            toolTip1.SetToolTip(this.paletteYellow, "Yellow Sphere");
            this.paletteYellow.MouseClick += new System.Windows.Forms.MouseEventHandler(this.paletteYellow_MouseClick);
            // 
            // paletteRing
            // 
            this.paletteRing.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.paletteRing.Location = new System.Drawing.Point(204, 6);
            this.paletteRing.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.paletteRing.Name = "paletteRing";
            this.paletteRing.Size = new System.Drawing.Size(36, 34);
            this.paletteRing.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.paletteRing.TabIndex = 4;
            this.paletteRing.TabStop = false;
            toolTip1.SetToolTip(this.paletteRing, "Ring");
            this.paletteRing.MouseClick += new System.Windows.Forms.MouseEventHandler(this.paletteRing_MouseClick);
            // 
            // paletteBumper
            // 
            this.paletteBumper.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.paletteBumper.Location = new System.Drawing.Point(167, 6);
            this.paletteBumper.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.paletteBumper.Name = "paletteBumper";
            this.paletteBumper.Size = new System.Drawing.Size(36, 34);
            this.paletteBumper.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.paletteBumper.TabIndex = 3;
            this.paletteBumper.TabStop = false;
            toolTip1.SetToolTip(this.paletteBumper, "Bumper");
            this.paletteBumper.MouseClick += new System.Windows.Forms.MouseEventHandler(this.paletteBumper_MouseClick);
            // 
            // paletteBlue
            // 
            this.paletteBlue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.paletteBlue.Location = new System.Drawing.Point(129, 6);
            this.paletteBlue.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.paletteBlue.Name = "paletteBlue";
            this.paletteBlue.Size = new System.Drawing.Size(36, 34);
            this.paletteBlue.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.paletteBlue.TabIndex = 2;
            this.paletteBlue.TabStop = false;
            toolTip1.SetToolTip(this.paletteBlue, "Blue Sphere");
            this.paletteBlue.MouseClick += new System.Windows.Forms.MouseEventHandler(this.paletteBlue_MouseClick);
            // 
            // paletteRed
            // 
            this.paletteRed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.paletteRed.Location = new System.Drawing.Point(92, 6);
            this.paletteRed.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.paletteRed.Name = "paletteRed";
            this.paletteRed.Size = new System.Drawing.Size(36, 34);
            this.paletteRed.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.paletteRed.TabIndex = 1;
            this.paletteRed.TabStop = false;
            toolTip1.SetToolTip(this.paletteRed, "Red Sphere");
            this.paletteRed.MouseClick += new System.Windows.Forms.MouseEventHandler(this.paletteRed_MouseClick);
            // 
            // paletteErase
            // 
            this.paletteErase.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.paletteErase.Location = new System.Drawing.Point(55, 6);
            this.paletteErase.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.paletteErase.Name = "paletteErase";
            this.paletteErase.Size = new System.Drawing.Size(36, 34);
            this.paletteErase.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.paletteErase.TabIndex = 0;
            this.paletteErase.TabStop = false;
            toolTip1.SetToolTip(this.paletteErase, "Erase");
            this.paletteErase.MouseClick += new System.Windows.Forms.MouseEventHandler(this.paletteErase_MouseClick);
            // 
            // startButton
            // 
            this.startButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.startButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.startButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.startButton.ImageKey = "start";
            this.startButton.ImageList = imageList1;
            this.startButton.Location = new System.Drawing.Point(44, 118);
            this.startButton.Margin = new System.Windows.Forms.Padding(4);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(32, 30);
            this.startButton.TabIndex = 7;
            toolTip1.SetToolTip(this.startButton, "Start Position Tool");
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.CheckedChanged += new System.EventHandler(this.startButton_CheckedChanged);
            // 
            // lineButton
            // 
            this.lineButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lineButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.lineButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lineButton.ImageKey = "line";
            this.lineButton.ImageList = imageList1;
            this.lineButton.Location = new System.Drawing.Point(44, 42);
            this.lineButton.Margin = new System.Windows.Forms.Padding(4);
            this.lineButton.Name = "lineButton";
            this.lineButton.Size = new System.Drawing.Size(32, 30);
            this.lineButton.TabIndex = 3;
            toolTip1.SetToolTip(this.lineButton, "Line Tool");
            this.lineButton.UseVisualStyleBackColor = true;
            this.lineButton.CheckedChanged += new System.EventHandler(this.lineButton_CheckedChanged);
            // 
            // fillButton
            // 
            this.fillButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.fillButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.fillButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.fillButton.ImageKey = "fill";
            this.fillButton.ImageList = imageList1;
            this.fillButton.Location = new System.Drawing.Point(4, 42);
            this.fillButton.Margin = new System.Windows.Forms.Padding(4);
            this.fillButton.Name = "fillButton";
            this.fillButton.Size = new System.Drawing.Size(32, 30);
            this.fillButton.TabIndex = 2;
            toolTip1.SetToolTip(this.fillButton, "Fill Tool");
            this.fillButton.UseVisualStyleBackColor = true;
            this.fillButton.CheckedChanged += new System.EventHandler(this.fillButton_CheckedChanged);
            // 
            // pencilButton
            // 
            this.pencilButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pencilButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.pencilButton.Checked = true;
            this.pencilButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.pencilButton.ImageKey = "pencil";
            this.pencilButton.ImageList = imageList1;
            this.pencilButton.Location = new System.Drawing.Point(44, 4);
            this.pencilButton.Margin = new System.Windows.Forms.Padding(4);
            this.pencilButton.Name = "pencilButton";
            this.pencilButton.Size = new System.Drawing.Size(32, 30);
            this.pencilButton.TabIndex = 1;
            this.pencilButton.TabStop = true;
            toolTip1.SetToolTip(this.pencilButton, "Pencil Tool");
            this.pencilButton.UseVisualStyleBackColor = true;
            this.pencilButton.CheckedChanged += new System.EventHandler(this.pencilButton_CheckedChanged);
            // 
            // selectButton
            // 
            this.selectButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.selectButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.selectButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.selectButton.ImageKey = "select";
            this.selectButton.ImageList = imageList1;
            this.selectButton.Location = new System.Drawing.Point(4, 4);
            this.selectButton.Margin = new System.Windows.Forms.Padding(4);
            this.selectButton.Name = "selectButton";
            this.selectButton.Size = new System.Drawing.Size(32, 30);
            this.selectButton.TabIndex = 0;
            toolTip1.SetToolTip(this.selectButton, "Select Tool");
            this.selectButton.UseVisualStyleBackColor = true;
            this.selectButton.CheckedChanged += new System.EventHandler(this.selectButton_CheckedChanged);
            // 
            // rectangleButton
            // 
            this.rectangleButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.rectangleButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.rectangleButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rectangleButton.ImageKey = "rectangle";
            this.rectangleButton.ImageList = imageList1;
            this.rectangleButton.Location = new System.Drawing.Point(4, 80);
            this.rectangleButton.Margin = new System.Windows.Forms.Padding(4);
            this.rectangleButton.Name = "rectangleButton";
            this.rectangleButton.Size = new System.Drawing.Size(32, 30);
            this.rectangleButton.TabIndex = 4;
            toolTip1.SetToolTip(this.rectangleButton, "Rectangle Tool");
            this.rectangleButton.UseVisualStyleBackColor = true;
            this.rectangleButton.CheckedChanged += new System.EventHandler(this.rectangleButton_CheckedChanged);
            // 
            // diamondButton
            // 
            this.diamondButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.diamondButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.diamondButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.diamondButton.ImageKey = "diamond";
            this.diamondButton.ImageList = imageList1;
            this.diamondButton.Location = new System.Drawing.Point(44, 80);
            this.diamondButton.Margin = new System.Windows.Forms.Padding(4);
            this.diamondButton.Name = "diamondButton";
            this.diamondButton.Size = new System.Drawing.Size(32, 30);
            this.diamondButton.TabIndex = 6;
            toolTip1.SetToolTip(this.diamondButton, "Diamond Tool");
            this.diamondButton.UseVisualStyleBackColor = true;
            this.diamondButton.CheckedChanged += new System.EventHandler(this.diamondButton_CheckedChanged);
            // 
            // ovalButton
            // 
            this.ovalButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ovalButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.ovalButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ovalButton.ImageKey = "oval";
            this.ovalButton.ImageList = imageList1;
            this.ovalButton.Location = new System.Drawing.Point(4, 118);
            this.ovalButton.Margin = new System.Windows.Forms.Padding(4);
            this.ovalButton.Name = "ovalButton";
            this.ovalButton.Size = new System.Drawing.Size(32, 30);
            this.ovalButton.TabIndex = 5;
            toolTip1.SetToolTip(this.ovalButton, "Oval Tool");
            this.ovalButton.UseVisualStyleBackColor = true;
            this.ovalButton.CheckedChanged += new System.EventHandler(this.ovalButton_CheckedChanged);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Image = global::S3SSEdit.Properties.Resources.cut;
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(242, 26);
            this.cutToolStripMenuItem.Text = "Cu&t";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Image = global::S3SSEdit.Properties.Resources.copy;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(242, 26);
            this.copyToolStripMenuItem.Text = "&Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteOnceToolStripMenuItem
            // 
            this.pasteOnceToolStripMenuItem.Image = global::S3SSEdit.Properties.Resources.paste;
            this.pasteOnceToolStripMenuItem.Name = "pasteOnceToolStripMenuItem";
            this.pasteOnceToolStripMenuItem.Size = new System.Drawing.Size(242, 26);
            this.pasteOnceToolStripMenuItem.Text = "&Paste Once";
            this.pasteOnceToolStripMenuItem.Click += new System.EventHandler(this.pasteOnceToolStripMenuItem_Click);
            // 
            // pasteRepeatingToolStripMenuItem
            // 
            this.pasteRepeatingToolStripMenuItem.Image = global::S3SSEdit.Properties.Resources.paste;
            this.pasteRepeatingToolStripMenuItem.Name = "pasteRepeatingToolStripMenuItem";
            this.pasteRepeatingToolStripMenuItem.Size = new System.Drawing.Size(242, 26);
            this.pasteRepeatingToolStripMenuItem.Text = "Paste &Repeating";
            this.pasteRepeatingToolStripMenuItem.Click += new System.EventHandler(this.pasteRepeatingToolStripMenuItem_Click);
            // 
            // flipHorizontallyToolStripMenuItem
            // 
            this.flipHorizontallyToolStripMenuItem.Image = global::S3SSEdit.Properties.Resources.fliph;
            this.flipHorizontallyToolStripMenuItem.Name = "flipHorizontallyToolStripMenuItem";
            this.flipHorizontallyToolStripMenuItem.Size = new System.Drawing.Size(242, 26);
            this.flipHorizontallyToolStripMenuItem.Text = "Flip &Horizontally";
            this.flipHorizontallyToolStripMenuItem.Click += new System.EventHandler(this.flipHorizontallyToolStripMenuItem_Click);
            // 
            // flipVerticallyToolStripMenuItem
            // 
            this.flipVerticallyToolStripMenuItem.Image = global::S3SSEdit.Properties.Resources.flipv;
            this.flipVerticallyToolStripMenuItem.Name = "flipVerticallyToolStripMenuItem";
            this.flipVerticallyToolStripMenuItem.Size = new System.Drawing.Size(242, 26);
            this.flipVerticallyToolStripMenuItem.Text = "Flip &Vertically";
            this.flipVerticallyToolStripMenuItem.Click += new System.EventHandler(this.flipVerticallyToolStripMenuItem_Click);
            // 
            // rotateLeftToolStripMenuItem
            // 
            this.rotateLeftToolStripMenuItem.Image = global::S3SSEdit.Properties.Resources.rotatel;
            this.rotateLeftToolStripMenuItem.Name = "rotateLeftToolStripMenuItem";
            this.rotateLeftToolStripMenuItem.Size = new System.Drawing.Size(242, 26);
            this.rotateLeftToolStripMenuItem.Text = "Rotate &Left";
            this.rotateLeftToolStripMenuItem.Click += new System.EventHandler(this.rotateLeftToolStripMenuItem_Click);
            // 
            // rotateRightToolStripMenuItem
            // 
            this.rotateRightToolStripMenuItem.Image = global::S3SSEdit.Properties.Resources.rotater;
            this.rotateRightToolStripMenuItem.Name = "rotateRightToolStripMenuItem";
            this.rotateRightToolStripMenuItem.Size = new System.Drawing.Size(242, 26);
            this.rotateRightToolStripMenuItem.Text = "Rotate R&ight";
            this.rotateRightToolStripMenuItem.Click += new System.EventHandler(this.rotateRightToolStripMenuItem_Click);
            // 
            // pasteSectionOnceToolStripMenuItem
            // 
            this.pasteSectionOnceToolStripMenuItem.Image = global::S3SSEdit.Properties.Resources.paste;
            this.pasteSectionOnceToolStripMenuItem.Name = "pasteSectionOnceToolStripMenuItem";
            this.pasteSectionOnceToolStripMenuItem.Size = new System.Drawing.Size(242, 26);
            this.pasteSectionOnceToolStripMenuItem.Text = "P&aste Section Once";
            this.pasteSectionOnceToolStripMenuItem.Visible = false;
            this.pasteSectionOnceToolStripMenuItem.Click += new System.EventHandler(this.pasteSectionOnceToolStripMenuItem_Click);
            // 
            // pasteSectionRepeatingToolStripMenuItem
            // 
            this.pasteSectionRepeatingToolStripMenuItem.Image = global::S3SSEdit.Properties.Resources.paste;
            this.pasteSectionRepeatingToolStripMenuItem.Name = "pasteSectionRepeatingToolStripMenuItem";
            this.pasteSectionRepeatingToolStripMenuItem.Size = new System.Drawing.Size(242, 26);
            this.pasteSectionRepeatingToolStripMenuItem.Text = "Paste Section R&epeating";
            this.pasteSectionRepeatingToolStripMenuItem.Visible = false;
            this.pasteSectionRepeatingToolStripMenuItem.Click += new System.EventHandler(this.pasteSectionRepeatingToolStripMenuItem_Click);
            // 
            // saveScreenshotDialog
            // 
            this.saveScreenshotDialog.DefaultExt = "png";
            this.saveScreenshotDialog.Filter = "PNG image|*.png|JPG image|*.jpg|GIF image|*.gif|All Files|*.*";
            this.saveScreenshotDialog.Title = "Save Image To...";
            // 
            // saveImageToolStripMenuItem
            // 
            this.saveImageToolStripMenuItem.Name = "saveImageToolStripMenuItem";
            this.saveImageToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.saveImageToolStripMenuItem.Size = new System.Drawing.Size(207, 26);
            this.saveImageToolStripMenuItem.Text = "Save Image";
            this.saveImageToolStripMenuItem.Click += new System.EventHandler(this.saveImageToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 692);
            this.Controls.Add(panel4);
            this.Controls.Add(this.panel2);
            this.Controls.Add(panel1);
            this.Controls.Add(panel5);
            this.Controls.Add(menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "S3SSEdit - New Stage";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.perfectCount)).EndInit();
            panel4.ResumeLayout(false);
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.layoutContextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.foreSpherePicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.backSpherePicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.paletteYellow)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.paletteRing)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.paletteBumper)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.paletteBlue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.paletteRed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.paletteErase)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton selectButton;
		private System.Windows.Forms.RadioButton fillButton;
		private System.Windows.Forms.RadioButton pencilButton;
		private System.Windows.Forms.RadioButton lineButton;
		private System.Windows.Forms.RadioButton rectangleButton;
		private System.Windows.Forms.RadioButton ovalButton;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.PictureBox paletteBlue;
		private System.Windows.Forms.PictureBox paletteRed;
		private System.Windows.Forms.PictureBox paletteErase;
		private System.Windows.Forms.PictureBox paletteYellow;
		private System.Windows.Forms.PictureBox paletteRing;
		private System.Windows.Forms.PictureBox paletteBumper;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.UserControl layoutPanel;
		private System.Windows.Forms.PictureBox backSpherePicture;
		private System.Windows.Forms.PictureBox foreSpherePicture;
		private System.Windows.Forms.NumericUpDown perfectCount;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
		private System.Windows.Forms.Panel toolOptionsPanel;
		private System.Windows.Forms.RadioButton diamondButton;
		private System.Windows.Forms.RadioButton startButton;
		private System.Windows.Forms.ContextMenuStrip layoutContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteOnceToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteRepeatingToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveSectionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteSectionOnceToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteSectionRepeatingToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem flipHorizontallyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem flipVerticallyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem rotateLeftToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem rotateRightToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveScreenshotDialog;
        private System.Windows.Forms.ToolStripMenuItem saveImageToolStripMenuItem;
    }
}

