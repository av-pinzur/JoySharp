using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvP.Joy.Adts
{
    public class Wrapper<T>
    {
        public Wrapper(T value)
        {
            Value = value;
        }

        public T Value { private set; get; }
    }
}
