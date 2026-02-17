using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;
using Abim.Platform.Program.App.Domain;

namespace Abim.Platform.Program.App.Services.CommandResults
{
    /// <summary>
    /// UpdateCertificationCommandResult Class.
    /// </summary>
    public class UpdateCertificationCommandResult :CertificationCommandResult
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public UpdateCertificationCommandResult(CommandStatus status, AbimValidationResult validation, Certification data)
            : base(status, validation, data)
        {
        }

        /// <summary>
        /// Parameterless Public Constructor
        /// </summary>
        public UpdateCertificationCommandResult()
        {
        }
    }
}
