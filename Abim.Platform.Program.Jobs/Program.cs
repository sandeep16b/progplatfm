using Abim.Platform.Program.Jobs.Config;
using Abim.Platform.Program.Util.Extensions;
using NLog;
using System;
using Topshelf;

namespace Abim.Platform.Program.Jobs
{
    public class Program
    {
        /// <summary>
        /// The logger
        /// </summary>
        protected static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public static int Main(string[] args)
        {
            var currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler);

            var configurationManager = Startup.ConfigurationManager;
            var config = configurationManager.AppSettings;
            var exitCode = HostFactory.Run(host =>
            {
                host.SetDescription("Microservice to run Program Platform-related Jobs");
                host.SetDisplayName("ABIM Program Platform Jobs ");
                host.SetServiceName("Abim.Platform.Program.Jobs");

                host.StartAutomatically();
                host.DependsOn("HTTP");
                host.DependsOn("NlaSvc");
                host.UseNLog();

                host.BeforeInstall(() =>
                {
                    Log.Info("Beginning Install");
                });
                host.AfterInstall(() =>
                {
                    Log.Info("Completed Install");
                });
                host.BeforeRollback(() =>
                {
                    Log.Info("Beginning Rollback");
                });
                host.AfterRollback(() =>
                {
                    Log.Info("Completed Rollback");
                });
                host.BeforeUninstall(() =>
                {
                    Log.Info("Beginning Uninstall");
                });
                host.AfterUninstall(() => { Log.Info("Completed Uninstall"); });

                host.EnableServiceRecovery(rc =>
                {
                    rc.RestartService(1);
                    rc.RestartService(3);
                    rc.RestartService(5);
                });

                host.Service<ProgramJobsApp>(service =>
                {
                    service.ConstructUsing(() => new ProgramJobsApp());
                    service.WhenStarted(a => a.Start());
                    service.WhenStopped(a => a.Stop());
                    service.WhenPaused(s => s.Pause());
                    service.WhenContinued(s => s.Continue());
                    service.WhenShutdown(s => s.Shutdown());
                });
            });

            Environment.Exit((int)exitCode);
            return (int)exitCode;
        }

        /// <summary>
        /// App Domain-level exception handler
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private static void ExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            var e = args.ExceptionObject as Exception;
            if (e == null)
            {
                Log.Error("This exception was not an exception");
            }
            else
            {
                string errorString;
                try
                {
                    errorString = e.Stringify();
                }
                catch (Exception ex)
                {
                    errorString = ex.ToString();
                }
                Log.Error(errorString);
            }
        }

    }
}
