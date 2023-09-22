using AvP.Joy.Sequences;

namespace AvP.Joy
{
    public static class StringExtensions
    {
        public static string Join(this IEnumerable<string> source, string separator)
        {
            return string.Join(separator, source);
        }

        public static string Join(this ISequence<string> source, string separator)
        {
            return source.AsEnumerable().Join(separator);
        }

        public static string[] Lines(this string value)
        {
            return value.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        }

        public static bool IsEmpty(this string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return value.Length == 0;
        }

        public static string OrEmpty(this string? value)
        {
            return value ?? string.Empty;
        }

        public static string? TrimToNull(this string? value)
        {
            if (value == null) return value;
            var trimmed = value.Trim();
            return trimmed.IsEmpty() ? null : trimmed;
        }

        public static string TrimOrEmpty(this string? value)
        {
            return value.OrEmpty().Trim();
        }
    }
}