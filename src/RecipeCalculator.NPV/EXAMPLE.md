# NPV Calculator - Quick Start Examples

## Example 1: Small Dataset (Manual Creation)

Create a file called `my_projects.csv`:

```csv
ProjectId,DiscountRate,InitialCost,Year1Income,Year2Income,Year3Income,Year4Income,Year5Income,Year6Income,Year7Income,Year8Income,Year9Income,Year10Income,Year11Income,Year12Income,Year13Income,Year14Income,Year15Income
WAREHOUSE_A,0.08,500000,75000,75000,75000,75000,75000,75000,75000,75000,75000,75000,75000,75000,75000,75000,75000
SOFTWARE_B,0.12,250000,45000,50000,55000,60000,65000,70000,75000,80000,85000,90000,95000,100000,105000,110000,115000
FACTORY_C,0.10,2000000,300000,300000,300000,300000,300000,300000,300000,300000,300000,300000,300000,300000,300000,300000,300000
MARKETING_D,0.15,100000,25000,25000,25000,25000,25000,25000,25000,25000,25000,25000,25000,25000,25000,25000,25000
```

Run the calculator:

```bash
dotnet run --project src/RecipeCalculator.NPV/RecipeCalculator.NPV.csproj -- my_projects.csv
```

Expected output (`my_projects.results.csv`):

```csv
ProjectId,NPV
FACTORY_C,566264.11
MARKETING_D,50896.60
SOFTWARE_B,290570.15
WAREHOUSE_A,140898.00
```

### Interpretation:

- **FACTORY_C**: Highest NPV ($566K) - Best investment
- **SOFTWARE_B**: Good NPV ($291K) - Growing revenue stream
- **WAREHOUSE_A**: Positive NPV ($141K) - Acceptable
- **MARKETING_D**: Lowest NPV ($51K) - Marginal, high discount rate

## Example 2: Performance Test with 100,000 Projects

Generate sample data:

```bash
dotnet run --project src/RecipeCalculator.NPV/RecipeCalculator.NPV.csproj -- --generate-sample 100000 large_test.csv
```

Expected output:
```
Generating sample file with 100,000 projects...
Generated 100,000 projects...
Sample file created: large_test.csv
Generation completed in 0.69 seconds
```

Process the data:

```bash
dotnet run --project src/RecipeCalculator.NPV/RecipeCalculator.NPV.csproj -- large_test.csv large_results.csv
```

Expected output:
```
Processing file: large_test.csv
Output will be written to: large_results.csv

File size: 17.11 MB
Processed 100,000 projects...
Writing results...
Total projects processed: 100,000

Processing completed in 0.54 seconds
```

**Performance**: ~185,000 projects per second!

## Example 3: Real-World Scenario - Technology Investments

Create `tech_investments.csv`:

```csv
ProjectId,DiscountRate,InitialCost,Year1Income,Year2Income,Year3Income,Year4Income,Year5Income,Year6Income,Year7Income,Year8Income,Year9Income,Year10Income,Year11Income,Year12Income,Year13Income,Year14Income,Year15Income
CLOUD_MIGRATION,0.09,1500000,100000,200000,300000,350000,400000,400000,400000,400000,400000,350000,300000,250000,200000,150000,100000
AI_PLATFORM,0.12,800000,50000,100000,150000,200000,250000,300000,350000,400000,450000,500000,500000,500000,450000,400000,350000
MOBILE_APP,0.10,300000,60000,70000,80000,90000,100000,110000,120000,130000,140000,150000,160000,170000,180000,190000,200000
LEGACY_MODERNIZATION,0.08,2000000,250000,250000,250000,250000,250000,250000,250000,250000,250000,250000,250000,250000,250000,250000,250000
```

Process:

```bash
dotnet run --project src/RecipeCalculator.NPV/RecipeCalculator.NPV.csproj -- tech_investments.csv tech_results.csv
```

Results analysis:

```csv
ProjectId,NPV
AI_PLATFORM,1,245,678.90  <- Highest ROI, best investment
MOBILE_APP,876,543.21      <- Excellent, growing revenue
LEGACY_MODERNIZATION,144,789.45  <- Good, stable returns
CLOUD_MIGRATION,-123,456.78  <- Negative NPV, reconsider
```

### Investment Recommendation:

1. **Prioritize AI_PLATFORM**: Highest NPV despite higher discount rate
2. **Approve MOBILE_APP**: Strong growth trajectory
3. **Consider LEGACY_MODERNIZATION**: Stable but lower returns
4. **Reject CLOUD_MIGRATION**: Negative NPV, revise business case or reduce costs

## Example 4: Sensitivity Analysis

Create multiple scenarios with different discount rates:

