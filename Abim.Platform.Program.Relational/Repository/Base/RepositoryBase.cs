using Abim.Platform.Program.Relational.Domain;
using Abim.Platform.Program.Relational.Domain.Types;
using Abim.Platform.Program.Relational.Queries;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.Impl;
using NHibernate.Linq;
using NHibernate.Loader.Criteria;
using NHibernate.Persister.Entity;
using NHibernate.Transform;
using NHibernate.Util;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Abim.Platform.Program.Relational.Repository.Base
{
    /// <summary>
    /// Base class for NHibernate repositories
    /// </summary>
    /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <seealso cref="Abim.Platform.Program.Relational.Repository.IRepository{TAggregateRoot,TKey}" />
    public abstract class RepositoryBase<TAggregateRoot, TKey> : IRepository<TAggregateRoot, TKey>
        where TAggregateRoot : class, IAggregateRoot, IEntity<TKey>, IDomainValidationHandler<TAggregateRoot>
    {
        #region Fields

        /// <summary>
        /// The log
        /// </summary>
        private readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The disposing flag
        /// </summary>
        protected internal bool Disposing;

        /// <summary>
        /// The disposed flag
        /// </summary>
        protected internal bool Disposed;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        protected ISession Session { get; private set; }

        /// <summary>
        /// The transaction
        /// </summary>
        protected ITransaction Transaction { get; private set; }

        /// <summary>
        /// Gets the IValidationFactory.
        /// </summary>
        /// <value>
        /// The IValidationFactory.
        /// </value>
        protected IValidationFactory Validation { get; private set; }

        /// <summary>
        /// Gets or sets the query factory.
        /// </summary>
        /// <value>
        /// The query factory.
        /// </value>
        protected IQueryFactory QueryFactory { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to commit each call in its own transaction. This is largely for
        /// backwards-compatibility with existing servies and repositories which work this way, which is also why
        /// it defaults to true
        /// </summary>
        /// <value>
        /// <c>true</c> if [commit each call in its own transaction]; otherwise, <c>false</c>.
        /// </value>
        public bool CommitEachCallInItsOwnTransaction { get; set; }

        /// <summary>
        /// Gets the log.
        /// </summary>
        /// <value>
        /// The log.
        /// </value>
        protected ILogger Log { get { return Logger; } }

        /// <summary>
        /// Gets a value indicating whether this instance has already been disposed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is disposed; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisposed { get { return Disposed; } }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TAggregateRoot, TKey}"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="query">The query.</param>
        /// <param name="validationFactory">The validation factory.</param>
        /// <exception cref="System.ArgumentNullException">
        /// session
        /// or
        /// validationFactory
        /// </exception>
        protected RepositoryBase(ISession session, IQueryFactory query, IValidationFactory validationFactory)
        {
            if(session == null)
                throw new ArgumentNullException("session");
            if(validationFactory == null)
                throw new ArgumentNullException("validationFactory");
            Session = session;
            Validation = validationFactory;
            QueryFactory = query;
            CommitEachCallInItsOwnTransaction = true;
        }

        #endregion

        #region Transaction Support

        /// <summary>
        /// Begins the transaction
        /// </summary>
        public void BeginTransaction()
        {
            Transaction = Session.BeginTransaction();
        }

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public void CommitTransaction()
        {
            if(Session != null)
                Session.Flush();
            if (Transaction != null)
                Transaction.Commit();
            //Transaction will be replaced with a new transaction by NHibernate, but we will close it to keep a consistent state.
            CloseTransaction();
        }

        /// <summary>
        /// Rolls back the Transaction
        /// </summary>
        public void RollbackTransaction()
        {
            RollbackTransaction(Transaction);
        }

        /// <summary>
        /// Rolls back a transaction
        /// </summary>
        /// <param name="tx">The transaction.</param>
        protected void RollbackTransaction(ITransaction tx)
        {
            //The Session must be closed and disposed after a transaction rollback to keep a consistent state. 
            if (tx != null) tx.Rollback();  
            CloseTransaction(tx);
            CloseSession(true);
        }

        /// <summary>
        /// Closes the Transaction.
        /// </summary>
        private void CloseTransaction()
        {
            CloseTransaction(Transaction);
        }

        /// <summary>
        /// Closes a transaction.
        /// </summary>
        /// <param name="tx">The transaction.</param>
        protected void CloseTransaction(ITransaction tx)
        {
            if(tx == null) return;
            try
            {
                tx.Dispose();
            }
            catch(ObjectDisposedException ex)
            {
                //if it's already been disposed, we don't care, however unlike the session this is not expected
                Log.Warn("Transaction was already disposed", ex);
            }
            #pragma warning disable 0168
            catch(Exception ex)
            {
                throw;
            }
            finally
            {
                Transaction = null;
            }
        }

        /// <summary>
        /// Closes the session.
        /// </summary>
        /// <param name="createNewSessionAfterward">if set to <c>true</c> [create new session afterward].</param>
        private void CloseSession(bool createNewSessionAfterward)
        {
            if(Session == null) return;

            //Close()
            try
            {
                Session.Close();
            }
            catch(SessionException ex)
            {
                //if it's already been closed, we don't care. This can happen because repositories share a Session. Log a debug anyway though
                Log.Debug("Session was already closed", ex);
            }
            catch(Exception ex)
            {
                throw;
            }

            //Dispose()
            try
            {
                Session.Dispose();
            }
            catch(ObjectDisposedException ex)
            {
                //if it's already been disposed, we don't care. This can happen because repositories share a Session. Log a debug anyway though
                Log.Debug("Session was already disposed", ex);
            }
            catch(Exception ex)
            {
                throw;
            }
            finally
            {
                Session = null;
            }

            if(createNewSessionAfterward)
            {
                Session = DependencyResolver.Container.GetInstance<ISession>();
                Transaction = Session.BeginTransaction();
            }
        }

        #endregion

        #region Disposal

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <remarks>
        /// See https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/dispose-pattern
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <remarks>
        /// See https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/dispose-pattern
        /// </remarks>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            //Dispose() in RepositoryBase can be hit multiple times concurrently, hence the Disposing flag
            if(Disposing || Disposed) return;
            Disposing = true;
            
            if(disposing) FreeManagedResources();
            
            //Free any unmanaged resources here
            
            Disposed = true;
            Disposing = false;
        }
        
        /// <summary>
        /// Frees the managed resources.
        /// </summary
        /// <returns>
        /// void.
        /// </returns>
        private void FreeManagedResources()
        {
            //Commit the last transaction by default
            try
            {
                if(Transaction != null) CommitTransaction();
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }
            
            //Now close the transaction and the session
            try
            {
                CloseTransaction();
                CloseSession(false);
            }
            catch(Exception ex)
            {
                Log.Error(ex);
            }
        }
        
        /// <summary>
        /// Finalizes an instance of the <see cref="RepositoryBase{TAggregateRoot, TKey}"/> class.
        /// </summary>
        ~RepositoryBase()
        {
            Dispose(false);
        }

        #endregion

        #region Implementation

        #region Get

        /// <summary>
        /// Loads the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public virtual TAggregateRoot Load(TKey id)
        {
            return Session.Load<TAggregateRoot>(id);
        }

        /// <summary>
        /// Loads the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public virtual TAggregateRoot Load(Guid id)
        {
            return Session.QueryOver<TAggregateRoot>()
                            .Where(_ => _.ExternalId == id)
                            .Cacheable().CacheMode(CacheMode.Normal)
                            .SingleOrDefault();
        }

        /// <summary>
        /// GetAll method.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TAggregateRoot> GetAllFromSession()
        {
            ISessionImplementor impl = Session.GetSessionImplementation();
            IPersistenceContext pc = impl.PersistenceContext;

            foreach(Object key in pc.EntityEntries.Keys)
            {
                if(key is TAggregateRoot)
                {
                    yield return ((TAggregateRoot)key);
                }
            }
        }

        /// <summary>
        /// GetAll method.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TAggregateRoot> GetAll()
        {
            return Session.Query<TAggregateRoot>()
                   .Cacheable().CacheMode<TAggregateRoot>(CacheMode.Normal)
                   .ToList();
        }

        /// <summary>
        /// Queries the specified query.
        /// </summary>
        /// <remarks>
        /// Protected because it returns IQueryable
        /// </remarks>
        /// <returns></returns>
        protected virtual IQueryable<TAggregateRoot> Query()
        {
            return Session.Query<TAggregateRoot>();
        }

        /// <summary>
        /// Queries the specified function.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public virtual IEnumerable<TAggregateRoot> QueryOn(Func<TAggregateRoot, bool> query)
        {
            return Query(x => query(x));
        }

        /// <summary>
        /// Queries the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public virtual IEnumerable<TAggregateRoot> Query(Expression<Func<TAggregateRoot, bool>> query)
        {
            var aggregateQuery = Session.QueryOver<TAggregateRoot>().TransformUsing(Transformers.DistinctRootEntity).Where(query)
                                .Cacheable().CacheMode(CacheMode.Normal);
            return aggregateQuery.Future<TAggregateRoot>();
        }

        /// <summary>
        /// Queries the specified query.
        /// </summary>
        /// <param name="paging">The paging and sorting query.</param>
        /// <param name="totalCount">Out parameter of the total records found</param>
        /// <returns></returns>
        public virtual IEnumerable<TAggregateRoot> Query(PageDefinition paging, out int totalCount)
        {
            return Query(new PagedQuery(paging), out totalCount);
        }

        /// <summary>
        /// Queries the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="totalCount">Out parameter of the total records found</param>
        /// <returns></returns>
        public virtual IEnumerable<TAggregateRoot> Query(ComplexQueryBase query, out int totalCount)
        {
            return Query(query, out totalCount, null);
        }

        /// <summary>
        /// Queries the specified query. Protected because no-one should be making ICriteria except for repositories
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="totalCount">Out parameter of the total records found</param>
        /// <returns></returns>
        protected virtual IEnumerable<TAggregateRoot> Query(ComplexQueryBase query, out int totalCount, ICriteria criteria = null)
        {
            //the caller can add filtering to criteria and pass it in, e.g. criteria.Add(Restrictions.Eq(search.Key, search.Value))
            //previously this was done here, when there was a SearchFields property in ComplexQueryBase
            criteria = criteria ?? Session.CreateCriteria<TAggregateRoot>().SetCacheable(true).SetCacheMode(CacheMode.Normal);
            
            //Error Checking (do this first, to save time if there is one)
            List<string> errorMessages;
            query.Validate(out errorMessages);
            if(errorMessages.Any())
                throw new Exception("Error(s) found in Query: " + string.Join("; ", errorMessages.ToArray()));
            
            //Total Count
            totalCount = ((ICriteria)criteria.Clone()).SetProjection(Projections.RowCount())
                         .SetCacheable(true).SetCacheMode(CacheMode.Normal)
                         .FutureValue<Int32>().Value;
            
            //Sorting
            CorrectSortCasing(query.PageDefinition.Sorts);
            foreach(var sort in query.PageDefinition.SortsOrDefault())
            {
                var order = sort.SortDirection == SortDirection.Ascending ? Order.Asc(sort.SortBy) : Order.Desc(sort.SortBy);
                criteria = criteria.AddOrder(order);
            }
            
            //Paging
            int skip = query.PageDefinition.SkippedItems;
            int take = query.PageDefinition.PageSize;
            if(skip >= totalCount)
                return new List<TAggregateRoot>();
            criteria = criteria
                    .SetFirstResult(skip)
                    .SetMaxResults(take);
            
            return criteria.SetCacheable(true).SetCacheMode(CacheMode.Normal).Future<TAggregateRoot>().ToList();
        }

        /// <summary>
        /// Queries based on an expression
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="paging">The paging and sorting information.</param>
        /// <param name="totalCount">Out parameter of the total records found</param>
        /// <returns></returns>
        public virtual IEnumerable<TAggregateRoot> Query(Expression<Func<TAggregateRoot, bool>> query, PageDefinition paging, out int totalCount)
        {
            //Error Checking (do this first, to save time if there is one)
            List<string> errorMessages;
            paging.Validate(out errorMessages);
            if(errorMessages.Any())
                throw new Exception("Error(s) found in Query: " + string.Join("; ", errorMessages.ToArray()));
            
            //Total Count
            totalCount = Session.QueryOver<TAggregateRoot>().Where(query)
                         .Select(Projections.RowCount())
                         .Cacheable()
                         .CacheMode(CacheMode.Normal)
                         .FutureValue<int>().Value;

            var aggregateQuery = Session.QueryOver<TAggregateRoot>()
                                .Where(query);

            //Sorting
            CorrectSortCasing(paging.Sorts);
            foreach(var sort in paging.SortsOrDefault())
            {
                var order = sort.SortDirection == SortDirection.Ascending ? Order.Asc(sort.SortBy) : Order.Desc(sort.SortBy);
                aggregateQuery.UnderlyingCriteria.AddOrder(order);
            }

            //Paging result
            var results = aggregateQuery.Take(paging.PageSize)
                             .Skip(paging.SkippedItems)
                             .Cacheable()
                             .CacheMode(CacheMode.Normal)
                             .Future<TAggregateRoot>();

            return results;
        }

        /// <summary>
        /// Creates the query.
        /// </summary>
        /// <typeparam name="TQuery">The type of the query.</typeparam>
        /// <returns></returns>
        public virtual TQuery CreateQuery<TQuery>() where TQuery : Queries.IQuery
        {
            return QueryFactory.CreateQuery<TQuery>();
        }

        /// <summary>
        /// Checks if any of the criteria exist
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        public virtual bool Exists(Func<TAggregateRoot, bool> criteria)
        {
            return Session.Query<TAggregateRoot>().Any(criteria);
        }

        /// <summary>
        /// Checks if an identifier exists
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public virtual bool Exists(TKey id)
        {
            return Session.Get<TAggregateRoot>(id) != null;
        }

        /// <summary>
        /// Checks if an identifier exists
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public virtual bool Exists(Guid id)
        {
            return Session.Query<TAggregateRoot>().Any(a => a.ExternalId == id);
        }
        
        /// <summary>
        /// Counts objects which satisfy the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public virtual int Count(Func<TAggregateRoot, bool> query)
        {
            return Session.Query<TAggregateRoot>().Count(query);
        }

        #endregion

        #region Add

        /// <summary>
        /// Adds the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public virtual AbimValidationResult Add(TAggregateRoot obj)
        {
            return Add(obj, null, CommitEachCallInItsOwnTransaction);
        }

        /// <summary>
        /// Adds the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="createdBy">The username of the user doing the Add</param>
        /// <returns></returns>
        public virtual AbimValidationResult Add(TAggregateRoot obj, string createdBy)
        {
            return Add(obj, createdBy, CommitEachCallInItsOwnTransaction);
        }

        /// <summary>
        /// Adds the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="createdBy">The username of the user doing the Add</param>
        /// <returns></returns>
        protected virtual AbimValidationResult Add(TAggregateRoot obj, string createdBy, bool wrapInTransactionAndCommit)
        {
            AbimValidationResult result;
            if(wrapInTransactionAndCommit)
            {
                using(var tx = Session.BeginTransaction())
                {
                    try
                    {
                        result = PerformAdd(obj, createdBy);
                        if(result.Succeeded)
                        {
                            Session.Flush();
                            tx.Commit();
                        }
                        else RollbackTransaction(tx);
                        return result;
                    }
                    catch(Exception ex)
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
            else return PerformAdd(obj, createdBy);
        }

        /// <summary>
        /// Adds the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="createdBy">The username of the user doing the Add</param>
        /// <returns></returns>
        protected virtual AbimValidationResult PerformAdd(TAggregateRoot obj, string createdBy)
        {
            if(obj is AggregateRoot)
            {
                var aggregateRoot = (AggregateRoot)((object)obj);
                if(createdBy != null)
                {
                    if(aggregateRoot.AuditData == null)
                        aggregateRoot.AuditData = AuditData.Create(createdBy);
                    else
                        aggregateRoot.AuditData.CreatedBy = createdBy;
                }
                else if(aggregateRoot.AuditData == null || aggregateRoot.AuditData.CreatedBy == null)
                {
                    var msg = "CreatedBy is null. Often this was originally set from a preferred_username claim; check that one exists";
                    throw new Exception(msg);
                }
            }
            
            var result = obj.Validate(Validation);
            if(!result.Succeeded)
                return result;
            
            //As a warning, this will ALREADY put the object into the database, even before CommitTransaction(). This is because Save() is responsible for
            //... setting the identity column. Persist() would also do this, unless the entity generating strategy was changed from Identity to Sequence or Auto,
            //... which we don't want to do. However, this is ok, because the calling method, Add(), or the calling service, can still roll the transaction back
            Session.Save(obj);
        
            return result;
        }

        /// <summary>
        /// Adds many.
        /// </summary>
        /// <param name="objs">The objs.</param>
        /// <returns></returns>
        public virtual AbimValidationResult AddMany(IEnumerable<TAggregateRoot> objs)
        {
            return AddMany(objs, null);
        }

        /// <summary>
        /// Adds many.
        /// </summary>
        /// <param name="objs">The objs.</param>
        /// <param name="createdBy">The username of the user doing the Add</param>
        /// <returns></returns>
        public virtual AbimValidationResult AddMany(IEnumerable<TAggregateRoot> objs, string createdBy)
        {
            var output = new AbimValidationResult();
            // set Succeeded to true by default because if one or many unsuccessfull Add can set to false
            output.Succeeded = true;
            objs.ForEach(o =>
            {
                var result = Add(o, createdBy, CommitEachCallInItsOwnTransaction);
                if (!result.Succeeded)
                {
                    result.Results.ForEach(r => output.Results = result.Results.Concat(new[] { r }));
                    //one unsuccessfull Add set to false and would remain false until the end
                    output.Succeeded = false;
                }
            });

            return output;
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public virtual AbimValidationResult Update(TAggregateRoot obj)
        {
            return Update(obj, null, CommitEachCallInItsOwnTransaction);
        }

        /// <summary>
        /// Updates the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="modifiedBy">The username of the user making the modification</param>
        /// <returns></returns>
        public virtual AbimValidationResult Update(TAggregateRoot obj, string modifiedBy)
        {
            return Update(obj, modifiedBy, CommitEachCallInItsOwnTransaction);
        }

        /// <summary>
        /// Updates the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="modifiedBy">The username of the user doing the Add</param>
        /// <returns></returns>
        protected virtual AbimValidationResult Update(TAggregateRoot obj, string modifiedBy, bool wrapInTransactionAndCommit)
        {
            AbimValidationResult result;
            if(wrapInTransactionAndCommit)
            {
                using(var tx = Session.BeginTransaction())
                {
                    try
                    {
                        result = PerformUpdate(obj, modifiedBy);
                        if(result.Succeeded)
                        {
                            Session.Flush();
                            tx.Commit();
                        }
                        else RollbackTransaction(tx);
                        return result;
                    }
                    catch(Exception ex)
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
            else return PerformUpdate(obj, modifiedBy);
        }

        /// <summary>
        /// Updates the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="modifiedBy">The username of the user making the modification</param>
        /// <returns></returns>
        protected virtual AbimValidationResult PerformUpdate(TAggregateRoot obj, string modifiedBy)
        {
            if(modifiedBy != null) obj.AuditData.ModifiedBy = modifiedBy;
            obj.AuditData.Modified = DateTime.Now;
            
            var result = obj.Validate(Validation);
            if(!result.Succeeded)
                return result;
            
            Session.Update(obj);
            
            return result;
        }

        /// <summary>
        /// Updates the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        public virtual void Update(Func<TAggregateRoot, bool> query)
        {
            Session.Query<TAggregateRoot>().Where(query).ForEach(o => Update(o, null));
        }

        /// <summary>
        /// Updates the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="modifiedBy">The username of the user making the modification</param>
        public virtual void Update(Func<TAggregateRoot, bool> query, string modifiedBy = null)
        {
            Session.Query<TAggregateRoot>().Where(query).ForEach(o => Update(o, modifiedBy));
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        public virtual void Delete(TAggregateRoot obj)
        {
            if(CommitEachCallInItsOwnTransaction)
            {
                using(var tx = Session.BeginTransaction())
                {
                    try
                    {
                        Session.Delete(obj);
                        Session.Flush();
                        tx.Commit();
                    }
                    catch (Exception e)
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
            else Session.Delete(obj);
        }

        /// <summary>
        /// Deletes the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        public virtual void Delete(Func<TAggregateRoot, bool> query)
        {
            Session.Query<TAggregateRoot>().Where(query).ForEach(Delete);
        }

        #endregion

        #region Other

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public virtual IEnumerator<TAggregateRoot> GetEnumerator()
        {
            return Transact(() => GetAll().GetEnumerator());
        }
        
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Transact(() => GetEnumerator());
        }

        /// <summary>
        /// Transaction function, taking in a Func
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        protected virtual TResult Transact<TResult>(Func<TResult> func)
        {
            if(!Session.Transaction.IsActive)
            {
                TResult result;
                using(var tx = Session.BeginTransaction())
                {
                    try
                    {
                        result = func.Invoke();
                        tx.Commit();
                        return result;
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                        tx.Rollback();
                        throw e;
                    }
                }
            }

            return func.Invoke();
        }
        
        /// <summary>
        /// Transact method
        /// </summary>
        /// <param name="action"></param>
        protected virtual void Transact(Action action)
        {
            Transact(() =>
            {
                action.Invoke();
                return false;
            });
        }
        
        /// <summary>
        /// Corrects the sort casing.
        /// </summary>
        /// <param name="sorts">The sorts.</param>
        protected static void CorrectSortCasing(List<Sort> sorts)
        {
            if(!sorts.Any()) return;
            PropertyInfo[] props = typeof(TAggregateRoot).GetProperties();
            
            foreach(var sort in sorts)
            {
                if(sort.SortBy.Contains('.')) continue;        //unable to validate this here; leave it as-is
                sort.SortBy = sort.SortBy.Trim();
                
                var anyCaseMatches = props.Where(p => p.Name.ToLower() == sort.SortBy.ToLower()).ToList();
                if(anyCaseMatches.Count() == 1)
                {
                    if(anyCaseMatches.First().Name != sort.SortBy) sort.SortBy = anyCaseMatches.First().Name;
                }
            }
        }
        
        /// <summary>
        /// Gets the SQL.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        protected static string GetSql(ICriteria criteria)
        {
            var criteriaImpl = (CriteriaImpl)criteria;
            var sessionImpl = (SessionImpl)criteriaImpl.Session;
            var factory = (SessionFactoryImpl)sessionImpl.SessionFactory;
            var implementors = factory.GetImplementors(criteriaImpl.EntityOrClassName);
            var loader = new CriteriaLoader((IOuterJoinLoadable)factory.GetEntityPersister(implementors[0]), factory, criteriaImpl,
                implementors[0], sessionImpl.EnabledFilters);
            return loader.SqlString.ToString();
        }

        #endregion

        #endregion
    }
}
