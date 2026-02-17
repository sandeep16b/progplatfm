using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;

namespace Abim.Platform.Program.App.Services.CommandResults.LookbackDateLog
{
    /// <summary>
    /// AddLookbackDateLogCommandResult
    /// </summary>
    public class AddLookbackDateLogCommandResult: CommandResult<Domain.LookbackDateLog>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddLookbackDateLogCommandResult"/> class.
        /// </summary>
        public AddLookbackDateLogCommandResult() : base(CommandStatus.Accepted, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddLookbackDateLogCommandResult"/> class from explicit arguments.
        /// </summary>
        protected AddLookbackDateLogCommandResult(CommandStatus status, AbimValidationResult validationResult, App.Domain.LookbackDateLog data)
            : base(status, validationResult, data)
        {
        }
        
    }
}
