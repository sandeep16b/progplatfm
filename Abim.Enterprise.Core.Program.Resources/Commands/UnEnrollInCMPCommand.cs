using System;

namespace Abim.Platform.Program.Resources.Commands
{
    /// <summary>
    /// Command to UnEnroll in CMP pathway
    /// </summary>
    /// <seealso cref="ICommand" />
    public class UnEnrollInCMPCommand : ICommand
    {
        /// <summary>
        /// Gets or sets the Abim Id
        /// </summary>
        /// <value>
        /// The issuance date.
        /// </value>
        public string AbimId { get; set; }

        /// <summary>
        /// Gets or sets the Member Id
        /// </summary>
        /// <value>
        /// The issuance date.
        /// </value>
        public Guid MemberId { get; set; }

        /// <summary>
        /// Gets or sets the Subspecialty Certification Code
        /// </summary>
        public string SubspecialtyCertCode { get; set; }

        /// <summary>
        /// Gets or sets the UnEnrollment Effective date.
        /// </summary>
        /// <value>
        /// The issuance date.
        /// </value>
        public DateTime UnEnrollmentDate { get; set; }

        /// <summary>
        /// Gets or sets the Requesting User Name.
        /// </summary>
        /// <value>
        /// The Requesting Client Name.
        /// </value>
        public string RequestingUserName { get; set; }
    }
}
