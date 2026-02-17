using Abim.Platform.Program.Relational.Validation.Impl;
using Abim.Platform.Program.Relational.Validation;

namespace Abim.Platform.Program.App.Services.CommandResults.CredentialDateLog
{
    /// <summary>
    /// A class representing the result of an attempt to add a credential date log entry
    /// </summary>
    public class AddCredentialDateLogResult : CommandResult<App.Domain.CredentialDateLog>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddCredentialDateLogResult"/> class.
        /// </summary>
        public AddCredentialDateLogResult() : base(CommandStatus.Accepted, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddCredentialDateLogResult"/> class from explicit arguments.
        /// </summary>
        protected AddCredentialDateLogResult(CommandStatus status, AbimValidationResult validationResult, App.Domain.CredentialDateLog data)
            : base(status, validationResult, data)
        {
        }
    }
}
