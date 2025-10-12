# Recipe Calculator UI

A Blazor WebAssembly application with formula management and browser local storage.

## Features

- **Simple Text Editor**: Easy-to-use textarea for writing formulas
- **Formula Management**: Create, save, edit, and delete formulas
- **Browser Local Storage**: All formulas persist in your browser
- **Formula Library**: View all saved formulas with timestamps
- **Quick Examples**: Load example formulas with one click
- **Real-time Execution**: Execute formulas and see results immediately
- **Syntax Guide**: Built-in reference for formula syntax
- **Error Handling**: Clear error messages for debugging

## Running the Application

```bash
cd src/RecipeCalculator.UI
dotnet run
```

Navigate to `https://localhost:5001` or the URL shown in console.

## Using the UI

### Creating & Saving Formulas

1. Click **➕** to create a new formula
2. Enter a formula name
3. Write your formula code
4. Click **💾 Save Formula** to save to local storage
5. Click **▶️ Execute** to run the formula

### Managing Formulas

- **Load**: Click any formula in the list
- **Save**: Modify and save formulas
- **Delete**: Click 🗑️ (requires confirmation)
- **Clear**: Click **🗑️ Clear** to reset editor

## Formula Syntax

See the syntax guide in the UI or check the main README.md for detailed syntax information.

## Local Storage

All formulas are stored in browser local storage (`RecipeCalculator.Formulas` key), meaning they persist across sessions but are specific to your browser/device.
