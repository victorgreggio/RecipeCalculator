namespace RecipeCalculator.Common.Values;

public interface IValue : IComparable
{
    bool Is<T>();
    T As<T>();
    object Get();
}