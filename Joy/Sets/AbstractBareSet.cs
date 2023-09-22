using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvP.Joy.Sets
{
    public abstract class AbstractBareSet<T> : IBareSet<T>
    {
        private readonly Predicate<T> contains;

        public AbstractBareSet()
        {
            this.contains = this.Contains;
        }

        public abstract bool Contains(T value);

        public IBareSet<T> Except(IBareSet<T> other)
        {
            return new DelegatingBareSet<T>(contains.Not());
        }

        public IBareSet<T> Intersect(IBareSet<T> other)
        {
            return new DelegatingBareSet<T>(contains.And(other.Contains));
        }

        public IBareSet<T> SymmetricExcept(IBareSet<T> other)
        {
            return new DelegatingBareSet<T>(contains.Xor(other.Contains));
        }

        public IBareSet<T> Union(IBareSet<T> other)
        {
            return new DelegatingBareSet<T>(contains.Or(other.Contains));
        }
    }
}
