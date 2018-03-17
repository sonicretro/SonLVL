namespace ObjectLayoutMerge
{
	partial class FileSelectDialog
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
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label1;
			System.Windows.Forms.GroupBox groupBox2;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label5;
			System.Windows.Forms.GroupBox groupBox3;
			System.Windows.Forms.Label label6;
			System.Windows.Forms.Label label7;
			this.layoutAFilePanel = new System.Windows.Forms.Panel();
			this.layoutAFileRingSelector = new SonicRetro.SonLVL.API.FileSelector();
			this.layoutAFileObjectSelector = new SonicRetro.SonLVL.API.FileSelector();
			this.layoutAFileGameSelector = new System.Windows.Forms.ComboBox();
			this.layoutAFileButton = new System.Windows.Forms.RadioButton();
			this.layoutAProjectPanel = new System.Windows.Forms.Panel();
			this.layoutAProjectLevelSelector = new System.Windows.Forms.ComboBox();
			this.layoutAProjectFileSelector = new SonicRetro.SonLVL.API.FileSelector();
			this.layoutAProjectButton = new System.Windows.Forms.RadioButton();
			this.layoutBRingSelector = new SonicRetro.SonLVL.API.FileSelector();
			this.layoutBObjectSelector = new SonicRetro.SonLVL.API.FileSelector();
			this.outputFilePanel = new System.Windows.Forms.Panel();
			this.outputFileRingSelector = new SonicRetro.SonLVL.API.FileSelector();
			this.outputFileObjectSelector = new SonicRetro.SonLVL.API.FileSelector();
			this.outputFileButton = new System.Windows.Forms.RadioButton();
			this.outputBButton = new System.Windows.Forms.RadioButton();
			this.outputAButton = new System.Windows.Forms.RadioButton();
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			groupBox1 = new System.Windows.Forms.GroupBox();
			label3 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			groupBox2 = new System.Windows.Forms.GroupBox();
			label4 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			groupBox3 = new System.Windows.Forms.GroupBox();
			label6 = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			groupBox1.SuspendLayout();
			this.layoutAFilePanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutAFileRingSelector)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutAFileObjectSelector)).BeginInit();
			this.layoutAProjectPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutAProjectFileSelector)).BeginInit();
			groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutBRingSelector)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutBObjectSelector)).BeginInit();
			groupBox3.SuspendLayout();
			this.outputFilePanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.outputFileRingSelector)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.outputFileObjectSelector)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			groupBox1.AutoSize = true;
			groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			groupBox1.Controls.Add(this.layoutAFilePanel);
			groupBox1.Controls.Add(this.layoutAFileButton);
			groupBox1.Controls.Add(this.layoutAProjectPanel);
			groupBox1.Controls.Add(this.layoutAProjectButton);
			groupBox1.Location = new System.Drawing.Point(12, 12);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(353, 234);
			groupBox1.TabIndex = 0;
			groupBox1.TabStop = false;
			groupBox1.Text = "Layout A";
			// 
			// layoutAFilePanel
			// 
			this.layoutAFilePanel.AutoSize = true;
			this.layoutAFilePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.layoutAFilePanel.Controls.Add(this.layoutAFileRingSelector);
			this.layoutAFilePanel.Controls.Add(label3);
			this.layoutAFilePanel.Controls.Add(this.layoutAFileObjectSelector);
			this.layoutAFilePanel.Controls.Add(label2);
			this.layoutAFilePanel.Controls.Add(this.layoutAFileGameSelector);
			this.layoutAFilePanel.Controls.Add(label1);
			this.layoutAFilePanel.Enabled = false;
			this.layoutAFilePanel.Location = new System.Drawing.Point(6, 128);
			this.layoutAFilePanel.Name = "layoutAFilePanel";
			this.layoutAFilePanel.Size = new System.Drawing.Size(341, 87);
			this.layoutAFilePanel.TabIndex = 3;
			// 
			// layoutAFileRingSelector
			// 
			this.layoutAFileRingSelector.DefaultExt = "";
			this.layoutAFileRingSelector.FileName = "";
			this.layoutAFileRingSelector.Filter = "Binary Files|*.bin|All Files|*.*";
			this.layoutAFileRingSelector.Location = new System.Drawing.Point(55, 60);
			this.layoutAFileRingSelector.Name = "layoutAFileRingSelector";
			this.layoutAFileRingSelector.Size = new System.Drawing.Size(283, 24);
			this.layoutAFileRingSelector.TabIndex = 5;
			this.layoutAFileRingSelector.FileNameChanged += new System.EventHandler(this.generic_FileNameChanged);
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(3, 65);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(37, 13);
			label3.TabIndex = 4;
			label3.Text = "Rings:";
			// 
			// layoutAFileObjectSelector
			// 
			this.layoutAFileObjectSelector.DefaultExt = "";
			this.layoutAFileObjectSelector.FileName = "";
			this.layoutAFileObjectSelector.Filter = "Binary Files|*.bin|All Files|*.*";
			this.layoutAFileObjectSelector.Location = new System.Drawing.Point(55, 30);
			this.layoutAFileObjectSelector.Name = "layoutAFileObjectSelector";
			this.layoutAFileObjectSelector.Size = new System.Drawing.Size(283, 24);
			this.layoutAFileObjectSelector.TabIndex = 3;
			this.layoutAFileObjectSelector.FileNameChanged += new System.EventHandler(this.generic_FileNameChanged);
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(3, 35);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(46, 13);
			label2.TabIndex = 2;
			label2.Text = "Objects:";
			// 
			// layoutAFileGameSelector
			// 
			this.layoutAFileGameSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.layoutAFileGameSelector.FormattingEnabled = true;
			this.layoutAFileGameSelector.Items.AddRange(new object[] {
            "Sonic 1",
            "Sonic 2 Nick Arcade",
            "Sonic 2",
            "Sonic 3 & Knuckles",
            "Sonic CD",
            "Sonic CD PC",
            "Sonic & Knuckles Collection",
            "Chaotix"});
			this.layoutAFileGameSelector.Location = new System.Drawing.Point(47, 3);
			this.layoutAFileGameSelector.Name = "layoutAFileGameSelector";
			this.layoutAFileGameSelector.Size = new System.Drawing.Size(150, 21);
			this.layoutAFileGameSelector.TabIndex = 1;
			this.layoutAFileGameSelector.SelectedIndexChanged += new System.EventHandler(this.layoutAFileGameSelector_SelectedIndexChanged);
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(3, 6);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(38, 13);
			label1.TabIndex = 0;
			label1.Text = "Game:";
			// 
			// layoutAFileButton
			// 
			this.layoutAFileButton.AutoSize = true;
			this.layoutAFileButton.Location = new System.Drawing.Point(6, 105);
			this.layoutAFileButton.Name = "layoutAFileButton";
			this.layoutAFileButton.Size = new System.Drawing.Size(44, 17);
			this.layoutAFileButton.TabIndex = 2;
			this.layoutAFileButton.TabStop = true;
			this.layoutAFileButton.Text = "File:";
			this.layoutAFileButton.UseVisualStyleBackColor = true;
			// 
			// layoutAProjectPanel
			// 
			this.layoutAProjectPanel.AutoSize = true;
			this.layoutAProjectPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.layoutAProjectPanel.Controls.Add(this.layoutAProjectLevelSelector);
			this.layoutAProjectPanel.Controls.Add(this.layoutAProjectFileSelector);
			this.layoutAProjectPanel.Location = new System.Drawing.Point(6, 42);
			this.layoutAProjectPanel.Name = "layoutAProjectPanel";
			this.layoutAProjectPanel.Size = new System.Drawing.Size(341, 57);
			this.layoutAProjectPanel.TabIndex = 1;
			// 
			// layoutAProjectLevelSelector
			// 
			this.layoutAProjectLevelSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.layoutAProjectLevelSelector.Enabled = false;
			this.layoutAProjectLevelSelector.FormattingEnabled = true;
			this.layoutAProjectLevelSelector.Location = new System.Drawing.Point(3, 33);
			this.layoutAProjectLevelSelector.Name = "layoutAProjectLevelSelector";
			this.layoutAProjectLevelSelector.Size = new System.Drawing.Size(335, 21);
			this.layoutAProjectLevelSelector.TabIndex = 1;
			this.layoutAProjectLevelSelector.SelectedIndexChanged += new System.EventHandler(this.layoutAProjectLevelSelector_SelectedIndexChanged);
			// 
			// layoutAProjectFileSelector
			// 
			this.layoutAProjectFileSelector.DefaultExt = "";
			this.layoutAProjectFileSelector.FileName = "";
			this.layoutAProjectFileSelector.Filter = "Project Files|*.ini";
			this.layoutAProjectFileSelector.Location = new System.Drawing.Point(3, 3);
			this.layoutAProjectFileSelector.Name = "layoutAProjectFileSelector";
			this.layoutAProjectFileSelector.Size = new System.Drawing.Size(335, 24);
			this.layoutAProjectFileSelector.TabIndex = 0;
			this.layoutAProjectFileSelector.FileNameChanged += new System.EventHandler(this.layoutAProjectFileSelector_FileNameChanged);
			// 
			// layoutAProjectButton
			// 
			this.layoutAProjectButton.AutoSize = true;
			this.layoutAProjectButton.Checked = true;
			this.layoutAProjectButton.Location = new System.Drawing.Point(6, 19);
			this.layoutAProjectButton.Name = "layoutAProjectButton";
			this.layoutAProjectButton.Size = new System.Drawing.Size(61, 17);
			this.layoutAProjectButton.TabIndex = 0;
			this.layoutAProjectButton.TabStop = true;
			this.layoutAProjectButton.Text = "Project:";
			this.layoutAProjectButton.UseVisualStyleBackColor = true;
			this.layoutAProjectButton.CheckedChanged += new System.EventHandler(this.layoutAProjectButton_CheckedChanged);
			// 
			// groupBox2
			// 
			groupBox2.AutoSize = true;
			groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			groupBox2.Controls.Add(this.layoutBRingSelector);
			groupBox2.Controls.Add(label4);
			groupBox2.Controls.Add(this.layoutBObjectSelector);
			groupBox2.Controls.Add(label5);
			groupBox2.Location = new System.Drawing.Point(12, 252);
			groupBox2.Name = "groupBox2";
			groupBox2.Size = new System.Drawing.Size(350, 92);
			groupBox2.TabIndex = 1;
			groupBox2.TabStop = false;
			groupBox2.Text = "Layout B";
			// 
			// layoutBRingSelector
			// 
			this.layoutBRingSelector.DefaultExt = "";
			this.layoutBRingSelector.FileName = "";
			this.layoutBRingSelector.Filter = "Binary Files|*.bin|All Files|*.*";
			this.layoutBRingSelector.Location = new System.Drawing.Point(61, 49);
			this.layoutBRingSelector.Name = "layoutBRingSelector";
			this.layoutBRingSelector.Size = new System.Drawing.Size(283, 24);
			this.layoutBRingSelector.TabIndex = 3;
			this.layoutBRingSelector.FileNameChanged += new System.EventHandler(this.generic_FileNameChanged);
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(9, 54);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(37, 13);
			label4.TabIndex = 2;
			label4.Text = "Rings:";
			// 
			// layoutBObjectSelector
			// 
			this.layoutBObjectSelector.DefaultExt = "";
			this.layoutBObjectSelector.FileName = "";
			this.layoutBObjectSelector.Filter = "Binary Files|*.bin|All Files|*.*";
			this.layoutBObjectSelector.Location = new System.Drawing.Point(61, 19);
			this.layoutBObjectSelector.Name = "layoutBObjectSelector";
			this.layoutBObjectSelector.Size = new System.Drawing.Size(283, 24);
			this.layoutBObjectSelector.TabIndex = 1;
			this.layoutBObjectSelector.FileNameChanged += new System.EventHandler(this.generic_FileNameChanged);
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(9, 24);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(46, 13);
			label5.TabIndex = 0;
			label5.Text = "Objects:";
			// 
			// groupBox3
			// 
			groupBox3.AutoSize = true;
			groupBox3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			groupBox3.Controls.Add(this.outputFilePanel);
			groupBox3.Controls.Add(this.outputFileButton);
			groupBox3.Controls.Add(this.outputBButton);
			groupBox3.Controls.Add(this.outputAButton);
			groupBox3.Location = new System.Drawing.Point(12, 350);
			groupBox3.Name = "groupBox3";
			groupBox3.Size = new System.Drawing.Size(353, 167);
			groupBox3.TabIndex = 2;
			groupBox3.TabStop = false;
			groupBox3.Text = "Output";
			// 
			// outputFilePanel
			// 
			this.outputFilePanel.AutoSize = true;
			this.outputFilePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.outputFilePanel.Controls.Add(this.outputFileRingSelector);
			this.outputFilePanel.Controls.Add(label6);
			this.outputFilePanel.Controls.Add(this.outputFileObjectSelector);
			this.outputFilePanel.Controls.Add(label7);
			this.outputFilePanel.Enabled = false;
			this.outputFilePanel.Location = new System.Drawing.Point(6, 88);
			this.outputFilePanel.Name = "outputFilePanel";
			this.outputFilePanel.Size = new System.Drawing.Size(341, 60);
			this.outputFilePanel.TabIndex = 3;
			// 
			// outputFileRingSelector
			// 
			this.outputFileRingSelector.DefaultExt = "";
			this.outputFileRingSelector.FileName = "";
			this.outputFileRingSelector.Filter = "Binary Files|*.bin|All Files|*.*";
			this.outputFileRingSelector.Location = new System.Drawing.Point(55, 33);
			this.outputFileRingSelector.Mode = SonicRetro.SonLVL.API.FileSelectorMode.Save;
			this.outputFileRingSelector.Name = "outputFileRingSelector";
			this.outputFileRingSelector.Size = new System.Drawing.Size(283, 24);
			this.outputFileRingSelector.TabIndex = 3;
			this.outputFileRingSelector.FileNameChanged += new System.EventHandler(this.generic_FileNameChanged);
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(3, 38);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(37, 13);
			label6.TabIndex = 2;
			label6.Text = "Rings:";
			// 
			// outputFileObjectSelector
			// 
			this.outputFileObjectSelector.DefaultExt = "";
			this.outputFileObjectSelector.FileName = "";
			this.outputFileObjectSelector.Filter = "Binary Files|*.bin|All Files|*.*";
			this.outputFileObjectSelector.Location = new System.Drawing.Point(55, 3);
			this.outputFileObjectSelector.Mode = SonicRetro.SonLVL.API.FileSelectorMode.Save;
			this.outputFileObjectSelector.Name = "outputFileObjectSelector";
			this.outputFileObjectSelector.Size = new System.Drawing.Size(283, 24);
			this.outputFileObjectSelector.TabIndex = 1;
			this.outputFileObjectSelector.FileNameChanged += new System.EventHandler(this.generic_FileNameChanged);
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(3, 8);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(46, 13);
			label7.TabIndex = 0;
			label7.Text = "Objects:";
			// 
			// outputFileButton
			// 
			this.outputFileButton.AutoSize = true;
			this.outputFileButton.Location = new System.Drawing.Point(6, 65);
			this.outputFileButton.Name = "outputFileButton";
			this.outputFileButton.Size = new System.Drawing.Size(44, 17);
			this.outputFileButton.TabIndex = 2;
			this.outputFileButton.TabStop = true;
			this.outputFileButton.Text = "File:";
			this.outputFileButton.UseVisualStyleBackColor = true;
			this.outputFileButton.CheckedChanged += new System.EventHandler(this.outputFileButton_CheckedChanged);
			// 
			// outputBButton
			// 
			this.outputBButton.AutoSize = true;
			this.outputBButton.Location = new System.Drawing.Point(6, 42);
			this.outputBButton.Name = "outputBButton";
			this.outputBButton.Size = new System.Drawing.Size(67, 17);
			this.outputBButton.TabIndex = 1;
			this.outputBButton.Text = "Layout B";
			this.outputBButton.UseVisualStyleBackColor = true;
			// 
			// outputAButton
			// 
			this.outputAButton.AutoSize = true;
			this.outputAButton.Checked = true;
			this.outputAButton.Location = new System.Drawing.Point(6, 19);
			this.outputAButton.Name = "outputAButton";
			this.outputAButton.Size = new System.Drawing.Size(67, 17);
			this.outputAButton.TabIndex = 0;
			this.outputAButton.TabStop = true;
			this.outputAButton.Text = "Layout A";
			this.outputAButton.UseVisualStyleBackColor = true;
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(290, 523);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 3;
			this.cancelButton.Text = "&Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Enabled = false;
			this.okButton.Location = new System.Drawing.Point(209, 523);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 4;
			this.okButton.Text = "&OK";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// FileSelectDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(384, 583);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(groupBox3);
			this.Controls.Add(groupBox2);
			this.Controls.Add(groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FileSelectDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Open...";
			this.Load += new System.EventHandler(this.FileSelectDialog_Load);
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			this.layoutAFilePanel.ResumeLayout(false);
			this.layoutAFilePanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutAFileRingSelector)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutAFileObjectSelector)).EndInit();
			this.layoutAProjectPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutAProjectFileSelector)).EndInit();
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutBRingSelector)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutBObjectSelector)).EndInit();
			groupBox3.ResumeLayout(false);
			groupBox3.PerformLayout();
			this.outputFilePanel.ResumeLayout(false);
			this.outputFilePanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.outputFileRingSelector)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.outputFileObjectSelector)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.RadioButton layoutAProjectButton;
		private SonicRetro.SonLVL.API.FileSelector layoutAProjectFileSelector;
		private System.Windows.Forms.Panel layoutAProjectPanel;
		private System.Windows.Forms.ComboBox layoutAProjectLevelSelector;
		private System.Windows.Forms.Panel layoutAFilePanel;
		private System.Windows.Forms.ComboBox layoutAFileGameSelector;
		private System.Windows.Forms.RadioButton layoutAFileButton;
		private SonicRetro.SonLVL.API.FileSelector layoutAFileObjectSelector;
		private SonicRetro.SonLVL.API.FileSelector layoutAFileRingSelector;
		private SonicRetro.SonLVL.API.FileSelector layoutBRingSelector;
		private SonicRetro.SonLVL.API.FileSelector layoutBObjectSelector;
		private System.Windows.Forms.RadioButton outputAButton;
		private System.Windows.Forms.RadioButton outputBButton;
		private System.Windows.Forms.Panel outputFilePanel;
		private SonicRetro.SonLVL.API.FileSelector outputFileRingSelector;
		private SonicRetro.SonLVL.API.FileSelector outputFileObjectSelector;
		private System.Windows.Forms.RadioButton outputFileButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
	}
}