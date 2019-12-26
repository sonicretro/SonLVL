namespace SonicRetro.SonLVL
{
	partial class ReplaceChunkBlocksDialog
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
			System.Windows.Forms.GroupBox groupBox1;
			System.Windows.Forms.GroupBox groupBox2;
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.replaceBlock = new SonicRetro.SonLVL.ChunkBlockSearchControl();
			this.findBlock = new SonicRetro.SonLVL.ChunkBlockSearchControl();
			groupBox1 = new System.Windows.Forms.GroupBox();
			groupBox2 = new System.Windows.Forms.GroupBox();
			groupBox1.SuspendLayout();
			groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(257, 340);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 9;
			this.cancelButton.Text = "&Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(176, 340);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 8;
			this.okButton.Text = "&OK";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			groupBox1.AutoSize = true;
			groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			groupBox1.Controls.Add(this.findBlock);
			groupBox1.Location = new System.Drawing.Point(12, 12);
			groupBox1.Name = "groupBox1";
			groupBox1.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
			groupBox1.Size = new System.Drawing.Size(157, 317);
			groupBox1.TabIndex = 10;
			groupBox1.TabStop = false;
			groupBox1.Text = "Find";
			// 
			// groupBox2
			// 
			groupBox2.AutoSize = true;
			groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			groupBox2.Controls.Add(this.replaceBlock);
			groupBox2.Location = new System.Drawing.Point(175, 12);
			groupBox2.Name = "groupBox2";
			groupBox2.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
			groupBox2.Size = new System.Drawing.Size(157, 317);
			groupBox2.TabIndex = 11;
			groupBox2.TabStop = false;
			groupBox2.Text = "Replace";
			// 
			// replaceBlock
			// 
			this.replaceBlock.Location = new System.Drawing.Point(7, 20);
			this.replaceBlock.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this.replaceBlock.Name = "replaceBlock";
			this.replaceBlock.Size = new System.Drawing.Size(144, 284);
			this.replaceBlock.TabIndex = 0;
			// 
			// findBlock
			// 
			this.findBlock.Location = new System.Drawing.Point(7, 20);
			this.findBlock.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this.findBlock.Name = "findBlock";
			this.findBlock.Size = new System.Drawing.Size(144, 284);
			this.findBlock.TabIndex = 0;
			// 
			// ReplaceChunkBlocksDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(344, 375);
			this.Controls.Add(groupBox2);
			this.Controls.Add(groupBox1);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ReplaceChunkBlocksDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Replace Chunk Blocks";
			this.VisibleChanged += new System.EventHandler(this.ReplaceChunkBlocksDialog_VisibleChanged);
			groupBox1.ResumeLayout(false);
			groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		public ChunkBlockSearchControl findBlock;
		public ChunkBlockSearchControl replaceBlock;

	}
}

