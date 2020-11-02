using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Copy9.Views
{
	public class ControlMessage
	{
		#region Member Variables.
		private Control _control;
		private string _message;
		#endregion

		#region Properties.
		public Control Control
		{
			get
			{
				return _control;
			}
			set
			{
				_control = value;
			}
		}

		public string Message
		{
			get
			{
				return _message;
			}
			set
			{
				_message = value;
			}
		}
		#endregion

		#region Constructors.
		public ControlMessage(Control control)
		{
			_control = control;
			_message = string.Empty;
		}
		#endregion
	}
}
