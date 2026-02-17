using Abim.Platform.Program.WebApi.Objects.Logging;
using System.Net;
using System.Net.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace Abim.Platform.Program.WebApi.Exceptions
{
    /// <summary>
    /// ContentNegotiatedExceptionHandler class.
    /// </summary>
    public class ContentNegotiatedExceptionHandler : ExceptionHandler
    {
        /// <summary>
        /// When overridden in a derived class, handles the exception synchronously.
        /// </summary>
        /// <param name="context">The exception handler context.</param>
        public override void Handle(ExceptionHandlerContext context)
        {
            var errorData = new BasicContextMetadata
            {
                Message    = "An unexpected error occurred! Please use the ticket ID to contact support",
                RequestUri = context.Request.RequestUri
            };
            var response   = context.Request.CreateResponse(HttpStatusCode.InternalServerError, errorData);
            context.Result = new ResponseMessageResult(response);
        }
    }
}