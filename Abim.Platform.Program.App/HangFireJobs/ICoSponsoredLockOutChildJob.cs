using Hangfire;
using Hangfire.Server;
using System;
using System.Threading.Tasks;

namespace Abim.Platform.Program.App.HangFireJobs
{
    /// <summary>
    /// A Hangfire child job for processing CoSponsored Lock out child 
    /// </summary>
    public interface ICoSponsoredLockOutChildJob
    {
        /// <summary>
        /// Executes the CoSponsored Lock out for a specific credential
        /// </summary>
        /// <param name="credentialId">The ID of the credential to execute the lookback for</param>
        /// <param name="lockOutDate">The lock out date the process is being run</param>
        /// <param name="processingDate">The date the process is being run</param>
        /// <param name="context">A Hangfire PerformContext. Passed in as null in code, but substituted by Hangfire with a real value.</param>
        /// <param name="cancellationToken">A Hangfire job cancellation token. Passed in as null in code, but substituted by Hangfire with a real value.</param>
        /// <returns></returns>
        Task ExecuteChild(
            Guid credentialId,
            DateTime lockOutDate,
            DateTime processingDate,
            PerformContext context,
            IJobCancellationToken cancellationToken);
    }
}
