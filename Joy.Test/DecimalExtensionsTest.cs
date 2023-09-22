using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AvP.Joy.Test
{
    [TestClass]
    public class DecimalExtensionsTest : HelpedTest
    {
        [TestMethod]
        public void IntegralDivideTest()
        {
            Assert.AreEqual(new IntegralDivisionResult<decimal>(.0M, 1234.4321M), 1234.4321M.IntegralDivide(10000M));
            Assert.AreEqual(new IntegralDivisionResult<decimal>(12.0M, 34.4321M), 1234.4321M.IntegralDivide(100M));
            Assert.AreEqual(new IntegralDivisionResult<decimal>(1234.0M, .4321M), 1234.4321M.IntegralDivide(1M));
            Assert.AreEqual(new IntegralDivisionResult<decimal>(123443M, .0021M), 1234.4321M.IntegralDivide(.01M));
            Assert.AreEqual(new IntegralDivisionResult<decimal>(12344321.0M, 0M), 1234.4321M.IntegralDivide(.0001M));

            Assert.AreEqual(new IntegralDivisionResult<decimal>(-.0M, -1234.4321M), (-1234.4321M).IntegralDivide(10000M));
            Assert.AreEqual(new IntegralDivisionResult<decimal>(-12.0M, -34.4321M), (-1234.4321M).IntegralDivide(100M));
            Assert.AreEqual(new IntegralDivisionResult<decimal>(-1234.0M, -.4321M), (-1234.4321M).IntegralDivide(1M));
            Assert.AreEqual(new IntegralDivisionResult<decimal>(-123443M, -.0021M), (-1234.4321M).IntegralDivide(.01M));
            Assert.AreEqual(new IntegralDivisionResult<decimal>(-12344321.0M, -0M), (-1234.4321M).IntegralDivide(.0001M));
        }

        [TestMethod]
        public void DigitsTest()
        {
            Assert.AreEqual("4,3,2,1", string.Join(",", DecimalExtensions.IntegralPartDigits(1234.0M, 10)));
            Assert.AreEqual("4,3,2,1", string.Join(",", DecimalExtensions.FractionalPartDigits(0.4321M, 10)));

            Assert.AreEqual("4,3,2,1", string.Join(",", DecimalExtensions.Digits(1234.5678M).Item1));
            Assert.AreEqual("5,6,7,8", string.Join(",", DecimalExtensions.Digits(1234.5678M).Item2));

            Assert.AreEqual("4,3,2,1", string.Join(",", DecimalExtensions.Digits(-1234.5678M).Item1));
            Assert.AreEqual("5,6,7,8", string.Join(",", DecimalExtensions.Digits(-1234.5678M).Item2));
        }
    }
}
