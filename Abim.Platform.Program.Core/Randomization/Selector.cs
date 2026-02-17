using System;
using System.Collections.Generic;
using System.Linq;

namespace Abim.Platform.Program.Util.Objects.Util
{
    /// <summary>
    /// Picks random entries, usually for test code purposes
    /// </summary>
    public static class Selector
    {
        private static Random Random = new Random();
        
        /// <summary>
        /// Picks one entry.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static T Select<T>(IEnumerable<T> data)
        {
            return data.ToArray()[Random.Next(0, data.Count() - 1)];
        }
    }
}
