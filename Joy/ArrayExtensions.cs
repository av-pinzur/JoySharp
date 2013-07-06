using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvP.Joy
{
    public static class ArrayExtensions
    {
        public static TSource[] ShiftedLeft<TSource>(this TSource[] source, int distance)
        {
            if (source == null) throw new ArgumentNullException("source");

            distance = distance % source.Length;
            if (distance < 0) distance += source.Length;

            var result = new TSource[source.Length];
            Array.Copy(source, distance, result, 0, source.Length - distance);
            Array.Copy(source, 0, result, source.Length - distance, distance);
            return result;
        }

        public static TSource[] ShiftedRight<TSource>(this TSource[] source, int distance) { return ShiftedLeft(source, -distance); }

        public static TSource[] ShiftedLeft<TSource>(this TSource[] source, long distance)
        {
            if (source == null) throw new ArgumentNullException("source");

            distance = distance % source.Length;
            if (distance < 0) distance += source.Length;

            var result = new TSource[source.Length];
            Array.Copy(source, distance, result, 0, source.Length - distance);
            Array.Copy(source, 0, result, source.Length - distance, distance);
            return result;
        }

        public static TSource[] ShiftedRight<TSource>(this TSource[] source, long distance) { return ShiftedLeft(source, -distance); }

        public static TSource[] ShiftedLeft<TSource>(this TSource[] source, params TSource[] fill)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (fill == null) throw new ArgumentNullException("fill");
            if (fill.Length > source.Length) throw new ArgumentException("Fill array must be smaller than source array.");

            var distance = fill.Length;

            var result = new TSource[source.Length];
            Array.Copy(source, distance, result, 0, source.Length - distance);
            Array.Copy(fill, 0, result, source.Length - distance, distance);
            return result;
        }

        public static TSource[] ShiftedRight<TSource>(this TSource[] source, params TSource[] fill)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (fill == null) throw new ArgumentNullException("fill");
            if (fill.Length > source.Length) throw new ArgumentException("Fill array must be smaller than source array.");

            var distance = fill.Length;

            var result = new TSource[source.Length];
            Array.Copy(fill, 0, result, 0, source.Length - distance);
            Array.Copy(source, 0, result, source.Length - distance, distance);
            return result;
        }

        public static TSource[] Skip<TSource>(this TSource[] source, int count)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (count > source.Length) return new TSource[0];

            var result = new TSource[source.Length - count];
            Array.Copy(source, count, result, 0, source.Length - count);
            return result;
        }

        public static TSource[] Take<TSource>(this TSource[] source, int count)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (count > source.Length) return (TSource[])source.Clone();

            var result = new TSource[count];
            Array.Copy(source, 0, result, source.Length - count, count);
            return result;
        }

        public static TSource[] Concat<TSource>(this TSource[] first, TSource[] second, params TSource[][] others)
        {
            if (first == null) throw new ArgumentNullException("first");
            if (second == null) throw new ArgumentNullException("second");
            if (others == null) throw new ArgumentNullException("others");
            if (others.Contains(null)) throw new ArgumentException("Parameter element must not be null.", "others");

            return Concat(first.InSingleton().Concat(second.InSingleton()).Concat(others));
        }

        public static TSource[] Concat<TSource>(this IEnumerable<TSource[]> sources)
        {
            if (sources == null) throw new ArgumentNullException("sources");
            sources = sources.ToList();
            if (sources.Contains(null)) throw new ArgumentException("Parameter element must not be null.", "sources");
            
            return Concat(sources.Select(o => Tuple.Create(o, 0, o.Length)));
        }

        public static TSource[] Concat<TSource>(this IEnumerable<Tuple<TSource[], int>> sources)
        {
            if (sources == null) throw new ArgumentNullException("sources");
            sources = sources.ToList();
            if (sources.Contains(null)) throw new ArgumentException("Parameter element must not be null.", "sources");
            if (sources.Any(o => o.Item1 == null)) throw new ArgumentException("Parameter element's Item1 must not be null.", "sources");

            return Concat(sources.Select(o => Tuple.Create(o.Item1, o.Item2, o.Item1.Length - o.Item2)));
        }

        public static TSource[] Concat<TSource>(this IEnumerable<Tuple<TSource[], int, int>> sources)
        {
            if (sources == null) throw new ArgumentNullException("sources");
            sources = sources.ToList();
            if (sources.Contains(null)) throw new ArgumentException("Parameter element must not be null.", "sources");
            if (sources.Any(o => o.Item1 == null)) throw new ArgumentException("Parameter element's Item1 must not be null.", "sources");
            if (sources.Any(o => o.Item2 + o.Item3 > o.Item1.Length || o.Item2 < 0 || o.Item3 < 0)) throw new ArgumentOutOfRangeException();

            var result = new TSource[sources.Sum(o => o.Item3)];
            var resultIndex = 0;
            foreach (var source in sources)
            {
                Array.Copy(source.Item1, source.Item2, result, resultIndex, source.Item3);
                resultIndex += source.Item3;
            }
            return result;
        }
    }
}