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
			System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
			this.categoryBSLayout = new System.Windows.Forms.RadioButton();
			this.categoryBSChunk = new System.Windows.Forms.RadioButton();
			this.categorySK = new System.Windows.Forms.RadioButton();
			this.categoryS3 = new System.Windows.Forms.RadioButton();
			this.stageList = new System.Windows.Forms.ListBox();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this.chunkSelector = new System.Windows.Forms.NumericUpDown();
			tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			groupBox1 = new System.Windows.Forms.GroupBox();
			groupBox2 = new System.Windows.Forms.GroupBox();
			tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			tableLayoutPanel1.SuspendLayout();
			groupBox1.SuspendLayout();
			groupBox2.SuspendLayout();
			tableLayoutPanel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.panel1.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chunkSelector)).BeginInit();
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
			this.categoryBSLayout.Visible = false;
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
			groupBox2.Location = new System.Drawing.Point(165, 47);
			groupBox2.Name = "groupBox2";
			groupBox2.Size = new System.Drawing.Size(132, 166);
			groupBox2.TabIndex = 1;
			groupBox2.TabStop = false;
			groupBox2.Text = "Stage";
			// 
			// stageList
			// 
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
			this.stageList.Location = new System.Drawing.Point(0, 0);
			this.stageList.Margin = new System.Windows.Forms.Padding(0);
			this.stageList.Name = "stageList";
			this.stageList.Size = new System.Drawing.Size(120, 108);
			this.stageList.TabIndex = 0;
			this.stageList.SelectedIndexChanged += new System.EventHandler(this.stageList_SelectedIndexChanged);
			// 
			// tableLayoutPanel2
			// 
			tableLayoutPanel2.ColumnCount = 1;
			tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel2.Controls.Add(this.pictureBox1, 0, 0);
			tableLayoutPanel2.Controls.Add(this.panel1, 0, 1);
			tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			tableLayoutPanel2.Location = new System.Drawing.Point(300, 0);
			tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
			tableLayoutPanel2.Name = "tableLayoutPanel2";
			tableLayoutPanel2.RowCount = 2;
			tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			tableLayoutPanel2.Size = new System.Drawing.Size(284, 261);
			tableLayoutPanel2.TabIndex = 2;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox1.Location = new System.Drawing.Point(0, 3);
			this.pictureBox1.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(281, 229);
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
			this.panel1.Location = new System.Drawing.Point(122, 232);
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
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.AutoSize = true;
			this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel3.ColumnCount = 1;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel3.Controls.Add(this.stageList, 0, 0);
			this.tableLayoutPanel3.Controls.Add(this.chunkSelector, 0, 1);
			this.tableLayoutPanel3.Location = new System.Drawing.Point(6, 19);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 2;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.Size = new System.Drawing.Size(120, 128);
			this.tableLayoutPanel3.TabIndex = 1;
			// 
			// chunkSelector
			// 
			this.chunkSelector.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.chunkSelector.Location = new System.Drawing.Point(27, 108);
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
			// StageSelectDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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
			tableLayoutPanel2.ResumeLayout(false);
			tableLayoutPanel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.panel1.ResumeLayout(false);
			this.tableLayoutPanel3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chunkSelector)).EndInit();
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
	}
}