# NPV Calculator Console Application

A high-performance console application for calculating Net Present Value (NPV) for large project portfolios.

## Features

- **Optimized for Large Files**: Efficiently processes millions of project entries
- **Parallel Processing**: Uses all available CPU cores for maximum throughput
- **Memory-Mapped Files**: Automatically switches to memory-mapped file handling for files > 100 MB
- **Progress Reporting**: Shows real-time progress during processing

## Input File Format

The input file should be a CSV file with the following format:

```csv
ProjectId,DiscountRate,InitialCost,Year1Income,Year2Income,Year3Income,...,Year15Income
PROJ001,0.10,1000000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000
PROJ002,0.08,2500000,350000,350000,350000,350000,350000,350000,350000,350000,350000,350000,350000,350000,350000,350000,350000
```

### Fields:
- **ProjectId**: Unique identifier for the project
- **DiscountRate**: Discount rate as decimal (e.g., 0.10 for 10%)
- **InitialCost**: Initial investment cost (positive value, will be treated as negative in NPV calculation)
- **Year1Income through Year15Income**: Annual income/cash flow for each of the 15 years

## Usage

```bash
# Process a file
dotnet run --project src/RecipeCalculator.NPV/RecipeCalculator.NPV.csproj -- input.csv

# Specify custom output file
dotnet run --project src/RecipeCalculator.NPV/RecipeCalculator.NPV.csproj -- input.csv output.csv
```

Or after building:

```bash
./RecipeCalculator.NPV input.csv [output.csv]
```

## Output Format

The application generates a CSV file with NPV results:

```csv
ProjectId,NPV
PROJ001,135450.23
PROJ002,1250789.45
```

## NPV Calculation

The Net Present Value is calculated using the formula:

```
NPV = -InitialCost + Σ(CashFlow_t / (1 + DiscountRate)^t)
```

Where:
- t = year (1 to 15)
- CashFlow_t = income for year t
- DiscountRate = discount rate (as decimal)

## Performance

The application is optimized for processing millions of entries:

- **Parallel Processing**: Utilizes all CPU cores
- **Chunked Processing**: For very large files, processes data in chunks to manage memory
- **Efficient I/O**: Uses buffered streams and async operations
- **Memory-Mapped Files**: For files > 100 MB

### Example Performance:
- 1 million projects: ~10-15 seconds (depends on CPU)
- 10 million projects: ~2-3 minutes

## Building

```bash
dotnet build src/RecipeCalculator.NPV/RecipeCalculator.NPV.csproj
```

## Testing

Generate a sample file for testing:

```csharp
// Add this to Program.cs temporarily or create a separate tool
if (args[0] == "--generate-sample")
{
    int count = args.Length > 1 ? int.Parse(args[1]) : 1000;
    string outputPath = args.Length > 2 ? args[2] : "sample_projects.csv";
    SampleFileGenerator.GenerateSampleFile(outputPath, count);
    return 0;
}
```

Then:
```bash
dotnet run -- --generate-sample 1000000 sample_projects.csv
dotnet run -- sample_projects.csv results.csv
```
