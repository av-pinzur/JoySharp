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

        [TestMethod]
        public void TryParse_WhenValueIsValid_ReturnsTrueWithNewInstance()
        {
            var input = "George.Washington@example.com";
            var result = EmailAddress.TryParse(input, out var success);
            Assert.IsTrue(result);
            Assert.AreEqual(input, success?.ToString());
        }

        [TestMethod]
        public void TryParse_WhenValueIsMalformed_ReturnsFalseWithNull()
        {
            var input = "George.Washington@@example.com";
            var result = EmailAddress.TryParse(input, out var success);
            Assert.IsFalse(result);
            Assert.IsNull(success);
        }

        [TestMethod]
        public void TryParse_WhenValueIsWhitespace_ReturnsFalseWithNull()
        {
            var input = "\t\r\n ";
            var result = EmailAddress.TryParse(input, out var success);
            Assert.IsFalse(result);
            Assert.IsNull(success);
        }

        [TestMethod]
        public void TryParse_WhenValueIsEmpty_ReturnsFalseWithNull()
        {
            var input = string.Empty;
            var result = EmailAddress.TryParse(input, out var success);
            Assert.IsFalse(result);
            Assert.IsNull(success);
        }
    }
}
