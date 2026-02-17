using System;
using System.Runtime.Serialization;

namespace Abim.Platform.Program.Util.Enums
{
    /// <summary>
    /// A response object for an enum value
    /// </summary>
    [DataContract]
    public class EnumValueResource<TEnum> : IEnumValue
        where TEnum : struct, IConvertible, IComparable, IFormattable
    {
        /// <summary>
        /// A single character or an integer, used to identify this enum value.
        /// </summary>
        [DataMember(Order = 1)]
        public string Code { get; set; }
        
        /// <summary>
        /// A single word string used to represent this enum value.
        /// </summary>
        [DataMember(Order = 2)]
        public string Value { get; set; }
        
        /// <summary>
        /// The name from the display attribute.
        /// </summary>
        [DataMember(Order = 3)]
        public string Name { get; set; }
        
        /// <summary>
        /// The short name from the display attribute.
        /// </summary>
        [DataMember(Order = 4)]
        public string ShortName { get; set; }
        
        /// <summary>
        /// A longform string describing this enum value from the display attribute.
        /// </summary>
        [DataMember(Order = 5)]
        public string Description { get; set; }
    }
}
