using AGTec.Common.Randomizer.Impl;
using AGTec.Common.Test;
using RecipeCalculator.Common.Values;

namespace RecipeCalculator.Common.Test.Values;

[TestClass]
public class StringValueTests : BaseTestWithSut<IValue>
{
    protected override IValue CreateSut()
    {
        return new Value("ABC");
    }

    [TestMethod]
    public void TestIsStringOperation()
    {
        Assert.IsTrue(Sut.Is<string>());
    }

    [TestMethod]
    public void TestAsStringOperation()
    {
        var stringValue = Sut.As<string>();
        Assert.IsNotNull(stringValue);
    }

    [TestMethod]
    public void TestEqualityIsTrue()
    {
        var sutValue = Sut.As<string>();
        var value = new Value(sutValue);
        Assert.IsTrue(Sut.Equals(value));
    }

    [TestMethod]
    public void TestEqualityIsFalse()
    {
        var randomizer = new RandomStringGenerator();
        var value = new Value(randomizer.GenerateValue());
        Assert.IsFalse(Sut.Equals(value));
    }

    [TestMethod]
    public void TestCompareTo()
    {
        var sutValue = Sut.As<string>();
        Assert.IsTrue(new Value(sutValue).CompareTo(Sut) == 0);
        Assert.IsTrue(new Value(sutValue.Replace('B', 'A')).CompareTo(Sut) < 0);
        Assert.IsTrue(new Value(sutValue.Replace('A', 'B')).CompareTo(Sut) > 0);
    }
}