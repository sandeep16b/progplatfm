using System;

namespace Abim.Platform.Program.WebApi.Api.Attributes
{
    /// <summary>
    /// This represents a feature which is currently in the decision process and might become obsolete. By contrast, the ObsoleteAttribute
    /// is reserved for functionality which is already obsolete, but which hasn't been deleted for backwards compatibility.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.All)]
    public class PotentiallyObsoleteAttribute : Attribute
    {
    }
}
