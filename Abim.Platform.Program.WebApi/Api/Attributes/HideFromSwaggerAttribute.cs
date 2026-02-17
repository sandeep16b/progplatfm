using System;

namespace Abim.Platform.Program.WebApi.Api.Attributes
{
    /// <summary>
    /// Represents an endpoint or property which should be hidden from Swagger documentation.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class HideFromSwaggerAttribute : Attribute
    {
    }
}
