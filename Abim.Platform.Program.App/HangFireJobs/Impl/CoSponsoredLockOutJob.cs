using Abim.Platform.Program.App.HangFireJobs.Filters;
using Abim.Platform.Program.App.HangFireJobs.Helpers;
using Abim.Platform.Program.App.Services;
using Hangfire;
using Hangfire.Server;
using NHibernate;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;

namespace Abim.Platform.Program.App.HangFireJobs.Impl
{
    /// <summary>
    /// A class for use as a Hangfire job to run the CoSponsored LockOut
    /// </summary>
    [SkipConcurrentExecution(0)]
    public class CoSponsoredLockOutJob : ICoSponsoredLockOutJob
    {
        private static ILogger Log = LogManager.GetCurrentClassLogger();
        private IHangfireWrapper _hangfireWrapper;
        private IProgramRulesService _programRulesSvc;
        private ISession _session;

        /// <summary>
        /// CoSponsoredLockOutJob
        /// </summary>
        /// <param name="hangfireWrapper"></param>
        /// <param name="programRulesService"></param>
        /// <param name="session"></param>
        public CoSponsoredLockOutJob(
            IHangfireWrapper hangfireWrapper,
            IProgramRulesService programRulesService,
            ISession session)
        {
            _hangfireWrapper = hangfireWrapper;
            _programRulesSvc = programRulesService;
            _session = session;
        }

        /// <summary>
        /// Executes the cosponsored lock out process
        /// </summary>
        /// <param name="context">A Hangfire PerformContext. Passed in as null in code, but substituted by Hangfire with a real value.</param>
        /// <param name="cancellationToken">A Hangfire job cancellation token. Passed in as null in code, but substituted by Hangfire with a real value.</param>
        public void Execute
            (PerformContext context,
            IJobCancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken != null)
                    cancellationToken.ThrowIfCancellationRequested();

                DateTime lockOutDate = new DateTime(DateTime.Now.AddYears(-1).Year, 12, 31);
                Log.Info($"Excecuting CoSponsoredLockOutJob for lookback date {lockOutDate}.");

                using (var dbCommand = GetDbCommand(lockOutDate))
                {
                    IEnumerable<Guid> credentialIds = GetCredentialIds(dbCommand);

                    foreach (var credentialId in credentialIds)
                    {
                        Log.Info($"Enqueueing background job for lockOut process, credentialId ID {credentialId}.");
                        _hangfireWrapper.BackgroundJobClient.Enqueue<CoSponsoredLockOutChildJob>(x =>
                            x.ExecuteChild(credentialId, lockOutDate, DateTime.Now, null, null));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred in CoSponsoredLockOutJob.Execute()");
                throw;
            }
        }

        private IDbCommand GetDbCommand(DateTime lockOutDate)
        {
            var dbCommand = _session.Connection.CreateCommand();

            //TODO: Implement the code from the example below outside of this class 
            //so that we can inject the IDbCommand
            //https://adriftwith.me/coding/2009/10/17/raw-sql-queries-with-nhibernate/
            dbCommand.CommandType = CommandType.Text;
            dbCommand.CommandText =
                "select cred.CredentialGuid " +
                "from Credential cred (nolock) " +
                "where cred.IsCosponsored = 1 and (cred.LookbackDate is null or cred.LookbackDate < '" + lockOutDate.ToString("yyyy-MM-dd") + "')";

            return dbCommand;
        }

        private IEnumerable<Guid> GetCredentialIds(IDbCommand dbCommand)
        {
            using (IDataReader reader = dbCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    yield return Guid.Parse(reader[0].ToString());
                }
            }
        }
    }
}
