using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Copy8.DataLayer.Variables
{
	public class DateStamp : Variable
	{
		#region Member Variables.
		private DateTime _dateStamp;
		#endregion

		#region Properties.
		public DateTime Value
		{
			get
			{
				return _dateStamp;
			}
			set
			{
				_dateStamp = value;
			}
		}
		#endregion

		#region Constructors.
		public DateStamp(string name, DateTime value, VariableType valueType, string parseFormat, string displayFormat)
			: base(name, value, valueType, parseFormat, displayFormat)
		{
			_dateStamp = value;
		}

		public DateStamp(string name, string value, VariableType valueType, string parseFormat, string displayFormat)
			: base(name, value, valueType, parseFormat, displayFormat)
		{
			Parse(value);
			Value = _dateStamp;
		}
		#endregion

		#region Public Methods.
		public override string Display()
		{
			return _dateStamp.ToString(DisplayFormat);
		}

		public override void Parse(string value)
		{
			CultureInfo provider = CultureInfo.CreateSpecificCulture("en-GB");
			_dateStamp = DateTime.ParseExact(value, ParseFormat, provider);
		}

		public override void Parse(object value)
		{
            if (value == System.DBNull.Value)
            {
                _dateStamp = new DateTime(1900, 01, 01);
            }
            else
            {
                _dateStamp = (DateTime)value;
            }
		}

		public void Parse(DateTime value)
		{
			_dateStamp = value;
		}
		#endregion
	}
}