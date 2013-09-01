namespace SonicRetro.SonLVL
{
    partial class LoadingAnimation
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
            this.Label1 = new System.Windows.Forms.Label();
            this.PictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(0, 14);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(113, 13);
            this.Label1.TabIndex = 2;
            this.Label1.Text = "Loading, please wait...";
            this.Label1.UseWaitCursor = true;
            // 
            // PictureBox1
            // 
            this.PictureBox1.Image = global::SonicRetro.SonLVL.Properties.Resources.sonicbored1;
            this.PictureBox1.Location = new System.Drawing.Point(119, 3);
            this.PictureBox1.Name = "PictureBox1";
            this.PictureBox1.Size = new System.Drawing.Size(40, 32);
            this.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.PictureBox1.TabIndex = 3;
            this.PictureBox1.TabStop = false;
            this.PictureBox1.UseWaitCursor = true;
            // 
            // LoadingAnimation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.PictureBox1);
            this.Controls.Add(this.Label1);
            this.Name = "LoadingAnimation";
            this.Size = new System.Drawing.Size(162, 38);
            this.UseWaitCursor = true;
            this.Load += new System.EventHandler(this.LoadingAnimation_Load);
            this.VisibleChanged += new System.EventHandler(this.LoadingAnimation_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Label Label1;
        private System.Windows.Forms.PictureBox PictureBox1;
    }
}
