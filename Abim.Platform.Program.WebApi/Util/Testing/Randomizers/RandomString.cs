using System;
using System.Text;

namespace Abim.Platform.Program.WebApi.Testing.Setup
{
    /// <summary>
    /// Creates random strings
    /// </summary>
    public static class RandomString
    {
        /// <summary>
        /// The random
        /// </summary>
        private static readonly Random Random = new Random();

        /// <summary>
        /// Builds the string.
        /// </summary>
        /// <returns></returns>
        public static string Build()
        {
            var length = Random.Next(3, 20);
            return BuildWithLength(length);
        }

        /// <summary>
        /// Builds the string within a specified length range.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">RandomString isn't intended to produce strings over 1 million in Length for memory reasons</exception>
        public static string BuildWithLength(int minLength, int maxLength)
        {
            int length = Random.Next(minLength, maxLength);
            return BuildWithLength(length);
        }

        /// <summary>
        /// Builds the string with specified length.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">RandomString isn't intended to produce strings over 1 million in Length for memory reasons</exception>
        public static string BuildWithLength(int length)
        {
            if(length > 1000000)
                throw new Exception("RandomString isn't intended to produce strings over 1 million in Length for memory reasons");
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
            var randomString = new StringBuilder();
            for(int i = 0; i < length; i++)
                randomString.Append(chars[Random.Next(chars.Length - 1)]);
            return randomString.ToString();
        }

        /// <summary>
        /// Builds the string or returns null.
        /// </summary>
        /// <returns></returns>
        public static string BuildOrNull()
        {
            if(RandomBool.IsTrue) return null;
            return Build();
        }
        
        /// <summary>
        /// Builds the string or returns null.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public static string BuildOrNull(int length)
        {
            if(RandomBool.IsTrue) return null;
            return BuildWithLength(length);
        }

        /// <summary>
        /// Builds the string or returns null.
        /// </summary>
        /// <param name="minLength">The minimum length.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <returns></returns>
        public static string BuildOrNull(int minLength, int maxLength)
        {
            if(RandomBool.IsTrue) return null;
            return BuildWithLength(minLength, maxLength);
        }

        /// <summary>
        /// Creates a first name.
        /// </summary>
        /// <returns></returns>
        public static string FirstName()
        {
            return Faker.Name.First();
        }
        
        /// <summary>
        /// Creates a first name, or null
        /// </summary>
        /// <returns></returns>
        public static string FirstNameOrNull()
        {
            if(RandomBool.IsTrue) return null;
            return Faker.Name.First();
        }
        
        /// <summary>
        /// Creates a last name.
        /// </summary>
        /// <returns></returns>
        public static string LastName()
        {
            return Faker.Name.Last();
        }
        
        /// <summary>
        /// Creates a last name, or null.
        /// </summary>
        /// <returns></returns>
        public static string LastNameOrNull()
        {
            if(RandomBool.IsTrue) return null;
            return Faker.Name.Last();
        }
    }
}
