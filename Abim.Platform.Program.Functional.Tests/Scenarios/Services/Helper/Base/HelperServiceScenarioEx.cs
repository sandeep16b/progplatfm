using Abim.Platform.Program.WebApi.Testing.Setup;
using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.Tests.Scenarios.Services.SourceService.Base
{
    public abstract class HelperServiceScenarioEx : BaseServiceScenario
    {
        protected override List<Type> AdditionalDependencies()
        {
            return new List<Type>()
            {
                /*Dependencies*/
            };
        }
    }
}