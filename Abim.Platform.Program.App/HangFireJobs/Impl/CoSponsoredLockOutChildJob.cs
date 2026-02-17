using Abim.Platform.Program.App.Services;
using Hangfire;
using Hangfire.Server;
using NLog;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Abim.Platform.Program.App.HangFireJobs.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public class CoSponsoredLockOutChildJob : ICoSponsoredLockOutChildJob
    {
        /// <summary>
        /// The program rules service
        /// </summary>
        IProgramRulesService ProgramRulesService;

        /// <summary>
        /// The log
        /// </summary>
        protected static ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="CoSponsoredLockOutChildJob"/> class.
        /// </summary>
        /// <param name="programRule">The program rule.</param>
        public CoSponsoredLockOutChildJob(IProgramRulesService programRule)
        {
            ProgramRulesService = programRule;
        }

        /// <summary>
        /// ExecuteChild
        /// </summary>
        /// <param name="credentialId"></param>
        /// <param name="lockOutDate"></param>
        /// <param name="processingDate"></param>
        /// <param name="context"></param>
        /// <param name="canellationToken"></param>
        [DisplayName("CoSponsored LockOut Child Job Id: {0}")]
        [AutomaticRetry(Attempts = 0, LogEvents = true, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public async Task ExecuteChild( Guid credentialId,
                                        DateTime lockOutDate,
                                        DateTime processingDate,
                                        PerformContext context,
                                        IJobCancellationToken canellationToken)
        {
            try
            {
                if (canellationToken != null)
                    canellationToken.ThrowIfCancellationRequested();

                await ProgramRulesService.RunCoSponsoredLockOut(credentialId, lockOutDate, processingDate);
            }
            catch (OperationCanceledException canceledEx)
            {
                Log.Error($"CoSponsoredLockOutChildJob.ExecuteChild cancelled for credentialId {credentialId}, lockOutDate {lockOutDate.ToString("yyyy-MM-dd")}, processingDate {processingDate}.");
                throw;
            }
            catch (Exception ex)
            {
                Log.Error($"CoSponsoredLockOutChildJob.ExecuteChild failed for credentialId:'{credentialId}', lockOutDate:'{lockOutDate.ToString("yyyy-MM-dd")}' with Exception:{ex.InnerException.Message}");
                throw;
            }
        }

    }
}
