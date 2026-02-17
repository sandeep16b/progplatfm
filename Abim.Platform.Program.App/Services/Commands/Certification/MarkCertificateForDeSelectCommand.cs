using Abim.Platform.Program.Relational.Services;
using Abim.Platform.Program.WebApi.Authentication;
using Newtonsoft.Json;
using System;

namespace Abim.Platform.Program.App.Services.Commands
{
    /// <summary>
    /// A command to be used when a diplomate has stated that they wish to deselect a certificate.
    /// To do this, we need the Credential's ID, so that we can flag the most recent issuance for 
    /// expiration when the deselection process is run.
    /// </summary>
    public class MarkCertificateForDeselectCommand : ICommand
    {
        /// <summary>
        /// The date the MarkCertificateForDeSelectCommand was submitted.
        /// </summary>
        /// <remarks>
        /// Because the date the deselect will occur is date-sensitive, this allows for more 
        /// precision. It also makes testing easier.
        /// </remarks>
        public DateTime SubmittedDate { get; set; }

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
