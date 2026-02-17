using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;

namespace Abim.Platform.Program.App.Services.CommandResults
{
    /// <summary>
    /// AddPrimaryCertificationCommandResult Class.
    /// </summary>
    public class AddPrimaryCertificationCommandResult : CertificationCommandResult 
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AddPrimaryCertificationCommandResult(CommandStatus status, AbimValidationResult validation, Certification data)
            : base(status, validation, data)
        {
        }
        
        /// <summary>
        /// Parameterless Public Constructor
        /// </summary>
        public AddPrimaryCertificationCommandResult()
        {
        }
    }
}
