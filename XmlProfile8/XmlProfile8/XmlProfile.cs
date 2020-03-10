
namespace XmlProfile8
{
    /// <summary>
    /// XML based profile.
    /// </summary>
    /// <remarks>
    /// XML based profile.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
	public class XmlProfile
    {
        #region Member variables.
		protected System.Xml.XmlDocument moXmlDoc;
		protected string msXmlFile = string.Empty;
        #endregion

        #region Constructors.
        /// <summary>
        /// Default constructor.
        /// </summary>
        public XmlProfile()
        {
        }
        #endregion

        #region Properties.
        /// <summary>
        /// Return reference to profile XML document.
        /// </summary>
        public System.Xml.XmlDocument Document
        {
			get
			{
				return moXmlDoc;
			}
        }
        #endregion

        #region Public methods.
        /// <summary>
        /// Load the profile from the specified XML file.
        /// </summary>
        /// <param name="psXmlFile">XML file to load.</param>
        public virtual void Load(string psXmlFile)
        {
			msXmlFile = psXmlFile;
			moXmlDoc = new System.Xml.XmlDocument();
			moXmlDoc.Load(psXmlFile);
		}	

        /// <summary>
        /// Build a delimited string by retrieving the same attribute from the set of
        /// consecutive elements returned by the XPath query.
        /// </summary>
        /// <param name="psXPath">Path to set of elements.</param>
        /// <param name="psAttribute">Attribute value to retrieve.</param>
        /// <param name="psDelimiter">String delimiter to use.</param>
        /// <returns>Delimited string of attribute values.</returns>
		public string FetchAttributes(string psXPath, string psAttribute, string psDelimiter)
        {
			System.Xml.XmlNode oNode;
			string sList = string.Empty;
			string sSeparator = string.Empty;
			oNode = moXmlDoc.SelectSingleNode(psXPath);
			if ( !(oNode == null) )
            {
				foreach ( System.Xml.XmlNode oChild in oNode.ChildNodes )
                {
					sList += sSeparator + oChild.Attributes.GetNamedItem(psAttribute).Value;
					sSeparator = psDelimiter;
				}
			}
			return sList;
		}

        /// <summary>
        /// Fetch the profile variable identified by the XPATH expression.
        /// </summary>
        /// <param name="psXPath">XPath to uniquely identify element.</param>
        /// <param name="psAttribute">Attribute value to retrieve.</param>
        /// <returns>Desired attribute value.</returns>
		public string Fetch(string psXPath, string psAttribute)
        {
			System.Xml.XmlNode oNode;
			string sAttribute = string.Empty;
			oNode = moXmlDoc.SelectSingleNode(psXPath);
			if ( !(oNode == null) )
            {
				foreach ( System.Xml.XmlAttribute oAttribute in oNode.Attributes )
                {
					System.Diagnostics.Debug.WriteLine("Attribute = " + oAttribute.Value);
					if ( oAttribute.Name == psAttribute )
                    {
						sAttribute = oAttribute.Value;
					}
				}
			}
			return sAttribute;
		}

        /// <summary>
        /// Helper routine to make it easier to fetch multiple different strings
        /// from the same XML containment area given by the XPath stem.
        /// </summary>
        /// <param name="psXPathStem">XPath to containment area.</param>
        /// <param name="psXPathArgument">Remainder of XPath to identify specific element.</param>
        /// <param name="psAttribute">Attribute value to retrieve.</param>
        /// <returns>Desired attribute value.</returns>
		public string FetchString(string psXPathStem, string psXPathArgument, string psAttribute)
        {
			return Fetch(psXPathStem + "[" + psXPathArgument + "]", psAttribute);
		}

        /// <summary>
        /// Helper routine to make it easier to fetch multiple different integers
        /// from the same XML containment area given by the XPath stem.
        /// </summary>
        /// <param name="psXPathStem">XPath to containment area.</param>
        /// <param name="psXPathArgument">Remainder of XPath to identify specific element.</param>
        /// <param name="psAttribute">Attribute value to retrieve.</param>
        /// <returns>Desired attribute value.</returns>
        public long FetchInteger(string psXPathStem, string psXPathArgument, string psAttribute)
        {
			string sValue = FetchString(psXPathStem, psXPathArgument, psAttribute).Trim();
			long nValue = 0;
			try
			{
				nValue = System.Int64.Parse(sValue);
			}
			catch
			{
				nValue = 0;
			}
			return nValue;
		}

		/// <summary>
		/// Helper routine to make it easier to fetch multiple different real numbers
		/// from the same XML containment area given by the XPath stem.
		/// </summary>
		/// <param name="psXPathStem">XPath to containment area.</param>
		/// <param name="psXPathArgument">Remainder of XPath to identify specific element.</param>
		/// <param name="psAttribute">Attribute value to retrieve.</param>
		/// <returns>Desired attribute value.</returns>
		public double FetchDouble(string psXPathStem, string psXPathArgument, string psAttribute)
		{
			string sValue = FetchString(psXPathStem, psXPathArgument, psAttribute).Trim();
			double nValue = 0;
			try
			{
				nValue = System.Double.Parse(sValue);
			}
			catch
			{
				nValue = 0;
			}
			return nValue;
		}

