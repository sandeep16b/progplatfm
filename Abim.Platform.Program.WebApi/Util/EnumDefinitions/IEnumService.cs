using Abim.Platform.Program.WebApi.Response;
using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.WebApi.Objects
{
    /// <summary>
    /// Enum service interface
    /// </summary>
    public interface IEnumService
    {
        /// <summary>
        /// Gets the enum definitions.
        /// </summary>
        /// <returns></returns>
        IList<EnumDefinition> GetEnumDefinitions();

        /// <summary>
        /// Gets the enum definitions.
        /// </summary>
        /// <param name="typesRequested">The types requested.</param>
        /// <returns></returns>
        IList<EnumDefinition> GetEnumDefinitions(IEnumerable<Type> typesRequested);

        /// <summary>
        /// Gets the enum definitions for a given type
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="alsoIncludeEnumsInNestedNonAggregateRootTypes">if set to <c>true</c> [also include enums in nested non aggregate root types].</param>
        /// <returns></returns>
        IList<EnumDefinition> GetEnumDefinitionsFor(Type modelType, bool alsoIncludeEnumsInNestedNonAggregateRootTypes);

        /// <summary>
        /// Gets the enum types for a given type
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="alsoIncludeEnumsInNestedNonAggregateRootTypes">if set to <c>true</c> [also include enums in nested non aggregate root types].</param>
        /// <returns></returns>
        IList<Type> GetEnumTypesFor(Type modelType, bool alsoIncludeEnumsInNestedNonAggregateRootTypes);
    }
}
