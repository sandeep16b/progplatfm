using Abim.Platform.Program.WebApi.Testing.Setup;
using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.Tests.Scenarios.Services.EnumService.Base
{
    public abstract class EnumServiceScenario : BaseServiceScenario
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
