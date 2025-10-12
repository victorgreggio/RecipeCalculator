# RecipeCalculator

A proof-of-concept Domain-Specific Language (DSL) for calculating values based on a recipe of formulas with automatic dependency resolution.

## Overview

RecipeCalculator is a C# library that provides a formula evaluation engine capable of parsing and executing mathematical and logical expressions. The project demonstrates how to build a DSL that can handle complex formula dependencies, function calls, and conditional logic through a custom grammar parser.

**New in this version**: Web-based UI built with Blazor WebAssembly that provides an interactive formula editor with Monaco Editor (VS Code's editor) for writing and testing formulas in your browser!

## Key Features

### Formula System
- **Named Formulas**: Define reusable formulas with unique names
- **Formula Dependencies**: Formulas can reference other formulas using `GetOutputFrom('FormulaName')`
- **Automatic Dependency Resolution**: Built-in topological sorting ensures formulas execute in the correct order
- **Parallel Execution**: Independent formulas in the same dependency layer execute in parallel for optimal performance

### Expression Support
- **Arithmetic Operations**: Addition, subtraction, multiplication, division, modulo, power
- **Comparison Operators**: Equals, not equals, greater than, less than, greater/less than or equals
- **Logical Operators**: AND, OR
- **Conditional Logic**: If-then-else statements with else-if support

### Built-in Functions
- **Mathematical**: `max()`, `min()`, `rnd()` (round), `ceil()`, `floor()`, `exp()`
- **Date Operations**: `year()`, `month()`, `day()`, `addDays()`, `getDiffDays()`, `differenceInMonths()`
- **String Operations**: `substr()`, `paddedString()`
- **Custom Functions**: Extensible system for adding custom function implementations

### Grammar & Parser
- Built using ANTLR 4 for robust parsing
- Custom grammar defined in `Formula.g4`
- Support for numbers, strings, booleans, and identifiers
- Error handling with custom error types

### Web UI
- **Blazor WebAssembly**: Modern web-based interface that runs entirely in the browser
- **Formula Management**: Create, save, edit, and delete multiple formulas with browser storage
- **Variable Support**: Define and manage typed variables (Number, String, Boolean)
- **Real-time Execution**: Execute formulas and see results immediately
- **Custom Functions**: Built-in examples (GetNum1Func, GetNum2Func, GetNum3Func)
- **AI-Powered Generation**: Natural language to formula code using LLM integration
- **Syntax Guide**: Built-in reference documentation
- **Error Display**: Clear error messages and stack traces
- **Production Ready**: Deploy to Azure Static Web Apps for FREE

## Getting Started

### Web UI (Recommended for Quick Start)

Run the Blazor WebAssembly application:

```bash
cd src/RecipeCalculator.UI
dotnet run
```

Navigate to `https://localhost:5001` (or the URL shown in console) and start writing formulas!

See [BLAZOR_UI_SUMMARY.md](BLAZOR_UI_SUMMARY.md) for more details about the web interface.

### Deploy to Azure (Production)

Deploy your application to Azure Static Web Apps for FREE:

```bash
./deploy-azure.sh
```

Or use the one-command deployment:

```bash
az staticwebapp create --name recipecalculator \
  --resource-group RecipeCalculatorRG \
  --source https://github.com/YOUR_USERNAME/RecipeCalculator \
  --branch main --app-location "/src/RecipeCalculator.UI" \
  --output-location "wwwroot" --login-with-github
```

See [QUICKSTART_AZURE.md](QUICKSTART_AZURE.md) for quick deployment guide or [AZURE_DEPLOYMENT.md](AZURE_DEPLOYMENT.md) for comprehensive deployment documentation.

### Programmatic Usage

Add references to `RecipeCalculator.Engine` and `RecipeCalculator.Common` in your project, then use the engine programmatically (see examples below).

## Usage Examples

### Simple Arithmetic
```csharp
const string formulaName = "SimpleCalc";
const string body = "return 2 + 2";
var formula = new Formula(formulaName, body);

engineRunner.Execute(new[] {formula}, Enumerable.Empty<IFunction>());
var result = parsingContext.FormulaResultCache.Get(formulaName);
// result = 4
```

### Conditional Logic with Functions
```csharp
const string formulaName = "ConditionalCalc";
const string body = "if (Var1 = 'A') then return max(GetNum1Func(), GetNum2Func()) else return 0.0 end";
var formula = new Formula(formulaName, body);

engineRunner.Execute(new[] {formula}, new[] {new GetNum1Func(), new GetNum2Func()});
```

### Formula Dependencies
```csharp
var formula1 = new Formula("Base1", "return 2 + 2");      // Evaluates to 4
var formula2 = new Formula("Base2", "return 2 + 3");      // Evaluates to 5
var formula3 = new Formula("Sum", "return GetOutputFrom('Base1') + GetOutputFrom('Base2')");  // Evaluates to 9

// The engine automatically resolves dependencies and executes in correct order
engineRunner.Execute(new[] {formula1, formula2, formula3}, Enumerable.Empty<IFunction>());
```

## Project Structure

```
RecipeCalculator/
├── src/
│   ├── RecipeCalculator.Common/     # Common interfaces and models
│   │   ├── Formulas/               # Formula definitions (IFormula, Formula)
│   │   ├── Function/               # Function interfaces and base classes
│   │   ├── Values/                 # Value types and wrappers
│   │   └── Variants/               # Variant types
│   ├── RecipeCalculator.Engine/    # Core calculation engine
│   │   ├── Grammar/                # ANTLR grammar definition (Formula.g4)
│   │   ├── Generated/              # ANTLR-generated parser code
│   │   ├── Parser/                 # Parsing context and visitor implementations
│   │   ├── Graphs/                 # Dependency graph and topological sorting
│   │   ├── Function/               # Function result caching
│   │   ├── Formulas/               # Formula result caching
│   │   └── Exceptions/             # Custom exception types
│   └── RecipeCalculator.UI/        # Blazor WebAssembly web interface
│       ├── Pages/                  # Razor pages and components
│       ├── Layout/                 # Layout components
│       └── wwwroot/                # Static web assets
└── test/
    ├── Common/                     # Tests for common components
    └── Engine/                     # Tests for engine functionality
```

## Building the Project

### Requirements
- .NET 9.0 SDK
- C# 12
- GitHub Personal Access Token (for accessing GitHub Packages)

### Setting Up GitHub Packages Authentication

This project uses packages from GitHub Packages. To build locally, you need to authenticate:

1. **Create a GitHub Personal Access Token:**
   - Go to GitHub Settings → Developer settings → Personal access tokens → Tokens (classic)
   - Generate a new token with `read:packages` scope
   - Copy the token

2. **Add credentials to NuGet:**
   ```bash
   dotnet nuget update source github --username YOUR_GITHUB_USERNAME --password YOUR_GITHUB_TOKEN --store-password-in-clear-text
   ```

   Replace `YOUR_GITHUB_USERNAME` with your GitHub username and `YOUR_GITHUB_TOKEN` with your personal access token.

### Build
```bash
dotnet build RecipeCalculator.sln
```

### Run Tests
```bash
dotnet test RecipeCalculator.sln
```

### Run Web UI
```bash
cd src/UI
dotnet run
```

## Technical Details

### Dependency Resolution
The engine uses a Directed Acyclic Graph (DAG) to model formula dependencies and performs topological sorting to determine execution order. Formulas with circular dependencies are detected and reported as errors.

### Caching
- **Parse Tree Cache**: Parsed formula trees are cached to avoid re-parsing
- **Function Result Cache**: Function results are cached based on function name and parameter types
- **Formula Result Cache**: Formula results are cached by formula name

### Grammar
The DSL grammar is defined using ANTLR 4 in `Formula.g4`. Key syntax elements:
- **Return statement**: `return <expression>`
- **If statement**: `if (<condition>) then <block> else if (<condition>) then <block> else <block> end`
- **Function call**: `functionName(arg1, arg2, ...)`
- **Formula reference**: `GetOutputFrom('FormulaName')`

## Use Cases

This project demonstrates a proof-of-concept for:
- Building domain-specific calculation engines
- Implementing dependency resolution systems
- Creating custom expression languages
- Parsing and evaluating mathematical formulas
- Handling complex business rule calculations with interdependencies
- Creating web-based formula editors and calculators

## License

This is a proof-of-concept project. License details not specified.

## Contributing

This is a proof-of-concept project. Contribution guidelines not specified.
