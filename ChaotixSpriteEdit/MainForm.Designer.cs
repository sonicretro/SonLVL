namespace ChaotixSpriteEdit
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
			System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
			System.Windows.Forms.Panel panel1;
			System.Windows.Forms.GroupBox groupBox1;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Panel panel2;
			System.Windows.Forms.Panel panel3;
			System.Windows.Forms.ToolStrip toolStrip1;
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.importFromROMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.palettePanel = new System.Windows.Forms.Panel();
			this.centerButton = new System.Windows.Forms.Button();
			this.showCenterCheckBox = new System.Windows.Forms.CheckBox();
			this.offsetYNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.offsetXNumericUpDown = new System.Windows.Forms.NumericUpDown();
			this.spriteImagePanel = new System.Windows.Forms.Panel();
			this.pencilToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.fillToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.importButton = new System.Windows.Forms.Button();
			this.exportButton = new System.Windows.Forms.Button();
			this.paletteContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.importPaletteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.importMDPaletteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportPaletteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			menuStrip1 = new System.Windows.Forms.MenuStrip();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			panel1 = new System.Windows.Forms.Panel();
			groupBox1 = new System.Windows.Forms.GroupBox();
			label2 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			panel2 = new System.Windows.Forms.Panel();
			panel3 = new System.Windows.Forms.Panel();
			toolStrip1 = new System.Windows.Forms.ToolStrip();
			menuStrip1.SuspendLayout();
			panel1.SuspendLayout();
			groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.offsetYNumericUpDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.offsetXNumericUpDown)).BeginInit();
			panel2.SuspendLayout();
			panel3.SuspendLayout();
			toolStrip1.SuspendLayout();
			this.paletteContextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
			menuStrip1.Location = new System.Drawing.Point(0, 0);
			menuStrip1.Name = "menuStrip1";
			menuStrip1.Size = new System.Drawing.Size(584, 24);
			menuStrip1.TabIndex = 0;
			menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.importFromROMToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            toolStripSeparator1,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// newToolStripMenuItem
			// 
			this.newToolStripMenuItem.Name = "newToolStripMenuItem";
			this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
			this.newToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.newToolStripMenuItem.Text = "&New";
			this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.openToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.openToolStripMenuItem.Text = "&Open...";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// importFromROMToolStripMenuItem
			// 
			this.importFromROMToolStripMenuItem.Name = "importFromROMToolStripMenuItem";
			this.importFromROMToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.importFromROMToolStripMenuItem.Text = "&Import from ROM...";
			this.importFromROMToolStripMenuItem.Click += new System.EventHandler(this.importFromROMToolStripMenuItem_Click);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Enabled = false;
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.saveToolStripMenuItem.Text = "&Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// saveAsToolStripMenuItem
			// 
			this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.S)));
			this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.saveAsToolStripMenuItem.Text = "Save &As...";
			this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new System.Drawing.Size(183, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// panel1
			// 
			panel1.AutoScroll = true;
			panel1.Controls.Add(this.palettePanel);
			panel1.Dock = System.Windows.Forms.DockStyle.Top;
			panel1.Location = new System.Drawing.Point(0, 24);
			panel1.Name = "panel1";
			panel1.Size = new System.Drawing.Size(584, 256);
			panel1.TabIndex = 1;
			// 
			// palettePanel
			// 
			this.palettePanel.Location = new System.Drawing.Point(0, 0);
			this.palettePanel.Margin = new System.Windows.Forms.Padding(0);
			this.palettePanel.Name = "palettePanel";
			this.palettePanel.Size = new System.Drawing.Size(512, 512);
			this.palettePanel.TabIndex = 0;
			this.palettePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.palettePanel_Paint);
			this.palettePanel.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.palettePanel_MouseDoubleClick);
			this.palettePanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.palettePanel_MouseDown);
			// 
			// groupBox1
			// 
			groupBox1.AutoSize = true;
			groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			groupBox1.Controls.Add(this.centerButton);
			groupBox1.Controls.Add(this.showCenterCheckBox);
			groupBox1.Controls.Add(this.offsetYNumericUpDown);
			groupBox1.Controls.Add(label2);
			groupBox1.Controls.Add(this.offsetXNumericUpDown);
			groupBox1.Controls.Add(label1);
			groupBox1.Location = new System.Drawing.Point(12, 286);
			groupBox1.Name = "groupBox1";
			groupBox1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
			groupBox1.Size = new System.Drawing.Size(182, 84);
			groupBox1.TabIndex = 2;
			groupBox1.TabStop = false;
			groupBox1.Text = "Offset";
			// 
			// centerButton
			// 
			this.centerButton.AutoSize = true;
			this.centerButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.centerButton.Location = new System.Drawing.Point(6, 45);
			this.centerButton.Name = "centerButton";
			this.centerButton.Size = new System.Drawing.Size(48, 23);
			this.centerButton.TabIndex = 4;
			this.centerButton.Text = "Center";
			this.centerButton.UseVisualStyleBackColor = true;
			this.centerButton.Click += new System.EventHandler(this.centerButton_Click);
			// 
			// showCenterCheckBox
			// 
			this.showCenterCheckBox.AutoSize = true;
			this.showCenterCheckBox.Location = new System.Drawing.Point(60, 49);
			this.showCenterCheckBox.Name = "showCenterCheckBox";
			this.showCenterCheckBox.Size = new System.Drawing.Size(53, 17);
			this.showCenterCheckBox.TabIndex = 17;
			this.showCenterCheckBox.Text = "Show";
			this.showCenterCheckBox.UseVisualStyleBackColor = true;
			this.showCenterCheckBox.CheckedChanged += new System.EventHandler(this.showCenterCheckBox_CheckedChanged);
			// 
			// offsetYNumericUpDown
			// 
			this.offsetYNumericUpDown.Location = new System.Drawing.Point(121, 19);
			this.offsetYNumericUpDown.Maximum = new decimal(new int[] {
            127,
            0,
            0,
            0});
			this.offsetYNumericUpDown.Minimum = new decimal(new int[] {
            128,
            0,
            0,
            -2147483648});
			this.offsetYNumericUpDown.Name = "offsetYNumericUpDown";
			this.offsetYNumericUpDown.Size = new System.Drawing.Size(55, 20);
			this.offsetYNumericUpDown.TabIndex = 3;
			this.offsetYNumericUpDown.Value = new decimal(new int[] {
            16,
            0,
            0,
            -2147483648});
			this.offsetYNumericUpDown.ValueChanged += new System.EventHandler(this.offsetYNumericUpDown_ValueChanged);
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(98, 21);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(17, 13);
			label2.TabIndex = 2;
			label2.Text = "Y:";
			// 
			// offsetXNumericUpDown
			// 
			this.offsetXNumericUpDown.Location = new System.Drawing.Point(29, 19);
			this.offsetXNumericUpDown.Maximum = new decimal(new int[] {
            32767,
            0,
            0,
            0});
			this.offsetXNumericUpDown.Minimum = new decimal(new int[] {
            32768,
            0,
            0,
            -2147483648});
			this.offsetXNumericUpDown.Name = "offsetXNumericUpDown";
			this.offsetXNumericUpDown.Size = new System.Drawing.Size(63, 20);
			this.offsetXNumericUpDown.TabIndex = 1;
			this.offsetXNumericUpDown.Value = new decimal(new int[] {
            16,
            0,
            0,
            -2147483648});
			this.offsetXNumericUpDown.ValueChanged += new System.EventHandler(this.offsetXNumericUpDown_ValueChanged);
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(6, 21);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(17, 13);
			label1.TabIndex = 0;
			label1.Text = "X:";
			// 
			// panel2
			// 
			panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			panel2.AutoScroll = true;
			panel2.Controls.Add(panel3);
			panel2.Controls.Add(toolStrip1);
			panel2.Location = new System.Drawing.Point(200, 286);
			panel2.Name = "panel2";
			panel2.Size = new System.Drawing.Size(372, 264);
			panel2.TabIndex = 16;
			// 
			// panel3
			// 
			panel3.Controls.Add(this.spriteImagePanel);
			panel3.Dock = System.Windows.Forms.DockStyle.Fill;
			panel3.Location = new System.Drawing.Point(24, 0);
			panel3.Name = "panel3";
			panel3.Size = new System.Drawing.Size(348, 264);
			panel3.TabIndex = 4;
			// 
			// spriteImagePanel
			// 
			this.spriteImagePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.spriteImagePanel.Location = new System.Drawing.Point(0, 0);
			this.spriteImagePanel.Margin = new System.Windows.Forms.Padding(0);
			this.spriteImagePanel.Name = "spriteImagePanel";
			this.spriteImagePanel.Size = new System.Drawing.Size(128, 128);
			this.spriteImagePanel.TabIndex = 3;
			this.spriteImagePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.spriteImagePanel_Paint);
			this.spriteImagePanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.spriteImagePanel_MouseDown);
			this.spriteImagePanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.spriteImagePanel_MouseMove);
			// 
			// toolStrip1
			// 
			toolStrip1.Dock = System.Windows.Forms.DockStyle.Left;
			toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pencilToolStripButton,
            this.fillToolStripButton});
			toolStrip1.Location = new System.Drawing.Point(0, 0);
			toolStrip1.Name = "toolStrip1";
			toolStrip1.Size = new System.Drawing.Size(24, 264);
			toolStrip1.TabIndex = 17;
			toolStrip1.Text = "toolStrip1";
			// 
			// pencilToolStripButton
			// 
			this.pencilToolStripButton.Checked = true;
			this.pencilToolStripButton.CheckState = System.Windows.Forms.CheckState.Checked;
			this.pencilToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.pencilToolStripButton.Image = global::ChaotixSpriteEdit.Properties.Resources.pencil;
			this.pencilToolStripButton.Name = "pencilToolStripButton";
			this.pencilToolStripButton.Size = new System.Drawing.Size(21, 20);
			this.pencilToolStripButton.Text = "Pencil";
			this.pencilToolStripButton.Click += new System.EventHandler(this.pencilToolStripButton_Click);
			// 
			// fillToolStripButton
			// 
			this.fillToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.fillToolStripButton.Image = global::ChaotixSpriteEdit.Properties.Resources.fill;
			this.fillToolStripButton.Name = "fillToolStripButton";
			this.fillToolStripButton.Size = new System.Drawing.Size(21, 20);
			this.fillToolStripButton.Text = "Fill";
			this.fillToolStripButton.Click += new System.EventHandler(this.fillToolStripButton_Click);
			// 
			// importButton
			// 
			this.importButton.AutoSize = true;
			this.importButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.importButton.Location = new System.Drawing.Point(12, 376);
			this.importButton.Name = "importButton";
			this.importButton.Size = new System.Drawing.Size(46, 23);
			this.importButton.TabIndex = 14;
			this.importButton.Text = "Import";
			this.importButton.UseVisualStyleBackColor = true;
			this.importButton.Click += new System.EventHandler(this.importButton_Click);
			// 
			// exportButton
			// 
			this.exportButton.AutoSize = true;
			this.exportButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.exportButton.Location = new System.Drawing.Point(64, 376);
			this.exportButton.Name = "exportButton";
			this.exportButton.Size = new System.Drawing.Size(47, 23);
			this.exportButton.TabIndex = 15;
			this.exportButton.Text = "Export";
			this.exportButton.UseVisualStyleBackColor = true;
			this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
			// 
			// paletteContextMenuStrip
			// 
			this.paletteContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importPaletteToolStripMenuItem,
            this.importMDPaletteToolStripMenuItem,
            this.exportPaletteToolStripMenuItem});
			this.paletteContextMenuStrip.Name = "paletteContextMenuStrip";
			this.paletteContextMenuStrip.Size = new System.Drawing.Size(153, 92);
			// 
			// importPaletteToolStripMenuItem
			// 
			this.importPaletteToolStripMenuItem.Name = "importPaletteToolStripMenuItem";
			this.importPaletteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.importPaletteToolStripMenuItem.Text = "Import...";
			this.importPaletteToolStripMenuItem.Click += new System.EventHandler(this.importPaletteToolStripMenuItem_Click);
			// 
			// importMDPaletteToolStripMenuItem
			// 
			this.importMDPaletteToolStripMenuItem.Name = "importMDPaletteToolStripMenuItem";
			this.importMDPaletteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.importMDPaletteToolStripMenuItem.Text = "Import MD...";
			this.importMDPaletteToolStripMenuItem.Click += new System.EventHandler(this.importMDPaletteToolStripMenuItem_Click);
			// 
			// exportPaletteToolStripMenuItem
			// 
			this.exportPaletteToolStripMenuItem.Name = "exportPaletteToolStripMenuItem";
			this.exportPaletteToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.exportPaletteToolStripMenuItem.Text = "Export...";
			this.exportPaletteToolStripMenuItem.Click += new System.EventHandler(this.exportPaletteToolStripMenuItem_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(584, 562);
			this.Controls.Add(panel2);
			this.Controls.Add(this.exportButton);
			this.Controls.Add(this.importButton);
			this.Controls.Add(groupBox1);
			this.Controls.Add(panel1);
			this.Controls.Add(menuStrip1);
			this.MainMenuStrip = menuStrip1;
			this.Name = "MainForm";
			this.Text = "Chaotix Sprite Editor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Load += new System.EventHandler(this.MainForm_Load);
			menuStrip1.ResumeLayout(false);
			menuStrip1.PerformLayout();
			panel1.ResumeLayout(false);
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.offsetYNumericUpDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.offsetXNumericUpDown)).EndInit();
			panel2.ResumeLayout(false);
			panel2.PerformLayout();
			panel3.ResumeLayout(false);
			toolStrip1.ResumeLayout(false);
			toolStrip1.PerformLayout();
			this.paletteContextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.Panel palettePanel;
		private System.Windows.Forms.NumericUpDown offsetXNumericUpDown;
		private System.Windows.Forms.NumericUpDown offsetYNumericUpDown;
		private System.Windows.Forms.Panel spriteImagePanel;
		private System.Windows.Forms.Button importButton;
		private System.Windows.Forms.Button exportButton;
		private System.Windows.Forms.CheckBox showCenterCheckBox;
		private System.Windows.Forms.ContextMenuStrip paletteContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem importPaletteToolStripMenuItem;
		private System.Windows.Forms.Button centerButton;
		private System.Windows.Forms.ToolStripButton pencilToolStripButton;
		private System.Windows.Forms.ToolStripButton fillToolStripButton;
		private System.Windows.Forms.ToolStripMenuItem importFromROMToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem importMDPaletteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exportPaletteToolStripMenuItem;
	}
}

