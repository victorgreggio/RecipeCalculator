using AGTec.Common.Randomizer.Impl;
using AGTec.Common.Test;
using RecipeCalculator.Common.Values;

namespace RecipeCalculator.Common.Test.Values;

[TestClass]
public class DoubleValueTests : BaseTestWithSut<IValue>
{
    protected override IValue CreateSut()
    {
        return new Value(1.0);
    }

    [TestMethod]
    public void TestIsDoubleOperation()
    {
        Assert.IsTrue(Sut.Is<double>());
    }

    [TestMethod]
    public void TestIsStringOperation()
    {
        Assert.IsFalse(Sut.Is<string>());
    }

    [TestMethod]
    public void TestAsDoubleOperation()
    {
        var doubleValue = Sut.As<double>();
        Assert.IsTrue(Math.Abs(doubleValue - 1.0) < 0.00001);
    }
    
    [TestMethod]
    [ExpectedException(typeof(InvalidCastException))]
    public void TestAsStringOperation()
    {
        var stringValue = Sut.As<string>();
        Assert.IsNotNull(stringValue);
    }

    [TestMethod]
    public void TestEqualityIsTrue()
    {
        var sutValue = Sut.As<double>();
        var value = new Value(sutValue);
        Assert.IsTrue(Sut.Equals(value));
    }

    [TestMethod]
    public void TestEqualityIsFalse()
    {
        var randomizer = new RandomDoubleGenerator();
        var value = new Value(randomizer.GenerateValue());
        Assert.IsFalse(Sut.Equals(value));
    }

    [TestMethod]
    public void TestCompareTo()
    {
        var sutValue = Sut.As<double>();
        Assert.IsTrue(new Value(sutValue).CompareTo(Sut) == 0);
        Assert.IsTrue(new Value(sutValue - 1.0).CompareTo(Sut) < 0);
        Assert.IsTrue(new Value(sutValue + 1.0).CompareTo(Sut) > 0);
    }
}