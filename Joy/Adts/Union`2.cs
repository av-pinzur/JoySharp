using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvP.Joy.Adts
{
    public interface Union<T1, T2>
    {
        TResult Match<TResult>(Func<T1, TResult> function1, Func<T2, TResult> function2);

        Union<T1, T2> UnionWith(Union<T1, T2> other);
        Union<T1, T2> UnionWith(Union<T2, T1> other);
        Union<T1, T2, T3> UnionWith<T3>(Union<T1, T3> other);
        Union<T1, T2, T3> UnionWith<T3>(Union<T3, T1> other);
        Union<T1, T2, T3> UnionWith<T3>(Union<T2, T3> other);
        Union<T1, T2, T3> UnionWith<T3>(Union<T3, T2> other);
        Union<T1, T2, T3, T4> UnionWith<T3, T4>(Union<T3, T4> other);
    }
}
