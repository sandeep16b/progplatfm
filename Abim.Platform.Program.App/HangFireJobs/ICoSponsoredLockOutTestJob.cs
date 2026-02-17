using Hangfire;
using Hangfire.Server;

namespace Abim.Platform.Program.App.HangFireJobs
{
    /// <summary>
    /// ICoSponsoredLockOutTestJob
    /// </summary>
    public interface ICoSponsoredLockOutTestJob
    {
        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="context"></param>
        /// <param name="token"></param>
        void Execute(
            PerformContext context,
            IJobCancellationToken token);
    }
}
