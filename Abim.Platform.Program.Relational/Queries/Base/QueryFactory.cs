namespace Abim.Platform.Program.Relational.Queries.Base
{
    /// <summary>
    /// QueryFactory class
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Relational.Queries.IQueryFactory" />
    public class QueryFactory : IQueryFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryFactory"/> class.
        /// </summary>
        public QueryFactory()
        {
        }

        /// <summary>
        /// Creates the query.
        /// </summary>
        /// <typeparam name="TQuery">The type of the query.</typeparam>
        /// <returns></returns>
        public TQuery CreateQuery<TQuery>() where TQuery : IQuery
        {
            return DependencyResolver.Container.GetInstance<TQuery>();
        }
    }
}
