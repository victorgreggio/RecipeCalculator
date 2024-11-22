namespace RecipeCalculator.Common.Variants;

internal abstract class VariantBase : IComparable
{
    private readonly IVariantHolder _variant;
    
    protected VariantBase(IVariantHolder item, int index)
    {
        _variant = item;
        Index = index;
    }

    private int Index { get; }

    public bool Is<T>() => _variant.Is<T>();

    public T? Get<T>() => ((VariantHolder<T?>)_variant).Item;

    public object Get() => _variant.Get();
    
    public override int GetHashCode() => Get().GetHashCode();
    public override string ToString() => Get().ToString()!;

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        var other = (VariantBase)obj;

        return Index == other.Index && Get().Equals(other.Get());
    }
    
    public int CompareTo(object? obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));
        
        if (GetType() != obj.GetType())
            throw new ArgumentException($"{obj.GetType()} is not comparable to {GetType()}");
        
        if (Equals(obj)) return 0;

        var other = (VariantBase)obj;
        
        return _variant.CompareTo(other._variant);
    }
}