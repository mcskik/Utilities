using System;
using System.Collections.Generic;
using System.Text;
using Notepod.Models;

namespace Notepod
{
    /// <summary>
    /// Edit Commands Format.
    /// </summary>
    /// <author>Kenneth McSKimming</author>
    public class EditCommandsFormat : BaseGenerator
    {
        public EditCommandsFormat(string[] inputLines)
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
            StringBuilder cmds = new StringBuilder();
            foreach (string inputLine in _inputLines)
            {
                cmds.AppendLine(inputLine);
            }
            CmdFormatter parser = new CmdFormatter(4);
            string formattedCmds = parser.Format(cmds.ToString());
            string[] lines = formattedCmds.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            List<string> output = new List<string>();
            foreach (string outputLine in lines)
            {
                output.Add(outputLine);
            }
            return output.ToArray();
        }
    }
}