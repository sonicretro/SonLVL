namespace SonicRetro.SonLVL
{
	partial class ChunkBlockSearchControl
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
			System.Windows.Forms.GroupBox groupBox1;
			this.solidity2 = new System.Windows.Forms.ComboBox();
			this.solidity1 = new System.Windows.Forms.ComboBox();
			this.xFlip = new System.Windows.Forms.CheckBox();
			this.yFlip = new System.Windows.Forms.CheckBox();
			this.blockList = new SonicRetro.SonLVL.API.TileList();
			this.block = new System.Windows.Forms.NumericUpDown();
			this.searchBlock = new System.Windows.Forms.CheckBox();
			groupBox1 = new System.Windows.Forms.GroupBox();
			groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.block)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			groupBox1.AutoSize = true;
			groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			groupBox1.Controls.Add(this.solidity2);
			groupBox1.Controls.Add(this.solidity1);
			groupBox1.Location = new System.Drawing.Point(3, 26);
			groupBox1.Name = "groupBox1";
			groupBox1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
			groupBox1.Size = new System.Drawing.Size(133, 83);
			groupBox1.TabIndex = 2;
			groupBox1.TabStop = false;
			groupBox1.Text = "Solidity";
			// 
			// solidity2
			// 
			this.solidity2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.solidity2.FormattingEnabled = true;
			this.solidity2.Items.AddRange(new object[] {
            "N/A",
            "Not Solid",
            "Top Solid",
            "Left/Right/Bottom Solid",
            "All Solid"});
			this.solidity2.Location = new System.Drawing.Point(6, 46);
			this.solidity2.Name = "solidity2";
			this.solidity2.Size = new System.Drawing.Size(121, 21);
			this.solidity2.TabIndex = 1;
			// 
			// solidity1
			// 
			this.solidity1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.solidity1.FormattingEnabled = true;
			this.solidity1.Items.AddRange(new object[] {
            "N/A",
            "Not Solid",
            "Top Solid",
            "Left/Right/Bottom Solid",
            "All Solid"});
			this.solidity1.Location = new System.Drawing.Point(6, 19);
			this.solidity1.Name = "solidity1";
			this.solidity1.Size = new System.Drawing.Size(121, 21);
			this.solidity1.TabIndex = 0;
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
			// blockList
			// 
			this.blockList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.blockList.BackColor = System.Drawing.SystemColors.Window;
			this.blockList.ImageHeight = 64;
			this.blockList.ImageSize = 64;
			this.blockList.ImageWidth = 64;
			this.blockList.Location = new System.Drawing.Point(3, 141);
			this.blockList.Name = "blockList";
			this.blockList.ScrollValue = 0;
			this.blockList.SelectedIndex = -1;
			this.blockList.Size = new System.Drawing.Size(138, 140);
			this.blockList.TabIndex = 3;
			this.blockList.SelectedIndexChanged += new System.EventHandler(this.blockList_SelectedIndexChanged);
			// 
			// block
			// 
			this.block.Hexadecimal = true;
			this.block.Location = new System.Drawing.Point(62, 115);
			this.block.Maximum = new decimal(new int[] {
            2047,
            0,
            0,
            0});
			this.block.Name = "block";
			this.block.Size = new System.Drawing.Size(53, 20);
			this.block.TabIndex = 10;
			this.block.ValueChanged += new System.EventHandler(this.block_ValueChanged);
			// 
			// searchBlock
			// 
			this.searchBlock.AutoSize = true;
			this.searchBlock.Checked = true;
			this.searchBlock.CheckState = System.Windows.Forms.CheckState.Checked;
			this.searchBlock.Location = new System.Drawing.Point(0, 116);
			this.searchBlock.Name = "searchBlock";
			this.searchBlock.Size = new System.Drawing.Size(56, 17);
			this.searchBlock.TabIndex = 11;
			this.searchBlock.Text = "Block:";
			this.searchBlock.UseVisualStyleBackColor = true;
			this.searchBlock.CheckedChanged += new System.EventHandler(this.searchBlock_CheckedChanged);
			// 
			// ChunkBlockSearchControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.searchBlock);
			this.Controls.Add(this.block);
			this.Controls.Add(this.blockList);
			this.Controls.Add(groupBox1);
			this.Controls.Add(this.yFlip);
			this.Controls.Add(this.xFlip);
			this.Name = "ChunkBlockSearchControl";
			this.Size = new System.Drawing.Size(144, 284);
			groupBox1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.block)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox xFlip;
		private System.Windows.Forms.CheckBox yFlip;
		private System.Windows.Forms.ComboBox solidity1;
		private System.Windows.Forms.ComboBox solidity2;
		private API.TileList blockList;
		private System.Windows.Forms.NumericUpDown block;
		private System.Windows.Forms.CheckBox searchBlock;
	}
}
