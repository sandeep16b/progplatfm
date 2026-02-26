using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using NLog;
using Owin;

namespace Abim.Platform.Program.Host.Config
{
    /// <summary>
    /// Startup Class.
    /// </summary>
    public partial class Startup
    { 
        /// <summary>
        /// A development flag. Will be false in production 
        ///  private static readonly bool IsDevelopment; 
        /// The logger
        /// </summary>
        protected static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        
        static Startup()
        {
            #if DEBUG
             //   IsDevelopment = true;
            #endif
        }

        /// <summary>
        /// Configuration method.
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            // removed per PBI 174832 (2.26) Reverse TLS 1.2 code changes in Program Platform
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Logger.Trace("Configuration Begin");
            
            UseIoc();
            UseIdentityClientConfig(app);
            UseResourceAuthorization(app);
            UseHttpConfig(app);
            UseServiceBus();
            UseFileServer(app);
            UseWelcomePage(app);
            UseErrorPage(app);
            UseMetrics(app);
            UseMappings();
            ConfigurationHealthyCheck(app);
            //ar@7/27/2018 Commented out by PBI:129655:Disable NCache in Program Platform (2.7)
            // NCachePreLoad();
        }
        /// <summary>
        /// UseFileServer
        /// </summary>
        /// <param name="app"></param>
        protected static void UseFileServer(IAppBuilder app)
        {
            app.UseFileServer(new FileServerOptions
            {
                FileSystem = new PhysicalFileSystem("assets"),
                RequestPath = new PathString("")
            });
        }
    }
}
