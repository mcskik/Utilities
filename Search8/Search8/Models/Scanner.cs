using Search8.DataLayer.Profile;
using System;
using System.Xml;
using System.Xml.Linq;

namespace Search8.Models
{
    /// <summary>
    /// Search scanner class.
    /// </summary>
    /// <remarks>
    /// Looks for matches of the search criteria within the specified text.
    /// Can handle three different search criteria types:
    /// SimpleText = Search for the exact text which the search criteria consists entirely of.
    /// ComplexText = Consists of one or more quoted literals separated by one or more boolean operators and parenthesis.
    /// PreGeneratedXml = No string parsing necessary, the search criteria XML document has already been created externally.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class Scanner
    {
        #region Constants.
        private const string SEARCH_CRITERIA = @"SearchCriteria";
        private const string BLOCK = @"Block";
        private const string FIND = @"Find";
        private const string NOT = @"Not";
        private const string AND = @"And";
        private const string OR = @"Or";
        private const string QUOTE = @"""";
        private const char QUOTE_CHAR = '"';
        #endregion

        #region Enumerations.
        public enum SearchCriteriaTypeEnum
        {
            SimpleText,
            ComplexText,
            PreGeneratedXml
        }
        #endregion

        #region Member variables.
        private SearchCriteriaTypeEnum _searchCriteriaType;
        private XmlDocument _searchCriteriaDocument;
        private string _searchCriteria = string.Empty;
        private string _textToSearch = string.Empty;
        #endregion

        #region Properties.
        /// <summary>
        /// Search criteria type.
        /// </summary>
        public SearchCriteriaTypeEnum SearchCriteriaType
        {
            get
            {
                return _searchCriteriaType;
            }
        }

        /// <summary>
        /// Search criteria XML document.
        /// </summary>
        public XmlDocument SearchCriteriaDocument
        {
            get
            {
                return _searchCriteriaDocument;
            }
        }
        #endregion

        #region Constructors.
        /// <summary>
        /// Main constructor.
        /// </summary>
        public Scanner(string searchCriteria)
        {
            _searchCriteria = searchCriteria;
            Parse(searchCriteria);
        }
        #endregion

        #region Public methods.
        /// <summary>
        /// Parse search criteria string.
        /// </summary>
        /// <remarks>
        /// Convert search criteria string to an XML document.
        /// </remarks>
        public void Parse(string searchCriteria)
        {
            XmlElement element = null;
            XmlNode node = null;
            CheckSearchCriteriaType(searchCriteria);
            switch (_searchCriteriaType)
            {
                case SearchCriteriaTypeEnum.PreGeneratedXml:
                    return;
                case SearchCriteriaTypeEnum.SimpleText:
                    _searchCriteriaDocument = new XmlDocument();
                    element = _searchCriteriaDocument.CreateElement(SEARCH_CRITERIA);
                    _searchCriteriaDocument.AppendChild(element);
                    node = _searchCriteriaDocument.DocumentElement;
                    element = _searchCriteriaDocument.CreateElement(FIND);
                    element.InnerText = searchCriteria;
                    node.AppendChild(element);
                    return;
                case SearchCriteriaTypeEnum.ComplexText:
                    break;
            }
            //Stick a white space at the end so that the last quote (or other control character) is always followed by a space.
            searchCriteria += " ";
            string code = string.Empty;
            string extra = string.Empty;
            _searchCriteriaDocument = new XmlDocument();
            element = _searchCriteriaDocument.CreateElement(SEARCH_CRITERIA);
            _searchCriteriaDocument.AppendChild(element);
            node = _searchCriteriaDocument.DocumentElement;
            for (int pos = 0; pos < _searchCriteria.Length - 1; pos++)
            {
                code = _searchCriteria.Substring(pos, 1);
                if (code == "&" || code == "|")
                {
                    pos++;
                    extra = _searchCriteria.Substring(pos, 1);
                    if (extra == code)
                    {
                        code += extra;
                    }
                }
                switch (code)
                {
                    case " ":
                        break;
                    case "(":
                        element = _searchCriteriaDocument.CreateElement(BLOCK);
                        node.AppendChild(element);
                        node = node.LastChild;
                        break;
                    case ")":
                        node = node.ParentNode;
                        if (node.Name == NOT)
                        {
                            node = node.ParentNode;
                        }
                        break;
                    case "+":
                        break;
                    case "-":
                        element = _searchCriteriaDocument.CreateElement(NOT);
                        node.AppendChild(element);
                        node = node.LastChild;
                        break;
                    case "!":
                        element = _searchCriteriaDocument.CreateElement(NOT);
                        node.AppendChild(element);
                        node = node.LastChild;
                        break;
                    case QUOTE:
                        int startPos = pos + code.Length;
                        int endPos = ConsumeLiteral(startPos, searchCriteria, 1);
                        string contents = searchCriteria.Substring(startPos, endPos - startPos - 1);
                        contents = UnNestQuotationMarks(contents);
                        pos = endPos - 1;
                        element = _searchCriteriaDocument.CreateElement(FIND);
                        element.InnerText = contents;
                        node.AppendChild(element);
                        break;
                    case "&&":
                        element = _searchCriteriaDocument.CreateElement(AND);
                        node.AppendChild(element);
                        break;
                    case "||":
                        element = _searchCriteriaDocument.CreateElement(OR);
                        node.AppendChild(element);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Evaluate the supplied textToSearch against the search criteria.
        /// </summary>
        /// <remarks>
        /// Text to search; could be a single line, a whole page, or a whole file.
        /// </remarks>
        public bool Evaluate(string textToSearch)
        {
            bool pass = false;
            string xPath = SEARCH_CRITERIA;
            _textToSearch = textToSearch;
            XmlNode node;
            node = _searchCriteriaDocument.SelectSingleNode(xPath);
            if (node != null)
            {
                pass = Router(node);
            }
            return pass;
        }

        /// <summary>
        /// Evaluate the supplied textToSearch against any of the search criteria.
        /// </summary>
        /// <remarks>
        /// Text to search; could be a single line, a whole page, or a whole file.
        /// </remarks>
        public bool EvaluateAny(string textToSearch)
        {
            _textToSearch = textToSearch;
            XDocument doc = XDocument.Load(Administrator.ProfileManager.SystemProfile.CurrentCriteria);
            bool pass = false;
            foreach (XElement element in doc.Descendants("Find"))
            {
                string find = (string)element;
                bool found = false;
                int pos = _textToSearch.IndexOf(find, StringComparison.InvariantCultureIgnoreCase);
                if (pos == -1)
                {
                    found = false;
                }
                else
                {
                    found = true;
                }
                pass |= found;
            }
            return pass;
        }
        #endregion

        #region Private Methods.
        /// <summary>
        /// Check search criteria type.
        /// </summary>
        private void CheckSearchCriteriaType(string searchCriteria)
        {
            _searchCriteriaType = SearchCriteriaTypeEnum.ComplexText;
            searchCriteria = searchCriteria.Trim();
            if (searchCriteria.StartsWith(string.Format(@"<{0}>", SEARCH_CRITERIA)))
            {
                if (searchCriteria.EndsWith(string.Format(@"</{0}>", SEARCH_CRITERIA)))
                {
                    try
                    {
                        _searchCriteriaDocument = new XmlDocument();
                        _searchCriteriaDocument.LoadXml(searchCriteria);
                        _searchCriteriaType = SearchCriteriaTypeEnum.PreGeneratedXml;
                        return;
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            //Stick a white space at the end so that the last quote (or other control character) is always followed by a space.
            searchCriteria += " ";
            string code = string.Empty;
            for (int pos = 0; pos < _searchCriteria.Length - 1; pos++)
            {
                code = _searchCriteria.Substring(pos, 1);
                switch (code)
                {
                    case " ":
                    case "(":
                    case ")":
                    case "+":
                    case "-":
                    case "!":
                    case "&":
                    case "|":
                        break;
                    case QUOTE:
                        int startPos = pos + code.Length;
                        int endPos = ConsumeLiteral(startPos, searchCriteria, 1);
                        string contents = searchCriteria.Substring(startPos, endPos - startPos - 1);
                        contents = UnNestQuotationMarks(contents);
                        pos = endPos - 1;
                        break;
                    default:
                        _searchCriteriaType = SearchCriteriaTypeEnum.SimpleText;
                        break;
                }
                if (_searchCriteriaType == SearchCriteriaTypeEnum.SimpleText)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Consume the full nested literal, coping with any level of nested quotes in the process.
        /// </summary>
        /// <remarks>
        /// The full nested literal becomes the contents of a <Find></Find> tag.
        /// Anything inside the outer most set of quotes in each literal is the data to find.
        /// Anything outside the outer most set of quotes in each literal is a boolean logic or parenthesis directive.
        /// </remarks>
        private int ConsumeLiteral(int startPos, string searchCriteria, int level)
        {
            int openQuotesCount = (int)System.Math.Pow(2, level);
            string openQuotes = new string(QUOTE_CHAR, openQuotesCount);
            int closeQuotesCount = (int)System.Math.Pow(2, level - 1);
            string closeQuotes = new string(QUOTE_CHAR, closeQuotesCount);
            int pos = startPos;
            int quoteBeginPos = 0;
            string code = string.Empty;
            string test = string.Empty;
            while (pos < searchCriteria.Length)
            {
                test = searchCriteria.Substring(pos, 1);
                while (test != QUOTE && pos < searchCriteria.Length)
                {
                    pos++;
                    if (pos < searchCriteria.Length)
                    {
                        test = searchCriteria.Substring(pos, 1);
                    }
                    else
                    {
                        test = string.Empty;
                    }
                }
                quoteBeginPos = pos;
                while (test == QUOTE && pos < searchCriteria.Length)
                {
                    code += test;
                    pos++;
                    test = searchCriteria.Substring(pos, 1);
                }
                if (code.Length >= openQuotes.Length)
                {
                    if (code.Substring(0, openQuotes.Length) == openQuotes)
                    {
                        pos = ConsumeLiteral(quoteBeginPos + code.Length, searchCriteria, level + 1);
                        code = string.Empty;
                        test = string.Empty;
                    }
                }
                if (code.Length >= closeQuotes.Length)
                {
                    if (code.Substring(0, closeQuotes.Length) == closeQuotes)
                    {
                        code = string.Empty;
                        test = string.Empty;
                        pos = quoteBeginPos + closeQuotes.Length;
                        break;
                    }
                }
            }
            return pos;
        }

        /// <summary>
        /// Change every occurrence of two consecutive quotes with a single quote.
        /// </summary>
        private string UnNestQuotationMarks(string text)
        {
            return text.Replace(QUOTE + QUOTE, QUOTE);
        }

        /// <summary>
        /// Route directive tags to the appropriate handler.
        /// </summary>
        private bool Router(XmlNode node)
        {
            bool pass = false;
            switch (node.Name)
            {
                case SEARCH_CRITERIA:
                case BLOCK:
                    pass = Block(node);
                    break;
                case NOT:
                    pass = !Block(node);
                    break;
                case FIND:
                    pass = Find(node);
                    break;
                default:
                    break;
            }
            return pass;
        }

        /// <summary>
        /// Handle the contents of any blocks or "Not Blocks".
        /// </summary>
        private bool Block(XmlNode node)
        {
            bool pass = false;
            bool success = false;
            bool first = true;
            string booleanOperator = OR; //Operator initially defaults to Or.
            foreach (XmlNode child in node.ChildNodes)
            {
                switch (child.Name)
                {
                    case BLOCK:
                    case NOT:
                    case FIND:
                        success = Router(child);
                        if (first)
                        {
                            pass = success;
                            first = false;
                        }
                        else if (booleanOperator == AND)
                        {
                            pass &= success;
                        }
                        else if (booleanOperator == OR)
                        {
                            pass |= success;
                        }
                        break;
                    case AND:
                    case OR:
                        booleanOperator = child.Name;
                        break;
                    default:
                        break;
                }
            }
            return pass;
        }

        /// <summary>
        /// Find the contents of the supplied node within the text to search.
        /// </summary>
        private bool Find(XmlNode node)
        {
            bool found = false;
            string find = node.InnerText;
            int pos = _textToSearch.IndexOf(find, StringComparison.InvariantCultureIgnoreCase);
            if (pos == -1)
            {
                found = false;
            }
            else
            {
                found = true;
            }
            return found;
        }
        #endregion
    }
}