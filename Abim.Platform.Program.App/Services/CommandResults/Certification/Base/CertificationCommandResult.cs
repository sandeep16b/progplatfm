using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;

namespace Abim.Platform.Program.App.Services.CommandResults
{
    /// <summary>
    /// CertificationCommandResult, a base CommandResult class for Certification
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Relational.Validation.Impl.CommandResult{Certification}" />
    public abstract class CertificationCommandResult : CommandResult<Certification>
    { 
        /// <summary>
        /// Initializes a new instance of the <see cref="CertificationCommandResult"/> class.
        /// </summary>
        protected CertificationCommandResult() : base(CommandStatus.Accepted, null, null)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CertificationCommandResult"/> class from explicit arguments.
        /// </summary>
        protected CertificationCommandResult(CommandStatus status, AbimValidationResult validationResult, Certification data)
            : base(status, validationResult, data)
        {
        }
    }
}
