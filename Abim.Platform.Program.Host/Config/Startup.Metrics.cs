using Metrics;
using Owin;
using Owin.Metrics;
using System;

namespace Abim.Platform.Program.Host.Config
{
    public partial class Startup
    {
        /// <summary>
        /// UseMetrics method
        /// </summary>
        /// <param name="app"></param>
        private void UseMetrics(IAppBuilder app)
        {
            Logger.Trace("UseMetrics Begin");
        
            if (new Feature_Metrics().FeatureEnabled)
            {
                var config = Metric.Config
                    .WithInternalMetrics()
                    .WithAppCounters("Program.App")
                    .WithAppCounters("Program.Host")
                    .WithOwin(middleware => app.Use(middleware), cfg => cfg
                        .WithRequestMetricsConfig(c => c.WithAllOwinMetrics())
                        .WithMetricsEndpoint()
                    );
            
                if (new Feature_MetricsConsole().FeatureEnabled)
                {
                    var timespan = ConfigurationManager.AppSettings["Metrics.RefreshInterval"];
                    var hours    = Int32.Parse(timespan.Split('.')[0]);
                    var minutes  = Int32.Parse(timespan.Split('.')[1]);
                    var seconds  = Int32.Parse(timespan.Split('.')[2]);

                    Logger.Info("FEATURE :: METRICS -- Console Reporting ---> ENABLED");
                    config.WithReporting(c => c.WithConsoleReport(new TimeSpan(hours, minutes, seconds)));
                    Logger.Debug("(REFRESH INTERVAL) :: {0} hours, {1} minutes, {2} seconds", hours, minutes, seconds);
                }
                else
                {
                    Logger.Warn("FEATURE :: METRICS -- Console Reporting ---> DISABLED");
                }

                if (new Feature_MetricsEndpoint().FeatureEnabled)
                {
                    Logger.Info("FEATURE :: METRICS -- OWIN EndPoint ---> ENABLED");
                    config.WithOwin(middleware => app.Use(middleware), cfg => cfg.WithRequestMetricsConfig(c => c.WithAllOwinMetrics()).WithMetricsEndpoint());
                }
                else
                {
                    Logger.Warn("FEATURE :: METRICS -- OWIN EndPoint ---> DISABLED");
                }
            }
            else
            {
                Logger.Warn("FEATURE :: METRICS ---> DISABLED");
            }

            Logger.Trace("UseMetrics Complete");
        }
    }
}

