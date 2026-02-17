using System;

namespace ServiceBus.Events
{
    /// <summary>
    /// MemberUnEnrolledEvent Class.
    /// </summary>
    public class MemberUnenrolled : IMemberUnenrolled
    {
        /// <summary>
        /// Gets or sets the member identifier.
        /// </summary>
        public Guid MemberId { get; set; }

        /// <summary>
        /// The ABIM identifier
        /// </summary>
        public string AbimId { get; set; }

        /// <summary>
        /// The cosponsored flag
        /// </summary>
        public bool Cosponsored { get; set; }

        /// <summary>
        /// Gets or sets the credential identifier.
        /// </summary>
        public Guid CredentialId { get; set; }
    }

    /// <summary>
    /// IMemberUnEnrolledEvent.
    /// </summary>
    public interface IMemberUnenrolled
    {
        /// <summary>
        /// Gets or sets the member identifier.
        /// </summary>
        Guid MemberId { get; set; }

        /// <summary>
        /// The ABIM identifier
        /// </summary>
        string AbimId { get; set; }

        /// <summary>
        /// The cosponsored flag
        /// </summary>
        bool Cosponsored { get; set; }

        /// <summary>
        /// Gets or sets the credential identifier.
        /// </summary>
        Guid CredentialId { get; set; }
    }
}
