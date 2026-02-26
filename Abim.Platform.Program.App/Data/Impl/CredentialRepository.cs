using Abim.Platform.Program.App.Data.HQL;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational;
using Abim.Platform.Program.Relational.Queries;
using Abim.Platform.Program.Relational.Repository.Base;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;
using Abim.Platform.Program.Resources;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Abim.Platform.Program.App.Data.Impl
{
    /// <summary>
    /// The repository for credentials (and issuances)
    /// </summary>
    public class CredentialRepository : RepositoryBase<Credential, int>, ICredentialRepository
    {
        /// <summary>
        /// The credential repository constructor
        /// </summary>
        /// <param name="session"></param>
        /// <param name="queryFactory"></param>
        /// <param name="factory"></param>
        public CredentialRepository(ISession session, IQueryFactory queryFactory, IValidationFactory factory)
            : base(session, queryFactory, factory)
        {
            CommitEachCallInItsOwnTransaction = false;
        }

        /// <summary>
        /// Searches Credentials by memberId
        /// </summary>
        /// <param name="memberId">The member Id</param>
        /// <param name="paging">The paging</param>
        /// <param name="totalCount">The total Count</param>
        /// <returns></returns>
        IEnumerable<Credential> ICredentialRepository.SearchByMemberId(Guid memberId, PageDefinition paging, out int totalCount)
        {
            return Query(cred => cred.MemberId == memberId, paging, out totalCount);
        }

        /// <summary>
        /// Searches Certification by member Id
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="paging"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        IEnumerable<Certification> ICredentialRepository.SearchCertsByMemberId(Guid memberId, PageDefinition paging, out int totalCount)
        {
            return QueryCerts(cred => cred.MemberId == memberId, paging, out totalCount);
        }

        /// <summary>
        /// Searches Certification by member Id
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        IEnumerable<Certification> ICredentialRepository.SearchCertsByMemberId(Guid memberId)
        {
            return Session.QueryOver<Credential>()
                         .Where(a => a.MemberId == memberId)
                         .JoinQueryOver<Issuance>(p => p.Issuances)
                         .List()
                         .Select(r => r.Certification).ToList(); 
        }

        /// <summary>
        /// Searches by memberId
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        IEnumerable<Credential> ICredentialRepository.SearchByMemberId(Guid memberId)
        {
            return Session.QueryOver<Credential>().Where(cred => cred.MemberId == memberId).List().ToList();
        }

        /// <summary>
        /// Searches by memberId
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        async Task<IEnumerable<Credential>> ICredentialRepository.SearchByMemberIdAsync(Guid memberId)
        {
            return await Task.FromResult<IEnumerable<Credential>>(Session.QueryOver<Credential>().Where(cred => cred.MemberId == memberId).List().ToList());
        }

        /// <summary>
        /// Gets credentials by memberId and code
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        Credential ICredentialRepository.GetCredentialByMemberAndCode(Guid memberId, string code)
        {
            return Query()
                      .Where(cred => cred.MemberId == memberId && cred.Certification.Code == code)
                      .SingleOrDefault();
        }

        /// <summary>
        /// Gets credentials by memberId and certificationId
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="certificationId"></param>
        /// <returns></returns>
        Credential ICredentialRepository.GetCredentialByMemberAndCert(Guid memberId, Guid certificationId)
        {
            return Session.QueryOver<Credential>()
                      .Where(cred => cred.MemberId == memberId)
                      .JoinQueryOver(p => p.Certification)
                      .Where(c => c.ExternalId == certificationId)
                      .SingleOrDefault();
        }

        /// <summary>
        /// Gets a diplomate's IM credential
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        Credential ICredentialRepository.GetIMCredentialByMember(Guid memberId)
        {
            return Session.QueryOver<Credential>()
                .Where(cred => cred.MemberId == memberId)
                .List()
                .Where(c => c.Certification.Code == ProgramResourceConstants.CertificationCode.InternalMedicine)
                .SingleOrDefault();
        }

        /// <summary>
        /// Gets expired credentials in a date range
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="abimCredentialsOnly">Optional parameter to specify ABIM-issued credentials only (default is false).</param>
        /// <returns></returns>
        IEnumerable<Tuple<Guid, int>> ICredentialRepository.GetExpiredCredentials(DateTime startDate, DateTime endDate, bool abimCredentialsOnly)
        {            
            string hql = 
                @"select c.ExternalId, i.Id 
                    from Credential c 
                    join c.Issuances i
                    where i.IssuanceStatus = 'Active'
                    and(i.Duration = 'Timelimited' and i.ExpirationDate is not null and i.ExpirationDate >= :startDate and i.ExpirationDate <= :endDate" + (abimCredentialsOnly ? " and i.Source = 1 " : "") + ")";
            
            var query = Session.CreateQuery(hql)
                .SetDateTime("startDate", startDate)
                .SetDateTime("endDate", endDate);
            var list = query.Future<Object[]>().ToList();
            return list.Select(x => new Tuple<Guid, int>((Guid)(x[0]), (int)(x[1])));
        }

        /// <summary>
        /// Gets the Guids of all the expired credentials for a given user as of a given date, and for each one
        /// also the Id of the active about-to-expire credential (in Tuple pairings)
        /// </summary>
        /// <param name="checkDate">The check date.</param>
        /// <returns>
        /// Each tuple consists of a CredentialId and its IssuanceId
        /// </returns>
        IEnumerable<Tuple<Guid, int>> ICredentialRepository.GetExpiredCredentials(DateTime checkDate)
        {
            string hql = GetExpiredCredentialsHQL.HQL;
            var query = Session.CreateQuery(hql)
                .SetDateTime("checkDate", checkDate);
            var list = query.Future<Object[]>().ToList();
            return list.Select(x => new Tuple<Guid, int>((Guid)(x[0]), (int)(x[1])));
        }

        /// <summary>
        /// Gets the Guids of all the expired credentials for a given user as of a given date, and for each one
        /// also the Id of the active about-to-expire credential (in Tuple pairings)
        /// </summary>
        /// <param name="checkDate">The check date.</param>
        /// <param name="memberId">The member id.</param>
        /// <returns>
        /// Each tuple consists of a CredentialId and its IssuanceId
        /// </returns>
        IEnumerable<Tuple<Guid, int>> ICredentialRepository.GetExpiredCredentials(DateTime checkDate, Guid memberId)
        {
            string hql = GetExpiredCredentialsHQL.MemberHQL;
            var query = Session.CreateQuery(hql)
                .SetDateTime("checkDate", checkDate)
                .SetGuid("memberId", memberId);
            var list = query.Future<Object[]>().ToList();
            return list.Select(x => new Tuple<Guid, int>((Guid)(x[0]), (int)(x[1])));
        }

        /// <summary>
        /// Gets the IDs of Credentials marked for deselection
        /// </summary>
        /// <param name="deselectionEffectiveDate">The DeSelectionEffectiveDate to query by</param>
        /// <returns>An IEnumerable of Tuple&lt;Guid, Guid, string, bool&gt;. The tuple elements are the member ID, credential ID, certificate name, and boolean indicating wether or not the credential is currently active, in that order.</returns>
        IEnumerable<Tuple<Guid, Guid, string, bool>> ICredentialRepository.GetInfoOfCredentialsMarkedForDeselection(DateTime deselectionEffectiveDate)
        { 
            string hql = @"select cred.MemberId, cred.ExternalId, cert.Name, cred.IsActive 
                            from Credential cred 
                            join cred.Issuances i
                            join cred.Certification cert
                            where i.DeselectionEffectiveDate = :deselectionEffectiveDate 
                            and i.DeselectionProcessedDate is null";

            var query = Session.CreateQuery(hql)
                .SetDateTime("deselectionEffectiveDate", deselectionEffectiveDate);

            var list = query.Future<Object[]>().ToList();
            return list.Select(x => new Tuple<Guid, Guid, string, bool>((Guid)(x[0]), (Guid)(x[1]), (string)(x[2]), (bool)(x[3])));
        }

        /// <summary>
        /// Gets issuances for a diplomate
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        IEnumerable<Issuance> ICredentialRepository.GetIssuancesForMemberId(Guid memberId)
        {
            return Session.QueryOver<Credential>().Where(cred => cred.MemberId == memberId).List().SelectMany(a => a.Issuances).ToList();
        }

        /// <summary>
        /// Gets the first issuance date for a diplomate
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        DateTime? ICredentialRepository.GetFirstIssuanceDate(Guid memberId)
        {
            string HQL = @"select Min(i.IssuanceDate)
                    from Credential c 
                     join c.Issuances i 
                     join i.Source s
                    where s.Code='ABIM' and c.MemberId = :memberId"; // i.Duration = 'Timelimited' and

                var query = Session.CreateQuery(HQL)
                            .SetGuid("memberId", memberId);
                //for exception situation when date is not found then return max DateTime.
                //var queryReturn = query.UniqueResult() ?? DateTime.MaxValue;
                return (DateTime?)query.UniqueResult();
        
        }

        /// <summary>
        /// Gets the latest lookback date for a diplomate
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        DateTime? ICredentialRepository.GetLatestLookbackDate(Guid memberId)
        {
                string HQL = @"select Max(LookbackDate)
                                from Credential
                                where MemberId = :memberId";

                var query = Session.CreateQuery(HQL)
                            .SetGuid("memberId", memberId);

                return (DateTime?)query.UniqueResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="credentialId"></param>
        /// <param name="issuanceDate"></param>
        /// <returns></returns>
        Issuance ICredentialRepository.GetIssuanceByCredentialIdIssuanceDate(Guid credentialId, DateTime issuanceDate)
        {
            return Session.QueryOver<Issuance>()
                .Where(i => i.IssuanceDate.Date == issuanceDate.Date)
                .JoinQueryOver(p => p.Credential)
                .Where(i => i.ExternalId == credentialId)
                .SingleOrDefault();
        }

        /// <summary>
        /// Get NonAbim Issuance Count
        /// </summary>
        /// <returns></returns>
        int ICredentialRepository.GetNonAbimIssuanceCount()
        {
            string hql = @"select max(i.IssuanceDate)
                        from Issuance i join i.Source s
                        where s.Code <> 'ABIM'
                        group by i.Credential.ExternalId";

            var query = Session.CreateQuery(hql);

            return query.Future<DateTime>().Count();
        }

        #region Consider to Redesign
        /// <summary>
        /// QueryCerts
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Certification> QueryCerts(Expression<Func<Credential, bool>> query,
                                                    PageDefinition paging,
                                                    out int totalCount)
        {
            //Error Checking (do this first, to save time if there is one)
            List<string> errorMessages;
            paging.Validate(out errorMessages);
            if (errorMessages.Any())
                throw new Exception("Error(s) found in Query: " + string.Join("; ", errorMessages.ToArray()));

            //Total Count
            totalCount = Session.QueryOver<Credential>()
                         .Where(query)
                         .JoinQueryOver<Issuance>(p => p.Issuances)
                         .Select(Projections.RowCount())
                         .Cacheable()
                         .CacheMode(CacheMode.Normal)
                         .FutureValue<int>().Value;

            var aggregateQuery = Session.QueryOver<Credential>()
                                .Where(query).JoinQueryOver<Issuance>(p => p.Issuances);

            //Sorting
            CorrectSortCasing(paging.Sorts);
            foreach (var sort in paging.SortsOrDefault())
            {
                var order = sort.SortDirection == SortDirection.Ascending ? Order.Asc(sort.SortBy) : Order.Desc(sort.SortBy);
                aggregateQuery.UnderlyingCriteria.AddOrder(order);
            }

            //Paging result
            var results = aggregateQuery.Take(paging.PageSize)
                             .Skip(paging.SkippedItems)
                             .Cacheable()
                             .CacheMode(CacheMode.Normal)
                             .Future<Credential>();

            return results.Select(a => a.Certification).ToList();
        }
        #endregion

        /// <summary>
        /// UpdateCredential ( skip logging as error for UNIQUE KEY constraint 'NK_Issuance')
        /// </summary>
        /// <param name="credential"></param>
        /// <param name="Username"></param>
        /// <returns></returns>
        AbimValidationResult ICredentialRepository.UpdateCredential(Credential credential, string Username)
        {
            AbimValidationResult objValidation;
            Log.Info($"CredentialRepository.UpdateCredential with CredentialId:{credential.Id} IssuanceDate:{credential.NewestIssuance.IssuanceDate.ToShortDateString()} on Thread:{Thread.CurrentThread.ManagedThreadId}");

            using (var t = Session.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    objValidation = Update(credential, Username);
                    Session.Flush();
                    t.Commit();
                }
                catch (Exception ex)
                {
                    string message = null;
                    // don't log as an error UNIQUE KEY constraint 'NK_Issuance', will be log as warning later
                    if (ex.InnerException != null &&
                        ex.InnerException.Message.StartsWith("Violation of UNIQUE KEY constraint 'NK_Issuance'.") &&
                        ex.Source == "NHibernate")
                    {
                        message = $"Caught UNIQUE KEY constraint 'NK_Issuance' Exception: {ex.Message}.";
                        Log.Warn(message);
                    }
                    else
                    {
                        // all others log as error ....
                        Log.Error(ex);
                        Log.Error($"Exception during Update {ex.Message}, at {Environment.StackTrace}");
                    }

                    RollbackTransaction(t);

                    // create Validation error manually....                               
                    objValidation = new AbimValidationResult()
                    {
                        Succeeded = false,
                        Results = new List<IValidationResult>()
                                    {
                                        new ValidationErrorResult()
                                        {
                                            Message =  message ?? ex.Message
                                        }
                                    }
                    };

                    return objValidation;
                }

            } // Session.BeginTransaction(IsolationLevel.Serializable))

            return objValidation;
        }
    }
}
