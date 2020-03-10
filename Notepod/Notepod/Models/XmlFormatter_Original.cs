using System;
using System.Text;
using System.Collections;

namespace Notepod.Models
{
    /// <summary>
    /// XML Formatter Class (based on CmdFormatter but exclusively for XML)
    /// </summary>
    /// <remarks>
    /// Reformats XML each new tag starts on a new line and is indented
    /// to indicate its level in the hierarchy
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public class XmlFormatter_Original
    {
        private Stack TagStack { get; set; }

        private int _indentSpacesPerLevel = 0;
        private int _indentLevel = 0;
        private StringBuilder _cmds;
        private enum TagTypeEnum
        {
            Open,
            Close,
            SelfClosing,
            Comment,
            Undefined
        }
        private TagTypeEnum _thisTagType = TagTypeEnum.Undefined;
        private TagTypeEnum _prevTagType = TagTypeEnum.Undefined;
        private bool _someXmlFound = false;

        public XmlFormatter_Original(int indentSpacesPerLevel)
        {
            _indentSpacesPerLevel = indentSpacesPerLevel;
            _someXmlFound = false;
        }

        public string Format(string cmdText)
        {
            TagStack = new Stack();
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
                    if (tag.StartsWith("<!--") && tag.EndsWith("-->"))
                    {
                        NoteTagType(TagTypeEnum.Comment);
                        string comment = tag;
                        AppendCommentXml(ref comment);
                    }
                    else if (tag.EndsWith("/>") || tag.EndsWith("/>}"))
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
                        bool includeNewLines = false;
                        if (consumedText.Contains("<") && consumedText.Contains(">"))
                        {
                            includeNewLines = true;
                        }
                        string poppedTagName = (string)TagStack.Pop();
                        string closingTagName = ParseClosingTagName(actualTagContent);
                        if (poppedTagName != closingTagName && TagStack.Count > 0)
                        {
                            string missingTag = String.Format(@"</{0}>", poppedTagName);
                            AppendClosingXml(missingTag, interTagContent, includeNewLines);
                            interTagContent = string.Empty;
                            _indentLevel--;
                            if (TagStack.Count > 0)
                            {
                                poppedTagName = (string)TagStack.Pop();
                            }
                        }
                        //while (poppedTagName != closingTagName && TagStack.Count > 0)
                        //{
                        //    string missingTag = String.Format(@"</{0}>", poppedTagName);
                        //    AppendClosingXml(missingTag, interTagContent, includeNewLines);
                        //    interTagContent = string.Empty;
                        //    _indentLevel--;
                        //    if (TagStack.Count > 0)
                        //    {
                        //        poppedTagName = (string)TagStack.Pop();
                        //    }
                        //};
                        AppendClosingXml(tag, interTagContent, includeNewLines);
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

                        tagName = ParseOpeningTagName(actualTagContent);
                        TagStack.Push(tagName);
                        if (TagStack.Count > 512)
                        {
                            endPos = xmlText.Length - 1;
                            break;
                        }

                        AppendOpeningXml(tag);
                        startPos = endPos + 1;
                        ParseElement(beginPos, startPos, ref endPos, tag, ref xmlText);
                    }
                }
                startPos = endPos + 1;
                if (TagStack.Count == 0)
                {
                    break;
                }
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

        private void AppendCommentXml(ref string comment)
        {
            if (comment.StartsWith(Environment.NewLine))
            {
                comment = comment.Substring(Environment.NewLine.Length);
            }
            string newLine = Environment.NewLine;
            if (_indentLevel >= 0)
            {
                if (comment.Length > 0)
                {
                    _cmds.Append(newLine + Indent(_indentLevel) + comment + newLine);
                }
            }
            comment = string.Empty;
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
                    //TODO: Work out why this was needed for the command formatter but is causing problems with the XML only formatter.
                    //_cmds.Append(newLine);
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

        private string ParseOpeningTagName(string actualTagContent)
        {
            string tagName = string.Empty;
            int pos = actualTagContent.IndexOf(' ');
            if (pos > -1)
            {
                tagName = actualTagContent.Substring(0, pos);
            }
            else
            {
                tagName = actualTagContent;
            }
            return tagName;
        }

        private string ParseClosingTagName(string actualTagContent)
        {
            string tagName = actualTagContent.Replace("/", string.Empty);
            return tagName;
        }
    }
}