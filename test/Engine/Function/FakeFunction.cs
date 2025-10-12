using RecipeCalculator.Common.Function;
using RecipeCalculator.Common.Values;

namespace RecipeCalculator.Engine.Test.Function;

internal class FakeFunction : BaseFunction
{
    public FakeFunction(string name, int numOfArgs) : base(name, numOfArgs)
    { }

    public override IValue Execute()
    {
        return new Value(42.0);
    }
}
