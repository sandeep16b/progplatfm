using Abim.Platform.Program.WebApi.Authentication;
using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.WebApi.Objects.Logging
{
    /// <summary>
    /// Metadata for the context in which an error occurred.
    /// </summary>
    public struct ContextMetadata
    {
        /// <summary>
        /// Gets or sets the caller.
        /// </summary>
        /// <value>
        /// The caller.
        /// </value>
        public ContextOriginMetadata Caller { get; set; }

        /// <summary>
        /// Gets or sets the resource requested.
        /// </summary>
        /// <value>
        /// The resource requested.
        /// </value>
        public ContextResourceMetadata ResourceRequested { get; set; }

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>
        /// The time.
        /// </value>
        public ContextTimeMetadata Time { get; set; }
        
        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        public ContextResultMetadata Result { get; set; }

        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        /// <value>
        /// The server.
        /// </value>
        public ContextServerMetadata Server { get; set; }

        /// <summary>
        /// Gets or sets the application.
        /// </summary>
        /// <value>
        /// The application.
        /// </value>
        public ContextApplicationMetadata Application { get; set; }
    }

    /// <summary>
    /// Metadata about the caller whose request invoked the error
    /// </summary>
    public struct ContextOriginMetadata
    {
        /// <summary>
        /// Gets or sets the ip.
        /// </summary>
        /// <value>
        /// The ip.
        /// </value>
        public string IP { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        public UserInfo User { get; set; }
    }

    /// <summary>
    /// Metadata about the server processing the request
    /// </summary>
    public struct ContextServerMetadata
    {
        /// <summary>
        /// Gets or sets the name of the machine.
        /// </summary>
        /// <value>
        /// The name of the machine.
        /// </value>
        public string MachineName { get; set; }

        /// <summary>
        /// Gets or sets the server ip.
        /// </summary>
        /// <value>
        /// The server ip.
        /// </value>
        public string ServerIP { get; set; }
    }

    /// <summary>
    /// Metadata about the time and identification for the error event
    /// </summary>
    public struct ContextTimeMetadata
    {
        /// <summary>
        /// Gets or sets the event time.
        /// </summary>
        /// <value>
        /// The event time.
        /// </value>
        public DateTime EventTime { get; set; }

        /// <summary>
        /// Gets or sets the interaction identifier.
        /// </summary>
        /// <value>
        /// The interaction identifier.
        /// </value>
        public Guid InteractionIdentifier { get; set; }
    }

    /// <summary>
    /// Metadata about the application in which the error occurred
    /// </summary>
    public struct ContextApplicationMetadata
    {
        /// <summary>
        /// Gets or sets the application.
        /// </summary>
        /// <value>
        /// The application.
        /// </value>
        public string Application { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public string Version { get; set; }
    }

    /// <summary>
    /// Metadata about the specific resource being requested by the caller which invoked the error
    /// </summary>
    public struct ContextResourceMetadata
    {
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the full headers.
        /// </summary>
        /// <value>
        /// The full headers.
        /// </value>
        public Dictionary<string, string> FullHeaders { get; set; }

        /// <summary>
        /// Gets or sets the post data.
        /// </summary>
        /// <value>
        /// The post data.
        /// </value>
        public Object PostData { get; set; }
    }

    /// <summary>
    /// Metadata about the result returned to the user after the error occurred
    /// </summary>
    public struct ContextResultMetadata
    {
        /// <summary>
        /// Gets or sets the exception occurrence.
        /// </summary>
        /// <value>
        /// The exception occurrence.
        /// </value>
        public ContextExceptionMetadata ExceptionOccurrence { get; set; }

        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        /// <value>
        /// The status code.
        /// </value>
        public string StatusCode { get; set; }

        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        /// <value>
        /// The log level.
        /// </value>
        public string LogLevel { get; set; }
    }

    /// <summary>
    /// The error itself and its location and type
    /// </summary>
    public struct ContextExceptionMetadata
    {
        /// <summary>
        /// Gets or sets the source file.
        /// </summary>
        /// <value>
        /// The source file.
        /// </value>
        public string SourceFile { get; set; }

        /// <summary>
        /// Gets or sets the type of the exception.
        /// </summary>
        /// <value>
        /// The type of the exception.
        /// </value>
        public string ExceptionType { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public string Exception { get; set; }
    }
}
