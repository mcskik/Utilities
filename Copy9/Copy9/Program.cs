using Copy9.Models;
using Copy9.Presenters;
using Copy9.Views;
using System;
using System.Windows.Forms;
using A = Copy9.DataLayer.Profile.Administrator;

namespace Copy9
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the Copy9 application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
			CopyEngine copyEngine = new CopyEngine();
			frmMain compareViewer = new frmMain();
            DirectoryEngineL directoryEngine = new DirectoryEngineL();
            SynchronizeEngine synchronizeEngine = new SynchronizeEngine(new Actor());
            Presenter presenter = new Presenter(copyEngine, compareViewer, directoryEngine, synchronizeEngine, A.ProfileManager);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            Application.Run(compareViewer);
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message,"Test Harness");
        }
    }
}