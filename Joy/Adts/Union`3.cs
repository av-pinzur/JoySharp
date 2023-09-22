using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvP.Joy.Adts
{
    public interface Union<T1, T2, T3>
    {
        TResult Match<TResult>(Func<T1, TResult> function1, Func<T2, TResult> function2, Func<T3, TResult> function3);
    }
}
