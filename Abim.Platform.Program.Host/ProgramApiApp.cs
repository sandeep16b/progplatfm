using Abim.Platform.Program.Host.Config;
using Abim.Platform.Program.Interservice.Shared;
using Abim.Platform.Program.Relational;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Config;
using Microsoft.Owin.Hosting;
using NLog;
using System;
using System.Linq;
using Abim.Platform.Product.Resources.Constants;

namespace Abim.Platform.Program.Host
{
    /// <summary>
    /// ProgramApi Host Server Application
    /// </summary>
    public class ProgramApiApp
    {
        /// <summary>
        /// The logger
        /// </summary>
        protected static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The Owin web application
        /// </summary>
        protected IDisposable WebApplication;

        /// <summary>
        /// Start ProgramApi Host Server
        /// </summary>
        public virtual void Start()
        {
            Logger.Info("Starting {0}", ProductResourceConstants.AppInfo.AssemblyHostName);
            var configurationManager = Startup.ConfigurationManager;
            WebApplication = WebApp.Start<Startup>(configurationManager.AppSettings["OwinUrl"]);
            HostProperties.Status = configurationManager.AppSettings["RunningStatusText"];
            Logger.Info("{0}.ProgramApiServer Started", ProgramResourceConstants.AppInfo.ShortName);
        }

        /// <summary>
        /// Method to stop the application.
        /// </summary>
        public virtual void Stop()
        {
            var serverName = ProgramResourceConstants.AppInfo.ShortName + ".ProgramApiServer";
            
            Logger.Info("Service Stop requested");
            var configurationManager = Startup.ConfigurationManager;
            var stoppingStatusText = configurationManager.AppSettings["StoppingStatusText"];
            var cooldownSeconds = Convert.ToInt32(configurationManager.AppSettings["CooldownSeconds"]);
            
            HostProperties.Status = stoppingStatusText;
            Logger.Info("Setting status to {0}", HostProperties.Status);
            Logger.Info("Beginning cooldown phase; service will stop in {0} seconds", cooldownSeconds);
            System.Threading.Thread.Sleep(cooldownSeconds * 1000);

            var interserviceInstanceNames = DependencyResolver.Container.Model.AllInstances.Where(x => x.PluginType == typeof(IInterservice)).Select(x => x.Name);
            foreach(var name in interserviceInstanceNames)
            {
                DependencyResolver.Container.GetInstance<IInterservice>(name).Dispose();
            }
            
            Logger.Info("Stopping " + serverName);
            WebApplication.Dispose();
            Logger.Info("{0} Stopped", serverName);
        }

        /// <summary>
        /// Method to pause the application.
        /// </summary>
        public virtual void Pause()
        {
            Logger.Info("{0}.ProgramApiServer Paused", ProgramResourceConstants.AppInfo.ShortName);
        }

        /// <summary>
        /// Method to continue the application.
        /// </summary>
        public virtual void Continue()
        {
            Logger.Info("{0}.ProgramApiServer Now Running", ProgramResourceConstants.AppInfo.ShortName);
        }

        /// <summary>
        /// Method to shut down the application.
        /// </summary>
        public virtual void Shutdown()
        {
            Logger.Info("{0}.ProgramApiServer Shutdown Completed", ProgramResourceConstants.AppInfo.ShortName);
        }
    }
}
