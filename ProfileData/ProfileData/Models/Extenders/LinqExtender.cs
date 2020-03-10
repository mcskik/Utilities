using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ProfileData.Models.Extenders
{
    /// <summary>
    /// Linq extender class.
    /// </summary>
    /// <remarks>
    /// Used to make simple Linq to XML statements tolerant of missing nodes or missing attributes.
    /// This prevents "object not set to an instance of an object" messages when nodes or attributes
    /// are not found.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public static class LinqExtender
    {
        #region Constants.
        private const string ZERO = "0";
        #endregion

        #region Default to empty string.
        public static IEnumerable<XElement> DescendantsOrEmpty(this XElement element, string argument)
        {
            return element.DescendantsOrDefault(argument, string.Empty);
        }

        public static XElement ElementOrEmpty(this XElement element, string argument)
        {
            return element.ElementOrDefault(argument, string.Empty);
        }

        public static XAttribute AttributeOrEmpty(this XElement element, string argument)
        {
            return element.AttributeOrDefault(argument, string.Empty);
        }
        #endregion

        #region Default to zero.
        public static IEnumerable<XElement> DescendantsOrZero(this XElement element, string argument)
        {
            return element.DescendantsOrDefault(argument, ZERO);
        }

        public static XElement ElementOrZero(this XElement element, string argument)
        {
            return element.ElementOrDefault(argument, ZERO);
        }

        public static XAttribute AttributeOrZero(this XElement element, string argument)
        {
            return element.AttributeOrDefault(argument, ZERO);
        }
        #endregion

        #region Default specified.
        public static IEnumerable<XElement> DescendantsOrDefault(this XElement element, string argument, string defaultValue)
        {
            List<XElement> empty = new List<XElement>();
            empty.Add(new XElement(argument, defaultValue));
            IEnumerable<XElement> subject = element.Descendants(argument);
            if (subject.Count() == 0)
            {
                subject = empty;
            }
            return subject;
        }

        public static XElement ElementOrDefault(this XElement element, string argument, string defaultValue)
        {
            XElement empty = new XElement(argument, defaultValue);
            XElement subject = element.Element(argument);
            if (subject == null)
            {
                subject = empty;
            }
            return subject;
        }

        public static XAttribute AttributeOrDefault(this XElement element, string argument, string defaultValue)
        {
            XAttribute empty = new XAttribute(argument, defaultValue);
            XAttribute subject = element.Attribute(argument);
            if (subject == null)
            {
                subject = empty;
            }
            return subject;
        }
        #endregion
    }
}