using System;
using System.Windows.Forms;

namespace SonicRetro.SonLVL.GUI
{
    static class Program
    {
        internal static string[] args;
        internal static readonly bool IsMonoRuntime = Type.GetType("Mono.Runtime") != null;
        internal static readonly bool IsWindows = !(Environment.OSVersion.Platform == PlatformID.MacOSX | Environment.OSVersion.Platform == PlatformID.Unix | Environment.OSVersion.Platform == PlatformID.Xbox);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Program.args = args;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (MainForm.Instance != null)
            {
                MainForm.Instance.Log(e.ExceptionObject.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
                System.IO.File.WriteAllLines("SonLVL.log", MainForm.Instance.LogFile.ToArray());
                using (ErrorDialog ed = new ErrorDialog("Unhandled Exception " + e.ExceptionObject.GetType().Name + "\nLog file has been saved.", false))
                    ed.ShowDialog();
            }
            else
            {
                System.IO.File.WriteAllText("SonLVL.log", e.ExceptionObject.ToString());
                MessageBox.Show("Unhandled Exception " + e.ExceptionObject.GetType().Name + "\nLog file has been saved.", "SonLVL Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}