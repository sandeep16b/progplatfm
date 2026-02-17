using Abim.Platform.Program.Relational.Services;
using Abim.Platform.Program.Resources;
using Embarr.WebAPI.AntiXss;
using Newtonsoft.Json;
using System;

namespace Abim.Platform.Program.App.Services.Commands
{
    /// <summary>
    /// When we need to move a credential to Active
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Relational.Services.ICommand" />
    public class ReinstateTLCommand : ICommand
    {
        /// <summary>
        /// CredentialId
        /// </summary>
        [JsonIgnore]
        public Guid CredentialId { get; set; }

        /// <summary>
        /// Gets or sets the maintenance status.
        /// </summary>
        /// <value>
        /// The maintenance status.
        /// </value>
        public MaintenanceStatusType MaintenanceStatus { get; set; }

        /// <summary>
        /// Gets or sets the IssuanceStatus.
        /// </summary>
        /// <value>
        /// The maintenance status.
        /// </value>
        public IssuanceStatusType IssuanceStatus { get; set; }

        /// <summary>
        /// ModifiedBy
        /// </summary>
        [AntiXss] 
        public string ModifiedBy { get; set; }
    }
}
