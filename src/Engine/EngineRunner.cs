using AGTec.Common.Base.Extensions;
using Microsoft.Extensions.Logging;
using RecipeCalculator.Common.Formulas;
using RecipeCalculator.Common.Function;
using RecipeCalculator.Engine.Exceptions;
using RecipeCalculator.Engine.Graphs;
using RecipeCalculator.Engine.Parser;

namespace RecipeCalculator.Engine;

public sealed class EngineRunner : IEngineRunner
{
    private readonly IParsingContext _parsingContext;
    private readonly EvalVisitor _visitor;

    private readonly ILogger<EngineRunner> _logger;

    public EngineRunner(IParsingContext parsingContext,
        ILogger<EngineRunner> logger)
    {
        _parsingContext = parsingContext;
        _logger = logger;
        _visitor = new EvalVisitor(_parsingContext);
    }

    public void Execute(IEnumerable<IFormula> formulas, IEnumerable<IFunction> functions)
    {
        var graph = new DAGraph<string, IFormula>();
        formulas.ForEach(f => graph.AddNode(f.Name, f, f.DependsOn.ToList()));
        functions.ForEach(f => _parsingContext.FunctionCache.Set(f));

        var sortedGraph = graph.TopologicalSort();
        sortedGraph.detached.ForEach(f => 
            HandleError(f, new Exception($"Could not resolve dependency path for formula: '{f}'")));
        
        sortedGraph.layers.ForEach(layer => 
            Parallel.ForEach(layer, 
                formula => ExecuteFormula(graph[formula])));
    }

    private void ExecuteFormula(IFormula formula)
    {
        try
        {
            FormulaContext.FormulaName = formula.Name;
            var executionContext = _parsingContext.ParseTreeCache.Get(formula);
            var formulaResult = _visitor.VisitExecute(executionContext);
            _parsingContext.FormulaResultCache.Set(formula, formulaResult);
        }
        catch (CalculatorException ce)
        {
            var errorType = ce.ErrorType;
            HandleError($"Calculation error: '{errorType}'.", ce);
        }
        catch (Exception e)
        {
            HandleError(formula.Name, e);
        }
    }

    private void HandleError(string formulaName, Exception ex)
    {
        _logger.LogError(ex, $"Error while running formula: '{formulaName}'.");
        _parsingContext.Errors.Add(formulaName, ex.Message);
    }
}