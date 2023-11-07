using AvP.Joy.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AvP.Joy.Test.Models;

[TestClass]
public class EmailAddressTest
{
    private const string ValueA = "George.Washington@example.com";
    private const string ValueB = "John.Adams@example.com";

    [TestMethod]
    public void Parse_WhenValueIsValid_ReturnsEquivalentInstance()
    {
        var input = ValueA;
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

    [TestMethod]
    public void Equals_WhenValuesAreEquivalent_ReturnsTrue()
    {
        var a = EmailAddress.Parse(ValueA);
        var b = EmailAddress.Parse(ValueA);
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void Equals_WhenValuesAreDifferent_ReturnsFalse()
    {
        var a = EmailAddress.Parse(ValueA);
        var b = EmailAddress.Parse(ValueB);
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void EqualsObject_WhenValuesAreEquivalent_ReturnsTrue()
    {
        object a = EmailAddress.Parse(ValueA);
        object b = EmailAddress.Parse(ValueA);
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void EqualsObject_WhenValuesAreDifferent_ReturnsFalse()
    {
        object a = EmailAddress.Parse(ValueA);
        object b = EmailAddress.Parse(ValueB);
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void GetHashCode_WhenValuesAreEquivalent_ReturnsSame()
    {
        var a = EmailAddress.Parse(ValueA);
        var b = EmailAddress.Parse(ValueA);
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_WhenValuesAreDifferent_ReturnsDifferent()
    {
        var a = EmailAddress.Parse(ValueA);
        var b = EmailAddress.Parse(ValueB);
        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void EqualEqual_WhenValuesAreEquivalent_ReturnsTrue()
    {
        var a = EmailAddress.Parse(ValueA);
        var b = EmailAddress.Parse(ValueA);
        Assert.IsTrue(a == b);
    }

    [TestMethod]
    public void EqualEqual_WhenValuesAreDifferent_ReturnsFalse()
    {
        var a = EmailAddress.Parse(ValueA);
        var b = EmailAddress.Parse(ValueB);
        Assert.IsFalse(a == b);
    }

    [TestMethod]
    public void NotEqual_WhenValuesAreEquivalent_ReturnsFalse()
    {
        var a = EmailAddress.Parse(ValueA);
        var b = EmailAddress.Parse(ValueA);
        Assert.IsFalse(a != b);
    }

    [TestMethod]
    public void EqualEqual_WhenValuesAreDifferent_ReturnsTrue()
    {
        var a = EmailAddress.Parse(ValueA);
        var b = EmailAddress.Parse(ValueB);
        Assert.IsTrue(a != b);
    }
}
