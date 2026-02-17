using Abim.Platform.Program.Relational.Services;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Authentication;
using Newtonsoft.Json;
using System;

namespace Abim.Platform.Program.App.Services.Commands
{
    /// <summary>
    /// Expires an existing Issuance
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Relational.Services.ICommand" />
    public class ExpireIssuanceCommand : ICommand
    {
        #region Properties

        /// <summary>
        /// Gets or sets the credential identifier.
        /// </summary>
        /// <value>
        /// The credential identifier.
        /// </value>
        [JsonIgnore]
        public Guid CredentialId { get; set; }

        /// <summary>
        /// Gets or sets the issuance identifier.
        /// </summary>
        /// <value>
        /// The issuance identifier.
        /// </value>
        public int IssuanceId { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public IssuanceStatusType Status { get; set; }

        /// <summary>
        /// Gets or sets the maintenance status.
        /// </summary>
        /// <value>
        /// The maintenance status.
        /// </value>
        public MaintenanceStatusType MaintenanceStatus { get; set; }

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
