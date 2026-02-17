


using Abim.Platform.Program.Resources;
using Hangfire;
using Hangfire.Server;
using System;
using System.Threading.Tasks;

namespace Abim.Platform.Program.App.HangFireJobs
{
    /// <remarks>
    /// IEarlyYearEndLookbackChildJob Interface 
    /// </remarks>
    interface IEarlyYearEndLookbackChildJob
    {
        /// <summary>
        /// Execute Child Job 
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="lookbackWindowType"></param>
        /// <param name="processingDate"></param>
        /// <param name="context"></param>
        /// <param name="token"></param>
        Task ExecuteChild(Guid memberId, WindowsIntervalType? lookbackWindowType, DateTime processingDate, PerformContext context, IJobCancellationToken token);

    }
}
