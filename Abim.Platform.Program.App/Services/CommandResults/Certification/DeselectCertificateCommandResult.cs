using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;

namespace Abim.Platform.Program.App.Services.CommandResults
{
    /// <summary>
    /// Command result for DeselectCertificateCommand
    /// </summary>
    public class DeselectCertificateCommandResult : CredentialCommandResult
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="status">The status of the command</param>
        /// <param name="validation">The validation result of the command</param>
        /// <param name="data">The Credential for the Certificate which was de-selected</param>
        public DeselectCertificateCommandResult(CommandStatus status, AbimValidationResult validation, Credential data) : base(status, validation, data)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public DeselectCertificateCommandResult()
        {
        }
    }
}
