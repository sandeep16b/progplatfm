using Abim.Platform.Program.Util;

namespace Abim.Platform.Program.WebApi.Response.AppInfo
{
    /// <summary>
    /// The reason for this class is that we can't XML-serialize an anonymous type
    /// </summary>
    /// <remarks>
    /// in camelCase because this is used for both json (for which camelCase is standard) and xml
    /// </remarks>
    public class VersionInfo
    {
        /// <summary>
        /// Gets or sets the version information.
        /// </summary>
        /// <value>
        /// The version information.
        /// </value>
        public VersionDetails versionInfo { get; set; }

        /// <summary>
        /// Gets or sets the links.
        /// </summary>
        /// <value>
        /// The links.
        /// </value>
        public Link[] links { get; set; }
    }
    public class VersionDetails
    {
        /// <summary>
        /// Gets or sets the assembly version.
        /// </summary>
        /// <value>
        /// The assembly version.
        /// </value>
        public System.Version assemblyVersion { get; set; }

        /// <summary>
        /// Gets or sets the file version.
        /// </summary>
        /// <value>
        /// The file version.
        /// </value>
        public string fileVersion { get; set; }

        /// <summary>
        /// Gets or sets the product version.
        /// </summary>
        /// <value>
        /// The product version.
        /// </value>
        public string productVersion { get; set; }
    }
}
