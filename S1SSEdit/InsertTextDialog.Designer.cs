namespace S1SSEdit
{
	partial class InsertTextDialog
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
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label2;
			this.fontDialog1 = new System.Windows.Forms.FontDialog();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.fontLabel = new System.Windows.Forms.Label();
			this.fontButton = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.transparentBGCheckBox = new System.Windows.Forms.CheckBox();
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// fontDialog1
			// 
			this.fontDialog1.FontMustExist = true;
			this.fontDialog1.ShowEffects = false;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.AutoSize = true;
			this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.tableLayoutPanel1.ColumnCount = 3;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.Controls.Add(label1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.fontLabel, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.fontButton, 2, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(126, 23);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// label1
			// 
			label1.Anchor = System.Windows.Forms.AnchorStyles.None;
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(0, 5);
			label1.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(31, 13);
			label1.TabIndex = 0;
			label1.Text = "Font:";
			// 
			// fontLabel
			// 
			this.fontLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.fontLabel.AutoSize = true;
			this.fontLabel.Location = new System.Drawing.Point(34, 5);
			this.fontLabel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.fontLabel.Name = "fontLabel";
			this.fontLabel.Size = new System.Drawing.Size(35, 13);
			this.fontLabel.TabIndex = 1;
			this.fontLabel.Text = "label2";
			// 
			// fontButton
			// 
			this.fontButton.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.fontButton.AutoSize = true;
			this.fontButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.fontButton.Location = new System.Drawing.Point(72, 0);
			this.fontButton.Margin = new System.Windows.Forms.Padding(0);
			this.fontButton.Name = "fontButton";
			this.fontButton.Size = new System.Drawing.Size(54, 23);
			this.fontButton.TabIndex = 2;
			this.fontButton.Text = "Change";
			this.fontButton.UseVisualStyleBackColor = true;
			this.fontButton.Click += new System.EventHandler(this.fontButton_Click);
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(12, 44);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(31, 13);
			label2.TabIndex = 1;
			label2.Text = "Text:";
			// 
			// textBox1
			// 
			this.textBox1.AcceptsReturn = true;
			this.textBox1.AcceptsTab = true;
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.Location = new System.Drawing.Point(49, 41);
			this.textBox1.MaxLength = 1000;
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(142, 74);
			this.textBox1.TabIndex = 2;
			this.textBox1.WordWrap = false;
			this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
			// 
			// transparentBGCheckBox
			// 
			this.transparentBGCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.transparentBGCheckBox.AutoSize = true;
			this.transparentBGCheckBox.Checked = true;
			this.transparentBGCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.transparentBGCheckBox.Location = new System.Drawing.Point(12, 121);
			this.transparentBGCheckBox.Name = "transparentBGCheckBox";
			this.transparentBGCheckBox.Size = new System.Drawing.Size(144, 17);
			this.transparentBGCheckBox.TabIndex = 3;
			this.transparentBGCheckBox.Text = "Transparent Background";
			this.transparentBGCheckBox.UseVisualStyleBackColor = true;
			this.transparentBGCheckBox.CheckedChanged += new System.EventHandler(this.transparentBGCheckBox_CheckedChanged);
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(197, 276);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 5;
			this.cancelButton.Text = "&Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Enabled = false;
			this.okButton.Location = new System.Drawing.Point(116, 276);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 6;
			this.okButton.Text = "&OK";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.pictureBox1.Location = new System.Drawing.Point(12, 144);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(260, 126);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox1.TabIndex = 4;
			this.pictureBox1.TabStop = false;
			// 
			// InsertTextDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(284, 311);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.transparentBGCheckBox);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(label2);
			this.Controls.Add(this.tableLayoutPanel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "InsertTextDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Insert Text...";
			this.Load += new System.EventHandler(this.InsertTextDialog_Load);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.FontDialog fontDialog1;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Label fontLabel;
		private System.Windows.Forms.Button fontButton;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.CheckBox transparentBGCheckBox;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
	}
}