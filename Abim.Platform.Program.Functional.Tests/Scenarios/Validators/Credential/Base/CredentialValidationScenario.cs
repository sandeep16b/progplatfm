using Abim.Platform.Program.WebApi.Testing.Setup;
using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.Tests.Scenarios.Validators.Credential.Base
{
    public abstract class CredentialValidationScenario : BaseValidationScenario
    {
        protected override List<Type> AdditionalDependencies()
        {
            return new List<Type>();
        }
    }
}
