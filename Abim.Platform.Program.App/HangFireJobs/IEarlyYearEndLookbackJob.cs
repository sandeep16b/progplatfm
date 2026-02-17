using Hangfire;
using Hangfire.Server;
using System;

namespace Abim.Platform.Program.App.HangFireJobs
{
    /// <summary>
    /// IEarlyYearEndLookbackJob
    /// </summary>
    interface IEarlyYearEndLookbackJob
    {
        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="context"></param>
        /// <param name="token"></param>
        /// <param name="eventDate"></param>
        /// <param name="recordsToProcess"></param>
        void Execute(PerformContext context, IJobCancellationToken token, DateTime? eventDate, int? recordsToProcess);
    }
}
