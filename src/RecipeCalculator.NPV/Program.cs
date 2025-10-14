using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RecipeCalculator.Common.Formulas;
using RecipeCalculator.Engine;
using RecipeCalculator.Engine.Formulas;
using RecipeCalculator.Engine.Function;
using RecipeCalculator.Engine.Parser;
using RecipeCalculator.NPV;
using RecipeCalculator.NPV.Functions;

if (args.Length == 0)
{
    Console.WriteLine("Usage: RecipeCalculator.NPV <input_file> [output_file] [budget]");
    Console.WriteLine("       RecipeCalculator.NPV --generate-sample <count> [output_file]");
    Console.WriteLine();
    Console.WriteLine("Arguments:");
    Console.WriteLine("  input_file   : Path to CSV file with project data");
    Console.WriteLine("  output_file  : Path for output CSV (default: input_file.results.csv)");
    Console.WriteLine("  budget       : Investment budget for portfolio optimization (default: 1,000,000)");
    Console.WriteLine();
    Console.WriteLine("Input file format (CSV):");
    Console.WriteLine("ProjectId,DiscountRate,InitialCost,Year1Income,...,Year15Income,AlignsWithBusinessStrategyABC");
    Console.WriteLine();
    Console.WriteLine("Example:");
    Console.WriteLine("PROJ001,0.10,1000000,150000,...,150000,TRUE");
    Console.WriteLine();
    Console.WriteLine("The application will:");
    Console.WriteLine("  1. Calculate NPV for each project using CalculateNPVFunc");
    Console.WriteLine("  2. Calculate AdjustedNPV based on business strategy alignment");
    Console.WriteLine("  3. Optimize portfolio selection to maximize AdjustedNPV within budget");
    return 1;
}

// Handle sample generation
if (args[0] == "--generate-sample")
{
    if (args.Length < 2)
    {
        Console.WriteLine("Error: Please specify the number of projects to generate.");
        Console.WriteLine("Usage: RecipeCalculator.NPV --generate-sample <count> [output_file]");
        return 1;
    }

    int count = int.Parse(args[1]);
    string outputPath = args.Length > 2 ? args[2] : "sample_projects.csv";

    var sw = Stopwatch.StartNew();
    SampleFileGenerator.GenerateSampleFile(outputPath, count);
    sw.Stop();

    Console.WriteLine($"Generation completed in {sw.Elapsed.TotalSeconds:F2} seconds");
    return 0;
}

string inputFile = args[0];
string outputFile = args.Length > 1 ? args[1] : Path.ChangeExtension(inputFile, ".results.csv");
double budget = args.Length > 2 ? double.Parse(args[2]) : 1_000_000; // Default: $1M

if (!File.Exists(inputFile))
{
    Console.WriteLine($"Error: Input file '{inputFile}' not found.");
    return 1;
}

var stopwatch = Stopwatch.StartNew();
Console.WriteLine($"Processing file: {inputFile}");
Console.WriteLine($"Output will be written to: {outputFile}");
Console.WriteLine($"Investment budget: ${budget:N0}");
Console.WriteLine();

try
{
    await ProcessFileAsync(inputFile, outputFile, budget);
    stopwatch.Stop();

    Console.WriteLine();
    Console.WriteLine($"Processing completed in {stopwatch.Elapsed.TotalSeconds:F2} seconds");
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
    return 1;
}

