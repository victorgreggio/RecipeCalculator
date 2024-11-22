using RecipeCalculator.Common.Formulas;

namespace RecipeCalculator.Common.Test.Formulas;

[TestClass]
public class FormulaTest
{
    [TestMethod]
    public void TestDependsOn()
    {
        const string formula1 = "Test1";
        const string formula2 = "Test2";
        const string dependencyOne = $"GetOutputFrom('{formula1}')";
        const string dependencyTwo = $"GetOutputFrom('{formula2}')";
        const string body = $@"If ({dependencyOne}) Then
                        // do something
                      ElseIf ({dependencyTwo}) Then
                        // do something else
                      End";
        var formula = new Formula("Test", body);
        
        Assert.IsTrue(formula.DependsOn.Contains(formula1));
        Assert.IsTrue(formula.DependsOn.Contains(formula2));
    }
}