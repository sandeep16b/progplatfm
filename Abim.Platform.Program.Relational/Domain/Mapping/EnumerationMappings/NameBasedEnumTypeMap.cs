using Abim.Platform.Program.Relational.Classes;
using System;

namespace Abim.Platform.Program.Relational.Domain.Mapping.EnumerationMappings
{
    /// <summary>
    /// The use of this mapping class indicates that the enum's DisplayAttribute.Name corresponds to the database value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Abim.Platform.Program.Relational.Domain.Mapping.EnumerationMappings.EnumTypeMap{T}" />
    public class NameBasedEnumTypeMap<T> : EnumTypeMap<T>
        where T : struct, IConvertible, IComparable, IFormattable
    {
        /// <summary>
        /// Gets an instance of the Enum
        /// </summary>
        /// <param name="code">The underlying value of an item in the Enum.</param>
        /// <returns>
        /// An instance of the Enum set to the <c>code</c> value.
        /// </returns>
        public override object GetInstance(object code)
        {
            
            return EnumAttributes.GetEnumByName<T>(code.ToString());
        }

        /// <summary>
        /// Gets the correct value for the Enum.
        /// </summary>
        /// <param name="code">The value to convert (an enum instance).</param>
        /// <returns>
        /// A boxed version of the code, converted to the correct type.
        /// </returns>
        /// <remarks>
        /// This handles situations where the DataProvider returns the value of the Enum
        /// from the db in the wrong underlying type.  It uses <see cref="T:System.Convert" /> to
        /// convert it to the correct type.
        /// </remarks>
        public override object GetValue(object code)
        {
            return EnumAttributes.ReadEnumName(code);
        }
    }
}
