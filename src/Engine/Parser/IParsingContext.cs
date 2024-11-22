using RecipeCalculator.Engine.Formulas;
using RecipeCalculator.Engine.Function;

namespace RecipeCalculator.Engine.Parser;

public interface IParsingContext
{
    IFunctionCache FunctionCache { get; }
    IFunctionResultCache FunctionResultCache { get; }
    IParseTreeCache ParseTreeCache { get; }
    IFormulaResultCache FormulaResultCache { get; }
    IVariableCache VariableCache { get; }
    IDictionary<string, string> Errors { get; }
}