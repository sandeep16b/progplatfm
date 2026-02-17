using Abim.Platform.Program.App.HangFireJobs;
using Abim.Platform.Program.App.HangFireJobs.Impl;
using Abim.Platform.Program.Relational;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Hangfire.StructureMap;
using Owin;
using System;

namespace Abim.Platform.Program.Jobs.Config
{
    public partial class Startup
    {
        private void UseOwinHangfire(IAppBuilder app)
        {
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });

            var optionsPollingInterval = new SqlServerStorageOptions
            {
                QueuePollInterval = TimeSpan.FromSeconds(60)
            };

            var configurationManager = ConfigurationManager;
            GlobalConfiguration.Configuration
                .UseSqlServerStorage(configurationManager.ConnectionStrings["HangfireDatabase"].ConnectionString, optionsPollingInterval);

            GlobalConfiguration.Configuration.UseStructureMapActivator(DependencyResolver.Container);

            var options = new BackgroundJobServerOptions
            {
                //Queues = new[] { ApiConstants.HangfireInfo.ProgramQueueName }
            };

            var enviroment = configurationManager.AppSettings["Abim.Common.Env"];

            //Add Year End Look Back
            //This is a recurring job that should never occur as scheduled, only when 
            //initiated manually. See this URL for info on the CRON expression used:
            //https://stackoverflow.com/questions/37587167/set-an-on-demand-only-job-in-hangfire
            RecurringJob.AddOrUpdate<YearEndLookbackJob>(x => x.Execute(null, null), Cron.Never(), TimeZoneInfo.Local);

            //Add Lock Out Process 
            //This is a recurring job that should never occur as scheduled, only when 
            //initiated manually. See this URL for info on the CRON expression used:
            //https://stackoverflow.com/questions/37587167/set-an-on-demand-only-job-in-hangfire
            RecurringJob.AddOrUpdate<CoSponsoredLockOutJob>(x => x.Execute(null, null), Cron.Never(), TimeZoneInfo.Local);

            //Add Year End Look Back Test Job. This also is a recurring job that should never occur as scheduled, 
            //only when initiated manually.
            RecurringJob.AddOrUpdate<YearEndLookbackTestJob>(x => x.Execute(null, null), Cron.Never(), TimeZoneInfo.Local);

            //Add Lock Out Process Test Job. This also is a recurring job that should never occur as scheduled, 
            //only when initiated manually.
            RecurringJob.AddOrUpdate<CoSponsoredLockOutTestJob>(x => x.Execute(null, null), Cron.Never(), TimeZoneInfo.Local);

            // will fire on September 01 each year.
            RecurringJob.AddOrUpdate<ExpireByTimeLimitJob>(x => x.Execute(null, null, null), Cron.Yearly(9, 1), TimeZoneInfo.Local);

            // will fire on January 01 each year.
            RecurringJob.AddOrUpdate<EarlyYearEndLookbackJob>(x => x.Execute(null, null, null, null), Cron.Yearly(1, 1), TimeZoneInfo.Local);

            if (enviroment == "PROD")
            {
                // will fire on April 01 each year.
                RecurringJob.AddOrUpdate<DeselectCertificateJob>(x => x.Execute(null, null), Cron.Yearly(4, 1), TimeZoneInfo.Local);
            }
            else
            {
                //only when initiated manually.
                RecurringJob.AddOrUpdate<DeselectCertificateJob>(x => x.Execute(null, null), Cron.Never(), TimeZoneInfo.Local);
            }

            app.UseHangfireServer(options);

            int numberOfUsers = int.Parse(configurationManager.AppSettings["HangfireUsers"]);
            BasicAuthAuthorizationUser[] hangfireUsers = new BasicAuthAuthorizationUser[numberOfUsers];
            for (int i = 0; i < numberOfUsers; i++)
            {
                hangfireUsers[i] = new BasicAuthAuthorizationUser
                {
                    Login = configurationManager.AppSettings["HangfireUsername" + (i + 1)],
                    PasswordClear = configurationManager.AppSettings["HangfirePassword" + (i + 1)]
                };
            }

            app.UseHangfireDashboard("/jobs", new DashboardOptions
            {
                AuthorizationFilters = new IAuthorizationFilter[]
                {
                    new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
                    {
                        LoginCaseSensitive = true,
                        RequireSsl = false,
                        SslRedirect = false,
                        Users = hangfireUsers
                    })
                }
            });
        }
    }
}