static async Task ProcessFileAsync(string inputFile, string outputFile, double budget)
{
    var fileInfo = new FileInfo(inputFile);
    long fileSize = fileInfo.Length;
    Console.WriteLine($"File size: {fileSize / (1024.0 * 1024.0):F2} MB");

    // Setup Engine with all required services
    var services = new ServiceCollection();
    services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Error));
    services.AddSingleton<IFunctionCache, DefaultFunctionCache>();
    services.AddSingleton<IFunctionResultCache, DefaultFunctionResultCache>();
    services.AddSingleton<IFormulaResultCache, DefaultFormulaResultCache>();
    services.AddSingleton<IParseTreeCache, DefaultParseTreeCache>();
    services.AddSingleton<IVariableCache, DefaultVariableCache>();
    services.AddSingleton<IParsingContext, DefaultParsingContext>();
    services.AddSingleton<IEngineRunner, EngineRunner>();
    var serviceProvider = services.BuildServiceProvider();

    var parsingContext = serviceProvider.GetRequiredService<IParsingContext>();
    var engine = serviceProvider.GetRequiredService<IEngineRunner>();

    Console.WriteLine("RecipeCalculator.Engine initialized successfully");
    Console.WriteLine();

    await ProcessStandardFileAsync(inputFile, outputFile, engine, parsingContext, budget);
}

static async Task ProcessStandardFileAsync(string inputFile, string outputFile, IEngineRunner engine, IParsingContext parsingContext, double budget)
{
    int totalProcessed = 0;
    int totalErrors = 0;
    var results = new ConcurrentBag<(ProjectData data, ProjectResult result)>();

    var lines = File.ReadLines(inputFile).ToList();

    // Skip header
    if (lines.Count > 0)
    {
        lines = lines.Skip(1).ToList();
    }

    foreach (var line in lines)
    {
        if (string.IsNullOrWhiteSpace(line))
            continue;

        try
        {
            var (data, result) = ProcessLineWithData(line, engine, parsingContext);
            results.Add((data, result));

            int processed = Interlocked.Increment(ref totalProcessed);
            if (processed % 100000 == 0)
            {
                Console.WriteLine($"Processed {processed:N0} projects...");
            }
        }
        catch (Exception)
        {
            Interlocked.Increment(ref totalErrors);
        }
    }

    Console.WriteLine($"Total projects processed: {totalProcessed:N0}");
    if (totalErrors > 0)
    {
        Console.WriteLine($"Total errors: {totalErrors:N0}");
    }
    Console.WriteLine();

    // Perform portfolio optimization
    Console.WriteLine("=== Portfolio Optimization ===");
    Console.WriteLine($"Budget: ${budget:N0}");
    Console.WriteLine("Optimizing project selection to maximize Adjusted NPV...");
    Console.WriteLine();

    var projectList = results.ToList();
    var optimizationResult = PortfolioOptimizer.OptimizePortfolioWithData(projectList, budget);

    Console.WriteLine($"Optimization Status: {optimizationResult.Status}");
    Console.WriteLine($"Solve Time: {optimizationResult.SolveTime:F3} seconds");
    Console.WriteLine($"Selected Projects: {optimizationResult.ProjectCount}");
    Console.WriteLine($"Total Investment: ${optimizationResult.TotalInvestment:N2}");
    Console.WriteLine($"Remaining Budget: ${optimizationResult.RemainingBudget:N2}");
    Console.WriteLine($"Total Adjusted NPV: ${optimizationResult.TotalAdjustedNPV:N2}");
    Console.WriteLine();

    Console.WriteLine("Selected Projects:");
    foreach (var projectId in optimizationResult.SelectedProjects.OrderBy(p => p))
    {
        Console.WriteLine($"  - {projectId}");
    }
    Console.WriteLine();

    // Write results to file
    using var outputStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.None, 65536);
    using var writer = new StreamWriter(outputStream, Encoding.UTF8);

    await writer.WriteLineAsync("ProjectId,InitialCost,AdjustedNPV,AlignsWithBusinessStrategyABC,Selected");

    Console.WriteLine("Writing results...");
    foreach (var (data, result) in results.OrderBy(r => r.data.ProjectId))
    {
        bool selected = optimizationResult.SelectedProjects.Contains(data.ProjectId);
        await writer.WriteLineAsync(
            $"{data.ProjectId},{data.InitialCost:F2},{result.AdjustedNPV:F2},{(data.AlignsWithBusinessStrategy ? "TRUE" : "FALSE")},{(selected ? "YES" : "NO")}");
    }

    // Write optimization summary to separate file
    string summaryFile = Path.ChangeExtension(outputFile, ".optimization.txt");
    await File.WriteAllTextAsync(summaryFile,
        $"Portfolio Optimization Summary\n" +
        $"==============================\n\n" +
        $"Budget: ${budget:N2}\n" +
        $"Optimization Status: {optimizationResult.Status}\n" +
        $"Solve Time: {optimizationResult.SolveTime:F3} seconds\n\n" +
        $"Results:\n" +
        $"  Selected Projects: {optimizationResult.ProjectCount}\n" +
        $"  Total Investment: ${optimizationResult.TotalInvestment:N2}\n" +
        $"  Remaining Budget: ${optimizationResult.RemainingBudget:N2}\n" +
        $"  Total Adjusted NPV: ${optimizationResult.TotalAdjustedNPV:N2}\n\n" +
        $"Selected Projects:\n" +
        string.Join("\n", optimizationResult.SelectedProjects.OrderBy(p => p).Select(p => $"  - {p}"))
    );

    Console.WriteLine($"Optimization summary written to: {summaryFile}");
}

