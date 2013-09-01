using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SonicRetro.SonLVL
{
    public partial class LoadingAnimation : UserControl
    {
        public LoadingAnimation()
        {
            InitializeComponent();
        }

        System.Timers.Timer anitimer = new System.Timers.Timer(250) { AutoReset = true };
        int frame = 0;

        private void LoadingAnimation_Load(object sender, EventArgs e)
        {
            anitimer.SynchronizingObject = this;
            anitimer.Elapsed += new System.Timers.ElapsedEventHandler(anitimer_Elapsed);
        }

        void anitimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            frame ^= 1;
            switch (frame)
            {
                case 1:
                    PictureBox1.Image = Properties.Resources.sonicbored2;
                    break;
                default:
                    PictureBox1.Image = Properties.Resources.sonicbored1;
                    break;
            }
        }

        private void LoadingAnimation_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                PictureBox1.Image = Properties.Resources.sonicbored1;
                frame = 0;
                anitimer.Start();
            }
            else
                anitimer.Stop();
        }
    }
}
