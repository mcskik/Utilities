using Git8.DataLayer.Profile;
using ProfileData.DataLayer.Profile;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Git8.Models
{
    /// <summary>
    /// Journaler class.
    /// </summary>
    /// <remarks>
    /// This class keeps a journal of all commands submitted and all command responses returned.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public class Journaler
    {
        private const char UNDERLINE = '=';
        private const string ALLOWED_CHARACTERS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
        private const string UNDERSCORE = "_";
        private const string EXT = ".txt";

        private RichTextBox _RtxOutput;
        private RichTextBox _RtxJournal;
        private StringBuilder _Output;
        private StringBuilder _Journal;
        private StreamWriter _streamWriter;

        public string CommandFileSpec { get; set; }
        public string JournalFileSpec { get; set; }

        public Journaler(RichTextBox rtxOutput, RichTextBox rtxJournal)
        {
            _RtxOutput = rtxOutput;
            _RtxJournal = rtxJournal;
            _Output = new StringBuilder();
            _Journal = new StringBuilder();
            Refresh();
            string journalPath = Administrator.ProfileManager.SystemProfile.JournalPath;
            JournalFileSpec = String.Format(@"{0}{1}_{2}.log", journalPath, "Git_Journal", DateTime.Now.ToString("yyyyMMdd@HHmmss"));
            Begin();
        }

        public void Begin()
        {
            PathCheck(JournalFileSpec);
            if (File.Exists(JournalFileSpec))
            {
                File.Delete(JournalFileSpec);
            }
            _streamWriter = File.CreateText(JournalFileSpec);
        }

        public void Write(string content)
        {
            _streamWriter.Write(content);
            _streamWriter.Flush();
        }

        public void WriteLine()
        {
            WriteLine(string.Empty);
        }

        public void WriteLine(string line)
        {
            _streamWriter.WriteLine(line);
            _streamWriter.Flush();
        }

        public void AppendCommand(string command)
        {
            CommandFileSpec = Administrator.ProfileManager.SystemProfile.OutputCommandsPath + GenerateFileName(command);
            string underline = new string(UNDERLINE, command.Length);
            _Output = new StringBuilder();
            _Output.AppendLine(command);
            _Output.AppendLine(underline);
            _Journal.AppendLine(command);
            _Journal.AppendLine(underline);
            Refresh();
        }

        private string GenerateFileName(string command)
        {
            StringBuilder fileName = new StringBuilder();
            char[] letters = command.ToCharArray();
            foreach (var letter in letters)
            {
                if (ALLOWED_CHARACTERS.Contains(letter.ToString()))
                {
                    fileName.Append(letter);
                }
                else
                {
                    fileName.Append(UNDERSCORE);
                }
            }
            fileName.Append(UNDERSCORE);
            fileName.Append(DateTime.Now.ToString("yyyyMMdd@HHmmss"));
            fileName.Append(EXT);
            return fileName.ToString();
        }

        public void AppendResponse(string response)
        {
            _Output.AppendLine(response);
            string output = _Output.ToString();
            FileHelper.WriteFile(CommandFileSpec, output);
            Write(output);
            //WriteLine();
            _Journal.AppendLine(response);
            //_Journal.AppendLine();
            Refresh();
        }

        private void Refresh()
        {
            _RtxOutput.Text = _Output.ToString();
            _RtxJournal.Text = _Journal.ToString();
            _RtxJournal.SelectionStart = _RtxJournal.Text.Length;
            _RtxJournal.ScrollToCaret();
        }

        private void PathCheck(string fileSpec)
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
    }
}