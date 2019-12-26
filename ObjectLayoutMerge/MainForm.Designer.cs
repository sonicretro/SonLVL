namespace ObjectLayoutMerge
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
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.uncheckAllObjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.checkAllObjectsInBothToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.checkAllObjectsInAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.checkAllObjectsInBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.checkAllObjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showOnlySelectedObjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showOverlaysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.objectListView = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.previewPanel = new ObjectLayoutMerge.ScrollingPanel();
			this.nextAToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.nextBToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.nextDiffToolStripButton = new System.Windows.Forms.ToolStripButton();
			this.menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem});
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
            this.saveToolStripMenuItem,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
			this.openToolStripMenuItem.Text = "&Open...";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
			this.saveToolStripMenuItem.Text = "&Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uncheckAllObjectsToolStripMenuItem,
            this.checkAllObjectsInBothToolStripMenuItem,
            this.checkAllObjectsInAToolStripMenuItem,
            this.checkAllObjectsInBToolStripMenuItem,
            this.checkAllObjectsToolStripMenuItem});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
			this.editToolStripMenuItem.Text = "&Edit";
			// 
			// uncheckAllObjectsToolStripMenuItem
			// 
			this.uncheckAllObjectsToolStripMenuItem.Name = "uncheckAllObjectsToolStripMenuItem";
			this.uncheckAllObjectsToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
			this.uncheckAllObjectsToolStripMenuItem.Text = "&Uncheck All Objects";
			this.uncheckAllObjectsToolStripMenuItem.Click += new System.EventHandler(this.uncheckAllObjectsToolStripMenuItem_Click);
			// 
			// checkAllObjectsInBothToolStripMenuItem
			// 
			this.checkAllObjectsInBothToolStripMenuItem.Name = "checkAllObjectsInBothToolStripMenuItem";
			this.checkAllObjectsInBothToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
			this.checkAllObjectsInBothToolStripMenuItem.Text = "Check All Objects in B&oth";
			this.checkAllObjectsInBothToolStripMenuItem.Click += new System.EventHandler(this.checkAllObjectsInBothToolStripMenuItem_Click);
			// 
			// checkAllObjectsInAToolStripMenuItem
			// 
			this.checkAllObjectsInAToolStripMenuItem.Name = "checkAllObjectsInAToolStripMenuItem";
			this.checkAllObjectsInAToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
			this.checkAllObjectsInAToolStripMenuItem.Text = "Check All Objects in &A Only";
			this.checkAllObjectsInAToolStripMenuItem.Click += new System.EventHandler(this.checkAllObjectsInAToolStripMenuItem_Click);
			// 
			// checkAllObjectsInBToolStripMenuItem
			// 
			this.checkAllObjectsInBToolStripMenuItem.Name = "checkAllObjectsInBToolStripMenuItem";
			this.checkAllObjectsInBToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
			this.checkAllObjectsInBToolStripMenuItem.Text = "Check All Objects in &B Only";
			this.checkAllObjectsInBToolStripMenuItem.Click += new System.EventHandler(this.checkAllObjectsInBToolStripMenuItem_Click);
			// 
			// checkAllObjectsToolStripMenuItem
			// 
			this.checkAllObjectsToolStripMenuItem.Name = "checkAllObjectsToolStripMenuItem";
			this.checkAllObjectsToolStripMenuItem.Size = new System.Drawing.Size(219, 22);
			this.checkAllObjectsToolStripMenuItem.Text = "&Check All Objects";
			this.checkAllObjectsToolStripMenuItem.Click += new System.EventHandler(this.checkAllObjectsToolStripMenuItem_Click);
			// 
			// viewToolStripMenuItem
			// 
			this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showOnlySelectedObjectsToolStripMenuItem,
            this.showOverlaysToolStripMenuItem});
			this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
			this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
			this.viewToolStripMenuItem.Text = "&View";
			// 
			// showOnlySelectedObjectsToolStripMenuItem
			// 
			this.showOnlySelectedObjectsToolStripMenuItem.CheckOnClick = true;
			this.showOnlySelectedObjectsToolStripMenuItem.Name = "showOnlySelectedObjectsToolStripMenuItem";
			this.showOnlySelectedObjectsToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
			this.showOnlySelectedObjectsToolStripMenuItem.Text = "&Show Only Selected Objects";
			this.showOnlySelectedObjectsToolStripMenuItem.CheckedChanged += new System.EventHandler(this.showOnlySelectedObjectsToolStripMenuItem_CheckedChanged);
			// 
			// showOverlaysToolStripMenuItem
			// 
			this.showOverlaysToolStripMenuItem.Checked = true;
			this.showOverlaysToolStripMenuItem.CheckOnClick = true;
			this.showOverlaysToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
			this.showOverlaysToolStripMenuItem.Name = "showOverlaysToolStripMenuItem";
			this.showOverlaysToolStripMenuItem.Size = new System.Drawing.Size(221, 22);
			this.showOverlaysToolStripMenuItem.Text = "Show &Overlays";
			this.showOverlaysToolStripMenuItem.CheckedChanged += new System.EventHandler(this.showOverlaysToolStripMenuItem_CheckedChanged);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 24);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.previewPanel);
			this.splitContainer1.Size = new System.Drawing.Size(584, 537);
			this.splitContainer1.SplitterDistance = 205;
			this.splitContainer1.TabIndex = 1;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.objectListView);
			this.splitContainer2.Panel1.Controls.Add(this.toolStrip1);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.propertyGrid1);
			this.splitContainer2.Size = new System.Drawing.Size(205, 537);
			this.splitContainer2.SplitterDistance = 338;
			this.splitContainer2.TabIndex = 2;
			// 
			// objectListView
			// 
			this.objectListView.CheckBoxes = true;
			this.objectListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
			this.objectListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.objectListView.FullRowSelect = true;
			this.objectListView.GridLines = true;
			this.objectListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.objectListView.HideSelection = false;
			this.objectListView.LabelWrap = false;
			this.objectListView.Location = new System.Drawing.Point(0, 25);
			this.objectListView.MultiSelect = false;
			this.objectListView.Name = "objectListView";
			this.objectListView.ShowGroups = false;
			this.objectListView.Size = new System.Drawing.Size(205, 313);
			this.objectListView.TabIndex = 0;
			this.objectListView.UseCompatibleStateImageBehavior = false;
			this.objectListView.View = System.Windows.Forms.View.Details;
			this.objectListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.objectListView_ItemChecked);
			this.objectListView.SelectedIndexChanged += new System.EventHandler(this.objectListView_SelectedIndexChanged);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Source";
			this.columnHeader1.Width = 49;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Data";
			this.columnHeader2.Width = 146;
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.nextDiffToolStripButton,
            this.nextAToolStripButton,
            this.nextBToolStripButton});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(205, 25);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
			this.propertyGrid1.Size = new System.Drawing.Size(205, 195);
			this.propertyGrid1.TabIndex = 0;
			this.propertyGrid1.ToolbarVisible = false;
			// 
			// previewPanel
			// 
			this.previewPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.previewPanel.HScrollEnabled = true;
			this.previewPanel.HScrollLargeChange = 128;
			this.previewPanel.HScrollMaximum = 32768;
			this.previewPanel.HScrollMinimum = 0;
			this.previewPanel.HScrollSmallChange = 16;
			this.previewPanel.HScrollValue = 0;
			this.previewPanel.Location = new System.Drawing.Point(0, 0);
			this.previewPanel.Name = "previewPanel";
			this.previewPanel.PanelAllowDrop = false;
			this.previewPanel.PanelCursor = System.Windows.Forms.Cursors.Default;
			this.previewPanel.Size = new System.Drawing.Size(375, 537);
			this.previewPanel.TabIndex = 0;
			this.previewPanel.VScrollEnabled = true;
			this.previewPanel.VScrollLargeChange = 128;
			this.previewPanel.VScrollMaximum = 4096;
			this.previewPanel.VScrollMinimum = 0;
			this.previewPanel.VScrollSmallChange = 16;
			this.previewPanel.VScrollValue = 0;
			this.previewPanel.PanelPaint += new System.Windows.Forms.PaintEventHandler(this.previewPanel_PanelPaint);
			this.previewPanel.PanelKeyDown += new System.Windows.Forms.KeyEventHandler(this.previewPanel_PanelKeyDown);
			this.previewPanel.PanelMouseDown += new System.Windows.Forms.MouseEventHandler(this.previewPanel_PanelMouseDown);
			this.previewPanel.PanelMouseMove += new System.Windows.Forms.MouseEventHandler(this.previewPanel_PanelMouseMove);
			this.previewPanel.ScrollBarValueChanged += new System.EventHandler(this.previewPanel_ScrollBarValueChanged);
			// 
			// nextAToolStripButton
			// 
			this.nextAToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.nextAToolStripButton.Name = "nextAToolStripButton";
			this.nextAToolStripButton.Size = new System.Drawing.Size(46, 22);
			this.nextAToolStripButton.Text = "Next A";
			this.nextAToolStripButton.Click += new System.EventHandler(this.nextAToolStripButton_Click);
			// 
			// nextBToolStripButton
			// 
			this.nextBToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.nextBToolStripButton.Name = "nextBToolStripButton";
			this.nextBToolStripButton.Size = new System.Drawing.Size(45, 22);
			this.nextBToolStripButton.Text = "Next B";
			this.nextBToolStripButton.Click += new System.EventHandler(this.nextBToolStripButton_Click);
			// 
			// nextDiffToolStripButton
			// 
			this.nextDiffToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.nextDiffToolStripButton.Name = "nextDiffToolStripButton";
			this.nextDiffToolStripButton.Size = new System.Drawing.Size(57, 22);
			this.nextDiffToolStripButton.Text = "Next Diff";
			this.nextDiffToolStripButton.Click += new System.EventHandler(this.nextDiffToolStripButton_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(584, 561);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainForm";
			this.Text = "Object Layout Merge";
			this.Shown += new System.EventHandler(this.MainForm_Shown);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel1.PerformLayout();
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ListView objectListView;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private ScrollingPanel previewPanel;
		private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem uncheckAllObjectsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem checkAllObjectsInBothToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem checkAllObjectsInAToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem checkAllObjectsInBToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem checkAllObjectsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem showOnlySelectedObjectsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem showOverlaysToolStripMenuItem;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.PropertyGrid propertyGrid1;
		private System.Windows.Forms.ToolStripButton nextAToolStripButton;
		private System.Windows.Forms.ToolStripButton nextDiffToolStripButton;
		private System.Windows.Forms.ToolStripButton nextBToolStripButton;
	}
}

