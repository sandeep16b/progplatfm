using System;

namespace ServiceBus.Events 
{
    /// <summary>
    /// MemberEnrolled Class.
    /// </summary>
    public class MemberEnrolled : IMemberEnrolled
    {
        /// <summary>
        /// Gets or sets the member identifier.
        /// </summary>
        public Guid MemberId { get; set; }

        /// <summary>
        /// Gets or sets the ABIM identifier
        /// </summary>
        public string AbimId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [re enrollment].
        /// </summary>
        public bool ReEnrollment { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether [cosponsored]
        /// </summary>
        public bool Cosponsored { get; set; } = false;

        /// <summary>
        /// Gets or sets the credential identifier.
        /// </summary>
        public Guid CredentialId { get; set; }
    }

    /// <summary>
    /// IMemberEnrolled.
    /// </summary>
    public interface IMemberEnrolled
    {
        /// <summary>
        /// Gets or sets the member identifier.
        /// </summary>
        Guid MemberId { get; set; }

        /// <summary>
        /// Gets or sets the ABIM identifier
        /// </summary>
        string AbimId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [re enrollment].
        /// </summary>
        bool ReEnrollment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [cosponsored]
        /// </summary>
        bool Cosponsored { get; set; }

        /// <summary>
        /// Gets or sets the credential identifier.
        /// </summary>
        Guid CredentialId { get; set; }
    }
}
