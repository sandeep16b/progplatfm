using System.ComponentModel.DataAnnotations;

namespace Abim.Platform.Program.Resources
{
    /// <summary>
    /// MaintenanceStatusType enum.
    /// </summary>
    public enum MaintenanceStatusType
    {
        /// <summary>
        /// Maintained.
        /// </summary>
        [Display(Name    = "Maintained", ShortName = "I", Description = "Maintained")]
        Maintained       = 'I',
        
        /// <summary>
        /// NotMaintained.
        /// </summary>
        [Display(Name    = "NotMaintained", ShortName = "N", Description = "Not Maintained")]
        NotMaintained    = 'N'
    }
}
