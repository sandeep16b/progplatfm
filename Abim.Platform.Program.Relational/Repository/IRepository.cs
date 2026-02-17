using Abim.Platform.Program.Relational.Domain;
using Abim.Platform.Program.Relational.Queries;
using Abim.Platform.Program.Relational.Validation.Impl;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Abim.Platform.Program.Relational.Repository
{
    /// <summary>
    /// Interface for NHibernate repositories
    /// </summary>
    public interface IRepository<TAggregateRoot, in TKey>:
        IEnumerable<TAggregateRoot>,
        IQueryFactory,
        IDisposable
        where TAggregateRoot : IAggregateRoot, IEntity<TKey>
    {
        /// <summary>
        /// Gets or sets a value indicating whether to commit each call in its own transaction. This is largely for
        /// backwards-compatibility with existing servies and repositories which work this way
        /// </summary>
        /// <value>
        /// <c>true</c> if [commit each call in its own transaction]; otherwise, <c>false</c>.
        /// </value>
        bool CommitEachCallInItsOwnTransaction { get; }

        /// <summary>
        /// Loads the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        TAggregateRoot Load(TKey id);
        
        /// <summary>
        /// Loads the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        TAggregateRoot Load(Guid id);
        
        /// <summary>
        /// GetAll method.
        /// </summary>
        /// <returns></returns>
        IEnumerable<TAggregateRoot> GetAll();
        
        /// <summary>
        /// Adds the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        AbimValidationResult Add(TAggregateRoot obj);
        
        /// <summary>
        /// Adds the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="createdBy">The user doing the Add</param>
        /// <returns></returns>
        AbimValidationResult Add(TAggregateRoot obj, string createdBy);
        
        /// <summary>
        /// Adds many.
        /// </summary>
        /// <param name="objs">The objs.</param>
        /// <returns></returns>
        AbimValidationResult AddMany(IEnumerable<TAggregateRoot> objs);
        
        /// <summary>
        /// Adds many.
        /// </summary>
        /// <param name="objs">The objs.</param>
        /// <param name="createdBy">The user doing the Add</param>
        /// <returns></returns>
        AbimValidationResult AddMany(IEnumerable<TAggregateRoot> objs, string createdBy);
        
        /// <summary>
        /// Updates the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        AbimValidationResult Update(TAggregateRoot obj);
        
        /// <summary>
        /// Updates the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        AbimValidationResult Update(TAggregateRoot obj, string modifiedBy);
        
        /// <summary>
        /// Updates the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="modifiedBy">The username of the user making the modification</param>
        /// <returns></returns>
        void Update(Func<TAggregateRoot, bool> query, string modifiedBy);
        
        /// <summary>
        /// Updates the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="modifiedBy">The username of the user making the modification</param>
        void Update(Func<TAggregateRoot, bool> query);
        
        /// <summary>
        /// Deletes the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        void Delete(TAggregateRoot obj);
        
        /// <summary>
        /// Deletes the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        void Delete(Func<TAggregateRoot, bool> query);
        
        /// <summary>
        /// Checks if any of the criteria exist
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        bool Exists(Func<TAggregateRoot, bool> criteria);
        
        /// <summary>
        /// Checks if an identifier exists
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        bool Exists(TKey id);
        
        /// <summary>
        /// Checks if an identifier exists
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        bool Exists(Guid id);
        
        /// <summary>
        /// Queries the specified function.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        IEnumerable<TAggregateRoot> QueryOn(Func<TAggregateRoot, bool> query);
        
        /// <summary>
        /// Queries the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        IEnumerable<TAggregateRoot> Query(Expression<Func<TAggregateRoot, bool>> filterExpression);
        
        /// <summary>
        /// Queries the specified query.
        /// </summary>
        /// <param name="paging">The paging and sorting query.</param>
        /// <param name="totalCount">Out parameter of the total records found</param>
        /// <returns></returns>
        IEnumerable<TAggregateRoot> Query(PageDefinition paging, out int totalCount);
        
        /// <summary>
        /// Queries the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="totalCount">Out parameter of the total records found</param>
        /// <returns></returns>
        IEnumerable<TAggregateRoot> Query(ComplexQueryBase query, out int totalCount);
        
        /// <summary>
        /// Queries the specified query. Protected because no-one should be making ICriteria except for repositories
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="totalCount">Out parameter of the total records found</param>
        /// <returns></returns>
        IEnumerable<TAggregateRoot> Query(Expression<Func<TAggregateRoot, bool>> filterExpression,PageDefinition query, out int totalCount);
        
        /// <summary>
        /// Counts objects which satisfy the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        int Count(Func<TAggregateRoot, bool> query);

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        void CommitTransaction();

        /// <summary>
        /// Rolls back the transaction.
        /// </summary>
        void RollbackTransaction();
    }
}
