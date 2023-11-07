using AvP.Joy.Caches;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AvP.Joy.Test.Caches;

[TestClass]
public class FifoReadCacheTest
{
    [TestMethod]
    public void RemembersValuePerKeyUntilOverMaxCount()
    {
        const string ignored = "ignoredValue";
        var subject = MemoryReadCache<byte, string>.WithFifoEviction(maxCount: 3);

        Assert.AreEqual("returnValue1", subject.GetOrAdd(1, () => "returnValue1"));
        Assert.AreEqual("returnValue2", subject.GetOrAdd(2, () => "returnValue2"));
        Assert.AreEqual("returnValue3", subject.GetOrAdd(3, () => "returnValue3"));

        // It should remember all three.
        Assert.AreEqual("returnValue1", subject.GetOrAdd(1, () => ignored));
        Assert.AreEqual("returnValue2", subject.GetOrAdd(2, () => ignored));
        Assert.AreEqual("returnValue3", subject.GetOrAdd(3, () => ignored));

        // Bump out #1.
        Assert.AreEqual("returnValue4", subject.GetOrAdd(4, () => "returnValue4"));

        // Now it should remember the last three.
        Assert.AreEqual("returnValue2", subject.GetOrAdd(2, () => ignored));
        Assert.AreEqual("returnValue3", subject.GetOrAdd(3, () => ignored));
        Assert.AreEqual("returnValue4", subject.GetOrAdd(4, () => ignored));

        // Bump out #2.
        Assert.AreEqual("returnValue1b", subject.GetOrAdd(1, () => "returnValue1b"));

        // It should still remember the last three..
        Assert.AreEqual("returnValue3", subject.GetOrAdd(3, () => ignored));
        Assert.AreEqual("returnValue4", subject.GetOrAdd(4, () => ignored));
        Assert.AreEqual("returnValue1b", subject.GetOrAdd(1, () => ignored));
    }

    [TestMethod]
    public void MaxCountOfZeroDisablesCaching()
    {
        var subject = MemoryReadCache<byte, int>.WithFifoEviction(maxCount: 0);
        var callCount = 0;
        var cached = subject.Memoize(_ => ++callCount);

        Assert.AreEqual(1, cached(0));
        Assert.AreEqual(2, cached(0));
        Assert.AreEqual(3, cached(0));
    }
}
