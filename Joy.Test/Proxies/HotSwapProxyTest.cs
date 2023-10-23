using AvP.Joy.Proxies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AvP.Joy.Test.Proxies
{
    [TestClass]
    public class HotSwapProxyTest
    {
        [TestMethod]
        public void Test()
        {
            var subject = new HotSwapProxy<StringFunction>(new Upper());
            var transparentProxy = subject.GetTransparentProxy();
            Assert.AreEqual("FOO", transparentProxy.F("foo"));

            subject.SetTarget(new Doubler());
            Assert.AreEqual("foofoo", transparentProxy.F("foo"));
        }

        public interface StringFunction
        {
            string F(string value);
        }

        public class Upper : StringFunction
        {
            public string F(string value) => value.ToUpperInvariant();
        }

        public class Doubler : StringFunction
        {
            public string F(string value) => value + value;
        }
    }
}
