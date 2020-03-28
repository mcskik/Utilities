using Git8.DataLayer.Profile;
using Git8.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Git8.Views
{
    public partial class frmMain : Form
    {
        private const string GIT_INSTALLED_DIRECTORY = @"C:\Users\K\AppData\Local\Programs\Git\bin\";
        private const string GIT_EXECUTABLE = GIT_INSTALLED_DIRECTORY + @"git.exe";
        private const string GIT_REPOSITORY_PATH = @"C:\_Bitbucket\FA";
        private const string GIT_COMMANDER_TITLE = @"Git Commander";

        private Journaler _Journaler;

        public frmMain()
        {
            InitializeComponent();
            Boolean debug = Administrator.ProfileManager.SystemProfile.Debug;
            _Journaler = new Journaler(rtxOutput, rtxJournal);
            InitialiseFields();
        }

        #region Event Handlers.
        private void cboCommandLine_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Run();
            }
        }

        private void cmdRun_Click(object sender, EventArgs e)
        {
            Run();
        }

        private void cmdClear_Click(object sender, EventArgs e)
        {
            Administrator.ProfileManager.CommandSettings.Clear();
            _Journaler = new Journaler(rtxOutput, rtxJournal);
            InitialiseCommandField();
        }

        private void cmdSettings_Click(object sender, EventArgs e)
        {
            var frmRepository = new frmRepository();
            frmRepository.FormClosed += frmRepository_FormClosed;
            frmRepository.Show();
        }

        void frmRepository_FormClosed(object sender, FormClosedEventArgs e)
        {
            InitialiseFields();
        }

        private void cmdGitStatus_Click(object sender, EventArgs e)
        {
            string command = "git status";
            //setCboCommandTemplateText(command);
            //setCboCommandLineText(command);
            Run(command);
        }

        private void cmdGitLog_Click(object sender, EventArgs e)
        {
            string command = "git log --graph -5";
            //setCboCommandTemplateText(command);
            //setCboCommandLineText(command);
            Run(command);
        }

        private void cmdGitLogFilter_Click(object sender, EventArgs e)
        {
            string command = "git log --graph -5000";
            Run(command);
            Filter();
        }

        private void Filter()
        {
            const string COMMENT_PREFIX = "XXX-";
            const string DIGITS = "0123456789";
            SortedDictionary<string, string> comments = new SortedDictionary<string, string>();
            StringBuilder sb = new StringBuilder();
            string[] lines = rtxOutput.Lines;
            foreach (string line in lines)
            {
                string[] parts = line.Split(new string[] { COMMENT_PREFIX }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string part in parts)
                {
                    string prefix = string.Empty;
                    string numeric = string.Empty;
                    string remainder = string.Empty;
                    string comment = string.Empty;
                    for (int ptr = 0; ptr < part.Length; ptr++)
                    {
                        string digit = part.Substring(ptr, 1);
                        if (DIGITS.Contains(digit))
                        {
                            numeric += digit;
                        }
                        else
                        {
                            //If number ends before end of line.
                            if (numeric.Length > 0)
                            {
                                prefix = COMMENT_PREFIX + numeric;
                                remainder = part.Substring(ptr);
                                remainder = remainder.Trim();
                                comment = prefix + " " + remainder;
                                if (comments.ContainsKey(prefix))
                                {
                                    string existing = comments[prefix];
                                    int existingHyphens = CountLetter(existing, "-");
                                    int candidateHyphens = CountLetter(comment, "-");
                                    int commentLength = comment.Length;
                                    if (candidateHyphens < existingHyphens)
                                    {
                                        commentLength += 12;
                                    }
                                    string candidate = comment;
                                    if (candidateHyphens > 1)
                                    {
                                        candidate = MakeCommentFromBranchName(comment);
                                    }
                                    if (commentLength > comments[prefix].Length)
                                    {
                                        comments[prefix] = candidate;
                                    }
                                }
                                else
                                {
                                    int candidateHyphens = CountLetter(comment, "-");
                                    string candidate = comment;
                                    if (candidateHyphens > 1)
                                    {
                                        candidate = MakeCommentFromBranchName(comment);
                                    }
                                    comments[prefix] = candidate;
                                }
                            }
                            break;
                        }
                    }
                    //If number ends at end of line.
                    if (numeric.Length > 0)
                    {
                        prefix = COMMENT_PREFIX + numeric;
                        remainder = string.Empty;
                        comment = prefix;
                        if (comments.ContainsKey(prefix))
                        {
                            if (comment.Length > comments[prefix].Length)
                            {
                                comments[prefix] = comment;
                            }
                        }
                        else
                        {
                            comments.Add(prefix, comment);
                        }
                    }
                }
            }
            foreach (KeyValuePair<string, string> pair in comments)
            {
                sb.AppendLine(pair.Key);
            }
            foreach (KeyValuePair<string, string> pair in comments)
            {
                sb.AppendLine(pair.Value);
            }
            if (comments.Count > 0)
            {
                rtxOutput.Text = sb.ToString();
            }
        }

        private int CountLetter(string text, string character)
        {
            int count = 0;
            text = text.Trim();
            for (int pos = 0; pos < text.Length; pos++)
            {
                string letter = text.Substring(pos, 1);
                if (letter.CompareTo(character) == 0)
                {
                    count++;
                }
            }
            return count;
        }

        private static string MakeCommentFromBranchName(string text)
        {
            const int ZERO = 0;
            const string SPACE = " ";
            const string HYPHEN = "-";
            const string DIGITS = "0123456789";
            StringBuilder comment = new StringBuilder();
            text = text.Trim() + SPACE;
            int pos1 = text.LastIndexOf(HYPHEN);
            if (pos1 >= ZERO)
            {
                int pos2 = text.IndexOf(SPACE, pos1);
                if (pos2 >= ZERO)
                {
                    text = text.Substring(ZERO, pos2);
                }
                bool firstHyphenReached = false;
                bool firstDigit = false;
                bool endOfPrefix = false;
                bool firstDescription = false;
                for (int pos = ZERO; pos < text.Length; pos++)
                {
                    String letter = text.Substring(pos, 1);
                    if (letter == HYPHEN)
                    {
                        firstHyphenReached = true;
                        if (!endOfPrefix)
                        {
                            comment.Append(HYPHEN);
                            endOfPrefix = true;
                        }
                        else
                        {
                            comment.Append(SPACE);
                        }
                    }
                    else if (DIGITS.Contains(letter))
                    {
                        comment.Append(letter);
                    }
                    else
                    {
                        if (!firstHyphenReached)
                        {
                            comment.Append(letter.ToUpper());
                        }
                        else
                        {
                            if (!firstDescription)
                            {
                                if (letter != SPACE)
                                {
                                    firstDescription = true;
                                }
                                if (letter != HYPHEN)
                                {
                                    comment.Append(letter.ToUpper());
                                }
                            }
                            else
                            {
                                comment.Append(letter);
                            }
                        }
                    }
                }
            }
            return comment.ToString();
        }

        private void cmdParse_Click(object sender, EventArgs e)
        {
            ParseModifieddFiles();
        }

        private void cmdGitDiff_Click(object sender, EventArgs e)
        {
            string template = "git diff {FileSpec}";
            string command = TranslateFromTokens(template);
            setCboCommandTemplateText(template);
            setCboCommandLineText(command);
            Run(command);
        }

        private void cmdGitAdd_Click(object sender, EventArgs e)
        {
            string template = "git add {FileSpec}";
            string command = TranslateFromTokens(template);
            setCboCommandTemplateText(template);
            setCboCommandLineText(command);
            Run(command);
        }

        private void cboCommandLine_SelectedIndexChanged(object sender, EventArgs e)
        {
            //SaveCommandField();
            string template = TranslateToTokens(cboCommandLine.Text);
            setCboCommandTemplateText(template);
            SaveTemplateField();
        }

        private void cboCommandTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveTemplateField();
            synchroniseTemplateAndCommandFields();
        }

        private void cboCheckoutBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveCheckoutBranchField();
            synchroniseTemplateAndCommandFields();
        }

        private void cboRemoteBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveRemoteBranchField();
            synchroniseTemplateAndCommandFields();
        }

        private void cboLocalBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveLocalBranchField();
            synchroniseTemplateAndCommandFields();
        }

        private void cboFileSpec_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveFileSpecField();
            synchroniseTemplateAndCommandFields();
        }

        private void cboHead_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveHeadField();
            synchroniseTemplateAndCommandFields();
        }

        private void cboSha_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveShaField();
            synchroniseTemplateAndCommandFields();
        }

        private void cboComment_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveCommentField();
            synchroniseTemplateAndCommandFields();
        }

        private void cboStash_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveStashField();
            synchroniseTemplateAndCommandFields();
        }

        private void cboCommandLine_Leave(object sender, EventArgs e)
        {
            //SaveCommandField();
            string template = TranslateToTokens(cboCommandLine.Text);
            setCboCommandTemplateText(template);
            SaveTemplateField();
        }

        private void cboCommandTemplate_Leave(object sender, EventArgs e)
        {
            SaveTemplateField();
            synchroniseTemplateAndCommandFields();
        }

        private void rtxDescription_Leave(object sender, EventArgs e)
        {
            SaveTemplateField();
        }

        private void cboCheckoutBranch_Leave(object sender, EventArgs e)
        {
            SaveCheckoutBranchField();
            synchroniseTemplateAndCommandFields();
        }

        private void cboRemoteBranch_Leave(object sender, EventArgs e)
        {
            SaveRemoteBranchField();
            synchroniseTemplateAndCommandFields();
        }

        private void cboLocalBranch_Leave(object sender, EventArgs e)
        {
            SaveLocalBranchField();
            synchroniseTemplateAndCommandFields();
        }

        private void cboFileSpec_Leave(object sender, EventArgs e)
        {
            SaveFileSpecField();
            synchroniseTemplateAndCommandFields();
        }

        private void cboHead_Leave(object sender, EventArgs e)
        {
            SaveHeadField();
            synchroniseTemplateAndCommandFields();
        }

        private void cboSha_Leave(object sender, EventArgs e)
        {
            SaveShaField();
            synchroniseTemplateAndCommandFields();
        }

        private void cboComment_Leave(object sender, EventArgs e)
        {
            SaveCommentField();
            synchroniseTemplateAndCommandFields();
        }

        private void cboStash_Leave(object sender, EventArgs e)
        {
            SaveStashField();
            synchroniseTemplateAndCommandFields();
        }

        private void synchroniseTemplateAndCommandFields()
        {
            string command = TranslateFromTokens(cboCommandTemplate.Text);
            setCboCommandLineText(command);
            //SaveCommandField();
        }
        #endregion

        #region Private Methods.
        private void Run()
        {
            string command = cboCommandLine.Text;
            command = command.Trim();
            cboCommandLine.Text = command;
            if (command.Length > 0)
            {
                Run(command);
            }
        }

        private void Run(string command)
        {
            SaveFields();
            _Journaler.AppendCommand(command);
            string response = Commander.RunGitCommand(command);
            rtxOutput.Text = response;
            _Journaler.AppendResponse(response);
        }

        private void InitialiseFields()
        {
            this.Text = String.Format("{0} - [{1}]", GIT_COMMANDER_TITLE, Administrator.ProfileManager.RepositorySettings.SelectedKey);
            InitialiseCommandField();
            InitialiseTemplateField();
            InitialiseCheckoutBranchField();
            InitialiseRemoteBranchField();
            InitialiseLocalBranchField();
            InitialiseFileSpecField();
            InitialiseHeadField();
            InitialiseShaField();
            InitialiseCommentField();
            InitialiseStashField();
        }

        private void InitialiseCommandField()
        {
            this.cboCommandLine.SelectedIndexChanged -= new System.EventHandler(this.cboCommandLine_SelectedIndexChanged);
            //List<string> sortedCommandSettings = Administrator.ProfileManager.CommandSettings.Keys.ToList();
            //sortedCommandSettings.Sort();
            cboCommandLine.DataSource = Administrator.ProfileManager.CommandSettings.Keys.ToList();
            //cboCommandLine.DataSource = sortedCommandSettings;
            cboCommandLine.DropDownStyle = ComboBoxStyle.DropDown;
            setCboCommandLineText(Administrator.ProfileManager.CommandSettings.SelectedKey);
        }

        private void InitialiseTemplateField()
        {
            this.cboCommandTemplate.SelectedIndexChanged -= new System.EventHandler(this.cboCommandTemplate_SelectedIndexChanged);
            List<string> sortedTemplateSettings = Administrator.ProfileManager.TemplateSettings.Keys.ToList();
            sortedTemplateSettings.Sort();
            //cboCommandTemplate.DataSource = Administrator.ProfileManager.TemplateSettings.Keys.ToList();
            cboCommandTemplate.DataSource = sortedTemplateSettings;
            cboCommandTemplate.DropDownStyle = ComboBoxStyle.DropDown;
            setCboCommandTemplateText(Administrator.ProfileManager.TemplateSettings.SelectedKey);
            rtxDescription.Text = Administrator.ProfileManager.TemplateSettings.SelectedItem.Description;
        }

        private void InitialiseCheckoutBranchField()
        {
            this.cboCheckoutBranch.SelectedIndexChanged -= new System.EventHandler(this.cboCheckoutBranch_SelectedIndexChanged);
            List<string> sortedBranchCheckoutSettings = Administrator.ProfileManager.BranchCheckoutSettings.Keys.ToList();
            sortedBranchCheckoutSettings.Sort();
            //cboCheckoutBranch.DataSource = Administrator.ProfileManager.BranchCheckoutSettings.Keys.ToList();
            cboCheckoutBranch.DataSource = sortedBranchCheckoutSettings;
            cboCheckoutBranch.DropDownStyle = ComboBoxStyle.DropDown;
            setCboCheckoutBranchText(Administrator.ProfileManager.BranchCheckoutSettings.SelectedKey);
        }

        private void InitialiseRemoteBranchField()
        {
            this.cboRemoteBranch.SelectedIndexChanged -= new System.EventHandler(this.cboRemoteBranch_SelectedIndexChanged);
            List<string> sortedBranchRemoteSettings = Administrator.ProfileManager.BranchRemoteSettings.Keys.ToList();
            sortedBranchRemoteSettings.Sort();
            //cboRemoteBranch.DataSource = Administrator.ProfileManager.BranchRemoteSettings.Keys.ToList();
            cboRemoteBranch.DataSource = sortedBranchRemoteSettings;
            cboRemoteBranch.DropDownStyle = ComboBoxStyle.DropDown;
            setCboRemoteBranchText(Administrator.ProfileManager.BranchRemoteSettings.SelectedKey);
        }

        private void InitialiseLocalBranchField()
        {
            this.cboLocalBranch.SelectedIndexChanged -= new System.EventHandler(this.cboLocalBranch_SelectedIndexChanged);
            List<string> sortedBranchLocalSettings = Administrator.ProfileManager.BranchLocalSettings.Keys.ToList();
            sortedBranchLocalSettings.Sort();
            //cboLocalBranch.DataSource = Administrator.ProfileManager.BranchLocalSettings.Keys.ToList();
            cboLocalBranch.DataSource = sortedBranchLocalSettings;
            cboLocalBranch.DropDownStyle = ComboBoxStyle.DropDown;
            setCboLocalBranchText(Administrator.ProfileManager.BranchLocalSettings.SelectedKey);
        }

        private void InitialiseFileSpecField()
        {
            this.cboFileSpec.SelectedIndexChanged -= new System.EventHandler(this.cboFileSpec_SelectedIndexChanged);
            List<string> sortedFileSpecSettings = Administrator.ProfileManager.FileSpecSettings.Keys.ToList();
            sortedFileSpecSettings.Sort();
            //cboFileSpec.DataSource = Administrator.ProfileManager.FileSpecSettings.Keys.ToList();
            cboFileSpec.DataSource = sortedFileSpecSettings;
            cboFileSpec.DropDownStyle = ComboBoxStyle.DropDown;
            setCboFileSpecText(Administrator.ProfileManager.FileSpecSettings.SelectedKey);
        }

        private void InitialiseHeadField()
        {
            this.cboHead.SelectedIndexChanged -= new System.EventHandler(this.cboHead_SelectedIndexChanged);
            List<string> sortedHeadSettings = Administrator.ProfileManager.HeadSettings.Keys.ToList();
            sortedHeadSettings.Sort();
            //cboHead.DataSource = Administrator.ProfileManager.HeadSettings.Keys.ToList();
            cboHead.DataSource = sortedHeadSettings;
            cboHead.DropDownStyle = ComboBoxStyle.DropDown;
            setCboHeadText(Administrator.ProfileManager.HeadSettings.SelectedKey);
        }

        private void InitialiseShaField()
        {
            this.cboSha.SelectedIndexChanged -= new System.EventHandler(this.cboSha_SelectedIndexChanged);
            List<string> sortedShaSettings = Administrator.ProfileManager.ShaSettings.Keys.ToList();
            sortedShaSettings.Sort();
            //cboSha.DataSource = Administrator.ProfileManager.ShaSettings.Keys.ToList();
            cboSha.DataSource = sortedShaSettings;
            cboSha.DropDownStyle = ComboBoxStyle.DropDown;
            setCboShaText(Administrator.ProfileManager.ShaSettings.SelectedKey);
        }

        private void InitialiseCommentField()
        {
            this.cboComment.SelectedIndexChanged -= new System.EventHandler(this.cboComment_SelectedIndexChanged);
            List<string> sortedCommentSettings = Administrator.ProfileManager.CommentSettings.Keys.ToList();
            sortedCommentSettings.Sort();
            //cboComment.DataSource = Administrator.ProfileManager.CommentSettings.Keys.ToList();
            cboComment.DataSource = sortedCommentSettings;
            cboComment.DropDownStyle = ComboBoxStyle.DropDown;
            setCboCommentText(Administrator.ProfileManager.CommentSettings.SelectedKey);
        }

        private void InitialiseStashField()
        {
            this.cboStash.SelectedIndexChanged -= new System.EventHandler(this.cboStash_SelectedIndexChanged);
            List<string> sortedStashSettings = Administrator.ProfileManager.StashSettings.Keys.ToList();
            sortedStashSettings.Sort();
            //cboStash.DataSource = Administrator.ProfileManager.StashSettings.Keys.ToList();
            cboStash.DataSource = sortedStashSettings;
            cboStash.DropDownStyle = ComboBoxStyle.DropDown;
            setCboStashText(Administrator.ProfileManager.StashSettings.SelectedKey);
        }

        private void setCboCommandLineText(string text)
        {
            this.cboCommandLine.SelectedIndexChanged -= new System.EventHandler(this.cboCommandLine_SelectedIndexChanged);
            cboCommandLine.Text = text;
            this.cboCommandLine.SelectedIndexChanged += new System.EventHandler(this.cboCommandLine_SelectedIndexChanged);
        }
        private void setCboCommandTemplateText(string text)
        {
            this.cboCommandTemplate.SelectedIndexChanged -= new System.EventHandler(this.cboCommandTemplate_SelectedIndexChanged);
            cboCommandTemplate.Text = text;
            this.cboCommandTemplate.SelectedIndexChanged += new System.EventHandler(this.cboCommandTemplate_SelectedIndexChanged);
        }
        private void setCboCheckoutBranchText(string text)
        {
            this.cboCheckoutBranch.SelectedIndexChanged -= new System.EventHandler(this.cboCheckoutBranch_SelectedIndexChanged);
            cboCheckoutBranch.Text = text;
            this.cboCheckoutBranch.SelectedIndexChanged += new System.EventHandler(this.cboCheckoutBranch_SelectedIndexChanged);
        }
        private void setCboRemoteBranchText(string text)
        {
            this.cboRemoteBranch.SelectedIndexChanged -= new System.EventHandler(this.cboRemoteBranch_SelectedIndexChanged);
            cboRemoteBranch.Text = text;
            this.cboRemoteBranch.SelectedIndexChanged += new System.EventHandler(this.cboRemoteBranch_SelectedIndexChanged);
        }
        private void setCboLocalBranchText(string text)
        {
            this.cboLocalBranch.SelectedIndexChanged -= new System.EventHandler(this.cboLocalBranch_SelectedIndexChanged);
            cboLocalBranch.Text = text;
            this.cboLocalBranch.SelectedIndexChanged += new System.EventHandler(this.cboLocalBranch_SelectedIndexChanged);
        }
        private void setCboFileSpecText(string text)
        {
            this.cboFileSpec.SelectedIndexChanged -= new System.EventHandler(this.cboFileSpec_SelectedIndexChanged);
            cboFileSpec.Text = text;
            this.cboFileSpec.SelectedIndexChanged += new System.EventHandler(this.cboFileSpec_SelectedIndexChanged);
        }
        private void setCboHeadText(string text)
        {
            this.cboHead.SelectedIndexChanged -= new System.EventHandler(this.cboHead_SelectedIndexChanged);
            cboHead.Text = text;
            this.cboHead.SelectedIndexChanged += new System.EventHandler(this.cboHead_SelectedIndexChanged);
        }
        private void setCboShaText(string text)
        {
            this.cboSha.SelectedIndexChanged -= new System.EventHandler(this.cboSha_SelectedIndexChanged);
            cboSha.Text = text;
            this.cboSha.SelectedIndexChanged += new System.EventHandler(this.cboSha_SelectedIndexChanged);
        }
        private void setCboCommentText(string text)
        {
            this.cboComment.SelectedIndexChanged -= new System.EventHandler(this.cboComment_SelectedIndexChanged);
            cboComment.Text = text;
            this.cboComment.SelectedIndexChanged += new System.EventHandler(this.cboComment_SelectedIndexChanged);
        }
        private void setCboStashText(string text)
        {
            this.cboStash.SelectedIndexChanged -= new System.EventHandler(this.cboStash_SelectedIndexChanged);
            cboStash.Text = text;
            this.cboStash.SelectedIndexChanged += new System.EventHandler(this.cboStash_SelectedIndexChanged);
        }

        private void SaveFields()
        {
            SaveCommandField();
            SaveTemplateField();
            SaveCheckoutBranchField();
            SaveRemoteBranchField();
            SaveLocalBranchField();
            SaveFileSpecField();
            SaveHeadField();
            SaveShaField();
            SaveCommentField();
            SaveStashField();
        }

        private void SaveCommandField()
        {
            string text = cboCommandLine.Text;
            if (text.Trim().Length == 0) return;
            string template = TranslateToTokens(text);
            if (!Administrator.ProfileManager.CommandSettings.Keys.Contains(text))
            {
                CommandSetting commandSetting = new CommandSetting
                {
                    Key = text,
                    Line = text,
                    Template = template
                };
                Administrator.ProfileManager.CommandSettings.Persist(commandSetting);
                InitialiseCommandField();
            }
            else
            {
                Administrator.ProfileManager.CommandSettings.Select(text);
                //Administrator.ProfileManager.CommandSettings.SelectedItem.Line = text;
                //Administrator.ProfileManager.CommandSettings.Save();
            }
            //setCboCommandLineText(Administrator.ProfileManager.CommandSettings.SelectedKey);
        }

        private void SaveTemplateField()
        {
            string text = cboCommandTemplate.Text;
            string description = rtxDescription.Text;
            if (text.Trim().Length == 0)
            {
                Administrator.ProfileManager.TemplateSettings.Delete(Administrator.ProfileManager.TemplateSettings.SelectedKey);
                InitialiseTemplateField();
                return;
            }
            if (!Administrator.ProfileManager.TemplateSettings.Keys.Contains(text))
            {
                TemplateParameterSetting setting = new TemplateParameterSetting
                {
                    Key = text,
                    Value = text,
                    Description = description
                };
                Administrator.ProfileManager.TemplateSettings.Persist(setting);
                InitialiseTemplateField();
            }
            else
            {
                if (description != Administrator.ProfileManager.TemplateSettings.SelectedItem.Description)
                {
                    TemplateParameterSetting setting = new TemplateParameterSetting
                    {
                        Key = text,
                        Value = text,
                        Description = description
                    };
                    Administrator.ProfileManager.TemplateSettings.Persist(setting);
                }
                Administrator.ProfileManager.TemplateSettings.Select(text);
                rtxDescription.Text = Administrator.ProfileManager.TemplateSettings.SelectedItem.Description;
            }
        }

        private void SaveCheckoutBranchField()
        {
            string text = cboCheckoutBranch.Text;
            if (text.Trim().Length == 0)
            {
                Administrator.ProfileManager.BranchCheckoutSettings.Delete(Administrator.ProfileManager.BranchCheckoutSettings.SelectedKey);
                InitialiseCheckoutBranchField();
                return;
            }
            if (!Administrator.ProfileManager.BranchCheckoutSettings.Keys.Contains(text))
            {
                ParameterSetting setting = new ParameterSetting
                {
                    Key = text,
                    Value = text,
                };
                Administrator.ProfileManager.BranchCheckoutSettings.Persist(setting);
                InitialiseCheckoutBranchField();
            }
            else
            {
                Administrator.ProfileManager.BranchCheckoutSettings.Select(text);
            }
            // Change related fields.
            if (text.Trim().Length > 0)
            {
                cboRemoteBranch.Text = text;
                cboLocalBranch.Text = text;
                string currentCommentIssueIdentifier = GetIssueIdentifier(cboComment.Text);
                string newBranchIssueIdentifier = GetIssueIdentifier(text);
                if (newBranchIssueIdentifier.Length > 0)
                {
                    if (newBranchIssueIdentifier != currentCommentIssueIdentifier)
                    {
                        List<string> sortedComments = Administrator.ProfileManager.CommentSettings.Keys.ToList();
                        sortedComments.Sort();
                        string comment = string.Empty;
                        foreach (string c in sortedComments)
                        {
                            if (c.StartsWith(newBranchIssueIdentifier))
                            {
                                comment = c;
                                break;
                            }
                        }
                        if (comment.Length > 0)
                        {
                            cboComment.Text = comment;
                        }
                    }
                }
            }
        }

        private void SaveRemoteBranchField()
        {
            string text = cboRemoteBranch.Text;
            if (text.Trim().Length == 0)
            {
                Administrator.ProfileManager.BranchRemoteSettings.Delete(Administrator.ProfileManager.BranchRemoteSettings.SelectedKey);
                InitialiseRemoteBranchField();
                return;
            }
            if (!Administrator.ProfileManager.BranchRemoteSettings.Keys.Contains(text))
            {
                ParameterSetting setting = new ParameterSetting
                {
                    Key = text,
                    Value = text,
                };
                Administrator.ProfileManager.BranchRemoteSettings.Persist(setting);
                InitialiseRemoteBranchField();
            }
            else
            {
                Administrator.ProfileManager.BranchRemoteSettings.Select(text);
            }
        }

        private void SaveLocalBranchField()
        {
            string text = cboLocalBranch.Text;
            if (text.Trim().Length == 0)
            {
                Administrator.ProfileManager.BranchLocalSettings.Delete(Administrator.ProfileManager.BranchLocalSettings.SelectedKey);
                InitialiseLocalBranchField();
                return;
            }
            if (!Administrator.ProfileManager.BranchLocalSettings.Keys.Contains(text))
            {
                ParameterSetting setting = new ParameterSetting
                {
                    Key = text,
                    Value = text,
                };
                Administrator.ProfileManager.BranchLocalSettings.Persist(setting);
                InitialiseLocalBranchField();
            }
            else
            {
                Administrator.ProfileManager.BranchLocalSettings.Select(text);
            }
        }

        private void SaveFileSpecField()
        {
            string text = cboFileSpec.Text;
            if (text.Trim().Length == 0)
            {
                Administrator.ProfileManager.FileSpecSettings.Delete(Administrator.ProfileManager.FileSpecSettings.SelectedKey);
                InitialiseFileSpecField();
                return;
            }
            if (!Administrator.ProfileManager.FileSpecSettings.Keys.Contains(text))
            {
                ParameterSetting setting = new ParameterSetting
                {
                    Key = text,
                    Value = text,
                };
                Administrator.ProfileManager.FileSpecSettings.Persist(setting);
                InitialiseFileSpecField();
            }
            else
            {
                Administrator.ProfileManager.FileSpecSettings.Select(text);
            }
        }

        private void ParseModifieddFiles()
        {
            List<string> fileSpecs = Parser.ParseModifieddFiles(rtxOutput.Lines);
            if (fileSpecs.Count > 0)
            {
                if (Administrator.ProfileManager.FileSpecSettings.Keys.Count > 0)
                {
                    List<string> keys = new List<string>();
                    foreach (var key in Administrator.ProfileManager.FileSpecSettings.Keys)
                    {
                        keys.Add(key);
                    }
                    foreach (var key in keys)
                    {
                        Administrator.ProfileManager.FileSpecSettings.Delete(key);
                    }
                }
                foreach (var fileSpec in fileSpecs)
                {
                    SaveOneFileSpecField(fileSpec);
                }
                InitialiseFileSpecField();
            }
        }

        private void SaveOneFileSpecField(string fileSpec)
        {
            if (!Administrator.ProfileManager.FileSpecSettings.Keys.Contains(fileSpec))
            {
                ParameterSetting setting = new ParameterSetting
                {
                    Key = fileSpec,
                    Value = fileSpec,
                };
                Administrator.ProfileManager.FileSpecSettings.Persist(setting);
            }
        }

        private void SaveHeadField()
        {
            string text = cboHead.Text;
            if (text.Trim().Length == 0)
            {
                Administrator.ProfileManager.HeadSettings.Delete(Administrator.ProfileManager.HeadSettings.SelectedKey);
                InitialiseHeadField();
                return;
            }
            if (!Administrator.ProfileManager.HeadSettings.Keys.Contains(text))
            {
                ParameterSetting setting = new ParameterSetting
                {
                    Key = text,
                    Value = text,
                };
                Administrator.ProfileManager.HeadSettings.Persist(setting);
                InitialiseHeadField();
            }
            else
            {
                Administrator.ProfileManager.HeadSettings.Select(text);
            }
        }

        private void SaveShaField()
        {
            string text = cboSha.Text;
            if (text.Trim().Length == 0)
            {
                Administrator.ProfileManager.ShaSettings.Delete(Administrator.ProfileManager.ShaSettings.SelectedKey);
                InitialiseShaField();
                return;
            }
            if (!Administrator.ProfileManager.ShaSettings.Keys.Contains(text))
            {
                ParameterSetting setting = new ParameterSetting
                {
                    Key = text,
                    Value = text,
                };
                Administrator.ProfileManager.ShaSettings.Persist(setting);
                InitialiseShaField();
            }
            else
            {
                Administrator.ProfileManager.ShaSettings.Select(text);
            }
        }

        private void SaveCommentField()
        {
            string text = cboComment.Text;
            if (text.Trim().Length == 0)
            {
                Administrator.ProfileManager.CommentSettings.Delete(Administrator.ProfileManager.CommentSettings.SelectedKey);
                InitialiseCommentField();
                return;
            }
            if (!Administrator.ProfileManager.CommentSettings.Keys.Contains(text))
            {
                ParameterSetting setting = new ParameterSetting
                {
                    Key = text,
                    Value = text,
                };
                Administrator.ProfileManager.CommentSettings.Persist(setting);
                InitialiseCommentField();
            }
            else
            {
                Administrator.ProfileManager.CommentSettings.Select(text);
            }
            // Change related fields.
            if (text.Trim().Length > 0)
            {
                string currentBranchIssueIdentifier = GetIssueIdentifier(cboCheckoutBranch.Text);
                string newCommentIssueIdentifier = GetIssueIdentifier(text);
                if (newCommentIssueIdentifier.Length > 0)
                {
                    if (newCommentIssueIdentifier != currentBranchIssueIdentifier)
                    {
                        List<string> sortedBranches = Administrator.ProfileManager.BranchCheckoutSettings.Keys.ToList();
                        sortedBranches.Sort();
                        string branch = string.Empty;
                        foreach (string b in sortedBranches)
                        {
                            if (b.Contains(newCommentIssueIdentifier))
                            {
                                branch = b;
                                break;
                            }
                        }
                        if (branch.Length > 0)
                        {
                            cboCheckoutBranch.Text = branch;
                        }
                    }
                }
            }
        }

        private string GetIssueIdentifier(string text)
        {
            string issueIdentifier = string.Empty;
            if (text.Trim().Length > 0)
            {
                const string ISSUE_PREFIX = "SAA-";
                int pos1 = text.IndexOf(ISSUE_PREFIX);
                if (pos1 != -1)
                {
                    pos1 += ISSUE_PREFIX.Length;
                    int pos2 = text.IndexOf(" ", pos1);
                    if (pos2 == -1)
                    {
                        pos2 = text.IndexOf("-", pos1);
                    }
                    if (pos2 != -1)
                    {
                        issueIdentifier = ISSUE_PREFIX + text.Substring(pos1, pos2 - pos1);
                    }
                }
            }
            return issueIdentifier;
        }

        private void SaveStashField()
        {
            string text = cboStash.Text;
            if (text.Trim().Length == 0)
            {
                Administrator.ProfileManager.StashSettings.Delete(Administrator.ProfileManager.StashSettings.SelectedKey);
                InitialiseStashField();
                return;
            }
            if (!Administrator.ProfileManager.StashSettings.Keys.Contains(text))
            {
                ParameterSetting setting = new ParameterSetting
                {
                    Key = text,
                    Value = text,
                };
                Administrator.ProfileManager.StashSettings.Persist(setting);
                InitialiseStashField();
            }
            else
            {
                Administrator.ProfileManager.StashSettings.Select(text);
            }
        }

        private string TranslateFromTokens(string template)
        {
            return Translator.TranslateFromTokens(template, cboCheckoutBranch.Text, cboRemoteBranch.Text, cboLocalBranch.Text, cboFileSpec.Text, cboHead.Text, cboSha.Text, cboComment.Text, cboStash.Text);
        }

        private string TranslateToTokens(string command)
        {
            return Translator.TranslateToTokens(command, cboCheckoutBranch.Text, cboRemoteBranch.Text, cboLocalBranch.Text, cboFileSpec.Text, cboHead.Text, cboSha.Text, cboComment.Text, cboStash.Text);
        }
        #endregion
    }
}
