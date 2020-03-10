using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Notepod
{
    /// <summary>
    /// Splits SQL Server Management Studio generated stored procedure syntax into separate files.
    /// </summary>
    /// <author>Kenneth McSkimming</author>
    public class SqlStoredProcedureSplitter
    {
        #region Constants.
        private const string OUTPUT_PATH = @"C:\_N\Notepod\Notepod\Output\";
        #endregion

        #region Member Variables.
        private long _storedProcedureCount = 0;
        #endregion

        #region Constructors.
        public SqlStoredProcedureSplitter()
        {
        }
        #endregion

        #region Public methods.
        /// <summary>
        /// Split one big file into separate files for each stored procedure.
        /// </summary>
        /// <remarks>
        /// A contiguous block of comments immediately prior to a create procedure
        /// statement belongs to the new stored procedure.
        /// </remarks>
        public void Split(string fileSpec)
        {
            List<string> buffer = new List<string>();
            StreamReader sr = new StreamReader(new MemoryStream());
            string record = string.Empty;
            string line = string.Empty;
            string thisName = string.Empty;
            string nextName = string.Empty;
            bool startOfProcedure = false;
            try
            {
                buffer = new List<string>();
                sr = new StreamReader(fileSpec);
                while ((record = sr.ReadLine()) != null)
                {
                    line = record.Trim();
                    /// Look for "****** Object:  StoredProcedure [dbo].[spDailyUnitsInForce_Scheme_Policy]"
                    startOfProcedure = false;
                    if (line.StartsWith(@"/****** Object:  StoredProcedure", StringComparison.CurrentCultureIgnoreCase))
                    {
                        thisName = nextName;
                        nextName = GetStoredProcedureName(line);
                        startOfProcedure = true;
                    }
                    if (startOfProcedure)
                    {
                        if (thisName != string.Empty)
                        {
                            WriteFile(OUTPUT_PATH + thisName + ".sql", buffer);
                        }
                        buffer = new List<string>();
                        buffer.Add(record);
                    }
                    else
                    {
                        buffer.Add(record);
                    }
                }
                if (nextName != string.Empty)
                {
                    WriteFile(OUTPUT_PATH + nextName + ".sql", buffer);
                }
            }
            finally
            {
                sr.Close();
                sr.Dispose();
            }
        }
        #endregion

        #region Private methods.
        /// <summary>
        /// Get stored procedure name provided it is in the format [dbo].[storedProcedureName] or [storedProcedureName].
        /// </summary>
        private string GetStoredProcedureName(string text)
        {
            const string DBO = @"dbo";
            string spName = GetParameter(text);
            if (spName == DBO)
            {
                int pos = text.IndexOf(DBO + "]");
                if (pos > -1)
                {
                    pos += (DBO + "]").Length;
                    spName = GetParameter(text.Substring(pos + 1));
                }
            }
            return spName;
        }

        /// <summary>
        /// Get parameter provided it is in the format [parameter].
        /// </summary>
        private string GetParameter(string text)
        {
            string parameter = string.Empty;
            int pos1 = text.IndexOf("[");
            if (pos1 > -1)
            {
                int pos2 = text.IndexOf("]");
                if (pos2 > -1)
                {
                    parameter = text.Substring(pos1 + 1, pos2 - pos1 - 1);
                }
            }
            return parameter;
        }

        /// <summary>
        /// Write the supplied buffer to the specified file.
        /// </summary>
        public void WriteFile(string fileSpec, List<string> buffer)
        {
            _storedProcedureCount++;
            try
            {
                File.Delete(fileSpec);
            }
            catch
            {
            }
            using (StreamWriter sw = File.CreateText(fileSpec))
            {
                foreach (string line in buffer)
                {
                    sw.WriteLine(line);
                }
                sw.Close();
            }
        }
        #endregion
    }
}