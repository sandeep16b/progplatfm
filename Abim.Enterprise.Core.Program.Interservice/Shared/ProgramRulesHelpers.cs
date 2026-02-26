using Abim.Platform.Program.Resources;
using System;

namespace Abim.Platform.Program.Interservice.Shared
{
    public static class ProgramRulesHelpers
    {

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DateTime ComputeScheduleUpdateDate()
        {
            var currentDate = DateTime.Now;
            return currentDate.Month < 3 ? new DateTime(currentDate.Year, 4, 1) : new DateTime(currentDate.Year + 1, 4, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DateTime ComputeScheduleUpdateDateForIssuance(DateTime evaluationDate)
        {
            return evaluationDate.Month < 3 ? new DateTime(evaluationDate.Year, 4, 1) : new DateTime(evaluationDate.Year + 1, 4, 1);
        }

        /// <summary>
        /// Compute Look back window range (start date, end date)  based on Earliest Cert issuance by ABIM date and
        /// certificate check date
        /// </summary>
        /// <param name="earliestCertDate"></param>
        /// <param name="checkDate"></param>
        /// <param name="windowsInterval"></param>
        /// <returns></returns>
        public static Tuple<DateTime, DateTime> ComputeLookBackWindow(DateTime earliestCertDate,
                                                                           DateTime checkDate,
                                                                           WindowsIntervalType windowsInterval)
        {
            int startDateYear;
            int endDateYear;
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();

            // ----- 5 years window ----------
            if (windowsInterval == WindowsIntervalType.FiveYearLookBack)
            {
                if (checkDate.AddDays(1).Year - earliestCertDate.Year < 11 && earliestCertDate.Year >= 2014)
                    startDate = earliestCertDate;
                else
                {
                    //*keep it: startdate = to_date('01/01/'|| to_char(greatest(2014,greatest(earlyyear+1, 2014) + ((trunc((checkyear1d - greatest(earlyyear+1, 2014)) / 5)-1) *5)) ),'mm/dd/yyyy');
                    startDateYear = Math.Max(2014, Math.Max(earliestCertDate.Year + 1, 2014) + ((((checkDate.AddDays(1).Year - Math.Max(earliestCertDate.Year + 1, 2014)) / 5) - 1) * 5));
                    startDate = new DateTime(startDateYear, 1, 1);
                }

                //*keep it: enddate = to_date('12/31/'|| to_char(greatest(2014,greatest(earlyyear+1, 2014) + ( greatest(0,(trunc((checkyear1d - greatest(earlyyear+1, 2014)) / 5)-1)) *5))+4 ),'mm/dd/yyyy');	
                endDateYear = Math.Max(2014, Math.Max(earliestCertDate.Year + 1, 2014) + (Math.Max(0, (((checkDate.AddDays(1).Year - Math.Max(earliestCertDate.Year + 1, 2014)) / 5) - 1)) * 5)) + 4;
                endDate = new DateTime(endDateYear, 12, 31);
            }
            // ----- 2 years window ----------
            else if (windowsInterval == WindowsIntervalType.TwoYearLookBack)
            {
                if (checkDate.AddDays(1).Year - earliestCertDate.Year < 5 && earliestCertDate.Year >= 2014)
                    startDate = earliestCertDate;
                else
                {
                    //*keep it: startdate = to_date('01/01/'|| to_char( greatest(earlyyear+1, 2014) + ( (trunc((checkyear1d - greatest(earlyyear+1, 2014)) / 2)-1) *2 ) ) ,'mm/dd/yyyy');
                    startDateYear = Math.Max(earliestCertDate.Year + 1, 2014) + ((((checkDate.AddDays(1).Year - Math.Max(earliestCertDate.Year + 1, 2014)) / 2) - 1) * 2);
                    startDate = new DateTime(startDateYear, 1, 1);
                }

                //*keep it: enddate = to_date('12/31/'|| to_char(greatest(2014,greatest(earlyyear+1, 2014) + ( greatest(0,(trunc((checkyear1d - greatest(earlyyear+1, 2014)) / 2)-1)) *2 ) )+1 ) ,'mm/dd/yyyy');
                endDateYear = Math.Max(2014, Math.Max(earliestCertDate.Year + 1, 2014) + (Math.Max(0, (((checkDate.AddDays(1).Year - Math.Max(earliestCertDate.Year + 1, 2014)) / 2) - 1)) * 2)) + 1;
                endDate = new DateTime(endDateYear, 12, 31);
            }

            return new Tuple<DateTime, DateTime>(startDate, endDate);
        }

        /// <summary>
        /// Compute Look back window range (start date, end date, and time period)  based on Earliest Cert issuance by ABIM date and
        /// certificate check date
        /// </summary>
        /// <param name="earliestCertDate"></param>
        /// <param name="checkDate"></param>
        /// <param name="windowsInterval"></param>
        /// <returns></returns>
        public static Tuple<DateTime, DateTime, int> ComputeLookBackWindowWithTimePeriod(DateTime earliestCertDate,
                                                                           DateTime checkDate,
                                                                           WindowsIntervalType windowsInterval)
        {
            int startDateYear;
            int endDateYear;
            int timePeriod = 0;
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();

            // ----- 5 years window ----------
            if (windowsInterval == WindowsIntervalType.FiveYearLookBack)
            {
                if (checkDate.AddDays(1).Year - earliestCertDate.Year < 11 && earliestCertDate.Year >= 2014)
                {
                    startDate = earliestCertDate;
                    timePeriod = 1;
                }
                else
                {
                    //*keep it: startdate = to_date('01/01/'|| to_char(greatest(2014,greatest(earlyyear+1, 2014) + ((trunc((checkyear1d - greatest(earlyyear+1, 2014)) / 5)-1) *5)) ),'mm/dd/yyyy');
                    startDateYear = Math.Max(2014, Math.Max(earliestCertDate.Year + 1, 2014) + ((((checkDate.AddDays(1).Year - Math.Max(earliestCertDate.Year + 1, 2014)) / 5) - 1) * 5));
                    startDate = new DateTime(startDateYear, 1, 1);
                    timePeriod = ((checkDate.Year - Math.Max(earliestCertDate.Year + 1, 2014)) / 5) + 1;
                }

                //*keep it: enddate = to_date('12/31/'|| to_char(greatest(2014,greatest(earlyyear+1, 2014) + ( greatest(0,(trunc((checkyear1d - greatest(earlyyear+1, 2014)) / 5)-1)) *5))+4 ),'mm/dd/yyyy');	
                endDateYear = Math.Max(2014, Math.Max(earliestCertDate.Year + 1, 2014) + (Math.Max(0, (((checkDate.AddDays(1).Year - Math.Max(earliestCertDate.Year + 1, 2014)) / 5) - 1)) * 5)) + 4;
                endDate = new DateTime(endDateYear, 12, 31);
            }
            // ----- 2 years window ----------
            else if (windowsInterval == WindowsIntervalType.TwoYearLookBack)
            {
                if (checkDate.AddDays(1).Year - earliestCertDate.Year < 5 && earliestCertDate.Year >= 2014)
                {
                    startDate = earliestCertDate;
                    timePeriod = 1;
                }
                else
                {
                    //*keep it: startdate = to_date('01/01/'|| to_char( greatest(earlyyear+1, 2014) + ( (trunc((checkyear1d - greatest(earlyyear+1, 2014)) / 2)-1) *2 ) ) ,'mm/dd/yyyy');
                    startDateYear = Math.Max(earliestCertDate.Year + 1, 2014) + ((((checkDate.AddDays(1).Year - Math.Max(earliestCertDate.Year + 1, 2014)) / 2) - 1) * 2);
                    startDate = new DateTime(startDateYear, 1, 1);
                    timePeriod = ((checkDate.Year - Math.Max(earliestCertDate.Year + 1, 2014)) / 2) + 1;
                }

                //*keep it: enddate = to_date('12/31/'|| to_char(greatest(2014,greatest(earlyyear+1, 2014) + ( greatest(0,(trunc((checkyear1d - greatest(earlyyear+1, 2014)) / 2)-1)) *2 ) )+1 ) ,'mm/dd/yyyy');
                endDateYear = Math.Max(2014, Math.Max(earliestCertDate.Year + 1, 2014) + (Math.Max(0, (((checkDate.AddDays(1).Year - Math.Max(earliestCertDate.Year + 1, 2014)) / 2) - 1)) * 2)) + 1;
                endDate = new DateTime(endDateYear, 12, 31);
            }

            return new Tuple<DateTime, DateTime, int>(startDate, endDate, timePeriod);
        }

        /// <summary>
        /// Compute Look back window range (start date, end date, cycle ) based on 
        /// Earliest Cert issuance by ABIM date and last lookback date per 
        /// the program rules
        /// </summary>
        /// <param name="earliestCertDate">The date of the diplomate's earliest issued ABIM certificate.</param>
        /// <param name="lastLookbackDate">The last lookback date (if any) for this diplomate.</param>
        /// <param name="windowsInterval">The lookback interval to compute for. Currently, only 2 and 5 year lookbacks are supported.</param>
        /// <returns></returns>
        public static Tuple<DateTime, DateTime, int> UIComputeLookBackWindow(DateTime earliestCertDate,
                                                                           DateTime? lastLookbackDate,
                                                                           WindowsIntervalType windowsInterval)
        {
            if (windowsInterval == WindowsIntervalType.TenYearLookBack || windowsInterval == WindowsIntervalType.ThreeYearLookBack)
                throw new ApplicationException("UIComputeLookbackWindow() currently only supports 2 and 5 year lookbacks.");

            int timePeriod = 1;
            DateTime startDate;
            DateTime endDate;

            var firstWindow = GetFirstLookbackWindow(earliestCertDate, windowsInterval);

            if (!lastLookbackDate.HasValue)
            {
                startDate = firstWindow.Item1;
                endDate = firstWindow.Item2;
            }
            else
            {
                var window =
                    GetLookbackWindow(
                        earliestCertDate,
                        firstWindow.Item1,
                        firstWindow.Item2,
                        lastLookbackDate.Value,
                        (windowsInterval == WindowsIntervalType.FiveYearLookBack ? 5 : 2));

                startDate = window.Item1;
                endDate = window.Item2;

                //PSR 143323
                //We're only in a later window if the start date of the current window 
                //is greater that the start date of the original window. Otherwise, they're
                //still in the first window.
                if (window.Item1 > firstWindow.Item1)
                    timePeriod = 2;
            }

            return new Tuple<DateTime, DateTime, int>(startDate, endDate, timePeriod);
        }

        /// <summary>
        /// Compute Look back window range (start date, end date) based on 
        /// Earliest Cert issuance by ABIM date and last lookback date per 
        /// the program rules
        /// </summary>
        /// <param name="earliestCertDate">The date of the diplomate's earliest issued ABIM certificate.</param>
        /// <param name="lastLookbackDate">The last lookback date for this diplomate.</param>
        /// <param name="windowsInterval">The lookback interval to compute for. Currently, only 2 and 5 year lookbacks are supported.</param>
        /// <returns></returns>
        public static Tuple<DateTime, DateTime> ComputeLookBackWindowForLB(DateTime earliestCertDate,
                                                                    DateTime lastLookbackDate,
                                                                    WindowsIntervalType windowsInterval)
        {
            if (windowsInterval == WindowsIntervalType.TenYearLookBack || windowsInterval == WindowsIntervalType.ThreeYearLookBack)
                throw new ApplicationException("UIComputeLookbackWindow() currently only supports 2 and 5 year lookbacks.");

            var firstWindow = GetFirstLookbackWindow(earliestCertDate, windowsInterval);

            return GetLookbackWindow(
                                earliestCertDate,
                                firstWindow.Item1,
                                firstWindow.Item2,
                                lastLookbackDate,
                                (int)windowsInterval);
        }

        /// <summary>
        /// Compute Look back window range (start date, end date, cycle ) based on 
        /// earliest cert issuance by ABIM date and current lookback window
        /// </summary>
        /// <param name="earliestCertDate">The date of the diplomate's earliest issued ABIM certificate.</param>
        /// <param name="currentWindowStart">The start date of the current lookback window.</param>
        /// <param name="currentWindowEnd">The end date of the current lookback window.</param>
        /// <param name="windowsInterval">The lookback interval to compute for. Currently, only 2 and 5 year lookbacks are supported.</param>
        /// <returns></returns>
        public static Tuple<DateTime, DateTime> UIComputePreviousLookbackWindow(
            DateTime earliestCertDate,
            DateTime currentWindowStart,
            DateTime currentWindowEnd,
            WindowsIntervalType windowsInterval)
        {
            if (windowsInterval == WindowsIntervalType.TenYearLookBack || windowsInterval == WindowsIntervalType.ThreeYearLookBack)
                throw new ApplicationException("UIComputePreviousLookbackWindow() currently only supports 2 and 5 year lookbacks.");

            /*
            We can't just subtract years from the current window to get 
            the previous window because the start date of the FIRST window 
            is the date of the diplomate's initial cert. So, we'll get 
            the first window and check it's end date with the current window 
            end date - years. If they match, their previous window was 
            their FIRST window. Otherwise, we can just subtract years to 
            get the previous window.
            */

            int yearsToSubtract = (windowsInterval == WindowsIntervalType.FiveYearLookBack ? 5 : 2);
            var firstWindow = GetFirstLookbackWindow(earliestCertDate, windowsInterval);

            if (currentWindowEnd.AddYears(-yearsToSubtract) == firstWindow.Item2)
                return firstWindow;
            else
                return new Tuple<DateTime, DateTime>(
                    currentWindowStart.AddYears(-yearsToSubtract),
                    currentWindowEnd.AddYears(-yearsToSubtract));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkDate"></param>
        /// <param name="windowsInterval"></param>
        /// <returns></returns>
        public static DateTime ComputeReattestationDueDate(DateTime checkDate,
                                                            WindowsIntervalType windowsInterval)
        {
            int numberOfYearsPlusOne = (int)windowsInterval; // was + 1

            //[P006*][C010] This can be computed by taking the check date, add 1 to the date, extract the year, subtract 5 from the year to obtain a start year,
            return new DateTime(checkDate.AddDays(1).Year - numberOfYearsPlusOne, 1, 1);

        }

        #region Private methods
        private static Tuple<DateTime, DateTime> GetFirstLookbackWindow(DateTime earliestCertDate, WindowsIntervalType windowsInterval)
        {
            if (windowsInterval == WindowsIntervalType.TenYearLookBack || windowsInterval == WindowsIntervalType.ThreeYearLookBack)
                throw new ApplicationException("GetFirstLookbackWindow() currently only supports 2 and 5 year lookbacks.");

            DateTime startDate;
            DateTime endDate;

            if (earliestCertDate.Year < 2014)
                startDate = new DateTime(2014, 1, 1);
            else
                startDate = earliestCertDate;

            if (earliestCertDate.Year < 2014)
                if (windowsInterval == WindowsIntervalType.FiveYearLookBack)
                    endDate = new DateTime(2018, 12, 31);
                else
                    endDate = new DateTime(2015, 12, 31);
            else
            {
                if (windowsInterval == WindowsIntervalType.FiveYearLookBack)
                {
                    /*
                    The 5-Year Lookback end date must be 12/31/<year of first initial certification> + 5 when all of the following are true:
                        - A 5-Year Lookback has never been performed for this Diplomate
                        - The Diplomate's first PASSED initial certification exam was on or after 2014                
                    */
                    endDate = new DateTime(startDate.Year + 5, 12, 31);
                }
                else
                {
                    /*
                    The 2 - Year Lookback end date must be 12 / 31 /< year of first initial certification> +2 when all of the following are true:
                        - A 2 - Year Lookback has never been performed for this Diplomate
                        - The Diplomate's first initial certification exam was in or after 2014
                    */
                    endDate = new DateTime(startDate.Year + 2, 12, 31);
                }
            }

            return new Tuple<DateTime, DateTime>(startDate, endDate);
        }

        private static Tuple<DateTime, DateTime> GetLookbackWindow(
            DateTime earliestCertDate,
            DateTime firstLookbackStart,
            DateTime firstLookbackEnd,
            DateTime lastLookback,
            int years)
        {
            DateTime start = firstLookbackStart;
            DateTime end = firstLookbackEnd;

            while (true)
            {
                if (lastLookback == end)
                {
                    //Last lookback is at the end of current window. Give them the 
                    //next window.
                    if (start == earliestCertDate)
                        start = new DateTime((start.Year + years + 1), 1, 1);
                    else
                        start = start.AddYears(years);

                    end = end.AddYears(years);
                    return new Tuple<DateTime, DateTime>(start, end);
                }
                if (end > lastLookback)
                {
                    //Last lookback was within current window. Return current window.
                    return new Tuple<DateTime, DateTime>(start, end);
                }
                else
                {
                    //Increment window and keep looking.
                    if (start == earliestCertDate)
                        start = new DateTime((start.Year + years + 1), 1, 1);
                    else
                        start = start.AddYears(years);

                    end = end.AddYears(years);
                }
            }
        }

        #endregion
    }
}
