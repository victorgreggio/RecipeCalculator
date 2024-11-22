using System.Collections.Concurrent;
using RecipeCalculator.Engine.Formulas;
using RecipeCalculator.Engine.Function;

namespace RecipeCalculator.Engine.Parser;

public class DefaultParsingContext : IParsingContext
{
    public DefaultParsingContext(IFunctionCache functionCache,
        IFunctionResultCache functionResultCache,
        IParseTreeCache parseTreeCache,
        IFormulaResultCache formulaResultCache,
        IVariableCache variableCache)
    {
        FunctionCache = functionCache;
        FunctionResultCache = functionResultCache;
        ParseTreeCache = parseTreeCache;
        FormulaResultCache = formulaResultCache;
        VariableCache = variableCache;
        Errors = new ConcurrentDictionary<string, string>();
    }
    
    public IFunctionCache FunctionCache { get; }
    public IFunctionResultCache FunctionResultCache { get; }
    public IParseTreeCache ParseTreeCache { get; }
    public IFormulaResultCache FormulaResultCache { get; }
    public IVariableCache VariableCache { get; }
    public IDictionary<string, string> Errors { get; }
}