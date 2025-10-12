namespace RecipeCalculator.NPV;

public static class SampleFileGenerator
{
    public static void GenerateSampleFile(string outputPath, int numberOfProjects)
    {
        Console.WriteLine($"Generating sample file with {numberOfProjects:N0} projects...");
        
        var random = new Random(42);
        
        using var stream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None, 65536);
        using var writer = new StreamWriter(stream);
        
        // Write header with Business Strategy flag
        writer.WriteLine("ProjectId,DiscountRate,InitialCost,Year1Income,Year2Income,Year3Income,Year4Income,Year5Income,Year6Income,Year7Income,Year8Income,Year9Income,Year10Income,Year11Income,Year12Income,Year13Income,Year14Income,Year15Income,AlignsWithBusinessStrategyABC");
        
        for (int i = 1; i <= numberOfProjects; i++)
        {
            string projectId = $"PROJ{i:D7}";
            double discountRate = Math.Round(random.NextDouble() * 0.15 + 0.05, 4); // 5% to 20%
            double initialCost = Math.Round(random.NextDouble() * 5000000 + 500000, 2); // $500k to $5.5M
            
            var cashFlows = new double[15];
            double baseIncome = initialCost * 0.15; // ~15% of initial cost per year
            
            for (int year = 0; year < 15; year++)
            {
                // Add some variation: +/- 20%
                double variation = 1 + (random.NextDouble() * 0.4 - 0.2);
                cashFlows[year] = Math.Round(baseIncome * variation, 2);
            }
            
            // 60% of projects align with business strategy
            bool alignsWithStrategy = random.NextDouble() < 0.6;
            
            writer.Write(projectId);
            writer.Write(',');
            writer.Write(discountRate.ToString("F4"));
            writer.Write(',');
            writer.Write(initialCost.ToString("F2"));
            
            for (int year = 0; year < 15; year++)
            {
                writer.Write(',');
                writer.Write(cashFlows[year].ToString("F2"));
            }
            
            writer.Write(',');
            writer.Write(alignsWithStrategy ? "TRUE" : "FALSE");
            writer.WriteLine();
            
            if (i % 100000 == 0)
            {
                Console.WriteLine($"Generated {i:N0} projects...");
            }
        }
        
        Console.WriteLine($"Sample file created: {outputPath}");
    }
}
