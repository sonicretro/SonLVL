using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SonicRetro.SonLVL.API;

namespace SonicRetro.SonLVL
{
	public partial class ScrollingPanel : UserControl
	{
		public ScrollingPanel()
		{
			InitializeComponent();
		}

		public event PaintEventHandler PanelPaint = delegate { };
		public event KeyEventHandler PanelKeyDown = delegate { };
		public event MouseEventHandler PanelMouseDown = delegate { };
		public event MouseEventHandler PanelMouseUp = delegate { };
		public event MouseEventHandler PanelMouseMove = delegate { };
		public event DragEventHandler PanelDragEnter = delegate { };
		public event DragEventHandler PanelDragOver = delegate { };
		public event EventHandler PanelDragLeave = delegate { };
		public event DragEventHandler PanelDragDrop = delegate { };
		public event EventHandler ScrollBarValueChanged = delegate { };

		public bool PanelAllowDrop
		{
			get { return panel.AllowDrop; }
			set { panel.AllowDrop = value; }
		}

		[Browsable(false)]
		public Graphics PanelGraphics { get; private set; }

		[Browsable(false)]
		public int PanelWidth { get { return panel.Width; } }

		[Browsable(false)]
		public int PanelHeight { get { return panel.Height; } }

		public Cursor PanelCursor
		{
			get { return panel.Cursor; }
			set { panel.Cursor = value; }
		}

		public int VScrollMinimum
		{
			get { return vScrollBar.Minimum; }
			set { vScrollBar.Minimum = value; }
		}

		public int VScrollMaximum
		{
			get { return vScrollBar.Maximum; }
			set { vScrollBar.Maximum = value; }
		}

		public int VScrollValue
		{
			get { return vScrollBar.Value; }
			set { vScrollBar.Value = value; }
		}

		public int VScrollSmallChange
		{
			get { return vScrollBar.SmallChange; }
			set { vScrollBar.SmallChange = value; }
		}

		public int VScrollLargeChange
		{
			get { return vScrollBar.LargeChange; }
			set { vScrollBar.LargeChange = value; }
		}

		public bool VScrollEnabled
		{
			get { return vScrollBar.Enabled; }
			set { vScrollBar.Enabled = value; }
		}

		public int HScrollMinimum
		{
			get { return hScrollBar.Minimum; }
			set { hScrollBar.Minimum = value; }
		}

		public int HScrollMaximum
		{
			get { return hScrollBar.Maximum; }
			set { hScrollBar.Maximum = value; }
		}

		public int HScrollValue
		{
			get { return hScrollBar.Value; }
			set { hScrollBar.Value = value; }
		}

		public int HScrollSmallChange
		{
			get { return hScrollBar.SmallChange; }
			set { hScrollBar.SmallChange = value; }
		}

		public int HScrollLargeChange
		{
			get { return hScrollBar.LargeChange; }
			set { hScrollBar.LargeChange = value; }
		}

		public bool HScrollEnabled
		{
			get { return hScrollBar.Enabled; }
			set { hScrollBar.Enabled = value; }
		}

		private bool bPan = false;
		private Point origPoint, origScroll;

		private void panel_Paint(object sender, PaintEventArgs e)
		{
			PanelPaint(sender, e);
		}

		private void panel_KeyDown(object sender, KeyEventArgs e)
		{
			PanelKeyDown(sender, e);
		}

		private void panel_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Middle) {
				origPoint = e.Location;
				origScroll = new Point(HScrollValue, VScrollValue);
				bPan = true;
			}
			PanelMouseDown(sender, e);
		}

		private void panel_MouseUp(object sender, MouseEventArgs e)
		{
			PanelMouseUp(sender, e);
		}

		private void panel_MouseMove(object sender, MouseEventArgs e)
		{
			if((MouseButtons & MouseButtons.Middle) != MouseButtons.Middle) {
				bPan = false;
			}
			if (bPan) {
				Point p = origScroll - (Size)(e.Location - (Size)origPoint);
				VScrollValue = (int)Math.Min(Math.Max(p.Y, VScrollMinimum), VScrollMaximum);
				HScrollValue = (int)Math.Min(Math.Max(p.X, HScrollMinimum), HScrollMaximum);
			}
			PanelMouseMove(sender, e);
		}

		private void panel_MouseWheel(object sender, MouseEventArgs e)
		{
			bool shiftHeld = 0 != (ModifierKeys & Keys.Shift);
			bool ctrlHeld = 0 != (ModifierKeys & Keys.Control);

			int smallChange = shiftHeld ? VScrollSmallChange : HScrollSmallChange;
			int largeChange = shiftHeld ? VScrollLargeChange : HScrollLargeChange;

			int delta;
			if (ctrlHeld) {
				delta = largeChange * e.Delta / -120;
			}
			else {
				int lines = SystemInformation.MouseWheelScrollLines < 1 ? 3 : SystemInformation.MouseWheelScrollLines;
				delta = smallChange * lines * e.Delta / -120;
			}

			if(shiftHeld && VScrollEnabled) {
				VScrollValue = (int)Math.Min(Math.Max(VScrollValue + delta, VScrollMinimum), VScrollMaximum);
			}
			else if(!shiftHeld && HScrollEnabled) {
				HScrollValue = (int)Math.Min(Math.Max(HScrollValue + delta, HScrollMinimum), HScrollMaximum);
			}
		}

		private void panel_DragEnter(object sender, DragEventArgs e)
		{
			PanelDragEnter(sender, e);
		}

		private void panel_DragOver(object sender, DragEventArgs e)
		{
			PanelDragOver(sender, e);
		}

		private void panel_DragLeave(object sender, EventArgs e)
		{
			PanelDragLeave(sender, e);
		}

		private void panel_DragDrop(object sender, DragEventArgs e)
		{
			PanelDragDrop(sender, e);
		}

		private void vScrollBar_ValueChanged(object sender, EventArgs e)
		{
			ScrollBarValueChanged(sender, e);
		}

		private void hScrollBar_ValueChanged(object sender, EventArgs e)
		{
			ScrollBarValueChanged(sender, e);
		}

		private void panel_Load(object sender, EventArgs e)
		{
			PanelGraphics = panel.CreateGraphics();
			PanelGraphics.SetOptions();
		}

		private void panel_Resize(object sender, EventArgs e)
		{
			PanelGraphics = panel.CreateGraphics();
			PanelGraphics.SetOptions();
		}

		public Point PanelPointToClient(Point p)
		{
			return panel.PointToClient(p);
		}
	}
}
