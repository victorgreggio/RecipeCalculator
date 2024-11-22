using RecipeCalculator.Common.Formulas;
using RecipeCalculator.Common.Function;
using RecipeCalculator.Engine.Parser;
using RecipeCalculator.Engine.Test.Util;

namespace RecipeCalculator.Engine.Test;

[TestClass]
public class MultipleFormulasWithDependencyTest : BaseEngineRunnerTest
{
    
    [TestMethod]
    public void ExecMultipleFormulasWithDependencyTest()
    {
        const string formula1Name = "Test1";
        const string formula1Body = "return 2 + 2";
        var formula1 = new Formula(formula1Name, formula1Body);
        ParsingContext.ParseTreeCache.Set(formula1, FormulaParserHelper.Parse(formula1));

        const string formula2Name = "Test2";
        const string formula2Body = "return 2 + 3";
        var formula2 = new Formula(formula2Name, formula2Body);
        ParsingContext.ParseTreeCache.Set(formula2, FormulaParserHelper.Parse(formula2));

        const string formula3Name = "Test3";
        const string formula3Body = "return GetOutputFrom('Test1') + GetOutputFrom('Test2')";
        var formula3 = new Formula(formula3Name, formula3Body);
        ParsingContext.ParseTreeCache.Set(formula3, FormulaParserHelper.Parse(formula3));
        
        Sut.Execute(new[] {formula1, formula2, formula3}, Enumerable.Empty<IFunction>());

        var result = ParsingContext.FormulaResultCache.Get(formula3Name);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Is<double>());
        Assert.IsTrue(result.As<double>().AlmostEqualTo(9));
    }
}