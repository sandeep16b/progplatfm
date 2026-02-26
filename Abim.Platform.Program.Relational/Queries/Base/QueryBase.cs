using NHibernate;
using NLog;
using System;

namespace Abim.Platform.Program.Relational.Queries.Base
{
    /// <summary>
    /// Nhibernate Querybase Class 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public abstract class QueryBase<TResult> : IQuery<TResult>, IDisposable
    {
        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        protected ISession Session { get; set; }

        /// <summary>
        /// Flag for whether Dispose has already been called
        /// </summary>
        protected bool Disposed;

        /// <summary>
        /// The log
        /// </summary>
        protected static ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryBase{TResult}"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public QueryBase(ISession session)
        {
            Session = session;
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns></returns>
        public abstract TResult Execute();
        
        /// <summary>
        /// Transaction function
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        //previously was generic with a redundant type <TResult>
        protected TResult Transact(Func<TResult> func)
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
                    catch(Exception ex)
                    {
                        Log.Error(ex);
                        tx.Rollback();
                        throw;
                    }
                }
            }

            return func.Invoke();
        }

        /// <summary>
        /// Transact method
        /// </summary>
        /// <param name="action"></param>
        protected void Transact(Action action)
        {
            //previously used the type parameter <bool>, with the action ending in "return false;"
            Transact(() =>
            {
                action.Invoke();
            });
        }

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
        protected virtual void Dispose(bool disposing)
        {
            if(Disposed) return;
            
            //Free managed resources
            if(disposing)
            {
                try
                {
                    CloseTransaction();
                    CloseSession();
                }
                catch(Exception ex)
                {
                    Log.Error(ex);
                }
            }
            
            //Free any unmanaged resources here
            
            Disposed = true;
        }
        
        /// <summary>
        /// Finalizes an instance of the <see cref="QueryBase{TResult}"/> class.
        /// </summary>
        ~QueryBase()
        {
            Dispose(false);
        }
        
        /// <summary>
        /// Closes the transaction.
        /// </summary>
        protected void CloseTransaction()
        {
            try
            {
                if(Session?.Transaction != null) Session.Transaction.Dispose();
            }
            catch(ObjectDisposedException ex)
            {
                Log.Debug("Transaction was already disposed", ex);
            }
        }

        /// <summary>
        /// Closes the session.
        /// </summary>
        private void CloseSession()
        {
            if(Session == null) return;

            //Close()
            try
            {
                Session.Close();
            }
            catch(SessionException ex)
            {
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
        }

        #endregion
    }
}
