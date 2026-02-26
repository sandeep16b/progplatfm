using System.ComponentModel.DataAnnotations;

namespace Abim.Platform.Program.Resources
{
    /// <summary>
    /// DeselectionType enum.
    /// </summary>
    public enum DeselectionType
    {
        /// <summary>
        /// Auto.
        /// </summary>
        [Display(Name = "Auto", ShortName = "A", Description = "Auto")]
        Auto = 'A',

        /// <summary>
        /// Self.
        /// </summary>
        [Display(Name = "Self", ShortName = "S", Description = "Self")]
        Self = 'S'
    }
}
