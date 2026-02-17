using Abim.Platform.Program.MembershipClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abim.Platform.Program.App.Services
{
    /// <summary>
    /// IMembershipClientService
    /// </summary>
    public interface IMembershipClientService 
    {
        /// <summary>
        /// GetApplyAccessToken
        /// </summary>
        string GetAccessToken();

        /// <summary>
        /// GetProfileGETAsync
        /// </summary>
        Task<ProfileResource>  GetProfileByMemberIdAsync(Guid memberId);

        /// <summary>
        /// GetCountriesAsync
        /// </summary>
        Task<ICollection<CountryResource>> GetCountriesAsync();

        /// <summary>
        /// GetCountryRegionsAsync
        /// </summary>
        Task<ICollection<RegionResource>> GetCountryRegionsAsync(string courntyCode);

        /// <summary>
        /// GetAbimAsync
        /// </summary>
        Task<ProfileResource> GetProfileByAbimIdAsync(string abim_id);

        /// <summary>
        /// SearchProfilesByNPI
        /// </summary>
        /// <returns>Task ICollection VocProfileResource </returns>
        Task<ICollection<VocProfileResource>> SearchProfilesByNPI(string npi);

        /// <summary>
        /// GetVocByAbimIdAsync
        /// </summary>
        /// <param name="abim_id"></param>
        /// <returns></returns>
        Task<ICollection<VocProfileResource>> GetVocByAbimIdAsync(string abim_id);

        /// <summary>
        ///  GetNameAllAsync
        /// </summary>
        /// <param name="lastName"></param>
        /// <param name="firstName"></param>
        /// <param name="dob"></param>
        /// <param name="soundEx"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param> 
        /// <returns>Task ICollection VocProfileResource </returns>
        Task<ICollection<VocProfileResource>> GetNameAllAsync(string lastName, string firstName, DateTime? dob, bool soundEx, int pageSize, int pageIndex);

    }
}
