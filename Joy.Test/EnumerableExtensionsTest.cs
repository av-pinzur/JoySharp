using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AvP.Joy;

namespace AvP.Joy.Test
{
    [TestClass]
    public class EnumerableExtensionsTest
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
        public void TestZipAll_Enumerable()
        {
            Assert.AreEqual("(0, 0)|(1, 1)|(2, _)|(3, _)",
                Enumerable.Range(0, 4).ZipAll(Enumerable.Range(0, 2), (x, y) => Tuple.Create(x.HasValue ? x.Value.ToString() : "_", y.HasValue ? y.Value.ToString() : "_")).ToStrings().Join("|"));
            Assert.AreEqual("(0, 0, 0)|(1, 1, 1)|(2, _, 2)|(3, _, _)",
                (new[] { Enumerable.Range(0, 4), Enumerable.Range(0, 2), Enumerable.Range(0, 3) }).ZipAll(e => '(' + e.Select(o => o.HasValue ? o.Value.ToString() : "_").Join(", ") + ')').Join("|"));
        }
    }
}