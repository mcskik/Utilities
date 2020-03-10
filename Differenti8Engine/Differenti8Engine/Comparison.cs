using System;
using System.Collections.Generic;
using System.Text;

namespace Differenti8Engine
{
	public class Comparison
	{
		#region Enumerations.
		public enum GranularityEnum
		{
			Line = 1,
			Character = 2
		}

		public enum MatchEnum
		{
			Match = 1,
			InsertNew = 2,
			DeleteOld = 3
		}
		#endregion

		#region Member Variables.
		private GranularityEnum _granularity;
		private MatchEnum _matchType;
		private long _newPosition;
		private string _newData;
		private long _oldPosition;
		private string _oldData;
		#endregion

		#region Properties.
		/// <summary>
		/// Granularity.
		/// </summary>
		public GranularityEnum Granularity
		{
			get
			{
				return _granularity;
			}
			set
			{
				_granularity = value;
			}
		}

		/// <summary>
		/// Match type.
		/// </summary>
		public MatchEnum MatchType
		{
			get
			{
				return _matchType;
			}
			set
			{
				_matchType = value;
			}
		}

		/// <summary>
		/// New position.
		/// </summary>
		public long NewPosition
		{
			get
			{
				return _newPosition;
			}
			set
			{
				_newPosition = value;
			}
		}

		/// <summary>
		/// New data.
		/// </summary>
		public string NewData
		{
			get
			{
				return _newData;
			}
			set
			{
				_newData = value;
			}
		}

		/// <summary>
		/// Old position.
		/// </summary>
		public long OldPosition
		{
			get
			{
				return _oldPosition;
			}
			set
			{
				_oldPosition = value;
			}
		}

		/// <summary>
		/// Old data.
		/// </summary>
		public string OldData
		{
			get
			{
				return _oldData;
			}
			set
			{
				_oldData = value;
			}
		}
		#endregion

		#region Constructors.
		public Comparison(GranularityEnum granularity, MatchEnum matchType, long newPosition, string newData, long oldPosition, string oldData)
		{
			_granularity = granularity;
			_matchType = matchType;
			_newPosition = newPosition;
			_newData = newData;
			_oldPosition = OldPosition;
			_oldData = oldData;
		}
		#endregion
	}
}
