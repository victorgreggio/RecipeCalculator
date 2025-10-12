namespace RecipeCalculator.Common.Variants;

internal sealed class Variant<T1, T2, T3> : VariantBase
{
    #region T1
    private Variant(IVariantHolder item, byte index)
        : base(item, index)
    { }

    public Variant(T1 item)
        : this(new VariantHolder<T1>(item), 0)
    { }

    public static implicit operator Variant<T1, T2, T3>(T1 item)
        => new (item);

    public static explicit operator T1?(Variant<T1, T2, T3> item)
        => item.Get<T1>();
    #endregion

    #region T2
    public Variant(T2 item)
        : this(new VariantHolder<T2>(item), 1)
    { }

    public static implicit operator Variant<T1, T2, T3>(T2 item)
        => new (item);

    public static explicit operator T2?(Variant<T1, T2, T3> item)
        => item.Get<T2>();
    #endregion
    
    #region T3
    public Variant(T3 item)
        : this(new VariantHolder<T3>(item), 1)
    { }

    public static implicit operator Variant<T1, T2, T3>(T3 item)
        => new (item);

    public static explicit operator T3?(Variant<T1, T2, T3> item)
        => item.Get<T3>();
    #endregion
}