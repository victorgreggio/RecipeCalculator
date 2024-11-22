using RecipeCalculator.Common.Function;
using RecipeCalculator.Common.Values;

namespace RecipeCalculator.Common.Test.Function;

[TestClass]
public class FunctionIdBuilderTests
{
    [TestMethod]
    public void TestOneParamFunctionId()
    {
        const string expectedFakeFunctionId = "fake_function_1";
        var fakeFunction = new FakeFunction() { Params = new[] { new Value("ABC")}};
        var fakeFunctionId = FunctionIdBuilder.Create(fakeFunction.Name, 1);
        Assert.IsTrue(string.CompareOrdinal(expectedFakeFunctionId, fakeFunctionId) == 0);
    }
    
    [TestMethod]
    public void TestTwoParamsFunctionId()
    {
        const string expectedFakeFunctionId = "fake_function_2";
        var fakeFunction = new FakeFunction(new[] { new Value(2.0), new Value(2.0) });
        var fakeFunctionId = FunctionIdBuilder.Create(fakeFunction.Name, 2);
        Assert.IsTrue(string.CompareOrdinal(expectedFakeFunctionId, fakeFunctionId) == 0);
    }
}