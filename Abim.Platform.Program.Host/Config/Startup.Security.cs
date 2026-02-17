using Abim.Platform.Program.Host.Classes;
using IdentityServer3.AccessTokenValidation;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace Abim.Platform.Program.Host.Config
{
    /// <summary>
    /// Startup class
    /// </summary>
    public partial class Startup
    {
        /// <summary>
        /// Uses the resource authorization.
        /// </summary>
        /// <param name="app">The application.</param>
        public static void UseResourceAuthorization(IAppBuilder app)
        {
            Logger.Trace("UseResourceAuthorization: Initializing.");

            app.UseResourceAuthorization(new ResourceAuthorization());

            Logger.Trace("UseResourceAuthorization: Complete.");
        }
        
        /// <summary>
        /// Sets the application to use the identity configuration.
        /// </summary>
        /// <param name="app">The application.</param>
        public static void UseIdentityClientConfig(IAppBuilder app)
        {
            Logger.Trace("UseIdentityClientConfig: Initializing.");

            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                Authority = ConfigurationManager.AppSettings["authority"],
                AuthenticationMode = AuthenticationMode.Active,
                ValidationMode = ValidationMode.Local,
                TokenProvider = new OAuthBearerAuthenticationProvider()
            });
            
            app.Use((context, continuation) =>
            {
                if (context.Authentication.User != null && context.Authentication.User.Identity != null && context.Authentication.User.Identity.IsAuthenticated)
                    return continuation();
                return continuation();
            });

            Logger.Trace("UseIdentityClientConfig: Complete.");
        }
    }
    
}