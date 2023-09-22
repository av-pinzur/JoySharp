namespace AvP.Joy.Models
{
    public class ValueWrapper<T> where T : notnull
    {
        protected T value;

        public ValueWrapper(T value)
        {
            if (value == null) throw new ArgumentNullException("value");
            this.value = value;
        }

        public override bool Equals(object? obj) =>
            obj is ValueWrapper<T> other
                && object.Equals(other.GetType(), this.GetType())
                && object.Equals(other.value, this.value);

        public override int GetHashCode() => value.GetHashCode();

        public override string? ToString() => value.ToString();
    }

    public class StringWrapper : ValueWrapper<string>
    {
        public StringWrapper(string value) : base(value) { }
    }
}
