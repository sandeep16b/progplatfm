using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;

namespace Abim.Platform.Program.App.Services.CommandResults
{
    /// <summary>
    /// ExpireIssuanceCommandResult Class.
    /// </summary>
    public class ExpireIssuanceCommandResult : CredentialCommandResult 
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ExpireIssuanceCommandResult(CommandStatus status, AbimValidationResult validation, Credential data)
            : base(status, validation, data)
        {
        }
        
        /// <summary>
        /// Parameterless Public Constructor
        /// </summary>
        public ExpireIssuanceCommandResult()
        {
        }
    }
}
