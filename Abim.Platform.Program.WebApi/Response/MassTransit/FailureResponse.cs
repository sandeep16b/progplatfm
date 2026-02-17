namespace Abim.Platform.Program.WebApi.Response.MassTransit
{
    /// <summary>
    /// A failure response for RabbitMQ
    /// </summary>
    public class FailureResponse
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="FailureResponse"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public FailureResponse(string message)
        {
            Message = message;
        }
    }
}
