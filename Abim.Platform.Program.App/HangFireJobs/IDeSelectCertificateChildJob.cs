using Abim.Platform.Program.App.DTOs;
using Hangfire;
using Hangfire.Server;
using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.App.HangFireJobs
{
    /// <summary>
    /// A Hangfire child job for processing certificate deselection
    /// </summary>
    public interface IDeselectCertificateChildJob
    {
        /// <summary>
        /// Executes the job to deselect a specified certificate
        /// </summary>
        /// <param name="memberId">The ID of the diplomate these credentials belong to</param>
        /// <param name="credentialsInfo">Information about the credentials to be deselected</param>
        /// <param name="deselectionEffectiveDate">The de-selection effective date to process de-selection for</param>
        /// <param name="context">A Hangfire PerformContext to be used by the job</param>
        /// <param name="cancellationToken">An IJobCancellationToken to be used to cancel the job if running</param>
        void ExecuteChild(Guid memberId, List<CredentialInfoDTO> credentialsInfo, DateTime deselectionEffectiveDate, PerformContext context, IJobCancellationToken cancellationToken);
    }
}
