using RecipeCalculator.Common.Formulas;
using RecipeCalculator.Common.Values;
using RecipeCalculator.Engine.Parser;

namespace RecipeCalculator.Engine.Test;

[TestClass]
public class ComplexScenarioTests : BaseEngineRunnerTest
{
    [TestMethod]
    public void GetOutputFrom_RetrievesAnotherFormulaResult()
    {
        const string formula1Body = "return 42";
        const string formula2Body = "return GetOutputFrom('Formula1') * 2";
        
        var formula1 = new Formula("Formula1", formula1Body);
        var formula2 = new Formula("Formula2", formula2Body);
        
        var parseTree1 = FormulaParserHelper.Parse(formula1);
        var parseTree2 = FormulaParserHelper.Parse(formula2);
        
        ParsingContext.ParseTreeCache.Set(formula1, parseTree1);
        ParsingContext.ParseTreeCache.Set(formula2, parseTree2);
        
        // Execute both formulas in dependency order
        Sut.Execute(new[] { formula1, formula2 }, Enumerable.Empty<RecipeCalculator.Common.Function.IFunction>());
        
        var result = ParsingContext.FormulaResultCache.Get("Formula2");
        Assert.IsNotNull(result);
        Assert.AreEqual(84.0, result.As<double>());
    }

    [TestMethod]
    public void GetOutputFrom_ChainedFormulas()
    {
        var formula1 = new Formula("F1", "return 10");
        var formula2 = new Formula("F2", "return GetOutputFrom('F1') + 5");
        var formula3 = new Formula("F3", "return GetOutputFrom('F2') * 2");
        
        ParsingContext.ParseTreeCache.Set(formula1, FormulaParserHelper.Parse(formula1));
        ParsingContext.ParseTreeCache.Set(formula2, FormulaParserHelper.Parse(formula2));
        ParsingContext.ParseTreeCache.Set(formula3, FormulaParserHelper.Parse(formula3));
        
        Sut.Execute(new[] { formula1, formula2, formula3 }, Enumerable.Empty<RecipeCalculator.Common.Function.IFunction>());
        
        var result = ParsingContext.FormulaResultCache.Get("F3");
        Assert.IsNotNull(result);
        Assert.AreEqual(30.0, result.As<double>()); // (10 + 5) * 2 = 30
    }

    [TestMethod]
    public void ComplexCalculation_WithMultipleFunctions()
    {
        const string body = "return Max(10, Min(20, 15)) + Rnd(3.7, 0) + Floor(5.9)";
        var result = ExecuteFormula(body);
        
        Assert.IsNotNull(result);
        // Max(10, 15) + Rnd(3.7, 0) + Floor(5.9) = 15 + 4 + 5 = 24
        Assert.AreEqual(24.0, result.As<double>());
    }

    [TestMethod]
    public void ComplexExpression_WithAllOperators()
    {
        const string body = "return ((10 + 5) * 2 - 8) / 2 + 3 ^ 2";
        var result = ExecuteFormula(body);
        
        Assert.IsNotNull(result);
        // ((15) * 2 - 8) / 2 + 9 = (30 - 8) / 2 + 9 = 22 / 2 + 9 = 11 + 9 = 20
        Assert.AreEqual(20.0, result.As<double>());
    }

    [TestMethod]
    public void DateCalculations_Complex()
    {
        const string body = @"
            return Year('2023-12-25') * 100 + 
                   Month('2023-12-25') * 10 + 
                   Day('2023-12-25')
        ";
        
        var result = ExecuteFormula(body);
        Assert.IsNotNull(result);
        // 2023 * 100 + 12 * 10 + 25 = 202300 + 120 + 25 = 202445
        Assert.AreEqual(202445.0, result.As<double>());
    }

    [TestMethod]
    public void NestedFunctionCalls_ThreeLevels()
    {
        const string body = "return Max(Min(Max(1, 5), Min(10, 8)), 6)";
        var result = ExecuteFormula(body);
        
        Assert.IsNotNull(result);
        // Max(1, 5) = 5, Min(10, 8) = 8, Min(5, 8) = 5, Max(5, 6) = 6
        Assert.AreEqual(6.0, result.As<double>());
    }

