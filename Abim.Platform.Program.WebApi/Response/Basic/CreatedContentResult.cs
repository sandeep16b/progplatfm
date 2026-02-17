using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Abim.Platform.Program.WebApi.Response
{
    /// <summary>
    /// CreatedContentActionResult Class.
    /// </summary>
    public class CreatedContentActionResult 
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
        /// Creates a new instance of the CreatedContentActionResult class.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="location"></param>
        public CreatedContentActionResult(HttpRequestMessage request, string location)
        {
            _request = request;
            _location = location;
        }
        
        /// <summary>
        /// ExecuteAsync method.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = _request.CreateResponse(HttpStatusCode.Created);
            response.Headers.Location = new Uri(_location);
            return Task.FromResult(response);
        }
    }
}
