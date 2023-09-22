namespace AvP.Joy
{
    public class DelegatingComparer<T> : ComparerBase<T>
    {
        private readonly Comparison<T> comparison;

        public DelegatingComparer(Comparison<T> comparison)
        {
            if (comparison == null) throw new ArgumentNullException(nameof(comparison));
            this.comparison = comparison;
        }

        protected override int CompareImpl(T x, T y) { return comparison(x, y); }
    }
}