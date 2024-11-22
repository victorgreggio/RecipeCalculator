using System.Collections.Concurrent;
using AGTec.Common.Base.Extensions;
using RecipeCalculator.Common.Values;

namespace RecipeCalculator.Engine.Parser;

public class DefaultVariableCache : IVariableCache
{
    private readonly IDictionary<string, IDictionary<string, IValue>> _cache;

    public DefaultVariableCache()
    {
        _cache = new ConcurrentDictionary<string, IDictionary<string, IValue>>();
    }
    
    public IValue Get(string variableName)
    {
        var snakeName = variableName.ToSnakeCase();
        
        if (_cache.ContainsKey(snakeName))
        {
            if (FormulaContext.FormulaName is not null && _cache[snakeName].ContainsKey(FormulaContext.FormulaName))
                return _cache[snakeName][FormulaContext.FormulaName]; 
            
            if (_cache[snakeName].ContainsKey(string.Empty)) // global context
                return _cache[snakeName][string.Empty];
        }

        throw new Exception($"There is no variable with name {variableName}");
    }

    public void Remove(string variableName)
    {
        var snakeName = variableName.ToSnakeCase();

        if (!_cache.ContainsKey(snakeName)) return;
        
        if (FormulaContext.FormulaName is not null && _cache[snakeName].ContainsKey(FormulaContext.FormulaName))
            _cache[snakeName].Remove(FormulaContext.FormulaName); 
            
        if (_cache[snakeName].ContainsKey(string.Empty)) // global context
            _cache[snakeName].Remove(string.Empty);
    }

    public void Set(string variableName, IValue value)
    {
        var snakeName = variableName.ToSnakeCase();
        if (!_cache.ContainsKey(snakeName))
            _cache.Add(snakeName, new Dictionary<string, IValue>());
        
        _cache[snakeName].Add(FormulaContext.FormulaName ?? string.Empty, value);
    }
}