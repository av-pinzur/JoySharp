using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvP.Joy.Sets
{
    public class DelegatingBareSet<T> : AbstractBareSet<T>
    {
        private Predicate<T> contains;

        public DelegatingBareSet(Predicate<T> contains)
        {
            this.contains = contains;
        }

        public override bool Contains(T value)
        {
            return contains(value);
        }
    }
}
