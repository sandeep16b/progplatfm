using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Services;
using Abim.Platform.Program.WebApi.Authentication;
using Newtonsoft.Json;

namespace Abim.Platform.Program.App.Services.Commands
{
    /// <summary>
    /// Command to Add Credential Command
    /// </summary>
    /// <seealso cref="ICommand" />
    public class UpdateCredentialFromObjectCommand : ICommand
    {

        #region Properties
        /// <summary>
        /// Gets or sets the credential
        /// </summary>
        public Credential Credential { get; set; }

        /// <summary>
        /// SetGracePeriod
        /// </summary>
        public bool SetGracePeriod { get; set; }

        /// <summary>
        /// ReistateCertificate
        /// </summary>
        public bool ReistateCertificate { get; set; }

        /// <summary>
        /// Set2YearPassDue
        /// </summary>
        public bool SetConsecutiveKCIPassRequired { get; set; }

        /// <summary>
        /// SetCertStatus
        /// </summary>
        public bool SetCertStatus { get; set; }

        /// <summary>
        /// SetParticipationStatus
        /// </summary>
        public bool SetParticipationStatus { get; set; }

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