using Abim.Platform.Program.Relational.Services;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Authentication;
using Embarr.WebAPI.AntiXss;
using Newtonsoft.Json;
using System;

namespace Abim.Platform.Program.App.Services.Commands
{
    /// <summary>
    /// Expires an existing issuance and makes a new one on the same credential
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Relational.Services.ICommand" />
    public class ExpireAndReissueCommand : ICommand
    {
        /// <summary>
        /// CredentialId
        /// </summary>
        public Guid CredentialId { get; set; }

        /// <summary>
        /// IssuanceDate
        /// </summary>
        public DateTime IssuanceDate { get; set; }

        /// <summary>
        /// MaintenanceStatus
        /// </summary>
        public MaintenanceStatusType MaintenanceStatus { get; set; }

        /// <summary>
        /// ScheduledUpdate
        /// </summary>
        public DateTime ScheduledUpdate { get; set; }

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
    }
}
