using AvP.Joy.Caches;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AvP.Joy.Test.Caches;

[TestClass]
public class TtlReadCacheTest
{
    [TestMethod]
    public void PrefetchRemembersResultUntilTtlExpires()
    {
        var subject = MemoryReadCache<Voidlike, int>.WithTtlExpiry(TimeSpan.FromMilliseconds(100));
        var callCount = 0;
        var cached = subject.Prefetch(() => ++callCount);

        Assert.AreEqual(1, callCount);

        Assert.AreEqual(1, cached());
        Assert.AreEqual(1, cached());

        Thread.Sleep(150);
        Assert.AreEqual(1, callCount);

        Assert.AreEqual(2, cached());
    }

    [TestMethod]
    public void MaxAgeOfZeroDisablesCaching()
    {
        var subject = MemoryReadCache<byte, int>.WithTtlExpiry(maxAge: TimeSpan.Zero);
        var callCount = 0;
        var cached = subject.Memoize(_ => ++callCount);

        Assert.AreEqual(1, cached(0));
        Assert.AreEqual(2, cached(0));
        Assert.AreEqual(3, cached(0));
    }
}
