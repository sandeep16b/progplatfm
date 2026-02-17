using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;
using Abim.Platform.Program.App.Domain;

namespace Abim.Platform.Program.App.Services.CommandResults
{
    /// <summary>
    /// UpdateSelectedToMaintainCommandResult Class.
    /// </summary>
    public class UpdateSelectedToMaintainCommandResult : CredentialCommandResult
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public UpdateSelectedToMaintainCommandResult(CommandStatus status, AbimValidationResult validation, Credential data)
            : base(status, validation, data)
        {
        }

        /// <summary>
        /// Parameterless Public Constructor
        /// </summary>
        public UpdateSelectedToMaintainCommandResult()
        {
        }
    }
}
