using System;
using System.Collections;

namespace Abim.Platform.Program.WebApi.Comparison
{
    /// <summary>
    /// Does comparison between DateTimes
    /// </summary>
    /// <seealso cref="System.Collections.IEqualityComparer" />
    public class DateComparison : IEqualityComparer
    {
        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="x">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <param name="y">The y.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public new bool Equals(object x, object y)
        {
            var xDate = (DateTime) x;
            var yDate = (DateTime) y;

            return (xDate - yDate).Days == 0;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public int GetHashCode(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
