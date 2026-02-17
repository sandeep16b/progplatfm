using Abim.Platform.Program.Relational.Services;
using System;

namespace Abim.Platform.Program.App.Services.Commands
{
    /// <summary>
    /// A command used to process certificate deselection.
    /// "Deselect a certificate" is the phrase the business uses, and is 
    /// retained here for ubiquitous language. In actuality, it is the 
    /// Credential (and its latest issuance) which gets updated.
    /// </summary>
    public class DeselectCertificateCommand : ICommand
    {
        /// <summary>
        /// The ID of the Credential to deselect.
        /// </summary>
        public Guid CredentialId { get; set; }

        /// <summary>
        /// The date the latest issuance should be expired 
        /// (when applicable -- see logic in service for details).
        /// </summary>
        public DateTime ExpiredDate { get; set; }

        /// <summary>
        /// The name of the user or process performing the deselect
        /// </summary>
        public string Username { get; set; }
    }
}
