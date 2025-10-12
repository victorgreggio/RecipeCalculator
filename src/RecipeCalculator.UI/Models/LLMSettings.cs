namespace RecipeCalculator.UI.Models;

public class LLMSettings
{
    public LLMProvider Provider { get; set; } = LLMProvider.Ollama;
    public string ApiUrl { get; set; } = "http://localhost:11434";
    public string ModelName { get; set; } = "llama2";
    public string ApiKey { get; set; } = string.Empty;
}

public enum LLMProvider
{
    Ollama,
    OpenAI,
    Anthropic,
    Custom
}
