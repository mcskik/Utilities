using System;

namespace ProfileData.Models.Extenders
{
    /// <summary>
    /// Csv extender class.
    /// </summary>
    /// <remarks>
    /// Adds useful CSV datatype handling methods to a number of nullable datatype classes.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public static class CsvExtender
    {
        public static string ToCsv(this bool? flag)
        {
            string text = string.Empty;
            if (flag == null)
            {
                text = "False";
            }
            else
            {
                text = (bool)flag ? "true" : "False";
            }
            return text;
        }

        public static string ToCsv(this DateTime? dateTime)
        {
            return dateTime != null ? ((DateTime)dateTime).ToString(@"yyyy-MM-dd HH:mm:ss") : string.Empty;
        }

        public static string ToCsv(this decimal? number)
        {
            return number != null ? number.ToString().Trim() : string.Empty;
        }

        public static string ToCsv(this Guid? guid)
        {
            return guid != null ? "{" + guid.ToString() + "}" : string.Empty;
        }

        public static string ToCsv(this int? number)
        {
            return number != null ? number.ToString().Trim() : string.Empty;
        }

        public static string ToCsvQuoted(this string text)
        {
            return text != null ? @"""" + text.Trim() + @"""" : string.Empty;
        }

        public static string ToCsv(this string text)
        {
            return text != null ? text.Trim() : string.Empty;
        }
    }
}