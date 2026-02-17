using Abim.Platform.Program.App.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abim.Platform.Program.Relational.Repository;
using Abim.Platform.Program.Relational;
using Abim.Platform.Program.Relational.Validation.Impl;

namespace Abim.Platform.Program.App.Data
{
    /// <summary>
    /// Credential Repository Interface
    /// </summary>
    public interface ICredentialRepository : IRepository<Credential, int>
    {
        /// <summary>
        /// Searches Credentials by member Id
        /// </summary>
        /// <param name="memberId">The member Id</param>
        /// <param name="paging">The paging</param>
        /// <param name="totalCount">The total Count</param>
        /// <returns></returns>
        IEnumerable<Credential> SearchByMemberId(Guid memberId, PageDefinition paging, out int totalCount);

        /// <summary>
        /// Searches Credentials by member Id
        /// </summary>
        /// <param name="memberId">The member Id</param>
        /// <returns></returns>
        IEnumerable<Credential> SearchByMemberId(Guid memberId);

        /// <summary>
        /// Searches Credentials by member Id Async
        /// </summary>
        /// <param name="memberId">The member Id</param>
        /// <returns></returns>
        Task<IEnumerable<Credential>> SearchByMemberIdAsync(Guid memberId);

        /// <summary>
        /// Gets the Guids of all the expired credentials for a given user as of a given date, and for each one
        /// also the Id of the active about-to-expire credential (in Tuple pairings)
        /// </summary>
        /// <param name="checkDate">The check date.</param>
        /// <returns>
        /// Each tuple consists of a CredentialId and its IssuanceId
        /// </returns>
        IEnumerable<Tuple<Guid, int>> GetExpiredCredentials(DateTime checkDate);

        /// <summary>
        /// Gets the Guids of all the expired credentials for a given user as of a given date, and for each one
        /// also the Id of the active about-to-expire credential (in Tuple pairings)
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="abimCredentialsOnly">Optional parameter to specify ABIM-issued credentials only (default is false).</param>        /// <returns>
        /// Each tuple consists of a CredentialId and its IssuanceId
        /// </returns>
        IEnumerable<Tuple<Guid, int>> GetExpiredCredentials(DateTime startDate, DateTime endDate, bool abimCredentialsOnly = false);
        /// <summary>
        /// Gets the Guids of all the expired credentials for a given user as of a given date, and for each one
        /// also the Id of the active about-to-expire credential (in Tuple pairings)
        /// </summary>
        /// <param name="checkDate">The check date.</param>
        /// <param name="memberId">The member id.</param>
        /// <returns>
        /// Each tuple consists of a CredentialId and its IssuanceId
        /// </returns>
        IEnumerable<Tuple<Guid, int>> GetExpiredCredentials(DateTime checkDate, Guid memberId);

        /// <summary>
        /// Gets the issuances for member identifier.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <returns></returns>
        IEnumerable<Issuance> GetIssuancesForMemberId(Guid memberId);

        /// <summary>
        /// Firsts the issuance date.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <returns></returns>
        DateTime? GetFirstIssuanceDate(Guid memberId);

        /// <summary>
        /// Gets the latest lookback date.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <returns></returns>
        DateTime? GetLatestLookbackDate(Guid memberId);

        /// <summary>
        /// Gets the credential by member and code.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <param name="Code">The code.</param>
        /// <returns></returns>
        Credential GetCredentialByMemberAndCode(Guid memberId, string Code);

        /// <summary>
        /// Gets the credential by member and certificate.
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="certificationId"></param>
        /// <returns></returns>
        Credential GetCredentialByMemberAndCert(Guid memberId, Guid certificationId);

        /// <summary>
        /// Gets the im credential by member.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <returns></returns>
        Credential GetIMCredentialByMember(Guid memberId);

        /// <summary>
        /// Searches Certification by member Id
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="paging"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        IEnumerable<Certification> SearchCertsByMemberId (Guid memberId, PageDefinition paging, out int totalCount);

        /// <summary>
        ///  Searches Certification by member Id
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        IEnumerable<Certification> SearchCertsByMemberId(Guid memberId);

        /// <summary>
        /// GetIssuanceByCredentialIdandIssuanceDate
        /// </summary>
        /// <param name="credential"></param>
        /// <param name="issuanceDate"></param>
        /// <returns></returns>
        Issuance GetIssuanceByCredentialIdIssuanceDate(Guid credential, DateTime issuanceDate);

        /// <summary>
        /// Get NonAbim Issuance Count
        /// </summary>
        /// <returns></returns>
        int GetNonAbimIssuanceCount();

        /// <summary>
        /// Gets information about Credentials marked for deselection
        /// </summary>
        /// <param name="deselectionEffectiveDate">The DeSelectionEffectiveDate to query by</param>
        /// <returns>An IEnumerable of Tuple&lt;Guid, Guid, string, bool&gt;. The tuple elements are the member ID, credential ID, certificate name, and boolean indicating wether or not the credential is currently active, in that order.</returns>
        IEnumerable<Tuple<Guid, Guid, string, bool>> GetInfoOfCredentialsMarkedForDeselection(DateTime deselectionEffectiveDate);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="credential"></param>
        /// <param name="Username"></param>
        /// <returns></returns>
        AbimValidationResult UpdateCredential(Credential credential, string Username);

    }
}
