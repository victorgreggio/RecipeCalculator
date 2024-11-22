using RecipeCalculator.Common.Function;
using RecipeCalculator.Common.Values;

namespace RecipeCalculator.Engine.Function;

public interface IFunctionResultCache
{
    IValue? Get(IFunction function);
    void Remove(IFunction function);
    void Set(IFunction function, IValue value);
}