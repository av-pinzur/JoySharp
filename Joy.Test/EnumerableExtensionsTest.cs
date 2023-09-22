using AvP.Joy.Enumerables;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace AvP.Joy.Test
{
    [TestClass]
    public class EnumerableExtensionsTest : HelpedTest
    {
        [TestMethod]
        public void TestGenerate()
        {
            Assert.AreEqual("5,10,15,20",
                EnumerableExtensions.Generate(5, 5).Take(4).ToStrings().Join(","));
            Assert.AreEqual("5,0,-5,-10",
                EnumerableExtensions.Generate(5, -5).Take(4).ToStrings().Join(","));
        }

        [TestMethod]
        public void TestSlide_Enumerable()
        {
            Assert.AreEqual("0,1,2,3,4,5,6|1,2,3,4,5,6,7|2,3,4,5,6,7,8|3,4,5,6,7,8,9|4,5,6,7,8,9|5,6,7,8,9|6,7,8,9|7,8,9|8,9|9",
                Enumerable.Range(0, 10).Slide(7).Select(o => o.ToStrings().Join(",")).Join("|"));

            Assert.AreEqual("0,1,2|1,2|2",
                Enumerable.Range(0, 3).Slide(5).Select(o => o.ToStrings().Join(",")).Join("|"));

            Assert.AreEqual("0,1,2|1,2|2",
                Enumerable.Range(0, 3).Slide(3).Select(o => o.ToStrings().Join(",")).Join("|"));

            Assert.AreEqual(0, Enumerable.Range(0, 1).Slide(7).Single().Single());
            Assert.IsTrue(Enumerable.Empty<object>().Slide(7).None());
        }

        [TestMethod]
        public void TestSlideSpeed_Enumerable()
        {
            foreach (var frame in Enumerable.Range(0, 1000000).Slide(25))
            {
                var dummy = frame;
            }
        }

        [TestMethod]
        public void TestZip_Enumerable()
        {
            Assert.AreEqual("(0, 0, 0)|(1, 1, 1)",
                (new[] { Enumerable.Range(0, 4), Enumerable.Range(0, 2), Enumerable.Range(0, 3) }).Zip(e => '(' + e.Select(o => o.ToString()).Join(", ") + ')').Join("|"));

            Assert.AreEqual("",
                (new[] { Enumerable.Range(0, 4), Enumerable.Range(0, 0), Enumerable.Range(0, 3) }).Zip(e => '(' + e.Select(o => o.ToString()).Join(", ") + ')').Join("|"));

            Assert.AreEqual("",
                Enumerable.Empty<IEnumerable<int>>().Zip(e => '(' + e.Select(o => o.ToString()).Join(", ") + ')').Join("|"));

            Assert.AreEqual("(0, 0, 0)|(1, 1, 1)|(2, 2, 2)",
                (new[] { Enumerable.Range(0, 3), Enumerable.Range(0, 3), Enumerable.Range(0, 3) }).Zip(e => '(' + e.Select(o => o.ToString()).Join(", ") + ')').Join("|"));
        }

        [TestMethod]
        public void TestZip_Enumerable_Deferred()
        {
            var actual = new[] { new[] { 0, 1 }, new[] { 0, 1 } }.Zip(o => o).Zip(o => o);
            for (int i = 0; i < 2; i++)
                Assert.AreEqual("(0, 1)|(0, 1)", actual.Select(o => '(' + o.ToStrings().Join(", ") + ')').Join("|"));
        }

        [TestMethod]
        public void TestZipAll_Enumerable()
        {
            Assert.AreEqual("(0, 0)|(1, 1)|(2, _)|(3, _)",
                Enumerable.Range(0, 4).ZipAll(Enumerable.Range(0, 2), (x, y) => Tuple.Create(x.HasValue ? x.Value.ToString() : "_", y.HasValue ? y.Value.ToString() : "_")).ToStrings().Join("|"));
            Assert.AreEqual("(0, 0, 0)|(1, 1, 1)|(2, _, 2)|(3, _, _)",
                (new[] { Enumerable.Range(0, 4), Enumerable.Range(0, 2), Enumerable.Range(0, 3) }).ZipAll(e => '(' + e.Select(o => o.HasValue ? o.Value.ToString() : "_").Join(", ") + ')').Join("|"));
        }

        [TestMethod]
        public void TestZipAll_Enumerable_Deferred()
        {
            var actual = new[] { new[] { 0, 1 }, new[] { 0, 1 } }.ZipAll(o => o).ZipAll(o => o);
            for (int i = 0; i < 2; i++)
                Assert.AreEqual("(0, 1)|(0, 1)", actual.Select(o => '(' + o.ToStrings().Join(", ") + ')').Join("|"));
        }

        [TestMethod]
        public void TestSelectDisposables()
        {
            var disposableStati = new[] { new[] { false }, new[] { false }, new[] { false } };
            IReadOnlyList<IDisposable> results;
            using (disposableStati.SelectDisposables(
                o => new DelegatingDisposable(() => { o[0] = true; }),
                out results))
            {
                foreach (var o in results.Select((_, i) => disposableStati[i]))
                    Assert.IsFalse(o[0]);
            }
            foreach (var o in disposableStati)
                Assert.IsTrue(o[0]);
        }

        [TestMethod]
        public void TestSelectDisposables_WorstCase()
        {
            var disposableStati = new[] { new[] { false }, new[] { false }, new[] { false } };
            bool caughtExpected = false;
            try
            {
                IReadOnlyList<IDisposable> results;
                using (disposableStati.SelectDisposables(
                    (o, i) =>
                    {
                        if (i == 2) throw new BadImageFormatException();
                        return new DelegatingDisposable(() => { o[0] = true; });
                    },
                    out results))
                {
                    foreach (var o in results.Select((_, i) => disposableStati[i]))
                        Assert.IsFalse(o[0]);
                }
            }
            catch (BadImageFormatException)
            {
                caughtExpected = true;
                for (var i = 0; i < disposableStati.Length; i++)
                {
                    var o = disposableStati[i];
                    if (i < 2) Assert.IsTrue(o[0]);
                    else Assert.IsFalse(o[0]);
                }
            }
            Assert.IsTrue(caughtExpected);
        }

        [TestMethod]
        public void TestSelectDisposables_Speed()
        {
            const int cycleCount = 1000;
            const int workThinkMs = 1;
            const int disposalThinkMs = 0;

            var cycles = Enumerable.Range(0, cycleCount).ToList();

            var baselineTime = MinTime(2, () =>
                    BuildUseDispose_Inconvenient(cycles, disposalThinkMs, workThinkMs));

            var actualTime = MinTime(2, () =>
                    BuildUseDispose_Convenient(cycles, disposalThinkMs, workThinkMs));

            var thinkTime = TimeSpan.FromMilliseconds(cycleCount * (workThinkMs + disposalThinkMs));
            var baseline = (baselineTime - thinkTime).TotalMilliseconds;
            var actual = (actualTime - thinkTime).TotalMilliseconds;

            Assert.AreEqual(1D, 1D.MaxVs(actual / baseline), 0.001D,
                $"Performance penalty too high. Actual Time: {actualTime}; Baseline Time: {baselineTime}; Think Time: {thinkTime}.");
        }

        private static void BuildUseDispose_Convenient(List<int> cycles, int disposalThinkMs, int workThinkMs)
        {
            IReadOnlyList<IDisposable> results;
            using (cycles.SelectDisposables(
                _ => new TestDisposable(disposalThinkMs),
                out results))
            {
                foreach (var _ in results)
                    Thread.Sleep(workThinkMs);
            }
        }

        private static void BuildUseDispose_Inconvenient(List<int> cycles, int disposalThinkMs, int workThinkMs)
        {
            var results = new List<IDisposable>();
            try
            {
                foreach (var _ in cycles)
                    results.Add(new TestDisposable(disposalThinkMs));

                foreach (var _ in results)
                    Thread.Sleep(workThinkMs);
            }
            finally
            {
                foreach (var o in results) o?.Dispose();
            }
        }

        private sealed class TestDisposable : IDisposable
        {
            private readonly int thinkMs;

            public TestDisposable(int thinkMs)
            {
                this.thinkMs = thinkMs;
            }

            public void Dispose()
            {
                if (thinkMs > 0) Thread.Sleep(thinkMs);
            }
        }

        private static TimeSpan MinTime(int iterations, Action action)
        {
            return Enumerable.Range(0, iterations)
                .Select(_ => Time(action)).Min();
        }

        private static TimeSpan Time(Action action)
        {
            var timer = Stopwatch.StartNew();
            action();
            timer.Stop();
            return timer.Elapsed;
        }
    }
}