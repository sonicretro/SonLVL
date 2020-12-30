namespace S3SSEdit
{
	partial class StatisticsDialog
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
			this.listView4 = new System.Windows.Forms.ListView();
			this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.SuspendLayout();
			// 
			// listView4
			// 
			this.listView4.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader8});
			this.listView4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listView4.FullRowSelect = true;
			this.listView4.GridLines = true;
			this.listView4.HideSelection = false;
			this.listView4.LabelWrap = false;
			this.listView4.Location = new System.Drawing.Point(0, 0);
			this.listView4.Name = "listView4";
			this.listView4.Size = new System.Drawing.Size(284, 262);
			this.listView4.TabIndex = 1;
			this.listView4.UseCompatibleStateImageBehavior = false;
			this.listView4.View = System.Windows.Forms.View.Details;
			this.listView4.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView_ColumnClick);
			// 
			// columnHeader7
			// 
			this.columnHeader7.Text = "Item";
			// 
			// columnHeader8
			// 
			this.columnHeader8.Text = "Count";
			// 
			// StatisticsDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Controls.Add(this.listView4);
			this.MinimizeBox = false;
			this.Name = "StatisticsDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Item Counts";
			this.Load += new System.EventHandler(this.StatisticsDialog_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.ListView listView4;
		private System.Windows.Forms.ColumnHeader columnHeader7;
		private System.Windows.Forms.ColumnHeader columnHeader8;
	}
}