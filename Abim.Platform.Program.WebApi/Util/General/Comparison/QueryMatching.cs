using Abim.Platform.Program.Relational;

namespace Abim.Platform.Program.WebApi.Comparison
{
    /// <summary>
    /// QueryMatching class
    /// </summary>
    public static class QueryMatching
    {
        /// <summary>
        /// Checks whether two sorts match.
        /// </summary>
        /// <param name="sort1">The sort1.</param>
        /// <param name="sort2">The sort2.</param>
        /// <returns></returns>
        public static bool Matches(this Sort sort1, Sort sort2)
        {
            if(sort1.SortDirection != sort2.SortDirection) return false;
            if(sort1.SortBy != sort2.SortBy) return false;
            
            return true;
        }

        /// <summary>
        /// Checks whether two PageDefinitions match.
        /// </summary>
        /// <param name="page1">The page1.</param>
        /// <param name="page2">The page2.</param>
        /// <returns></returns>
        public static bool Matches(this PageDefinition page1, PageDefinition page2)
        {
            if(page1.PageIndex != page2.PageIndex) return false;
            if(page1.PageSize != page2.PageSize) return false;
            
            if((page1.Sorts == null) != (page2.Sorts == null)) return false;
            if(page1.Sorts != null)
            {
                if(page1.Sorts.Count != page2.Sorts.Count) return false;
                for(var i = 0; i < page1.Sorts.Count; i++)
                {
                    if(!page1.Sorts[i].Matches(page2.Sorts[i])) return false;
                }
            }
            
            return true;
        }

        /// <summary>
        /// Checks whether two queries match.
        /// </summary>
        /// <param name="base1">The base1.</param>
        /// <param name="base2">The base2.</param>
        /// <returns></returns>
        public static bool Matches(this ComplexQueryBase base1, ComplexQueryBase base2)
        {
            if(!base1.PageDefinition.Matches(base2.PageDefinition)) return false;
            
            return true;
        }
    }
}
