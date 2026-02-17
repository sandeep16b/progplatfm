using Abim.Platform.Product.Extensions.ExternalResponses;
using Abim.Platform.Product.Util.Enums;
using System;
using System.Globalization;

namespace Abim.Platform.Program.Tests.Setup.Responses
{
    /// <summary>
    /// Copied from EnumValueResponsrResource
    /// </summary>
    /// <typeparam name="TEnum">The enum class represented by this resource.</typeparam>
    public class ProductEnumValueResponseResource<TEnum> : EnumValueResource<TEnum>
        where TEnum : struct, IConvertible, IComparable, IFormattable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductEnumValueResponseResource{T}"/> class.
        /// </summary>
        public ProductEnumValueResponseResource(TEnum value)
        {
            SetValue(value);
        }
        
        /// <summary>
        /// Converts EnumValueResource to an ProductEnumValueResponseResource
        /// </summary>
        /// <param name="parentClass">The parent class.</param>
        /// <returns></returns>
        public static ProductEnumValueResponseResource<TEnum> FromEnumValueResource(EnumValueResource<TEnum> parentClass)
        {
            if(parentClass is ProductEnumValueResponseResource<TEnum>) return ((ProductEnumValueResponseResource<TEnum>)parentClass);
            var entry = EnumAttributes.ParseFromText<TEnum>(parentClass.Value, false, true);
            if(entry.Equals(default(TEnum))) return null;
            var obj = new ProductEnumValueResponseResource<TEnum>(entry);
            return obj;
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
        /// Either initializes a new instance of the <see cref="ProductEnumValueResponseResource{T}"/> class, or returns null
        /// </summary>
        public static ProductEnumValueResponseResource<TEnum> CreateOrNull(TEnum value)
        {
            var instance = new ProductEnumValueResponseResource<TEnum>(value);
            if(instance.Name == null && instance.Code == "\u0000") return null;
            else return instance;
        }
        
        /// <summary>
        /// Either initializes a new instance of the <see cref="ProductEnumValueResponseResource{T}"/> class, or returns null
        /// </summary>
        public static ProductEnumValueResponseResource<TEnum> CreateOrNull(TEnum? value)
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
            return new[]{ typeof(ProductEnumValueResponseResource<TEnum>) };
        }
    }
}