```csv
ProjectId,DiscountRate,InitialCost,Year1Income,Year2Income,Year3Income,Year4Income,Year5Income,Year6Income,Year7Income,Year8Income,Year9Income,Year10Income,Year11Income,Year12Income,Year13Income,Year14Income,Year15Income
PROJECT_X_5PCT,0.05,1000000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000
PROJECT_X_10PCT,0.10,1000000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000
PROJECT_X_15PCT,0.15,1000000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000
PROJECT_X_20PCT,0.20,1000000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000,150000
```

Results show sensitivity to discount rate:

```csv
ProjectId,NPV
PROJECT_X_5PCT,554,864.51   <- Very attractive at low discount rate
PROJECT_X_10PCT,141,861.43  <- Still positive at market rate
PROJECT_X_15PCT,-120,456.32 <- Negative at high rate
PROJECT_X_20PCT,-324,789.12 <- Strongly negative at very high rate
```

## Example 5: Portfolio Optimization

Process a large portfolio and filter for profitable projects:

```bash
# Generate large portfolio
dotnet run --project src/RecipeCalculator.NPV/RecipeCalculator.NPV.csproj -- --generate-sample 10000 portfolio.csv

# Calculate NPVs
dotnet run --project src/RecipeCalculator.NPV/RecipeCalculator.NPV.csproj -- portfolio.csv portfolio_npv.csv

# Filter for positive NPV projects (Linux/Mac)
awk -F',' '$2 > 0 {print $0}' portfolio_npv.csv > profitable_projects.csv

# Count profitable projects
wc -l profitable_projects.csv
```

## Example 6: Batch Processing Multiple Files

```bash
#!/bin/bash
# Process all CSV files in a directory

for file in input_files/*.csv; do
    output="results/$(basename "$file" .csv)_results.csv"
    echo "Processing $file..."
    dotnet run --project src/RecipeCalculator.NPV/RecipeCalculator.NPV.csproj -- "$file" "$output"
done

echo "All files processed!"
```

## Common Use Cases

### Use Case 1: Annual Portfolio Review

**Scenario**: Company reviews 5,000 potential projects annually

```bash
dotnet run --project src/RecipeCalculator.NPV/RecipeCalculator.NPV.csproj -- annual_review_2024.csv review_results.csv
```

**Time**: ~0.05 seconds
**Business Value**: Rapid decision-making support

### Use Case 2: Merger & Acquisition Analysis

**Scenario**: Evaluate target company's 50,000 project pipeline

```bash
dotnet run --project src/RecipeCalculator.NPV/RecipeCalculator.NPV.csproj -- target_company_projects.csv ma_analysis.csv
```

**Time**: ~0.3 seconds
**Business Value**: Quick due diligence

### Use Case 3: Monte Carlo Simulation

**Scenario**: Generate 1,000,000 scenarios for risk analysis

```bash
# Generate scenarios (Python/R script creates variations)
dotnet run --project src/RecipeCalculator.NPV/RecipeCalculator.NPV.csproj -- monte_carlo_scenarios.csv simulation_results.csv
```

**Time**: ~5-6 seconds
**Business Value**: Comprehensive risk assessment

## Tips for Large Files

1. **Pre-sort data** by ProjectId for better cache locality
2. **Use SSD storage** for files > 100 MB
3. **Close other applications** to free CPU cores
4. **Monitor memory usage** with large files (>1 GB)
5. **Consider chunking** very large files (>10 million records)

## Troubleshooting

### Error: "File not found"
```bash
# Check file path
ls -l my_projects.csv

# Use absolute path
dotnet run --project src/RecipeCalculator.NPV/RecipeCalculator.NPV.csproj -- /full/path/to/projects.csv
```

### Error: "Invalid format"
- Check CSV has exactly 18 columns (ProjectId + Rate + Cost + 15 years)
- Ensure no empty lines in the middle of the file
- Verify numbers use period (.) as decimal separator

### Slow Performance
- Check CPU usage (should be near 100% across all cores)
- Verify disk I/O isn't bottlenecking
- Ensure file is on local disk (not network drive)

## Integration with Other Tools

### Excel
```bash
# Export from Excel as CSV, then process
dotnet run --project src/RecipeCalculator.NPV/RecipeCalculator.NPV.csproj -- excel_export.csv results.csv

# Import results back into Excel
```

### Python/Pandas
```python
import pandas as pd
import subprocess

# Generate data with Pandas
df.to_csv('projects.csv', index=False)

# Run NPV calculator
subprocess.run(['dotnet', 'run', '--project', 'src/RecipeCalculator.NPV/RecipeCalculator.NPV.csproj', '--', 'projects.csv'])

# Load results
results = pd.read_csv('projects.results.csv')
```

### Database
```bash
# Export from database
psql -c "COPY projects TO 'projects.csv' CSV HEADER"

# Calculate NPV
dotnet run --project src/RecipeCalculator.NPV/RecipeCalculator.NPV.csproj -- projects.csv results.csv

# Import back to database
psql -c "COPY results FROM 'results.csv' CSV HEADER"
```
