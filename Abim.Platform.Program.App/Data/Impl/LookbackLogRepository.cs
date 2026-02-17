using Abim.Platform.Program.App.Domain;
using NHibernate;
using Abim.Platform.Program.Relational.Repository.Base;
using Abim.Platform.Program.Relational.Queries;
using Abim.Platform.Program.Relational.Validation;

namespace Abim.Platform.Program.App.Data.Impl
{
    /// <summary>
    /// LookbackLogRepository Class.
    /// </summary>
    public class LookbackLogRepository : RepositoryBase<LookbackLog, int>, ILookbackLogRepository
    {
        /// <summary>
        /// Creates a new instance of the LookbaclLogrtRepository
        /// </summary>
        /// <param name="session"></param>
        /// <param name="queryFactory"></param>
        /// <param name="factory"></param>
        public LookbackLogRepository(ISession session, IQueryFactory queryFactory, IValidationFactory factory)
            : base(session, queryFactory, factory)
        {
            CommitEachCallInItsOwnTransaction = false;
        }

        #region Impl methods

        #endregion
    }
}
