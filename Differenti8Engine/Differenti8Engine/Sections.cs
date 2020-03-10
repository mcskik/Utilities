using System;
using System.Collections;
using System.Text;

namespace Differenti8Engine
{
	/// <summary>
	/// Collection of matched sections.
	/// </summary>
	/// <remarks>
	/// Generic type safe collection of matched sections.
	/// </remarks>
	/// <author>Kenneth McSkimming</author>
	public class Sections : ArrayList //CollectionBase
	{
		#region Enumerations.
		public enum ProgressOnEnum
		{
			Alpha = 0,
			Beta = 1
		}
		#endregion

		#region Member Variables.
		private ProgressOnEnum _progressOn;
		#endregion

		#region Properties.
		public ProgressOnEnum ProgressOn
		{
			get
			{
				return _progressOn;
			}
			set
			{
				_progressOn = value;
			}
		}
		#endregion

		#region Custom Event Arguments.
		public class AddSectionEventArgs : EventArgs
		{
			public readonly long SectionLength;
			public AddSectionEventArgs(long pnSectionLength)
			{
				this.SectionLength = pnSectionLength;
			}
		}
		#endregion

		#region Delegates.
		public delegate void AddSectionHandler(AddSectionEventArgs e);
		#endregion

		#region Event Declarations.
		public event AddSectionHandler OnAddSection;
		#endregion

		#region Event raising helper methods.
		/// <summary>
		/// Trigger add section event.
		/// </summary>
		/// <param name="pnSectionLength">Section length.</param>
		private void SignalAddSection(long pnSectionLength)
		{
			if (OnAddSection != null)
			{
				OnAddSection(new AddSectionEventArgs(pnSectionLength));
			}
		}
		#endregion

		#region Constructors.
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Sections()
		{
		}
		#endregion

		#region Public methods.
		/// <summary>
		/// Indexer.
		/// </summary>
		/// <param name="index">Index number.</param>
		/// <returns>A section.</returns>
		public override object this[int index]
		{
			get
			{
				return base[index];
			}
			set
			{
				base[index] = value;
			}
		}

		/// <summary>
		/// Add a section.
		/// </summary>
		/// <param name="value">Section.</param>
		/// <returns>Index number of new section.</returns>
		public int Add(Section poSection)
		{
			int index = base.Add(poSection);
			if (_progressOn == ProgressOnEnum.Alpha)
			{
				if (poSection.Type.Substring(0, 1) == "A" || poSection.Type.Substring(0, 1) == "M")
				{
					SignalAddSection(poSection.Data.ContentsLength);
				}
			}
			else if (_progressOn == ProgressOnEnum.Beta)
			{
				if (poSection.Type.Substring(0, 1) == "B" || poSection.Type.Substring(0, 1) == "M")
				{
					SignalAddSection(poSection.Data.ContentsLength);
				}
			}
			return index;
		}
		#endregion
	}
}
