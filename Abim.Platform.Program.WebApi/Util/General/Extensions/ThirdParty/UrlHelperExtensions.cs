using System.Web.Http.Routing;

namespace Abim.Platform.Program.WebApi.Objects.Extensions
{
    /// <summary>
    /// Extension class
    /// </summary>
    public static class UrlHelperExtensions
    {
        /// <remarks>
        /// Collection resources' self links can only be Get or Post
        /// </remarks>
        public static string CollectionSelfLinkMethod(this UrlHelper urlHelper)
        {
            var currentMethod = urlHelper.Request.Method.ToString().ToUpper();
            if(currentMethod == "GET" || currentMethod == "POST") return currentMethod;
            return "GET";
        }
    }
}
