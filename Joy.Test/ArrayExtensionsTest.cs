using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AvP.Joy.Enumerables;

namespace AvP.Joy.Test
{
    [TestClass]
    public class ArrayExtensionsTest
    {
        private static int[] subject = Enumerable.Range(0, 10).ToArray();

        [TestMethod]
        public void TestShiftedLeft()
        {
            Assert.AreEqual("5,6,7,8,9,0,1,2,3,4", subject.ShiftedLeft(5).ToStrings().Join(","));
            Assert.AreEqual("3,4,5,6,7,8,9,0,1,2", subject.ShiftedLeft(3).ToStrings().Join(","));
            Assert.AreEqual("7,8,9,0,1,2,3,4,5,6", subject.ShiftedLeft(7).ToStrings().Join(","));

            Assert.AreEqual("0,1,2,3,4,5,6,7,8,9", subject.ShiftedLeft(0).ToStrings().Join(","));
            Assert.AreEqual("3,4,5,6,7,8,9,0,1,2", subject.ShiftedLeft(-7).ToStrings().Join(","));
            Assert.AreEqual("5,6,7,8,9,0,1,2,3,4", subject.ShiftedLeft(105).ToStrings().Join(","));
        }

        [TestMethod]
        public void TestShiftedRight()
        {
            Assert.AreEqual("5,6,7,8,9,0,1,2,3,4", subject.ShiftedRight(5).ToStrings().Join(","));
            Assert.AreEqual("7,8,9,0,1,2,3,4,5,6", subject.ShiftedRight(3).ToStrings().Join(","));
            Assert.AreEqual("3,4,5,6,7,8,9,0,1,2", subject.ShiftedRight(7).ToStrings().Join(","));

            Assert.AreEqual("0,1,2,3,4,5,6,7,8,9", subject.ShiftedRight(0).ToStrings().Join(","));
            Assert.AreEqual("7,8,9,0,1,2,3,4,5,6", subject.ShiftedRight(-7).ToStrings().Join(","));
            Assert.AreEqual("5,6,7,8,9,0,1,2,3,4", subject.ShiftedRight(105).ToStrings().Join(","));
        }
    }
}
