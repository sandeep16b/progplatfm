using Abim.Platform.Program.Util;

namespace Abim.Platform.Program.WebApi.Response.AppInfo
{
    /// <summary>
    /// The reason for this class is that we can't XML-serialize an anonymous type
    /// </summary>
    /// <remarks>
    /// in camelCase because this is used for both json (for which camelCase is standard) and xml
    /// </remarks>
    public class SystemInfo
    {
        /// <summary>
        /// Gets or sets the links.
        /// </summary>
        /// <value>
        /// The links.
        /// </value>
        public Link[] links { get; set; }
    }
}
