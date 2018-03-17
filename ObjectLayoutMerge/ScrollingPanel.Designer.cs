namespace ObjectLayoutMerge
{
	partial class ScrollingPanel
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.panel = new SonicRetro.SonLVL.API.KeyboardPanel();
			this.vScrollBar = new System.Windows.Forms.VScrollBar();
			this.hScrollBar = new System.Windows.Forms.HScrollBar();
			this.tableLayoutPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel
			// 
			this.tableLayoutPanel.ColumnCount = 2;
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel.Controls.Add(this.panel, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.vScrollBar, 1, 0);
			this.tableLayoutPanel.Controls.Add(this.hScrollBar, 0, 1);
			this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 2;
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel.Size = new System.Drawing.Size(150, 150);
			this.tableLayoutPanel.TabIndex = 3;
			// 
			// panel
			// 
			this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel.Location = new System.Drawing.Point(0, 0);
			this.panel.Margin = new System.Windows.Forms.Padding(0);
			this.panel.Name = "panel";
			this.panel.Size = new System.Drawing.Size(133, 133);
			this.panel.TabIndex = 1;
			this.panel.Load += new System.EventHandler(this.panel_Load);
			this.panel.DragDrop += new System.Windows.Forms.DragEventHandler(this.panel_DragDrop);
			this.panel.DragEnter += new System.Windows.Forms.DragEventHandler(this.panel_DragEnter);
			this.panel.DragOver += new System.Windows.Forms.DragEventHandler(this.panel_DragOver);
			this.panel.DragLeave += new System.EventHandler(this.panel_DragLeave);
			this.panel.Paint += new System.Windows.Forms.PaintEventHandler(this.panel_Paint);
			this.panel.KeyDown += new System.Windows.Forms.KeyEventHandler(this.panel_KeyDown);
			this.panel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel_MouseDown);
			this.panel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel_MouseMove);
			this.panel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panel_MouseUp);
			this.panel.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.panel_MouseWheel);
			this.panel.Resize += new System.EventHandler(this.panel_Resize);
			// 
			// vScrollBar
			// 
			this.vScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.vScrollBar.Enabled = false;
			this.vScrollBar.LargeChange = 128;
			this.vScrollBar.Location = new System.Drawing.Point(133, 0);
			this.vScrollBar.Maximum = 128;
			this.vScrollBar.Name = "vScrollBar";
			this.vScrollBar.Size = new System.Drawing.Size(17, 133);
			this.vScrollBar.SmallChange = 16;
			this.vScrollBar.TabIndex = 2;
			this.vScrollBar.ValueChanged += new System.EventHandler(this.vScrollBar_ValueChanged);
			// 
			// hScrollBar
			// 
			this.hScrollBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.hScrollBar.Enabled = false;
			this.hScrollBar.LargeChange = 128;
			this.hScrollBar.Location = new System.Drawing.Point(0, 133);
			this.hScrollBar.Maximum = 128;
			this.hScrollBar.Name = "hScrollBar";
			this.hScrollBar.Size = new System.Drawing.Size(133, 17);
			this.hScrollBar.SmallChange = 16;
			this.hScrollBar.TabIndex = 3;
			this.hScrollBar.ValueChanged += new System.EventHandler(this.hScrollBar_ValueChanged);
			// 
			// ScrollingPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel);
			this.Name = "ScrollingPanel";
			this.tableLayoutPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
		private SonicRetro.SonLVL.API.KeyboardPanel panel;
		private System.Windows.Forms.VScrollBar vScrollBar;
		private System.Windows.Forms.HScrollBar hScrollBar;
	}
}
