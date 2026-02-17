using System;

namespace Abim.Platform.Program.App.DTOs
{
    /// <summary>
    /// A class containing minimal information regarding a credential
    /// </summary>
    public class CredentialInfoDTO
    {
        /// <summary>
        /// The external ID of the credential
        /// </summary>
        public Guid ExternalId { get; set; }
        
        /// <summary>
        /// The name of the certificate the credential applies to
        /// </summary>
        public string CertificateName { get; set; }

        /// <summary>
        /// Indicates whether or not the credential is active
        /// </summary>
        public bool IsActive { get; set; }
    }
}
