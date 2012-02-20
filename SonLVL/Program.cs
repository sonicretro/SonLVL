using System;
using System.Windows.Forms;

namespace SonicRetro.SonLVL
{
    static class Program
    {
        internal static string[] args;

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
            if (LevelData.MainForm != null)
            {
                LevelData.MainForm.Log(e.ExceptionObject.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
                System.IO.File.WriteAllLines("SonLVL.log", LevelData.MainForm.LogFile.ToArray());
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
