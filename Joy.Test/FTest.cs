using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AvP.Joy.Test
{
    [TestClass]
    public class FTest
    {
        [TestMethod]
        public void TestLoop()
        {
            Func<int, int> factorial = n => F.Loop<int, int, int>(n, 1, loop => (cnt, acc) => cnt == 0 ? loop.Complete(acc) : loop.Recur(cnt - 1, acc * cnt));
            Assert.AreEqual(120, factorial(5));
        }

        [TestMethod]
        public void TestY()
        {
            Func<int, int> factorial = F.Y<int, int>(self => n => n == 0 ? 1 : n * self(n - 1));
            Assert.AreEqual(120, factorial(5));
        }
    }
}