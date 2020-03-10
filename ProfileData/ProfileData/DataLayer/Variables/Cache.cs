using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProfileData.DataLayer.Variables
{
	public class Cache : Dictionary<string, Variable>
    {
        #region Constructors.
        public Cache()
		{
			this.Clear();
		}
		#endregion

		#region Public Methods.
		public void Declare(DataTable dt)
		{
			foreach (DataColumn dc in dt.Columns)
			{
                string name = dc.ColumnName;
				object value = 0;
				System.Type dataType = dc.DataType;
				string variableType = string.Empty;
				string parseFormat = string.Empty;
				string displayFormat = string.Empty;
				if (dataType == System.Type.GetType("System.String"))
				{
					value = string.Empty;
					variableType = "Text";
					parseFormat = string.Empty;
					displayFormat = string.Empty;
				}
				else if (dataType == System.Type.GetType("System.Boolean"))
				{
					value = false;
					variableType = "Boolean";
					parseFormat = "True,False";
					displayFormat = "1,0";
				}
				else if (dataType == System.Type.GetType("System.DateTime"))
				{
					value = DateTime.Now;
					variableType = "DateStamp";
					parseFormat = "yyyy-MM-dd HH:mm:ss";
					displayFormat = "dd/MM/yyyy HH:mm:ss";
				}
				else if (dataType == System.Type.GetType("System.Decimal"))
				{
					value = 0M;
					variableType = "Currency";
					parseFormat = string.Empty;
					displayFormat = "#.00";
				}
				else if (dataType == System.Type.GetType("System.Double"))
				{
					value = 0d;
					variableType = "Real";
					parseFormat = string.Empty;
					displayFormat = "#.0000";
				}
                else if (dataType == System.Type.GetType("System.Guid"))
                {
                    value = new System.Guid();
                    variableType = "Guid";
                    parseFormat = string.Empty;
                    displayFormat = string.Empty;
                }
                else if (dataType == System.Type.GetType("System.Int32"))
				{
					value = 0;
					variableType = "Integer";
					parseFormat = string.Empty;
					displayFormat = "#";
				}
                else
                {
                    value = string.Empty;
                    variableType = "Text";
                    parseFormat = string.Empty;
                    displayFormat = string.Empty;
                }
                Declare(name, value, variableType, parseFormat, displayFormat);
			}
		}

		public void Declare(string name, object value, string variableType, string parseFormat, string displayFormat)
		{
			if (this.ContainsKey(name))
			{
				//Used if a variable is re-declared to change the parse or display formats. 
				//Changing the variable type is not allowed.
				//The original value in the pre-declared variable is used
				//in preference to the newly declared value.
				Variable variable = (Variable)this[name];
				variable.ParseFormat = parseFormat;
				variable.DisplayFormat = displayFormat;
				this[name].Value = variable;
			}
			if (!this.ContainsKey(name))
			{
				Variable.VariableType valueType = Variable.VariableType.Text;
				switch (variableType)
				{
					case "DateStamp":
					case "Text":
						valueType = GetEnumValue<Variable.VariableType>(variableType);
						break;
				}
				switch (variableType)
				{
					case "Text":
						this.Add(name, new Text(name, string.Empty, valueType, parseFormat, displayFormat));
						this[name].Parse(value);
						break;
					case "DateStamp":
						this.Add(name, new DateStamp(name, DateTime.Now, valueType, parseFormat, displayFormat));
						this[name].Parse(value);
						break;
				}
			}
		}

		public void Store(DataTable dt, DataRow dr)
		{
			foreach (DataColumn dc in dt.Columns)
			{
                string name = dc.ColumnName;
				object value = dr[name];
                if (value == System.DBNull.Value)
                {
                }
                Store(name, value);
			}
		}

		/// <remarks>
		/// Only Text variable types don't need to be declared beforehand.
		/// All other variable types should have been declared already and
		/// so a matching name, initialised with the correct variable type
		/// should be found.
		/// </remarks>
		public void Store(string name, object value)
		{
            try
            {
                if (this.ContainsKey(name))
                {
                    this[name].Parse(value);
                }
                else
                {
                    this.Add(name, new Text(name, string.Empty, Variable.VariableType.Text, string.Empty, string.Empty));
                    this[name].Parse(value);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
		}

		/// <remarks>
		/// If not found then a Text error marker is returned.
		/// </remarks>
		public Variable Fetch(string name)
		{
			Variable value;
			try
			{
				value = this[name];
			}
			catch
			{
				Text errorMarker = new Text(name, "[#" + name + "#]", Variable.VariableType.Text, string.Empty, string.Empty);
				value = errorMarker;
			}
			return value;
		}
		#endregion

		#region Private Methods.
		private T GetEnumValue<T>(string enumText)
		{
			return (T)Enum.Parse(typeof(T), enumText);
		}
		#endregion
	}
}