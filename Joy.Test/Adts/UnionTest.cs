using System;
using AvP.Joy.Adts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AvP.Joy.Test
{
    [TestClass]
    public class UnionTest
    {
        [TestMethod]
        public void Union2With2()
        {
            Union<string, char> stringOrChar = Union.of<string, char>("foo");
            Union<char, int> charOrInt = Union.of<char, int>('z');
            Union<int, char> intOrChar = Union.of<int, char>(15);
            Union<int, long> intOrLong = Union.of<int, long>(73L);

            Union<string, char, int, long> overlapNone = stringOrChar.UnionWith(intOrLong);

            Union<string, char, int> overlap2With1 = stringOrChar.UnionWith(charOrInt);
            Union<string, char, int> overlap2With2 = stringOrChar.UnionWith(intOrChar);
            Union<char, int, string> overlap1With2 = charOrInt.UnionWith(stringOrChar);
            Union<int, char, long> overlap1With1 = intOrChar.UnionWith(intOrLong);

            Union<char, int> overlapBothSame = charOrInt.UnionWith(charOrInt);
            Union<char, int> overlapBothOpposite = charOrInt.UnionWith(intOrChar);
        }
    }
}