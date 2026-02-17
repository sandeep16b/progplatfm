using Hangfire;
using Hangfire.Server;

namespace Abim.Platform.Program.App.HangFireJobs
{
    /// <remarks>
    /// IExpireByTimeLimitJob Interface 
    /// </remarks>
    interface IExpireByTimeLimitJob
    {
        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="context"></param>
        /// <param name="token"></param>
        /// <param name="recordsToProcess"></param>
        void Execute(PerformContext context, IJobCancellationToken token, int? recordsToProcess);
    }
}
