using System.Collections.Generic;

namespace Abim.Platform.Program.App.Domain
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPdfMergeData
    {
        /// <summary>
        /// Create a dictionary from the class properties
        /// </summary>
        IDictionary<string, string> MergeFieldValues { get; }
    }
}
