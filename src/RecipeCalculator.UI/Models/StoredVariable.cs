namespace RecipeCalculator.UI.Models;

public class StoredVariable
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public VariableType Type { get; set; } = VariableType.Number;
}

public enum VariableType
{
    Number,
    String,
    Boolean
}
