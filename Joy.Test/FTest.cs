using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AvP.Joy.Test;

[TestClass]
public class FTest
{
    [TestMethod]
    public void TestLoop()
    {
        Func<int, int> factorial = n => F.Loop<int, int, int>(n, 1, loop => (cnt, acc) => cnt == 0 ? loop.Complete(acc) : loop.Recur(cnt - 1, acc * cnt));
        Assert.AreEqual(120, factorial(5));

        Assert.AreEqual(120, F<int>.Loop(5, 1, loop => (cnt, acc) => cnt == 0 ? loop.Complete(acc) : loop.Recur(cnt - 1, acc * cnt)));
    }

    [TestMethod]
    public void TestY()
    {
        Func<int, int> factorial = F.Y<int, int>(self => n => n == 0 ? 1 : n * self(n - 1));
        Assert.AreEqual(120, factorial(5));

        Assert.AreEqual(120, F<int>.YEval(5, self => n => n == 0 ? 1 : n * self(n - 1)));
    }

    [TestMethod]
    public void Implement_HotSwapTest()
    {
        Func<string, string> upper = s => s.ToUpperInvariant();
        Func<string, string> doubler = s => s + s;

        var target = upper;
        var subject = F.Facade(() => target);
        Assert.AreEqual("FOO", subject("foo"));

        target = doubler;
        Assert.AreEqual("foofoo", subject("foo"));
    }

    [TestMethod]
    public void Implement_WithDelegateTest()
    {
        var subject = F.Implement<StringFunction>((string s) => s.ToUpperInvariant());
        Assert.AreEqual("FOO", subject.F("foo"));
    }

    [TestMethod]
    public void DecorateTest()
    {
        var logger = new Logger();
        var target = new StringConcatator();
        var subject = F.Decorate<StringBiFunction>(target, logger.Decorate);

        var actual = subject.F("foo", "bar");

        Assert.AreEqual("foobar", actual);
        Assert.AreEqual(1, logger.Log.Count);
        Assert.AreEqual("Invocation { Method = StringBiFunction.F(String, String), Arguments = [foo, bar] } => foobar", logger.Log.First());
    }

    private class Logger
    {
        public readonly List<string> Log = new();

        public void Write(object obj)
        {
            Log.Add(obj.ToString()!);
        }

        public Func<T, TResult> Decorate<T, TResult>(Func<T, TResult> fn)
            => arg =>
                {
                    var result = fn(arg);
                    Write($"{arg} => {result}");
                    return result;
                };
    }

    public interface StringFunction
    {
        string F(string value);
    }

    public interface StringBiFunction
    {
        string F(string value1, string value2);
    }

    public class StringConcatator : StringBiFunction
    {
        public string F(string value1, string value2) =>
            value1 + value2;
    }
}
