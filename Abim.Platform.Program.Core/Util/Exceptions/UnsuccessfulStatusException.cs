using System;
using System.Net;

namespace Abim.Platform.Program.Utils.Exceptions
{
    /// <summary>
    /// UnsuccessfulStatusException class
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class UnsuccessfulStatusException : Exception
    {
        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        /// <value>
        /// The status code.
        /// </value>
        public HttpStatusCode StatusCode { get; set; }
        
        /// <summary>
        /// Gets or sets the response message.
        /// </summary>
        /// <value>
        /// The response message.
        /// </value>
        public string ResponseMessage { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsuccessfulStatusException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UnsuccessfulStatusException(string message)
            : base(message)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsuccessfulStatusException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public UnsuccessfulStatusException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
