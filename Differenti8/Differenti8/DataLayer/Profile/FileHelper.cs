using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Differenti8.DataLayer.Profile
{
    public static class FileHelper
    {
        private const string CONNECTION_TEMPLATE_MDB = @"Provider=Microsoft.Jet.OLEDB.4.0; Data Source={0};Jet OLEDB:Engine Type=5";
        private static bool _cancelled = false;

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
        public static void CompactDb(string connectionString, string fileSpec, string tempFileSpec)
        {
            object[] parameters;
            object jro = Activator.CreateInstance(Type.GetTypeFromProgID("JRO.JetEngine"));
            string temporaryConnectionString = String.Format(CONNECTION_TEMPLATE_MDB, tempFileSpec);
            parameters = new object[] { connectionString, temporaryConnectionString };
            jro.GetType().InvokeMember("CompactDatabase", System.Reflection.BindingFlags.InvokeMethod, null, jro, parameters);
            File.Delete(fileSpec);
            File.Move(tempFileSpec, fileSpec);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(jro);
            jro = null;
        }
    }
}