using RecipeCalculator.Common.Function;

namespace RecipeCalculator.Engine.Function;

public interface IFunctionCache
{
    IFunction? Get(string functionId);
    void Remove(string functionId);
    void Set(IFunction value);
}