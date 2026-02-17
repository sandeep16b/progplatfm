using System.ComponentModel.DataAnnotations;

namespace Abim.Platform.Program.Resources
{
    /// <summary>
    /// OccurrenceType enum.
    /// </summary>
    public enum OccurrenceType
    {
        /// <summary>
        /// Initial.
        /// </summary>
        [Display(Name   = "Initial", ShortName = "I", Description = "Initial")]
        Initial         = 'I',
        
        /// <summary>
        /// Recertification.
        /// </summary>
        [Display(Name   = "Recertification", ShortName = "R", Description = "Recertification")]
        Recertification = 'R'
    }
}
