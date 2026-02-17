using System;
using Abim.Platform.Program.WebApi.Objects.Logging;
using System.Net;
using System.Net.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace Abim.Platform.Program.WebApi.Exceptions
{
    /// <summary>
    /// GlobalExceptionHandler Class.
    /// </summary>
    public class GlobalExceptionHandler
        : ExceptionHandler
    {
        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>
        /// The name of the application.
        /// </value>
        public static string AppName { get; set; }

        /// <summary>
        /// Gets or sets the application version.
        /// </summary>
        /// <value>
        /// The application version.
        /// </value>
        public static string AppVersion { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalExceptionHandler"/> class.
        /// </summary>
        public GlobalExceptionHandler()
        {
        }

        /// <summary>
        /// Handle method.
        /// </summary>
        /// <param name="context"></param>
        public override void Handle(ExceptionHandlerContext context)
        {
            HttpResponseMessage response;
            try
            {
                var metadata = ErrorContextData.GetErrorDescription(context.Exception, context.Request);
                response = context.Request.CreateResponse(HttpStatusCode.InternalServerError, metadata);
            }
            #pragma warning disable 0168
            catch(Exception ex)
            {
                var simpleMetadata = new BasicContextMetadata
                {
                    Message    = ErrorMessages.UnexpectedError,
                    RequestUri = context.Request.RequestUri
                };
                response = context.Request.CreateResponse(HttpStatusCode.InternalServerError, simpleMetadata);
            }
            context.Result = new ResponseMessageResult(response);
        }
    }
}
