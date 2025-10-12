namespace RecipeCalculator.NPV;

public static class CalculateNPV
{
    /// <summary>
    /// Calculates the Net Present Value (NPV) for a project
    /// </summary>
    /// <param name="discountRate">The discount rate (e.g., 0.1 for 10%)</param>
    /// <param name="initialCost">The initial investment cost (negative value)</param>
    /// <param name="cashFlows">Array of cash flows for each year (15 years)</param>
    /// <returns>The Net Present Value</returns>
    public static double Calculate(double discountRate, double initialCost, double[] cashFlows)
    {
        double npv = -initialCost;
        
        for (int year = 0; year < cashFlows.Length; year++)
        {
            npv += cashFlows[year] / Math.Pow(1 + discountRate, year + 1);
        }
        
        return npv;
    }
    
    /// <summary>
    /// Calculates NPV for a project with uniform annual cash flows
    /// </summary>
    /// <param name="discountRate">The discount rate (e.g., 0.1 for 10%)</param>
    /// <param name="initialCost">The initial investment cost (negative value)</param>
    /// <param name="annualCashFlow">Annual cash flow (same for all years)</param>
    /// <param name="years">Number of years (default 15)</param>
    /// <returns>The Net Present Value</returns>
    public static double CalculateUniform(double discountRate, double initialCost, double annualCashFlow, int years = 15)
    {
        double npv = -initialCost;
        
        // Use present value of annuity formula for optimization
        if (Math.Abs(discountRate) < 0.0000001)
        {
            // If discount rate is ~0, NPV is simply sum of cash flows minus initial cost
            npv += annualCashFlow * years;
        }
        else
        {
            // Present value of annuity formula: PMT * [(1 - (1 + r)^-n) / r]
            double pvFactor = (1 - Math.Pow(1 + discountRate, -years)) / discountRate;
            npv += annualCashFlow * pvFactor;
        }
        
        return npv;
    }
}
