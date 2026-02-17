using System.Net;
using System.Net.Http;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Abim.Platform.Program.WebApi.Api.Handlers
{
    /// <summary>
    /// Handles preflight options requests
    /// </summary>
    /// <seealso cref="System.Net.Http.DelegatingHandler" />
    public class PreflightRequestsHandler : DelegatingHandler
    {
        /// <summary>
        /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1" />. The task object representing the asynchronous operation.
        /// </returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var hasOriginHeader = request.Headers.Contains("Origin");
            if(hasOriginHeader && request.Headers.GetValues("Origin").First().StartsWith("chrome-extension://"))
                hasOriginHeader = false;          //ignore fake origin headers
            if(!hasOriginHeader || !request.Method.Method.Equals("OPTIONS") || request.Headers.Contains("Authorization") ||
                    (request.Headers.Contains("Explicit") && request.Headers.GetValues("Explicit").First().ToLower() == "true") ||
                    (request.Headers.Contains("explicit") && request.Headers.GetValues("explicit").First().ToLower() == "true"))
                return base.SendAsync(request, cancellationToken);
            
            if(NonPreflightPath(request.RequestUri.AbsoluteUri))
                return base.SendAsync(request, cancellationToken);
            
            var response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
            
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Credentials", "true");
            response.Headers.Add("Access-Control-Allow-Headers", "Authorization,Access-Control-Allow-Headers,Origin,Accept,X-Requested-With,Content-Type,Access-Control-Request-Method,Access-Control-Request-Headers,x-impersonateas");
            response.Headers.Add("Access-Control-Allow-Methods", "GET,OPTIONS,POST,PUT,DELETE,PATCH");
            response.Headers.Add("Access-Control-Max-Age", "86400");
            
            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            tsc.SetResult(response);
            return tsc.Task;
        }

        /// <summary>
        /// Can be overridden to specify that certain Urls should never be treated as empty-200 preflights
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        protected virtual bool NonPreflightPath(string uri)
        {
            return false;
        }
    }
}
