using Abim.Platform.Program.Relational.Classes;
using System;

namespace Abim.Platform.Program.Relational.Domain.Mapping.EnumerationMappings
{
    /// <summary>
    /// The use of this mapping class indicates that the enum's code name (e.g. "Aaa" if the entry is Aaa='A') corresponds to the database value, with the same casing
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Abim.Platform.Program.Relational.Domain.Mapping.EnumerationMappings.EnumTypeMap{T}" />
    public class CaseSensitiveEntryBasedEnumTypeMap<T> : EnumTypeMap<T>
        where T : struct, IConvertible, IComparable, IFormattable
    {
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public override object GetInstance(object code)
        {
            try
            {
                return (T)(Enum.Parse(typeof(T), code.ToString(), false));
            }
            #pragma warning disable 0168
            catch(Exception ex)
            {
                throw new Exception(string.Format("Entry {0} does not exist in enumeration {1}", code.ToString(), typeof(T).Name));
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public override object GetValue(object code)
        {
            return EnumAttributes.ReadEnumValue(code);
        }
    }
}
