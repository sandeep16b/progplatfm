using Abim.Platform.Program.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace Abim.Platform.Program.WebApi.Api.Configuration.Serialization
{
    /// <summary>
    /// A camelCase contract resolver
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver" />
    public class CamelCaseAbimContractResolver : CamelCasePropertyNamesContractResolver
    {
        /// <summary>
        /// Determines which contract type is created for the given type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// A <see cref="T:Newtonsoft.Json.Serialization.JsonContract" /> for the given type.
        /// </returns>
        protected override JsonContract CreateContract(Type objectType) 
        {
            //The purpose of this is it to avoid JsonSerializationExceptions such as the common
            //... "Error getting value from 'DefaultValue' on 'NHibernate.Type.DateTimeOffsetType'."
            if(typeof(NHibernate.Proxy.INHibernateProxy).IsAssignableFrom(objectType))
                return base.CreateContract(objectType.BaseType);
            
            return base.CreateContract(objectType);
        }

        /// <summary>
        /// Creates the property.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="memberSerialization">The member serialization.</param>
        /// <returns></returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            
            //Hide the "Enabled" property Links, unless it is "false"
            if(property.DeclaringType == typeof(Link) &&
                property.PropertyName.ToLower() == "enabled")
            {
                property.ShouldSerialize = instance =>
                {
                    Link value = (Link)instance;
                    return !value.Enabled;
                };
            }
            
            //If we find that several more such cases are needed, on different object types, we can add an interface for the type to implement
            //... to tell us what not to serialize and when
            
            return property;
        }
    }
}
