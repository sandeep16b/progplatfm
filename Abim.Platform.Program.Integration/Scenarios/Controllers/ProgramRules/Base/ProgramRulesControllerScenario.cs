using Abim.Platform.Program.Relational;
using Abim.Platform.Program.WebApi.Testing.Setup;
using Abim.Platform.Program.Host.Config;
using Owin;
using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.Integration.Scenarios.Controllers.ProgramRules.Base
{
    public abstract class ProgramRulesControllerScenario : BaseHttpServerScenario
    {
        protected override List<Type> AdditionalDependencies()
        {
            return new List<Type>();
        }

        protected override Action<IAppBuilder> UseStartup()
        {
            Action<IAppBuilder> action = (app) =>
            {
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
