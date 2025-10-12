# NPV Calculator - Project Portfolio Analysis Tool

## Overview

A high-performance console application designed to calculate Net Present Value (NPV) for large project portfolios. This tool can efficiently process millions of project entries, making it ideal for enterprise-level portfolio analysis. 

**Key Feature**: The application includes business strategy alignment scoring where projects that align with "Business Strategy ABC" receive full NPV value, while non-aligned projects have their NPV multiplied by 0.3.

## Architecture

### Components

1. **CalculateNPVFunc.cs** - Custom function extending BaseFunction from RecipeCalculator.Engine
   - Integrates with the RecipeCalculator formula engine
   - Takes 17 parameters: discount rate, initial cost, and 15 years of cash flows
   - Returns calculated NPV as IValue

2. **ProjectData.cs** - Data structures
   - `ProjectData` - Input data structure including BusinessStrategy flag
   - `ProjectResult` - Output structure with NPV and AdjustedNPV

3. **Program.cs** - Main console application
   - Uses CalculateNPVFunc (extends BaseFunction)
   - Applies business strategy multiplier formula
   - Parallel processing using all CPU cores
   - Memory-mapped file support for files > 100 MB
   - Chunked processing for very large datasets

4. **SampleFileGenerator.cs** - Test data generator
   - Creates realistic sample data with strategy alignment flags
   - 60% of generated projects align with business strategy

## Business Strategy Alignment

The calculator implements a strategic project prioritization model:

**Formula**: `AdjustedNPV = NPV * (AlignsWithBusinessStrategyABC ? 1.0 : 0.3)`

- **Aligned projects** (TRUE): Full NPV value (multiplied by 1.0)
- **Non-aligned projects** (FALSE): Discounted NPV (multiplied by 0.3)

This allows organizations to factor strategic fit into financial analysis, effectively reducing the value of projects that don't support core business objectives.

## Features

### Performance Optimizations

1. **Parallel Processing**
   - Uses `Parallel.ForEachAsync` to utilize all CPU cores
   - Concurrent data structures for thread-safe operations
   - Interlocked operations for atomic counters

2. **Memory Management**
   - Automatic detection of large files (> 100 MB)
   - Memory-mapped files for large datasets
   - Chunked processing (1 million records per chunk)
   - Buffered I/O streams (64KB buffers)

3. **Efficient Algorithms**
   - Present Value of Annuity formula for uniform cash flows
   - Minimized memory allocations
   - Struct-based data types for value semantics

### Scalability

The application is designed to handle:
- Small files (< 1 MB): Direct in-memory processing
- Medium files (1-100 MB): Parallel processing with standard file I/O
- Large files (> 100 MB): Memory-mapped files with chunked processing

**Performance Benchmarks:**
- 1,000 projects: ~0.03 seconds
- 100,000 projects: ~0.54 seconds
- 1,000,000 projects: ~5-6 seconds (estimated)
- 10,000,000 projects: ~1-2 minutes (estimated)

*Note: Performance varies based on CPU cores and disk speed*

## Input File Format

CSV file with the following structure:

```csv
ProjectId,DiscountRate,InitialCost,Year1Income,Year2Income,...,Year15Income,AlignsWithBusinessStrategyABC
PROJ001,0.10,1000000,150000,150000,...,150000,TRUE
PROJ002,0.12,500000,80000,80000,...,80000,FALSE
```

### Field Descriptions

- **ProjectId**: Unique identifier (string)
- **DiscountRate**: Discount rate as decimal (e.g., 0.10 = 10%)
- **InitialCost**: Initial investment (positive value, treated as negative in calculation)
- **Year1Income through Year15Income**: Annual cash flows for 15 years
- **AlignsWithBusinessStrategyABC**: TRUE if project aligns with strategy, FALSE otherwise

## Output File Format

CSV file with NPV results including strategy-adjusted values:

```csv
ProjectId,NPV,AdjustedNPV,AlignsWithBusinessStrategyABC
PROJ001,141861.43,141861.43,TRUE
PROJ002,45000.00,13500.00,FALSE
```

