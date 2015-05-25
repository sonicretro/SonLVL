using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SonAni
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
			Program.args = args;
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}
}
