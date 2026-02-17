namespace Abim.Platform.Program.Relational.Queries
{
    /// <summary>
    /// IQuery interface
    /// </summary>
    public interface IQuery
    {
    }

    /// <summary>
    /// Generic IQuery interface
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface IQuery<TResult> : IQuery
    {
        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns></returns>
        TResult Execute();
    }
}
