using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Copy9.DataLayer.Variables
{
	public class Variable
	{
		#region Enumerations.
        public enum VariableType
        {
            DateStamp = 1,
            Text = 2
        }
		#endregion

		#region Member Variables.
		private string _name;
		private object _value;
		private VariableType _valueType;
		private string _parseFormat;
		private string _displayFormat;
		#endregion

		#region Properties.
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		public object Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		public VariableType ValueType
		{
			get
			{
				return _valueType;
			}
			set
			{
				_valueType = value;
			}
		}

		public string ParseFormat
		{
			get
			{
				return _parseFormat;
			}
			set
			{
				_parseFormat = value;
			}
		}

		public string DisplayFormat
		{
			get
			{
				return _displayFormat;
			}
			set
			{
				_displayFormat = value;
			}
		}
		#endregion

		#region Constructors.
		public Variable(string name, object value, VariableType valueType, string parseFormat, string displayFormat)
		{
			_name = name;
			_value = value;
			_valueType = valueType;
			_parseFormat = parseFormat;
			_displayFormat = displayFormat;
		}
		#endregion

		#region Public Methods.
		public virtual string Display()
		{
			return _value.ToString();
		}

		public virtual void Parse(string value)
		{
		}

		public virtual void Parse(object value)
		{
		}
		#endregion
	}
}