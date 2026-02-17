using Abim.Platform.Program.WebApi.Testing.Setup;
using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.Tests.Scenarios.Services.ProgramRulesServicePart2.Base
{
    public abstract class ProgramRulesServicePart2Scenario : BaseServiceScenario
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
