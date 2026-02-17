using Microsoft.Owin.Diagnostics;
using Owin;

namespace Abim.Platform.Program.Host.Config
{
    /// <summary>
    /// StartUp Class.
    /// </summary>
    public partial class Startup
    {
        /// <summary>
        /// UseWelcomePage mehtod
        /// </summary>
        /// <param name="app"></param>
        private void UseWelcomePage(IAppBuilder app)
        {
            Logger.Trace("UseWelcomePage Begin");

            if (new Feature_WelcomePage().FeatureEnabled)
            {
                Logger.Info("FEATURE :: SPLASH PAGE --> ENABLED");
                app.UseWelcomePage("/assets");
            }
            else
            {
                Logger.Warn("FEATURE :: SPLASH PAGE --> DISABLED");
            }

            Logger.Trace("UseWelcomePage Complete");
        }

        /// <summary>
        /// UseErrorPage method
        /// </summary>
        /// <param name="app"></param>
        private void UseErrorPage(IAppBuilder app)
        {
            Logger.Trace("UseErrorPage Begin");
            
            app.UseErrorPage(new ErrorPageOptions()
                {
                    ShowCookies =           true,
                    ShowEnvironment =       true,
                    ShowExceptionDetails =  true,
                    ShowHeaders =           true,
                    ShowQuery =             true,
                    ShowSourceCode =        true
                });

            if (new Feature_ErrorPage().FeatureEnabled)
            {
                Logger.Info("FEATURE :: ERROR PAGE --> ENABLED");
                app.UseErrorPage();
            }
            else
            {
                Logger.Warn("FEATURE :: ERROR PAGE --> DISABLED");
            }

            Logger.Trace("UseErrorPage Complete");
        }
    }
}
