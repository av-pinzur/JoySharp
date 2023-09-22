using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvP.Joy.Sets
{
    interface IRelatableBareSet<T, TOtherSet> : IBareSet<T>
        where TOtherSet : IBareSet<T>
    {
        bool SetEquals(TOtherSet other);
        bool Overlaps(TOtherSet other);
        bool IsSubsetOf(TOtherSet other);
        bool IsProperSubsetOf(TOtherSet other);
        bool IsSupersetOf(TOtherSet other);
        bool IsProperSupersetOf(TOtherSet other);
    }
}
