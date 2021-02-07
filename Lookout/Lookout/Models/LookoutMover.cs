using Lookout.DataLayer.Profile;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Lookout.Models
{
    public class LookoutMover : ILookoutMover
    {
        #region Member variables.
        private DateTime completeTaskStartTime;
        private Interrupt interrupt;
        public SortedDictionary<string, int> _uniqueSenders;
        public SortedDictionary<string, int> _uniqueSubjects;
        public SortedDictionary<string, int> _uniqueCombinations;
        public SortedDictionary<string, string> _domainEndings;
        #endregion

        #region Properties.
        /// <summary>
        /// Complete Task Start Time.
        /// </summary>
        public DateTime CompleteTaskStartTime
        {
            get
            {
                return completeTaskStartTime;
            }
        }

        /// <summary>
        /// User interrupt object.
        /// </summary>
        public Interrupt Interrupt
        {
            get
            {
                return interrupt;
            }
            set
            {
                interrupt = value;
            }
        }
        #endregion

        #region Custom Event Arguments.
        public class BeginMoveEventArgs : EventArgs
        {
            public readonly long maximum;
            public BeginMoveEventArgs(long maximum)
            {
                this.maximum = maximum;
            }
        }
        public class UpdateMoveEventArgs : EventArgs
        {
            public readonly long increment;
            public UpdateMoveEventArgs(long increment)
            {
                this.increment = increment;
            }
        }
        public class EndOfMoveEventArgs : EventArgs
        {
            public EndOfMoveEventArgs()
            {
            }
            public readonly long total;
            public EndOfMoveEventArgs(long total)
            {
                this.total = total;
            }
        }
        #endregion

        #region Delegates.
        public delegate void BeginCountHandler(BeginMoveEventArgs e);
        public delegate void UpdateCountHandler(UpdateMoveEventArgs e);
        public delegate void EndOfCountHandler(EndOfMoveEventArgs e);
        public delegate void BeginScanHandler(BeginMoveEventArgs e);
        public delegate void UpdateScanHandler(UpdateMoveEventArgs e);
        public delegate void EndOfScanHandler(EndOfMoveEventArgs e);
        public delegate void BeginMoveHandler(BeginMoveEventArgs e);
        public delegate void UpdateMoveHandler(UpdateMoveEventArgs e);
        public delegate void EndOfMoveHandler(EndOfMoveEventArgs e);
        #endregion

        #region Event Declarations.
        public event BeginCountHandler OnBeginCount;
        public event UpdateCountHandler OnUpdateCount;
        public event EndOfCountHandler OnEndOfCount;
        public event BeginScanHandler OnBeginScan;
        public event UpdateScanHandler OnUpdateScan;
        public event EndOfScanHandler OnEndOfScan;
        public event BeginMoveHandler OnBeginMove;
        public event UpdateMoveHandler OnUpdateMove;
        public event EndOfMoveHandler OnEndOfMove;
        #endregion

        #region Event raising helper methods.
        /// <summary>
        /// Trigger begin count event.
        /// </summary>
        private void SignalBeginCount(long maximum)
        {
            if (OnBeginCount != null)
            {
                OnBeginCount(new BeginMoveEventArgs(maximum));
            }
        }

        /// <summary>
        /// Trigger update count event.
        /// </summary>
        private void SignalUpdateCount(long increment)
        {
            if (OnUpdateCount != null)
            {
                OnUpdateCount(new UpdateMoveEventArgs(increment));
            }
        }

        /// <summary>
        /// Trigger end of count event.
        /// </summary>
        private void SignalEndOfCount(long total)
        {
            if (OnEndOfCount != null)
            {
                OnEndOfCount(new EndOfMoveEventArgs(total));
            }
        }

        /// <summary>
        /// Trigger begin scan event.
        /// </summary>
        private void SignalBeginScan(long maximum)
        {
            if (OnBeginScan != null)
            {
                OnBeginScan(new BeginMoveEventArgs(maximum));
            }
        }

        /// <summary>
        /// Trigger update scan event.
        /// </summary>
        private void SignalUpdateScan(long increment)
        {
            if (OnUpdateScan != null)
            {
                OnUpdateScan(new UpdateMoveEventArgs(increment));
            }
        }

        /// <summary>
        /// Trigger end of scan event.
        /// </summary>
        private void SignalEndOfScan(long total)
        {
            if (OnEndOfScan != null)
            {
                OnEndOfScan(new EndOfMoveEventArgs(total));
            }
        }

        /// <summary>
        /// Trigger begin move event.
        /// </summary>
        private void SignalBeginMove(long maximum)
        {
            if (OnBeginMove != null)
            {
                OnBeginMove(new BeginMoveEventArgs(maximum));
            }
        }

        /// <summary>
        /// Trigger update move event.
        /// </summary>
        private void SignalUpdateMove(long increment)
        {
            if (OnUpdateMove != null)
            {
                OnUpdateMove(new UpdateMoveEventArgs(increment));
            }
        }

        /// <summary>
        /// Trigger end of move event.
        /// </summary>
        private void SignalEndOfMove(long total)
        {
            if (OnEndOfMove != null)
            {
                OnEndOfMove(new EndOfMoveEventArgs(total));
            }
        }
        #endregion

        public LookoutMover()
        {
            _uniqueSenders = new SortedDictionary<string, int>();
            _uniqueSubjects = new SortedDictionary<string, int>();
            _uniqueCombinations = new SortedDictionary<string, int>();
            _domainEndings = new SortedDictionary<string, string>();
            _domainEndings.Add(".com", ".com");
            _domainEndings.Add(".co.uk", ".co.uk");
            _domainEndings.Add(".eu", ".eu");
            _domainEndings.Add(".ie", ".ie");
            _domainEndings.Add(".org.uk", ".org.uk");
            _domainEndings.Add(".sch.uk", ".sch.uk");
        }

        private void Track(string key, SortedDictionary<string, int> dictionary)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key]++;
            }
            else
            {
                dictionary.Add(key, 1);
            }
        }

        private void DumpAll()
        {
            Administrator.Tracer.WriteLine();
            Administrator.Tracer.WriteLine("Unique Senders");
            Administrator.Tracer.WriteLine("==============");
            Dump(_uniqueSenders);
            Administrator.Tracer.WriteLine();
            Administrator.Tracer.WriteLine("Unique Subjects");
            Administrator.Tracer.WriteLine("===============");
            Dump(_uniqueSubjects);
            //Administrator.Tracer.WriteLine();
            //Administrator.Tracer.WriteLine("Unique Combinations");
            //Administrator.Tracer.WriteLine("===================");
            //Dump(_uniqueCombinations);
            Administrator.Tracer.WriteLine();
            Administrator.Tracer.WriteLine("UserSetting Filter Criteria");
            Administrator.Tracer.WriteLine("===========================");
            ExtractFilterCriteria(_uniqueSenders);
        }

        private void Dump(SortedDictionary<string, int> dictionary)
        {
            // First sort unique entries by highest frequency first.
            SortedDictionary<int, string> results = new SortedDictionary<int, string>();
            foreach (var entry in dictionary)
            {
                int reverseKey = int.MaxValue - (entry.Value * 1000);
                while (results.ContainsKey(reverseKey))
                {
                    reverseKey++;
                }
                string key = String.Format(@"""{0}""", entry.Key).PadRight(64);
                string content = String.Format(@"{0},{1},{2}", key, entry.Value.ToString().PadLeft(12), reverseKey.ToString().PadLeft(12));
                if (!results.ContainsKey(reverseKey))
                {
                    results.Add(reverseKey, content);
                }
            }
            // Now write all unique entries to the log in highest frequency first order.
            foreach (var entry in results)
            {
                Administrator.Tracer.WriteLine(entry.Value);
            }
        }

        private void ExtractFilterCriteria(SortedDictionary<string, int> dictionary)
        {
            // First sort unique entries by highest frequency first.
            SortedDictionary<int, string> results = new SortedDictionary<int, string>();
            foreach (var entry in dictionary)
            {
                if (entry.Value > 4)
                {
                    StringBuilder filterCriteria = new StringBuilder();
                    int reverseKey = int.MaxValue - (entry.Value * 1000);
                    while (results.ContainsKey(reverseKey))
                    {
                        reverseKey++;
                    }
                    string indent = new String(' ', 2);
                    filterCriteria.AppendLine(String.Format(@"{0}{0}{1}", indent, entry.Value.ToString().PadRight(12)));
                    string category = "Trash"; // Can't determine this automatically so just default to this.
                    string sender = entry.Key;
                    string subFolder = MakeSubFolderFromSender(sender);
                    subFolder = CamelCaseFromSeparatedText(subFolder);
                    filterCriteria.AppendLine(String.Format(@"{0}{0}<UserSetting Key=""{1}_{2}"">", indent, category, subFolder));
                    filterCriteria.AppendLine(String.Format(@"{0}{0}{0}<Folder>kenneth.mcskimming@gmail.com\{1}\{2}</Folder>", indent, category, subFolder));
                    filterCriteria.AppendLine(String.Format(@"{0}{0}{0}<Sender><![CDATA[""{1}""]]></Sender>", indent, sender));
                    filterCriteria.AppendLine(String.Format(@"{0}{0}{0}<Subject><![CDATA[]]></Subject>", indent));
                    filterCriteria.AppendLine(String.Format(@"{0}{0}{0}<Body><![CDATA[]]></Body>", indent));
                    filterCriteria.AppendLine(String.Format(@"{0}{0}</UserSetting>", indent));
                    if (!results.ContainsKey(reverseKey))
                    {
                        results.Add(reverseKey, filterCriteria.ToString());
                    }
                }
            }
            // Now write all unique entries to the log in highest frequency first order.
            foreach (var entry in results)
            {
                Administrator.Tracer.WriteLine(entry.Value);
            }
        }

        private string MakeSubFolderFromSender(string sender)
        {
            string subFolder = sender;
            int pos = subFolder.IndexOf("@");
            if (pos >= 0)
            {
                if (!subFolder.EndsWith("@"))
                {
                    subFolder = subFolder.Substring(pos + 1);
                }
            }
            foreach (var entry in _domainEndings)
            {
                if (subFolder.Length > entry.Key.Length)
                {
                    if (subFolder.EndsWith(entry.Key))
                    {
                        int pos1 = subFolder.IndexOf(entry.Key);
                        int pos2 = subFolder.LastIndexOf(".", pos1 - 1);
                        if (pos2 >= 0)
                        {
                            subFolder = subFolder.Substring(pos2 + 1);
                        }
                        break;
                    }
                }
            }
            pos = subFolder.IndexOf(".");
            if (pos >= 0)
            {
                if (pos - 1 > 0)
                {
                    subFolder = subFolder.Substring(0, pos);
                }
            }
            return UpperFirst(subFolder);
        }

        /// <summary>
        /// Separated text to camel case.
        /// </summary>
        /// <param name="camelCase">Hyphen or underscore or dot separated text.</param>
        /// <returns>Camel case text</returns>
        public string CamelCaseFromSeparatedText(string separatedText)
        {
            StringBuilder camelCase = new StringBuilder();
            bool triggerUpperCase = false;
            string letter = string.Empty;
            for (int pos = 0; pos < separatedText.Length; pos++)
            {
                letter = separatedText.Substring(pos, 1);
                if (letter == "-" || letter== "_" || letter == ".")
                {
                    triggerUpperCase = true;
                }
                else
                {
                    if (triggerUpperCase)
                    {
                        triggerUpperCase = false;
                        letter = letter.ToUpper();
                    }
                    camelCase.Append(letter);
                }
            }
            return camelCase.ToString().Trim();
        }

        /// <summary>
        /// Change word to proper case.
        /// </summary>
        /// <param name="word">One word.</param>
        /// <returns>Word in proper case.</returns>
        public string ProperCase(string word)
        {
            string proper = string.Empty;
            word = word.Trim();
            if (word.Length > 0)
            {
                proper = word.Substring(0, 1).ToUpper();
                if (word.Length > 1)
                {
                    proper += word.Substring(1).ToLower();
                }
            }
            return proper;
        }

        /// <summary>
        /// Change first letter of word to upper case.
        /// </summary>
        /// <param name="word">One word.</param>
        /// <returns>Word with first letter in upper case.</returns>
        public string UpperFirst(string word)
        {
            string upper = string.Empty;
            word = word.Trim();
            if (word.Length > 0)
            {
                upper = word.Substring(0, 1).ToUpper();
                if (word.Length > 1)
                {
                    upper += word.Substring(1);
                }
            }
            return upper;
        }

        /// <summary>
        /// Change first letter of word to lower case.
        /// </summary>
        /// <param name="word">One word.</param>
        /// <returns>Word with first letter in lower case.</returns>
        public string LowerFirst(string word)
        {
            string lower = string.Empty;
            word = word.Trim();
            if (word.Length > 0)
            {
                lower = word.Substring(0, 1).ToLower();
                if (word.Length > 1)
                {
                    lower += word.Substring(1);
                }
            }
            return lower;
        }

        public bool Exists(string outlookFolder)
        {
            bool exists = false;
            try
            {
                string[] parts = outlookFolder.Split(Path.DirectorySeparatorChar);
                Microsoft.Office.Interop.Outlook.Application outlook;
                _NameSpace nameSpace;
                outlook = new Microsoft.Office.Interop.Outlook.Application();
                nameSpace = outlook.GetNamespace("MAPI");
                nameSpace.Logon(string.Empty, string.Empty, Missing.Value, Missing.Value);
                MAPIFolder targetFolder = nameSpace.Folders[parts[0]];
                for (int ptr = 1; ptr < parts.Length; ptr++)
                {
                    targetFolder = targetFolder.Folders[parts[ptr]];
                }
                string fullPath0 = targetFolder.FullFolderPath;
                string shortPath0 = targetFolder.FolderPath;
                string name0 = targetFolder.Name;
                exists = true;
            }
            catch (System.Exception ex)
            {
                exists = false;
            }
            return exists;
        }

        /// <summary>
        /// Empty trash mail from inbox.
        /// </summary>
        public void MoveMail()
        {
            int movedItemsCount = 0;
            Microsoft.Office.Interop.Outlook.Application outlook;
            _NameSpace nameSpace;
            MAPIFolder inbox;
            MAPIFolder outbox;
            outlook = new Microsoft.Office.Interop.Outlook.Application();
            nameSpace = outlook.GetNamespace("MAPI");
            nameSpace.Logon(string.Empty, string.Empty, Missing.Value, Missing.Value);
            inbox = nameSpace.GetDefaultFolder(Microsoft.Office.Interop.Outlook.OlDefaultFolders.olFolderInbox);
            outbox = nameSpace.GetDefaultFolder(Microsoft.Office.Interop.Outlook.OlDefaultFolders.olFolderOutbox);
            Items items = (Items)inbox.Items;
            Administrator.Tracer.WriteLine();
            Administrator.Tracer.WriteTimedMsg("I", String.Format(@"Total items = {0}", items.Count));
            Administrator.Tracer.WriteLine();
            Administrator.Tracer.WriteTimedMsg("I", "Begin Move");
            MailItem mailItem = null;
            SignalBeginMove(items.Count);
            foreach (object item in items)
            {
                if (Interrupt.Reason != "Cancel")
                {
                    try
                    {
                        SignalUpdateMove(1);
                        mailItem = item as MailItem;
                        if (mailItem != null)
                        {
                            string sender = (string)mailItem.SenderEmailAddress ?? string.Empty;
                            string subject = (string)mailItem.Subject ?? string.Empty;
                            string body = (string)mailItem.Body ?? string.Empty;
                            bool anyCriteriaApplied = false;
                            bool anyPasses = ScanAll(nameSpace, mailItem, sender, subject, body, ref movedItemsCount, out anyCriteriaApplied);
                            if (anyCriteriaApplied && anyPasses)
                            {
                                // No need to track as the mail item has already been filtered by an existing criteria.
                            }
                            else
                            {
                                Track(sender, _uniqueSenders);
                                Track(subject, _uniqueSubjects);
                                //string combination = String.Format(@"{0}#|#{1}", sender, subject);
                                //Track(combination, _uniqueCombinations);
                                //string message = "Mail Item => Sender = " + sender + " Subject = " + subject;
                                //Administrator.Tracer.WriteLine(message);
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Administrator.Tracer.WriteTimedMsg("E", ex.Message);
                    }
                }
                else
                {
                    break;
                }
            }
            if (Interrupt.Reason == "Cancel")
            {
                Administrator.Tracer.WriteLine();
                Administrator.Tracer.WriteTimedMsg("I", String.Format(@"Move cancelled"));
            }
            SignalEndOfMove(movedItemsCount);
            if (movedItemsCount > 0)
            {
                Administrator.Tracer.WriteLine();
                Administrator.Tracer.WriteTimedMsg("I", String.Format(@"{0} mail items moved", movedItemsCount));
            }
            else
            {
                Administrator.Tracer.WriteLine();
                Administrator.Tracer.WriteTimedMsg("I", String.Format(@"No mail items moved"));
            }
            Administrator.Tracer.WriteLine();
            Administrator.Tracer.WriteTimedMsg("I", "End Move");
            DumpAll();
        }

        /// <summary>
        /// This method scans a single mail item against all criteria to determine if the mail item needs to be moved.
        /// If so then it moves the mail item to the specified target outlook folder for the first set of criteria that it matches.
        /// </summary>
        private bool ScanAll(_NameSpace nameSpace, MailItem mailItem, string sender, string subject, string body, ref int movedItemsCount, out bool anyCriteriaApplied)
        {
            bool anyPasses = false;
            anyCriteriaApplied = false;
            SignalBeginScan(Administrator.ProfileManager.UserSettings.Entries.Count);
            foreach (var pair in Administrator.ProfileManager.UserSettings.Entries)
            {
                if (Interrupt.Reason != "Cancel")
                {
                    SignalUpdateScan(1);
                    bool criteriaDisplayed = false;
                    UserSetting userSetting = pair.Value;
                    string[] parts = userSetting.Folder.Split(Path.DirectorySeparatorChar);
                    MAPIFolder targetFolder = null;
                    try
                    {
                        targetFolder = nameSpace.Folders[parts[0]];
                        for (int ptr = 1; ptr < parts.Length; ptr++)
                        {
                            targetFolder = targetFolder.Folders[parts[ptr]];
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Administrator.Tracer.WriteTimedMsg("E", ex.Message);
                        targetFolder = null;
                    }
                    if (targetFolder != null)
                    {
                        string fullPath = targetFolder.FullFolderPath;
                        string shortPath = targetFolder.FolderPath;
                        string name = targetFolder.Name;
                        string senderCriteria = string.Empty;
                        Scanner senderScanner = null;
                        if (userSetting.SenderCriteria.Trim().Length > 0)
                        {
                            senderCriteria = Clean(userSetting.SenderCriteria);
                            senderScanner = new Scanner(senderCriteria);
                        }
                        string subjectCriteria = string.Empty;
                        Scanner subjectScanner = null;
                        if (userSetting.SubjectCriteria.Trim().Length > 0)
                        {
                            subjectCriteria = Clean(userSetting.SubjectCriteria);
                            subjectScanner = new Scanner(subjectCriteria);
                        }
                        string bodyCriteria = string.Empty;
                        Scanner bodyScanner = null;
                        if (userSetting.BodyCriteria.Trim().Length > 0)
                        {
                            bodyCriteria = Clean(userSetting.BodyCriteria);
                            bodyScanner = new Scanner(bodyCriteria);
                        }
                        bool criteriaApplied = false;
                        string prefix = string.Empty;
                        bool pass = Scan(sender, subject, body, senderScanner, subjectScanner, bodyScanner, out criteriaApplied, out prefix);
                        anyPasses |= pass;
                        anyCriteriaApplied |= criteriaApplied;
                        if (criteriaApplied && pass)
                        {
                            if (!criteriaDisplayed)
                            {
                                criteriaDisplayed = true;
                                Administrator.Tracer.WriteLine();
                                Administrator.Tracer.WriteMsg("I", String.Format(@"Folder : ""{0}""", userSetting.Folder));
                                if (senderCriteria.Trim().Length > 0)
                                {
                                    Administrator.Tracer.WriteTimedMsg("I", String.Format(@"Sender Criteria = {0}", senderCriteria));
                                }
                                else
                                {
                                    Administrator.Tracer.WriteTimedMsg("I", "Sender Criteria = None");
                                }
                                if (subjectCriteria.Trim().Length > 0)
                                {
                                    Administrator.Tracer.WriteTimedMsg("I", String.Format(@"Subject Criteria = {0}", subjectCriteria));
                                }
                                else
                                {
                                    Administrator.Tracer.WriteTimedMsg("I", "Subject Criteria = None");
                                }
                                if (bodyCriteria.Trim().Length > 0)
                                {
                                    Administrator.Tracer.WriteTimedMsg("I", String.Format(@"Body Criteria = {0}", bodyCriteria));
                                }
                                else
                                {
                                    Administrator.Tracer.WriteTimedMsg("I", "Body Criteria = None");
                                }
                                Administrator.Tracer.WriteLine();
                            }
                            Administrator.Tracer.WriteTimedMsg("I", String.Format(@"Matched [{0}] Moving -> Sender : ""{1}"", Subject : ""{2}""", prefix, sender, subject));
                            mailItem.Move(targetFolder);
                            movedItemsCount++;
                            //No need to check any further criteria as the mail item has already been matched to a preceding criteria and moved.
                            break;
                        }
                    }
                }
                else
                {
                    break;
                }
            }
            SignalEndOfScan(Administrator.ProfileManager.UserSettings.Entries.Count);
            return anyPasses;
        }

        /// <summary>
        /// This code AND's the sender, subject, and body scanners together.
        /// </summary>
        private bool Scan(string sender, string subject, string body, Scanner senderScanner, Scanner subjectScanner, Scanner bodyScanner, out bool criteriaApplied, out string prefix)
        {
            const string PREFIX_AND = " and ";
            prefix = string.Empty;
            criteriaApplied = false;
            bool pass = true;
            string delimiter = string.Empty;
            if (senderScanner != null)
            {
                criteriaApplied = true;
                pass &= senderScanner.Evaluate(sender);
                if (pass)
                {
                    prefix += delimiter + "Sender";
                    delimiter = PREFIX_AND;
                }
            }
            if (subjectScanner != null)
            {
                criteriaApplied = true;
                pass &= subjectScanner.Evaluate(subject);
                if (pass)
                {
                    prefix += delimiter + "Subject";
                    delimiter = PREFIX_AND;
                }
            }
            if (bodyScanner != null)
            {
                criteriaApplied = true;
                pass &= bodyScanner.Evaluate(body);
                if (pass)
                {
                    prefix += delimiter + "Body";
                    delimiter = PREFIX_AND;
                }
            }
            return pass;
        }

        private string Clean(string criteria)
        {
            criteria = criteria.Replace(Environment.NewLine, string.Empty);
            criteria = criteria.Trim();
            return criteria;
        }
    }
}
