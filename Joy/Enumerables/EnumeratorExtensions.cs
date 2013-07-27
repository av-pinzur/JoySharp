using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvP.Joy.Enumerables
{
    public static class EnumeratorExtensions
    {
        public static IEnumerable<TSource> Take<TSource>(this IEnumerator<TSource> source, int count)
        {
            var i = 0;
            while (i++ < count && source.MoveNext()) yield return source.Current;
        }

        public static IEnumerable<TSource> AsEnumerable<TSource>(this IEnumerator<TSource> source)
        {
            try
            {
                while (source.MoveNext()) yield return source.Current;
            }
            finally
            {
                source.Dispose();
            }
        }
    }
}