using Abim.Platform.Program.Relational.Services;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Authentication;
using Embarr.WebAPI.AntiXss;
using Newtonsoft.Json;
using System;

namespace Abim.Platform.Program.App.Services.Commands
{
    /// <summary>
    /// When we need to move a credential to Active, we issue this command which makes a new Active status Issuance
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Relational.Services.ICommand" />
    public class ReissueCommand : ICommand
    {
        /// <summary>
        /// CredentialId
        /// </summary>
        [JsonIgnore]
        public Guid CredentialId { get; set; }

        /// <summary>
        /// IssuanceDate
        /// </summary>
        public DateTime IssuanceDate { get; set; }


        /// <summary>
        /// Gets or sets the maintenance status.
        /// </summary>
        /// <value>
        /// The maintenance status.
        /// </value>
        public MaintenanceStatusType MaintenanceStatus { get; set; }

        /// <summary>
        /// ScheduledUpdate
        /// </summary>
        public DateTime ScheduledUpdate { get; set; }

        /// <summary>
        /// ReAttestationDueDate
        /// </summary>
        public DateTime? ReAttestationDueDate { get; set; }

        /// <summary>
        /// CreatedBy
        /// </summary>
        [AntiXss]  //all public string properties with setters in our command classes get this attribute
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the user information.
        /// </summary>
        /// <value>
        /// The user information.
        /// </value>
        [JsonIgnore]
        public UserInfo UserInfo { get; set; }

        /// <summary>
        /// Gets or sets the re Processing date.
        /// </summary>
        public DateTime ProcessingDate { get; set; }
    }
}
