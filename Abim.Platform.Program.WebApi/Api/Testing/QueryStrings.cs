using Abim.Platform.Program.Relational;
using System.Linq;

namespace Abim.Platform.Program.WebApi.Testing
{
    /// <summary>
    /// Query string helper class
    /// </summary>
    public static class QueryStrings
    {
        /// <summary>
        /// Gets a page definition query string
        /// </summary>
        /// <param name="paging">The paging.</param>
        /// <returns></returns>
        public static string PageDefinitionQueryStringFor(PageDefinition paging)
        {
            var str = string.Format("PageIndex={0}&PageSize={1}", paging.PageIndex, paging.PageSize);
            if(paging.Sorts != null && paging.Sorts.Any())
            {
                for(var i = 0; i < paging.Sorts.Count; i++)
                {
                    str += string.Format("&Sorts[{0}].SortBy={1}", i, paging.Sorts[i].SortBy);
                    str += string.Format("&Sorts[{0}].SortDirection={1}", i, paging.Sorts[i].SortDirection);
                }
            }
            return str;
        }

        /// <summary>
        /// Gets a page definition query string for a complex query
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public static string PageDefinitionQueryStringFor(ComplexQueryBase query)
        {
            var str = string.Format("PageIndex={0}&PageSize={1}", query.PageDefinition.PageIndex, query.PageDefinition.PageSize);
            if(query.PageDefinition.Sorts != null && query.PageDefinition.Sorts.Any())
            {
                for(var i = 0; i < query.PageDefinition.Sorts.Count; i++)
                {
                    str += string.Format("&Sorts[{0}].SortBy={1}", i, query.PageDefinition.Sorts[i].SortBy);
                    str += string.Format("&Sorts[{0}].SortDirection={1}", i, query.PageDefinition.Sorts[i].SortDirection);
                }
            }
            return str;
        }

        /// <summary>
        /// Gets a query string for a complex query
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public static string QueryStringFor(ComplexQueryBase query)
        {
            var str = string.Format("PageIndex={0}&PageSize={1}", query.PageDefinition.PageIndex, query.PageDefinition.PageSize);
            if(query.PageDefinition.Sorts != null && query.PageDefinition.Sorts.Any())
            {
                for(var i = 0; i < query.PageDefinition.Sorts.Count; i++)
                {
                    str += string.Format("&Sorts[{0}].SortBy={1}", i, query.PageDefinition.Sorts[i].SortBy);
                    str += string.Format("&Sorts[{0}].SortDirection={1}", i, query.PageDefinition.Sorts[i].SortDirection);
                }
            }
            //no support yet for searching (this would go here)
            return str;
        }
    }
}
