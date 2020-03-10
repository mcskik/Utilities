using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Differenti8.DataLayer.Variables;

namespace Differenti8.DataLayer.Profile
{
    /// <summary>
    ///  Profile cache of variables which can be stored, fetched, and used for substitution of imbedded symbolic variables.
    /// </summary>
    /// <author>Ken McSkimming</author>
    public class ProfileCache
    {
        #region Constants.
        private const string MARK_OPEN = "[";
        private const string MARK_CLOSE = "]";
        private const string DOT = ".";
        private const string BOTH = "BOTH";
        #endregion

        #region Member Variables.
        private Stack _variableStack;
        private int _stackLevel;
        #endregion

        #region Constructors.
        public ProfileCache()
        {
            _stackLevel = 0;
            _variableStack = new Stack();
            _variableStack.Add(_stackLevel, new Cache());
            _variableStack[_stackLevel].Declare("_CurrentDate", DateTime.Now, "DateStamp", string.Empty, "dd/MM/yyyy");
            _variableStack[_stackLevel].Declare("_CurrentDateTime", DateTime.Now, "DateStamp", string.Empty, "dd/MM/yyyy HH:mm:ss");
        }
        #endregion

        #region Public Methods.
        /// <summary>
        ///  Substitution of imbedded symbolic variables with matching variable stack fields.
        /// </summary>
        /// <remarks>
        ///  Eg. [@SubLoc@]\[@System@]\[@SubSys@] = "X\Abc\Def"
        ///  Eg. @Prefix  = "Alpha"
        ///      @Suffix  = "01"
        ///      @Alpha01 = "The Answer"
        ///      [@[@Prefix@][@Suffix@]@] = [@Alpha01@] = "The Answer"
        /// </remarks>
        public string Substitute(string text)
        {
            string outcome = string.Empty;
            return Expand(text, 0, ref outcome);
        }

        public string Fetch(string name)
        {
            string value;
            try
            {
                int stackIndex = _stackLevel;
                do
                {
                    value = _variableStack[stackIndex].Fetch(name).Display();
                    stackIndex--;
                } while (value == "[#" + name + "#]" && stackIndex >= 0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                value = ErrorInline(name);
            }
            return value;
        }

        public void Store(string name, string value)
        {
            _variableStack[_stackLevel].Store(name, value);
        }

        public int AddStackLevel()
        {
            _stackLevel++;
            if (_variableStack.ContainsKey(_stackLevel))
            {
                _variableStack.Remove(_stackLevel);
            }
            _variableStack.Add(_stackLevel, new Cache());
            return _stackLevel;
        }

        public int RemoveStackLevel()
        {
            if (_variableStack.ContainsKey(_stackLevel))
            {
                _variableStack.Remove(_stackLevel);
            }
            _stackLevel--;
            return _stackLevel;
        }
        #endregion

        #region Private Methods.
        private string Expand(string template, int position, ref string code)
        {
            int pointer;
            int length;
            length = template.Length;
            if (length > 0)
            {
                pointer = 0;
                do
                {
                    template = Router(template, pointer, ref code);
                    length = template.Length;
                    if (code != "OK")
                    {
                        pointer++;
                        pointer = Accelerator(template, pointer);
                    }
                } while (pointer < length - 1);
            }
            return template;
        }

        private string Router(string template, int position, ref string outcome)
        {
            string leader = string.Empty;
            string middle = string.Empty;
            string trailer = string.Empty;
            string code = string.Empty;
            outcome = "WIP";
            if (position + 2 <= template.Length)
            {
                code = template.Substring(position, 2);
                switch (code)
                {
                    case "[@":
                        leader = template.Substring(0, position);
                        middle = template.Substring(position);
                        trailer = Substitute(middle, position, ref outcome);
                        template = leader + trailer;
                        break;
                    case "@]":
                        break;
                    default:
                        break;
                }
            }
            return template;
        }

        private int Accelerator(string template, int currentPos)
        {
            int jumpToPos = currentPos;
            int openPos = template.IndexOf(MARK_OPEN, currentPos);
            int closePos = template.IndexOf(MARK_CLOSE, currentPos);
            if (openPos > -1 && closePos > -1)
            {
                if (openPos <= closePos)
                {
                    jumpToPos = openPos;
                }
                else
                {
                    if (closePos > 0)
                    {
                        string letter = template.Substring(closePos - 1, 1);
                        switch (letter)
                        {
                            case "@":
                                closePos--;
                                jumpToPos = closePos;
                                break;
                            default:
                                closePos = -1;
                                break;
                        }
                    }
                    else
                    {
                        closePos = -1;
                    }
                }
            }
            else if (openPos == -1 && closePos == -1)
            {
                jumpToPos = template.Length - 1;
            }
            return jumpToPos;
        }

        private string Substitute(string template, int position, ref string outcome)
        {
            int usedLen;
            string contents = GetFragment(template, "[@", "@]", out usedLen);
            string trailer = Extract(template, usedLen);
            contents = Expand(contents, position, ref outcome);
            try
            {
                contents = Fetch(contents);
                contents = Expand(contents, position, ref outcome);
                outcome = "OK";
            }
            catch
            {
                contents = ErrorInline(contents);
                outcome = "ERR";
            }
            return contents + trailer;
        }
        #endregion

        #region Parsing Methods.
        private string GetFragment(string template, string startMarker, string endMarker, out int usedLen)
        {
            //Don't count the first start marker as one to ignore as this is the one we are dealing with.
            int endMArkerIgnoreCount = -1; //Deliberately set to -1 for the above reason.
            string startCode = string.Empty;
            string endCode = string.Empty;
            int startPos = -1;
            int endPos = -1;
            int pointer = 0;
            usedLen = 0;
            int startUsedLen = 0;
            int endUsedLen = 0;
            do
            {
                startCode = Extract(template, pointer, startMarker.Length);
                endCode = Extract(template, pointer, endMarker.Length);
                if (startCode == startMarker)
                {
                    endMArkerIgnoreCount++;
                    if (startPos == -1)
                    {
                        startPos = pointer + startMarker.Length;
                        startUsedLen = startMarker.Length;
                    }
                }
                else if (endCode == endMarker)
                {
                    if (endMArkerIgnoreCount <= 0)
                    {
                        if (endPos == -1)
                        {
                            endPos = pointer;
                            endUsedLen = endMarker.Length;
                        }
                        break;
                    }
                    else
                    {
                        endMArkerIgnoreCount--;
                    }
                }
                pointer++;
            } while (pointer < template.Length);
            string fragment = template.Substring(startPos, endPos - startPos);
            usedLen = startUsedLen + endUsedLen + fragment.Length;
            return fragment;
        }

        private string Extract(string template, int position, int length)
        {
            string extract = string.Empty;
            if ((position + length) < template.Length)
            {
                extract = template.Substring(position, length);
            }
            else
            {
                if (position < template.Length)
                {
                    extract = template.Substring(position);
                }
                else
                {
                    extract = string.Empty;
                }
            }
            return extract;
        }

        private string Extract(string template, int position)
        {
            string extract = string.Empty;
            if (position < template.Length)
            {
                extract = template.Substring(position);
            }
            else
            {
                extract = string.Empty;
            }
            return extract;
        }

        private string ErrorInline(string error)
        {
            return "[#" + error + "#]";
        }
        #endregion
    }
}