using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;
using Abim.Platform.Program.App.Domain;

namespace Abim.Platform.Program.App.Services.CommandResults
{
    /// <summary>
    /// UpdateIssuanceCommandResult Class.
    /// </summary>
    public class UpdateIssuanceCommandResult :IssuanceCommandResult
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public UpdateIssuanceCommandResult(CommandStatus status, AbimValidationResult validation, Issuance data)
            : base(status, validation, data)
        {
        }

        /// <summary>
        /// Parameterless Public Constructor
        /// </summary>
        public UpdateIssuanceCommandResult()
        {
        }
    }
}
