using RecipeCalculator.Common.Formulas;
using RecipeCalculator.Common.Function;

namespace RecipeCalculator.Engine;

public interface IEngineRunner
{
    void Execute(IEnumerable<IFormula> formulas, IEnumerable<IFunction> functions);
}