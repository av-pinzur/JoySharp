using System;
using System.Collections.Generic;
using System.Linq;
using AvP.Joy.Enumerables;

namespace AvP.Joy
{
    public static class RandomExtensions
    {
        public static TResult NextFrom<TResult>(this Random source, IEnumerable<TResult> options)
        {
            if (null == source) throw new ArgumentNullException(nameof(source));
            if (null == options) throw new ArgumentNullException(nameof(options));

            options = options.ToList();
            if (options.None()) throw new ArgumentException("Sequence must not be empty.", nameof(options));

            return options.Nth(source.Next(options.Count()));
        }
    }
}