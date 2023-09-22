namespace AvP.Joy.Models
{
    public class ValueWrapper<T> where T : notnull
    {
        protected T value;

        public ValueWrapper(T value)
        {
            this.value = value;
        }

        public override bool Equals(object? obj) =>
            obj is ValueWrapper<T> other
                && Equals(GetType(), other.GetType())
                && Equals(value, other.value);

        public override int GetHashCode() => value.GetHashCode();

        public override string? ToString() => value.ToString();
    }

    public class StringWrapper : ValueWrapper<string>
    {
        public StringWrapper(string value) : base(value) { }
    }
}
