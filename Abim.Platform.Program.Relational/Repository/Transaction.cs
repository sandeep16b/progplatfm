using Abim.Platform.Program.Relational.Validation.Impl;
using NHibernate;
using System;

namespace Abim.Platform.Program.Relational.Repository
{
    /// <summary>
    /// Transaction class
    /// </summary>
    [Obsolete]
    public class Transaction
    {
        /// <summary>
        /// Gets or sets the validation result.
        /// </summary>
        /// <value>
        /// The validation result.
        /// </value>
        public AbimValidationResult ValidationResult { get; set; }

        /// <summary>
        /// Gets or sets the NHibernate transaction.
        /// </summary>
        /// <value>
        /// The NHibernate transaction.
        /// </value>
        internal ITransaction NHibernateTransaction { get; set; }

        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        /// <value>
        /// The session.
        /// </value>
        internal ISession Session { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Transaction"/> class.
        /// </summary>
        /// <param name="nHibTrans">The n hib trans.</param>
        /// <param name="session">The session.</param>
        internal Transaction(ITransaction nHibTrans, ISession session)
        {
            NHibernateTransaction = nHibTrans;
            Session = session;
        }

        /// <summary>
        /// Commits this transaction.
        /// </summary>
        public void Commit()
        {
            try
            {
                Session.Flush();
                NHibernateTransaction.Commit();
            }
            finally
            {
                Rollback();
            }
        }

        /// <summary>
        /// Rolls back this transaction.
        /// </summary>
        public void Rollback()
        {
            NHibernateTransaction.Rollback();
        }
    }
}
