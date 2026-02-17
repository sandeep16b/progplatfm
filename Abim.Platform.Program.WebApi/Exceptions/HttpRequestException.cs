using NLog;
using System.Net.Http;
using System.Web.Http.Filters;

namespace Abim.Platform.Program.WebApi.Exceptions
{
    /// <summary>
    /// HttpRequest Exception Filter Attribute class for handling global exception
    /// -HttpRequestException is suppressed as warning
    /// </summary>
    public class HttpRequestExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// The base protected readonly logger
        /// </summary>
        protected readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception is HttpRequestException)
                Logger.Warn(actionExecutedContext);
            else
                base.OnException(actionExecutedContext);
        }
    }
}
