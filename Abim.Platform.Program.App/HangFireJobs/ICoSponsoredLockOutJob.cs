using Hangfire;
using Hangfire.Server;

namespace Abim.Platform.Program.App.HangFireJobs
{
    /// <summary>
    /// An interface describing a Hangfire job to be run to CoSponsored lock out process
    /// </summary>
    public interface ICoSponsoredLockOutJob
    {
        /// <summary>
        /// Executes the Hangfire job
        /// </summary>
        /// <param name="context">A Hangfire PerformContext to be used by the job</param>
        /// <param name="cancellationToken">An IJobCancellationToken to be used to cancel the job if running</param>
        void Execute(
            PerformContext context,
            IJobCancellationToken cancellationToken);
    }
}
