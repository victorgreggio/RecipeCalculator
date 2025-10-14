using RecipeCalculator.Common.Function;
using RecipeCalculator.Common.Values;

namespace RecipeCalculator.NPV.Functions;

/// <summary>
/// Custom function that calculates Net Present Value (NPV) for a project.
/// Parameters:
/// - Param 0: Discount Rate (double)
/// - Param 1: Initial Cost (double)
/// - Param 2-16: Cash flows for years 1-15 (double each)
/// Total: 17 parameters
/// </summary>
public class CalculateNPVFunc : BaseFunction
{
    public CalculateNPVFunc() : base("CalculateNPV", 17)
    {
    }

    public override IValue Execute()
    {
        // Validate parameter count
        if (Params.Count() != 17)
        {
            throw new ArgumentException($"CalculateNPVFunc expects exactly 17 parameters (discount rate, initial cost, and 15 cash flows), but got {Params.Count()}");
        }

        // Validate all parameters are numbers
        if (Params.Any(x => !x.Is<double>()))
        {
            throw new ArgumentException("CalculateNPVFunc expects all parameters to be numbers.");
        }

        var paramList = Params.ToList();
        
        // Extract parameters
        double discountRate = paramList[0].As<double>();
        double initialCost = paramList[1].As<double>();
        
        // Extract cash flows (years 1-15)
        double[] cashFlows = new double[15];
        for (int i = 0; i < 15; i++)
        {
            cashFlows[i] = paramList[2 + i].As<double>();
        }
        
        // Calculate NPV
        double npv = CalculateNPV(discountRate, initialCost, cashFlows);
        
        return new Value(npv);
    }

    private static double CalculateNPV(double discountRate, double initialCost, double[] cashFlows)
    {
        double npv = -initialCost;
        
        for (int year = 0; year < cashFlows.Length; year++)
        {
            npv += cashFlows[year] / Math.Pow(1 + discountRate, year + 1);
        }
        
        return npv;
    }
}
