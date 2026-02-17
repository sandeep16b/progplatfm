using Abim.Platform.Program.Relational.Queries.Base;
using NHibernate;

namespace Abim.Platform.Program.Relational.Queries
{
    /// <summary>
    /// The NamedQueryBase class
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <seealso cref="Abim.Platform.Program.Relational.Queries.Base.QueryBase{TResult}" />
    /// <seealso cref="Abim.Platform.Program.Relational.Queries.INamedQuery" />
    public abstract class NamedQueryBase<TResult> : 
        QueryBase<TResult>, INamedQuery
    {
        /// <summary>
        /// Gets or sets the name of the query.
        /// </summary>
        /// <value>
        /// The name of the query.
        /// </value>
        string INamedQuery.QueryName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedQueryBase{TResult}"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public NamedQueryBase(ISession session) : base(session)
        {

        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns></returns>
        public override TResult Execute()
        {
            var query = GetNamedQuery();
            return Transact(() => Execute(query));
        }

        /// <summary>
        /// Executes the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        protected abstract TResult Execute(NHibernate.IQuery query);

        /// <summary>
        /// Gets the named query.
        /// </summary>
        /// <returns></returns>
        private NHibernate.IQuery GetNamedQuery()
        {
            var query = Session.GetNamedQuery(((INamedQuery)this).QueryName);
            SetParameters(query);
            return query;
        }

        /// <summary>
        /// Sets the parameters.
        /// </summary>
        /// <param name="query">The query.</param>
        protected abstract void SetParameters(NHibernate.IQuery query);
    }
}
