﻿using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace AvP.Joy.Models;

public delegate bool Parser<T>(string value, [MaybeNullWhen(false)] out T success, [MaybeNullWhen(true)] out Exception failure);

public static class Parser
{
    public static Parser<T> Of<T>(Func<string, T> constructor, Regex validator)
    {
        return (string value, [MaybeNullWhen(false)] out T success, [MaybeNullWhen(true)] out Exception failure) =>
        {
            if (validator.IsMatch(value))
            {
                success = constructor(value);
                failure = null;
                return true;
            }
            else
            {
                success = default;
                failure = new ArgumentException($"value must be a valid {typeof(T).Name}.");
                return false;
            }
        };
    }

    public static T Parse<T>(this Parser<T> parser, string value) =>
        parser(value, out T? success, out Exception? failure) ? success : throw failure;

    public static bool TryParse<T>(this Parser<T> parser, string value, [MaybeNullWhen(false)] out T success) =>
        parser(value, out success, out _);
}
