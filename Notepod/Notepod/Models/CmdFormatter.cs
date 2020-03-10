using System;
using System.Text;

namespace Notepod.Models
{
    /// <summary>
    /// Cmd Formatter Class.
    /// </summary>
    /// <remarks>
    /// Reformats commands so that for any embedded XML each new tag starts on a new line and is indented
    /// to indicate its level in the hierarchy, but any commands outside of any XML tags are unaffected
    /// and are left with no indentation.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public class CmdFormatter
    {
        private int _indentSpacesPerLevel = 0;
        private int _indentLevel = 0;
        private StringBuilder _cmds;
        private enum TagTypeEnum
        {
            Open,
            Close,
            SelfClosing,
            Undefined
        }
        private TagTypeEnum _thisTagType = TagTypeEnum.Undefined;
        private TagTypeEnum _prevTagType = TagTypeEnum.Undefined;
        private bool _someXmlFound = false;

        public CmdFormatter(int indentSpacesPerLevel)
        {
            _indentSpacesPerLevel = indentSpacesPerLevel;
            _someXmlFound = false;
        }

        public string Format(string cmdText)
        {
            int beginPos = 0;
            int startPos = 0;
            int endPos = 0;
            string rootTag = String.Format(@"<{0}>", "Root");
            _indentLevel = -1;
            _cmds = new StringBuilder();
            ParseElement(beginPos, startPos, ref endPos, rootTag, ref cmdText);
            string formattedCmds = _cmds.ToString();
            if (formattedCmds.EndsWith(Environment.NewLine))
            {
                formattedCmds = formattedCmds.Substring(0, formattedCmds.Length - Environment.NewLine.Length);
            }
            return formattedCmds;
        }

        public void ParseElement(int beginPos, int startPos, ref int endPos, string opener, ref string xmlText)
        {
            int initialPos = startPos;
            _indentLevel++;
            beginPos = startPos;
            string actualTagContent = string.Empty;
            string interTagContent = string.Empty;
            string tagName = string.Empty;
            string tag = null;
            while (startPos < xmlText.Length)
            {
                actualTagContent = string.Empty;
                interTagContent = string.Empty;
                tag = ReadTag(xmlText, ref actualTagContent, ref interTagContent, ref startPos, ref endPos);
                if (tag != null)
                {
                    if (tag.EndsWith("/>") || tag.EndsWith("/>}"))
                    {
                        NoteTagType(TagTypeEnum.SelfClosing);
                        if (_indentLevel == 0)
                        {
                            AppendCommandsOutsideXml(ref interTagContent);
                        }
                        RemoveIntertagContentWhitespace(ref tag, ref interTagContent);
                        if (interTagContent.Length > 0)
                        {
                            AppendCommandsOutsideXml(ref interTagContent);
                        }
                        AppendClosingXml(tag, interTagContent, true);
                    }
                    else if (tag.EndsWith("?>") || tag.EndsWith("?>}"))
                    {
                        NoteTagType(TagTypeEnum.SelfClosing);
                        if (_indentLevel == 0)
                        {
                            AppendCommandsOutsideXml(ref interTagContent);
                        }
                        AppendClosingXml(tag, interTagContent, true);
                    }
                    else if (tag.StartsWith("</"))
                    {
                        RemoveIntertagContentWhitespace(ref tag, ref interTagContent);
                        NoteTagType(TagTypeEnum.Close);
                        int consumedLen = endPos - initialPos + 1 - tag.Length;
                        string consumedText = xmlText.Substring(initialPos, consumedLen);
                        if (consumedText.Contains("<") && consumedText.Contains(">"))
                        {
                            AppendClosingXml(tag, interTagContent, true);
                        }
                        else
                        {
                            AppendClosingXml(tag, interTagContent, false);
                        }
                        break;
                    }
                    else if (tag.StartsWith("<?"))
                    {
                        NoteTagType(TagTypeEnum.SelfClosing);
                        AppendClosingXml(tag, interTagContent, true);
                    }
                    else if (tag.Trim().Length == 0)
                    {
                        NoteTagType(TagTypeEnum.Undefined);
                        if (_someXmlFound)
                        {
                            endPos++;
                        }
                        break;
                    }
                    else
                    {
                        if (_indentLevel == 0)
                        {
                            if (interTagContent.EndsWith("?") || interTagContent.EndsWith("¤"))
                            {
                                //This will force a new line if there is not already a new line between the "?" or "¤" and the first opening "<" of the first XML tag.
                                _thisTagType = TagTypeEnum.Open;
                            }
                            AppendCommandsOutsideXml(ref interTagContent);
                        }
                        else
                        {
                            RemoveIntertagContentWhitespace(ref tag, ref interTagContent);
                            if (interTagContent.Length > 0)
                            {
                                AppendCommandsOutsideXml(ref interTagContent);
                            }
                        }
                        NoteTagType(TagTypeEnum.Open);
                        AppendOpeningXml(tag);
                        startPos = endPos + 1;
                        ParseElement(beginPos, startPos, ref endPos, tag, ref xmlText);
                    }
                }
                startPos = endPos + 1;
            }
            if (_indentLevel == 0)
            {
                if (endPos <= xmlText.Length)
                {
                    string endText = xmlText.Substring(endPos);
                    if (endText != ">")
                    {
                        AppendCommandsOutsideXml(ref endText);
                    }
                }
            }
            _indentLevel--;
        }

        /// <remarks>
        /// Remove any inter tag content which consists solely of whitespace characters.
        /// </remarks>
        private void RemoveIntertagContentWhitespace(ref string tag, ref string interTagContent)
        {
            string whiteSpace = interTagContent;
            whiteSpace = whiteSpace.Replace(Environment.NewLine, string.Empty);
            whiteSpace = whiteSpace.Replace("\t", string.Empty);
            whiteSpace = whiteSpace.Trim();
            if (whiteSpace.Length == 0)
            {
                interTagContent = whiteSpace;
            }
            else if (whiteSpace.StartsWith("{") && whiteSpace.EndsWith(":"))
            {
                tag = whiteSpace + tag;
                interTagContent = string.Empty;
            }
            else if (whiteSpace.StartsWith("}") && whiteSpace.EndsWith(":"))
            {
                whiteSpace = whiteSpace.Replace("}", string.Empty);
                whiteSpace = whiteSpace.Trim();
                tag = whiteSpace + tag;
                interTagContent = string.Empty;
            }
            else if (whiteSpace == "}")
            {
                interTagContent = string.Empty;
            }
        }

        private void NoteTagType(TagTypeEnum tagType)
        {
            _prevTagType = _thisTagType;
            _thisTagType = tagType;
        }

        private void AppendCommandsOutsideXml(ref string commandsOutsideXml)
        {
            if (commandsOutsideXml == String.Format(@"{0}", Environment.NewLine))
            {
                commandsOutsideXml = string.Empty;
            }
            else if (commandsOutsideXml.StartsWith(Environment.NewLine))
            {
                commandsOutsideXml = commandsOutsideXml.Substring(Environment.NewLine.Length);
            }
            _cmds.Append(commandsOutsideXml);
            commandsOutsideXml = string.Empty;
        }

        private void AppendOpeningXml(string tag)
        {
            string newLine = string.Empty;
            if (_thisTagType == _prevTagType)
            {
                newLine = Environment.NewLine;
            }
            if (_indentLevel >= 0)
            {
                if (tag.Length > 0)
                {
                    _cmds.Append(newLine + Indent(_indentLevel) + tag);
                }
            }
        }

        private void AppendClosingXml(string tag, string interTagContent, bool newLines)
        {
            //if (_thisTagType == TagTypeEnum.Close || _thisTagType == TagTypeEnum.SelfClosing)
            //{
            //    if (_prevTagType == TagTypeEnum.Close || _prevTagType == TagTypeEnum.SelfClosing)
            //    {
            //        interTagContent = interTagContent.Replace(Environment.NewLine, string.Empty);
            //        interTagContent = interTagContent.Replace("\t", string.Empty);
            //        if (interTagContent.Trim().Length == 0)
            //        {
            //            interTagContent = string.Empty;
            //        }
            //    }
            //}
            //Remove any inter tag content which consists solely of whitespace characters.
            string whiteSpace = interTagContent;
            whiteSpace = whiteSpace.Replace(Environment.NewLine, string.Empty);
            whiteSpace = whiteSpace.Replace("\t", string.Empty);
            whiteSpace = whiteSpace.Trim();
            if (whiteSpace.Length == 0)
            {
                interTagContent = whiteSpace;
            }
            string newLine = string.Empty;
            string leadingIndent = string.Empty;
            string trailingIndent = string.Empty;
            if (newLines)
            {
                newLine = Environment.NewLine;
                leadingIndent = Indent(_indentLevel);
                if (_thisTagType == TagTypeEnum.SelfClosing)
                {
                    trailingIndent = Indent(_indentLevel);
                }
                else
                {
                    trailingIndent = Indent(_indentLevel - 1);
                }
            }
            if (_indentLevel >= 0)
            {
                if (interTagContent.Length > 0)
                {
                    _cmds.Append(newLine + leadingIndent + interTagContent + newLine);
                }
                else
                {
                    _cmds.Append(newLine);
                }
            }
            if (_indentLevel > 0)
            {
                if (tag.Length > 0)
                {
                    _cmds.Append(trailingIndent + tag + Environment.NewLine);
                }
            }
        }

        private string Indent(int indentLevel)
        {
            int indentSpaces = indentLevel * _indentSpacesPerLevel;
            string indent = string.Empty;
            if (indentSpaces > 0)
            {
                indent = new string(' ', indentSpaces);
            }
            return indent;
        }

        public string ReadTag(string xmlText, ref string actualTagContent, ref string interTagContent, ref int startPos, ref int endPos)
        {
            StringBuilder actualTag = new StringBuilder();
            StringBuilder interTag = new StringBuilder();
            int len = xmlText.Length;
            string tag = null;
            bool insideTag = false;
            bool tagOpenFound = false;
            bool tagCloseFound = false;
            for (int pos = startPos; pos < len; pos++)
            {
                string letter = xmlText.Substring(pos, 1);
                if (letter == "<")
                {
                    tagOpenFound = true;
                    insideTag = true;
                    startPos = pos;
                }
                else if (letter == ">")
                {
                    tagCloseFound = true;
                    insideTag = false;
                    endPos = pos;
                    break;
                }
                else
                {
                    if (insideTag)
                    {
                        actualTag.Append(letter);
                    }
                    else
                    {
                        interTag.Append(letter);
                    }
                }
            }
            if (tagOpenFound && tagCloseFound)
            {
                _someXmlFound = true;
                if (startPos > -1 && endPos > -1)
                {
                    tag = xmlText.Substring(startPos, endPos - startPos + 1);
                    if (endPos + 1 < xmlText.Length)
                    {
                        string testLetter = xmlText.Substring(endPos + 1, 1);
                        if (testLetter == "}")
                        {
                            tag += testLetter;
                        }
                    }
                }
                else
                {
                    tag = string.Empty;
                }
            }
            else
            {
                tag = string.Empty;
            }
            actualTagContent = actualTag.ToString();
            interTagContent = interTag.ToString();
            return tag;
        }
    }
}