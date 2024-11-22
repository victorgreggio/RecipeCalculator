using AGTec.Common.Randomizer.Impl;
using AGTec.Common.Randomizer.ReferenceTypes;
using AGTec.Common.Randomizer.ValueTypes;
using RecipeCalculator.Common.Formulas;
using RecipeCalculator.Common.Function;
using RecipeCalculator.Engine.Parser;
using RecipeCalculator.Engine.Test.Util;

namespace RecipeCalculator.Engine.Test.Grammar;

[TestClass]
public class GrammarTests : BaseEngineRunnerTest
{
    private readonly IRandomAlphanumericString _randomString = new RandomStringGenerator();
    private readonly IRandomInteger _randomNumber = new RandomIntegerGenerator();
    private readonly IRandomDateTime _randomDateTime = new RandomDateTimeGenerator();

    [TestMethod]
    public void MaxTest()
    {
        var formulaName = _randomString.GenerateValueWithoutSpecialChar();
        var value1 = _randomNumber.GenerateValue();
        var value2 = _randomNumber.GenerateValue();
        var formulaBody = $"return Max({value1} , {value2})";
        var formula = new Formula(formulaName, formulaBody);
        var parseTree = FormulaParserHelper.Parse(formula);
        ParsingContext.ParseTreeCache.Set(formula, parseTree);
        
        Sut.Execute(new[] {formula}, Enumerable.Empty<IFunction>());

        var result = ParsingContext.FormulaResultCache.Get(formulaName);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Is<double>());
        Assert.IsTrue(result.As<double>().AlmostEqualTo(Math.Max(value1, value2)));
    }
    
    [TestMethod]
    public void MinTest()
    {
        var formulaName = _randomString.GenerateValueWithoutSpecialChar();
        var value1 = _randomNumber.GenerateValue();
        var value2 = _randomNumber.GenerateValue();
        var formulaBody = $"return Min({value1} , {value2})";
        var formula = new Formula(formulaName, formulaBody);
        var parseTree = FormulaParserHelper.Parse(formula);
        ParsingContext.ParseTreeCache.Set(formula, parseTree);
        
        Sut.Execute(new[] {formula}, Enumerable.Empty<IFunction>());

        var result = ParsingContext.FormulaResultCache.Get(formulaName);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Is<double>());
        Assert.IsTrue(result.As<double>().AlmostEqualTo(Math.Min(value1, value2)));
    }
    
    [TestMethod]
    public void RndTest()
    {
        var formulaName = _randomString.GenerateValueWithoutSpecialChar();
        var value1 = _randomNumber.GenerateValue();
        var value2 = _randomNumber.GenerateValue(0, 15);
        var formulaBody = $"return Rnd({value1} , {value2})";
        var formula = new Formula(formulaName, formulaBody);
        var parseTree = FormulaParserHelper.Parse(formula);
        ParsingContext.ParseTreeCache.Set(formula, parseTree);
        
        Sut.Execute(new[] {formula}, Enumerable.Empty<IFunction>());

        var result = ParsingContext.FormulaResultCache.Get(formulaName);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Is<double>());
        Assert.IsTrue(result.As<double>().AlmostEqualTo(Math.Round((double)value1, value2)));
    }
    
    [TestMethod]
    public void CeilTest()
    {
        var formulaName = _randomString.GenerateValueWithoutSpecialChar(); 
        const string formulaBody = "return Ceil(3.2)";
        var formula = new Formula(formulaName, formulaBody);
        var parseTree = FormulaParserHelper.Parse(formula);
        ParsingContext.ParseTreeCache.Set(formula, parseTree);
        
        Sut.Execute(new[] {formula}, Enumerable.Empty<IFunction>());

        var result = ParsingContext.FormulaResultCache.Get(formulaName);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Is<double>());
        Assert.IsTrue(result.As<double>().AlmostEqualTo(4));
    }

    [TestMethod]
    public void FloorTest()
    {
        var formulaName = _randomString.GenerateValueWithoutSpecialChar();
        const string formulaBody = "return Floor(3.8)";
        var formula = new Formula(formulaName, formulaBody);
        var parseTree = FormulaParserHelper.Parse(formula);
        ParsingContext.ParseTreeCache.Set(formula, parseTree);

        Sut.Execute(new[] { formula }, Enumerable.Empty<IFunction>());

        var result = ParsingContext.FormulaResultCache.Get(formulaName);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Is<double>());
        Assert.IsTrue(result.As<double>().AlmostEqualTo(3));
    }

    [TestMethod]
    public void ExpTest()
    {
        var formulaName = _randomString.GenerateValueWithoutSpecialChar();
        var value1 = _randomNumber.GenerateValue(1, 10);
        var formulaBody = $"return Exp({value1})";
        var formula = new Formula(formulaName, formulaBody);
        var parseTree = FormulaParserHelper.Parse(formula);
        ParsingContext.ParseTreeCache.Set(formula, parseTree);

        Sut.Execute(new[] { formula }, Enumerable.Empty<IFunction>());

        var result = ParsingContext.FormulaResultCache.Get(formulaName);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Is<double>());
        Assert.IsTrue(result.As<double>().AlmostEqualTo(Math.Exp(value1)));
    }
    
    [TestMethod]
    public void DayTest()
    {
        var formulaName = _randomString.GenerateValueWithoutSpecialChar();
        var value1 = _randomDateTime.GenerateValue();
        var formulaBody = $"return Day('{value1:s}')";
        var formula = new Formula(formulaName, formulaBody);
        var parseTree = FormulaParserHelper.Parse(formula);
        ParsingContext.ParseTreeCache.Set(formula, parseTree);

        Sut.Execute(new[] { formula }, Enumerable.Empty<IFunction>());

        var result = ParsingContext.FormulaResultCache.Get(formulaName);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Is<double>());
        Assert.IsTrue(result.As<double>().AlmostEqualTo(value1.Day));
    }
    
    [TestMethod]
    public void MonthTest()
    {
        var formulaName = _randomString.GenerateValueWithoutSpecialChar();
        var value1 = _randomDateTime.GenerateValue();
        var formulaBody = $"return Month('{value1:s}')";
        var formula = new Formula(formulaName, formulaBody);
        var parseTree = FormulaParserHelper.Parse(formula);
        ParsingContext.ParseTreeCache.Set(formula, parseTree);

        Sut.Execute(new[] { formula }, Enumerable.Empty<IFunction>());

        var result = ParsingContext.FormulaResultCache.Get(formulaName);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Is<double>());
        Assert.IsTrue(result.As<double>().AlmostEqualTo(value1.Month));
    }
    
    [TestMethod]
    public void YearTest()
    {
        var formulaName = _randomString.GenerateValueWithoutSpecialChar();
        var value1 = _randomDateTime.GenerateValue();
        var formulaBody = $"return Year('{value1:s}')";
        var formula = new Formula(formulaName, formulaBody);
        var parseTree = FormulaParserHelper.Parse(formula);
        ParsingContext.ParseTreeCache.Set(formula, parseTree);

        Sut.Execute(new[] { formula }, Enumerable.Empty<IFunction>());

        var result = ParsingContext.FormulaResultCache.Get(formulaName);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Is<double>());
        Assert.IsTrue(result.As<double>().AlmostEqualTo(value1.Year));
    }
    
    [TestMethod]
    public void SubstrTest()
    {
        var formulaName = _randomString.GenerateValueWithoutSpecialChar();
        var value1 = _randomString.GenerateValueWithoutSpecialChar();
        var value2 = _randomNumber.GenerateValue(0, value1.Length / 2);
        var value3 = _randomNumber.GenerateValue(1, 3);
        var formulaBody = $"return Substr('{value1}', {value2}, {value3})";
        var formula = new Formula(formulaName, formulaBody);
        var parseTree = FormulaParserHelper.Parse(formula);
        ParsingContext.ParseTreeCache.Set(formula, parseTree);

        Sut.Execute(new[] { formula }, Enumerable.Empty<IFunction>());

        var result = ParsingContext.FormulaResultCache.Get(formulaName);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Is<string>());
        Assert.AreEqual(value1.Substring(Convert.ToInt32(value2), Convert.ToInt32(value3)), result.As<string>());
    }
    
    [TestMethod]
    public void AddDaysTest()
    {
        var formulaName = _randomString.GenerateValueWithoutSpecialChar();
        var value1 = _randomDateTime.GenerateValue();
        var value2 = _randomNumber.GenerateValue(0, 15);
        var formulaBody = $"return AddDays('{value1:s}', {value2})";
        var formula = new Formula(formulaName, formulaBody);
        var parseTree = FormulaParserHelper.Parse(formula);
        ParsingContext.ParseTreeCache.Set(formula, parseTree);

        Sut.Execute(new[] { formula }, Enumerable.Empty<IFunction>());

        var result = ParsingContext.FormulaResultCache.Get(formulaName);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Is<string>());
        Assert.AreEqual(value1.AddDays(value2).ToString("s"), result.As<string>());
    }
    
    [TestMethod]
    public void GetDiffDaysTest()
    {
        var formulaName = _randomString.GenerateValueWithoutSpecialChar();
        var value1 = _randomDateTime.GenerateValue();
        var value2 = _randomDateTime.GenerateValue();
        var formulaBody = $"return GetDiffDays('{value1:s}', '{value2:s}')";
        var formula = new Formula(formulaName, formulaBody);
        var parseTree = FormulaParserHelper.Parse(formula);
        ParsingContext.ParseTreeCache.Set(formula, parseTree);

        Sut.Execute(new[] { formula }, Enumerable.Empty<IFunction>());

        var result = ParsingContext.FormulaResultCache.Get(formulaName);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Is<double>());
        Assert.IsTrue((value1 - value2).TotalDays.AlmostEqualTo(result.As<double>()));
    }
    
    [TestMethod]
    public void PaddedStringTest()
    {
        var formulaName = _randomString.GenerateValueWithoutSpecialChar();
        var value1 = _randomString.GenerateValueWithoutSpecialChar();
        var value2 = _randomNumber.GenerateValue(value1.Length, value1.Length + 5);
        var formulaBody = $"return PaddedString('{value1}', {value2})";
        var formula = new Formula(formulaName, formulaBody);
        var parseTree = FormulaParserHelper.Parse(formula);
        ParsingContext.ParseTreeCache.Set(formula, parseTree);

        Sut.Execute(new[] { formula }, Enumerable.Empty<IFunction>());

        var result = ParsingContext.FormulaResultCache.Get(formulaName);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Is<string>());
        Assert.IsTrue(result.As<string>().Length == value2);
        Assert.AreEqual(string.Empty.PadLeft(value2 - value1.Length, '0'), 
            result.As<string>()[..(value2 - value1.Length)]);
    }
    
    [TestMethod]
    public void DifferenceIntMonthsTest()
    {
        const int expectedDiff = 5;
        var formulaName = _randomString.GenerateValueWithoutSpecialChar();
        var value1 = _randomDateTime.GenerateValue();
        var value2 = value1.AddMonths(expectedDiff);
        var formulaBody = $"return DifferenceInMonths('{value1:s}', '{value2:s}')";
        var formula = new Formula(formulaName, formulaBody);
        var parseTree = FormulaParserHelper.Parse(formula);
        ParsingContext.ParseTreeCache.Set(formula, parseTree);

        Sut.Execute(new[] { formula }, Enumerable.Empty<IFunction>());

        var result = ParsingContext.FormulaResultCache.Get(formulaName);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Is<double>());
        Assert.IsTrue(result.As<double>().AlmostEqualTo(expectedDiff));
    }
}