using Abim.Platform.Program.MembershipClient;
using Abim.Platform.Program.Core.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
 
namespace Abim.Platform.Program.App.Services.Impl
{
    /// <summary>
    /// MembershipClientService
    /// </summary>
    public class MembershipClientService : ClientWrapperService, IMembershipClientService
    {
        /// <summary>
        /// _apiClientFactory
        /// </summary>
        protected readonly IApiClientFactory _apiClientFactory;

        /// <summary>
        /// MembershipClientService
        /// </summary> 
        /// <param name="accessTokenService"></param>
        /// <param name="httpClientFactory"></param>
        /// <param name="apiClientFactory"></param>
        ///  <param name="apiBaseUrl"></param>
        /// <param name="pollyRetries"></param>
        public MembershipClientService(IAccessTokenService accessTokenService, IHttpClientFactory httpClientFactory, IApiClientFactory apiClientFactory, string apiBaseUrl, int pollyRetries) :
            base(accessTokenService, httpClientFactory, apiBaseUrl, pollyRetries)
        {
            _apiClientFactory = apiClientFactory;
        }

        // Only Exposing client End Point and return Task. Do not put any logic on any of these methods here. 

        /// <summary>
        /// GetProfileGETAsync
        /// </summary> 
        /// <returns>ICollection ProfileResource </returns>
        public async Task<ProfileResource> GetProfileByMemberIdAsync(Guid memberId)
        { 
            return await ExecuteCall(async () =>
            { 
                var client = _apiClientFactory.GetMembershipClient(_apiBaseUrl, CreateHttpClient());
                return await client.GetProfileByIdAsync(memberId); 
            });  
        }

        /// <summary>
        /// GetCountriesAsync
        /// </summary> 
        /// <returns>ICollection CountryResource </returns>
        public async Task<ICollection<CountryResource>> GetCountriesAsync()
        {
            return await ExecuteCall(async () =>
            {
                var client = _apiClientFactory.GetMembershipClient(_apiBaseUrl, CreateHttpClient());
                return await client.GetCountriesAsync();
            });
        }

        /// <summary>
        /// GetCountryRegionsAsync
        /// </summary> 
        /// <returns>ICollection RegionResource </returns>
        public async Task<ICollection<RegionResource>> GetCountryRegionsAsync(string courntyCode)
        {
            return await ExecuteCall(async () =>
            {
                var client = _apiClientFactory.GetMembershipClient(_apiBaseUrl, CreateHttpClient());
                return await client.GetRegionsAsync(courntyCode); 
            });
        }

        /// <summary>
        /// GetNameAllAsync
        /// </summary> 
        /// <param name="lastName"></param> 
        /// <param name="firstName"></param> 
        /// <param name="dob"></param> 
        /// <param name="soundEx"></param> 
        /// <param name="pageSize"></param> 
        /// <param name="pageIndex"></param> 
        /// <returns>ICollection ProfileCollectionResource </returns>
        public async Task<ICollection<VocProfileResource>> GetNameAllAsync(string lastName, string firstName, DateTime? dob, bool soundEx, int pageSize, int pageIndex)
        {
            // need to review this end point
            // return ProfileApiClient.NameAllAsync(lastName, firstName, dob, soundEx, pageSize, pageIndex);
            return await ExecuteCall(async () =>
            {
                var client = _apiClientFactory.GetMembershipClient(_apiBaseUrl, CreateHttpClient());
                return await client.NameAllAsync(lastName, firstName, dob, soundEx, pageSize, pageIndex);
            });
        }

        /// <summary>
        /// SearchProfilesByNPI
        /// </summary> 
        /// <param name="npi"></param> 
        /// <returns> ICollection VocProfileResource </returns>
        public async Task<ICollection<VocProfileResource>> SearchProfilesByNPI(string npi)
        {
            return await ExecuteCall(async () =>
            {
                var client = _apiClientFactory.GetMembershipClient(_apiBaseUrl, CreateHttpClient());
                return await client.GetVocByNpiAsync(npi);
            });
        }

        /// <summary>
        /// GetAbimAsync
        /// </summary> 
        /// <param name="abim_id"></param> 
        /// <returns>ICollection ProfileResource </returns>
        public async Task<ProfileResource> GetProfileByAbimIdAsync(string abim_id)
        {
            return await ExecuteCall(async () =>
            {
                var client = _apiClientFactory.GetMembershipClient(_apiBaseUrl, CreateHttpClient());
                return await client.GetProfileByAbimIdAsync(abim_id); 
            });
        }

        /// <summary>
        /// GetVocByAbimIdAsync
        /// </summary> 
        /// <param name="abim_id"></param> 
        /// <returns>ICollection VocProfileResource </returns>
        public async Task<ICollection<VocProfileResource>> GetVocByAbimIdAsync(string abim_id)
        {
            return await ExecuteCall(async () =>
            {
                var client = _apiClientFactory.GetMembershipClient(_apiBaseUrl, CreateHttpClient());
                return await client.GetVocByAbimIdAsync(abim_id);
            });
        }

    }
}
