using System;
using AvP.Joy.Adts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AvP.Joy.Test.Adts
{
    [TestClass]
    public class ResultTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            Result<string, char> result0 = Method0();
            Result<string, Union<char, int>> result1 = result0.Map(Method1);
            Result<string, Union<char, int, long>> result2 = result1.Map(Method2);

        }

        private Result<String, char> Method0()
        {

        }

        private Result<String, int> Method1(String value)
        {

        }

        private Result<String, long> Method2(String value)
        {

        }
    }
}
