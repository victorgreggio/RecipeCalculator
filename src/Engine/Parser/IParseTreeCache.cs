using RecipeCalculator.Common.Formulas;

namespace RecipeCalculator.Engine.Parser;

public interface IParseTreeCache
{
    FormulaParser.ExecuteContext? Get(IFormula formula);
    void Remove(IFormula formula);
    void Set(IFormula formula, FormulaParser.ExecuteContext value);
}