using RecipeCalculator.Common.Function;
using RecipeCalculator.Common.Values;

namespace RecipeCalculator.Common.Test.Function;

internal class FakeFunction : BaseFunction
{
    public FakeFunction() : base(nameof(FakeFunction), 1)
    { }
    
    public FakeFunction(IEnumerable<IValue> parameters) : base(nameof(FakeFunction), 2)
    {
        Params = parameters;
    }

    public override IValue Execute()
    {
        if (Params.Any(p => !p.Is<double>())) throw new Exception(
            $"Expected 2 numbers, but got ({string.Join(',', Params.Select(p => p.GetType()))}) parameters");
        
        var firstNumber = Params.First();
        var secondNumber = Params.Last();

        return new Value(firstNumber.As<double>() * secondNumber.As<double>());
    }
}