using System;
using System.Collections.Generic;
using System.Text;

namespace Differenti8Engine
{
	public class ParameterException : ApplicationException
	{
		#region Member Variables.
		private readonly string _parameter;
		private readonly string _message;
		#endregion

		#region Properties.
		public string Parameter
		{
			get
			{
				return _parameter;
			}
		}

		public override string Message
		{
			get
			{
				return _message;
			}
		}
		#endregion

		#region Constructors.
		public ParameterException(string parameter, string message)
		{
			_parameter = parameter;
			_message = message;
		}
		#endregion
	}
}
