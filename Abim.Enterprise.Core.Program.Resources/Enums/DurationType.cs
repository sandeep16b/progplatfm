using System.ComponentModel.DataAnnotations;

namespace Abim.Platform.Program.Resources
{
    /// <summary>
    /// DurationType enum.
    /// </summary>
    public enum DurationType
    {
        /// <summary>
        /// Lifetime.
        /// </summary>
        [Display(Name = "Lifetime", ShortName = "L", Description = "Lifetime")]
        Lifetime      = 'L',
        
        /// <summary>
        /// Timelimited.
        /// </summary>
        [Display(Name = "Timelimited", ShortName = "T", Description = "Time limited")]
        Timelimited   = 'T',
        
        /// <summary>
        /// Continuous.
        /// </summary>
        [Display(Name = "Continuous", ShortName = "C", Description = "Continuous")]
        Continuous    = 'C'
    }
}
