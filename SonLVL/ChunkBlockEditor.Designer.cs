namespace SonicRetro.SonLVL
{
	partial class ChunkBlockEditor
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
			System.Windows.Forms.Label label2;
			this.solidity2 = new System.Windows.Forms.ComboBox();
			this.solidity1 = new System.Windows.Forms.ComboBox();
			this.xFlip = new System.Windows.Forms.CheckBox();
			this.yFlip = new System.Windows.Forms.CheckBox();
			this.block = new SonicRetro.SonLVL.NumericUpDownMulti();
			groupBox1 = new System.Windows.Forms.GroupBox();
			label2 = new System.Windows.Forms.Label();
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
            "Not Solid",
            "Top Solid",
            "Left/Right/Bottom Solid",
            "All Solid"});
			this.solidity2.Location = new System.Drawing.Point(6, 46);
			this.solidity2.Name = "solidity2";
			this.solidity2.Size = new System.Drawing.Size(121, 21);
			this.solidity2.TabIndex = 1;
			this.solidity2.SelectedIndexChanged += new System.EventHandler(this.solidity2_SelectedIndexChanged);
			// 
			// solidity1
			// 
			this.solidity1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.solidity1.FormattingEnabled = true;
			this.solidity1.Items.AddRange(new object[] {
            "Not Solid",
            "Top Solid",
            "Left/Right/Bottom Solid",
            "All Solid"});
			this.solidity1.Location = new System.Drawing.Point(6, 19);
			this.solidity1.Name = "solidity1";
			this.solidity1.Size = new System.Drawing.Size(121, 21);
			this.solidity1.TabIndex = 0;
			this.solidity1.SelectedIndexChanged += new System.EventHandler(this.solidity1_SelectedIndexChanged);
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(3, 117);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(37, 13);
			label2.TabIndex = 9;
			label2.Text = "Block:";
			// 
			// xFlip
			// 
			this.xFlip.AutoSize = true;
			this.xFlip.Location = new System.Drawing.Point(3, 3);
			this.xFlip.Name = "xFlip";
			this.xFlip.Size = new System.Drawing.Size(52, 17);
			this.xFlip.TabIndex = 0;
			this.xFlip.Text = "X Flip";
			this.xFlip.UseVisualStyleBackColor = true;
			this.xFlip.CheckedChanged += new System.EventHandler(this.xFlip_CheckedChanged);
			// 
			// yFlip
			// 
			this.yFlip.AutoSize = true;
			this.yFlip.Location = new System.Drawing.Point(61, 3);
			this.yFlip.Name = "yFlip";
			this.yFlip.Size = new System.Drawing.Size(52, 17);
			this.yFlip.TabIndex = 1;
			this.yFlip.Text = "Y Flip";
			this.yFlip.UseVisualStyleBackColor = true;
			this.yFlip.CheckedChanged += new System.EventHandler(this.yFlip_CheckedChanged);
			// 
			// block
			// 
			this.block.Hexadecimal = true;
			this.block.Location = new System.Drawing.Point(46, 115);
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
			// ChunkBlockEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.block);
			this.Controls.Add(label2);
			this.Controls.Add(groupBox1);
			this.Controls.Add(this.yFlip);
			this.Controls.Add(this.xFlip);
			this.Name = "ChunkBlockEditor";
			this.Size = new System.Drawing.Size(139, 138);
			this.Load += new System.EventHandler(this.ChunkBlockEditor_Load);
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
		private NumericUpDownMulti block;
	}
}
