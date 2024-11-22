using Antlr4.Runtime.Tree;

namespace RecipeCalculator.Engine.Exceptions;

public class EvalException : Exception
{
    public EvalException(IParseTree ctx) 
        : base($"Illegal expression: {ctx.GetText()}")
    { }

    public EvalException(IParseTree ctx, Exception innerException) 
        : base($"Illegal expression: {ctx.GetText()}", innerException)
    { }

    public EvalException(string message, IParseTree ctx)
        : base(message + $"\nIllegal expression: {ctx.GetText()}")
    { }
}