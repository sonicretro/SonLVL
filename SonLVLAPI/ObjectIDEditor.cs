using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SonicRetro.SonLVL.API
{
	[DefaultEvent("ValueChanged")]
	[DefaultProperty("Value")]
	public class IDControl : UserControl
	{
		internal ImageList imageList1;
		private IContainer components;
		internal ListView listView1;
		private NumericUpDown numericUpDown1;

		public event EventHandler ValueChanged = delegate { };

		private byte _value;
		public byte Value
		{
			get { return _value; }
			private set
			{
				_value = value;
				ValueChanged(this, new EventArgs());
			}
		}
		private IWindowsFormsEditorService edSvc;

		public IDControl()
		{
			InitializeComponent();
		}

		public IDControl(byte val, IWindowsFormsEditorService edSvc)
		{
			Value = val;
			this.edSvc = edSvc;
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.listView1 = new System.Windows.Forms.ListView();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			this.SuspendLayout();
			// 
			// imageList1
			// 
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.imageList1.ImageSize = new System.Drawing.Size(32, 32);
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// listView1
			// 
			this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listView1.HideSelection = false;
			this.listView1.LargeImageList = this.imageList1;
			this.listView1.Location = new System.Drawing.Point(0, 0);
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(150, 130);
			this.listView1.TabIndex = 1;
			this.listView1.TileSize = new System.Drawing.Size(120, 48);
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Tile;
			this.listView1.ItemActivate += new System.EventHandler(this.listView1_ItemActivate);
			this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.numericUpDown1.Hexadecimal = true;
			this.numericUpDown1.Location = new System.Drawing.Point(0, 130);
			this.numericUpDown1.Maximum = new decimal(new int[] {
			255,
			0,
			0,
			0});
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(150, 20);
			this.numericUpDown1.TabIndex = 2;
			this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
			// 
			// IDControl
			// 
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.numericUpDown1);
			this.Name = "IDControl";
			this.Load += new System.EventHandler(this.IDControl_Load);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.IDControl_KeyDown);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			this.ResumeLayout(false);

		}

		private void IDControl_Load(object sender, EventArgs e)
		{
			listView1.BeginUpdate();
			listView1.Items.Clear();
			imageList1.Images.Clear();
			if (LevelData.ObjTypes != null)
				foreach (KeyValuePair<byte, ObjectDefinition> item in LevelData.ObjTypes)
				{
					imageList1.Images.Add(item.Value.Image.Image.ToBitmap(LevelData.BmpPal).Resize(imageList1.ImageSize));
					listView1.Items.Add(new ListViewItem(item.Value.Name, imageList1.Images.Count - 1) { Tag = item.Key, Selected = item.Key == Value });
				}
			listView1.EndUpdate();
			numericUpDown1.Value = Value;
		}

		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listView1.SelectedIndices.Count == 0) return;
			numericUpDown1.Value = Value = (byte)listView1.SelectedItems[0].Tag;
		}

		private void listView1_ItemActivate(object sender, EventArgs e)
		{
			if (edSvc != null)
				edSvc.CloseDropDown();
		}

		private void IDControl_KeyDown(object sender, KeyEventArgs e)
		{
			if (edSvc != null && e.KeyCode == Keys.Enter) edSvc.CloseDropDown();
		}

		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			Value = (byte)numericUpDown1.Value;
		}
	}

	[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
	public class IDEditor : UITypeEditor
	{
		public IDEditor()
		{
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}

		// Displays the UI for value selection.
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (!(context.Instance is ObjectEntry)) return value;
			// Uses the IWindowsFormsEditorService to display a 
			// drop-down UI in the Properties window.
			IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if (edSvc != null)
			{
				// Display an angle selection control and retrieve the value.
				IDControl idControl = new IDControl((byte)value, edSvc);
				edSvc.DropDownControl(idControl);
				return idControl.Value;
			}
			return value;
		}

		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override void PaintValue(PaintValueEventArgs e)
		{
			if (e.Value == null) return;
			byte val = (byte)e.Value;
			if (LevelData.ObjTypes.ContainsKey(val))
				e.Graphics.DrawImage(LevelData.ObjTypes[val].Image.Image.ToBitmap(LevelData.BmpPal).Resize(e.Bounds.Size), e.Bounds);
			else
				e.Graphics.DrawImage(LevelData.UnknownImg.Resize(e.Bounds.Size), e.Bounds);
		}

		public override bool IsDropDownResizable
		{
			get
			{
				return true;
			}
		}
	}
}
