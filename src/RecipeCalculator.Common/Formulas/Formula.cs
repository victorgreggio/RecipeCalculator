using System.Text.RegularExpressions;
using AGTec.Common.Base.Extensions;

namespace RecipeCalculator.Common.Formulas;

public sealed class Formula : IFormula
{
    public Formula(string name, string body)
    {
        Name = name;
        Body = body;
        DependsOn = BuildDependsOn(body);
    }
    
    public string Name { get; }
    public string Body { get; }
    public IEnumerable<string> DependsOn { get; }

    private static IEnumerable<string> BuildDependsOn(string body)
    {
        const string pattern = "(?s)(?<=GetOutputFrom\\(').*?(?='\\))";
        var result = new List<string>();
        Regex.Matches(body, pattern, RegexOptions.None).ForEach(match => result.Add(match.Value));
        return result;
    }
}