namespace Abim.Platform.Program.Relational.Validation.Impl
{
    /// <summary>
    /// ObjectResult class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectResult<T> : CommandResult, ICommandResult<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectResult{T}"/> class.
        /// </summary>
        /// <param name="t">The t.</param>
        public ObjectResult(T t)
        {
            Object = t;
        }

        /// <summary>
        /// Gets or sets the object resulting from the Command.
        /// </summary>
        /// <value>
        /// The object.
        /// </value>
        public T Object { get; set; }
    }
}
