using RecipeCalculator.Common.Function;
using RecipeCalculator.Common.Values;

namespace RecipeCalculator.UI.Functions;

/// <summary>
/// Example custom function that returns a constant number.
/// Demonstrates a simple function with no parameters.
/// </summary>
public class GetNum1Func : BaseFunction
{
    public GetNum1Func() : base(nameof(GetNum1Func), 0)
    {
    }

    public override IValue Execute()
    {
        return new Value(1.0);
    }
}
