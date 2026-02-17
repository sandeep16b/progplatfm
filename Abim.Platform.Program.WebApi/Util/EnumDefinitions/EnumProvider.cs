using Abim.Platform.Program.Relational.Classes.Extensions;
using Abim.Platform.Program.Util.Extensions;
using Abim.Platform.Program.WebApi.Api;
using Abim.Platform.Program.WebApi.Response;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Abim.Platform.Program.WebApi.Objects
{
    /// <summary>
    /// Basic enum service
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.WebApi.Objects.IEnumService" />
    public abstract class EnumProvider : IEnumService
    {
        /// <summary>
        /// The default enum definitions
        /// </summary>
        protected IList<EnumDefinition> defaultEnumDefinitions;
        
        /// <summary>
        /// Gets the enums
        /// </summary>
        /// <returns></returns>
        public virtual IList<EnumDefinition> GetEnumDefinitions()
        {
            if(defaultEnumDefinitions == null) defaultEnumDefinitions = BuildEnumDefinitionsForDomainEnumTypes();
            return defaultEnumDefinitions;
        }
        
        /// <summary>
        /// Gets a subset of the enum definitions
        /// </summary>
        /// <returns></returns>
        public IList<EnumDefinition> GetEnumDefinitions(IEnumerable<Type> typesRequested)
        {
            return GetEnumDefinitions().Where(e => typesRequested.Contains(e.Type)).ToList();
        }

        /// <summary>
        /// Gets all domain enum types for this platform
        /// </summary>
        /// <returns></returns>
        public IList<Type> GetDomainEnumTypes()
        {
            var types = new List<Type>();
            types.AddRange(TypeExtensions.SearchTypes(t => t.IsEnum && t.Namespace != null && IsPlatformEnum(t)));
            return types;
        }
        
        /// <summary>
        /// Creates a list of Enum Definitions for all domain enum types
        /// </summary>
        /// <returns></returns>
        public IList<EnumDefinition> BuildEnumDefinitionsForDomainEnumTypes()
        {
            IList<Type> types = GetDomainEnumTypes();
            var definitions = new List<EnumDefinition>();
            foreach(var type in types)
            {
                definitions.Add(new EnumDefinition()
                    {
                        Name = type.Name.LowerCaseStart().Pluralize(),
                        Type = type
                    }
                );
            }
            return definitions;
        }
        
        /// <summary>
        /// Gets a subset of the enum definitions, which are used by the properties on a given Type
        /// </summary>
        /// <returns></returns>
        public IList<EnumDefinition> GetEnumDefinitionsFor(Type modelType, bool alsoIncludeEnumsInNestedNonAggregateRootTypes)
        {
            var enumsNeeded = new List<Type>();
            var lookup = new Dictionary<Type, bool>();
            CollectEnumTypes(modelType, enumsNeeded, lookup, alsoIncludeEnumsInNestedNonAggregateRootTypes, true);
            return GetEnumDefinitions(enumsNeeded.OrderBy(e => e.Name));
        }
        
        /// <summary>
        /// Gets the enumeration types used by the properties on a given model Type
        /// </summary>
        /// <returns></returns>
        public IList<Type> GetEnumTypesFor(Type modelType, bool alsoIncludeEnumsInNestedNonAggregateRootTypes)
        {
            return GetEnumDefinitionsFor(modelType, alsoIncludeEnumsInNestedNonAggregateRootTypes).Select(def => def.Type).ToList();
        }
        
        /// <summary>
        /// Goes down the type tree to find all the enums used (not stepping into any other aggregate roots)
        /// </summary>
        /// <returns></returns>
        private void CollectEnumTypes(Type t, List<Type> collection, Dictionary<Type, bool> lookup, bool includeNested, bool root)
        {
            if(lookup.ContainsKey(t)) return;            //avoid type cycles
            lookup[t] = true;                            //mark down that we've already visited this type now
            if(!root && IsAggregateRoot(t)) return;      //don't step into other aggregate root types
            if(includeNested)
            {
                var classMemberTypes = new List<Type>();
                //collect classes of direct members
                classMemberTypes.AddRange(t.GetProperties().Where(p => p.PropertyType.IsClass).Select(p => p.PropertyType));
                //collect classes inside enumerable class members
                classMemberTypes.AddRange(t.GetProperties().Where(p => typeof(IEnumerable<>).IsAssignableFrom(p.PropertyType))
                    .Select(p => p.PropertyType.GetGenericArguments()[0]));
                //collect classes inside enumerable interface members
                classMemberTypes.AddRange(t.GetProperties().Where(p => p.PropertyType.IsGenericType &&
                    p.PropertyType.GetInterfaces().Any(it => it.Name == "IEnumerable`1"))
                    .Select(p => p.PropertyType.GetGenericArguments()[0]));
                foreach(var c in classMemberTypes.Where(ct => IsAbimType(ct)))
                {
                    CollectEnumTypes(c, collection, lookup, true, false);
                }
            }
            var enumDefinitions = GetEnumDefinitions();
            var enumsUsed = new List<Type>();
            //collect enums of direct members
            enumsUsed.AddRange(t.GetProperties().Where(p => p.PropertyType.IsEnum && enumDefinitions.Any(e => e.Type.Equals(p.PropertyType)))
                .Select(p => p.PropertyType));
            //collect enums of nullable members
            enumsUsed.AddRange(t.GetProperties().Where(p => p.PropertyType.IsGenericType && p.PropertyType.Name == "Nullable`1" &&
                enumDefinitions.Any(e => e.Type.Equals(p.PropertyType.GetGenericArguments()[0]))).Select(p => p.PropertyType.GetGenericArguments()[0]));
            foreach(var e in enumsUsed)
            {
                if(!collection.Any(existing => existing.Equals(e)))
                {
                    collection.Add(e);
                }
            }
        }
        
        /// <summary>
        /// Determines whether a type is an Abim platform type, by namespace
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        /// <returns>
        ///   <c>true</c> if [is abim enum] [the specified enum type]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsAbimType(Type type)
        {
            return type.Namespace.StartsWith("Abim.Platform.");
        }

        /// <summary>
        /// Determines whether an enum is an Abim enum
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        /// <returns>
        ///   <c>true</c> if [is abim enum] [the specified enum type]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsAbimEnum(Type enumType)
        {
            bool oldStyle = enumType.Namespace.Contains("App.Domain") || enumType.Namespace.Contains("Abim.Platform.Program.Resource");
            bool newStyle = enumType.Namespace.Contains("Abim.Platform.Program.") && enumType.Namespace.Contains(".Enum");
            return (oldStyle || newStyle);
        }

        /// <summary>
        /// Determines whether an enum is an enum for this platform
        /// </summary>
        /// <param name="enumType">Type of the enum.</param>
        /// <returns>
        ///   <c>true</c> if [is platform enum] [the specified enum type]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsPlatformEnum(Type enumType)
        {
            bool newStyle = enumType.Namespace.ToLower().Contains(("Abim.Platform." + PlatformName()).ToLower());
            bool oldStyle = enumType.Namespace.ToLower().Contains($"Abim.Enterprise.Core.{PlatformName()}.Enum".ToLower());
            return (oldStyle || newStyle);
        }

        /// <summary>
        /// Gets the Platform name used in namespaces.
        /// </summary>
        /// <returns></returns>
        #pragma warning disable 0612        //this is just the default implementation
        protected virtual string PlatformName()
        {
            return ApiPlatform.PlatformName;
        }

        /// <summary>
        /// Determines whether a type is considered to be an aggregate root, for our purposes.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if [is aggregate root] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsAggregateRoot(Type type)
        {
            if(type.IsAggregateRoot()) return true;
            if(type.GetInterfaces().Any(i => i.Name.ToLower().Contains("aggregateroot"))) return true;
            return false;
        }
    }
}
