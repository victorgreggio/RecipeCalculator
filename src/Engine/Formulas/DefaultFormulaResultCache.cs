using System.Collections.Concurrent;
using RecipeCalculator.Common.Formulas;
using RecipeCalculator.Common.Values;

namespace RecipeCalculator.Engine.Formulas;

public class DefaultFormulaResultCache : IFormulaResultCache
{
    private readonly IDictionary<string, IValue> _cache;

    public DefaultFormulaResultCache()
    {
        _cache = new ConcurrentDictionary<string, IValue>();
    }

    public IValue? Get(string formulaName) => _cache.ContainsKey(formulaName) ? _cache[formulaName] : null;

    public void Remove(string formulaName)
    {
        if (_cache.ContainsKey(formulaName))
            _cache.Remove(formulaName);
    }

    public void Set(IFormula formula, IValue value) => _cache[formula.Name] = value;
}