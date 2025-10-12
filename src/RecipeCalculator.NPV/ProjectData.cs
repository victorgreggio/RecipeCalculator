namespace RecipeCalculator.NPV;

public readonly struct ProjectData
{
    public string ProjectId { get; init; }
    public double DiscountRate { get; init; }
    public double InitialCost { get; init; }
    public double[] CashFlows { get; init; }
    public bool AlignsWithBusinessStrategy { get; init; }
    
    public ProjectData(string projectId, double discountRate, double initialCost, double[] cashFlows, bool alignsWithBusinessStrategy)
    {
        ProjectId = projectId;
        DiscountRate = discountRate;
        InitialCost = initialCost;
        CashFlows = cashFlows;
        AlignsWithBusinessStrategy = alignsWithBusinessStrategy;
    }
}

public readonly struct ProjectResult
{
    public string ProjectId { get; init; }
    public double NPV { get; init; }
    public double AdjustedNPV { get; init; }
    
    public ProjectResult(string projectId, double npv, double adjustedNpv)
    {
        ProjectId = projectId;
        NPV = npv;
        AdjustedNPV = adjustedNpv;
    }
}
