using AvP.Joy.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AvP.Joy.Test.Models;

[TestClass]
public class ValueWrapperTest
{
    private const string ValueA = "sample-value-a";
    private const string ValueB = "sample-value-b";

    [TestMethod]
    public void Constructor_ReturnsEquivalentInstance()
    {
        var input = ValueA;
        var result = new ValueWrapper<string>(input);
        Assert.AreEqual(input, result.ToString());
    }

    [TestMethod]
    public void EqualsObject_WhenValuesAreEquivalent_ReturnsTrue()
    {
        object a = new ValueWrapper<string>(ValueA);
        object b = new ValueWrapper<string>(ValueA);
        Assert.IsTrue(a.Equals(b));
    }

    [TestMethod]
    public void EqualsObject_WhenValuesAreDifferent_ReturnsFalse()
    {
        object a = new ValueWrapper<string>(ValueA);
        object b = new ValueWrapper<string>(ValueB);
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void EqualsObject_WhenTypesAreDifferent_ReturnsFalse()
    {
        object a = new ValueWrapper<string>(ValueA);
        object b = new StringWrapper(ValueA);
        Assert.IsFalse(a.Equals(b));
    }

    [TestMethod]
    public void GetHashCode_WhenValuesAreEquivalent_ReturnsSame()
    {
        var a = new ValueWrapper<string>(ValueA);
        var b = new ValueWrapper<string>(ValueA);
        Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_WhenValuesAreDifferent_ReturnsDifferent()
    {
        var a = new ValueWrapper<string>(ValueA);
        var b = new ValueWrapper<string>(ValueB);
        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_WhenTypesAreDifferent_ReturnsDifferent()
    {
        var a = new ValueWrapper<string>(ValueA);
        var b = new StringWrapper(ValueA);
        Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [TestMethod]
    public void EqualEqual_WhenValuesAreEquivalent_ReturnsTrue()
    {
        var a = new ValueWrapper<string>(ValueA);
        var b = new ValueWrapper<string>(ValueA);
        Assert.IsTrue(a == b);
    }

    [TestMethod]
    public void EqualEqual_WhenValuesAreDifferent_ReturnsFalse()
    {
        var a = new ValueWrapper<string>(ValueA);
        var b = new ValueWrapper<string>(ValueB);
        Assert.IsFalse(a == b);
    }

    [TestMethod]
    public void EqualEqual_WhenTypesAreDifferent_ReturnsFalse()
    {
        var a = new ValueWrapper<string>(ValueA);
        var b = new StringWrapper(ValueA);
        Assert.IsFalse(a == b);
    }

    [TestMethod]
    public void NotEqual_WhenValuesAreEquivalent_ReturnsFalse()
    {
        var a = new ValueWrapper<string>(ValueA);
        var b = new ValueWrapper<string>(ValueA);
        Assert.IsFalse(a != b);
    }

    [TestMethod]
    public void EqualEqual_WhenValuesAreDifferent_ReturnsTrue()
    {
        var a = new ValueWrapper<string>(ValueA);
        var b = new ValueWrapper<string>(ValueB);
        Assert.IsTrue(a != b);
    }

    [TestMethod]
    public void EqualEqual_WhenTypesAreDifferent_ReturnsTrue()
    {
        var a = new ValueWrapper<string>(ValueA);
        var b = new StringWrapper(ValueA);
        Assert.IsTrue(a != b);
    }
}