- **NPV**: Raw Net Present Value
- **AdjustedNPV**: NPV adjusted for business strategy alignment
- **AlignsWithBusinessStrategyABC**: Strategy alignment flag for reference

Results are sorted by ProjectId for easy lookup.

## Usage

### Basic Usage

```bash
# Process an input file (output defaults to input_file.results.csv)
dotnet run --project src/RecipeCalculator.NPV/RecipeCalculator.NPV.csproj -- projects.csv

# Specify custom output file
dotnet run --project src/RecipeCalculator.NPV/RecipeCalculator.NPV.csproj -- projects.csv results.csv
```

### Generate Sample Data

```bash
# Generate 1,000 sample projects
dotnet run --project src/RecipeCalculator.NPV/RecipeCalculator.NPV.csproj -- --generate-sample 1000 sample.csv

# Generate 1 million sample projects for performance testing
dotnet run --project src/RecipeCalculator.NPV/RecipeCalculator.NPV.csproj -- --generate-sample 1000000 large_sample.csv
```

### Build and Run

```bash
# Build the project
dotnet build src/RecipeCalculator.NPV/RecipeCalculator.NPV.csproj

# Run tests
dotnet test test/RecipeCalculator.NPV.Tests/RecipeCalculator.NPV.Tests.csproj

# Create release build
dotnet publish src/RecipeCalculator.NPV/RecipeCalculator.NPV.csproj -c Release -o ./publish/npv

# Run published version
./publish/npv/RecipeCalculator.NPV projects.csv results.csv
```

## NPV Calculation Formula

### Standard Calculation

```
NPV = -InitialCost + Σ(CashFlow_t / (1 + r)^t)
```

Where:
- t = year (1 to 15)
- CashFlow_t = income for year t
- r = discount rate

### Uniform Cash Flow Optimization

For uniform cash flows, the application uses the Present Value of Annuity formula:

```
NPV = -InitialCost + (PMT × [(1 - (1 + r)^-n) / r])
```

Where:
- PMT = annual payment (uniform cash flow)
- r = discount rate
- n = number of years

This optimization significantly improves performance when all cash flows are identical.

## Example Scenarios

### Positive NPV Project

```csv
PROJ001,0.10,1000000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000
```

With a 10% discount rate and $150K annual income, this project has a positive NPV of approximately $141,861.

### Negative NPV Project

```csv
PROJ002,0.15,1000000,80000,80000,80000,80000,80000,80000,80000,80000,80000,80000,80000,80000,80000,80000,80000
```

With a 15% discount rate and $80K annual income, this project has a negative NPV and should be rejected.

## Error Handling

The application handles:
- Invalid file formats (reports error count)
- Missing fields (skips malformed lines)
- Non-numeric values (skips malformed lines)
- File not found (error message and exit code 1)

Errors are counted and reported in the summary, but don't stop processing of other records.

## Testing

Comprehensive unit tests cover:
- Positive and negative NPV scenarios
- Zero discount rate edge cases
- Varying cash flows
- Uniform vs. standard calculation equivalence
- High discount rate scenarios
- Large value handling

Run tests:
```bash
dotnet test test/RecipeCalculator.NPV.Tests/RecipeCalculator.NPV.Tests.csproj
```

## Integration with RecipeCalculator

This NPV calculator is part of the RecipeCalculator solution and follows the same architectural patterns:
- Separation of concerns (calculation logic, data structures, UI)
- Comprehensive testing
- Performance optimization
- Clean code principles

## Future Enhancements

Potential improvements:
1. Support for different time periods (not just 15 years)
2. Multiple discount rate scenarios
3. Sensitivity analysis
4. Risk-adjusted NPV calculations
5. Excel file support (XLSX)
6. Database integration
7. REST API for cloud deployment
8. Real-time streaming for very large files

## Dependencies

- .NET 9.0
- System.Buffers (built-in)
- System.IO.MemoryMappedFiles (built-in)
- xUnit (testing only)

No external NuGet packages required.

## License

Part of the RecipeCalculator project.
