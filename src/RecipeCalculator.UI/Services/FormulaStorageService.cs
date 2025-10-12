using System.Text.Json;
using Microsoft.JSInterop;
using RecipeCalculator.UI.Models;

namespace RecipeCalculator.UI.Services;

public class FormulaStorageService
{
    private const string StorageKey = "RecipeCalculator.Formulas";
    private readonly IJSRuntime _jsRuntime;

    public FormulaStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<List<StoredFormula>> GetAllFormulasAsync()
    {
        try
        {
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", StorageKey);
            if (string.IsNullOrEmpty(json))
            {
                return new List<StoredFormula>();
            }
            return JsonSerializer.Deserialize<List<StoredFormula>>(json) ?? new List<StoredFormula>();
        }
        catch
        {
            return new List<StoredFormula>();
        }
    }

    public async Task SaveFormulaAsync(StoredFormula formula)
    {
        var formulas = await GetAllFormulasAsync();
        
        var existing = formulas.FirstOrDefault(f => f.Id == formula.Id);
        if (existing != null)
        {
            existing.Name = formula.Name;
            existing.Code = formula.Code;
            existing.ModifiedAt = DateTime.Now;
        }
        else
        {
            formula.CreatedAt = DateTime.Now;
            formula.ModifiedAt = DateTime.Now;
            formulas.Add(formula);
        }

        await SaveAllFormulasAsync(formulas);
    }

    public async Task DeleteFormulaAsync(string id)
    {
        var formulas = await GetAllFormulasAsync();
        formulas.RemoveAll(f => f.Id == id);
        await SaveAllFormulasAsync(formulas);
    }

    private async Task SaveAllFormulasAsync(List<StoredFormula> formulas)
    {
        var json = JsonSerializer.Serialize(formulas);
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
    }

    public async Task ClearAllAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", StorageKey);
    }
}
