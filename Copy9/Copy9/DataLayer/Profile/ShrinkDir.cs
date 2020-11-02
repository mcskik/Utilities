using System.IO;
using System.Text;

namespace Copy9.DataLayer.Profile
{
    /// <summary>
    /// Directory shrinking class.
    /// </summary>
    /// <remarks>
    /// Directory shrinking class.
    /// </remarks>
    /// <author>Kenneth McSkimming</author>
    public class ShrinkDir
	{
		#region Member variables.
		private int mnMaxDisplayLength = 50;
		#endregion

		#region Properties.
		/// <summary>
		/// Maximum shrunk directory display length.
		/// </summary>
		public int MaxDisplayLength
		{
			get
			{
				return mnMaxDisplayLength;
			}
			set
			{
				mnMaxDisplayLength = value;
			}
		}
		#endregion

		#region Constructors.
		/// <summary>
		/// Default constructor.
		/// </summary>
		public ShrinkDir()
		{
		}

		/// <summary>
		/// Primary constructor.
		/// </summary>
		/// <param name="pnMaxDisplayLength">Maximum shrunk directory display length.</param>
		public ShrinkDir(int pnMaxDisplayLength)
		{
			mnMaxDisplayLength = pnMaxDisplayLength;
		}
		#endregion

		#region Public methods.
		/// <summary>
		/// Shrink the supplied directory to the current maximum
		/// display length by replacing some path elements with ellipses.
		/// </summary>
		/// <param name="psDirectory">Full directory specification.</param>
		/// <returns>Shortened display name.</returns>
		public string ShrinkDirectory(string psDirectory)
		{
			return ShrinkDirectory(psDirectory, mnMaxDisplayLength);
		}

		/// <summary>
		/// Shrink the supplied directory to the specified maximum
		/// display length by replacing some path elements with ellipses.
		/// </summary>
		/// <param name="psDirectory">Full directory specification.</param>
		/// <param name="pnMaxLength">Maximum length to display.</param>
		/// <returns>Shortened display name.</returns>
		public string ShrinkDirectory(string psDirectory, int pnMaxLength)
		{
			const string sEllipsis = "...";
			char sSeparator = Path.DirectorySeparatorChar;

			psDirectory = ReplaceSpecialFolders(psDirectory);
			if (psDirectory.Length <= pnMaxLength)
			{
				//If short enough then use without further modification.
				return psDirectory;
			}

			//Split the directory specification into an array of path elements.
			string[] aElements = psDirectory.Split(sSeparator);
			string sLeader = aElements[0] + sSeparator.ToString() + sEllipsis;
			int nLeaderLength = sLeader.Length;
			bool bLeaderReserved = false;

			//Build a display string using as many path elements as possible.
			StringBuilder sDisplay = new StringBuilder();
			int nDisplayLength = sDisplay.Length;

			//Loop backwards through all elements except the first.
			for (int nIndex = aElements.Length - 1; nIndex > 0; nIndex--)
			{
				string sElement = sSeparator.ToString() + aElements[nIndex];
				int nElementLength = sElement.Length;
				if (nDisplayLength + nElementLength <= pnMaxLength)
				{
					//This element will fit.
					sDisplay.Insert(0, sElement);
					nDisplayLength += nElementLength;
				}
				else
				{
					//Exit loop immediately at the first element which will not fit.
					break;
				}

				if (!bLeaderReserved)
				{
					if (nDisplayLength + nLeaderLength <= pnMaxLength)
					{
						//Reserve space for leader, if not yet reserved.
						bLeaderReserved = true;
						nDisplayLength += nLeaderLength;
					}
				}
			}

			//If no elements will fit then truncate the last one.
			if (sDisplay.Length == 0)
			{
				return aElements[aElements.Length - 1].Substring(0, pnMaxLength);
			}

			//Add leader if reserved.
			if (bLeaderReserved)
			{
				sDisplay.Insert(0, sLeader);
			}
			return sDisplay.ToString();
		}
		#endregion

		#region Private methods.
		/// <summary>
		/// Any directory specification found to contain the path
		/// to a special folder will have that part of its
		/// path replaced with a shortened representation of
		/// that special folder.
		/// </summary>
		/// <param name="psDirectory">Full directory specification.</param>
		/// <returns>Shortened directory specification.</returns>
		private string ReplaceSpecialFolders(string psDirectory)
		{
			psDirectory = ReplaceSpecialFolder(psDirectory, System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal));
			psDirectory = ReplaceSpecialFolder(psDirectory, System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop));
			return psDirectory;
		}

		/// <summary>
		/// Any directory specification found to contain the path
		/// to a special folder will have that part of its
		/// path replaced with a shortened representation of
		/// that special folder.
		/// </summary>
		/// <param name="psDirectory">Full directory specification.</param>
		/// <param name="psSpecialFolder">Special folder directory specification.</param>
		/// <returns>Shortened directory specification.</returns>
		private string ReplaceSpecialFolder(string psDirectory, string psSpecialFolder)
		{
			char sSeparator = Path.DirectorySeparatorChar;
			if (psDirectory.StartsWith(psSpecialFolder))
			{
				string[] aNodes = psSpecialFolder.Split(sSeparator);
				string sToken = aNodes[aNodes.Length - 1];
				psDirectory = sToken + psDirectory.Substring(psSpecialFolder.Length);
			}
			return psDirectory;
		}
		#endregion
	}
}
