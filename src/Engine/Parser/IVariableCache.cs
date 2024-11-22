using RecipeCalculator.Common.Values;

namespace RecipeCalculator.Engine.Parser;

public interface IVariableCache
{
    IValue? Get(string variableName);
    void Remove(string variableName);
    void Set(string variableName, IValue value);
}