using Abim.Platform.Program.Relational.Services;
using Abim.Platform.Program.Resources;
using Embarr.WebAPI.AntiXss;
using Newtonsoft.Json;
using System;

namespace Abim.Platform.Program.App.Services.Commands
{
    /// <summary>
    /// Issues a new issuance for FPHM
    /// </summary>
    public class IssueFPHMCommand : ICommand
    {
        /// <summary>
        /// CredentialId
        /// </summary>
        [JsonIgnore]
        public Guid CredentialId { get; set; }

        /// <summary>
        /// Gets or sets the issuance date.
        /// </summary>
        /// <value>
        /// The issuance date.
        /// </value>
        public DateTime IssuanceDate { get; set; }

        /// <summary>
        /// Gets or sets the maintenance status.
        /// </summary>
        /// <value>
        /// The maintenance status.
        /// </value>
        public MaintenanceStatusType MaintenanceStatus { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>
        /// The created by.
        /// </value>
        [AntiXss]  //all public string properties with setters in our command classes get this attribute
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the scheduled update.
        /// </summary>
        /// <value>
        /// The scheduled update.
        /// </value>
        public DateTime ScheduledUpdate { get; set; }

        /// <summary>
        /// Gets or sets the re Attestation due date.
        /// </summary>
        /// <value>
        /// The Re Attestation Due Date.
        /// </value>
        public DateTime ReAttestationDueDate { get; set; }

        /// <summary>
        /// Gets or sets the re Processing date.
        /// </summary>
        public DateTime ProcessingDate { get; set; }

    }
}
