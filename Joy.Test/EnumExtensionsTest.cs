using System;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AvP.Joy.Test
{
    [TestClass]
    public class EnumExtensionsTest
    {
        private enum MyEnum { [Description("Value A")] A = 1, B = 2 }

        [TestMethod]
        public void TestIsDefined()
        {
            MyEnum defined = (MyEnum)2;
            MyEnum undefined = (MyEnum)3;
            Assert.IsTrue(defined.IsDefined());
            Assert.IsFalse(undefined.IsDefined());
        }

        [TestMethod]
        public void TestGetDisplayName()
        {
            Assert.AreEqual("Value A", MyEnum.A.GetDescription());
            Assert.AreEqual("B", MyEnum.B.GetDescription());
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetDisplayNameUndefined()
        {
            var dummy = ((MyEnum)3).GetDescription();
        }
    }
}