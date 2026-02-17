using Microsoft.Owin.Diagnostics;
using Owin;

namespace Abim.Platform.Program.Jobs.Config
{
    public partial class Startup
    {
        /// <summary>
        /// UseWelcomePage method
        /// </summary>
        /// <param name="app"></param>
        private void UseWelcomePage(IAppBuilder app)
        {
            app.UseWelcomePage("/assets");
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
                ShowCookies = true,
                ShowEnvironment = true,
                ShowExceptionDetails = true,
                ShowHeaders = true,
                ShowQuery = true,
                ShowSourceCode = true
            });

            app.UseErrorPage();
        }
    }
}
