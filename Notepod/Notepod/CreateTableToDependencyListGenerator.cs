using System;
using System.Collections.Generic;
using System.Text;

namespace Notepod
{
    /// <summary>
    /// Create table statement to dependency list generator.
    /// </summary>
    /// <author>Kenneth McSkimming</author>
    public class CreateTableToDependencyListGenerator : BaseGenerator
    {
        #region Member variables.
        private TableRelationshipMetaBlock _tableRelationshipBlock;
        #endregion

        #region Constructors.
        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="lines">Array of lines from the text window.</param>
        public CreateTableToDependencyListGenerator(string[] inputLines) : base(inputLines)
        {
        }
        #endregion

        #region Public methods.
        /// <summary>
        /// Import foreign key details from create table statement.
        /// </summary>
        public override void Import()
        {
            ImportLines();
        }

        /// <summary>
        /// Generate table dependency list from create table statement.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        public override string[] Generate()
        {
            return GenerateLines();
        }
        #endregion

        #region Private methods.
        /// <summary>
        /// Import relationship foreign key details from create table statement.
        /// </summary>
        private void ImportLines()
        {
            _tableRelationshipBlock = ImportForeignKeysFromCreateTableStatement();
        }

        /// <summary>
        /// Import relationship foreign key details from create table statement.
        /// </summary>
        /// <returns>Entity meta block.</returns>
        public TableRelationshipMetaBlock ImportForeignKeysFromCreateTableStatement()
        {
            TableRelationshipMetaBlock tableRelationshipBlock = new TableRelationshipMetaBlock();
            TableRelationship relationship = new TableRelationship();
            string line = string.Empty;
            int nPosAlterTable = 0;
            int nPosForeignKey = 0;
            int nPosReferences = 0;
            int nPos1 = 0;
            int nPos2 = 0;
            bool regionOpened = false;
            bool regionClosed = false;
            bool tableFound = false;
            bool tableOpened = false;
            bool tableClosed = false;
            bool tablePrimaryKeyOpened = false;
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
                                tableRelationshipBlock = new TableRelationshipMetaBlock();
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
                                                    tableRelationshipBlock.TableName = line.Substring(nPos1 + 1, nPos2 - nPos1 - 1);
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
                                    if (!tableClosed)
                                    {
                                        if (line.StartsWith("CONSTRAINT") && line.Contains("PRIMARY KEY"))
                                        {
                                            tablePrimaryKeyOpened = true;
                                        }
                                    }
                                    if (!line.StartsWith("["))
                                    {
                                        if (!tableClosed)
                                        {
                                            tableClosed = true;
                                        }
                                    }
                                }
                                if (tablePrimaryKeyOpened)
                                {
                                    if (line.StartsWith("["))
                                    {
                                        tableRelationshipBlock.PrimaryKey = GetParameter(line);
                                        tablePrimaryKeyOpened = false;
                                    }
                                }
                                if (tableOpened && tableClosed)
                                {
                                    //ALTER TABLE [dbo].[tblSource]  WITH CHECK ADD  CONSTRAINT [FK_tblSource_tblDivorceDetails] FOREIGN KEY([DivorceDetailID])
                                    //REFERENCES [dbo].[tblDivorceDetails] ([ID])
                                    nPosAlterTable = line.IndexOf("ALTER TABLE ");
                                    if (nPosAlterTable > -1)
                                    {
                                        relationship.Dependant.TableName = GetTableName(line.Substring(nPosAlterTable));
                                    }
                                    nPosForeignKey = line.IndexOf("FOREIGN KEY");
                                    if (nPosForeignKey > -1)
                                    {
                                        relationship.Dependant.ColumnName = GetParameter(line.Substring(nPosForeignKey));
                                    }
                                    nPosReferences = line.IndexOf("REFERENCES");
                                    if (nPosReferences > -1)
                                    {
                                        relationship.DependsOn.TableName = GetTableName(line.Substring(nPosReferences));
                                        nPos1 = line.IndexOf("(", nPosReferences);
                                        if (nPos1 > -1)
                                        {
                                            relationship.DependsOn.ColumnName = GetParameter(line.Substring(nPos1));
                                            tableRelationshipBlock.Relationships.Add(relationship);
                                            relationship = new TableRelationship();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                lineNumber++;
            }
            return tableRelationshipBlock;
        }

        /// <summary>
        /// Get table name provided it is in the format [dbo].[table] or [table].
        /// </summary>
        private string GetTableName(string text)
        {
            string tableName = GetParameter(text);
            if (tableName == "dbo")
            {
                int nPos1 = text.IndexOf("dbo]");
                if (nPos1 > -1)
                {
                    nPos1 += ("dbo]").Length;
                    tableName = GetParameter(text.Substring(nPos1 + 1));
                }
            }
            return tableName;
        }

        /// <summary>
        /// Get parameter provided it is in the format [parameter].
        /// </summary>
        private string GetParameter(string text)
        {
            string parameter = string.Empty;
            int nPos1 = text.IndexOf("[");
            if (nPos1 > -1)
            {
                int nPos2 = text.IndexOf("]");
                if (nPos2 > -1)
                {
                    parameter = text.Substring(nPos1 + 1, nPos2 - nPos1 - 1);
                }
            }
            return parameter;
        }

        /// <summary>
        /// Generate table dependency list from create table statement.
        /// </summary>
        /// <returns>Array of output lines to paste into the text window.</returns>
        private string[] GenerateLines()
        {
            string TAB = Convert.ToString((char)9);
            List<string> output = new List<string>();
            string line = string.Empty;
            if (_tableRelationshipBlock.Relationships.Count == 0)
            {
                line = _tableRelationshipBlock.TableName + TAB + _tableRelationshipBlock.PrimaryKey + TAB + "None" + TAB + "None";
                output.Add(line);
            }
            else
            {
                foreach (TableRelationship relationship in _tableRelationshipBlock.Relationships)
                {
                    line = relationship.Dependant.TableName + TAB + relationship.Dependant.ColumnName + TAB + relationship.DependsOn.TableName + TAB + relationship.DependsOn.ColumnName;
                    output.Add(line);
                }
            }
            return output.ToArray();
        }        
        #endregion
    }
}