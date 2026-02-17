using System;

namespace Abim.Platform.Program.Relational.Domain.Mapping.EnumerationMappings
{
    /// <summary>
    /// The use of this mapping class indicates that the enum's int/char code (e.g. "A" if the entry is AAA='A') corresponds
    /// to the single-character database value (e.g. 'A')
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Abim.Platform.Program.Relational.Domain.Mapping.EnumerationMappings.EnumTypeMap{T}" />
    public class CodeBasedEnumTypeMap<T> : EnumTypeMap<T>
        where T : struct, IConvertible, IComparable, IFormattable
    {
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public override object GetInstance(object code)
        {
            return (T)Enum.ToObject(typeof(T), (Byte)((Char)GetValue(code)));
        }
    }
}
