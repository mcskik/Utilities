using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Differenti8.Views;
using Differenti8.Presenters;
using Differenti8Engine;
using A = Differenti8.DataLayer.Profile.Administrator;

namespace Differenti8
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the aDifferenti8ication.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
			CompareEngine compareEngine = new CompareEngine();
			frmMain compareViewer = new frmMain();
            Presenter presenter = new Presenter(compareEngine, compareViewer, A.ProfileManager);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            Application.Run(compareViewer);
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message,"Test Harness");
        }
    }
}
