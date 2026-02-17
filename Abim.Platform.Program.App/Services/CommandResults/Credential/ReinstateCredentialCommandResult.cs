using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;
using Abim.Platform.Program.App.Domain;

namespace Abim.Platform.Program.App.Services.CommandResults
{
    /// <summary>
    /// ReinstateCredentialCommandResult Class.
    /// </summary>
    public class ReinstateCredentialCommandResult : CredentialCommandResult
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ReinstateCredentialCommandResult(CommandStatus status, AbimValidationResult validation, Credential data)
            : base(status, validation, data)
        {
        }

        /// <summary>
        /// Parameterless Public Constructor
        /// </summary>
        public ReinstateCredentialCommandResult()
        {
        }
    }
}
