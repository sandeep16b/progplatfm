using System;

namespace Abim.Platform.Program.WebApi.Response
{
    /// <summary>
    /// A basic structure to describe an enum to be output by our resource classes.
    /// </summary>
    public class EnumDefinition
    {
        /// <summary>
        /// Gets or sets the name of the enum.  This is used to generate URLs and log messages.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the enum.
        /// </summary>
        public Type Type { get; set; }
    }
}
