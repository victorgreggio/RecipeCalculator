using System.Text.Json;
using Microsoft.JSInterop;
using RecipeCalculator.UI.Models;

namespace RecipeCalculator.UI.Services;

public class VariableStorageService
{
    private const string StorageKey = "RecipeCalculator.Variables";
    private readonly IJSRuntime _jsRuntime;

    public VariableStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<List<StoredVariable>> GetAllVariablesAsync()
    {
        try
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", StorageKey);
            if (string.IsNullOrEmpty(json))
            {
                return new List<StoredVariable>();
            }
            return JsonSerializer.Deserialize<List<StoredVariable>>(json) ?? new List<StoredVariable>();
        }
        catch
        {
            return new List<StoredVariable>();
        }
    }

    public async Task SaveVariableAsync(StoredVariable variable)
    {
        var variables = await GetAllVariablesAsync();
        
        var existing = variables.FirstOrDefault(v => v.Name == variable.Name);
        if (existing != null)
        {
            existing.Value = variable.Value;
            existing.Type = variable.Type;
        }
        else
        {
            variables.Add(variable);
        }

        await SaveAllVariablesAsync(variables);
    }

    public async Task DeleteVariableAsync(string name)
    {
        var variables = await GetAllVariablesAsync();
        variables.RemoveAll(v => v.Name == name);
        await SaveAllVariablesAsync(variables);
    }

    private async Task SaveAllVariablesAsync(List<StoredVariable> variables)
    {
        var json = JsonSerializer.Serialize(variables);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
    }

    public async Task ClearAllAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", StorageKey);
    }
}
