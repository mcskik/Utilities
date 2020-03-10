using System;
using System.Collections.Generic;
using System.Text;

namespace Notepod
{
    /// <summary>
    /// Create table statement to entity generator.
    /// </summary>
    /// <author>Kenneth McSkimming</author>
    public class CreateTableToEntityGenerator : BaseGenerator
    {
        #region Member variables.
        private TableMetaBlock _tableBlock;
        #endregion

        #region Constructors.
        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="lines">Array of lines from the text window.</param>
        public CreateTableToEntityGenerator(string[] inputLines) : base(inputLines)
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
                                                    table.TableName = line.Substring(nPos1 + 1, nPos2 - nPos1 - 1).Trim();
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
                                            if (line.ToUpper().Contains("NOT NULL"))
                                            {
                                                column.Nullable = false;
                                            }
                                            else
                                            {
                                                column.Nullable = true;
                                            }
                                            column.Name = line.Substring(nPos1 + 1, nPos2 - nPos1 - 1).Trim();
                                            nPos1 = line.IndexOf("[", nPos2);
                                            if (nPos1 > -1)
                                            {
                                                nPos2 = line.IndexOf("]", nPos1);
                                                if (nPos2 > -1)
                                                {
                                                    column.Type = line.Substring(nPos1 + 1, nPos2 - nPos1 - 1).Trim().ToLower();
                                                    bool hasDimentions = false;
                                                    if (column.Type.Contains("char"))
                                                    {
                                                        hasDimentions = true;
                                                    }
                                                    else
                                                    {
                                                        switch (column.Type)
                                                        {
                                                            case "decimal":
                                                                hasDimentions = true;
                                                                break;
                                                            case "number":
                                                                hasDimentions = true;
                                                                break;
                                                            default:
                                                                hasDimentions = false;
                                                                break;
                                                        }
                                                    }
                                                    column.Length = -1;
                                                    column.Precision = -1;
                                                    if (hasDimentions)
                                                    {
                                                        nPos1 = line.IndexOf("(", nPos2);
                                                        if (nPos1 > -1)
                                                        {
                                                            nPos2 = line.IndexOf(")", nPos1);
                                                            if (nPos2 > -1)
                                                            {
                                                                string dimensions = line.Substring(nPos1 + 1, nPos2 - nPos1 - 1);
                                                                if (dimensions.Contains(","))
                                                                {
                                                                    string[] components = dimensions.Split(',');
                                                                    column.Length = int.Parse(components[0]);
                                                                    column.Precision = int.Parse(components[1]);
                                                                }
                                                                else
                                                                {
                                                                    column.Length = int.Parse(dimensions);
                                                                    column.Precision = 0;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }    
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
            line = @"#region Member variables.";
            output.Add(line);
            foreach (TableColumn column in _tableBlock.Columns)
            {
                string dataType = "string";
                if (column.Type.Contains("char"))
                {
                    dataType = "string";
                }
                else
                {
                    switch (column.Type)
                    {
                        case "datetime":
                            dataType = "DateTime";
                            break;
                        case "decimal":
                            dataType = "decimal";
                            break;
                        case "number":
                            dataType = "decimal";
                            break;
                        case "smallint":
                            dataType = "short";
                            break;
                        case "int":
                            dataType = "int";
                            break;
                        case "bigint":
                            dataType = "long";
                            break;
                        case "bit":
                            dataType = "bool";
                            break;
                        default:
                            dataType = "string";
                            break;
                    }
                }
                string name = string.Empty;
                if (column.Name.ToLower() == "id")
                {
                    name = column.Name.ToUpper();
                }
                else
                {
                    name = LowerFirst(column.Name).Trim();
                }
                if (column.Nullable)
                {
                    if (dataType != "string")
                    {
                        dataType += "?";
                    }
                }
                line = "private " + dataType + " _" + name + ";";
                output.Add(line);
            }
            line = @"#endregion";
            output.Add(line);
            return output.ToArray();
        }        
        #endregion
    }
}