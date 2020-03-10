using System.Collections.Generic;
using Notepod.Models;

namespace Notepod
{
    /// <summary>
    /// Edit Spreadsheet Gt To Ac Methods.
    /// </summary>
    /// <author>Kenneth McSKimming</author>
    public class EditSpreadsheetGtToAcMethods : BaseGenerator
    {
        public EditSpreadsheetGtToAcMethods(string[] inputLines)
            : base(inputLines)
        {
        }

        public override void Import()
        {
            ImportLines();
        }

        public override string[] Generate()
        {
            return GenerateLines();
        }

        private void ImportLines()
        {
        }

        private string[] GenerateLines()
        {
            List<string> output = new List<string>();
            string line = string.Empty;
            output.Add(line);
            line = @"//Tracking Methods.";
            output.Add(line);
            line = string.Empty;
            output.Add(line);
            for (int index = 0; index < _inputLines.Length; index++)
            {
                line = _inputLines[index];
                line = line.Trim();
                if (line.Contains("public void"))
                {
                    output.Add(string.Empty);
                    output.Add(line);
                    line = line.Replace("public void", string.Empty);
                    line = line.Trim();
                    int pos1 = line.IndexOf("(");
                    string method = line.Substring(0, pos1);
                    int pos2 = line.IndexOf(")");
                    string parms = line.Substring(pos1, pos2 - pos1);
                    parms = parms.Replace("(", string.Empty);
                    parms = parms.Replace(")", string.Empty);
                    parms = parms.Trim();
                    List<string> parmNames = new List<string>();
                    if (parms.Length > 0)
                    {
                        if (parms.Contains(","))
                        {
                            string[] args = parms.Split(',');
                            foreach (string arg in args)
                            {
                                string ar = arg.Trim();
                                string[] parts = ar.Split(' ');
                                parmNames.Add(parts[1]);
                            }
                        }
                        else
                        {
                            string[] parts = parms.Split(' ');
                            parmNames.Add(parts[1]);
                        }
                    }
                    line = @"if (getIsTrackingEnabled()) {";
                    output.Add(line);
                    string parameters = string.Empty;
                    string delimiter = string.Empty;
                    foreach (string parmName in parmNames)
                    {
                        parameters += delimiter + parmName;
                        delimiter = ", ";
                    }
                    line = string.Format(@"GoogleTagManagerController.getInstance().{0}({1});", method, parameters);
                    output.Add(line);
                    line = @"}";
                    output.Add(line);
                    line = @"}";
                    output.Add(line);
                }
            }
            return output.ToArray();
        }

        private string RemoveEventTriggerName(string action)
        {
            action = RemoveEventTriggerName(action, "Default");
            action = RemoveEventTriggerName(action, "Interaction");
            action = RemoveEventTriggerName(action, "Submit");
            return action;
        }

        private string RemoveEventTriggerName(string action, string eventTriggerName)
        {
            if (action.EndsWith(eventTriggerName))
            {
                int pos = action.IndexOf(eventTriggerName);
                if (pos > -1)
                {
                    action = action.Substring(0, pos);
                }
            }
            return action;
        }

        private string LowerFirst(string text)
        {
            text = text.Trim();
            if (text.Length > 0)
            {
                if (text.Length == 1)
                {
                    text = text.ToLower();
                }
                else
                {
                    string first = text.Substring(0, 1);
                    first = first.ToLower();
                    string rest = text.Substring(1);
                    text = first + rest;
                }
            }
            return text;
        }

        private string UpperFirst(string text)
        {
            text = text.Trim();
            if (text.Length > 0)
            {
                if (text.Length == 1)
                {
                    text = text.ToUpper();
                }
                else
                {
                    string first = text.Substring(0, 1);
                    first = first.ToUpper();
                    string rest = text.Substring(1);
                    text = first + rest;
                }
            }
            return text;
        }

        private string CamelCaseToConstant(string camelCase)
        {
            string constant = string.Empty;
            List<string> words = new List<string>();
            string word = string.Empty;
            for (int index = 0; index < camelCase.Length; index++)
            {
                string letter = camelCase.Substring(index, 1);
                if (letter == letter.ToUpper())
                {
                    if (word.Length > 0)
                    {
                        words.Add(word);
                    }
                    word = string.Empty;
                    word += letter.ToUpper();
                }
                else
                {
                    word += letter.ToUpper();
                }
            }
            if (word.Length > 0)
            {
                words.Add(word);
            }
            constant = string.Empty;
            string delimiter = "";
            foreach (string wrd in words)
            {
                constant += delimiter + wrd;
                delimiter = "_";
            }
            if (constant.Contains("E_U_R"))
            {
                constant = constant.Replace("E_U_R", "EUR");
            }
            if (constant.Contains("G_B_P"))
            {
                constant = constant.Replace("G_B_P", "GBP");
            }
            return constant;
        }
    }
}