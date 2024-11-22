using Antlr4.Runtime;
using RecipeCalculator.Common.Formulas;

namespace RecipeCalculator.Engine.Parser;

public static class FormulaParserHelper
{
    public static FormulaParser.ExecuteContext Parse(IFormula formula) => 
        new FormulaParser(new CommonTokenStream(
            new FormulaLexer(
                CharStreams.fromString(formula.Body))))
            .execute();
}