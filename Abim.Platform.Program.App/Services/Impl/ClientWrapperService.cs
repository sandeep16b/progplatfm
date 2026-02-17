using Abim.Platform.Program.MembershipClient;
using Polly.Retry;
using Polly;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Abim.Platform.Program.Core.Identity;
using NLog;
using System.Net;
using System.Net.Http.Headers;

namespace Abim.Platform.Program.App.Services.Impl
{
    /// <summary>
    /// ClientWrapperService
    /// </summary>
    public class ClientWrapperService : IClientWrapperService
    {
        /// <summary>
        /// _httpClientFactory
        /// </summary>
        protected readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Gets or sets the Polly MaxRetryCount.
        /// </summary> 
        private int _maxRetryCount;

        /// <summary>
        /// accessTokenService
        /// </summary>
        private readonly IAccessTokenService _accessTokenService;

        /// <summary>
        /// _apiBaseUrl
        /// </summary>
        protected static string _apiBaseUrl;

        /// <summary>
        /// Logger
        /// </summary>
        /// <returns>ILogger</returns>
        protected ILogger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// ClientWrapperService
        /// </summary> 
        /// <param name="accessTokenService"></param>
        /// <param name="httpClientFactory"></param>
        /// <param name="apiBaseUrl"></param>
        /// <param name="pollyRetries"></param>
        public ClientWrapperService(IAccessTokenService accessTokenService, IHttpClientFactory httpClientFactory, string apiBaseUrl, int pollyRetries)
        {
            _apiBaseUrl = apiBaseUrl ?? throw new ArgumentNullException($"apiBaseUrl parameter is required.");
            _accessTokenService = accessTokenService ?? throw new ArgumentNullException($"accessTokenService");
            _maxRetryCount = pollyRetries;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// CreateClient
        /// </summary> 
        public HttpClient CreateHttpClient()
        {
            HttpClient httpClient = _httpClientFactory.CreateClient("HttpClientFactory");
            if (httpClient == null) { throw new Exception("Failed to create HttpClient."); }
            httpClient.BaseAddress = new Uri(_apiBaseUrl);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetAccessToken());
            return httpClient;
        }

        /// <summary>
        /// ExecuteCall using retryPolicy. 
        /// </summary>  
        public async Task<TResponse> ExecuteCall<TResponse>(Func<Task<TResponse>> httpRequestFunc)
        {
            Task<TResponse> response = default(Task<TResponse>);
            var retryPolicy = ReTryPolicy();
            await retryPolicy.ExecuteAsync(() => response = httpRequestFunc());
            return response.Result;
        }
        /// <summary> 
        /// Assign or ReAssign token to header. 
        /// </summary>
        public string GetAccessToken()
        {
            return _accessTokenService.GetAccessToken();
        }
        /// <summary>
        /// The RetryPolicy can be overridden in a subclass to implement custom policies.
        /// </summary> 
        public virtual RetryPolicy ReTryPolicy()
        {
            var retryPolicy = Policy.Handle<ApiException>(r => r.StatusCode == (int)HttpStatusCode.InternalServerError
                        || r.StatusCode == (int)HttpStatusCode.BadGateway
                        || r.StatusCode == (int)HttpStatusCode.ServiceUnavailable
                        || r.StatusCode == (int)HttpStatusCode.GatewayTimeout)
                        .RetryAsync(_maxRetryCount, (exception, retryCount) =>
                        {
                            Logger.Info($"Retry {retryCount} due to the {exception.Message}");
                        });
            return retryPolicy;
        }
    } 
  
}
  
 