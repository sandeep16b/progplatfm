using Abim.Platform.Program.Relational.Services;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Authentication;
using Newtonsoft.Json;
using System;

namespace Abim.Platform.Program.App.Services.Commands
{
    /// <summary>
    /// Command to Update Issuance Command
    /// </summary>
    /// <seealso cref="ICommand" />
    public class UpdateIssuanceCommand : ICommand
    {

        #region Properties

        /// <summary>
        /// Gets or sets the credential identifier.
        /// </summary>
        /// <value>
        /// The member identifier.
        /// </value>
        public Guid CredentialId { get; set; }

        /// <summary>
        /// Gets or sets the IssuanceDate
        /// </summary>
        public DateTime IssuanceDate { get; set; }

        /// <summary>
        /// Gets or sets the EffectiveDate
        /// </summary>
        public DateTime EffectiveDate { get; set; }

        /// <summary>
        /// Gets or sets the Duration
        /// </summary>
        public DurationType Duration { get; set; }

        /// <summary>
        /// Gets or sets the ExpirationDate
        /// </summary>
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Gets or sets the MaintenanceRequirement
        /// </summary>
        public MaintenanceRequirementType MaintenanceRequirement { get; set; }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public virtual MaintenanceStatusType MaintenanceStatus { get; set; }

        /// <summary>
        /// Gets the issuance status.
        /// </summary>
        /// <value>
        /// The issuance status.
        /// </value>
        public virtual IssuanceStatusType IssuanceStatus { get; set; }

        /// <summary>
        /// Gets or sets the Occurrence
        /// </summary>
        public OccurrenceType Occurrence { get; set; }

        /// <summary>
        /// Gets or sets the SourceId
        /// </summary>
        public Guid SourceId { get; set; }

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