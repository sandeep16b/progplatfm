using Abim.Platform.Program.Extensions.ExternalResponses;
using Abim.Platform.Program.Util.Enums;
using System;
using System.Runtime.Serialization;
using EnumAttributes = Abim.Platform.Program.Extensions.ExternalResponses.EnumAttributes;

namespace Abim.Platform.Program.WebApi.Responses
{
    /// <summary>
    /// A class used to represent a full enum type as an API resource.
    /// </summary>
    /// <typeparam name="TEnum">An enum type to represent.</typeparam>
    [DataContract]
    [KnownType("GetTypes")]
    public class EnumTypeResponseResource<TEnum> : EnumTypeResource<TEnum>
        where TEnum : struct, IConvertible, IComparable, IFormattable
    {
        /// <summary>
        /// Create method
        /// </summary>
        public static EnumTypeResponseResource<TEnum> Create()
        {
            var instance = new EnumTypeResponseResource<TEnum>();
            foreach(TEnum status in EnumAttributes.GetEnumValues<TEnum>(true))
                instance.Values.Add(Activator.CreateInstance(typeof(EnumValueResponseResource<TEnum>), status) as EnumValueResponseResource<TEnum>);
            return instance;
        }
        
        /// <summary>
        /// Gets the types used to serialize this object to XML.
        /// </summary>
        /// <returns></returns>
        public static Type[] GetTypes()
        {
            return new[]{ typeof(EnumTypeResponseResource<TEnum>) };
        }
    }
}
