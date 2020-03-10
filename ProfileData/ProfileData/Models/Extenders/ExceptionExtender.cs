using System;

namespace ProfileData.Models.Extenders
{
    /// <summary>
    /// Exception extender class.
    /// </summary>
    /// <remarks>
    /// Contains methods to format exception information in a standard way.
    /// </remarks>
    /// <author>Ken McSkimming</author>
    public static class ExceptionExtender
    {
        public static string Format(this Exception ex, string objectName, string methodName, string errorMessage)
        {
            string message = String.Format("[Object:] {1} [Method:] {2} [Error:] {3}{0}{0}[Exception:]{0}{4}{0}[StackTrace:]{0}{5}{0}{0}", Environment.NewLine, objectName, methodName, errorMessage, ex.Message, ex.StackTrace);
            return message;
        }
    }
}