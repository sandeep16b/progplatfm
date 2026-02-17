using Abim.Platform.Program.Relational;
using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.WebApi.Testing.Setup.Builders
{
    /// <summary>
    /// Constructs a PageDefinition or the query string representing one
    /// </summary>
    public class PageDefinitionBuilder
    {
        /// <summary>
        /// Gets or sets the index of the page.
        /// </summary>
        /// <value>
        /// The index of the page.
        /// </value>
        public int PageIndex { get; set; }

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>
        /// The size of the page.
        /// </value>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the sorts.
        /// </summary>
        /// <value>
        /// The sorts.
        /// </value>
        public List<SortData> Sorts { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageDefinitionBuilder"/> class.
        /// </summary>
        public PageDefinitionBuilder()
        {
            Sorts = new List<SortData>();
        }
        
        /// <summary>
        /// Sets the PageIndex
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <returns></returns>
        public PageDefinitionBuilder WithPageIndex(int pageIndex)
        {
            PageIndex = pageIndex;
            return this;
        }

        /// <summary>
        /// Sets the PageSize
        /// </summary>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        public PageDefinitionBuilder WithPageSize(int pageSize)
        {
            PageSize = pageSize;
            return this;
        }

        /// <summary>
        /// Adds a Sort to the list of Sorts
        /// </summary>
        /// <param name="sort">The sort.</param>
        /// <returns></returns>
        public PageDefinitionBuilder WithSort(string sortBy, string sortDirection)
        {
            var sortData = new SortData()
            {
                SortBy = sortBy,
                SortDirection = sortDirection
            };
            Sorts.Add(sortData);
            return this;
        }

        /// <summary>
        /// Builds the PageDefinition object.
        /// </summary>
        /// <returns></returns>
        public PageDefinition BuildObject()
        {
            var sortList = new List<Sort>();
            foreach(var sortData in Sorts)
            {
                var sort = new Sort()
                {
                    SortBy = sortData.SortBy,
                    SortDirection = (SortDirection)Enum.Parse(typeof(SortDirection), sortData.SortDirection)
                };
                sortList.Add(sort);
            }
            return new PageDefinition()
            {
                PageIndex = PageIndex,
                PageSize = PageSize,
                Sorts = sortList
            };
        }

        /// <summary>
        /// Builds the query string.
        /// </summary>
        /// <returns></returns>
        public string BuildQueryString()
        {
            var queryString = string.Format("PageIndex={0}&PageSize={1}", PageIndex, PageSize);
            for(var i = 0; i < Sorts.Count; i++)
            {
                queryString += string.Format("&Sorts[{0}].SortBy={1}", i, Sorts[i].SortBy);
                queryString += string.Format("&Sorts[{0}].SortDirection={1}", i, Sorts[i].SortDirection);
            }
            return queryString;
        }

        /// <summary>
        /// Builds a query string for a ComplexQuery.
        /// </summary>
        /// <returns></returns>
        public string BuildComplexQueryQueryString(string prefix = null)
        {
            if(prefix == null) prefix = "PageDefinition";
            var queryString = string.Format("{0}.PageIndex={1}&{0}.PageSize={2}", prefix, PageIndex, PageSize);
            for(var i = 0; i < Sorts.Count; i++)
            {
                queryString += string.Format("&{0}.Sorts[{1}].SortBy={2}", prefix, i, Sorts[i].SortBy);
                queryString += string.Format("&{0}.Sorts[{1}].SortDirection={2}", prefix, i, Sorts[i].SortDirection);
            }
            return queryString;
        }
    }

    /// <summary>
    /// represents user input for a Sort, with string properties since the user can pass any [valid or invalid] value in a query string
    /// </summary>
    public class SortData
    {
        /// <summary>
        /// Gets or sets the sort by.
        /// </summary>
        /// <value>
        /// The sort by.
        /// </value>
        public string SortBy { get; set; }

        /// <summary>
        /// Gets or sets the sort direction.
        /// </summary>
        /// <value>
        /// The sort direction.
        /// </value>
        public string SortDirection { get; set; }
    }
}
