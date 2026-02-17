using System.ComponentModel.DataAnnotations;

namespace Abim.Platform.Program.Resources
{
    /// <summary>
    /// CertificationType enum.
    /// </summary>
    public enum CertificationType
    {
        /// <summary>
        /// General.
        /// </summary>
        [Display(Name   = "General", ShortName = "G", Description = "General")]
        General         = 'G',
        
        /// <summary>
        /// Subspecialty.
        /// </summary>
        [Display(Name   = "Subspecialty", ShortName = "S", Description = "Sub-Specialty")]
        Subspecialty    = 'S',
        
        /// <summary>
        /// Primary.
        /// </summary>
        [Display(Name   = "Primary", ShortName = "P", Description = "Primary")]
        Primary         = 'P',
        
        /// <summary>
        /// Recertification.
        /// </summary>
        [Display(Name   = "Recertification", ShortName = "R", Description = "Recertification")]
        Recertification = 'R',
        
        /// <summary>
        /// JointAgreement.
        /// </summary>
        [Display(Name   = "JointAgreement", ShortName = "J", Description = "Joint Agreement")]
        JointAgreement  = 'J',
        
        /// <summary>
        /// OtherBoard.
        /// </summary>
        [Display(Name   = "OtherBoard", ShortName = "O", Description = "Other Board")]
        OtherBoard      = 'O',
        
        /// <summary>
        /// FocusPractice.
        /// </summary>
        [Display(Name   = "FocusPractice", ShortName = "F", Description = "Focus Practice")]
        FocusPractice   = 'F'
    }
}
