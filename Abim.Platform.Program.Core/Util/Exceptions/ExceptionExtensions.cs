using System;
using System.Linq;
using System.Reflection;


namespace Abim.Platform.Program.Util.Extensions
{
    /// <summary>
    /// Extension class
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Returns a string containing both the message AND stack trace of an exception, for preservation purposes. Also includes InnerExceptions, LoaderExceptions,
        /// etc. Note that depending what's in the exception, Dump() may not work and SafeDump() may miss some necessary data. This provides a safe alternative with
        /// complete and readable output
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="includeStackTrace">if set to <c>true</c> [include stack trace].</param>
        /// <returns></returns>
        public static string Stringify(this Exception ex, bool includeStackTrace = true)
        {
            if (ex == null) return "(this Exception is null)";
            if (ex.Message == null) return "(this Exception has a null Message)";
            try
            {
                //Message
                var str = string.Format("Message: '{0}'", ex.Message);

                //LoaderExceptions
                if (ex is ReflectionTypeLoadException)
                {
                    var loaderExceptions = ((ReflectionTypeLoadException)ex).LoaderExceptions;
                    if (loaderExceptions == null || !loaderExceptions.Any()) str += "; LoaderExceptions: []";
                    else str += string.Format("; LoaderExceptions: [{0}]", string.Join(", ", loaderExceptions.Select(l => l.Stringify()).ToArray()));
                }

                //EntityValidationErrors
                else if (ex.GetType().Name.Contains("DbEntityValidationException"))
                {
                    try
                    {
                        dynamic d = ex;
                        if (d.EntityValidationErrors != null)
                        {
                            string validationText = "";
                            foreach (var e in d.EntityValidationErrors)
                            {
                                if (validationText != "") validationText += ", ";
                                string innerValidationText = "";
                                foreach (var v in e.ValidationErrors)
                                {
                                    if (innerValidationText != "") innerValidationText += ", ";
                                    innerValidationText += JsonExtensions.JsonFormat(v);
                                }
                                validationText += innerValidationText;
                            }
                            str += string.Format("; EntityValidationErrors: [{0}]", validationText);
                        }
                        else str += "; EntityValidationErrors: []";
                    }
                    catch (Exception ex3)
                    {
                        str += string.Format($"; EntityValidationErrors: (unknown; failed to serialize (exception text '{ex3.Message}')");
                    }
                }

                //InnerException
                str += string.Format("; InnerException: '{0}'", ex.InnerException == null ? "null" : ex.InnerException.Stringify());

                //Stack Trace
                if (includeStackTrace) str += string.Format("; Stack Trace: '{0}'", ex.StackTrace ?? "(null)");

                return str;
            }
            catch (Exception ex1)
            {
                try
                {
                    var backupText = ex.JsonFormat();
                    return $"{backupText} (original serialization failed (exception text '{ex1.Message}')";
                }
                catch (Exception ex2)
                {
                    return $"{(ex.Message ?? $"(No exception text)")} (failed to obtain more verbose logging for this exception due to reason '{ex1.Message}' and then '{ex2.Message}')";
                }
            }
        }
    }
}
