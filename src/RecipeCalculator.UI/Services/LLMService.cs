using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using RecipeCalculator.UI.Models;

namespace RecipeCalculator.UI.Services;

public class LLMService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly LLMSettingsService _settingsService;

    public LLMService(IHttpClientFactory httpClientFactory, LLMSettingsService settingsService)
    {
        _httpClientFactory = httpClientFactory;
        _settingsService = settingsService;
    }

    public async Task<string> GenerateFormulaAsync(string userPrompt, string grammarContext, string examples, string customFunctions = "")
    {
        var settings = await _settingsService.GetSettingsAsync();
        
        var systemPrompt = BuildSystemPrompt(grammarContext, examples, customFunctions);
        
        // Create explicit user message that reinforces the output format
        var userMessage = $"{userPrompt}\n\nIMPORTANT: Output ONLY the executable formula code. Start directly with 'return' or 'if'. No explanations, no preamble, no markdown blocks.";
        
        var fullPrompt = $"{systemPrompt}\n\nUser Request: {userMessage}";

        return settings.Provider switch
        {
            LLMProvider.Ollama => await CallOllamaAsync(settings, fullPrompt),
            LLMProvider.OpenAI => await CallOpenAIAsync(settings, systemPrompt, userMessage),
            LLMProvider.Anthropic => await CallAnthropicAsync(settings, systemPrompt, userMessage),
            LLMProvider.Custom => await CallCustomAsync(settings, fullPrompt),
            _ => throw new NotSupportedException($"Provider {settings.Provider} is not supported")
        };
    }

    private string BuildSystemPrompt(string grammar, string examples, string customFunctions)
    {
        return $@"You are a formula code generator for a calculation engine. Your ONLY job is to output valid formula code.

# Grammar (ANTLR4)
{grammar}

# Available Functions
- Math: Max, Min, Rnd, Ceil, Floor, Exp
- Date: Day, Month, Year, AddDays, GetDiffDays, DifferenceInMonths
- String: Substr, PaddedString
- Dependencies: GetOutputFrom('FormulaName')

# Custom Functions (User-Defined)
{customFunctions}

# Operators
- Arithmetic: +, -, *, /, ^, mod
- Comparison: =, <>, <, >, <=, >=
- Logical: and, or, !

# Examples
{examples}

# CRITICAL RULES - FOLLOW EXACTLY:
1. Output ONLY executable formula code
2. Do NOT include explanatory text like 'Here is', 'The formula', etc.
3. Do NOT use markdown code blocks (no ```)
4. Start directly with the code: either 'return' or 'if'
5. Every formula must have a 'return' statement
6. Strings use single quotes: 'text'
7. Use 'if...then...else...end' for conditionals
8. Variables and formula names are accessed directly by name
9. Comments in code start with // - use sparingly only for clarification
10. No semicolons needed

# OUTPUT FORMAT EXAMPLES:

GOOD (Just code):
return price * quantity

GOOD (With conditional):
if (quantity > 50) then
  return price * 0.9
else
  return price
end

GOOD (With helpful comment):
// Calculate discount based on quantity
if (quantity > 100) then
  return price * 0.8
else
  return price
end

BAD (Has explanation):
Here is the formula:
return price * quantity

BAD (Has markdown):
```
return price * quantity
```

Remember: Output ONLY the code that can be directly executed. No preamble, no explanations, no markdown.";
    }

    private async Task<string> CallOllamaAsync(LLMSettings settings, string prompt)
    {
        using var httpClient = _httpClientFactory.CreateClient();
        
        httpClient.Timeout = TimeSpan.FromMinutes(5); // Ollama can be slow for large models
        
        try
        {
            var request = new
            {
                model = settings.ModelName,
                prompt = prompt,
                stream = false,
                temperature = 0.1,  // Lower temperature for more deterministic output
                system = "You are a precise code generator. Output only executable code."
            };

            var response = await httpClient.PostAsJsonAsync($"{settings.ApiUrl}/api/generate", request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OllamaResponse>();
            return result?.Response ?? "Error: No response from Ollama";
        }
        catch (Exception ex)
        {
            return $"Error calling Ollama: {ex.Message}";
        }
    }

    private async Task<string> CallOpenAIAsync(LLMSettings settings, string systemPrompt, string userPrompt)
    {
        using var httpClient = _httpClientFactory.CreateClient();
        
        try
        {
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {settings.ApiKey}");

            var request = new
            {
                model = settings.ModelName,
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt }
                },
                temperature = 0.1  // Lower temperature for more precise, deterministic output
            };

            var response = await httpClient.PostAsJsonAsync($"{settings.ApiUrl}/v1/chat/completions", request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OpenAIResponse>();
            return result?.Choices?[0]?.Message?.Content ?? "Error: No response from OpenAI";
        }
        catch (Exception ex)
        {
            return $"Error calling OpenAI: {ex.Message}";
        }
    }

    private async Task<string> CallAnthropicAsync(LLMSettings settings, string systemPrompt, string userPrompt)
    {
        using var httpClient = _httpClientFactory.CreateClient();
        
        try
        {
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("x-api-key", settings.ApiKey);
            httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

            var request = new
            {
                model = settings.ModelName,
                max_tokens = 1024,
                system = systemPrompt,
                messages = new[]
                {
                    new { role = "user", content = userPrompt }
                }
            };

            var response = await httpClient.PostAsJsonAsync($"{settings.ApiUrl}/v1/messages", request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<AnthropicResponse>();
            return result?.Content?[0]?.Text ?? "Error: No response from Anthropic";
        }
        catch (Exception ex)
        {
            return $"Error calling Anthropic: {ex.Message}";
        }
    }

    private async Task<string> CallCustomAsync(LLMSettings settings, string prompt)
    {
        using var httpClient = _httpClientFactory.CreateClient();
        
        try
        {
            if (!string.IsNullOrEmpty(settings.ApiKey))
            {
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {settings.ApiKey}");
            }

            var request = new { prompt = prompt };
            var response = await httpClient.PostAsJsonAsync(settings.ApiUrl, request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
        catch (Exception ex)
        {
            return $"Error calling custom API: {ex.Message}";
        }
    }

    // Response models
    private class OllamaResponse
    {
        public string Response { get; set; } = string.Empty;
    }

    private class OpenAIResponse
    {
        public OpenAIChoice[]? Choices { get; set; }
    }
    
    private class OpenAIChoice
    {
        public OpenAIMessage? Message { get; set; }
    }
    
    private class OpenAIMessage
    {
        public string Content { get; set; } = string.Empty;
    }

    private class AnthropicResponse
    {
        public AnthropicContent[]? Content { get; set; }
    }
    
    private class AnthropicContent
    {
        public string Text { get; set; } = string.Empty;
    }
}
