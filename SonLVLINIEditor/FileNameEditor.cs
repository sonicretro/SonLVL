using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.Security.Permissions;
using System.Windows.Forms;

namespace SonLVLINIEditor
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public class FileNameEditor : UITypeEditor
    {
        public FileNameEditor()
        {
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        // Displays the UI for value selection.
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            using (OpenFileDialog fd = new OpenFileDialog())
            {
                fd.DefaultExt = "bin";
                fd.Filter = "BIN Files|*.bin|All Files|*.*";
                if (System.IO.File.Exists((string)value))
                {
                    fd.InitialDirectory = System.IO.Path.GetDirectoryName((string)value);
                    fd.FileName = System.IO.Path.GetFileName((string)value);
                }
                if (fd.ShowDialog() == DialogResult.OK)
                    value = fd.FileName;
            }
            // Uses the IWindowsFormsEditorService to display a 
            // drop-down UI in the Properties window.
            /*IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc != null)
            {
                // Display an angle selection control and retrieve the value.
                IDControl idControl = new IDControl((ushort)value, edSvc);
                edSvc.DropDownControl(idControl);
                return idControl.value;
            }*/
            return value;
        }
    }
}
