using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;

namespace Abim.Platform.Program.App.Services.CommandResults
{
    /// <summary>
    /// IssuanceCommandResult, a base CommandResult class for Issuance
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Relational.Validation.Impl.CommandResult{Issuance}" />
    public abstract class IssuanceCommandResult : CommandResult<Issuance>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IssuanceCommandResult"/> class.
        /// </summary>
        protected IssuanceCommandResult() : base(CommandStatus.Accepted, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IssuanceCommandResult"/> class from explicit arguments.
        /// </summary>
        protected IssuanceCommandResult(CommandStatus status, AbimValidationResult validationResult, Issuance data)
            : base(status, validationResult, data)
        {
        }
    }
}
