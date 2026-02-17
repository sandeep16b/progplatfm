using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;

namespace Abim.Platform.Program.App.Services.CommandResults
{
    /// <summary>
    /// CreateCredentialIssuanceCommandResult Class.
    /// </summary>
    public class CreateCredentialIssuanceCommandResult : CredentialCommandResult
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CreateCredentialIssuanceCommandResult(CommandStatus status, AbimValidationResult validation, Credential data)
            : base(status, validation, data)
        {
        }

        /// <summary>
        /// Parameterless Public Constructor
        /// </summary>
        public CreateCredentialIssuanceCommandResult()
        {
        }
    }
}
