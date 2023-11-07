using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AvP.Joy.Test;

[TestClass]
public class MaybeTest
{
    [TestMethod]
    public void TestConstructors()
    {
        Maybe<int> i = 512;
        Assert.AreEqual(Maybe<int>.Some(512), i);

        Maybe<IEnumerable<char>> e = "hello!";
        Assert.AreEqual(Maybe<IEnumerable<char>>.Some("hello!"), e);

        // Maybe<IEnumerable<char>> ie = (IEnumerable<char>)"hello!";  // Won't compile! See http://stackoverflow.com/a/1209560/26222
        Maybe<IEnumerable<char>> ie = Maybe.Some((IEnumerable<char>)"hello!");
        Assert.AreEqual(Maybe<IEnumerable<char>>.Some("hello!"), ie);
    }
}
