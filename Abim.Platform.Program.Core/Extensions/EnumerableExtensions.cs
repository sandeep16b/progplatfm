using Abim.Platform.Program.Util.Objects.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Abim.Platform.Program.Util.Extensions
{
    /// <summary>
    /// Extension class
    /// </summary>
    public static class EnumerableExtensions
    {

        /// <summary>
        /// System.Linq does not provide this for the non-generic IEnumerable.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public static int Count(this IEnumerable collection)
        {
            int result = 0;
            foreach (Object o in collection) result++;
            return result;
        }

        /// <summary>
        /// Picks one entry.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static T Pick<T>(this IEnumerable<T> data)
        {
            return Selector.Select<T>(data.ToArray());
        }

        /// <summary>
        /// Returns all entries except for certain ones (a more convenient method than the normal .Except()).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        public static List<T> Except<T>(this IEnumerable<T> data, params T[] items)
        {
            return data.Except<T>(items.ToList()).ToList();
        }

        /// <summary>
        /// Reverseds the specified data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static List<T> Reversed<T>(this IEnumerable<T> data)
        {
            var copy = new List<T>();
            foreach (var entry in data) copy.Add(entry);
            copy.Reverse();
            return copy;
        }

        /// <summary>
        /// Returns the minimum, based on some func.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <param name="convertToNumber">The convert to number.</param>
        /// <returns></returns>
        public static T MinEntry<T>(this IEnumerable<T> data, Func<T, long> convertToNumber)
        {
            var dataLst = data.ToList();
            if (!dataLst.Any()) return default(T);

            long min = convertToNumber(dataLst.First());
            T minValue = dataLst.First();
            foreach (var entry in dataLst)
            {
                var number = convertToNumber(entry);
                if (number < min)
                {
                    minValue = entry;
                    min = number;
                }
            }
            return minValue;
        }

        /// <summary>
        /// Returns the maximum, based on some func.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <param name="convertToNumber">The func that returns a comparable number for a given entry.</param>
        /// <returns></returns>
        public static T MaxEntry<T>(this IEnumerable<T> data, Func<T, long> convertToNumber)
        {
            var dataLst = data.ToList();
            if (!dataLst.Any()) return default(T);
            long max = convertToNumber(dataLst.First());
            T maxValue = dataLst.First();
            foreach (var entry in dataLst)
            {
                var number = convertToNumber(entry);
                if (number > max)
                {
                    maxValue = entry;
                    max = number;
                }
            }
            return maxValue;
        }

        /// <summary>
        /// Returns a List of this collection minus the last element (length being 1 shorter than before).
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static List<T> WithoutLast<T>(this IEnumerable<T> data)
        {
            if (data == null) return null;
            if (data.Count() == 0) return new List<T>();
            return data.ToArray().SubArray(0, data.Count() - 1).ToList();
        }

        /// <summary>
        /// Makes a comma-separated list as a string, with the word "and" before the last entry.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public static string JoinWithAnd<TObject>(this IEnumerable<TObject> collection)
        {
            return StringExtensions.JoinWithAnd(collection);
        }

        /// <summary>
        /// Makes a comma-separated list as a string, with the word "or" before the last entry.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <returns></returns>
        public static string JoinWithOr<TObject>(this IEnumerable<TObject> collection)
        {
            return StringExtensions.JoinWithOr(collection);
        }

        /// <summary>
        /// Makes a comma/semicolon (depending on the joiner) separated list as a string, with the word "and" before the last entry.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="joiner">The joiner.</param>
        /// <returns></returns>
        public static string JoinWithAnd<TObject>(this IEnumerable<TObject> collection, string joiner)
        {
            return StringExtensions.JoinWithAnd(joiner, collection);
        }

        /// <summary>
        /// Makes a comma/semicolon (depending on the joiner) separated list as a string, with the word "or" before the last entry.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="joiner">The joiner.</param>
        /// <returns></returns>
        public static string JoinWithOr<TObject>(this IEnumerable<TObject> collection, string joiner)
        {
            return StringExtensions.JoinWithOr(joiner, collection);
        }

        /// <summary>
        /// Joins the collection.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="joiner">The joiner.</param>
        /// <returns></returns>
        public static string Join<TObject>(this IEnumerable<TObject> collection, string joiner)
        {
            return string.Join(joiner, collection.ToArray());
        }

        /// <summary>
        /// Divides a collection into multiple collections, based on some rule.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="collection">The collection.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>The new collections</returns>
        public static List<List<T>> Divide<T, TValue>(this IEnumerable<T> collection, Func<T, TValue> predicate)
        {
            var dict = new Dictionary<TValue, List<T>>();
            var nulls = new List<T>();
            foreach (T value in collection)
            {
                var result = predicate(value);
                if (result == null) nulls.Add(value);
                else
                {
                    if (!dict.ContainsKey(result)) dict[result] = new List<T>();
                    dict[result].Add(value);
                }
            }
            var returnList = new List<List<T>>();
            foreach (var key in dict.Keys) returnList.Add(dict[key]);
            if (nulls.Any()) returnList.Add(nulls);
            return returnList;
        }


    }
}
