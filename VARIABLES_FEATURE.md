# Variables Feature

## Overview

Users can now define variables that will be available in the execution context for all formulas. Variables are stored in browser local storage and can be of type Number, String, or Boolean.

## Features

- **Add Variables**: Define variables with name, type, and value
- **Edit Variables**: Update existing variables
- **Delete Variables**: Remove variables from storage
- **Persistent Storage**: Variables saved in browser local storage
- **Type Support**: Number, String, and Boolean types
- **Execution Context**: All variables are loaded into the ParsingContext before formula execution

## Using Variables

### Adding a Variable

1. In the left panel, find the "Variables" card
2. Click the **➕** button
3. Enter variable details:
   - **Name**: Variable name (e.g., `price`, `quantity`, `taxRate`)
   - **Type**: Select Number, String, or Boolean
   - **Value**: Enter the value based on type
4. Click "Save Variable"

### Variable Types

**Number**
- Used for numeric calculations
- Examples: `100`, `25.5`, `-10`, `0.15`
- Input: Number field with decimal support

**String**
- Used for text values
- Examples: `"ABC"`, `"Customer Name"`, `"2024-01-15"`
- Input: Text field
- Note: Strings in formulas should use single quotes: `'ABC'`

**Boolean**
- Used for true/false logic
- Values: `true` or `false`
- Input: Dropdown selector

### Using Variables in Formulas

Variables can be referenced directly by name in formulas:

**Example 1: Basic Calculation**
```
Variables:
- price: Number = 100
- taxRate: Number = 0.15

Formula:
return price + (price * taxRate)

Result: 115
```

**Example 2: Conditional Logic**
```
Variables:
- quantity: Number = 75
- price: Number = 100

Formula (DiscountCalculator):
if (quantity > 100) then
  return price * 0.8
else if (quantity > 50) then
  return price * 0.9
else
  return price
end

Result: 90 (10% discount applied)
```

**Example 3: String Comparison**
```
Variables:
- customerType: String = "Premium"
- basePrice: Number = 100

Formula:
if (customerType = 'Premium') then
  return basePrice * 0.8
else if (customerType = 'Regular') then
  return basePrice * 0.9
else
  return basePrice
end

Result: 80 (Premium discount)
```

**Example 4: Boolean Flags**
```
Variables:
- isVIP: Boolean = true
- price: Number = 100

Formula:
if (isVIP) then
  return price * 0.7
else
  return price
end

Result: 70 (VIP 30% discount)
```

## Complex Example: Invoice Calculator

### Variables
```
basePrice: Number = 1000
quantity: Number = 5
taxRate: Number = 0.13
shippingFee: Number = 25
isMember: Boolean = true
```

### Formulas

**Formula 1: Subtotal**
```
return basePrice * quantity
```
Result: 5000

**Formula 2: MemberDiscount**
```
if (isMember) then
  return GetOutputFrom('Subtotal') * 0.1
else
  return 0
end
```
Result: 500

**Formula 3: DiscountedSubtotal**
```
return GetOutputFrom('Subtotal') - GetOutputFrom('MemberDiscount')
```
Result: 4500

**Formula 4: Tax**
```
return GetOutputFrom('DiscountedSubtotal') * taxRate
```
Result: 585

**Formula 5: Total**
```
return GetOutputFrom('DiscountedSubtotal') + GetOutputFrom('Tax') + shippingFee
```
Result: 5110

## Variable Management

### Viewing Variables
- All variables are listed in the "Variables" card in the left panel
- Sorted alphabetically by name
- Shows: Name, Type, and Value

### Editing Variables
- Click ➕ to add a new variable
- To edit: Delete the old variable and create a new one with the same name

### Deleting Variables
- Click the 🗑️ button next to any variable
- Confirmation dialog appears
- Variable is removed from storage and execution context

## Technical Details

### Storage
- Variables stored in browser local storage
- Storage key: `RecipeCalculator.Variables`
- Format: JSON array of `StoredVariable` objects

### Execution Flow
```csharp
1. Load all variables from storage
2. For each variable:
   - Parse value based on type
   - Create IValue instance
   - Add to ParsingContext.VariableCache
3. Execute formulas (variables available in context)
```

### Type Conversion
```csharp
Number:
  double.TryParse(value) → new Value(double)

String:
  new Value(string)

Boolean:
  bool.TryParse(value) → new Value(bool)
```

### Variable Cache
Variables are stored in `IVariableCache` interface:
- `Set(string name, IValue value)`: Add/update variable
- `Get(string name)`: Retrieve variable value
- `Remove(string name)`: Delete variable

## Best Practices

1. **Meaningful Names**: Use descriptive variable names (`taxRate` not `x`)
2. **Consistent Types**: Keep variable types consistent with their usage
3. **Document Dependencies**: Comment which variables a formula needs
4. **Test Incrementally**: Test formulas with simple variable values first
5. **Group Related Variables**: Organize related variables together

## Example Workflows

### Scenario 1: Price Calculator
```
1. Add variables:
   - price: 100
   - quantity: 10
   - discount: 0.15

2. Create formula:
   return (price * quantity) * (1 - discount)

3. Execute → Result: 850
```

### Scenario 2: Date-Based Logic
```
1. Add variables:
   - orderDate: '2024-01-15'
   - currentDate: '2024-01-20'

2. Create formula:
   return GetDiffDays(orderDate, currentDate)

3. Execute → Result: 5 days
```

### Scenario 3: Multi-Tier Pricing
```
1. Add variables:
   - quantity: 150
   - tier1Limit: 50
   - tier2Limit: 100
   - price: 10

2. Create formula:
   if (quantity > tier2Limit) then
     return quantity * price * 0.8
   else if (quantity > tier1Limit) then
     return quantity * price * 0.9
   else
     return quantity * price
   end

3. Execute → Result: 1200 (20% discount)
```

## Limitations

1. **Variable Scope**: Variables are global - available to all formulas
2. **Type Safety**: Type checking happens at execution time
3. **No Arrays/Objects**: Only scalar types supported (Number, String, Boolean)
4. **Name Conflicts**: Variable names must not conflict with formula names

## Troubleshooting

**Q: My formula says variable is undefined**
A: Make sure the variable is saved in the Variables panel before executing

**Q: Boolean comparison not working**
A: Use lowercase `true` or `false` in formulas

**Q: Number variable giving wrong result**
A: Check decimal format - use `.` not `,` for decimals

**Q: String comparison not working**
A: Strings in formulas must use single quotes: `if (customerType = 'Premium')`

**Q: Can I use variables in formula dependencies?**
A: Yes! Variables work with both simple formulas and formulas using `GetOutputFrom()`

## UI Components

### Variables Card
- Location: Left panel, below Quick Examples
- Header: "Variables" with ➕ button
- Body: List of all variables with delete buttons
- Empty state: "No variables defined"

### Variable Modal
- Fields:
  - Variable Name (text input)
  - Type (dropdown: Number/String/Boolean)
  - Value (dynamic input based on type)
- Buttons:
  - Cancel: Close without saving
  - Save Variable: Add/update variable

## Summary

The Variables feature enables:
- Dynamic formula inputs without code changes
- Reusable formula logic with different values
- Complex calculations combining variables and formulas
- Persistent storage of common values
- Type-safe execution context

Variables + Formulas + Dependencies = Powerful calculation system! 🎉
