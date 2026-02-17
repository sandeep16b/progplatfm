using System;
using System.Globalization;

namespace Abim.Platform.Program.WebApi.Util.General.Helpers
{
    /// <summary>
    /// A DateTime library
    /// </summary>
    public static class DateParse
    {
        /// <summary>
        /// DateTime to string, using preservation of TimeZone information.
        /// </summary>
        /// <param name="d">The dateTime.</param>
        /// <returns></returns>
        public static string DateToString(DateTime d)
        {
            return d.ToString("O");
        }

        /// <summary>
        /// String to DateTime, using preservation of TimeZone information.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns></returns>
        public static DateTime StringToDate(string s)
        {
            return DateTime.ParseExact(s, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }

        /// <summary>
        /// DateTime to simple string.
        /// </summary>
        /// <param name="d">The dateTime.</param>
        /// <returns></returns>
        public static string DateToSimpleString(DateTime d)
        {
            return d.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Simple string to DateTime.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns></returns>
        public static DateTime SimpleStringToDate(string s)
        {
            return DateTime.ParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }

        /// <summary>
        /// DateTime to time string.
        /// </summary>
        /// <param name="d">The dateTime.</param>
        /// <returns></returns>
        public static string DateToTimeString(DateTime d)
        {
            return d.ToString("yyyy-MM-dd HH:mm");
        }

        /// <summary>
        /// Time string to DateTime.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns></returns>
        public static DateTime TimeStringToDate(string s)
        {
            return DateTime.ParseExact(s, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }
    }
}
