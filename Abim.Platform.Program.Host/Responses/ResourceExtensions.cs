using Abim.Platform.Program.Relational;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Util;
using Abim.Platform.Program.WebApi.Objects;
using Abim.Platform.Program.WebApi.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Routing;


namespace Abim.Platform.Program.Host.Extensions
{
    /// <summary>
    /// Extensions class for Resources
    /// </summary>
    public static class ResourceExtensions
    {
        /// <summary>
        /// The cached enum definitions for types
        /// </summary>
        private static readonly Dictionary<Type, IList<EnumDefinition>> CachedEnumDefinitionsForTypes = new Dictionary<Type, IList<EnumDefinition>>();
        
        /// <summary>
        /// Sets the enum links.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="urlHelper">The URL helper.</param>
        /// <param name="getEnumRouteName">Name of the get enum route.</param>
        /// <param name="includeNestedEnums">if set to <c>true</c> [include nested enums].</param>
        /// <exception cref="System.Exception">Container is null</exception>
        public static void SetEnumLinksForType<TEntity>(this ResourceBase resource, UrlHelper urlHelper, string getEnumRouteName, bool includeNestedEnums)
        {
            if(urlHelper == null) return;
            if(DependencyResolver.Container == null) throw new Exception("Container is null");
            if(!CachedEnumDefinitionsForTypes.ContainsKey(typeof(TEntity)))
            {
                var enumService = DependencyResolver.Container.GetInstance<IEnumService>();
                if(enumService == null) throw new Exception("No IEnumService instance found, in SetEnumLinksForType()");
                CachedEnumDefinitionsForTypes[typeof(TEntity)] = enumService.GetEnumDefinitionsFor(typeof(TEntity), includeNestedEnums);
            }
            var enumDefinitions = CachedEnumDefinitionsForTypes[typeof(TEntity)];
            foreach(var e in enumDefinitions)
            {
                resource.Links.Add(new Link(e.Type.Name, HttpVerbs.Get, urlHelper.Link(getEnumRouteName, new { name = e.Name })));
            }
        }

        /// <summary>
        /// Sets the enum links.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="enumTypes">The enum types.</param>
        /// <param name="urlHelper">The URL helper.</param>
        /// <param name="getEnumRouteName">Name of the get enum route.</param>
        public static void SetEnumLinks(this ResourceBase resource, List<Type> enumTypes, UrlHelper urlHelper, string getEnumRouteName)
        {
            var enumService = DependencyResolver.Container.GetInstance<IEnumService>();
            if(enumService == null) throw new Exception("No IEnumService instance found, in SetEnumLinks()");
            var specificEnumDefinitions = enumService.GetEnumDefinitions(enumTypes);
            foreach(var e in specificEnumDefinitions)
            {
                resource.Links.Add(new Link(e.Type.Name, HttpVerbs.Get, urlHelper.Link(getEnumRouteName, new { name = e.Name })));
            }
        }

        /// <summary>
        /// Gets the self link, if any
        /// </summary>
        /// <param name="resource">The ?.</param>
        public static Link GetSelfLink(this ResourceBase resource)
        {
            return resource.Links.FirstOrDefault(l => l.Name == "self");
        }

        /// <summary>
        /// Sets the paging properties and links on a collection resource.
        /// </summary> 
        /// <param name="resource">The resource.</param>
        /// <param name="pageDefinition">The page definition.</param>
        /// <param name="totalCount">The total count.</param>
        /// <param name="urlHelper">urlHelper.</param>
        /// <param name="usesComplexQuery">if set to <c>true</c> [uses complex query].</param>
        public static void SetPaging(this IPagedCollectionResource resource, PageDefinition pageDefinition, int totalCount, UrlHelper urlHelper,
            bool usesComplexQuery = false)
        {
            if(pageDefinition == null) throw new Exception("PageDefinition is null in SetPaging()");
            if(resource == null) throw new Exception("Resource is null in SetPaging()");
            resource.TotalPages = (int)(Math.Ceiling((float)totalCount / pageDefinition.PageSize));
            if(resource.TotalPages == 0) resource.TotalPages = 1;
            resource.TotalCount = totalCount;
            resource.CurrentPage = pageDefinition.PageIndex;
            resource.PageSize = pageDefinition.PageSize;
            
            SetPagingLinksFromSelfLink(resource, urlHelper, usesComplexQuery);
        }

        /// <summary>
        /// Sets the resource's paging links from its self link.
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="urlHelper">The URL helper.</param>
        /// <param name="usesComplexQuery">if set to <c>true</c> [uses complex query].</param>
        public static void SetPagingLinksFromSelfLink(this IPagedCollectionResource resource, UrlHelper urlHelper, bool usesComplexQuery = false)
        {
            if(resource.CurrentPage < 1 || resource.CurrentPage > resource.TotalPages)
                return;
            
            var originalUri = urlHelper.Request.RequestUri;
            var selfLink = resource.Links.FirstOrDefault(link => link.Name == "self");
            if(selfLink == null)
                return;
            
            string pageIndexQueryParameter = "pageIndex";
            
            string constPossibility1 = "complexQuery.pageDefinition";
            string constPossibility2 = "pageDefinition";
            if(usesComplexQuery)
            {
                pageIndexQueryParameter = constPossibility1 + "." + pageIndexQueryParameter;
            }
            else
            {
                if(originalUri.Query.ToLower().Contains(constPossibility1.ToLower()))
                {
                    pageIndexQueryParameter = originalUri.Query.Substring(originalUri.Query.ToLower().IndexOf(constPossibility1.ToLower()), constPossibility1.Length)
                        + "." + pageIndexQueryParameter;
                }
                else if(originalUri.Query.ToLower().Contains(constPossibility2.ToLower()))
                {
                    pageIndexQueryParameter = originalUri.Query.Substring(originalUri.Query.ToLower().IndexOf(constPossibility2.ToLower()), constPossibility2.Length)
                        + "." + pageIndexQueryParameter;
                }
            }
            
            var prevPage = resource.CurrentPage - 1;
            if(prevPage > 0)
            {
                var queryString = HttpUtility.ParseQueryString(originalUri.Query);
                queryString.Set(pageIndexQueryParameter, prevPage.ToString());
                
                var builder = new UriBuilder(selfLink.Href) { Query = queryString.ToString() };
                resource.Links.Add(new Link("prev", HttpVerbs.Get, builder.Uri.ToString()));
            }
            
            var nextPage = resource.CurrentPage + 1;
            if(nextPage <= resource.TotalPages)
            {
                var queryString = HttpUtility.ParseQueryString(originalUri.Query);
                queryString.Set(pageIndexQueryParameter, nextPage.ToString());
                
                var builder = new UriBuilder(selfLink.Href) { Query = queryString.ToString() };
                resource.Links.Add(new Link("next", HttpVerbs.Get, builder.Uri.ToString()));
            }
        }
    }
}
