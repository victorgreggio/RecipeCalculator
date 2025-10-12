namespace RecipeCalculator.Engine.Exceptions;

public class ErrorCallException : Exception
{
    public ErrorCallException(string message, Exception? innerException = null) : base(message, innerException)
    { }
}