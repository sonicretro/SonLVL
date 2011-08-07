using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SonicRetro.SonLVL
{
    public class BlockControl : UserControl
    {
        internal BlockList listView1;
        public ushort value { get; private set; }
        private IWindowsFormsEditorService edSvc;

        public BlockControl(ushort val, IWindowsFormsEditorService edSvc)
        {
            value = val;
            this.edSvc = edSvc;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.listView1 = new BlockList();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(150, 150);
            this.listView1.TabIndex = 1;
            this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.listView1.ImageSize = 16;
            // 
            // BlockControl
            // 
            this.Controls.Add(this.listView1);
            this.Name = "BlockControl";
            this.Load += new System.EventHandler(this.IDControl_Load);
            this.ResumeLayout(false);

        }

        private void IDControl_Load(object sender, EventArgs e)
        {
            listView1.Images = LevelData.BlockBmps;
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
    public class BlockEditor : UITypeEditor
    {
        public BlockEditor()
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
                BlockControl idControl = new BlockControl(ushort.Parse((string)value, System.Globalization.NumberStyles.HexNumber), edSvc);
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
            if (val >= LevelData.Blocks.Count) return;
            e.Graphics.DrawImage(LevelData.BlockBmps[val][0], e.Bounds);
            e.Graphics.DrawImage(LevelData.BlockBmps[val][1], e.Bounds);
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