using RecipeCalculator.Common.Formulas;
using RecipeCalculator.Common.Function;
using RecipeCalculator.Common.Values;
using RecipeCalculator.Engine.Parser;
using RecipeCalculator.Engine.Test.Util;

namespace RecipeCalculator.Engine.Test;

[TestClass]
public class IfWithFuncCallTest : BaseEngineRunnerTest
{

    [TestMethod]
    public void ExecIfWithFuncCallTest()
    {
        ParsingContext.VariableCache.Set("Var1", new Value("A"));
        
        const string formulaName = "Test";
        const string body = "if (Var1 = 'A') then return max(GetNum1Func(), GetNum2Func()) * GetNum3Func('ABC') else return 0.0 end";
        var formula = new Formula(formulaName, body);
        var parseTree = FormulaParserHelper.Parse(formula);
        ParsingContext.ParseTreeCache.Set(formula, parseTree);
        
        Sut.Execute(new[] {formula}, new IFunction[]{new GetNum1Func(), new GetNum2Func(), new GetNum3Func()});

        var result = ParsingContext.FormulaResultCache.Get(formulaName);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Is<double>());
        Assert.IsTrue(result.As<double>().AlmostEqualTo(6));
    }

    [TestMethod]
    public void ExecElseWithFuncCallTest()
    {
        ParsingContext.VariableCache.Set("Var1", new Value("B"));
        
        const string formulaName = "Test";
        const string body = "if (Var1 = 'A') then return max(GetNum1Func(), GetNum2Func()) * GetNum3Func('ABC') else return GetNum2Func() end";
        var formula = new Formula(formulaName, body);
        var parseTree = FormulaParserHelper.Parse(formula);
        ParsingContext.ParseTreeCache.Set(formula, parseTree);
        
        Sut.Execute(new[] {formula}, new IFunction[]{new GetNum1Func(), new GetNum2Func(), new GetNum3Func()});

        var result = ParsingContext.FormulaResultCache.Get(formulaName);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Is<double>());
        Assert.IsTrue(result.As<double>().AlmostEqualTo(2));
    }
    
    [TestMethod]
    public void ExecElseIfWithFuncCallTest()
    {
        ParsingContext.VariableCache.Set("Var1", new Value("B"));
        
        const string formulaName = "Test";
        const string body = "if (Var1 = 'A') then return max(GetNum1Func(), GetNum2Func()) * GetNum3Func('ABC') else if (Var1 = 'B') then return GetNum3Func('BCA') else return GetNum2Func() end";
        var formula = new Formula(formulaName, body);
        var parseTree = FormulaParserHelper.Parse(formula);
        ParsingContext.ParseTreeCache.Set(formula, parseTree);
        
        Sut.Execute(new[] {formula}, new IFunction[]{new GetNum1Func(), new GetNum2Func(), new GetNum3Func()});

        var result = ParsingContext.FormulaResultCache.Get(formulaName);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Is<double>());
        Assert.IsTrue(result.As<double>().AlmostEqualTo(0));
    }

    private class GetNum1Func : BaseFunction
    {
        public GetNum1Func() : base(nameof(GetNum1Func), 0)
        { }

        public override IValue Execute()
        {
            return new Value(1.0);
        }
    }

    private class GetNum2Func : BaseFunction
    {
        public GetNum2Func() : base(nameof(GetNum2Func), 0)
        { }

        public override IValue Execute()
        {
            return new Value(2.0);
        }
    }
    
    private class GetNum3Func : BaseFunction
    {
        public GetNum3Func() : base(nameof(GetNum3Func), 1)
        { }

        public override IValue Execute()
        {
            if (Params.Count() != 1) throw new Exception("Expected one parameter.");
            if (Params.Any(x => !x.Is<string>())) throw new Exception("Expected string parameter.");
            var firstParam = Params.First();
            return firstParam.As<string>().Equals("ABC") ? new Value(3.0) : new Value(0.0);
        }
    }
}