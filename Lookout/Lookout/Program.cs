using Lookout.Models;
using Lookout.Presenters;
using Lookout.Views;
using System;
using System.Collections.Specialized;
using System.Windows.Forms;
using A = Lookout.DataLayer.Profile.Administrator;

namespace Lookout
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the aCopy8ication.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
			LookoutMover lookoutMover = new LookoutMover();
			frmMain lookoutViewer = new frmMain();
            Presenter presenter = new Presenter(lookoutMover, lookoutViewer, A.ProfileManager);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            Application.Run(lookoutViewer);
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message,"Test Harness");
        }
    }
}
