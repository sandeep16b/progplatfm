using Abim.Platform.Program.Relational.Services;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Authentication;
using Newtonsoft.Json;
using System;

namespace Abim.Platform.Program.App.Services.Commands
{
    /// <summary>
    /// Command to Add Credential Command
    /// </summary>
    /// <seealso cref="ICommand" />
    public class AddCredentialCommand: ICommand
    {

        #region Properties

        /// <summary>
        /// Gets or sets the member identifier.
        /// </summary>
        /// <value>
        /// The member identifier.
        /// </value>
        public Guid MemberId { get; set; }

        /// <summary>
        /// Gets or sets the certification identifier.
        /// </summary>
        /// <value>
        /// The certification identifier.
        /// </value>
        public Guid CertificationId { get; set; }

        /// <summary>
        /// Gets or sets the pathway.
        /// </summary>
        /// <value>
        /// The pathway.
        /// </value>
        public PathwayType Pathway { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public CredentialType Type { get; set; }

        /// <summary>
        /// Gets or sets the user information.
        /// </summary>
        /// <value>
        /// The user information.
        /// </value>
        [JsonIgnore]
        public UserInfo UserInfo { get; set; }

        #endregion

    }
}