using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvP.Joy
{
    public static class SetExtensions
    {
        public static bool Excludes<TValue>(this ISet<TValue> set, TValue item)
        {
            return !set.Contains(item);
        }
    }
}