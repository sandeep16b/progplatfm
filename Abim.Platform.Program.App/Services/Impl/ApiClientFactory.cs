using Abim.Platform.Program.MembershipClient;
 using System.Net.Http;
 
namespace Abim.Platform.Program.App.Services.Impl
{
    /// <summary>
    /// ApiClientFactory
    /// </summary>
    public class ApiClientFactory : IApiClientFactory
    {
        /// <summary>
        ///  Gets new OpenAPI client
        /// </summary>
        /// <param name="baseUrl">BaseUrl</param>
        /// <param name="httpClient">httpClient with accesstoken.
        /// LifeCycle of the httpClient must be managed by the caller</param>
        /// <returns></returns>
        public IClient GetMembershipClient(string baseUrl, HttpClient httpClient)
        {
            return new Client(baseUrl, httpClient);
        }
    }
}
