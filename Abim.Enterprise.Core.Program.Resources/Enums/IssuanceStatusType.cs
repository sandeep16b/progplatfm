using System.ComponentModel.DataAnnotations;

namespace Abim.Platform.Program.Resources
{
    /// <summary>
    /// IssuanceStatusType enum.
    /// </summary>
    public enum IssuanceStatusType
    {
        /// <summary>
        /// Active.
        /// </summary>
        [Display(Name = "Active", ShortName = "A", Description = "Active")]
        Active        = 'A',
        
        /// <summary>
        /// Inactive.
        /// </summary>
        [Display(Name = "Inactive", ShortName = "I", Description = "Inactive")]
        Inactive      = 'I',
        
        /// <summary>
        /// Expired.
        /// </summary>
        [Display(Name = "Expired", ShortName = "E", Description = "Expired")]
        Expired       = 'E',
        
        /// <summary>
        /// Surrendered.
        /// </summary>
        [Display(Name = "Surrendered", ShortName = "S", Description = "Surrendered")]
        Surrendered   = 'S',
        
        /// <summary>
        /// Suspended.
        /// </summary>
        [Display(Name = "Suspended", ShortName = "X", Description = "Suspended")]
        Suspended     = 'X',
        
        /// <summary>
        /// Revoked.
        /// </summary>
        [Display(Name = "Revoked", ShortName = "R", Description = "Revoked")]
        Revoked       = 'R',

        /// <summary>
        /// Cancelled.
        /// </summary>
        [Display(Name = "Cancelled", ShortName = "C", Description = "Cancelled")]
        Cancelled = 'C'
    }
}
