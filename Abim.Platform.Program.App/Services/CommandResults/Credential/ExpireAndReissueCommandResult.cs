using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;

namespace Abim.Platform.Program.App.Services.CommandResults
{
    /// <summary>
    /// ExpireAndReissueCommandResult Class.
    /// </summary>
    public class ExpireAndReissueCommandResult : CredentialCommandResult
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ExpireAndReissueCommandResult(CommandStatus status, AbimValidationResult validation, Credential data)
            : base(status, validation, data)
        {
        }

        /// <summary>
        /// Parameterless Public Constructor
        /// </summary>
        public ExpireAndReissueCommandResult()
        {
        }
    }
}
