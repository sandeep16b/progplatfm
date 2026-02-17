using System.ComponentModel.DataAnnotations;

namespace Abim.Platform.Program.Resources
{
    /// <summary>
    /// MaintenanceRequirementType enum.
    /// </summary>
    public enum MaintenanceRequirementType
    {
        /// <summary>
        /// Required.
        /// </summary>
        [Display(Name = "Required", ShortName = "R", Description = "Required")]
        Required      = 'R',
        
        /// <summary>
        /// NotRequired.
        /// </summary>
        [Display(Name = "NotRequired", ShortName = "N", Description = "Not Required")]
        NotRequired   = 'N'
    }
}
