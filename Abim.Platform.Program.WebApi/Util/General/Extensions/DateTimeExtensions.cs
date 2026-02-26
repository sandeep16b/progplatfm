using System;
using System.Collections.Generic;
using System.Linq;

namespace Abim.Platform.Program.WebApi.Util.General.Extensions
{
    /// <summary>
    /// A DateTime extensions class
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// The SQL minimum year
        /// </summary>
        public const int SQLMinYear = 1753;

        /// <summary>
        /// The SQL maximum year
        /// </summary>
        public const int SQLMaxYear = 9999;

        /// <summary>
        /// An NCrontab-format Cron Expression specifically designed to never occur (February 29th won't be a Wednesday until 2040)
        /// </summary>
        public const string NCrontabNever = "0 0 29 2/12000 WED";

        /// <summary>
        /// The unsupported SQL date error message
        /// </summary>
        public const string UnsupportedSQLDate = "outside the SQL-supported date range (1753 - 9999)";

        /// <summary>
        /// Checks for a SQL-supported year.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        public static bool InSQLRange(this DateTime? dateTime)
        {
            if(!dateTime.HasValue) return true;
            return InSQLRange(dateTime.Value);
        }

        /// <summary>
        /// Checks for a SQL-supported year.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        public static bool InSQLRange(this DateTime dateTime)
        {
            return (dateTime.Year >= SQLMinYear && dateTime.Year <= SQLMaxYear);
        }

        /// <summary>
        /// Generates a one-time-only Cron Expression from a DateTime
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <param name="nCrontabFormat">if set to <c>true</c> [n crontab format].</param>
        /// <param name="convertToUTC">if set to <c>true</c> [convert to UTC].</param>
        /// <returns></returns>
        public static string ToCron(this DateTime dateTime, bool nCrontabFormat = false, bool convertToUTC = false)
        {
            if(convertToUTC)
                dateTime = dateTime.ToUniversalTime();
            if(nCrontabFormat)
                return string.Format("{0} {1} {2} {3} {4}", dateTime.Minute, dateTime.Hour, dateTime.Day, dateTime.Month, (int)(dateTime.DayOfWeek));
            else
                return string.Format("0 {0} {1} {2} {3} ? {4}", dateTime.Minute, dateTime.Hour, dateTime.Day, dateTime.Month - 1, dateTime.Year);
        }

        /// <summary>
        /// Returns whether two date ranges overlap at all.
        /// </summary>
        /// <param name="dateRange1">range 1.</param>
        /// <param name="dateRange2">range 2.</param>
        /// <returns></returns>
        public static bool Overlaps(Tuple<DateTime, DateTime> dateRange1, Tuple<DateTime, DateTime> dateRange2)
        {
            return Overlaps(dateRange1.Item1, dateRange1.Item2, dateRange2.Item1, dateRange2.Item2);
        }
        
        /// <summary>
        /// Returns whether two date ranges overlap at all.
        /// </summary>
        /// <param name="range1Start">The range1 start.</param>
        /// <param name="range1End">The range1 end.</param>
        /// <param name="range2Start">The range2 start.</param>
        /// <param name="range2End">The range2 end.</param>
        /// <param name="inclusive">Whether to perform an inclusive comparison.</param>
        /// <returns></returns>
        public static bool Overlaps(DateTime range1Start, DateTime range1End, DateTime range2Start, DateTime range2End, bool inclusive = true)
        {
            if(inclusive)
                return (range1End >= range2Start && range1Start <= range2End);
            else
                return (range1End > range2Start && range1Start < range2End);
        }
        
        /// <summary>
        /// Returns the earlier of two possible dates.
        /// </summary>
        /// <param name="dateOne">The date one.</param>
        /// <param name="dateTwo">The date two.</param
        /// <param name="defaultIfBothArgumentsAreNull">If this is passed, it will be used as a return value when dateOne and dateTwo are both null.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Neither of the two DateTimes is non-null</exception>
        public static DateTime Lesser(DateTime? dateOne, DateTime? dateTwo, DateTime? defaultIfBothArgumentsAreNull = null)
        {
            if(dateOne == null && dateTwo == null)
            {
                if(defaultIfBothArgumentsAreNull != null)
                    return defaultIfBothArgumentsAreNull.Value;
                throw new Exception("Neither of the two DateTimes is non-null");
            }

            if(dateOne == null) return dateTwo.Value;
            if(dateTwo == null) return dateOne.Value;

            if(dateOne.Value < dateTwo.Value) return dateOne.Value;
            return dateTwo.Value;
        }

        /// <summary>
        /// Returns the later of two possible dates.
        /// </summary>
        /// <param name="dateOne">The date one.</param>
        /// <param name="dateTwo">The date two.</param>
        /// <param name="defaultIfBothArgumentsAreNull">If this is passed, it will be used as a return value when dateOne and dateTwo are both null.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Neither of the two DateTimes is non-null</exception>
        public static DateTime Greater(DateTime? dateOne, DateTime? dateTwo, DateTime? defaultIfBothArgumentsAreNull = null)
        {
            if(dateOne == null && dateTwo == null)
            {
                if(defaultIfBothArgumentsAreNull != null)
                    return defaultIfBothArgumentsAreNull.Value;
                throw new Exception("Neither of the two DateTimes is non-null");
            }

            if(dateOne == null) return dateTwo.Value;
            if(dateTwo == null) return dateOne.Value;

            if(dateOne.Value > dateTwo.Value) return dateOne.Value;
            return dateTwo.Value;
        }
        
        /// <summary>
        /// Splits a date time list into sublists if gaps are found.
        /// </summary>
        /// <param name="dates">The dates.</param>
        /// <returns></returns>
        public static List<List<DateTime>> SplitDateTimeListIntoSubListsIfGapsAreFound(List<DateTime> dates)
        {
            dates = dates.OrderBy(d => d).ToList();
            var listOfLists = new List<List<DateTime>>();
            DateTime? previousDate = null;
            List<DateTime> list;
            foreach(var date in dates)
            {
                if(previousDate == null || previousDate.Value.Date.AddDays(1) < date.Date)
                {
                    //new disjoint date found
                    list = new List<DateTime>();
                    listOfLists.Add(list);
                    list.Add(date);
                }
                else listOfLists.Last().Add(date);
                previousDate = date;
            }
            return listOfLists;
        }
    }
}