static (ProjectData, ProjectResult) ProcessLineWithData(string line, IEngineRunner engine, IParsingContext parsingContext)
{
    var parts = line.Split(',');

    if (parts.Length < 19) // ProjectId + DiscountRate + InitialCost + 15 years + BusinessStrategyFlag
    {
        throw new FormatException($"Invalid line format: expected at least 19 fields, got {parts.Length}");
    }

    string projectId = parts[0];
    string discountRateStr = parts[1];
    string initialCostStr = parts[2];
    double discountRate = double.Parse(discountRateStr, CultureInfo.InvariantCulture);
    double initialCost = double.Parse(initialCostStr, CultureInfo.InvariantCulture);

    var cashFlowsStr = new List<string>();
    var cashFlows = new double[15];
    for (int i = 0; i < 15; i++)
    {
        cashFlowsStr.Add(parts[3 + i]);
        cashFlows[i] = double.Parse(parts[3 + i], CultureInfo.InvariantCulture);
    }

    bool alignsWithStrategy = parts[18].Trim().Equals("TRUE", StringComparison.OrdinalIgnoreCase);

    // Create ProjectData
    var projectData = new ProjectData(projectId, discountRate, initialCost, cashFlows, alignsWithStrategy);

    // Build DSL formula to calculate NPV and apply strategy multiplier
    var cashFlowsArgs = string.Join(", ", cashFlows.Select(cf => cf.ToString("F2", CultureInfo.InvariantCulture)));
    double strategyMultiplier = alignsWithStrategy ? 1.0 : 0.3;
    string formulaBody = $"return CalculateNPV({discountRate.ToString("F10", CultureInfo.InvariantCulture)}, {initialCost.ToString("F2", CultureInfo.InvariantCulture)}, {cashFlowsArgs}) * {strategyMultiplier.ToString("F1", CultureInfo.InvariantCulture)}";

    // Create unique formula with global counter
    string formulaName = $"NPV_{projectId}_{FormulaIdGenerator.GetNext()}";
    var formula = new Formula(formulaName, formulaBody);

    // Parse and execute formula through Engine with CalculateNPV function
    var parseTree = FormulaParserHelper.Parse(formula);
    parsingContext.ParseTreeCache.Set(formula, parseTree);
    engine.Execute(new[] { formula }, new[] { new CalculateNPVFunc() });

    // Get result from cache
    var result = parsingContext.FormulaResultCache.Get(formulaName);
    double adjustedNpv = result.As<double>();

    var projectResult = new ProjectResult(projectId, adjustedNpv);
    return (projectData, projectResult);
}

// Global counter for unique formula IDs (must be after top-level statements)
static class FormulaIdGenerator
{
    private static long _counter = 0;
    public static long GetNext() => Interlocked.Increment(ref _counter);
}
