using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Resources.Commands;
using Polly.Retry;
using System;
using System.Threading.Tasks;

namespace Abim.Platform.Program.Interservice
{
    /// <summary>
    /// IRegistrationInterservice interface
    /// </summary>
    public interface IProgramInterservice
    {
        /// <summary>
        /// Gets or sets the Polly retry policy, if Polly should be used to make interservice calls.
        /// </summary>
        /// <value>
        /// The retry policy.
        /// </value>
        RetryPolicy RetryPolicy { get; set; }

        /// <summary>
        /// GetCertificationById.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="accessToken">The access token.</param>
        /// <returns></returns>
        Task<CertificationResource> GetCertificationById(Guid id, string accessToken);

        /// <summary>
        /// GetAllCertifications.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns></returns>
        Task<CertificationFullCollectionResource> GetAllCertifications(string accessToken);

        /// <summary>
        /// GetMyCredentials.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns></returns>
        Task<CredentialCollectionResource> GetMyCredentials(string accessToken);

        /// <summary>
        /// GetCredentialsbyMemberId.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <param name="accessToken">The access token.</param>
        /// <returns></returns>
        Task<CredentialCollectionResource> GetCredentialsbyMemberId(Guid memberId, string accessToken);

        /// <summary>
        /// Get First ABIM Issuance Date.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <param name="accessToken">The access token.</param>
        /// <returns></returns>
        Task<DateTime?> GetFirstABIMIssuanceDate(Guid memberId, string accessToken);

        /// <summary>
        /// Get latest Lookback Date.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <param name="accessToken">The access token.</param>
        /// <returns></returns>
        Task<DateTime?> GetLatestLookbackDate(Guid memberId, string accessToken);

        /// <summary>
        /// EnrollInCMP
        /// </summary>
        /// <param name="command"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        Task<CredentialResource> EnrollInCMP(EnrollInCMPCommand command, string accessToken);

        /// <summary>
        /// UnEnrollInCMP
        /// </summary>
        /// <param name="command"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        Task<CredentialResource> UnEnrollInCMP(UnEnrollInCMPCommand command, string accessToken);

    }
}
