using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;
using Abim.Platform.Program.App.Domain;

namespace Abim.Platform.Program.App.Services.CommandResults
{
    /// <summary>
    /// UpdateGrandfatherMOCPrintDateCommandResult Class.
    /// </summary>
    public class UpdateGrandfatherMOCPrintDateCommandResult : CredentialCommandResult
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public UpdateGrandfatherMOCPrintDateCommandResult(CommandStatus status, AbimValidationResult validation, Credential data)
            : base(status, validation, data)
        {
        }

        /// <summary>
        /// Parameterless Public Constructor
        /// </summary>
        public UpdateGrandfatherMOCPrintDateCommandResult()
        {
        }
    }
}
