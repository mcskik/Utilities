namespace Copy8.Presenters
{
    public class ProgressLink
	{
		#region Member Variables.
		private int _minimum;
		private int _maximum;
		private int _value;
		#endregion

		#region Properties.
		public int Minimum
		{
			get
			{
				return _minimum;
			}
			set
			{
				_minimum = value;
			}
		}

		public int Maximum
		{
			get
			{
				return _maximum;
			}
			set
			{
				_maximum = value;
			}
		}

		public int Value
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
		#endregion

		#region Constructors.
		public ProgressLink()
		{
			_minimum = 0;
			_maximum = 0;
			_value = 0;
		}

		public ProgressLink(int minimum, int maximum, int value)
		{
			_minimum = minimum;
			_maximum = maximum;
			_value = value;
		}
		#endregion
	}
}
