using RecipeCalculator.Common.Formulas;
using RecipeCalculator.Common.Values;
using RecipeCalculator.Engine.Parser;

namespace RecipeCalculator.Engine.Test;

[TestClass]
public class VariableTests : BaseEngineRunnerTest
{
    [TestMethod]
    public void Variable_SetAndRetrieve_GlobalContext()
    {
        FormulaContext.FormulaName = null;
        ParsingContext.VariableCache.Set("myVar", new Value(42.0));
        
        var result = ExecuteFormula("return myVar");
        
        Assert.IsNotNull(result);
        Assert.AreEqual(42.0, result.As<double>());
    }

    [TestMethod]
    public void Variable_SetAndRetrieve_FormulaContext()
    {
        FormulaContext.FormulaName = "TestFormula";
        ParsingContext.VariableCache.Set("contextVar", new Value(100.0));
        
        var result = ExecuteFormula("return contextVar");
        
        Assert.IsNotNull(result);
        Assert.AreEqual(100.0, result.As<double>());
    }

    [TestMethod]
    public void Variable_StringValue()
    {
        FormulaContext.FormulaName = null;
        ParsingContext.VariableCache.Set("textVar", new Value("Hello World"));
        
        var result = ExecuteFormula("return textVar");
        
        Assert.IsNotNull(result);
        Assert.AreEqual("Hello World", result.As<string>());
    }

    [TestMethod]
    public void Variable_UsedInComparison()
    {
        FormulaContext.FormulaName = null;
        ParsingContext.VariableCache.Set("threshold", new Value(50.0));
        
        var result = ExecuteFormula("return 75 > threshold");
        
        Assert.IsNotNull(result);
        Assert.IsTrue(result.As<bool>());
    }

    [TestMethod]
    public void Variable_UsedInIfStatement()
    {
        FormulaContext.FormulaName = null;
        ParsingContext.VariableCache.Set("value", new Value(15.0));
        
        const string body = @"
            if (value > 10) then
                return 'high'
            else
                return 'low'
            end
        ";
        
        var result = ExecuteFormula(body);
        
        Assert.IsNotNull(result);
        Assert.AreEqual("high", result.As<string>());
    }

    [TestMethod]
    public void Variable_UsedInFunctionCall()
    {
        FormulaContext.FormulaName = null;
        ParsingContext.VariableCache.Set("num1", new Value(5.5));
        ParsingContext.VariableCache.Set("num2", new Value(2.0));
        
        var result = ExecuteFormula("return Rnd(num1, num2)");
        
        Assert.IsNotNull(result);
        Assert.AreEqual(5.5, result.As<double>());
    }

    [TestMethod]
    public void Variable_CamelCaseConversion()
    {
        FormulaContext.FormulaName = null;
        ParsingContext.VariableCache.Set("myTestVariable", new Value(123.0));
        
        // Should be accessible as snake_case
        var result = ExecuteFormula("return my_test_variable");
        
        Assert.IsNotNull(result);
        Assert.AreEqual(123.0, result.As<double>());
    }

    [TestMethod]
    public void Variable_FormulaContextOverridesGlobal()
    {
        // Set global
        FormulaContext.FormulaName = null;
        ParsingContext.VariableCache.Set("sharedVar", new Value(10.0));
        
        // Set formula-specific
        FormulaContext.FormulaName = "TestFormula";
        ParsingContext.VariableCache.Set("sharedVar", new Value(20.0));
        
        var result = ExecuteFormula("return sharedVar");
        
        Assert.IsNotNull(result);
        Assert.AreEqual(20.0, result.As<double>()); // Should get formula-specific value
    }

    [TestMethod]
    public void Variable_FallsBackToGlobal()
    {
        // Set global only
        FormulaContext.FormulaName = null;
        ParsingContext.VariableCache.Set("globalOnly", new Value(99.0));
        
        // Try to access from formula context
        FormulaContext.FormulaName = "TestFormula";
        
        var result = ExecuteFormula("return globalOnly");
        
        Assert.IsNotNull(result);
        Assert.AreEqual(99.0, result.As<double>()); // Should fallback to global
    }

    [TestMethod]
    public void Variable_BooleanValue()
    {
        FormulaContext.FormulaName = null;
        ParsingContext.VariableCache.Set("isEnabled", new Value(true));
        
        var result = ExecuteFormula("return isEnabled");
        
        Assert.IsNotNull(result);
        Assert.IsTrue(result.As<bool>());
    }

    [TestMethod]
    public void Variable_UsedInLogicalExpression()
    {
        FormulaContext.FormulaName = null;
        ParsingContext.VariableCache.Set("flag1", new Value(true));
        ParsingContext.VariableCache.Set("flag2", new Value(false));
        
        var result = ExecuteFormula("return flag1 And !flag2");
        
        Assert.IsNotNull(result);
        Assert.IsTrue(result.As<bool>());
    }

    [TestMethod]
    public void Variable_StringConcatenation()
    {
        FormulaContext.FormulaName = null;
        ParsingContext.VariableCache.Set("firstName", new Value("John"));
        ParsingContext.VariableCache.Set("lastName", new Value("Doe"));
        
        var result = ExecuteFormula("return firstName + ' ' + lastName");
        
        Assert.IsNotNull(result);
        Assert.AreEqual("John Doe", result.As<string>());
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
}
