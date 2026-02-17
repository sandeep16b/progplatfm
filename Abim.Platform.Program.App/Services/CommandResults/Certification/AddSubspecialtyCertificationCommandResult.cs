using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;

namespace Abim.Platform.Program.App.Services.CommandResults
{
    /// <summary>
    /// AddSubspecialtyCertificationCommandResult Class.
    /// </summary>
    public class AddSubspecialtyCertificationCommandResult : CertificationCommandResult 
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AddSubspecialtyCertificationCommandResult(CommandStatus status, AbimValidationResult validation, Certification data)
            : base(status, validation, data)
        {
        }
        
        /// <summary>
        /// Parameterless Public Constructor
        /// </summary>
        public AddSubspecialtyCertificationCommandResult()
        {
        }
    }
}
