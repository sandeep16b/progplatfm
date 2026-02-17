using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;

namespace Abim.Platform.Program.App.Services.CommandResults
{
    /// <summary>
    /// AddAddedQualificationCertificationCommandResult Class.
    /// </summary>
    public class AddAddedQualificationCertificationCommandResult : CertificationCommandResult 
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AddAddedQualificationCertificationCommandResult(CommandStatus status, AbimValidationResult validation, Certification data)
            : base(status, validation, data)
        {
        }
        
        /// <summary>
        /// Parameterless Public Constructor
        /// </summary>
        public AddAddedQualificationCertificationCommandResult()
        {
        }
    }
}
