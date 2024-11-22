using System.Collections.Concurrent;
using AGTec.Common.Base.Extensions;
using RecipeCalculator.Common.Formulas;

namespace RecipeCalculator.Engine.Parser;

public class DefaultParseTreeCache : IParseTreeCache
{
    private readonly IDictionary<string, FormulaParser.ExecuteContext> _cache;

    public DefaultParseTreeCache()
    {
        _cache = new ConcurrentDictionary<string, FormulaParser.ExecuteContext>();
    }

    public FormulaParser.ExecuteContext Get(IFormula formula) => _cache[formula.Name.ToSnakeCase()];

    public void Remove(IFormula formula) => _cache.Remove(formula.Name.ToSnakeCase());

    public void Set(IFormula formula, FormulaParser.ExecuteContext value) => _cache[formula.Name.ToSnakeCase()] = value;
}