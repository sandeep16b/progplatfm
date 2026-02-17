using Hangfire;
using Hangfire.Server;

namespace Abim.Platform.Program.App.HangFireJobs
{
    /// <summary>
    /// IYearEndLookbackJob
    /// </summary>
    public interface IYearEndLookbackJob
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
