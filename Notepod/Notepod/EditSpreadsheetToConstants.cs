using System.Collections.Generic;
using Notepod.Models;

namespace Notepod
{
    /// <summary>
    /// Edit Spreadsheet To Constants.
    /// </summary>
    /// <author>Kenneth McSKimming</author>
    public class EditSpreadsheetToConstants : BaseGenerator
    {
        public EditSpreadsheetToConstants(string[] inputLines)
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
            List<string> categories = new List<string>();
            List<string> actions = new List<string>();
            List<string> labels = new List<string>();
            Dictionary<string, string> eventNameConstants = new Dictionary<string, string>();
            Dictionary<string, string> categoryConstants = new Dictionary<string, string>();
            Dictionary<string, string> actionConstants = new Dictionary<string, string>();
            Dictionary<string, string> labelConstants = new Dictionary<string, string>();
            List<string> methods = new List<string>();
            List<string> output = new List<string>();

            string line = string.Empty;
            output.Add(line);
            line = @"//Label Name Enumerations.";
            output.Add(line);
            line = string.Empty;
            output.Add(line);
            List<string> enumUniqueKeys = new List<string>();
            List<string> enumCompleteUniqueKeys = new List<string>();
            Dictionary<string, string> labelNameEnumerations = new Dictionary<string, string>();
            string previousActionTriggerUniqueKey = string.Empty;
            string currentActionTriggerUniqueKey = string.Empty;
            string previousActionUniqueKey = string.Empty;
            string currentActionUniqueKey = string.Empty;
            for (int index = 0; index < _inputLines.Length; index++)
            {
                line = _inputLines[index];
                var values = CsvSplitter.Split(line);
                string eventName = values[0];
                string category = values[1];
                if (!categories.Contains(category))
                {
                    categories.Add(category);
                }
                string actionTrigger = values[2];
                //TODO: This way we use an enum for interaction type and we take the interaction off the end of all actions.
                string action = RemoveEventTriggerName(actionTrigger);
                if (!actions.Contains(action))
                {
                    actions.Add(action);
                }
                string label = values[3];
                if (!labels.Contains(label))
                {
                    labels.Add(label);
                }
                currentActionTriggerUniqueKey = category + actionTrigger;
                currentActionUniqueKey = category + action;
                if (currentActionTriggerUniqueKey != previousActionTriggerUniqueKey)
                {
                    if (previousActionTriggerUniqueKey.Length > 0)
                    {
                        if (!enumCompleteUniqueKeys.Contains(previousActionUniqueKey))
                        {
                            enumCompleteUniqueKeys.Add(previousActionUniqueKey);
                            line = @"}";
                            output.Add(line);
                        }
                    }
                    previousActionTriggerUniqueKey = currentActionTriggerUniqueKey;
                    previousActionUniqueKey = currentActionUniqueKey;
                    if (!enumUniqueKeys.Contains(currentActionUniqueKey))
                    {
                        enumUniqueKeys.Add(currentActionUniqueKey);
                        string enumName = RemoveEventTriggerName(action) + "Enum";
                        string parmName = RemoveEventTriggerName(action);
                        parmName = LowerFirst(parmName);
                        if (!labelNameEnumerations.ContainsKey(enumName))
                        {
                            labelNameEnumerations.Add(enumName, parmName);
                        }
                        line = string.Format(@"public enum {0}", enumName) + " {";
                        output.Add(line);
                        line = label + ",";
                        output.Add(line);
                    }
                }
                else
                {
                    if (!enumCompleteUniqueKeys.Contains(currentActionUniqueKey))
                    {
                        line = label + ",";
                        output.Add(line);
                    }
                }
            }
            if (!enumCompleteUniqueKeys.Contains(currentActionUniqueKey))
            {
                enumCompleteUniqueKeys.Add(currentActionUniqueKey);
                line = @"}";
                output.Add(line);
            }

