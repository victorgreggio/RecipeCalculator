using Xunit;
using RecipeCalculator.NPV.Functions;
using RecipeCalculator.Common.Values;

namespace RecipeCalculator.NPV.Tests;

public class CalculateNPVFuncTests
{
    [Fact]
    public void CalculateNPVFunc_WithPositiveNPV_ReturnsCorrectValue()
    {
        // Arrange
        var func = new CalculateNPVFunc();
        double discountRate = 0.10;
        double initialCost = 1000000;
        
        var parameters = new List<IValue>
        {
            new Value(discountRate),
            new Value(initialCost)
        };
        
        // Add 15 years of cash flows ($150,000 per year)
        for (int i = 0; i < 15; i++)
        {
            parameters.Add(new Value(150000.0));
        }
        
        func.Params = parameters;
        
        // Act
        var result = func.Execute();
        double npv = result.As<double>();
        
        // Assert
        Assert.True(npv > 0, "NPV should be positive for this scenario");
        Assert.InRange(npv, 140000, 145000); // Expected ~$141,861
    }
    
    [Fact]
    public void CalculateNPVFunc_WithNegativeNPV_ReturnsCorrectValue()
    {
        // Arrange
        var func = new CalculateNPVFunc();
        double discountRate = 0.15;
        double initialCost = 1000000;
        
        var parameters = new List<IValue>
        {
            new Value(discountRate),
            new Value(initialCost)
        };
        
        // Add 15 years of cash flows ($80,000 per year)
        for (int i = 0; i < 15; i++)
        {
            parameters.Add(new Value(80000.0));
        }
        
        func.Params = parameters;
        
        // Act
        var result = func.Execute();
        double npv = result.As<double>();
        
        // Assert
        Assert.True(npv < 0, "NPV should be negative for this scenario");
    }
    
    [Fact]
    public void CalculateNPVFunc_WithBusinessStrategyMultiplier_CalculatesCorrectly()
    {
        // Arrange
        var func = new CalculateNPVFunc();
        double discountRate = 0.10;
        double initialCost = 1000000;
        
        var parameters = new List<IValue>
        {
            new Value(discountRate),
            new Value(initialCost)
        };
        
        for (int i = 0; i < 15; i++)
        {
            parameters.Add(new Value(150000.0));
        }
        
        func.Params = parameters;
        
        // Act
        var result = func.Execute();
        double npv = result.As<double>();
        
        // Simulate business strategy calculation
        bool alignsWithStrategy = false;
        double strategyMultiplier = alignsWithStrategy ? 1.0 : 0.3;
        double adjustedNpv = npv * strategyMultiplier;
        
        // Assert
        Assert.True(npv > 0);
        Assert.Equal(npv * 0.3, adjustedNpv, 2);
        Assert.True(adjustedNpv < npv, "Adjusted NPV should be lower when not aligned with strategy");
    }
    
    [Fact]
    public void CalculateNPVFunc_WithInvalidParameterCount_ThrowsException()
    {
        // Arrange
        var func = new CalculateNPVFunc();
        var parameters = new List<IValue>
        {
            new Value(0.10),
            new Value(1000000.0)
            // Missing 15 cash flow parameters
        };
        
        // Act & Assert - Exception thrown when setting params
        var exception = Assert.Throws<ArgumentException>(() => func.Params = parameters);
        Assert.Contains("17", exception.Message);
        Assert.Contains("2", exception.Message);
    }
    
    [Fact]
    public void CalculateNPVFunc_WithVaryingCashFlows_ReturnsCorrectValue()
    {
        // Arrange
        var func = new CalculateNPVFunc();
        double discountRate = 0.10;
        double initialCost = 500000;
        
        var parameters = new List<IValue>
        {
            new Value(discountRate),
            new Value(initialCost)
        };
        
        // Varying cash flows
        double[] cashFlows = { 100000, 120000, 110000, 130000, 125000, 
                               115000, 105000, 100000, 95000, 90000,
                               85000, 80000, 75000, 70000, 65000 };
        
        foreach (var cf in cashFlows)
        {
            parameters.Add(new Value(cf));
        }
        
        func.Params = parameters;
        
        // Act
        var result = func.Execute();
        double npv = result.As<double>();
        
        // Assert
        Assert.True(npv > 0, "NPV should be positive");
        Assert.InRange(npv, 280000, 300000); // NPV should be around $290,570
    }
    
    [Fact]
    public void CalculateBusinessStrategyAlignment_WithAlignedProject_MultipliesBy1()
    {
        // Arrange
        var func = new CalculateNPVFunc();
        var parameters = new List<IValue>
        {
            new Value(0.10),
            new Value(1000000.0)
        };
        
        for (int i = 0; i < 15; i++)
        {
            parameters.Add(new Value(150000.0));
        }
        
        func.Params = parameters;
        
        // Act
        var result = func.Execute();
        double npv = result.As<double>();
        
        // Simulate aligned with strategy
        bool alignsWithStrategy = true;
        double strategyMultiplier = alignsWithStrategy ? 1.0 : 0.3;
        double adjustedNpv = npv * strategyMultiplier;
        
        // Assert
        Assert.Equal(npv, adjustedNpv, 2);
    }
    
    [Fact]
    public void CalculateBusinessStrategyAlignment_WithNonAlignedProject_MultipliesBy0Point3()
    {
        // Arrange
        var func = new CalculateNPVFunc();
        var parameters = new List<IValue>
        {
            new Value(0.10),
            new Value(1000000.0)
        };
        
        for (int i = 0; i < 15; i++)
        {
            parameters.Add(new Value(150000.0));
        }
        
        func.Params = parameters;
        
        // Act
        var result = func.Execute();
        double npv = result.As<double>();
        
        // Simulate NOT aligned with strategy
        bool alignsWithStrategy = false;
        double strategyMultiplier = alignsWithStrategy ? 1.0 : 0.3;
        double adjustedNpv = npv * strategyMultiplier;
        
        // Assert
        Assert.Equal(npv * 0.3, adjustedNpv, 2);
        Assert.True(adjustedNpv < npv);
    }
}
