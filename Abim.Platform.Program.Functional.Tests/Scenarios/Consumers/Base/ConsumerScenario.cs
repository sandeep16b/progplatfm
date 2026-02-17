using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.WebApi.Testing.Setup;
using MassTransit;
using NLog;
using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.Tests.Scenarios.Consumers.Base
{
    public abstract class ConsumerScenario : BaseServiceScenario
    {
        protected override List<Type> AdditionalDependencies()
        {
            return new List<Type>()
            {
                typeof(IBusControl),
                typeof(ILogger),
               // typeof(IValidationFactory),
                typeof(ICredentialService),
                typeof(IAccessTokenService)
                //typeof(IProgramInterservice),
                //typeof(IProfileInterservice),
                //typeof(IRegistrationInterservice)
            };
        }
    }
}
