using RecipeCalculator.Common.Formulas;
using RecipeCalculator.Engine.Exceptions;
using RecipeCalculator.Engine.Parser;

namespace RecipeCalculator.Engine.Test.Exceptions;

[TestClass]
public class ExceptionTests : BaseEngineRunnerTest
{
    [TestMethod]
    public void CalculatorException_HasCorrectErrorType()
    {
        var exception = new CalculatorException(ErrorType.InputParameterMissing, "Test message");
        
        Assert.AreEqual(ErrorType.InputParameterMissing, exception.ErrorType);
        Assert.AreEqual("Test message", exception.Message);
    }

    [TestMethod]
    public void CalculatorException_WithInnerException()
    {
        var innerException = new InvalidOperationException("Inner");
        var exception = new CalculatorException(ErrorType.InputParameterMissing, "Outer", innerException);
        
        Assert.AreEqual("Outer", exception.Message);
        Assert.AreSame(innerException, exception.InnerException);
    }

    [TestMethod]
    public void ErrorCallException_WithMessage()
    {
        var exception = new ErrorCallException("Error occurred");
        
        Assert.AreEqual("Error occurred", exception.Message);
    }

    [TestMethod]
    public void ErrorCallException_WithInnerException()
    {
        var innerException = new InvalidOperationException("Inner");
        var exception = new ErrorCallException("Outer", innerException);
        
        Assert.AreSame(innerException, exception.InnerException);
    }

    [TestMethod]
    public void EvalException_CreatesMessageFromParseTree()
    {
        var formula = new Formula("Test", "return invalid + syntax");
        var parseTree = FormulaParserHelper.Parse(formula);
        
        var exception = new EvalException(parseTree);
        
        Assert.IsTrue(exception.Message.Contains("Illegal expression"));
    }

    [TestMethod]
    public void EvalException_WithCustomMessage()
    {
        var formula = new Formula("Test", "return 42");
        var parseTree = FormulaParserHelper.Parse(formula);
        
        var exception = new EvalException("Custom error", parseTree);
        
        Assert.IsTrue(exception.Message.Contains("Custom error"));
        Assert.IsTrue(exception.Message.Contains("Illegal expression"));
    }

    [TestMethod]
    public void EvalException_WithInnerException()
    {
        var formula = new Formula("Test", "return 42");
        var parseTree = FormulaParserHelper.Parse(formula);
        var innerException = new ArgumentException("Inner");
        
        var exception = new EvalException(parseTree, innerException);
        
        Assert.AreSame(innerException, exception.InnerException);
    }
}
