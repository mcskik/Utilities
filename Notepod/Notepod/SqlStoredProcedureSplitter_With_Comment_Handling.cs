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
    public class SqlStoredProcedureSplitter_With_Comment_Handling
    {
        #region Constants.
        private const string OUTPUT_PATH = @"C:\_N\Notepod\Notepod\Output\";
        #endregion

        #region Constructors.
        public SqlStoredProcedureSplitter_With_Comment_Handling()
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
            const string CREATE = @"CREATE ";
            const string PROCEDURE = @"Procedure ";
            List<string> comments = new List<string>();
            List<string> buffer = new List<string>();
            StreamReader sr = new StreamReader(new MemoryStream());
            string record = string.Empty;
            string line = string.Empty;
            string statement = string.Empty;
            string thisName = string.Empty;
            string nextName = string.Empty;
            bool startOfProcedure = false;
            bool commentBlock = false;
            bool commentLine = false;
            int pos = 0;
            try
            {
                comments = new List<string>();
                buffer = new List<string>();
                sr = new StreamReader(fileSpec);
                while ((record = sr.ReadLine()) != null)
                {
                    line = record.Trim();
                    //Look for "CREATE Procedure [dbo].[sp_StoredProcName]".
                    startOfProcedure = false;
                    if (line.StartsWith(CREATE, StringComparison.CurrentCultureIgnoreCase))
                    {
                        pos = line.IndexOf(CREATE, StringComparison.CurrentCultureIgnoreCase);
                        pos += CREATE.Length;
                        statement = line.Substring(pos).Trim();
                        if (statement.StartsWith(PROCEDURE, StringComparison.CurrentCultureIgnoreCase))
                        {
                            pos = statement.IndexOf(PROCEDURE, StringComparison.CurrentCultureIgnoreCase);
                            pos += PROCEDURE.Length;
                            statement = statement.Substring(pos);
                            thisName = nextName;
                            nextName = GetStoredProcedureName(statement);
                            startOfProcedure = true;
                        }
                    }
                    if (startOfProcedure)
                    {
                        if (thisName != string.Empty)
                        {
                            WriteFile(OUTPUT_PATH + thisName + ".sql", buffer);
                        }
                        buffer = new List<string>();
                        if (comments.Count > 0)
                        {
                            buffer.AddRange(comments);
                            comments = new List<string>();
                        }
                        buffer.Add(record);
                    }
                    else
                    {
                        commentLine = false;
                        if (line.StartsWith("//") || line.StartsWith("--"))
                        {
                            commentLine = true;
                        }
                        else
                        {
                            if (line.StartsWith(@"/*") && line.EndsWith(@"*/"))
                            {
                                commentLine = true;
                            }
                            else
                            {

                                if (line.StartsWith(@"/*"))
                                {
                                    commentBlock = true;
                                }
                                else
                                {
                                    if (line.EndsWith(@"*/"))
                                    {
                                        comments.Add(record);
                                        commentBlock = false;
                                    }
                                }
                            }
                        }
                        if (commentLine || commentBlock)
                        {
                            comments.Add(record);
                        }
                        else
                        {
                            if (comments.Count > 0)
                            {
                                buffer.AddRange(comments);
                                comments = new List<string>();
                            }
                            buffer.Add(record);
                        }
                    }
                }
                if (thisName != string.Empty)
                {
                    WriteFile(OUTPUT_PATH + thisName + ".sql", buffer);
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