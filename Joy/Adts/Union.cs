using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvP.Joy.Adts
{
    public class Union
    {
        public static Union<T1, T2> of<T1, T2>(T1 value) => new Case1<T1, T2>(value);
        public static Union<T1, T2> of<T1, T2>(T2 value) => new Case2<T1, T2>(value);
        public static Union<T1, T2, T3> of<T1, T2, T3>(T1 value) => new Case1<T1, T2, T3>(value);
        public static Union<T1, T2, T3> of<T1, T2, T3>(T2 value) => new Case2<T1, T2, T3>(value);
        public static Union<T1, T2, T3> of<T1, T2, T3>(T3 value) => new Case3<T1, T2, T3>(value);
        public static Union<T1, T2, T3, T4> of<T1, T2, T3, T4>(T1 value) => new Case1<T1, T2, T3, T4>(value);
        public static Union<T1, T2, T3, T4> of<T1, T2, T3, T4>(T2 value) => new Case2<T1, T2, T3, T4>(value);
        public static Union<T1, T2, T3, T4> of<T1, T2, T3, T4>(T3 value) => new Case3<T1, T2, T3, T4>(value);
        public static Union<T1, T2, T3, T4> of<T1, T2, T3, T4>(T4 value) => new Case4<T1, T2, T3, T4>(value);
    }
}
