# AI Formula Generation Feature

## Overview

The application now includes AI-powered formula generation using Large Language Models (LLMs). Users can describe what they want to calculate in natural language, and the AI generates the formula code automatically using RAG (Retrieval-Augmented Generation) with the ANTLR4 grammar and examples.

## Features

### 1. AI Formula Chat Component
- Located in the left panel of the Formula Calculator
- Natural language input for describing calculations
- One-click formula generation
- Direct integration with the formula editor
- Example prompts for guidance

### 2. LLM Configuration Page (`/llm-settings`)
- Support for multiple LLM providers:
  - **Ollama** (Local - Recommended)
  - **OpenAI** (GPT-3.5, GPT-4)
  - **Anthropic** (Claude)
  - **Custom API** endpoints
- API key management (stored locally in browser)
- Model selection
- Connection testing
- Detailed setup instructions

### 3. RAG Implementation
The system uses context from:
- ANTLR4 grammar rules
- 10+ example formulas covering all features
- Available functions (built-in and custom)
- Operators and syntax rules
- Best practices

## Supported LLM Providers

### Ollama (Recommended - No API Key Required)

**Why Ollama?**
- ✅ Runs locally on your computer
- ✅ Completely free
- ✅ No API key required
- ✅ Private - your data never leaves your machine
- ✅ Fast responses
- ✅ Multiple model options

