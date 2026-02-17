using System;

namespace Abim.Platform.Program.WebApi.Objects.Logging
{
    /// <summary>
    /// BasicContextMetadata. A shortened fallback, or space-saving alternative, to ContextMetadata
    /// </summary>
    public class BasicContextMetadata
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the request URI.
        /// </summary>
        /// <value>
        /// The request URI.
        /// </value>
        public Uri RequestUri { get; set; }

        /// <summary>
        /// Gets the date time.
        /// </summary>
        /// <value>
        /// The date time.
        /// </value>
        public DateTime DateTime { get; private set; }

        /// <summary>
        /// Gets the error identifier.
        /// </summary>
        /// <value>
        /// The error identifier.
        /// </value>
        public Guid ErrorId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicContextMetadata"/> class.
        /// </summary>
        public BasicContextMetadata()
        {
            DateTime = DateTime.Now;
            ErrorId  = Guid.NewGuid();
        }
    }
}