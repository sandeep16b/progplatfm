using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Abim.Platform.Program.WebApi.Response
{
    /// <summary>
    /// TextResult response class
    /// </summary>
    /// <seealso cref="System.Web.Http.IHttpActionResult" />
    public class TextResult : IHttpActionResult
    {
        /// <summary>
        /// The value
        /// </summary>
        private readonly string _value;

        /// <summary>
        /// The request
        /// </summary>
        private readonly HttpRequestMessage _request;

        /// <summary>
        /// The status code
        /// </summary>
        private readonly HttpStatusCode _statusCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextResult"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="request">The request.</param>
        /// <param name="statusCode">The status code.</param>
        public TextResult(string value, HttpRequestMessage request, HttpStatusCode statusCode)
        {
            _value = value;
            _request = request;
            _statusCode = statusCode;
        }

        /// <summary>
        /// Creates an <see cref="T:System.Net.Http.HttpResponseMessage" /> asynchronously.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task that, when completed, contains the <see cref="T:System.Net.Http.HttpResponseMessage" />.
        /// </returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_value),
                RequestMessage = _request
            };
            return Task.FromResult(response);
        }
    }
}