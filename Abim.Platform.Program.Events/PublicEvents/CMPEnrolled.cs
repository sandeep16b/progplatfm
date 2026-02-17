using System;

namespace ServiceBus.Events
{
    public class CMPEnrolled : ICMPEnrolled
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
        /// EnrollmentDate
        /// </summary>
        public DateTime EnrollmentDate { get; set; }

        /// <summary>
        /// ProcessingDate
        /// </summary>
        public DateTime ProcessingDate { get; set; }
    }

    public interface ICMPEnrolled
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
        /// EnrollmentDate
        /// </summary>
        DateTime EnrollmentDate { get; set; }

        /// <summary>
        /// ProcessingDate
        /// </summary>
        DateTime ProcessingDate { get; set; }
    }

}
