using RecipeCalculator.Common.Formulas;
using RecipeCalculator.Common.Function;
using RecipeCalculator.Engine.Parser;
using RecipeCalculator.Engine.Test.Util;

namespace RecipeCalculator.Engine.Test;

[TestClass]
public class SumTwoNumbersTest : BaseEngineRunnerTest
{
    [TestMethod]
    public void ExecSumTwoNumbersTest()
    {
        const string formulaName = "Test";
        const string body = "return 2 + 2";
        var formula = new Formula(formulaName, body);
        var parseTree = FormulaParserHelper.Parse(formula);
        ParsingContext.ParseTreeCache.Set(formula, parseTree);
        
        Sut.Execute(new[] {formula}, Enumerable.Empty<IFunction>());

        var result = ParsingContext.FormulaResultCache.Get(formulaName);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Is<double>());
        Assert.IsTrue(result.As<double>().AlmostEqualTo(4));
    }
}