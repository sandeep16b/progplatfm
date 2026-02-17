using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Abim.Platform.Program.Util.Enums
{
    /// <summary>
    /// A response object for an enum value
    /// </summary>
    [DataContract]
    public class EnumTypeResource<TEnum>
        where TEnum : struct, IConvertible, IComparable, IFormattable
    {
        /// <summary>
        /// A list of the applicable values for this enum in resource form.
        /// </summary>
        [DataMember(Order = 1)]
        public List<EnumValueResource<TEnum>> Values { get; set; }
        
        /// <summary>
        /// Gets the links.
        /// </summary>
        /// <value>
        /// The links.
        /// </value>
        [DataMember(Order = 2)]
        public IList<Link> Links { get; private set; }
        
        /// <summary>
        /// Create a new instance of this resource.
        /// </summary>
        public EnumTypeResource()
        {
            Values = new List<EnumValueResource<TEnum>>();
            Links = new List<Link>();
        }
    }
}
