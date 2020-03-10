using System;
using System.IO;

namespace Lookout.Models
{
    /// <summary>
    /// Delete helper class.
    /// </summary>
    /// <remarks>
    /// I have to have a copy of the DeleteAll method inside this project so that I can decide
    /// if I can safely ignore certain errors.  This code was taken from the shared FileHelper class.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public static class DeleteHelper
    {
        /// <summary>
        /// Delete a complete directory tree and all its contents.
        /// </summary>
        public static void DeleteAll(string sourceDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            XDelete(diSource);
        }

        private static void XDelete(DirectoryInfo sourceDirectory)
        {
            foreach (DirectoryInfo di in sourceDirectory.GetDirectories())
            {
                XDelete(di);
            }
            foreach (FileInfo fi in sourceDirectory.GetFiles())
            {
                if (fi.Exists)
                {
                    try
                    {
                        fi.IsReadOnly = false;
                        fi.Delete();
                    }
                    catch (Exception ex)
                    {
                        string message = ex.Message;
                    }
                }
            }
            if (Directory.Exists(sourceDirectory.FullName))
            {
                try
                {
                    sourceDirectory.Delete();
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                }
            }
        }
    }
}