# Formula Dependencies Feature

## Overview

The UI now executes ALL saved formulas together when you click Execute, allowing formulas to reference each other using the `GetOutputFrom()` function.

## How It Works

When you click **▶️ Execute**:

1. The system loads all saved formulas from browser storage
2. Includes the current formula being edited (if not already saved)
3. Parses all formulas
4. Sends them all to the Engine together
5. The Engine resolves dependencies automatically using topological sorting
6. Returns the result for the current formula

## Using Dependencies

### Example 1: Basic Dependency

**Formula 1: Base1**
```
return 10
```

**Formula 2: Base2**
```
return 20
```

**Formula 3: Sum**
```
return GetOutputFrom('Base1') + GetOutputFrom('Base2')
```

**Result:** `30`

### Example 2: Chain Dependencies

**Formula 1: Price**
```
return 100
```

**Formula 2: Tax**
```
return GetOutputFrom('Price') * 0.1
```

**Formula 3: Total**
```
return GetOutputFrom('Price') + GetOutputFrom('Tax')
```

**Result:** `110` (100 + 10% tax)

### Example 3: Complex Dependencies

**Formula 1: Quantity**
```
return 50
```

**Formula 2: UnitPrice**
```
return 25
```

**Formula 3: Subtotal**
```
return GetOutputFrom('Quantity') * GetOutputFrom('UnitPrice')
```

**Formula 4: Discount**
```
if (GetOutputFrom('Quantity') > 40) then
  return GetOutputFrom('Subtotal') * 0.1
else
  return 0
end
```

**Formula 5: GrandTotal**
```
return GetOutputFrom('Subtotal') - GetOutputFrom('Discount')
```

**Result:** `1125` (1250 - 10% discount)

## Workflow

1. **Create Base Formulas**
   - Click ➕ to create a new formula
   - Name it (e.g., "Price")
   - Write the code: `return 100`
   - Click 💾 Save Formula

2. **Create Dependent Formulas**
   - Create another formula (e.g., "Tax")
   - Use `GetOutputFrom('Price')` to reference the first formula
   - Write: `return GetOutputFrom('Price') * 0.1`
   - Click 💾 Save Formula

3. **Execute**
   - Select any formula from the list (or create a new one)
   - Click ▶️ Execute
   - ALL saved formulas execute together
   - The result shows for the selected formula

## Dependency Resolution

The Engine automatically:
- **Detects Dependencies**: Analyzes which formulas reference others
- **Resolves Order**: Uses topological sorting to determine execution order
- **Parallel Execution**: Runs independent formulas in parallel
- **Error Detection**: Identifies circular dependencies and missing formulas

## Error Handling

### Missing Formula
If you reference a formula that doesn't exist:
```
return GetOutputFrom('NonExistent')
```
**Error:** "Could not resolve dependency path for formula"

### Circular Dependency
If Formula A depends on Formula B, and Formula B depends on Formula A:
```
// Formula A
return GetOutputFrom('B') + 1

// Formula B
return GetOutputFrom('A') + 1
```
**Error:** Circular dependency detected

## UI Features

### Dependencies Example Button
- Click the green "Dependencies" button in Quick Examples
- Loads a template showing how to use `GetOutputFrom()`
- Instructions on creating prerequisite formulas

### Syntax Guide
The syntax guide now includes:
```
// Reference other formulas
return GetOutputFrom('FormulaName')
```

### Formula Execution Message
When successful, the result shows:
```
Formula: YourFormulaName
Result: 42
Type: Double
```

## Benefits

1. **Modular Formulas**: Break complex calculations into smaller, reusable pieces
2. **Maintainability**: Update one formula, all dependent formulas use the new value
3. **Testability**: Test individual formulas independently
4. **Reusability**: Use the same base formulas in multiple calculations
5. **Organization**: Organize related calculations into a formula library

## Technical Details

### Execution Flow

```csharp
// 1. Load all saved formulas
var allFormulas = await StorageService.GetAllFormulasAsync();

// 2. Add current formula being edited
if (not in saved list) {
    add current formula
}

// 3. Parse all formulas
foreach (formula in allFormulas) {
    parseTree = Parse(formula);
    cache parseTree
}

// 4. Execute all together
EngineRunner.Execute(allFormulas);

// 5. Get result for current formula
result = FormulaResultCache.Get(currentFormulaName);
```

### Dependency Graph Example

```
Price (100)
  ├─> Tax (10)
  └─> Subtotal (100)
       └─> Total (110)
```

The Engine executes in order:
1. Price (no dependencies)
2. Tax and Subtotal (depend on Price, can run in parallel)
3. Total (depends on Subtotal)

## Best Practices

1. **Meaningful Names**: Use descriptive formula names for clarity
2. **Single Responsibility**: Each formula should do one thing
3. **Document Dependencies**: Use comments to note which formulas are needed
4. **Test Incrementally**: Test base formulas before dependent ones
5. **Save Frequently**: Save formulas as you build them

## Example Use Cases

### Invoice Calculation
- BasePrice
- TaxRate
- Tax (uses BasePrice, TaxRate)
- ShippingCost
- Total (uses BasePrice, Tax, ShippingCost)

### Loan Calculator
- Principal
- InterestRate
- Term
- MonthlyPayment (uses Principal, InterestRate, Term)
- TotalPayment (uses MonthlyPayment, Term)
- TotalInterest (uses TotalPayment, Principal)

### Discount System
- OriginalPrice
- CustomerType
- DiscountRate (uses CustomerType)
- DiscountAmount (uses OriginalPrice, DiscountRate)
- FinalPrice (uses OriginalPrice, DiscountAmount)

## Troubleshooting

**Q: Why does my formula show an error about missing dependencies?**
A: Make sure all referenced formulas are saved. The formula name in `GetOutputFrom()` must match exactly (case-sensitive).

**Q: Can I reference a formula that hasn't been saved yet?**
A: No, only saved formulas can be referenced. Click 💾 Save Formula first.

**Q: What happens if I edit a formula that others depend on?**
A: When you execute, the current edited version is used, even if not saved yet. But save it to make changes permanent.

**Q: How do I know which formulas depend on each other?**
A: Currently, you need to check the formula code. Future enhancement could add a dependency viewer.

## Summary

The formula dependency feature enables you to:
- Build complex calculations from simple building blocks
- Reference formulas using `GetOutputFrom('FormulaN ame')`
- Execute all formulas together automatically
- Leverage the Engine's dependency resolution
- Create maintainable, modular formula libraries

All formulas work together seamlessly when you click Execute! 🎉
