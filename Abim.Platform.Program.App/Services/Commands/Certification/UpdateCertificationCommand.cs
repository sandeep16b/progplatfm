using Abim.Platform.Program.Relational.Services;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Authentication;
using Newtonsoft.Json;
using System;

namespace Abim.Platform.Program.App.Services.Commands
{
    /// <summary>
    /// Command to Add Certification Command
    /// </summary>
    /// <seealso cref="ICommand" />
    public class UpdateCertificationCommand: ICommand
    {
        #region Properties
        /// <summary>
        /// Gets or sets the SourceId identifier.
        /// </summary>
        /// <value>
        /// The member identifier.
        /// </value>
        public Guid SourceId { get; set; }

        /// <summary>
        /// Gets or sets the Code.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public CertificationType Type { get; set; }

        /// <summary>
        /// Gets or sets Consecutive Attempt.
        /// </summary>
        /// <value>
        /// The ConsecutiveAttempt.
        /// </value>
        public int? ConsecutiveAttempt { get; set; }

        /// <summary>
        /// Gets or sets the user information.
        /// </summary>
        /// <value>
        /// The user information.
        /// </value>
        [JsonIgnore]
        public UserInfo UserInfo { get; set; }

        #endregion
    }
}