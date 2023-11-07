namespace AvP.Joy;

public static class IOExtensions
{
    public static IEnumerable<char> ReadingChars(this TextReader source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        int buffer;
        while ((buffer = source.Read()) > -1)
            yield return (char)buffer;
    }

    public static IEnumerable<string> ReadingLines(this TextReader source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        string? buffer;
        while ((buffer = source.ReadLine()) != null)
            yield return buffer;
    }

    public static void WriteChars(this TextWriter target, IEnumerable<char> values)
    {
        if (target == null) throw new ArgumentNullException(nameof(target));
        if (values == null) throw new ArgumentNullException(nameof(values));
        foreach (var o in values)
            target.Write(o);
    }

    public static void WriteLines(this TextWriter target, IEnumerable<string> values)
    {
        if (target == null) throw new ArgumentNullException(nameof(target));
        if (values == null) throw new ArgumentNullException(nameof(values));
        foreach (var o in values)
            target.WriteLine(o);
    }
}
