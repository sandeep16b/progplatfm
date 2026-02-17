using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;
using Abim.Platform.Program.App.Domain;

namespace Abim.Platform.Program.App.Services.CommandResults
{
    /// <summary>
    /// AddCertificationCommandResult Class.
    /// </summary>
    public class AddCertificationCommandResult :CertificationCommandResult
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AddCertificationCommandResult(CommandStatus status, AbimValidationResult validation, Certification data)
            : base(status, validation, data)
        {
        }

        /// <summary>
        /// Parameterless Public Constructor
        /// </summary>
        public AddCertificationCommandResult()
        {
        }
    }
}
