using System;

namespace Abim.Platform.Program.WebApi.Exceptions
{
    /// <summary>
    /// The purpose of this base class is that we can catch(UserProfileException) without caring the particular subtype, and treat it differently
    /// than a general Exception, e.g. returning BadRequest instead of InternalServerError. Because of this we want to restrict the usage of
    /// UserProfileExceptions to those cases that do not constitute server errors
    /// </summary>
    /// <seealso cref="System.Exception" />
    public abstract class UserProfileException : InputException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UserProfileException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public UserProfileException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    /// <summary>
    /// The profile was either not found by Guid in the database, or a request through the Interservice to fetch it returned NotFound
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.WebApi.Exceptions.UserProfileException" />
    public class UserProfileNotFoundException : UserProfileException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UserProfileNotFoundException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public UserProfileNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    /// <summary>
    /// The user is marked as deceased
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.WebApi.Exceptions.UserProfileException" />
    public class UserProfileMarkedDeceasedException : UserProfileException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileMarkedDeceasedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UserProfileMarkedDeceasedException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileMarkedDeceasedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner.</param>
        public UserProfileMarkedDeceasedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

