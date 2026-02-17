using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;

namespace Abim.Platform.Program.App.Services.CommandResults
{
    /// <summary>
    /// UpdateCredentialGracePeriodForTLandMBMCommandResult Class.
    /// </summary>
    public class UpdateCredentialGracePeriodTLandMBMCommandResult : CredentialCommandResult
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public UpdateCredentialGracePeriodTLandMBMCommandResult(CommandStatus status, AbimValidationResult validation, Credential data)
            : base(status, validation, data)
        {
        }

        /// <summary>
        /// Parameterless Public Constructor
        /// </summary>
        public UpdateCredentialGracePeriodTLandMBMCommandResult()
        {
        }
    }
}
