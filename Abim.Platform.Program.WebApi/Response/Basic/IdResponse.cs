using Abim.Platform.Program.Util;
using System.Collections.Generic;
using System.Web.Http.Routing;

namespace Abim.Platform.Program.WebApi
{
    /// <summary>
    /// A response object to return a long ID as a JSON response along with links.
    /// </summary>
    public class IdResponse
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public long Id { get; protected set; }

        /// <summary>
        /// Gets or sets the links.
        /// </summary>
        /// <value>
        /// The links.
        /// </value>
        public IEnumerable<Link> Links { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdResponse"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="urlHelper">The URL helper.</param>
        /// <param name="routeName">Name of the route.</param>
        public IdResponse(long id, UrlHelper urlHelper, string routeName)
        {
            Id    = id;
            Links = new[]
            {
                new Link(HttpVerbs.Get, urlHelper.Link(routeName, new {id})),
            };
        }
    }
}