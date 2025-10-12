# Quick Start Guide - Recipe Calculator with Blazor UI

## Try the Web UI (Easiest Way)

1. **Navigate to the UI project:**
   ```bash
   cd src/RecipeCalculator.UI
   ```

2. **Run the application:**
   ```bash
   dotnet run
   ```

3. **Open your browser:**
   Navigate to `https://localhost:5001` (or the URL shown in the console)

4. **Start creating formulas!**

## Your First Formula

Once the UI loads:

1. In the "Formula Name" field, enter: `MyFirstFormula`

2. In the code editor, type:
   ```
   return 2 + 2
   ```

3. Click "Execute Formula"

4. See the result in the right panel: `Result: 4`

## Try More Examples

### Using Functions

Formula Name: `MaxExample`
```
return Max(15, 25)
```
Result: `25`

### Conditional Logic

Formula Name: `DiscountCalculator`
```
// Calculate discount based on quantity
if (quantity > 100) then
  return price * 0.8
else if (quantity > 50) then
  return price * 0.9
else
  return price
end
```

Note: This formula references variables `quantity` and `price` which would need to be provided in a real scenario.

### Date Functions

Formula Name: `YearExtractor`
```
return Year('2024-12-25')
```
Result: `2024`

### String Functions

Formula Name: `SubstringExample`
```
return Substr('Hello World', 0, 5)
```
Result: `Hello`

## Understanding the Syntax

### Basic Structure
Every formula must have a `return` statement:
```
return <expression>
```

### Conditional Statements
```
if (<condition>) then
  <block>
else if (<condition>) then
  <block>
else
  <block>
end
```

### Comments
```
// Single line comment

/* 
   Multi-line
   comment
*/
```

### Available Operators

**Arithmetic:** `+`, `-`, `*`, `/`, `^` (power), `mod`

**Comparison:** `=`, `<>`, `<`, `>`, `<=`, `>=`

**Logical:** `and`, `or`, `!` (not)

### Built-in Functions

**Math:**
- `Max(a, b)` - Maximum value
- `Min(a, b)` - Minimum value
- `Rnd(value, decimals)` - Round
- `Ceil(value)` - Round up
- `Floor(value)` - Round down
- `Exp(value)` - Exponential

**Date:**
- `Year(date)` - Extract year
- `Month(date)` - Extract month
- `Day(date)` - Extract day
- `AddDays(date, days)` - Add days to date
- `GetDiffDays(date1, date2)` - Days between dates
- `DifferenceInMonths(date1, date2)` - Months between dates

**String:**
- `Substr(string, start, length)` - Extract substring
- `PaddedString(string, length)` - Pad with zeros

## Tips

1. **Use the "Load Example" button** to see a pre-built formula
2. **Check the Syntax Guide** in the right panel for quick reference
3. **Error messages** appear in red in the result panel
4. **Clear button** resets the editor
5. Dates should be in ISO format: `'2024-12-25'` or `'2024-12-25T10:30:00'`
6. Strings must be in single quotes: `'my string'`

## What's Next?

- Explore the grammar file: `src/RecipeCalculator.Engine/Grammar/Formula.g4`
- Check out the test files for more examples: `test/Engine/Grammar/GrammarTests.cs`
- Read the full documentation: `README.md` and `src/RecipeCalculator.UI/README.md`

## Building and Testing

### Build the entire solution:
```bash
dotnet build
```

### Run all tests:
```bash
dotnet test
```

### Publish for production:
```bash
cd src/RecipeCalculator.UI
dotnet publish -c Release
```

## Troubleshooting

**Port already in use?**
Edit `src/RecipeCalculator.UI/Properties/launchSettings.json` to change the port.

**Build errors?**
Make sure you have .NET 9.0 SDK installed:
```bash
dotnet --version
```

**Monaco Editor not loading?**
Check browser console for errors. Make sure all Monaco scripts loaded correctly.

## Need Help?

- Check the syntax guide in the UI
- Look at example formulas in the test files
- Read the ANTLR grammar: `src/RecipeCalculator.Engine/Grammar/Formula.g4`
- Review the implementation summary: `BLAZOR_UI_SUMMARY.md`

Enjoy creating formulas with Recipe Calculator! 🎉