            line = string.Empty;
            output.Add(line);
            line = @"//Event Name Constants.";
            output.Add(line);
            line = string.Empty;
            output.Add(line);
            for (int index = 0; index < _inputLines.Length; index++)
            {
                line = _inputLines[index];
                var values = CsvSplitter.Split(line);
                string eventName = values[0];
                string category = values[1];
                string action = values[2];
                //TODO: This way we use an enum for interaction type and we take the interaction off the end of all actions.
                action = RemoveEventTriggerName(action);
                string label = values[3];
                string eventNameLiteral = category + action;
                string eventNameConstant = CamelCaseToConstant(eventNameLiteral);
                if (!eventNameConstants.ContainsKey(eventNameLiteral))
                {
                    eventNameConstants.Add(eventNameLiteral, eventNameConstant);
                    line = string.Format(@"private final String EVENT_NAME_{0} = APP_EVENT_PREFIX + ""{1}"";", eventNameConstant, eventNameLiteral);
                    output.Add(line);
                }
            }
            line = string.Empty;
            output.Add(line);
            line = @"//Category Constants.";
            output.Add(line);
            line = string.Empty;
            output.Add(line);
            categories.Sort();
            foreach (string category in categories)
            {
                string eventCategoryLiteral = category;
                string eventCategoryConstant = CamelCaseToConstant(eventCategoryLiteral);
                categoryConstants.Add(eventCategoryLiteral, eventCategoryConstant);
                line = string.Format(@"private final String EVENT_CATEGORY_{0} = ""{1}"";", eventCategoryConstant, eventCategoryLiteral);
                output.Add(line);
            }
            line = string.Empty;
            output.Add(line);
            line = @"//Action Constants.";
            output.Add(line);
            line = string.Empty;
            output.Add(line);
            actions.Sort();
            foreach (string action in actions)
            {
                string eventActionLiteral = action;
                string eventActionConstant = CamelCaseToConstant(eventActionLiteral);
                actionConstants.Add(eventActionLiteral, eventActionConstant);
                line = string.Format(@"private final String EVENT_ACTION_{0} = ""{1}"";", eventActionConstant, eventActionLiteral);
                output.Add(line);
            }
            line = string.Empty;
            output.Add(line);
            line = @"//Label Constants.";
            output.Add(line);
            line = string.Empty;
            output.Add(line);
            labels.Sort();
            foreach (string label in labels)
            {
                string eventLabelLiteral = label;
                string eventLabelConstant = CamelCaseToConstant(eventLabelLiteral);
                labelConstants.Add(eventLabelLiteral, eventLabelConstant);
                line = string.Format(@"private final String EVENT_LABEL_{0} = ""{1}"";", eventLabelConstant, eventLabelLiteral);
                output.Add(line);
            }
            line = string.Empty;
            output.Add(line);
            line = @"//Tracking Methods.";
            output.Add(line);
            line = string.Empty;
            output.Add(line);
            for (int index = 0; index < _inputLines.Length; index++)
            {
                line = _inputLines[index];
                var values = CsvSplitter.Split(line);
                string eventName = values[0];
                string category = values[1];
                string actionTrigger = values[2];
                //TODO: This way we use an enum for interaction type and we take the interaction off the end of all actions.
                string action = RemoveEventTriggerName(actionTrigger);
                string label = values[3];
                string eventNameLiteral = category + action;
                string method = "track" + eventNameLiteral;
                string eventNameConstant = CamelCaseToConstant(eventNameLiteral);
                string enumName = RemoveEventTriggerName(action) + "Enum";
                string parmName = RemoveEventTriggerName(action);
                parmName = LowerFirst(parmName);
                if (!methods.Contains(method))
                {
                    methods.Add(method);
                    line = string.Format(@"public void track{0}(AnalyticsController.{1} {2}, AnalyticsController.EventTriggerEnum eventTrigger) ", eventNameLiteral, enumName, parmName) + "{";
                    output.Add(line);
                    line = string.Format(@"String label = {0}.name();", parmName);
                    output.Add(line);
                    //This line uses label constants.
                    //line = string.Format(@"trackUserEvent(EVENT_NAME_{0}, EVENT_CATEGORY_{1}, EVENT_ACTION_{2}, EVENT_LABEL_{3});", eventNameConstants[eventNameLiteral], categoryConstants[category], actionConstants[action], labelConstants[label]);
                    //This line uses label enumerations.
                    line = string.Format(@"trackUserEvent(EVENT_NAME_{0}, EVENT_CATEGORY_{1}, EVENT_ACTION_{2}, label, eventTrigger);", eventNameConstants[eventNameLiteral], categoryConstants[category], actionConstants[action]);
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