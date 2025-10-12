using RecipeCalculator.Common.Variants;

namespace RecipeCalculator.Common.Values;

public class Value : IValue
{
    private readonly Variant<string, double, bool> _item;
    
    private Value(Variant<string, double, bool> item)
    {
        _item = item;
    }

    public Value(double value) : this(new Variant<string, double, bool>(value))
    { }

    public Value(string value) : this(new Variant<string, double, bool>(value))
    { }

    public Value(bool value) : this(new Variant<string, double, bool>(value))
    { }

    public bool Is<T>() => _item.Is<T>();
    public T As<T>() => _item.Get<T>()!;

    public object Get() => _item.Get();
    
    public override int GetHashCode() => _item.GetHashCode();
    public override string ToString() => _item.ToString();

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        var other = (Value)obj;

        return _item.Equals(other._item);
    }

    public int CompareTo(object? obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));
        
        if (GetType() != obj.GetType())
            throw new ArgumentException($"{obj.GetType()} is not comparable to {GetType()}");
        
        if (Equals(obj)) return 0;
        
        var other = (Value)obj;

        return _item.CompareTo(other._item);
    }
}