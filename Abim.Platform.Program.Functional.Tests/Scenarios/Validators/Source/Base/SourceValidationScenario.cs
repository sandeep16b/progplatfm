using Abim.Platform.Program.WebApi.Testing.Setup;
using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.Tests.Scenarios.Validators.Source.Base
{
    public abstract class SourceValidationScenario : BaseValidationScenario
    {
        protected override List<Type> AdditionalDependencies()
        {
            return new List<Type>();
        }
    }
}
