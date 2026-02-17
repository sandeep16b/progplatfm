using Abim.Platform.Program.App.Services.Commands;
using Hangfire;
using Hangfire.Server;
using System;

namespace Abim.Platform.Program.App.HangFireJobs
{
    /// <remarks>
    /// IExpireTLChildJob Interface 
    /// </remarks>
    interface IExpireTLChildJob
    {
        /// <summary>
        /// Execute Child Job for each expiring timelimited certificate.
        /// </summary>
        /// <param name="credentialId">The credential identifier.</param>
        /// <param name="command">The command.</param>
        /// <param name="context">The context.</param>
        /// <param name="token">The token.</param>
        void ExecuteChild(Guid credentialId, RunRulesForMustBeMaintainedCertificateCommand command, PerformContext context, IJobCancellationToken token);
        
    }
}
