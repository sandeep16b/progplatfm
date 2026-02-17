using System;
using System.Linq;

namespace Abim.Platform.Program.WebApi.Comparison
{
    /// <summary>
    /// GuidComparison class
    /// </summary>
    public static class GuidComparison
    {
        /// <summary>
        /// Compares the guids.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public static int Compare(this Guid a, Guid b)
        {
            return string.Compare(Reorder(a), Reorder(b));
        }
        
        /// <summary>
        /// Compares the guids.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public static int CompareGuids(Guid a, Guid b)
        {
            return string.Compare(Reorder(a), Reorder(b));
        }

        /// <summary>
        /// Reorders the specified unique identifiers.
        /// </summary>
        /// <param name="guid">The unique identifier.</param>
        /// <returns></returns>
        private static string Reorder(Guid guid)
        {
            var split = guid.ToString().Split('-').ToList();
            split.Reverse();
            return string.Join("-", split.ToArray());
        }
    }
}
