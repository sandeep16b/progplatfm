using System.ComponentModel.DataAnnotations;

namespace Abim.Platform.Program.Resources
{
    /// <summary>
    /// PathwayType enum.
    /// </summary>
    public enum PathwayType
    {
        /// <summary>
        /// MOC.
        /// </summary>
        [Display(Name = "10Year", ShortName = "10Yr", Description = "10 Year")]
        MOC = 'M',
        
        /// <summary>
        /// KCI.
        /// </summary>
        [Display(Name = "2Year", ShortName = "2Yr", Description = "2 Year")]
        KCI  = 'K',

        /// <summary>
        /// 1Year.
        /// </summary>
        [Display(Name = "1Year", ShortName = "1Yr", Description = "1 Year")]
        OneYear = 'O',

        /// <summary>
        /// LNG.
        /// </summary>
        [Display(Name = "LNG", ShortName = "LNG", Description = "Longitudinal")]
        LNG = 'L'
    }
}
