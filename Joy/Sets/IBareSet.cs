using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvP.Joy.Sets
{
    public interface IBareSet<T>
    {
        bool Contains(T value);
        IBareSet<T> Except(IBareSet<T> other);
        IBareSet<T> Intersect(IBareSet<T> other);
        IBareSet<T> SymmetricExcept(IBareSet<T> other);
        IBareSet<T> Union(IBareSet<T> other);
    }
}
