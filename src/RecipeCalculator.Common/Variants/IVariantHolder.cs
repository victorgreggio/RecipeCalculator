namespace RecipeCalculator.Common.Variants;

internal interface IVariantHolder : IComparable
{
    bool Is<T>();
    object Get();
}