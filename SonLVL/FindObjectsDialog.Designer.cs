namespace SonicRetro.SonLVL
{
    partial class FindObjectsDialog
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
            this.components = new System.ComponentModel.Container();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.findSubtype = new System.Windows.Forms.CheckBox();
            this.subtypeSelect = new System.Windows.Forms.NumericUpDown();
            this.idSelect = new SonicRetro.SonLVL.API.IDControl();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.subtypeList = new System.Windows.Forms.ListView();
            ((System.ComponentModel.ISupportInitialize)(this.subtypeSelect)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(82, 233);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "&Find";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(163, 233);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "&Select All";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // findSubtype
            // 
            this.findSubtype.AutoSize = true;
            this.findSubtype.Location = new System.Drawing.Point(12, 118);
            this.findSubtype.Name = "findSubtype";
            this.findSubtype.Size = new System.Drawing.Size(68, 17);
            this.findSubtype.TabIndex = 4;
            this.findSubtype.Text = "Subtype:";
            this.findSubtype.UseVisualStyleBackColor = true;
            this.findSubtype.CheckedChanged += new System.EventHandler(this.findSubtype_CheckedChanged);
            // 
            // subtypeSelect
            // 
            this.subtypeSelect.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.subtypeSelect.Hexadecimal = true;
            this.subtypeSelect.Location = new System.Drawing.Point(0, 80);
            this.subtypeSelect.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.subtypeSelect.Name = "subtypeSelect";
            this.subtypeSelect.Size = new System.Drawing.Size(150, 20);
            this.subtypeSelect.TabIndex = 5;
            // 
            // idSelect
            // 
            this.idSelect.Location = new System.Drawing.Point(86, 12);
            this.idSelect.Name = "idSelect";
            this.idSelect.Size = new System.Drawing.Size(150, 100);
            this.idSelect.TabIndex = 3;
            this.idSelect.ValueChanged += new System.EventHandler(this.idSelect_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "ID:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.subtypeList);
            this.panel1.Controls.Add(this.subtypeSelect);
            this.panel1.Enabled = false;
            this.panel1.Location = new System.Drawing.Point(86, 118);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(150, 100);
            this.panel1.TabIndex = 7;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(32, 32);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // subtypeList
            // 
            this.subtypeList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.subtypeList.HideSelection = false;
            this.subtypeList.LargeImageList = this.imageList1;
            this.subtypeList.Location = new System.Drawing.Point(0, 0);
            this.subtypeList.MultiSelect = false;
            this.subtypeList.Name = "subtypeList";
            this.subtypeList.Size = new System.Drawing.Size(150, 80);
            this.subtypeList.TabIndex = 6;
            this.subtypeList.TileSize = new System.Drawing.Size(120, 48);
            this.subtypeList.UseCompatibleStateImageBehavior = false;
            this.subtypeList.View = System.Windows.Forms.View.Tile;
            this.subtypeList.SelectedIndexChanged += new System.EventHandler(this.subtypeList_SelectedIndexChanged);
            // 
            // FindObjectsDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(250, 268);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.findSubtype);
            this.Controls.Add(this.idSelect);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FindObjectsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Find Objects";
            ((System.ComponentModel.ISupportInitialize)(this.subtypeSelect)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        internal API.IDControl idSelect;
        internal System.Windows.Forms.CheckBox findSubtype;
        internal System.Windows.Forms.NumericUpDown subtypeSelect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ListView subtypeList;
    }
}

