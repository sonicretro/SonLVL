namespace SonicRetro.SonLVL.GUI
{
	partial class CollisionSelector
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
			this.tileList1 = new SonicRetro.SonLVL.API.TileList();
			this.SuspendLayout();
			// 
			// tileList1
			// 
			this.tileList1.BackColor = System.Drawing.Color.Black;
			this.tileList1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tileList1.ImageSize = 16;
			this.tileList1.Location = new System.Drawing.Point(0, 0);
			this.tileList1.Name = "tileList1";
			this.tileList1.SelectedIndex = -1;
			this.tileList1.Size = new System.Drawing.Size(284, 264);
			this.tileList1.TabIndex = 3;
			this.tileList1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.tileList1_MouseDoubleClick);
			// 
			// CollisionSelector
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 264);
			this.ControlBox = false;
			this.Controls.Add(this.tileList1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "CollisionSelector";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Collision Selector";
			this.Load += new System.EventHandler(this.CollisionSelector_Load);
			this.ResumeLayout(false);

		}

		#endregion

		internal SonicRetro.SonLVL.API.TileList tileList1;
	}
}
