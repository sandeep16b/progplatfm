using Abim.Platform.Program.Relational.Services;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Authentication;
using Newtonsoft.Json;
using System;

namespace Abim.Platform.Program.App.Services.Commands
{
    /// <summary>
    /// Command to toggle the Pathway value
    /// </summary>
    /// <seealso cref="ICommand" />
    public class UpdatePathwayCommand : ICommand
    {
        /// <summary>
        /// Id of the Credential
        /// </summary>
        [JsonIgnore]
        public Guid CredentialId { get; set; }

        /// <summary>
        /// The Pathway value
        /// </summary>
        public PathwayType Pathway { get; set; }

        /// <summary>
        /// Gets or sets the user information.
        /// </summary>
        /// <value>
        /// The user information.
        /// </value>
        [JsonIgnore]
        public UserInfo UserInfo { get; set; }

        /// <summary>
        /// Gets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        [JsonIgnore]
        public string Username
        {
            get
            {
                if (UserInfo == null) return null;
                return UserInfo.Username;
            }
        }
    }
}
