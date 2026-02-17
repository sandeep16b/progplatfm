using Abim.Platform.Program.MembershipClient;
using System.Net.Http; 

namespace Abim.Platform.Program.App.Services
{
    /// <summary>
    /// GetMembershipClient
    /// </summary> 
    public interface IApiClientFactory
    {
        /// <param name="baseUrl">baseUrl.</param>
        /// <param name="httpClient">httpClient.</param>
        IClient GetMembershipClient(string baseUrl, HttpClient httpClient);
    }
}