    [TestMethod]
    public void StringManipulation_Complex()
    {
        const string body = "return PaddedString(SubStr('Testing', 0, 4), 10)";
        var result = ExecuteFormula(body);
        
        Assert.IsNotNull(result);
        // SubStr('Testing', 0, 4) = 'Test', PaddedString('Test', 10) = '000000Test'
        Assert.AreEqual("000000Test", result.As<string>());
    }

    [TestMethod]
    public void MixedTypeOperations()
    {
        const string body = "return 'Result: ' + (10 + 5) + ', Status: ' + 'OK'";
        var result = ExecuteFormula(body);
        
        Assert.IsNotNull(result);
        Assert.AreEqual("Result: 15, Status: OK", result.As<string>());
    }

    [TestMethod]
    public void EdgeCase_VeryLargeNumbers()
    {
        const string body = "return 999999999 + 1";
        var result = ExecuteFormula(body);
        
        Assert.IsNotNull(result);
        Assert.AreEqual(1000000000.0, result.As<double>());
    }

    [TestMethod]
    public void EdgeCase_VerySmallNumbers()
    {
        const string body = "return 0.0000001 + 0.0000002";
        var result = ExecuteFormula(body);
        
        Assert.IsNotNull(result);
        Assert.AreEqual(0.0000003, result.As<double>(), 0.0000001);
    }

    [TestMethod]
    public void EdgeCase_NegativeNumbers()
    {
        const string body = "return -10 + -5";
        var result = ExecuteFormula(body);
        
        Assert.IsNotNull(result);
        Assert.AreEqual(-15.0, result.As<double>());
    }

    [TestMethod]
    public void EdgeCase_ZeroComparisons()
    {
        const string body = "return 0 = 0";
        var result = ExecuteFormula(body);
        
        Assert.IsNotNull(result);
        Assert.IsTrue(result.As<bool>());
    }

    [TestMethod]
    public void EdgeCase_EmptyStringConcatenation()
    {
        const string body = "return '' + 'test' + ''";
        var result = ExecuteFormula(body);
        
        Assert.IsNotNull(result);
        Assert.AreEqual("test", result.As<string>());
    }

    [TestMethod]
    public void EdgeCase_StringWithEscapes()
    {
        const string body = @"return 'He said \'hello\''";
        var result = ExecuteFormula(body);
        
        Assert.IsNotNull(result);
        Assert.AreEqual("He said 'hello'", result.As<string>());
    }

    [TestMethod]
    public void EdgeCase_BooleanEquality()
    {
        const string body = "return true = true";
        var result = ExecuteFormula(body);
        
        Assert.IsNotNull(result);
        Assert.IsTrue(result.As<bool>());
    }

    [TestMethod]
    public void EdgeCase_BooleanInequality()
    {
        const string body = "return true <> false";
        var result = ExecuteFormula(body);
        
        Assert.IsNotNull(result);
        Assert.IsTrue(result.As<bool>());
    }

    [TestMethod]
    public void Performance_NestedCalculations()
    {
        const string body = "return ((((1 + 2) * 3) - 4) / 5) + ((((6 + 7) * 8) - 9) / 10)";
        var result = ExecuteFormula(body);
        
        Assert.IsNotNull(result);
        // ((3 * 3) - 4) / 5 = (9 - 4) / 5 = 5 / 5 = 1
        // ((13 * 8) - 9) / 10 = (104 - 9) / 10 = 95 / 10 = 9.5
        // 1 + 9.5 = 10.5
        Assert.AreEqual(10.5, result.As<double>(), 0.0001);
    }

    [TestMethod]
    public void DateDifference_AcrossYears()
    {
        const string body = "return GetDiffDays('2024-01-01', '2023-01-01')";
        var result = ExecuteFormula(body);
        
        Assert.IsNotNull(result);
        Assert.AreEqual(365.0, result.As<double>(), 1.0); // Allow for leap years
    }

    [TestMethod]
    public void MonthDifference_AcrossYears()
    {
        const string body = "return DifferenceInMonths('2024-06-01', '2023-01-01')";
        var result = ExecuteFormula(body);
        
        Assert.IsNotNull(result);
        Assert.AreEqual(17.0, result.As<double>());
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
