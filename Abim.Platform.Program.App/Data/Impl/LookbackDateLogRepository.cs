using Abim.Platform.Program.Relational.Repository.Base;
using Abim.Platform.Program.Relational.Queries;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.App.Domain;
using NHibernate;

namespace Abim.Platform.Program.App.Data.Impl
{
    /// <summary>
    /// Repository Layer Implementation for LookbackDateLog
    /// </summary>
    public class LookbackDateLogRepository : RepositoryBase<LookbackDateLog, int>, ILookbackDateLogRepository
    {
        /// <summary>
        /// Creates a new instance of the LookbaclLogrtRepository
        /// </summary>
        /// <param name="session"></param>
        /// <param name="queryFactory"></param>
        /// <param name="factory"></param>
        public LookbackDateLogRepository(ISession session, IQueryFactory queryFactory, IValidationFactory factory)
            : base(session, queryFactory, factory)
        {
            CommitEachCallInItsOwnTransaction = false;
        }
    }
}
