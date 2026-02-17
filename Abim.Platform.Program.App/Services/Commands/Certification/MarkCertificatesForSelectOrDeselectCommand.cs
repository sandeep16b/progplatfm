using Abim.Platform.Program.Relational.Services;
using Abim.Platform.Program.WebApi.Authentication;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.App.Services.Commands
{
    /// <summary>
    /// A command to be used when a diplomate has stated that they wish to select or 
    /// deselect one or more certificates. The business requirement is that multiple certificates 
    /// can be selected at the same time in the UI, and that changes are all saved 
    /// together.
    /// </summary>
    public class MarkCertificatesForSelectOrDeselectCommand : ICommand
    {
        /// <summary>
        /// Gets or sets the member identifier.
        /// </summary>
        /// <value>
        /// The member identifier.
        /// </value>
        public Guid MemberId { get; set; }

        /// <summary>
        /// The IDs of the Credentials whose latest issuances will be marked for deselection.
        /// </summary>
        public IEnumerable<Guid> CredentialIdsForDeselect { get; set; }

        /// <summary>
        /// The IDs of the Credentials whose latest issuances will be selected.
        /// </summary>
        public IEnumerable<Guid> CredentialIdsForSelect { get; set; }

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
