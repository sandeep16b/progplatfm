using Abim.Platform.Program.Relational;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Hangfire.StructureMap;
using Owin;
using System;

namespace Abim.Platform.Program.Host.Config
{
    /// <summary>
    /// StartUp Class.
    /// </summary>
    public partial class Startup
    {
        /// <summary>
        /// UseOwinHangfire method.
        /// </summary>
        /// <param name="app"></param>
        private void UseOwinHangfire(IAppBuilder app)
        {
            Logger.Trace("UseHangfire: Initializing Hangfire.");
        
            if (new Feature_HangfireDashboard().FeatureEnabled)
            {
                Logger.Info("FEATURE :: HANGFIRE DASHBOARD --> ENABLED");

                GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });

                var optionsPollingInterval = new SqlServerStorageOptions
                {
                    QueuePollInterval  = TimeSpan.FromSeconds(60)
                };
                
                var configurationManager = ConfigurationManager;
                GlobalConfiguration.Configuration
                    .UseSqlServerStorage(configurationManager.ConnectionStrings["HangfireDatabase"].ConnectionString, optionsPollingInterval);
                
                GlobalConfiguration.Configuration.UseStructureMapActivator(DependencyResolver.Container);
                
                var options = new BackgroundJobServerOptions
                {
                    //Queues = new[] { ApiConstants.HangfireInfo.ProgramQueueName }
                };

                //// will fire on September 01 each year.
                //RecurringJob.AddOrUpdate<ExpireByTimeLimitJob>(x => x.Execute(null, null, null), Cron.Yearly(9, 1), TimeZoneInfo.Local);

                //// will fire on January 01 each year.
                //RecurringJob.AddOrUpdate<EarlyYearEndLookbackJob>(x => x.Execute(null, null, null, null), Cron.Yearly(1, 1), TimeZoneInfo.Local);

                app.UseHangfireServer(options);
                
                int numberOfUsers = int.Parse(configurationManager.AppSettings["HangfireUsers"]);
                BasicAuthAuthorizationUser[] hangfireUsers = new BasicAuthAuthorizationUser[numberOfUsers];
                for(int i = 0; i < numberOfUsers; i++)
                {
                    hangfireUsers[i] = new BasicAuthAuthorizationUser
                    {
                        Login = configurationManager.AppSettings["HangfireUsername" + (i + 1)],
                        PasswordClear = configurationManager.AppSettings["HangfirePassword" + (i + 1)]
                    };
                }

                app.UseHangfireDashboard("/jobs", new DashboardOptions
                {
                    AuthorizationFilters = new IAuthorizationFilter []
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
                
                Logger.Trace("UseHangfire: Hangfire initialized.");
            }
            else
            {
                Logger.Warn("FEATURE :: HANGFIRE DASHBOARD --> DISABLED");
            }
        }
    }
}
