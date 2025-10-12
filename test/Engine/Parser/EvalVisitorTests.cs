using RecipeCalculator.Common.Formulas;
using RecipeCalculator.Common.Values;
using RecipeCalculator.Engine.Parser;

namespace RecipeCalculator.Engine.Test.Parser;

[TestClass]
public class EvalVisitorTests : BaseEngineRunnerTest
{
    [TestMethod]
    public void TestArithmeticOperations()
    {
        // Addition
        ExecuteAndAssert("return 5 + 3", 8.0);
        
        // Subtraction
        ExecuteAndAssert("return 10 - 4", 6.0);
        
        // Multiplication
        ExecuteAndAssert("return 6 * 7", 42.0);
        
        // Division
        ExecuteAndAssert("return 20 / 4", 5.0);
        
        // Modulo
        ExecuteAndAssert("return 17 Mod 5", 2.0);
        
        // Power
        ExecuteAndAssert("return 2 ^ 3", 8.0);
    }

    [TestMethod]
    public void TestUnaryMinus()
    {
        ExecuteAndAssert("return -5", -5.0);
        ExecuteAndAssert("return -(3 + 2)", -5.0);
    }

    [TestMethod]
    public void TestOperatorPrecedence()
    {
        ExecuteAndAssert("return 2 + 3 * 4", 14.0);
        ExecuteAndAssert("return (2 + 3) * 4", 20.0);
        ExecuteAndAssert("return 2 ^ 3 * 4", 32.0);
        ExecuteAndAssert("return 10 - 2 - 3", 5.0);
    }

    [TestMethod]
    public void TestStringOperations()
    {
        ExecuteAndAssert("return 'Hello' + ' ' + 'World'", "Hello World");
        ExecuteAndAssert("return 'Number: ' + 42", "Number: 42");
    }

    [TestMethod]
    public void TestComparisonOperators()
    {
        // Less than
        ExecuteAndAssert("return 5 < 10", true);
        ExecuteAndAssert("return 10 < 5", false);
        
        // Greater than
        ExecuteAndAssert("return 10 > 5", true);
        ExecuteAndAssert("return 5 > 10", false);
        
        // Less than or equal
        ExecuteAndAssert("return 5 <= 5", true);
        ExecuteAndAssert("return 5 <= 10", true);
        ExecuteAndAssert("return 10 <= 5", false);
        
        // Greater than or equal
        ExecuteAndAssert("return 10 >= 10", true);
        ExecuteAndAssert("return 10 >= 5", true);
        ExecuteAndAssert("return 5 >= 10", false);
        
        // Equality
        ExecuteAndAssert("return 5 = 5", true);
        ExecuteAndAssert("return 5 = 10", false);
        ExecuteAndAssert("return 'test' = 'test'", true);
        
        // Inequality
        ExecuteAndAssert("return 5 <> 10", true);
        ExecuteAndAssert("return 5 <> 5", false);
    }

    [TestMethod]
    public void TestStringComparison()
    {
        ExecuteAndAssert("return 'apple' < 'banana'", true);
        ExecuteAndAssert("return 'zebra' > 'apple'", true);
        ExecuteAndAssert("return 'test' <= 'test'", true);
        ExecuteAndAssert("return 'test' >= 'test'", true);
    }

    [TestMethod]
    public void TestLogicalOperators()
    {
        // AND
        ExecuteAndAssert("return true And true", true);
        ExecuteAndAssert("return true And false", false);
        ExecuteAndAssert("return false And false", false);
        
        // OR
        ExecuteAndAssert("return true Or false", true);
        ExecuteAndAssert("return false Or false", false);
        ExecuteAndAssert("return true Or true", true);
        
        // NOT
        ExecuteAndAssert("return !true", false);
        ExecuteAndAssert("return !false", true);
    }

    [TestMethod]
    public void TestBooleanLiterals()
    {
        ExecuteAndAssert("return true", true);
        ExecuteAndAssert("return false", false);
    }

    [TestMethod]
    public void TestMathFunctions_Max()
    {
        ExecuteAndAssert("return Max(5, 10)", 10.0);
        ExecuteAndAssert("return Max(10, 5)", 10.0);
        ExecuteAndAssert("return Max(-5, -10)", -5.0);
    }

    [TestMethod]
    public void TestMathFunctions_Min()
    {
        ExecuteAndAssert("return Min(5, 10)", 5.0);
        ExecuteAndAssert("return Min(10, 5)", 5.0);
        ExecuteAndAssert("return Min(-5, -10)", -10.0);
    }

    [TestMethod]
    public void TestMathFunctions_Round()
    {
        ExecuteAndAssert("return Rnd(3.14159, 2)", 3.14);
        ExecuteAndAssert("return Rnd(3.14159, 0)", 3.0);
        ExecuteAndAssert("return Rnd(3.5, 0)", 4.0);
    }

