using System.Threading.Tasks;
using Abim.Platform.Program.Relational.Validation;

namespace Abim.Platform.Program.Relational.Services
{
    /// <summary>
    /// A command handler that returns ICommandResult without a particular concrete type
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        ICommandResult Handle(TCommand command);
    }

    /// <summary>
    /// A command handler that returns a certain concrete command result type
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <typeparam name="TCommandResult">The type of the command result.</typeparam>
    public interface ICommandHandler<in TCommand, out TCommandResult> 
        where TCommand : ICommand where TCommandResult : ICommandResult
    {
        TCommandResult Handle(TCommand command);
    }

    /// <summary>
    /// An async command handler that returns ICommandResult without a particular concrete type
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    public interface IAsyncCommandHandler<in TCommand> where TCommand : ICommand
    {
        Task<ICommandResult> Handle(TCommand command);
    }

    /// <summary>
    /// An async command handler that returns a certain concrete command result type
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    /// <typeparam name="TCommandResult">The type of the command result.</typeparam>
    public interface IAsyncCommandHandler<in TCommand, TCommandResult> 
        where TCommand : ICommand where TCommandResult : ICommandResult
    {
        Task<TCommandResult> Handle(TCommand command);
    }
}
