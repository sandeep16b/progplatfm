using Abim.Platform.Program.Relational.Services;
using Abim.Platform.Program.WebApi.Authentication;
using Newtonsoft.Json;
using System;

namespace Abim.Platform.Program.App.Services.Commands
{
    /// <summary>
    /// ommand to UnEnroll in CMP pathway
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
        /// Gets or sets the Enrollment Effective date.
        /// </summary>
        /// <value>
        /// The issuance date.
        /// </value>
        public DateTime UnEnrollmentDate { get; set; }

        /// <summary>
        /// Gets or sets the Requesting Client Name.
        /// </summary>
        /// <value>
        /// The Requesting Client Name.
        /// </value>
        public string RequestingUserName { get; set; }

        /// <summary>
        /// Gets or sets the user information.
        /// </summary>
        /// <value>
        /// The user information.
        /// </value>
        [JsonIgnore]
        public UserInfo UserInfo { get; set; }
    }
}
