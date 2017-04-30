using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnnualReports.Common.Extensions
{
    /// <summary>
    /// Extensions for IEnumerable objects
    /// </summary>

    public static class LinqExtensions
    {
        /// <summary>
        /// Loop through IEnumerable with ability to get loop index.
        /// </summary>
        /// <typeparam name="T">enumerable object type to operate on</typeparam>
        /// <param name="source">The enumerable source to loop on.</param>
        /// <param name="handler">The loop handler or body.</param>
        public static void ForEachWithIndex<T>(this IEnumerable<T> source, Action<T, int> handler)
        {
            int idx = 0;
            foreach (T item in source)
            {
                handler(item, idx++);
            }
        }

        /// <summary>
        /// Return empty collection in case of null collection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original">The original collection.</param>
        /// <returns></returns>
        public static IEnumerable<T> AsNotNull<T>(this IEnumerable<T> original)
        {
            return original ?? new T[0];
        }
    }
}
