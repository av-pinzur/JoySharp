namespace AvP.Joy.Models;

public class ValueWrapper<T> : IEquatable<ValueWrapper<T>> where T : notnull
{
    protected T value;

    public ValueWrapper(T value)
    {
        this.value = value;
    }

    public override string? ToString() => value.ToString();

    public override bool Equals(object? obj) =>
        obj is ValueWrapper<T> other
            && Equals(GetType(), other.GetType())
            && Equals(value, other.value);

    public override int GetHashCode() => GetType().GetHashCode() ^ value.GetHashCode();

    public bool Equals(ValueWrapper<T>? other) => Equals(this, other);

    public static bool operator ==(ValueWrapper<T> a, ValueWrapper<T> b) => Equals(a, b);
    public static bool operator !=(ValueWrapper<T> a, ValueWrapper<T> b) => !Equals(a, b);
}

public class StringWrapper : ValueWrapper<string>
{
    public StringWrapper(string value) : base(value) { }
}
