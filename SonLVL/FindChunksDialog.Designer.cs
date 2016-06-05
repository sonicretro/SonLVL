namespace SonicRetro.SonLVL
{
	partial class FindChunksDialog
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.tileList1 = new SonicRetro.SonLVL.API.TileList();
			this.chunkSelect = new System.Windows.Forms.NumericUpDown();
			this.label1 = new System.Windows.Forms.Label();
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chunkSelect)).BeginInit();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.tileList1);
			this.panel1.Controls.Add(this.chunkSelect);
			this.panel1.Location = new System.Drawing.Point(39, 12);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(293, 292);
			this.panel1.TabIndex = 13;
			// 
			// tileList1
			// 
			this.tileList1.BackColor = System.Drawing.SystemColors.Window;
			this.tileList1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tileList1.Location = new System.Drawing.Point(0, 0);
			this.tileList1.Name = "tileList1";
			this.tileList1.ScrollValue = 0;
			this.tileList1.SelectedIndex = -1;
			this.tileList1.Size = new System.Drawing.Size(293, 272);
			this.tileList1.TabIndex = 6;
			this.tileList1.SelectedIndexChanged += new System.EventHandler(this.tileList1_SelectedIndexChanged);
			// 
			// chunkSelect
			// 
			this.chunkSelect.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.chunkSelect.Hexadecimal = true;
			this.chunkSelect.Location = new System.Drawing.Point(0, 272);
			this.chunkSelect.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
			this.chunkSelect.Name = "chunkSelect";
			this.chunkSelect.Size = new System.Drawing.Size(293, 20);
			this.chunkSelect.TabIndex = 5;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(21, 13);
			this.label1.TabIndex = 12;
			this.label1.Text = "ID:";
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Yes;
			this.cancelButton.Location = new System.Drawing.Point(259, 323);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 9;
			this.cancelButton.Text = "&Count";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(178, 323);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 8;
			this.okButton.Text = "&Find";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// FindChunksDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(363, 356);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FindChunksDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Find Chunks";
			this.VisibleChanged += new System.EventHandler(this.FindChunksDialog_VisibleChanged);
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chunkSelect)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		internal System.Windows.Forms.NumericUpDown chunkSelect;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private API.TileList tileList1;

	}
}

