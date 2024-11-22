using RecipeCalculator.Common.Values;

namespace RecipeCalculator.Common.Function;

public abstract class BaseFunction : IFunction
{
    private IEnumerable<IValue> _params;
    protected BaseFunction(string name, int numOfArgs)
    {
        Name = name;
        NumOfArgs = numOfArgs;
        _params = Enumerable.Empty<IValue>();
    }
    public string Name { get; }
    public int NumOfArgs { get; }

    public IEnumerable<IValue> Params
    {
        get => _params;
        set
        {
            if (value.Count() != NumOfArgs)
                throw new ArgumentException($"Expected {NumOfArgs}, but got {value.Count()}");
            _params = value;
        }
    }

    public abstract IValue Execute();
}