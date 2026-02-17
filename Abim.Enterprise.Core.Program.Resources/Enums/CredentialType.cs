using System.ComponentModel.DataAnnotations;

namespace Abim.Platform.Program.Resources
{
    /// <summary>
    /// CredentialType enum.
    /// </summary>
    public enum CredentialType
    {
        /// <summary>
        /// General.
        /// </summary>
        [Display(Name = "General", ShortName = "G", Description = "General")]
        General = 'G',

        /// <summary>
        /// Subspecialty.
        /// </summary>
        [Display(Name = "Subspecialty", ShortName = "S", Description = "Sub-Specialty")]
        Subspecialty = 'S'
    }
}
