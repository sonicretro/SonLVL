namespace SonicRetro.SonLVL
{
	partial class PatternIndexSearchControl
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
			this.xFlip = new System.Windows.Forms.CheckBox();
			this.yFlip = new System.Windows.Forms.CheckBox();
			this.tileList = new SonicRetro.SonLVL.API.TileList();
			this.tile = new System.Windows.Forms.NumericUpDown();
			this.searchTile = new System.Windows.Forms.CheckBox();
			this.priority = new System.Windows.Forms.CheckBox();
			this.searchPalette = new System.Windows.Forms.CheckBox();
			this.palette = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.tile)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.palette)).BeginInit();
			this.SuspendLayout();
			// 
			// xFlip
			// 
			this.xFlip.AutoSize = true;
			this.xFlip.Checked = true;
			this.xFlip.CheckState = System.Windows.Forms.CheckState.Indeterminate;
			this.xFlip.Location = new System.Drawing.Point(3, 3);
			this.xFlip.Name = "xFlip";
			this.xFlip.Size = new System.Drawing.Size(52, 17);
			this.xFlip.TabIndex = 0;
			this.xFlip.Text = "X Flip";
			this.xFlip.ThreeState = true;
			this.xFlip.UseVisualStyleBackColor = true;
			// 
			// yFlip
			// 
			this.yFlip.AutoSize = true;
			this.yFlip.Checked = true;
			this.yFlip.CheckState = System.Windows.Forms.CheckState.Indeterminate;
			this.yFlip.Location = new System.Drawing.Point(61, 3);
			this.yFlip.Name = "yFlip";
			this.yFlip.Size = new System.Drawing.Size(52, 17);
			this.yFlip.TabIndex = 1;
			this.yFlip.Text = "Y Flip";
			this.yFlip.ThreeState = true;
			this.yFlip.UseVisualStyleBackColor = true;
			// 
			// tileList
			// 
			this.tileList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tileList.BackColor = System.Drawing.SystemColors.Window;
			this.tileList.ImageHeight = 64;
			this.tileList.ImageSize = 64;
			this.tileList.ImageWidth = 64;
			this.tileList.Location = new System.Drawing.Point(3, 100);
			this.tileList.Name = "tileList";
			this.tileList.ScrollValue = 0;
			this.tileList.SelectedIndex = -1;
			this.tileList.Size = new System.Drawing.Size(138, 195);
			this.tileList.TabIndex = 3;
			this.tileList.SelectedIndexChanged += new System.EventHandler(this.tileList_SelectedIndexChanged);
			// 
			// tile
			// 
			this.tile.Hexadecimal = true;
			this.tile.Location = new System.Drawing.Point(55, 74);
			this.tile.Maximum = new decimal(new int[] {
            2047,
            0,
            0,
            0});
			this.tile.Name = "tile";
			this.tile.Size = new System.Drawing.Size(58, 20);
			this.tile.TabIndex = 10;
			this.tile.ValueChanged += new System.EventHandler(this.tile_ValueChanged);
			// 
			// searchTile
			// 
			this.searchTile.AutoSize = true;
			this.searchTile.Checked = true;
			this.searchTile.CheckState = System.Windows.Forms.CheckState.Checked;
			this.searchTile.Location = new System.Drawing.Point(3, 77);
			this.searchTile.Name = "searchTile";
			this.searchTile.Size = new System.Drawing.Size(46, 17);
			this.searchTile.TabIndex = 11;
			this.searchTile.Text = "Tile:";
			this.searchTile.UseVisualStyleBackColor = true;
			this.searchTile.CheckedChanged += new System.EventHandler(this.searchTile_CheckedChanged);
			// 
			// priority
			// 
			this.priority.AutoSize = true;
			this.priority.Checked = true;
			this.priority.CheckState = System.Windows.Forms.CheckState.Indeterminate;
			this.priority.Location = new System.Drawing.Point(3, 26);
			this.priority.Name = "priority";
			this.priority.Size = new System.Drawing.Size(57, 17);
			this.priority.TabIndex = 12;
			this.priority.Text = "Priority";
			this.priority.ThreeState = true;
			this.priority.UseVisualStyleBackColor = true;
			// 
			// searchPalette
			// 
			this.searchPalette.AutoSize = true;
			this.searchPalette.Location = new System.Drawing.Point(3, 49);
			this.searchPalette.Name = "searchPalette";
			this.searchPalette.Size = new System.Drawing.Size(62, 17);
			this.searchPalette.TabIndex = 13;
			this.searchPalette.Text = "Palette:";
			this.searchPalette.UseVisualStyleBackColor = true;
			this.searchPalette.CheckedChanged += new System.EventHandler(this.searchPalette_CheckedChanged);
			// 
			// palette
			// 
			this.palette.Enabled = false;
			this.palette.Location = new System.Drawing.Point(71, 48);
			this.palette.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
			this.palette.Name = "palette";
			this.palette.Size = new System.Drawing.Size(42, 20);
			this.palette.TabIndex = 14;
			this.palette.ValueChanged += new System.EventHandler(this.palette_ValueChanged);
			// 
			// PatternIndexSearchControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.palette);
			this.Controls.Add(this.searchPalette);
			this.Controls.Add(this.priority);
			this.Controls.Add(this.searchTile);
			this.Controls.Add(this.tile);
			this.Controls.Add(this.tileList);
			this.Controls.Add(this.yFlip);
			this.Controls.Add(this.xFlip);
			this.Name = "PatternIndexSearchControl";
			this.Size = new System.Drawing.Size(144, 298);
			((System.ComponentModel.ISupportInitialize)(this.tile)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.palette)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox xFlip;
		private System.Windows.Forms.CheckBox yFlip;
		private API.TileList tileList;
		private System.Windows.Forms.NumericUpDown tile;
		private System.Windows.Forms.CheckBox searchTile;
		private System.Windows.Forms.CheckBox priority;
		private System.Windows.Forms.CheckBox searchPalette;
		private System.Windows.Forms.NumericUpDown palette;
	}
}
