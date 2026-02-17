using System;

namespace Abim.Platform.Program.Util.Extensions
{
    /// <summary>
    /// Extension class
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Returns a subarray.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">The array.</param>
        /// <param name="start">The start.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Cannot create subarray starting at index " + start</exception>
        public static T[] SubArray<T>(this T[] array, int start, int length)
        {
            if(start < 0) throw new Exception("Cannot create subarray starting at index " + start);
            int finalLength = Math.Min(array.Length - start, length);
            T[] ret = new T[finalLength];
            for(int i = 0; i < finalLength; i++)
                ret[i] = array[start + i];
            return ret;
        }
    }
}
