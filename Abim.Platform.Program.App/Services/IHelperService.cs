using Abim.Enterprise.Core.Registration.Enums;
using Abim.Platform.Program.MembershipClient;
using Abim.Platform.Program.App.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks; 

namespace Abim.Platform.Program.App.Services
{
    /// <summary>
    /// IHelperService interface.
    /// </summary>
    public interface IHelperService : IDisposable
    {
        /// <summary>
        /// TriggeredCommunication
        /// </summary>
        /// <param name="credential"></param>
        /// <param name="triggeredCommunication"></param>
        /// <param name="memberId">Member id of the diplomate</param>
        /// <param name="credentialService">Credential Service</param>
        /// <returns></returns>
        Task TriggeredCommunication(Credential credential, string triggeredCommunication, Guid memberId, ICredentialService credentialService);

        /// <summary>
        /// TriggeredCommunication_Reactivate_Certifications
        /// </summary>
        /// <param name="certNames"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        Task TriggeredCommunication_Reactivate_Certifications(IList<string> certNames, Guid memberId);

        /// <summary>
        /// Get Profile By MemberId
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        //Task<ProfileNestedResource> GetProfileById(Guid memberId);

        /// <summary>
        /// Get Profile By ABIMId
        /// </summary>
        /// <param name="abimId"></param>
        /// <returns></returns>
        //Task<ProfileNestedResource> GetProfileByABIMId(string abimId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        MemoryStream CreateVocLetter(VocPdfData data);

        /// <summary>
        /// GetMemberIdByAbimId
        /// </summary>
        /// <param name="abimId"></param>
        /// <returns></returns>
        Task<Guid> GetMemberIdByAbimId(string abimId);

        /// <summary>
        /// GetVocLetterContent
        /// </summary>
        /// <returns></returns>
        Task<VocPdfData> GetVocLetterContent(ProfileResource profile, IEnumerable<Credential> credentials);

        /// <summary>
        /// GetCountryByCountryId
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        Task<CountryResource> GetCountryByCountryId(ProfileAddressResource address); 

        /// <summary>
        /// GetMatchingRegionByRegionId
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        Task<RegionResource> GetMatchingRegionByRegionId(ProfileAddressResource address);

        /// <summary>
        /// GetPriviousPathwayForSubspecialtyCertCode
        /// </summary>
        /// <param name="MemberId"></param>
        /// <param name="CertificationId"></param>
        /// <returns></returns>
        Task<ExamType> GetMostRecentExamTypeByCode(Guid MemberId, Guid CertificationId);
    }
}
