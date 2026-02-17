using Abim.Platform.Program.WebApi.Testing.Setup;
using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.FunctionalTests.Scenarios.Services.ProgramRulesService.Base
{
    public abstract class ProgramRulesScenario : BaseServiceScenario
    {
        protected override List<Type> AdditionalDependencies()
        {
            return new List<Type>()
            {
                //typeof(IDeliverySystemService),
                //typeof(ISponsorService),
                //typeof(IProviderService),
                //typeof(IBusControl),
                //typeof(IProductFormatService),
                //typeof(IProductGroupService),
                //typeof(ICreditTypeService)
            };
        }
    }
}
