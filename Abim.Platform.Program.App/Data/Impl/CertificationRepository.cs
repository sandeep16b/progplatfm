using Abim.Platform.Program.App.Domain;
using NHibernate;
using System.Collections.Generic;
using System.Linq;
using System;
using Abim.Platform.Program.Relational.Repository.Base;
using Abim.Platform.Program.Relational.Queries;
using Abim.Platform.Program.Relational.Validation;

namespace Abim.Platform.Program.App.Data.Impl
{
    /// <summary>
    /// The repository for certifications
    /// </summary>
    public class CertificationRepository : RepositoryBase<Certification, int>, ICertificationRepository
    {
        /// <summary>
        /// The certification repository constructor
        /// </summary>
        /// <param name="session"></param>
        /// <param name="queryFactory"></param>
        /// <param name="factory"></param>
        public CertificationRepository(ISession session, IQueryFactory queryFactory, IValidationFactory factory)
            : base(session, queryFactory, factory)
        {
            CommitEachCallInItsOwnTransaction = false;
        }

        /// <summary>
        /// Gets certifications which match any of the desired Id's
        /// </summary>
        /// <param name="ids">The ids.</param>
        /// <returns></returns>
        List<Certification> ICertificationRepository.GetByIds(List<Guid> ids)
        {
            return Session.QueryOver<Certification>()
                .WhereRestrictionOn(x => x.ExternalId).IsIn(ids)
                .List().ToList();
        }

        /// <summary>
        /// Gets Certification by SourceId and Code
        /// </summary>
        /// <param name="sourceId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        Certification ICertificationRepository.GetBySourceIdAndCode(Guid sourceId, string code)
        {
            return Session.QueryOver<Certification>()
                       .Where(c => c.Code == code)
                        .JoinQueryOver(p => p.Source)
                      .Where(s => s.ExternalId == sourceId)
                      .SingleOrDefault();
        }

    }
}
