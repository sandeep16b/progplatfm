using Abim.Platform.Program.Relational.Services;
using Newtonsoft.Json;
using System;

namespace Abim.Platform.Program.App.Services.Commands
{
    /// <summary>
    /// Command to toggle the SelectedToMaintain flag
    /// </summary>
    /// <seealso cref="ICommand" />
    public class UpdateGrandfatherMOCPrintDateCommand : ICommand
    {
        /// <summary>
        /// Id of the Credential
        /// </summary>
        [JsonIgnore]
        public Guid CredentialId { get; set; }

        /// <summary>
        /// The GrandfatherMOCPrintDate flag
        /// </summary>
        public DateTime GrandfatherMOCPrintDate { get; set; }

        /// <summary>
        /// ModifiedBy.
        /// </summary>
        /// <value>
        /// The ModifiedBy.
        /// </value>
        [JsonIgnore]
        public string ModifiedBy { get; set; }
    }
}