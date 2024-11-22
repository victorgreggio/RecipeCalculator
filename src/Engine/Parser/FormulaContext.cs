namespace RecipeCalculator.Engine.Parser;

public static class FormulaContext
{
    private static readonly AsyncLocal<string?> _formulaName = new();

    public static string? FormulaName
    {
        get => _formulaName.Value;
        set => _formulaName.Value = value;
    }
}