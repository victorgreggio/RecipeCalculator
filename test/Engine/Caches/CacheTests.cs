using RecipeCalculator.Common.Formulas;
using RecipeCalculator.Common.Function;
using RecipeCalculator.Common.Values;
using RecipeCalculator.Engine.Formulas;
using RecipeCalculator.Engine.Function;
using RecipeCalculator.Engine.Parser;
using RecipeCalculator.Engine.Test.Function;

namespace RecipeCalculator.Engine.Test.Caches;

[TestClass]
public class FormulaResultCacheTests
{
    private DefaultFormulaResultCache _cache = null!;

    [TestInitialize]
    public void Setup()
    {
        _cache = new DefaultFormulaResultCache();
    }

    [TestMethod]
    public void Get_NonExistent_ReturnsNull()
    {
        var result = _cache.Get("NonExistent");
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Set_AndGet_ReturnsValue()
    {
        var formula = new Formula("TestFormula", "return 42");
        var value = new Value(42.0);
        
        _cache.Set(formula, value);
        var result = _cache.Get("TestFormula");
        
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Is<double>());
        Assert.AreEqual(42.0, result.As<double>());
    }

    [TestMethod]
    public void Set_OverwritesExistingValue()
    {
        var formula = new Formula("TestFormula", "return 42");
        
        _cache.Set(formula, new Value(42.0));
        _cache.Set(formula, new Value(100.0));
        
        var result = _cache.Get("TestFormula");
        Assert.AreEqual(100.0, result!.As<double>());
    }

    [TestMethod]
    public void Remove_ExistingKey_RemovesValue()
    {
        var formula = new Formula("TestFormula", "return 42");
        _cache.Set(formula, new Value(42.0));
        
        _cache.Remove("TestFormula");
        
        var result = _cache.Get("TestFormula");
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Remove_NonExistentKey_DoesNotThrow()
    {
        _cache.Remove("NonExistent");
        // Test passes if no exception is thrown
    }

    [TestMethod]
    public void Clear_RemovesAllValues()
    {
        _cache.Set(new Formula("Formula1", ""), new Value(1.0));
        _cache.Set(new Formula("Formula2", ""), new Value(2.0));
        _cache.Set(new Formula("Formula3", ""), new Value(3.0));
        
        _cache.Clear();
        
        Assert.IsNull(_cache.Get("Formula1"));
        Assert.IsNull(_cache.Get("Formula2"));
        Assert.IsNull(_cache.Get("Formula3"));
    }

    [TestMethod]
    public void Set_MultipleFormulas_IndependentStorage()
    {
        var formula1 = new Formula("Formula1", "");
        var formula2 = new Formula("Formula2", "");
        
        _cache.Set(formula1, new Value(100.0));
        _cache.Set(formula2, new Value(200.0));
        
        Assert.AreEqual(100.0, _cache.Get("Formula1")!.As<double>());
        Assert.AreEqual(200.0, _cache.Get("Formula2")!.As<double>());
    }
}

[TestClass]
public class FunctionCacheTests
{
    private DefaultFunctionCache _cache = null!;

    [TestInitialize]
    public void Setup()
    {
        _cache = new DefaultFunctionCache();
    }

