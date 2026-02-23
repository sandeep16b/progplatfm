
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.HangFireJobs.Filters;
using Abim.Platform.Program.App.HangFireJobs.Helpers;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.Resources;
using Hangfire;
using Hangfire.Server;
using NLog;
using System;
using System.ComponentModel;
using System.Linq;

namespace Abim.Platform.Program.App.HangFireJobs
{
    /// <summary>
    /// A class for use as a Hangfire job to run the Early Year End Lookback
    /// </summary>
    [SkipConcurrentExecution(0)]
    public class EarlyYearEndLookbackJob : IEarlyYearEndLookbackJob
    {

        private ILookBackDatesInfoService LookBackDatesInfoService;
        private IHangfireWrapper _hangfireWrapper;

        /// <summary>
        /// The log
        /// </summary>
        protected static ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="EarlyYearEndLookbackJob"/> class.
        /// </summary>
        /// <param name="lookBackDatesInfoService">The credential service.</param>
        /// <param name="hangfireWrapper">hangfireWrapper.</param>
        public EarlyYearEndLookbackJob(ILookBackDatesInfoService lookBackDatesInfoService, IHangfireWrapper hangfireWrapper)
        {
            LookBackDatesInfoService = lookBackDatesInfoService;
            _hangfireWrapper = hangfireWrapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="token"></param>
        /// <param name="recordsToProcess"></param>
        /// <param name="eventDate"></param>
        [DisableConcurrentExecution(120)]
        [DisplayName("Early Year End Look Back Job on January 1")]
        [AutomaticRetry(Attempts = 0, LogEvents = true, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public void Execute(PerformContext context, IJobCancellationToken token, DateTime? eventDate, int? recordsToProcess)
        {
            try
            {
                if (token != null)
                    token.ThrowIfCancellationRequested();

                DateTime processingDate = DateTime.Now;
                Log.Info($"Started EarlyYearEndLookbackJob.Execute() on '{processingDate}'");

                eventDate = !eventDate.HasValue ? new DateTime(processingDate.Year - 1, 12, 31) : eventDate;

                var expiringWindows = LookBackDatesInfoService.GetExpiredLookBackDatesInfo(eventDate.Value).Result;

                Log.Info("GetExpiredLookBackDatesInfo Count : '{0}'", expiringWindows.Count());

                // limit number of records to limit in pricessing
                if (recordsToProcess.HasValue && recordsToProcess > 0)
                    expiringWindows = expiringWindows.Take(recordsToProcess.Value);

                WindowsIntervalType? lookbackWindowType = null;

                foreach (LookBackDatesInfo lookBackDatesInfo in expiringWindows)
                {
                    // find what window we should re-calculate
                    if ((lookBackDatesInfo.Lookback5YearEndDate?.Date <= eventDate?.Date &&
                        lookBackDatesInfo.Lookback2YearEndDate?.Date <= eventDate?.Date) ||
                        (!lookBackDatesInfo.Lookback5YearEndDate.HasValue || !lookBackDatesInfo.Lookback2YearEndDate.HasValue))
                        lookbackWindowType = null;
                    else if (lookBackDatesInfo.Lookback5YearEndDate?.Date <= eventDate?.Date)
                        lookbackWindowType = WindowsIntervalType.FiveYearLookBack;
                    else if (lookBackDatesInfo.Lookback2YearEndDate?.Date <= eventDate?.Date)
                        lookbackWindowType = WindowsIntervalType.TwoYearLookBack;
                    else
                        lookbackWindowType = null;

                    _hangfireWrapper.BackgroundJobClient.Enqueue<EarlyYearEndLookbackChildJob>(x => x.ExecuteChild(lookBackDatesInfo.ExternalId, lookbackWindowType, processingDate, null, null));

                }

                Log.Debug("End EarlyYearEndLookbackJob.Execute()");

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred in EarlyYearEndLookbackJob.Execute()");
                throw;
            }
        }

    }

}