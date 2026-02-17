using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.Host.Config;
using Abim.Platform.Program.Relational;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.WebApi.Testing.Setup;
using Hangfire;
using MassTransit;
using Owin;
using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.Integration.Scenarios.Controllers.PhysicianCertification.Base
{
    public abstract class PhysicianCertificationControllerScenario : BaseHttpServerScenario
    {
        protected override List<Type> AdditionalDependencies()
        {
            var list = base.AdditionalDependencies();
            list.Add(typeof(ICredentialService));
            list.Add(typeof(ICredentialRepository));
            list.Add(typeof(IBusControl));
            list.Add(typeof(IBackgroundJobClient));
            list.Add(typeof(IValidationFactory));
            list.Add(typeof(ISourceService)); 
            list.Add(typeof(IHelperService));
            list.Add(typeof(IMembershipClientService));
            return list;
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
