﻿using AvP.Joy.Enumerables;
using System.Diagnostics.CodeAnalysis;

namespace AvP.Joy;

public static class RandomExtensions
{
    public static TResult NextFrom<TResult>(this Random source, IEnumerable<TResult> options)
    {
        if (null == source) throw new ArgumentNullException(nameof(source));
        if (null == options) throw new ArgumentNullException(nameof(options));

        options = options.ToList();
        if (options.None()) throw new ArgumentException("Sequence is empty.", nameof(options));

        return options.Nth(source.Next(options.Count()));
    }

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static TSource? NextFromOrDefault<TSource>(this Random source, IEnumerable<TSource> options, TSource? defaultValue = default(TSource))
    {
        if (null == source) throw new ArgumentNullException(nameof(source));
        if (null == options) throw new ArgumentNullException(nameof(options));

        options = options.ToList();
        if (options.None()) return defaultValue;

        return options.Nth(source.Next(options.Count()));
    }
}