    [TestMethod]
    public void Get_NonExistent_ReturnsNull()
    {
        var result = _cache.Get("NonExistent_0");
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Set_AndGet_ReturnsFunction()
    {
        var function = new FakeFunction("TestFunc", 2);
        
        _cache.Set(function);
        var result = _cache.Get(FunctionIdBuilder.Create("TestFunc", 2));
        
        Assert.IsNotNull(result);
        Assert.AreEqual("TestFunc", result.Name);
        Assert.AreEqual(2, result.NumOfArgs);
    }

    [TestMethod]
    public void Set_DifferentParameterCounts_StoredSeparately()
    {
        var func0Args = new FakeFunction("TestFunc", 0);
        var func2Args = new FakeFunction("TestFunc", 2);
        
        _cache.Set(func0Args);
        _cache.Set(func2Args);
        
        var result0 = _cache.Get(FunctionIdBuilder.Create("TestFunc", 0));
        var result2 = _cache.Get(FunctionIdBuilder.Create("TestFunc", 2));
        
        Assert.IsNotNull(result0);
        Assert.IsNotNull(result2);
        Assert.AreEqual(0, result0.NumOfArgs);
        Assert.AreEqual(2, result2.NumOfArgs);
    }

    [TestMethod]
    public void Remove_ExistingFunction_RemovesIt()
    {
        var function = new FakeFunction("TestFunc", 2);
        var functionId = FunctionIdBuilder.Create("TestFunc", 2);
        
        _cache.Set(function);
        _cache.Remove(functionId);
        
        var result = _cache.Get(functionId);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Remove_NonExistent_DoesNotThrow()
    {
        _cache.Remove("NonExistent_0");
        // Test passes if no exception is thrown
    }

    [TestMethod]
    public void Clear_RemovesAllFunctions()
    {
        _cache.Set(new FakeFunction("Func1", 0));
        _cache.Set(new FakeFunction("Func2", 1));
        _cache.Set(new FakeFunction("Func3", 2));
        
        _cache.Clear();
        
        Assert.IsNull(_cache.Get(FunctionIdBuilder.Create("Func1", 0)));
        Assert.IsNull(_cache.Get(FunctionIdBuilder.Create("Func2", 1)));
        Assert.IsNull(_cache.Get(FunctionIdBuilder.Create("Func3", 2)));
    }
}

[TestClass]
public class FunctionResultCacheTests
{
    private DefaultFunctionResultCache _cache = null!;

    [TestInitialize]
    public void Setup()
    {
        _cache = new DefaultFunctionResultCache();
    }

    [TestMethod]
    public void Get_NonExistent_ReturnsNull()
    {
        var function = new FakeFunction("Test", 0);
        var result = _cache.Get(function);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Set_AndGet_ReturnsValue()
    {
        var function = new FakeFunction("Test", 0);
        var value = new Value(123.0);
        
        _cache.Set(function, value);
        var result = _cache.Get(function);
        
        Assert.IsNotNull(result);
        Assert.AreEqual(123.0, result.As<double>());
    }

    [TestMethod]
    public void Remove_ExistingFunction_RemovesResult()
    {
        var function = new FakeFunction("Test", 0);
        _cache.Set(function, new Value(123.0));
        
        _cache.Remove(function);
        
        var result = _cache.Get(function);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void Clear_RemovesAllResults()
    {
        var func1 = new FakeFunction("Func1", 0);
        var func2 = new FakeFunction("Func2", 1);
        
        _cache.Set(func1, new Value(1.0));
        _cache.Set(func2, new Value(2.0));
        
        _cache.Clear();
        
        Assert.IsNull(_cache.Get(func1));
        Assert.IsNull(_cache.Get(func2));
    }
}

[TestClass]
public class VariableCacheTests
{
    private DefaultVariableCache _cache = null!;

    [TestInitialize]
    public void Setup()
    {
        _cache = new DefaultVariableCache();
        FormulaContext.FormulaName = null; // Reset to global context
    }

    [TestMethod]
    public void Get_NonExistent_ThrowsException()
    {
        Assert.ThrowsException<Exception>(() =>
        {
            _cache.Get("NonExistentVariable");
        });
    }

    [TestMethod]
    public void Set_AndGet_GlobalContext_ReturnsValue()
    {
        FormulaContext.FormulaName = null;
        var value = new Value(42.0);
        
        _cache.Set("testVariable", value);
        var result = _cache.Get("testVariable");
        
        Assert.IsNotNull(result);
        Assert.AreEqual(42.0, result.As<double>());
    }

    [TestMethod]
    public void Set_AndGet_FormulaContext_ReturnsValue()
    {
        FormulaContext.FormulaName = "TestFormula";
        var value = new Value(100.0);
        
        _cache.Set("myVar", value);
        var result = _cache.Get("myVar");
        
        Assert.AreEqual(100.0, result.As<double>());
    }

    [TestMethod]
    public void Get_PreferFormulaContextOverGlobal()
    {
        // Set global variable
        FormulaContext.FormulaName = null;
        _cache.Set("sharedVar", new Value(10.0));
        
        // Set formula-specific variable
        FormulaContext.FormulaName = "TestFormula";
        _cache.Set("sharedVar", new Value(20.0));
        
        // Should get formula-specific value
        var result = _cache.Get("sharedVar");
        Assert.AreEqual(20.0, result.As<double>());
    }

    [TestMethod]
    public void Get_FallbackToGlobalContext()
    {
        // Set global variable
        FormulaContext.FormulaName = null;
        _cache.Set("globalVar", new Value(50.0));
        
        // Try to get from formula context (should fallback to global)
        FormulaContext.FormulaName = "TestFormula";
        var result = _cache.Get("globalVar");
        
        Assert.AreEqual(50.0, result.As<double>());
    }

    [TestMethod]
    public void Remove_FormulaContext_RemovesVariable()
    {
        FormulaContext.FormulaName = "TestFormula";
        _cache.Set("testVar", new Value(123.0));
        
        _cache.Remove("testVar");
        
        Assert.ThrowsException<Exception>(() => _cache.Get("testVar"));
    }

    [TestMethod]
    public void Clear_RemovesAllVariables()
    {
        _cache.Set("var1", new Value(1.0));
        _cache.Set("var2", new Value(2.0));
        
        _cache.Clear();
        
        Assert.ThrowsException<Exception>(() => _cache.Get("var1"));
        Assert.ThrowsException<Exception>(() => _cache.Get("var2"));
    }

    [TestMethod]
    public void VariableNames_ConvertedToSnakeCase()
    {
        FormulaContext.FormulaName = null;
        _cache.Set("TestVariable", new Value(42.0));
        
        // Should be able to retrieve with different casing
        var result = _cache.Get("test_variable");
        Assert.AreEqual(42.0, result.As<double>());
    }
}

[TestClass]
public class ParseTreeCacheTests
{
    private DefaultParseTreeCache _cache = null!;

    [TestInitialize]
    public void Setup()
    {
        _cache = new DefaultParseTreeCache();
    }

    [TestMethod]
    public void Get_NonExistent_ThrowsException()
    {
        var formula = new Formula("Test", "return 42");
        Assert.ThrowsException<KeyNotFoundException>(() => _cache.Get(formula));
    }

    [TestMethod]
    public void Set_AndGet_ReturnsParseTree()
    {
        var formula = new Formula("Test", "return 42");
        var parseTree = FormulaParserHelper.Parse(formula);
        
        _cache.Set(formula, parseTree);
        var result = _cache.Get(formula);
        
        Assert.IsNotNull(result);
        Assert.AreSame(parseTree, result);
    }

    [TestMethod]
    public void Remove_ExistingFormula_RemovesParseTree()
    {
        var formula = new Formula("Test", "return 42");
        var parseTree = FormulaParserHelper.Parse(formula);
        
        _cache.Set(formula, parseTree);
        _cache.Remove(formula);
        
        Assert.ThrowsException<KeyNotFoundException>(() => _cache.Get(formula));
    }

    [TestMethod]
    public void Clear_RemovesAllParseTrees()
    {
        var formula1 = new Formula("Test1", "return 1");
        var formula2 = new Formula("Test2", "return 2");
        
        _cache.Set(formula1, FormulaParserHelper.Parse(formula1));
        _cache.Set(formula2, FormulaParserHelper.Parse(formula2));
        
        _cache.Clear();
        
        Assert.ThrowsException<KeyNotFoundException>(() => _cache.Get(formula1));
        Assert.ThrowsException<KeyNotFoundException>(() => _cache.Get(formula2));
    }
}
