using Abim.Platform.Program.Util.Extensions;
using Abim.Platform.Program.WebApi.Authentication;
using Abim.Platform.Program.WebApi.Exceptions;
using Abim.Platform.Program.WebApi.Objects.Logging;
using Abim.Platform.Program.WebApi.Util.Infrastructure.Email;
using NLog;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Abim.Platform.Program.WebApi.Extensions
{
    /// <summary>
    /// Extension class
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Traces the with metadata.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="request">The request.</param>
        /// <param name="statusCodeResult">The status code result.</param>
        /// <param name="postData">The post data.</param>
        /// <param name="interactionIdentifier">The interaction identifier.</param>
        /// <returns></returns>
        public static string TraceWithMetadata(this Logger logger, string message,
            //other parameters (same in every metadata method here)
            HttpRequestMessage request = null,
            HttpStatusCode? statusCodeResult = HttpStatusCode.InternalServerError, Object postData = null, Guid? interactionIdentifier = null)
        {
            request = FillRequest(request);
            UserInfo user = FillUser(request);
            string fullMessage = ErrorContextData.GetErrorDescription(message, null, "code check for error condition", GlobalExceptionHandler.AppName,
                GlobalExceptionHandler.AppVersion, null, "Trace", user, request, statusCodeResult, postData, interactionIdentifier);
            logger.Trace(fullMessage);
            return fullMessage;
        }

        /// <summary>
        /// Errors the with metadata.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="request">The request.</param>
        /// <param name="statusCodeResult">The status code result.</param>
        /// <param name="postData">The post data.</param>
        /// <param name="interactionIdentifier">The interaction identifier.</param>
        /// <returns></returns>
        public static string ErrorWithMetadata(this Logger logger, string message,
            //other parameters (same in every metadata method here)
            HttpRequestMessage request = null,
            HttpStatusCode? statusCodeResult = HttpStatusCode.InternalServerError, Object postData = null, Guid? interactionIdentifier = null)
        {
            request = FillRequest(request);
            UserInfo user = FillUser(request);
            string fullMessage = ErrorContextData.GetErrorDescription(message, null, "code check for error condition", GlobalExceptionHandler.AppName,
                GlobalExceptionHandler.AppVersion, null, "Error", user, request, statusCodeResult, postData, interactionIdentifier);
            logger.Error(fullMessage);
            return fullMessage;
        }

        /// <summary>
        /// Errors the with metadata.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="request">The request.</param>
        /// <param name="statusCodeResult">The status code result.</param>
        /// <param name="postData">The post data.</param>
        /// <param name="interactionIdentifier">The interaction identifier.</param>
        /// <returns></returns>
        public static string ErrorWithMetadata(this Logger logger, string message, Exception ex,
            //other parameters (same in every metadata method here)
            HttpRequestMessage request = null,
            HttpStatusCode? statusCodeResult = HttpStatusCode.InternalServerError, Object postData = null, Guid? interactionIdentifier = null)
        {
            request = FillRequest(request);
            UserInfo user = FillUser(request);
            string fullMessage = ErrorContextData.GetErrorDescription(message + ": " + ex.Message, ex, ex.GetType().ToString(),
                GlobalExceptionHandler.AppName, GlobalExceptionHandler.AppVersion, ex.StackTrace, "Error", user, request, statusCodeResult,
                postData, interactionIdentifier);
            logger.Error(fullMessage);
            return fullMessage;
        }

        /// <summary>
        /// Errors the with metadata.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="request">The request.</param>
        /// <param name="statusCodeResult">The status code result.</param>
        /// <param name="postData">The post data.</param>
        /// <param name="interactionIdentifier">The interaction identifier.</param>
        /// <returns></returns>
        public static string ErrorWithMetadata(this Logger logger, Exception ex,
            //other parameters (same in every metadata method here)
            HttpRequestMessage request = null,
            HttpStatusCode? statusCodeResult = HttpStatusCode.InternalServerError, Object postData = null, Guid? interactionIdentifier = null)
        {
            request = FillRequest(request);
            UserInfo user = FillUser(request);
            string fullMessage = ErrorContextData.GetErrorDescription(ex.Message, ex, ex.GetType().ToString(), GlobalExceptionHandler.AppName,
                GlobalExceptionHandler.AppVersion, ex.StackTrace, "Error", user, request, statusCodeResult, postData, interactionIdentifier);
            Exception newException = new Exception(fullMessage, ex);
            logger.Error(newException);
            return fullMessage;
        }

        /// <summary>
        /// Fills the request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        private static HttpRequestMessage FillRequest(HttpRequestMessage request = null)
        {
            if(request != null) return request;
            try
            {
                if(HttpContext.Current == null) return null;
                if(!HttpContext.Current.Items.Contains("MS_HttpRequestMessage")) return null;
                return HttpContext.Current.Items["MS_HttpRequestMessage"] as HttpRequestMessage;
            }
            #pragma warning disable 0168
            catch(Exception ex)
            {
            }
            return null;
        }

        /// <summary>
        /// Fills the user.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        private static UserInfo FillUser(HttpRequestMessage request)
        {
            if(request == null) return null;
            try
            {
                return UserInfo.Create(request);
            }
            catch(Exception ex)
            {
            }
            return null;
        }

        /// <summary>
        /// Flushes all logs, without awaiting the call. Normally LogManager.Flush() takes over a second and blocks the calling thread
        /// </summary>
        /// <param name="logger">The logger.</param>
        public static void FlushAsync(this ILogger logger)
        {
            Task.Run(() => { LogManager.Flush(TimeSpan.FromMinutes(2)); });
        }

        /// <summary>
        /// Flushes all logs
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns></returns>
        public static bool TryFlush(this ILogger logger)
        {
            try
            {
                LogManager.Flush(TimeSpan.FromMinutes(2));
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Logs a fatal error and sends out an error email.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="synopsis">The synopsis.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="environment">The environment, if known.</param>
        public static void FatalError(this ILogger logger, string synopsis, Exception ex, string environment = null)
        {
            Fatal(logger, synopsis, ex, environment);
        }

        /// <summary>
        /// Logs a fatal error and sends out an error email.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="synopsis">The synopsis.</param>
        /// <param name="ex">The ex.</param>
        public static void FatalError(this ILogger logger, string synopsis, Exception ex)
        {
            Fatal(logger, synopsis, ex);
        }

        /// <summary>
        /// Logs a fatal error and sends out an error email.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The message.</param>
        public static void FatalError(this ILogger logger, string message)
        {
            Fatal(logger, message, null);
        }

        /// <summary>
        /// Logs a fatal error and sends out an error email.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ex">The ex.</param>
        public static void FatalError(this ILogger logger, Exception ex)
        {
            Fatal(logger, null, ex);
        }

        /// <summary>
        /// Logs a fatal error and sends out an error email.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="synopsis">The synopsis.</param>
        /// <param name="exception">The exception.</param>
        /// <param name="environment">The environment, if known.</param>
        private static void Fatal(this ILogger logger, string synopsis, Exception exception, string environment = null)
        {
            //perform the basic log action
            if(exception != null)
                logger.Fatal(exception);
            else
                logger.Fatal(synopsis);
            
            //send an error email
            string message;
            if(exception == null)
                message = synopsis;
            else
            {
                if(synopsis == null)
                    synopsis = "A fatal exception has occurred:\r\n" + exception.Message;
                message = string.Format("{0}\r\n\r\nFull error:\r\n{1}", synopsis, exception.Stringify());
            }
            try
            {
                SendEmail.FatalErrorEmail(message, environment);
            }
            catch(Exception sendEmailException)
            {
                logger.Error("Error sending error email: " + sendEmailException.Stringify());
            }
        }
    }
}
