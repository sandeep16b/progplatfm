using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.Resources;
using Hangfire;
using Hangfire.Server;
using NLog;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Abim.Platform.Program.App.HangFireJobs
{
    /// <summary>
    /// Expire Timelimited Child job
    /// </summary>
    public class EarlyYearEndLookbackChildJob : IEarlyYearEndLookbackChildJob
    {
        /// <summary>
        /// The program rules service
        /// </summary>
        IProgramRulesService ProgramRulesService;

        /// <summary>
        /// The log
        /// </summary>
        protected static ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="EarlyYearEndLookbackChildJob"/> class.
        /// </summary>
        /// <param name="programRule">The program rule.</param>
        public EarlyYearEndLookbackChildJob(IProgramRulesService programRule)
        {
            ProgramRulesService = programRule;
        }

        /// <summary>
        /// ExecuteChild
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="lookbackWindowType"></param>
        /// <param name="processingDate"></param>
        /// <param name="context"></param>
        /// <param name="canellationToken"></param>
        [DisplayName("Early Year End Lookback Child Job Id: {0}")]
        [AutomaticRetry(Attempts = 0, LogEvents = true, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public async Task ExecuteChild(Guid memberId,
                                WindowsIntervalType? lookbackWindowType,
                                DateTime processingDate,
                                PerformContext context,
                                IJobCancellationToken canellationToken)
        {
            try
            {
                if (canellationToken!=null)
                    canellationToken.ThrowIfCancellationRequested();

                await ProgramRulesService.HandleEarlyYearEndLookbackChildJob( memberId, lookbackWindowType, processingDate);
            }
            catch (Exception ex)
            {
                Log.Error($"EarlyYearEndLookbackChildJob.ExecuteChild faild for memberId:'{memberId}', lookbackWindowType:'{lookbackWindowType.ToString()}' with Exception:{ex.InnerException.Message}");
                throw;
            }
        }
    }
}
