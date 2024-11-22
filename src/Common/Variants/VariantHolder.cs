namespace RecipeCalculator.Common.Variants;

internal sealed class VariantHolder<T> : IVariantHolder
{
    public VariantHolder(T item)
    {
        if (item is null) throw new ArgumentException($"{nameof(item)} can't be null");
        Item = item;  
    } 
    public T Item { get; }

    public bool Is<TType>() => typeof(TType) == typeof(T);

    public object Get() => Item!;

    public override string ToString() => Item!.ToString()!;

    public int CompareTo(object? obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));
        
        if (obj is not VariantHolder<T> other)
            throw new ArgumentException($"{obj.GetType()} is not comparable to {GetType()}");

        return Type.GetTypeCode(Item!.GetType()) switch
        {
            TypeCode.Boolean => Convert.ToBoolean(Item).CompareTo(other.Item),
            TypeCode.Char => Convert.ToChar(Item).CompareTo(other.Item),
            TypeCode.SByte => Convert.ToSByte(Item).CompareTo(other.Item),
            TypeCode.Byte => Convert.ToByte(Item).CompareTo(other.Item),
            TypeCode.Int16 => Convert.ToInt16(Item).CompareTo(other.Item),
            TypeCode.UInt16 => Convert.ToUInt16(Item).CompareTo(other.Item),
            TypeCode.Int32 => Convert.ToInt32(Item).CompareTo(other.Item),
            TypeCode.UInt32 => Convert.ToUInt32(Item).CompareTo(other.Item),
            TypeCode.Int64 => Convert.ToInt64(Item).CompareTo(other.Item),
            TypeCode.UInt64 => Convert.ToUInt64(Item).CompareTo(other.Item),
            TypeCode.Single => Convert.ToSingle(Item).CompareTo(other.Item),
            TypeCode.Double => Convert.ToDouble(Item).CompareTo(other.Item),
            TypeCode.Decimal => Convert.ToDecimal(Item).CompareTo(other.Item),
            TypeCode.DateTime => Convert.ToDateTime(Item).CompareTo(other.Item),
            TypeCode.String => string.CompareOrdinal(Item.ToString(), other.Item!.ToString()),
            _ => throw new ArgumentException($"Type {Item!.GetType()} is not supported.")
        };
    }
}