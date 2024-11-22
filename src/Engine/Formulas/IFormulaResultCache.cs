using RecipeCalculator.Common.Formulas;
using RecipeCalculator.Common.Values;

namespace RecipeCalculator.Engine.Formulas;

public interface IFormulaResultCache
{
    IValue? Get(string formulaName);
    void Remove(string formulaName);
    void Set(IFormula formula, IValue value);
}