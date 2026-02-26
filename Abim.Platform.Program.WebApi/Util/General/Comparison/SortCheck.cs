using Abim.Platform.Program.Relational;
using Abim.Platform.Program.Relational.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Abim.Platform.Program.WebApi.Comparison
{
    /// <summary>
    /// Runs a sort check on an enumerable.
    /// </summary>
    public static class SortCheck
    {
        /// <summary>
        /// Determines whether the collection is successfully sorted numerically by an int field (expressed by the Func)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="enumValue">The enum value.</param>
        /// <param name="direction">The direction.</param>
        /// <returns>
        ///   <c>true</c> if [is sorted by enum] [the specified collection]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSorted<T>(IEnumerable<T> collection, Func<T, int> intValue, SortDirection direction)
        {
            var array = collection.ToArray();
            for(int i = 0; i < array.Length - 1; i++)
            {
                if(direction == SortDirection.Ascending && intValue(array[i]) > intValue(array[i + 1])) return false;
                else if(direction == SortDirection.Descending && intValue(array[i]) < intValue(array[i + 1])) return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether the collection is successfully sorted numerically by an int field (expressed by the Func)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="intValue">The int value.</param>
        /// <param name="direction">The direction.</param>
        /// <returns>
        ///   <c>true</c> if [is sorted by] [the specified int value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSortedBy<T>(this IEnumerable<T> collection, Func<T, int> intValue, SortDirection direction)
        {
            return IsSorted(collection, intValue, direction);
        }

        /// <summary>
        /// Determines whether the collection is successfully sorted alphabetically by a string field (expressed by the Func)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="enumValue">The enum value.</param>
        /// <param name="direction">The direction.</param>
        /// <returns>
        ///   <c>true</c> if [is sorted by enum] [the specified collection]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSorted<T>(IEnumerable<T> collection, Func<T, string> stringValue, SortDirection direction)
        {
            var array = collection.ToArray();
            for(int i = 0; i < array.Length - 1; i++)
            {
                if(direction == SortDirection.Ascending && string.Compare(stringValue(array[i]), stringValue(array[i + 1])) == 1) return false;
                else if(direction == SortDirection.Descending && string.Compare(stringValue(array[i]), stringValue(array[i + 1])) == -1) return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether the collection is successfully sorted alphabetically by a string field (expressed by the Func)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="stringValue">The string value.</param>
        /// <param name="direction">The direction.</param>
        /// <returns>
        ///   <c>true</c> if [is sorted by] [the specified string value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSortedBy<T>(this IEnumerable<T> collection, Func<T, string> stringValue, SortDirection direction)
        {
            return IsSorted(collection, stringValue, direction);
        }

        /// <summary>
        /// Determines whether the collection is successfully sorted bytewise (as in SQL) by a Guid field (expressed by the Func)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="guidValue">The enum value.</param>
        /// <param name="direction">The direction.</param>
        /// <returns>
        ///   <c>true</c> if [is sorted by enum] [the specified collection]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSorted<T>(IEnumerable<T> collection, Func<T, Guid> guidValue, SortDirection direction)
        {
            var array = collection.ToArray();
            for(int i = 0; i < array.Length - 1; i++)
            {
                if(direction == SortDirection.Ascending && GuidComparison.Compare(guidValue(array[i]), guidValue(array[i + 1])) == 1) return false;
                else if(direction == SortDirection.Descending && GuidComparison.Compare(guidValue(array[i]), guidValue(array[i + 1])) == -1) return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether the collection is successfully sorted bytewise (as in SQL) by a Guid field (expressed by the Func)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="guidValue">The unique identifier value.</param>
        /// <param name="direction">The direction.</param>
        /// <returns>
        ///   <c>true</c> if [is sorted by] [the specified unique identifier value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSortedBy<T>(this IEnumerable<T> collection, Func<T, Guid> guidValue, SortDirection direction)
        {
            return IsSorted(collection, guidValue, direction);
        }

        /// <summary>
        /// Determines whether the collection is successfully sorted alphabetically by an Object field (expressed by the Func)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="enumValue">The enum value.</param>
        /// <param name="direction">The direction.</param>
        /// <returns>
        ///   <c>true</c> if [is sorted by enum] [the specified collection]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSorted<T>(IEnumerable<T> collection, Func<T, Object> alphaObjectValue, SortDirection direction)
        {
            var array = collection.ToArray();
            for(int i = 0; i < array.Length - 1; i++)
            {
                if(direction == SortDirection.Ascending &&
                    string.Compare(alphaObjectValue(array[i]).ToString(), alphaObjectValue(array[i + 1]).ToString()) == 1)
                {
                    return false;
                }
                else if(direction == SortDirection.Descending &&
                    string.Compare(alphaObjectValue(array[i]).ToString(), alphaObjectValue(array[i + 1]).ToString()) == -1)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Determines whether the collection is successfully sorted alphabetically by an Object field (expressed by the Func)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="alphaObjectValue">The alpha object value.</param>
        /// <param name="direction">The direction.</param>
        /// <returns>
        ///   <c>true</c> if [is sorted by] [the specified alpha object value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSortedBy<T>(this IEnumerable<T> collection, Func<T, Object> alphaObjectValue, SortDirection direction)
        {
            return IsSorted(collection, alphaObjectValue, direction);
        }

        /// <summary>
        /// Determines whether the collection is successfully sorted alphabetically by an Enum field (expressed by the Func)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="enumValue">The enum value.</param>
        /// <param name="direction">The direction.</param>
        /// <returns>
        ///   <c>true</c> if [is sorted by enum] [the specified collection]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSortedOnEnum<T>(IEnumerable<T> collection, Func<T, Object> enumValue, SortDirection direction)
        {
            var array = collection.ToArray();
            for(int i = 0; i < array.Length - 1; i++)
            {
                if(direction == SortDirection.Ascending &&
                    string.Compare(EnumAttributes.ReadEnumValue(enumValue(array[i])), EnumAttributes.ReadEnumValue(enumValue(array[i + 1]))) == 1)
                {
                    return false;
                }
                else if(direction == SortDirection.Descending &&
                    string.Compare(EnumAttributes.ReadEnumValue(enumValue(array[i])), EnumAttributes.ReadEnumValue(enumValue(array[i + 1]))) == -1)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Determines whether the collection is successfully sorted alphabetically by an Enum field (expressed by the Func)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="enumValue">The enum value.</param>
        /// <param name="direction">The direction.</param>
        /// <returns>
        ///   <c>true</c> if [is sorted on enum by] [the specified enum value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSortedOnEnumBy<T>(this IEnumerable<T> collection, Func<T, Object> enumValue, SortDirection direction)
        {
            return IsSortedOnEnum(collection, enumValue, direction);
        }
    }
}
