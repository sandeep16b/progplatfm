using Abim.Platform.Program.App.HangFireJobs.Helpers;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.Commands;
using Hangfire;
using Hangfire.Server;
using NLog;
using System;
using System.ComponentModel;
using System.Linq;

namespace Abim.Platform.Program.App.HangFireJobs
{
    /// <summary>
    /// Expire Timelimited credential expire main job
    /// </summary>
    public class ExpireByTimeLimitJob : IExpireByTimeLimitJob
    {
        private IHangfireWrapper _hangfireWrapper;
        private IProgramRulesService _programRuleSerice;
        private ICredentialService _credentialService;

        /// <summary>
        /// The log
        /// </summary>
        protected static ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpireByTimeLimitJob"/> class.
        /// </summary>
        /// <param name="hangfireWrapper">A wrapper around Hangfire to allow for unit testing.</param>
        /// <param name="programRule">The program rule.</param>
        /// <param name="credentialService">The credential service.</param>
        public ExpireByTimeLimitJob(
            IHangfireWrapper hangfireWrapper, 
            IProgramRulesService programRule, 
            ICredentialService credentialService)
        {
            _hangfireWrapper = hangfireWrapper;
            _programRuleSerice = programRule;
            _credentialService = credentialService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="token"></param>
        /// <param name="recordsToProcess"></param>
        [DisableConcurrentExecution(120)]
        [DisplayName("Must Be Maintained Certificate Jobs")]
        [AutomaticRetry(Attempts = 0, LogEvents = true, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public void Execute(PerformContext context, IJobCancellationToken token, int? recordsToProcess)
        {
            try
            {
                DateTime eventDate = new DateTime(DateTime.Now.Year, 9, 1);
                DateTime endDate = eventDate.AddDays(-1).AddMonths(4);

                Log.Debug(" Start ExpireByTimeLimitJob.Execute() Event Date :: '{0}'", eventDate);

                var expiringCredentials = _credentialService.GetExpiredCredentials(eventDate, endDate, true).ToList(); //Last param specifies to get only creds issued by ABIM

                Log.Debug(" GetExpiredCredentials Count : '{0}'", expiringCredentials.Count());

                // limit number of records to limit in processing
                if (recordsToProcess.HasValue && recordsToProcess > 0)
                    expiringCredentials = expiringCredentials.Take(recordsToProcess.Value).ToList();

                foreach (Tuple<Guid, int> credentialIds in expiringCredentials)
                {
                    var command = new RunRulesForMustBeMaintainedCertificateCommand
                    {
                        CredentialId = credentialIds.Item1,
                        IssuanceId = credentialIds.Item2,
                        EventDate = eventDate,
                        CreatedBy = "MBMforTL", // per PBI
                        ProcessingDate=DateTime.Now
                    };

                    _hangfireWrapper
                        .BackgroundJobClient
                        .Enqueue<ExpireTLChildJob>(x => 
                            x.ExecuteChild(command.CredentialId, command, null, null));
                }

                Log.Debug("End ExpireByTimeLimitJob.Execute()");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

    }

}