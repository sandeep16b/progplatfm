using Abim.Platform.Program.Host.Config;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Util.Extensions;
using NLog;
using System;
using Topshelf;

namespace Abim.Platform.Program.Host
{
    /// <summary>
    /// Program Class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The logger
        /// </summary>
        protected static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Main method
        /// </summary>
        /// <returns></returns>
        public static int Main(string[] args)
        {
            App_Start.NHibernateProfilerBootstrapper.PreStart();
            var currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler);
            
            var configurationManager = Startup.ConfigurationManager;
            var config = configurationManager.AppSettings;
            var exitCode = HostFactory.Run(host =>
            {
                host.SetDescription(ProgramResourceConstants.AppInfo.Description);
                host.SetDisplayName("ABIM " + ProgramResourceConstants.AppInfo.DisplayName);
                host.SetServiceName(ProgramResourceConstants.AppInfo.AssemblyName);
                //host.RunAs(config["DomainUser"], config["DomainPassword"]);
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

                host.Service<ProgramApiApp>(service =>
                {
                    service.ConstructUsing(() => new ProgramApiApp());
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
        /// Exceptions the handler.
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
                catch(Exception ex)
                {
                    errorString = e.ToString();
                }
                Log.Error(errorString);
            }
        }

        /// <summary>
        /// Handles the UnhandledException event of the CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error("Service Notified of a thread exception... application is terminating:: Message: {0}, IsTerminating: {1}", e.ExceptionObject.ToString(), e.IsTerminating);
        }
    }
}


