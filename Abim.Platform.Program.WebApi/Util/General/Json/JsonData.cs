using Abim.Platform.Program.Util.Extensions;
using Newtonsoft.Json;
using System;

namespace Abim.Platform.Program.WebApi.Util.General.Json
{
    /// <summary>
    /// JsonData class
    /// </summary>
    public static class JsonData
    {
        /// <summary>
        /// Parses the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="json">The json.</param>
        /// <param name="ignoreParseErrors">if set to <c>true</c> [ignore errors].</param>
        /// <returns></returns>
        public static Object Parse(Type type, string json, bool ignoreParseErrors = true)
        {
            try
            {
                return JsonConvert.DeserializeObject(json, type);
            }
            catch(Exception)
            {
                if(ignoreParseErrors)
                {
                    return Activator.CreateInstance(type);
                }

                throw;
            }
        }

        /// <summary>
        /// Parses the specified json.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json">The json.</param>
        /// <param name="ignoreParseErrors">if set to <c>true</c> [ignore errors].</param>
        /// <returns></returns>
        public static Object Parse<T>(string json, bool ignoreParseErrors = true)
        {
            return Parse(typeof(T), json, ignoreParseErrors);
        }

        /// <summary>
        /// Parses the specified type name.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="json">The json.</param>
        /// <param name="ignoreParseErrors">if set to <c>true</c> [ignore errors].</param>
        /// <returns></returns>
        public static Object Parse(string typeName, string json, bool ignoreParseErrors = true)
        {
            var type = TypeExtensions.GetType(typeName);
            return Parse(type, json, ignoreParseErrors);
        }
    }
}
