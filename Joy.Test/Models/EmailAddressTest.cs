using AvP.Joy.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AvP.Joy.Test.Models
{
    [TestClass]
    public class EmailAddressTest
    {
        [TestMethod]
        public void Parse_WhenValueIsValid_ReturnsNewInstance()
        {
            var input = "George.Washington@example.com";
            var result = EmailAddress.Parse(input);
            Assert.AreEqual(input, result.ToString());
        }

        [TestMethod]
        public void Parse_WhenValueIsMalformed_ThrowsArgumentExceptionWithDescriptiveMessage()
        {
            var input = "George.Washington@@example.com";
            Assert.ThrowsException<ArgumentException>(() => EmailAddress.Parse(input), "value must a valid EmailAddress.");
        }

        [TestMethod]
        public void Parse_WhenValueIsWhitespace_ThrowsArgumentExceptionWithDescriptiveMessage()
        {
            var input = "\t\r\n ";
            Assert.ThrowsException<ArgumentException>(() => EmailAddress.Parse(input), "value must a valid EmailAddress.");
        }

        [TestMethod]
        public void Parse_WhenValueIsEmpty_ThrowsArgumentExceptionWithDescriptiveMessage()
        {
            var input = string.Empty;
            Assert.ThrowsException<ArgumentException>(() => EmailAddress.Parse(input), "value must a valid EmailAddress.");
        }
    }
}
