# Custom Functions Feature

## Overview

The application now includes three custom functions as examples of how to extend the formula engine by creating functions that inherit from `BaseFunction`. These functions are available in all formula executions and are documented in a dedicated page.

## Custom Functions Included

### 1. GetNum1Func
- **Parameters**: None
- **Returns**: Number (1.0)
- **Description**: Simple function that returns a constant value
- **Usage**: `return GetNum1Func()`

### 2. GetNum2Func
- **Parameters**: None
- **Returns**: Number (2.0)
- **Description**: Simple function that returns a constant value
- **Usage**: `return GetNum2Func()`

### 3. GetNum3Func
- **Parameters**: 1 (String)
- **Returns**: Number (3.0 if parameter is "ABC", otherwise 0.0)
- **Description**: Function with parameter validation and conditional logic
- **Usage**: `return GetNum3Func('ABC')`

## Implementation

All three functions are implemented in the `RecipeCalculator.UI/Functions` folder:

```csharp
public class GetNum1Func : BaseFunction
{
    public GetNum1Func() : base(nameof(GetNum1Func), 0)
    {
    }

    public override IValue Execute()
    {
        return new Value(1.0);
    }
}
```

```csharp
public class GetNum3Func : BaseFunction
{
    public GetNum3Func() : base(nameof(GetNum3Func), 1)
    {
    }

    public override IValue Execute()
    {
        // Validate parameter count
        if (Params.Count() != 1)
        {
            throw new Exception("GetNum3Func expects exactly one parameter.");
        }

        // Validate parameter type
        if (Params.Any(x => !x.Is<string>()))
        {
            throw new Exception("GetNum3Func expects a string parameter.");
        }

        // Get the first parameter
        var firstParam = Params.First();
        var paramValue = firstParam.As<string>();

        // Return different values based on input
        return paramValue.Equals("ABC", StringComparison.OrdinalIgnoreCase) 
            ? new Value(3.0) 
            : new Value(0.0);
    }
}
```

## Features Added

### 1. Custom Functions Page (`/custom-functions`)
A dedicated documentation page that shows:
- Function descriptions
- Parameter information
- Return values
- Usage examples
- Complete source code for each function
- Instructions on how to create custom functions
- Interactive examples to try

### 2. Navigation Menu
Added "Custom Functions" link to the navigation menu for easy access

### 3. Integration with Formula Engine
All custom functions are automatically registered when formulas execute:

```csharp
var customFunctions = new IFunction[]
{
    new GetNum1Func(),
    new GetNum2Func(),
    new GetNum3Func()
};

EngineRunner.Execute(allFormulas, customFunctions);
```

### 4. Syntax Guide Update
The syntax guide in the Formula Calculator now includes a "Custom Functions" section with a link to the documentation page

## Usage Examples

### Example 1: Basic Usage
```
Formula:
return GetNum1Func() + GetNum2Func()

Result: 3
```

### Example 2: With Built-in Functions
```
Formula:
return Max(GetNum1Func(), GetNum2Func())

Result: 2
```

### Example 3: With Parameters
```
Formula:
return GetNum3Func('ABC')

Result: 3

Formula:
return GetNum3Func('XYZ')

Result: 0
```

### Example 4: Complex Expression
```
Formula:
return Max(GetNum1Func(), GetNum2Func()) * GetNum3Func('ABC')

Result: 6
Explanation: Max(1, 2) = 2, multiplied by 3 = 6
```

### Example 5: With Conditionals
```
Formula:
if (GetNum3Func('ABC') > 0) then
  return GetNum1Func() + GetNum2Func()
else
  return 0
end

Result: 3
```

### Example 6: With Variables
```
Variables:
- code: String = "ABC"

Formula:
return GetNum3Func(code)

Result: 3
```

## Creating Your Own Custom Functions

### Step 1: Create a Class
Create a new class in the `Functions` folder that extends `BaseFunction`:

```csharp
using RecipeCalculator.Common.Function;
using RecipeCalculator.Common.Values;

namespace RecipeCalculator.UI.Functions;

public class MyCustomFunc : BaseFunction
{
    // Constructor: name and parameter count
    public MyCustomFunc() : base(nameof(MyCustomFunc), parameterCount)
    {
    }

    public override IValue Execute()
    {
        // Your logic here
        // Access parameters via: Params
        // Return: new Value(result)
        return new Value(42.0);
    }
}
```

