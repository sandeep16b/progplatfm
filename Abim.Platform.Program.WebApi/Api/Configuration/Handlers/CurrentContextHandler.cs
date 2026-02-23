using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Abim.Platform.Program.WebApi.Api.Handlers
{
    /// <summary>
    /// CurrentContextHandler Class
    /// </summary>
    public class CurrentContextHandler : DelegatingHandler
    {
        /// <summary>
        /// Adds the HttpRequestMessage to the Context.Items dictionary, to mimic WebApi2
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1" />. The task object representing the asynchronous operation.
        /// </returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if(HttpContext.Current != null && HttpContext.Current.Items != null)
            {
                try
                {
                    HttpContext.Current.Items["MS_HttpRequestMessage"] = request;
                }
                catch(Exception ex)
                {
                }
            }
            
            return base.SendAsync(request, cancellationToken);
        }
    }
}