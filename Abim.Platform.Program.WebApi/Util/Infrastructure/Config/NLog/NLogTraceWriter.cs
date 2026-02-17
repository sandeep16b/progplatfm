using Abim.Platform.Program.Util.Extensions;
using NLog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web.Http.Tracing;

namespace Abim.Platform.Program.WebApi.NLog
{
    /// <summary>
    /// NLogTraceWriter Class.
    /// </summary>
    public sealed class NLogTraceWriter : ITraceWriter
    {
        /// <summary>
        /// The class logger
        /// </summary>
        private static readonly Logger ClassLogger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The logging map
        /// </summary>
        private static readonly Lazy<Dictionary<TraceLevel, Action<string>>> LoggingMap =
            new Lazy<Dictionary<TraceLevel, Action<string>>>(() => new Dictionary<TraceLevel, Action<string>>
                {
                    // See https://blogs.msdn.microsoft.com/roncain/2012/04/12/tracing-in-asp-net-web-api/ for this quote:
                    // "The WebApi product code currently traces normal activity at Info level."
                    {TraceLevel.Info,  ClassLogger.Trace},
                    {TraceLevel.Debug, ClassLogger.Trace},

                    // [ibid]
                    // "... All exceptions caught by the product tracing code traces at the Error level."
                    {TraceLevel.Error, ClassLogger.Error},
                    {TraceLevel.Fatal, ClassLogger.Fatal},

                    // [ibid]
                    // "... Situations that *may* cause a downstream failure, such as a model binding error, trace at the Warn level."
                    {TraceLevel.Warn,  ClassLogger.Warn}
                });

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        private Dictionary<TraceLevel, Action<string>> Logger
        {
            get { return LoggingMap.Value; }
        }

        /// <summary>
        /// Invokes the specified traceAction to allow setting values in a new <see cref="T:System.Web.Http.Tracing.TraceRecord" /> if and only if tracing is permitted at the given category and level.
        /// </summary>
        /// <param name="request">The current <see cref="T:System.Net.Http.HttpRequestMessage" />.   It may be null but doing so will prevent subsequent trace analysis  from correlating the trace to a particular request.</param>
        /// <param name="category">The logical category for the trace.  Users can define their own.</param>
        /// <param name="level">The <see cref="T:System.Web.Http.Tracing.TraceLevel" /> at which to write this trace.</param>
        /// <param name="traceAction">The action to invoke if tracing is enabled.  The caller is expected to fill in the fields of the given <see cref="T:System.Web.Http.Tracing.TraceRecord" /> in this action.</param>
        public void Trace(HttpRequestMessage request, string category, TraceLevel level, Action<TraceRecord> traceAction)
        {
            if(level != TraceLevel.Off)
            {
                var record = new TraceRecord(request, category, level);
                traceAction(record);
                Log(record);
            }
        }

        /// <summary>
        /// Logs the specified record.
        /// </summary>
        /// <param name="record">The record.</param>
        private void Log(TraceRecord record)
        {
            var message = new StringBuilder();
            
            if(record.Request != null)
            {
                if(record.Request.Method != null)
                    message.Append(record.Request.Method);
                
                if(record.Request.RequestUri != null)
                    message.Append(" ").Append(record.Request.RequestUri);
            }
            
            if(!string.IsNullOrWhiteSpace(record.Category))
                message.Append(" ").Append(record.Category);
            
            if(!string.IsNullOrWhiteSpace(record.Operator))
                message.Append(" ").Append(record.Operator).Append(" ").Append(record.Operation);
            
            if(!string.IsNullOrWhiteSpace(record.Message))
                message.Append(" ").Append(record.Message);
            
            if(record.Exception != null && !string.IsNullOrWhiteSpace(record.Exception.GetBaseException().Message))
                message.Append(" ").Append(record.Exception.GetBaseException().Message);
            
            bool typeLoadException =
                (record.Message != null && record.Message.ToLower().Contains("retrieve the loaderexceptions")) ||
                (record.Exception != null && (record.Exception is ReflectionTypeLoadException || record.Exception is TypeLoadException ||
                    (record.Exception.Message != null && record.Exception.Message.ToLower().Contains("retrieve the loaderexceptions"))));
            if(typeLoadException)
                message.Append($"  Full exception details: {record.Exception.Stringify()}");
            
            Logger[record.Level](message.ToString());
        }
    }
}
