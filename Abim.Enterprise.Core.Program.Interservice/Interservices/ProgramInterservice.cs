using Abim.Platform.Program.Interservice.Shared;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Resources.Commands;
using Polly.Retry;
using System;
using System.Threading.Tasks;

namespace Abim.Platform.Program.Interservice
{
    /// <summary>
    /// RegistrationInterservice class
    /// </summary>
    /// <seealso cref="IProgramInterservice" />
    public class ProgramInterservice : IProgramInterservice
    {
        /// <summary>
        /// Gets or sets the Polly retry policy, if Polly should be used to make interservice calls.
        /// </summary>
        /// <value>
        /// The retry policy.
        /// </value>
        public RetryPolicy RetryPolicy { get; set; } = null;
        
        /// <summary>
        /// Gets or sets an override HTTP version.
        /// </summary>
        /// <value>
        /// The HTTP version.
        /// </value>
        public Version HttpVersion { get; set; } = null;

        /// <summary>
        /// Gets or sets the interservice.
        /// </summary>
        /// <value>
        /// The interservice.
        /// </value>
        public IInterservice Interservice { get; set; }

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgramInterservice"/> class.
        /// </summary>
        /// <param name="interservice">The interservice.</param>
        public ProgramInterservice(IInterservice interservice)
        {
            Interservice = interservice;
        }

        #endregion

        /// <summary>
        /// GetCertificationById.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="accessToken">The access token.</param>
        /// <returns></returns>
        public async Task<CertificationResource> GetCertificationById(Guid id, string accessToken)
        {
            var url = (ProgramResourceConstants.Routes.Prefix.Certification + "/" +
                ProgramResourceConstants.Routes.Certifications.GetCertificationById).Replace("{id:Guid}", id.ToString());
            return await Interservice.Get<CertificationResource>(url, accessToken, RetryPolicy).ConfigureAwait(false);
        }

        /// <summary>
        /// GetAllCertifications.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns></returns>
        public async Task<CertificationFullCollectionResource> GetAllCertifications(string accessToken)
        {
            var url = (ProgramResourceConstants.Routes.Prefix.Certification + "/" +
                ProgramResourceConstants.Routes.Certifications.GetCertifications);
            return await Interservice.Get<CertificationFullCollectionResource>(url, accessToken, RetryPolicy).ConfigureAwait(false);
        }

        /// <summary>
        /// GetMyCredentials.
        /// </summary>
        /// <param name="accessToken">The access token.</param>
        /// <returns></returns>
        public async Task<CredentialCollectionResource> GetMyCredentials(string accessToken)
        {
            var url = ProgramResourceConstants.Routes.Prefix.Certification + "/" +
                ProgramResourceConstants.Routes.Credentials.GetCurrentUserCredentials + "?PageIndex=1&PageSize=500";
            return await Interservice.Get<CredentialCollectionResource>(url, accessToken, RetryPolicy).ConfigureAwait(false);
        }

        /// <summary>
        /// GetCredentialsbyMemberId.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <param name="accessToken">The access token.</param>
        /// <returns></returns>
        public async Task<CredentialCollectionResource> GetCredentialsbyMemberId(Guid memberId, string accessToken)
        {
            var url = ProgramResourceConstants.Routes.Prefix.Certification + "/" +
                ProgramResourceConstants.Routes.Credentials.GetCredentialsByMemberId.Replace("{memberId:Guid}", memberId.ToString()) + "?PageIndex=1&PageSize=500";
            return await Interservice.Get<CredentialCollectionResource>(url, accessToken, RetryPolicy).ConfigureAwait(false);
        }

        /// <summary>
        /// Get First ABIM Issuance Date for user.
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task<DateTime?> GetFirstABIMIssuanceDate(Guid memberId, string accessToken)
        {
            var url = ProgramResourceConstants.Routes.Prefix.Certification + "/" +
               ProgramResourceConstants.Routes.Credentials.GetFirstABIMIssuanceDate.Replace("{memberId:Guid}", memberId.ToString());
            return await Interservice.Get<DateTime?>(url, accessToken, RetryPolicy).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the latest LookbackDate for a user.
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task<DateTime?> GetLatestLookbackDate(Guid memberId, string accessToken)
        {
            var url = ProgramResourceConstants.Routes.Prefix.Certification + "/" +
                ProgramResourceConstants.Routes.Credentials.GetLatestLookbackDate.Replace("{memberId:Guid}", memberId.ToString());
            return await Interservice.Get<DateTime?>(url, accessToken, RetryPolicy).ConfigureAwait(false);
        }

        /// <summary>
        /// EnrollInCMP
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task<CredentialResource> EnrollInCMP(EnrollInCMPCommand command, string accessToken)
        {
            var url = ProgramResourceConstants.Routes.Prefix.Certification + "/" + ProgramResourceConstants.Routes.Credentials.EnrollInCMP;

            return await Interservice.Post<CredentialResource>(
                    url, 
                    accessToken,
                    command).ConfigureAwait(false);
        }

        /// <summary>
        /// UnEnrollInCMP
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="accessToken"></param>
        /// <param name="hostUrl"></param>
        /// <returns></returns>
        public async Task<CredentialResource> UnEnrollInCMP(UnEnrollInCMPCommand command, string accessToken)
        {
            var url = ProgramResourceConstants.Routes.Prefix.Certification + "/" + ProgramResourceConstants.Routes.Credentials.UnEnrollInCMP;

            return await Interservice.Post<CredentialResource>(
                    url,
                    accessToken,
                    command).ConfigureAwait(false);
        }
    }
}
