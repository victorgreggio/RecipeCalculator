using RecipeCalculator.Common.Function;

namespace RecipeCalculator.Common.Parser;

public interface IParsingContext
{
    IFunctionResultCache FunctionResultCache { get; }
    IParseTreeCache ParseTreeCache { get; }
}