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
			this.label1 = new System.Windows.Forms.Label();
			this.previewPanel = new System.Windows.Forms.Panel();
			this.anitimer = new System.Timers.Timer();
			((System.ComponentModel.ISupportInitialize)(this.anitimer)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(113, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Loading, please wait...";
			this.label1.UseWaitCursor = true;
			// 
			// previewPanel
			// 
			this.previewPanel.Location = new System.Drawing.Point(122, 3);
			this.previewPanel.Name = "previewPanel";
			this.previewPanel.Size = new System.Drawing.Size(40, 32);
			this.previewPanel.TabIndex = 3;
			this.previewPanel.UseWaitCursor = true;
			this.previewPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.previewPanel_Paint);
			// 
			// anitimer
			// 
			this.anitimer.Interval = 16.666666666666668D;
			this.anitimer.SynchronizingObject = this;
			this.anitimer.Elapsed += new System.Timers.ElapsedEventHandler(this.anitimer_Elapsed);
			// 
			// LoadingAnimation
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.previewPanel);
			this.Controls.Add(this.label1);
			this.Name = "LoadingAnimation";
			this.Size = new System.Drawing.Size(165, 38);
			this.UseWaitCursor = true;
			this.SizeChanged += new System.EventHandler(this.LoadingAnimation_SizeChanged);
			this.VisibleChanged += new System.EventHandler(this.LoadingAnimation_VisibleChanged);
			((System.ComponentModel.ISupportInitialize)(this.anitimer)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel previewPanel;
        private System.Timers.Timer anitimer;
    }
}
