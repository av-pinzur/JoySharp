using System;
using AvP.Joy.Proxies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace AvP.Joy.Test
{
    [TestClass]
    public class ProxiesTest : HelpedTest
    {
        private interface IWeirdArgs
        {
            int Foo(int a, ref int b, out int c);
        }

        private delegate int WeirdArgs(int a, ref int b, out int c);

        [TestMethod]
        public void TestProxyArgumentPassing()
        {
            var subject = LocalProxy<IWeirdArgs>.DelegatingSingleMethodTo(
                (WeirdArgs) delegate(int a, ref int b, out int c)
                {
                    Assert.AreEqual(1, a);
                    Assert.AreEqual(2, b);
                    b = b + 1;
                    c = -1;
                    return int.MinValue;
                });

            int refParam = 2;
            int outParam;
            var result = subject.GetTransparentProxy().Foo(1, ref refParam, out outParam);
            Assert.AreEqual(3, refParam);
            Assert.AreEqual(-1, outParam);
            Assert.AreEqual(int.MinValue, result);
        }

        private interface IA { char GetA(); }
        private interface IB { char GetB(); }
        private interface IBSub : IB { new char GetB(); }
        private class A : IA { public char GetA() { return 'A'; } }
        private class B : IB { public char GetB() { return 'B'; } }
        private class BSub : B, IBSub { char IBSub.GetB() { return 'b'; } }
        private class AB : IA, IB { public char GetA() { return 'A'; } public char GetB() { return 'B'; } }
        private class MockA { public char GetA() { return 'A'; } }

        private class PassThroughProxy : LocalProxy
        {
            private readonly object target;

            public PassThroughProxy(object target, params Type[] supportedInterfaces)
                : base(supportedInterfaces)
            {
                this.target = target;
            }

            protected override object Invoke(System.Reflection.MethodInfo method, object[] parameters)
            {
                return method.Invoke(target, parameters);
            }
        }

        [TestMethod]
        public void TestProxyCastLimits()
        {
            object transparentProxy;
            IA a;
            IB b;

            a = (IA) new PassThroughProxy(new A(), typeof(IA)).GetTransparentProxy();

            b = (IB) new PassThroughProxy(new A(), typeof(IB)).GetTransparentProxy();
            AssertThrows(typeof(InvalidCastException), () => a = (IA)b);
            AssertThrows(typeof(TargetException), () => b.GetB());

            transparentProxy = new PassThroughProxy(new BSub(), typeof(IBSub)).GetTransparentProxy();
            Assert.AreEqual('b', ((IBSub)transparentProxy).GetB());
            Assert.AreEqual('B', ((IB)transparentProxy).GetB());

            transparentProxy = new PassThroughProxy(new AB(), typeof(IA), typeof(IB)).GetTransparentProxy();
            Assert.AreEqual('A', ((IA)transparentProxy).GetA());
            Assert.AreEqual('B', ((IB)transparentProxy).GetB());

            transparentProxy = new PassThroughProxy(new MockA(), typeof(IA));
            AssertThrows(typeof(InvalidCastException), () => ((IA)transparentProxy).GetA());
        }

        [TestMethod]
        public void TestProxyDisposable()
        {
            int x;
            using (LocalProxy<IDisposable>
                    .DelegatingSingleMethodTo((Action) delegate() { x = int.MaxValue; })
                    .GetTransparentProxy() )
                x = 0;
            Assert.AreEqual(int.MaxValue, x);
        }
    }
}