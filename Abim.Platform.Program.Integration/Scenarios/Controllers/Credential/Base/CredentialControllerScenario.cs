using Abim.Enterprise.Core.Profile.Interservice.Interservices.Interfaces;
using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.Host.Config;
using Abim.Platform.Program.Relational;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.WebApi.Objects;
using Abim.Platform.Program.WebApi.Testing.Setup;
using Hangfire;
using MassTransit;
using NLog;
using Owin;
using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.Integration.Scenarios.Controllers.Credential.Base
{
    public abstract class CredentialControllerScenario : BaseHttpServerScenario
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
            list.Add(typeof(ICredentialService));
            list.Add(typeof(IHelperService));
            list.Add(typeof(IProfileInterservice));
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

                Container.Inject(My<IAccessTokenService>().Object);
                Container.Inject(My<IEnumService>().Object);

                Container.Inject(My<ILogger>().Object);
                Container.Inject(My<ICertificationService>().Object);
                Container.Inject(My<ISourceService>().Object);
                Container.Inject(My<IProgramRulesService>().Object);
                Container.Inject(My<IBackgroundJobClient>().Object);

                Container.Inject(My<IProfileInterservice>().Object);
            };
            return action;
        }
    }
}
