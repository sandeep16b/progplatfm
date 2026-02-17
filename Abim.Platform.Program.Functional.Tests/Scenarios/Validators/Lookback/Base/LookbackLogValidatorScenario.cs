using Abim.Platform.Program.WebApi.Testing.Setup;
using FluentValidation.Results;

namespace Abim.Platform.Program.Tests.Scenarios.Validators.Lookback
{
    /// <summary>
    /// LookbackLogValidationScenario Class.
    /// </summary>
    public abstract class LookbackLogValidatorScenario : BaseValidationScenario
    {
        protected ValidationResult ValidationResult { get; set; }
        protected abstract string ExpectedErrorMessage();
    }
}
