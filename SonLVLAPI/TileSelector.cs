using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SonicRetro.SonLVL.API
{
	public class TileControl : UserControl
	{
		internal TileList listView1;
		public ushort value { get; private set; }
		private IWindowsFormsEditorService edSvc;

		public TileControl(ushort val, IWindowsFormsEditorService edSvc)
		{
			value = val;
			this.edSvc = edSvc;
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TileControl));
			this.listView1 = new SonicRetro.SonLVL.API.TileList();
			this.SuspendLayout();
			// 
			// listView1
			// 
			this.listView1.BackColor = System.Drawing.SystemColors.Window;
			this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listView1.ImageSize = 16;
			this.listView1.Location = new System.Drawing.Point(0, 0);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(150, 150);
			this.listView1.TabIndex = 1;
			this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
			this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
			// 
			// TileControl
			// 
			this.Controls.Add(this.listView1);
			this.Name = "TileControl";
			this.Load += new System.EventHandler(this.IDControl_Load);
			this.ResumeLayout(false);

		}

		private void IDControl_Load(object sender, EventArgs e)
		{
			listView1.Images.Clear();
			if (LevelData.Level.TwoPlayerCompatible)
			{
				listView1.ImageHeight = 32;
				for (int i = 0; i < LevelData.Tiles.Count - 1; i += 2)
					listView1.Images.Add(LevelData.InterlacedTileToBmp4bpp(LevelData.TileArray, i, 2));
			}
			else
				for (int i = 0; i < LevelData.Tiles.Count; i++)
					listView1.Images.Add(LevelData.TileToBmp4bpp(LevelData.Tiles[i], 0, 2));
			listView1.ChangeSize();
		}

		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listView1.SelectedIndex > -1)
				value = (ushort)listView1.SelectedIndex;
		}

		private void listView1_DoubleClick(object sender, EventArgs e)
		{
			edSvc.CloseDropDown();
		}
	}

	[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
	public class TileEditor : UITypeEditor
	{
		public TileEditor()
		{
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}

		// Displays the UI for value selection.
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			// Uses the IWindowsFormsEditorService to display a 
			// drop-down UI in the Properties window.
			IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			if (edSvc != null)
			{
				// Display an angle selection control and retrieve the value.
				TileControl idControl = new TileControl(ushort.Parse((string)value, System.Globalization.NumberStyles.HexNumber), edSvc);
				edSvc.DropDownControl(idControl);
				return idControl.value.ToString("X");
			}
			return value;
		}

		public override bool GetPaintValueSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override void PaintValue(PaintValueEventArgs e)
		{
			ushort val = ushort.Parse((string)e.Value, System.Globalization.NumberStyles.HexNumber);
			if (val >= LevelData.Tiles.Count) return;
			if (LevelData.Level.TwoPlayerCompatible)
				e.Graphics.DrawImage(LevelData.InterlacedTileToBmp4bpp(LevelData.TileArray, val, 2).Resize(e.Bounds.Size), e.Bounds);
			else
				e.Graphics.DrawImage(LevelData.TileToBmp4bpp(LevelData.Tiles[val], 0, 2).Resize(e.Bounds.Size), e.Bounds);
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
