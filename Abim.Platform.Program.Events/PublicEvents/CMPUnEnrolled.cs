using System;

namespace ServiceBus.Events
{
    public class CMPUnEnrolled : ICMPUnEnrolled
    {
        /// <summary>
        /// MemberId
        /// </summary>
        public Guid MemberId { get; set; }

        /// <summary>
        /// CredentialGuid
        /// </summary>
        public Guid CredentialGuid { get; set; }

        /// <summary>
        /// UnEnrollmentDate
        /// </summary>
        public DateTime UnEnrollmentDate { get; set; }

        /// <summary>
        /// Voluntary
        /// </summary>
        public Boolean Voluntary { get; set; }

        /// <summary>
        /// ProcessingDate
        /// </summary>
        public DateTime ProcessingDate { get; set; }
    }

    public interface ICMPUnEnrolled
    {
        /// <summary>
        /// MemberId
        /// </summary>
        Guid MemberId { get; set; }

        /// <summary>
        /// CredentialGuid
        /// </summary>
        Guid CredentialGuid { get; set; }

        /// <summary>
        /// UnEnrollmentDate
        /// </summary>
        DateTime UnEnrollmentDate { get; set; }

        /// <summary>
        /// Voluntary
        /// </summary>
        Boolean Voluntary { get; set; }

        /// <summary>
        /// ProcessingDate
        /// </summary>
        DateTime ProcessingDate { get; set; }
    }
}
