using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;

namespace Abim.Platform.Program.App.Services.CommandResults
{
    /// <summary>
    /// SourceCommandResult, a base CommandResult class for Source
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Relational.Validation.Impl.CommandResult{Source}" />
    public abstract class SourceCommandResult : CommandResult<Source>
    { 
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceCommandResult"/> class.
        /// </summary>
        protected SourceCommandResult() : base(CommandStatus.Accepted, null, null)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceCommandResult"/> class from explicit arguments.
        /// </summary>
        protected SourceCommandResult(CommandStatus status, AbimValidationResult validationResult, Source data)
            : base(status, validationResult, data)
        {
        }
    }
}
