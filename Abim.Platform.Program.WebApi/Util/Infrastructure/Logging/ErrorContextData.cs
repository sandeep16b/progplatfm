using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.Util.Extensions;
using Abim.Platform.Program.WebApi.Authentication;
using Abim.Platform.Program.WebApi.Exceptions;
using Abim.Platform.Program.WebApi.Objects.Extensions;
using Newtonsoft.Json;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Abim.Platform.Program.WebApi.Objects.Logging
{
    /// <summary>
    /// Provides data for the context in which an error occurred
    /// </summary>
    public static class ErrorContextData
    {
        /// <summary>
        /// The prefix
        /// </summary>
        public const string Prefix = "[Error]";

        /// <summary>
        /// Turns an exception, and a request, into a full error-description json object
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public static string GetErrorDescription(Exception exception, HttpRequestMessage request)
        {
            return GetErrorDescription(null, exception, exception != null ? exception.GetType().ToString() : null, GlobalExceptionHandler.AppName,
                GlobalExceptionHandler.AppVersion, null, null, UserInfo.Create(request), request, null,
                request.GetRequestBody());
        }
        
        /// <summary>
        /// Turns a variety of error context parameters into a full error-description json object
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public static string GetErrorDescription(string message, Exception exception, string exceptionType, string appName,
            string appVersion, string stackTrace, string logLevel, UserInfo user = null, HttpRequestMessage request = null,
            HttpStatusCode? statusCodeResult = null, Object postData = null, Guid? interactionIdentifier = null)
        {
            Guid interaction;
            if(interactionIdentifier != null && !interactionIdentifier.Value.Equals(Guid.Empty))
                interaction = interactionIdentifier.Value;
            else interaction = Guid.NewGuid();
            if(message == null && exception != null) message = exception.Message;
            
            var metadata = new ContextMetadata()
            {
                Time = new ContextTimeMetadata()
                {
                    EventTime = SystemTime.UtcNow,
                    InteractionIdentifier = interaction
                },
                Server = new ContextServerMetadata()
                {
                    MachineName = Environment.MachineName,
                    ServerIP = IPReader.GetServerIP(request)
                },
                Application = new ContextApplicationMetadata()
                {
                    Application = appName,
                    Version = appVersion
                },
                Caller = new ContextOriginMetadata()
                {
                    IP = IPReader.GetClientIP(request),
                    User = user
                },
                ResourceRequested = new ContextResourceMetadata()
                {
                    Url = request.RequestUri.AbsoluteUri,
                    FullHeaders = ReadHeaders(request),
                    PostData = postData
                },
                Result = new ContextResultMetadata()
                {
                    ExceptionOccurrence = new ContextExceptionMetadata()
                    {
                        SourceFile = ParseSourceFile(stackTrace),
                        ExceptionType = exceptionType,
                        Exception = (exception == null ? null : exception.Stringify())
                    },
                    StatusCode = ParseStatusCode(statusCodeResult),
                    LogLevel = logLevel
                }
            };
            var formatted = string.Format("{0} {1} ... Metadata: {2}", Prefix, message, JsonConvert.SerializeObject(metadata));
            return formatted;
        }

        /// <summary>
        /// Reads the headers.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        private static Dictionary<string, string> ReadHeaders(HttpRequestMessage request)
        {
            var dict = new Dictionary<string, string>();
            foreach(var header in request.Headers)
            {
                string val = null;
                if(header.Value != null)
                {
                    if(header.Value.Count() == 1) val = header.Value.First();
                    else val = string.Join(", ", header.Value.ToArray());
                }
                dict[header.Key] = val;
            }
            return dict;
        }

        /// <summary>
        /// Parses the source file.
        /// </summary>
        /// <param name="stackTrace">The stack trace.</param>
        /// <returns></returns>
        private static string ParseSourceFile(string stackTrace)
        {
            if(stackTrace == null) return null;
            int index = stackTrace.IndexOf(".cs:line");
            if(index < 1) return null;
            string[] split = stackTrace.Substring(0, index).Split('\\');
            string sourceFile = split.Last() + ".cs";
            if(sourceFile.Length > 100) return null;        //sanity check
            return sourceFile;
        }

        /// <summary>
        /// Parses the status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <returns></returns>
        private static string ParseStatusCode(HttpStatusCode? statusCode)
        {
            if(statusCode == null) return null;
            return string.Format("{0} ({1})", EnumAttributes.ReadEnumInteger(statusCode), statusCode.Value);
        }
    }
}
