using Abim.Platform.Program.Relational.Services;
using Abim.Platform.Program.WebApi.Authentication;
using Embarr.WebAPI.AntiXss;
using Newtonsoft.Json;
using System;

namespace Abim.Platform.Program.App.Services.Commands
{
    /// <summary>
    /// RunRulesForMustBeMaintainedCertificateCommand class. Used by background jobs
    /// </summary>
    public class RunRulesForMustBeMaintainedCertificateCommand : ICommand
    {
        #region Properties

        /// <summary>
        /// Gets or sets the credential identifier.
        /// </summary>
        /// <value>
        /// The credential identifier.
        /// </value>
        public Guid CredentialId { get; set; }

        /// <summary>
        /// Gets or sets the issuance identifier.
        /// </summary>
        /// <value>
        /// The issuance identifier.
        /// </value>
        public int IssuanceId { get; set; }

        /// <summary>
        /// Gets or sets the event date.
        /// </summary>
        /// <value>
        /// The event date.
        /// </value>
        public DateTime EventDate { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>
        /// The created by.
        /// </value>
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
        /// Gets or sets the Processing Date.
        /// </summary>
        /// <value>
        /// The user information.
        /// </value>
        public DateTime ProcessingDate { get; set; }

        #endregion
    }
}
