using Abim.Platform.Program.Relational.Validation.Impl;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Validation;

namespace Abim.Platform.Program.App.Services.CommandResults
{
    /// <summary>
    /// 
    /// </summary>
    public class AddLookbackLogCommandResult : CommandResult<LookbackLog>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddLookbackLogCommandResult"/> class.
        /// </summary>
        public AddLookbackLogCommandResult() : base(CommandStatus.Accepted, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CertificationCommandResult"/> class from explicit arguments.
        /// </summary>
        protected AddLookbackLogCommandResult(CommandStatus status, AbimValidationResult validationResult, LookbackLog data)
            : base(status, validationResult, data)
        {
        }
    }
}