    [TestMethod]
    public void TestMathFunctions_Ceil()
    {
        ExecuteAndAssert("return Ceil(3.1)", 4.0);
        ExecuteAndAssert("return Ceil(3.9)", 4.0);
        ExecuteAndAssert("return Ceil(-3.1)", -3.0);
    }

    [TestMethod]
    public void TestMathFunctions_Floor()
    {
        ExecuteAndAssert("return Floor(3.9)", 3.0);
        ExecuteAndAssert("return Floor(3.1)", 3.0);
        ExecuteAndAssert("return Floor(-3.9)", -4.0);
    }

    [TestMethod]
    public void TestMathFunctions_Exp()
    {
        var result = ExecuteFormula("return Exp(1)");
        Assert.IsTrue(result.Is<double>());
        Assert.IsTrue(Math.Abs(result.As<double>() - Math.E) < 0.000001);
    }

    [TestMethod]
    public void TestDateFunctions_Day()
    {
        ExecuteAndAssert("return Day('2023-12-25')", 25.0);
    }

    [TestMethod]
    public void TestDateFunctions_Month()
    {
        ExecuteAndAssert("return Month('2023-12-25')", 12.0);
    }

    [TestMethod]
    public void TestDateFunctions_Year()
    {
        ExecuteAndAssert("return Year('2023-12-25')", 2023.0);
    }

    [TestMethod]
    public void TestDateFunctions_AddDays()
    {
        var result = ExecuteFormula("return AddDays('2023-01-01', 10)");
        Assert.IsTrue(result.Is<string>());
        Assert.IsTrue(result.As<string>().StartsWith("2023-01-11"));
    }

    [TestMethod]
    public void TestDateFunctions_GetDiffDays()
    {
        ExecuteAndAssert("return GetDiffDays('2023-01-11', '2023-01-01')", 10.0);
        ExecuteAndAssert("return GetDiffDays('2023-01-01', '2023-01-11')", -10.0);
    }

    [TestMethod]
    public void TestDateFunctions_DifferenceInMonths()
    {
        ExecuteAndAssert("return DifferenceInMonths('2023-06-01', '2023-01-01')", 5.0);
        ExecuteAndAssert("return DifferenceInMonths('2024-01-01', '2023-01-01')", 12.0);
    }

    [TestMethod]
    public void TestStringFunctions_SubStr()
    {
        ExecuteAndAssert("return SubStr('Hello World', 0, 5)", "Hello");
        ExecuteAndAssert("return SubStr('Testing', 4, 3)", "ing");
    }

    [TestMethod]
    public void TestStringFunctions_PaddedString()
    {
        ExecuteAndAssert("return PaddedString('42', 5)", "00042");
        ExecuteAndAssert("return PaddedString('test', 6)", "00test");
    }

    [TestMethod]
    public void TestDivisionByZero_ReturnsInfinity()
    {
        var result = ExecuteFormula("return 10 / 0");
        Assert.IsTrue(result.Is<double>());
        Assert.IsTrue(double.IsInfinity(result.As<double>()));
    }

    [TestMethod]
    public void TestComplexExpression()
    {
        ExecuteAndAssert("return (5 + 3) * 2 - 10 / 2", 11.0);
        ExecuteAndAssert("return Max(10, Min(20, 15)) + 5", 20.0);
    }

    [TestMethod]
    public void TestNestedFunctionCalls()
    {
        ExecuteAndAssert("return Max(Min(10, 5), Min(8, 12))", 8.0);
        ExecuteAndAssert("return Floor(Ceil(3.5) + 0.5)", 4.0);
    }

    private IValue ExecuteFormula(string body)
    {
        const string formulaName = "TestFormula";
        var formula = new Formula(formulaName, body);
        var parseTree = FormulaParserHelper.Parse(formula);
        ParsingContext.ParseTreeCache.Set(formula, parseTree);
        
        Sut.Execute(new[] { formula }, Enumerable.Empty<RecipeCalculator.Common.Function.IFunction>());
        
        var result = ParsingContext.FormulaResultCache.Get(formulaName);
        Assert.IsNotNull(result);
        return result;
    }

    private void ExecuteAndAssert(string body, double expectedValue)
    {
        var result = ExecuteFormula(body);
        Assert.IsTrue(result.Is<double>(), $"Expected double but got {result.Get().GetType().Name}");
        Assert.AreEqual(expectedValue, result.As<double>(), 0.000001);
    }

    private void ExecuteAndAssert(string body, string expectedValue)
    {
        var result = ExecuteFormula(body);
        Assert.IsTrue(result.Is<string>(), $"Expected string but got {result.Get().GetType().Name}");
        Assert.AreEqual(expectedValue, result.As<string>());
    }

    private void ExecuteAndAssert(string body, bool expectedValue)
    {
        var result = ExecuteFormula(body);
        Assert.IsTrue(result.Is<bool>(), $"Expected bool but got {result.Get().GetType().Name}");
        Assert.AreEqual(expectedValue, result.As<bool>());
    }
}
