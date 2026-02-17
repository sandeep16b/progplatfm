

using System;
using Abim.Platform.Program.Relational.Services;
using Abim.Platform.Program.Resources;
using Embarr.WebAPI.AntiXss;


namespace Abim.Platform.Program.App.Services.Commands
{
    /// <summary>
    /// Command to set an Issuance to be 'Maintained'
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Relational.Services.ICommand" />
    public class UpdateCredentialGracePeriodGFCommand : ICommand
    {
        /// <summary>
        /// Id of the Credential
        /// </summary>
        public Guid CredentialId { get; set; }

        /// <summary>
        /// The audit username
        /// </summary>
        [AntiXss]  //all public string properties with setters in our command classes get this attribute
        public string ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the maintenance status.
        /// </summary>
        /// <value>
        /// The maintenance status.
        /// </value>
        public MaintenanceStatusType MaintenanceStatus { get; set; }

        /// <summary>
        /// Gets or sets the re Processing date.
        /// </summary>
        public DateTime ProcessingDate { get; set; }
    }
}
