using RecipeCalculator.Common.Function;
using RecipeCalculator.Common.Values;

namespace RecipeCalculator.UI.Functions;

/// <summary>
/// Example custom function that returns a constant number.
/// Demonstrates a simple function with no parameters.
/// </summary>
public class GetNum2Func : BaseFunction
{
    public GetNum2Func() : base(nameof(GetNum2Func), 0)
    {
    }

    public override IValue Execute()
    {
        return new Value(2.0);
    }
}
