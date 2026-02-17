using System.Collections.Generic;


namespace Abim.Platform.Program.Util
{
    /// <summary>
    /// IPagedCollectionResource interface
    /// </summary>
    public interface IPagedCollectionResource
    {
        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        /// <value>
        /// The current page.
        /// </value>
        int CurrentPage { get; set; }
        
        /// <summary>
        /// Gets or sets the total pages.
        /// </summary>
        /// <value>
        /// The total pages.
        /// </value>
        int TotalPages { get; set; }
        
        /// <summary>
        /// Gets or sets the total count.
        /// </summary>
        /// <value>
        /// The total count.
        /// </value>
        int TotalCount { get; set; }
        
        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        int PageSize { get; set; }
        
        /// <summary>
        /// Gets the links.
        /// </summary>
        /// <value>
        /// The links.
        /// </value>
        List<Link> Links { get; }
    }
}
