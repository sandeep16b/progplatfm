using System.Net.Http.Headers;

namespace Abim.Platform.Program.WebApi.Filters
{
    /// <summary>
    /// WebCacheItem Class.
    /// </summary>
    public class WebCacheItem
    {
        /// <summary>
        /// Gets the type of the content.
        /// </summary>
        /// <value>
        /// The type of the content.
        /// </value>
        public string ContentType { get; private set; }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public byte[] Content { get; private set; }

        /// <summary>
        /// WebCacheItem Constructor
        /// </summary>
        /// <param name="header"></param>
        /// <param name="content"></param>
        public WebCacheItem(MediaTypeHeaderValue header, byte[] content)
        {
            Content     = content;
            ContentType = header.MediaType;
        }
        
        /// <summary>
        /// IsValid method
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            return ContentType != null && Content != null;
        }
    }
}