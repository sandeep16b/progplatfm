using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Abim.Platform.Program.WebApi.Response
{
    /// <summary>
    /// GoneContentResult
    /// </summary>
    public class GoneContentResult 
        : IHttpActionResult
    {
        /// <summary>
        /// The request
        /// </summary>
        private readonly HttpRequestMessage _request;

        /// <summary>
        /// Creates a new instance of the GoneContentResult class.
        /// </summary>
        /// <param name="request"></param>
        public GoneContentResult(HttpRequestMessage request)
        {
            _request = request;
        }

        /// <summary>
        /// ExecuteAsync method
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = _request.CreateResponse(HttpStatusCode.Gone);
            return Task.FromResult(response);
        }
    }
}