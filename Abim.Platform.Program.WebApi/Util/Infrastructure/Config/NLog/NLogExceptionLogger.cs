using NLog;
using System.Net.Http;
using System.Text;
using System.Web.Http.ExceptionHandling;

namespace Abim.Platform.Program.WebApi.NLog
{
    /// <summary>
    /// NLogExceptionLogger class.
    /// </summary>
    public class NLogExceptionLogger : ExceptionLogger
    {
        /// <summary>
        /// The nlog
        /// </summary>
        private static readonly Logger Nlog = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// When overridden in a derived class, logs the exception synchronously.
        /// </summary>
        /// <param name="context">The exception logger context.</param>
        public override void Log(ExceptionLoggerContext context)
        {
            Nlog.Log(LogLevel.Error, context.Exception, RequestToString(context.Request));
        }

        /// <summary>
        /// Request to string method.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        private static string RequestToString(HttpRequestMessage request)
        {
            var message = new StringBuilder();
            if(request.Method != null)
            {
                message.Append(request.Method);
            }

            if(request.RequestUri != null)
            {
                message.Append(" ").Append(request.RequestUri);
            }

            return message.ToString();
        }
    }
}
