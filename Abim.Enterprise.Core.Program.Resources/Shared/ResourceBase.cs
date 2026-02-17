using Abim.Platform.Program.Util;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Abim.Platform.Program.Resources
{
    /// <summary>
    /// ResourceBase
    /// </summary>
    [DataContract]
    public abstract class ResourceBase
    {
        /// <summary>
        /// Gets the links.
        /// </summary>
        /// <value>
        /// The links.
        /// </value>
        [DataMember(Order = 1)]
        public List<Link> Links { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceBase"/> class.
        /// </summary>
        protected ResourceBase()
        {
            Links = new List<Link>();
        }
    }
}
