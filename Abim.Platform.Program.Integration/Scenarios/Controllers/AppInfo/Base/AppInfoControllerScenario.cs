using Abim.Platform.Program.Relational;
using Abim.Platform.Program.WebApi.Config;
using Abim.Platform.Program.WebApi.Testing.Setup;
using Abim.Platform.Program.Host.Config;
using Owin;
using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.Integration.Scenarios.Controllers.AppInfo.Base
{
    public abstract class AppInfoControllerScenario : BaseHttpServerScenario
    {
        protected override List<Type> AdditionalDependencies()
        {
            return new List<Type>();
        }

        protected override Action<IAppBuilder> UseStartup()
        {
            Action<IAppBuilder> action = (app) =>
            {
                HostProperties.Status = System.Configuration.ConfigurationManager.AppSettings["RunningStatusText"];
                Startup.Container = DependencyResolver.Container;
                Startup.UseIdentityClientConfig(app);
                Startup.UseResourceAuthorization(app);
                Startup.UseHttpConfig(app);
                Startup.UseMappings();
            };
            return action;
        }
    }
}