		/// <summary>
		/// Helper routine to make it easier to fetch multiple different boolean values
		/// from the same XML containment area given by the XPath stem.
		/// </summary>
		/// <param name="psXPathStem">XPath to containment area.</param>
		/// <param name="psXPathArgument">Remainder of XPath to identify specific element.</param>
		/// <param name="psAttribute">Attribute value to retrieve.</param>
		/// <returns>Desired attribute value.</returns>
		public bool FetchBool(string psXPathStem, string psXPathArgument, string psAttribute)
		{
			string sValue = FetchString(psXPathStem, psXPathArgument, psAttribute).Trim();
			bool bValue = false;
			bValue = sValue == "Yes" ? true : false;
			return bValue;
		}

		/// <summary>
        /// Store value in the profile variable identified by the XPATH expression.
        /// </summary>
        /// <param name="psXPath">XPath to uniquely identify element.</param>
        /// <param name="psAttribute">Attribute to be saved.</param>
        /// <param name="psValue">Value to save.</param>
		public void Store(string psXPath, string psAttribute, string psValue)
        {
			System.Xml.XmlNode oNode;
			oNode = moXmlDoc.SelectSingleNode(psXPath);
			if ( !(oNode == null) )
            {
				foreach ( System.Xml.XmlAttribute oAttribute in oNode.Attributes )
                {
					if ( oAttribute.Name == psAttribute )
                    {
						oAttribute.Value = psValue;
					}
				}
			}
		}

        /// <summary>
        /// Helper routine to make it easier to store multiple different strings
        /// in the same XML containment area given by the XPath stem.
        /// </summary>
        /// <param name="psXPathStem">XPath to containment area.</param>
        /// <param name="psXPathArgument">Remainder of XPath to identify specific element.</param>
        /// <param name="psAttribute">Attribute to be saved.</param>
        /// <param name="psValue">Value to save.</param>
        public void StoreString(string psXPathStem, string psXPathArgument, string psAttribute, string psValue)
        {
			Store(psXPathStem + "[" + psXPathArgument + "]", psAttribute, psValue);
		}

        /// <summary>
        /// Helper routine to make it easier to store multiple different integers
        /// in the same XML containment area given by the XPath stem.
        /// </summary>
        /// <param name="psXPathStem">XPath to containment area.</param>
        /// <param name="psXPathArgument">Remainder of XPath to identify specific element.</param>
        /// <param name="psAttribute">Attribute to be saved.</param>
        /// <param name="pnValue">Value to save.</param>
        public void StoreInteger(string psXPathStem, string psXPathArgument, string psAttribute, long pnValue)
        {
			StoreString(psXPathStem, psXPathArgument, psAttribute, pnValue.ToString().Trim());
		}

		/// <summary>
		/// Helper routine to make it easier to store multiple different real numbers
		/// in the same XML containment area given by the XPath stem.
		/// </summary>
		/// <param name="psXPathStem">XPath to containment area.</param>
		/// <param name="psXPathArgument">Remainder of XPath to identify specific element.</param>
		/// <param name="psAttribute">Attribute to be saved.</param>
		/// <param name="pnValue">Value to save.</param>
		public void StoreDouble(string psXPathStem, string psXPathArgument, string psAttribute, double pnValue)
		{
			StoreString(psXPathStem, psXPathArgument, psAttribute, pnValue.ToString().Trim());
		}

		/// <summary>
		/// Helper routine to make it easier to store multiple different boolean values
		/// in the same XML containment area given by the XPath stem.
		/// </summary>
		/// <param name="psXPathStem">XPath to containment area.</param>
		/// <param name="psXPathArgument">Remainder of XPath to identify specific element.</param>
		/// <param name="psAttribute">Attribute to be saved.</param>
		/// <param name="pbValue">Value to save.</param>
		public void StoreBool(string psXPathStem, string psXPathArgument, string psAttribute, bool pbValue)
		{
			string sValue = pbValue ? "Yes" : "No";
			StoreString(psXPathStem, psXPathArgument, psAttribute, sValue);
		}

		/// <summary>
        /// Save the profile to the XML file it was loaded from.
        /// </summary>
		public void Save()
        {
			Save(msXmlFile);
		}

        /// <summary>
        /// Save the profile to the specified XML file.
        /// </summary>
        /// <param name="psXmlFile">XML file to save to.</param>
		public virtual void Save(string psXmlFile)
        {
			moXmlDoc.Save(psXmlFile);
		}
        #endregion
    }
}
