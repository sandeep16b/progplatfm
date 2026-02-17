namespace Abim.Platform.Program.WebApi.Util.General.Lookups
{
    /// <summary>
    /// Month lookup class
    /// </summary>
    public static class Month
    {
        /// <summary>
        /// The month names
        /// </summary>
        public static readonly string[] MonthName = new string[]
        {
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December"
        };

        /// <summary>
        /// The month lengths (on non-leap years)
        /// </summary>
        public static readonly int[] MonthLength = new int[]
        {
            31,
            28,
            31,
            30,
            31,
            30,
            31,
            31,
            30,
            31,
            30,
            31
        };
        
        /// <summary>
        /// Determines whether a certain year is a leap year
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns>
        ///   <c>true</c> if [is leap year] [the specified year]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsLeapYear(int year)
        {
            if(year % 400 == 0) return true;
            if(year % 100 == 0) return false;
            if(year % 4 == 0) return true;
            return false;
        }

        /// <summary>
        /// Returns the days in February in a given year.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns></returns>
        public static int DaysInFebruary(int year)
        {
            if(IsLeapYear(year)) return 29;
            else return 28;
        }
    }
}
