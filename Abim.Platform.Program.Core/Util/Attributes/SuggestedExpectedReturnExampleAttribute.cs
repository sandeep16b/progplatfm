using System;

namespace Abim.Platform.Program.Core.Api.Attributes
{
    /// <summary>
    /// A child class of the ExpectedReturnExampleAttribute for automated use
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Method)]
    public class SuggestedExpectedReturnExampleAttribute : ExpectedReturnExampleAttribute
    {
    }
}
