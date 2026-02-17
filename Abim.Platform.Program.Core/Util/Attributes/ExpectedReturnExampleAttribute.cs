using System;

namespace Abim.Platform.Program.Core.Api.Attributes
{
    /// <summary>
    /// Can be used to mark a method with an example return value
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Method)]
    public class ExpectedReturnExampleAttribute : Attribute
    {
        /// <summary>
        /// The example arguments.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        public object[] Arguments { get; set; }
        
        /// <summary>
        /// The expected return value for these arguments.
        /// </summary>
        /// <value>
        /// The should return.
        /// </value>
        public object ShouldReturn { get; set; }
    }
}
