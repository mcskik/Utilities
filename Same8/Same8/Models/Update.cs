using System;
using System.IO;

namespace Same8.Models
{
    /// <summary>
    /// Directory synchronisation update object.
    /// </summary>
    /// <remarks>
    /// To update files in each directory to achieve synchronisation.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    class Update
    {
        /// <summary>
        /// Copy source to target.
        /// </summary>
        /// <param name="psSource">Current task source.</param>
        /// <param name="psTarget">Current task target.</param>
        /// <param name="psFile">Current task file.</param>
        public static void Copy(string psSource, string psTarget, string psFile)
        {
            //Delete target file so that read-only files will not cause a problem.
            try
            {
                if (File.Exists(psTarget + psFile))
                {
                    //Do this to handle read-only files.
                    File.SetAttributes(psTarget + psFile, FileAttributes.Normal);
                    //Delete the file.
                    File.Delete(psTarget + psFile);
                }
            }
            catch
            {
                throw new ApplicationException(@"DSTX350E - Target file : """ + psFile + @""" could not be deleted prior to copy.");
            }

            //Perform main action.
            try
            {
                PathCheck(psTarget);
                File.Copy(psSource + psFile, psTarget + psFile, true);
            }
            catch
            {
                throw new ApplicationException(@"DSTX370E - Source file : """ + psFile + @""" not copied.");
            }
            return;
        }

        /// <summary>
        /// Move source to target.
        /// </summary>
        /// <param name="psSource">Current task source.</param>
        /// <param name="psTarget">Current task target.</param>
        /// <param name="psFile">Current task file.</param>
        public static void Move(string psSource, string psTarget, string psFile)
        {
            //Delete target file so that read-only files will not cause a problem.
            try
            {
                if (File.Exists(psTarget + psFile))
                {
                    //Do this to handle read-only files.
                    File.SetAttributes(psTarget + psFile, FileAttributes.Normal);
                    //Delete the file.
                    File.Delete(psTarget + psFile);
                }
            }
            catch
            {
                throw new ApplicationException(@"DSTX380E - Target file : """ + psFile + @""" could not be deleted prior to move.");
            }

            //Perform main action.
            try
            {
                if (File.Exists(psSource + psFile))
                {
                    PathCheck(psTarget);
                    File.Move(psSource + psFile, psTarget + psFile);
                }
            }
            catch
            {
                throw new ApplicationException(@"DSTX410E - Source file : """ + psFile + @""" not moved.");
            }
            return;
        }

        /// <summary>
        /// Delete source or target file.
        /// </summary>
        /// <param name="psLocation">Source or Target to label the original directory.</param>
        /// <param name="psOriginal">Original directory.</param>
        /// <param name="psFile">Current task file.</param>
        public static void Delete(string psLocation, string psOriginal, string psFile)
        {
            try
            {
                if (File.Exists(psOriginal + psFile))
                {
                    //Do this to handle read-only files.
                    File.SetAttributes(psOriginal + psFile, FileAttributes.Normal);
                    //Delete the file.
                    File.Delete(psOriginal + psFile);
                }
            }
            catch
            {
                throw new ApplicationException("DSTX440E - " + psLocation + @" file : """ + psFile + @""" not deleted.");
            }
            return;
        }

        /// <summary>
        /// Create all sub directories in the path to this file spec if necessary.
        /// </summary>
        /// <param name="psFileSpec">Full file specification.</param>
        public static void PathCheck(string psFileSpec)
        {
            string sPath = string.Empty;
            int nPos = 0;
            int nPtr = 0;
            try
            {
                nPos = 0;
                do
                {
                    nPos = psFileSpec.IndexOf(@"\", nPos + 1);
                    if (nPos > -1)
                    {
                        sPath = psFileSpec.Substring(0, nPos);
                        if (!Directory.Exists(sPath))
                        {
                            if (sPath.Substring(0, 2) == @"\\")
                            {
                                nPtr = sPath.IndexOf(@"\", 2);
                                if (nPtr > 0 && sPath.Length > nPtr)
                                {
                                    nPtr = sPath.IndexOf(@"\", nPtr + 1);
                                    if (nPtr > -1)
                                    {
                                        try
                                        {
                                            Directory.CreateDirectory(sPath);
                                        }
                                        catch
                                        {
                                            throw new ApplicationException(@"DSTX500I - Could Not Create """ + sPath + @""".");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                try
                                {
                                    Directory.CreateDirectory(sPath);
                                }
                                catch
                                {
                                    throw new ApplicationException(@"DSTX520I - Could Not Create """ + sPath + @""".");
                                }
                            }
                        }
                    }
                } while (nPos >= 0);
            }
            catch
            {
            }
            return;
        }

        /// <summary>
        /// Remove all sub directories in the path to this file spec if they empty.
        /// </summary>
        /// <param name="psFileSpec">Full file specification.</param>
        public static void PathRemove(string psFileSpec)
        {
            string sPath = string.Empty;
            int nPos = 0;
            try
            {
                sPath = psFileSpec;
                do
                {
                    nPos = sPath.LastIndexOf(@"\");
                    if (nPos >= 0)
                    {
                        sPath = sPath.Substring(0, nPos);
                        nPos = sPath.IndexOf(@"\");
                    }
                    if (nPos >= 2)
                    {
                        if (Directory.Exists(sPath))
                        {
                            try
                            {
                                Directory.Delete(sPath, false);
                            }
                            catch (Exception e)
                            {
                                throw new ApplicationException(e.Message);
                            }
                        }
                        else
                        {
                            sPath = string.Empty;
                        }
                    }
                    else
                    {
                        sPath = string.Empty;
                    }
                } while (sPath.Length > 0);
            }
            catch
            {
            }
            return;
        }
    }
}