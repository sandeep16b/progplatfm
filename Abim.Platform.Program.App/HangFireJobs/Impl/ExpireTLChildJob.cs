using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.Commands;
using Hangfire;
using Hangfire.Server;
using NLog;
using System;
using System.ComponentModel;

namespace Abim.Platform.Program.App.HangFireJobs
{
    /// <summary>
    /// Expire Timelimited Child job
    /// </summary>
    public class ExpireTLChildJob : IExpireTLChildJob
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
        /// Initializes a new instance of the <see cref="ExpireTLChildJob"/> class.
        /// </summary>
        /// <param name="programRule">The program rule.</param>
        public ExpireTLChildJob(IProgramRulesService programRule)
        {
            ProgramRulesService = programRule;
        }

        ///
        /// <param name="credentialId"></param>
        /// <param name="command"></param>
        /// <param name="context"></param>
        /// <param name="canellationToken"></param>
        [DisplayName("Must Be Maintained Certificate Processing Id: {0}")]
        [AutomaticRetry(Attempts = 0, LogEvents = true, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public void ExecuteChild(Guid credentialId, 
                                RunRulesForMustBeMaintainedCertificateCommand command, 
                                PerformContext context, 
                                IJobCancellationToken canellationToken)
        {
            try
            {
                canellationToken.ThrowIfCancellationRequested();

                ProgramRulesService.HandleJob(command);
            }
            catch(Exception ex)
            {
                //to-do: check for "Interservice error:"
                Log.Error(ex, $"ExecuteChild() failed with Exception - CredentialId:'{credentialId}', IssuanceId:'{command.IssuanceId}'");
                throw;
            }
        }
    }
}
