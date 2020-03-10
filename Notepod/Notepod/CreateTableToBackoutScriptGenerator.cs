using System;
using System.Collections.Generic;
using System.Text;

namespace Notepod
{
    /// <summary>
    /// Create table statement to backout script generator.
    /// </summary>
    /// <author>Kenneth McSkimming</author>
    public class CreateTableToBackoutScriptGenerator : BaseGenerator
    {
        #region Member variables.
        private TableMetaBlock _tableBlock;
        #endregion

        #region Constructors.
        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="lines">Array of lines from the text window.</param>
        public CreateTableToBackoutScriptGenerator(string[] inputLines) : base(inputLines)
        {
        }
        #endregion

        #region Public methods.
        /// <summary>
        /// Import change number and table columns from create table statement.
        /// </summary>
        public override void Import()
        {
            ImportLines();
        }

        /// <summary>
        /// Generate backout script from change number and table columns.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        public override string[] Generate()
        {
            return GenerateLines();
        }
        #endregion

        #region Private methods.
        /// <summary>
        /// Import change number and table columns from create table statement.
        /// </summary>
        private void ImportLines()
        {
            _tableBlock = ImportCreateTableStatement();
        }

        /// <summary>
        /// Import change number and table columns from create table statement.
        /// </summary>
        /// <returns>Entity meta block.</returns>
        public TableMetaBlock ImportCreateTableStatement()
        {
            TableMetaBlock table = new TableMetaBlock();
            TableColumn column = null;
            string line = string.Empty;
            int nPos1 = 0;
            int nPos2 = 0;
            bool regionOpened = false;
            bool regionClosed = false;
            bool tableFound = false;
            bool tableOpened = false;
            bool tableClosed = false;
            int lineNumber = 0;
            while (!regionClosed && lineNumber < _inputLines.Length)
            {
                line = _inputLines[lineNumber].Trim();
                if (line.Length > 0)
                {
                    if (!line.StartsWith("//") && !line.StartsWith("--"))
                    {
                        if (!regionOpened)
                        {
                            if (line.Contains("#region Create"))
                            {
                                table = new TableMetaBlock();
                                nPos1 = line.IndexOf("[");
                                if (nPos1 > -1)
                                {
                                    nPos2 = line.IndexOf("]");
                                    if (nPos2 > -1)
                                    {
                                        table.ChangeNumber = line.Substring(nPos1 + 1, nPos2 - nPos1 - 1);
                                    }
                                }
                                regionOpened = true;
                            }
                        }
                        else
                        {
                            if (line.EndsWith("#endregion"))
                            {
                                regionClosed = true;
                            }
                            else
                            {
                                if (!tableOpened)
                                {
                                    if (!tableFound)
                                    {
                                        if (line.ToUpper().Contains("CREATE TABLE "))
                                        {
                                            nPos1 = line.LastIndexOf("[");
                                            if (nPos1 > -1)
                                            {
                                                nPos2 = line.LastIndexOf("]");
                                                if (nPos2 > -1)
                                                {
                                                    table.TableName = line.Substring(nPos1 + 1, nPos2 - nPos1 - 1);
                                                }
                                            }
                                            tableFound = true;
                                        }
                                    }
                                    if (line.StartsWith("["))
                                    {
                                        tableOpened = true;
                                    }
                                }
                                else
                                {
                                    if (!line.StartsWith("["))
                                    {
                                        tableClosed = true;
                                    }
                                }
                                if (tableOpened && !tableClosed)
                                {
                                    column = new TableColumn();
                                    nPos1 = line.IndexOf("[");
                                    if (nPos1 > -1)
                                    {
                                        nPos2 = line.IndexOf("]");
                                        if (nPos2 > -1)
                                        {
                                            if (line.ToUpper().Contains("IDENTITY"))
                                            {
                                                table.HasIdentityColumn = true;
                                            }
                                            column.Name = line.Substring(nPos1 + 1, nPos2 - nPos1 - 1);
                                            column.Type = "Not Used Yet";
                                        }
                                    }
                                    table.Columns.Add(column);
                                }
                            }
                        }
                    }
                }
                lineNumber++;
            }
            return table;
        }

        /// <summary>
        /// Generate backout script from change number and table columns.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateLines()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            line = @"--";
            output.Add(line);
            line = @"-- Step 1 : Restore original rows to affected tables.";
            output.Add(line);
            line = @"--";
            output.Add(line);
            line = @"PRINT 'Step 1 : Restore original rows to affected tables.'";
            output.Add(line);
            line = string.Empty;
            output.Add(line);
            line = @"SET XACT_ABORT ON";
            output.Add(line);
            line = string.Empty;
            output.Add(line);
            line = @"BEGIN TRANSACTION";
            output.Add(line);
            line = string.Empty;
            output.Add(line);
            if (_tableBlock.HasIdentityColumn)
            {
                line = @"SET IDENTITY_INSERT [dbo].[" + _tableBlock.TableName + @"] ON";
                output.Add(line);
            }
            line = @"INSERT INTO [dbo].[" + _tableBlock.TableName + @"]";
            output.Add(line);
            string comma = string.Empty;
            string columns = string.Empty;
            foreach (TableColumn column in _tableBlock.Columns)
            {
                columns += comma + "[" + column.Name + "]";
                comma = ", ";
            }
            line = @"(" + columns + @")";
            output.Add(line);
            line = @"SELECT " + columns;
            output.Add(line);
            line = @"FROM [dbo].[" + _tableBlock.TableName + @"_Backup_" + _tableBlock.ChangeNumber + @"]";
            output.Add(line);
            if (_tableBlock.HasIdentityColumn)
            {
                line = @"SET IDENTITY_INSERT [dbo].[" + _tableBlock.TableName + @"] OFF";
                output.Add(line);
            }
            line = string.Empty;
            output.Add(line);
            line = @"COMMIT TRANSACTION";
            output.Add(line);
            line = string.Empty;
            output.Add(line);
            line = @"SET XACT_ABORT OFF";
            output.Add(line);
            return output.ToArray();
        }        
        #endregion
    }
}