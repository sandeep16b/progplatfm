using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Abim.Platform.Program.Relational.Classes
{
    /// <summary>
    /// Sorts a collection based on a tiered list of Sorts
    /// </summary>
    public static class PropertySort
    {
        /// <summary>
        /// Sorts by the ordered list of sorts.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="sorts">The sorts.</param>
        /// <param name="throwExceptions">if set to <c>true</c> [throw exceptions].</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">
        /// Collection to sort cannot be null
        /// or
        /// Invalid SortBy. SortBy cannot be null or empty.
        /// or
        /// Invalid SortBy. Nested properties are not supported.
        /// or
        /// Invalid SortBy. Property does not exist.
        /// or
        /// Invalid SortBy. Multiple properties exist that match the name, none by case. Please correct your casing.
        /// </exception>
        public static IEnumerable<T> SortBy<T>(IEnumerable<T> collection, List<Sort> sorts, bool throwExceptions = true)
        {
            if(collection == null) throw new Exception("Collection to sort cannot be null");
            PropertyInfo[] props = typeof(T).GetProperties();
            IOrderedEnumerable<T> orderedCollection = null;
            foreach(var sort in sorts)
            {
                if(string.IsNullOrEmpty(sort.SortBy))
                {
                    if(throwExceptions) throw new Exception("Invalid SortBy. SortBy cannot be null or empty.");
                    else continue;
                }
                if(sort.SortBy.Contains("."))
                {
                    if(throwExceptions) throw new Exception("Invalid SortBy. Nested properties are not supported.");
                    else continue;
                }
                PropertyInfo prop = props.FirstOrDefault(p => p.Name == sort.SortBy);
                if(prop == null)
                {
                    var anyCaseMatches = props.Where(p => p.Name.ToLower() == sort.SortBy.ToLower()).ToList();
                    if(anyCaseMatches.Count() == 1) prop = anyCaseMatches.First();
                    else if(anyCaseMatches.Count() == 0)
                    {
                        if(throwExceptions) throw new Exception("Invalid SortBy. Property does not exist.");
                        else continue;
                    }
                    else if(anyCaseMatches.Count() > 1)
                    {
                        if(throwExceptions) throw new Exception("Invalid SortBy. Multiple properties exist that match the name, none by case. Please correct your casing.");
                        else continue;
                    }
                }
                if(orderedCollection == null)
                {
                    if(sort.SortDirection == SortDirection.Ascending)
                        orderedCollection = collection.OrderBy(x => prop.GetValue(x));
                    else
                        orderedCollection = collection.OrderByDescending(x => prop.GetValue(x));
                }
                else
                {
                    if(sort.SortDirection == SortDirection.Ascending)
                        orderedCollection = orderedCollection.ThenBy(x => prop.GetValue(x));
                    else
                        orderedCollection = orderedCollection.ThenByDescending(x => prop.GetValue(x));
                }
            }
            return orderedCollection ?? collection;
        }
    }
}
