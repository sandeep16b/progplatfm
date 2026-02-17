using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.WebApi.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Abim.Enterprise.Core.Util.EnumResource
{
    /// <summary>
    /// A class used to represent an individual enum value as an API resource.
    /// </summary>
    /// <typeparam name="TEnum">The enum class represented by this resource.</typeparam>
    [DataContract]
    [KnownType("GetTypes")]
    public class EnumValueResponseResource<TEnum> : EnumValueResource<TEnum>
        where TEnum : struct, IConvertible, IComparable, IFormattable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumValueResponseResource{T}"/> class.
        /// </summary>
        public EnumValueResponseResource(TEnum value)
        {
            SetValue(value);
        }

        /// <summary>
        /// Converts EnumValueResource to an EnumValueResponseResource
        /// </summary>
        /// <param name="parentClass">The parent class.</param>
        /// <returns></returns>
        public static EnumValueResponseResource<TEnum> FromEnumValueResource(EnumValueResource<TEnum> parentClass)
        {
            if(parentClass is EnumValueResponseResource<TEnum>) return ((EnumValueResponseResource<TEnum>)parentClass);
            var entry = EnumAttributes.ParseFromText<TEnum>(parentClass.Value, false, true);
            if(entry.Equals(default(TEnum))) return null;
            var obj = new EnumValueResponseResource<TEnum>(entry);
            return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public EnumValueResource<TEnum> ToEnumValueResource()
        {
            return (EnumValueResource <TEnum>) this;
        }

        /// <summary>
        /// Sets the enum value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="InvalidOperationException"></exception>
        public void SetValue(TEnum value)
        {
            if(!typeof(TEnum).IsEnum)
                throw new InvalidOperationException();
            
            Code = EnumAttributes.ReadEnumCode(value);
            Value = value.ToString(CultureInfo.InvariantCulture);
            Name = EnumAttributes.ReadEnumName(value);
            ShortName = EnumAttributes.ReadEnumShortName(value);
            Description = EnumAttributes.ReadEnumDescription(value);
        }
        
        /// <summary>
        /// Either initializes a new instance of the <see cref="EnumValueResponseResource{T}"/> class, or returns null
        /// </summary>
        public static EnumValueResponseResource<TEnum> CreateOrNull(TEnum value)
        {
            var instance = new EnumValueResponseResource<TEnum>(value);
            if(instance.Name == null && instance.Code == "\u0000") return null;
            else return instance;
        }
        
        /// <summary>
        /// Either initializes a new instance of the <see cref="EnumValueResponseResource{T}"/> class, or returns null
        /// </summary>
        public static EnumValueResponseResource<TEnum> CreateOrNull(TEnum? value)
        {
            if(value.HasValue) return CreateOrNull(value.Value);
            else return null;
        }
        
        /// <summary>
        /// Returns the strongly-typed enum object
        /// </summary>
        public TEnum ToEnum()
        {
            return EnumAttributes.ToEnum<TEnum>(Code);
        }
        
        /// <summary>
        /// Gets the types used to serialize this object to XML.
        /// </summary>
        /// <returns></returns>
        public static Type[] GetTypes()
        {
            return new Type[]{ typeof(EnumValueResponseResource<TEnum>) };
        }
    }
}
