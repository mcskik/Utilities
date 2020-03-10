using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;
using Notepod.Models;
using System.Text;

namespace Notepod.Views
{
    /// <summary>
    /// Main form class.
    /// </summary>
    /// <author>Ken McSkimming</author>
    public partial class frmMain : Form
    {
        #region Member variables.
        private string _undoContents;
        #endregion

        #region Constructors.
        /// <summary>
        /// Default constructor.
        /// </summary>
        public frmMain()
        {
            InitializeComponent();
        }
        #endregion

        #region Event handlers.
        #region Generate event handlers.
        /// <summary>
        /// Generate properties from member variable declarations.
        /// </summary>
        private void memVarToPropToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            MemberVariableToPropertyGenerator generator = new MemberVariableToPropertyGenerator(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        /// <summary>
        /// Generate data members from member variable declarations.
        /// </summary>
        private void memVarToDataMemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            MemberVariableToDataMemberGenerator generator = new MemberVariableToDataMemberGenerator(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        /// <summary>
        /// Generate mock entities.
        /// </summary>
        private void mockEntGenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            MockEntityGenerator generator = new MockEntityGenerator(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        /// <summary>
        /// Generate mock entity validators.
        /// </summary>
        private void validatorGenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            EntityValidatorGenerator generator = new EntityValidatorGenerator(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        private void genMethodToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            GenerateMethodGenerator generator = new GenerateMethodGenerator(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        private void entListGenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            EntityListGenerator generator = new EntityListGenerator(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        private void strippedPropGenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            StrippedPropertyGenerator generator = new StrippedPropertyGenerator(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        private void enumMapGenToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void compareSameGenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            CompareSameGenerator generator = new CompareSameGenerator(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        private void constructorGenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            EntityConstructorGenerator generator = new EntityConstructorGenerator(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        private void templateGenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            TemplateGenerator generator = new TemplateGenerator(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        /// <summary>
        /// New mock entity generator.
        /// </summary>
        private void mockNewGenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            NewMockEntityGenerator generator = new NewMockEntityGenerator(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        /// <summary>
        /// Generate new syntax properties from member variable declarations.
        /// </summary>
        private void memToPropNewSyntaxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            MemberVariableToNewPropertySyntaxGenerator generator = new MemberVariableToNewPropertySyntaxGenerator(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        /// <summary>
        /// Generate backout script from change number and create table statement.
        /// </summary>
        private void tableToBackoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            CreateTableToBackoutScriptGenerator generator = new CreateTableToBackoutScriptGenerator(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        /// <summary>
        /// Generate table dependency list from create table statement.
        /// </summary>
        private void tableToDependenciesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            CreateTableToDependencyListGenerator generator = new CreateTableToDependencyListGenerator(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        /// <summary>
        /// Generate entity member variables from create table statement.
        /// </summary>
        private void tableToEntityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            CreateTableToEntityGenerator generator = new CreateTableToEntityGenerator(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        private void memVarToEntityLoadMethodToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            MemberVariableToEntityLoadMethodGenerator generator = new MemberVariableToEntityLoadMethodGenerator(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        private void memVarToEntityStoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            MemberVariableToEntityStoreMethodGenerator generator = new MemberVariableToEntityStoreMethodGenerator(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        private void memVarToEntityInitializeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            MemberVariableToEntityInitializeMethodGenerator generator = new MemberVariableToEntityInitializeMethodGenerator(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        /// <summary>
        /// Generate an aspx markup page using the information found in an original html page.
        /// </summary>
        private void hTMLToASPXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string basePath = @"C:\_A\SyntaxComputerServices\Syntax\Content\Pages\";
            string defaultFile = @"C:\_A\SyntaxComputerServices\Syntax\Content\Pages\Home.htm";
            string htmlFile = BrowseFile(basePath, defaultFile);
            if (File.Exists(htmlFile))
            {
                string[] lines = ReadFile(htmlFile);
                HtmlToAspxGenerator generator = new HtmlToAspxGenerator(lines);
                generator.HtmlFile = htmlFile;
                generator.Import();
                lines = generator.Generate();
                txtDocument.Lines = lines;
            }
        }

        /// <summary>
        /// Generate an XML test fragment creation method from a fragment of XML.
        /// </summary>
        private void xMLFragmentGenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            GenerateXmlFragmentCreationMethodGenerator generator = new GenerateXmlFragmentCreationMethodGenerator(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        /// <summary>
        /// Test only.
        /// </summary>
        private void txtDocument_DoubleClick(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            CreateTableToDependencyListGeneratorMultiTable generator = new CreateTableToDependencyListGeneratorMultiTable(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        /// <summary>
        /// Split a QueryString at every ampersand.
        /// </summary>
        private void splitQueryStringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            GenerateQueryStringSplit generator = new GenerateQueryStringSplit(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        /// <summary>
        /// Join and format a QueryString into traditional format.
        /// </summary>
        private void joinQueryStringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            GenerateQueryStringTraditional generator = new GenerateQueryStringTraditional(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        /// <summary>
        /// Join and format a QueryString into modern format.
        /// </summary>
        private void joinQueryStringModernToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            GenerateQueryStringModern generator = new GenerateQueryStringModern(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        /// <summary>
        /// Generate an XML modern format POST from a fragment of XML.
        /// </summary>
        private void xMLFragmentPostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            GenerateXmlFragmentModernPost generator = new GenerateXmlFragmentModernPost(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }
        #endregion
        #endregion

        #region Edit Event Handlers.
        #region Database Event Handlers.
        private void databaseWrapSQLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            EditDatabaseWrap generator = new EditDatabaseWrap(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        private void databaseUnwrapSQLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            EditDatabaseUnwrap generator = new EditDatabaseUnwrap(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        private void databaseInitialiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            for (int index = 0; index < lines.Length; index++)
            {
                string line = lines[index];
                string text = line.Trim();
                bool valid = false;
                int pos = text.IndexOf(" ");
                if (pos > -1)
                {
                    string modifier = text.Substring(0, pos);
                    text = text.Substring(pos + 1);
                    pos = text.IndexOf(" ");
                    if (pos > -1)
                    {
                        string type = text.Substring(0, pos);
                        string variable = string.Empty;
                        string property = string.Empty;
                        text = text.Substring(pos + 1);
                        pos = text.IndexOf(" ");
                        if (pos > -1)
                        {
                            variable = text.Substring(0, pos);
                            property = variable.Substring(2);
                            valid = true;
                        }
                        else
                        {
                            pos = text.IndexOf(";");
                            if (pos > -1)
                            {
                                variable = text.Substring(0, pos);
                                property = variable.Substring(2);
                                valid = true;
                            }
                        }
                        if (valid)
                        {
                            switch (type)
                            {
                                case "string":
                                    text = variable + " = string.Empty;";
                                    break;
                                case "int":
                                    text = variable + " = 0;";
                                    break;
                                case "long":
                                    text = variable + " = 0;";
                                    break;
                                case "double":
                                    text = variable + " = 0;";
                                    break;
                                case "decimal":
                                    text = variable + " = 0;";
                                    break;
                                case "DateTime":
                                    text = variable + " = DateTime.Now;";
                                    break;
                                default:
                                    text = variable + " = string.Empty;";
                                    break;
                            }
                        }
                    }
                }
                lines[index] = text;
            }
            txtDocument.Lines = lines;
        }
        #endregion

        #region Command Event Handlers.
        private void wrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditCommandsTokenizer tokenizer = new EditCommandsTokenizer();
            string[] lines = tokenizer.TokenizeLines(txtDocument.Lines);
            EditCommandsFormat formatter = new EditCommandsFormat(lines);
            formatter.Import();
            lines = formatter.Generate();
            lines = tokenizer.DeTokenizeLines(lines, false);
            EditCommandsWrap wrapper = new EditCommandsWrap(lines);
            wrapper.Import();
            lines = wrapper.Generate();
            txtDocument.Lines = tokenizer.DeTokenizeLines(lines);
        }

        private void unwrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditCommandsTokenizer tokenizer = new EditCommandsTokenizer();
            string[] lines = tokenizer.TokenizeLines(txtDocument.Lines);
            lines = tokenizer.DeTokenizeLines(lines, false);
            EditCommandsUnwrap generator = new EditCommandsUnwrap(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = tokenizer.DeTokenizeLines(lines);
        }

        private void compressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditCommandsTokenizer tokenizer = new EditCommandsTokenizer();
            string[] lines = tokenizer.TokenizeLines(txtDocument.Lines);
            EditCommandsCompress generator = new EditCommandsCompress(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = tokenizer.DeTokenizeLines(lines);
        }

        private void uncompressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditCommandsTokenizer tokenizer = new EditCommandsTokenizer();
            string[] lines = tokenizer.TokenizeLines(txtDocument.Lines);
            EditCommandsUncompress generator = new EditCommandsUncompress(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = tokenizer.DeTokenizeLines(lines);
        }

        private void formatToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            EditCommandsTokenizer tokenizer = new EditCommandsTokenizer();
            string[] lines = tokenizer.TokenizeLines(txtDocument.Lines);
            EditCommandsFormat generator = new EditCommandsFormat(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = tokenizer.DeTokenizeLines(lines);
        }
        #endregion

        #region Traditional Event Handlers.
        private void wrapQSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            EditTraditionalWrapQueryString generator = new EditTraditionalWrapQueryString(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        private void wrapPDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            EditTraditionalWrapPostData generator = new EditTraditionalWrapPostData(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        private void wrapXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            EditTraditionalWrapXml generator = new EditTraditionalWrapXml(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        private void unwrapToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            EditTraditionalUnwrap generator = new EditTraditionalUnwrap(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        private void compressToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            EditCommandsCompress generator = new EditCommandsCompress(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        private void uncompressToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            EditCommandsUncompress generator = new EditCommandsUncompress(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        private void formatToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            EditCommandsFormat generator = new EditCommandsFormat(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }
        #endregion

        #region Other Event Handlers.
        private void otherReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmParameters parameters = new frmParameters();
            DialogResult result = parameters.ShowDialog();
            if (result == DialogResult.OK)
            {
                string replace = parameters.Replace;
                string with = parameters.With;
                if (replace.Length > 0)
                {
                    replace = SubstituteShorthand(replace);
                    if (with.Length > 0)
                    {
                        with = SubstituteShorthand(with);
                        txtDocument.Text = txtDocument.Text.Replace(replace, with);
                    }
                }
            }
        }

        private void splitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSplitJoinParameters parameters = new frmSplitJoinParameters();
            DialogResult result = parameters.ShowDialog();
            if (result == DialogResult.OK)
            {
                string delimiter = parameters.Delimiter;
                if (delimiter.Length > 0)
                {
                    string[] lines = txtDocument.Text.Split(new string[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);
                    StringBuilder sb = new StringBuilder();
                    for (int index = 0; index < lines.Length - 1; index++)
                    {
                        var line = lines[index];
                        sb.AppendLine(line);
                    }
                    sb.Append(lines[lines.Length - 1]);
                    txtDocument.Text = sb.ToString();
                }
            }
        }

        private void joinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSplitJoinParameters parameters = new frmSplitJoinParameters();
            DialogResult result = parameters.ShowDialog();
            if (result == DialogResult.OK)
            {
                string delimiter = parameters.Delimiter;
                if (delimiter.Length > 0)
                {
                    string[] lines = txtDocument.Lines;
                    StringBuilder sb = new StringBuilder();
                    for (int index = 0; index < lines.Length - 1; index++)
                    {
                        var line = lines[index];
                        sb.Append(String.Format(@"{0}{1}", line, delimiter));
                    }
                    sb.Append(lines[lines.Length - 1]);
                    txtDocument.Text = sb.ToString();
                }
            }
        }

        private void otherSwapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmParameters parameters = new frmParameters();
            DialogResult result = parameters.ShowDialog();
            if (result == DialogResult.OK)
            {
                string swapAt = parameters.SwapAt;
                if (swapAt.Length > 0)
                {
                    swapAt = SubstituteShorthand(swapAt);
                    string[] lines = txtDocument.Lines;
                    for (int index = 0; index < lines.Length; index++)
                    {
                        string text = string.Empty;
                        string line = lines[index];
                        int pos = line.IndexOf(swapAt);
                        if (pos > -1)
                        {
                            text = line.Substring(pos + swapAt.Length);
                            text += swapAt;
                            text += line.Substring(0, pos);
                        }
                        else
                        {
                            text = line;
                        }
                        lines[index] = text;
                    }
                    txtDocument.Lines = lines;
                }
            }
        }

        private void otherSingleWrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            for (int index = 0; index < lines.Length; index++)
            {
                string line = lines[index];
                string text = line.Trim();
                text = "{S}" + text + "{E}";
                lines[index] = text;
            }
            txtDocument.Lines = lines;
        }

        private void otherDoubleWrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            for (int index = 0; index < lines.Length; index++)
            {
                string line = lines[index];
                string text = line.Trim();
                text = "{S}" + text + "{M}" + text + "{E}";
                lines[index] = text;
            }
            txtDocument.Lines = lines;

        }

        private void otherTrebleWrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            for (int index = 0; index < lines.Length; index++)
            {
                string line = lines[index];
                string text = line.Trim();
                text = "{S}" + text + "{M1}" + text + "{M2}" + text + "{E}";
                lines[index] = text;
            }
            txtDocument.Lines = lines;
        }

        private void otherCsvTextToXmlStringsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            EditOtherCsvTextToXmlStrings generator = new EditOtherCsvTextToXmlStrings(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }

        private void otherCsvIsoCodesToJsonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            EditOtherCsvIsoCodesToJsonStrings generator = new EditOtherCsvIsoCodesToJsonStrings(lines);
            generator.Import();
            lines = generator.Generate();
            txtDocument.Lines = lines;
        }
        #endregion

        #region Splitter
        private void splitterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SqlStoredProcedureSplitter splitter = new SqlStoredProcedureSplitter();
            splitter.Split(@"C:\_N\Notepod\Notepod\Input\StoredProcs.sql");
        }
        #endregion

        #region JSON
        private void jsonFormatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtDocument.Text = JsonFormatter.Format(txtDocument.Text);
        }

        private void jsonUnformatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtDocument.Text = JsonUnformatter.Unformat(txtDocument.Text);
        }

        private void jsonTMWrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string leader = @"{'localesSupportedByMomondo':";
            string apost = "'";
            string trailer = @"'}";
            int pos = txtDocument.Text.IndexOf(leader);
            if (pos == -1)
            {
                txtDocument.Text = String.Format(@"{0}{1}{2}{3}", leader, apost, txtDocument.Text, trailer);
            }
        }

        private void jsonTMUnwrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string leader = @"{'localesSupportedByMomondo':";
            int pos = txtDocument.Text.IndexOf(leader);
            if (pos > -1)
            {
                pos = txtDocument.Text.IndexOf(":", pos);
                if (pos > -1)
                {
                    pos = txtDocument.Text.IndexOf("'", pos);
                    if (pos > -1)
                    {
                        txtDocument.Text = txtDocument.Text.Substring(pos + 1);
                    }
                }
            }
            string trailer = @"'}";
            pos = txtDocument.Text.LastIndexOf(trailer);
            if (pos > -1)
            {
                txtDocument.Text = txtDocument.Text.Substring(0, pos);
            }
        }
        #endregion

        #region XML
        private void formatToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            EditXmlFormat formatter = new EditXmlFormat(lines);
            formatter.Import();
            lines = formatter.Generate();
            txtDocument.Lines = lines;
        }

        private void unformatToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void formatLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] lines = txtDocument.Lines;
            EditLayoutFormat formatter = new EditLayoutFormat(lines);
            formatter.Import();
            lines = formatter.Generate();
            txtDocument.Lines = lines;
        }

        private void unformatLToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        #endregion
        #endregion

        #region Copy and paste event handlers.
        /// <summary>
        /// Copy.
        /// </summary>
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtDocument.Copy();
        }

        /// <summary>
        /// Copy.
        /// </summary>
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtDocument.Paste();
            _undoContents = txtDocument.Text;
        }

        /// <summary>
        /// Select all.
        /// </summary>
        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtDocument.SelectAll();
        }

        /// <summary>
        /// Undo.
        /// </summary>
        /// <remarks>
        /// Only goes back to the contents as they were after the last [ctrl] + V.
        /// This may be enhanced to a multi-level undo in the future.
        /// </remarks>
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //txtDocument.SelectAll();
            //txtDocument.Cut();
            txtDocument.Text = _undoContents;
        }
        #endregion

        #region Private Methods.
        /// <summary>
        /// Substitute shorthand.
        /// </summary>
        private string SubstituteShorthand(string shorthand)
        {
            StringDictionary substitutionCodes = new StringDictionary();
            substitutionCodes.Add("{TAB}", Convert.ToString((char)9));
            substitutionCodes.Add("{CR}", Convert.ToString((char)13));
            substitutionCodes.Add("{LF}", Convert.ToString((char)10));
            substitutionCodes.Add("{CRLF}", Convert.ToString((char)13) + Convert.ToString((char)10));
            substitutionCodes.Add("{EMPTY}", string.Empty);
            foreach (DictionaryEntry entry in substitutionCodes)
            {
                shorthand = shorthand.Replace(((string)entry.Key).ToUpper(), (string)entry.Value);
            }
            return shorthand;
        }

        /// <summary>
        /// File browser.
        /// </summary>
        /// <param name="psBase">Base directory to start from.</param>
        /// <param name="psDefault">Default file to use if no file selected.</param>
        /// <returns>Selected or default file.</returns>
        private string BrowseFile(string psBase, string psDefault)
        {
            string sFile = psDefault;
            OpenFileDialog dlgFile = new OpenFileDialog();
            dlgFile.InitialDirectory = psBase;
            dlgFile.AddExtension = true;
            dlgFile.CheckPathExists = true;
            dlgFile.CheckFileExists = true;
            dlgFile.DefaultExt = "txt";
            dlgFile.Filter = "All files (*.*)|*.*|Text files (*.txt)|*.txt|Word files (*.doc)|*.doc";
            dlgFile.FilterIndex = 0;
            DialogResult oDr = dlgFile.ShowDialog();
            if (oDr == DialogResult.OK)
            {
                sFile = dlgFile.FileName;
            }
            return sFile;
        }

        /// <summary>
        /// Read the specified file and return its contents as a string array.
        /// </summary>
        /// <param name="fileSpec">Text file specification.</param>
        /// <returns>Entire file contents as a string array.</returns>
        public string[] ReadFile(string fileSpec)
        {
            List<string> inputLines = new List<string>();
            StreamReader sr = new StreamReader(new MemoryStream());
            string record;
            try
            {
                sr = new StreamReader(fileSpec);
                while ((record = sr.ReadLine()) != null)
                {
                    inputLines.Add(record);
                }
            }
            finally
            {
                sr.Close();
                sr.Dispose();
            }
            return inputLines.ToArray();
        }
        #endregion
    }
}