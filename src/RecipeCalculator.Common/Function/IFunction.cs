using RecipeCalculator.Common.Values;

namespace RecipeCalculator.Common.Function;

public interface IFunction
{
    string Name { get; }
    int NumOfArgs { get; }
    IEnumerable<IValue> Params { get; set; }
    IValue Execute();
}