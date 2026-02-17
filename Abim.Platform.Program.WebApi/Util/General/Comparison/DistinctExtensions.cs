using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.WebApi.Objects.Comparison
{
    /// <summary>
    /// Extension class
    /// </summary>
    public static class DistinctExtensions
    {
        /// <summary>
        /// Distinct by a func.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="keySelector">The key selector.</param>
        /// <returns></returns>
        public static IEnumerable<TSource> DistinctOn<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach(TSource element in source)
            {
                if(seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}
