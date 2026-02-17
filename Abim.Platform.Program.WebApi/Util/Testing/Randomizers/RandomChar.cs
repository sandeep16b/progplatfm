using System;

namespace Abim.Platform.Program.WebApi.Testing.Setup
{
    /// <summary>
    /// Creates random chars
    /// </summary>
    public static class RandomChar
    {
        /// <summary>
        /// The random
        /// </summary>
        private static readonly Random Random = new Random();

        /// <summary>
        /// An upper char.
        /// </summary>
        /// <returns></returns>
        public static char Upper()
        {
            return (char)(Random.Next('A', 'Z'));
        }

        /// <summary>
        /// A lower char.
        /// </summary>
        /// <returns></returns>
        public static char Lower()
        {
            return (char)(Random.Next('a', 'z'));
        }
    }
}
