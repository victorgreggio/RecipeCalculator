using Xunit;
using RecipeCalculator.NPV;

namespace RecipeCalculator.NPV.Tests;

public class CalculateNPVTests
{
    [Fact]
    public void Calculate_WithPositiveNPV_ReturnsCorrectValue()
    {
        // Arrange
        double discountRate = 0.10;
        double initialCost = 1000000;
        double[] cashFlows = new double[15];
        for (int i = 0; i < 15; i++)
        {
            cashFlows[i] = 150000; // $150,000 per year
        }
        
        // Act
        double npv = CalculateNPV.Calculate(discountRate, initialCost, cashFlows);
        
        // Assert
        Assert.True(npv > 0, "NPV should be positive for this scenario");
        Assert.InRange(npv, 140000, 145000); // Expected ~$141,861
    }
    
    [Fact]
    public void Calculate_WithNegativeNPV_ReturnsCorrectValue()
    {
        // Arrange
        double discountRate = 0.15;
        double initialCost = 1000000;
        double[] cashFlows = new double[15];
        for (int i = 0; i < 15; i++)
        {
            cashFlows[i] = 80000; // $80,000 per year
        }
        
        // Act
        double npv = CalculateNPV.Calculate(discountRate, initialCost, cashFlows);
        
        // Assert
        Assert.True(npv < 0, "NPV should be negative for this scenario");
    }
    
    [Fact]
    public void Calculate_WithZeroDiscountRate_ReturnsSimpleSum()
    {
        // Arrange
        double discountRate = 0.0;
        double initialCost = 1000000;
        double[] cashFlows = new double[15];
        for (int i = 0; i < 15; i++)
        {
            cashFlows[i] = 100000;
        }
        
        // Act
        double npv = CalculateNPV.Calculate(discountRate, initialCost, cashFlows);
        
        // Assert
        double expected = -initialCost + (100000 * 15);
        Assert.Equal(expected, npv, 2);
    }
    
    [Fact]
    public void Calculate_WithVaryingCashFlows_ReturnsCorrectValue()
    {
        // Arrange
        double discountRate = 0.10;
        double initialCost = 500000;
        double[] cashFlows = { 100000, 120000, 110000, 130000, 125000, 
                               115000, 105000, 100000, 95000, 90000,
                               85000, 80000, 75000, 70000, 65000 };
        
        // Act
        double npv = CalculateNPV.Calculate(discountRate, initialCost, cashFlows);
        
        // Assert
        Assert.True(npv > 0, "NPV should be positive");
        // NPV should be around $290,570
        Assert.InRange(npv, 280000, 300000);
    }
    
    [Fact]
    public void CalculateUniform_WithStandardInputs_ReturnsCorrectValue()
    {
        // Arrange
        double discountRate = 0.10;
        double initialCost = 1000000;
        double annualCashFlow = 150000;
        
        // Act
        double npvUniform = CalculateNPV.CalculateUniform(discountRate, initialCost, annualCashFlow);
        
        // Also calculate using the standard method for comparison
        double[] cashFlows = new double[15];
        for (int i = 0; i < 15; i++)
        {
            cashFlows[i] = annualCashFlow;
        }
        double npvStandard = CalculateNPV.Calculate(discountRate, initialCost, cashFlows);
        
        // Assert
        Assert.Equal(npvStandard, npvUniform, 2);
    }
    
    [Fact]
    public void CalculateUniform_WithZeroDiscountRate_ReturnsSimpleSum()
    {
        // Arrange
        double discountRate = 0.0;
        double initialCost = 1000000;
        double annualCashFlow = 100000;
        int years = 15;
        
        // Act
        double npv = CalculateNPV.CalculateUniform(discountRate, initialCost, annualCashFlow, years);
        
        // Assert
        double expected = -initialCost + (annualCashFlow * years);
        Assert.Equal(expected, npv, 2);
    }
    
    [Fact]
    public void CalculateUniform_WithHighDiscountRate_ReturnsLowerNPV()
    {
        // Arrange
        double lowRate = 0.05;
        double highRate = 0.15;
        double initialCost = 1000000;
        double annualCashFlow = 150000;
        
        // Act
        double npvLowRate = CalculateNPV.CalculateUniform(lowRate, initialCost, annualCashFlow);
        double npvHighRate = CalculateNPV.CalculateUniform(highRate, initialCost, annualCashFlow);
        
        // Assert
        Assert.True(npvLowRate > npvHighRate, "NPV with lower discount rate should be higher");
    }
    
    [Fact]
    public void Calculate_WithLargeCashFlows_HandlesCorrectly()
    {
        // Arrange
        double discountRate = 0.12;
        double initialCost = 10000000; // $10M
        double[] cashFlows = new double[15];
        for (int i = 0; i < 15; i++)
        {
            cashFlows[i] = 1500000; // $1.5M per year
        }
        
        // Act
        double npv = CalculateNPV.Calculate(discountRate, initialCost, cashFlows);
        
        // Assert
        Assert.True(npv > 0, "NPV should be positive");
        Assert.InRange(npv, 200000, 300000);
    }
}
