namespace Abim.Platform.Program.Relational.Validation
{
    /// <summary>
    /// ICommandResult Interface.
    /// </summary>
    public interface ICommandResult
    {
        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        CommandStatus Status { get; }
    }

    /// <summary>
    /// ICommandResultWithErrors Interface.
    /// </summary>
    public interface ICommandResultWithErrors : ICommandResult
    {
        /// <summary>
        /// Adds a custom error message.
        /// </summary>
        /// <param name="text">The text.</param>
        void AddErrorMessage(string text);
    }

    /// <summary>
    /// Generic ICommandResult interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICommandResult<T> : ICommandResult
    {
        /// <summary>
        /// Gets or sets the object resulting from the Command.
        /// </summary>
        /// <value>
        /// The object.
        /// </value>
        T Object { get; set; }
    }

    /// <summary>
    /// CommandStatus Enumeration
    /// </summary>
    public enum CommandStatus
    {
        /// <summary>
        /// accepted
        /// </summary>
        Accepted = 1,

        /// <summary>
        /// rejected
        /// </summary>
        Rejected = 2
    }
}
