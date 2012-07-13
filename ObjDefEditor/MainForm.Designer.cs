namespace ObjDefEditor
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
            System.Windows.Forms.Label label6;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label24;
            System.Windows.Forms.Label label23;
            System.Windows.Forms.Label label22;
            System.Windows.Forms.Label label21;
            System.Windows.Forms.Label label18;
            System.Windows.Forms.Label label19;
            System.Windows.Forms.Label label20;
            System.Windows.Forms.Label label17;
            System.Windows.Forms.Label label16;
            System.Windows.Forms.Label label15;
            System.Windows.Forms.Label label14;
            System.Windows.Forms.Label label13;
            System.Windows.Forms.Label label12;
            System.Windows.Forms.Label label11;
            System.Windows.Forms.Label label10;
            System.Windows.Forms.Label label9;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label28;
            System.Windows.Forms.Label label27;
            System.Windows.Forms.Label label26;
            System.Windows.Forms.Label label25;
            System.Windows.Forms.Label label7;
            System.Windows.Forms.Label label8;
            System.Windows.Forms.Label label29;
            System.Windows.Forms.Label label30;
            System.Windows.Forms.Label label31;
            System.Windows.Forms.Label label32;
            System.Windows.Forms.Label label33;
            System.Windows.Forms.Label label34;
            System.Windows.Forms.Label label35;
            System.Windows.Forms.Label label36;
            System.Windows.Forms.Label label37;
            System.Windows.Forms.Label label38;
            System.Windows.Forms.Label label39;
            System.Windows.Forms.Label label40;
            System.Windows.Forms.Label label41;
            System.Windows.Forms.Label label42;
            System.Windows.Forms.Label label43;
            System.Windows.Forms.Label label44;
            System.Windows.Forms.Label label45;
            System.Windows.Forms.Label label46;
            System.Windows.Forms.Label label47;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeLevelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noneToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.recentProjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editorTabs = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.defImagePreview = new System.Windows.Forms.PictureBox();
            this.defDebug = new System.Windows.Forms.CheckBox();
            this.defRemember = new System.Windows.Forms.CheckBox();
            this.defImage = new System.Windows.Forms.ComboBox();
            this.defLanguage = new System.Windows.Forms.ComboBox();
            this.defType = new System.Windows.Forms.TextBox();
            this.defNamespace = new System.Windows.Forms.TextBox();
            this.defName = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.selectedImagePreview = new System.Windows.Forms.PictureBox();
            this.imageControls = new System.Windows.Forms.Panel();
            this.mappingsImageControls = new System.Windows.Forms.Panel();
            this.mappingsPalette = new System.Windows.Forms.NumericUpDown();
            this.mappingsFrame = new System.Windows.Forms.NumericUpDown();
            this.dplcFormat = new System.Windows.Forms.ComboBox();
            this.dplcLabel = new System.Windows.Forms.TextBox();
            this.dplcFile = new ObjDefEditor.FileSelector();
            this.mappingsFormat = new System.Windows.Forms.ComboBox();
            this.mappingsLabel = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.mappingsASM = new System.Windows.Forms.RadioButton();
            this.mappingsBinary = new System.Windows.Forms.RadioButton();
            this.artCompression = new System.Windows.Forms.ComboBox();
            this.useLevelArt = new System.Windows.Forms.CheckBox();
            this.mappingsFile = new ObjDefEditor.FileSelector();
            this.artFilename = new ObjDefEditor.FileSelector();
            this.defaultArtOffset = new System.Windows.Forms.CheckBox();
            this.artOffset = new System.Windows.Forms.NumericUpDown();
            this.artList = new System.Windows.Forms.ListBox();
            this.artDownButton = new System.Windows.Forms.Button();
            this.artAddButton = new System.Windows.Forms.Button();
            this.artUpButton = new System.Windows.Forms.Button();
            this.artRemoveButton = new System.Windows.Forms.Button();
            this.spriteImageControls = new System.Windows.Forms.Panel();
            this.spriteNum = new System.Windows.Forms.NumericUpDown();
            this.bitmapImageControls = new System.Windows.Forms.Panel();
            this.bitmapFilename = new ObjDefEditor.FileSelector();
            this.imageType = new System.Windows.Forms.ComboBox();
            this.imageOffsetY = new System.Windows.Forms.NumericUpDown();
            this.imageOffsetX = new System.Windows.Forms.NumericUpDown();
            this.imageName = new System.Windows.Forms.TextBox();
            this.deleteImageButton = new System.Windows.Forms.Button();
            this.addImageButton = new System.Windows.Forms.Button();
            this.selectedImage = new System.Windows.Forms.ComboBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.subtypeControls = new System.Windows.Forms.Panel();
            this.subtypeImagePreview = new System.Windows.Forms.PictureBox();
            this.subtypeImage = new System.Windows.Forms.ComboBox();
            this.subtypeName = new System.Windows.Forms.TextBox();
            this.subtypeID = new System.Windows.Forms.NumericUpDown();
            this.deleteSubtypeButton = new System.Windows.Forms.Button();
            this.addSubtypeButton = new System.Windows.Forms.Button();
            this.selectedSubtype = new System.Windows.Forms.ComboBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.enumControls = new System.Windows.Forms.Panel();
            this.enumMemberName = new System.Windows.Forms.TextBox();
            this.enumName = new System.Windows.Forms.TextBox();
            this.enumMemberDefault = new System.Windows.Forms.CheckBox();
            this.enumMemberValue = new System.Windows.Forms.NumericUpDown();
            this.enumMemberList = new System.Windows.Forms.ListBox();
            this.enumMemberRemoveButton = new System.Windows.Forms.Button();
            this.enumMemberUpButton = new System.Windows.Forms.Button();
            this.enumMemberDownButton = new System.Windows.Forms.Button();
            this.enumMemberAddButton = new System.Windows.Forms.Button();
            this.deleteEnumButton = new System.Windows.Forms.Button();
            this.addEnumButton = new System.Windows.Forms.Button();
            this.selectedEnum = new System.Windows.Forms.ComboBox();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.propertyControls = new System.Windows.Forms.Panel();
            this.customPropertyControls = new System.Windows.Forms.Panel();
            this.overrideDefaultProperty = new System.Windows.Forms.CheckBox();
            this.setMethod = new System.Windows.Forms.TextBox();
            this.getMethod = new System.Windows.Forms.TextBox();
            this.bitsPropertyControls = new System.Windows.Forms.Panel();
            this.bitLength = new System.Windows.Forms.NumericUpDown();
            this.startBit = new System.Windows.Forms.NumericUpDown();
            this.propertyType = new System.Windows.Forms.ComboBox();
            this.propertyDescription = new System.Windows.Forms.TextBox();
            this.propertyValueType = new System.Windows.Forms.ComboBox();
            this.propertyName = new System.Windows.Forms.TextBox();
            this.deletePropertyButton = new System.Windows.Forms.Button();
            this.addPropertyButton = new System.Windows.Forms.Button();
            this.selectedProperty = new System.Windows.Forms.ComboBox();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.deleteDisplayOptionButton = new System.Windows.Forms.Button();
            this.addDisplayOptionButton = new System.Windows.Forms.Button();
            this.displayPreview = new System.Windows.Forms.PictureBox();
            this.displayOptionControls = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.removeImageRefButton = new System.Windows.Forms.Button();
            this.addImageRefButton = new System.Windows.Forms.Button();
            this.selectedImageRef = new System.Windows.Forms.ComboBox();
            this.imageRefControls = new System.Windows.Forms.Panel();
            this.imageRefYFlip = new System.Windows.Forms.ComboBox();
            this.imageRefXFlip = new System.Windows.Forms.ComboBox();
            this.imageRefImage = new System.Windows.Forms.ComboBox();
            this.imageRefOffsetY = new System.Windows.Forms.NumericUpDown();
            this.imageRefOffsetX = new System.Windows.Forms.NumericUpDown();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.conditionValueNum = new System.Windows.Forms.NumericUpDown();
            this.conditionValueEnum = new System.Windows.Forms.ComboBox();
            this.conditionProperty = new System.Windows.Forms.ComboBox();
            this.removeConditionButton = new System.Windows.Forms.Button();
            this.addConditionButton = new System.Windows.Forms.Button();
            this.selectedCondition = new System.Windows.Forms.ComboBox();
            this.selectedDisplayOption = new System.Windows.Forms.ComboBox();
            this.objectListList = new System.Windows.Forms.ComboBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.objectDefinitionList = new System.Windows.Forms.ComboBox();
            this.listPanel = new System.Windows.Forms.Panel();
            this.deleteDefinitionButton = new System.Windows.Forms.Button();
            this.addDefinitionButton = new System.Windows.Forms.Button();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportBugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            label6 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label24 = new System.Windows.Forms.Label();
            label23 = new System.Windows.Forms.Label();
            label22 = new System.Windows.Forms.Label();
            label21 = new System.Windows.Forms.Label();
            label18 = new System.Windows.Forms.Label();
            label19 = new System.Windows.Forms.Label();
            label20 = new System.Windows.Forms.Label();
            label17 = new System.Windows.Forms.Label();
            label16 = new System.Windows.Forms.Label();
            label15 = new System.Windows.Forms.Label();
            label14 = new System.Windows.Forms.Label();
            label13 = new System.Windows.Forms.Label();
            label12 = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            label28 = new System.Windows.Forms.Label();
            label27 = new System.Windows.Forms.Label();
            label26 = new System.Windows.Forms.Label();
            label25 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            label29 = new System.Windows.Forms.Label();
            label30 = new System.Windows.Forms.Label();
            label31 = new System.Windows.Forms.Label();
            label32 = new System.Windows.Forms.Label();
            label33 = new System.Windows.Forms.Label();
            label34 = new System.Windows.Forms.Label();
            label35 = new System.Windows.Forms.Label();
            label36 = new System.Windows.Forms.Label();
            label37 = new System.Windows.Forms.Label();
            label38 = new System.Windows.Forms.Label();
            label39 = new System.Windows.Forms.Label();
            label40 = new System.Windows.Forms.Label();
            label41 = new System.Windows.Forms.Label();
            label42 = new System.Windows.Forms.Label();
            label43 = new System.Windows.Forms.Label();
            label44 = new System.Windows.Forms.Label();
            label45 = new System.Windows.Forms.Label();
            label46 = new System.Windows.Forms.Label();
            label47 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.editorTabs.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.defImagePreview)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.selectedImagePreview)).BeginInit();
            this.imageControls.SuspendLayout();
            this.mappingsImageControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mappingsPalette)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mappingsFrame)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dplcFile)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mappingsFile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.artFilename)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.artOffset)).BeginInit();
            this.spriteImageControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spriteNum)).BeginInit();
            this.bitmapImageControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bitmapFilename)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageOffsetY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageOffsetX)).BeginInit();
            this.tabPage3.SuspendLayout();
            this.subtypeControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.subtypeImagePreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.subtypeID)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.enumControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.enumMemberValue)).BeginInit();
            this.tabPage5.SuspendLayout();
            this.propertyControls.SuspendLayout();
            this.customPropertyControls.SuspendLayout();
            this.bitsPropertyControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bitLength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.startBit)).BeginInit();
            this.tabPage6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.displayPreview)).BeginInit();
            this.displayOptionControls.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.imageRefControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageRefOffsetY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageRefOffsetX)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.conditionValueNum)).BeginInit();
            this.listPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(6, 88);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(39, 13);
            label6.TabIndex = 8;
            label6.Text = "Image:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(6, 61);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(58, 13);
            label5.TabIndex = 6;
            label5.Text = "Language:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(237, 35);
            label4.Margin = new System.Windows.Forms.Padding(0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(10, 13);
            label4.TabIndex = 4;
            label4.Text = ".";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(6, 35);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(65, 13);
            label3.TabIndex = 2;
            label3.Text = "Type Name:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(6, 9);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(38, 13);
            label2.TabIndex = 0;
            label2.Text = "Name:";
            // 
            // label24
            // 
            label24.AutoSize = true;
            label24.Location = new System.Drawing.Point(3, 5);
            label24.Name = "label24";
            label24.Size = new System.Drawing.Size(37, 13);
            label24.TabIndex = 0;
            label24.Text = "Sprite:";
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.Location = new System.Drawing.Point(3, 4);
            label23.Name = "label23";
            label23.Size = new System.Drawing.Size(52, 13);
            label23.TabIndex = 0;
            label23.Text = "Filename:";
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Location = new System.Drawing.Point(117, 314);
            label22.Name = "label22";
            label22.Size = new System.Drawing.Size(43, 13);
            label22.TabIndex = 36;
            label22.Text = "Palette:";
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Location = new System.Drawing.Point(3, 314);
            label21.Name = "label21";
            label21.Size = new System.Drawing.Size(39, 13);
            label21.TabIndex = 34;
            label21.Text = "Frame:";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new System.Drawing.Point(3, 288);
            label18.Name = "label18";
            label18.Size = new System.Drawing.Size(42, 13);
            label18.TabIndex = 32;
            label18.Text = "Format:";
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new System.Drawing.Point(272, 260);
            label19.Name = "label19";
            label19.Size = new System.Drawing.Size(36, 13);
            label19.TabIndex = 30;
            label19.Text = "Label:";
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Location = new System.Drawing.Point(3, 260);
            label20.Name = "label20";
            label20.Size = new System.Drawing.Size(43, 13);
            label20.TabIndex = 28;
            label20.Text = "DPLCs:";
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new System.Drawing.Point(3, 231);
            label17.Name = "label17";
            label17.Size = new System.Drawing.Size(42, 13);
            label17.TabIndex = 26;
            label17.Text = "Format:";
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new System.Drawing.Point(285, 203);
            label16.Name = "label16";
            label16.Size = new System.Drawing.Size(36, 13);
            label16.TabIndex = 24;
            label16.Text = "Label:";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new System.Drawing.Point(164, 59);
            label15.Name = "label15";
            label15.Size = new System.Drawing.Size(70, 13);
            label15.TabIndex = 21;
            label15.Text = "Compression:";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new System.Drawing.Point(164, 31);
            label14.Name = "label14";
            label14.Size = new System.Drawing.Size(52, 13);
            label14.TabIndex = 20;
            label14.Text = "Filename:";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new System.Drawing.Point(3, 203);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(56, 13);
            label13.TabIndex = 17;
            label13.Text = "Mappings:";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new System.Drawing.Point(129, 85);
            label12.Name = "label12";
            label12.Size = new System.Drawing.Size(38, 13);
            label12.TabIndex = 13;
            label12.Text = "Offset:";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(3, 58);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(34, 13);
            label11.TabIndex = 5;
            label11.Text = "Type:";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(3, 31);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(38, 13);
            label10.TabIndex = 2;
            label10.Text = "Offset:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(3, 6);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(38, 13);
            label9.TabIndex = 0;
            label9.Text = "Name:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(6, 9);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(39, 13);
            label1.TabIndex = 5;
            label1.Text = "Image:";
            // 
            // label28
            // 
            label28.AutoSize = true;
            label28.Location = new System.Drawing.Point(3, 58);
            label28.Name = "label28";
            label28.Size = new System.Drawing.Size(39, 13);
            label28.TabIndex = 5;
            label28.Text = "Image:";
            // 
            // label27
            // 
            label27.AutoSize = true;
            label27.Location = new System.Drawing.Point(3, 32);
            label27.Name = "label27";
            label27.Size = new System.Drawing.Size(38, 13);
            label27.TabIndex = 3;
            label27.Text = "Name:";
            // 
            // label26
            // 
            label26.AutoSize = true;
            label26.Location = new System.Drawing.Point(3, 5);
            label26.Name = "label26";
            label26.Size = new System.Drawing.Size(21, 13);
            label26.TabIndex = 0;
            label26.Text = "ID:";
            // 
            // label25
            // 
            label25.AutoSize = true;
            label25.Location = new System.Drawing.Point(6, 9);
            label25.Name = "label25";
            label25.Size = new System.Drawing.Size(49, 13);
            label25.TabIndex = 9;
            label25.Text = "Subtype:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(12, 9);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(60, 13);
            label7.TabIndex = 2;
            label7.Text = "Object List:";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(12, 36);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(88, 13);
            label8.TabIndex = 4;
            label8.Text = "Object Definition:";
            // 
            // label29
            // 
            label29.AutoSize = true;
            label29.Location = new System.Drawing.Point(6, 9);
            label29.Name = "label29";
            label29.Size = new System.Drawing.Size(37, 13);
            label29.TabIndex = 13;
            label29.Text = "Enum:";
            // 
            // label30
            // 
            label30.AutoSize = true;
            label30.Location = new System.Drawing.Point(129, 111);
            label30.Name = "label30";
            label30.Size = new System.Drawing.Size(37, 13);
            label30.TabIndex = 21;
            label30.Text = "Value:";
            // 
            // label31
            // 
            label31.AutoSize = true;
            label31.Location = new System.Drawing.Point(3, 6);
            label31.Name = "label31";
            label31.Size = new System.Drawing.Size(38, 13);
            label31.TabIndex = 0;
            label31.Text = "Name:";
            // 
            // label32
            // 
            label32.AutoSize = true;
            label32.Location = new System.Drawing.Point(129, 86);
            label32.Name = "label32";
            label32.Size = new System.Drawing.Size(38, 13);
            label32.TabIndex = 25;
            label32.Text = "Name:";
            // 
            // label33
            // 
            label33.AutoSize = true;
            label33.Location = new System.Drawing.Point(6, 9);
            label33.Name = "label33";
            label33.Size = new System.Drawing.Size(49, 13);
            label33.TabIndex = 9;
            label33.Text = "Property:";
            // 
            // label34
            // 
            label34.AutoSize = true;
            label34.Location = new System.Drawing.Point(3, 6);
            label34.Name = "label34";
            label34.Size = new System.Drawing.Size(38, 13);
            label34.TabIndex = 0;
            label34.Text = "Name:";
            // 
            // label35
            // 
            label35.AutoSize = true;
            label35.Location = new System.Drawing.Point(3, 32);
            label35.Name = "label35";
            label35.Size = new System.Drawing.Size(64, 13);
            label35.TabIndex = 2;
            label35.Text = "Value Type:";
            // 
            // label36
            // 
            label36.AutoSize = true;
            label36.Location = new System.Drawing.Point(3, 59);
            label36.Name = "label36";
            label36.Size = new System.Drawing.Size(63, 13);
            label36.TabIndex = 4;
            label36.Text = "Description:";
            // 
            // label37
            // 
            label37.AutoSize = true;
            label37.Location = new System.Drawing.Point(3, 85);
            label37.Name = "label37";
            label37.Size = new System.Drawing.Size(76, 13);
            label37.TabIndex = 6;
            label37.Text = "Property Type:";
            // 
            // label38
            // 
            label38.AutoSize = true;
            label38.Location = new System.Drawing.Point(3, 5);
            label38.Name = "label38";
            label38.Size = new System.Drawing.Size(47, 13);
            label38.TabIndex = 0;
            label38.Text = "Start Bit:";
            // 
            // label39
            // 
            label39.AutoSize = true;
            label39.Location = new System.Drawing.Point(3, 31);
            label39.Name = "label39";
            label39.Size = new System.Drawing.Size(43, 13);
            label39.TabIndex = 2;
            label39.Text = "Length:";
            // 
            // label40
            // 
            label40.AutoSize = true;
            label40.Location = new System.Drawing.Point(3, 6);
            label40.Name = "label40";
            label40.Size = new System.Drawing.Size(66, 13);
            label40.TabIndex = 0;
            label40.Text = "Get Method:";
            // 
            // label41
            // 
            label41.AutoSize = true;
            label41.Location = new System.Drawing.Point(3, 112);
            label41.Name = "label41";
            label41.Size = new System.Drawing.Size(65, 13);
            label41.TabIndex = 2;
            label41.Text = "Set Method:";
            // 
            // label42
            // 
            label42.AutoSize = true;
            label42.Location = new System.Drawing.Point(6, 9);
            label42.Name = "label42";
            label42.Size = new System.Drawing.Size(41, 13);
            label42.TabIndex = 0;
            label42.Text = "Option:";
            // 
            // label43
            // 
            label43.AutoSize = true;
            label43.Location = new System.Drawing.Point(131, 50);
            label43.Margin = new System.Windows.Forms.Padding(0);
            label43.Name = "label43";
            label43.Size = new System.Drawing.Size(13, 13);
            label43.TabIndex = 5;
            label43.Text = "=";
            // 
            // label44
            // 
            label44.AutoSize = true;
            label44.Location = new System.Drawing.Point(3, 32);
            label44.Name = "label44";
            label44.Size = new System.Drawing.Size(38, 13);
            label44.TabIndex = 5;
            label44.Text = "Offset:";
            // 
            // label45
            // 
            label45.AutoSize = true;
            label45.Location = new System.Drawing.Point(3, 6);
            label45.Name = "label45";
            label45.Size = new System.Drawing.Size(39, 13);
            label45.TabIndex = 2;
            label45.Text = "Image:";
            // 
            // label46
            // 
            label46.AutoSize = true;
            label46.Location = new System.Drawing.Point(3, 59);
            label46.Name = "label46";
            label46.Size = new System.Drawing.Size(83, 13);
            label46.TabIndex = 6;
            label46.Text = "Flip Horizontally:";
            // 
            // label47
            // 
            label47.AutoSize = true;
            label47.Location = new System.Drawing.Point(3, 84);
            label47.Name = "label47";
            label47.Size = new System.Drawing.Size(71, 13);
            label47.TabIndex = 8;
            label47.Text = "Flip Vertically:";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(584, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.changeLevelToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.toolStripSeparator1,
            this.recentProjectsToolStripMenuItem,
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
            // changeLevelToolStripMenuItem
            // 
            this.changeLevelToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noneToolStripMenuItem1});
            this.changeLevelToolStripMenuItem.Name = "changeLevelToolStripMenuItem";
            this.changeLevelToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.changeLevelToolStripMenuItem.Text = "&Change Level...";
            // 
            // noneToolStripMenuItem1
            // 
            this.noneToolStripMenuItem1.Enabled = false;
            this.noneToolStripMenuItem1.Name = "noneToolStripMenuItem1";
            this.noneToolStripMenuItem1.Size = new System.Drawing.Size(109, 22);
            this.noneToolStripMenuItem1.Text = "(none)";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Enabled = false;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(152, 6);
            // 
            // recentProjectsToolStripMenuItem
            // 
            this.recentProjectsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.noneToolStripMenuItem});
            this.recentProjectsToolStripMenuItem.Name = "recentProjectsToolStripMenuItem";
            this.recentProjectsToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.recentProjectsToolStripMenuItem.Text = "Recent &Projects";
            this.recentProjectsToolStripMenuItem.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.recentFilesToolStripMenuItem_DropDownItemClicked);
            // 
            // noneToolStripMenuItem
            // 
            this.noneToolStripMenuItem.Enabled = false;
            this.noneToolStripMenuItem.Name = "noneToolStripMenuItem";
            this.noneToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.noneToolStripMenuItem.Text = "(none)";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editorTabs
            // 
            this.editorTabs.Controls.Add(this.tabPage1);
            this.editorTabs.Controls.Add(this.tabPage2);
            this.editorTabs.Controls.Add(this.tabPage3);
            this.editorTabs.Controls.Add(this.tabPage4);
            this.editorTabs.Controls.Add(this.tabPage5);
            this.editorTabs.Controls.Add(this.tabPage6);
            this.editorTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editorTabs.Enabled = false;
            this.editorTabs.Location = new System.Drawing.Point(0, 84);
            this.editorTabs.Name = "editorTabs";
            this.editorTabs.SelectedIndex = 0;
            this.editorTabs.Size = new System.Drawing.Size(584, 480);
            this.editorTabs.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.AutoScroll = true;
            this.tabPage1.Controls.Add(this.defImagePreview);
            this.tabPage1.Controls.Add(this.defDebug);
            this.tabPage1.Controls.Add(this.defRemember);
            this.tabPage1.Controls.Add(this.defImage);
            this.tabPage1.Controls.Add(label6);
            this.tabPage1.Controls.Add(this.defLanguage);
            this.tabPage1.Controls.Add(label5);
            this.tabPage1.Controls.Add(this.defType);
            this.tabPage1.Controls.Add(label4);
            this.tabPage1.Controls.Add(this.defNamespace);
            this.tabPage1.Controls.Add(label3);
            this.tabPage1.Controls.Add(this.defName);
            this.tabPage1.Controls.Add(label2);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(576, 454);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "General";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // defImagePreview
            // 
            this.defImagePreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.defImagePreview.Location = new System.Drawing.Point(197, 58);
            this.defImagePreview.Name = "defImagePreview";
            this.defImagePreview.Size = new System.Drawing.Size(100, 100);
            this.defImagePreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.defImagePreview.TabIndex = 12;
            this.defImagePreview.TabStop = false;
            // 
            // defDebug
            // 
            this.defDebug.AutoSize = true;
            this.defDebug.Location = new System.Drawing.Point(117, 112);
            this.defDebug.Name = "defDebug";
            this.defDebug.Size = new System.Drawing.Size(58, 17);
            this.defDebug.TabIndex = 11;
            this.defDebug.Text = "Debug";
            this.defDebug.UseVisualStyleBackColor = true;
            // 
            // defRemember
            // 
            this.defRemember.AutoSize = true;
            this.defRemember.Location = new System.Drawing.Point(6, 112);
            this.defRemember.Name = "defRemember";
            this.defRemember.Size = new System.Drawing.Size(105, 17);
            this.defRemember.TabIndex = 10;
            this.defRemember.Text = "Remember State";
            this.defRemember.UseVisualStyleBackColor = true;
            // 
            // defImage
            // 
            this.defImage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.defImage.FormattingEnabled = true;
            this.defImage.Location = new System.Drawing.Point(51, 85);
            this.defImage.Name = "defImage";
            this.defImage.Size = new System.Drawing.Size(121, 21);
            this.defImage.TabIndex = 9;
            this.defImage.SelectedIndexChanged += new System.EventHandler(this.defImage_SelectedIndexChanged);
            // 
            // defLanguage
            // 
            this.defLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.defLanguage.FormattingEnabled = true;
            this.defLanguage.Items.AddRange(new object[] {
            "C#",
            "VB.NET"});
            this.defLanguage.Location = new System.Drawing.Point(70, 58);
            this.defLanguage.Name = "defLanguage";
            this.defLanguage.Size = new System.Drawing.Size(121, 21);
            this.defLanguage.TabIndex = 7;
            this.defLanguage.SelectedIndexChanged += new System.EventHandler(this.defLanguage_SelectedIndexChanged);
            // 
            // defType
            // 
            this.defType.Location = new System.Drawing.Point(250, 32);
            this.defType.Name = "defType";
            this.defType.Size = new System.Drawing.Size(157, 20);
            this.defType.TabIndex = 5;
            this.defType.TextChanged += new System.EventHandler(this.defType_TextChanged);
            // 
            // defNamespace
            // 
            this.defNamespace.Location = new System.Drawing.Point(77, 32);
            this.defNamespace.Name = "defNamespace";
            this.defNamespace.Size = new System.Drawing.Size(157, 20);
            this.defNamespace.TabIndex = 3;
            this.defNamespace.TextChanged += new System.EventHandler(this.defNamespace_TextChanged);
            // 
            // defName
            // 
            this.defName.Location = new System.Drawing.Point(50, 6);
            this.defName.Name = "defName";
            this.defName.Size = new System.Drawing.Size(247, 20);
            this.defName.TabIndex = 1;
            this.defName.TextChanged += new System.EventHandler(this.defName_TextChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.AutoScroll = true;
            this.tabPage2.Controls.Add(this.selectedImagePreview);
            this.tabPage2.Controls.Add(this.imageControls);
            this.tabPage2.Controls.Add(this.deleteImageButton);
            this.tabPage2.Controls.Add(this.addImageButton);
            this.tabPage2.Controls.Add(label1);
            this.tabPage2.Controls.Add(this.selectedImage);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(576, 454);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Images";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // selectedImagePreview
            // 
            this.selectedImagePreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.selectedImagePreview.Location = new System.Drawing.Point(452, 33);
            this.selectedImagePreview.Name = "selectedImagePreview";
            this.selectedImagePreview.Size = new System.Drawing.Size(100, 100);
            this.selectedImagePreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.selectedImagePreview.TabIndex = 9;
            this.selectedImagePreview.TabStop = false;
            // 
            // imageControls
            // 
            this.imageControls.AutoSize = true;
            this.imageControls.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.imageControls.Controls.Add(this.mappingsImageControls);
            this.imageControls.Controls.Add(this.spriteImageControls);
            this.imageControls.Controls.Add(this.bitmapImageControls);
            this.imageControls.Controls.Add(this.imageType);
            this.imageControls.Controls.Add(label11);
            this.imageControls.Controls.Add(this.imageOffsetY);
            this.imageControls.Controls.Add(this.imageOffsetX);
            this.imageControls.Controls.Add(label10);
            this.imageControls.Controls.Add(this.imageName);
            this.imageControls.Controls.Add(label9);
            this.imageControls.Enabled = false;
            this.imageControls.Location = new System.Drawing.Point(6, 33);
            this.imageControls.Name = "imageControls";
            this.imageControls.Size = new System.Drawing.Size(439, 417);
            this.imageControls.TabIndex = 8;
            // 
            // mappingsImageControls
            // 
            this.mappingsImageControls.AutoSize = true;
            this.mappingsImageControls.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.mappingsImageControls.Controls.Add(this.mappingsPalette);
            this.mappingsImageControls.Controls.Add(label22);
            this.mappingsImageControls.Controls.Add(this.mappingsFrame);
            this.mappingsImageControls.Controls.Add(label21);
            this.mappingsImageControls.Controls.Add(this.dplcFormat);
            this.mappingsImageControls.Controls.Add(label18);
            this.mappingsImageControls.Controls.Add(this.dplcLabel);
            this.mappingsImageControls.Controls.Add(label19);
            this.mappingsImageControls.Controls.Add(this.dplcFile);
            this.mappingsImageControls.Controls.Add(label20);
            this.mappingsImageControls.Controls.Add(this.mappingsFormat);
            this.mappingsImageControls.Controls.Add(label17);
            this.mappingsImageControls.Controls.Add(this.mappingsLabel);
            this.mappingsImageControls.Controls.Add(label16);
            this.mappingsImageControls.Controls.Add(this.groupBox1);
            this.mappingsImageControls.Controls.Add(this.artCompression);
            this.mappingsImageControls.Controls.Add(label15);
            this.mappingsImageControls.Controls.Add(label14);
            this.mappingsImageControls.Controls.Add(this.useLevelArt);
            this.mappingsImageControls.Controls.Add(this.mappingsFile);
            this.mappingsImageControls.Controls.Add(label13);
            this.mappingsImageControls.Controls.Add(this.artFilename);
            this.mappingsImageControls.Controls.Add(this.defaultArtOffset);
            this.mappingsImageControls.Controls.Add(this.artOffset);
            this.mappingsImageControls.Controls.Add(label12);
            this.mappingsImageControls.Controls.Add(this.artList);
            this.mappingsImageControls.Controls.Add(this.artDownButton);
            this.mappingsImageControls.Controls.Add(this.artAddButton);
            this.mappingsImageControls.Controls.Add(this.artUpButton);
            this.mappingsImageControls.Controls.Add(this.artRemoveButton);
            this.mappingsImageControls.Location = new System.Drawing.Point(0, 82);
            this.mappingsImageControls.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.mappingsImageControls.Name = "mappingsImageControls";
            this.mappingsImageControls.Size = new System.Drawing.Size(439, 335);
            this.mappingsImageControls.TabIndex = 7;
            this.mappingsImageControls.Visible = false;
            // 
            // mappingsPalette
            // 
            this.mappingsPalette.Location = new System.Drawing.Point(166, 312);
            this.mappingsPalette.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.mappingsPalette.Name = "mappingsPalette";
            this.mappingsPalette.Size = new System.Drawing.Size(43, 20);
            this.mappingsPalette.TabIndex = 37;
            this.mappingsPalette.ValueChanged += new System.EventHandler(this.mappingsPalette_ValueChanged);
            // 
            // mappingsFrame
            // 
            this.mappingsFrame.Location = new System.Drawing.Point(48, 312);
            this.mappingsFrame.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.mappingsFrame.Name = "mappingsFrame";
            this.mappingsFrame.Size = new System.Drawing.Size(63, 20);
            this.mappingsFrame.TabIndex = 35;
            this.mappingsFrame.ValueChanged += new System.EventHandler(this.mappingsFrame_ValueChanged);
            // 
            // dplcFormat
            // 
            this.dplcFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.dplcFormat.FormattingEnabled = true;
            this.dplcFormat.Items.AddRange(new object[] {
            "Default",
            "Sonic 1",
            "Sonic 2",
            "Sonic 3 & Knuckles"});
            this.dplcFormat.Location = new System.Drawing.Point(51, 285);
            this.dplcFormat.Name = "dplcFormat";
            this.dplcFormat.Size = new System.Drawing.Size(121, 21);
            this.dplcFormat.TabIndex = 33;
            this.dplcFormat.SelectedIndexChanged += new System.EventHandler(this.dplcFormat_SelectedIndexChanged);
            // 
            // dplcLabel
            // 
            this.dplcLabel.Location = new System.Drawing.Point(314, 257);
            this.dplcLabel.Name = "dplcLabel";
            this.dplcLabel.Size = new System.Drawing.Size(100, 20);
            this.dplcLabel.TabIndex = 31;
            this.dplcLabel.TextChanged += new System.EventHandler(this.dplcLabel_TextChanged);
            // 
            // dplcFile
            // 
            this.dplcFile.DefaultExt = "";
            this.dplcFile.FileName = "";
            this.dplcFile.Filter = "";
            this.dplcFile.Location = new System.Drawing.Point(52, 255);
            this.dplcFile.Name = "dplcFile";
            this.dplcFile.Size = new System.Drawing.Size(214, 24);
            this.dplcFile.TabIndex = 29;
            this.dplcFile.FileNameChanged += new System.EventHandler(this.dplcFile_FileNameChanged);
            // 
            // mappingsFormat
            // 
            this.mappingsFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mappingsFormat.FormattingEnabled = true;
            this.mappingsFormat.Items.AddRange(new object[] {
            "Default",
            "Sonic 1",
            "Sonic 2",
            "Sonic 3 & Knuckles"});
            this.mappingsFormat.Location = new System.Drawing.Point(51, 228);
            this.mappingsFormat.Name = "mappingsFormat";
            this.mappingsFormat.Size = new System.Drawing.Size(121, 21);
            this.mappingsFormat.TabIndex = 27;
            this.mappingsFormat.SelectedIndexChanged += new System.EventHandler(this.mappingsFormat_SelectedIndexChanged);
            // 
            // mappingsLabel
            // 
            this.mappingsLabel.Location = new System.Drawing.Point(327, 200);
            this.mappingsLabel.Name = "mappingsLabel";
            this.mappingsLabel.Size = new System.Drawing.Size(100, 20);
            this.mappingsLabel.TabIndex = 25;
            this.mappingsLabel.TextChanged += new System.EventHandler(this.mappingsLabel_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.mappingsASM);
            this.groupBox1.Controls.Add(this.mappingsBinary);
            this.groupBox1.Location = new System.Drawing.Point(3, 146);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.groupBox1.Size = new System.Drawing.Size(117, 46);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Mappings Type";
            // 
            // mappingsASM
            // 
            this.mappingsASM.AutoSize = true;
            this.mappingsASM.Location = new System.Drawing.Point(66, 16);
            this.mappingsASM.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.mappingsASM.Name = "mappingsASM";
            this.mappingsASM.Size = new System.Drawing.Size(48, 17);
            this.mappingsASM.TabIndex = 1;
            this.mappingsASM.TabStop = true;
            this.mappingsASM.Text = "ASM";
            this.mappingsASM.UseVisualStyleBackColor = true;
            this.mappingsASM.CheckedChanged += new System.EventHandler(this.mappingsType_CheckedChanged);
            // 
            // mappingsBinary
            // 
            this.mappingsBinary.AutoSize = true;
            this.mappingsBinary.Checked = true;
            this.mappingsBinary.Location = new System.Drawing.Point(6, 16);
            this.mappingsBinary.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.mappingsBinary.Name = "mappingsBinary";
            this.mappingsBinary.Size = new System.Drawing.Size(54, 17);
            this.mappingsBinary.TabIndex = 0;
            this.mappingsBinary.TabStop = true;
            this.mappingsBinary.Text = "Binary";
            this.mappingsBinary.UseVisualStyleBackColor = true;
            this.mappingsBinary.CheckedChanged += new System.EventHandler(this.mappingsType_CheckedChanged);
            // 
            // artCompression
            // 
            this.artCompression.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.artCompression.FormattingEnabled = true;
            this.artCompression.Items.AddRange(new object[] {
            "Default (Nemesis)",
            "Uncompressed",
            "Kosinski",
            "Moduled Kosinski",
            "Nemesis",
            "Enigma",
            "SZDD"});
            this.artCompression.Location = new System.Drawing.Point(240, 56);
            this.artCompression.Name = "artCompression";
            this.artCompression.Size = new System.Drawing.Size(121, 21);
            this.artCompression.TabIndex = 22;
            this.artCompression.SelectedIndexChanged += new System.EventHandler(this.artCompression_SelectedIndexChanged);
            // 
            // useLevelArt
            // 
            this.useLevelArt.AutoSize = true;
            this.useLevelArt.Location = new System.Drawing.Point(164, 3);
            this.useLevelArt.Name = "useLevelArt";
            this.useLevelArt.Size = new System.Drawing.Size(90, 17);
            this.useLevelArt.TabIndex = 19;
            this.useLevelArt.Text = "Use Level Art";
            this.useLevelArt.UseVisualStyleBackColor = true;
            this.useLevelArt.CheckedChanged += new System.EventHandler(this.useLevelArt_CheckedChanged);
            // 
            // mappingsFile
            // 
            this.mappingsFile.DefaultExt = "";
            this.mappingsFile.FileName = "";
            this.mappingsFile.Filter = "";
            this.mappingsFile.Location = new System.Drawing.Point(65, 198);
            this.mappingsFile.Name = "mappingsFile";
            this.mappingsFile.Size = new System.Drawing.Size(214, 24);
            this.mappingsFile.TabIndex = 18;
            this.mappingsFile.FileNameChanged += new System.EventHandler(this.mappingsFile_FileNameChanged);
            // 
            // artFilename
            // 
            this.artFilename.DefaultExt = "bin";
            this.artFilename.FileName = "";
            this.artFilename.Filter = "Binary Files|*.bin|All Files|*.*";
            this.artFilename.Location = new System.Drawing.Point(222, 26);
            this.artFilename.Name = "artFilename";
            this.artFilename.Size = new System.Drawing.Size(214, 24);
            this.artFilename.TabIndex = 16;
            this.artFilename.FileNameChanged += new System.EventHandler(this.artFilename_FileNameChanged);
            // 
            // defaultArtOffset
            // 
            this.defaultArtOffset.AutoSize = true;
            this.defaultArtOffset.Enabled = false;
            this.defaultArtOffset.Location = new System.Drawing.Point(236, 84);
            this.defaultArtOffset.Name = "defaultArtOffset";
            this.defaultArtOffset.Size = new System.Drawing.Size(60, 17);
            this.defaultArtOffset.TabIndex = 15;
            this.defaultArtOffset.Text = "Default";
            this.defaultArtOffset.UseVisualStyleBackColor = true;
            this.defaultArtOffset.CheckedChanged += new System.EventHandler(this.defaultArtOffset_CheckedChanged);
            // 
            // artOffset
            // 
            this.artOffset.Enabled = false;
            this.artOffset.Hexadecimal = true;
            this.artOffset.Location = new System.Drawing.Point(173, 83);
            this.artOffset.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.artOffset.Name = "artOffset";
            this.artOffset.Size = new System.Drawing.Size(57, 20);
            this.artOffset.TabIndex = 14;
            this.artOffset.ValueChanged += new System.EventHandler(this.artOffset_ValueChanged);
            // 
            // artList
            // 
            this.artList.FormattingEnabled = true;
            this.artList.Location = new System.Drawing.Point(3, 3);
            this.artList.Name = "artList";
            this.artList.Size = new System.Drawing.Size(120, 108);
            this.artList.TabIndex = 8;
            this.artList.SelectedIndexChanged += new System.EventHandler(this.artList_SelectedIndexChanged);
            // 
            // artDownButton
            // 
            this.artDownButton.AutoSize = true;
            this.artDownButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.artDownButton.Enabled = false;
            this.artDownButton.Location = new System.Drawing.Point(129, 32);
            this.artDownButton.Name = "artDownButton";
            this.artDownButton.Size = new System.Drawing.Size(29, 23);
            this.artDownButton.TabIndex = 12;
            this.artDownButton.Text = "↓";
            this.artDownButton.UseVisualStyleBackColor = true;
            this.artDownButton.Click += new System.EventHandler(this.artDownButton_Click);
            // 
            // artAddButton
            // 
            this.artAddButton.AutoSize = true;
            this.artAddButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.artAddButton.Location = new System.Drawing.Point(3, 117);
            this.artAddButton.Name = "artAddButton";
            this.artAddButton.Size = new System.Drawing.Size(36, 23);
            this.artAddButton.TabIndex = 9;
            this.artAddButton.Text = "Add";
            this.artAddButton.UseVisualStyleBackColor = true;
            this.artAddButton.Click += new System.EventHandler(this.artAddButton_Click);
            // 
            // artUpButton
            // 
            this.artUpButton.AutoSize = true;
            this.artUpButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.artUpButton.Enabled = false;
            this.artUpButton.Location = new System.Drawing.Point(129, 3);
            this.artUpButton.Name = "artUpButton";
            this.artUpButton.Size = new System.Drawing.Size(29, 23);
            this.artUpButton.TabIndex = 11;
            this.artUpButton.Text = "↑";
            this.artUpButton.UseVisualStyleBackColor = true;
            this.artUpButton.Click += new System.EventHandler(this.artUpButton_Click);
            // 
            // artRemoveButton
            // 
            this.artRemoveButton.AutoSize = true;
            this.artRemoveButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.artRemoveButton.Enabled = false;
            this.artRemoveButton.Location = new System.Drawing.Point(45, 117);
            this.artRemoveButton.Name = "artRemoveButton";
            this.artRemoveButton.Size = new System.Drawing.Size(57, 23);
            this.artRemoveButton.TabIndex = 10;
            this.artRemoveButton.Text = "Remove";
            this.artRemoveButton.UseVisualStyleBackColor = true;
            this.artRemoveButton.Click += new System.EventHandler(this.artRemoveButton_Click);
            // 
            // spriteImageControls
            // 
            this.spriteImageControls.AutoSize = true;
            this.spriteImageControls.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.spriteImageControls.Controls.Add(this.spriteNum);
            this.spriteImageControls.Controls.Add(label24);
            this.spriteImageControls.Location = new System.Drawing.Point(0, 82);
            this.spriteImageControls.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.spriteImageControls.Name = "spriteImageControls";
            this.spriteImageControls.Size = new System.Drawing.Size(169, 26);
            this.spriteImageControls.TabIndex = 9;
            this.spriteImageControls.Visible = false;
            // 
            // spriteNum
            // 
            this.spriteNum.Location = new System.Drawing.Point(46, 3);
            this.spriteNum.Name = "spriteNum";
            this.spriteNum.Size = new System.Drawing.Size(120, 20);
            this.spriteNum.TabIndex = 1;
            this.spriteNum.ValueChanged += new System.EventHandler(this.spriteNum_ValueChanged);
            // 
            // bitmapImageControls
            // 
            this.bitmapImageControls.AutoSize = true;
            this.bitmapImageControls.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.bitmapImageControls.Controls.Add(this.bitmapFilename);
            this.bitmapImageControls.Controls.Add(label23);
            this.bitmapImageControls.Location = new System.Drawing.Point(0, 82);
            this.bitmapImageControls.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.bitmapImageControls.Name = "bitmapImageControls";
            this.bitmapImageControls.Size = new System.Drawing.Size(439, 30);
            this.bitmapImageControls.TabIndex = 8;
            this.bitmapImageControls.Visible = false;
            // 
            // bitmapFilename
            // 
            this.bitmapFilename.DefaultExt = "png";
            this.bitmapFilename.FileName = "";
            this.bitmapFilename.Filter = "Image Files|*.bmp;*.png;*.gif;*.jpg;*.jpeg";
            this.bitmapFilename.Location = new System.Drawing.Point(61, 3);
            this.bitmapFilename.Name = "bitmapFilename";
            this.bitmapFilename.Size = new System.Drawing.Size(375, 24);
            this.bitmapFilename.TabIndex = 1;
            this.bitmapFilename.FileNameChanged += new System.EventHandler(this.bitmapFilename_FileNameChanged);
            // 
            // imageType
            // 
            this.imageType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.imageType.FormattingEnabled = true;
            this.imageType.Items.AddRange(new object[] {
            "Art+Mappings",
            "Bitmap",
            "Sprite"});
            this.imageType.Location = new System.Drawing.Point(47, 55);
            this.imageType.Name = "imageType";
            this.imageType.Size = new System.Drawing.Size(121, 21);
            this.imageType.TabIndex = 6;
            this.imageType.SelectedIndexChanged += new System.EventHandler(this.imageType_SelectedIndexChanged);
            // 
            // imageOffsetY
            // 
            this.imageOffsetY.Location = new System.Drawing.Point(117, 29);
            this.imageOffsetY.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.imageOffsetY.Minimum = new decimal(new int[] {
            65536,
            0,
            0,
            -2147483648});
            this.imageOffsetY.Name = "imageOffsetY";
            this.imageOffsetY.Size = new System.Drawing.Size(64, 20);
            this.imageOffsetY.TabIndex = 4;
            this.imageOffsetY.ValueChanged += new System.EventHandler(this.imageOffsetY_ValueChanged);
            // 
            // imageOffsetX
            // 
            this.imageOffsetX.Location = new System.Drawing.Point(47, 29);
            this.imageOffsetX.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.imageOffsetX.Minimum = new decimal(new int[] {
            65536,
            0,
            0,
            -2147483648});
            this.imageOffsetX.Name = "imageOffsetX";
            this.imageOffsetX.Size = new System.Drawing.Size(64, 20);
            this.imageOffsetX.TabIndex = 3;
            this.imageOffsetX.ValueChanged += new System.EventHandler(this.imageOffsetX_ValueChanged);
            // 
            // imageName
            // 
            this.imageName.Location = new System.Drawing.Point(47, 3);
            this.imageName.Name = "imageName";
            this.imageName.Size = new System.Drawing.Size(100, 20);
            this.imageName.TabIndex = 1;
            this.imageName.TextChanged += new System.EventHandler(this.imageName_TextChanged);
            // 
            // deleteImageButton
            // 
            this.deleteImageButton.AutoSize = true;
            this.deleteImageButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.deleteImageButton.Enabled = false;
            this.deleteImageButton.Location = new System.Drawing.Point(220, 4);
            this.deleteImageButton.Name = "deleteImageButton";
            this.deleteImageButton.Size = new System.Drawing.Size(48, 23);
            this.deleteImageButton.TabIndex = 7;
            this.deleteImageButton.Text = "Delete";
            this.deleteImageButton.UseVisualStyleBackColor = true;
            this.deleteImageButton.Click += new System.EventHandler(this.deleteImageButton_Click);
            // 
            // addImageButton
            // 
            this.addImageButton.AutoSize = true;
            this.addImageButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.addImageButton.Location = new System.Drawing.Point(178, 4);
            this.addImageButton.Name = "addImageButton";
            this.addImageButton.Size = new System.Drawing.Size(36, 23);
            this.addImageButton.TabIndex = 6;
            this.addImageButton.Text = "Add";
            this.addImageButton.UseVisualStyleBackColor = true;
            this.addImageButton.Click += new System.EventHandler(this.addImageButton_Click);
            // 
            // selectedImage
            // 
            this.selectedImage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectedImage.FormattingEnabled = true;
            this.selectedImage.Location = new System.Drawing.Point(51, 6);
            this.selectedImage.Name = "selectedImage";
            this.selectedImage.Size = new System.Drawing.Size(121, 21);
            this.selectedImage.TabIndex = 4;
            this.selectedImage.SelectedIndexChanged += new System.EventHandler(this.selectedImage_SelectedIndexChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.AutoScroll = true;
            this.tabPage3.Controls.Add(this.subtypeControls);
            this.tabPage3.Controls.Add(this.deleteSubtypeButton);
            this.tabPage3.Controls.Add(this.addSubtypeButton);
            this.tabPage3.Controls.Add(label25);
            this.tabPage3.Controls.Add(this.selectedSubtype);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(576, 454);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Subtypes";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // subtypeControls
            // 
            this.subtypeControls.AutoSize = true;
            this.subtypeControls.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.subtypeControls.Controls.Add(this.subtypeImagePreview);
            this.subtypeControls.Controls.Add(label28);
            this.subtypeControls.Controls.Add(this.subtypeImage);
            this.subtypeControls.Controls.Add(label27);
            this.subtypeControls.Controls.Add(this.subtypeName);
            this.subtypeControls.Controls.Add(this.subtypeID);
            this.subtypeControls.Controls.Add(label26);
            this.subtypeControls.Location = new System.Drawing.Point(6, 33);
            this.subtypeControls.Name = "subtypeControls";
            this.subtypeControls.Size = new System.Drawing.Size(306, 106);
            this.subtypeControls.TabIndex = 12;
            // 
            // subtypeImagePreview
            // 
            this.subtypeImagePreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.subtypeImagePreview.Location = new System.Drawing.Point(203, 3);
            this.subtypeImagePreview.Name = "subtypeImagePreview";
            this.subtypeImagePreview.Size = new System.Drawing.Size(100, 100);
            this.subtypeImagePreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.subtypeImagePreview.TabIndex = 13;
            this.subtypeImagePreview.TabStop = false;
            // 
            // subtypeImage
            // 
            this.subtypeImage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.subtypeImage.FormattingEnabled = true;
            this.subtypeImage.Location = new System.Drawing.Point(48, 55);
            this.subtypeImage.Name = "subtypeImage";
            this.subtypeImage.Size = new System.Drawing.Size(121, 21);
            this.subtypeImage.TabIndex = 4;
            this.subtypeImage.SelectedIndexChanged += new System.EventHandler(this.subtypeImage_SelectedIndexChanged);
            // 
            // subtypeName
            // 
            this.subtypeName.Location = new System.Drawing.Point(47, 29);
            this.subtypeName.Name = "subtypeName";
            this.subtypeName.Size = new System.Drawing.Size(150, 20);
            this.subtypeName.TabIndex = 2;
            this.subtypeName.TextChanged += new System.EventHandler(this.subtypeName_TextChanged);
            // 
            // subtypeID
            // 
            this.subtypeID.Hexadecimal = true;
            this.subtypeID.Location = new System.Drawing.Point(47, 5);
            this.subtypeID.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.subtypeID.Name = "subtypeID";
            this.subtypeID.Size = new System.Drawing.Size(50, 20);
            this.subtypeID.TabIndex = 1;
            this.subtypeID.ValueChanged += new System.EventHandler(this.subtypeID_ValueChanged);
            // 
            // deleteSubtypeButton
            // 
            this.deleteSubtypeButton.AutoSize = true;
            this.deleteSubtypeButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.deleteSubtypeButton.Enabled = false;
            this.deleteSubtypeButton.Location = new System.Drawing.Point(230, 4);
            this.deleteSubtypeButton.Name = "deleteSubtypeButton";
            this.deleteSubtypeButton.Size = new System.Drawing.Size(48, 23);
            this.deleteSubtypeButton.TabIndex = 11;
            this.deleteSubtypeButton.Text = "Delete";
            this.deleteSubtypeButton.UseVisualStyleBackColor = true;
            this.deleteSubtypeButton.Click += new System.EventHandler(this.deleteSubtypeButton_Click);
            // 
            // addSubtypeButton
            // 
            this.addSubtypeButton.AutoSize = true;
            this.addSubtypeButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.addSubtypeButton.Location = new System.Drawing.Point(188, 4);
            this.addSubtypeButton.Name = "addSubtypeButton";
            this.addSubtypeButton.Size = new System.Drawing.Size(36, 23);
            this.addSubtypeButton.TabIndex = 10;
            this.addSubtypeButton.Text = "Add";
            this.addSubtypeButton.UseVisualStyleBackColor = true;
            this.addSubtypeButton.Click += new System.EventHandler(this.addSubtypeButton_Click);
            // 
            // selectedSubtype
            // 
            this.selectedSubtype.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectedSubtype.FormattingEnabled = true;
            this.selectedSubtype.Location = new System.Drawing.Point(61, 6);
            this.selectedSubtype.Name = "selectedSubtype";
            this.selectedSubtype.Size = new System.Drawing.Size(121, 21);
            this.selectedSubtype.TabIndex = 8;
            this.selectedSubtype.SelectedIndexChanged += new System.EventHandler(this.selectedSubtype_SelectedIndexChanged);
            // 
            // tabPage4
            // 
            this.tabPage4.AutoScroll = true;
            this.tabPage4.Controls.Add(this.enumControls);
            this.tabPage4.Controls.Add(this.deleteEnumButton);
            this.tabPage4.Controls.Add(this.addEnumButton);
            this.tabPage4.Controls.Add(label29);
            this.tabPage4.Controls.Add(this.selectedEnum);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(576, 454);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Enums";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // enumControls
            // 
            this.enumControls.AutoSize = true;
            this.enumControls.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.enumControls.Controls.Add(label32);
            this.enumControls.Controls.Add(this.enumMemberName);
            this.enumControls.Controls.Add(this.enumName);
            this.enumControls.Controls.Add(this.enumMemberDefault);
            this.enumControls.Controls.Add(label31);
            this.enumControls.Controls.Add(this.enumMemberValue);
            this.enumControls.Controls.Add(this.enumMemberList);
            this.enumControls.Controls.Add(label30);
            this.enumControls.Controls.Add(this.enumMemberRemoveButton);
            this.enumControls.Controls.Add(this.enumMemberUpButton);
            this.enumControls.Controls.Add(this.enumMemberDownButton);
            this.enumControls.Controls.Add(this.enumMemberAddButton);
            this.enumControls.Enabled = false;
            this.enumControls.Location = new System.Drawing.Point(6, 33);
            this.enumControls.Name = "enumControls";
            this.enumControls.Size = new System.Drawing.Size(282, 169);
            this.enumControls.TabIndex = 24;
            // 
            // enumMemberName
            // 
            this.enumMemberName.Location = new System.Drawing.Point(173, 83);
            this.enumMemberName.Name = "enumMemberName";
            this.enumMemberName.Size = new System.Drawing.Size(100, 20);
            this.enumMemberName.TabIndex = 24;
            this.enumMemberName.TextChanged += new System.EventHandler(this.enumMemberName_TextChanged);
            // 
            // enumName
            // 
            this.enumName.Location = new System.Drawing.Point(47, 3);
            this.enumName.Name = "enumName";
            this.enumName.Size = new System.Drawing.Size(100, 20);
            this.enumName.TabIndex = 1;
            this.enumName.TextChanged += new System.EventHandler(this.enumName_TextChanged);
            // 
            // enumMemberDefault
            // 
            this.enumMemberDefault.AutoSize = true;
            this.enumMemberDefault.Enabled = false;
            this.enumMemberDefault.Location = new System.Drawing.Point(219, 110);
            this.enumMemberDefault.Name = "enumMemberDefault";
            this.enumMemberDefault.Size = new System.Drawing.Size(60, 17);
            this.enumMemberDefault.TabIndex = 23;
            this.enumMemberDefault.Text = "Default";
            this.enumMemberDefault.UseVisualStyleBackColor = true;
            this.enumMemberDefault.CheckedChanged += new System.EventHandler(this.enumMemberDefault_CheckedChanged);
            // 
            // enumMemberValue
            // 
            this.enumMemberValue.Enabled = false;
            this.enumMemberValue.Hexadecimal = true;
            this.enumMemberValue.Location = new System.Drawing.Point(172, 109);
            this.enumMemberValue.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.enumMemberValue.Name = "enumMemberValue";
            this.enumMemberValue.Size = new System.Drawing.Size(41, 20);
            this.enumMemberValue.TabIndex = 22;
            this.enumMemberValue.ValueChanged += new System.EventHandler(this.enumMemberValue_ValueChanged);
            // 
            // enumMemberList
            // 
            this.enumMemberList.FormattingEnabled = true;
            this.enumMemberList.Location = new System.Drawing.Point(3, 29);
            this.enumMemberList.Name = "enumMemberList";
            this.enumMemberList.Size = new System.Drawing.Size(120, 108);
            this.enumMemberList.TabIndex = 16;
            this.enumMemberList.SelectedIndexChanged += new System.EventHandler(this.enumMemberList_SelectedIndexChanged);
            // 
            // enumMemberRemoveButton
            // 
            this.enumMemberRemoveButton.AutoSize = true;
            this.enumMemberRemoveButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.enumMemberRemoveButton.Enabled = false;
            this.enumMemberRemoveButton.Location = new System.Drawing.Point(45, 143);
            this.enumMemberRemoveButton.Name = "enumMemberRemoveButton";
            this.enumMemberRemoveButton.Size = new System.Drawing.Size(57, 23);
            this.enumMemberRemoveButton.TabIndex = 18;
            this.enumMemberRemoveButton.Text = "Remove";
            this.enumMemberRemoveButton.UseVisualStyleBackColor = true;
            this.enumMemberRemoveButton.Click += new System.EventHandler(this.enumMemberRemoveButton_Click);
            // 
            // enumMemberUpButton
            // 
            this.enumMemberUpButton.AutoSize = true;
            this.enumMemberUpButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.enumMemberUpButton.Enabled = false;
            this.enumMemberUpButton.Location = new System.Drawing.Point(129, 29);
            this.enumMemberUpButton.Name = "enumMemberUpButton";
            this.enumMemberUpButton.Size = new System.Drawing.Size(29, 23);
            this.enumMemberUpButton.TabIndex = 19;
            this.enumMemberUpButton.Text = "↑";
            this.enumMemberUpButton.UseVisualStyleBackColor = true;
            this.enumMemberUpButton.Click += new System.EventHandler(this.enumMemberUpButton_Click);
            // 
            // enumMemberDownButton
            // 
            this.enumMemberDownButton.AutoSize = true;
            this.enumMemberDownButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.enumMemberDownButton.Enabled = false;
            this.enumMemberDownButton.Location = new System.Drawing.Point(129, 58);
            this.enumMemberDownButton.Name = "enumMemberDownButton";
            this.enumMemberDownButton.Size = new System.Drawing.Size(29, 23);
            this.enumMemberDownButton.TabIndex = 20;
            this.enumMemberDownButton.Text = "↓";
            this.enumMemberDownButton.UseVisualStyleBackColor = true;
            this.enumMemberDownButton.Click += new System.EventHandler(this.enumMemberDownButton_Click);
            // 
            // enumMemberAddButton
            // 
            this.enumMemberAddButton.AutoSize = true;
            this.enumMemberAddButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.enumMemberAddButton.Location = new System.Drawing.Point(3, 143);
            this.enumMemberAddButton.Name = "enumMemberAddButton";
            this.enumMemberAddButton.Size = new System.Drawing.Size(36, 23);
            this.enumMemberAddButton.TabIndex = 17;
            this.enumMemberAddButton.Text = "Add";
            this.enumMemberAddButton.UseVisualStyleBackColor = true;
            this.enumMemberAddButton.Click += new System.EventHandler(this.enumMemberAddButton_Click);
            // 
            // deleteEnumButton
            // 
            this.deleteEnumButton.AutoSize = true;
            this.deleteEnumButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.deleteEnumButton.Enabled = false;
            this.deleteEnumButton.Location = new System.Drawing.Point(218, 4);
            this.deleteEnumButton.Name = "deleteEnumButton";
            this.deleteEnumButton.Size = new System.Drawing.Size(48, 23);
            this.deleteEnumButton.TabIndex = 15;
            this.deleteEnumButton.Text = "Delete";
            this.deleteEnumButton.UseVisualStyleBackColor = true;
            this.deleteEnumButton.Click += new System.EventHandler(this.deleteEnumButton_Click);
            // 
            // addEnumButton
            // 
            this.addEnumButton.AutoSize = true;
            this.addEnumButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.addEnumButton.Location = new System.Drawing.Point(176, 4);
            this.addEnumButton.Name = "addEnumButton";
            this.addEnumButton.Size = new System.Drawing.Size(36, 23);
            this.addEnumButton.TabIndex = 14;
            this.addEnumButton.Text = "Add";
            this.addEnumButton.UseVisualStyleBackColor = true;
            this.addEnumButton.Click += new System.EventHandler(this.addEnumButton_Click);
            // 
            // selectedEnum
            // 
            this.selectedEnum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectedEnum.FormattingEnabled = true;
            this.selectedEnum.Location = new System.Drawing.Point(49, 6);
            this.selectedEnum.Name = "selectedEnum";
            this.selectedEnum.Size = new System.Drawing.Size(121, 21);
            this.selectedEnum.TabIndex = 12;
            this.selectedEnum.SelectedIndexChanged += new System.EventHandler(this.selectedEnum_SelectedIndexChanged);
            // 
            // tabPage5
            // 
            this.tabPage5.AutoScroll = true;
            this.tabPage5.Controls.Add(this.propertyControls);
            this.tabPage5.Controls.Add(this.deletePropertyButton);
            this.tabPage5.Controls.Add(this.addPropertyButton);
            this.tabPage5.Controls.Add(label33);
            this.tabPage5.Controls.Add(this.selectedProperty);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(576, 454);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Properties";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // propertyControls
            // 
            this.propertyControls.AutoSize = true;
            this.propertyControls.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.propertyControls.Controls.Add(this.customPropertyControls);
            this.propertyControls.Controls.Add(this.bitsPropertyControls);
            this.propertyControls.Controls.Add(this.propertyType);
            this.propertyControls.Controls.Add(label37);
            this.propertyControls.Controls.Add(this.propertyDescription);
            this.propertyControls.Controls.Add(label36);
            this.propertyControls.Controls.Add(this.propertyValueType);
            this.propertyControls.Controls.Add(label35);
            this.propertyControls.Controls.Add(this.propertyName);
            this.propertyControls.Controls.Add(label34);
            this.propertyControls.Location = new System.Drawing.Point(6, 33);
            this.propertyControls.Name = "propertyControls";
            this.propertyControls.Size = new System.Drawing.Size(444, 347);
            this.propertyControls.TabIndex = 12;
            // 
            // customPropertyControls
            // 
            this.customPropertyControls.AutoSize = true;
            this.customPropertyControls.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.customPropertyControls.Controls.Add(this.overrideDefaultProperty);
            this.customPropertyControls.Controls.Add(this.setMethod);
            this.customPropertyControls.Controls.Add(label41);
            this.customPropertyControls.Controls.Add(this.getMethod);
            this.customPropertyControls.Controls.Add(label40);
            this.customPropertyControls.Location = new System.Drawing.Point(0, 109);
            this.customPropertyControls.Name = "customPropertyControls";
            this.customPropertyControls.Size = new System.Drawing.Size(278, 235);
            this.customPropertyControls.TabIndex = 9;
            this.customPropertyControls.Visible = false;
            // 
            // overrideDefaultProperty
            // 
            this.overrideDefaultProperty.AutoSize = true;
            this.overrideDefaultProperty.Location = new System.Drawing.Point(75, 215);
            this.overrideDefaultProperty.Name = "overrideDefaultProperty";
            this.overrideDefaultProperty.Size = new System.Drawing.Size(150, 17);
            this.overrideDefaultProperty.TabIndex = 4;
            this.overrideDefaultProperty.Text = "Overrides Default Property";
            this.overrideDefaultProperty.UseVisualStyleBackColor = true;
            this.overrideDefaultProperty.CheckedChanged += new System.EventHandler(this.overrideDefaultProperty_CheckedChanged);
            // 
            // setMethod
            // 
            this.setMethod.AcceptsReturn = true;
            this.setMethod.AcceptsTab = true;
            this.setMethod.Location = new System.Drawing.Point(75, 109);
            this.setMethod.Multiline = true;
            this.setMethod.Name = "setMethod";
            this.setMethod.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.setMethod.Size = new System.Drawing.Size(200, 100);
            this.setMethod.TabIndex = 3;
            this.setMethod.TextChanged += new System.EventHandler(this.setMethod_TextChanged);
            // 
            // getMethod
            // 
            this.getMethod.AcceptsReturn = true;
            this.getMethod.AcceptsTab = true;
            this.getMethod.Location = new System.Drawing.Point(75, 3);
            this.getMethod.Multiline = true;
            this.getMethod.Name = "getMethod";
            this.getMethod.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.getMethod.Size = new System.Drawing.Size(200, 100);
            this.getMethod.TabIndex = 1;
            this.getMethod.TextChanged += new System.EventHandler(this.getMethod_TextChanged);
            // 
            // bitsPropertyControls
            // 
            this.bitsPropertyControls.AutoSize = true;
            this.bitsPropertyControls.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.bitsPropertyControls.Controls.Add(this.bitLength);
            this.bitsPropertyControls.Controls.Add(label39);
            this.bitsPropertyControls.Controls.Add(this.startBit);
            this.bitsPropertyControls.Controls.Add(label38);
            this.bitsPropertyControls.Location = new System.Drawing.Point(0, 109);
            this.bitsPropertyControls.Name = "bitsPropertyControls";
            this.bitsPropertyControls.Size = new System.Drawing.Size(101, 52);
            this.bitsPropertyControls.TabIndex = 8;
            this.bitsPropertyControls.Visible = false;
            // 
            // bitLength
            // 
            this.bitLength.Location = new System.Drawing.Point(52, 29);
            this.bitLength.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.bitLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.bitLength.Name = "bitLength";
            this.bitLength.Size = new System.Drawing.Size(42, 20);
            this.bitLength.TabIndex = 3;
            this.bitLength.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.bitLength.ValueChanged += new System.EventHandler(this.bitLength_ValueChanged);
            // 
            // startBit
            // 
            this.startBit.Location = new System.Drawing.Point(56, 3);
            this.startBit.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.startBit.Name = "startBit";
            this.startBit.Size = new System.Drawing.Size(42, 20);
            this.startBit.TabIndex = 1;
            this.startBit.ValueChanged += new System.EventHandler(this.startBit_ValueChanged);
            // 
            // propertyType
            // 
            this.propertyType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.propertyType.FormattingEnabled = true;
            this.propertyType.Items.AddRange(new object[] {
            "Bits",
            "Custom"});
            this.propertyType.Location = new System.Drawing.Point(85, 82);
            this.propertyType.Name = "propertyType";
            this.propertyType.Size = new System.Drawing.Size(121, 21);
            this.propertyType.TabIndex = 7;
            this.propertyType.SelectedIndexChanged += new System.EventHandler(this.propertyType_SelectedIndexChanged);
            // 
            // propertyDescription
            // 
            this.propertyDescription.Location = new System.Drawing.Point(72, 56);
            this.propertyDescription.Name = "propertyDescription";
            this.propertyDescription.Size = new System.Drawing.Size(369, 20);
            this.propertyDescription.TabIndex = 5;
            this.propertyDescription.TextChanged += new System.EventHandler(this.propertyDescription_TextChanged);
            // 
            // propertyValueType
            // 
            this.propertyValueType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.propertyValueType.FormattingEnabled = true;
            this.propertyValueType.Items.AddRange(new object[] {
            "Boolean",
            "Byte",
            "Integer"});
            this.propertyValueType.Location = new System.Drawing.Point(73, 29);
            this.propertyValueType.Name = "propertyValueType";
            this.propertyValueType.Size = new System.Drawing.Size(121, 21);
            this.propertyValueType.TabIndex = 3;
            this.propertyValueType.SelectedIndexChanged += new System.EventHandler(this.propertyValueType_SelectedIndexChanged);
            // 
            // propertyName
            // 
            this.propertyName.Location = new System.Drawing.Point(47, 3);
            this.propertyName.Name = "propertyName";
            this.propertyName.Size = new System.Drawing.Size(120, 20);
            this.propertyName.TabIndex = 1;
            this.propertyName.TextChanged += new System.EventHandler(this.propertyName_TextChanged);
            // 
            // deletePropertyButton
            // 
            this.deletePropertyButton.AutoSize = true;
            this.deletePropertyButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.deletePropertyButton.Enabled = false;
            this.deletePropertyButton.Location = new System.Drawing.Point(230, 4);
            this.deletePropertyButton.Name = "deletePropertyButton";
            this.deletePropertyButton.Size = new System.Drawing.Size(48, 23);
            this.deletePropertyButton.TabIndex = 11;
            this.deletePropertyButton.Text = "Delete";
            this.deletePropertyButton.UseVisualStyleBackColor = true;
            this.deletePropertyButton.Click += new System.EventHandler(this.deletePropertyButton_Click);
            // 
            // addPropertyButton
            // 
            this.addPropertyButton.AutoSize = true;
            this.addPropertyButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.addPropertyButton.Location = new System.Drawing.Point(188, 4);
            this.addPropertyButton.Name = "addPropertyButton";
            this.addPropertyButton.Size = new System.Drawing.Size(36, 23);
            this.addPropertyButton.TabIndex = 10;
            this.addPropertyButton.Text = "Add";
            this.addPropertyButton.UseVisualStyleBackColor = true;
            this.addPropertyButton.Click += new System.EventHandler(this.addPropertyButton_Click);
            // 
            // selectedProperty
            // 
            this.selectedProperty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectedProperty.FormattingEnabled = true;
            this.selectedProperty.Location = new System.Drawing.Point(61, 6);
            this.selectedProperty.Name = "selectedProperty";
            this.selectedProperty.Size = new System.Drawing.Size(121, 21);
            this.selectedProperty.TabIndex = 8;
            this.selectedProperty.SelectedIndexChanged += new System.EventHandler(this.selectedProperty_SelectedIndexChanged);
            // 
            // tabPage6
            // 
            this.tabPage6.AutoScroll = true;
            this.tabPage6.Controls.Add(this.deleteDisplayOptionButton);
            this.tabPage6.Controls.Add(this.addDisplayOptionButton);
            this.tabPage6.Controls.Add(this.displayPreview);
            this.tabPage6.Controls.Add(this.displayOptionControls);
            this.tabPage6.Controls.Add(this.selectedDisplayOption);
            this.tabPage6.Controls.Add(label42);
            this.tabPage6.Location = new System.Drawing.Point(4, 22);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage6.Size = new System.Drawing.Size(576, 454);
            this.tabPage6.TabIndex = 5;
            this.tabPage6.Text = "Display";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // deleteDisplayOptionButton
            // 
            this.deleteDisplayOptionButton.AutoSize = true;
            this.deleteDisplayOptionButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.deleteDisplayOptionButton.Enabled = false;
            this.deleteDisplayOptionButton.Location = new System.Drawing.Point(378, 4);
            this.deleteDisplayOptionButton.Name = "deleteDisplayOptionButton";
            this.deleteDisplayOptionButton.Size = new System.Drawing.Size(48, 23);
            this.deleteDisplayOptionButton.TabIndex = 13;
            this.deleteDisplayOptionButton.Text = "Delete";
            this.deleteDisplayOptionButton.UseVisualStyleBackColor = true;
            this.deleteDisplayOptionButton.Click += new System.EventHandler(this.deleteDisplayOptionButton_Click);
            // 
            // addDisplayOptionButton
            // 
            this.addDisplayOptionButton.AutoSize = true;
            this.addDisplayOptionButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.addDisplayOptionButton.Location = new System.Drawing.Point(336, 4);
            this.addDisplayOptionButton.Name = "addDisplayOptionButton";
            this.addDisplayOptionButton.Size = new System.Drawing.Size(36, 23);
            this.addDisplayOptionButton.TabIndex = 12;
            this.addDisplayOptionButton.Text = "Add";
            this.addDisplayOptionButton.UseVisualStyleBackColor = true;
            this.addDisplayOptionButton.Click += new System.EventHandler(this.addDisplayOptionButton_Click);
            // 
            // displayPreview
            // 
            this.displayPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.displayPreview.Location = new System.Drawing.Point(292, 33);
            this.displayPreview.Name = "displayPreview";
            this.displayPreview.Size = new System.Drawing.Size(100, 100);
            this.displayPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.displayPreview.TabIndex = 3;
            this.displayPreview.TabStop = false;
            // 
            // displayOptionControls
            // 
            this.displayOptionControls.AutoSize = true;
            this.displayOptionControls.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.displayOptionControls.Controls.Add(this.groupBox3);
            this.displayOptionControls.Controls.Add(this.groupBox2);
            this.displayOptionControls.Enabled = false;
            this.displayOptionControls.Location = new System.Drawing.Point(6, 33);
            this.displayOptionControls.Name = "displayOptionControls";
            this.displayOptionControls.Size = new System.Drawing.Size(280, 257);
            this.displayOptionControls.TabIndex = 2;
            // 
            // groupBox3
            // 
            this.groupBox3.AutoSize = true;
            this.groupBox3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox3.Controls.Add(this.removeImageRefButton);
            this.groupBox3.Controls.Add(this.addImageRefButton);
            this.groupBox3.Controls.Add(this.selectedImageRef);
            this.groupBox3.Controls.Add(this.imageRefControls);
            this.groupBox3.Location = new System.Drawing.Point(3, 90);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.groupBox3.Size = new System.Drawing.Size(249, 164);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Images";
            // 
            // removeImageRefButton
            // 
            this.removeImageRefButton.AutoSize = true;
            this.removeImageRefButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.removeImageRefButton.Enabled = false;
            this.removeImageRefButton.Location = new System.Drawing.Point(186, 17);
            this.removeImageRefButton.Name = "removeImageRefButton";
            this.removeImageRefButton.Size = new System.Drawing.Size(57, 23);
            this.removeImageRefButton.TabIndex = 5;
            this.removeImageRefButton.Text = "Remove";
            this.removeImageRefButton.UseVisualStyleBackColor = true;
            this.removeImageRefButton.Click += new System.EventHandler(this.removeImageRefButton_Click);
            // 
            // addImageRefButton
            // 
            this.addImageRefButton.AutoSize = true;
            this.addImageRefButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.addImageRefButton.Location = new System.Drawing.Point(144, 17);
            this.addImageRefButton.Name = "addImageRefButton";
            this.addImageRefButton.Size = new System.Drawing.Size(36, 23);
            this.addImageRefButton.TabIndex = 4;
            this.addImageRefButton.Text = "Add";
            this.addImageRefButton.UseVisualStyleBackColor = true;
            this.addImageRefButton.Click += new System.EventHandler(this.addImageRefButton_Click);
            // 
            // selectedImageRef
            // 
            this.selectedImageRef.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectedImageRef.FormattingEnabled = true;
            this.selectedImageRef.Location = new System.Drawing.Point(6, 19);
            this.selectedImageRef.Name = "selectedImageRef";
            this.selectedImageRef.Size = new System.Drawing.Size(132, 21);
            this.selectedImageRef.TabIndex = 1;
            this.selectedImageRef.SelectedIndexChanged += new System.EventHandler(this.selectedImageRef_SelectedIndexChanged);
            // 
            // imageRefControls
            // 
            this.imageRefControls.AutoSize = true;
            this.imageRefControls.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.imageRefControls.Controls.Add(this.imageRefYFlip);
            this.imageRefControls.Controls.Add(label47);
            this.imageRefControls.Controls.Add(this.imageRefXFlip);
            this.imageRefControls.Controls.Add(label46);
            this.imageRefControls.Controls.Add(this.imageRefImage);
            this.imageRefControls.Controls.Add(this.imageRefOffsetY);
            this.imageRefControls.Controls.Add(this.imageRefOffsetX);
            this.imageRefControls.Controls.Add(label45);
            this.imageRefControls.Controls.Add(label44);
            this.imageRefControls.Enabled = false;
            this.imageRefControls.Location = new System.Drawing.Point(3, 46);
            this.imageRefControls.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.imageRefControls.Name = "imageRefControls";
            this.imageRefControls.Size = new System.Drawing.Size(216, 105);
            this.imageRefControls.TabIndex = 0;
            // 
            // imageRefYFlip
            // 
            this.imageRefYFlip.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.imageRefYFlip.FormattingEnabled = true;
            this.imageRefYFlip.Items.AddRange(new object[] {
            "Normal",
            "Reversed",
            "Never",
            "Always"});
            this.imageRefYFlip.Location = new System.Drawing.Point(92, 81);
            this.imageRefYFlip.Name = "imageRefYFlip";
            this.imageRefYFlip.Size = new System.Drawing.Size(121, 21);
            this.imageRefYFlip.TabIndex = 9;
            this.imageRefYFlip.SelectedIndexChanged += new System.EventHandler(this.imageRefYFlip_SelectedIndexChanged);
            // 
            // imageRefXFlip
            // 
            this.imageRefXFlip.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.imageRefXFlip.FormattingEnabled = true;
            this.imageRefXFlip.Items.AddRange(new object[] {
            "Normal",
            "Reversed",
            "Never",
            "Always"});
            this.imageRefXFlip.Location = new System.Drawing.Point(92, 56);
            this.imageRefXFlip.Name = "imageRefXFlip";
            this.imageRefXFlip.Size = new System.Drawing.Size(121, 21);
            this.imageRefXFlip.TabIndex = 7;
            this.imageRefXFlip.SelectedIndexChanged += new System.EventHandler(this.imageRefXFlip_SelectedIndexChanged);
            // 
            // imageRefImage
            // 
            this.imageRefImage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.imageRefImage.FormattingEnabled = true;
            this.imageRefImage.Location = new System.Drawing.Point(48, 3);
            this.imageRefImage.Name = "imageRefImage";
            this.imageRefImage.Size = new System.Drawing.Size(121, 21);
            this.imageRefImage.TabIndex = 0;
            this.imageRefImage.SelectedIndexChanged += new System.EventHandler(this.imageRefImage_SelectedIndexChanged);
            // 
            // imageRefOffsetY
            // 
            this.imageRefOffsetY.Location = new System.Drawing.Point(117, 30);
            this.imageRefOffsetY.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.imageRefOffsetY.Minimum = new decimal(new int[] {
            65536,
            0,
            0,
            -2147483648});
            this.imageRefOffsetY.Name = "imageRefOffsetY";
            this.imageRefOffsetY.Size = new System.Drawing.Size(64, 20);
            this.imageRefOffsetY.TabIndex = 2;
            this.imageRefOffsetY.ValueChanged += new System.EventHandler(this.imageRefOffsetY_ValueChanged);
            // 
            // imageRefOffsetX
            // 
            this.imageRefOffsetX.Location = new System.Drawing.Point(47, 30);
            this.imageRefOffsetX.Maximum = new decimal(new int[] {
            65536,
            0,
            0,
            0});
            this.imageRefOffsetX.Minimum = new decimal(new int[] {
            65536,
            0,
            0,
            -2147483648});
            this.imageRefOffsetX.Name = "imageRefOffsetX";
            this.imageRefOffsetX.Size = new System.Drawing.Size(64, 20);
            this.imageRefOffsetX.TabIndex = 1;
            this.imageRefOffsetX.ValueChanged += new System.EventHandler(this.imageRefOffsetX_ValueChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.AutoSize = true;
            this.groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox2.Controls.Add(this.conditionValueNum);
            this.groupBox2.Controls.Add(this.conditionValueEnum);
            this.groupBox2.Controls.Add(label43);
            this.groupBox2.Controls.Add(this.conditionProperty);
            this.groupBox2.Controls.Add(this.removeConditionButton);
            this.groupBox2.Controls.Add(this.addConditionButton);
            this.groupBox2.Controls.Add(this.selectedCondition);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.groupBox2.Size = new System.Drawing.Size(274, 81);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Conditions";
            // 
            // conditionValueNum
            // 
            this.conditionValueNum.Enabled = false;
            this.conditionValueNum.Hexadecimal = true;
            this.conditionValueNum.Location = new System.Drawing.Point(147, 48);
            this.conditionValueNum.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.conditionValueNum.Name = "conditionValueNum";
            this.conditionValueNum.Size = new System.Drawing.Size(120, 20);
            this.conditionValueNum.TabIndex = 7;
            this.conditionValueNum.Visible = false;
            this.conditionValueNum.ValueChanged += new System.EventHandler(this.conditionValueNum_ValueChanged);
            // 
            // conditionValueEnum
            // 
            this.conditionValueEnum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.conditionValueEnum.Enabled = false;
            this.conditionValueEnum.FormattingEnabled = true;
            this.conditionValueEnum.Location = new System.Drawing.Point(147, 47);
            this.conditionValueEnum.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.conditionValueEnum.Name = "conditionValueEnum";
            this.conditionValueEnum.Size = new System.Drawing.Size(121, 21);
            this.conditionValueEnum.TabIndex = 6;
            this.conditionValueEnum.SelectedIndexChanged += new System.EventHandler(this.conditionValueEnum_SelectedIndexChanged);
            // 
            // conditionProperty
            // 
            this.conditionProperty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.conditionProperty.Enabled = false;
            this.conditionProperty.FormattingEnabled = true;
            this.conditionProperty.Location = new System.Drawing.Point(7, 47);
            this.conditionProperty.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.conditionProperty.Name = "conditionProperty";
            this.conditionProperty.Size = new System.Drawing.Size(121, 21);
            this.conditionProperty.TabIndex = 4;
            this.conditionProperty.SelectedIndexChanged += new System.EventHandler(this.conditionProperty_SelectedIndexChanged);
            // 
            // removeConditionButton
            // 
            this.removeConditionButton.AutoSize = true;
            this.removeConditionButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.removeConditionButton.Enabled = false;
            this.removeConditionButton.Location = new System.Drawing.Point(198, 17);
            this.removeConditionButton.Name = "removeConditionButton";
            this.removeConditionButton.Size = new System.Drawing.Size(57, 23);
            this.removeConditionButton.TabIndex = 3;
            this.removeConditionButton.Text = "Remove";
            this.removeConditionButton.UseVisualStyleBackColor = true;
            this.removeConditionButton.Click += new System.EventHandler(this.removeConditionButton_Click);
            // 
            // addConditionButton
            // 
            this.addConditionButton.AutoSize = true;
            this.addConditionButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.addConditionButton.Location = new System.Drawing.Point(156, 17);
            this.addConditionButton.Name = "addConditionButton";
            this.addConditionButton.Size = new System.Drawing.Size(36, 23);
            this.addConditionButton.TabIndex = 2;
            this.addConditionButton.Text = "Add";
            this.addConditionButton.UseVisualStyleBackColor = true;
            this.addConditionButton.Click += new System.EventHandler(this.addConditionButton_Click);
            // 
            // selectedCondition
            // 
            this.selectedCondition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectedCondition.FormattingEnabled = true;
            this.selectedCondition.Location = new System.Drawing.Point(6, 19);
            this.selectedCondition.Name = "selectedCondition";
            this.selectedCondition.Size = new System.Drawing.Size(144, 21);
            this.selectedCondition.TabIndex = 1;
            this.selectedCondition.SelectedIndexChanged += new System.EventHandler(this.selectedCondition_SelectedIndexChanged);
            // 
            // selectedDisplayOption
            // 
            this.selectedDisplayOption.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectedDisplayOption.FormattingEnabled = true;
            this.selectedDisplayOption.Location = new System.Drawing.Point(53, 6);
            this.selectedDisplayOption.Name = "selectedDisplayOption";
            this.selectedDisplayOption.Size = new System.Drawing.Size(277, 21);
            this.selectedDisplayOption.TabIndex = 1;
            this.selectedDisplayOption.SelectedIndexChanged += new System.EventHandler(this.selectedDisplayOption_SelectedIndexChanged);
            // 
            // objectListList
            // 
            this.objectListList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.objectListList.FormattingEnabled = true;
            this.objectListList.Location = new System.Drawing.Point(78, 6);
            this.objectListList.Name = "objectListList";
            this.objectListList.Size = new System.Drawing.Size(160, 21);
            this.objectListList.TabIndex = 3;
            this.objectListList.SelectedIndexChanged += new System.EventHandler(this.objectListList_SelectedIndexChanged);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // objectDefinitionList
            // 
            this.objectDefinitionList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.objectDefinitionList.FormattingEnabled = true;
            this.objectDefinitionList.Location = new System.Drawing.Point(106, 33);
            this.objectDefinitionList.Name = "objectDefinitionList";
            this.objectDefinitionList.Size = new System.Drawing.Size(132, 21);
            this.objectDefinitionList.TabIndex = 5;
            this.objectDefinitionList.SelectedIndexChanged += new System.EventHandler(this.objectDefinitionList_SelectedIndexChanged);
            // 
            // listPanel
            // 
            this.listPanel.AutoSize = true;
            this.listPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.listPanel.Controls.Add(this.deleteDefinitionButton);
            this.listPanel.Controls.Add(this.addDefinitionButton);
            this.listPanel.Controls.Add(label7);
            this.listPanel.Controls.Add(this.objectDefinitionList);
            this.listPanel.Controls.Add(this.objectListList);
            this.listPanel.Controls.Add(label8);
            this.listPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.listPanel.Enabled = false;
            this.listPanel.Location = new System.Drawing.Point(0, 24);
            this.listPanel.Name = "listPanel";
            this.listPanel.Padding = new System.Windows.Forms.Padding(3);
            this.listPanel.Size = new System.Drawing.Size(584, 60);
            this.listPanel.TabIndex = 6;
            // 
            // deleteDefinitionButton
            // 
            this.deleteDefinitionButton.AutoSize = true;
            this.deleteDefinitionButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.deleteDefinitionButton.Enabled = false;
            this.deleteDefinitionButton.Location = new System.Drawing.Point(286, 31);
            this.deleteDefinitionButton.Name = "deleteDefinitionButton";
            this.deleteDefinitionButton.Size = new System.Drawing.Size(48, 23);
            this.deleteDefinitionButton.TabIndex = 7;
            this.deleteDefinitionButton.Text = "Delete";
            this.deleteDefinitionButton.UseVisualStyleBackColor = true;
            // 
            // addDefinitionButton
            // 
            this.addDefinitionButton.AutoSize = true;
            this.addDefinitionButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.addDefinitionButton.Enabled = false;
            this.addDefinitionButton.Location = new System.Drawing.Point(244, 31);
            this.addDefinitionButton.Name = "addDefinitionButton";
            this.addDefinitionButton.Size = new System.Drawing.Size(36, 23);
            this.addDefinitionButton.TabIndex = 6;
            this.addDefinitionButton.Text = "Add";
            this.addDefinitionButton.UseVisualStyleBackColor = true;
            this.addDefinitionButton.Click += new System.EventHandler(this.addDefinitionButton_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reportBugToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // reportBugToolStripMenuItem
            // 
            this.reportBugToolStripMenuItem.Name = "reportBugToolStripMenuItem";
            this.reportBugToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.reportBugToolStripMenuItem.Text = "&Report Bug...";
            this.reportBugToolStripMenuItem.Click += new System.EventHandler(this.reportBugToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 564);
            this.Controls.Add(this.editorTabs);
            this.Controls.Add(this.listPanel);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "SonLVL Object Definition Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.editorTabs.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.defImagePreview)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.selectedImagePreview)).EndInit();
            this.imageControls.ResumeLayout(false);
            this.imageControls.PerformLayout();
            this.mappingsImageControls.ResumeLayout(false);
            this.mappingsImageControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mappingsPalette)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mappingsFrame)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dplcFile)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mappingsFile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.artFilename)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.artOffset)).EndInit();
            this.spriteImageControls.ResumeLayout(false);
            this.spriteImageControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spriteNum)).EndInit();
            this.bitmapImageControls.ResumeLayout(false);
            this.bitmapImageControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bitmapFilename)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageOffsetY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageOffsetX)).EndInit();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.subtypeControls.ResumeLayout(false);
            this.subtypeControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.subtypeImagePreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.subtypeID)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.enumControls.ResumeLayout(false);
            this.enumControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.enumMemberValue)).EndInit();
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.propertyControls.ResumeLayout(false);
            this.propertyControls.PerformLayout();
            this.customPropertyControls.ResumeLayout(false);
            this.customPropertyControls.PerformLayout();
            this.bitsPropertyControls.ResumeLayout(false);
            this.bitsPropertyControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bitLength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.startBit)).EndInit();
            this.tabPage6.ResumeLayout(false);
            this.tabPage6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.displayPreview)).EndInit();
            this.displayOptionControls.ResumeLayout(false);
            this.displayOptionControls.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.imageRefControls.ResumeLayout(false);
            this.imageRefControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageRefOffsetY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageRefOffsetX)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.conditionValueNum)).EndInit();
            this.listPanel.ResumeLayout(false);
            this.listPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem recentProjectsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TabControl editorTabs;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button deleteImageButton;
        private System.Windows.Forms.Button addImageButton;
        private System.Windows.Forms.ComboBox selectedImage;
        private System.Windows.Forms.TextBox defName;
        private System.Windows.Forms.TextBox defNamespace;
        private System.Windows.Forms.TextBox defType;
        private System.Windows.Forms.ComboBox defLanguage;
        private System.Windows.Forms.ComboBox defImage;
        private System.Windows.Forms.CheckBox defDebug;
        private System.Windows.Forms.CheckBox defRemember;
        private System.Windows.Forms.PictureBox defImagePreview;
        private System.Windows.Forms.ToolStripMenuItem noneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeLevelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noneToolStripMenuItem1;
        private System.Windows.Forms.ComboBox objectListList;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ComboBox objectDefinitionList;
        private System.Windows.Forms.Panel listPanel;
        private System.Windows.Forms.Panel imageControls;
        private System.Windows.Forms.TextBox imageName;
        private System.Windows.Forms.NumericUpDown imageOffsetX;
        private System.Windows.Forms.NumericUpDown imageOffsetY;
        private System.Windows.Forms.ComboBox imageType;
        private System.Windows.Forms.Panel mappingsImageControls;
        private System.Windows.Forms.NumericUpDown artOffset;
        private System.Windows.Forms.ListBox artList;
        private System.Windows.Forms.Button artDownButton;
        private System.Windows.Forms.Button artAddButton;
        private System.Windows.Forms.Button artUpButton;
        private System.Windows.Forms.Button artRemoveButton;
        private System.Windows.Forms.CheckBox defaultArtOffset;
        private FileSelector artFilename;
        private FileSelector mappingsFile;
        private System.Windows.Forms.CheckBox useLevelArt;
        private System.Windows.Forms.ComboBox artCompression;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton mappingsBinary;
        private System.Windows.Forms.RadioButton mappingsASM;
        private System.Windows.Forms.TextBox mappingsLabel;
        private System.Windows.Forms.ComboBox mappingsFormat;
        private System.Windows.Forms.ComboBox dplcFormat;
        private System.Windows.Forms.TextBox dplcLabel;
        private FileSelector dplcFile;
        private System.Windows.Forms.NumericUpDown mappingsFrame;
        private System.Windows.Forms.NumericUpDown mappingsPalette;
        private System.Windows.Forms.PictureBox selectedImagePreview;
        private System.Windows.Forms.Panel bitmapImageControls;
        private FileSelector bitmapFilename;
        private System.Windows.Forms.Panel spriteImageControls;
        private System.Windows.Forms.NumericUpDown spriteNum;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button deleteSubtypeButton;
        private System.Windows.Forms.Button addSubtypeButton;
        private System.Windows.Forms.ComboBox selectedSubtype;
        private System.Windows.Forms.Panel subtypeControls;
        private System.Windows.Forms.ComboBox subtypeImage;
        private System.Windows.Forms.TextBox subtypeName;
        private System.Windows.Forms.NumericUpDown subtypeID;
        private System.Windows.Forms.PictureBox subtypeImagePreview;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button deleteEnumButton;
        private System.Windows.Forms.Button addEnumButton;
        private System.Windows.Forms.ComboBox selectedEnum;
        private System.Windows.Forms.Panel enumControls;
        private System.Windows.Forms.CheckBox enumMemberDefault;
        private System.Windows.Forms.NumericUpDown enumMemberValue;
        private System.Windows.Forms.ListBox enumMemberList;
        private System.Windows.Forms.Button enumMemberDownButton;
        private System.Windows.Forms.Button enumMemberAddButton;
        private System.Windows.Forms.Button enumMemberUpButton;
        private System.Windows.Forms.Button enumMemberRemoveButton;
        private System.Windows.Forms.TextBox enumName;
        private System.Windows.Forms.TextBox enumMemberName;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Button addDefinitionButton;
        private System.Windows.Forms.Button deleteDefinitionButton;
        private System.Windows.Forms.Button deletePropertyButton;
        private System.Windows.Forms.Button addPropertyButton;
        private System.Windows.Forms.ComboBox selectedProperty;
        private System.Windows.Forms.Panel propertyControls;
        private System.Windows.Forms.TextBox propertyName;
        private System.Windows.Forms.ComboBox propertyValueType;
        private System.Windows.Forms.TextBox propertyDescription;
        private System.Windows.Forms.ComboBox propertyType;
        private System.Windows.Forms.Panel bitsPropertyControls;
        private System.Windows.Forms.NumericUpDown startBit;
        private System.Windows.Forms.NumericUpDown bitLength;
        private System.Windows.Forms.Panel customPropertyControls;
        private System.Windows.Forms.TextBox setMethod;
        private System.Windows.Forms.TextBox getMethod;
        private System.Windows.Forms.CheckBox overrideDefaultProperty;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.ComboBox selectedDisplayOption;
        private System.Windows.Forms.Panel displayOptionControls;
        private System.Windows.Forms.ComboBox selectedCondition;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button removeConditionButton;
        private System.Windows.Forms.Button addConditionButton;
        private System.Windows.Forms.ComboBox conditionProperty;
        private System.Windows.Forms.ComboBox conditionValueEnum;
        private System.Windows.Forms.NumericUpDown conditionValueNum;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Panel imageRefControls;
        private System.Windows.Forms.NumericUpDown imageRefOffsetY;
        private System.Windows.Forms.NumericUpDown imageRefOffsetX;
        private System.Windows.Forms.ComboBox selectedImageRef;
        private System.Windows.Forms.Button removeImageRefButton;
        private System.Windows.Forms.Button addImageRefButton;
        private System.Windows.Forms.ComboBox imageRefImage;
        private System.Windows.Forms.PictureBox displayPreview;
        private System.Windows.Forms.Button deleteDisplayOptionButton;
        private System.Windows.Forms.Button addDisplayOptionButton;
        private System.Windows.Forms.ComboBox imageRefXFlip;
        private System.Windows.Forms.ComboBox imageRefYFlip;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reportBugToolStripMenuItem;
    }
}

