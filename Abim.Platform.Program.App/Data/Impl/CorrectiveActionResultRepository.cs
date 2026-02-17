using Abim.Platform.Program.App.Domain;
using NHibernate;
using Abim.Platform.Program.Relational.Repository.Base;
using Abim.Platform.Program.Relational.Queries;
using Abim.Platform.Program.Relational.Validation;

namespace Abim.Platform.Program.App.Data.Impl
{
    /// <summary>
    /// The repository for sources
    /// </summary>
    public class CorrectiveActionResultRepository : RepositoryBase<CorrectiveActionResult, int>, ICorrectiveActionResultRepository
    {
        /// <summary>
        /// The source repository constructor
        /// </summary>
        /// <param name="session"></param>
        /// <param name="queryFactory"></param>
        /// <param name="factory"></param>
        public CorrectiveActionResultRepository(ISession session, IQueryFactory queryFactory, IValidationFactory factory)
            : base(session, queryFactory, factory)
        {
            CommitEachCallInItsOwnTransaction = false;
        }
    }
}
