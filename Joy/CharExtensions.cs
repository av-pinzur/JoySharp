namespace AvP.Joy;

public static class CharExtensions
{
    public static bool IsDigit(this char value)
    {
        return char.IsDigit(value);
    }

    public static int ToDigit(this char value)
    {
        if (!value.IsDigit()) throw new ArgumentOutOfRangeException(nameof(value), value, "Parameter must be a decimal digit.");
        return (int)value - (int)'0';
    }
}
