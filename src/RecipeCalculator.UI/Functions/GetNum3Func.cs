using RecipeCalculator.Common.Function;
using RecipeCalculator.Common.Values;

namespace RecipeCalculator.UI.Functions;

/// <summary>
/// Example custom function that takes a string parameter and returns a number based on the input.
/// Demonstrates:
/// - Function with one parameter
/// - Parameter validation
/// - Conditional logic
/// - Type checking (string parameter)
/// </summary>
public class GetNum3Func : BaseFunction
{
    public GetNum3Func() : base(nameof(GetNum3Func), 1)
    {
    }

    public override IValue Execute()
    {
        // Validate parameter count
        if (Params.Count() != 1)
        {
            throw new Exception("GetNum3Func expects exactly one parameter.");
        }

        // Validate parameter type
        if (Params.Any(x => !x.Is<string>()))
        {
            throw new Exception("GetNum3Func expects a string parameter.");
        }

        // Get the first parameter
        var firstParam = Params.First();
        var paramValue = firstParam.As<string>();

        // Return different values based on input
        return paramValue.Equals("ABC", StringComparison.OrdinalIgnoreCase) 
            ? new Value(3.0) 
            : new Value(0.0);
    }
}
