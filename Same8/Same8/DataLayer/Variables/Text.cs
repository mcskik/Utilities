using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Same8.DataLayer.Variables
{
	public class Text : Variable
	{
		#region Member Variables.
		private string _text;
		#endregion

		#region Properties.
		public string Value
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
			}
		}
		#endregion

		#region Constructors.
		public Text(string name, string value, VariableType valueType, string parseFormat, string displayFormat)
			: base(name, value, valueType, parseFormat, displayFormat)
		{
			_text = value;
		}
		#endregion

		#region Public Methods.
		public override string Display()
		{
			return _text;
		}

		public override void Parse(string value)
		{
			_text = value;
		}

		public override void Parse(object value)
		{
            if (value == System.DBNull.Value)
            {
                _text = "[#DBNull#]";
            }
            else
            {
                _text = (string)value;
            }
		}
		#endregion
	}
}