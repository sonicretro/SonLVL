using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SonicRetro.SonLVL.LevelConverter
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (MainForm.Instance != null)
			{
				MainForm.Instance.Log(e.ExceptionObject.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
				System.IO.File.WriteAllLines("LevelConverter.log", MainForm.Instance.LogFile.ToArray());
				using (ErrorDialog ed = new ErrorDialog("Unhandled Exception " + e.ExceptionObject.GetType().Name + "\nLog file has been saved.", false))
					ed.ShowDialog();
			}
			else
			{
				System.IO.File.WriteAllText("LevelConverter.log", e.ExceptionObject.ToString());
				MessageBox.Show("Unhandled Exception " + e.ExceptionObject.GetType().Name + "\nLog file has been saved.", "LevelConverter Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
