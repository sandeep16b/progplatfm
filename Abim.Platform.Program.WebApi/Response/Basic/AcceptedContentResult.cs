using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Abim.Platform.Program.WebApi.Response
{
    /// <summary>
    /// AcceptedContentResult Class
    /// </summary>
    public class AcceptedContentResult
        : IHttpActionResult
    {
        /// <summary>
        /// The request
        /// </summary>
        private readonly HttpRequestMessage _request;

        /// <summary>
        /// The location
        /// </summary>
        private readonly string _location;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="location"></param>
        public AcceptedContentResult(HttpRequestMessage request, string location)
        {
            _request = request;
            _location = location;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = _request.CreateResponse(HttpStatusCode.Accepted);
            response.Headers.Location = new Uri(_location);
            return Task.FromResult(response);
        }
    }
}