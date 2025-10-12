using RecipeCalculator.Common.Formulas;
using RecipeCalculator.Common.Values;
using RecipeCalculator.Engine.Parser;

namespace RecipeCalculator.Engine.Test;

[TestClass]
public class ConditionalStatementsTests : BaseEngineRunnerTest
{
    [TestMethod]
    public void IfStatement_TrueCondition_ExecutesThenBlock()
    {
        const string body = @"
            if (5 > 3) then
                return 100
            end
        ";
        
        ExecuteAndAssert(body, 100.0);
    }

    [TestMethod]
    public void IfStatement_FalseCondition_ReturnsNull()
    {
        const string body = @"
            if (5 < 3) then
                return 100
            end
        ";
        
        var result = ExecuteFormula(body);
        // When no else and condition is false, should return null or default
        Assert.IsNull(result);
    }

    [TestMethod]
    public void IfElseStatement_TrueCondition_ExecutesThenBlock()
    {
        const string body = @"
            if (10 > 5) then
                return 'greater'
            else
                return 'not greater'
            end
        ";
        
        ExecuteAndAssert(body, "greater");
    }

    [TestMethod]
    public void IfElseStatement_FalseCondition_ExecutesElseBlock()
    {
        const string body = @"
            if (3 > 5) then
                return 'greater'
            else
                return 'not greater'
            end
        ";
        
        ExecuteAndAssert(body, "not greater");
    }

    [TestMethod]
    public void IfElseIfStatement_FirstConditionTrue()
    {
        const string body = @"
            if (10 > 5) then
                return 1
            else if (10 > 3) then
                return 2
            else
                return 3
            end
        ";
        
        ExecuteAndAssert(body, 1.0);
    }

    [TestMethod]
    public void IfElseIfStatement_SecondConditionTrue()
    {
        const string body = @"
            if (3 > 5) then
                return 1
            else if (10 > 3) then
                return 2
            else
                return 3
            end
        ";
        
        ExecuteAndAssert(body, 2.0);
    }

    [TestMethod]
    public void IfElseIfStatement_AllFalse_ExecutesElse()
    {
        const string body = @"
            if (3 > 5) then
                return 1
            else if (2 > 10) then
                return 2
            else
                return 3
            end
        ";
        
        ExecuteAndAssert(body, 3.0);
    }

    [TestMethod]
    public void MultipleElseIfStatements()
    {
        const string body = @"
            if (1 > 10) then
                return 'first'
            else if (2 > 10) then
                return 'second'
            else if (5 > 3) then
                return 'third'
            else if (6 > 4) then
                return 'fourth'
            else
                return 'else'
            end
        ";
        
        ExecuteAndAssert(body, "third");
    }

    [TestMethod]
    public void NestedIfStatements()
    {
        const string body = @"
            if (10 > 5) then
                if (3 > 2) then
                    return 'nested true'
                else
                    return 'nested false'
                end
            else
                return 'outer false'
            end
        ";
        
        ExecuteAndAssert(body, "nested true");
    }

    [TestMethod]
    public void IfStatement_StringComparison()
    {
        const string body = @"
            if ('apple' < 'banana') then
                return 'alphabetically correct'
            else
                return 'not correct'
            end
        ";
        
        ExecuteAndAssert(body, "alphabetically correct");
    }

    [TestMethod]
    public void IfStatement_WithFunctionCall()
    {
        const string body = @"
            if (Max(5, 10) > 8) then
                return 'max is greater'
            else
                return 'max is not greater'
            end
        ";
        
        ExecuteAndAssert(body, "max is greater");
    }

    [TestMethod]
    public void IfStatement_WithNotOperator()
    {
        const string body = @"
            if (!(5 > 10)) then
                return 'negation works'
            else
                return 'negation failed'
            end
        ";
        
        ExecuteAndAssert(body, "negation works");
    }

    private IValue? ExecuteFormula(string body)
    {
        const string formulaName = "TestFormula";
        var formula = new Formula(formulaName, body);
        var parseTree = FormulaParserHelper.Parse(formula);
        ParsingContext.ParseTreeCache.Set(formula, parseTree);
        
        Sut.Execute(new[] { formula }, Enumerable.Empty<RecipeCalculator.Common.Function.IFunction>());
        
        return ParsingContext.FormulaResultCache.Get(formulaName);
    }

    private void ExecuteAndAssert(string body, double expectedValue)
    {
        var result = ExecuteFormula(body);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Is<double>(), $"Expected double but got {result.Get().GetType().Name}");
        Assert.AreEqual(expectedValue, result.As<double>(), 0.000001);
    }

    private void ExecuteAndAssert(string body, string expectedValue)
    {
        var result = ExecuteFormula(body);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Is<string>(), $"Expected string but got {result.Get().GetType().Name}");
        Assert.AreEqual(expectedValue, result.As<string>());
    }
}
