using Abim.Platform.Program.App.Domain;
using NHibernate;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Abim.Platform.Program.Relational.Repository.Base;
using Abim.Platform.Program.Relational.Queries;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;
using System.Linq;

namespace Abim.Platform.Program.App.Data.Impl
{
    /// <summary>
    /// The repository for LookBackDatesInfo
    /// </summary>
    public class LookBackDatesInfoRepository : RepositoryBase<LookBackDatesInfo, int>, ILookBackDatesInfoRepository
    {
        /// <summary>
        /// The source repository constructor
        /// </summary>
        /// <param name="session"></param>
        /// <param name="queryFactory"></param>
        /// <param name="factory"></param>
        public LookBackDatesInfoRepository(ISession session, IQueryFactory queryFactory, IValidationFactory factory)
            : base(session, queryFactory, factory)
        {
            CommitEachCallInItsOwnTransaction = false;
        }

        /// <summary>
        /// GetExpiredYearEndMemberValues
        /// </summary>
        /// <param name="expiredDate"></param>
        /// <returns></returns>
        IEnumerable<LookBackDatesInfo> ILookBackDatesInfoRepository.GetExpiredYearEndLookBackDatesInfo(DateTime expiredDate)
        {
            return Session.QueryOver<LookBackDatesInfo>()
                         .Where(a => (a.Lookback2YearEndDate!=null
                                && a.Lookback2YearEndDate.Value.Date <= expiredDate.Date) ||
                                (a.Lookback5YearEndDate!=null
                                && a.Lookback5YearEndDate.Value.Date <= expiredDate.Date))
                         .List().ToList();
        }

        /// <summary>
        /// UpdateLookBackDatesInfo
        /// </summary>
        /// <param name="lookBackDatesInfo"></param>
        /// <returns></returns>
        public Task UpdateLookBackDatesInfo(LookBackDatesInfo lookBackDatesInfo)
        {
            BeginTransaction();

            AbimValidationResult objValidation = Update(lookBackDatesInfo);

            if (objValidation.Succeeded)
                CommitTransaction();
            else
            {
                RollbackTransaction();
                Log.Error($"Failed Repository.Update with message: '{objValidation.Message}'");
            }
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// AddLookBackDatesInfo
        /// </summary>
        /// <param name="lookBackDatesInfo"></param>
        /// <returns></returns>
        public Task AddLookBackDatesInfo(LookBackDatesInfo lookBackDatesInfo)
        {
            BeginTransaction();

            AbimValidationResult objValidation = Add(lookBackDatesInfo);

            if (objValidation.Succeeded)
                CommitTransaction();
            else
            {
                RollbackTransaction();
                Log.Error($"Failed Repository.Add with message: '{objValidation.Message}'");
            }
            return Task.FromResult<object>(null);
        }
    }
}
