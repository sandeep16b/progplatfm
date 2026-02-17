using Hangfire;
using Hangfire.Server;

namespace Abim.Platform.Program.App.HangFireJobs
{
    /// <summary>
    /// IYearEndLookbackTestJob
    /// </summary>
    public interface IYearEndLookbackTestJob
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
