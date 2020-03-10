using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProfileData.DataLayer.Profile
{
    /// <summary>
    /// File helper class.
    /// </summary>
    /// <remarks>
    /// Contains methods which are commonly used to manipulate files.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public static class FileHelper
    {
        private const string CONNECTION_TEMPLATE_MDB = @"Provider=Microsoft.Jet.OLEDB.4.0; Data Source={0};Jet OLEDB:Engine Type=5";
        private static bool _cancelled = false;

        #region Public Methods.
        public static void Cancel()
        {
            _cancelled = true;
        }

        public static void EnsureExists(string fileSpec, string masterFileSpec)
        {
            if (File.Exists(masterFileSpec))
            {
                if (!File.Exists(fileSpec))
                {
                    FileHelper.PathCheck(fileSpec);
                    File.Copy(masterFileSpec, fileSpec);
                }
            }
        }

        public static void AllowOverwirte(string fileSpec)
        {
            if (File.Exists(fileSpec))
            {
                bool isReadOnly = ((File.GetAttributes(fileSpec) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly);
                if (isReadOnly)
                {
                    File.SetAttributes(fileSpec, FileAttributes.Normal);
                }
            }
        }

        public static void PathCheck(string fileSpec)
        {
            string separator = Path.DirectorySeparatorChar.ToString();
            string path = string.Empty;
            int pos = 0;
            do
            {
                pos = fileSpec.IndexOf(separator, pos + 1);
                if (pos > -1)
                {
                    path = fileSpec.Substring(0, pos);
                    if (!Directory.Exists(path) && !path.EndsWith(":"))
                    {
                        try
                        {
                            Directory.CreateDirectory(path);
                        }
                        catch
                        {
                        }
                    }
                }
            } while (pos >= 0);
        }

        public static string ReadFile(string fileSpec)
        {
            StreamReader sr = new StreamReader(fileSpec);
            string contents = sr.ReadToEnd();
            sr.Close();
            return contents;
        }

        public static void WriteFile(string fileSpec, string content)
        {
            if (File.Exists(fileSpec))
            {
                FileHelper.AllowOverwirte(fileSpec);
                File.Delete(fileSpec);
            }
            StreamWriter sw = File.CreateText(fileSpec);
            sw.Write(content);
            sw.Close();
        }

        /// <summary>
        /// Delete all files in the specified directory.
        /// </summary>
        public static void DeleteAllFiles(string directory)
        {
            _cancelled = false;
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                directoryInfo.Refresh();
                DirectoryInfo[] dirs = directoryInfo.GetDirectories();
                for (int row = 0; row < dirs.Length; row++)
                {
                    string name = dirs[row].Name;
                    string subDirectory = String.Format("{0}{1}{2}", directory, name, Path.DirectorySeparatorChar.ToString());
                    if (dirs[row].Exists)
                    {
                        DeleteAllFiles(subDirectory);
                        Directory.Delete(subDirectory);
                    }
                }
                FileInfo[] files = directoryInfo.GetFiles();
                for (int row = 0; row < files.Length; row++)
                {
                    try
                    {
                        string name = files[row].Name;
                        string spec = String.Format("{0}{1}", directory, name);
                        if (files[row].Exists)
                        {
                            FileHelper.AllowOverwirte(spec);
                            File.Delete(spec);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    if (_cancelled)
                    {
                        break;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Delete the the specified file.
        /// </summary>
        public static void Delete(string fileSpec)
        {
            try
            {
                if (File.Exists(fileSpec))
                {
                    FileHelper.AllowOverwirte(fileSpec);
                    File.Delete(fileSpec);
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Access database compact method.
        /// </summary>
        /// <remarks>
        /// JRO = Jet Replication Object.
        /// </remarks>
        public static void CompactDb(string fileSpec, string tempFileSpec)
        {
            object[] parameters;
            object jro = Activator.CreateInstance(Type.GetTypeFromProgID("JRO.JetEngine"));
            string connectionString = String.Format(CONNECTION_TEMPLATE_MDB, fileSpec);
            string temporaryConnectionString = String.Format(CONNECTION_TEMPLATE_MDB, tempFileSpec);
            File.Delete(tempFileSpec);
            parameters = new object[] { connectionString, temporaryConnectionString };
            jro.GetType().InvokeMember("CompactDatabase", System.Reflection.BindingFlags.InvokeMethod, null, jro, parameters);
            File.Delete(fileSpec);
            File.Move(tempFileSpec, fileSpec);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(jro);
            jro = null;
        }

        /// <summary>
        /// Copy the latest versions of files from a complete directory tree to another directory.
        /// </summary>
        /// <remarks>
        /// If the target files are newer they will not be overwritten.
        /// </remarks>
        public static void CopyIfNewer(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);
            XCopyIfNewer(diSource, diTarget);
        }

        /// <summary>
        /// Delete a complete directory tree and all its contents.
        /// </summary>
        public static void DeleteAll(string sourceDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            XDelete(diSource);
        }
        #endregion

        #region Private Methods.
        private static void XCopyIfNewer(DirectoryInfo sourceDirectory, DirectoryInfo targetDirectory)
        {
            if (!Directory.Exists(targetDirectory.FullName))
            {
                Directory.CreateDirectory(targetDirectory.FullName);
            }

            foreach (FileInfo fi in sourceDirectory.GetFiles())
            {
                string targetFile = Path.Combine(targetDirectory.ToString(), fi.Name);
                FileInfo targetFileInfo = new FileInfo(targetFile);
                bool doCopy = false;
                if (targetFileInfo.Exists)
                {
                    if (fi.LastWriteTime >= targetFileInfo.LastWriteTime)
                    {
                        doCopy = true;
                    }
                }
                else
                {
                    doCopy = true;
                }
                if (doCopy)
                {
                    if (targetFileInfo.Exists)
                    {
                        targetFileInfo.IsReadOnly = false;
                    }
                    fi.CopyTo(Path.Combine(targetDirectory.ToString(), fi.Name), true);
                    targetFileInfo = new FileInfo(targetFile);
                    if (targetFileInfo.Exists)
                    {
                        targetFileInfo.IsReadOnly = false;
                    }
                }
            }

            foreach (DirectoryInfo di in sourceDirectory.GetDirectories())
            {
                DirectoryInfo targetDirectoryInfo = targetDirectory.CreateSubdirectory(di.Name);
                XCopyIfNewer(di, targetDirectoryInfo);
            }
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
                    fi.IsReadOnly = false;
                    fi.Delete();
                }
            }

            if (Directory.Exists(sourceDirectory.FullName))
            {
                sourceDirectory.Delete();
            }
        }
        #endregion
    }
}