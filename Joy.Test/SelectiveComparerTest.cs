using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AvP.Joy.Test
{
    [TestClass]
    public class SelectiveComparerTest
    {
        private sealed class PersonName
        {
            public string Surname;
            public string GivenName;
        }

        private readonly PersonName george1 = new PersonName { Surname = "Washington", GivenName = "George" };
        private readonly PersonName george2 = new PersonName { Surname = "Bush", GivenName = "George" };
        private readonly PersonName john = new PersonName { Surname = "Adams", GivenName = "John" };

        [TestMethod]
        public void TestSelectiveComparer()
        {
            Assert.AreEqual(0, SelectiveComparer<PersonName>.OrderBy(name => name.GivenName).Compare(george1, george2));
            Assert.AreEqual(1, SelectiveComparer<PersonName>.OrderBy(name => name.GivenName).ThenBy(name => name.Surname).Compare(george1, george2));
            Assert.AreEqual(1, SelectiveComparer<PersonName>.OrderByDescending(name => name.GivenName).Compare(george1, john));
        }

        [TestMethod]
        public void TestSelectiveComparerSpeed()
        {
            const int iterationCount = 1000000;
            int dummy;

            var subject = SelectiveComparer<PersonName>.OrderBy(name => name.Surname).ThenBy(name => name.GivenName);
            dummy = subject.Compare(george1, george2);

            var baseline = Comparer<string>.Default;
            dummy = baseline.Compare(george1.Surname + george1.GivenName, george2.Surname + george2.GivenName);

            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < iterationCount; i++)
                dummy = subject.Compare(george1, george2);
            var subjectTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Restart();
            for (int i = 0; i < iterationCount; i++)
                dummy = baseline.Compare(george1.Surname + george1.GivenName, george2.Surname + george2.GivenName);
            var baselineTime = stopwatch.ElapsedMilliseconds;

            var stats = string.Format("Subject time: {0}. Baseline time: {1}.", TimeSpan.FromMilliseconds(subjectTime), TimeSpan.FromMilliseconds(baselineTime));
            Assert.IsTrue(subjectTime < baselineTime * 2, stats);
            Trace.WriteLine(stats);
        }
    }
}