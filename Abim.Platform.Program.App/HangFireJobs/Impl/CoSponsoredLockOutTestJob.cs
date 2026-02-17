using System;
using System.Collections.Generic;
using System.Data;
using Abim.Platform.Program.App.HangFireJobs.Helpers;
using Abim.Platform.Program.App.Services;
using Hangfire;
using Hangfire.Server;
using NHibernate;
using NLog;
using Abim.Platform.Program.App.HangFireJobs.Filters;

namespace Abim.Platform.Program.App.HangFireJobs.Impl
{
    /// <summary>
    /// A class for use as a Hangfire job to run the Year End Lookback
    /// </summary>
    [SkipConcurrentExecution(0)]
    public class CoSponsoredLockOutTestJob : ICoSponsoredLockOutTestJob
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
        public CoSponsoredLockOutTestJob(
            IHangfireWrapper hangfireWrapper,
            IProgramRulesService programRulesService,
            ISession session)
        {
            _hangfireWrapper = hangfireWrapper;
            _programRulesSvc = programRulesService;
            _session = session;
        }

        /// <summary>
        /// Executes the lock out test job process
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
                Log.Info($"Excecuting CoSponsoredLockOutJob for lockOut date {lockOutDate}.");

                using (var dbCommand = GetDbCommand())
                {
                    IEnumerable<Guid> credentialIds = GetCredentialIds(dbCommand);

                    foreach (var credentialId in credentialIds)
                    {
                        Log.Info($"Enqueueing background job for lockOut Date, credential ID {credentialId}.");
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

        private IDbCommand GetDbCommand()
        {
            var dbCommand = _session.Connection.CreateCommand();

            //TODO: Implement the code from the example below outside of this class 
            //so that we can inject the IDbCommand
            //https://adriftwith.me/coding/2009/10/17/raw-sql-queries-with-nhibernate/
            dbCommand.CommandType = CommandType.Text;
            dbCommand.CommandText =
                "select CredentialId " +
                "from LockOutPeriodCoSponsoredCredential (nolock) ";

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
