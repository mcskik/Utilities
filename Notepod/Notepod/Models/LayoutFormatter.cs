using System;
using System.Text;
using System.Collections.Generic;

namespace Notepod.Models
{
    /// <summary>
    /// Layout Formatter Class (based on XmlFormatter but exclusively for Android Layouts)
    /// </summary>
    /// <remarks>
    /// Reformats Layouts
    /// Each new tag starts on a new line and is indented to indicate its level in the hierarchy
    /// Each new attribute starts on a new line and is indented to indicate its level in the hierarchy
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public class LayoutFormatter
    {
        private int _indentSpacesPerLevel = 0;
        private int _indentLevel = 0;
        private StringBuilder _xml;
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
        private int _tagCount = 0;

        public LayoutFormatter(int indentSpacesPerLevel)
        {
            _indentSpacesPerLevel = indentSpacesPerLevel;
            _someXmlFound = false;
        }

        public string Format(string xmlText)
        {
            int beginPos = 0;
            int startPos = 0;
            int endPos = 0;
            string rootTag = String.Format(@"<{0}>", "Root");
            _indentLevel = -1;
            _xml = new StringBuilder();
            ParseElement(beginPos, startPos, ref endPos, rootTag, ref xmlText);
            string formattedXml = _xml.ToString();
            if (formattedXml.EndsWith(Environment.NewLine))
            {
                formattedXml = formattedXml.Substring(0, formattedXml.Length - Environment.NewLine.Length);
            }
            return formattedXml;
        }

        public void ParseElement(int beginPos, int startPos, ref int endPos, string opener, ref string xmlText)
        {
            int initialPos = startPos;
            if (opener.StartsWith("<!--") && opener.EndsWith("-->"))
            {
            }
            else
            {
                _indentLevel++;
            }
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
                        RemoveIntertagContentWhitespace(ref tag, ref interTagContent);
                        RemoveTagContentWhitespace(ref tag);
                        AppendClosingXml(tag, interTagContent, true);
                    }
                    else if (tag.EndsWith("?>") || tag.EndsWith("?>}"))
                    {
                        NoteTagType(TagTypeEnum.SelfClosing);
                        AppendClosingXml(tag, interTagContent, true);
                    }
                    else if (tag.StartsWith("</"))
                    {
                        RemoveIntertagContentWhitespace(ref tag, ref interTagContent);
                        RemoveTagContentWhitespace(ref tag);
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
                        RemoveIntertagContentWhitespace(ref tag, ref interTagContent);
                        RemoveTagContentWhitespace(ref tag);
                        NoteTagType(TagTypeEnum.Open);
                        AppendOpeningXml(tag);
                        startPos = endPos + 1;
                        ParseElement(beginPos, startPos, ref endPos, tag, ref xmlText);
                    }
                }
                startPos = endPos + 1;
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
                interTagContent = Environment.NewLine + Indent(_indentLevel - 1);
            }
        }

        /// <remarks>
        /// Remove any whitespace characters from within the tag itself.
        /// </remarks>
        private void RemoveTagContentWhitespace(ref string tag)
        {
            _tagCount++;
            if (tag.Contains("<?"))
            {
                _tagCount = 0;
            }
            tag = tag.Trim();
            if (tag.StartsWith("<!--") && tag.EndsWith("-->"))
            {
            }
            else
            {
                var attributes = XmlAttributesSplitter.Split(tag);
                tag = XmlAttributesSplitter.Join(attributes, _tagCount == 1);
                tag = tag.Replace("\t", string.Empty);
            }
            //Put back standard indentation at the start of each new line.
            tag = tag.Replace(Environment.NewLine, Environment.NewLine + Indent(_indentLevel + 1));
            tag = tag.Trim();
        }

        private void NoteTagType(TagTypeEnum tagType)
        {
            _prevTagType = _thisTagType;
            _thisTagType = tagType;
        }

        private void AppendOpeningXml(string tag)
        {
            tag = tag.Trim();
            string newLine = string.Empty;
            if (_thisTagType == _prevTagType)
            {
                newLine = Environment.NewLine;
            }
            if (_indentLevel >= 0)
            {
                if (tag.Length > 0)
                {
                    _xml.Append(newLine + Indent(_indentLevel) + tag);
                }
            }
        }

        private void AppendClosingXml(string tag, string interTagContent, bool newLines)
        {
            tag = tag.Trim();
            if (_thisTagType == TagTypeEnum.SelfClosing)
            {
                _xml.Append(Environment.NewLine + Indent(_indentLevel) + tag + Environment.NewLine);
            }
            else
            {
                _xml.Append(Environment.NewLine + Indent(_indentLevel - 1) + tag + Environment.NewLine);
            }
        }

        private string Indent(int indentLevel)
        {
            string indent = string.Empty;
            if (indentLevel > 0)
            {
                indent = new string('\t', indentLevel);
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
