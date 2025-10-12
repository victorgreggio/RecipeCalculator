namespace RecipeCalculator.Common.Formulas;

public interface IFormula
{
    string Name { get; }
    string Body { get; }
    IEnumerable<string> DependsOn { get; }
}