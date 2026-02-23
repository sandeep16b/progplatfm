using Abim.Enterprise.Core.Profile.Interservice.Interservices.Interfaces;
using Abim.Enterprise.Core.Registration.Interservice;
using Abim.Platform.Product.Interservices.Interfaces;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.WebApi.Testing.Setup;
using Hangfire;
using MassTransit;
using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.Tests.Scenarios.Services.ProgramRulesServiceInternalFunctions.Base
{
    public abstract class ProgramRulesServiceInternalFunctionsScenario : BaseServiceScenario
    {
        protected override List<Type> AdditionalDependencies()
        {
            return new List<Type>()
            {
                /*Dependencies*/
                typeof(ICertificationService),
                typeof(ICredentialService),
                typeof(ISourceService),
                typeof(IProductInterservice),
                typeof(IRegistrationInterservice),
                typeof(IBusControl),
                typeof(IBackgroundJobClient),
                typeof(IValidationFactory),
                typeof(IAccessTokenService),
                typeof(ICorrectiveActionResultService),
                typeof(IProfileInterservice),
                typeof(ILookBackDatesInfoService),
                typeof(ILookbackLogService)
            };
        }
    }
}
