using AvP.Joy.Sequences;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Enumerable = System.Linq.Enumerable;

namespace AvP.Joy.Test;

[TestClass]
public class SequenceTest
{
    [TestMethod]
    public void TestSelectMany()
    {
        const int sourceLength = 1000;
        var subject = Sequence.Range(0, sourceLength);

        var expecteds = Enumerable.Range(0, sourceLength ^ 2);
        var actuals = subject.SelectMany(i => Sequence.Range(0, sourceLength).Select(j => i * sourceLength + j));

        foreach (var pair in Enumerable.Zip(expecteds, actuals.AsEnumerable(), (expected, actual) => new { expected, actual }))
            Assert.AreEqual(pair.expected, pair.actual);
    }

    [TestMethod]
    public void TestConcatDeep()
    {
        var subject = Sequence.Empty<int>();
        for (var i = 0; i < 10000; i++)
        {
            subject = subject.Concat(i.InSingletonSeq());
        }
        for (var cur = subject; cur.Any; cur = cur.GetTail())
        {
            var dummy = cur.Head;
        }
    }

    [TestMethod]
    public void TestSlide_Sequence()
    {
        Assert.AreEqual("0,1,2,3,4,5,6|1,2,3,4,5,6,7|2,3,4,5,6,7,8|3,4,5,6,7,8,9|4,5,6,7,8,9|5,6,7,8,9|6,7,8,9|7,8,9|8,9|9",
            Sequence.Range(0, 10).Slide(7).Select(o => o.ToStrings().Join(",")).Join("|"));

        Assert.AreEqual("0,1,2|1,2|2",
            Sequence.Range(0, 3).Slide(5).Select(o => o.ToStrings().Join(",")).Join("|"));

        Assert.AreEqual("0,1,2|1,2|2",
            Sequence.Range(0, 3).Slide(3).Select(o => o.ToStrings().Join(",")).Join("|"));

        Assert.AreEqual(0, Sequence.Range(0, 1).Slide(7).Single().Single());
        Assert.IsTrue(Sequence.Empty<object>().Slide(7).None());
    }

    [TestMethod]
    public void TestSlideSpeed_Sequence()
    {
        for (var frame = Sequence.Range(0, 1000000).Slide(25); frame.Any; frame = frame.GetTail())
        {
            var dummy = frame;
        }
    }

    [TestMethod]
    public void TestSlideUnbounded()
    {
        Assert.AreEqual("10,9,8,7,6,5,4,3,2,1",
            Sequence.Range(0, 10).Slide().Select(o => o.Count()).ToStrings().Join(","));
    }

    [TestMethod]
    public void TestZipAll_Sequence()
    {
        Assert.AreEqual("(0, 0)|(1, 1)|(2, _)|(3, _)",
            Sequence.Range(0, 4).ZipAll(Sequence.Range(0, 2), (x, y) => Tuple.Create(x.ToStringOrDefault("_"), y.ToStringOrDefault("_"))).ToStrings().Join("|"));
        Assert.AreEqual("(0, 0, 0)|(1, 1, 1)|(2, _, 2)|(3, _, _)",
            Sequence.Over(Sequence.Range(0, 4), Sequence.Range(0, 2), Sequence.Range(0, 3)).ZipAll(e => '(' + e.Select(o => o.ToStringOrDefault("_")).Join(", ") + ')').Join("|"));
    }
}
