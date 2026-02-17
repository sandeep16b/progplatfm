using Hangfire.Dashboard;
using Microsoft.Owin;

namespace Abim.Platform.Program.Host.Config
{
    /// <summary>
    /// DashboardAuthorizationFilter
    /// </summary>
    public class DashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        /// <summary>
        /// Authorizes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public bool Authorize(DashboardContext context)
        {
            // In case you need an OWIN context, use the next line, `OwinContext` class
            // is the part of the `Microsoft.Owin` package.
            var owinContext = new OwinContext(context.GetOwinEnvironment());

            // Allow all authenticated users to see the Dashboard (potentially dangerous).
            //return owinContext.Authentication.User.Identity.IsAuthenticated;
            // temporary would allow everyone (before this we would need  //app.UseCookieAuthentication(...);)
            return true;
        }
    }
}
