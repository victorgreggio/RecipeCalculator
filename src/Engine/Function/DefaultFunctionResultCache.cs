using System.Collections.Concurrent;
using AGTec.Common.Base.Extensions;
using RecipeCalculator.Common.Function;
using RecipeCalculator.Common.Values;

namespace RecipeCalculator.Engine.Function;

public class DefaultFunctionResultCache : IFunctionResultCache
{
    private readonly IDictionary<string, IValue> _cache;

    public DefaultFunctionResultCache()
    {
        _cache = new ConcurrentDictionary<string, IValue>();
    }
    
    public IValue Get(IFunction function)
    {
        var cacheKey = CreateCacheKey(function);
        return (_cache.ContainsKey(cacheKey) ? _cache[cacheKey] : null)!;
    }

    public void Remove(IFunction function)
    {
        var cacheKey = CreateCacheKey(function);
        if (_cache.ContainsKey(cacheKey))
            _cache.Remove(cacheKey);
    }

    public void Set(IFunction function, IValue value)
    {
        var cacheKey = CreateCacheKey(function);
        _cache[cacheKey] = value;
    }
    
    private static string CreateCacheKey(IFunction function) => function.Name.ToSnakeCase() + "_" +
                                                                function.Params.Aggregate(string.Empty,
                                                                    (current, next) =>
                                                                        current + GetTypePrefix(next.Get().GetType()));
    private static string GetTypePrefix(Type type)
    {
        return Type.GetTypeCode(type) switch
        {
            TypeCode.Boolean => "B",
            TypeCode.Byte => "By",
            TypeCode.Char => "C",
            TypeCode.Decimal => "De",
            TypeCode.Double => "D",
            TypeCode.Int16 => "I16",
            TypeCode.Int32 => "I32",
            TypeCode.Int64 => "I64",
            TypeCode.String => "S",
            TypeCode.DateTime => "Dt",
            TypeCode.SByte => "SBy",
            TypeCode.UInt16 => "UI16",
            TypeCode.UInt32 => "UI32",
            TypeCode.UInt64 => "UI64",
            _ => "O"
        };
    }
}