using Abim.Platform.Program.App.HangFireJobs.Helpers;
using Abim.Platform.Program.App.Services;
using Hangfire;
using Hangfire.Server;
using NLog;
using System;
using System.Linq;

namespace Abim.Platform.Program.App.HangFireJobs
{
    /// <summary>
    /// A Hangfire job to be run to deselect certificates
    /// </summary>
    public class DeselectCertificateJob : IDeselectCertificateJob
    {
        private static ILogger Log = LogManager.GetCurrentClassLogger();
        private IHangfireWrapper _hangfireWrapper;
        private ICredentialService _credentialService;


        /// <summary>
        /// Instantiates a new instance of DeSelectCertificateJob
        /// </summary>
        /// <param name="credentialService">An instance of ICredentialService to be used for retrieving credential IDs</param>
        /// <param name="hangfireWrapper">An instance of IHangfireWrapper for executing child Hangfire jobs</param>
        public DeselectCertificateJob(
            ICredentialService credentialService,
            IHangfireWrapper hangfireWrapper)
        {
            _credentialService = credentialService ?? throw new ArgumentNullException("credentialService");
            _hangfireWrapper = hangfireWrapper ?? throw new ArgumentNullException("hangfireWrapper");
        }

        /// <summary>
        /// Executes the Hangfire job
        /// </summary>
        /// <param name="context">A Hangfire PerformContext to be used by the job</param>
        /// <param name="cancellationToken">An IJobCancellationToken to be used to cancel the job if running</param>
        public void Execute(PerformContext context, IJobCancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken != null)
                    cancellationToken.ThrowIfCancellationRequested();

                DateTime deselectionEffectiveDate = new DateTime(DateTime.Now.Year, 4, 1);
                
                Log.Info($"Executing DeSelectCertificateJob for deselection effective date {deselectionEffectiveDate}.");

                var infoOfCredentialsToDeselect = 
                    _credentialService.GetInfoOfCredentialsMarkedForDeselection(deselectionEffectiveDate);

                foreach (var credentialGroup in infoOfCredentialsToDeselect)
                {
                    var memberId = credentialGroup.Key;
                    var credentialsInfo = credentialGroup.ToList();

                    Log.Info($"Enqueueing child job for credential deselection: Member ID {memberId}.");
                    _hangfireWrapper
                        .BackgroundJobClient
                        .Enqueue<IDeselectCertificateChildJob>(job =>
                            job.ExecuteChild(memberId, credentialsInfo, deselectionEffectiveDate, null, null));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred in DeselectCertificateJob.Execute()");
                throw;
            }
        }
    }
}