**Installation:**
1. Visit [ollama.ai](https://ollama.ai)
2. Download and install for your OS
3. Pull a model: `ollama pull llama2`
4. Keep Ollama running in the background

**Popular Models:**
- `llama2` - Good balance of speed and quality
- `codellama` - Optimized for code generation
- `mistral` - Fast and accurate
- `phi` - Lightweight option

**Default Configuration:**
- API URL: `http://localhost:11434`
- Model: `llama2`
- No API Key needed

### OpenAI

**Requirements:**
- API key from [platform.openai.com](https://platform.openai.com)
- Pay-per-use pricing

**Configuration:**
- API URL: `https://api.openai.com`
- Models: `gpt-3.5-turbo`, `gpt-4`, `gpt-4-turbo`
- API Key: Required

### Anthropic (Claude)

**Requirements:**
- API key from [console.anthropic.com](https://console.anthropic.com)
- Pay-per-use pricing

**Configuration:**
- API URL: `https://api.anthropic.com`
- Models: `claude-3-opus-20240229`, `claude-3-sonnet-20240229`
- API Key: Required

### Custom API

**For self-hosted or other LLM services:**
- Configure custom endpoint URL
- Optional API key support
- Flexible integration

## How It Works

### 1. RAG Context Building

The system builds a comprehensive context including:

```
# Grammar Rules
- Complete ANTLR4 grammar (Formula.g4) embedded in the application
- Full formula syntax from the grammar definition
- All operators and keywords
- Control flow structures
- Token definitions and lexer rules

# Available Functions
- Built-in: Max, Min, Rnd, Ceil, Floor, etc.
- Custom: GetNum1Func, GetNum2Func, GetNum3Func
- Dependencies: GetOutputFrom()

# Examples (10+ formulas)
- Simple calculations
- Conditional logic
- Variable usage
- Function calls
- Formula dependencies
- Complex expressions
```

**Note:** The Formula.g4 file from `RecipeCalculator.Engine\Grammar\` is embedded as a resource in the UI project and loaded at runtime to provide the LLM with the complete, authoritative grammar definition.

### 2. Prompt Engineering

User input is combined with the RAG context:

```
System Prompt:
- You are a formula code generator
- Here's the grammar: [ANTLR4 grammar]
- Here are examples: [10+ examples]
- Rules: [syntax rules]

User Request:
"Calculate 10% discount if quantity > 50"

Generate ONLY the formula code without explanation.
```

### 3. Response Processing

- AI generates formula code
- System cleans markdown formatting
- Code loaded into editor
- User can review, modify, and execute

## Usage Examples

### Example 1: Simple Discount

**User Input:**
```
Calculate a discount: if quantity is more than 100, apply 20% discount.
If quantity is more than 50, apply 10% discount. Otherwise no discount.
```

**Generated Formula:**
```
if (quantity > 100) then
  return price * 0.8
else if (quantity > 50) then
  return price * 0.9
else
  return price
end
```

### Example 2: Tax Calculation

**User Input:**
```
Calculate tax: multiply the price by a tax rate of 0.15, then add it to the original price to get the total.
```

**Generated Formula:**
```
return price + (price * 0.15)
```

### Example 3: Complex Logic

**User Input:**
```
For VIP customers with quantity over 50, apply 30% discount. For VIP customers, apply 20% discount. For regular customers with quantity over 100, apply 15% discount. Otherwise, no discount.
```

**Generated Formula:**
```
if (isVIP and quantity > 50) then
  return price * 0.7
else if (isVIP) then
  return price * 0.8
else if (quantity > 100) then
  return price * 0.85
else
  return price
end
```

## Setup Guide

### Quick Start with Ollama (5 minutes)

1. **Install Ollama:**
   ```bash
   # Visit ollama.ai and download
   # Or on macOS:
   brew install ollama
   ```

2. **Pull a Model:**
   ```bash
   ollama pull llama2
   ```

3. **Start Ollama:**
   ```bash
   ollama serve
   # Or just open the Ollama app
   ```

4. **Configure in App:**
   - Go to "AI Settings" in the navigation menu
   - Provider: Select "Ollama (Local - No API Key)"
   - API URL: `http://localhost:11434` (default)
   - Model Name: `llama2`
   - Click "Save Settings"

5. **Test:**
   - Click "Test Connection"
   - Should show success message

6. **Use:**
   - Go to Formula Calculator
   - Find "AI Formula Generator" card in left panel
   - Describe your calculation
   - Click "Generate Formula"
   - Review and use the generated code

### Setup with OpenAI

1. **Get API Key:**
   - Visit [platform.openai.com](https://platform.openai.com)
   - Create account / sign in
   - Go to API Keys section
   - Create new secret key
   - Copy the key (you won't see it again!)

2. **Configure:**
   - Go to "AI Settings"
   - Provider: "OpenAI"
   - API URL: `https://api.openai.com`
   - Model Name: `gpt-3.5-turbo` or `gpt-4`
   - API Key: Paste your key
   - Save Settings

### Setup with Anthropic

1. **Get API Key:**
   - Visit [console.anthropic.com](https://console.anthropic.com)
   - Create account
   - Generate API key

2. **Configure:**
   - Go to "AI Settings"
   - Provider: "Anthropic (Claude)"
   - API URL: `https://api.anthropic.com`
   - Model Name: `claude-3-sonnet-20240229`
   - API Key: Paste your key
   - Save Settings

## Tips for Better Results

1. **Be Specific:**
   - Bad: "calculate discount"
   - Good: "if quantity is greater than 100, apply 20% discount to the price"

2. **Mention Variable Names:**
   - Include the names you want to use: "price", "quantity", "taxRate"

3. **Describe Conditions Clearly:**
   - Use phrases like "if... then...", "greater than", "less than"

4. **Specify Return Values:**
   - Mention what the formula should return

5. **Use Examples:**
   - "Similar to: if x > 10 then return x * 2"

## Security & Privacy

### Local Storage
- API keys stored in browser's localStorage
- Never sent to our servers
- Stays on your device
- Clear browser data to remove

### Ollama Privacy
- Completely local execution
- No data leaves your computer
- No API calls to external services
- Full privacy

### API-Based Providers
- API keys sent only to the provider's servers
- Subject to provider's privacy policy
- Prompts sent to provider for processing

## Files Created

- `Models/LLMSettings.cs` - Configuration model
- `Services/LLMSettingsService.cs` - Settings persistence
- `Services/LLMService.cs` - LLM integration with multiple providers
- `Components/AIFormulaChat.razor` - Chat UI component
- `Pages/LLMConfiguration.razor` - Settings page

## Files Modified

- `Program.cs` - Registered LLM services
- `Layout/NavMenu.razor` - Added AI Settings link
- `Pages/FormulaCalculator.razor` - Integrated AI chat component
- `_Imports.razor` - Added Components namespace
- `RecipeCalculator.UI.csproj` - Added Formula.g4 as embedded resource
- `Components/AIFormulaChat.razor` - Loads Formula.g4 grammar at runtime
- `Pages/FormulaCalculator.razor` - Integrated AI chat component
- `_Imports.razor` - Added Components namespace

## Technical Architecture

### LLM Service

```csharp
public class LLMService
{
    // Provider-agnostic interface
    Task<string> GenerateFormulaAsync(
        string userPrompt, 
        string grammarContext, 
        string examples
    )
    
    // Provider-specific implementations
    - CallOllamaAsync()
    - CallOpenAIAsync()
    - CallAnthropicAsync()
    - CallCustomAsync()
}
```

### RAG Context

The system provides rich context to the LLM:

**Grammar Context:**
- Complete ANTLR4 grammar definition loaded from embedded Formula.g4 file
- Full syntax rules including all production rules
- All operators and their precedence
- Control flow structures
- Function signatures
- Token definitions and lexer rules
- Fallback to simplified grammar if resource loading fails

**Implementation:**
```csharp
// AIFormulaChat.razor - GetGrammarContext()
private string GetGrammarContext()
{
    try
    {
        var assembly = typeof(AIFormulaChat).Assembly;
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith("Formula.g4"));
        
        if (resourceName != null)
        {
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream != null)
            {
                using var reader = new System.IO.StreamReader(stream);
                var grammarContent = reader.ReadToEnd();
                return grammarContent; // Full ANTLR4 grammar
            }
        }
    }
    catch (Exception)
    {
        // Falls back to simplified version
    }
}
```

**Examples Context:**
- 10 diverse formula examples
- All language features covered
- Best practices demonstrated

### Response Processing

1. Call LLM with context + user prompt
2. Receive generated code
3. Clean markdown formatting (remove ```)
4. Validate basic structure
5. Load into editor for user review

## Benefits

1. **Accessibility:** Non-programmers can create formulas
2. **Speed:** Generate complex formulas in seconds
3. **Learning:** See examples of correct syntax
4. **Flexibility:** Multiple LLM options
5. **Privacy:** Local option available (Ollama)
6. **Cost-Effective:** Free local option

## Limitations

1. **AI Accuracy:** Review generated code before use
2. **Context Window:** Very complex requests may hit token limits
3. **Model Dependent:** Quality varies by model choice
4. **Internet/Local:** API providers need internet; Ollama needs local resources

## Troubleshooting

**Q: "Error calling Ollama" message**
A: Make sure Ollama is running. Try `ollama serve` in terminal.

**Q: Generated formula has syntax errors**
A: Try rephrasing your request more clearly. Use example prompts as guides.

**Q: "Connection test failed"**
A: Check API URL and API key. For Ollama, ensure it's running on port 11434.

**Q: Slow generation**
A: Try a smaller/faster model (e.g., `phi` for Ollama, `gpt-3.5-turbo` for OpenAI)

**Q: API key not working**
A: Verify key is correct. Check if you have API credits/quota remaining.

## Summary

The AI Formula Generation feature brings natural language processing to formula creation, making the system accessible to users of all skill levels. By supporting multiple LLM providers including free local options, it provides flexibility while maintaining privacy. The RAG approach ensures generated formulas follow the correct syntax and leverage all available features.

AI-Powered Formula Generation + Custom Functions + Variables + Dependencies = Ultimate calculation platform! 🤖✨

## Recent Improvements

### Enhanced Output Quality (Latest Update)

To ensure LLMs return clean, executable code without explanatory text, several improvements were made:

**1. Improved System Prompt:**
- More explicit instructions with "CRITICAL RULES - FOLLOW EXACTLY"
- Shows GOOD vs BAD examples directly in the prompt
- Emphasizes output format: "Output ONLY executable formula code"
- Added visual examples showing what to do and what NOT to do

**2. Enhanced User Message:**
- Reinforces format with each request: "Output ONLY the executable formula code"
- Reminds to start with 'return' or 'if'
- Explicitly states "No explanations, no preamble, no markdown blocks"

**3. Advanced Code Cleaning:**
The `CleanGeneratedCode` function now:
- Removes markdown code blocks (```)
- Strips common explanatory phrases:
  - "Here is the formula:"
  - "Here's your formula:"
  - "The formula is:"
  - "Code:", "Output:", "Result:", etc.
- Detects and removes non-code first lines
- Checks if first line starts with code keywords (`return`, `if`, `//`)
- Skips explanatory first lines automatically

**4. Lower Temperature:**
- Ollama: temperature = 0.1 (more deterministic)
- OpenAI: temperature = 0.1 (more precise)
- Results in more consistent, predictable output

**5. System Instructions:**
- Ollama: Additional system field: "You are a precise code generator. Output only executable code."
- All providers: Clear separation between system prompt and user message

### Example of Cleaning Process

**Before Cleaning:**
```
Here is the formula you requested:

if (quantity > 50) then
  return price * 0.9
else
  return price
end
```

**After Cleaning:**
```
if (quantity > 50) then
  return price * 0.9
else
  return price
end
```

The system automatically detects and removes the explanatory first line, leaving only the executable code.

### Why These Improvements Matter

1. **Better User Experience:** Users get clean code immediately
2. **Fewer Errors:** No syntax errors from explanatory text
3. **Direct Integration:** Code can be used immediately without manual cleanup
4. **Consistent Output:** Lower temperature means more predictable results
5. **Robust Cleaning:** Multiple layers of cleaning catch various LLM response patterns

### Testing Your Configuration

After setting up your LLM, test with this prompt:

```
Calculate 15% tax on a price
```

Expected clean output:
```
return price * 1.15
```

or

```
return price + (price * 0.15)
```

If you get explanatory text, the cleaning system will automatically remove it. If issues persist, try:
1. Switching to a different model (codellama for Ollama, gpt-4 for OpenAI)
2. Reducing the complexity of your request
3. Being more specific about variable names


## Technical Notes

### HttpClient Configuration

The LLM service uses `IHttpClientFactory` to create HttpClient instances without a BaseAddress. This is crucial because:

1. **Blazor WASM Default HttpClient** has BaseAddress set to the app's URL
2. **LLM APIs** need to make requests to external URLs (Ollama, OpenAI, etc.)
3. **Using HttpClientFactory** creates fresh clients without BaseAddress restrictions
4. **Each request** gets its own HttpClient instance to avoid header pollution

**Before (Broken):**
```csharp
public LLMService(HttpClient httpClient) // Would use app's BaseAddress
{
    _httpClient = httpClient;
}
```

**After (Fixed):**
```csharp
public LLMService(IHttpClientFactory httpClientFactory) // Creates clean clients
{
    _httpClientFactory = httpClientFactory;
}

private async Task CallAPI()
{
    using var httpClient = _httpClientFactory.CreateClient(); // Fresh client
    await httpClient.PostAsync("http://localhost:11434/...", ...);
}
```

This ensures absolute URLs work correctly for all LLM provider endpoints.

