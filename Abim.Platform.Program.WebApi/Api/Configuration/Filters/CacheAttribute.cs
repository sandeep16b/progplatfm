using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Abim.Platform.Program.WebApi.Filters
{
    /// <summary>
    /// CacheAttribute Class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class CacheAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// The cache
        /// </summary>
        private static readonly ObjectCache Cache = MemoryCache.Default;

        /// <summary>
        /// Gets or sets the cache key.
        /// </summary>
        /// <value>
        /// The cache key.
        /// </value>
        private string cacheKey { get; set; }

        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        /// <value>
        /// The server.
        /// </value>
        public int Server { get; set; }

        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        public int Client { get; set; }

        /// <summary>
        /// Called when [action executed asynchronous].
        /// </summary>
        /// <param name="actionExecutedContext">The action executed context.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public override async Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            if(!Cache.Contains(cacheKey))
            {
                if(actionExecutedContext.Response.Content != null)
                {
                    var body = await actionExecutedContext.Response.Content.ReadAsByteArrayAsync();
                    var cacheItem = new WebCacheItem(actionExecutedContext.Response.Content.Headers.ContentType, body);

                    Cache.Add(cacheKey, cacheItem, DateTime.Now.AddSeconds(Server));
                }
                else
                {
                    return;
                }
            }

            if(IsCacheable(Client, actionExecutedContext.ActionContext))
            {
                actionExecutedContext.Response.Headers.CacheControl = GetClientCache();
            }
        }

        /// <summary>
        /// OnActionExecuting method.
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if(IsCacheable(Server, actionContext))
            {
                var accept = actionContext.Request.Headers.Accept.FirstOrDefault() ?? new MediaTypeHeaderValue("application/json");
                cacheKey = string.Format("{0}|{1}", actionContext.Request.RequestUri.PathAndQuery, accept);
                var cacheContent = Cache.Get(cacheKey) as WebCacheItem;

                if(cacheContent == null || !cacheContent.IsValid()) return;

                actionContext.Response = actionContext.Request.CreateResponse();
                actionContext.Response.Content = new ByteArrayContent(cacheContent.Content);
                actionContext.Response.Content.Headers.ContentType = new MediaTypeHeaderValue(cacheContent.ContentType);

                if(IsCacheable(Client, actionContext))
                {
                    actionContext.Response.Headers.CacheControl = GetClientCache();
                }
            }
        }

        #region Impl methods

        /// <summary>
        /// isCacheable method.
        /// </summary>
        /// <returns>
        /// Per RFC2616 only get and heads should be cacheable.
        /// </returns>
        private bool IsCacheable(int prop, HttpActionContext context)
        {
            if(prop <= 0) return false;

            return context.Request.Method == HttpMethod.Get || context.Request.Method == HttpMethod.Head;
        }

        /// <summary>
        /// GetClientCache method.
        /// </summary>
        /// <returns></returns>
        private CacheControlHeaderValue GetClientCache()
        {
            var cacheControl = new CacheControlHeaderValue
            {
                MaxAge = TimeSpan.FromSeconds(Client),
                MustRevalidate = true
            };

            return cacheControl;
        }

        #endregion
    }
}