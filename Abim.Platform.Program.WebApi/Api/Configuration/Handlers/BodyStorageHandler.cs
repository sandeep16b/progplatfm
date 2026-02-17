using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Abim.Platform.Program.WebApi.Api.Handlers
{
    /// <summary>
    /// Stores the original posted body
    /// </summary>
    /// <seealso cref="System.Net.Http.DelegatingHandler" />
    public class BodyStorageHandler : DelegatingHandler
    {
        /// <summary>
        /// Sends an HTTP request to the inner handler to send to the server as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to send to the server.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns>
        /// Returns <see cref="T:System.Threading.Tasks.Task`1" />. The task object representing the asynchronous operation.
        /// </returns>
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if(request.Content != null)
            {
                string body = await request.Content.ReadAsStringAsync();
                request.Properties["body"] = body;
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
