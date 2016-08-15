namespace SonicRetro.SonLVL.GUI
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
			System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
			System.Windows.Forms.ToolStrip chunkListToolStrip;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			System.Windows.Forms.ToolStrip blockListToolStrip;
			System.Windows.Forms.ToolStrip tileListToolStrip;
			System.Windows.Forms.TabControl tabControl2;
			System.Windows.Forms.ToolStrip layoutSectionListToolStrip;
			System.Windows.Forms.ToolStripMenuItem line0ToolStripMenuItem;
			System.Windows.Forms.ToolStripMenuItem line1ToolStripMenuItem;
			System.Windows.Forms.ToolStripMenuItem line2ToolStripMenuItem;
			System.Windows.Forms.ToolStripMenuItem line3ToolStripMenuItem;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
			System.Windows.Forms.ToolStripMenuItem fullToolStripMenuItem;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
			System.Windows.Forms.ToolStripMenuItem line0ToolStripMenuItem1;
			System.Windows.Forms.ToolStripMenuItem line1ToolStripMenuItem1;
			System.Windows.Forms.ToolStripMenuItem line2ToolStripMenuItem1;
			System.Windows.Forms.ToolStripMenuItem line3ToolStripMenuItem1;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
			System.Windows.Forms.ToolStripMenuItem fullToolStripMenuItem1;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Panel panel11;
			System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
			System.Windows.Forms.Panel panel1;
			System.Windows.Forms.Panel panel5;
			System.Windows.Forms.Panel panel9;
			this.importChunksToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.drawChunkToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.deleteUnusedChunksToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.removeDuplicateChunksToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.replaceChunkBlocksToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.remapChunksButton = new System.Windows.Forms.ToolStripButton();
			this.enableDraggingChunksButton = new System.Windows.Forms.ToolStripButton();
			this.importBlocksToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.drawBlockToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.deleteUnusedBlocksToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.removeDuplicateBlocksToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.replaceBlockTilesToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.remapBlocksButton = new System.Windows.Forms.ToolStripButton();
			this.enableDraggingBlocksButton = new System.Windows.Forms.ToolStripButton();
			this.importTilesToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.drawTileToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.deleteUnusedTilesToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.removeDuplicateTilesToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.remapTilesButton = new System.Windows.Forms.ToolStripButton();
			this.enableDraggingTilesButton = new System.Windows.Forms.ToolStripButton();
			this.tabPage8 = new System.Windows.Forms.TabPage();
			this.ChunkSelector = new SonicRetro.SonLVL.API.TileList();
			this.tabPage9 = new System.Windows.Forms.TabPage();
			this.layoutSectionSplitContainer = new System.Windows.Forms.SplitContainer();
			this.layoutSectionListBox = new System.Windows.Forms.ListBox();
			this.importToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.layoutSectionPreview = new System.Windows.Forms.PictureBox();
			this.flipTileHButton = new System.Windows.Forms.Button();
			this.TilePicture = new System.Windows.Forms.Panel();
			this.flipTileVButton = new System.Windows.Forms.Button();
			this.TileID = new System.Windows.Forms.TextBox();
			this.rotateTileRightButton = new System.Windows.Forms.Button();
			this.TileCount = new System.Windows.Forms.Label();
			this.ChunkPicture = new SonicRetro.SonLVL.API.KeyboardPanel();
			this.flipChunkVButton = new System.Windows.Forms.Button();
			this.chunkBlockEditor = new SonicRetro.SonLVL.ChunkBlockEditor();
			this.flipChunkHButton = new System.Windows.Forms.Button();
			this.ChunkCount = new System.Windows.Forms.Label();
			this.chunkCtrlLabel = new System.Windows.Forms.Label();
			this.ChunkID = new System.Windows.Forms.TextBox();
			this.blockTileEditor = new SonicRetro.SonLVL.PatternIndexEditor();
			this.flipBlockVButton = new System.Windows.Forms.Button();
			this.flipBlockHButton = new System.Windows.Forms.Button();
			this.BlockCount = new System.Windows.Forms.Label();
			this.BlockID = new System.Windows.Forms.TextBox();
			this.BlockPicture = new SonicRetro.SonLVL.API.KeyboardPanel();
			this.ColIndBox = new System.Windows.Forms.GroupBox();
			this.button2 = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.BlockCollision2 = new System.Windows.Forms.NumericUpDown();
			this.BlockCollision1 = new System.Windows.Forms.NumericUpDown();
			this.calculateAngleButton = new System.Windows.Forms.Button();
			this.showBlockBehindCollisionCheckBox = new System.Windows.Forms.CheckBox();
			this.ColID = new System.Windows.Forms.TextBox();
			this.ColAngle = new System.Windows.Forms.NumericUpDown();
			this.ColPicture = new System.Windows.Forms.Panel();
			this.TileSelector = new SonicRetro.SonLVL.API.TileList();
			this.BlockSelector = new SonicRetro.SonLVL.API.TileList();
			this.panel10 = new System.Windows.Forms.Panel();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.changeLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.noneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.buildAndRunToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.setupEmulatorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.recentProjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.noneToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.undoCtrlZToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.redoCtrlYToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.findNextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.findPreviousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.blendAlternatePaletteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.resizeLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.includeObjectsWithForegroundSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.switchMouseButtonsInChunkAndBlockEditorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.objectsAboveHighPlaneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.hUDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.paletteToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.waterPaletteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.selectPaletteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.setPositionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.layersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.lowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.highToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.collisionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.noneToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.path1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.path2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.anglesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.timeZoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.currentOnlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.allToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.gridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.enableGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.gridColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.zoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.xToolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
			this.xToolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
			this.xToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.xToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.xToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.xToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
			this.xToolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
			this.xToolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.usageCountsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.paletteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pNGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.yYCHRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.jASCPALToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.paletteLine0ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.paletteLine1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.paletteLine2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.paletteLine3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.blocksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.chunksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.solidityMapsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.foregroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.backgroundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.transparentBackFGBGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.includeobjectsWithFGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.hideDebugObjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.useHexadecimalIndexesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportArtcollisionpriorityToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewReadmeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.reportBugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.backgroundLevelLoader = new System.ComponentModel.BackgroundWorker();
			this.objectContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.addObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addRingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addGroupOfObjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addGroupOfRingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.selectAllObjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.selectAllRingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
			this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.objectPanel = new SonicRetro.SonLVL.API.KeyboardPanel();
			this.ObjectProperties = new System.Windows.Forms.PropertyGrid();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer4 = new System.Windows.Forms.SplitContainer();
			this.objectTypeList = new System.Windows.Forms.ListView();
			this.objectTypeImages = new System.Windows.Forms.ImageList(this.components);
			this.objToolStrip = new System.Windows.Forms.ToolStrip();
			this.objGridSizeDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripMenuItem();
			this.alignLeftWallToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.alignGroundToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.alignRightWallToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.alignCeilingToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.alignLeftsToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.alignCentersToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.alignRightsToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.alignTopsToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.alignMiddlesToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.alignBottomsToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.foregroundPanel = new SonicRetro.SonLVL.API.KeyboardPanel();
			this.vScrollBar2 = new System.Windows.Forms.VScrollBar();
			this.hScrollBar2 = new System.Windows.Forms.HScrollBar();
			this.fgToolStrip = new System.Windows.Forms.ToolStrip();
			this.fgDrawToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.fgSelectToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.replaceForegroundToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.clearForegroundToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this.bgToolStrip = new System.Windows.Forms.ToolStrip();
			this.bgDrawToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.bgSelectToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.replaceBackgroundToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.clearBackgroundToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.backgroundPanel = new SonicRetro.SonLVL.API.KeyboardPanel();
			this.vScrollBar3 = new System.Windows.Forms.VScrollBar();
			this.hScrollBar3 = new System.Windows.Forms.HScrollBar();
			this.tabControl3 = new System.Windows.Forms.TabControl();
			this.tabPage10 = new System.Windows.Forms.TabPage();
			this.tabPage11 = new System.Windows.Forms.TabPage();
			this.tabPage4 = new System.Windows.Forms.TabPage();
			this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
			this.tabControl4 = new System.Windows.Forms.TabControl();
			this.tabPage12 = new System.Windows.Forms.TabPage();
			this.tabPage13 = new System.Windows.Forms.TabPage();
			this.tabPage14 = new System.Windows.Forms.TabPage();
			this.tabPage5 = new System.Windows.Forms.TabPage();
			this.CollisionSelector = new SonicRetro.SonLVL.API.TileList();
			this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
			this.panel8 = new System.Windows.Forms.Panel();
			this.PalettePanel = new System.Windows.Forms.Panel();
			this.colorEditingPanel = new System.Windows.Forms.Panel();
			this.colorBlue = new System.Windows.Forms.NumericUpDown();
			this.colorGreen = new System.Windows.Forms.NumericUpDown();
			this.colorRed = new System.Windows.Forms.NumericUpDown();
			this.tileContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.cutTilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyTilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deepCopyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteBeforeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteOverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteAfterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.importOverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.duplicateTilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.insertBeforeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.insertAfterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteTilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.paletteContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.importToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.layoutContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.cutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.copyToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteOnceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteRepeatingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.importToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
			this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.fillToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
			this.saveSectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteSectionOnceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteSectionRepeatingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
			this.insertLayoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteLayoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.solidsContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.copySolidsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteSolidsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.clearSolidsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadingAnimation1 = new SonicRetro.SonLVL.LoadingAnimation();
			this.importProgressControl1 = new SonicRetro.SonLVL.ImportProgressControl();
			toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
			label4 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
			chunkListToolStrip = new System.Windows.Forms.ToolStrip();
			blockListToolStrip = new System.Windows.Forms.ToolStrip();
			tileListToolStrip = new System.Windows.Forms.ToolStrip();
			tabControl2 = new System.Windows.Forms.TabControl();
			layoutSectionListToolStrip = new System.Windows.Forms.ToolStrip();
			line0ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			line1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			line2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			line3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			fullToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			line0ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			line1ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			line2ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			line3ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
			fullToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
			label1 = new System.Windows.Forms.Label();
			panel11 = new System.Windows.Forms.Panel();
			tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
			panel1 = new System.Windows.Forms.Panel();
			panel5 = new System.Windows.Forms.Panel();
			panel9 = new System.Windows.Forms.Panel();
			chunkListToolStrip.SuspendLayout();
			blockListToolStrip.SuspendLayout();
			tileListToolStrip.SuspendLayout();
			tabControl2.SuspendLayout();
			this.tabPage8.SuspendLayout();
			this.tabPage9.SuspendLayout();
			this.layoutSectionSplitContainer.Panel1.SuspendLayout();
			this.layoutSectionSplitContainer.Panel2.SuspendLayout();
			this.layoutSectionSplitContainer.SuspendLayout();
			layoutSectionListToolStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutSectionPreview)).BeginInit();
			panel11.SuspendLayout();
			tableLayoutPanel5.SuspendLayout();
			panel1.SuspendLayout();
			panel5.SuspendLayout();
			this.ColIndBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.BlockCollision2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.BlockCollision1)).BeginInit();
			panel9.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ColAngle)).BeginInit();
			this.menuStrip1.SuspendLayout();
			this.objectContextMenuStrip.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.splitContainer4.Panel1.SuspendLayout();
			this.splitContainer4.Panel2.SuspendLayout();
			this.splitContainer4.SuspendLayout();
			this.objToolStrip.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.fgToolStrip.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			this.bgToolStrip.SuspendLayout();
			this.tabControl3.SuspendLayout();
			this.tabPage4.SuspendLayout();
			this.tableLayoutPanel4.SuspendLayout();
			this.tabControl4.SuspendLayout();
			this.tabPage12.SuspendLayout();
			this.tabPage13.SuspendLayout();
			this.tabPage14.SuspendLayout();
			this.tabPage5.SuspendLayout();
			this.tableLayoutPanel8.SuspendLayout();
			this.panel8.SuspendLayout();
			this.colorEditingPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.colorBlue)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.colorGreen)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.colorRed)).BeginInit();
			this.tileContextMenuStrip.SuspendLayout();
			this.paletteContextMenuStrip.SuspendLayout();
			this.layoutContextMenuStrip.SuspendLayout();
			this.solidsContextMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripSeparator8
			// 
			toolStripSeparator8.Name = "toolStripSeparator8";
			toolStripSeparator8.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripSeparator9
			// 
			toolStripSeparator9.Name = "toolStripSeparator9";
			toolStripSeparator9.Size = new System.Drawing.Size(6, 25);
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(5, 57);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(31, 13);
			label4.TabIndex = 4;
			label4.Text = "Blue:";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(5, 31);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(39, 13);
			label3.TabIndex = 2;
			label3.Text = "Green:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(5, 5);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(30, 13);
			label2.TabIndex = 0;
			label2.Text = "Red:";
			// 
			// toolStripSeparator10
			// 
			toolStripSeparator10.Name = "toolStripSeparator10";
			toolStripSeparator10.Size = new System.Drawing.Size(6, 25);
			// 
			// chunkListToolStrip
			// 
			chunkListToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			chunkListToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importChunksToolStripButton,
            this.drawChunkToolStripButton,
            this.deleteUnusedChunksToolStripButton,
            this.removeDuplicateChunksToolStripButton,
            this.replaceChunkBlocksToolStripButton,
            this.remapChunksButton,
            this.enableDraggingChunksButton});
			chunkListToolStrip.Location = new System.Drawing.Point(3, 3);
			chunkListToolStrip.Name = "chunkListToolStrip";
			chunkListToolStrip.Size = new System.Drawing.Size(286, 25);
			chunkListToolStrip.TabIndex = 0;
			chunkListToolStrip.Text = "toolStrip1";
			// 
			// importChunksToolStripButton
			// 
			this.importChunksToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.importChunksToolStripButton.Enabled = false;
			this.importChunksToolStripButton.Name = "importChunksToolStripButton";
			this.importChunksToolStripButton.Size = new System.Drawing.Size(56, 22);
			this.importChunksToolStripButton.Text = "Import...";
			this.importChunksToolStripButton.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
			// 
			// drawChunkToolStripButton
			// 
			this.drawChunkToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.drawChunkToolStripButton.Enabled = false;
			this.drawChunkToolStripButton.Name = "drawChunkToolStripButton";
			this.drawChunkToolStripButton.Size = new System.Drawing.Size(47, 22);
			this.drawChunkToolStripButton.Text = "Draw...";
			this.drawChunkToolStripButton.Click += new System.EventHandler(this.drawToolStripButton_Click);
			// 
			// deleteUnusedChunksToolStripButton
			// 
			this.deleteUnusedChunksToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.deleteUnusedChunksToolStripButton.Enabled = false;
			this.deleteUnusedChunksToolStripButton.Name = "deleteUnusedChunksToolStripButton";
			this.deleteUnusedChunksToolStripButton.Size = new System.Drawing.Size(87, 22);
			this.deleteUnusedChunksToolStripButton.Text = "Delete Unused";
			this.deleteUnusedChunksToolStripButton.Click += new System.EventHandler(this.deleteUnusedChunksToolStripButton_Click);
			// 
			// removeDuplicateChunksToolStripButton
			// 
			this.removeDuplicateChunksToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.removeDuplicateChunksToolStripButton.Enabled = false;
			this.removeDuplicateChunksToolStripButton.Name = "removeDuplicateChunksToolStripButton";
			this.removeDuplicateChunksToolStripButton.Size = new System.Drawing.Size(112, 19);
			this.removeDuplicateChunksToolStripButton.Text = "Remove Duplicates";
			this.removeDuplicateChunksToolStripButton.Click += new System.EventHandler(this.removeDuplicateChunksToolStripButton_Click);
			// 
			// replaceChunkBlocksToolStripButton
			// 
			this.replaceChunkBlocksToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.replaceChunkBlocksToolStripButton.Enabled = false;
			this.replaceChunkBlocksToolStripButton.Name = "replaceChunkBlocksToolStripButton";
			this.replaceChunkBlocksToolStripButton.Size = new System.Drawing.Size(52, 19);
			this.replaceChunkBlocksToolStripButton.Text = "Replace";
			this.replaceChunkBlocksToolStripButton.Click += new System.EventHandler(this.replaceChunkBlocksToolStripButton_Click);
			// 
			// remapChunksButton
			// 
			this.remapChunksButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.remapChunksButton.Enabled = false;
			this.remapChunksButton.Image = ((System.Drawing.Image)(resources.GetObject("remapChunksButton.Image")));
			this.remapChunksButton.Name = "remapChunksButton";
			this.remapChunksButton.Size = new System.Drawing.Size(137, 19);
			this.remapChunksButton.Text = "Advanced Remapping...";
			this.remapChunksButton.Click += new System.EventHandler(this.remapChunksButton_Click);
			// 
			// enableDraggingChunksButton
			// 
			this.enableDraggingChunksButton.Checked = true;
			this.enableDraggingChunksButton.CheckOnClick = true;
			this.enableDraggingChunksButton.CheckState = System.Windows.Forms.CheckState.Checked;
			this.enableDraggingChunksButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.enableDraggingChunksButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.enableDraggingChunksButton.Name = "enableDraggingChunksButton";
			this.enableDraggingChunksButton.Size = new System.Drawing.Size(98, 19);
			this.enableDraggingChunksButton.Text = "Enable Dragging";
			// 
			// blockListToolStrip
			// 
			blockListToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			blockListToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importBlocksToolStripButton,
            this.drawBlockToolStripButton,
            this.deleteUnusedBlocksToolStripButton,
            this.removeDuplicateBlocksToolStripButton,
            this.replaceBlockTilesToolStripButton,
            this.remapBlocksButton,
            this.enableDraggingBlocksButton});
			blockListToolStrip.Location = new System.Drawing.Point(3, 3);
			blockListToolStrip.Name = "blockListToolStrip";
			blockListToolStrip.Size = new System.Drawing.Size(286, 25);
			blockListToolStrip.TabIndex = 1;
			blockListToolStrip.Text = "toolStrip2";
			// 
			// importBlocksToolStripButton
			// 
			this.importBlocksToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.importBlocksToolStripButton.Enabled = false;
			this.importBlocksToolStripButton.Name = "importBlocksToolStripButton";
			this.importBlocksToolStripButton.Size = new System.Drawing.Size(56, 22);
			this.importBlocksToolStripButton.Text = "Import...";
			this.importBlocksToolStripButton.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
			// 
			// drawBlockToolStripButton
			// 
			this.drawBlockToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.drawBlockToolStripButton.Enabled = false;
			this.drawBlockToolStripButton.Name = "drawBlockToolStripButton";
			this.drawBlockToolStripButton.Size = new System.Drawing.Size(47, 22);
			this.drawBlockToolStripButton.Text = "Draw...";
			this.drawBlockToolStripButton.Click += new System.EventHandler(this.drawToolStripButton_Click);
			// 
			// deleteUnusedBlocksToolStripButton
			// 
			this.deleteUnusedBlocksToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.deleteUnusedBlocksToolStripButton.Enabled = false;
			this.deleteUnusedBlocksToolStripButton.Name = "deleteUnusedBlocksToolStripButton";
			this.deleteUnusedBlocksToolStripButton.Size = new System.Drawing.Size(87, 22);
			this.deleteUnusedBlocksToolStripButton.Text = "Delete Unused";
			this.deleteUnusedBlocksToolStripButton.Click += new System.EventHandler(this.deleteUnusedBlocksToolStripButton_Click);
			// 
			// removeDuplicateBlocksToolStripButton
			// 
			this.removeDuplicateBlocksToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.removeDuplicateBlocksToolStripButton.Enabled = false;
			this.removeDuplicateBlocksToolStripButton.Name = "removeDuplicateBlocksToolStripButton";
			this.removeDuplicateBlocksToolStripButton.Size = new System.Drawing.Size(112, 19);
			this.removeDuplicateBlocksToolStripButton.Text = "Remove Duplicates";
			this.removeDuplicateBlocksToolStripButton.Click += new System.EventHandler(this.removeDuplicateBlocksToolStripButton_Click);
			// 
			// replaceBlockTilesToolStripButton
			// 
			this.replaceBlockTilesToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.replaceBlockTilesToolStripButton.Enabled = false;
			this.replaceBlockTilesToolStripButton.Name = "replaceBlockTilesToolStripButton";
			this.replaceBlockTilesToolStripButton.Size = new System.Drawing.Size(52, 19);
			this.replaceBlockTilesToolStripButton.Text = "Replace";
			this.replaceBlockTilesToolStripButton.Click += new System.EventHandler(this.replaceBlockTilesToolStripButton_Click);
			// 
			// remapBlocksButton
			// 
			this.remapBlocksButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.remapBlocksButton.Enabled = false;
			this.remapBlocksButton.Image = ((System.Drawing.Image)(resources.GetObject("remapBlocksButton.Image")));
			this.remapBlocksButton.Name = "remapBlocksButton";
			this.remapBlocksButton.Size = new System.Drawing.Size(137, 19);
			this.remapBlocksButton.Text = "Advanced Remapping...";
			this.remapBlocksButton.Click += new System.EventHandler(this.remapBlocksButton_Click);
			// 
			// enableDraggingBlocksButton
			// 
			this.enableDraggingBlocksButton.Checked = true;
			this.enableDraggingBlocksButton.CheckOnClick = true;
			this.enableDraggingBlocksButton.CheckState = System.Windows.Forms.CheckState.Checked;
			this.enableDraggingBlocksButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.enableDraggingBlocksButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.enableDraggingBlocksButton.Name = "enableDraggingBlocksButton";
			this.enableDraggingBlocksButton.Size = new System.Drawing.Size(98, 19);
			this.enableDraggingBlocksButton.Text = "Enable Dragging";
			// 
			// tileListToolStrip
			// 
			tileListToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			tileListToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importTilesToolStripButton,
            this.drawTileToolStripButton,
            this.deleteUnusedTilesToolStripButton,
            this.removeDuplicateTilesToolStripButton,
            this.remapTilesButton,
            this.enableDraggingTilesButton});
			tileListToolStrip.Location = new System.Drawing.Point(3, 3);
			tileListToolStrip.Name = "tileListToolStrip";
			tileListToolStrip.Size = new System.Drawing.Size(286, 25);
			tileListToolStrip.TabIndex = 2;
			tileListToolStrip.Text = "toolStrip3";
			// 
			// importTilesToolStripButton
			// 
			this.importTilesToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.importTilesToolStripButton.Enabled = false;
			this.importTilesToolStripButton.Name = "importTilesToolStripButton";
			this.importTilesToolStripButton.Size = new System.Drawing.Size(56, 22);
			this.importTilesToolStripButton.Text = "Import...";
			this.importTilesToolStripButton.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
			// 
			// drawTileToolStripButton
			// 
			this.drawTileToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.drawTileToolStripButton.Enabled = false;
			this.drawTileToolStripButton.Name = "drawTileToolStripButton";
			this.drawTileToolStripButton.Size = new System.Drawing.Size(47, 22);
			this.drawTileToolStripButton.Text = "Draw...";
			this.drawTileToolStripButton.Click += new System.EventHandler(this.drawToolStripButton_Click);
			// 
			// deleteUnusedTilesToolStripButton
			// 
			this.deleteUnusedTilesToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.deleteUnusedTilesToolStripButton.Enabled = false;
			this.deleteUnusedTilesToolStripButton.Name = "deleteUnusedTilesToolStripButton";
			this.deleteUnusedTilesToolStripButton.Size = new System.Drawing.Size(87, 22);
			this.deleteUnusedTilesToolStripButton.Text = "Delete Unused";
			this.deleteUnusedTilesToolStripButton.Click += new System.EventHandler(this.deleteUnusedTilesToolStripButton_Click);
			// 
			// removeDuplicateTilesToolStripButton
			// 
			this.removeDuplicateTilesToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.removeDuplicateTilesToolStripButton.Enabled = false;
			this.removeDuplicateTilesToolStripButton.Name = "removeDuplicateTilesToolStripButton";
			this.removeDuplicateTilesToolStripButton.Size = new System.Drawing.Size(112, 19);
			this.removeDuplicateTilesToolStripButton.Text = "Remove Duplicates";
			this.removeDuplicateTilesToolStripButton.Click += new System.EventHandler(this.removeDuplicateTilesToolStripButton_Click);
			// 
			// remapTilesButton
			// 
			this.remapTilesButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.remapTilesButton.Enabled = false;
			this.remapTilesButton.Image = ((System.Drawing.Image)(resources.GetObject("remapTilesButton.Image")));
			this.remapTilesButton.Name = "remapTilesButton";
			this.remapTilesButton.Size = new System.Drawing.Size(137, 19);
			this.remapTilesButton.Text = "Advanced Remapping...";
			this.remapTilesButton.Click += new System.EventHandler(this.remapTilesButton_Click);
			// 
			// enableDraggingTilesButton
			// 
			this.enableDraggingTilesButton.Checked = true;
			this.enableDraggingTilesButton.CheckOnClick = true;
			this.enableDraggingTilesButton.CheckState = System.Windows.Forms.CheckState.Checked;
			this.enableDraggingTilesButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.enableDraggingTilesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.enableDraggingTilesButton.Name = "enableDraggingTilesButton";
			this.enableDraggingTilesButton.Size = new System.Drawing.Size(98, 19);
			this.enableDraggingTilesButton.Text = "Enable Dragging";
			// 
			// tabControl2
			// 
			tabControl2.Controls.Add(this.tabPage8);
			tabControl2.Controls.Add(this.tabPage9);
			tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
			tabControl2.Location = new System.Drawing.Point(0, 0);
			tabControl2.Name = "tabControl2";
			tabControl2.SelectedIndex = 0;
			tabControl2.Size = new System.Drawing.Size(262, 397);
			tabControl2.TabIndex = 2;
			// 
			// tabPage8
			// 
			this.tabPage8.Controls.Add(this.ChunkSelector);
			this.tabPage8.Location = new System.Drawing.Point(4, 22);
			this.tabPage8.Name = "tabPage8";
			this.tabPage8.Size = new System.Drawing.Size(254, 371);
			this.tabPage8.TabIndex = 0;
			this.tabPage8.Text = "Chunks";
			this.tabPage8.UseVisualStyleBackColor = true;
			// 
			// ChunkSelector
			// 
			this.ChunkSelector.BackColor = System.Drawing.SystemColors.Window;
			this.ChunkSelector.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ChunkSelector.ImageHeight = 128;
			this.ChunkSelector.ImageSize = 128;
			this.ChunkSelector.ImageWidth = 128;
			this.ChunkSelector.Location = new System.Drawing.Point(0, 0);
			this.ChunkSelector.Margin = new System.Windows.Forms.Padding(0);
			this.ChunkSelector.Name = "ChunkSelector";
			this.ChunkSelector.ScrollValue = 0;
			this.ChunkSelector.SelectedIndex = -1;
			this.ChunkSelector.Size = new System.Drawing.Size(254, 371);
			this.ChunkSelector.TabIndex = 1;
			this.ChunkSelector.SelectedIndexChanged += new System.EventHandler(this.ChunkSelector_SelectedIndexChanged);
			this.ChunkSelector.ItemDrag += new System.EventHandler(this.ChunkSelector_ItemDrag);
			this.ChunkSelector.DragDrop += new System.Windows.Forms.DragEventHandler(this.ChunkSelector_DragDrop);
			this.ChunkSelector.DragEnter += new System.Windows.Forms.DragEventHandler(this.ChunkSelector_DragEnter);
			this.ChunkSelector.DragOver += new System.Windows.Forms.DragEventHandler(this.ChunkSelector_DragOver);
			this.ChunkSelector.DragLeave += new System.EventHandler(this.ChunkSelector_DragLeave);
			this.ChunkSelector.Paint += new System.Windows.Forms.PaintEventHandler(this.ChunkSelector_Paint);
			this.ChunkSelector.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TileList_KeyDown);
			this.ChunkSelector.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChunkSelector_MouseDown);
			// 
			// tabPage9
			// 
			this.tabPage9.Controls.Add(this.layoutSectionSplitContainer);
			this.tabPage9.Location = new System.Drawing.Point(4, 22);
			this.tabPage9.Name = "tabPage9";
			this.tabPage9.Size = new System.Drawing.Size(254, 371);
			this.tabPage9.TabIndex = 1;
			this.tabPage9.Text = "Layout Sections";
			this.tabPage9.UseVisualStyleBackColor = true;
			// 
			// layoutSectionSplitContainer
			// 
			this.layoutSectionSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutSectionSplitContainer.Location = new System.Drawing.Point(0, 0);
			this.layoutSectionSplitContainer.Margin = new System.Windows.Forms.Padding(0);
			this.layoutSectionSplitContainer.Name = "layoutSectionSplitContainer";
			this.layoutSectionSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// layoutSectionSplitContainer.Panel1
			// 
			this.layoutSectionSplitContainer.Panel1.Controls.Add(this.layoutSectionListBox);
			this.layoutSectionSplitContainer.Panel1.Controls.Add(layoutSectionListToolStrip);
			// 
			// layoutSectionSplitContainer.Panel2
			// 
			this.layoutSectionSplitContainer.Panel2.Controls.Add(this.layoutSectionPreview);
			this.layoutSectionSplitContainer.Size = new System.Drawing.Size(254, 371);
			this.layoutSectionSplitContainer.SplitterDistance = 168;
			this.layoutSectionSplitContainer.TabIndex = 0;
			// 
			// layoutSectionListBox
			// 
			this.layoutSectionListBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutSectionListBox.FormattingEnabled = true;
			this.layoutSectionListBox.IntegralHeight = false;
			this.layoutSectionListBox.Location = new System.Drawing.Point(0, 25);
			this.layoutSectionListBox.Name = "layoutSectionListBox";
			this.layoutSectionListBox.Size = new System.Drawing.Size(254, 143);
			this.layoutSectionListBox.TabIndex = 0;
			this.layoutSectionListBox.SelectedIndexChanged += new System.EventHandler(this.layoutSectionListBox_SelectedIndexChanged);
			this.layoutSectionListBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.layoutSectionListBox_KeyDown);
			// 
			// layoutSectionListToolStrip
			// 
			layoutSectionListToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			layoutSectionListToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripButton});
			layoutSectionListToolStrip.Location = new System.Drawing.Point(0, 0);
			layoutSectionListToolStrip.Name = "layoutSectionListToolStrip";
			layoutSectionListToolStrip.Size = new System.Drawing.Size(254, 25);
			layoutSectionListToolStrip.TabIndex = 1;
			layoutSectionListToolStrip.Text = "toolStrip1";
			// 
			// importToolStripButton
			// 
			this.importToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.importToolStripButton.Name = "importToolStripButton";
			this.importToolStripButton.Size = new System.Drawing.Size(56, 22);
			this.importToolStripButton.Text = "I&mport...";
			this.importToolStripButton.Click += new System.EventHandler(this.importToolStripButton_Click);
			// 
			// layoutSectionPreview
			// 
			this.layoutSectionPreview.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutSectionPreview.Location = new System.Drawing.Point(0, 0);
			this.layoutSectionPreview.Margin = new System.Windows.Forms.Padding(0);
			this.layoutSectionPreview.Name = "layoutSectionPreview";
			this.layoutSectionPreview.Size = new System.Drawing.Size(254, 199);
			this.layoutSectionPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.layoutSectionPreview.TabIndex = 0;
			this.layoutSectionPreview.TabStop = false;
			// 
			// line0ToolStripMenuItem
			// 
			line0ToolStripMenuItem.Name = "line0ToolStripMenuItem";
			line0ToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
			line0ToolStripMenuItem.Text = "Line &0";
			// 
			// line1ToolStripMenuItem
			// 
			line1ToolStripMenuItem.Name = "line1ToolStripMenuItem";
			line1ToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
			line1ToolStripMenuItem.Text = "Line &1";
			// 
			// line2ToolStripMenuItem
			// 
			line2ToolStripMenuItem.Name = "line2ToolStripMenuItem";
			line2ToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
			line2ToolStripMenuItem.Text = "Line &2";
			// 
			// line3ToolStripMenuItem
			// 
			line3ToolStripMenuItem.Name = "line3ToolStripMenuItem";
			line3ToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
			line3ToolStripMenuItem.Text = "Line &3";
			// 
			// toolStripSeparator2
			// 
			toolStripSeparator2.Name = "toolStripSeparator2";
			toolStripSeparator2.Size = new System.Drawing.Size(102, 6);
			// 
			// fullToolStripMenuItem
			// 
			fullToolStripMenuItem.Name = "fullToolStripMenuItem";
			fullToolStripMenuItem.Size = new System.Drawing.Size(105, 22);
			fullToolStripMenuItem.Text = "&Full";
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new System.Drawing.Size(219, 6);
			// 
			// line0ToolStripMenuItem1
			// 
			line0ToolStripMenuItem1.Name = "line0ToolStripMenuItem1";
			line0ToolStripMenuItem1.Size = new System.Drawing.Size(105, 22);
			line0ToolStripMenuItem1.Text = "Line &0";
			// 
			// line1ToolStripMenuItem1
			// 
			line1ToolStripMenuItem1.Name = "line1ToolStripMenuItem1";
			line1ToolStripMenuItem1.Size = new System.Drawing.Size(105, 22);
			line1ToolStripMenuItem1.Text = "Line &1";
			// 
			// line2ToolStripMenuItem1
			// 
			line2ToolStripMenuItem1.Name = "line2ToolStripMenuItem1";
			line2ToolStripMenuItem1.Size = new System.Drawing.Size(105, 22);
			line2ToolStripMenuItem1.Text = "Line &2";
			// 
			// line3ToolStripMenuItem1
			// 
			line3ToolStripMenuItem1.Name = "line3ToolStripMenuItem1";
			line3ToolStripMenuItem1.Size = new System.Drawing.Size(105, 22);
			line3ToolStripMenuItem1.Text = "Line &3";
			// 
			// toolStripSeparator13
			// 
			toolStripSeparator13.Name = "toolStripSeparator13";
			toolStripSeparator13.Size = new System.Drawing.Size(102, 6);
			// 
			// fullToolStripMenuItem1
			// 
			fullToolStripMenuItem1.Name = "fullToolStripMenuItem1";
			fullToolStripMenuItem1.Size = new System.Drawing.Size(105, 22);
			fullToolStripMenuItem1.Text = "&Full";
			// 
			// toolStripSeparator14
			// 
			toolStripSeparator14.Name = "toolStripSeparator14";
			toolStripSeparator14.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripSeparator15
			// 
			toolStripSeparator15.Name = "toolStripSeparator15";
			toolStripSeparator15.Size = new System.Drawing.Size(6, 25);
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(3, 147);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(37, 13);
			label1.TabIndex = 6;
			label1.Text = "Angle:";
			// 
			// panel11
			// 
			panel11.AutoSize = true;
			panel11.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			panel11.Controls.Add(this.flipTileHButton);
			panel11.Controls.Add(this.TilePicture);
			panel11.Controls.Add(this.flipTileVButton);
			panel11.Controls.Add(this.TileID);
			panel11.Controls.Add(this.rotateTileRightButton);
			panel11.Controls.Add(this.TileCount);
			panel11.Location = new System.Drawing.Point(460, 0);
			panel11.Margin = new System.Windows.Forms.Padding(0);
			panel11.Name = "panel11";
			panel11.Size = new System.Drawing.Size(180, 215);
			panel11.TabIndex = 11;
			// 
			// flipTileHButton
			// 
			this.flipTileHButton.AutoSize = true;
			this.flipTileHButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flipTileHButton.Enabled = false;
			this.flipTileHButton.Location = new System.Drawing.Point(3, 134);
			this.flipTileHButton.Name = "flipTileHButton";
			this.flipTileHButton.Size = new System.Drawing.Size(90, 23);
			this.flipTileHButton.TabIndex = 9;
			this.flipTileHButton.Text = "Flip Horizontally";
			this.flipTileHButton.UseVisualStyleBackColor = true;
			this.flipTileHButton.Click += new System.EventHandler(this.flipTileHButton_Click);
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
			// flipTileVButton
			// 
			this.flipTileVButton.AutoSize = true;
			this.flipTileVButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flipTileVButton.Enabled = false;
			this.flipTileVButton.Location = new System.Drawing.Point(99, 134);
			this.flipTileVButton.Name = "flipTileVButton";
			this.flipTileVButton.Size = new System.Drawing.Size(78, 23);
			this.flipTileVButton.TabIndex = 10;
			this.flipTileVButton.Text = "Flip Vertically";
			this.flipTileVButton.UseVisualStyleBackColor = true;
			this.flipTileVButton.Click += new System.EventHandler(this.flipTileVButton_Click);
			// 
			// TileID
			// 
			this.TileID.Location = new System.Drawing.Point(3, 192);
			this.TileID.Name = "TileID";
			this.TileID.ReadOnly = true;
			this.TileID.Size = new System.Drawing.Size(100, 20);
			this.TileID.TabIndex = 3;
			this.TileID.Text = "0";
			// 
			// rotateTileRightButton
			// 
			this.rotateTileRightButton.AutoSize = true;
			this.rotateTileRightButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.rotateTileRightButton.Enabled = false;
			this.rotateTileRightButton.Location = new System.Drawing.Point(3, 163);
			this.rotateTileRightButton.Name = "rotateTileRightButton";
			this.rotateTileRightButton.Size = new System.Drawing.Size(77, 23);
			this.rotateTileRightButton.TabIndex = 5;
			this.rotateTileRightButton.Text = "Rotate Right";
			this.rotateTileRightButton.UseVisualStyleBackColor = true;
			this.rotateTileRightButton.Click += new System.EventHandler(this.rotateTileRightButton_Click);
			// 
			// TileCount
			// 
			this.TileCount.AutoSize = true;
			this.TileCount.Location = new System.Drawing.Point(109, 195);
			this.TileCount.Name = "TileCount";
			this.TileCount.Size = new System.Drawing.Size(42, 13);
			this.TileCount.TabIndex = 4;
			this.TileCount.Text = "0 / 800";
			// 
			// tableLayoutPanel5
			// 
			tableLayoutPanel5.AutoSize = true;
			tableLayoutPanel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			tableLayoutPanel5.ColumnCount = 1;
			tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel5.Controls.Add(panel1, 0, 1);
			tableLayoutPanel5.Controls.Add(this.ChunkPicture, 0, 0);
			tableLayoutPanel5.Location = new System.Drawing.Point(0, 0);
			tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
			tableLayoutPanel5.Name = "tableLayoutPanel5";
			tableLayoutPanel5.RowCount = 2;
			this.tableLayoutPanel4.SetRowSpan(tableLayoutPanel5, 2);
			tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel5.Size = new System.Drawing.Size(262, 487);
			tableLayoutPanel5.TabIndex = 9;
			// 
			// ChunkPicture
			// 
			this.ChunkPicture.Location = new System.Drawing.Point(3, 3);
			this.ChunkPicture.Name = "ChunkPicture";
			this.ChunkPicture.Size = new System.Drawing.Size(256, 256);
			this.ChunkPicture.TabIndex = 1;
			this.ChunkPicture.Paint += new System.Windows.Forms.PaintEventHandler(this.ChunkPicture_Paint);
			this.ChunkPicture.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ChunkPicture_KeyDown);
			this.ChunkPicture.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChunkPicture_MouseDown);
			this.ChunkPicture.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ChunkPicture_MouseMove);
			this.ChunkPicture.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ChunkPicture_MouseUp);
			// 
			// panel1
			// 
			panel1.AutoSize = true;
			panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			panel1.Controls.Add(this.flipChunkVButton);
			panel1.Controls.Add(this.chunkBlockEditor);
			panel1.Controls.Add(this.flipChunkHButton);
			panel1.Controls.Add(this.ChunkCount);
			panel1.Controls.Add(this.chunkCtrlLabel);
			panel1.Controls.Add(this.ChunkID);
			panel1.Location = new System.Drawing.Point(0, 262);
			panel1.Margin = new System.Windows.Forms.Padding(0);
			panel1.Name = "panel1";
			panel1.Size = new System.Drawing.Size(180, 225);
			panel1.TabIndex = 2;
			// 
			// flipChunkVButton
			// 
			this.flipChunkVButton.AutoSize = true;
			this.flipChunkVButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flipChunkVButton.Enabled = false;
			this.flipChunkVButton.Location = new System.Drawing.Point(99, 3);
			this.flipChunkVButton.Name = "flipChunkVButton";
			this.flipChunkVButton.Size = new System.Drawing.Size(78, 23);
			this.flipChunkVButton.TabIndex = 6;
			this.flipChunkVButton.Text = "Flip Vertically";
			this.flipChunkVButton.UseVisualStyleBackColor = true;
			this.flipChunkVButton.Click += new System.EventHandler(this.flipChunkVButton_Click);
			// 
			// chunkBlockEditor
			// 
			this.chunkBlockEditor.AutoSize = true;
			this.chunkBlockEditor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.chunkBlockEditor.Location = new System.Drawing.Point(3, 84);
			this.chunkBlockEditor.Name = "chunkBlockEditor";
			this.chunkBlockEditor.SelectedObjects = null;
			this.chunkBlockEditor.Size = new System.Drawing.Size(139, 138);
			this.chunkBlockEditor.TabIndex = 3;
			this.chunkBlockEditor.PropertyValueChanged += new System.EventHandler(this.chunkBlockEditor_PropertyValueChanged);
			// 
			// flipChunkHButton
			// 
			this.flipChunkHButton.AutoSize = true;
			this.flipChunkHButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flipChunkHButton.Enabled = false;
			this.flipChunkHButton.Location = new System.Drawing.Point(3, 3);
			this.flipChunkHButton.Name = "flipChunkHButton";
			this.flipChunkHButton.Size = new System.Drawing.Size(90, 23);
			this.flipChunkHButton.TabIndex = 5;
			this.flipChunkHButton.Text = "Flip Horizontally";
			this.flipChunkHButton.UseVisualStyleBackColor = true;
			this.flipChunkHButton.Click += new System.EventHandler(this.flipChunkHButton_Click);
			// 
			// ChunkCount
			// 
			this.ChunkCount.AutoSize = true;
			this.ChunkCount.Location = new System.Drawing.Point(109, 35);
			this.ChunkCount.Name = "ChunkCount";
			this.ChunkCount.Size = new System.Drawing.Size(42, 13);
			this.ChunkCount.TabIndex = 3;
			this.ChunkCount.Text = "0 / 100";
			// 
			// chunkCtrlLabel
			// 
			this.chunkCtrlLabel.AutoSize = true;
			this.chunkCtrlLabel.Location = new System.Drawing.Point(3, 55);
			this.chunkCtrlLabel.Name = "chunkCtrlLabel";
			this.chunkCtrlLabel.Size = new System.Drawing.Size(147, 26);
			this.chunkCtrlLabel.TabIndex = 4;
			this.chunkCtrlLabel.Text = "LMB: Paint w/ selected block\r\nRMB: Select block";
			this.chunkCtrlLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ChunkID
			// 
			this.ChunkID.Location = new System.Drawing.Point(3, 32);
			this.ChunkID.Name = "ChunkID";
			this.ChunkID.ReadOnly = true;
			this.ChunkID.Size = new System.Drawing.Size(100, 20);
			this.ChunkID.TabIndex = 2;
			this.ChunkID.Text = "0";
			// 
			// panel5
			// 
			panel5.AutoSize = true;
			panel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			panel5.Controls.Add(this.blockTileEditor);
			panel5.Controls.Add(this.flipBlockVButton);
			panel5.Controls.Add(this.flipBlockHButton);
			panel5.Controls.Add(this.BlockCount);
			panel5.Controls.Add(this.BlockID);
			panel5.Controls.Add(this.BlockPicture);
			panel5.Controls.Add(this.ColIndBox);
			panel5.Location = new System.Drawing.Point(262, 0);
			panel5.Margin = new System.Windows.Forms.Padding(0);
			panel5.Name = "panel5";
			this.tableLayoutPanel4.SetRowSpan(panel5, 2);
			panel5.Size = new System.Drawing.Size(198, 326);
			panel5.TabIndex = 0;
			// 
			// blockTileEditor
			// 
			this.blockTileEditor.AutoSize = true;
			this.blockTileEditor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.blockTileEditor.Location = new System.Drawing.Point(3, 192);
			this.blockTileEditor.Name = "blockTileEditor";
			this.blockTileEditor.SelectedObjects = null;
			this.blockTileEditor.Size = new System.Drawing.Size(179, 75);
			this.blockTileEditor.TabIndex = 3;
			this.blockTileEditor.PropertyValueChanged += new System.EventHandler(this.blockTileEditor_PropertyValueChanged);
			// 
			// flipBlockVButton
			// 
			this.flipBlockVButton.AutoSize = true;
			this.flipBlockVButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flipBlockVButton.Enabled = false;
			this.flipBlockVButton.Location = new System.Drawing.Point(99, 137);
			this.flipBlockVButton.Name = "flipBlockVButton";
			this.flipBlockVButton.Size = new System.Drawing.Size(78, 23);
			this.flipBlockVButton.TabIndex = 8;
			this.flipBlockVButton.Text = "Flip Vertically";
			this.flipBlockVButton.UseVisualStyleBackColor = true;
			this.flipBlockVButton.Click += new System.EventHandler(this.flipBlockVButton_Click);
			// 
			// flipBlockHButton
			// 
			this.flipBlockHButton.AutoSize = true;
			this.flipBlockHButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.flipBlockHButton.Enabled = false;
			this.flipBlockHButton.Location = new System.Drawing.Point(3, 137);
			this.flipBlockHButton.Name = "flipBlockHButton";
			this.flipBlockHButton.Size = new System.Drawing.Size(90, 23);
			this.flipBlockHButton.TabIndex = 7;
			this.flipBlockHButton.Text = "Flip Horizontally";
			this.flipBlockHButton.UseVisualStyleBackColor = true;
			this.flipBlockHButton.Click += new System.EventHandler(this.flipBlockHButton_Click);
			// 
			// BlockCount
			// 
			this.BlockCount.AutoSize = true;
			this.BlockCount.Location = new System.Drawing.Point(109, 168);
			this.BlockCount.Name = "BlockCount";
			this.BlockCount.Size = new System.Drawing.Size(42, 13);
			this.BlockCount.TabIndex = 3;
			this.BlockCount.Text = "0 / 400";
			// 
			// BlockID
			// 
			this.BlockID.Location = new System.Drawing.Point(3, 166);
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
			this.BlockPicture.Size = new System.Drawing.Size(128, 128);
			this.BlockPicture.TabIndex = 1;
			this.BlockPicture.Paint += new System.Windows.Forms.PaintEventHandler(this.BlockPicture_Paint);
			this.BlockPicture.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BlockPicture_KeyDown);
			this.BlockPicture.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BlockPicture_MouseDown);
			this.BlockPicture.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BlockPicture_MouseMove);
			this.BlockPicture.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BlockPicture_MouseUp);
			// 
			// ColIndBox
			// 
			this.ColIndBox.Controls.Add(this.button2);
			this.ColIndBox.Controls.Add(this.button1);
			this.ColIndBox.Controls.Add(this.BlockCollision2);
			this.ColIndBox.Controls.Add(this.BlockCollision1);
			this.ColIndBox.Enabled = false;
			this.ColIndBox.Location = new System.Drawing.Point(3, 273);
			this.ColIndBox.Name = "ColIndBox";
			this.ColIndBox.Size = new System.Drawing.Size(192, 50);
			this.ColIndBox.TabIndex = 4;
			this.ColIndBox.TabStop = false;
			this.ColIndBox.Text = "Collision Index";
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
			// panel9
			// 
			panel9.AutoSize = true;
			panel9.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			panel9.Controls.Add(this.calculateAngleButton);
			panel9.Controls.Add(label1);
			panel9.Controls.Add(this.showBlockBehindCollisionCheckBox);
			panel9.Controls.Add(this.ColID);
			panel9.Controls.Add(this.ColAngle);
			panel9.Controls.Add(this.ColPicture);
			panel9.Location = new System.Drawing.Point(460, 215);
			panel9.Margin = new System.Windows.Forms.Padding(0);
			panel9.Name = "panel9";
			panel9.Size = new System.Drawing.Size(134, 246);
			panel9.TabIndex = 1;
			// 
			// calculateAngleButton
			// 
			this.calculateAngleButton.AutoSize = true;
			this.calculateAngleButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.calculateAngleButton.Location = new System.Drawing.Point(5, 171);
			this.calculateAngleButton.Name = "calculateAngleButton";
			this.calculateAngleButton.Size = new System.Drawing.Size(91, 23);
			this.calculateAngleButton.TabIndex = 7;
			this.calculateAngleButton.Text = "Calculate Angle";
			this.calculateAngleButton.UseVisualStyleBackColor = true;
			this.calculateAngleButton.Click += new System.EventHandler(this.calculateAngleButton_Click);
			// 
			// showBlockBehindCollisionCheckBox
			// 
			this.showBlockBehindCollisionCheckBox.AutoSize = true;
			this.showBlockBehindCollisionCheckBox.Location = new System.Drawing.Point(3, 226);
			this.showBlockBehindCollisionCheckBox.Name = "showBlockBehindCollisionCheckBox";
			this.showBlockBehindCollisionCheckBox.Size = new System.Drawing.Size(83, 17);
			this.showBlockBehindCollisionCheckBox.TabIndex = 5;
			this.showBlockBehindCollisionCheckBox.Text = "Show Block";
			this.showBlockBehindCollisionCheckBox.UseVisualStyleBackColor = true;
			this.showBlockBehindCollisionCheckBox.CheckedChanged += new System.EventHandler(this.showBlockBehindCollisionCheckBox_CheckedChanged);
			// 
			// ColID
			// 
			this.ColID.Location = new System.Drawing.Point(3, 200);
			this.ColID.Name = "ColID";
			this.ColID.ReadOnly = true;
			this.ColID.Size = new System.Drawing.Size(100, 20);
			this.ColID.TabIndex = 4;
			this.ColID.Text = "0";
			// 
			// ColAngle
			// 
			this.ColAngle.Hexadecimal = true;
			this.ColAngle.Location = new System.Drawing.Point(45, 145);
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
			this.ColPicture.Size = new System.Drawing.Size(128, 136);
			this.ColPicture.TabIndex = 2;
			this.ColPicture.Paint += new System.Windows.Forms.PaintEventHandler(this.ColPicture_Paint);
			this.ColPicture.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ColPicture_MouseDown);
			this.ColPicture.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ColPicture_MouseMove);
			this.ColPicture.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ColPicture_MouseUp);
			// 
			// TileSelector
			// 
			this.TileSelector.AllowDrop = true;
			this.TileSelector.BackColor = System.Drawing.SystemColors.Window;
			this.TileSelector.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TileSelector.ImageHeight = 64;
			this.TileSelector.ImageSize = 64;
			this.TileSelector.ImageWidth = 64;
			this.TileSelector.Location = new System.Drawing.Point(3, 28);
			this.TileSelector.Name = "TileSelector";
			this.TileSelector.ScrollValue = 0;
			this.TileSelector.SelectedIndex = -1;
			this.TileSelector.Size = new System.Drawing.Size(286, 437);
			this.TileSelector.TabIndex = 2;
			this.TileSelector.SelectedIndexChanged += new System.EventHandler(this.TileSelector_SelectedIndexChanged);
			this.TileSelector.ItemDrag += new System.EventHandler(this.TileSelector_ItemDrag);
			this.TileSelector.DragDrop += new System.Windows.Forms.DragEventHandler(this.TileSelector_DragDrop);
			this.TileSelector.DragEnter += new System.Windows.Forms.DragEventHandler(this.TileSelector_DragEnter);
			this.TileSelector.DragOver += new System.Windows.Forms.DragEventHandler(this.TileSelector_DragOver);
			this.TileSelector.DragLeave += new System.EventHandler(this.TileSelector_DragLeave);
			this.TileSelector.Paint += new System.Windows.Forms.PaintEventHandler(this.TileSelector_Paint);
			this.TileSelector.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TileList_KeyDown);
			this.TileSelector.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TileSelector_MouseDoubleClick);
			this.TileSelector.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TileSelector_MouseDown);
			// 
			// BlockSelector
			// 
			this.BlockSelector.AllowDrop = true;
			this.BlockSelector.BackColor = System.Drawing.SystemColors.Window;
			this.BlockSelector.Dock = System.Windows.Forms.DockStyle.Fill;
			this.BlockSelector.ImageHeight = 64;
			this.BlockSelector.ImageSize = 64;
			this.BlockSelector.ImageWidth = 64;
			this.BlockSelector.Location = new System.Drawing.Point(3, 28);
			this.BlockSelector.Name = "BlockSelector";
			this.BlockSelector.ScrollValue = 0;
			this.BlockSelector.SelectedIndex = -1;
			this.BlockSelector.Size = new System.Drawing.Size(286, 437);
			this.BlockSelector.TabIndex = 2;
			this.BlockSelector.SelectedIndexChanged += new System.EventHandler(this.BlockSelector_SelectedIndexChanged);
			this.BlockSelector.ItemDrag += new System.EventHandler(this.BlockSelector_ItemDrag);
			this.BlockSelector.DragDrop += new System.Windows.Forms.DragEventHandler(this.BlockSelector_DragDrop);
			this.BlockSelector.DragEnter += new System.Windows.Forms.DragEventHandler(this.BlockSelector_DragEnter);
			this.BlockSelector.DragOver += new System.Windows.Forms.DragEventHandler(this.BlockSelector_DragOver);
			this.BlockSelector.DragLeave += new System.EventHandler(this.BlockSelector_DragLeave);
			this.BlockSelector.Paint += new System.Windows.Forms.PaintEventHandler(this.BlockSelector_Paint);
			this.BlockSelector.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TileList_KeyDown);
			this.BlockSelector.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.BlockSelector_MouseDoubleClick);
			this.BlockSelector.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BlockSelector_MouseDown);
			// 
			// panel10
			// 
			this.panel10.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel10.Location = new System.Drawing.Point(3, 28);
			this.panel10.Margin = new System.Windows.Forms.Padding(0);
			this.panel10.Name = "panel10";
			this.panel10.Size = new System.Drawing.Size(286, 437);
			this.panel10.TabIndex = 4;
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.changeLevelToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.buildAndRunToolStripMenuItem,
            this.setupEmulatorToolStripMenuItem,
            this.recentProjectsToolStripMenuItem,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.ShortcutKeyDisplayString = "";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.ShortcutKeyDisplayString = "";
			this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.openToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.openToolStripMenuItem.Text = "&Open...";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// changeLevelToolStripMenuItem
			// 
			this.changeLevelToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noneToolStripMenuItem});
			this.changeLevelToolStripMenuItem.Name = "changeLevelToolStripMenuItem";
			this.changeLevelToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.changeLevelToolStripMenuItem.Text = "&Change Level...";
			// 
			// noneToolStripMenuItem
			// 
			this.noneToolStripMenuItem.Enabled = false;
			this.noneToolStripMenuItem.Name = "noneToolStripMenuItem";
			this.noneToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
			this.noneToolStripMenuItem.Text = "(none)";
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Enabled = false;
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.saveToolStripMenuItem.Text = "&Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// buildAndRunToolStripMenuItem
			// 
			this.buildAndRunToolStripMenuItem.Enabled = false;
			this.buildAndRunToolStripMenuItem.Name = "buildAndRunToolStripMenuItem";
			this.buildAndRunToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.buildAndRunToolStripMenuItem.Text = "&Build and Run...";
			this.buildAndRunToolStripMenuItem.Click += new System.EventHandler(this.buildAndRunToolStripMenuItem_Click);
			// 
			// setupEmulatorToolStripMenuItem
			// 
			this.setupEmulatorToolStripMenuItem.Name = "setupEmulatorToolStripMenuItem";
			this.setupEmulatorToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.setupEmulatorToolStripMenuItem.Text = "Setup E&mulator...";
			this.setupEmulatorToolStripMenuItem.Click += new System.EventHandler(this.setupEmulatorToolStripMenuItem_Click);
			// 
			// recentProjectsToolStripMenuItem
			// 
			this.recentProjectsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noneToolStripMenuItem2});
			this.recentProjectsToolStripMenuItem.Name = "recentProjectsToolStripMenuItem";
			this.recentProjectsToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.recentProjectsToolStripMenuItem.Text = "&Recent Projects";
			this.recentProjectsToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.recentProjectsToolStripMenuItem_DropDownItemClicked);
			// 
			// noneToolStripMenuItem2
			// 
			this.noneToolStripMenuItem2.Enabled = false;
			this.noneToolStripMenuItem2.Name = "noneToolStripMenuItem2";
			this.noneToolStripMenuItem2.Size = new System.Drawing.Size(109, 22);
			this.noneToolStripMenuItem2.Text = "(none)";
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.ShortcutKeyDisplayString = "Alt+F4";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(658, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoCtrlZToolStripMenuItem,
            this.redoCtrlYToolStripMenuItem,
            this.findToolStripMenuItem,
            this.findNextToolStripMenuItem,
            this.findPreviousToolStripMenuItem,
            this.toolStripSeparator3,
            this.blendAlternatePaletteToolStripMenuItem,
            this.resizeLevelToolStripMenuItem,
            this.includeObjectsWithForegroundSelectionToolStripMenuItem,
            this.switchMouseButtonsInChunkAndBlockEditorsToolStripMenuItem});
			this.editToolStripMenuItem.Enabled = false;
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
			this.editToolStripMenuItem.Text = "&Edit";
			// 
			// undoCtrlZToolStripMenuItem
			// 
			this.undoCtrlZToolStripMenuItem.Name = "undoCtrlZToolStripMenuItem";
			this.undoCtrlZToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
			this.undoCtrlZToolStripMenuItem.Size = new System.Drawing.Size(335, 22);
			this.undoCtrlZToolStripMenuItem.Text = "&Undo";
			this.undoCtrlZToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.undoCtrlZToolStripMenuItem_DropDownItemClicked);
			// 
			// redoCtrlYToolStripMenuItem
			// 
			this.redoCtrlYToolStripMenuItem.Name = "redoCtrlYToolStripMenuItem";
			this.redoCtrlYToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
			this.redoCtrlYToolStripMenuItem.Size = new System.Drawing.Size(335, 22);
			this.redoCtrlYToolStripMenuItem.Text = "&Redo";
			this.redoCtrlYToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.redoCtrlYToolStripMenuItem_DropDownItemClicked);
			// 
			// findToolStripMenuItem
			// 
			this.findToolStripMenuItem.Name = "findToolStripMenuItem";
			this.findToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
			this.findToolStripMenuItem.Size = new System.Drawing.Size(335, 22);
			this.findToolStripMenuItem.Text = "&Find...";
			this.findToolStripMenuItem.Click += new System.EventHandler(this.findToolStripMenuItem_Click);
			// 
			// findNextToolStripMenuItem
			// 
			this.findNextToolStripMenuItem.Enabled = false;
			this.findNextToolStripMenuItem.Name = "findNextToolStripMenuItem";
			this.findNextToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
			this.findNextToolStripMenuItem.Size = new System.Drawing.Size(335, 22);
			this.findNextToolStripMenuItem.Text = "Find &Next";
			this.findNextToolStripMenuItem.Click += new System.EventHandler(this.findNextToolStripMenuItem_Click);
			// 
			// findPreviousToolStripMenuItem
			// 
			this.findPreviousToolStripMenuItem.Enabled = false;
			this.findPreviousToolStripMenuItem.Name = "findPreviousToolStripMenuItem";
			this.findPreviousToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
			this.findPreviousToolStripMenuItem.Size = new System.Drawing.Size(335, 22);
			this.findPreviousToolStripMenuItem.Text = "Find &Previous";
			this.findPreviousToolStripMenuItem.Click += new System.EventHandler(this.findPreviousToolStripMenuItem_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(332, 6);
			// 
			// blendAlternatePaletteToolStripMenuItem
			// 
			this.blendAlternatePaletteToolStripMenuItem.Name = "blendAlternatePaletteToolStripMenuItem";
			this.blendAlternatePaletteToolStripMenuItem.Size = new System.Drawing.Size(335, 22);
			this.blendAlternatePaletteToolStripMenuItem.Text = "&Blend Alternate Palette...";
			this.blendAlternatePaletteToolStripMenuItem.Click += new System.EventHandler(this.blendAlternatePaletteToolStripMenuItem_Click);
			// 
			// resizeLevelToolStripMenuItem
			// 
			this.resizeLevelToolStripMenuItem.Name = "resizeLevelToolStripMenuItem";
			this.resizeLevelToolStripMenuItem.Size = new System.Drawing.Size(335, 22);
			this.resizeLevelToolStripMenuItem.Text = "Re&size Level...";
			this.resizeLevelToolStripMenuItem.Click += new System.EventHandler(this.resizeLevelToolStripMenuItem_Click);
			// 
			// includeObjectsWithForegroundSelectionToolStripMenuItem
			// 
			this.includeObjectsWithForegroundSelectionToolStripMenuItem.CheckOnClick = true;
			this.includeObjectsWithForegroundSelectionToolStripMenuItem.Name = "includeObjectsWithForegroundSelectionToolStripMenuItem";
			this.includeObjectsWithForegroundSelectionToolStripMenuItem.Size = new System.Drawing.Size(335, 22);
			this.includeObjectsWithForegroundSelectionToolStripMenuItem.Text = "&Include objects with foreground selection";
			// 
			// switchMouseButtonsInChunkAndBlockEditorsToolStripMenuItem
			// 
			this.switchMouseButtonsInChunkAndBlockEditorsToolStripMenuItem.CheckOnClick = true;
			this.switchMouseButtonsInChunkAndBlockEditorsToolStripMenuItem.Name = "switchMouseButtonsInChunkAndBlockEditorsToolStripMenuItem";
			this.switchMouseButtonsInChunkAndBlockEditorsToolStripMenuItem.Size = new System.Drawing.Size(335, 22);
			this.switchMouseButtonsInChunkAndBlockEditorsToolStripMenuItem.Text = "Switch mouse buttons in chunk and block editors";
			this.switchMouseButtonsInChunkAndBlockEditorsToolStripMenuItem.CheckedChanged += new System.EventHandler(this.switchMouseButtonsInChunkAndBlockEditorsToolStripMenuItem_CheckedChanged);
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.objectsAboveHighPlaneToolStripMenuItem,
            this.hUDToolStripMenuItem,
            this.paletteToolStripMenuItem2,
            this.waterPaletteToolStripMenuItem,
            this.layersToolStripMenuItem,
            this.collisionToolStripMenuItem,
            this.timeZoneToolStripMenuItem,
            this.gridToolStripMenuItem,
            this.zoomToolStripMenuItem,
            this.toolStripSeparator4,
            this.usageCountsToolStripMenuItem,
            this.logToolStripMenuItem});
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.viewToolStripMenuItem.Text = "&View";
			// 
			// objectsAboveHighPlaneToolStripMenuItem
			// 
			this.objectsAboveHighPlaneToolStripMenuItem.Name = "objectsAboveHighPlaneToolStripMenuItem";
			this.objectsAboveHighPlaneToolStripMenuItem.ShortcutKeyDisplayString = "T";
			this.objectsAboveHighPlaneToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.objectsAboveHighPlaneToolStripMenuItem.Text = "&Objects above high plane";
			this.objectsAboveHighPlaneToolStripMenuItem.CheckedChanged += new System.EventHandler(this.objectsAboveHighPlaneToolStripMenuItem_CheckedChanged);
			this.objectsAboveHighPlaneToolStripMenuItem.Click += new System.EventHandler(this.objectsAboveHighPlaneToolStripMenuItem_Click);
			// 
			// hUDToolStripMenuItem
			// 
			this.hUDToolStripMenuItem.Checked = true;
			this.hUDToolStripMenuItem.CheckOnClick = true;
			this.hUDToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.hUDToolStripMenuItem.Name = "hUDToolStripMenuItem";
			this.hUDToolStripMenuItem.ShortcutKeyDisplayString = "O";
			this.hUDToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.hUDToolStripMenuItem.Text = "&HUD";
			// 
			// paletteToolStripMenuItem2
			// 
			this.paletteToolStripMenuItem2.Name = "paletteToolStripMenuItem2";
			this.paletteToolStripMenuItem2.ShortcutKeyDisplayString = "[ ]";
			this.paletteToolStripMenuItem2.Size = new System.Drawing.Size(222, 22);
			this.paletteToolStripMenuItem2.Text = "&Palette";
			this.paletteToolStripMenuItem2.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.paletteToolStripMenuItem2_DropDownItemClicked);
			// 
			// waterPaletteToolStripMenuItem
			// 
			this.waterPaletteToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectPaletteToolStripMenuItem,
            this.setPositionToolStripMenuItem});
			this.waterPaletteToolStripMenuItem.Name = "waterPaletteToolStripMenuItem";
			this.waterPaletteToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.waterPaletteToolStripMenuItem.Text = "&Water Palette";
			this.waterPaletteToolStripMenuItem.Visible = false;
			// 
			// selectPaletteToolStripMenuItem
			// 
			this.selectPaletteToolStripMenuItem.Name = "selectPaletteToolStripMenuItem";
			this.selectPaletteToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
			this.selectPaletteToolStripMenuItem.Text = "Select &Palette";
			this.selectPaletteToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.selectPaletteToolStripMenuItem_DropDownItemClicked);
			// 
			// setPositionToolStripMenuItem
			// 
			this.setPositionToolStripMenuItem.Name = "setPositionToolStripMenuItem";
			this.setPositionToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
			this.setPositionToolStripMenuItem.Text = "Set P&osition...";
			this.setPositionToolStripMenuItem.Click += new System.EventHandler(this.setPositionToolStripMenuItem_Click);
			// 
			// layersToolStripMenuItem
			// 
			this.layersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lowToolStripMenuItem,
            this.highToolStripMenuItem});
			this.layersToolStripMenuItem.Name = "layersToolStripMenuItem";
			this.layersToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.layersToolStripMenuItem.Text = "&Layers";
			// 
			// lowToolStripMenuItem
			// 
			this.lowToolStripMenuItem.Checked = true;
			this.lowToolStripMenuItem.CheckOnClick = true;
			this.lowToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.lowToolStripMenuItem.Name = "lowToolStripMenuItem";
			this.lowToolStripMenuItem.ShortcutKeyDisplayString = "Y";
			this.lowToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
			this.lowToolStripMenuItem.Text = "&Low";
			this.lowToolStripMenuItem.CheckedChanged += new System.EventHandler(this.lowToolStripMenuItem_CheckedChanged);
			this.lowToolStripMenuItem.Click += new System.EventHandler(this.lowToolStripMenuItem_Click);
			// 
			// highToolStripMenuItem
			// 
			this.highToolStripMenuItem.Checked = true;
			this.highToolStripMenuItem.CheckOnClick = true;
			this.highToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.highToolStripMenuItem.Name = "highToolStripMenuItem";
			this.highToolStripMenuItem.ShortcutKeyDisplayString = "U";
			this.highToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
			this.highToolStripMenuItem.Text = "&High";
			this.highToolStripMenuItem.CheckedChanged += new System.EventHandler(this.highToolStripMenuItem_CheckedChanged);
			this.highToolStripMenuItem.Click += new System.EventHandler(this.highToolStripMenuItem_Click);
			// 
			// collisionToolStripMenuItem
			// 
			this.collisionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noneToolStripMenuItem1,
            this.path1ToolStripMenuItem,
            this.path2ToolStripMenuItem,
            this.toolStripSeparator6,
            this.anglesToolStripMenuItem});
			this.collisionToolStripMenuItem.Name = "collisionToolStripMenuItem";
			this.collisionToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.collisionToolStripMenuItem.Text = "&Collision";
			this.collisionToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.collisionToolStripMenuItem_DropDownItemClicked);
			// 
			// noneToolStripMenuItem1
			// 
			this.noneToolStripMenuItem1.Checked = true;
			this.noneToolStripMenuItem1.CheckState = System.Windows.Forms.CheckState.Checked;
			this.noneToolStripMenuItem1.Name = "noneToolStripMenuItem1";
			this.noneToolStripMenuItem1.ShortcutKeyDisplayString = "Q";
			this.noneToolStripMenuItem1.Size = new System.Drawing.Size(125, 22);
			this.noneToolStripMenuItem1.Text = "&None";
			// 
			// path1ToolStripMenuItem
			// 
			this.path1ToolStripMenuItem.Name = "path1ToolStripMenuItem";
			this.path1ToolStripMenuItem.ShortcutKeyDisplayString = "W";
			this.path1ToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
			this.path1ToolStripMenuItem.Text = "Path &1";
			// 
			// path2ToolStripMenuItem
			// 
			this.path2ToolStripMenuItem.Name = "path2ToolStripMenuItem";
			this.path2ToolStripMenuItem.ShortcutKeyDisplayString = "E";
			this.path2ToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
			this.path2ToolStripMenuItem.Text = "Path &2";
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(122, 6);
			// 
			// anglesToolStripMenuItem
			// 
			this.anglesToolStripMenuItem.CheckOnClick = true;
			this.anglesToolStripMenuItem.Name = "anglesToolStripMenuItem";
			this.anglesToolStripMenuItem.ShortcutKeyDisplayString = "R";
			this.anglesToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
			this.anglesToolStripMenuItem.Text = "Angles";
			// 
			// timeZoneToolStripMenuItem
			// 
			this.timeZoneToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.currentOnlyToolStripMenuItem,
            this.allToolStripMenuItem});
			this.timeZoneToolStripMenuItem.Name = "timeZoneToolStripMenuItem";
			this.timeZoneToolStripMenuItem.ShortcutKeyDisplayString = "P";
			this.timeZoneToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.timeZoneToolStripMenuItem.Text = "T&ime Zone";
			this.timeZoneToolStripMenuItem.Visible = false;
			// 
			// currentOnlyToolStripMenuItem
			// 
			this.currentOnlyToolStripMenuItem.Checked = true;
			this.currentOnlyToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.currentOnlyToolStripMenuItem.Name = "currentOnlyToolStripMenuItem";
			this.currentOnlyToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
			this.currentOnlyToolStripMenuItem.Text = "&Current Only";
			this.currentOnlyToolStripMenuItem.Click += new System.EventHandler(this.currentOnlyToolStripMenuItem_Click);
			// 
			// allToolStripMenuItem
			// 
			this.allToolStripMenuItem.Name = "allToolStripMenuItem";
			this.allToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
			this.allToolStripMenuItem.Text = "&All";
			this.allToolStripMenuItem.Click += new System.EventHandler(this.allToolStripMenuItem_Click);
			// 
			// gridToolStripMenuItem
			// 
			this.gridToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.enableGridToolStripMenuItem,
            this.gridColorToolStripMenuItem});
			this.gridToolStripMenuItem.Name = "gridToolStripMenuItem";
			this.gridToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.gridToolStripMenuItem.Text = "&Grid";
			// 
			// enableGridToolStripMenuItem
			// 
			this.enableGridToolStripMenuItem.CheckOnClick = true;
			this.enableGridToolStripMenuItem.Name = "enableGridToolStripMenuItem";
			this.enableGridToolStripMenuItem.ShortcutKeyDisplayString = "I";
			this.enableGridToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
			this.enableGridToolStripMenuItem.Text = "&Enable";
			// 
			// gridColorToolStripMenuItem
			// 
			this.gridColorToolStripMenuItem.Name = "gridColorToolStripMenuItem";
			this.gridColorToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
			this.gridColorToolStripMenuItem.Text = "&Color...";
			this.gridColorToolStripMenuItem.Click += new System.EventHandler(this.gridColorToolStripMenuItem_Click);
			// 
			// zoomToolStripMenuItem
			// 
			this.zoomToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.xToolStripMenuItem7,
            this.xToolStripMenuItem6,
            this.xToolStripMenuItem,
            this.xToolStripMenuItem1,
            this.xToolStripMenuItem2,
            this.xToolStripMenuItem3,
            this.xToolStripMenuItem4,
            this.xToolStripMenuItem5});
			this.zoomToolStripMenuItem.Name = "zoomToolStripMenuItem";
			this.zoomToolStripMenuItem.ShortcutKeyDisplayString = "+ -";
			this.zoomToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.zoomToolStripMenuItem.Text = "&Zoom";
			this.zoomToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.zoomToolStripMenuItem_DropDownItemClicked);
			// 
			// xToolStripMenuItem7
			// 
			this.xToolStripMenuItem7.Name = "xToolStripMenuItem7";
			this.xToolStripMenuItem7.Size = new System.Drawing.Size(96, 22);
			this.xToolStripMenuItem7.Text = "1/8x";
			// 
			// xToolStripMenuItem6
			// 
			this.xToolStripMenuItem6.Name = "xToolStripMenuItem6";
			this.xToolStripMenuItem6.Size = new System.Drawing.Size(96, 22);
			this.xToolStripMenuItem6.Text = "1/4x";
			// 
			// xToolStripMenuItem
			// 
			this.xToolStripMenuItem.Name = "xToolStripMenuItem";
			this.xToolStripMenuItem.Size = new System.Drawing.Size(96, 22);
			this.xToolStripMenuItem.Text = "1/2x";
			// 
			// xToolStripMenuItem1
			// 
			this.xToolStripMenuItem1.Checked = true;
			this.xToolStripMenuItem1.CheckState = System.Windows.Forms.CheckState.Checked;
			this.xToolStripMenuItem1.Name = "xToolStripMenuItem1";
			this.xToolStripMenuItem1.Size = new System.Drawing.Size(96, 22);
			this.xToolStripMenuItem1.Text = "1x";
			// 
			// xToolStripMenuItem2
			// 
			this.xToolStripMenuItem2.Name = "xToolStripMenuItem2";
			this.xToolStripMenuItem2.Size = new System.Drawing.Size(96, 22);
			this.xToolStripMenuItem2.Text = "2x";
			// 
			// xToolStripMenuItem3
			// 
			this.xToolStripMenuItem3.Name = "xToolStripMenuItem3";
			this.xToolStripMenuItem3.Size = new System.Drawing.Size(96, 22);
			this.xToolStripMenuItem3.Text = "3x";
			// 
			// xToolStripMenuItem4
			// 
			this.xToolStripMenuItem4.Name = "xToolStripMenuItem4";
			this.xToolStripMenuItem4.Size = new System.Drawing.Size(96, 22);
			this.xToolStripMenuItem4.Text = "4x";
			// 
			// xToolStripMenuItem5
			// 
			this.xToolStripMenuItem5.Name = "xToolStripMenuItem5";
			this.xToolStripMenuItem5.Size = new System.Drawing.Size(96, 22);
			this.xToolStripMenuItem5.Text = "5x";
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(219, 6);
			// 
			// usageCountsToolStripMenuItem
			// 
			this.usageCountsToolStripMenuItem.Enabled = false;
			this.usageCountsToolStripMenuItem.Name = "usageCountsToolStripMenuItem";
			this.usageCountsToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.usageCountsToolStripMenuItem.Text = "&Usage Counts";
			this.usageCountsToolStripMenuItem.Click += new System.EventHandler(this.usageCountsToolStripMenuItem_Click);
			// 
			// logToolStripMenuItem
			// 
			this.logToolStripMenuItem.Name = "logToolStripMenuItem";
			this.logToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.logToolStripMenuItem.Text = "&Log";
			this.logToolStripMenuItem.Click += new System.EventHandler(this.logToolStripMenuItem_Click);
			// 
			// exportToolStripMenuItem
			// 
			this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.paletteToolStripMenuItem,
            this.tilesToolStripMenuItem,
            this.blocksToolStripMenuItem,
            this.chunksToolStripMenuItem,
            this.solidityMapsToolStripMenuItem,
            this.foregroundToolStripMenuItem,
            this.backgroundToolStripMenuItem,
            toolStripSeparator1,
            this.transparentBackFGBGToolStripMenuItem,
            this.includeobjectsWithFGToolStripMenuItem,
            this.hideDebugObjectsToolStripMenuItem,
            this.useHexadecimalIndexesToolStripMenuItem,
            this.exportArtcollisionpriorityToolStripMenuItem});
			this.exportToolStripMenuItem.Enabled = false;
			this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
			this.exportToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
			this.exportToolStripMenuItem.Text = "E&xport";
			// 
			// paletteToolStripMenuItem
			// 
			this.paletteToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pNGToolStripMenuItem,
            this.yYCHRToolStripMenuItem,
            this.jASCPALToolStripMenuItem});
			this.paletteToolStripMenuItem.Name = "paletteToolStripMenuItem";
			this.paletteToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.paletteToolStripMenuItem.Text = "&Palette";
			// 
			// pNGToolStripMenuItem
			// 
			this.pNGToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            line0ToolStripMenuItem,
            line1ToolStripMenuItem,
            line2ToolStripMenuItem,
            line3ToolStripMenuItem,
            toolStripSeparator2,
            fullToolStripMenuItem});
			this.pNGToolStripMenuItem.Name = "pNGToolStripMenuItem";
			this.pNGToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
			this.pNGToolStripMenuItem.Text = "&PNG";
			this.pNGToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.pNGToolStripMenuItem_DropDownItemClicked);
			// 
			// yYCHRToolStripMenuItem
			// 
			this.yYCHRToolStripMenuItem.Name = "yYCHRToolStripMenuItem";
			this.yYCHRToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
			this.yYCHRToolStripMenuItem.Text = "&YY-CHR";
			this.yYCHRToolStripMenuItem.Click += new System.EventHandler(this.yYCHRToolStripMenuItem_Click);
			// 
			// jASCPALToolStripMenuItem
			// 
			this.jASCPALToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            line0ToolStripMenuItem1,
            line1ToolStripMenuItem1,
            line2ToolStripMenuItem1,
            line3ToolStripMenuItem1,
            toolStripSeparator13,
            fullToolStripMenuItem1});
			this.jASCPALToolStripMenuItem.Name = "jASCPALToolStripMenuItem";
			this.jASCPALToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
			this.jASCPALToolStripMenuItem.Text = "JASC-PAL";
			this.jASCPALToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.jASCPALToolStripMenuItem_DropDownItemClicked);
			// 
			// tilesToolStripMenuItem
			// 
			this.tilesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.paletteLine0ToolStripMenuItem,
            this.paletteLine1ToolStripMenuItem,
            this.paletteLine2ToolStripMenuItem,
            this.paletteLine3ToolStripMenuItem});
			this.tilesToolStripMenuItem.Name = "tilesToolStripMenuItem";
			this.tilesToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.tilesToolStripMenuItem.Text = "&Tiles";
			this.tilesToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tilesToolStripMenuItem_DropDownItemClicked);
			// 
			// paletteLine0ToolStripMenuItem
			// 
			this.paletteLine0ToolStripMenuItem.Name = "paletteLine0ToolStripMenuItem";
			this.paletteLine0ToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
			this.paletteLine0ToolStripMenuItem.Text = "Palette line &0";
			// 
			// paletteLine1ToolStripMenuItem
			// 
			this.paletteLine1ToolStripMenuItem.Name = "paletteLine1ToolStripMenuItem";
			this.paletteLine1ToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
			this.paletteLine1ToolStripMenuItem.Text = "Palette line &1";
			// 
			// paletteLine2ToolStripMenuItem
			// 
			this.paletteLine2ToolStripMenuItem.Name = "paletteLine2ToolStripMenuItem";
			this.paletteLine2ToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
			this.paletteLine2ToolStripMenuItem.Text = "Palette line &2";
			// 
			// paletteLine3ToolStripMenuItem
			// 
			this.paletteLine3ToolStripMenuItem.Name = "paletteLine3ToolStripMenuItem";
			this.paletteLine3ToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
			this.paletteLine3ToolStripMenuItem.Text = "Palette line &3";
			// 
			// blocksToolStripMenuItem
			// 
			this.blocksToolStripMenuItem.Name = "blocksToolStripMenuItem";
			this.blocksToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.blocksToolStripMenuItem.Text = "&Blocks";
			this.blocksToolStripMenuItem.Click += new System.EventHandler(this.blocksToolStripMenuItem_Click);
			// 
			// chunksToolStripMenuItem
			// 
			this.chunksToolStripMenuItem.Name = "chunksToolStripMenuItem";
			this.chunksToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.chunksToolStripMenuItem.Text = "&Chunks";
			this.chunksToolStripMenuItem.Click += new System.EventHandler(this.chunksToolStripMenuItem_Click);
			// 
			// solidityMapsToolStripMenuItem
			// 
			this.solidityMapsToolStripMenuItem.Name = "solidityMapsToolStripMenuItem";
			this.solidityMapsToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.solidityMapsToolStripMenuItem.Text = "&Solidity Maps";
			this.solidityMapsToolStripMenuItem.Click += new System.EventHandler(this.solidityMapsToolStripMenuItem_Click);
			// 
			// foregroundToolStripMenuItem
			// 
			this.foregroundToolStripMenuItem.Name = "foregroundToolStripMenuItem";
			this.foregroundToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.foregroundToolStripMenuItem.Text = "&Foreground";
			this.foregroundToolStripMenuItem.Click += new System.EventHandler(this.foregroundToolStripMenuItem_Click);
			// 
			// backgroundToolStripMenuItem
			// 
			this.backgroundToolStripMenuItem.Name = "backgroundToolStripMenuItem";
			this.backgroundToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.backgroundToolStripMenuItem.Text = "B&ackground";
			this.backgroundToolStripMenuItem.Click += new System.EventHandler(this.backgroundToolStripMenuItem_Click);
			// 
			// transparentBackFGBGToolStripMenuItem
			// 
			this.transparentBackFGBGToolStripMenuItem.Checked = true;
			this.transparentBackFGBGToolStripMenuItem.CheckOnClick = true;
			this.transparentBackFGBGToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.transparentBackFGBGToolStripMenuItem.Name = "transparentBackFGBGToolStripMenuItem";
			this.transparentBackFGBGToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.transparentBackFGBGToolStripMenuItem.Text = "T&ransparent back (FG/BG)";
			this.transparentBackFGBGToolStripMenuItem.CheckedChanged += new System.EventHandler(this.transparentBackFGBGToolStripMenuItem_CheckedChanged);
			// 
			// includeobjectsWithFGToolStripMenuItem
			// 
			this.includeobjectsWithFGToolStripMenuItem.CheckOnClick = true;
			this.includeobjectsWithFGToolStripMenuItem.Name = "includeobjectsWithFGToolStripMenuItem";
			this.includeobjectsWithFGToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.includeobjectsWithFGToolStripMenuItem.Text = "Include &objects with FG";
			this.includeobjectsWithFGToolStripMenuItem.CheckedChanged += new System.EventHandler(this.includeObjectsWithFGToolStripMenuItem_CheckedChanged);
			// 
			// hideDebugObjectsToolStripMenuItem
			// 
			this.hideDebugObjectsToolStripMenuItem.CheckOnClick = true;
			this.hideDebugObjectsToolStripMenuItem.Name = "hideDebugObjectsToolStripMenuItem";
			this.hideDebugObjectsToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.hideDebugObjectsToolStripMenuItem.Text = "&Hide debug objects";
			this.hideDebugObjectsToolStripMenuItem.CheckedChanged += new System.EventHandler(this.hideDebugObjectsToolStripMenuItem_CheckedChanged);
			// 
			// useHexadecimalIndexesToolStripMenuItem
			// 
			this.useHexadecimalIndexesToolStripMenuItem.CheckOnClick = true;
			this.useHexadecimalIndexesToolStripMenuItem.Name = "useHexadecimalIndexesToolStripMenuItem";
			this.useHexadecimalIndexesToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.useHexadecimalIndexesToolStripMenuItem.Text = "Use he&xadecimal indexes";
			this.useHexadecimalIndexesToolStripMenuItem.CheckedChanged += new System.EventHandler(this.useHexadecimalIndexesToolStripMenuItem_CheckedChanged);
			// 
			// exportArtcollisionpriorityToolStripMenuItem
			// 
			this.exportArtcollisionpriorityToolStripMenuItem.CheckOnClick = true;
			this.exportArtcollisionpriorityToolStripMenuItem.Name = "exportArtcollisionpriorityToolStripMenuItem";
			this.exportArtcollisionpriorityToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
			this.exportArtcollisionpriorityToolStripMenuItem.Text = "Export art+collision+priority";
			this.exportArtcollisionpriorityToolStripMenuItem.CheckedChanged += new System.EventHandler(this.exportArtcollisionpriorityToolStripMenuItem_CheckedChanged);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewReadmeToolStripMenuItem,
            this.reportBugToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.helpToolStripMenuItem.Text = "&Help";
			// 
			// viewReadmeToolStripMenuItem
			// 
			this.viewReadmeToolStripMenuItem.Name = "viewReadmeToolStripMenuItem";
			this.viewReadmeToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
			this.viewReadmeToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.viewReadmeToolStripMenuItem.Text = "View &Readme";
			this.viewReadmeToolStripMenuItem.Click += new System.EventHandler(this.viewReadmeToolStripMenuItem_Click);
			// 
			// reportBugToolStripMenuItem
			// 
			this.reportBugToolStripMenuItem.Name = "reportBugToolStripMenuItem";
			this.reportBugToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
			this.reportBugToolStripMenuItem.Text = "Report &Bug...";
			this.reportBugToolStripMenuItem.Click += new System.EventHandler(this.reportBugToolStripMenuItem_Click);
			// 
			// backgroundLevelLoader
			// 
			this.backgroundLevelLoader.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundLevelLoader_DoWork);
			this.backgroundLevelLoader.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundLevelLoader_RunWorkerCompleted);
			// 
			// objectContextMenuStrip
			// 
			this.objectContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addObjectToolStripMenuItem,
            this.addRingToolStripMenuItem,
            this.addGroupOfObjectsToolStripMenuItem,
            this.addGroupOfRingsToolStripMenuItem,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator5,
            this.selectAllObjectsToolStripMenuItem,
            this.selectAllRingsToolStripMenuItem});
			this.objectContextMenuStrip.Name = "contextMenuStrip1";
			this.objectContextMenuStrip.Size = new System.Drawing.Size(199, 230);
			// 
			// addObjectToolStripMenuItem
			// 
			this.addObjectToolStripMenuItem.Image = global::SonicRetro.SonLVL.Properties.Resources.mon;
			this.addObjectToolStripMenuItem.Name = "addObjectToolStripMenuItem";
			this.addObjectToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
			this.addObjectToolStripMenuItem.Text = "Add &Object...";
			this.addObjectToolStripMenuItem.Click += new System.EventHandler(this.addObjectToolStripMenuItem_Click);
			// 
			// addRingToolStripMenuItem
			// 
			this.addRingToolStripMenuItem.Image = global::SonicRetro.SonLVL.Properties.Resources.rng;
			this.addRingToolStripMenuItem.Name = "addRingToolStripMenuItem";
			this.addRingToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
			this.addRingToolStripMenuItem.Text = "Add &Ring...";
			this.addRingToolStripMenuItem.Click += new System.EventHandler(this.addRingToolStripMenuItem_Click);
			// 
			// addGroupOfObjectsToolStripMenuItem
			// 
			this.addGroupOfObjectsToolStripMenuItem.Image = global::SonicRetro.SonLVL.Properties.Resources.mon;
			this.addGroupOfObjectsToolStripMenuItem.Name = "addGroupOfObjectsToolStripMenuItem";
			this.addGroupOfObjectsToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
			this.addGroupOfObjectsToolStripMenuItem.Text = "Add Group of O&bjects...";
			this.addGroupOfObjectsToolStripMenuItem.Click += new System.EventHandler(this.addGroupOfObjectsToolStripMenuItem_Click);
			// 
			// addGroupOfRingsToolStripMenuItem
			// 
			this.addGroupOfRingsToolStripMenuItem.Image = global::SonicRetro.SonLVL.Properties.Resources.rng;
			this.addGroupOfRingsToolStripMenuItem.Name = "addGroupOfRingsToolStripMenuItem";
			this.addGroupOfRingsToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
			this.addGroupOfRingsToolStripMenuItem.Text = "Add Group of R&ings...";
			this.addGroupOfRingsToolStripMenuItem.Click += new System.EventHandler(this.addGroupOfRingsToolStripMenuItem_Click);
			// 
			// cutToolStripMenuItem
			// 
			this.cutToolStripMenuItem.Image = global::SonicRetro.SonLVL.Properties.Resources.cut;
			this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
			this.cutToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
			this.cutToolStripMenuItem.Text = "Cu&t";
			this.cutToolStripMenuItem.Click += new System.EventHandler(this.cutToolStripMenuItem_Click);
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.Image = global::SonicRetro.SonLVL.Properties.Resources.copy;
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			this.copyToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
			this.copyToolStripMenuItem.Text = "&Copy";
			this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
			// 
			// pasteToolStripMenuItem
			// 
			this.pasteToolStripMenuItem.Image = global::SonicRetro.SonLVL.Properties.Resources.paste;
			this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
			this.pasteToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
			this.pasteToolStripMenuItem.Text = "&Paste";
			this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
			// 
			// deleteToolStripMenuItem
			// 
			this.deleteToolStripMenuItem.Image = global::SonicRetro.SonLVL.Properties.Resources.delete;
			this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
			this.deleteToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
			this.deleteToolStripMenuItem.Text = "&Delete";
			this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(195, 6);
			// 
			// selectAllObjectsToolStripMenuItem
			// 
			this.selectAllObjectsToolStripMenuItem.Image = global::SonicRetro.SonLVL.Properties.Resources.mon;
			this.selectAllObjectsToolStripMenuItem.Name = "selectAllObjectsToolStripMenuItem";
			this.selectAllObjectsToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
			this.selectAllObjectsToolStripMenuItem.Text = "&Select All Objects";
			this.selectAllObjectsToolStripMenuItem.Click += new System.EventHandler(this.selectAllObjectsToolStripMenuItem_Click);
			// 
			// selectAllRingsToolStripMenuItem
			// 
			this.selectAllRingsToolStripMenuItem.Image = global::SonicRetro.SonLVL.Properties.Resources.rng;
			this.selectAllRingsToolStripMenuItem.Name = "selectAllRingsToolStripMenuItem";
			this.selectAllRingsToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
			this.selectAllRingsToolStripMenuItem.Text = "S&elect All Rings";
			this.selectAllRingsToolStripMenuItem.Click += new System.EventHandler(this.selectAllRingsToolStripMenuItem_Click);
			// 
			// hScrollBar1
			// 
			this.hScrollBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.hScrollBar1.Enabled = false;
			this.hScrollBar1.LargeChange = 128;
			this.hScrollBar1.Location = new System.Drawing.Point(0, 355);
			this.hScrollBar1.Maximum = 128;
			this.hScrollBar1.Name = "hScrollBar1";
			this.hScrollBar1.Size = new System.Drawing.Size(331, 17);
			this.hScrollBar1.SmallChange = 16;
			this.hScrollBar1.TabIndex = 3;
			this.hScrollBar1.ValueChanged += new System.EventHandler(this.ScrollBar_ValueChanged);
			// 
			// vScrollBar1
			// 
			this.vScrollBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.vScrollBar1.Enabled = false;
			this.vScrollBar1.LargeChange = 128;
			this.vScrollBar1.Location = new System.Drawing.Point(331, 0);
			this.vScrollBar1.Maximum = 128;
			this.vScrollBar1.Name = "vScrollBar1";
			this.vScrollBar1.Size = new System.Drawing.Size(17, 355);
			this.vScrollBar1.SmallChange = 16;
			this.vScrollBar1.TabIndex = 2;
			this.vScrollBar1.ValueChanged += new System.EventHandler(this.ScrollBar_ValueChanged);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Controls.Add(this.objectPanel, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.vScrollBar1, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.hScrollBar1, 0, 1);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 25);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 2;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(348, 372);
			this.tableLayoutPanel1.TabIndex = 2;
			// 
			// objectPanel
			// 
			this.objectPanel.AllowDrop = true;
			this.objectPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.objectPanel.Location = new System.Drawing.Point(0, 0);
			this.objectPanel.Margin = new System.Windows.Forms.Padding(0);
			this.objectPanel.Name = "objectPanel";
			this.objectPanel.Size = new System.Drawing.Size(331, 355);
			this.objectPanel.TabIndex = 1;
			this.objectPanel.DragDrop += new System.Windows.Forms.DragEventHandler(this.objectPanel_DragDrop);
			this.objectPanel.DragEnter += new System.Windows.Forms.DragEventHandler(this.objectPanel_DragEnter);
			this.objectPanel.DragOver += new System.Windows.Forms.DragEventHandler(this.objectPanel_DragOver);
			this.objectPanel.DragLeave += new System.EventHandler(this.objectPanel_DragLeave);
			this.objectPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.panel_Paint);
			this.objectPanel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.objectPanel_KeyDown);
			this.objectPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.objectPanel_MouseDown);
			this.objectPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.objectPanel_MouseMove);
			this.objectPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.objectPanel_MouseUp);
			this.objectPanel.Resize += new System.EventHandler(this.panel_Resize);
			// 
			// ObjectProperties
			// 
			this.ObjectProperties.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
			this.ObjectProperties.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ObjectProperties.Location = new System.Drawing.Point(0, 0);
			this.ObjectProperties.Margin = new System.Windows.Forms.Padding(0);
			this.ObjectProperties.Name = "ObjectProperties";
			this.ObjectProperties.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
			this.ObjectProperties.Size = new System.Drawing.Size(196, 397);
			this.ObjectProperties.TabIndex = 12;
			this.ObjectProperties.ToolbarVisible = false;
			this.ObjectProperties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.ObjectProperties_PropertyValueChanged);
			this.ObjectProperties.SelectedGridItemChanged += new System.Windows.Forms.SelectedGridItemChangedEventHandler(this.ObjectProperties_SelectedGridItemChanged);
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Controls.Add(this.tabPage4);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 104);
			this.tabControl1.Multiline = true;
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(658, 423);
			this.tabControl1.TabIndex = 3;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.splitContainer1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Margin = new System.Windows.Forms.Padding(0);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(650, 397);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Objects";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Margin = new System.Windows.Forms.Padding(0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.splitContainer4);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.ObjectProperties);
			this.splitContainer1.Size = new System.Drawing.Size(650, 397);
			this.splitContainer1.SplitterDistance = 450;
			this.splitContainer1.TabIndex = 3;
			// 
			// splitContainer4
			// 
			this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer4.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer4.Location = new System.Drawing.Point(0, 0);
			this.splitContainer4.Name = "splitContainer4";
			// 
			// splitContainer4.Panel1
			// 
			this.splitContainer4.Panel1.Controls.Add(this.objectTypeList);
			// 
			// splitContainer4.Panel2
			// 
			this.splitContainer4.Panel2.Controls.Add(this.tableLayoutPanel1);
			this.splitContainer4.Panel2.Controls.Add(this.objToolStrip);
			this.splitContainer4.Size = new System.Drawing.Size(450, 397);
			this.splitContainer4.SplitterDistance = 98;
			this.splitContainer4.TabIndex = 3;
			// 
			// objectTypeList
			// 
			this.objectTypeList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.objectTypeList.LargeImageList = this.objectTypeImages;
			this.objectTypeList.Location = new System.Drawing.Point(0, 0);
			this.objectTypeList.MultiSelect = false;
			this.objectTypeList.Name = "objectTypeList";
			this.objectTypeList.Size = new System.Drawing.Size(98, 397);
			this.objectTypeList.TabIndex = 0;
			this.objectTypeList.UseCompatibleStateImageBehavior = false;
			this.objectTypeList.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.objectTypeList_ItemDrag);
			// 
			// objectTypeImages
			// 
			this.objectTypeImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.objectTypeImages.ImageSize = new System.Drawing.Size(32, 32);
			this.objectTypeImages.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// objToolStrip
			// 
			this.objToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.objToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.objGridSizeDropDownButton,
            toolStripSeparator10,
            this.alignLeftWallToolStripButton,
            this.alignGroundToolStripButton,
            this.alignRightWallToolStripButton,
            this.alignCeilingToolStripButton,
            toolStripSeparator8,
            this.alignLeftsToolStripButton,
            this.alignCentersToolStripButton,
            this.alignRightsToolStripButton,
            toolStripSeparator9,
            this.alignTopsToolStripButton,
            this.alignMiddlesToolStripButton,
            this.alignBottomsToolStripButton});
			this.objToolStrip.Location = new System.Drawing.Point(0, 0);
			this.objToolStrip.Name = "objToolStrip";
			this.objToolStrip.Size = new System.Drawing.Size(348, 25);
			this.objToolStrip.TabIndex = 4;
			this.objToolStrip.Text = "toolStrip1";
			// 
			// objGridSizeDropDownButton
			// 
			this.objGridSizeDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.objGridSizeDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5,
            this.toolStripMenuItem6,
            this.toolStripMenuItem7,
            this.toolStripMenuItem8,
            this.toolStripMenuItem9,
            this.toolStripMenuItem10,
            this.toolStripMenuItem11,
            this.toolStripMenuItem3,
            this.toolStripMenuItem12});
			this.objGridSizeDropDownButton.Image = ((System.Drawing.Image)(resources.GetObject("objGridSizeDropDownButton.Image")));
			this.objGridSizeDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.objGridSizeDropDownButton.Name = "objGridSizeDropDownButton";
			this.objGridSizeDropDownButton.Size = new System.Drawing.Size(77, 22);
			this.objGridSizeDropDownButton.Text = "Grid Size: 1";
			this.objGridSizeDropDownButton.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.objGridSizeDropDownButton_DropDownItemClicked);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(98, 22);
			this.toolStripMenuItem2.Text = "1";
			// 
			// toolStripMenuItem4
			// 
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			this.toolStripMenuItem4.Size = new System.Drawing.Size(98, 22);
			this.toolStripMenuItem4.Text = "2";
			// 
			// toolStripMenuItem5
			// 
			this.toolStripMenuItem5.Name = "toolStripMenuItem5";
			this.toolStripMenuItem5.Size = new System.Drawing.Size(98, 22);
			this.toolStripMenuItem5.Text = "4";
			// 
			// toolStripMenuItem6
			// 
			this.toolStripMenuItem6.Name = "toolStripMenuItem6";
			this.toolStripMenuItem6.Size = new System.Drawing.Size(98, 22);
			this.toolStripMenuItem6.Text = "8";
			// 
			// toolStripMenuItem7
			// 
			this.toolStripMenuItem7.Name = "toolStripMenuItem7";
			this.toolStripMenuItem7.Size = new System.Drawing.Size(98, 22);
			this.toolStripMenuItem7.Text = "16";
			// 
			// toolStripMenuItem8
			// 
			this.toolStripMenuItem8.Name = "toolStripMenuItem8";
			this.toolStripMenuItem8.Size = new System.Drawing.Size(98, 22);
			this.toolStripMenuItem8.Text = "32";
			// 
			// toolStripMenuItem9
			// 
			this.toolStripMenuItem9.Name = "toolStripMenuItem9";
			this.toolStripMenuItem9.Size = new System.Drawing.Size(98, 22);
			this.toolStripMenuItem9.Text = "64";
			// 
			// toolStripMenuItem10
			// 
			this.toolStripMenuItem10.Name = "toolStripMenuItem10";
			this.toolStripMenuItem10.Size = new System.Drawing.Size(98, 22);
			this.toolStripMenuItem10.Text = "128";
			// 
			// toolStripMenuItem11
			// 
			this.toolStripMenuItem11.Name = "toolStripMenuItem11";
			this.toolStripMenuItem11.Size = new System.Drawing.Size(98, 22);
			this.toolStripMenuItem11.Text = "256";
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(98, 22);
			this.toolStripMenuItem3.Text = "512";
			// 
			// toolStripMenuItem12
			// 
			this.toolStripMenuItem12.Name = "toolStripMenuItem12";
			this.toolStripMenuItem12.Size = new System.Drawing.Size(98, 22);
			this.toolStripMenuItem12.Text = "1024";
			// 
			// alignLeftWallToolStripButton
			// 
			this.alignLeftWallToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.alignLeftWallToolStripButton.Enabled = false;
			this.alignLeftWallToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("alignLeftWallToolStripButton.Image")));
			this.alignLeftWallToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.alignLeftWallToolStripButton.Name = "alignLeftWallToolStripButton";
			this.alignLeftWallToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.alignLeftWallToolStripButton.Text = "Align with Left Wall";
			this.alignLeftWallToolStripButton.Click += new System.EventHandler(this.alignLeftWallToolStripButton_Click);
			// 
			// alignGroundToolStripButton
			// 
			this.alignGroundToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.alignGroundToolStripButton.Enabled = false;
			this.alignGroundToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("alignGroundToolStripButton.Image")));
			this.alignGroundToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.alignGroundToolStripButton.Name = "alignGroundToolStripButton";
			this.alignGroundToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.alignGroundToolStripButton.Text = "Align with ground";
			this.alignGroundToolStripButton.Click += new System.EventHandler(this.alignGroundToolStripButton_Click);
			// 
			// alignRightWallToolStripButton
			// 
			this.alignRightWallToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.alignRightWallToolStripButton.Enabled = false;
			this.alignRightWallToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("alignRightWallToolStripButton.Image")));
			this.alignRightWallToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.alignRightWallToolStripButton.Name = "alignRightWallToolStripButton";
			this.alignRightWallToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.alignRightWallToolStripButton.Text = "Align with Right Wall";
			this.alignRightWallToolStripButton.Click += new System.EventHandler(this.alignRightWallToolStripButton_Click);
			// 
			// alignCeilingToolStripButton
			// 
			this.alignCeilingToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.alignCeilingToolStripButton.Enabled = false;
			this.alignCeilingToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("alignCeilingToolStripButton.Image")));
			this.alignCeilingToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.alignCeilingToolStripButton.Name = "alignCeilingToolStripButton";
			this.alignCeilingToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.alignCeilingToolStripButton.Text = "Align with Ceiling";
			this.alignCeilingToolStripButton.Click += new System.EventHandler(this.alignCeilingToolStripButton_Click);
			// 
			// alignLeftsToolStripButton
			// 
			this.alignLeftsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.alignLeftsToolStripButton.Enabled = false;
			this.alignLeftsToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("alignLeftsToolStripButton.Image")));
			this.alignLeftsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.alignLeftsToolStripButton.Name = "alignLeftsToolStripButton";
			this.alignLeftsToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.alignLeftsToolStripButton.Text = "Align Lefts";
			this.alignLeftsToolStripButton.Click += new System.EventHandler(this.alignLeftsToolStripButton_Click);
			// 
			// alignCentersToolStripButton
			// 
			this.alignCentersToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.alignCentersToolStripButton.Enabled = false;
			this.alignCentersToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("alignCentersToolStripButton.Image")));
			this.alignCentersToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.alignCentersToolStripButton.Name = "alignCentersToolStripButton";
			this.alignCentersToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.alignCentersToolStripButton.Text = "Align Centers";
			this.alignCentersToolStripButton.Click += new System.EventHandler(this.alignCentersToolStripButton_Click);
			// 
			// alignRightsToolStripButton
			// 
			this.alignRightsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.alignRightsToolStripButton.Enabled = false;
			this.alignRightsToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("alignRightsToolStripButton.Image")));
			this.alignRightsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.alignRightsToolStripButton.Name = "alignRightsToolStripButton";
			this.alignRightsToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.alignRightsToolStripButton.Text = "Align Rights";
			this.alignRightsToolStripButton.Click += new System.EventHandler(this.alignRightsToolStripButton_Click);
			// 
			// alignTopsToolStripButton
			// 
			this.alignTopsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.alignTopsToolStripButton.Enabled = false;
			this.alignTopsToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("alignTopsToolStripButton.Image")));
			this.alignTopsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.alignTopsToolStripButton.Name = "alignTopsToolStripButton";
			this.alignTopsToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.alignTopsToolStripButton.Text = "Align Tops";
			this.alignTopsToolStripButton.Click += new System.EventHandler(this.alignTopsToolStripButton_Click);
			// 
			// alignMiddlesToolStripButton
			// 
			this.alignMiddlesToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.alignMiddlesToolStripButton.Enabled = false;
			this.alignMiddlesToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("alignMiddlesToolStripButton.Image")));
			this.alignMiddlesToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.alignMiddlesToolStripButton.Name = "alignMiddlesToolStripButton";
			this.alignMiddlesToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.alignMiddlesToolStripButton.Text = "Align Middles";
			this.alignMiddlesToolStripButton.Click += new System.EventHandler(this.alignMiddlesToolStripButton_Click);
			// 
			// alignBottomsToolStripButton
			// 
			this.alignBottomsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.alignBottomsToolStripButton.Enabled = false;
			this.alignBottomsToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("alignBottomsToolStripButton.Image")));
			this.alignBottomsToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.alignBottomsToolStripButton.Name = "alignBottomsToolStripButton";
			this.alignBottomsToolStripButton.Size = new System.Drawing.Size(23, 22);
			this.alignBottomsToolStripButton.Text = "Align Bottoms";
			this.alignBottomsToolStripButton.Click += new System.EventHandler(this.alignBottomsToolStripButton_Click);
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.splitContainer2);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Margin = new System.Windows.Forms.Padding(0);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(650, 397);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Foreground";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Margin = new System.Windows.Forms.Padding(0);
			this.splitContainer2.Name = "splitContainer2";
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.tableLayoutPanel2);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(tabControl2);
			this.splitContainer2.Size = new System.Drawing.Size(650, 397);
			this.splitContainer2.SplitterDistance = 384;
			this.splitContainer2.TabIndex = 4;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 2;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel2.Controls.Add(this.foregroundPanel, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this.vScrollBar2, 1, 1);
			this.tableLayoutPanel2.Controls.Add(this.hScrollBar2, 0, 2);
			this.tableLayoutPanel2.Controls.Add(this.fgToolStrip, 0, 0);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 3;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 66.66666F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.Size = new System.Drawing.Size(384, 397);
			this.tableLayoutPanel2.TabIndex = 3;
			// 
			// foregroundPanel
			// 
			this.foregroundPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.foregroundPanel.Location = new System.Drawing.Point(0, 25);
			this.foregroundPanel.Margin = new System.Windows.Forms.Padding(0);
			this.foregroundPanel.Name = "foregroundPanel";
			this.foregroundPanel.Size = new System.Drawing.Size(367, 355);
			this.foregroundPanel.TabIndex = 1;
			this.foregroundPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.panel_Paint);
			this.foregroundPanel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.foregroundPanel_KeyDown);
			this.foregroundPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.foregroundPanel_MouseDown);
			this.foregroundPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.foregroundPanel_MouseMove);
			this.foregroundPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.foregroundPanel_MouseUp);
			this.foregroundPanel.Resize += new System.EventHandler(this.panel_Resize);
			// 
			// vScrollBar2
			// 
			this.vScrollBar2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.vScrollBar2.Enabled = false;
			this.vScrollBar2.LargeChange = 128;
			this.vScrollBar2.Location = new System.Drawing.Point(367, 25);
			this.vScrollBar2.Maximum = 128;
			this.vScrollBar2.Name = "vScrollBar2";
			this.vScrollBar2.Size = new System.Drawing.Size(17, 355);
			this.vScrollBar2.SmallChange = 16;
			this.vScrollBar2.TabIndex = 2;
			this.vScrollBar2.ValueChanged += new System.EventHandler(this.ScrollBar_ValueChanged);
			// 
			// hScrollBar2
			// 
			this.hScrollBar2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.hScrollBar2.Enabled = false;
			this.hScrollBar2.LargeChange = 128;
			this.hScrollBar2.Location = new System.Drawing.Point(0, 380);
			this.hScrollBar2.Maximum = 128;
			this.hScrollBar2.Name = "hScrollBar2";
			this.hScrollBar2.Size = new System.Drawing.Size(367, 17);
			this.hScrollBar2.SmallChange = 16;
			this.hScrollBar2.TabIndex = 3;
			this.hScrollBar2.ValueChanged += new System.EventHandler(this.ScrollBar_ValueChanged);
			// 
			// fgToolStrip
			// 
			this.tableLayoutPanel2.SetColumnSpan(this.fgToolStrip, 2);
			this.fgToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.fgToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fgDrawToolStripButton,
            this.fgSelectToolStripButton,
            toolStripSeparator14,
            this.replaceForegroundToolStripButton,
            this.clearForegroundToolStripButton});
			this.fgToolStrip.Location = new System.Drawing.Point(0, 0);
			this.fgToolStrip.Name = "fgToolStrip";
			this.fgToolStrip.Size = new System.Drawing.Size(384, 25);
			this.fgToolStrip.TabIndex = 4;
			this.fgToolStrip.Text = "toolStrip1";
			// 
			// fgDrawToolStripButton
			// 
			this.fgDrawToolStripButton.Checked = true;
			this.fgDrawToolStripButton.CheckState = System.Windows.Forms.CheckState.Checked;
			this.fgDrawToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("fgDrawToolStripButton.Image")));
			this.fgDrawToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.fgDrawToolStripButton.Name = "fgDrawToolStripButton";
			this.fgDrawToolStripButton.Size = new System.Drawing.Size(54, 22);
			this.fgDrawToolStripButton.Text = "Draw";
			this.fgDrawToolStripButton.Click += new System.EventHandler(this.fgDrawToolStripButton_Click);
			// 
			// fgSelectToolStripButton
			// 
			this.fgSelectToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("fgSelectToolStripButton.Image")));
			this.fgSelectToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.fgSelectToolStripButton.Name = "fgSelectToolStripButton";
			this.fgSelectToolStripButton.Size = new System.Drawing.Size(58, 22);
			this.fgSelectToolStripButton.Text = "Select";
			this.fgSelectToolStripButton.Click += new System.EventHandler(this.fgSelectToolStripButton_Click);
			// 
			// replaceForegroundToolStripButton
			// 
			this.replaceForegroundToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.replaceForegroundToolStripButton.Enabled = false;
			this.replaceForegroundToolStripButton.Name = "replaceForegroundToolStripButton";
			this.replaceForegroundToolStripButton.Size = new System.Drawing.Size(52, 22);
			this.replaceForegroundToolStripButton.Text = "Replace";
			this.replaceForegroundToolStripButton.Click += new System.EventHandler(this.replaceForegroundToolStripButton_Click);
			// 
			// clearForegroundToolStripButton
			// 
			this.clearForegroundToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.clearForegroundToolStripButton.Enabled = false;
			this.clearForegroundToolStripButton.Name = "clearForegroundToolStripButton";
			this.clearForegroundToolStripButton.Size = new System.Drawing.Size(38, 22);
			this.clearForegroundToolStripButton.Text = "Clear";
			this.clearForegroundToolStripButton.Click += new System.EventHandler(this.clearForegroundToolStripButton_Click);
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.splitContainer3);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Margin = new System.Windows.Forms.Padding(0);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(650, 397);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Background";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// splitContainer3
			// 
			this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer3.Location = new System.Drawing.Point(0, 0);
			this.splitContainer3.Margin = new System.Windows.Forms.Padding(0);
			this.splitContainer3.Name = "splitContainer3";
			// 
			// splitContainer3.Panel1
			// 
			this.splitContainer3.Panel1.Controls.Add(this.tableLayoutPanel3);
			// 
			// splitContainer3.Panel2
			// 
			this.splitContainer3.Panel2.Controls.Add(this.tabControl3);
			this.splitContainer3.Size = new System.Drawing.Size(650, 397);
			this.splitContainer3.SplitterDistance = 384;
			this.splitContainer3.TabIndex = 4;
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.ColumnCount = 2;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel3.Controls.Add(this.bgToolStrip, 0, 0);
			this.tableLayoutPanel3.Controls.Add(this.backgroundPanel, 0, 1);
			this.tableLayoutPanel3.Controls.Add(this.vScrollBar3, 1, 1);
			this.tableLayoutPanel3.Controls.Add(this.hScrollBar3, 0, 2);
			this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 3;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel3.Size = new System.Drawing.Size(384, 397);
			this.tableLayoutPanel3.TabIndex = 3;
			// 
			// bgToolStrip
			// 
			this.tableLayoutPanel3.SetColumnSpan(this.bgToolStrip, 2);
			this.bgToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.bgToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bgDrawToolStripButton,
            this.bgSelectToolStripButton,
            toolStripSeparator15,
            this.replaceBackgroundToolStripButton,
            this.clearBackgroundToolStripButton});
			this.bgToolStrip.Location = new System.Drawing.Point(0, 0);
			this.bgToolStrip.Name = "bgToolStrip";
			this.bgToolStrip.Size = new System.Drawing.Size(384, 25);
			this.bgToolStrip.TabIndex = 5;
			this.bgToolStrip.Text = "toolStrip1";
			// 
			// bgDrawToolStripButton
			// 
			this.bgDrawToolStripButton.Checked = true;
			this.bgDrawToolStripButton.CheckState = System.Windows.Forms.CheckState.Checked;
			this.bgDrawToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("bgDrawToolStripButton.Image")));
			this.bgDrawToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.bgDrawToolStripButton.Name = "bgDrawToolStripButton";
			this.bgDrawToolStripButton.Size = new System.Drawing.Size(54, 22);
			this.bgDrawToolStripButton.Text = "Draw";
			this.bgDrawToolStripButton.Click += new System.EventHandler(this.bgDrawToolStripButton_Click);
			// 
			// bgSelectToolStripButton
			// 
			this.bgSelectToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("bgSelectToolStripButton.Image")));
			this.bgSelectToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.bgSelectToolStripButton.Name = "bgSelectToolStripButton";
			this.bgSelectToolStripButton.Size = new System.Drawing.Size(58, 22);
			this.bgSelectToolStripButton.Text = "Select";
			this.bgSelectToolStripButton.Click += new System.EventHandler(this.bgSelectToolStripButton_Click);
			// 
			// replaceBackgroundToolStripButton
			// 
			this.replaceBackgroundToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.replaceBackgroundToolStripButton.Enabled = false;
			this.replaceBackgroundToolStripButton.Name = "replaceBackgroundToolStripButton";
			this.replaceBackgroundToolStripButton.Size = new System.Drawing.Size(52, 22);
			this.replaceBackgroundToolStripButton.Text = "Replace";
			this.replaceBackgroundToolStripButton.Click += new System.EventHandler(this.replaceBackgroundToolStripButton_Click);
			// 
			// clearBackgroundToolStripButton
			// 
			this.clearBackgroundToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.clearBackgroundToolStripButton.Enabled = false;
			this.clearBackgroundToolStripButton.Name = "clearBackgroundToolStripButton";
			this.clearBackgroundToolStripButton.Size = new System.Drawing.Size(38, 22);
			this.clearBackgroundToolStripButton.Text = "Clear";
			this.clearBackgroundToolStripButton.Click += new System.EventHandler(this.clearBackgroundToolStripButton_Click);
			// 
			// backgroundPanel
			// 
			this.backgroundPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.backgroundPanel.Location = new System.Drawing.Point(0, 25);
			this.backgroundPanel.Margin = new System.Windows.Forms.Padding(0);
			this.backgroundPanel.Name = "backgroundPanel";
			this.backgroundPanel.Size = new System.Drawing.Size(367, 355);
			this.backgroundPanel.TabIndex = 1;
			this.backgroundPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.panel_Paint);
			this.backgroundPanel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.backgroundPanel_KeyDown);
			this.backgroundPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.backgroundPanel_MouseDown);
			this.backgroundPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.backgroundPanel_MouseMove);
			this.backgroundPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.backgroundPanel_MouseUp);
			this.backgroundPanel.Resize += new System.EventHandler(this.panel_Resize);
			// 
			// vScrollBar3
			// 
			this.vScrollBar3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.vScrollBar3.Enabled = false;
			this.vScrollBar3.LargeChange = 128;
			this.vScrollBar3.Location = new System.Drawing.Point(367, 25);
			this.vScrollBar3.Maximum = 128;
			this.vScrollBar3.Name = "vScrollBar3";
			this.vScrollBar3.Size = new System.Drawing.Size(17, 355);
			this.vScrollBar3.SmallChange = 16;
			this.vScrollBar3.TabIndex = 2;
			this.vScrollBar3.ValueChanged += new System.EventHandler(this.ScrollBar_ValueChanged);
			// 
			// hScrollBar3
			// 
			this.hScrollBar3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.hScrollBar3.Enabled = false;
			this.hScrollBar3.LargeChange = 128;
			this.hScrollBar3.Location = new System.Drawing.Point(0, 380);
			this.hScrollBar3.Maximum = 128;
			this.hScrollBar3.Name = "hScrollBar3";
			this.hScrollBar3.Size = new System.Drawing.Size(367, 17);
			this.hScrollBar3.SmallChange = 16;
			this.hScrollBar3.TabIndex = 3;
			this.hScrollBar3.ValueChanged += new System.EventHandler(this.ScrollBar_ValueChanged);
			// 
			// tabControl3
			// 
			this.tabControl3.Controls.Add(this.tabPage10);
			this.tabControl3.Controls.Add(this.tabPage11);
			this.tabControl3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl3.Location = new System.Drawing.Point(0, 0);
			this.tabControl3.Name = "tabControl3";
			this.tabControl3.SelectedIndex = 0;
			this.tabControl3.Size = new System.Drawing.Size(262, 397);
			this.tabControl3.TabIndex = 0;
			// 
			// tabPage10
			// 
			this.tabPage10.Location = new System.Drawing.Point(4, 22);
			this.tabPage10.Name = "tabPage10";
			this.tabPage10.Size = new System.Drawing.Size(254, 371);
			this.tabPage10.TabIndex = 0;
			this.tabPage10.Text = "Chunks";
			this.tabPage10.UseVisualStyleBackColor = true;
			// 
			// tabPage11
			// 
			this.tabPage11.Location = new System.Drawing.Point(4, 22);
			this.tabPage11.Name = "tabPage11";
			this.tabPage11.Size = new System.Drawing.Size(254, 371);
			this.tabPage11.TabIndex = 1;
			this.tabPage11.Text = "Layout Sections";
			this.tabPage11.UseVisualStyleBackColor = true;
			// 
			// tabPage4
			// 
			this.tabPage4.Controls.Add(this.tableLayoutPanel4);
			this.tabPage4.Location = new System.Drawing.Point(4, 22);
			this.tabPage4.Margin = new System.Windows.Forms.Padding(0);
			this.tabPage4.Name = "tabPage4";
			this.tabPage4.Size = new System.Drawing.Size(650, 397);
			this.tabPage4.TabIndex = 3;
			this.tabPage4.Text = "Art";
			this.tabPage4.UseVisualStyleBackColor = true;
			// 
			// tableLayoutPanel4
			// 
			this.tableLayoutPanel4.AutoScroll = true;
			this.tableLayoutPanel4.AutoSize = true;
			this.tableLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel4.ColumnCount = 4;
			this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel4.Controls.Add(tableLayoutPanel5, 0, 0);
			this.tableLayoutPanel4.Controls.Add(this.tabControl4, 3, 0);
			this.tableLayoutPanel4.Controls.Add(panel5, 1, 0);
			this.tableLayoutPanel4.Controls.Add(panel11, 2, 0);
			this.tableLayoutPanel4.Controls.Add(panel9, 2, 1);
			this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel4.Name = "tableLayoutPanel4";
			this.tableLayoutPanel4.RowCount = 2;
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel4.Size = new System.Drawing.Size(650, 397);
			this.tableLayoutPanel4.TabIndex = 1;
			// 
			// tabControl4
			// 
			this.tabControl4.Controls.Add(this.tabPage12);
			this.tabControl4.Controls.Add(this.tabPage13);
			this.tabControl4.Controls.Add(this.tabPage14);
			this.tabControl4.Controls.Add(this.tabPage5);
			this.tabControl4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl4.Location = new System.Drawing.Point(640, 0);
			this.tabControl4.Margin = new System.Windows.Forms.Padding(0);
			this.tabControl4.MinimumSize = new System.Drawing.Size(300, 300);
			this.tabControl4.Name = "tabControl4";
			this.tableLayoutPanel4.SetRowSpan(this.tabControl4, 2);
			this.tabControl4.SelectedIndex = 0;
			this.tabControl4.Size = new System.Drawing.Size(300, 494);
			this.tabControl4.TabIndex = 0;
			// 
			// tabPage12
			// 
			this.tabPage12.Controls.Add(this.panel10);
			this.tabPage12.Controls.Add(chunkListToolStrip);
			this.tabPage12.Location = new System.Drawing.Point(4, 22);
			this.tabPage12.Name = "tabPage12";
			this.tabPage12.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage12.Size = new System.Drawing.Size(292, 468);
			this.tabPage12.TabIndex = 0;
			this.tabPage12.Text = "Chunks";
			this.tabPage12.UseVisualStyleBackColor = true;
			// 
			// tabPage13
			// 
			this.tabPage13.Controls.Add(this.BlockSelector);
			this.tabPage13.Controls.Add(blockListToolStrip);
			this.tabPage13.Location = new System.Drawing.Point(4, 22);
			this.tabPage13.Name = "tabPage13";
			this.tabPage13.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage13.Size = new System.Drawing.Size(292, 468);
			this.tabPage13.TabIndex = 1;
			this.tabPage13.Text = "Blocks";
			this.tabPage13.UseVisualStyleBackColor = true;
			// 
			// tabPage14
			// 
			this.tabPage14.Controls.Add(this.TileSelector);
			this.tabPage14.Controls.Add(tileListToolStrip);
			this.tabPage14.Location = new System.Drawing.Point(4, 22);
			this.tabPage14.Name = "tabPage14";
			this.tabPage14.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage14.Size = new System.Drawing.Size(292, 468);
			this.tabPage14.TabIndex = 2;
			this.tabPage14.Text = "Tiles";
			this.tabPage14.UseVisualStyleBackColor = true;
			// 
			// tabPage5
			// 
			this.tabPage5.Controls.Add(this.CollisionSelector);
			this.tabPage5.Location = new System.Drawing.Point(4, 22);
			this.tabPage5.Name = "tabPage5";
			this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage5.Size = new System.Drawing.Size(292, 468);
			this.tabPage5.TabIndex = 3;
			this.tabPage5.Text = "Solids";
			this.tabPage5.UseVisualStyleBackColor = true;
			// 
			// CollisionSelector
			// 
			this.CollisionSelector.BackColor = System.Drawing.Color.Black;
			this.CollisionSelector.Dock = System.Windows.Forms.DockStyle.Fill;
			this.CollisionSelector.ImageHeight = 16;
			this.CollisionSelector.ImageSize = 16;
			this.CollisionSelector.ImageWidth = 16;
			this.CollisionSelector.Location = new System.Drawing.Point(3, 3);
			this.CollisionSelector.Margin = new System.Windows.Forms.Padding(0);
			this.CollisionSelector.Name = "CollisionSelector";
			this.CollisionSelector.ScrollValue = 0;
			this.CollisionSelector.SelectedIndex = -1;
			this.CollisionSelector.Size = new System.Drawing.Size(286, 462);
			this.CollisionSelector.TabIndex = 2;
			this.CollisionSelector.SelectedIndexChanged += new System.EventHandler(this.CollisionSelector_SelectedIndexChanged);
			this.CollisionSelector.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CollisionSelector_MouseDown);
			// 
			// tableLayoutPanel8
			// 
			this.tableLayoutPanel8.ColumnCount = 2;
			this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel8.Controls.Add(this.panel8, 1, 0);
			this.tableLayoutPanel8.Controls.Add(this.colorEditingPanel, 0, 0);
			this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Top;
			this.tableLayoutPanel8.Location = new System.Drawing.Point(0, 24);
			this.tableLayoutPanel8.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel8.Name = "tableLayoutPanel8";
			this.tableLayoutPanel8.RowCount = 1;
			this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel8.Size = new System.Drawing.Size(658, 80);
			this.tableLayoutPanel8.TabIndex = 4;
			// 
			// panel8
			// 
			this.panel8.AutoScroll = true;
			this.panel8.Controls.Add(this.PalettePanel);
			this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel8.Location = new System.Drawing.Point(106, 0);
			this.panel8.Margin = new System.Windows.Forms.Padding(0);
			this.panel8.Name = "panel8";
			this.panel8.Size = new System.Drawing.Size(552, 80);
			this.panel8.TabIndex = 3;
			// 
			// PalettePanel
			// 
			this.PalettePanel.Location = new System.Drawing.Point(0, 0);
			this.PalettePanel.Margin = new System.Windows.Forms.Padding(0);
			this.PalettePanel.Name = "PalettePanel";
			this.PalettePanel.Size = new System.Drawing.Size(320, 80);
			this.PalettePanel.TabIndex = 0;
			this.PalettePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.PalettePanel_Paint);
			this.PalettePanel.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.PalettePanel_MouseDoubleClick);
			this.PalettePanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PalettePanel_MouseDown);
			// 
			// colorEditingPanel
			// 
			this.colorEditingPanel.AutoSize = true;
			this.colorEditingPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.colorEditingPanel.Controls.Add(this.colorBlue);
			this.colorEditingPanel.Controls.Add(label4);
			this.colorEditingPanel.Controls.Add(this.colorGreen);
			this.colorEditingPanel.Controls.Add(label3);
			this.colorEditingPanel.Controls.Add(this.colorRed);
			this.colorEditingPanel.Controls.Add(label2);
			this.colorEditingPanel.Enabled = false;
			this.colorEditingPanel.Location = new System.Drawing.Point(0, 0);
			this.colorEditingPanel.Margin = new System.Windows.Forms.Padding(0);
			this.colorEditingPanel.Name = "colorEditingPanel";
			this.colorEditingPanel.Size = new System.Drawing.Size(106, 78);
			this.colorEditingPanel.TabIndex = 5;
			// 
			// colorBlue
			// 
			this.colorBlue.Location = new System.Drawing.Point(50, 55);
			this.colorBlue.Name = "colorBlue";
			this.colorBlue.Size = new System.Drawing.Size(53, 20);
			this.colorBlue.TabIndex = 5;
			this.colorBlue.ValueChanged += new System.EventHandler(this.color_ValueChanged);
			// 
			// colorGreen
			// 
			this.colorGreen.Location = new System.Drawing.Point(50, 29);
			this.colorGreen.Name = "colorGreen";
			this.colorGreen.Size = new System.Drawing.Size(53, 20);
			this.colorGreen.TabIndex = 3;
			this.colorGreen.ValueChanged += new System.EventHandler(this.color_ValueChanged);
			// 
			// colorRed
			// 
			this.colorRed.Location = new System.Drawing.Point(50, 3);
			this.colorRed.Name = "colorRed";
			this.colorRed.Size = new System.Drawing.Size(53, 20);
			this.colorRed.TabIndex = 1;
			this.colorRed.ValueChanged += new System.EventHandler(this.color_ValueChanged);
			// 
			// tileContextMenuStrip
			// 
			this.tileContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutTilesToolStripMenuItem,
            this.copyTilesToolStripMenuItem,
            this.deepCopyToolStripMenuItem,
            this.pasteBeforeToolStripMenuItem,
            this.pasteOverToolStripMenuItem,
            this.pasteAfterToolStripMenuItem,
            this.importOverToolStripMenuItem,
            this.duplicateTilesToolStripMenuItem,
            this.insertBeforeToolStripMenuItem,
            this.insertAfterToolStripMenuItem,
            this.deleteTilesToolStripMenuItem});
			this.tileContextMenuStrip.Name = "contextMenuStrip1";
			this.tileContextMenuStrip.Size = new System.Drawing.Size(148, 246);
			// 
			// cutTilesToolStripMenuItem
			// 
			this.cutTilesToolStripMenuItem.Image = global::SonicRetro.SonLVL.Properties.Resources.cut;
			this.cutTilesToolStripMenuItem.Name = "cutTilesToolStripMenuItem";
			this.cutTilesToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.cutTilesToolStripMenuItem.Text = "Cu&t";
			this.cutTilesToolStripMenuItem.Click += new System.EventHandler(this.cutTilesToolStripMenuItem_Click);
			// 
			// copyTilesToolStripMenuItem
			// 
			this.copyTilesToolStripMenuItem.Image = global::SonicRetro.SonLVL.Properties.Resources.copy;
			this.copyTilesToolStripMenuItem.Name = "copyTilesToolStripMenuItem";
			this.copyTilesToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.copyTilesToolStripMenuItem.Text = "&Copy";
			this.copyTilesToolStripMenuItem.Click += new System.EventHandler(this.copyTilesToolStripMenuItem_Click);
			// 
			// deepCopyToolStripMenuItem
			// 
			this.deepCopyToolStripMenuItem.Image = global::SonicRetro.SonLVL.Properties.Resources.copy;
			this.deepCopyToolStripMenuItem.Name = "deepCopyToolStripMenuItem";
			this.deepCopyToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.deepCopyToolStripMenuItem.Text = "Deep Co&py";
			this.deepCopyToolStripMenuItem.Click += new System.EventHandler(this.deepCopyToolStripMenuItem_Click);
			// 
			// pasteBeforeToolStripMenuItem
			// 
			this.pasteBeforeToolStripMenuItem.Image = global::SonicRetro.SonLVL.Properties.Resources.paste;
			this.pasteBeforeToolStripMenuItem.Name = "pasteBeforeToolStripMenuItem";
			this.pasteBeforeToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.pasteBeforeToolStripMenuItem.Text = "Paste &Before";
			this.pasteBeforeToolStripMenuItem.Click += new System.EventHandler(this.pasteBeforeToolStripMenuItem_Click);
			// 
			// pasteOverToolStripMenuItem
			// 
			this.pasteOverToolStripMenuItem.Image = global::SonicRetro.SonLVL.Properties.Resources.paste;
			this.pasteOverToolStripMenuItem.Name = "pasteOverToolStripMenuItem";
			this.pasteOverToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.pasteOverToolStripMenuItem.Text = "Paste &Over";
			this.pasteOverToolStripMenuItem.Click += new System.EventHandler(this.pasteOverToolStripMenuItem_Click);
			// 
			// pasteAfterToolStripMenuItem
			// 
			this.pasteAfterToolStripMenuItem.Image = global::SonicRetro.SonLVL.Properties.Resources.paste;
			this.pasteAfterToolStripMenuItem.Name = "pasteAfterToolStripMenuItem";
			this.pasteAfterToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.pasteAfterToolStripMenuItem.Text = "Paste &After";
			this.pasteAfterToolStripMenuItem.Click += new System.EventHandler(this.pasteAfterToolStripMenuItem_Click);
			// 
			// importOverToolStripMenuItem
			// 
			this.importOverToolStripMenuItem.Name = "importOverToolStripMenuItem";
			this.importOverToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.importOverToolStripMenuItem.Text = "&Import Over...";
			this.importOverToolStripMenuItem.Click += new System.EventHandler(this.importOverToolStripMenuItem_Click);
			// 
			// duplicateTilesToolStripMenuItem
			// 
			this.duplicateTilesToolStripMenuItem.Name = "duplicateTilesToolStripMenuItem";
			this.duplicateTilesToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.duplicateTilesToolStripMenuItem.Text = "D&uplicate";
			this.duplicateTilesToolStripMenuItem.Click += new System.EventHandler(this.duplicateTilesToolStripMenuItem_Click);
			// 
			// insertBeforeToolStripMenuItem
			// 
			this.insertBeforeToolStripMenuItem.Name = "insertBeforeToolStripMenuItem";
			this.insertBeforeToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.insertBeforeToolStripMenuItem.Text = "Insert B&efore";
			this.insertBeforeToolStripMenuItem.Click += new System.EventHandler(this.insertBeforeToolStripMenuItem_Click);
			// 
			// insertAfterToolStripMenuItem
			// 
			this.insertAfterToolStripMenuItem.Name = "insertAfterToolStripMenuItem";
			this.insertAfterToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.insertAfterToolStripMenuItem.Text = "Insert A&fter";
			this.insertAfterToolStripMenuItem.Click += new System.EventHandler(this.insertAfterToolStripMenuItem_Click);
			// 
			// deleteTilesToolStripMenuItem
			// 
			this.deleteTilesToolStripMenuItem.Image = global::SonicRetro.SonLVL.Properties.Resources.delete;
			this.deleteTilesToolStripMenuItem.Name = "deleteTilesToolStripMenuItem";
			this.deleteTilesToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
			this.deleteTilesToolStripMenuItem.Text = "&Delete";
			this.deleteTilesToolStripMenuItem.Click += new System.EventHandler(this.deleteTilesToolStripMenuItem_Click);
			// 
			// paletteContextMenuStrip
			// 
			this.paletteContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem1});
			this.paletteContextMenuStrip.Name = "contextMenuStrip2";
			this.paletteContextMenuStrip.Size = new System.Drawing.Size(111, 26);
			// 
			// importToolStripMenuItem1
			// 
			this.importToolStripMenuItem1.Name = "importToolStripMenuItem1";
			this.importToolStripMenuItem1.Size = new System.Drawing.Size(110, 22);
			this.importToolStripMenuItem1.Text = "&Import";
			this.importToolStripMenuItem1.Click += new System.EventHandler(this.importToolStripMenuItem1_Click);
			// 
			// layoutContextMenuStrip
			// 
			this.layoutContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripMenuItem1,
            this.copyToolStripMenuItem1,
            this.pasteOnceToolStripMenuItem,
            this.pasteRepeatingToolStripMenuItem,
            this.importToolStripMenuItem2,
            this.toolStripSeparator11,
            this.deleteToolStripMenuItem1,
            this.fillToolStripMenuItem,
            this.toolStripSeparator12,
            this.saveSectionToolStripMenuItem,
            this.pasteSectionOnceToolStripMenuItem,
            this.pasteSectionRepeatingToolStripMenuItem,
            this.toolStripSeparator7,
            this.insertLayoutToolStripMenuItem,
            this.deleteLayoutToolStripMenuItem});
			this.layoutContextMenuStrip.Name = "layoutContextMenuStrip";
			this.layoutContextMenuStrip.Size = new System.Drawing.Size(203, 286);
			// 
			// cutToolStripMenuItem1
			// 
			this.cutToolStripMenuItem1.Image = global::SonicRetro.SonLVL.Properties.Resources.cut;
			this.cutToolStripMenuItem1.Name = "cutToolStripMenuItem1";
			this.cutToolStripMenuItem1.Size = new System.Drawing.Size(202, 22);
			this.cutToolStripMenuItem1.Text = "Cu&t";
			this.cutToolStripMenuItem1.Click += new System.EventHandler(this.cutToolStripMenuItem1_Click);
			// 
			// copyToolStripMenuItem1
			// 
			this.copyToolStripMenuItem1.Image = global::SonicRetro.SonLVL.Properties.Resources.copy;
			this.copyToolStripMenuItem1.Name = "copyToolStripMenuItem1";
			this.copyToolStripMenuItem1.Size = new System.Drawing.Size(202, 22);
			this.copyToolStripMenuItem1.Text = "&Copy";
			this.copyToolStripMenuItem1.Click += new System.EventHandler(this.copyToolStripMenuItem1_Click);
			// 
			// pasteOnceToolStripMenuItem
			// 
			this.pasteOnceToolStripMenuItem.Image = global::SonicRetro.SonLVL.Properties.Resources.paste;
			this.pasteOnceToolStripMenuItem.Name = "pasteOnceToolStripMenuItem";
			this.pasteOnceToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
			this.pasteOnceToolStripMenuItem.Text = "&Paste Once";
			this.pasteOnceToolStripMenuItem.Click += new System.EventHandler(this.pasteOnceToolStripMenuItem_Click);
			// 
			// pasteRepeatingToolStripMenuItem
			// 
			this.pasteRepeatingToolStripMenuItem.Image = global::SonicRetro.SonLVL.Properties.Resources.paste;
			this.pasteRepeatingToolStripMenuItem.Name = "pasteRepeatingToolStripMenuItem";
			this.pasteRepeatingToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
			this.pasteRepeatingToolStripMenuItem.Text = "Paste &Repeating";
			this.pasteRepeatingToolStripMenuItem.Click += new System.EventHandler(this.pasteRepeatingToolStripMenuItem_Click);
			// 
			// importToolStripMenuItem2
			// 
			this.importToolStripMenuItem2.Name = "importToolStripMenuItem2";
			this.importToolStripMenuItem2.Size = new System.Drawing.Size(202, 22);
			this.importToolStripMenuItem2.Text = "I&mport...";
			this.importToolStripMenuItem2.Click += new System.EventHandler(this.importToolStripMenuItem2_Click);
			// 
			// toolStripSeparator11
			// 
			this.toolStripSeparator11.Name = "toolStripSeparator11";
			this.toolStripSeparator11.Size = new System.Drawing.Size(199, 6);
			// 
			// deleteToolStripMenuItem1
			// 
			this.deleteToolStripMenuItem1.Image = global::SonicRetro.SonLVL.Properties.Resources.delete;
			this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
			this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(202, 22);
			this.deleteToolStripMenuItem1.Text = "C&lear";
			this.deleteToolStripMenuItem1.Click += new System.EventHandler(this.deleteToolStripMenuItem1_Click);
			// 
			// fillToolStripMenuItem
			// 
			this.fillToolStripMenuItem.Name = "fillToolStripMenuItem";
			this.fillToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
			this.fillToolStripMenuItem.Text = "&Fill With Selected Chunk";
			this.fillToolStripMenuItem.Click += new System.EventHandler(this.fillToolStripMenuItem_Click);
			// 
			// toolStripSeparator12
			// 
			this.toolStripSeparator12.Name = "toolStripSeparator12";
			this.toolStripSeparator12.Size = new System.Drawing.Size(199, 6);
			// 
			// saveSectionToolStripMenuItem
			// 
			this.saveSectionToolStripMenuItem.Name = "saveSectionToolStripMenuItem";
			this.saveSectionToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
			this.saveSectionToolStripMenuItem.Text = "&Save Section...";
			this.saveSectionToolStripMenuItem.Click += new System.EventHandler(this.saveSectionToolStripMenuItem_Click);
			// 
			// pasteSectionOnceToolStripMenuItem
			// 
			this.pasteSectionOnceToolStripMenuItem.Image = global::SonicRetro.SonLVL.Properties.Resources.paste;
			this.pasteSectionOnceToolStripMenuItem.Name = "pasteSectionOnceToolStripMenuItem";
			this.pasteSectionOnceToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
			this.pasteSectionOnceToolStripMenuItem.Text = "P&aste Section Once";
			this.pasteSectionOnceToolStripMenuItem.Click += new System.EventHandler(this.pasteSectionOnceToolStripMenuItem_Click);
			// 
			// pasteSectionRepeatingToolStripMenuItem
			// 
			this.pasteSectionRepeatingToolStripMenuItem.Image = global::SonicRetro.SonLVL.Properties.Resources.paste;
			this.pasteSectionRepeatingToolStripMenuItem.Name = "pasteSectionRepeatingToolStripMenuItem";
			this.pasteSectionRepeatingToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
			this.pasteSectionRepeatingToolStripMenuItem.Text = "Paste Section R&epeating";
			this.pasteSectionRepeatingToolStripMenuItem.Click += new System.EventHandler(this.pasteSectionRepeatingToolStripMenuItem_Click);
			// 
			// toolStripSeparator7
			// 
			this.toolStripSeparator7.Name = "toolStripSeparator7";
			this.toolStripSeparator7.Size = new System.Drawing.Size(199, 6);
			// 
			// insertLayoutToolStripMenuItem
			// 
			this.insertLayoutToolStripMenuItem.Name = "insertLayoutToolStripMenuItem";
			this.insertLayoutToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
			this.insertLayoutToolStripMenuItem.Text = "&Insert...";
			this.insertLayoutToolStripMenuItem.Click += new System.EventHandler(this.insertLayoutToolStripMenuItem_Click);
			// 
			// deleteLayoutToolStripMenuItem
			// 
			this.deleteLayoutToolStripMenuItem.Name = "deleteLayoutToolStripMenuItem";
			this.deleteLayoutToolStripMenuItem.Size = new System.Drawing.Size(202, 22);
			this.deleteLayoutToolStripMenuItem.Text = "&Delete...";
			this.deleteLayoutToolStripMenuItem.Click += new System.EventHandler(this.deleteLayoutToolStripMenuItem_Click);
			// 
			// solidsContextMenuStrip
			// 
			this.solidsContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copySolidsToolStripMenuItem,
            this.pasteSolidsToolStripMenuItem,
            this.clearSolidsToolStripMenuItem});
			this.solidsContextMenuStrip.Name = "solidsContextMenuStrip";
			this.solidsContextMenuStrip.Size = new System.Drawing.Size(103, 70);
			// 
			// copySolidsToolStripMenuItem
			// 
			this.copySolidsToolStripMenuItem.Image = global::SonicRetro.SonLVL.Properties.Resources.copy;
			this.copySolidsToolStripMenuItem.Name = "copySolidsToolStripMenuItem";
			this.copySolidsToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
			this.copySolidsToolStripMenuItem.Text = "&Copy";
			this.copySolidsToolStripMenuItem.Click += new System.EventHandler(this.copySolidsToolStripMenuItem_Click);
			// 
			// pasteSolidsToolStripMenuItem
			// 
			this.pasteSolidsToolStripMenuItem.Image = global::SonicRetro.SonLVL.Properties.Resources.paste;
			this.pasteSolidsToolStripMenuItem.Name = "pasteSolidsToolStripMenuItem";
			this.pasteSolidsToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
			this.pasteSolidsToolStripMenuItem.Text = "&Paste";
			this.pasteSolidsToolStripMenuItem.Click += new System.EventHandler(this.pasteSolidsToolStripMenuItem_Click);
			// 
			// clearSolidsToolStripMenuItem
			// 
			this.clearSolidsToolStripMenuItem.Image = global::SonicRetro.SonLVL.Properties.Resources.delete;
			this.clearSolidsToolStripMenuItem.Name = "clearSolidsToolStripMenuItem";
			this.clearSolidsToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
			this.clearSolidsToolStripMenuItem.Text = "&Clear";
			this.clearSolidsToolStripMenuItem.Click += new System.EventHandler(this.clearSolidsToolStripMenuItem_Click);
			// 
			// loadingAnimation1
			// 
			this.loadingAnimation1.AutoSize = true;
			this.loadingAnimation1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.loadingAnimation1.Location = new System.Drawing.Point(0, 0);
			this.loadingAnimation1.Name = "loadingAnimation1";
			this.loadingAnimation1.Size = new System.Drawing.Size(165, 38);
			this.loadingAnimation1.TabIndex = 4;
			this.loadingAnimation1.UseWaitCursor = true;
			this.loadingAnimation1.Visible = false;
			this.loadingAnimation1.SizeChanged += new System.EventHandler(this.loadingAnimation1_SizeChanged);
			// 
			// importProgressControl1
			// 
			this.importProgressControl1.AutoSize = true;
			this.importProgressControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.importProgressControl1.CurrentProgress = 0;
			this.importProgressControl1.Location = new System.Drawing.Point(0, 0);
			this.importProgressControl1.MaximumProgress = 100;
			this.importProgressControl1.Name = "importProgressControl1";
			this.importProgressControl1.Size = new System.Drawing.Size(183, 56);
			this.importProgressControl1.TabIndex = 5;
			this.importProgressControl1.UseWaitCursor = true;
			this.importProgressControl1.Visible = false;
			this.importProgressControl1.SizeChanged += new System.EventHandler(this.importProgressControl1_SizeChanged);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(658, 527);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.tableLayoutPanel8);
			this.Controls.Add(this.menuStrip1);
			this.Controls.Add(this.loadingAnimation1);
			this.Controls.Add(this.importProgressControl1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.KeyPreview = true;
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainForm";
			this.Text = "SonLVL";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
			chunkListToolStrip.ResumeLayout(false);
			chunkListToolStrip.PerformLayout();
			blockListToolStrip.ResumeLayout(false);
			blockListToolStrip.PerformLayout();
			tileListToolStrip.ResumeLayout(false);
			tileListToolStrip.PerformLayout();
			tabControl2.ResumeLayout(false);
			this.tabPage8.ResumeLayout(false);
			this.tabPage9.ResumeLayout(false);
			this.layoutSectionSplitContainer.Panel1.ResumeLayout(false);
			this.layoutSectionSplitContainer.Panel1.PerformLayout();
			this.layoutSectionSplitContainer.Panel2.ResumeLayout(false);
			this.layoutSectionSplitContainer.ResumeLayout(false);
			layoutSectionListToolStrip.ResumeLayout(false);
			layoutSectionListToolStrip.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutSectionPreview)).EndInit();
			panel11.ResumeLayout(false);
			panel11.PerformLayout();
			tableLayoutPanel5.ResumeLayout(false);
			tableLayoutPanel5.PerformLayout();
			panel1.ResumeLayout(false);
			panel1.PerformLayout();
			panel5.ResumeLayout(false);
			panel5.PerformLayout();
			this.ColIndBox.ResumeLayout(false);
			this.ColIndBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.BlockCollision2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.BlockCollision1)).EndInit();
			panel9.ResumeLayout(false);
			panel9.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.ColAngle)).EndInit();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.objectContextMenuStrip.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer4.Panel1.ResumeLayout(false);
			this.splitContainer4.Panel2.ResumeLayout(false);
			this.splitContainer4.Panel2.PerformLayout();
			this.splitContainer4.ResumeLayout(false);
			this.objToolStrip.ResumeLayout(false);
			this.objToolStrip.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.fgToolStrip.ResumeLayout(false);
			this.fgToolStrip.PerformLayout();
			this.tabPage3.ResumeLayout(false);
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel2.ResumeLayout(false);
			this.splitContainer3.ResumeLayout(false);
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			this.bgToolStrip.ResumeLayout(false);
			this.bgToolStrip.PerformLayout();
			this.tabControl3.ResumeLayout(false);
			this.tabPage4.ResumeLayout(false);
			this.tabPage4.PerformLayout();
			this.tableLayoutPanel4.ResumeLayout(false);
			this.tableLayoutPanel4.PerformLayout();
			this.tabControl4.ResumeLayout(false);
			this.tabPage12.ResumeLayout(false);
			this.tabPage12.PerformLayout();
			this.tabPage13.ResumeLayout(false);
			this.tabPage13.PerformLayout();
			this.tabPage14.ResumeLayout(false);
			this.tabPage14.PerformLayout();
			this.tabPage5.ResumeLayout(false);
			this.tableLayoutPanel8.ResumeLayout(false);
			this.tableLayoutPanel8.PerformLayout();
			this.panel8.ResumeLayout(false);
			this.colorEditingPanel.ResumeLayout(false);
			this.colorEditingPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.colorBlue)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.colorGreen)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.colorRed)).EndInit();
			this.tileContextMenuStrip.ResumeLayout(false);
			this.paletteContextMenuStrip.ResumeLayout(false);
			this.layoutContextMenuStrip.ResumeLayout(false);
			this.solidsContextMenuStrip.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem changeLevelToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem noneToolStripMenuItem;
		private System.ComponentModel.BackgroundWorker backgroundLevelLoader;
		private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem tilesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem blocksToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem chunksToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem foregroundToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem backgroundToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem transparentBackFGBGToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem paletteLine0ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem paletteLine1ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem paletteLine2ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem paletteLine3ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem includeobjectsWithFGToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem objectsAboveHighPlaneToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem undoCtrlZToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem redoCtrlYToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip objectContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addObjectToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addRingToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem paletteToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		internal System.Windows.Forms.ToolStripMenuItem logToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem blendAlternatePaletteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem collisionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem noneToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem path1ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem path2ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem paletteToolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem hideDebugObjectsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addGroupOfObjectsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addGroupOfRingsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem buildAndRunToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem timeZoneToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem currentOnlyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem allToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem hUDToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem layersToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem lowToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem highToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem setupEmulatorToolStripMenuItem;
		private System.Windows.Forms.HScrollBar hScrollBar1;
		private System.Windows.Forms.VScrollBar vScrollBar1;
		private SonicRetro.SonLVL.API.KeyboardPanel objectPanel;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private SonicRetro.SonLVL.API.KeyboardPanel foregroundPanel;
		private System.Windows.Forms.VScrollBar vScrollBar2;
		private System.Windows.Forms.HScrollBar hScrollBar2;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private SonicRetro.SonLVL.API.KeyboardPanel backgroundPanel;
		private System.Windows.Forms.VScrollBar vScrollBar3;
		private System.Windows.Forms.HScrollBar hScrollBar3;
		private System.Windows.Forms.TabPage tabPage4;
		private System.Windows.Forms.PropertyGrid ObjectProperties;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private SonicRetro.SonLVL.API.TileList ChunkSelector;
		private System.Windows.Forms.SplitContainer splitContainer3;
		private System.Windows.Forms.ToolStripMenuItem recentProjectsToolStripMenuItem;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
		private System.Windows.Forms.Label ChunkCount;
		private System.Windows.Forms.TextBox ChunkID;
		private ChunkBlockEditor chunkBlockEditor;
		private SonicRetro.SonLVL.API.TileList BlockSelector;
		private System.Windows.Forms.Label BlockCount;
		private System.Windows.Forms.TextBox BlockID;
		private PatternIndexEditor blockTileEditor;
		private System.Windows.Forms.GroupBox ColIndBox;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.NumericUpDown BlockCollision2;
		private System.Windows.Forms.NumericUpDown BlockCollision1;
		private SonicRetro.SonLVL.API.TileList TileSelector;
		private System.Windows.Forms.Button rotateTileRightButton;
		private System.Windows.Forms.Label TileCount;
		private System.Windows.Forms.TextBox TileID;
		private System.Windows.Forms.Panel panel8;
		private System.Windows.Forms.Panel PalettePanel;
		private System.Windows.Forms.TextBox ColID;
		private System.Windows.Forms.NumericUpDown ColAngle;
		private System.Windows.Forms.ContextMenuStrip tileContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem cutTilesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyTilesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteBeforeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteAfterToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem insertBeforeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem insertAfterToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteTilesToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip paletteContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem1;
		private System.Windows.Forms.Panel panel10;
		private System.Windows.Forms.ToolStripMenuItem gridToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem enableGridToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem gridColorToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem zoomToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem viewReadmeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem reportBugToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem xToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem xToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem xToolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem xToolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem xToolStripMenuItem4;
		private System.Windows.Forms.ToolStripMenuItem xToolStripMenuItem5;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripMenuItem selectAllObjectsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem selectAllRingsToolStripMenuItem;
		private System.Windows.Forms.ToolStrip fgToolStrip;
		private System.Windows.Forms.ToolStripButton fgSelectToolStripButton;
		private System.Windows.Forms.ToolStripButton fgDrawToolStripButton;
		private System.Windows.Forms.ContextMenuStrip layoutContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem pasteOnceToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem1;
		private System.Windows.Forms.ToolStrip bgToolStrip;
		private System.Windows.Forms.ToolStripButton bgDrawToolStripButton;
		private System.Windows.Forms.ToolStripButton bgSelectToolStripButton;
		private System.Windows.Forms.ToolStripMenuItem noneToolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem resizeLevelToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private System.Windows.Forms.ToolStripMenuItem anglesToolStripMenuItem;
		private System.Windows.Forms.SplitContainer splitContainer4;
		private System.Windows.Forms.ListView objectTypeList;
		private System.Windows.Forms.ImageList objectTypeImages;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
		private System.Windows.Forms.ToolStripMenuItem insertLayoutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteLayoutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem includeObjectsWithForegroundSelectionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteRepeatingToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fillToolStripMenuItem;
		private LoadingAnimation loadingAnimation1;
		private System.Windows.Forms.ToolStrip objToolStrip;
		private System.Windows.Forms.ToolStripButton alignGroundToolStripButton;
		private System.Windows.Forms.ToolStripButton alignLeftWallToolStripButton;
		private System.Windows.Forms.ToolStripButton alignRightWallToolStripButton;
		private System.Windows.Forms.ToolStripButton alignCeilingToolStripButton;
		private System.Windows.Forms.ToolStripButton alignLeftsToolStripButton;
		private System.Windows.Forms.ToolStripButton alignCentersToolStripButton;
		private System.Windows.Forms.ToolStripButton alignRightsToolStripButton;
		private System.Windows.Forms.ToolStripButton alignTopsToolStripButton;
		private System.Windows.Forms.ToolStripButton alignMiddlesToolStripButton;
		private System.Windows.Forms.ToolStripButton alignBottomsToolStripButton;
		private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem findNextToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem findPreviousToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem useHexadecimalIndexesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem solidityMapsToolStripMenuItem;
		private System.Windows.Forms.Button flipChunkHButton;
		private System.Windows.Forms.Button flipChunkVButton;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
		private System.Windows.Forms.Panel colorEditingPanel;
		private System.Windows.Forms.NumericUpDown colorBlue;
		private System.Windows.Forms.NumericUpDown colorGreen;
		private System.Windows.Forms.NumericUpDown colorRed;
		private System.Windows.Forms.ToolStripDropDownButton objGridSizeDropDownButton;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem8;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem9;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem10;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem11;
		private System.Windows.Forms.ToolStripMenuItem xToolStripMenuItem7;
		private System.Windows.Forms.ToolStripMenuItem xToolStripMenuItem6;
		private System.Windows.Forms.ToolStripButton remapChunksButton;
		private System.Windows.Forms.ToolStripButton remapBlocksButton;
		private System.Windows.Forms.ToolStripButton remapTilesButton;
		private System.Windows.Forms.TabPage tabPage8;
		private System.Windows.Forms.SplitContainer layoutSectionSplitContainer;
		private System.Windows.Forms.PictureBox layoutSectionPreview;
		private System.Windows.Forms.ListBox layoutSectionListBox;
		private System.Windows.Forms.TabControl tabControl3;
		private System.Windows.Forms.TabPage tabPage10;
		private System.Windows.Forms.TabPage tabPage11;
		private System.Windows.Forms.TabPage tabPage9;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
		private System.Windows.Forms.ToolStripMenuItem saveSectionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteSectionOnceToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteSectionRepeatingToolStripMenuItem;
		private System.Windows.Forms.ToolStripButton enableDraggingChunksButton;
		private System.Windows.Forms.ToolStripButton enableDraggingBlocksButton;
		private System.Windows.Forms.ToolStripButton enableDraggingTilesButton;
		private System.Windows.Forms.ToolStripMenuItem deepCopyToolStripMenuItem;
		private System.Windows.Forms.Button flipBlockVButton;
		private System.Windows.Forms.Button flipBlockHButton;
		private System.Windows.Forms.Button flipTileVButton;
		private System.Windows.Forms.Button flipTileHButton;
		private System.Windows.Forms.CheckBox showBlockBehindCollisionCheckBox;
		private System.Windows.Forms.ToolStripMenuItem pasteOverToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pNGToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem yYCHRToolStripMenuItem;
		private System.Windows.Forms.ToolStripButton importToolStripButton;
		private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem2;
		private System.Windows.Forms.ToolStripButton importChunksToolStripButton;
		private System.Windows.Forms.ToolStripButton importBlocksToolStripButton;
		private System.Windows.Forms.ToolStripButton importTilesToolStripButton;
		private System.Windows.Forms.ToolStripMenuItem waterPaletteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem setPositionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem selectPaletteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem switchMouseButtonsInChunkAndBlockEditorsToolStripMenuItem;
		private System.Windows.Forms.Label chunkCtrlLabel;
		private System.Windows.Forms.ToolStripMenuItem exportArtcollisionpriorityToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem jASCPALToolStripMenuItem;
		private ImportProgressControl importProgressControl1;
		private System.Windows.Forms.ToolStripButton deleteUnusedTilesToolStripButton;
		private System.Windows.Forms.ToolStripButton deleteUnusedBlocksToolStripButton;
		private System.Windows.Forms.ToolStripButton deleteUnusedChunksToolStripButton;
		private System.Windows.Forms.ToolStripButton clearForegroundToolStripButton;
		private System.Windows.Forms.ToolStripButton clearBackgroundToolStripButton;
		private System.Windows.Forms.Button calculateAngleButton;
		private System.Windows.Forms.ContextMenuStrip solidsContextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem copySolidsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteSolidsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem clearSolidsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem duplicateTilesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem usageCountsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem12;
		private System.Windows.Forms.ToolStripButton replaceForegroundToolStripButton;
		private System.Windows.Forms.ToolStripButton replaceBackgroundToolStripButton;
		private System.Windows.Forms.ToolStripButton replaceChunkBlocksToolStripButton;
		private System.Windows.Forms.ToolStripButton replaceBlockTilesToolStripButton;
		private System.Windows.Forms.ToolStripButton removeDuplicateChunksToolStripButton;
		private System.Windows.Forms.ToolStripButton removeDuplicateBlocksToolStripButton;
		private System.Windows.Forms.ToolStripButton removeDuplicateTilesToolStripButton;
		private System.Windows.Forms.ToolStripButton drawChunkToolStripButton;
		private System.Windows.Forms.ToolStripButton drawBlockToolStripButton;
		private System.Windows.Forms.ToolStripButton drawTileToolStripButton;
		private System.Windows.Forms.ToolStripMenuItem importOverToolStripMenuItem;
		private System.Windows.Forms.TabControl tabControl4;
		private System.Windows.Forms.TabPage tabPage12;
		private System.Windows.Forms.TabPage tabPage13;
		private System.Windows.Forms.TabPage tabPage14;
		private System.Windows.Forms.TabPage tabPage5;
		private API.TileList CollisionSelector;
		private API.KeyboardPanel ChunkPicture;
		private API.KeyboardPanel BlockPicture;
		private System.Windows.Forms.Panel TilePicture;
		private System.Windows.Forms.Panel ColPicture;

	}
}
