using AvP.Joy.Proxies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AvP.Joy.Test.Proxies
{
    [TestClass]
    public class LocalProxyTest
    {
        [TestMethod]
        public void DelegatingSingleMethodToTest()
        {
            var subject = LocalProxy<StringFunction>.DelegatingSingleMethodTo((string s) => s.ToUpperInvariant());
            var transparentProxy = subject.GetTransparentProxy();
            Assert.AreEqual("FOO", transparentProxy.F("foo"));
        }

        public interface StringFunction
        {
            string F(string value);
        }
    }
}
