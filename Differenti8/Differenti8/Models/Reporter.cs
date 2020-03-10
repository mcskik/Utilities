using System;
using System.Collections.Generic;
using System.Text;
using Differenti8Engine;
using Log8;

namespace Differenti8.Models
{
	/// <summary>
	/// Reporter base class.
	/// </summary>
	/// <remarks>
	/// Reporter base class.
	/// </remarks>
	/// <author>Kenneth McSkimming</author>
	public abstract class Reporter
	{
		#region Member variables.
		protected Section moSection = null;
		protected Block moBlock = null;
		protected Logger moLog = null;
		#endregion

		#region Properties.
        /// <summary>
        /// Handover type of match ("M". "I", "D").
        /// </summary>
        /// <remarks>
        /// Passed between ReporterOfLines and ReporterOfCharacters to enable the suppression of repeat
        /// comparison type markers when one reporter hands over to the next reporter starting with the same
        /// comparison type marker as the previous one.
        /// </remarks>
        public string HandoverType { get; set; }

        /// <summary>
		/// Type of match ("M". "I", "D").
		/// </summary>
		public string Type
		{
			get
			{
				return moSection.Type;
			}
		}

		/// <summary>
		/// First displayed type marker.
		/// </summary>
		public string FirstDisplayedTypeMarker
		{
			get
			{
				string sMarker = String.Empty;
				switch (Type.Substring(0, 1))
				{
					case "M":
						sMarker = "M~= ";
						break;
					case "A":
						sMarker = "I~+ ";
						break;
					case "B":
						sMarker = "D~- ";
						break;
					case "X":
						sMarker = "X~? ";
						break;
					case "Z":
						sMarker = "Z~? ";
						break;
				}
				return sMarker;
			}
		}

		/// <summary>
		/// Subsequent displayed type marker.
		/// </summary>
		public string SubsequentDisplayedTypeMarker
		{
			get
			{
				string sMarker = String.Empty;
				switch (Type.Substring(0, 1))
				{
					case "M":
						sMarker = "  = ";
						break;
					case "A":
						sMarker = "  + ";
						break;
					case "B":
						sMarker = "  - ";
						break;
					case "X":
						sMarker = "  ? ";
						break;
					case "Z":
						sMarker = "  ? ";
						break;
				}
				return sMarker;
			}
		}
		#endregion

		#region Constructors.
		/// <summary>
		/// Construct a reporter to report the specified section of data.
		/// </summary>
		public Reporter(Section poSection, Logger poLog)
		{
			moSection = poSection;
			moBlock = poSection.Data;
			moLog = poLog;
		}
		#endregion
	}
}