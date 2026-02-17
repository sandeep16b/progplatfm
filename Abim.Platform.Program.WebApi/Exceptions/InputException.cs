using System;

namespace Abim.Platform.Program.WebApi.Exceptions
{
    /// <summary>
    /// Represents any exception that represents bad user input, for cases in which for some reason we want to throw an exception, but want our
    /// caller to know that it does not represent an internal server error
    /// </summary>
    public class InputException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InputException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InputException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public InputException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
