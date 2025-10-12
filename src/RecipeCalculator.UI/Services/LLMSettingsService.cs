using System.Text.Json;
using Microsoft.JSInterop;
using RecipeCalculator.UI.Models;

namespace RecipeCalculator.UI.Services;

public class LLMSettingsService
{
    private const string StorageKey = "RecipeCalculator.LLMSettings";
    private readonly IJSRuntime _jsRuntime;

    public LLMSettingsService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<LLMSettings> GetSettingsAsync()
    {
        try
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", StorageKey);
            if (string.IsNullOrEmpty(json))
            {
                return new LLMSettings();
            }
            return JsonSerializer.Deserialize<LLMSettings>(json) ?? new LLMSettings();
        }
        catch
        {
            return new LLMSettings();
        }
    }

    public async Task SaveSettingsAsync(LLMSettings settings)
    {
        var json = JsonSerializer.Serialize(settings);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
    }

    public async Task ClearAllAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", StorageKey);
    }
}
