


using Abim.Platform.Program.Relational.Services;
using Newtonsoft.Json;
using System;
using Abim.Platform.Program.WebApi.Authentication;

namespace Abim.Platform.Program.App.Services.Commands
{
    /// <summary>
    /// Command to Enroll in CMP pathway
    /// </summary>
    /// <seealso cref="ICommand" />
    public class EnrollInCMPCommand : ICommand
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
        public DateTime EnrollmentDate { get; set; }

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
