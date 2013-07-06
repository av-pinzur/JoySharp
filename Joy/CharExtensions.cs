using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvP.Joy
{
    public static class CharExtensions
    {
        public static bool IsDigit(this char value)
        {
            return char.IsDigit(value);
        }
    }
}
