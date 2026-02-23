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
    /// A class for use as a Hangfire job to run the Year End Lookback
    /// </summary>
    [SkipConcurrentExecution(0)]
    public class YearEndLookbackTestJob : IYearEndLookbackTestJob
    {
        private static ILogger Log = LogManager.GetCurrentClassLogger();
        private IHangfireWrapper _hangfireWrapper;
        private IProgramRulesService _programRulesSvc;
        private ISession _session;

        /// <summary>
        /// YearEndLookbackTestJob
        /// </summary>
        /// <param name="hangfireWrapper"></param>
        /// <param name="programRulesService"></param>
        /// <param name="session"></param>
        public YearEndLookbackTestJob(
            IHangfireWrapper hangfireWrapper,
            IProgramRulesService programRulesService,
            ISession session)
        {
            _hangfireWrapper = hangfireWrapper;
            _programRulesSvc = programRulesService;
            _session = session;
        }

        /// <summary>
        /// Executes the year end lookback process
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

                DateTime lookbackDate = new DateTime(DateTime.Now.AddYears(-1).Year, 12, 31);
                Log.Info($"Excecuting YearEndLookbackTestJob for lookback date {lookbackDate}.");

                using (var dbCommand = GetDbCommand(lookbackDate))
                {
                    IEnumerable<Guid> memberIds = GetDiplomateIds(dbCommand);

                    foreach (var memberId in memberIds)
                    {
                        Log.Info($"Enqueueing background job for lookback, member ID {memberId}.");
                        _hangfireWrapper.BackgroundJobClient.Enqueue<YearEndLookbackChildJob>(x =>
                            x.ExecuteChild(memberId, lookbackDate, DateTime.Now, null, null));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred in YearEndLookbackTestJob.Execute()");
                throw;
            }
        }

        private IDbCommand GetDbCommand(DateTime lookbackDate)
        {
            var dbCommand = _session.Connection.CreateCommand();

            //TODO: Implement the code from the example below outside of this class 
            //so that we can inject the IDbCommand
            //https://adriftwith.me/coding/2009/10/17/raw-sql-queries-with-nhibernate/
            dbCommand.CommandType = CommandType.Text;
            dbCommand.CommandText =
                "select distinct MemberId " +
                "from    LookbackMember (nolock) ";

            return dbCommand;
        }

        private IEnumerable<Guid> GetDiplomateIds(IDbCommand dbCommand)
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
