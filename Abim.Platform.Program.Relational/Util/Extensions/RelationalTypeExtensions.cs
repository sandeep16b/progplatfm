using Abim.Platform.Program.Relational.Domain;
using System;

namespace Abim.Platform.Program.Relational.Classes.Extensions
{
    /// <summary>
    /// Extension class for relational types
    /// </summary>
    public static class RelationalTypeExtensions
    {
        /// <summary>
        /// Determines whether the type is an aggregate root.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if [is aggregate root] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAggregateRoot(this Type type)
        {
            //cannot say "typeof(AggregateRoot<>).MakeGenericType(type).IsInheritableFrom(type)" because the Where constraint on AggregateRoot is self-referencing
            return (type.BaseType.Name == (typeof(AggregateRoot<>)).Name && type.BaseType.Namespace == (typeof(AggregateRoot<>)).Namespace);
        }
    }
}
