using Abim.Platform.Program.Util.Extensions;
using Abim.Platform.Program.WebApi.Exceptions;
using System;

namespace Abim.Platform.Program.WebApi.Testing.Setup.Builders
{
    /// <summary>
    /// Static non-generic object-building class
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.WebApi.Testing.Setup.Builders.BaseObjectBuilder{T,Abim.Platform.Program.WebApi.Testing.Setup.Builders.ObjectBuilder{T}}" />
    public static class StaticObjectBuilder
    {
        /// <summary>
        /// Builds an ObjectBuilder<> with the specified type as its "T" type, then calls Build() on it and returns the result
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static dynamic Build(Type type)
        {
            if(type.GetConstructor(Type.EmptyTypes) == null)
            {
                var msg = string.Format("Cannot create an ObjectBuilder<T> for T type {0} because that type doesn't have a parameterless public contructor",
                    type.ReadableName());
                throw new InputException(msg);
            }
            var genericBuilderType = (typeof(ObjectBuilder<>).MakeGenericType(type));
            Object builderInstance = Activator.CreateInstance(genericBuilderType);
            return (builderInstance as dynamic).Build();
        }
        
        /// <summary>
        /// A version of Build() that returns Object instead of dynamic.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        public static Object BuildObject(Type type)
        {
            return Build(type) as Object;
        }
        
        /// <summary>
        /// Builds an ObjectBuilder<> with the specified type as its "T" type, then calls Build() on it and returns the result. Make sure the targetted assembly
        /// is loaded first
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static dynamic Build(string typeName)
        {
            var type = TypeExtensions.GetType(typeName);
            return Build(type);
        }

        /// <summary>
        /// A version of Build() that returns Object instead of dynamic.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        public static Object BuildObject(string typeName)
        {
            return Build(typeName) as Object;
        }
    }
}
