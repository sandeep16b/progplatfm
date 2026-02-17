using Hangfire;
using Hangfire.Server;
using System;
using System.Threading.Tasks;

namespace Abim.Platform.Program.App.HangFireJobs
{
    /// <summary>
    /// Interface defining the job for the Year End Lookback child job (the job run per individual member ID)
    /// </summary>
    public interface IYearEndLookbackChildJob
    {
        /// <summary>
        /// Executes the Year End Lookback for a specific member
        /// </summary>
        /// <param name="memberId">The ID of the member to execute the lookback for</param>
        /// <param name="lookbackDate">The lookback date</param>
        /// <param name="processingDate">The date the process is being run</param>
        /// <param name="context">A Hangfire PerformContext. Passed in as null in code, but substituted by Hangfire with a real value.</param>
        /// <param name="cancellationToken">A Hangfire job cancellation token. Passed in as null in code, but substituted by Hangfire with a real value.</param>
        /// <returns></returns>
        Task ExecuteChild(
            Guid memberId, 
            DateTime lookbackDate, 
            DateTime processingDate, 
            PerformContext context, 
            IJobCancellationToken cancellationToken);
    }
}
