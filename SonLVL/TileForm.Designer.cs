namespace SonicRetro.SonLVL
{
    partial class TileForm
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ChunkSelector = new SonicRetro.SonLVL.BlockList();
            this.panel4 = new System.Windows.Forms.Panel();
            this.ChunkID = new System.Windows.Forms.TextBox();
            this.ChunkPicture = new System.Windows.Forms.Panel();
            this.ChunkBlockPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.BlockSelector = new SonicRetro.SonLVL.BlockList();
            this.panel5 = new System.Windows.Forms.Panel();
            this.BlockID = new System.Windows.Forms.TextBox();
            this.BlockPicture = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.BlockTilePropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.BlockCollision2 = new System.Windows.Forms.NumericUpDown();
            this.BlockCollision1 = new System.Windows.Forms.NumericUpDown();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.TileSelector = new SonicRetro.SonLVL.TileList();
            this.panel6 = new System.Windows.Forms.Panel();
            this.TileID = new System.Windows.Forms.TextBox();
            this.TilePicture = new System.Windows.Forms.Panel();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.CollisionSelector = new SonicRetro.SonLVL.TileList();
            this.panel3 = new System.Windows.Forms.Panel();
            this.ColID = new System.Windows.Forms.TextBox();
            this.ColAngle = new System.Windows.Forms.NumericUpDown();
            this.ColPicture = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.PalettePanel = new System.Windows.Forms.Panel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteBeforeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteAfterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertBeforeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.insertAfterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.importToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.ChunkCount = new System.Windows.Forms.Label();
            this.BlockCount = new System.Windows.Forms.Label();
            this.TileCount = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BlockCollision2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.BlockCollision1)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.panel6.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ColAngle)).BeginInit();
            this.panel1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 128);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(884, 436);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tableLayoutPanel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(876, 410);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Chunks";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.ChunkSelector, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel4, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ChunkBlockPropertyGrid, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(870, 404);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // ChunkSelector
            // 
            this.ChunkSelector.BackColor = System.Drawing.SystemColors.Window;
            this.ChunkSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChunkSelector.ImageSize = 128;
            this.ChunkSelector.Location = new System.Drawing.Point(561, 3);
            this.ChunkSelector.Name = "ChunkSelector";
            this.ChunkSelector.SelectedIndex = -1;
            this.ChunkSelector.Size = new System.Drawing.Size(306, 398);
            this.ChunkSelector.TabIndex = 2;
            this.ChunkSelector.SelectedIndexChanged += new System.EventHandler(this.ChunkSelector_SelectedIndexChanged);
            this.ChunkSelector.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChunkSelector_MouseDown);
            // 
            // panel4
            // 
            this.panel4.AutoSize = true;
            this.panel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel4.Controls.Add(this.ChunkCount);
            this.panel4.Controls.Add(this.ChunkID);
            this.panel4.Controls.Add(this.ChunkPicture);
            this.panel4.Location = new System.Drawing.Point(3, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(262, 301);
            this.panel4.TabIndex = 0;
            // 
            // ChunkID
            // 
            this.ChunkID.Location = new System.Drawing.Point(3, 265);
            this.ChunkID.Name = "ChunkID";
            this.ChunkID.ReadOnly = true;
            this.ChunkID.Size = new System.Drawing.Size(100, 20);
            this.ChunkID.TabIndex = 2;
            this.ChunkID.Text = "0";
            // 
            // ChunkPicture
            // 
            this.ChunkPicture.Location = new System.Drawing.Point(3, 3);
            this.ChunkPicture.Name = "ChunkPicture";
            this.ChunkPicture.Size = new System.Drawing.Size(256, 256);
            this.ChunkPicture.TabIndex = 1;
            this.ChunkPicture.Paint += new System.Windows.Forms.PaintEventHandler(this.ChunkPicture_Paint);
            this.ChunkPicture.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ChunkPicture_MouseClick);
            // 
            // ChunkBlockPropertyGrid
            // 
            this.ChunkBlockPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChunkBlockPropertyGrid.HelpVisible = false;
            this.ChunkBlockPropertyGrid.Location = new System.Drawing.Point(271, 3);
            this.ChunkBlockPropertyGrid.Name = "ChunkBlockPropertyGrid";
            this.ChunkBlockPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.ChunkBlockPropertyGrid.Size = new System.Drawing.Size(284, 398);
            this.ChunkBlockPropertyGrid.TabIndex = 3;
            this.ChunkBlockPropertyGrid.ToolbarVisible = false;
            this.ChunkBlockPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.ChunkBlockPropertyGrid_PropertyValueChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tableLayoutPanel2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(876, 410);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Blocks";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.BlockSelector, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel5, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel2, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(870, 404);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // BlockSelector
            // 
            this.BlockSelector.BackColor = System.Drawing.SystemColors.Window;
            this.BlockSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BlockSelector.ImageSize = 16;
            this.BlockSelector.Location = new System.Drawing.Point(405, 3);
            this.BlockSelector.Name = "BlockSelector";
            this.BlockSelector.SelectedIndex = -1;
            this.BlockSelector.Size = new System.Drawing.Size(462, 398);
            this.BlockSelector.TabIndex = 2;
            this.BlockSelector.SelectedIndexChanged += new System.EventHandler(this.BlockSelector_SelectedIndexChanged);
            this.BlockSelector.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BlockSelector_MouseDown);
            // 
            // panel5
            // 
            this.panel5.AutoSize = true;
            this.panel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel5.Controls.Add(this.BlockCount);
            this.panel5.Controls.Add(this.BlockID);
            this.panel5.Controls.Add(this.BlockPicture);
            this.panel5.Location = new System.Drawing.Point(3, 3);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(106, 113);
            this.panel5.TabIndex = 0;
            // 
            // BlockID
            // 
            this.BlockID.Location = new System.Drawing.Point(3, 73);
            this.BlockID.Name = "BlockID";
            this.BlockID.ReadOnly = true;
            this.BlockID.Size = new System.Drawing.Size(100, 20);
            this.BlockID.TabIndex = 2;
            this.BlockID.Text = "0";
            // 
            // BlockPicture
            // 
            this.BlockPicture.Location = new System.Drawing.Point(3, 3);
            this.BlockPicture.Name = "BlockPicture";
            this.BlockPicture.Size = new System.Drawing.Size(64, 64);
            this.BlockPicture.TabIndex = 1;
            this.BlockPicture.Paint += new System.Windows.Forms.PaintEventHandler(this.BlockPicture_Paint);
            this.BlockPicture.MouseClick += new System.Windows.Forms.MouseEventHandler(this.BlockPicture_MouseClick);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.BlockTilePropertyGrid);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(115, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(284, 398);
            this.panel2.TabIndex = 3;
            // 
            // BlockTilePropertyGrid
            // 
            this.BlockTilePropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BlockTilePropertyGrid.HelpVisible = false;
            this.BlockTilePropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.BlockTilePropertyGrid.Name = "BlockTilePropertyGrid";
            this.BlockTilePropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.BlockTilePropertyGrid.Size = new System.Drawing.Size(284, 348);
            this.BlockTilePropertyGrid.TabIndex = 3;
            this.BlockTilePropertyGrid.ToolbarVisible = false;
            this.BlockTilePropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.BlockTilePropertyGrid_PropertyValueChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.BlockCollision2);
            this.groupBox1.Controls.Add(this.BlockCollision1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(0, 348);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(284, 50);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Collision Index";
            // 
            // button2
            // 
            this.button2.AutoSize = true;
            this.button2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button2.Location = new System.Drawing.Point(125, 17);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(26, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "...";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.AutoSize = true;
            this.button1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button1.Location = new System.Drawing.Point(48, 17);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(26, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // BlockCollision2
            // 
            this.BlockCollision2.Hexadecimal = true;
            this.BlockCollision2.Location = new System.Drawing.Point(80, 20);
            this.BlockCollision2.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.BlockCollision2.Name = "BlockCollision2";
            this.BlockCollision2.Size = new System.Drawing.Size(39, 20);
            this.BlockCollision2.TabIndex = 1;
            this.BlockCollision2.ValueChanged += new System.EventHandler(this.BlockCollision2_ValueChanged);
            // 
            // BlockCollision1
            // 
            this.BlockCollision1.Hexadecimal = true;
            this.BlockCollision1.Location = new System.Drawing.Point(7, 20);
            this.BlockCollision1.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.BlockCollision1.Name = "BlockCollision1";
            this.BlockCollision1.Size = new System.Drawing.Size(39, 20);
            this.BlockCollision1.TabIndex = 0;
            this.BlockCollision1.ValueChanged += new System.EventHandler(this.BlockCollision1_ValueChanged);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.tableLayoutPanel4);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(876, 410);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Tiles";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.TileSelector, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.panel6, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(870, 404);
            this.tableLayoutPanel4.TabIndex = 1;
            // 
            // TileSelector
            // 
            this.TileSelector.BackColor = System.Drawing.SystemColors.Window;
            this.TileSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TileSelector.ImageSize = 64;
            this.TileSelector.Location = new System.Drawing.Point(143, 3);
            this.TileSelector.Name = "TileSelector";
            this.TileSelector.SelectedIndex = -1;
            this.TileSelector.Size = new System.Drawing.Size(724, 398);
            this.TileSelector.TabIndex = 2;
            this.TileSelector.SelectedIndexChanged += new System.EventHandler(this.TileSelector_SelectedIndexChanged);
            this.TileSelector.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TileSelector_MouseDown);
            // 
            // panel6
            // 
            this.panel6.AutoSize = true;
            this.panel6.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel6.Controls.Add(this.TileCount);
            this.panel6.Controls.Add(this.TileID);
            this.panel6.Controls.Add(this.TilePicture);
            this.panel6.Location = new System.Drawing.Point(3, 3);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(134, 173);
            this.panel6.TabIndex = 0;
            // 
            // TileID
            // 
            this.TileID.Location = new System.Drawing.Point(3, 137);
            this.TileID.Name = "TileID";
            this.TileID.ReadOnly = true;
            this.TileID.Size = new System.Drawing.Size(100, 20);
            this.TileID.TabIndex = 3;
            this.TileID.Text = "0";
            // 
            // TilePicture
            // 
            this.TilePicture.Location = new System.Drawing.Point(3, 3);
            this.TilePicture.Name = "TilePicture";
            this.TilePicture.Size = new System.Drawing.Size(128, 128);
            this.TilePicture.TabIndex = 1;
            this.TilePicture.Paint += new System.Windows.Forms.PaintEventHandler(this.TilePicture_Paint);
            this.TilePicture.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TilePicture_MouseDown);
            this.TilePicture.MouseMove += new System.Windows.Forms.MouseEventHandler(this.TilePicture_MouseMove);
            this.TilePicture.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TilePicture_MouseUp);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.tableLayoutPanel3);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(876, 410);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Solids";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.CollisionSelector, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.panel3, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(870, 404);
            this.tableLayoutPanel3.TabIndex = 2;
            // 
            // CollisionSelector
            // 
            this.CollisionSelector.BackColor = System.Drawing.Color.Black;
            this.CollisionSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CollisionSelector.ImageSize = 16;
            this.CollisionSelector.Location = new System.Drawing.Point(127, 3);
            this.CollisionSelector.Name = "CollisionSelector";
            this.CollisionSelector.SelectedIndex = -1;
            this.CollisionSelector.Size = new System.Drawing.Size(740, 398);
            this.CollisionSelector.TabIndex = 2;
            this.CollisionSelector.SelectedIndexChanged += new System.EventHandler(this.CollisionSelector_SelectedIndexChanged);
            // 
            // panel3
            // 
            this.panel3.AutoSize = true;
            this.panel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel3.Controls.Add(this.ColID);
            this.panel3.Controls.Add(this.ColAngle);
            this.panel3.Controls.Add(this.ColPicture);
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(118, 112);
            this.panel3.TabIndex = 1;
            // 
            // ColID
            // 
            this.ColID.Location = new System.Drawing.Point(3, 89);
            this.ColID.Name = "ColID";
            this.ColID.ReadOnly = true;
            this.ColID.Size = new System.Drawing.Size(100, 20);
            this.ColID.TabIndex = 4;
            this.ColID.Text = "0";
            // 
            // ColAngle
            // 
            this.ColAngle.Hexadecimal = true;
            this.ColAngle.Location = new System.Drawing.Point(74, 4);
            this.ColAngle.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.ColAngle.Name = "ColAngle";
            this.ColAngle.Size = new System.Drawing.Size(41, 20);
            this.ColAngle.TabIndex = 3;
            this.ColAngle.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.ColAngle.ValueChanged += new System.EventHandler(this.ColAngle_ValueChanged);
            // 
            // ColPicture
            // 
            this.ColPicture.BackColor = System.Drawing.Color.Black;
            this.ColPicture.Location = new System.Drawing.Point(3, 3);
            this.ColPicture.Name = "ColPicture";
            this.ColPicture.Size = new System.Drawing.Size(64, 80);
            this.ColPicture.TabIndex = 2;
            this.ColPicture.Paint += new System.Windows.Forms.PaintEventHandler(this.ColPicture_Paint);
            this.ColPicture.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ColPicture_MouseDown);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.PalettePanel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(884, 128);
            this.panel1.TabIndex = 1;
            // 
            // PalettePanel
            // 
            this.PalettePanel.Location = new System.Drawing.Point(0, 0);
            this.PalettePanel.Margin = new System.Windows.Forms.Padding(0);
            this.PalettePanel.Name = "PalettePanel";
            this.PalettePanel.Size = new System.Drawing.Size(512, 128);
            this.PalettePanel.TabIndex = 0;
            this.PalettePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.PalettePanel_Paint);
            this.PalettePanel.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.PalettePanel_MouseDoubleClick);
            this.PalettePanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PalettePanel_MouseDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteBeforeToolStripMenuItem,
            this.pasteAfterToolStripMenuItem,
            this.insertBeforeToolStripMenuItem,
            this.insertAfterToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.importToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(141, 180);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Image = global::SonicRetro.SonLVL.Properties.Resources.cut;
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.cutToolStripMenuItem.Text = "Cu&t";
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Image = global::SonicRetro.SonLVL.Properties.Resources.copy;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.copyToolStripMenuItem.Text = "&Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // pasteBeforeToolStripMenuItem
            // 
            this.pasteBeforeToolStripMenuItem.Image = global::SonicRetro.SonLVL.Properties.Resources.paste;
            this.pasteBeforeToolStripMenuItem.Name = "pasteBeforeToolStripMenuItem";
            this.pasteBeforeToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.pasteBeforeToolStripMenuItem.Text = "Paste &Before";
            this.pasteBeforeToolStripMenuItem.Click += new System.EventHandler(this.pasteBeforeToolStripMenuItem_Click);
            // 
            // pasteAfterToolStripMenuItem
            // 
            this.pasteAfterToolStripMenuItem.Image = global::SonicRetro.SonLVL.Properties.Resources.paste;
            this.pasteAfterToolStripMenuItem.Name = "pasteAfterToolStripMenuItem";
            this.pasteAfterToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.pasteAfterToolStripMenuItem.Text = "Paste &After";
            this.pasteAfterToolStripMenuItem.Click += new System.EventHandler(this.pasteAfterToolStripMenuItem_Click);
            // 
            // insertBeforeToolStripMenuItem
            // 
            this.insertBeforeToolStripMenuItem.Name = "insertBeforeToolStripMenuItem";
            this.insertBeforeToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.insertBeforeToolStripMenuItem.Text = "Insert B&efore";
            this.insertBeforeToolStripMenuItem.Click += new System.EventHandler(this.insertBeforeToolStripMenuItem_Click);
            // 
            // insertAfterToolStripMenuItem
            // 
            this.insertAfterToolStripMenuItem.Name = "insertAfterToolStripMenuItem";
            this.insertAfterToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.insertAfterToolStripMenuItem.Text = "Insert A&fter";
            this.insertAfterToolStripMenuItem.Click += new System.EventHandler(this.insertAfterToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Image = global::SonicRetro.SonLVL.Properties.Resources.delete;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.deleteToolStripMenuItem.Text = "&Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.importToolStripMenuItem.Text = "&Import";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem1});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(111, 26);
            // 
            // importToolStripMenuItem1
            // 
            this.importToolStripMenuItem1.Name = "importToolStripMenuItem1";
            this.importToolStripMenuItem1.Size = new System.Drawing.Size(110, 22);
            this.importToolStripMenuItem1.Text = "&Import";
            this.importToolStripMenuItem1.Click += new System.EventHandler(this.importToolStripMenuItem1_Click);
            // 
            // ChunkCount
            // 
            this.ChunkCount.AutoSize = true;
            this.ChunkCount.Location = new System.Drawing.Point(3, 288);
            this.ChunkCount.Name = "ChunkCount";
            this.ChunkCount.Size = new System.Drawing.Size(42, 13);
            this.ChunkCount.TabIndex = 3;
            this.ChunkCount.Text = "0 / 100";
            // 
            // BlockCount
            // 
            this.BlockCount.AutoSize = true;
            this.BlockCount.Location = new System.Drawing.Point(4, 100);
            this.BlockCount.Name = "BlockCount";
            this.BlockCount.Size = new System.Drawing.Size(42, 13);
            this.BlockCount.TabIndex = 3;
            this.BlockCount.Text = "0 / 400";
            // 
            // TileCount
            // 
            this.TileCount.AutoSize = true;
            this.TileCount.Location = new System.Drawing.Point(3, 160);
            this.TileCount.Name = "TileCount";
            this.TileCount.Size = new System.Drawing.Size(42, 13);
            this.TileCount.TabIndex = 4;
            this.TileCount.Text = "0 / 800";
            // 
            // TileForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 564);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TileForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Tile Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TileForm_FormClosing);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.BlockCollision2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.BlockCollision1)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ColAngle)).EndInit();
            this.panel1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        internal System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        internal SonicRetro.SonLVL.BlockList ChunkSelector;
        private System.Windows.Forms.PropertyGrid ChunkBlockPropertyGrid;
        internal System.Windows.Forms.Panel ChunkPicture;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        internal SonicRetro.SonLVL.BlockList BlockSelector;
        internal System.Windows.Forms.Panel BlockPicture;
        private System.Windows.Forms.PropertyGrid BlockTilePropertyGrid;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown BlockCollision1;
        internal System.Windows.Forms.NumericUpDown BlockCollision2;
        internal System.Windows.Forms.Panel PalettePanel;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        internal SonicRetro.SonLVL.TileList CollisionSelector;
        internal System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteAfterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteBeforeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        internal TileList TileSelector;
        internal System.Windows.Forms.Panel TilePicture;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem insertBeforeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem insertAfterToolStripMenuItem;
        internal System.Windows.Forms.Panel ColPicture;
        private System.Windows.Forms.NumericUpDown ColAngle;
        private System.Windows.Forms.Button button1;
        internal System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TextBox ChunkID;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.TextBox BlockID;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.TextBox TileID;
        private System.Windows.Forms.TextBox ColID;
        private System.Windows.Forms.Label ChunkCount;
        private System.Windows.Forms.Label BlockCount;
        private System.Windows.Forms.Label TileCount;
    }
}