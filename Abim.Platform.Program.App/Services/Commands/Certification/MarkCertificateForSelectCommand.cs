using Abim.Platform.Program.Relational.Services;
using Abim.Platform.Program.WebApi.Authentication;
using Newtonsoft.Json;
using System;

namespace Abim.Platform.Program.App.Services.Commands
{
    /// <summary>
    /// A command to be used when a diplomate has stated that they wish to select a certificate.
    /// To do this, we need the Credential's ID
    /// </summary>
    public class MarkCertificateForSelectCommand : ICommand
    {
        /// <summary>
        /// The ID of the Credential whose latest issuance will be marked for deselection.
        /// The Credential itself will have its SelectedToMaintain value set to false when the 
        /// deselection process runs, but not until then.
        /// </summary>
        public Guid CredentialId { get; set; }

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
