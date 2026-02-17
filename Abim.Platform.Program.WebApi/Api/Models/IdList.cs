using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.WebApi.Api
{
    /// <summary>
    /// A list of Ids posted
    /// </summary>
    public class IdList
    {
        /// <summary>
        /// Gets or sets the ids.
        /// </summary>
        /// <value>
        /// The ids.
        /// </value>
        public List<Guid> Ids { get; set; }
    }
}
