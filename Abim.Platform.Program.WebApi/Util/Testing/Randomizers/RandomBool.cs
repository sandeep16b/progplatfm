using System;

namespace Abim.Platform.Program.WebApi.Testing.Setup
{
    /// <summary>
    /// Creates random bools
    /// </summary>
    public static class RandomBool
    {
        /// <summary>
        /// The random
        /// </summary>
        private static readonly Random Random = new Random();

        /// <summary>
        /// Gets a value indicating whether this instance is true.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is true; otherwise, <c>false</c>.
        /// </value>
        public static bool IsTrue
        {
            get
            {
                return (Random.Next(2) == 0);
            }
        }
    }
}
