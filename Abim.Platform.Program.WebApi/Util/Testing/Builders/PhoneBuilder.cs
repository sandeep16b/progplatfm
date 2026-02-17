using System;

namespace Abim.Platform.Program.WebApi.Objects.Testing.Setup.Builders
{
    /// <summary>
    /// Static because it currently supports only one option
    /// </summary>
    public static class PhoneBuilder
    {
        /// <summary>
        /// The Random
        /// </summary>
        private static Random Random = new Random();

        /// <summary>
        /// Creates a US phone number.
        /// </summary>
        /// <returns></returns>
        public static string SampleUSPhone()
        {
            var section1 = Random.Next(200, 999).ToString();
            var section2 = Random.Next(200, 999).ToString();
            var section3 = Random.Next(1, 9999).ToString().PadLeft(4, '0');
            
            return string.Format("{0}-{1}-{2}", section1, section2, section3);
        }

        /// <summary>
        /// Creates a sample US phone number in the format required by Pearson.
        /// </summary>
        /// <returns></returns>
        public static string SamplePearsonPhone()
        {
            return SampleUSPhone().Replace("-", "");
        }
    }
}
