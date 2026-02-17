using NLog;
using Owin;

namespace Abim.Platform.Program.Jobs.Config
{
    public partial class Startup
    {
        protected static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Configuration method.
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            Logger.Trace("Configuration Begin");

            UseIoc();
            UseErrorPage(app);
            UseServiceBus();
            UseOwinHangfire(app);

            Logger.Trace("Configuration End");
        }
    }
}
