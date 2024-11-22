using System.Collections.Concurrent;
using RecipeCalculator.Common.Function;

namespace RecipeCalculator.Engine.Function;

public class DefaultFunctionCache : IFunctionCache
{
    private readonly IDictionary<string, IFunction> _cache;

    public DefaultFunctionCache()
    {
        _cache = new ConcurrentDictionary<string, IFunction>();
    }

    public IFunction? Get(string functionId) => _cache.ContainsKey(functionId) ? _cache[functionId] : null;


    public void Remove(string functionId)
    {
        if (_cache.ContainsKey(functionId))
            _cache.Remove(functionId);
    }

    public void Set(IFunction value) => _cache[ FunctionIdBuilder.Create(value.Name, value.NumOfArgs)] = value;
}