### Step 2: Register the Function
Add your function to the custom functions array in `FormulaCalculator.razor`:

```csharp
var customFunctions = new IFunction[]
{
    new GetNum1Func(),
    new GetNum2Func(),
    new GetNum3Func(),
    new MyCustomFunc()  // Add your function here
};
```

### Step 3: Use in Formulas
```
return MyCustomFunc(parameters)
```

## BaseFunction Key Points

### Constructor Parameters
- **name**: Function name as it appears in formulas
- **parameterCount**: Expected number of parameters

### Execute Method
- Override this method to implement your function logic
- Access parameters through `Params` property (IEnumerable<IValue>)
- Return `IValue` (wrap with `new Value()`)

### Parameter Access
```csharp
// Check parameter count
if (Params.Count() != expectedCount)
    throw new Exception("Wrong number of parameters");

// Check parameter type
if (!Params.First().Is<string>())
    throw new Exception("Expected string parameter");

// Get parameter value
var value = Params.First().As<string>();
```

### Supported Types
- **double**: Numeric values
- **string**: Text values
- **bool**: Boolean values

### Return Values
```csharp
return new Value(1.0);      // Number
return new Value("text");   // String
return new Value(true);     // Boolean
```

## Best Practices

1. **Validate Parameters**: Always check parameter count and types
2. **Clear Errors**: Throw exceptions with descriptive messages
3. **Document Functions**: Add XML documentation comments
4. **Naming Convention**: Use descriptive, PascalCase names ending with "Func"
5. **Type Safety**: Use `Is<T>()` and `As<T>()` for type checking
6. **Immutability**: Don't modify parameters; create new values

## Example Use Cases

### Data Lookup Function
```csharp
public class LookupPriceFunc : BaseFunction
{
    public LookupPriceFunc() : base(nameof(LookupPriceFunc), 1)
    {
    }

    public override IValue Execute()
    {
        var productCode = Params.First().As<string>();
        var price = GetPriceFromDatabase(productCode);
        return new Value(price);
    }
}
```

### Calculation Function
```csharp
public class CalculateTaxFunc : BaseFunction
{
    public CalculateTaxFunc() : base(nameof(CalculateTaxFunc), 2)
    {
    }

    public override IValue Execute()
    {
        var amount = Params.ElementAt(0).As<double>();
        var rate = Params.ElementAt(1).As<double>();
        return new Value(amount * rate);
    }
}
```

### Validation Function
```csharp
public class IsValidEmailFunc : BaseFunction
{
    public IsValidEmailFunc() : base(nameof(IsValidEmailFunc), 1)
    {
    }

    public override IValue Execute()
    {
        var email = Params.First().As<string>();
        var isValid = email.Contains("@") && email.Contains(".");
        return new Value(isValid);
    }
}
```

## Testing Custom Functions

Test your functions thoroughly:

1. **Test with correct parameters**: Verify expected behavior
2. **Test with wrong parameter count**: Should throw exception
3. **Test with wrong parameter types**: Should throw exception
4. **Test edge cases**: Empty strings, zero values, etc.
5. **Test in formulas**: Use in actual formula execution

## Files Created

- `src/RecipeCalculator.UI/Functions/GetNum1Func.cs`
- `src/RecipeCalculator.UI/Functions/GetNum2Func.cs`
- `src/RecipeCalculator.UI/Functions/GetNum3Func.cs`
- `src/RecipeCalculator.UI/Pages/CustomFunctions.razor`

## Files Modified

- `src/RecipeCalculator.UI/Layout/NavMenu.razor` - Added link to Custom Functions page
- `src/RecipeCalculator.UI/Pages/FormulaCalculator.razor` - Integrated custom functions in execution
- Syntax guide updated to include custom functions section

## Benefits

1. **Extensibility**: Easy to add new functions without modifying the engine
2. **Reusability**: Functions can be used across all formulas
3. **Documentation**: Built-in page showing how functions work
4. **Examples**: Real working code to learn from
5. **Type Safety**: Parameter validation ensures correct usage

## Summary

The Custom Functions feature demonstrates how to extend the formula engine with user-defined operations. The three example functions (GetNum1Func, GetNum2Func, GetNum3Func) show different patterns: simple constants, parameterless functions, and functions with validated parameters. The dedicated documentation page makes it easy for users to understand and use these functions, and provides a template for creating their own custom functions.

Custom Functions + Variables + Formulas + Dependencies = Complete calculation system! 🎉
