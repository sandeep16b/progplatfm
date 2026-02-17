using System;
using System.Threading.Tasks;
using Abim.Platform.Program.App.Services;
using Hangfire;
using Hangfire.Server;
using NLog;

namespace Abim.Platform.Program.App.HangFireJobs.Impl
{
    /// <summary>
    /// YearEndLookbackChildJob
    /// </summary>
    public class YearEndLookbackChildJob : IYearEndLookbackChildJob
    {
        private IProgramRulesService _programRulesSvc;
        private static ILogger Log = LogManager.GetCurrentClassLogger();


        /// <summary>
        /// YearEndLookbackChildJob
        /// </summary>
        /// <param name="programRulesServce"></param>
        public YearEndLookbackChildJob(IProgramRulesService programRulesServce)
        {
            if (programRulesServce == null)
                throw new ArgumentNullException("programRulesService");

            _programRulesSvc = programRulesServce;
        }

        /// <summary>
        /// Executes the Year End Lookback for a specific member
        /// </summary>
        /// <param name="memberId">The ID of the member to execute the lookback for</param>
        /// <param name="lookbackDate">The lookback date</param>
        /// <param name="processingDate">The date the process is being run</param>
        /// <param name="context">A Hangfire PerformContext. Passed in as null in code, but substituted by Hangfire with a real value.</param>
        /// <param name="cancellationToken">A Hangfire job cancellation token. Passed in as null in code, but substituted by Hangfire with a real value.</param>
        /// <returns></returns>
        public async Task ExecuteChild(
            Guid memberId, 
            DateTime lookbackDate, 
            DateTime processingDate, 
            PerformContext context, 
            IJobCancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken != null)
                    cancellationToken.ThrowIfCancellationRequested();

                await _programRulesSvc.RunYearEndLookback(memberId, lookbackDate, processingDate);
            }
            catch (OperationCanceledException canceledEx)
            {
                Log.Error($"YearEndLookbackChildJob.ExecuteChild cancelled for memberId {memberId}, lookbackDate {lookbackDate}, processingDate {processingDate}.");
                throw;
            }
            catch (Exception ex)
            {
                Log.Error($"YearEndLookbackChildJob.ExecuteChild faild for memberId {memberId}, lookbackDate {lookbackDate}, processingDate {processingDate} with exception \"{ex.Message}\".");
                throw;
            }
        }
    }
}
