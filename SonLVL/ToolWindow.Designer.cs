namespace SonicRetro.SonLVL
{
    partial class ToolWindow
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ObjName = new System.Windows.Forms.Label();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.objPicture = new System.Windows.Forms.PictureBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.ChunkSelector = new SonicRetro.SonLVL.BlockList();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.objPicture)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(371, 325);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tableLayoutPanel1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(363, 299);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Object Properties";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.ObjName, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.propertyGrid1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.objPicture, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(357, 293);
            this.tableLayoutPanel1.TabIndex = 12;
            // 
            // ObjName
            // 
            this.ObjName.AutoEllipsis = true;
            this.ObjName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ObjName.Location = new System.Drawing.Point(0, 0);
            this.ObjName.Margin = new System.Windows.Forms.Padding(0);
            this.ObjName.Name = "ObjName";
            this.ObjName.Size = new System.Drawing.Size(178, 146);
            this.ObjName.TabIndex = 0;
            this.ObjName.Text = "No objects selected";
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid1.HelpVisible = false;
            this.propertyGrid1.Location = new System.Drawing.Point(178, 0);
            this.propertyGrid1.Margin = new System.Windows.Forms.Padding(0);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.tableLayoutPanel1.SetRowSpan(this.propertyGrid1, 2);
            this.propertyGrid1.Size = new System.Drawing.Size(179, 293);
            this.propertyGrid1.TabIndex = 11;
            this.propertyGrid1.ToolbarVisible = false;
            // 
            // objPicture
            // 
            this.objPicture.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.objPicture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objPicture.Location = new System.Drawing.Point(0, 146);
            this.objPicture.Margin = new System.Windows.Forms.Padding(0);
            this.objPicture.Name = "objPicture";
            this.objPicture.Size = new System.Drawing.Size(178, 147);
            this.objPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.objPicture.TabIndex = 9;
            this.objPicture.TabStop = false;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.ChunkSelector);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(363, 299);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Chunks";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // ChunkSelector
            // 
            this.ChunkSelector.BackColor = System.Drawing.SystemColors.Window;
            this.ChunkSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChunkSelector.ImageSize = 128;
            this.ChunkSelector.Location = new System.Drawing.Point(3, 3);
            this.ChunkSelector.Name = "ChunkSelector";
            this.ChunkSelector.SelectedIndex = -1;
            this.ChunkSelector.Size = new System.Drawing.Size(357, 293);
            this.ChunkSelector.TabIndex = 0;
            // 
            // ToolWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(371, 325);
            this.Controls.Add(this.tabControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ToolWindow";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "SonLVL - Tools";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ToolWindow_FormClosing);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.objPicture)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        internal BlockList ChunkSelector;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        internal System.Windows.Forms.Label ObjName;
        internal System.Windows.Forms.PictureBox objPicture;
        internal System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

    }
}