using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;

namespace Abim.Platform.Program.App.Services.CommandResults
{
    /// <summary>
    /// CredentialCommandResult, a base CommandResult class for Credential
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Relational.Validation.Impl.CommandResult{Credential}" />
    public abstract class CredentialCommandResult : CommandResult<Credential>
    { 
        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialCommandResult"/> class.
        /// </summary>
        protected CredentialCommandResult() : base(CommandStatus.Accepted, null, null)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialCommandResult"/> class from explicit arguments.
        /// </summary>
        protected CredentialCommandResult(CommandStatus status, AbimValidationResult validationResult, Credential data)
            : base(status, validationResult, data)
        {
        }
    }
}
