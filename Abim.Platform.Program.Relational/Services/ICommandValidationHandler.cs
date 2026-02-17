using Abim.Platform.Program.Relational.Validation.Impl;

namespace Abim.Platform.Program.Relational.Services
{
    /// <summary>
    /// Validates a specific command
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    public interface ICommandValidationHandler<in TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// Validates the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        AbimValidationResult Validate(TCommand command);
    }
}
