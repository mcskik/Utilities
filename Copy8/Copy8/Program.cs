using Copy8.Presenters;
using Copy8.Views;
using System;
using System.Collections.Specialized;
using System.Windows.Forms;
using A = Copy8.DataLayer.Profile.Administrator;

namespace Copy8
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
			CopyEngine copyEngine = new CopyEngine();
			frmMain compareViewer = new frmMain();
            DirectoryEngine directoryEngine = new DirectoryEngine();
            SynchronizeEngine synchronizeEngine = new SynchronizeEngine();
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
