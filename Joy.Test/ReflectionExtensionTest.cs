using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AvP.Joy.Test
{
    [TestClass]
    public class ReflectionExtensionTest
    {
        [TestMethod]
        public void TestSignatureEquals()
        {
            var baseline = GetType().GetMethod("Baseline");

            Assert.IsTrue(ReflectionExtensions.SignatureEquals(baseline, GetType().GetMethod("Same")));
            Assert.IsTrue(ReflectionExtensions.SignatureEquals(baseline, GetType().GetMethod("DifferentNames")));

            Assert.IsFalse(ReflectionExtensions.SignatureEquals(baseline, GetType().GetMethod("DifferentParamType")));
            Assert.IsFalse(ReflectionExtensions.SignatureEquals(baseline, GetType().GetMethod("DifferentReturnType")));
            Assert.IsFalse(ReflectionExtensions.SignatureEquals(baseline, GetType().GetMethod("VoidReturnType")));
            Assert.IsFalse(ReflectionExtensions.SignatureEquals(baseline, GetType().GetMethod("DifferentParamOrder")));
            Assert.IsFalse(ReflectionExtensions.SignatureEquals(baseline, GetType().GetMethod("RefInsteadOfOut")));
            Assert.IsFalse(ReflectionExtensions.SignatureEquals(baseline, GetType().GetMethod("OutInsteadOfRef")));
            Assert.IsFalse(ReflectionExtensions.SignatureEquals(baseline, GetType().GetMethod("FewerParams")));
            Assert.IsFalse(ReflectionExtensions.SignatureEquals(baseline, GetType().GetMethod("MoreParams")));
        }

        public object Baseline(string a, ref object b, out bool c) { throw new NotImplementedException(); }

        public object Same(string a, ref object b, out bool c) { throw new NotImplementedException(); }
        public object DifferentNames(string x, ref object y, out bool z) { throw new NotImplementedException(); }

        public object DifferentParamType(object a, out bool b, ref object c) { throw new NotImplementedException(); }
        public int DifferentReturnType(string a, out bool b, ref object c) { throw new NotImplementedException(); }
        public void VoidReturnType(string a, out bool b, ref object c) { throw new NotImplementedException(); }
        public object DifferentParamOrder(string a, out bool c, ref object b) { throw new NotImplementedException(); }
        public object RefInsteadOfOut(string a, ref object b, ref bool c) { throw new NotImplementedException(); }
        public object OutInsteadOfRef(string a, out object b, out bool c) { throw new NotImplementedException(); }
        public object FewerParams(string a, ref object b) { throw new NotImplementedException(); }
        public object MoreParams(string a, ref object b, out bool c, int d) { throw new NotImplementedException(); }
    }
}
