namespace Abim.Platform.Program.Relational.Queries
{
    /// <summary>
    /// IQueryFactory interface
    /// </summary>
    public interface IQueryFactory
    {
        /// <summary>
        /// Creates the query.
        /// </summary>
        /// <typeparam name="TQuery">The type of the query.</typeparam>
        /// <returns></returns>
        TQuery CreateQuery<TQuery>() where TQuery : IQuery;
    }
}
