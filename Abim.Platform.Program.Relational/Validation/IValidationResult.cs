
namespace Abim.Platform.Program.Relational.Validation
{
    /// <summary>
    /// IValidationResult interface
    /// </summary>
    public interface IValidationResult
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        string Message { get; set; }
    }

    /// <summary>
    /// IValidationResult interface
    /// </summary>
    public class ValidationErrorResult : IValidationResult
    {
        /// <summary>
        /// Gets or sets the property.
        /// </summary>
        /// <value>
        /// The property.
        /// </value>
        public string Property { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }
    }
}
