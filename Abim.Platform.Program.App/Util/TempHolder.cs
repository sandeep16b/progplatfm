using Abim.Platform.Program.Resources;
using System;

namespace Abim.Enterprise.Core.Interservice.ProgramRules
{
    /// <summary>
    /// ProgramRulesHelpers1
    /// </summary>
    public static class ProgramRulesHelpers1
    {
        /// <summary>
        /// Computes the moc look back window.
        /// </summary>
        /// <param name="earliestCertDate">The earliest cert date.</param>
        /// <param name="checkDate">The check date.</param>
        /// <param name="windowsInterval">The windows interval.</param>
        /// <returns></returns>
        public static Tuple<DateTime, DateTime> ComputeMOCLookBackWindow(DateTime earliestCertDate,
                                                                               DateTime checkDate,
                                                                               WindowsIntervalType windowsInterval)
        {

            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();

            int numberOfYearsRule = (int)windowsInterval;
            int startDateYear;

            // ----- 5 years window ----------
            if (windowsInterval == WindowsIntervalType.FiveYearLookBack)
            {
                if (checkDate.AddDays(1).Year - earliestCertDate.Year < 11 && earliestCertDate.Year >= 2014)
                    startDate = earliestCertDate;
                else
                {

                    startDateYear = Math.Max(earliestCertDate.Year + 1, 2014) + ((((checkDate.AddDays(1).Year - Math.Max(earliestCertDate.Year + 1, 2014)) / numberOfYearsRule) - 1) * numberOfYearsRule);

                    startDate = new DateTime(startDateYear, 1, 1);
                }
                endDate = new DateTime(startDate.Year + 4, 12, 31);

            }
            // ----- 2 years window ----------
            else if (windowsInterval == WindowsIntervalType.TwoYearLookBack)
            {
                if (checkDate.AddDays(1).Year - earliestCertDate.Year < 5 && earliestCertDate.Year >= 2014)
                    startDate = earliestCertDate;
                else
                {

                    startDateYear = Math.Max(earliestCertDate.Year + 1, 2014) + ((((checkDate.AddDays(1).Year - Math.Max(earliestCertDate.Year + 1, 2014)) / numberOfYearsRule) - 1) * numberOfYearsRule);

                    startDate = new DateTime(startDateYear, 1, 1);
                }
                endDate = new DateTime(startDate.Year + 1, 12, 31);
            }

            return new Tuple<DateTime, DateTime>(startDate, endDate);

        }

    }
}
