namespace RecipeCalculator.Engine.Exceptions;

public class CalculatorException : Exception
{
    public CalculatorException(ErrorType errorType, string? message = null, Exception? innerException = null) : base(message,
        innerException)
    {
        ErrorType = errorType;
    }

    public ErrorType ErrorType { get; }
}